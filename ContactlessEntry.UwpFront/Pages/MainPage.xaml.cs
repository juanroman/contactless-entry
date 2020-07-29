using ContactlessEntry.UwpFront.Services;
using ContactlessEntry.UwpFront.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace ContactlessEntry.UwpFront.Pages
{
    public sealed partial class MainPage : Page
    {
        private int _busy;
        private FaceTracker _faceTracker;
        private MediaCapture _mediaCapture;
        private RecognitionMode _currentState;
        private ThreadPoolTimer _frameProcessingTimer;
        private VideoEncodingProperties _videoProperties;

        private enum RecognitionMode
        {
            Idle,
            Recognizing
        }

        public MainPage()
        {
            InitializeComponent();

            _currentState = RecognitionMode.Idle;
            DataContext = Locator.Instance.Resolve<MainWindowViewModel>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Application.Current.Suspending += OnApplicationSuspending;
            await ChangeScenarioStateAsync(RecognitionMode.Recognizing);
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            Application.Current.Suspending -= OnApplicationSuspending;
            await ChangeScenarioStateAsync(RecognitionMode.Idle);
        }

        private async void OnApplicationSuspending(object sender, SuspendingEventArgs e)
        {
            if (RecognitionMode.Recognizing == _currentState)
            {
                var deferral = e.SuspendingOperation.GetDeferral();

                try
                {
                    await ChangeScenarioStateAsync(RecognitionMode.Idle);
                }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        private async Task<bool> StartWebcamStreamingAsync()
        {
            _faceTracker = await FaceTracker.CreateAsync();

            try
            {
                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video
                });

                _mediaCapture.Failed += (s, a) => AbandonStreaming();

                var deviceController = _mediaCapture.VideoDeviceController;
                _videoProperties = deviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

                CameraPreview.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();

                var timerInterval = TimeSpan.FromMilliseconds(66); // 66ms, aprox 15 fps
                _frameProcessingTimer = ThreadPoolTimer.CreatePeriodicTimer(ProcessCurrentVideoFrame, timerInterval);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                NavigateToPermissionsPage();
                return false;
            }
            catch (Exception exception)
            {
                await DisplayMessage($"Error al iniciar el stream de la cámara: {exception.Message}");
                return false;
            }
        }

        private async Task ShutdownWebcamAsync()
        {
            if (null != _frameProcessingTimer)
            {
                _frameProcessingTimer.Cancel();
            }

            if (null != _mediaCapture)
            {
                if (CameraStreamState.Streaming == _mediaCapture.CameraStreamState)
                {
                    try
                    {
                        await _mediaCapture.StopPreviewAsync();
                    }
                    catch
                    {
                        ; // Since we're going to destroy the MediaCapture object there's nothing to do here
                    }
                }

                _mediaCapture.Dispose();
            }

            _frameProcessingTimer = null;
            CameraPreview.Source = null;
            _mediaCapture = null;
        }

        private async void ProcessCurrentVideoFrame(ThreadPoolTimer timer)
        {
            if (RecognitionMode.Recognizing != _currentState)
            {
                return;
            }

            // If busy is already 1, then the previous frame is still being processed,
            // in which case we skip the current frame.
            if (0 != Interlocked.CompareExchange(ref _busy, 1, 0))
            {
                return;
            }

            await ProcessCurrentVideoFrameAsync();
            Interlocked.Exchange(ref _busy, 0);
        }

        private async Task ProcessCurrentVideoFrameAsync()
        {
            // Create a VideoFrame object specifying the pixel format we want our capture image to be (NV12 bitmap in this case).
            // GetPreviewFrame will convert the native webcam frame into this format.
            const BitmapPixelFormat InputPixelFormat = BitmapPixelFormat.Nv12;

            IList<DetectedFace> faces;

            using (var previewFrame = new VideoFrame(InputPixelFormat, (int)_videoProperties.Width, (int)_videoProperties.Height))
            {
                try
                {
                    await _mediaCapture.GetPreviewFrameAsync(previewFrame);
                }
                catch (UnauthorizedAccessException)
                {
                    // Lost access to the camera
                    AbandonStreaming();
                    NavigateToPermissionsPage();
                    return;
                }
                catch (Exception exception)
                {
                    await DisplayMessage($"Error en GetPreviewFrameAsync: {exception.Message}");
                    return;
                }

                if (!FaceDetector.IsBitmapPixelFormatSupported(previewFrame.SoftwareBitmap.BitmapPixelFormat))
                {
                    Console.WriteLine($"PixelFormat '{previewFrame.SoftwareBitmap.BitmapPixelFormat}' is not supported by FaceDetector");
                    return;
                }

                try
                {
                    faces = await _faceTracker.ProcessNextFrameAsync(previewFrame);
                }
                catch (Exception exception)
                {
                    await DisplayMessage($"Error al procesar el frame del reconocimiento facial: {exception.Message}");
                    return;
                }

                var previewFrameSize = new Size(previewFrame.SoftwareBitmap.PixelWidth, previewFrame.SoftwareBitmap.PixelHeight);
                var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    SetupVisualization(previewFrameSize, faces);
                });
            }
        }

        private void SetupVisualization(Size framePixelSize, IList<DetectedFace> foundFaces)
        {
            VisualizationCanvas.Children.Clear();

            if (RecognitionMode.Recognizing == _currentState && 0.0 != framePixelSize.Width && 0.0 != framePixelSize.Height)
            {
                double widthScale = VisualizationCanvas.ActualWidth / framePixelSize.Width;
                double heightScale = VisualizationCanvas.ActualHeight / framePixelSize.Height;

                foreach (var face in foundFaces)
                {
                    VisualizationCanvas.Children.Add(new Rectangle
                    {
                        Width = face.FaceBox.Width * widthScale,
                        Height = face.FaceBox.Height * heightScale,
                        Margin = new Thickness(face.FaceBox.X * widthScale, face.FaceBox.Y * heightScale, 0, 0),
                        Style = HighlightedFaceBoxStyle
                    });
                }
            }
        }

        private async Task ChangeScenarioStateAsync(RecognitionMode newState)
        {
            switch (newState)
            {
                case RecognitionMode.Idle:
                    _currentState = newState;
                    await ShutdownWebcamAsync();

                    VisualizationCanvas.Children.Clear();
                    break;

                case RecognitionMode.Recognizing:
                    if (!await StartWebcamStreamingAsync())
                    {
                        await ChangeScenarioStateAsync(RecognitionMode.Idle);
                        break;
                    }

                    VisualizationCanvas.Children.Clear();
                    _currentState = newState;
                    break;
            }
        }

        private void AbandonStreaming()
        {
            // MediaCapture is not Agile and so we cannot invoke its methods on this caller's thread
            // and instead need to schedule the state change on the UI thread.
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await ChangeScenarioStateAsync(RecognitionMode.Idle);
            });
        }

        private void NavigateToPermissionsPage()
        {
            if (Dispatcher.HasThreadAccess)
            {
                Frame.Navigate(typeof(PermissionsPage));
            }
            else
            {
                var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    NavigateToPermissionsPage();
                });
            }
        }

        private async Task DisplayMessage(string message, string title = null)
        {
            if (Dispatcher.HasThreadAccess)
            {
                var messageDialog = new MessageDialog(message, title ?? "Contactless Entry");
                messageDialog.Commands.Add(new UICommand("OK"));

                await messageDialog.ShowAsync();
            }
            else
            {
                var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await DisplayMessage(message, title);
                });
            }
        }
    }
}
