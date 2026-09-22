namespace Sicherheitsfreigabe
{
    class Employee
    {
        private int employeeId { get; set; }
        private string name { get; set; }
        private DateTime hiredDate { get; set; }
        private DateTime birthday { get; set; };
        private bool isOnVacation { get; set; }
        private bool isOnBusinessTrip { get; set; }

        public Employee(string pName, int pEmployeeId, bool pIsOnBusinessTrip, bool pIsOnVacation, DateTime pHiredDate, DateTime pBirthday)
        {
            name = pName;
            employeeId = pEmployeeId;
            isOnBusinessTrip = pIsOnBusinessTrip;
            isOnVacation = pIsOnVacation;
            hiredDate = pHiredDate;
            birthday = pBirthday;
        }

        public static bool isAvailable(Employee employee)
        {
            if (employee.isOnVacation) return false;
            if (employee.isOnBusinessTrip) return true;
            return true;
        }

        public int GetId()
        {
            return employeeId;
        }

        public string GetName()
        {
            return name;
        }

    }
}
