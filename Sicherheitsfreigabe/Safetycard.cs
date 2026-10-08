namespace Sicherheitsfreigabe 
{
    class Safetycard
    {
        private int cardId { get; set; }

        private int ownerId { get; set; }

        private DateTime lastUsed { get; set; }

        private ReleaseLevel releaseLevel { get; set; }

        private Sector Sector { get; set; }

        public Safetycard(int pCardId, int pOwnerId, DateTime pLastUsed, Sector sector  )
        {
            cardId = pCardId;
            ownerId = pOwnerId;
            lastUsed = pLastUsed;
            Sector = sector;
        }

        public void AddReleaseLevel() // Filter für releaseLevel; if sector == _ -> releaselevel._;
        {
            if (Sector == Sector.HR)
            {
                
            }
        }

        public int GetSafetycard()
        {
            return cardId;
        }

        public ReleaseLevel GetReleaseLevel()
        {
            return releaseLevel;
        }

        public Sector GetSector()
        {
            return Sector;
        }
    }
}
