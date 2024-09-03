using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "dye_infos", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class dye_infos : DataObject
    {
        [PrimaryKey]
        public ushort Entry { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint Price { get; set; }

        [DataElement(AllowDbNull = false, Varchar = 255)]
        public string Name { get; set; }
    }
}