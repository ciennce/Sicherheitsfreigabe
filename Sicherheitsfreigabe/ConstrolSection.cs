namespace Sicherheitsfreigabe
{
    class ConstrolSection
    {
        private Terminal terminal;
        public ConstrolSection()
        {
            terminal = new();
        }

        public void Challange(int id)
        {
            SafeteycardData.GetCard(id);
        }

        public void Control(int id)
        {
            if (!terminal.CanAccess(id))
            {
                terminal.Deny();
                return;
            }
            terminal.Open();
        }
    }
}
