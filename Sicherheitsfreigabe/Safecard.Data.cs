namespace Sicherheitsfreigabe
{
    class SafeteycardData
    {
        private readonly Dictionary<int, Safetycard> safetycards = [];

        private readonly List<releaseLevel> releaseLevels= [];

        public void Add(Safetycard card, releaseLevel release)
        {
            safetycards[card.GetSafetycard()] = card;
            releaseLevels.Add(release);
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

        public releaseLevel CardReleaselevel(int Id)
        {
            int index = releaseLevels.IndexOf((releaseLevel)Id);
            return releaseLevels[index];
        }
    }
}
