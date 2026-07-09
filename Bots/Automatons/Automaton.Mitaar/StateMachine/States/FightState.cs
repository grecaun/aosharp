using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using System.Linq;

namespace AutomatonMitaar
{
    public class FightState : IState
    {
        bool TeamReady;

        public void OnStateEnter()
        {
            TeamReady = false;

            MovementController.Instance.SetDestination(Constants._redPodium);

            Chat.WriteLine("Starting Hardcore Parkour!");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            if (Playfield.ModelIdentity.Instance == Constants.XanHubId || Playfield.Name == "Central Gateway")
                return AutomatonMitaar.Idle;

            if (!AutomatonMitaar._settings["Farming"].AsBool()) return null;

            if (!Extensions.CanProceed()) return null;

            var mobs = DynelManager.NPCs.Where(c => c.Health > 0 && 
            (c.Name == "Alien Coccoon" || c.Name == "Xan Spirit" || c.Name == "Technomaster Sinuh")).ToList();

            if (mobs.Count == 0)
                return AutomatonMitaar.Loot;

            return null;
        }

        public void Tick()
        {
            if (!Team.IsInTeam || Game.IsZoning) { return; }

            if (Playfield.ModelIdentity.Instance != Constants.MitaarId) { return; }

            var _sinuh = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Technomaster Sinuh");

            var _alienCoccoon = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name == "Alien Coccoon").OrderBy(d => DynelManager.LocalPlayer.Position.DistanceFrom(d.Position)).ToList();

            var AltaroftheLight = DynelManager.AllDynels.FirstOrDefault(a => a.Name == "Altar of the Light");
            var AltaroftheOutsider = DynelManager.AllDynels.FirstOrDefault(a => a.Name == "Altar of the Outsider");
            var AltaroftheSource = DynelManager.AllDynels.FirstOrDefault(a => a.Name == "Altar of the Source");
            var AltaroftheTrueBlood = DynelManager.AllDynels.FirstOrDefault(a => a.Name == "Altar of the True Blood");

            if (!Team.Members.Any(t => t.Character == null))
                TeamReady = true;

            if (TeamReady)
            {
                if (DynelManager.NPCs.Where(c => c.Health > 0 && c.Name == "Xan Spirit").Any())
                {
                    if (AutomatonMitaar._settings["StopAttack"].AsBool())
                    {
                        if (DynelManager.LocalPlayer.IsAttacking == true)
                            DynelManager.LocalPlayer.StopAttack(false);
                    }

                    if (AutomatonMitaar._settings["Red"].AsBool())
                        HandleSpirits(AltaroftheTrueBlood, AutomatonMitaar.SpiritNanos.BlessingofTheBlood);

                    if (AutomatonMitaar._settings["Blue"].AsBool())
                        HandleSpirits(AltaroftheSource, AutomatonMitaar.SpiritNanos.BlessingofTheSource);

                    if (AutomatonMitaar._settings["Green"].AsBool())
                        HandleSpirits(AltaroftheOutsider, AutomatonMitaar.SpiritNanos.BlessingofTheOutsider);

                    if (AutomatonMitaar._settings["Yellow"].AsBool())
                        HandleSpirits(AltaroftheLight, AutomatonMitaar.SpiritNanos.BlessingofTheLight);
                }
                else if (_alienCoccoon.Any())
                    HandleAttack(_alienCoccoon.FirstOrDefault());
                else if (_sinuh != null)
                    HandleAttack(_sinuh);
            }
        }
        void HandleSpirits(Dynel altar, int spell)
        {
            var localplayer = DynelManager.LocalPlayer;

            if (altar != null)
            {
                if (!localplayer.Buffs.Contains(spell))
                    HandleSpiritPathing(altar);
                else if (localplayer.Buffs.Find(spell, out Buff buff) && buff.RemainingTime < 3)
                    HandleSpiritPathing(altar);
            }
        }

        void HandleSpiritPathing(Dynel altar)
        {
            if (DynelManager.LocalPlayer.Position.DistanceFrom(altar.Position) > 0.9f)
            {
                if (!MovementController.Instance.IsNavigating)
                    MovementController.Instance.SetDestination(altar.Position);
            }
        }

        void HandleAttack(SimpleChar target)
        {
            if (DynelManager.LocalPlayer.IsAttacking == true)
            {
                if (DynelManager.LocalPlayer.FightingTarget?.Identity != target.Identity)
                    DynelManager.LocalPlayer.StopAttack(false);
            }
            else if (!DynelManager.LocalPlayer.IsAttackPending)
            {
                DynelManager.LocalPlayer.Attack(target, false);
                return;
            }
        }

        public void OnStateExit()
        {
            TeamReady = false;
        }
    }
}