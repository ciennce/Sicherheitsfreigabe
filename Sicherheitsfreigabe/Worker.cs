using System.Numerics;

namespace Sicherheitsfreigabe
{
    class Worker
    {
        private int workerId { get; set; }
        private string? name { get; set; }
        private bool isOnVacation { get; set; }
        private bool isOnBusinessTrip { get; set; }

        public Worker(string pName, int pWorkerId, bool pIsOnBusinessTrip, bool pIsOnVacation)
        {
            name = pName;
            workerId = pWorkerId;
            isOnBusinessTrip = pIsOnBusinessTrip;
            isOnVacation = pIsOnVacation;
        }

        public static bool isAvailable(Worker worker)
        {
            if(worker.isOnVacation || worker.isOnBusinessTrip) return false;
            return true;
        }

    }
}
