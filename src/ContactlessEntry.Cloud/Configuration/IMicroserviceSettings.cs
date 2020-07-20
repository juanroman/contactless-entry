namespace ContactlessEntry.Cloud.Configuration
{
    public interface IMicroserviceSettings
    {
        double MaxAllowedTemperature { get; set; }

        string FaceApiUrl { get; set; }

        string FaceSubscriptionKey { get; set; }

        string RecognitionModel { get; set; }
    }
}
