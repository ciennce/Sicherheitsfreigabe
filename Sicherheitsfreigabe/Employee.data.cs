namespace Sicherheitsfreigabe
{
    class EmployeeData
    {
        private readonly Dictionary<int, Employee> employees = [];

        public void Add(Employee employee)
        {
            employees[employee.GetId()] = employee;
        }

        public Employee? GetEmployee(int Id)
        {
            employees.TryGetValue(Id, out var employee);
            return employee?.GetById(Id);
        }
    }
}
