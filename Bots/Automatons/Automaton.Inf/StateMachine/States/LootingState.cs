using AOSharp.Core;
using AOSharp.Core.UI;
using System.Linq;
using AOSharp.Pathfinding;
using AOSharp.Common.GameData;

namespace AutomatonInf
{
    public class LootingState : IState
    {
        public void OnStateEnter()
        {
            AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();

            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            Chat.WriteLine("Moving to corpse");

            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.ClanPandeGarden:
                case Constants.OmniPandeGarden:
                     return AutomatonInf.Died;
                case Constants.Mission:

                    var corpse = DynelManager.Corpses.FirstOrDefault();

                    if (corpse == null)
                    {
                        if (AutomatonInf._settings["ModeSelection"].AsInt32() == 0)
                            return AutomatonInf.Defend;

                        else
                            return AutomatonInf.Roam;
                    }
                    break;
                case Constants.Inferno:
                     return AutomatonInf.Idle;
            }

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            var corpse = DynelManager.Corpses.FirstOrDefault();

            if (corpse == null) { return; }

            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) { return; }

            if (DynelManager.LocalPlayer.Position.DistanceFrom(corpse.Position) > 5f)
            {
                if (DynelManager.LocalPlayer.Velocity == 0)
                    SMovementController.SetNavDestination(corpse.Position);
            }
        }

        public void OnStateExit()
        {
        }
    }
}