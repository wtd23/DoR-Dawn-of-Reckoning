using FrameWork;
using System;

namespace Common.Database.World.Battlefront
{
    [DataTable(PreCache = false, TableName = "battlefront_keep_status", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class battlefront_keep_status : DataObject
    {
        [DataElement(AllowDbNull = false), PrimaryKey]
        public int KeepId { get; set; }

        [DataElement(AllowDbNull = false)]
        public int Status { get; set; }
    }
}