namespace Sicherheitsfreigabe
{
    class Terminal : ITerminal
    {
        public bool CanAccess(int id)
        {

            switch (SafeteycardData.CardReleaselevel(id))
            {
                case releaseLevel.green: return false;
                case releaseLevel.red: return true;
                case releaseLevel.blue: return false;
            }
            Console.WriteLine("There currently is no safety level to this card.");
            return false;
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
