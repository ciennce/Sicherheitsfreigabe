namespace Sicherheitsfreigabe.Access
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
            if (cardId != 0)
            {
                var card = SafeteycardData.GetCard(cardId);
                if (card == null) return terminal.Deny();

                var employee = employeeData.GetEmployee(card.GetOwnerId());
                if (employee == null) return terminal.Deny();

                card.UpdateLastUsed(DateTime.Now);
                return terminal.CanAccess(card, employee) ? terminal.Open() : terminal.Deny();
            }
            else
            {
                return "Invalid Input";
            }
        }

        public string CanAccess()
        {
            Console.Clear();
            string access = Console.ReadLine() ?? string.Empty;

            access = int.TryParse(access, out int cardId) switch
            {
                true => Challenge(cardId),
                false => Challenge(cardId),
            };
            return $"{cardId}: {access}";
        }
    }
}
