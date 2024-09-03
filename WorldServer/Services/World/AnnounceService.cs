using Common;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    public class AnnounceService : ServiceBase
    {
        public static List<timed_announces> Announces = new List<timed_announces>();

        [LoadingFunction(true)]
        public static void LoadAnnounces()
        {
            Announces = Database.SelectAllObjects<timed_announces>() as List<timed_announces>;
        }

        public static timed_announces GetNextAnnounce(ref int Id, int ZoneId)
        {
            if (Id >= Announces.Count)
                Id = 0;

            for (; Id < Announces.Count; ++Id)
                if (Announces[Id].ZoneId == 0 || Announces[Id].ZoneId == ZoneId)
                    return Announces[Id];

            return null;
        }
    }
}