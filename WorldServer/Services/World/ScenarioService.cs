using Common;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    internal class ScenarioService : ServiceBase
    {
        public static List<scenario_infos> Scenarios;
        public static List<scenario_infos> ActiveScenarios;

        [LoadingFunction(true)]
        public static void LoadScenarioInfo()
        {
            Log.Debug("WorldMgr", "Loading Scenario_Info...");

            Scenarios = new List<scenario_infos>();
            IList<scenario_infos> infos = Database.SelectAllObjects<scenario_infos>();
            if (infos != null)
                Scenarios.AddRange(infos);

            ActiveScenarios = new List<scenario_infos>();

            foreach (var info in Scenarios)
            {
                IList<scenario_objects> scenObjects =
                    Database.SelectObjects<scenario_objects>("ScenarioId = " + info.ScenarioId);

                foreach (var obj in scenObjects)
                    info.ScenObjects.Add(obj);

                if (info.Enabled > 0)
                    ActiveScenarios.Add(info);
            }

            Log.Success("Scenario_Info", "Loaded " + Scenarios.Count + " Scenario_Info");
        }

        public static scenario_infos GetScenario_Info(ushort ScenarioId)
        {
            foreach (scenario_infos scenario in Scenarios)
                if (scenario != null && scenario.ScenarioId == ScenarioId)
                    return scenario;
            return null;
        }
    }
}