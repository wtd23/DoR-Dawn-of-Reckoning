using Common;
using Common.Database.World.Characters;
using FrameWork;
using GameData;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SystemData;
using WorldServer.NetWork;
using WorldServer.Services.World;
using WorldServer.World.Auction;
using WorldServer.World.Guild;
using WorldServer.World.Interfaces;
using WorldServer.World.Objects;
using Item = WorldServer.World.Objects.Item;

namespace WorldServer.Managers
{
    public class AccountChars
    {
        public int AccountId;
        public GameData.SetRealms Realm = GameData.SetRealms.REALMS_REALM_NEUTRAL;

        public bool Loaded { get; set; }

        public AccountChars(int accountId)
        {
            AccountId = accountId;
        }

        public characters[] Chars = new characters[CharMgr.MaxSlot];

        public byte GenerateFreeSlot()
        {
            for (byte i = 0; i < Chars.Length; i++)
                if (Chars[i] == null)
                    return i;

            return CharMgr.MaxSlot;
        }

        public bool AddChar(characters Char)
        {
            if (Char == null)
                return false;

            Realm = (GameData.SetRealms)Char.Realm;

            Chars[Char.SlotId] = Char;

            return true;
        }

        public uint RemoveCharacter(byte slot)
        {
            uint characterId = 0;
            if (Chars[slot] != null)
                characterId = Chars[slot].CharacterId;

            Chars[slot] = null;
            Realm = GameData.SetRealms.REALMS_REALM_NEUTRAL;

            foreach (characters Char in Chars)
                if (Char != null)
                {
                    Realm = (GameData.SetRealms)Char.Realm;
                    break;
                }

            return characterId;
        }

        public characters GetCharacterBySlot(byte slot)
        {
            if (slot > Chars.Length)
                return null;

            return Chars[slot];
        }
    };

    [Service(
        typeof(ZoneService),
        typeof(PQuestService),
        typeof(ChapterService),
        typeof(ItemService))]
    public static class CharMgr
    {
        public static IObjectDatabase Database = null;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region CharacterInfo

        public static Dictionary<byte, character_info> CharacterInfos = new Dictionary<byte, character_info>();
        public static ConcurrentDictionary<byte, List<character_info_items>> CharacterStartingItems = new ConcurrentDictionary<byte, List<character_info_items>>();
        public static Dictionary<byte, List<character_info_renown>> RenownAbilityInfo = new Dictionary<byte, List<character_info_renown>>();
        public static Dictionary<byte, List<character_info_stats>> CharacterBaseStats = new Dictionary<byte, List<character_info_stats>>();
        public static Dictionary<byte, List<pet_stat_override>> PetOverrideStats = new Dictionary<byte, List<pet_stat_override>>();
        public static Dictionary<byte, List<pet_mastery_modifiers>> PetMasteryMods = new Dictionary<byte, List<pet_mastery_modifiers>>();
        public static List<random_names> RandomNameList = new List<random_names>();

        [LoadingFunction(true)]
        public static void LoadCharacterInfos()
        {
            IList<character_info> chars = WorldMgr.Database.SelectAllObjects<character_info>();
            foreach (character_info info in chars)
                if (!CharacterInfos.ContainsKey(info.Career))
                    CharacterInfos.Add(info.Career, info);

            RandomNameList = WorldMgr.Database.SelectAllObjects<random_names>() as List<random_names>;

            Log.Success("CharacterMgr", "Loaded " + chars.Count + " CharacterInfo");
        }

        [LoadingFunction(true)]
        public static void LoadDefaultCharacterItems()
        {
            IList<character_info_items> referenceListStartingItems = WorldMgr.Database.SelectAllObjects<character_info_items>();

            if (referenceListStartingItems != null)
            {
                foreach (character_info_items item in referenceListStartingItems)
                {
                    CharacterStartingItems.AddOrUpdate(
                        item.CareerLine, new List<character_info_items> { item },
                        (k, v) =>
                        {
                            v.Add(item);
                            return v;
                        });
                }
            }

            //if (!CharacterStartingItems.ContainsKey(info.CareerLine))
            //{
            //    List<CharacterInfo_item> items = new List<CharacterInfo_item>(1);
            //    items.Add(info);
            //    CharacterStartingItems.Add(info.CareerLine, items);
            //}
            //else CharacterStartingItems[info.CareerLine].Add(info);

            Log.Success("CharacterMgr", "Loaded " + CharacterStartingItems.Count + " CharacterInfo_Item");
        }

        [LoadingFunction(true)]
        public static void LoadCharacterRenownInfo()
        {
            IList<character_info_renown> characterRenownInfo = WorldMgr.Database.SelectAllObjects<character_info_renown>().OrderBy(x => x.ID).ToList();

            foreach (character_info_renown renInfo in characterRenownInfo)
                if (!RenownAbilityInfo.ContainsKey(renInfo.Tree))
                {
                    List<character_info_renown> items = new List<character_info_renown>(1) { renInfo };
                    RenownAbilityInfo.Add(renInfo.Tree, items);
                }
                else RenownAbilityInfo[renInfo.Tree].Add(renInfo);

            Log.Success("CharacterMgr", "Loaded " + RenownAbilityInfo.Count + " CharacterInfo_renown");
        }

        [LoadingFunction(true)]
        public static void LoadCharacterBaseStats()
        {
            IList<character_info_stats> characterStatInfo = WorldMgr.Database.SelectAllObjects<character_info_stats>();
            foreach (character_info_stats statInfo in characterStatInfo)
            {
                if (!CharacterBaseStats.ContainsKey(statInfo.CareerLine))
                {
                    List<character_info_stats> stats = new List<character_info_stats>(1) { statInfo };
                    CharacterBaseStats.Add(statInfo.CareerLine, stats);
                }
                else CharacterBaseStats[statInfo.CareerLine].Add(statInfo);
            }

            Log.Success("CharacterMgr", "Loaded " + characterStatInfo.Count + " CharacterInfo_Stats");
        }

        public static character_info GetCharacterInfo(byte career)
        {
            lock (CharacterInfos)
                if (CharacterInfos.ContainsKey(career))
                    return CharacterInfos[career];

            return null;
        }

        public static Dictionary<ushort, List<character_info_stats>> CareerLevelStats = new Dictionary<ushort, List<character_info_stats>>();

        public static List<character_info_stats> GetCharacterInfoStats(byte careerLine, byte level)
        {
            List<character_info_stats> stats = new List<character_info_stats>();
            if (!CareerLevelStats.TryGetValue((ushort)((careerLine << 8) + level), out stats))
            {
                stats = new List<character_info_stats>();

                List<character_info_stats> infoStats;
                if (CharacterBaseStats.TryGetValue(careerLine, out infoStats))
                {
                    foreach (character_info_stats stat in infoStats)
                        if (stat.CareerLine == careerLine && stat.Level == level)
                            stats.Add(stat);

                    stats = stats.OrderBy(x => x.StatId).ToList();
                }

                CareerLevelStats[(ushort)((careerLine << 8) + level)] = stats;
            }
            return stats;
        }

        public static void ReloadPetModifiers()
        {
            PetOverrideStats.Clear();
            PetMasteryMods.Clear();
            CharacterBaseStats.Clear();

            LoadPetStatOverrides();
            LoadPetMasteryMods();
            LoadCharacterBaseStats();
        }

        [LoadingFunction(true)]
        public static void LoadPetStatOverrides()
        {
            IList<pet_stat_override> overrides = WorldMgr.Database.SelectAllObjects<pet_stat_override>();
            foreach (pet_stat_override ovrInfo in overrides)
            {
                if (!PetOverrideStats.ContainsKey(ovrInfo.CareerLine))
                {
                    List<pet_stat_override> ovr = new List<pet_stat_override>(1) { ovrInfo };
                    PetOverrideStats.Add(ovrInfo.CareerLine, ovr);
                }
                else PetOverrideStats[ovrInfo.CareerLine].Add(ovrInfo);
                Log.Success("CharacterMgr", "Loaded " + overrides.Count + " PetStatOverrides");
            }
        }

