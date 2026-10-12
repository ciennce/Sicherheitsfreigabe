using Sicherheitsfreigabe.Model;

namespace Sicherheitsfreigabe
{
    interface ITerminal
    {
        public bool CanAccess(Safetycard card, Employee employee);
        public string Open();
        public string Deny();
    }
}