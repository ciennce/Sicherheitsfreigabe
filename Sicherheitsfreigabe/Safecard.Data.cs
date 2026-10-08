namespace Sicherheitsfreigabe
{
    static class SafeteycardData
    {
        private static readonly Dictionary<int, Safetycard> safetycards = [];

        public static void Add(Safetycard card, ReleaseLevel release)
        {
            safetycards[card.GetSafetycard()] = card;
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

        public static ReleaseLevel GetCardReleaselevel(int Id)
        {
            return SafeteycardData.GetCard(Id)?
                .GetReleaseLevel() ?? ReleaseLevel.none;
        }

        public static Sector GetCardSector(int Id)
        {
            return SafeteycardData.GetCard(Id)?
                .GetSector() ?? Sector.none;
        }
    }
}
