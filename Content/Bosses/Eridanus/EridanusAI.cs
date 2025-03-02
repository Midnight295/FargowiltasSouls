using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Luminance.Common.StateMachines;
using System.Runtime.CompilerServices;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;
using FargowiltasSouls.Core.Globals;
using Terraria.DataStructures;

namespace FargowiltasSouls.Content.Bosses.Eridanus
{
    public partial class Eridanus : ModNPC
    {
        public Player Player => Main.player[NPC.target];
        public enum BehaviorStates
        {
            Opening,
            SecondPhaseTransition,

            Meteors,
            Vortex,


            Count
        }

        public readonly List<BehaviorStates> Attacks =
        [
            BehaviorStates.Meteors,
            BehaviorStates.Vortex,
        ];

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false, bool useAntiWobble = true)
        {
            if (useAntiWobble)
            {
                float accel = 1f * speedModifier;
                float decel = 1.5f * speedModifier;
                float resistance = NPC.velocity.Length() * accel / (35f * speedModifier);
                NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, targetPos, NPC.velocity, accel - resistance, decel + resistance);
            }
            else
            {
                if (NPC.Center.X < targetPos.X)
                {
                    NPC.velocity.X += speedModifier;
                    if (NPC.velocity.X < 0)
                        NPC.velocity.X += speedModifier * 2;
                }
                else
                {
                    NPC.velocity.X -= speedModifier;
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X -= speedModifier * 2;
                }
                if (NPC.Center.Y < targetPos.Y)
                {
                    NPC.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y += speedModifier * 2;
                }
                else
                {
                    NPC.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y -= speedModifier * 2;
                }
            }

            float dist = NPC.Distance(targetPos);
            if (dist == 0)
                dist = 0.1f;
            if (NPC.velocity.Length() > dist)
                NPC.velocity = Vector2.Normalize(NPC.velocity) * dist;
            MathHelper.Clamp(NPC.velocity.X, -cap, cap);
            MathHelper.Clamp(NPC.velocity.Y, -cap, cap);
        }

        //public override void FindFrame(int frameHeight)
        //{
        //    base.FindFrame(frameHeight);
        //}

        public override void AI()
        {
            EModeGlobalNPC.championBoss = NPC.whoAmI;

            if (Player.Center.X >= NPC.Center.X)
                NPC.spriteDirection = -1;
            else
                NPC.spriteDirection = 1;

            StateMachine.PerformBehaviors();
            StateMachine.PerformStateTransitionCheck();

            // Ensure that there is a valid state timer to get.
            if (StateMachine.StateStack.Count > 0)
                Timer++;
            //base.AI();
        }

        #region States
        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.Opening)]
        public void Opening()
        {
            NPC.alpha = 255;
            if (Timer == 0)
            { 
                //NPC.Center = Main.player[NPC.target].Center - 250 * Vector2.UnitY;
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CosmosVortex>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
            }
            if (Timer == 117) // 1 frame before so can music fade up next frame
            {
                if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
                {
                    Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/PlatinumStar");
                }
                else
                {
                    Music = MusicID.OtherworldlyLunarBoss;
                }
            }
            if (Timer > 117)
                Main.musicFade[Main.curMusic] += 0.2f;
            if (Timer > 120)
            {
                NPC.netUpdate = true;
                //NPC.ai[1] = 0;
                //NPC.localAI[3] = 1;
                NPC.alpha = 0;
                NPC.dontTakeDamage = false;
                NPC.velocity = NPC.DirectionFrom(Player.Center).RotatedByRandom(MathHelper.PiOver2) * 20f;
            }
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.Meteors)]
        public void Meteors()
        {   
            if (AI2 < 360)
            {
                float x = NPC.spriteDirection == -1 ? -505 : 505;
                Vector2 targetpos = new Vector2(x, 0);
                Movement(Player.Center + targetpos,0.5f, 32, false, true);
            }
            else
            {
                float x = NPC.spriteDirection == -1 ? -805 : 805;
                Vector2 targetpos = new Vector2(x, 0);
                Movement(Player.Center + targetpos, 0.5f, 32, false, true);
            }
            
            //NPC.position += Player.velocity / 3f;

            Vector2 predict = Player.Center + Player.velocity * 15;
            Vector2 predict2 = Player.Center + Player.velocity * 25;
            Vector2 predict3 = Player.Center + Player.velocity * 35;

            if (Timer == 15)
            {   
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(0, -1000), Vector2.Zero, ModContent.ProjectileType<EriMeteor>(), 15, 0, Main.myPlayer, 0, NPC.whoAmI, Main.rand.NextFloat(0, 5f));
                
                
            }
            if (Timer == 30)
                Projectile.NewProjectile(NPC.GetSource_FromThis(), predict2 + new Vector2(0, -1000), Vector2.Zero, ModContent.ProjectileType<EriMeteor>(), 15, 0, Main.myPlayer, 0, NPC.whoAmI, Main.rand.NextFloat(0, 5f));
            if (Timer >= 60)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), predict3 + new Vector2(0, -1000), Vector2.Zero, ModContent.ProjectileType<EriMeteor>(), 15, 0, Main.myPlayer, 0, NPC.whoAmI, Main.rand.NextFloat(0, 5f));
                Timer = 0;
            }

            if (++AI2 >= 360)
            {

                Timer = 0;
                //++AI3;
                if (++AI3 >= 35)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Player.Center, ModContent.ProjectileType<CosmosMeteor>(), 15, 0, Main.myPlayer);
                    NPC.Center += Player.Center * 0.001f;
                    AI3 = 0;
                }

            }
                


        }
        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.Vortex)]
        public void Vortex()
        {
            NPC.velocity *= 0;
            if (Timer == 5)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CosmosVortex>(), 15, 0, Main.myPlayer);
            }
        }

        #endregion
    }
}
