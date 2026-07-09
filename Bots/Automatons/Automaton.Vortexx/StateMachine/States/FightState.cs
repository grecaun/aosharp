using AOSharp.Core;
using AOSharp.Core.Movement;
using System.Linq;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.ChatMessages;
using AutomatonVortexx.IPCMessages;
using AOSharp.Core.UI;
using AOSharp.Common.GameData;

namespace AutomatonVortexx
{
    public class FightState : IState
    {
        private bool _shout = false;
        private Dynel _currentNanoEruptionTarget = null;

        public void OnStateEnter()
        {
            Network.ChatMessageReceived += VortexxShout;

            if (AutomatonVortexx.NavMeshMovementController.IsNavigating)
                AutomatonVortexx.NavMeshMovementController.Halt();
            AutomatonVortexx._clearToEnter = false;
            _shout = false;

            Chat.WriteLine("Fight!");
        }

        public IState GetNextState()
        {
            if (Playfield.ModelIdentity.Instance != Constants.VortexxId) return AutomatonVortexx.Idle;

            var _releasedSpirit = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Released Spirit");

            if (AutomatonVortexx._settings["Immunity"].AsBool() && _releasedSpirit != null)
            {
                if (DynelManager.LocalPlayer.IsAttacking)
                    DynelManager.LocalPlayer.StopAttack(false);
                else
                    return AutomatonVortexx.Immunity;
            }

            var Mobs = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 &&
            (c.Name == "Ground Chief Vortexx" || c.Name == "Desecrated Spirit"));

            if (Mobs != null) return null;

            if (AutomatonVortexx._settings["Farming"].AsBool())
                return AutomatonVortexx.Loot;

            return null;
        }

        public void VortexxShout(object s, ChatMessageBody msg)
        {
            if (msg.PacketType == ChatMessageType.NpcMessage)
            {
                var npcMsg = (NpcMessage)msg;

                string[] triggerMsg = new string[4] { "Flee you pathetic insects", "Fear my power", "I will have your heads", "Breathe in the terror" };

                if (triggerMsg.Any(x => npcMsg.Text.Contains(x)))
                    _shout = true;
            }
        }

        public void Tick()
        {
            if (!Team.IsInTeam || Game.IsZoning) return;

            if (Playfield.ModelIdentity.Instance != Constants.VortexxId) return;

            var _releasedSpiritCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name == "Remains of Released Spirit");

            if (_releasedSpiritCorpse != null && DynelManager.LocalPlayer.Identity == AutomatonVortexx.Leader)
            {
                if (!AutomatonVortexx._clearToEnter && _releasedSpiritCorpse.Position.DistanceFrom(Constants._bluePodium) < 5)
                {
                    AutomatonVortexx.IPCChannel.Broadcast(new EnterMessage());
                    AutomatonVortexx._clearToEnter = true;
                }
            }

            if (_shout)
            {
                var newestEruption = DynelManager.AllDynels.Where(c => c.Name == "Nano Eruption").OrderByDescending(t => t.GetStat(Stat.TimeExist)).FirstOrDefault();

                if (newestEruption != null)
                {
                    bool isNewTarget = _currentNanoEruptionTarget == null || _currentNanoEruptionTarget != newestEruption;

                    _currentNanoEruptionTarget = newestEruption;

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(newestEruption.Position) > 1f)
                    {
                        if (isNewTarget || !MovementController.Instance.IsNavigating)
                            AutomatonVortexx.NavMeshMovementController.SetNavMeshDestination(newestEruption.Position);
                    }
                    else if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonVortexx.Nanos.NanoInfusion))
                    {
                        _shout = false;
                        _currentNanoEruptionTarget = null;
                    }
                }
                else
                {
                    _currentNanoEruptionTarget = null;
                    HandlePathtoCenter();
                }
            }
            else
            {
                _currentNanoEruptionTarget = null;
                HandlePathtoCenter();

                var _vortexx = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Ground Chief Vortexx");
                var _desecratedSpirits = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Desecrated Spirit");

                if (_vortexx != null)
                    HandleAttacking(_vortexx);
                else if (_desecratedSpirits != null)
                    HandleAttacking(_desecratedSpirits);
            }
        }

        private void HandlePathtoCenter()
        {
            var _nanoErruption = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Nano Eruption");

            if ((!DynelManager.LocalPlayer.Buffs.Contains(AutomatonVortexx.Nanos.Terrified) || _nanoErruption == null) && DynelManager.LocalPlayer.Position.DistanceFrom(Constants._centerPodium) > 2f)
            {
                if (!MovementController.Instance.IsNavigating)
                    AutomatonVortexx.NavMeshMovementController.SetNavMeshDestination(Constants._centerPodium);
            }
        }

        private void HandleAttacking(SimpleChar target)
        {
            if (target == null) return;

            if (DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttackPending)
                DynelManager.LocalPlayer.Attack(target, false);
        }

        public void OnStateExit()
        {
            Network.ChatMessageReceived -= VortexxShout;

            if (DynelManager.LocalPlayer.IsAttacking)
                DynelManager.LocalPlayer.StopAttack(false);

            if (AutomatonVortexx.NavMeshMovementController.IsNavigating)
                AutomatonVortexx.NavMeshMovementController.Halt();

            AutomatonVortexx._clearToEnter = false;
            _shout = false;
        }
    }
}