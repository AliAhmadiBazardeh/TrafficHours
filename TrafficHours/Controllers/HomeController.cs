using EmployeesTrafficHours.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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

    }
}
