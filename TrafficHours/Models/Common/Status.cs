using System.ComponentModel;

namespace EmployeesTrafficHours.Models.Common
{
    public enum Status
    {
        [Description("عادی")]
        Normal = 0,

        [Description("مرخصی ساعتی")]
        HourlyVacation = 1,

        [Description("تاخیر")]
        Delay = 2,

        [Description("مرخصی روزانه")]
        DaylyVacation = 3,

        [Description("خطا")]
        Error = 4,

    }
}
