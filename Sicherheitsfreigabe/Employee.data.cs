namespace Sicherheitsfreigabe
{
    class Employeedata
    {
        private List<Employee> employees;

        public Employeedata()
        {
            employees = [];
        }


        public int GetEmployee(int Id)
        {
            var id = employees.FirstOrDefault(i => i.Equals(Id));
            if (id == null) return 0;
            return id; //error
        }


    }
}
