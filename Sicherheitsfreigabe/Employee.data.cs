namespace Sicherheitsfreigabe
{
    class Employeedata
    {
        private List<Employee> employees;

        public Employeedata()
        {
            employees = [];
        }

        public string GetEmployee(int Id)
        {
            var employee = employees.FirstOrDefault(i => i.GetId() == Id);
            if(employee == null) return string.Empty;
            return employee.GetName();
        }
    }
}
