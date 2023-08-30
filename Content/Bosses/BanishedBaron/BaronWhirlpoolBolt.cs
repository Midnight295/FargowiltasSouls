﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{

    public class BaronWhirlpoolBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_385";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron Mine Shrapnel");
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width / 2;

        public override void AI()
        {
            ref float variant = ref Projectile.ai[0];

            if (++Projectile.localAI[0] > 600f)
            {
                Projectile.Kill();
            }
            //a bit after spawning, become tangible when it finds an open space
            if (!Projectile.tileCollide && Projectile.localAI[0] > 60 * Projectile.MaxUpdates)
            {
                Tile tile = Framing.GetTileSafely(Projectile.Center);
                if (!(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]))
                    Projectile.tileCollide = true;
            }

            //animate
            if (++Projectile.frameCounter > 2)
            {
                if (++Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }

            switch (variant)
            {
                case 1: //underwater whirlpool, go straight out
                    int sign = Math.Sign(Projectile.velocity.X);
                    if (Projectile.velocity.X < 14)
                    {
                        Projectile.velocity.X += sign * 0.06f;
                    }
                    break;
                case 2:
                    break;
            }
        }
    }
}
