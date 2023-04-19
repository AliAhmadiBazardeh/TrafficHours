using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace EmployeesTrafficHours.Models
{
    public class FileModel
    {
        public string FileName { get; set; }
        public List<EmployeeTrafficHours>? EmployeeTrafficHours { get; set; }
    }
}
