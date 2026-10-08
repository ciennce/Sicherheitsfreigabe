using System.Reflection.Metadata.Ecma335;

namespace Sicherheitsfreigabe
{
    static class SafeteycardData
    {
        private static readonly Dictionary<int, Safetycard> safetycards = [];

        public static void Add(Safetycard card, releaseLevel release)
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

        public static releaseLevel CardReleaselevel(int Id)
        {
            return SafeteycardData.GetCard(Id)?.GetReleaseLevel() ?? releaseLevel.none;
        }
    }
}
