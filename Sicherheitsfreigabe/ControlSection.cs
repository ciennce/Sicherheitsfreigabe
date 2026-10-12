using Sicherheitsfreigabe.Model;

namespace Sicherheitsfreigabe
{
    class ControlSection
    {
        private Terminal terminal;

        private EmployeeData employeeData;
        public ControlSection()
        {
            terminal = new();
            employeeData = new();
        }

        public string Challange(int id, Safetycard card, Employee employee)
        {
            var input = Console.ReadKey().ToString();

            input = input switch
            {
                "1" => SafeteycardData.GetCard(id)?.ToString(),
                "2" => terminal.CanAccess(card, employee).ToString(),
                "3" => terminal.Open(),
                "4" => terminal.Deny(),
                _ => throw new Exception("Wrong input.")
            };
            return input ?? string.Empty;
           
        }

        public void Control(Safetycard card, Employee employee)
        {

            if (!terminal.CanAccess(card, employee))
            {
                employeeData.GetEmployee(employee.GetId());
                terminal.Deny();
                return;
            }
            terminal.Open();
        }
    }
}
