using Common;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    public class DyeService : ServiceBase
    {
        public static List<dye_infos> _Dyes = new List<dye_infos>();

        [LoadingFunction(true)]
        public static void LoadDyes()
        {
            _Dyes = new List<dye_infos>();

            Log.Debug("WorldMgr", "Loading Dye_Info...");

            IList<dye_infos> Dyes = Database.SelectAllObjects<dye_infos>();

            int Count = 0;
            foreach (dye_infos Dye in Dyes)
            {
                if (!_Dyes.Contains(Dye))
                    _Dyes.Add(Dye);

                ++Count;
            }
            _Dyes.Sort((a, b) => a.Price.CompareTo(b.Price));

            Log.Success("WorldMgr", "Loaded " + Count + " Dyes");
        }

        /// <summary>
        /// Gets the existing dyes sorted by price.
        /// </summary>
        /// <returns>List of dyes sorted by price, never null</returns>
        public static List<dye_infos> GetDyes()
        {
            return _Dyes;
        }
    }
}