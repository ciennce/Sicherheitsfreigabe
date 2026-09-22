namespace Sicherheitsfreigabe
{
    class Employee
    {
        private int employeeId { get; set; }
        private string name { get; set; }
        private bool isOnVacation { get; set; }
        private bool isOnBusinessTrip { get; set; }

        public Employee(string pName, int pEmployeeId, bool pIsOnBusinessTrip, bool pIsOnVacation)
        {
            name = pName;
            employeeId = pEmployeeId;
            isOnBusinessTrip = pIsOnBusinessTrip;
            isOnVacation = pIsOnVacation;
        }

        public static bool isAvailable(Employee employee)
        {
            if (employee.isOnVacation) return false;
            if (employee.isOnBusinessTrip) return true;
            return true;
        }

        public static int GetId(Employee employee)
        {
            return employee.employeeId;
        }

    }
}