        public static Dictionary<ushort, List<pet_stat_override>> PetOverriddenStats = new Dictionary<ushort, List<pet_stat_override>>();

        public static List<pet_stat_override> GetPetStatOverride(byte careerLine)
        {
            List<pet_stat_override> overrides = new List<pet_stat_override>();

            // if (!PetOverriddenStats.TryGetValue((ushort)(careerLine << 8), out overrides))
            // {
            overrides = new List<pet_stat_override>();

            List<pet_stat_override> infoOverrides;
            if (PetOverrideStats.TryGetValue(careerLine, out infoOverrides))
            {
                foreach (pet_stat_override ovr in infoOverrides)
                    if (ovr.CareerLine == careerLine && ovr.Active == true)
                        overrides.Add(ovr);

                overrides = overrides.OrderBy(x => x.PrimaryValue).ToList();
            }

            PetOverriddenStats[(ushort)(careerLine << 8)] = overrides;
            // }

            return overrides;
        }

        [LoadingFunction(true)]
        public static void LoadPetMasteryMods()
        {
            IList<pet_mastery_modifiers> modifiers = WorldMgr.Database.SelectAllObjects<pet_mastery_modifiers>();
            foreach (pet_mastery_modifiers modInfo in modifiers)
            {
                if (!PetMasteryMods.ContainsKey(modInfo.CareerLine))
                {
                    List<pet_mastery_modifiers> mod = new List<pet_mastery_modifiers>(1) { modInfo };
                    PetMasteryMods.Add(modInfo.CareerLine, mod);
                }
                else PetMasteryMods[modInfo.CareerLine].Add(modInfo);
            }
            Log.Success("CharacterMgr", "Loaded " + modifiers.Count + " PetMosteryModifiers");
        }

        public static Dictionary<ushort, List<pet_mastery_modifiers>> PetModifiedMastery = new Dictionary<ushort, List<pet_mastery_modifiers>>();

        public static List<pet_mastery_modifiers> GetPetMasteryModifiers(byte careerLine)
        {
            List<pet_mastery_modifiers> modifiers = new List<pet_mastery_modifiers>();

            modifiers = new List<pet_mastery_modifiers>();

            List<pet_mastery_modifiers> infoModifiers;
            if (PetMasteryMods.TryGetValue(careerLine, out infoModifiers))
            {
                foreach (pet_mastery_modifiers mod in infoModifiers)
                    if (mod.CareerLine == careerLine && mod.Active == true)
                        modifiers.Add(mod);

                modifiers = modifiers.OrderBy(x => x.PrimaryValue).ToList();
            }
            PetModifiedMastery[(ushort)(careerLine << 8)] = modifiers;

            return modifiers;
        }

        public static List<character_info_items> GetCharacterInfoItem(byte careerLine)
        {
            List<character_info_items> items;
            if (!CharacterStartingItems.TryGetValue(careerLine, out items))
            {
                items = new List<character_info_items>();
                CharacterStartingItems.TryAdd(careerLine, items);
            }
            return items;
        }

        public static List<random_names> GetRandomNames()
        {
            return RandomNameList;
        }

        #endregion CharacterInfo

        #region Characters

        // Only 20 will work
        public static byte MaxSlot = 20;

        private static long _maxCharGuid = 1;
        public static Dictionary<uint, characters> Chars = new Dictionary<uint, characters>();
        public static Dictionary<string, uint> CharIdLookup = new Dictionary<string, uint>();
        public static Dictionary<int, AccountChars> AcctChars = new Dictionary<int, AccountChars>();

        public static long RecentHistoryTime = (TCPManager.GetTimeStamp() - ((60 * 60 * 24 * 7 * 4 * 2)));

        [LoadingFunction(true)]
        public static void LoadCharacters()
        {
            if (Core.Config.PreloadAllCharacters)
            {
                List<characters> chars = (List<characters>)Database.SelectAllObjects<characters>();
                Dictionary<uint, characters_value> charValues = Database.SelectAllObjects<characters_value>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.FirstOrDefault());
                Dictionary<uint, characters_client_data> charClientData = Database.SelectAllObjects<characters_client_data>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.FirstOrDefault());
                Dictionary<uint, List<characters_socials>> charSocials = Database.SelectAllObjects<characters_socials>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_toks>> charToks = Database.SelectAllObjects<characters_toks>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_toks_kills>> charToksKills = Database.SelectAllObjects<characters_toks_kills>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_quests>> charQuests = Database.SelectAllObjects<characters_quests>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_influences>> charInfluences = Database.SelectAllObjects<characters_influences>().GroupBy(v => v.CharacterId).ToDictionary(g => (uint)g.Key, g => g.ToList());
                Dictionary<uint, List<characters_bag_pools>> charBagPools = Database.SelectAllObjects<characters_bag_pools>().GroupBy(v => v.CharacterId).ToDictionary(g => (uint)g.Key, g => g.ToList());
                Dictionary<uint, List<characters_mails>> charMail = Database.SelectAllObjects<characters_mails>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_saved_buffs>> charBuffs = Database.SelectAllObjects<characters_saved_buffs>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_honor_reward_cooldown>> charHonorCooldowns = Database.SelectAllObjects<characters_honor_reward_cooldown>().GroupBy(v => v.CharacterId).ToDictionary(g => (uint)g.Key, g => g.ToList());

                int count = 0;
                foreach (characters Char in chars)
                {
                    if (charValues.ContainsKey(Char.CharacterId)) Char.Value = charValues[Char.CharacterId];
                    if (charClientData.ContainsKey(Char.CharacterId)) Char.ClientData = charClientData[Char.CharacterId];
                    if (charSocials.ContainsKey(Char.CharacterId)) Char.Socials = charSocials[Char.CharacterId];
                    if (charToks.ContainsKey(Char.CharacterId)) Char.Toks = charToks[Char.CharacterId];
                    if (charToksKills.ContainsKey(Char.CharacterId)) Char.TokKills = charToksKills[Char.CharacterId];
                    if (charQuests.ContainsKey(Char.CharacterId)) Char.Quests = charQuests[Char.CharacterId];
                    if (charInfluences.ContainsKey(Char.CharacterId)) Char.Influences = charInfluences[Char.CharacterId];
                    if (charBagPools.ContainsKey(Char.CharacterId)) Char.Bag_Pools = charBagPools[Char.CharacterId];
                    if (charMail.ContainsKey(Char.CharacterId)) Char.Mails = charMail[Char.CharacterId];
                    if (charBuffs.ContainsKey(Char.CharacterId)) Char.Buffs = charBuffs[Char.CharacterId];
                    if (charHonorCooldowns.ContainsKey(Char.CharacterId)) Char.HonorCooldowns = charHonorCooldowns[Char.CharacterId];

                    // Mail list must never be null
                    if (Char.Mails == null)
                        Char.Mails = new List<characters_mails>();
                    if (Char.HonorCooldowns == null)
                        Char.HonorCooldowns = new List<characters_honor_reward_cooldown>();
                    AddChar(Char);
                    ++count;
                }

                Log.Success("LoadCharacters", count + " characters loaded.");
            }
            else
            {
                string whereString = $"CharacterId IN (SELECT CharacterId FROM `{Database.GetSchemaName()}`.characters t1 WHERE t1.AccountId IN (SELECT AccountId FROM `{Core.AcctMgr.GetAccountSchemaName()}`.accounts t2 WHERE t2.LastLogged >= {RecentHistoryTime}))";
                /*_maxCharGuid = Database.GetMaxColValue<characters>("CharacterId");

                Log.Success("LoadCharacters", _maxCharGuid + " is the max char GUID.");

                List<characters> auctionSellers = (List<characters>)Database.SelectObjects<characters>("CharacterId IN (SELECT SellerId FROM war_characters.auctions)");

                foreach (characters seller in auctionSellers)
                {
                    if (!Chars.ContainsKey(seller.CharacterId))
                        AddChar(seller);
                }*/

                // Full load
                List<characters> chars = (List<characters>)Database.SelectAllObjects<characters>();
                Dictionary<uint, List<characters_socials>> charSocials = Database.SelectAllObjects<characters_socials>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());

