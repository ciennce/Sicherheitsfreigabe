namespace Sicherheitsfreigabe.Model
{
    class Safetycard
    {
        private int cardId { get; set; }

        private int ownerId { get; set; }

        private DateTime lastUsed { get; set; }

        private ReleaseLevel releaseLevel { get; set; }

        private Sector Sector { get; set; }

        public Safetycard(int pCardId, int pOwnerId, DateTime pLastUsed, Sector sector)
        {
            cardId = pCardId;
            ownerId = pOwnerId;
            lastUsed = pLastUsed;
            Sector = sector;
            AddReleaseLevel();
        }

        private void AddReleaseLevel()
        {
            releaseLevel = Sector switch
            {
                Sector.HR => ReleaseLevel.red,
                Sector.logistic => ReleaseLevel.blue,
                Sector.marketing => ReleaseLevel.green,
                Sector.kitchenPersonal => ReleaseLevel.green,
                _ => ReleaseLevel.none
            };
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

        public int GetOwnerId()
        {
            return ownerId;
        }

        public void UpdateLastUsed(DateTime time)
        {
            lastUsed = time;
        }

        public override string ToString()
            => $"Karte {cardId}, Besitzer {ownerId}, Stufe {releaseLevel}, Bereich {Sector}, zuletzt {lastUsed}";
    }
}
