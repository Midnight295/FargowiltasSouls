using FargowiltasSouls.Assets.ExtraTextures;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Projectiles.Souls;
using Terraria.Audio;

namespace FargowiltasSouls.Content.Bosses.Eridanus
{
    public class EriMeteor : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Souls/MeteorEnchantMeatball";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 60;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            //Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            //Projectile.penetrate = 1;
            //Projectile.usesIDStaticNPCImmunity = true;
            //Projectile.idStaticNPCHitCooldown = 120;
            //Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
            //Projectile.FargoSouls().CanSplit = false;
            //Projectile.extraUpdates = 2;

            Projectile.scale = 1;
        }

        public override void AI()
        {
            float dir;
            if (Main.LocalPlayer.Center.X >= Projectile.Center.X)
                dir = -0.15f;
            else
                dir = 0.15f;
            Vector2 vel = new Vector2(dir, Main.rand.NextFloat(0.3f, 1));
            Projectile.velocity += vel;


            if (Projectile.Center.Y > Main.LocalPlayer.Center.Y)
                Projectile.Kill();
            //base.AI();
        }

        public override void OnKill(int timeLeft)
        {
            ScreenShakeSystem.StartShake(2f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < Main.rand.Next(0, 3); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(35, 0), Vector2.Zero,
                    ModContent.ProjectileType<EriMeteorite>(), 0, Projectile.knockBack, Main.myPlayer, 0, Projectile.ai[1]);
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<MeteorExplosion>(), 0, Projectile.knockBack, Main.myPlayer);
            }
                
            base.OnKill(timeLeft);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.1f;
            float ratio = MathF.Pow(completionRatio, 1.5f);
            return MathHelper.SmoothStep(baseWidth, 25f * Projectile.scale, ratio);
        }

        public static readonly Color OrangeColor = Color.Lerp(Color.OrangeRed, Color.Orange, 0.4f);
        public static Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(OrangeColor, Color.SkyBlue, completionRatio);
            float opacity = 0.7f;
            return color * opacity;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawMeteor(Projectile, Texture, ref lightColor);
            return false;
        }
        public static void DrawMeteor(Projectile Projectile, string textureString, ref Color lightColor)
        {
            Vector2 normalizedVel = Projectile.velocity.SafeNormalize(Vector2.Zero);
            //draw projectile
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            int num156 = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            //Vector2 drawOffset = Projectile.rotation.ToRotationVector2() * (texture2D13.Height - Projectile.height) / 2;

            Color drawColor = Projectile.GetAlpha(lightColor);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D circle = FargosTextureRegistry.BloomTexture.Value;
            float circleScale = 0.35f * Projectile.scale;
            Vector2 circleOffset = normalizedVel * 4f * Projectile.scale;
            Main.EntitySpriteDraw(circle, Projectile.Center + circleOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, OrangeColor * 0.1f * Projectile.Opacity, Projectile.rotation, circle.Size() / 2f, circleScale, effects, 0);
            //glow
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            float count = 12f;
            Color glowColor = Color.Lerp(Color.White, OrangeColor, 1f) * (1 / (count * 0.1f)) * Projectile.Opacity;
            for (int j = 0; j < count; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / count).ToRotationVector2() * 4.5f;
                afterimageOffset += normalizedVel * 3f;

                Main.EntitySpriteDraw(texture, Projectile.Center + afterimageOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), glowColor, Projectile.rotation, origin2, Projectile.scale, effects, 0f);
            }
            Vector2 glowOffset = normalizedVel * 3.5f;
            Main.EntitySpriteDraw(texture, Projectile.Center + glowOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), OrangeColor * 1f * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, effects, 0f);

            Main.spriteBatch.ResetToDefault();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), drawColor, Projectile.rotation, origin2, Projectile.scale, effects, 0);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 3; i++)
            {
                Texture2D glowTexture = ModContent.Request<Texture2D>(textureString + "Glow").Value;
                Vector2 offset = normalizedVel * (i - 1) * 4;
                float glowScale = 1.12f * Projectile.scale;
                Rectangle glowRect = new(0, 0, glowTexture.Width, glowTexture.Height);
                Main.EntitySpriteDraw(glowTexture, Projectile.Center + offset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), glowRect, OrangeColor with { A = 160 } * Projectile.Opacity * 0.8f, normalizedVel.ToRotation() + MathHelper.PiOver2, glowTexture.Size() / 2, glowScale, effects, 0f);
            }
            Main.spriteBatch.ResetToDefault();
        }
    }
}
