using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.Globalization;

namespace ElmaSmartFarm.SharedLibrary
{
    public static class Extensions
    {
        public static string ToPersianDate(this DateTime date)
        {
            PersianCalendar pCal = new();
            return string.Format("{0:0000}/{1:00}/{2:00}", pCal.GetYear(date), pCal.GetMonth(date), pCal.GetDayOfMonth(date));
        }

        public static SensorBaseModel AsBaseModel(this TemperatureSensorModel s)
        {
            return new SensorBaseModel()
            {
                Id = s.Id,
                Name = s.Name,
                LocationId = s.LocationId,
                Section=s.Section,
                IsEnabled = s.IsEnabled,
                Type = s.Type,
                Descriptions = s.Descriptions
            };
        }

        public static SensorBaseModel AsBaseModel(this HumiditySensorModel s)
        {
            return new SensorBaseModel()
            {
                Id = s.Id,
                Name = s.Name,
                LocationId = s.LocationId,
                Section=s.Section,
                IsEnabled = s.IsEnabled,
                Type = s.Type,
                Descriptions = s.Descriptions
            };
        }

        public static SensorBaseModel AsBaseModel(this AmbientLightSensorModel s)
        {
            return new SensorBaseModel()
            {
                Id = s.Id,
                Name = s.Name,
                LocationId = s.LocationId,
                Section=s.Section,
                IsEnabled = s.IsEnabled,
                Type = s.Type,
                Descriptions = s.Descriptions
            };
        }

        public static SensorBaseModel AsBaseModel(this PushButtonSensorModel s)
        {
            return new SensorBaseModel()
            {
                Id = s.Id,
                Name = s.Name,
                LocationId = s.LocationId,
                Section=s.Section,
                IsEnabled = s.IsEnabled,
                Type = s.Type,
                Descriptions = s.Descriptions
            };
        }

        public static SensorBaseModel AsBaseModel(this CommuteSensorModel s)
        {
            return new SensorBaseModel()
            {
                Id = s.Id,
                Name = s.Name,
                LocationId = s.LocationId,
                Section=s.Section,
                IsEnabled = s.IsEnabled,
                Type = s.Type,
                Descriptions = s.Descriptions
            };
        }

        public static SensorBaseModel AsBaseModel(this BinarySensorModel s)
        {
            return new SensorBaseModel()
            {
                Id = s.Id,
                Name = s.Name,
                LocationId = s.LocationId,
                Section=s.Section,
                IsEnabled = s.IsEnabled,
                Type = s.Type,
                Descriptions = s.Descriptions
            };
        }
    }
}