using System.ComponentModel;

namespace EmployeesTrafficHours.Models.Common
{
    public enum DaysOfWeek
    {
        [Description("یکشنبه")]
        Sunday = 0,
    
        [Description("دوشنبه")]
        Monday = 1,
     
        [Description("سه شنبه")]
        Tuesday = 2,
       

        [Description("چهارشنبه")]
        Wednesday = 3,
    

        [Description("پنجشنبه")]
        Thursday = 4,
      

        [Description("جمعه")]
        Friday = 5,
      

        [Description("شنبه")]
        Saturday = 6
    }
}
