namespace Sicherheitsfreigabe
{
    class Terminal : ITerminal
    {

        public Terminal()
        {
        }

        public bool CanAccess(releaseLevel mode)
        {
            
            switch (mode)
            {
                case releaseLevel.green:
                    return true;
                case releaseLevel.red:
                    return true;
                case releaseLevel.blue:
                    return true;
            }
            return false;
        }

        public void Deny(Safetycard safetycard)
        {
            
        }

        public void Open()
        {
            if()
        }
    }
}
