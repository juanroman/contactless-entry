namespace ContactlessEntry.Cloud.Configuration
{
    public class MicroserviceSettings : IMicroserviceSettings
    {
        public double MaxAllowedTemperature { get; set; }

        public string FaceApiUrl { get; set; }

        public string FaceSubscriptionKey { get; set; }

        public string RecognitionModel { get; set; }
    }
}
