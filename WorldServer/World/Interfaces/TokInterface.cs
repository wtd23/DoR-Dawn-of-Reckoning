using Common;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldServer.Managers;
using WorldServer.Services.World;
using WorldServer.World.Objects;
using Opcodes = WorldServer.NetWork.Opcodes;

namespace WorldServer.World.Interfaces
{
    public class TokInterface : BaseInterface
    {
        private readonly Dictionary<ushort, characters_toks> _tokUnlocks = new Dictionary<ushort, characters_toks>();
        private readonly Dictionary<ushort, characters_toks_kills> _tokKillCount = new Dictionary<ushort, characters_toks_kills>();

        private bool _loaded;

        public void Load(List<characters_toks> toks, List<characters_toks_kills> toksKills)
        {
            if (toks != null)
            {
                if (_tokUnlocks.Count > 0)
                {
                    Log.Error(_Owner.Name, "ToK system was loaded multiple times!");
                    _tokUnlocks.Clear();
                }

                foreach (characters_toks tok in toks)
                {
                    if (!_tokUnlocks.ContainsKey(tok.TokEntry))
                        _tokUnlocks.Add(tok.TokEntry, tok);
                }
            }

            if (toksKills != null)
            {
                if (_tokKillCount.Count > 0)
                {
                    Log.Error(_Owner.Name, "ToKKill system was loaded multiple times!");
                    toksKills.Clear();
                }

                foreach (characters_toks_kills tok in toksKills)
                {
                    if (!_tokKillCount.ContainsKey(tok.NPCEntry))
                        _tokKillCount.Add(tok.NPCEntry, tok);
                }
            }

            characters_toks_kills kills;
            if (!_tokKillCount.TryGetValue(495, out kills))
            {
                uint totalcount = 0;

                foreach (KeyValuePair<ushort, characters_toks_kills> k in _tokKillCount)
                {
                    totalcount += k.Value.Count;
                }
                kills = new characters_toks_kills
                {
                    NPCEntry = 495,
                    CharacterId = GetPlayer().CharacterId,
                    Count = totalcount
                };
                _tokKillCount.Add(495, kills);
                GetPlayer().Info.TokKills = _tokKillCount.Values.ToList();
                CharMgr.Database.AddObject(kills);
            }

            _loaded = true;

            base.Load();
        }

        public override void Save()
        {
            foreach (KeyValuePair<ushort, characters_toks> Kp in _tokUnlocks)
                CharMgr.Database.SaveObject(Kp.Value);
        }

        public bool HasTok(ushort Entry)
        {
            return _tokUnlocks.ContainsKey(Entry);
        }

        public void AddToks(string Toks)
        {
            if (!_loaded)
            {
                Log.Error("ToKSystem", "Tried to add ToK when system wasn't loaded.\n" + Environment.StackTrace);
                return;
            }

            if (!string.IsNullOrEmpty(Toks))
            {
                ushort tok;

                string[] tmp = Toks.Split(';');
                if (tmp.Length > 0)
                {
                    foreach (string st in tmp)
                    {
                        if (ushort.TryParse(st, out tok))
                            AddTok(tok);
                    }
                }
                else if (ushort.TryParse(Toks, out tok))
                    AddTok(tok);
            }
        }

        public void AddTok(tok_infos Info)
        {
            if (!_loaded)
            {
                Log.Error("ToKSystem", "Tried to add ToK when system wasn't loaded.\n" + Environment.StackTrace);
                return;
            }

            if (Info != null)
                AddTok(Info.Entry);
        }

