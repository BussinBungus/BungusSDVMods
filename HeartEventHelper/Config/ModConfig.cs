namespace HeartEventHelper.Config
{
    public class ModConfig
    {
        public string beforePositive { get; set; } = "{heart}|";
        public string beforeNeutral { get; set; } = "*|";
        public string beforeNegative { get; set; } = "{3}|";
        public string afterPositive { get; set; } = " ({#_abs})";
        public string afterNeutral { get; set; } = "";
        public string afterNegative { get; set; } = " ({#})";
        public string icon1 { get; set; } = "thumbsup";
        public string icon2 { get; set; } = "thumbsdown";
        public string icon3 { get; set; } = "tear";
    }
}