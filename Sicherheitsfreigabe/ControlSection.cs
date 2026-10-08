using System.Dynamic;
using System.Numerics;

namespace Sicherheitsfreigabe
{
    class ControlSection
    {
        private Terminal terminal;
        public ControlSection()
        {
            terminal = new();
            dynamic employee = new ExpandoObject(); //
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