        // variable itemEquipedToK checks if this ToK was triggered by equiping item. If it is true it was, otherwise it is false
        public void AddTok(ushort Entry, bool itemEquipedToK = false)
        {
            if (HasTok(Entry))
                return;

            if (!_loaded)
            {
                Log.Error("ToKSystem", "Tried to add ToK when system wasn't loaded.\n" + Environment.StackTrace);
                return;
            }

            tok_infos Info = TokService.GetTok(Entry);

            if (Info == null)
                return;

            if (Info.Realm != 0 && Info.Realm != _Owner.GetPlayer().Info.Realm)
                return;

            SendTok(Entry, true);

            characters_toks Tok = new characters_toks
            {
                TokEntry = Entry,
                CharacterId = GetPlayer().CharacterId,
                Count = 1
            };

            _tokUnlocks.Add(Entry, Tok);
            GetPlayer().AddXp(Info.Xp, false, false);

            // This checks if ToK we are adding is a part of larger ToK, for example title
            // "Sovereign Trinket" is part of title "The Sovereign"
            if (itemEquipedToK)
            {
                // Selects item we equiped from DB
                item_infos tokItemUnlock2 = WorldMgr.Database.SelectObject<item_infos>("career=" + GetPlayer().Info.CareerFlags + " AND TokUnlock=" + Entry);

                if (tokItemUnlock2 != null && tokItemUnlock2.TokUnlock != 0 && tokItemUnlock2.TokUnlock2 != 0)
                {
                    // Selects secondary ToK we want to setup if we completed full set
                    IList<item_infos> tokItems = WorldMgr.Database.SelectObjects<item_infos>("career=" + GetPlayer().Info.CareerFlags + " AND TokUnlock2 = " + tokItemUnlock2.TokUnlock2);
                    int count = tokItems.Count();

                    // If there is more than 0 items with complete set unlock we proceed
                    if (count > 0)
                    {
                        foreach (item_infos tokItem in tokItems)
                        {
                            if (HasTok(tokItem.TokUnlock))
                            {
                                count--;
                            }
                        }
                        // If we have all required unlocks count = 0 and we can proceed
                        if (count == 0)
                        {
                            // Tok is send to player
                            SendTok((ushort)tokItemUnlock2.TokUnlock2, true);

                            characters_toks Tok2 = new characters_toks
                            {
                                TokEntry = (ushort)tokItemUnlock2.TokUnlock2,
                                CharacterId = GetPlayer().CharacterId,
                                Count = 1
                            };

                            tok_infos InfoSetTok = TokService.GetTok((ushort)tokItemUnlock2.TokUnlock2);

                            // ToK is added to the book
                            _tokUnlocks.Add((ushort)tokItemUnlock2.TokUnlock2, Tok2);
                            GetPlayer().AddXp(InfoSetTok.Xp, false, false);

                            // Adding reward from final ToK - Title
                            SendTok((ushort)InfoSetTok.Rewards, true);

                            characters_toks Tok2Title = new characters_toks
                            {
                                TokEntry = (ushort)InfoSetTok.Rewards,
                                CharacterId = GetPlayer().CharacterId,
                                Count = 1
                            };

                            tok_infos TokInfoTitle = TokService.GetTok((ushort)InfoSetTok.Rewards);

                            _tokUnlocks.Add((ushort)InfoSetTok.Rewards, Tok2Title);
                            GetPlayer().AddXp(TokInfoTitle.Xp, false, false);

                            //ToKs saved in DB :)
                            CharMgr.Database.AddObject(Tok2);
                            CharMgr.Database.AddObject(Tok2Title);
                        }
                    }
                }
            }

            if (Info.Rewards > 0)
            {
                // this will be used for future additions like the tome tactics and gear to buy
                if (Info.Rewards == 1)
                {
                    GetPlayer().ItmInterface.CreateItem(80001, 1);   // Betial Token
                }
            }

            GetPlayer().Info.Toks = _tokUnlocks.Values.ToList();

            CharMgr.Database.AddObject(Tok);
        }

        public void SendAllToks()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_TOK_ENTRY_UPDATE, 1509);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteUInt16(1500);
            Out.WriteByte(0);
            Out.WriteByte(0);

