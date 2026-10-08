namespace Sicherheitsfreigabe
{
    class Terminal : ITerminal
    {
        public bool CanAccess(int id)
        {

            switch (SafeteycardData.GetCardReleaselevel(id))
            {
                case ReleaseLevel.green: return false;
                case ReleaseLevel.red: return true;
                case ReleaseLevel.blue: return false;
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
