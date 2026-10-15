using Sicherheitsfreigabe.Model;

namespace Sicherheitsfreigabe
{
    class ControlSection
    {
        private Terminal terminal;

        private EmployeeData employeeData;

        public ControlSection(EmployeeData pEmployeeData)
        {
            terminal = new();
            employeeData = pEmployeeData;
        }

        public string Challenge(int cardId)
        {
            var card = SafeteycardData.GetCard(cardId);
            if (card == null) return terminal.Deny();               

            var employee = employeeData.GetEmployee(card.GetOwnerId());
            if (employee == null) return terminal.Deny();

            card.UpdateLastUsed(DateTime.Now);
            return terminal.CanAccess(card, employee) ? terminal.Open() : terminal.Deny();
        }
    }
}
