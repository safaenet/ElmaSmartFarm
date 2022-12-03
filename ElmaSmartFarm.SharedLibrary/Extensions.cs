using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using Serilog;
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

        public static SensorBaseModel AsBaseModel(this SensorModel s)
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

        public static bool RemoveOldestNotSaved<T>(this List<SensorReadModel<T>> list, bool log)
        {
            if (log) Log.Information($"Count of commute value list exceeded it's limit. Removing the oldest one. Count: {list.Count}");
            if (list == null || list.Count == 0) return false;
            for (int i = 0; i < list.Count; i++)
                if (list[i].IsSavedToDb == false)
                {
                    list.Remove(list[i]);
                    return true;
                }
            return false;
        }

        public static bool AddError(this List<SensorErrorModel> Errors, SensorErrorModel newError, SensorErrorType t, int MaxSensorErrorCount)
        {
            if (newError == null) return false;
            var e = Errors.Where(e => e.ErrorType == t && e.DateErased == null);
            if (e != null && e.Any()) return false;
            Errors.Add(newError);
            if (Errors.Count > MaxSensorErrorCount) //Remove oldest record.
            {
                var error = Errors.Where(x => x.DateErased != null)?.MinBy(x => x.DateErased);
                if (error != null) Errors.Remove(error);
                else Log.Warning($"Error count in Sensor Id: {Errors[0].SensorId} has reached limit but not erased! (System Error).");
            }
            return true;
        }

        public static bool EraseError(this List<SensorErrorModel> Errors, SensorErrorType t, DateTime now)
        {
            if (Errors == null) return false;
            var err = Errors.FirstOrDefault(e => e.ErrorType == t && e.DateErased == null);
            if (err == null) return false;
            err.DateErased = now;
            return true;
        }

        public static bool IsPushButtonSensor(this SensorType sensorType)
        {
            return new[] { SensorType.FarmCheckup, SensorType.FarmFeed }.Contains(sensorType);
        }

        public static bool IsBinarySensor(this SensorType sensorType)
        {
            return new[] { SensorType.FarmElectricPower, SensorType.PoultryMainElectricPower, SensorType.PoultryBackupElectricPower }.Contains(sensorType);
        }

        public static bool IsTemperatureSensor(this SensorType sensorType)
        {
            return new[] { SensorType.FarmTemperature, SensorType.OutdoorTemperature }.Contains(sensorType);
        }

        public static bool IsHumiditySensor(this SensorType sensorType)
        {
            return new[] { SensorType.FarmHumidity, SensorType.OutdoorHumidity }.Contains(sensorType);
        }

        public static bool IsScalarSensor(this SensorType sensorType)
        {
            return new[] { SensorType.FarmTemperature, SensorType.FarmHumidity, SensorType.FarmAmbientLight, SensorType.OutdoorTemperature, SensorType.OutdoorHumidity, }.Contains(sensorType);
        }
    }
}