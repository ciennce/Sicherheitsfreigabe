using Sicherheitsfreigabe.Model;

namespace Sicherheitsfreigabe
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now; //test
            DateTime today = DateTime.Today; //Test

            var employees = new EmployeeData();
            var control = new ControlSection(employees);

            employees.Add(new Employee("Anna", 1, false, false, now, today));
            SafeteycardData.Add(new Safetycard(100, 1, DateTime.Now, Sector.HR));
        }
    }
}
