using System;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.Unmanaged.Imports;
using AOSharp.Core;
using Shared;

namespace ManagerAttack
{
    public partial class ManagerAttack
    {
        AttackState State;

        internal void AttackBase()
        {
            try
            {
                Scanning();

                var localPlayer = DynelManager.LocalPlayer;

                var leader = DynelManager.Players.FirstOrDefault(c => c.Health > 0 && c.Identity == Leader && c.DistanceFrom(localPlayer) < _settings["AttackRange"].AsInt32()
                && !c.Buffs.Contains(new[] { 280470, 257127, 260301, 291135, 280813, 302676 }) && !c.Buffs.Contains(new[] { NanoLine.Stun, NanoLine.Fear, NanoLine.MindControl }));

                SimpleChar helperTarget = null;
                SimpleChar attackingTarget = null;

                if (_helpers.Count > 0)
                {
                    var helper = DynelManager.Characters.FirstOrDefault(p => _helpers.Contains(p.Name, StringComparer.OrdinalIgnoreCase));

                    if (helper?.FightingTarget != null && helper.FightingTarget.Position.Distance2DFrom(localPlayer.Position) <= _settings["AttackRange"].AsInt32())
                        helperTarget = helper.FightingTarget;
                }

                if (_settings["Switching"].AsBool())
                    attackingTarget = DynelManager.NPCs.FirstOrDefault(a => a.Health > 0 && !a.Buffs.Contains(256507) && !_bossMob.Any(b => b.Identity == a.Identity) &&
                    (_mob.Any(m => m.Identity == a.Identity) || _switchMob.Any(s => s.Identity == a.Identity)) && a.IsAttacking && !a.IsPet &&
                    Team.Members.Any(t => t.Identity == a.FightingTarget?.Identity) && a.FightingTarget?.Identity != Leader);

                var switchTarget = _switchMob.FirstOrDefault(c => c.Position.Distance2DFrom(localPlayer.Position) <= _settings["AttackRange"].AsInt32());

                var mob = _mob.FirstOrDefault(c => c.Position.Distance2DFrom(localPlayer.Position) <= _settings["AttackRange"].AsInt32());

                var boss = _bossMob.FirstOrDefault(c => c.Position.Distance2DFrom(localPlayer.Position) <= _settings["AttackRange"].AsInt32());

                if (localPlayer.Identity == Leader || leader == null)
                {
                    var candidates = new[] { helperTarget, attackingTarget, switchTarget, mob, boss };
                    var targetToAttack = candidates.FirstOrDefault(t => t != null && ValidTarget(t));

                    if (targetToAttack != null)
                        HandleAttack(targetToAttack);
                    else if (_settings["Taunt"].AsBool())
                    {
                        if (DynelManager.LocalPlayer.IsMoving) return;
                        if (DynelManager.LocalPlayer.IsAttacking) return;
                        if (DynelManager.LocalPlayer.IsAttackPending) return;
                        if (localPlayer.HealthPercent < _settings["KitHealthPercentageBox"].AsInt32() || localPlayer.NanoPercent < _settings["KitNanoPercentageBox"].AsInt32()) return;

                        var outOfRangeMob = _switchMob.Where(c => c.Health > 0 && c.IsInLineOfSight && c.Position.Distance2DFrom(localPlayer.Position) > _settings["AttackRange"].AsInt32() 
                        && c.Position.Distance2DFrom(localPlayer.Position) <= _settings["TauntRange"].AsInt32()).OrderBy(c => c.Position.Distance2DFrom(localPlayer.Position)).FirstOrDefault() 
                        ?? _mob.Where(c => c.Health > 0 && c.IsInLineOfSight && c.Position.Distance2DFrom(localPlayer.Position) > _settings["AttackRange"].AsInt32() 
                        && c.Position.Distance2DFrom(localPlayer.Position) <= _settings["TauntRange"].AsInt32()).OrderBy(c => c.Position.Distance2DFrom(localPlayer.Position)).FirstOrDefault();

                        if (outOfRangeMob != null)
                            TauntingTools.HandleTaunting(outOfRangeMob);
                    }
                    else if (localPlayer.IsAttacking)
                    {
                        if (State != AttackState.Stop)
                        {
                            StopAttacking();
                            State = AttackState.Stop;
                        }
                    }
                }
                else
                {
                    if (leader?.IsAttacking == true)
                        HandleAttack(leader?.FightingTarget);
                }
            }
            catch (Exception ex)
            {
               ErrorCatch(ex);
            }
        }
        enum AttackState { Waiting, Attack, Stop }
        bool ValidTarget(SimpleChar target)
        {
            return !target.Buffs.Contains(253953) &&
                    !target.Buffs.Contains(NanoLine.ShovelBuffs) &&
                    !target.IsPlayer && !target.IsPet && target.IsInLineOfSight &&
                    !target.Buffs.Contains(NanoLine.CharmOther) &&
                    !target.Buffs.Contains(NanoLine.Charm_Short);
        }

        void HandleAttack(SimpleChar target)
        {
            var localPlayer = DynelManager.LocalPlayer;
            if (target == null) { StopAttacking(); return; }
            var inRange = target?.Position.Distance2DFrom(localPlayer.Position) <= _settings["AttackRange"].AsInt32();

            if (localPlayer.IsAttacking == true)
            {
                if (ShouldStopAttack(target) || localPlayer.FightingTarget?.Identity != target?.Identity)
                {
                    if (State == AttackState.Stop) { return; }
                    StopAttacking();
                    State = AttackState.Stop;
                }
            }
            else if (inRange && target.IsInLineOfSight)
            {
                if (localPlayer.FightingTarget == null && localPlayer.IsAttackPending == false)
                {
                    Targeting.SetTarget(target);
                    localPlayer.Attack(target, false);
                    State = AttackState.Attack;
                }
            }
        }

        bool ShouldStopAttack(SimpleChar target)
        {
            var localPlayerBuffs = DynelManager.LocalPlayer?.Buffs;

            if (target == null) return false;

            //Name: Khalum, Id int: 1786491342, Identity: (SimpleChar: 6A7BB1CE), type: SimpleChar
            //205633 Other 205633
            //205607 Immortal 205607
            //205611 Soul of Khalum Pulser 205611

            return target.Buffs.Contains(253953) || //target.Buffs.Contains(205607) ||
                   localPlayerBuffs.Contains(305862) || localPlayerBuffs.Contains(281114) ||
                   DynelManager.LocalPlayer.Position.Distance2DFrom(target.Position) > _settings["AttackRange"].AsInt32() ||
                   !target.IsInLineOfSight || target.IsPet;
        }

        void StopAttacking()
        {
            IntPtr instance = N3Engine_t.GetInstance();
            if (!(instance == IntPtr.Zero))
            {
                N3EngineClientAnarchy_t.StopAttack(instance);
            }
        }
    }
}
