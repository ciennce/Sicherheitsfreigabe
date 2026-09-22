namespace Sicherheitsfreigabe
{
    class Safetycard
    {
        private int cardId { get; set; }

        private releaseLevel releaseLevel { get; set; }

        public Safetycard(int pCardId, releaseLevel pReleaseLevel)
        {
            cardId = pCardId;
            releaseLevel = pReleaseLevel;
        }
    }
}
