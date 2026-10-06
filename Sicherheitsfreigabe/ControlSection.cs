using System.Numerics;

namespace Sicherheitsfreigabe
{
    class ControlSection
    {
        private Terminal terminal;
        public ControlSection()
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
