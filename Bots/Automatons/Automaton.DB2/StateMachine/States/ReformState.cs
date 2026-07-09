using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System.Collections.Generic;
using System.Linq;

namespace AutomatonDB2
{
    public class ReformState : IState
    {
        ReformPhase _phase = ReformPhase.Completed;
        ReformPhase last;

        List<Identity> _invitedList = new List<Identity>();
        double _disbandDelay = 0;

        public void OnStateEnter()
        {
            if (SMovementController.IsNavigating())
                SMovementController.Halt();

            Chat.WriteLine("Reforming");
            _phase = ReformPhase.Disbanding;
            _invitedList.Clear();
            _disbandDelay = Time.AONormalTime + 5.0;
            AutomatonDB2._stateTimeOut = Time.AONormalTime + 10.0;
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            if (Playfield.ModelIdentity.Instance != Constants.PWId)
                return AutomatonDB2.Idle;

            if (Time.AONormalTime < AutomatonDB2._stateTimeOut)
                return null;

            if (_phase != ReformPhase.Completed) return null;

            return AutomatonDB2.Idle;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            if (_phase != last) { Chat.WriteLine(_phase, ChatColor.Green); last = _phase; }

            switch (_phase)
            {
                case ReformPhase.Disbanding:
                    if (!Team.IsInTeam)
                    {
                        _phase = ReformPhase.Inviting;
                    }
                    else if (!Team.Members.Any(t => t.Character == null))
                    {
                        if (AutomatonDB2.Leader == DynelManager.LocalPlayer.Identity)
                            if (Time.AONormalTime < _disbandDelay) return;

                        foreach (var member in Team.Members)
                        {
                            if (!AutomatonDB2._teamCache.Contains(member.Identity))
                                AutomatonDB2._teamCache.Add(member.Identity);
                        }

                        if (AutomatonDB2.Leader == DynelManager.LocalPlayer.Identity)
                            foreach (var member in Team.Members)
                            {
                                if (member.Identity == DynelManager.LocalPlayer.Identity) continue;
                                Team.Kick(member.Identity);
                            }
                    }
                    return;
                case ReformPhase.Inviting:
                    if (Time.AONormalTime < _disbandDelay) return;
                    if (Team.IsInTeam)
                    {
                        if (AutomatonDB2._teamCache.All(id => Team.Members.Any(m => m.Identity == id)))
                            _phase = ReformPhase.Completed;
                    }
                    else if (AutomatonDB2.Leader == DynelManager.LocalPlayer.Identity)
                        InvitePlayers();

                    return;

            }
        }
        void InvitePlayers()
        {
            foreach (var player in AutomatonDB2._teamCache.Where(p => !Team.Members.Contains(p) && !_invitedList.Contains(p) && p != DynelManager.LocalPlayer.Identity))
            {
                _invitedList.Add(player);
                Team.Invite(player);
            }
        }
        enum ReformPhase { Disbanding, Inviting, Completed, }

        public void OnStateExit()
        {
            Chat.WriteLine("Done Reforming");
            _phase = ReformPhase.Disbanding;
            AutomatonDB2._teamCache.Clear();
            _invitedList.Clear();
            _disbandDelay = 0;
            AutomatonDB2.Counter++;
        }
    }
}