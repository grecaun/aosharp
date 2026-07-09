
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using Shared;

namespace AutomatonKiteHill
{
    public class NukeState : IState
    {
        private int[] sides = { 0, 2, 1, 3 };

        private List<Vector3> positions = new List<Vector3>
        {
            new Vector3(901.9f, 4.4f, 299.6f), //Beach
            new Vector3(1043.2f, 1.6f, 1020.5f), //West
            new Vector3(1115.9f, 1.6f, 1064.3f) //East
        };

        private double _refreshMongoTimer;

        public void OnStateEnter()
        {
            AutomatonKiteHill._stateTimeOut = 0;
            switch (AutomatonKiteHill._settings["SideSelection"].AsInt32())
            {
                case 0://Beach
                    AutomatonKiteHill._beachTimer = 0;
                    break;
                case 1://East
                    AutomatonKiteHill._eastTimer = 0;
                    break;
                case 2://West
                    AutomatonKiteHill._westTimer = 0;
                    break;
            }

            Chat.WriteLine("Nuke state");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning || Playfield.ModelIdentity.Instance != Constants.ElyID) return AutomatonKiteHill.Idle;

            var tank = DynelManager.Players.FirstOrDefault(t => t.Identity == AutomatonKiteHill.Tank && t.Health > 0);

            if (tank == null) return null;

            if (AutomatonKiteHill._stateTimeOut == 0)
                AutomatonKiteHill._stateTimeOut = Time.AONormalTime + 120;

            if (Time.AONormalTime < AutomatonKiteHill._stateTimeOut) return null;

            var _hecks = DynelManager.NPCs.Any(x => x.Health > 0 && (x.Name.Contains("Heckler") || x.Name.Contains("Voracious"))
                    && x.IsAttacking && x.FightingTarget.Identity == AutomatonKiteHill.Tank);

            if (_hecks) return null;

            return AutomatonKiteHill.Idle;
        }

        public void Tick()
        {
            if (Game.IsZoning) return;
            if (Playfield.ModelIdentity.Instance != Constants.ElyID) return;

            if (DynelManager.Players.Any(p => p.Buffs.Contains(AutomatonKiteHill.PVPEnabled)))
            {
                if (DynelManager.LocalPlayer.IsAttacking)
                    DynelManager.LocalPlayer.StopAttack(false);
                return;
            }

            switch (DynelManager.LocalPlayer.Profession)
            {
                case Profession.NanoTechnician:

                    float[] distances = { 10f, 43f, 60f, 60f };

                    for (int i = 0; i < sides.Length; i++)
                    {
                        if (sides[i] == AutomatonKiteHill._settings["SideSelection"].AsInt32() || (i > 0 && sides[3] == AutomatonKiteHill._settings["SideSelection"].AsInt32()))
                        {
                            var _hecksInPos = DynelManager.NPCs
                                .Where(x => (x.Name.Contains("Heckler") || x.Name.Contains("Voracious"))
                                    && x.DistanceFrom(DynelManager.LocalPlayer) <= distances[i]
                                    && x.IsAlive && x.IsInLineOfSight && x.IsAttacking
                                    && !x.IsMoving
                                    && x.FightingTarget.Identity != DynelManager.LocalPlayer.Identity
                                    && x.Position.DistanceFrom(positions[i]) < 10f)
                                .ToList();

                            if (_hecksInPos.Count >= 1 && DynelManager.LocalPlayer.FightingTarget == null
                                && (DynelManager.LocalPlayer.NanoPercent >= AutomatonKiteHill._settings["KitNanoPercentageBox"].AsInt32()
                                || DynelManager.LocalPlayer.HealthPercent >= AutomatonKiteHill._settings["KitHealthPercentageBox"].AsInt32()))
                            {
                                if (DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttacking && !DynelManager.LocalPlayer.IsAttackPending)
                                    DynelManager.LocalPlayer.Attack(_hecksInPos.FirstOrDefault(), false);
                            }

                            if (DynelManager.LocalPlayer.NanoPercent < AutomatonKiteHill._settings["KitNanoPercentageBox"].AsInt32()
                                || DynelManager.LocalPlayer.HealthPercent < AutomatonKiteHill._settings["KitHealthPercentageBox"].AsInt32())
                                DynelManager.LocalPlayer.StopAttack(false);
                        }
                    }
                    break;
                case Profession.Enforcer:

                    var Hecks = DynelManager.NPCs.Where(x => x.Health > 0 && (x.Name.Contains("Heckler") || x.Name.Contains("Voracious"))).ToList();

                    var _hecksWithinMongo = Hecks.Where(x => x.DistanceFrom(DynelManager.LocalPlayer) <= 20f
                    && x.IsInLineOfSight && !x.IsMoving && x.FightingTarget != null).ToList();

                    var _distantHeck = Hecks.FirstOrDefault(x => x.DistanceFrom(DynelManager.LocalPlayer) > 20f && x.IsInLineOfSight
                    && (!x.IsAttacking || x.FightingTarget.Identity != DynelManager.LocalPlayer.Identity));

                    if (_distantHeck != null)
                        TauntingTools.HandleTaunting(_distantHeck);

                    if (_hecksWithinMongo.Count >= 1)
                    {
                        var mongo = Spell.GetSpellsForNanoline(NanoLine.MongoBuff).OrderByStackingOrder().FirstOrDefault();

                        if (!Spell.HasPendingCast && mongo.IsReady && Time.AONormalTime > _refreshMongoTimer + 8)
                        {
                            mongo.Cast();
                            _refreshMongoTimer = Time.AONormalTime;
                        }
                    }

                    break;
            }
        }

        public void OnStateExit()
        {
            AutomatonKiteHill._stateTimeOut = 0;

            switch (AutomatonKiteHill._settings["SideSelection"].AsInt32())
            {
                case 0://Beach
                    AutomatonKiteHill._beachTimer = Time.AONormalTime + 580.0;
                    break;
                case 1://East
                    AutomatonKiteHill._eastTimer = Time.AONormalTime + 580.0;
                    break;
                case 2://West
                    AutomatonKiteHill._westTimer = Time.AONormalTime + 580.0;
                    break;
            }

            AutomatonKiteHill.Counter++;
            Chat.WriteLine("Exit nuke state");
        }
    }
}
