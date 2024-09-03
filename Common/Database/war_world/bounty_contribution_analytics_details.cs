using FrameWork;
using System;

namespace Common.Database.World.Battlefront
{
    [DataTable(PreCache = false, TableName = "bounty_contribution_analytics_details", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class ContributionAnalyticsDetails : DataObject
    {
        [PrimaryKey(AutoIncrement = true)]
        public int AnalyticsDetailId { get; set; }

        [DataElement(AllowDbNull = false)]
        public DateTime Timestamp { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint CharacterId { get; set; }

        [DataElement(AllowDbNull = false)]
        public short ContributionId { get; set; }

        [DataElement(AllowDbNull = false)]
        public int ContributionSum { get; set; }
    }
}