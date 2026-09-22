namespace Sicherheitsfreigabe
{
    class SafeteycardData
    {
        private List<Safetycard> safetycards;

        public SafeteycardData()
        {
            safetycards = [];
        }

        public bool HasCard(int Id)
        {
            var employee = safetycards.FirstOrDefault(i => i.GetSafetycard() == Id);
            if (employee == null) return false;
            return true;
        }

        public Safetycard GetCard(int Id)
        {
            return safetycards.FirstOrDefault(i => i.GetSafetycard() == Id);
        }

    }
}
