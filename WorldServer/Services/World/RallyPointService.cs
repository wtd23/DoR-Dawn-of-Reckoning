using Common;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    public class RallyPointService : ServiceBase
    {
        public static List<rally_points> RallyPoints;

        [LoadingFunction(true)]
        public static void LoadRallyPoints()
        {
            Log.Debug("WorldMgr", "Loading RallyPoints...");

            RallyPoints = Database.SelectAllObjects<rally_points>() as List<rally_points>;

            Log.Success("RallyPoint", "Loaded " + RallyPoints.Count + " RallyPoints");
        }

        public static rally_points GetRallyPoint(uint Id)
        {
            foreach (rally_points point in RallyPoints)
                if (point.Id == Id)
                    return point;
            return null;
        }

        public static rally_points GetRallyPointFromNPC(uint CreatureId)
        {
            foreach (rally_points point in RallyPoints)
                if (point.CreatureId == CreatureId)
                    return point;
            return null;
        }
    }
}