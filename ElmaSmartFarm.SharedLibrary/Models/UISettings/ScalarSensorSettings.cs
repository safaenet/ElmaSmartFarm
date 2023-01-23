namespace ElmaSmartFarm.SharedLibrary.Models.UISettings;
public class ScalarSensorSettings
{
    public double LowTemperatureThreshold { get; set; } = 30;
    public double HighTemperatureThreshold { get; set; } = 35;
    public int LowHumidityThreshold { get; set; } = 10;
    public int HighHumidityThreshold { get; set; } = 70;
    public int LightThreshold { get; set; } = 50;
    public double HighAmmoniaThreshold { get; set; } = 50;
    public double HighCo2Threshold { get; set; } = 50;

    public string LowTemperatureColor { get; set; } = "#ff2c67ab";
    public string HighTemperatureColor { get; set; } = "#ffab2c2c";
    public string LowHumidityColor { get; set; } = "#ff6a731e";
    public string HighHumidityColor { get; set; } = "#ff2c67ab";
    public string DarkLightColor { get; set; } = "#00000000";
    public string BrightLightColor { get; set; } = "#ff378c6b";
    public string HighAmmoniaColor { get; set; } = "#ff046b02";
    public string HighCo2Color { get; set; } = "#ff02546b";
    public string NormalValueColor { get; set; } = "#ff378c6b";
    public string UnwatchColor { get; set; } = "#ff98a39f";
    public string HeaderTextOkColor { get; set; } = "#ff378c6b";
    public string StatusTextOkColor { get; set; } = "#ff378c6b";
    public string StatusTextErrorColor { get; set; } = "#ff991a1a";
    public string StatusIconOkColor { get; set; } = "#ff1a991e";
    public string StatusIconErrorColor { get; set; } = "#ff991a1a";
    public string ItemDisabledColor { get; set; } = "#ffd1cfcf";
    public string BorderOkColor { get; set; } = "#ff696969";
    public string BorderErrorColor { get; set; } = "#ff850000";
}