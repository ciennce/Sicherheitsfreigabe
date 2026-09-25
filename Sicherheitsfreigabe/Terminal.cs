using System.Runtime.InteropServices;

namespace Sicherheitsfreigabe
{
    class Terminal : ITerminal
    {

        public Terminal(){}

        public bool CanAccess(int id, SafeteycardData safetycard)
        {

            switch (safetycard.CardReleaselevel(id))
            {
                case releaseLevel.green: Open();return true;
                case releaseLevel.red: Open();return true;
                case releaseLevel.blue: Open();return true;
            }
            Deny();
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
