using Common.Database.World.LiveEvents;
using FrameWork;
using System.Collections.Generic;
using System.Linq;

namespace WorldServer.Services.World
{
    [Service]
    internal class LiveEventService : ServiceBase
    {
        public static List<live_event_infos> LiveEvents = new List<live_event_infos>();

        [LoadingFunction(true)]
        public static void LoadLiveEvents()
        {
            Log.Debug("WorldMgr", "Loading  LiveEvent_Info...");

            var liveEvents = Database.SelectAllObjects<live_event_infos>().ToDictionary(e => e.Entry, e => e);

            var rewards = Database.SelectAllObjects<live_event_reward_infos>();
            var tasks = Database.SelectAllObjects<live_event_task_infos>().ToDictionary(e => e.Entry, e => e);
            var subTasks = Database.SelectAllObjects<live_event_subtask_infos>();

            foreach (var reward in rewards)
            {
                if (liveEvents.ContainsKey(reward.LiveEventId))
                    liveEvents[reward.LiveEventId].Rewards.Add(reward);
            }

            foreach (var task in tasks.Values)
            {
                if (liveEvents.ContainsKey(task.LiveEventId))
                    liveEvents[task.LiveEventId].Tasks.Add(task);
            }

            foreach (var task in subTasks)
            {
                if (tasks.ContainsKey(task.LiveEventTaskId))
                    tasks[task.LiveEventTaskId].Tasks.Add(task);
            }

            LiveEvents = liveEvents.Values.ToList();

            Log.Success("LiveEvent_Info", "Loaded " + LiveEvents.Count + " LiveEvent_Info");
        }
    }
}