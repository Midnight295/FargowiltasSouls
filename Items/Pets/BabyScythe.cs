﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Pets
{
    public class BabyScythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Scythe");
            Tooltip.SetDefault("Summons Baby Abom\n'Don't worry, it's dull'");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WispinaBottle);
            item.value = Item.sellPrice(0, 5);
            item.rare = -13;
            item.shoot = mod.ProjectileType("BabyAbom");
            item.buffType = mod.BuffType("BabyAbomBuff");
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}