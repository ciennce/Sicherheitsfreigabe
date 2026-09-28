namespace Sicherheitsfreigabe
{
    static class SafeteycardData
    {
        private static readonly Dictionary<int, Safetycard> safetycards = [];

        private static readonly List<releaseLevel> releaseLevels= [];

        public static void Add(Safetycard card, releaseLevel release)
        {
            safetycards[card.GetSafetycard()] = card;
            releaseLevels.Add(release);
        }

        public static bool HasCard(int Id)
        {
            return safetycards.ContainsKey(Id);
        }

        public static Safetycard? GetCard(int Id)
        {
            safetycards.TryGetValue(Id, out var card);
            return card;
        }

        public static releaseLevel CardReleaselevel(int Id)
        {
            int index = releaseLevels.IndexOf((releaseLevel)Id);
            return releaseLevels[index];
        }
    }
}
