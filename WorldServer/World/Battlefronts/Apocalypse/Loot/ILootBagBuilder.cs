using Common;
using System.Collections.Generic;
using WorldServer.World.Objects;

namespace WorldServer.World.Battlefronts.Apocalypse.Loot
{
    public interface ILootBagBuilder
    {
        KeyValuePair<item_infos, List<Talisman>> BuildChestLootBag(LootBagTypeDefinition lootBag, Player player);

        KeyValuePair<item_infos, List<Talisman>> BuildChestLootBag(LootBagRarity rarity, uint itemId, Player player);
    }
}