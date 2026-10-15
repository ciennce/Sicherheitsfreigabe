using Sicherheitsfreigabe.Model;

namespace Sicherheitsfreigabe
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            DateTime today = DateTime.Today; 

            var employees = new EmployeeData();
            var control = new ControlSection(employees);

            employees.Add(new Employee("Anna", 1, false, false, now, today));
            SafeteycardData.Add(new Safetycard(100, 1, DateTime.Now, Sector.HR));

            employees.Add(new Employee("Max", 2, false, false, now, today));
            SafeteycardData.Add(new Safetycard(101, 2, DateTime.Now, Sector.marketing));

            employees.Add(new Employee("Yunus", 3, true, false, now, today));
            SafeteycardData.Add(new Safetycard(102, 3, DateTime.Now, Sector.logistic));


            foreach (int cardId in new[] { 1, 2, 3, 4, 5, 6, 7, 8 })
            {
                Console.WriteLine($"Karte {cardId}: {control.Challenge(cardId)}");
            }
        }
    }
}
