namespace Sicherheitsfreigabe
{
    interface ITerminal
    {
        public bool CanAccess(int id, SafeteycardData safetycard);

        public void Open();

        public void Deny();
    }
}