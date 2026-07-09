using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System.Linq;

namespace AutomatonDB1
{
    public class GetBuffState : IState
    {
        private BuffStage _stage;
        private Vector3 _currentTarget = Vector3.Zero;

        private enum BuffStage { Yellow,  Blue, Green, Red, FinalCheck  }

        public void OnStateEnter()
        {
            Chat.WriteLine("GetBuffState");
            _stage = BuffStage.Yellow;
            _currentTarget = Vector3.Zero;
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            if (Playfield.ModelIdentity.Instance != Constants.DB1Id)
                return AutomatonDB1.Idle;

            if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.ThriceBlessedbytheAncients))
                return AutomatonDB1.Fight;

            return null;
        }

        public void Tick()
        {
            if (!Team.IsInTeam || Game.IsZoning)
                return;

            if (Playfield.ModelIdentity.Instance != Constants.DB1Id)
                return;

            if (DynelManager.LocalPlayer.Identity == AutomatonDB1.Leader && DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttackPending)
            {
                var mikkelsen = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Ground Chief Mikkelsen"));

                if (mikkelsen != null)
                    DynelManager.LocalPlayer.Attack(mikkelsen, false);
            }

            switch (_stage)
            {
                case BuffStage.Yellow:
                    if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.BlessingoftheAncientMachinist))
                    {
                        _stage = BuffStage.Blue;
                        ResetMovement();
                        return;
                    }
                    MoveTo(Constants._yellowPodium);
                    break;

                case BuffStage.Blue:
                    if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.BlessingoftheEternalCraftsman))
                    {
                        _stage = BuffStage.Green;
                        ResetMovement();
                        return;
                    }
                    MoveTo(Constants._bluePodium);
                    break;

                case BuffStage.Green:
                    if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.BlessingoftheAncientForm))
                    {
                        _stage = BuffStage.Red;
                        ResetMovement();
                        return;
                    }
                    MoveTo(Constants._greenPodium);
                    break;

                case BuffStage.Red:
                    if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.BlessingoftheEternalCleric))
                    {
                        _stage = BuffStage.FinalCheck;
                        ResetMovement();
                        return;
                    }
                    MoveTo(Constants._redPodium);
                    break;
                case BuffStage.FinalCheck:
                    if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.ThriceBlessedbytheAncients))
                        return;

                    _stage = BuffStage.Yellow;
                    ResetMovement();
                    break;
            }
        }

        private void MoveTo(Vector3 position)
        {
            if (_currentTarget != position && DynelManager.LocalPlayer.Position.DistanceFrom(position) > 2f)
            {
                AutomatonDB1.NavMeshMovementController.SetNavMeshDestination(position);
                _currentTarget = position;
            }
        }

        private void ResetMovement()
        {
            AutomatonDB1.NavMeshMovementController.Halt();
            _currentTarget = Vector3.Zero;
        }

        public void OnStateExit()
        {
            Chat.WriteLine("Exit GetBuffState");
            _currentTarget = Vector3.Zero;
        }
    }
}