            byte flags = 0;
            if (Core.Config.DiscoverAll)
            {
                Out.Fill(0xFF, 1500);
            }
            else
            {
                for (ushort i = 0; i < 1500 * 8; i++)
                {
                    if (_tokUnlocks.ContainsKey(i))
                        flags |= (byte)(1 << ((byte)(i % 8)));

                    if (i % 8 == 7)
                    {
                        Out.WriteByte(flags);
                        flags = 0;
                    }
                }
            }
            GetPlayer().SendPacket(Out);
        }

        public void SendTok(ushort Entry, bool Print)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_TOK_ENTRY_UPDATE);
            Out.WriteUInt32(1);
            Out.WriteUInt16(Entry);
            Out.WriteByte(1);
            Out.WriteByte((byte)(Print ? 1 : 0));
            Out.WriteByte(1);
            GetPlayer().SendPacket(Out);
        }

        public void SendBestiary(ref PacketOut Out)
        {
            // total kills  01 EF 00 00 C5 17
            Out.WriteUInt32((UInt32)_tokKillCount.Count);
            foreach (KeyValuePair<ushort, characters_toks_kills> entry in _tokKillCount)
            {
                Out.WriteUInt16(entry.Key);
                Out.WriteUInt32(entry.Value.Count);
            }
        }

        public void SendActionCounterUpdate(ushort Subtype, uint Count)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_ACTION_COUNTER_UPDATE, 11);
            Out.WriteUInt16(Subtype);
            Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteUInt32(Count);
            _Owner.GetPlayer().SendPacket(Out);
        }

        public void AddKill(ushort type)
        {
            tok_bestiary TB = TokService.GetTokBestary(type);
            if (TB == null)
                return;

            characters_toks_kills kills;
            if (_tokKillCount.TryGetValue(TB.Bestary_ID, out kills))
            {
                kills.Count++;
                kills.Dirty = true;
                CharMgr.Database.SaveObject(kills);
            }
            else
            {
                kills = new characters_toks_kills
                {
                    NPCEntry = TB.Bestary_ID,
                    CharacterId = GetPlayer().CharacterId,
                    Count = 1
                };
                _tokKillCount.Add(TB.Bestary_ID, kills);
                GetPlayer().Info.TokKills = _tokKillCount.Values.ToList();
                CharMgr.Database.AddObject(kills);
            }
            uint kill = kills.Count;

            //Log.Info("creature type", "" + type+"  bestid "+ TB.Bestary_ID + " kills "+ kill);

            SendActionCounterUpdate(TB.Bestary_ID, kill);

            // total kill counter

            if (_tokKillCount.TryGetValue(495, out kills))
            {
                kills.Count++;
                kills.Dirty = true;
                CharMgr.Database.SaveObject(kills);
            }
            SendActionCounterUpdate(495, kills.Count);

            string tok;

            if (kill == 100000 && TB.Kill100000 != null)
                tok = TB.Kill100000;
            else if (kill == 10000 && TB.Kill10000 != null)
                tok = TB.Kill10000;
            else if (kill == 1000 && TB.Kill1000 != null)
                tok = TB.Kill1000;
            else if (kill == 100 && TB.Kill100 != null)
                tok = TB.Kill100;
            else if (kill == 25 && TB.Kill25 != null)
                tok = TB.Kill25;
            else if (kill == 1 && TB.Kill1 != null)
                tok = TB.Kill1;
            else
                return;

            string[] tmp = tok.Split(';');
            if (tmp.Length > 0)
            {
                foreach (string st in tmp)
                {
                    AddTok(UInt16.Parse(st));
                }
            }
            else
                AddTok(UInt16.Parse(tok));
        }

        public void CheckTokKills(ushort type, uint count)
        {
            tok_bestiary TB = TokService.GetTokBestary(type);
            if (TB == null)
                return;

            uint kill = count;

            string tok;

            if (kill >= 1 && TB.Kill1 != null)
            {
                tok = TB.Kill1;
                string[] tmp = tok.Split(';');
                if (tmp.Length > 0)
                {
                    foreach (string st in tmp)
                    {
                        FixTokKills(UInt16.Parse(st));
                    }
                }
                else
                    FixTokKills(UInt16.Parse(tok));
            }

            if (kill >= 25 && TB.Kill25 != null)
            {
                tok = TB.Kill25;
                string[] tmp = tok.Split(';');
                if (tmp.Length > 0)
                {
                    foreach (string st in tmp)
                    {
                        FixTokKills(UInt16.Parse(st));
                    }
                }
                else
                    FixTokKills(UInt16.Parse(tok));
            }

            if (kill >= 100 && TB.Kill100 != null)
            {
                tok = TB.Kill100;
                string[] tmp = tok.Split(';');
                if (tmp.Length > 0)
                {
                    foreach (string st in tmp)
                    {
                        FixTokKills(UInt16.Parse(st));
                    }
                }
                else
                    FixTokKills(UInt16.Parse(tok));
            }

            if (kill >= 1000 && TB.Kill1000 != null)
            {
                tok = TB.Kill1000;
                string[] tmp = tok.Split(';');
                if (tmp.Length > 0)
                {
                    foreach (string st in tmp)
                    {
                        FixTokKills(UInt16.Parse(st));
                    }
                }
                else
                    FixTokKills(UInt16.Parse(tok));
            }

            if (kill >= 10000 && TB.Kill10000 != null)
            {
                tok = TB.Kill10000;
                string[] tmp = tok.Split(';');
                if (tmp.Length > 0)
                {
                    foreach (string st in tmp)
                    {
                        FixTokKills(UInt16.Parse(st));
                    }
                }
                else
                    FixTokKills(UInt16.Parse(tok));
            }

            if (kill >= 100000 && TB.Kill100000 != null)
            {
                tok = TB.Kill100000;
                string[] tmp = tok.Split(';');
                if (tmp.Length > 0)
                {
                    foreach (string st in tmp)
                    {
                        FixTokKills(UInt16.Parse(st));
                    }
                }
                else
                    FixTokKills(UInt16.Parse(tok));
            }
        }

        private void FixTokKills(ushort Entry)
        {
            tok_infos Info = TokService.GetTok(Entry);

            if (Info == null)
                return;

            if (Info.Realm != 0 && Info.Realm != _Owner.GetPlayer().Info.Realm)
                return;

            characters_toks Tok = new characters_toks
            {
                TokEntry = Entry,
                CharacterId = GetPlayer().CharacterId,
                Count = 1
            };

            if (Tok == null)
                return;

            if (!HasTok(Entry))
            {
                SendTok(Entry, true);

                GetPlayer().AddXp(Info.Xp, false, false);

                _tokUnlocks.Add(Entry, Tok);

                GetPlayer().Info.Toks = _tokUnlocks.Values.ToList();

                CharMgr.Database.AddObject(Tok);
            }
        }

        public void FixTokItems()
        {
            //IList<Item_Info> tokItems = WorldMgr.Database.SelectObjects<Item_Info>("career=" + GetPlayer().Info.CareerFlags + " AND TokUnlock2 = " + item.Info.TokUnlock2);
            List<item_infos> tokItems = new List<item_infos>();

            for (ushort i = 10; i < 35; i++)
            {
                if (i != 29 && i != 30)
                {
                    Item item = GetPlayer().ItmInterface.GetItemInSlot(i);
                    if (item != null)
                        tokItems.Add(WorldMgr.Database.SelectObject<item_infos>("entry =" + item.Info.Entry));
                }
            }

            foreach (item_infos item in tokItems)
            {
                if (item != null && item.TokUnlock2 != 0 && !HasTok(item.TokUnlock2))
                {
                    IList<item_infos> currentSet = WorldMgr.Database.SelectObjects<item_infos>("career=" + GetPlayer().Info.CareerFlags + " AND TokUnlock2 = " + item.TokUnlock2);

                    int count = currentSet.Count();

                    foreach (item_infos itm in currentSet)
                    {
                        if (count > 0)
                        {
                            foreach (item_infos setItem in currentSet)
                            {
                                if (HasTok(setItem.TokUnlock))
                                    count--;
                            }
                        }

                        if (count == 0 && !HasTok(itm.TokUnlock2))
                        {
                            // Tok is send to player
                            SendTok((ushort)item.TokUnlock2, true);

                            characters_toks Tok2 = new characters_toks
                            {
                                TokEntry = (ushort)item.TokUnlock2,
                                CharacterId = GetPlayer().CharacterId,
                                Count = 1
                            };

                            tok_infos InfoSetTok = TokService.GetTok((ushort)item.TokUnlock2);

                            // ToK is added to the book
                            _tokUnlocks.Add((ushort)item.TokUnlock2, Tok2);
                            GetPlayer().AddXp(InfoSetTok.Xp, false, false);

                            // Adding reward from final ToK - Title
                            SendTok((ushort)InfoSetTok.Rewards, true);

                            characters_toks Tok2Title = new characters_toks
                            {
                                TokEntry = (ushort)InfoSetTok.Rewards,
                                CharacterId = GetPlayer().CharacterId,
                                Count = 1
                            };

                            tok_infos TokInfoTitle = TokService.GetTok((ushort)InfoSetTok.Rewards);

                            _tokUnlocks.Add((ushort)InfoSetTok.Rewards, Tok2Title);
                            GetPlayer().AddXp(TokInfoTitle.Xp, false, false);

                            // ToKs saved in DB :)
                            CharMgr.Database.AddObject(Tok2);
                            CharMgr.Database.AddObject(Tok2Title);
                        }
                    }
                }
            }
        }
    }
}