namespace Sicherheitsfreigabe
{
    interface ITerminal
    {
        public bool CanAccess(int id);
        public string Open();
        public string Deny();
    }
}