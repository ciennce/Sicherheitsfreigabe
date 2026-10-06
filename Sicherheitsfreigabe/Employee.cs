namespace Sicherheitsfreigabe
{
    abstract class Employee
    {
        private int EmployeeId { get; set; }

        private string Name { get; set; }
        
        private DateTime HiredDate { get; set; }

        private DateTime Birthday { get; set; }

        private bool IsOnVacation { get; set; }

        private bool IsOnBusinessTrip { get; set; }

        public Employee(string pName, int pEmployeeId, bool pIsOnBusinessTrip, bool pIsOnVacation, DateTime pHiredDate, DateTime pBirthday)
        {
            Name = pName;
            EmployeeId = pEmployeeId;
            IsOnBusinessTrip = pIsOnBusinessTrip;
            IsOnVacation = pIsOnVacation;
            HiredDate = pHiredDate;
            Birthday = pBirthday;
        }

        public static bool isAvailable(Employee employee)
        {
            if (employee.IsOnVacation) return false;
            if (employee.IsOnBusinessTrip) return true;
            return true;
        }



        public int GetId()
        {
            return EmployeeId;
        }

        public Employee? GetById(int id)
        {
            if (GetId() == id) return this;
            return null;
        }

    }
}
