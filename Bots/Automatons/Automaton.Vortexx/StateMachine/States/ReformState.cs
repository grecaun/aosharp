using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System.Collections.Generic;
using System.Linq;

namespace AutomatonVortexx
{
    public class ReformState : IState
    {
        ReformPhase _phase = ReformPhase.Completed;
        ReformPhase last;
        List<Identity> _teamCache = new List<Identity>();
        List<Identity> _invitedList = new List<Identity>();
        double _disbandDelay = 0;

        public void OnStateEnter()
        {
            if (AutomatonVortexx.NavMeshMovementController.IsNavigating)
                AutomatonVortexx.NavMeshMovementController.Halt();
            Chat.WriteLine("Reforming");
            _phase = ReformPhase.Disbanding;
            _teamCache.Clear();
            _invitedList.Clear();
            _disbandDelay = Time.AONormalTime + 5.0;
            AutomatonVortexx._stateTimeOut = Time.AONormalTime + 10.0;
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            if (Time.AONormalTime < AutomatonVortexx._stateTimeOut)
                return null;

            if (_phase == ReformPhase.Completed)
                return AutomatonVortexx.Idle;

            return null;
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
                        if (AutomatonVortexx.Leader == DynelManager.LocalPlayer.Identity)
                            if (Time.AONormalTime < _disbandDelay) return;

                        foreach (var member in Team.Members)
                        {
                            if (!_teamCache.Contains(member.Identity))
                                _teamCache.Add(member.Identity);
                        }

                        if (AutomatonVortexx.Leader == DynelManager.LocalPlayer.Identity)
                            foreach (var member in Team.Members)
                            {
                                if (member.Identity == DynelManager.LocalPlayer.Identity) continue;
                                Team.Kick(member.Identity);
                            }
                    }
                    return;
                case ReformPhase.Inviting:
                    if (Team.IsInTeam)
                    {
                        if (_teamCache.Count == Team.Members.Count)
                            _phase = ReformPhase.Completed;
                    }
                    else if (AutomatonVortexx.Leader == DynelManager.LocalPlayer.Identity)
                        InvitePlayers();

                    return;
            }
        }
        void InvitePlayers()
        {
            foreach (var player in _teamCache.Where(p => !Team.Members.Contains(p) && !_invitedList.Contains(p) && p != DynelManager.LocalPlayer.Identity))
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
            _teamCache.Clear();
            _invitedList.Clear();
            _disbandDelay = 0;
            AutomatonVortexx.Counter++;
        }
    }
}
