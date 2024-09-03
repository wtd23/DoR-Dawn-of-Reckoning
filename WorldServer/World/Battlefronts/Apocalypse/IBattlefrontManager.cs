using Common.Database.World.Battlefront;
using GameData;
using System.Collections.Generic;
using WorldServer.World.Battlefronts.Bounty;
using WorldServer.World.Map;

namespace WorldServer.World.Battlefronts.Apocalypse
{
    public interface IBattleFrontManager
    {
        byte GetBattleFrontState(int pairing, int tier);

        ImpactMatrixManager ImpactMatrixManagerInstance { get; set; }

        BountyManager BountyManagerInstance { get; set; }

        List<rvr_progression> BattleFrontProgressions { get; }

        Dictionary<int, rvr_progression> ActiveBattlefronts { get; set; }

        rvr_progression GetActiveBattleFrontFromProgression(int pairingId);

        rvr_progression GetBattleFrontByName(string name);

        rvr_progression GetBattleFrontByBattleFrontId(int id);


        void AuditBattleFronts(int tier);

        void LockBattleFrontsAllRegions(int tier, bool forceDefaultRealm = false);

        rvr_progression AdvanceBattleFront(int pairingId, SetRealms lockingRealm);

        void OpenActiveBattlefront();

        void LockActiveBattleFront(int pairingId, SetRealms realm, int forceNumberOfBags = 0);

        List<BattleFrontStatus> GetBattleFrontStatusList();

        bool IsBattleFrontLocked(int battleFrontId);

        BattleFrontStatus GetBattleFrontStatus(int battleFrontId);

        BattleFrontStatus GetActiveCampaign(RegionMgr region);

        void LockBattleFrontStatus(int battleFrontId, SetRealms lockingRealm, VictoryPointProgress vpp);

        BattleFrontStatus GetRegionBattleFrontStatus(int regionId);

        Campaign GetActiveCampaign(int pairingId);

        Campaign GetActiveCampaign(ushort zoneId);

        BattleFrontStatus GetActiveBattleFrontStatus(int battleFrontId);

        void Update(long tick);

        void UpdateRVRPRogression(SetRealms lockingRealm, rvr_progression oldProg, rvr_progression newProg);

        bool IsInContestedZone(ushort zoneId);
    }
}