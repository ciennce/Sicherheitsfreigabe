namespace Sicherheitsfreigabe
{
    class ControlSection
    {
        private Terminal terminal;
        public ControlSection()
        {
            terminal = new();
        }

        public string Challange(int id)
        {
            var input = Console.ReadKey().ToString();

            input = input switch
            {
                "1" => SafeteycardData.GetCard(id)?.ToString(),
                "2" => terminal.CanAccess(id).ToString(),
                "3" => terminal.Open(),
                "4" => terminal.Deny(),
                _ => throw new Exception("Wrong input.")
            };
            return input ?? string.Empty;
           
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
