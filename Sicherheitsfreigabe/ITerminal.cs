namespace Sicherheitsfreigabe
{
    interface ITerminal
    {
        public bool CanAccess();

        public void Open();

        public void Deny();
    }
}