using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public static HashSet<int> LoadedDebuffRemoverSpells = new HashSet<int>();

        private void RegisterDebuffRemoverSpells(int spellID)
        {
            try
            {
                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Adventurer:
                        break;

                    case Profession.Agent:
                        break;

                    case Profession.Bureaucrat:
                        if (SpellID._CorporateLeadership.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, RootReducer, CombatActionPriority.High, RuleContext.Both);
                        }

                        if (SpellID.CratOtherRootReduction.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }
                        if (SpellID.CratOtherSnareReduction.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }
                        if (SpellID.CratSelfRootReduction.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }
                        if (SpellID.CratSelfSnareReduction.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }
                        if (SpellID.CratPetReduction.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }
                        break;

                    case Profession.Doctor:

                        if (SpellID._EpsilonPurge == spellID)
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID._EpsilonPurge, EpsilonPurge, CombatActionPriority.High, RuleContext.Both);
                        }
                        break;

                    case Profession.Enforcer:
                        if (Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).Any(s => s.Id == spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).OrderByStackingOrder(), BreakRoot, CombatActionPriority.High, RuleContext.Both);
                        }
                        break;

                    case Profession.Engineer:
                        break;

                    case Profession.Fixer:

                        if (SpellID._WakeUpCall == spellID)
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID._WakeUpCall, WakeUpCall, CombatActionPriority.High, RuleContext.Both);
                        }

                        if (SpellID._RefactorNCUMatrix == spellID)
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID._RefactorNCUMatrix, RefactorNCUMatrix, CombatActionPriority.High, RuleContext.Both);
                        }

                        if (SpellID.FixerSelfRootRemoval.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }
                        if (SpellID.FixerSelfSnareRemoval.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }
                        if (SpellID.FixerTargetSnareReduction.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }

                        break;

                    case Profession.Keeper:
                        if (Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).Any(s => s.Id == spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).OrderByStackingOrder(), BreakRoot, CombatActionPriority.High, RuleContext.Both);
                        }
                        break;

                    case Profession.MartialArtist:
                        break;

                    case Profession.Metaphysicist:
                        break;

                    case Profession.NanoTechnician:

                        if (Spell.GetSpellsForNanoline(NanoLine.DeTaunt).Any(s => s.Id == spellID))
                        {
                            int highestDeTaunt = Spell.GetSpellsForNanoline(NanoLine.DeTaunt).OrderByStackingOrder().GetIds().FirstOrDefault(id => Spell.List.Any(x => x.Id == id));
                            RegisterSpellProcessor(highestDeTaunt, DeTaunt, CombatActionPriority.High);
                        }

                        break;

                    case Profession.Shade:
                        if (Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).Any(s => s.Id == spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).OrderByStackingOrder(), BreakRoot, CombatActionPriority.High, RuleContext.Both);
                        }
                        break;

                    case Profession.Soldier:
                        if (Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).Any(s => s.Id == spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).OrderByStackingOrder(), BreakRoot, CombatActionPriority.High, RuleContext.Both);
                        }

                        if (SpellID.DeTaunt.Contains(spellID))
                        {
                            int highestDeTaunt = SpellID.DeTaunt.FirstOrDefault(id => Spell.List.Any(s => s.Id == id));
                            RegisterSpellProcessor(highestDeTaunt, DeTaunt, CombatActionPriority.High);
                        }

                        break;

                    case Profession.Trader:

                        if (SpellID.TraderSelfRootDurationReduction.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }
                        if (SpellID.TraderTargetRootDurationReduction.Contains(spellID))
                        {
                            LoadedDebuffRemoverSpells.Add(spellID);
                            RegisterSpellProcessor(spellID, HoldReduction, CombatActionPriority.High, RuleContext.Both);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        protected bool BreakRoot(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!_settings["ReleaseMeNow"].AsBool()) return false;
                if (!CanCast(spell)) return false;

                return DynelManager.LocalPlayer.Buffs.Contains(new NanoLine[] { NanoLine.Root, NanoLine.Snare });
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        private bool HoldReduction(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!CanCast(spell)) return false;

                switch (spell.Id)
                {
                    // Target Root
                    case 203956:
                    case 203954:
                    case 203952:
                    case 203950:
                    case 203948: // CratOtherRootReduction
                    case 203970:
                    case 203968:
                    case 203966: // TraderTargetRootDurationReduction
                        if (Team.IsInTeam)
                        {
                            var member = Team.Members.Where(m=>m.Character !=null && m.Character.Buffs.Contains(NanoLine.Root)).FirstOrDefault()?.Character;
                            if (member != null)
                            {
                                actionTarget = (member, true);
                                return true;
                            }
                        }
                        return false;

                    // Target Snare
                    case 203964:
                    case 203962:
                    case 203960:
                    case 203958: // CratOtherSnareReduction
                    case 203984:
                    case 203982:
                    case 203980:
                    case 203978:
                    case 203976: // FixerTargetSnareReduction
                        if (Team.IsInTeam)
                        {
                            var member = Team.Members.Where(m => m.Character != null && m.Character.Buffs.Contains(NanoLine.Snare)).FirstOrDefault()?.Character;
                            if (member != null)
                            {
                                actionTarget = (member, true);
                                return true;
                            }
                        }
                        return false;

                    // Self Root
                    case 203846:
                    case 203844:
                    case 203842:
                    case 203839:
                    case 203837:
                    case 203835:
                    case 203831: // CratSelfRootReduction
                    case 203813:
                    case 203811: // FixerSelfRootRemoval
                    case 203792:
                    case 203790:
                    case 203788:
                    case 203786:
                    case 203784: // TraderSelfRootDurationReduction
                        if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Root))
                        {
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                        return false;

                    // Self Snare
                    case 203665:
                    case 203663:
                    case 203661:
                    case 203659:
                    case 203657: // CratSelfSnareReduction
                    case 203603:
                    case 203601:
                    case 203599:
                    case 203595:
                    case 203597:
                    case 203593: // FixerSelfSnareRemoval
                        if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Snare))
                        {
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                        return false;

                    // Pet
                    case 203859:
                    case 203857:
                    case 203855:
                    case 203852:
                    case 203850: // CratPetReduction
                        var pet = DynelManager.LocalPlayer.Pets.Where(p =>p.Character !=null && p.Character.Buffs.Contains(new [] { NanoLine.Root, NanoLine.Snare })).FirstOrDefault()?.Character;
                        if (pet != null)
                        {
                            actionTarget = (pet, true);
                            return true;
                        }

                        return false;

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        #region Adventurer

        #endregion

        #region Agent

        #endregion

        #region Bureaucrat

        private bool RootReducer(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Exoneration"].AsBool()) return false;
            if (!CanCast(spell)) return false;

            if (Team.IsInTeam)
            {
                SimpleChar target = null;

                target = Team.Members.Find(t => t.Character != null && t.Character.IsInLineOfSight && t.Character.IsAlive && InCastRange(spell, t.Character)
                && t.Character.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Snare, (NanoLine)305244, (NanoLine)268174, (NanoLine)82166 }))?.Character;

                if (target == null)
                    target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Character != null && p.Character.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Snare, (NanoLine)305244, (NanoLine)268174, (NanoLine)82166 }))?.Character;

                if (target != null) { actionTarget = (DynelManager.LocalPlayer, true); return true; }
            }
            else if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Snare, (NanoLine)305244, (NanoLine)268174, (NanoLine)82166 }))
            {
                actionTarget = (DynelManager.LocalPlayer, true); return true;
            }
            else if (DynelManager.LocalPlayer.Pets.Any(p => p.Character != null && p.Character.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Snare, (NanoLine)305244, (NanoLine)268174, (NanoLine)82166 })))
            {
                actionTarget = (DynelManager.LocalPlayer, true); return true;
            }

            return false;
        }

        #endregion

        #region Doctor

        private bool EpsilonPurge(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["EpsilonPurge"].AsBool()) return false;
            if (!CanCast(spell)) return false;

            if (Team.IsInTeam)
            {
                var target = Team.Members.Find(p => p.Character != null && p.Character.IsInLineOfSight && InCastRange(spell, p.Character) &&
                p.Character.Buffs.Contains(new[] { NanoLine.DOT_LineA, NanoLine.DOT_LineB, NanoLine.DOTAgentStrainA, NanoLine.DOTNanotechnicianStrainA, NanoLine.DOTNanotechnicianStrainB, NanoLine.DOTStrainC }))?.Character;

                if (target != null) { actionTarget = (target, true); return true; }
            }
            else if (DynelManager.LocalPlayer.Buffs.Contains(new[]
            { NanoLine.DOT_LineA, NanoLine.DOT_LineB, NanoLine.DOTAgentStrainA, NanoLine.DOTNanotechnicianStrainA, NanoLine.DOTNanotechnicianStrainB, NanoLine.DOTStrainC }))
            {
                actionTarget = (DynelManager.LocalPlayer, true); return true;
            }

            return false;
        }

        #endregion

        #region Enforcer

        #endregion

        #region Engineer

        #endregion

        #region Fixer

        private bool WakeUpCall(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanCast(spell)) return false;

            if (Team.IsInTeam)
            {
                var target = Team.Members.Find(c => c.Character != null && c.Character.IsInLineOfSight && InCastRange(spell, c.Character)
                && c.Character.Buffs.Contains(new[] { NanoLine.DOT_LineA, NanoLine.DOT_LineB, NanoLine.DOTNanotechnicianStrainA, NanoLine.DOTAgentStrainA, NanoLine.DOTAgentStrainA, NanoLine.DOTNanotechnicianStrainB,
                NanoLine.Mezz, NanoLine.InitiativeDebuffs, NanoLine.NanoDrain_LineB, NanoLine.Fear_PVP, NanoLine.UnremovableSnare, NanoLine.UnremovableSnare, (NanoLine)960 }))?.Character;
                if (target != null) { actionTarget = (target, true); return true; }
            }
            else if (DynelManager.LocalPlayer.Buffs.Contains(new[]
            { NanoLine.DOT_LineA, NanoLine.DOT_LineB, NanoLine.DOTNanotechnicianStrainA, NanoLine.DOTAgentStrainA, NanoLine.DOTAgentStrainA, NanoLine.DOTNanotechnicianStrainB,
                NanoLine.Mezz, NanoLine.InitiativeDebuffs, NanoLine.NanoDrain_LineB, NanoLine.Fear_PVP, NanoLine.UnremovableSnare, NanoLine.UnremovableSnare, (NanoLine)960 }))
            {
                actionTarget = (DynelManager.LocalPlayer, true); return true;
            }

            return false;
        }

        private bool RefactorNCUMatrix(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanCast(spell)) return false;

            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.DOT_LineA, NanoLine.DOT_LineB, NanoLine.DOTNanotechnicianStrainA, NanoLine.DOTAgentStrainA, NanoLine.DOTNanotechnicianStrainB,
                NanoLine.Snare, NanoLine.Root, NanoLine.Mezz, NanoLine.InitiativeDebuffs, NanoLine.DOTStrainC, NanoLine.NanoDrain_LineB, NanoLine.Fear_PVP, NanoLine.UnremovableSnare }))
            {
                actionTarget.ShouldSetTarget = false;
                return true;
            }

            return false;
        }

        #endregion

        #region Keeper

        #endregion

        #region MartialArtist

        #endregion

        #region Metaphysicist

        #endregion

        #region NanoTechnician

        #endregion

        #region Shade

        #endregion

        #region Soldier

        #endregion

        #region Trader

        #endregion
    }
}
