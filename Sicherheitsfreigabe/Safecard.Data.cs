namespace Sicherheitsfreigabe
{
    class SafeteycardData
    {
        private readonly Dictionary<int, Safetycard> safetycards = [];

        public void Add(Safetycard card)
        {
            safetycards[card.GetSafetycard()] = card;
        }

        public bool HasCard(int Id)
        {
            return safetycards.ContainsKey(Id);
        }

        public Safetycard? GetCard(int Id)
        {
            safetycards.TryGetValue(Id, out var card);
            return card;
        }
    }
}
