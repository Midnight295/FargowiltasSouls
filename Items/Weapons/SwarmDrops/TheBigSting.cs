﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Fargowiltas.Items.Tiles;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class TheBigSting : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Big Sting");
            Tooltip.SetDefault("Uses darts for ammo\n66% chance to not consume ammo\n'The reward for slaughtering many..'");
            DisplayName.AddTranslation(GameCulture.Chinese, "大螫刺");
            Tooltip.AddTranslation(GameCulture.Chinese, "'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            item.damage = 300;
            item.ranged = true;
            item.width = 24;
            item.height = 24;
            item.useTime = 6;
            item.useAnimation = 6;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = 500000;
            item.rare = 11;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.Stinger>();
            item.useAmmo = AmmoID.Dart;
            item.UseSound = SoundID.Item97;
            item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = item.shoot;

            //tsunami code
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            float num = 0.314159274f;
            int numShots = 3;
            Vector2 vel = new Vector2(speedX, speedY);
            vel.Normalize();
            vel *= 40f;
            bool collide = Collision.CanHit(vector, 0, 0, vector + vel, 0, 0);

            float rotation = MathHelper.ToRadians(Main.rand.NextFloat(0, 10));

            for (int i = 0; i < numShots; i++)
            {
                float num3 = i - (numShots - 1f) / 2f;
                Vector2 value = Utils.RotatedBy(vel, num * num3, default(Vector2));

                if (!collide)
                {
                    value -= vel;
                }

                Vector2 speed = new Vector2(speedX, speedY).RotatedBy(rotation * num3);
                Projectile.NewProjectile(vector.X + value.X, vector.Y + value.Y, speed.X, speed.Y, type, damage, knockBack, player.whoAmI);
            }

            return false;
        }

        //make them hold it different
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.Next(3) == 0;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TheSmallSting");
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerBee"));
            recipe.AddIngredient(ItemID.LunarBar, 10);
            //recipe.AddIngredient(null, "LunarCrystal", 5);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}