                // Partial load
                Dictionary<uint, characters_value> charValues = Database.SelectObjects<characters_value>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.FirstOrDefault());
                Dictionary<uint, characters_client_data> charClientData = Database.SelectAllObjects<characters_client_data>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.FirstOrDefault());
                Dictionary<uint, List<characters_toks>> charToks = Database.SelectObjects<characters_toks>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_toks_kills>> charToksKills = Database.SelectObjects<characters_toks_kills>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_quests>> charQuests = Database.SelectObjects<characters_quests>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_influences>> charInfluences = Database.SelectObjects<characters_influences>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => (uint)g.Key, g => g.ToList());
                Dictionary<uint, List<characters_bag_pools>> charBagPools = Database.SelectObjects<characters_bag_pools>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => (uint)g.Key, g => g.ToList());
                Dictionary<uint, List<characters_mails>> charMail = Database.SelectObjects<characters_mails>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_saved_buffs>> charBuffs = Database.SelectObjects<characters_saved_buffs>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
                Dictionary<uint, List<characters_honor_reward_cooldown>> charHonorCooldowns = Database.SelectAllObjects<characters_honor_reward_cooldown>().GroupBy(v => v.CharacterId).ToDictionary(g => (uint)g.Key, g => g.ToList());

                int count = 0;
                foreach (characters Char in chars)
                {
                    if (charValues.ContainsKey(Char.CharacterId)) Char.Value = charValues[Char.CharacterId];
                    if (charClientData.ContainsKey(Char.CharacterId)) Char.ClientData = charClientData[Char.CharacterId];
                    if (charSocials.ContainsKey(Char.CharacterId)) Char.Socials = charSocials[Char.CharacterId];
                    if (charToks.ContainsKey(Char.CharacterId)) Char.Toks = charToks[Char.CharacterId];
                    if (charToksKills.ContainsKey(Char.CharacterId)) Char.TokKills = charToksKills[Char.CharacterId];
                    if (charQuests.ContainsKey(Char.CharacterId)) Char.Quests = charQuests[Char.CharacterId];
                    if (charInfluences.ContainsKey(Char.CharacterId)) Char.Influences = charInfluences[Char.CharacterId];
                    if (charBagPools.ContainsKey(Char.CharacterId)) Char.Bag_Pools = charBagPools[Char.CharacterId];
                    if (charMail.ContainsKey(Char.CharacterId)) Char.Mails = charMail[Char.CharacterId];
                    if (charBuffs.ContainsKey(Char.CharacterId)) Char.Buffs = charBuffs[Char.CharacterId];
                    if (charHonorCooldowns.ContainsKey(Char.CharacterId)) Char.HonorCooldowns = charHonorCooldowns[Char.CharacterId];

                    // Mail list must never be null
                    if (Char.Mails == null)
                        Char.Mails = new List<characters_mails>();
                    if (Char.HonorCooldowns == null)
                        Char.HonorCooldowns = new List<characters_honor_reward_cooldown>();

                    AddChar(Char);
                    if (Char.Value != null)
                        GetAccountChar(Char.AccountId).Loaded = true;

                    ++count;
                }

                Log.Success("LoadCharacters", $"{count} characters loaded, of which {charValues.Count} fully precached.");
            }

            AuctionHouse.LoadAuctions();
            LoadAlliances();
        }

        public static characters LoadCharacterInfo(string name, bool fullLoad)
        {
            characters Char = Database.SelectObject<characters>("Name='" + Database.Escape(name) + "'");
            if (Char != null)
            {
                if (Char.Value == null)
                    Char.Value = Database.SelectObject<characters_value>("CharacterId=" + Char.CharacterId);
                if (Char.ClientData == null)
                    Char.ClientData = Database.SelectObject<characters_client_data>("CharacterId=" + Char.CharacterId);
                if (fullLoad)
                    LoadAdditionalCharacterInfo(Char);

                AddChar(Char);
                Log.Info("LoadCharacter (Name)", Char.Name);
            }

            return Char;
        }

        public static characters LoadCharacterInfo(uint id, bool fullLoad)
        {
            characters Char = Database.SelectObject<characters>("CharacterId='" + id + "'");
            if (Char != null)
            {
                if (Char.Value == null)
                    Char.Value = Database.SelectObject<characters_value>("CharacterId=" + Char.CharacterId);
                if (Char.ClientData == null)
                    Char.ClientData = Database.SelectObject<characters_client_data>("CharacterId=" + Char.CharacterId);

                if (fullLoad)
                    LoadAdditionalCharacterInfo(Char);

                AddChar(Char);
                Log.Info("LoadCharacter (Name)", Char.Name);
            }

            return Char;
        }

        private static void LoadAdditionalCharacterInfo(characters Char)
        {
            Char.Socials = (List<characters_socials>)Database.SelectObjects<characters_socials>("CharacterId=" + Char.CharacterId);
            Char.Toks = (List<characters_toks>)Database.SelectObjects<characters_toks>("CharacterId=" + Char.CharacterId);
            Char.TokKills = (List<characters_toks_kills>)Database.SelectObjects<characters_toks_kills>("CharacterId=" + Char.CharacterId);
            Char.Quests = (List<characters_quests>)Database.SelectObjects<characters_quests>("CharacterId=" + Char.CharacterId);
            Char.Influences = (List<characters_influences>)Database.SelectObjects<characters_influences>("CharacterId=" + Char.CharacterId);
            Char.Bag_Pools = (List<characters_bag_pools>)Database.SelectObjects<characters_bag_pools>("CharacterId=" + Char.CharacterId);
            Char.Buffs = (List<characters_saved_buffs>)Database.SelectObjects<characters_saved_buffs>("CharacterId=" + Char.CharacterId);
            Char.HonorCooldowns = (List<characters_honor_reward_cooldown>)Database.SelectObjects<characters_honor_reward_cooldown>("CharacterId=" + Char.CharacterId);

            if (Char.Mails == null)
                Char.Mails = (List<characters_mails>)Database.SelectObjects<characters_mails>("CharacterId=" + Char.CharacterId);
        }

        public static uint GenerateMaxCharId()
        {
            return (uint)Interlocked.Increment(ref _maxCharGuid);
        }

        public static bool CreateChar(characters Char)
        {
            AccountChars chars = GetAccountChar(Char.AccountId);
            Char.SlotId = chars.GenerateFreeSlot();

            lock (Chars)
            {
                uint charId = GenerateMaxCharId();

                while (Chars.ContainsKey(charId))
                    charId = GenerateMaxCharId();

                if (charId >= uint.MaxValue || charId <= 0)
                {
                    Log.Error("CreateChar", "Failed: maximum number of characters reached.");
                    return false;
                }

                Char.CharacterId = charId;
                Chars[Char.CharacterId] = Char;
                chars.AddChar(Char);
            }

            lock (CharIdLookup)
                CharIdLookup.Add(Char.Name, Char.CharacterId);

            Database.AddObject(Char);

            return true;
        }

        public static void AddChar(characters Char)
        {
            lock (Chars)
            {
                if (Chars.ContainsKey(Char.CharacterId))
                    return;

                Chars.Add(Char.CharacterId, Char);

                GetAccountChar(Char.AccountId).AddChar(Char);

                if (Char.CharacterId > _maxCharGuid)
                    _maxCharGuid = Char.CharacterId;
            }

            lock (CharIdLookup)
                CharIdLookup.Add(Char.Name, Char.CharacterId);
        }

        public static uint GetCharacterId(string name)
        {
            lock (CharIdLookup)
                return CharIdLookup.ContainsKey(name) ? CharIdLookup[name] : 0;
        }

        public static void UpdateCharacterName(characters chara, string newName)
        {
            lock (CharIdLookup)
            {
                CharIdLookup.Remove(chara.Name);
                chara.OldName = chara.Name;
                chara.Name = newName;
                CharIdLookup.Add(chara.Name, chara.CharacterId);
                Database.SaveObject(chara);
                Database.ForceSave();
            }
        }

        public static characters GetCharacter(string name, bool fullLoad)
        {
            uint characterId = GetCharacterId(name);

            lock (Chars)
            {
                if (characterId > 0)
                    return Chars[characterId];

                foreach (characters chara in Chars.Values)
                    if (chara != null && name.Equals(chara.Name, StringComparison.OrdinalIgnoreCase))
                        return chara;
            }

            return LoadCharacterInfo(name, fullLoad);
        }

        public static characters GetCharacter(uint id, bool fullLoad)
        {
            lock (Chars)
                if (Chars.ContainsKey(id))
                    return Chars[id];

            return LoadCharacterInfo(id, fullLoad);
        }

        public static void GetCharactersWithName(string name, List<characters> inList)
        {
            uint characterId = GetCharacterId(name);

            lock (Chars)
            {
                if (characterId > 0)
                    inList.Add(Chars[characterId]);

                foreach (characters chara in Chars.Values)
                    if (!string.IsNullOrEmpty(chara?.OldName) && name.Equals(chara.OldName, StringComparison.OrdinalIgnoreCase))
                        inList.Add(chara);
            }
        }

        public static void RemoveCharacter(byte slot, GameClient client)
        {
            int accountId = client._Account.AccountId;

            uint characterId = GetAccountChar(accountId).RemoveCharacter(slot);

            lock (Chars)
                if (characterId > 0 && Chars.ContainsKey(characterId))
                {
                    characters_deletions record = new characters_deletions
                    {
                        DeletionIP = client.GetIp(),
                        AccountID = client._Account.AccountId,
                        AccountName = client._Account.Username,
                        CharacterID = characterId,
                        CharacterName = Chars[characterId].Name,
                        DeletionTimeSeconds = TCPManager.GetTimeStamp()
                    };

                    Database.AddObject(record);

                    characters Char = Chars[characterId];
                    Chars.Remove(characterId);
                    RemoveItemsFromCharacterId(characterId);
                    DeleteChar(Char);

                    Core.AcctMgr.UpdateRealmCharacters(Core.Rm.RealmId, (uint)Database.GetObjectCount<characters>(" Realm=1"), (uint)Database.GetObjectCount<characters>(" Realm=2"));
                }
        }

        public static void RemoveCharacter(Player deleter, int accountId, byte slot)
        {
            uint characterId = GetAccountChar(accountId).RemoveCharacter(slot);

            lock (Chars)
                if (characterId > 0 && Chars.ContainsKey(characterId))
                {
                    characters_deletions record = new characters_deletions
                    {
                        DeletionIP = deleter.Client.GetIp(),
                        AccountID = accountId,
                        AccountName = deleter.Client._Account.Username,
                        CharacterID = characterId,
                        CharacterName = Chars[characterId].Name,
                        DeletionTimeSeconds = TCPManager.GetTimeStamp()
                    };

                    Database.AddObject(record);

                    characters Char = Chars[characterId];
                    Chars.Remove(characterId);
                    RemoveItemsFromCharacterId(characterId);
                    DeleteChar(Char);

                    Core.AcctMgr.UpdateRealmCharacters(Core.Rm.RealmId, (uint)Database.GetObjectCount<characters>(" Realm=1"), (uint)Database.GetObjectCount<characters>(" Realm=2"));
                }
        }

        public static bool DeleteChar(characters Char)
        {
            lock (CharIdLookup)
                CharIdLookup.Remove(Char.Name);

            Database.DeleteObject(Char);
            Database.DeleteObject(Char.Value);
            Database.DeleteObject(Char.ClientData);

            if (Char.Socials != null)
                foreach (characters_socials obj in Char.Socials)
                    Database.DeleteObject(obj);

            if (Char.Toks != null)
                foreach (characters_toks obj in Char.Toks)
                    Database.DeleteObject(obj);

            if (Char.Quests != null)
                foreach (characters_quests obj in Char.Quests)
                    Database.DeleteObject(obj);

            if (Char.Influences != null)
                foreach (characters_influences obj in Char.Influences)
                    Database.DeleteObject(obj);
            if (Char.Bag_Pools != null)
                foreach (characters_bag_pools obj in Char.Bag_Pools)
                    Database.DeleteObject(obj);
            foreach (Guild gld in Guild.Guilds)
            {
                // Shouldnt be more than 1, but might as well check
                List<guild_members> toRemove = new List<guild_members>();
                foreach (guild_members member in gld.Info.Members.Values)
                    if (member.CharacterId == Char.CharacterId)
                        toRemove.Add(member);

                foreach (guild_members member in toRemove)
                    gld.LeaveGuild(member, false);
            }

            return true;
        }

        public static AccountChars GetAccountChar(int accountId)
        {
            lock (AcctChars)
            {
                if (!AcctChars.ContainsKey(accountId))
                    AcctChars[accountId] = new AccountChars(accountId);

                return AcctChars[accountId];
            }
        }

        public static bool NameIsUsed(string name)
        {
            return Database.SelectObject<characters>("Name='" + Database.Escape(name) + "'") != null;
        }

        /// <summary>
        /// To check if the characters name is deleted and a boot has not happened yet
        /// </summary>
        public static bool NameIsDeleted(string name)
        {
            Common.realms Rm = Core.Rm;
            return Database.SelectObject<characters_deletions>("CharacterName='" + Database.Escape(name) + "' AND DeletionTimeSeconds > " + Rm.BootTime) != null;
        }

        /// <summary>
        /// Precaches the characters for an account that is logging in.
        /// </summary>
        public static void LoadPendingCharacters()
        {
            AccountMgr mgr = Core.AcctMgr;

            if (mgr == null)
            {
                Log.Error("LoadPendingCharacters", "AccountMgr not available!");
                return;
            }

            List<int> accountIds = mgr.GetPendingAccounts();

            if (accountIds == null)
                return;

            StringBuilder sb = new StringBuilder($"CharacterId IN (SELECT CharacterId FROM {Database.GetSchemaName()}.characters t1 WHERE t1.AccountId IN (SELECT AccountId FROM {Core.AcctMgr.GetAccountSchemaName()}.accounts t2 WHERE t2.AccountId IN (");

            for (int i = 0; i < accountIds.Count; ++i)
            {
                AccountChars chars = GetAccountChar(accountIds[i]);
                if (chars != null && chars.Loaded)
                {
                    accountIds.RemoveAt(i);
                    --i;
                }
            }

            if (accountIds.Count == 0)
                return;

            for (int i = 0; i < accountIds.Count; ++i)
            {
                if (i > 0)
                    sb.Append(",");
                sb.Append(accountIds[i]);
            }
            sb.Append(")))");

            string whereString = sb.ToString();

            Dictionary<uint, characters_value> charValues = Database.SelectObjects<characters_value>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.FirstOrDefault());
            Dictionary<uint, characters_client_data> charClientData = Database.SelectObjects<characters_client_data>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.FirstOrDefault());
            Dictionary<uint, List<characters_socials>> charSocials = Database.SelectAllObjects<characters_socials>().GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
            Dictionary<uint, List<characters_toks>> charToks = Database.SelectObjects<characters_toks>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
            Dictionary<uint, List<characters_toks_kills>> charToksKills = Database.SelectObjects<characters_toks_kills>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
            Dictionary<uint, List<characters_quests>> charQuests = Database.SelectObjects<characters_quests>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
            Dictionary<uint, List<characters_influences>> charInfluences = Database.SelectObjects<characters_influences>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => (uint)g.Key, g => g.ToList());
            Dictionary<uint, List<characters_bag_pools>> charBagPools = Database.SelectObjects<characters_bag_pools>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => (uint)g.Key, g => g.ToList());
            Dictionary<uint, List<characters_mails>> charMail = Database.SelectObjects<characters_mails>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
            Dictionary<uint, List<characters_saved_buffs>> charBuffs = Database.SelectObjects<characters_saved_buffs>(whereString).GroupBy(v => v.CharacterId).ToDictionary(g => g.Key, g => g.ToList());
            Dictionary<uint, List<characters_honor_reward_cooldown>> charHonorCooldowns = Database.SelectAllObjects<characters_honor_reward_cooldown>().GroupBy(v => v.CharacterId).ToDictionary(g => (uint)g.Key, g => g.ToList());

            List<characters_items> charItems = (List<characters_items>)Database.SelectObjects<characters_items>(whereString);

            int count = 0;
            foreach (uint characterId in charValues.Keys)
            {
                characters chara = GetCharacter(characterId, false);

                if (charValues.ContainsKey(characterId)) chara.Value = charValues[characterId];
                if (charClientData.ContainsKey(characterId)) chara.ClientData = charClientData[characterId];
                if (charSocials.ContainsKey(characterId)) chara.Socials = charSocials[characterId];
                if (charToks.ContainsKey(characterId)) chara.Toks = charToks[characterId];
                if (charToksKills.ContainsKey(characterId)) chara.TokKills = charToksKills[characterId];
                if (charQuests.ContainsKey(characterId)) chara.Quests = charQuests[characterId];
                if (charInfluences.ContainsKey(characterId)) chara.Influences = charInfluences[characterId];
                if (charBagPools.ContainsKey(characterId)) chara.Bag_Pools = charBagPools[characterId];
                if (charMail.ContainsKey(characterId)) chara.Mails = charMail[characterId];
                if (charBuffs.ContainsKey(characterId)) chara.Buffs = charBuffs[characterId];
                if (charHonorCooldowns.ContainsKey(characterId)) chara.HonorCooldowns = charHonorCooldowns[characterId];
                // Mail list must never be null
                if (chara.Mails == null)
                    chara.Mails = new List<characters_mails>();
                if (chara.HonorCooldowns == null)
                    chara.HonorCooldowns = new List<characters_honor_reward_cooldown>();
                ++count;
            }

            foreach (characters_items item in charItems)
                LoadItem(item);

            foreach (int accountid in accountIds)
                GetAccountChar(accountid).Loaded = true;

            Log.Success("LoadPendingCharacters", $"{count} characters loaded.");
        }

        // I'm not sure whether multiple threads can load characters at once, so here be semaphores
        private static readonly Semaphore CharLoadSemaphore = new Semaphore(5, 5);

        /// <summary>
        /// Returns the characters associated with the given account ID, loading them if they are not yet cached.
        /// </summary>
        public static characters[] LoadCharacters(int accountId)
        {
            AccountChars accountChars = GetAccountChar(accountId);

            if (accountChars.Loaded || Core.Config.PreloadAllCharacters)
                return accountChars.Chars;

            string whereString = $"CharacterId IN (SELECT CharacterId from `{Database.GetSchemaName()}`.characters WHERE AccountId = '{accountId}')";
            CharLoadSemaphore.WaitOne();

            Log.Info("LoadCharacters", "Forced to load from connection thread for account ID " + accountId);

            List<characters_toks> charactersTok = (List<characters_toks>)Database.SelectObjects<characters_toks>(whereString);
            List<characters_quests> charactersQuest = (List<characters_quests>)Database.SelectObjects<characters_quests>(whereString);
            List<characters_influences> charactersInf = (List<characters_influences>)Database.SelectObjects<characters_influences>(whereString);
            List<characters_bag_pools> charactersBgPls = (List<characters_bag_pools>)Database.SelectObjects<characters_bag_pools>(whereString);
            List<characters_items> charactersItems = (List<characters_items>)Database.SelectObjects<characters_items>(whereString);
            List<characters_toks_kills> charToksKills = (List<characters_toks_kills>)Database.SelectObjects<characters_toks_kills>(whereString);
            List<characters_mails> charMail = (List<characters_mails>)Database.SelectObjects<characters_mails>(whereString);
            List<characters_saved_buffs> charBuffs = (List<characters_saved_buffs>)Database.SelectObjects<characters_saved_buffs>(whereString);

            CharLoadSemaphore.Release();

            for (int i = 0; i < accountChars.Chars.Length; ++i)
            {
                characters character = accountChars.Chars[i];
                if (character == null)
                    continue;

                if (character.Value == null)
                    character.Value = Database.SelectObject<characters_value>("CharacterId=" + character.CharacterId);
                if (character.ClientData == null)
                    character.ClientData = Database.SelectObject<characters_client_data>("CharacterId=" + character.CharacterId);
                character.Toks = charactersTok.FindAll(tok => tok.CharacterId == character.CharacterId);
                character.TokKills = charToksKills.FindAll(tok => tok.CharacterId == character.CharacterId);
                character.Quests = charactersQuest.FindAll(quest => quest.CharacterId == character.CharacterId);
                character.Influences = charactersInf.FindAll(inf => inf.CharacterId == character.CharacterId);
                character.Bag_Pools = charactersBgPls.FindAll(bgPls => bgPls.CharacterId == character.CharacterId);
                character.Mails = charMail.FindAll(mail => mail.CharacterId == character.CharacterId);
                character.Buffs = charBuffs.FindAll(buff => buff.CharacterId == character.CharacterId);

                List<characters_items> charItm = charactersItems.FindAll(item => item.CharacterId == character.CharacterId);

                lock (CharItems)
                {
                    if (!CharItems.ContainsKey(character.CharacterId))
                        CharItems.Add(character.CharacterId, charItm);
                }
            }

            Log.Info("LoadCharacters", "characters loading for account ID " + accountId + " completed.");

            accountChars.Loaded = true;

            return accountChars.Chars;
        }

        /// <summary>
        /// Loads the player's characters and builds a packet describing them.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static byte[] BuildCharacters(int accountId)
        {
            Log.Debug("BuildCharacters", "AccountId = " + accountId);

            characters[] chars = GetAccountChar(accountId).Chars;

            PacketOut Out = new PacketOut(0) { Position = 0 };

            Out.WriteByte(0x00); // in packetlogs this is 0

            for (int i = 0; i < MaxSlot; ++i)
            {
                characters Char = chars[i];

                if (Char == null)
                    Out.Fill(0, 284); // 284
                else
                {
                    if (Char.Value == null)
                        throw new NullReferenceException("characters " + Char.Name + " with ID " + Char.CharacterId + " is missing its character values!");

                    List<characters_items> items;

                    CharItems.TryGetValue(Char.CharacterId, out items);

                    if (items == null)
                        items = new List<characters_items>();

                    // The first and last name strings are each up to 24 bytes in length,
                    // and need to be null-terminated in the packet, allowing for 23 characters total.
                    Out.FillString(Char.Name, 23);
                    Out.WriteByte(0);
                    Out.FillString(Char.Surname, 23);
                    Out.WriteByte(0);
                    Out.WriteByte(Char.Value.Level);
                    Out.WriteByte(Char.Career);
                    Out.WriteByte(Char.Realm);
                    Out.WriteByte(Char.Sex);
                    Out.WriteByte(Char.ModelId);
                    Out.WriteByte(0);
                    Out.WriteUInt16R(Char.Value.ZoneId);
                    Out.Fill(0, 4);

                    characters_items Item;
                    for (ushort slotId = 19; slotId < 37; ++slotId)
                    {
                        Item = items.Find(item => item != null && item.SlotId == slotId);
                        if (Item == null)
                        {
                            Out.WriteUInt32R(0); // ModelId
                            Out.WriteUInt16R(0); // PrimaryDye
                            Out.WriteUInt16R(0); // SecondaryDye
                        }
                        else
                        {
                            if (Item.Alternate_AppereanceEntry > 0 && ItemService.GetItem_Info(Item.Alternate_AppereanceEntry) != null)
                                Out.WriteUInt32R(ItemService.GetItem_Info(Item.Alternate_AppereanceEntry).ModelId); // ModelId
                            else
                                Out.WriteUInt32R(Item.ModelId); // ModelId

                            if (Item.PrimaryDye == 0)
                                Out.WriteUInt16R(ItemService.GetItem_Info(Item.Entry).BaseColor1); // PrimaryDye
                            else
                                Out.WriteUInt16R(Item.PrimaryDye); // PrimaryDye
                            if (Item.SecondaryDye == 0)
                                Out.WriteUInt16R(ItemService.GetItem_Info(Item.Entry).BaseColor2); // PrimaryDye
                            else
                                Out.WriteUInt16R(Item.SecondaryDye); // SecondaryDye
                        }
                    }

                    for (int j = 0; j < 4; ++j)
                    {
                        Out.Fill(0, 4);
                        Out.WriteUInt16(0xFF00);
                        Out.Fill(0, 2);
                    }

                    for (ushort slotId = 10; slotId < 13; ++slotId)
                    {
                        Item = items.Find(item => item != null && item.SlotId == slotId);
                        Out.WriteUInt32R(Item?.ModelId ?? 0); // ModelId
                    }

                    Out.WriteHexStringBytes("0000000000000000010000"); // 05 || 0000 || 0B07000A03000602 || 00 00 00 00 00 00 00 || 00 00 00 00 00 00 00
                    /*Out.Fill(0, 10);
                    Out.WriteUInt16(0xFF00);
                    Out.WriteByte(0);
                    */
                    Out.WriteByte(Char.Race);
                    Out.WriteUInt16(0); // (Char.Value.TitleId); // title canot be seen in char selection this cause crashes
                    Out.Write(Char.bTraits, 0, Char.bTraits.Length);
                    Out.Fill(0, 14);// 272*/
                }
            }
            return Out.ToArray();
        }

        public static GameData.SetRealms GetAccountRealm(int accountId) => GetAccountChar(accountId).Realm;

        #endregion Characters

        #region Name filtering

        private static List<banned_names> BannedNameRecords;

        [LoadingFunction(false)]
        public static void LoadBannedNames()
        {
            BannedNameRecords = (List<banned_names>)Database.SelectAllObjects<banned_names>();
        }

        public static bool AddBannedName(string name, NameFilterType filtertype)
        {
            if (!AllowName(name))
                return false;

            lock (BannedNameRecords)
            {
                foreach (var record in BannedNameRecords)
                {
                    if (record.NameString.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                banned_names newRecord = new banned_names { NameString = name, FilterType = filtertype };
                Database.AddObject(newRecord);
                BannedNameRecords.Add(newRecord);
            }

            return true;
        }

        public static bool RemoveBannedName(string name)
        {
            lock (BannedNameRecords)
            {
                banned_names record = BannedNameRecords.Find(rec => rec.NameString == name);

                if (record == null)
                    return false;

                Database.DeleteObject(record);
                BannedNameRecords.Remove(record);
            }

            return true;
        }

        public static void ListBlockedNames(Player requester)
        {
            lock (BannedNameRecords)
            {
                requester.SendClientMessage("The following names are prohibited:", ChatLogFilters.CHATLOGFILTERS_CSR_TELL_RECEIVE);
                PrintBlockedNamesMatching(requester, NameFilterType.Equals);
                requester.SendClientMessage("The following are prohibited at the beginning of a name:", ChatLogFilters.CHATLOGFILTERS_CSR_TELL_RECEIVE);
                PrintBlockedNamesMatching(requester, NameFilterType.StartsWith);
                requester.SendClientMessage("The following are prohibited within a name:", ChatLogFilters.CHATLOGFILTERS_CSR_TELL_RECEIVE);
                PrintBlockedNamesMatching(requester, NameFilterType.Contains);
            }
        }

        private static void PrintBlockedNamesMatching(Player requester, NameFilterType filter)
        {
            int count = 0;

            StringBuilder names = new StringBuilder(1024);

            foreach (var record in BannedNameRecords)
            {
                if (record.FilterType != filter)
                    continue;

                if (count > 0)
                    names.Append(", ");

                names.Append(record.NameString);
                ++count;

                if (count == 16)
                {
                    requester.SendClientMessage(names.ToString());
                    count = 0;
                    names.Clear();
                }
            }

            if (count > 0)
                requester.SendClientMessage(names.ToString());
        }

        public static bool AllowName(string name)
        {
            lock (BannedNameRecords)
            {
                foreach (banned_names rec in BannedNameRecords)
                {
                    switch (rec.FilterType)
                    {
                        case NameFilterType.Equals:
                            if (name.Equals(rec.NameString, StringComparison.OrdinalIgnoreCase))
                                return false;
                            break;

                        case NameFilterType.StartsWith:
                            if (name.StartsWith(rec.NameString, StringComparison.OrdinalIgnoreCase))
                                return false;
                            break;

                        case NameFilterType.Contains:
                            if (name.IndexOf(rec.NameString, StringComparison.OrdinalIgnoreCase) != -1)
                                return false;
                            break;
                    }
                }
            }

            return true;
        }

        #endregion Name filtering

        #region Guilds

        public static void LoadAlliances()
        {
            Log.Info("LoadGuildAllianes", "Loading guild Allianes...");

            if (Core.Config.PreloadAllCharacters)
            {
                List<guild_alliance_info> Alliances = (List<guild_alliance_info>)Database.SelectAllObjects<guild_alliance_info>();
                foreach (guild_alliance_info ali in Alliances)
                {
                    Alliance.Alliances.Add(ali.AllianceId, ali);
                }
            }
            Log.Success("LoadGuildAlliance", Alliance.Alliances.Count + " Alliances loaded.");
            LoadGuilds();
        }

        public static void LoadGuilds()
        {
            Log.Info("LoadGuilds", "Loading guilds...");
            List<guild_info> guilds = (List<guild_info>)Database.SelectAllObjects<guild_info>();
            List<guild_members> guildMembers = (List<guild_members>)Database.SelectAllObjects<guild_members>();
            List<guild_ranks> guildRanks = (List<guild_ranks>)Database.SelectAllObjects<guild_ranks>();
            List<guild_logs> guildLogs = (List<guild_logs>)Database.SelectAllObjects<guild_logs>();
            List<guild_event> guildEvents = (List<guild_event>)Database.SelectAllObjects<guild_event>();
            List<guild_vault_item> guildVault = (List<guild_vault_item>)Database.SelectAllObjects<guild_vault_item>();

            if (Core.Config.PreloadAllCharacters)
            {
                List<guild_members> toRemove = new List<guild_members>();

                foreach (guild_members gldMem in guildMembers)
                {
                    if (Chars.ContainsKey(gldMem.CharacterId))
                        gldMem.Member = Chars[gldMem.CharacterId];
                    else
                        toRemove.Add(gldMem);
                }

                if (toRemove.Count > 0)
                    foreach (guild_members mem in toRemove)
                    {
                        Database.DeleteObject(mem);
                        guildMembers.Remove(mem);
                    }

                foreach (guild_info guild in guilds)
                {
                    if (guild.AllianceId > 0)
                    {
                        if (Alliance.Alliances.ContainsKey(guild.AllianceId))
                            Alliance.Alliances[guild.AllianceId].Members.Add(guild.GuildId);
                        else
                        {
                            Log.Error("LoadGuilds", guild.Name + " has an invalid allianceID");
                            guild.AllianceId = 0;
                            Database.SaveObject(guild);
                        }
                    }

                    try
                    {
                        guild.Members = guildMembers.FindAll(info => info.GuildId == guild.GuildId).ToDictionary(x => x.CharacterId, x => x);
                        guild.Ranks = guildRanks.FindAll(info => info.GuildId == guild.GuildId).OrderBy(info => info.RankId).ToDictionary(x => x.RankId, x => x);
                        guild.Logs = guildLogs.FindAll(info => info.GuildId == guild.GuildId).OrderBy(info => info.Time).ThenByDescending(info => info.Type).ToList();
                        guild.Event = guildEvents.FindAll(info => info.GuildId == guild.GuildId).OrderBy(info => info.SlotId).ToDictionary(x => x.SlotId, x => x);
                    }
                    catch (Exception)
                    {
                        Log.Error("LoadGuilds", "Failed load of guild: " + guild.Name);
                    }

                    guild.Vaults[0] = guildVault.FindAll(info => info.GuildId == guild.GuildId && info.VaultId == 1).OrderBy(info => info.SlotId).ToDictionary(x => x.SlotId, x => x);
                    guild.Vaults[1] = guildVault.FindAll(info => info.GuildId == guild.GuildId && info.VaultId == 2).OrderBy(info => info.SlotId).ToDictionary(x => x.SlotId, x => x);
                    guild.Vaults[2] = guildVault.FindAll(info => info.GuildId == guild.GuildId && info.VaultId == 3).OrderBy(info => info.SlotId).ToDictionary(x => x.SlotId, x => x);
                    guild.Vaults[3] = guildVault.FindAll(info => info.GuildId == guild.GuildId && info.VaultId == 4).OrderBy(info => info.SlotId).ToDictionary(x => x.SlotId, x => x);
                    guild.Vaults[4] = guildVault.FindAll(info => info.GuildId == guild.GuildId && info.VaultId == 5).OrderBy(info => info.SlotId).ToDictionary(x => x.SlotId, x => x);

                    Guild.Guilds.Add(new Guild(guild));

                    List<guild_members> members = guild.Members.Values.OrderByDescending(x => x.RankId).ToList();

                    //checks if theres more then 1 guildmember with guild rank of 9 (leader)

                    int rank9count = 0;

                    for (int x = 0; x < members.Count; x++)
                    {
                        if (members[x].RankId == 9)
                            rank9count++;
                        else
                            break;
                    }

                    if (rank9count > 1)
                    {
                        Log.Info("LoadGuilds", guild.Name + "More then 1 guildleader found removeing all rank 9 players");
                        for (int x = 0; x < members.Count; x++)
                        {
                            if (members[x].RankId == 9)
                            {
                                members[x].RankId = 8;
                                Database.SaveObject(members[x]);
                            }
                            else
                                break;
                        }

                        if (guild.Members.ContainsKey(guild.LeaderId))
                        {
                            guild_members mem;
                            guild.Members.TryGetValue(guild.LeaderId, out mem);
                            mem.RankId = 9;
                            Database.SaveObject(mem);
                        }
                    }

                    //checks for guild leader id player not found guildleader banned or guild leader inactive is so tryes to set a new guild leader if no guildleader can be found guild is set to inactive
                    accounts accountEntity = null;
                    var characterEntity = CharMgr.GetCharacter(guild.LeaderId, true);
                    if (characterEntity != null)
                        accountEntity = Core.AcctMgr.GetAccountById(characterEntity.AccountId);

                    if ((characterEntity != null) && (accountEntity != null))
                    {
                        if (!guild.Members.ContainsKey(guild.LeaderId)
                            || guild.Members[guild.LeaderId].RankId != 9
                            || accountEntity.Banned == 1
                            || accountEntity.GmLevel == -1
                            || CharMgr.GetCharacter(guild.LeaderId, true).Value.LastSeen + 2246400 <
                            TCPManager.GetTimeStamp())
                        {
                            bool newleaderfound = false;

                            for (int i = 0; i < members.Count; i++)
                            {
                                if (CharMgr.GetCharacter(members[i].CharacterId, true).Value.LastSeen + 2246400 >
                                    TCPManager.GetTimeStamp())
                                {
                                    newleaderfound = true;
                                    guild.LeaderId = members[i].CharacterId;
                                    members[i].RankId = 9;
                                    CharMgr.Database.SaveObject(members[i]);
                                    Database.SaveObject(guild);
                                    Log.Info("LoadGuilds",
                                        guild.Name +
                                        " Leader not found banned or not loged in for 26 days, set to new leader " +
                                        members[i].CharacterId);
                                    break;
                                }
                            }

                            if (!newleaderfound)
                                Guild.GetGuild(guild.Name).Inactive = true;
                        }
                    }
                    else
                    {
                        Log.Error("Missing Guild Leader", $"Guild Leader {guild.LeaderId} is missing in the the DB");
                    }

                    if (guild.GuildId > Guild.MaxGuildGUID)
                        Guild.MaxGuildGUID = (int)guild.GuildId;
                }
            }
            else
            {
                foreach (guild_info gld in guilds)
                {
                    //Log.Success("LoadGuilds", "Loading guild " + gld.Name);

                    List<characters> guildCharacters = (List<characters>)Database.SelectObjects<characters>($"CharacterId IN (SELECT CharacterId FROM `{Database.GetSchemaName()}`.guild_members WHERE GuildId = {gld.GuildId})");

                    foreach (characters gChar in guildCharacters)
                    {
                        if (!Chars.ContainsKey(gChar.CharacterId))
                        {
                            AddChar(gChar);
                            gChar.Value = Database.SelectObject<characters_value>("CharacterId=" + gChar.CharacterId);
                        }
                    }

                    gld.Members = guildMembers.FindAll(info => info.GuildId == gld.GuildId).ToDictionary(x => x.CharacterId, x => x);

                    foreach (guild_members m in gld.Members.Values)
                        m.Member = GetCharacter(m.CharacterId, false);

                    gld.Ranks = guildRanks.FindAll(info => info.GuildId == gld.GuildId).OrderBy(info => info.RankId).ToDictionary(x => x.RankId, x => x);
                    gld.Logs = guildLogs.FindAll(info => info.GuildId == gld.GuildId).OrderBy(info => info.Time).ThenByDescending(info => info.Type).ToList();
                    Guild.Guilds.Add(new Guild(gld));

                    if (gld.GuildId > Guild.MaxGuildGUID)
                        Guild.MaxGuildGUID = (int)gld.GuildId;
                }
            }
        }

        public static bool ChangeGuildName(guild_info guild, string newName)
        {
            guild.Name = newName;
            Database.SaveObject(guild);
            Database.ForceSave();
            return true;
        }

        public static bool DeleteGuild(guild_info guild)
        {
            Database.DeleteObject(guild);

            if (guild.Members != null)
                foreach (guild_members obj in guild.Members.Values)
                    Database.DeleteObject(obj);

            if (guild.Ranks != null)
                foreach (guild_ranks obj in guild.Ranks.Values)
                    Database.DeleteObject(obj);

            if (guild.Logs != null)
                foreach (guild_logs obj in guild.Logs)
                    Database.DeleteObject(obj);

            return true;
        }

        #endregion Guilds

        #region CharacterItems

        public static Dictionary<uint, List<characters_items>> CharItems = new Dictionary<uint, List<characters_items>>();

        [LoadingFunction(true)]
        public static void LoadItems()
        {
            long myCount;

            Log.Info("LoadItems", "Loading items...");
            lock (CharItems)
            {
                CharItems.Clear();
            }

            IList<characters_items> charItems;

            if (Core.Config.PreloadAllCharacters)
                charItems = Database.SelectAllObjects<characters_items>();
            else
            {
                string whereString = $"CharacterId IN (SELECT CharacterId FROM `{Database.GetSchemaName()}`.characters t1 WHERE t1.AccountId IN (SELECT AccountId FROM `{Core.AcctMgr.GetAccountSchemaName()}`.accounts t2 WHERE t2.LastLogged >= {RecentHistoryTime}))";
                charItems = Database.SelectObjects<characters_items>(whereString);
            }

            myCount = charItems.Count;

            lock (CharItems)
                foreach (characters_items itm in charItems)
                    LoadItem(itm);

            Log.Success("LoadItems", $"{myCount} inventory items {(Core.Config.PreloadAllCharacters ? "loaded" : "precached")}.");
        }

        public static void CreateItem(characters_items item)
        {
            LoadItem(item);
            Database.AddObject(item);
            Database.ForceSave();
        }

        public static void LoadItem(characters_items charItem)
        {
            lock (CharItems)
            {
                if (!CharItems.ContainsKey(charItem.CharacterId))
                    CharItems.Add(charItem.CharacterId, new List<characters_items> { charItem });
                else
                    CharItems[charItem.CharacterId].Add(charItem);
            }
        }

        public static List<characters_items> GetItemsForCharacter(characters chara)
        {
            try
            {
                _logger.Debug($"GetItemsForCharacter ==> {chara.Name}");
                lock (CharItems)
                {
                    if (CharItems.ContainsKey(chara.CharacterId))
                        return CharItems[chara.CharacterId];
                }

                Log.Info("GetItemsForChar", "Loading items for CharacterId: " + chara.CharacterId);
                _logger.Debug($"Loading items for CharacterId ==> {chara.Name}");

                List<characters_items> myItems = (List<characters_items>)Database.SelectObjects<characters_items>("CharacterId='" + chara.CharacterId + "'");

                if (myItems != null && myItems.Count > 0)
                {
                    lock (CharItems)
                    {
                        if (!CharItems.ContainsKey(chara.CharacterId))
                        {
                            _logger.Debug($"Adding items for CharacterId ==> {chara.Name}");
                            CharItems.Add(chara.CharacterId, myItems);
                        }
                        _logger.Debug($"Returning items for CharacterId ==> {chara.Name}");
                        return CharItems[chara.CharacterId];
                    }
                }
                _logger.Debug($"Getting CharacterInfoItem for {chara.Name} career {chara.CareerLine}");
                List<character_info_items> Items = GetCharacterInfoItem(chara.CareerLine);
                _logger.Debug($"Found CharacterInfoItem Count={Items.Count} for {chara.Name}");
                foreach (character_info_items Itm in Items)
                {
                    if (Itm == null)
                        continue;

                    _logger.Debug($"Adding item {Itm.Entry} to character {chara.Name}");

                    characters_items Citm = new characters_items
                    {
                        Counts = Itm.Count,
                        CharacterId = chara.CharacterId,
                        Entry = Itm.Entry,
                        ModelId = Itm.ModelId,
                        SlotId = Itm.SlotId,
                        PrimaryDye = 0,
                        SecondaryDye = 0
                    };
                    CreateItem(Citm);
                }

                lock (CharItems)
                {
                    if (CharItems.ContainsKey(chara.CharacterId))
                        return CharItems[chara.CharacterId];
                    else
                    {
                        _logger.Warn($"Returning empty char item list for character {chara.Name}");
                        return new List<characters_items>();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Debug($"Exception creating items for character {e.Message} {e.StackTrace}");
                throw;
            }
        }

        public static void SaveItems(uint characterId, List<Item> oldItems)
        {
            List<characters_items> newItems = new List<characters_items>();
            for (int i = 0; i < oldItems.Count; ++i)
                if (oldItems[i] != null)
                    newItems.Add(oldItems[i].Save(characterId));

            lock (CharItems)
            {
                CharItems.Remove(characterId);
                CharItems.Add(characterId, newItems);
            }
        }

        public static void DeleteItem(characters_items itm)
        {
            lock (CharItems)
            {
                if (CharItems.ContainsKey(itm.CharacterId))
                    CharItems[itm.CharacterId].Remove(itm);
            }

            Database.DeleteObject(itm);
        }

        public static void RemoveItemsFromCharacterId(uint characterId, bool excludeBook = false)
        {
            lock (CharItems)
            {
                characters_items book = null;

                if (!CharItems.ContainsKey(characterId))
                    return;

                foreach (characters_items item in CharItems[characterId])
                {
                    if (item.Entry == 11919 && excludeBook)
                    {
                        book = item;
                        continue;
                    }
                    Database.DeleteObject(item);
                }

                if (book == null)
                    CharItems.Remove(characterId);
                else
                {
                    CharItems[characterId].Clear();
                    CharItems[characterId].Add(book);
                }
            }
        }

        #endregion CharacterItems

        #region CharacterMail

        private static int _maxMailGuid = 1;

        public static int GenerateMailGuid()
        {
            return Interlocked.Increment(ref _maxMailGuid);
        }

        [LoadingFunction(true)]
        public static void LoadMailCount()
        {
            if (Core.Config.PreloadAllCharacters)
            {
                Log.Debug("WorldMgr", "Loading Character_mails...");

                List<characters_mails> mails = (List<characters_mails>)Database.SelectAllObjects<characters_mails>();
                int count = 0;
                if (mails != null)
                {
                    List<characters_mails> expired = mails.FindAll(mail => MailInterface.TimeToExpire(mail) <= 0);

                    if (expired.Count > 0)
                    {
                        foreach (var mail in expired)
                        {
                            Database.DeleteObject(mail);
                            mails.Remove(mail);
                        }

                        Log.Success("LoadMails", "Removed " + expired.Count + " expired mails.");
                    }

                    foreach (characters_mails mail in mails)
                    {
                        if (mail.Guid > _maxMailGuid)
                            _maxMailGuid = mail.Guid;
                        count++;
                    }
                }
                Log.Success("LoadMails", "Loaded " + count + " items of mail.");
            }
            else
            {
                _maxMailGuid = Database.GetObjectCount<characters_mails>();
                Log.Success("LoadMails", _maxMailGuid + " existing mails.");
            }
        }

        public static void AddMail(characters_mails mail)
        {
            characters character = GetCharacter(mail.CharacterId, false);

            if (character == null)
                return;

            if (character.Mails == null)
            {
                _logger.Info("Mail System loading mail for " + character.Name);
                character.Mails = (List<characters_mails>)Database.SelectObjects<characters_mails>("CharacterId='" + mail.CharacterId + "'");
            }

            character.Mails.Add(mail);
            Database.AddObject(mail);

            _logger.Info($"Mail System mail count = {character.Mails.Count}");
            Player receiver = Player.GetPlayer(mail.ReceiverName);
            _logger.Debug($"Mail Receiver : {mail.ReceiverName}");

            receiver?.MlInterface?.AddMail(mail);
        }

        public static void DeleteMail(characters_mails mail)
        {
            Chars[mail.CharacterId].Mails.Remove(mail);
            Database.DeleteObject(mail);

            Player receiver = Player.GetPlayer(mail.ReceiverName);
            receiver?.MlInterface?.RemoveMail(mail);
        }

        public static void RemoveMailFromCharacter(characters chara)
        {
            if (chara.Mails == null)
                return;

            foreach (characters_mails mail in chara.Mails)
                Database.DeleteObject(mail);

            chara.Mails.Clear();
        }

        #endregion CharacterMail

        #region Support Tickets

        public static List<bugs_reports> _report = new List<bugs_reports>();

        [LoadingFunction(true)]
        public static void LoadTickets()
        {
            List<bugs_reports> reports = (List<bugs_reports>)Database.SelectAllObjects<bugs_reports>();

            foreach (bugs_reports report in reports)
                _report.Add(report);

            Log.Success("CharacterMgr", "Loaded " + _report.Count + " Support Tickets");
        }

        public static bugs_reports GetReport(string reportID)
        {
            var ticket = _report.Find(x => x.ObjectId == reportID);

            if (ticket == null)
                return null;

            return ticket;
        }

        #endregion Support Tickets

        public static void RemoveQuestsFromCharacter(characters chara)
        {
            if (chara.Quests == null)
                return;
            foreach (characters_quests quest in chara.Quests)
                Database.DeleteObject(quest);

            chara.Quests.Clear();
        }

        public static void RemoveToKsFromCharacter(characters chara)
        {
            if (chara.Toks == null)
                return;
            foreach (characters_toks tok in chara.Toks)
                Database.DeleteObject(tok);

            chara.Toks.Clear();
        }

        public static void RemoveToKKillsFromCharacter(characters chara)
        {
            if (chara.TokKills == null)
                return;

            foreach (characters_toks_kills tokKill in chara.TokKills)
                Database.DeleteObject(tokKill);

            chara.TokKills.Clear();
        }
    }
}