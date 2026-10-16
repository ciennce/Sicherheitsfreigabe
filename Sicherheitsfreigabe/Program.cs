using Sicherheitsfreigabe.Data;
using Sicherheitsfreigabe.Enums;
using Sicherheitsfreigabe.Model;

namespace Sicherheitsfreigabe
{
    class Program
    {
        static void Main(string[] args)
        {

            //Dictionary<int> cards = new Dictionary<int, int>();


            DateTime now = DateTime.Now;
            DateTime today = DateTime.Today; 

            var employees = new EmployeeData();
            var control = new ControlSection(employees);

            employees.Add(new Employee("Anna", 1, false, false, now, today));
            SafeteycardData.Add(new Safetycard(100, 1, DateTime.Now, Sector.HR));

            employees.Add(new Employee("Max", 2, false, false, now, today));
            SafeteycardData.Add(new Safetycard(101, 2, DateTime.Now, Sector.HR));

            employees.Add(new Employee("Yunus", 3, true, false, now, today));
            SafeteycardData.Add(new Safetycard(102, 3, DateTime.Now, Sector.HR));

            employees.Add(new Employee("Leander", 4, false, true, now, today));
            SafeteycardData.Add(new Safetycard(103, 4, DateTime.Now, Sector.HR));



            employees.Add(new Employee("Jarne", 5, false, true, now, today));
            SafeteycardData.Add(new Safetycard(104, 5, DateTime.Now, Sector.logistic));

            employees.Add(new Employee("Hans", 6, true, false, now, today));
            SafeteycardData.Add(new Safetycard(105, 6, DateTime.Now, Sector.logistic));

            employees.Add(new Employee("Tobias", 7, false, false, now, today));
            SafeteycardData.Add(new Safetycard(106, 7, DateTime.Now, Sector.logistic));




            employees.Add(new Employee("Basti", 8, false, false, now, today));
            SafeteycardData.Add(new Safetycard(107, 8, DateTime.Now, Sector.marketing));

            employees.Add(new Employee("Lennart", 9, false, true, now, today));
            SafeteycardData.Add(new Safetycard(108, 9, DateTime.Now, Sector.kitchenPersonal));

            employees.Add(new Employee("Makar", 10, false, false, now, today));
            SafeteycardData.Add(new Safetycard(109, 10, DateTime.Now, Sector.marketing));




            foreach (int cardId in new[] { 100, 101, 102, 103, 104, 105, 106, 107, 108, 109})
            {
                Console.WriteLine($"Karte {cardId}: {control.Challenge(cardId)}");
            }
        }
    }
}
