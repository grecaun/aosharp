using System;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public void RegisterSpellProcessors()
        {
            try
            {
                foreach (var spell in Spell.List.OrderByDescending(s => s.StackingOrder))
                {
                    RegisterHealSpells(spell);
                    RegisterTauntSpells(spell.Id);
                    RegisterDebuffRemoverSpells(spell.Id);
                    RegisterHoldsSpells(spell.Id);
                    RegisterDebuffSpells(spell.Id);
                    RegisterCombatSpells(spell.Id);
                    RegisterNukeSpells(spell.Id);
                    RegisterBuffSpells(spell.Id);
                    RegisterPetBuffSpells(spell.Id);
                }

                if (LoadedNonCombatBuffs != null && LoadedNonCombatBuffs.Count > 0 && !_buttonDefinitions.Any(x => x.Label == "Buffs"))
                    _buttonDefinitions.Add(("Buffs", Buffs_Button_Clicked));

                if (LoadedDeBuffs != null && LoadedDeBuffs.Count > 0 && !_buttonDefinitions.Any(x => x.Label == "Debuffs"))
                    _buttonDefinitions.Add(("Debuffs", Debuff_Button_Clicked));

                if (LoadedHeals != null && LoadedHeals.Count > 0 && !_buttonDefinitions.Any(x => x.Label == "Heals"))
                    _buttonDefinitions.Add(("Heals", Heals_Button_Clicked));

                if (LoadedHolds != null && LoadedHolds.Count > 0 && !_buttonDefinitions.Any(x => x.Label == "Holds"))
                    _buttonDefinitions.Add(("Holds", Calms_Button_Clicked));

                if (Spell.List.Any(s => SpellID.Morphs.Contains(s.Id)) && !_buttonDefinitions.Any(x => x.Label == "Morphs"))
                    _buttonDefinitions.Add(("Morphs", HandleMorphViewClick));

                if (LoadedNukeSpells != null && LoadedNukeSpells.Count > 0 && !_buttonDefinitions.Any(x => x.Label == "Nukes"))
                    _buttonDefinitions.Add(("Nukes", Nuke_Buttion_Clicked));

                if (LoadedPetSpells != null && LoadedPetSpells.Any(x => x) && !_buttonDefinitions.Any(x => x.Label == "Pets"))
                    _buttonDefinitions.Add(("Pets", Pet_Button_Clicked));

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Agent:
                    case Profession.Bureaucrat:
                    case Profession.Engineer:
                    case Profession.Metaphysicist:
                        if (!_buttonDefinitions.Any(x => x.Label == "PetComs"))
                            _buttonDefinitions.Add(("PetComs", HandlePetCommandViewClick));
                        break;
                }

                if (LoadedTaunts != null && LoadedTaunts.Count > 0 && !_buttonDefinitions.Any(x => x.Label == "Taunts"))
                    _buttonDefinitions.Add(("Taunts", Taunts_Butoon_Clicked));
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Combat

        protected bool DeTaunt(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["DeTaunt"].AsBool()) return false;
            if (_settings["DeTaunt_If_AMS_False"] != null && _settings["DeTaunt_If_AMS_False"].AsBool())
            {
                if (DynelManager.LocalPlayer.Buffs.Contains(SpellID.UserReflectShieldSoldier)) return false;
            }
            if (!Team.IsInTeam) return false;
            if (!CanCast(spell)) return false;

            actionTarget.Target = null;

            actionTarget.Target = DynelManager.NPCs.FirstOrDefault(npc => npc.IsAttacking && npc.FightingTarget != null && npc.FightingTarget.Identity == DynelManager.LocalPlayer.Identity && InCastRange(spell, npc));

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        #endregion

        #region Weapon Type

        protected bool TeamBuffWeaponCheck(Spell spell, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, params CharacterWieldedWeapon[] supportedCharacterWieldedWeapons)
        {
            try
            {
                if (!CanCast(spell)) return false;
                if (!Team.IsInTeam) return false;

                actionTarget.Target = null;
                //Chat.WriteLine($"{spell.Name}, {spell.Nanoline}");
                //foreach (var we in supportedCharacterWieldedWeapons)
                //    Chat.WriteLine(we);
                actionTarget.Target = Team.Members.Where(t => t.Character != null && t.Character.IsInLineOfSight && t.Character.Health > 0 && InCastRange(spell, t.Character)
                && SpellCheckLocalTeam(spell, t.Character) && supportedCharacterWieldedWeapons.Any(w => GetWieldedWeapons(t.Character).HasFlag(w))).Select(t => t.Character).FirstOrDefault();

                if (actionTarget.Target == null) return false;

                //if (actionTarget.Target.Buffs.Contains(NanoLine.FixerSuppressorBuff) && (spell.Nanoline == NanoLine.FixerSuppressorBuff || spell.Nanoline == NanoLine.AssaultRifleBuffs)) return false;
                //if (actionTarget.Target.Buffs.Contains(NanoLine.PistolBuff) && spell.Nanoline == NanoLine.PistolBuff) return false;
                //if (actionTarget.Target.Buffs.Contains(NanoLine.AssaultRifleBuffs) && (spell.Nanoline == NanoLine.AssaultRifleBuffs || spell.Nanoline == NanoLine.GrenadeBuffs)) return false;

                actionTarget = (actionTarget.Target, true);
                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        #endregion

        #region Spell Checks

        public bool InCastRange(Spell spell, SimpleChar target)
        {
            if (spell == null) return false;
            if (spell.AttackRange == 0) return false;
            if (target == null) return false;
            if (target.Position == null) return false;
            if (!spell.MeetsUseReqs(target)) return false;

            return DynelManager.LocalPlayer.Position.Distance2DFrom(target.Position) <= spell?.AttackRange;
        }

        private bool PolymorphCheck(SimpleChar target, Spell spell)
        {
            try
            {
                if (!target.Buffs.Contains(NanoLine.Polymorph)) return false;

                if ((spell.Nanoline == NanoLine.MajorEvasionBuffs || spell.Nanoline == NanoLine.RunspeedBuffs || spell.Nanoline == NanoLine.PerceptionBuffs || spell.Nanoline == NanoLine.TeamRunSpeedBuffs) &&
                    target.Buffs.Contains(SpellID.Polymorph223_MajorEvasionBuffs144_RunspeedBuffs150_PerceptionBuffs191))
                    return true;

                if ((spell.Nanoline == NanoLine.HPBuff || spell.Nanoline == NanoLine.ArmorBuff) &&
                    target.Buffs.Contains(SpellID.Polymorph223_HPBuff151_ArmorBuff3))
                    return true;

                if (spell.Nanoline == NanoLine.AAOBuffs &&
                    target.Buffs.Contains(SpellID.Polymorph223_AAOBuffs836))
                    return true;

                if ((spell.Nanoline == NanoLine.RunspeedBuffs || spell.Nanoline == NanoLine.ConcealmentBuff || spell.Nanoline == NanoLine.CriticalIncreaseBuff || spell.Nanoline == NanoLine.TeamRunSpeedBuffs) &&
                    target.Buffs.Contains(SpellID.Polymorph223_RunspeedBuffs150_ConcealmentBuff193_CriticalIncreaseBuff182))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        protected bool SpellCheckSelf(Spell spell)
        {
            try
            {
                if (!_settings["Buffing"].AsBool()) return false;

                var localPlayer = DynelManager.LocalPlayer;

                if (PolymorphCheck(localPlayer, spell)) return false;

                switch (spell.Nanoline)
                {
                    case NanoLine.BioMetBuff:
                    case NanoLine.MatMetBuff:
                    case NanoLine.PsyModBuff:
                    case NanoLine.MatLocBuff:
                    case NanoLine.MatCreaBuff:
                        if (localPlayer.Buffs.Contains(SpellID.MPCompositeNano)) return false;
                        break;
                    case NanoLine.GrenadeBuffs:
                        if (localPlayer.Buffs.Contains(SpellID.CompositeHeavyArtillery)) return false;
                        break;
                    case NanoLine.Challenger:
                        if (localPlayer.Buffs.Contains(SpellID.NanoSkillsInoperative)) return false;
                        break;
                    case NanoLine.RunspeedBuffs:
                        if (localPlayer.Buffs.Contains(SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer)) return false;
                        break;
                    case NanoLine.AADBuffs:
                        if (localPlayer.Buffs.Contains(269935)) return false;//Eye of the Predator
                        break;
                    case NanoLine.AAOBuffs:
                        if (localPlayer.Buffs.Contains(269935)) return false;//Eye of the Predator
                        break;
                    case NanoLine.AbsorbACBuff:
                        if (localPlayer.Buffs.Contains(NanoLine.PerkBioCocoon)) return false;
                        break;
                    case NanoLine.SenseBuff:
                        if (localPlayer.Buffs.Contains(SpellID.UserAgilityBuffAgent)) return false;
                        break;
                }

                if (SpellID.SenImpBuffs.Contains(spell.Id))
                {
                    if (DynelManager.LocalPlayer.Buffs.Contains(SpellID.MPCompositeNano)) return false;
                }

                if (SpellID.UserDamageBuffs_LineAAdventurer.Contains(spell.Id))
                {
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Polymorph)) return false;
                }

                if (SpellID.TeamConcealmentBuffAgent.Contains(spell.Id))
                {
                    if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.ConcealmentBuff)) return false;
                }

                if (SpellID.DoctorHPBuffs.Contains(spell.Id))
                {
                    if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.DoctorHPBuffs)) return false;
                }

                Buff ExistingBuff = null;

                if (int.TryParse(spell.Nanoline.ToString(), out int result) || spell.Nanoline == NanoLine.NOSTACKING)
                    ExistingBuff = localPlayer.Buffs.FirstOrDefault(b => b.Name == spell.Name);
                else
                    ExistingBuff = localPlayer.Buffs.FirstOrDefault(b => b.Nanoline == spell.Nanoline);

                if (ExistingBuff != null)
                {
                    if (spell.StackingOrder <= ExistingBuff.StackingOrder) return false;
                    if (spell.StackingOrder == ExistingBuff.StackingOrder && ExistingBuff.RemainingTime > 20f) return false;
                }

                return localPlayer.RemainingNCU >= spell.NCU;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        protected bool SpellCheckLocalTeam(Spell spell, SimpleChar teamMember)
        {
            try
            {
                if (!_settings["Buffing"].AsBool()) return false;
                if (!RemainingNCU.ContainsKey(teamMember.Identity))
                {
                    //Chat.WriteLine($"{teamMember.Name} not found in RemainingNCU");
                    return false;
                }
                if (!teamMember.IsInLineOfSight) return false;
                if (!spell.MeetsUseReqs(teamMember)) return false;

                if (RemainingNCU[teamMember.Identity] <= spell.NCU) return false;

                if (PolymorphCheck(teamMember, spell)) return false;

                if (SpellID.MPCompositeNano.Contains(spell.Id) && teamMember.Buffs.Contains(SpellID.UserMatCreaBuff)) return false;

                if (SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer.Contains(spell.Id) && teamMember.Buffs.Contains(NanoLine.RunspeedBuffs))
                {
                    teamMember.Buffs.Find(NanoLine.RunspeedBuffs, out Buff buff);
                    if (spell.StackingOrder < buff.StackingOrder) return false;
                }

                switch (spell.Nanoline)
                {
                    case NanoLine.NanoResistanceBuffs:
                        if (teamMember.Buffs.Contains(NanoLine.Rage)) return false;
                        break;
                    case NanoLine.StrengthBuff:
                    case NanoLine.AgilityBuff:
                    case NanoLine.SenseBuff:
                        if (teamMember.Buffs.Contains(NanoLine.KeeperStr_Stam_AgiBuff)) return false;
                        break;
                    case NanoLine.RunspeedBuffs:
                        if (teamMember.Buffs.Contains(SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer)) return false;
                        if (teamMember.Buffs.Contains(NanoLine.TeamRunSpeedBuffs)) return false;
                        break;
                    case NanoLine.TeamRunSpeedBuffs:
                        if (teamMember.Buffs.Contains(NanoLine.RunspeedBuffs)) return false;
                        break;
                    case NanoLine.CriticalIncreaseBuff:
                        if (teamMember.Buffs.Contains(SpellID.AAOTransfer)) return false;
                        break;
                    case NanoLine.AADBuffs:
                        if (teamMember.Buffs.Contains(269935)) return false; //Eye of the Predator
                        break;
                    case NanoLine.AAOBuffs:
                        if (teamMember.Buffs.Contains(269935)) return false; //Eye of the Predator
                        break;
                    case NanoLine.AbsorbACBuff:
                        if (teamMember.Buffs.Contains(NanoLine.PerkBioCocoon)) return false;
                        break;
                }

                if (SpellID.FixerNCUBuffs.Contains(spell.Id))
                {
                    int levelReq = 0;
                    switch (spell.Id)
                    {
                        case 275043: //Firewalled Sync Compressor
                            levelReq = 215;
                            break;
                        case 163095: //Sentient Viral Recoder
                            levelReq = 185;
                            break;
                        case 163094: //Active Viral Compressor 
                            levelReq = 165;
                            break;
                        case 163087: //QuarkStor NCU Core
                            levelReq = 135;
                            break;
                        case 163085: //Recompiling Memory Analyzer
                            levelReq = 125;
                            break;
                        case 163083: //Deck Recoder
                            levelReq = 75;
                            break;
                        case 163081: //Jury-rigged NCU Analyzer
                            levelReq = 50;
                            break;
                        case 163079: //Retool NCU
                        case 162995: //NCU Compressor
                            levelReq = 25;
                            break;
                    }

                    if (levelReq != 0)
                    {
                        if (teamMember.Level < levelReq) return false;
                    }
                }

                Buff ExistingBuff = null;

                if (int.TryParse(spell.Nanoline.ToString(), out int result) || spell.Nanoline == NanoLine.NOSTACKING)
                    ExistingBuff = teamMember.Buffs.FirstOrDefault(b => b.Name == spell.Name);
                else
                    ExistingBuff = teamMember.Buffs.FirstOrDefault(b => b.Nanoline == spell.Nanoline);

                if (ExistingBuff != null)
                {
                    if (spell.StackingOrder < ExistingBuff.StackingOrder) return false;
                    if (spell.StackingOrder == ExistingBuff.StackingOrder && ExistingBuff.RemainingTime > 20f) return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        protected bool SpellCheckFightingTarget(Spell spell, SimpleChar fightingTarget)
        {
            try
            {
                if (fightingTarget == null) return false;
                if (spell.Nanoline == NanoLine.GeneralPsychicDebuff) return false;

                Buff ExistingBuff = null;

                if (int.TryParse(spell.Nanoline.ToString(), out int result) || spell.Nanoline == NanoLine.NOSTACKING)
                    ExistingBuff = fightingTarget.Buffs.FirstOrDefault(b => b.Name == spell.Name);
                else
                    ExistingBuff = fightingTarget.Buffs.FirstOrDefault(b => b.Nanoline == spell.Nanoline);

                if (ExistingBuff != null)
                {
                    if (spell.StackingOrder < ExistingBuff.StackingOrder) return false;
                    if (spell.StackingOrder == ExistingBuff.StackingOrder && ExistingBuff.RemainingTime > 5f) return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        protected bool CanCast(Spell spell)
        {
            try
            {
                if (Game.IsZoning) return false;
                if (Now < _lastZonedTime) return false;
                if (Spell.HasPendingCast) return false;
                if (spell == null) return false;
                if (!spell.IsReady) return false;
                if (!spell.MeetsSelfUseReqs()) return false;
                if (Playfield.ModelIdentity.Instance == 152) return false;
                if (IsPlayerFlyingOrFalling()) return false;
                if (DynelManager.LocalPlayer.IsMoving) return false;
                if (!DynelManager.LocalPlayer.MovementStatePermitsCasting) return false;

                if (_settings["WaitForRez"].AsBool())
                {
                    if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) > 1)
                        return false;
                }

                if (DynelManager.LocalPlayer.Buffs.Contains(SpellID.InnerSanctumDebuff))
                {
                    switch (spell.Nanoline)
                    {
                        case NanoLine.RunspeedBuffs:
                        case NanoLine.FixerRunspeedBase:
                        case NanoLine.GeneralRunspeedBuffs:
                        case NanoLine.TeamRunSpeedBuffs:
                            return false;
                    }

                    if (SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer.Contains(spell.Id))
                        return false;
                }

                return spell.Cost < DynelManager.LocalPlayer.Nano;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        #endregion
    }
}
