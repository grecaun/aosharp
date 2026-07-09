using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.Linq;

namespace AutomatonCity
{
    public class BossLootState : IState
    {
        private bool _atCorpse = false;
        private double _timeToLeave;

        public void OnStateEnter()
        {
            _atCorpse = false;
            _timeToLeave = 0;

            Chat.WriteLine("Boss loot state");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;
            if (!AutomatonCity._settings["Enable"].AsBool()) return AutomatonCity.Idle_State; ;

            if (_timeToLeave == 0 || Time.AONormalTime < _timeToLeave) return null;

            if (Playfield.IsDungeon && DynelManager.LocalPlayer.Room.Name == "AI_bossroom")
                return AutomatonCity.Button_Exit_State;

            if (AutomatonCity.validPlayfields.Contains(Playfield.ModelIdentity.Instance))
                return AutomatonCity.Idle_State;

            return null;
        }

        public void Tick()
        {
            try
            {
                if (Game.IsZoning || !Team.IsInTeam) { return; }

                AutomatonCity.ChangedFloors();

                if (MovementController.Instance.IsNavigating) return;

                Corpse bossCorpse = null;

                if (Playfield.IsDungeon)
                {
                    bossCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name.Contains("Fleet Admiral") || c.Name.Contains("General") || c.Name.Contains("Recruitment Director"));
                }
                else
                {
                    bossCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name.Contains("General"));
                }

                if (bossCorpse != null)
                {
                    if (_atCorpse) return;

                    if (AtPosition(bossCorpse.Position, 2))
                    {
                        Chat.WriteLine("Pause for looting, 30 sec");
                        _timeToLeave = Time.AONormalTime + 30;

                        if (!Playfield.IsDungeon && bossCorpse.Name.Contains("General"))
                            if (!AutomatonCity.Bosses.Contains(bossCorpse.Identity))
                                AutomatonCity.Bosses.Add(bossCorpse.Identity);

                        _atCorpse = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred on line " + AutomatonCity.GetLineNumber(ex) + ": " + ex.Message;

                if (errorMessage != AutomatonCity.previousErrorMessage)
                {
                    Chat.WriteLine(errorMessage);
                    Chat.WriteLine("Stack Trace: " + ex.StackTrace);
                    AutomatonCity.previousErrorMessage = errorMessage;
                }
            }
        }

        bool AtPosition(Vector3 position, int distance)
        {
            HandlePathing(position, distance);
            return MovementController.Instance.IsNavigating == false;
        }

        void HandlePathing(Vector3 position, int distance)
        {
            if (DynelManager.LocalPlayer.Position.DistanceFrom(position) > distance)
            {
                if (!MovementController.Instance.IsNavigating)
                    MovementController.Instance.SetDestination(position);
            }
            else
            {
                if (MovementController.Instance.IsNavigating)
                    MovementController.Instance.Halt();
            }
        }

        public void OnStateExit()
        {
            _atCorpse = false;
            _timeToLeave = 0;
            SMovementController.Halt();
            Chat.WriteLine("Exit Boss loot state");
        }
    }
}