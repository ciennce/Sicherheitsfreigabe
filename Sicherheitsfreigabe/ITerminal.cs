namespace Sicherheitsfreigabe
{
    interface ITerminal
    {
        public bool CanAccess(releaseLevel mode);

        public void Open();

        public void Deny();
    }
}