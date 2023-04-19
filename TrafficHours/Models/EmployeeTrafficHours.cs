using EmployeesTrafficHours.Models.Common;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployeesTrafficHours.Models
{
    public class EmployeeTrafficHours
    {
        [DisplayName("شناسه")]
        public string Id { get; set; }

        [DisplayName("نام فرد")]
        public string Name { get; set; }

        [DisplayName("روز")]
        public DaysOfWeek Day { get; set; }

        [DisplayName("نوع")]
        public Status Status { get; set; }

        [DisplayName("تاریخ")]
        public string Date { get; set; }

        [DisplayName("کارکرد")]
        public TimeSpan EffortTime { get; set; }


        [DisplayName("رکوردها")]
        public List<string> Times { get; set; }

        [DisplayName("رکوردها")]
        public List<DateTime> DateTimes { get; set; }
    }
}
