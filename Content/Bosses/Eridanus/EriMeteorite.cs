using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Eridanus
{
    public class EriMeteorite : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 250;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] <= 15)
            {
                Projectile.Center += new Vector2(0, -15f);
            }
            
            if (Projectile.ai[0] >= 15)
            {
                NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<Eridanus>());
                if (npc != null)
                {
                    Projectile.velocity = Projectile.SafeDirectionTo(npc.Center) * MathHelper.Lerp(0, 5, ++Projectile.ai[2] * 0.07f);
                    if (Projectile.Distance(npc.Center) <= 50)
                    {
                        SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, npc.Center);
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.Kill();
                    return;
                }
                
                    
            }
            Projectile.rotation += 0.5f;
            base.AI();
        }
    }
}
