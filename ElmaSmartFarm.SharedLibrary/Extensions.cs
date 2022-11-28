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
    }
}