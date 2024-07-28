﻿/*
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using FrameWork;
using System;

namespace Common
{
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "characterinfo_stats", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class CharacterInfo_stats : DataObject
    {
        private byte _CareerLine;
        private byte _Level;
        private byte _StatId;
        private ushort _StatValue;

        [PrimaryKey]
        public byte CareerLine
        {
            get { return _CareerLine; }
            set { _CareerLine = value; Dirty = true; }
        }

        [PrimaryKey]
        public byte Level
        {
            get { return _Level; }
            set { _Level = value; Dirty = true; }
        }

        [PrimaryKey]
        public byte StatId
        {
            get { return _StatId; }
            set { _StatId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort StatValue
        {
            get { return _StatValue; }
            set { _StatValue = value; Dirty = true; }
        }
    }
}