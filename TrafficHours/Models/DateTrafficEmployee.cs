using EmployeesTrafficHours.Models.Common;

namespace EmployeesTrafficHours.Models
{
    public class DateTrafficEmployee
    { 
        public DateTime DateTime { get; set; }
        public string Date { get; set; }
        public DaysOfWeek Day { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
