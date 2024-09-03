using FrameWork;
using System;

namespace Common.Database.World.Battlefront
{
    [DataTable(PreCache = false, TableName = "rvr_capital", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class rvr_capital : DataObject
    {
        [PrimaryKey]
        public int Realm { get; set; }

        [DataElement(AllowDbNull = false)]
        public int ZoneId { get; set; }

        [DataElement(AllowDbNull = false)]
        public int RegionId { get; set; }

        [DataElement(AllowDbNull = false)]
        public int LastRealmTaken { get; set; }
    }
}