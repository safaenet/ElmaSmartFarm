namespace ElmaSmartFarm.SharedLibrary.Models.UISettings;
public class AIOSensorColors
{
    public double LowTemperatureThreshold { get; set; }
    public double HighTemperatureThreshold { get; set; }
    public int LowHumidityThreshld { get; set; }
    public int HighHumidityThreshold { get; set; }
    public double HighAmmoniaThreshold { get; set; }
    public double HighCo2Threshold { get; set; }

    public string LowTemperatureColor { get; set; }
    public string NormalTemperatureColor { get; set; }
    public string HighTemperatureColor { get; set; }
    public string LowHumidityColor { get; set; }
    public string NormalHumidityColor { get; set; }
    public string HighHumidityColor { get; set; }
    public string DarkLightColor { get; set; }
    public string BrightLightColor { get; set; }
    public string HighAmmoniaColor { get; set; }
    public string HighCo2Color { get; set; }
}