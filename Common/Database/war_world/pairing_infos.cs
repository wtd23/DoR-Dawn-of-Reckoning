using FrameWork;
using System;

namespace Common.Database.World.Battlefront
{
    [DataTable(PreCache = false, TableName = "pairing_infos", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class pairing_infos : DataObject
    {
        [PrimaryKey]
        public int PairingId { get; set; }

        [DataElement(AllowDbNull = false)]
        public string PairingName { get; set; }

        [DataElement(AllowDbNull = false)]
        public bool Enabled { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte Rotations { get; set; }
    }
}