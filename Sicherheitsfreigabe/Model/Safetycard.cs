namespace Sicherheitsfreigabe.Model
{
    class Safetycard
    {
        public int CardId { get; private set;}

        private readonly int _ownerId; 

        private DateTime LastUsed { get; set; }

        private ReleaseLevel _releaseLevel;

        private Sector _Sector;

        public Safetycard(int pCardId, int pOwnerId, DateTime pLastUsed, Sector sector)
        {
            CardId = pCardId;
            _ownerId = pOwnerId;
            LastUsed = pLastUsed;
            _Sector = sector;
            AddReleaseLevel();
        }

        private void AddReleaseLevel()
        {
            _releaseLevel = _Sector switch
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
            return CardId;
        }

        public ReleaseLevel GetReleaseLevel()
        {
            return _releaseLevel;
        }

        public Sector GetSector()
        {
            return _Sector;
        }

        public int GetOwnerId()
        {
            return _ownerId;
        }

        public void UpdateLastUsed(DateTime time)
        {
            LastUsed = time;
        }

        public override string ToString()
            => $"Karte {CardId}, Besitzer {_ownerId}, Stufe {_releaseLevel}, Bereich {_Sector}, zuletzt {LastUsed}";
    }
}
