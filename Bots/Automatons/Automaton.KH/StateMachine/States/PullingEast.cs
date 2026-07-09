using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AutomatonKiteHill.IPCMessages;
using Shared;

namespace AutomatonKiteHill
{
    public class PullEast : IState
    {
        private bool _castedMongo = false;
        private double _LastCastTimeStamp = 0;

        public void OnStateEnter()
        {
            AutomatonKiteHill.PullStartIndex = 13;
            AutomatonKiteHill.vectorList = Constants.KHEastVectorList;
            AutomatonKiteHill._counterVec = 0;
            _castedMongo = false;
            _LastCastTimeStamp = 0;

            Chat.WriteLine("Pulling East");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning || Playfield.ModelIdentity.Instance != Constants.ElyID) return AutomatonKiteHill.Idle;

            switch (DynelManager.LocalPlayer.Profession)
            {
                case Profession.NanoTechnician:
                    var _hecks = DynelManager.NPCs.FirstOrDefault(x => x.Health > 0 && (x.Name.Contains("Heckler") || x.Name.Contains("Voracious"))
                    && x.IsInLineOfSight && x.IsAttacking && x.FightingTarget.Identity == AutomatonKiteHill.Tank);

                    if (_hecks == null) return null;
                    return AutomatonKiteHill.Nuke_State;
                case Profession.Enforcer:
                    if (AutomatonKiteHill._counterVec >= AutomatonKiteHill.vectorList.Count - 1)
                    {
                        AutomatonKiteHill.IPCChannel.Broadcast(new MoveEastMessage());
                        return AutomatonKiteHill.Nuke_State;
                    }
                    break;
            }

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) return;
            if (Playfield.ModelIdentity.Instance != Constants.ElyID) return;
            if (DynelManager.LocalPlayer.Profession != Profession.Enforcer) return;
            if (Time.AONormalTime <= AutomatonKiteHill._westTimer) return;

            AutomatonKiteHill.SetMongoBasedOnHealth();

            if (MovementController.Instance.IsNavigating) return;

            if (AutomatonKiteHill._counterVec <= AutomatonKiteHill.PullStartIndex)
            {
                if (Spell.HasPendingCast) return;

                if (AutomatonKiteHill._counterVec < AutomatonKiteHill.vectorList.Count - 1)
                {
                    AutomatonKiteHill._counterVec++;
                    MovementController.Instance.SetMovement(MovementAction.Update);
                    MovementController.Instance.SetDestination(AutomatonKiteHill.vectorList[AutomatonKiteHill._counterVec]);
                    _LastCastTimeStamp = Time.AONormalTime;
                    return;
                }
            }
            else
            {
                if (AutomatonKiteHill._counterVec < AutomatonKiteHill.PullStartIndex + 1) return;

                if (AutomatonKiteHill.mongo == null) return;

                if (!_castedMongo)
                {
                    if (Time.AONormalTime < _LastCastTimeStamp + AutomatonKiteHill._settings["Time"].AsFloat())
                        return;

                    if (DynelManager.LocalPlayer.IsMoving || !AutomatonKiteHill.mongo.IsReady || Spell.HasPendingCast)
                        return;

                    AutomatonKiteHill.mongo.Cast();
                    _castedMongo = true;
                    _LastCastTimeStamp = Time.AONormalTime;
                    return;
                }

                if (Time.AONormalTime < _LastCastTimeStamp + AutomatonKiteHill._settings["Time"].AsFloat())
                    return;

                if (!AutomatonKiteHill.mongo.IsReady && !Spell.HasPendingCast)
                {
                    var _distantHeck = DynelManager.NPCs.FirstOrDefault(x =>
                        x.Health > 0 &&
                        (x.Name.Contains("Heckler") || x.Name.Contains("Voracious")) &&
                        x.DistanceFrom(DynelManager.LocalPlayer) > 20f &&
                        x.IsInLineOfSight &&
                        !x.IsAttacking);

                    if (_distantHeck != null)
                        TauntingTools.HandleTaunting(_distantHeck);
                }

                if (AutomatonKiteHill._counterVec < AutomatonKiteHill.vectorList.Count - 1)
                    AutomatonKiteHill._counterVec++;

                MovementController.Instance.SetMovement(MovementAction.JumpStart);
                MovementController.Instance.SetDestination(AutomatonKiteHill.vectorList[AutomatonKiteHill._counterVec]);
                _castedMongo = false;
                _LastCastTimeStamp = Time.AONormalTime;
            }
        }

        public void OnStateExit()
        {
            AutomatonKiteHill.PullStartIndex = 0;
            AutomatonKiteHill.vectorList = null;
            AutomatonKiteHill._counterVec = 0;
            _castedMongo = false;
            _LastCastTimeStamp = 0;

            Chat.WriteLine("Exit pull state");
        }
    }
}
