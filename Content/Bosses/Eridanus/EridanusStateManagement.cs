using FargowiltasSouls.Core.Systems;
using Luminance.Common.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Eridanus
{
    public partial class Eridanus
    {
        private PushdownAutomata<EntityAIState<BehaviorStates>, BehaviorStates> stateMachine;

        /// <summary>
        /// The state machine that controls the behavior of this NPC.
        /// </summary>
        public PushdownAutomata<EntityAIState<BehaviorStates>, BehaviorStates> StateMachine
        {
            get
            {
                if (stateMachine == null)
                    LoadStateMachine();

                return stateMachine;
            }
            private set => stateMachine = value;
        }

        private void LoadStateMachine()
        {
            StateMachine = new(new(BehaviorStates.Opening));

            for (int i = 0; i < (int)BehaviorStates.Count; i++)
                StateMachine.RegisterState(new((BehaviorStates)i));

            StateMachine.OnStateTransition += OnStateTransition;
            StateMachine.OnStackEmpty += OnStackEmpty;

            // Autoload the state behaviors.
            AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>.FillStateMachineBehaviors<ModNPC>(StateMachine, this);

            StateMachine.RegisterTransition(BehaviorStates.Opening, BehaviorStates.Meteors, false, () => Timer > 125, () =>
            {
                --AI3;
            });

            StateMachine.RegisterTransition(BehaviorStates.Meteors, null, false, () => AI2 >= 500);
        }

        public void OnStateTransition(bool stateWasPopped, EntityAIState<BehaviorStates> oldState)
        {
            NPC.netUpdate = true;
            NPC.TargetClosest(false);
            AI2 = 0;
            AI3 = 0;

            if (oldState != null && Attacks.Contains(oldState.Identifier))
                LastAttackChoice = (int)oldState.Identifier;

        }
        // This is ran when the stack runs out of attacks.
        public void OnStackEmpty()
        {
            NPC.netUpdate = true;

            if (!FargoSoulsUtil.HostCheck)
                return;

            StateMachine.StateStack.Clear();

            // Get the correct attack list, and remove the last attack to avoid repeating it.
            List<BehaviorStates> attackList = Attacks.Where(attack => attack != (BehaviorStates)LastAttackChoice).ToList();

            // Fill a list of indices.
            var indices = new List<int>();
            for (int i = 0; i < attackList.Count; i++)
                indices.Add(i);

            // Randomly push the attack list using the indices list accessed with a random index.
            for (int i = 0; i < attackList.Count; i++)
            {
                var currentIndex = indices[Main.rand.Next(0, indices.Count)];
                StateMachine.StateStack.Push(StateMachine.StateRegistry[attackList[currentIndex]]);
                indices.Remove(currentIndex);
            }
        }
    }
}
