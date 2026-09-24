namespace Sicherheitsfreigabe
{
    class Employeedata
    {
        private readonly Dictionary<int, Employee> employees = new();

        public void Add(Employee employee)
        {
            employees[employee.GetId()] = employee;
        }

        public string GetEmployee(int Id)
        {
            employees.TryGetValue(Id, out var employee);
            return employee?.GetName() ?? string.Empty;
        }
    }
}
