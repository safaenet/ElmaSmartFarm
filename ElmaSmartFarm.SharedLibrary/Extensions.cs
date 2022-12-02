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

        public static bool RemoveOldestNotSaved<T>(this List<SensorReadModel<T>> l)
        {
            if (l == null || l.Count == 0) return false;
            for (int i = 0; i < l.Count; i++)
                if (l[i].IsSavedToDb == false)
                {
                    l.Remove(l[i]);
                    return true;
                }
            return false;
        }

        public static bool AddError(this TemperatureSensorModel s, SensorErrorModel newError, SensorErrorType t, int MaxSensorErrorCount)
        {
            if (s == null || newError == null) return false;
            if (s.Errors == null) s.Errors = new();
            var e = s.Errors.Where(e => e.ErrorType == t && e.DateErased == null);
            if (e != null && e.Any()) return false;
            s.Errors.Add(newError);
            if (s.Errors.Count > MaxSensorErrorCount) //Remove oldest record.
            {
                var error = s.Errors.Where(x => x.DateErased != null)?.MinBy(x => x.DateErased);
                if (error != null) s.Errors.Remove(error);
                else Log.Warning($"Error count in Sensor Id: {s.Errors[0].SensorId} has reached limit but not erased! (System Error).");
            }
            return true;
        }

        public static bool EraseError(this TemperatureSensorModel s, SensorErrorType t, DateTime now)
        {
            if (s.Errors == null) return false;
            var err = s.Errors.FirstOrDefault(e => e.ErrorType == t && e.DateErased == null);
            if (err == null) return false;
            err.DateErased = now;
            return true;
        }
    }
}