namespace Sicherheitsfreigabe
{
    class Safetycard
    {
        private int cardId { get; set; }

        private int ownerId { get; set; }

        private DateTime lastUsed { get; set; }

        private releaseLevel releaseLevel { get; set; }

        public Safetycard(int pCardId, releaseLevel pReleaseLevel, int pOwnerId, DateTime pLastUsed)
        {
            cardId = pCardId;
            releaseLevel = pReleaseLevel;
            ownerId = pOwnerId;
            lastUsed = pLastUsed;
        }

        public int GetSafetycard()
        {
            return cardId;
        }

        public releaseLevel GetReleaseLevel()
        {
            return releaseLevel;
        }
    }
}
