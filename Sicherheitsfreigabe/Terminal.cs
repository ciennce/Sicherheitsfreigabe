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

        public void Deny()
        {
            Console.WriteLine("Access denied");
        }

        public void Open()
        {
            Console.WriteLine("Access granted");
        }
    }
}
