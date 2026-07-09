using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using System.Linq;

namespace AutomatonMitaar
{
    public class SoloState : IState
    {
        double attackDelay;

        public void OnStateEnter()
        {
            if (AutomatonMitaar.NavMeshMovementController.IsNavigating)
                AutomatonMitaar.NavMeshMovementController.Halt();

            Chat.WriteLine("Some alone time..");
            MovementController.Instance.SetDestination(Constants._redPodium);
            attackDelay = 0;
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }
            if (Playfield.ModelIdentity.Instance == Constants.XanHubId || Playfield.Name == "Central Gateway")
                return AutomatonMitaar.Idle;

            if (!AutomatonMitaar._settings["Farming"].AsBool()) return null;

            if (!Extensions.CanProceed()) return null;

            var _alienCoccoon = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name == "Alien Coccoon").ToList();

            var _xanSpirits = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name == "Xan Spirit").ToList();

            if (_xanSpirits.Count == 0 && _alienCoccoon.Count == 0)
                return AutomatonMitaar.Loot;

            return null;
        }

        public void Tick()
        {
            if (!Team.IsInTeam || Game.IsZoning) { return; }

            if (Playfield.ModelIdentity.Instance != Constants.MitaarId) { return; }

            var _sinuh = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Technomaster Sinuh");

            var _alienCoccoon = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name == "Alien Coccoon")
                .OrderBy(d => DynelManager.LocalPlayer.Position.DistanceFrom(d.Position)).ToList();

            var AltaroftheLight = DynelManager.AllDynels.Where(a => a.Name == "Altar of the Light").FirstOrDefault();
            var AltaroftheOutsider = DynelManager.AllDynels.Where(a => a.Name == "Altar of the Outsider").FirstOrDefault();
            var AltaroftheSource = DynelManager.AllDynels.Where(a => a.Name == "Altar of the Source").FirstOrDefault();
            var AltaroftheTrueBlood = DynelManager.AllDynels.Where(a => a.Name == "Altar of the True Blood").FirstOrDefault();

           
            if (DynelManager.NPCs.Where(c => c.Health > 0 && c.Name == "Xan Spirit").Any())
            {
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
        void HandleSpirits(Dynel altar, int spell)
        {
            var localplayer = DynelManager.LocalPlayer;

            if (altar != null)
            {
                if (!localplayer.Buffs.Contains(spell))
                    HandleSpiritPathing(altar);
                else
                {
                    if (localplayer.Buffs.Find(spell, out Buff buff) && buff.RemainingTime < 3)
                        HandleSpiritPathing(altar);
                }
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
            if (Time.AONormalTime > attackDelay)
            {
                foreach (var pet in DynelManager.LocalPlayer.Pets.Where(p => p.Type == PetType.Attack || p.Type == PetType.Support))
                {
                    if (pet != null)
                    {
                        if (pet.Character.IsAttacking == true)
                        {
                            if (pet.Character.FightingTarget?.Identity != target.Identity)
                                pet.Attack(target.Identity);
                        }
                        else
                            pet.Attack(target.Identity);
                    }
                }
                attackDelay = Time.AONormalTime + 0.5;
            }
        }

        public void OnStateExit()
        {
            attackDelay = 0;
        }
    }
}