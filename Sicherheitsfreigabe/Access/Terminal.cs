using Sicherheitsfreigabe.Model;

namespace Sicherheitsfreigabe.Access
{
    class Terminal : ITerminal
    {
        public bool CanAccess(Safetycard card, Employee employee)
            => card.GetReleaseLevel() == ReleaseLevel.red && employee.IsAvailable();

        public string Deny()
        {
            return "Access denied";
        }

        public string Open()
        {
            return "Access granted";
        }
    }
}
