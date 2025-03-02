using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;

namespace FargowiltasSouls.Content.Bosses.Eridanus
{
    [AutoloadBossHead]
    public partial class Eridanus : ModNPC
    {
        public float Timer
        {
            get => StateMachine.StateStack.Count != 0 ? StateMachine.CurrentState.Time : 0;
            set
            {
                if (StateMachine.StateStack.Count != 0)
                    StateMachine.CurrentState.Time = (int)value;
            }
        }
        private int LastAttackChoice { get; set; }

        public ref float AI2 => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eridanus, Champion of Cosmos");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "厄里达诺斯, 宇宙英灵");

            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPC.AddDebuffImmunities(
            [
                BuffID.Confused,
                BuffID.Chilled,
                BuffID.OnFire,
                BuffID.Suffocation,
                BuffID.Lovestruck,
                ModContent.BuffType<LethargicBuff>(),
                ModContent.BuffType<ClippedWingsBuff>(),
                ModContent.BuffType<TimeFrozenBuff>(),
                ModContent.BuffType<LightningRodBuff>()
            ]);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = false,
                Position = new Vector2(8, 16),
                PortraitScale = 1f,
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 8
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            ]);
        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                // This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
                return NPC.GetBestiaryEntryColor();
            }
            return base.GetAlpha(drawColor);
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 100;
            NPC.damage = 150;
            NPC.defense = 70;
            NPC.lifeMax = 600000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(10);
            NPC.boss = true;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Silent") : MusicID.OtherworldlyLunarBoss;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.scale *= 1.5f;

            NPC.dontTakeDamage = true;
            NPC.alpha = 255;

            NPC.trapImmune = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public ref float Animation => ref NPC.ai[0];
    }
}
