using FrameWork;
using GameData;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "creature_protos", DatabaseName = "World")]
    [Serializable]
    public class Creature_proto : DataObject
    {
        public ushort[] _Unks = new ushort[7];

        [PrimaryKey]
        public uint Entry { get; set; }

        private string _name;

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Name
        {
            get { return _name; }

            set
            {
                GenderedName = value;

                int caratPos = value.IndexOf("^", StringComparison.Ordinal);

                if (caratPos == -1)
                    _name = value;
                else
                    _name = value.Substring(0, caratPos);
            }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Model1 { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort Model2 { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort MinScale { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort MaxScale { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte MinLevel { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte MaxLevel { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte Faction { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte CreatureType { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte CreatureSubType { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort Ranged { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort IsWandering { get; set; }

        [DataElement(AllowDbNull = false)]
        public bool IsStationary { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte Icone { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte Emote { get; set; }

        private ushort _title;

        [DataElement(AllowDbNull = false)]
        public ushort Title
        {
            get { return _title; }
            set
            {
                _title = value;
                TitleId = (CreatureTitle)value;
            }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Unk
        {
            get { return _Unks[0]; }
            set { if (_Unks == null) _Unks = new ushort[7]; _Unks[0] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Unk1
        {
            get { return _Unks[1]; }
            set { if (_Unks == null) _Unks = new ushort[7]; _Unks[1] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Unk2
        {
            get { return _Unks[2]; }
            set { if (_Unks == null) _Unks = new ushort[7]; _Unks[2] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Unk3
        {
            get { return _Unks[3]; }
            set { if (_Unks == null) _Unks = new ushort[7]; _Unks[3] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Unk4
        {
            get { return _Unks[4]; }
            set { if (_Unks == null) _Unks = new ushort[7]; _Unks[4] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Unk5
        {
            get { return _Unks[5]; }
            set { if (_Unks == null) _Unks = new ushort[7]; _Unks[5] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Unk6
        {
            get { return _Unks[6]; }
            set { if (_Unks == null) _Unks = new ushort[7]; _Unks[6] = value; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Flag { get; set; }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string ScriptName { get; set; }

        [DataElement(AllowDbNull = false)]
        public bool LairBoss { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort VendorID { get; set; }

        public List<quests> StartingQuests;
        public List<quests> FinishingQuests;

        [DataElement(AllowDbNull = false)]
        public string TokUnlock { get; set; }

        public string GenderedName;

        [DataElement]
        public byte[] States { get; set; }

        [DataElement]
        public byte[] FigLeafData { get; set; }

        [DataElement]
        public int BaseRadiusUnits { get; set; }

        [DataElement]
        public byte Career { get; set; }

        private float _powerModifier;

        [DataElement]
        public float PowerModifier
        {
            get { return _powerModifier; }
            set { if (_powerModifier < 0.01f) _powerModifier = 0.01f; _powerModifier = value; }
        }

        private float _woundsModifier;

        [DataElement]
        public float WoundsModifier
        {
            get { return _woundsModifier; }
            set { if (_woundsModifier < 0.01f) _woundsModifier = 0.01f; _woundsModifier = value; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Invulnerable { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort WeaponDPS { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte ImmuneToCC { get; set; }

        public InteractType InteractType = InteractType.INTERACTTYPE_IDLE_CHAT;

        public CreatureTitle TitleId;

        public byte InteractTrainerType;
    }
}