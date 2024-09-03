using GameData;
using WorldServer.World.Objects;

namespace WorldServer.World.Battlefronts.Apocalypse
{
    public class RamSpawnFlagComparitor : ILocationComparitor
    {
        public SetRealms PlayerRealm { get; }
        public int ComparisonRange = 50;

        public RamSpawnFlagComparitor(SetRealms playerRealm)
        {
            PlayerRealm = playerRealm;
        }

        public bool InRange(Player player)
        {
            if (player.Realm != PlayerRealm)
                return false;

            var objectsInRange = player.GetInRange<GameObject>(ComparisonRange);
            foreach (var test in objectsInRange)
            {
                switch (PlayerRealm)
                {
                    case SetRealms.REALMS_REALM_DESTRUCTION:
                        {
                            if (test.Entry == 666572)
                                return true;
                            else
                            {
                                break;
                            }
                        }
                    case SetRealms.REALMS_REALM_ORDER:
                        {
                            if (test.Entry == 666571)
                                return true;
                            else
                            {
                                break;
                            }
                        }
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}