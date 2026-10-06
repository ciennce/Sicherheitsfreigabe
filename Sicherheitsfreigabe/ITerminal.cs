namespace Sicherheitsfreigabe
{
    interface ITerminal
    {
        public bool CanAccess(int id);
        public void Open();
        public void Deny();
    }
}