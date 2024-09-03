using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = true, TableName = "creature_vendors", DatabaseName = "World")]
    [Serializable]
    public class creature_vendors : DataObject
    {
        public ushort _VendorId;
        public item_infos Info;
        private uint _ItemGuid;
        private uint _ItemId;
        private uint _Price;
        private string _ReqItems;
        private ushort _ReqTokUnlock;
        private byte _ReqGuildlvl;

        [PrimaryKey]
        public uint ItemGuid
        {
            get { return _ItemGuid; }
            set { _ItemGuid = value; Dirty = true; }
        }

        [PrimaryKey]
        public ushort VendorId
        {
            get { return _VendorId; }
            set { _VendorId = value; Dirty = true; }
        }

        [PrimaryKey]
        public uint ItemId
        {
            get { return _ItemId; }
            set { _ItemId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint Price
        {
            get { return _Price; }
            set { _Price = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = true)]
        public ushort ReqTokUnlock
        {
            get { return _ReqTokUnlock; }
            set { _ReqTokUnlock = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = true)]
        public byte ReqGuildlvl
        {
            get { return _ReqGuildlvl; }
            set { _ReqGuildlvl = value; Dirty = true; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string ReqItems
        {
            get { return _ReqItems; }
            set
            {
                _ReqItems = value;
                string[] Infos = _ReqItems.Split(')');
                foreach (string Info in Infos)
                {
                    if (Info.Length <= 0)
                        continue;

                    string[] Items = Info.Split(',');
                    if (Items.Length < 2)
                        continue;

                    Items[0] = Items[0].Remove(0, 1);

                    ushort Count = ushort.Parse(Items[0]);
                    uint Entry = uint.Parse(Items[1]);

                    if (!ItemsReq.ContainsKey(Entry))
                        ItemsReq.Add(Entry, Count);
                }
                Dirty = true;
            }
        }

        public Dictionary<uint, ushort> ItemsReq = new Dictionary<uint, ushort>();
    }
}