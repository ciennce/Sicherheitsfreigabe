namespace Sicherheitsfreigabe
{
    interface ITerminal
    {
        public bool CanAccess(releaseLevel mode);

        public void Open(Safetycard safetycard);

        public void Deny(Safetycard safetycard);
    }
}