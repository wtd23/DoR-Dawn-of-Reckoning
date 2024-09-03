using FrameWork;
using System;

namespace Common.Database.World.Battlefront
{
    [DataTable(PreCache = false, TableName = "campaign_objective_buff", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class campaign_objective_buff : DataObject
    {
        [PrimaryKey]
        public int BuffId { get; set; }

        [PrimaryKey]
        public int ObjectiveId { get; set; }

        [DataElement(AllowDbNull = false)]
        public string BuffName { get; set; }
    }
}