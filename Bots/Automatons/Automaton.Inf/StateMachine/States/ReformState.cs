using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System.Collections.Generic;
using System.Linq;

namespace AutomatonInf
{
    public class ReformState : IState
    {
        double disbandDelay;

        List<Identity> TeamList = new List<Identity>();
        Dictionary<Identity, double> InvitedPlayers = new Dictionary<Identity, double>();
        List<Identity> kickedMember = new List<Identity>();

        enum Reform { Disband, Waiting, Form, Resend, Done }
        Reform CurrentReformState;
        Reform LastReformState = Reform.Waiting;

        public void OnStateEnter()
        {
            AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();

            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            if (Game.IsZoning) { return; }

            if (DynelManager.LocalPlayer.Identity != AutomatonInf.Leader)
                AutomatonInf.Leader = Identity.None;

            TeamList.Clear();
            InvitedPlayers.Clear();
            kickedMember.Clear();
            disbandDelay = 0.0;
            AutomatonInf.teamDelay = Time.AONormalTime + 5.0;

            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();

            CurrentReformState = Reform.Disband;

            Chat.WriteLine("Reforming");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }
            if (!AutomatonInf.Enable) { return null; }
            if (CurrentReformState != Reform.Done) { return null; }

            return AutomatonInf.Idle;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            if (CurrentReformState != LastReformState) { Chat.WriteLine(CurrentReformState, ChatColor.Green); LastReformState = CurrentReformState; }

            switch (CurrentReformState)
            {
                case Reform.Disband:
                    if (Team.IsInTeam)
                    {
                        if (Time.AONormalTime < AutomatonInf.teamDelay) { return; }

                        if (!Team.Members.Any(t => t.Character == null) && disbandDelay == 0.0)
                            disbandDelay = Time.AONormalTime + 5.0;

                        if (disbandDelay > 0.0 && Time.AONormalTime >= disbandDelay)
                        {
                            foreach (var m in Team.Members.Where(m => m != null && !TeamList.Contains(m.Identity)))
                                TeamList.Add(m.Identity);

                            if (DynelManager.LocalPlayer.Identity != AutomatonInf.Leader) { return; }

                            var member = Team.Members.FirstOrDefault(m => m != null && m.Identity != AutomatonInf.Leader);

                            if (member != null)
                            {
                                if (!kickedMember.Contains(member.Identity)) kickedMember.Add(member.Identity);
                                else Team.Kick(member.Identity);
                            }
                        }
                    }
                    else if (DynelManager.LocalPlayer.Identity != AutomatonInf.Leader)
                        CurrentReformState = Reform.Waiting;
                    else
                        CurrentReformState = Reform.Form;

                    break;

                case Reform.Waiting:
                    if (!Team.IsInTeam) { return; }
                    if (Team.Members.Count < TeamList.Count)
                    {
                        if (DynelManager.LocalPlayer.Identity != AutomatonInf.Leader) { return; }

                        var expired = InvitedPlayers.FirstOrDefault(kvp => kvp.Value <= Time.AONormalTime && !Team.Members.Any(m => m.Identity == kvp.Key));

                        if (!expired.Equals(default(KeyValuePair<Identity, double>)))
                        {
                            InvitedPlayers.Remove(expired.Key);
                            InvitePlayers();
                        }
                    }
                    else
                        CurrentReformState = Reform.Done;
                    break;
                case Reform.Form:
                    InvitePlayers();
                    break;
            }
        }
        private void InvitePlayers()
        {
            var player = DynelManager.Players.FirstOrDefault(c => TeamList.Contains(c.Identity) && !InvitedPlayers.ContainsKey(c.Identity) && c.Identity != DynelManager.LocalPlayer.Identity);
            if (player != null)
            {
                InvitedPlayers.Add(player.Identity, Time.AONormalTime + 60);
                Team.Invite(player.Identity);
                return;
            }

            CurrentReformState = Reform.Waiting;
        }

        public void OnStateExit()
        {
            AutomatonInf.Counter++;
            TeamList.Clear();
            InvitedPlayers.Clear();
            kickedMember.Clear();
            disbandDelay = 0.0;

            CurrentReformState = Reform.Disband;
            LastReformState = Reform.Waiting;
        }
    }
}
