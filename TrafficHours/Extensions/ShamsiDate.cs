using System.Globalization;

namespace EmployeesTrafficHours.Extensions
{
    public static class ShamsiDate
    {
        public static string ConvertDateToShamsi(string date)
        {
            PersianCalendar pc = new PersianCalendar();
            //DateTime myDate = DateTime.ParseExact(date+ " 00:00:00,000", "yyyy-MM-dd HH:mm:ss,fff",
            //                           System.Globalization.CultureInfo.InvariantCulture);

            var cultureInfo = new CultureInfo("de-DE");
            string dateString = "12 Juni 2008";
            var myDate = DateTime.Parse(date, cultureInfo);

            string shamsi = pc.GetYear(myDate).ToString() + "/" +
                pc.GetMonth(myDate).ToString() + "/" +
                pc.GetDayOfMonth(myDate).ToString();


            return shamsi;
        }
    }
}
