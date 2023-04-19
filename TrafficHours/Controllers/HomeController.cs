using CSVDownload.Extensions;
using EmployeesTrafficHours.Extensions;
using EmployeesTrafficHours.Models;
using EmployeesTrafficHours.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using Utilities;

namespace TrafficHours.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Calculate(IFormFile file)
        {
            try
            {
                // Convert file to model
                var employeesTraffics = await ConvertFileToViewModel(file);

                // Convert model to report and calculate conditions
                var employeesTrafficHours = await ConvertToReport(employeesTraffics);

                // Create csv file
                var csv = await CreateCSV(employeesTrafficHours);

                return View("~/Views/Home/Calculate.cshtml", new FileModel());

            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;

                return View("~/Views/Home/Index.cshtml", new FileInputModel());
            }

        }

        private async Task<List<EmployeeTraffic>> ConvertFileToViewModel(IFormFile file)
        {
            if (file == null)
                throw new Exception("You must select a file!");


            List<EmployeeTraffic> employeesTraffics = new List<EmployeeTraffic>();

            if (file.Length > 0)
            {

                var filePath = Path.GetTempFileName();
                const Int32 BufferSize = 128;

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                using (var fileStream = System.IO.File.OpenRead(filePath))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    // Read line by line
                    String line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        // Process line
                        char[] delimiterChars = { ' ', ',', '.', '\t' };
                        string[] words = line.Split(delimiterChars);

                        int index = 0;

                        EmployeeTraffic employee = new EmployeeTraffic();

                        string date = "";

                        // Read word of lines
                        foreach (string s in words)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                // Matching columns with model fields
                                switch (index)
                                {
                                    case 0:
                                        // ID validation
                                        if (!int.TryParse(s, out _))
                                            throw new Exception("Incorrect ID format!");

                                        employee.Id = s;

                                        break;
                                    case 1:
                                        // Name validation
                                        if (!s.All(char.IsLetter))
                                            throw new Exception("Incorrect name format!");

                                        employee.Name = s;
                                        break;
                                    case 2:
                                        // Date validation
                                        DateTime.Parse(s);
                                        employee.Date = s;

                                        // Store date for time store to two types of variables
                                        date = s;

                                        break;
                                    case 3:
                                        // Time validation
                                        TimeSpan.Parse(s);

                                        // Store time to two types of variables
                                        employee.Time = s;
                                        employee.DateTime = DateTime.Parse(date + " " + s);

                                        break;

                                    default:
                                        throw new Exception("Incorrect column count!");
                                }

                                index++;
                            }
                        }
                        employeesTraffics.Add(employee);

                    }
                }

            }
            else
            {
                throw new Exception("File length is zero!");
            }
            return employeesTraffics;
        }

        private async Task<List<EmployeeTrafficHours>> ConvertToReport(List<EmployeeTraffic> employeesTraffics)
        {
            List<EmployeeTrafficHours> employeesTrafficHours = new List<EmployeeTrafficHours>();
            List<DateTrafficEmployee> datesTrafficEmployees = new List<DateTrafficEmployee>();

            foreach (var record in employeesTraffics)
            {

                #region Creating working dates list and employees who worked on the date

                var workDate = datesTrafficEmployees.SingleOrDefault(x => x.Date == record.Date);

                if (workDate != null)
                {
                    if (!workDate.Employees.Any(x => x.Id == record.Id))
                    {
                        workDate.Employees.Add(new Employee()
                        {
                            Id = record.Id,
                            Name = record.Name
                        });
                    }

                }
                else
                {

                    datesTrafficEmployees.Add(new DateTrafficEmployee
                    {
                        Date = record.Date,
                        Day = (DaysOfWeek)(int)record.DateTime.DayOfWeek,
                        Employees = new List<Employee>() {
                            new Employee() {
                                Name = record.Name, Id = record.Id
                            }
                        },
                    });
                }


                #endregion


                #region Create final list

                var employeeTraffic = employeesTrafficHours.SingleOrDefault(x => x.Id == record.Id && x.Date == record.Date);
                if (employeeTraffic != null)
                {
                    employeeTraffic.DateTimes.Add(record.DateTime);
                    employeeTraffic.Times.Add(record.Time);
                }
                else
                {
                    employeesTrafficHours.Add(new EmployeeTrafficHours
                    {
                        Id = record.Id,
                        Name = record.Name,
                        Date = record.Date,
                        Day = (DaysOfWeek)(int)record.DateTime.DayOfWeek,
                        Times = new List<string> { record.Time },
                        DateTimes = new List<DateTime> { record.DateTime },
                    });
                }

                #endregion

            }

            // Getting list of all employees
            var employees = employeesTrafficHours.DistinctBy(x => x.Id).Select(x => new Employee()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            #region Finding dayly vacation from working dates list

            foreach (var workDate in datesTrafficEmployees)
            {

                foreach (var employee in employees)
                {
                    if (!workDate.Employees.Any(x => x.Id == employee.Id))
                    {
                        employeesTrafficHours.Add(new EmployeeTrafficHours
                        {
                            Id = employee.Id,
                            Name = employee.Name,
                            Date = workDate.Date,
                            Day = workDate.Day,
                            Status = Status.DaylyVacation,
                            DateTimes = new List<DateTime>(),
                            Times = new List<string>(),
                            EffortTime = new TimeSpan(0, 0, 0)
                        });
                    }
                }
            }

            #endregion

            return employeesTrafficHours;
        }

        private async Task<string> CreateCSV(List<EmployeeTrafficHours> employeesTrafficHours)
        {
            DataTable myDataTable = new DataTable();

            myDataTable.Columns.Add("ردیف", typeof(string));
            myDataTable.Columns.Add("تاریخ", typeof(string));
            myDataTable.Columns.Add("روز", typeof(string));
            myDataTable.Columns.Add("نام فرد", typeof(string));
            myDataTable.Columns.Add("نوع", typeof(string));
            myDataTable.Columns.Add("کارکرد", typeof(string));
            myDataTable.Columns.Add("اولین ورود", typeof(string));
            myDataTable.Columns.Add("آخرین خروج", typeof(string));
            myDataTable.Columns.Add("رکوردها", typeof(string));

            int i = 1;
            foreach (var TrafficHours in employeesTrafficHours.OrderBy(x => x.Date))
            {
                // Space between times
                string times = "";

                foreach (var time in TrafficHours.Times)
                {
                    times += time + " ";
                }

                //Daily vacation are considered in previous functions and do not need to be calculated
                if (TrafficHours.Status != Status.DaylyVacation)
                {
                    //TODO calculate effort and status.
                }

                myDataTable.Rows.Add(i.ToString(), ShamsiDate.ConvertDateToShamsi(TrafficHours.Date), TrafficHours.Day.GetEnumDescription(), TrafficHours.Name,
                    TrafficHours.Status.GetEnumDescription(), TrafficHours.EffortTime, TrafficHours.Times.FirstOrDefault(), TrafficHours.Times.LastOrDefault(), times);
            }

            var workingDates = employeesTrafficHours.DistinctBy(x => x.Date).Select(x => x.Date).ToList();

            string fileName = "EmployeesTraffic " +
                DateTime.Today.ToString("yyyy-MM-dd") + " " +
                DateTime.Now.TimeOfDay.ToString(@"hh\-mm\-ss") + ".csv";

            // requires using System
            string path = @"UploadedFiles\" + fileName;

            // this is an extension method
            var output = myDataTable.ToCsvByteArray();

            // Create the file, or overwrite if the file exists.
            using (FileStream fs = System.IO.File.Create(path))
            {
                fs.Write(output, 0, output.Length);
            }

            return fileName;
        }


    }
}
