namespace Sicherheitsfreigabe
{
    class Terminal : ITerminal
    {
        public bool CanAccess(int id)
        {
            bool result = SafeteycardData.GetCardReleaselevel(id) switch
            {
                ReleaseLevel.red => true,
                _ => false
            };
            return result;
        }

        public string Deny()
        {
            return "Access denied";
        }

        public string Open()
        {
            return "Access granted";
        }
    }
}
