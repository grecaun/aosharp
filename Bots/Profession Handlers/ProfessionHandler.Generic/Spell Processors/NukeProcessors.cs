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
        public static HashSet<int> LoadedNukeSpells = new HashSet<int>();

        public static HashSet<int> LoadedNukeBureaucrat = new HashSet<int>();
        public static HashSet<int> LoadedNukeDoctor = new HashSet<int>();
        public static HashSet<int> LoadedNukeMetaphysicist = new HashSet<int>();
        public static HashSet<int> LoadedNukeNanoTechnician = new HashSet<int>();

        private void RegisterNukeSpells(int spellID)
        {
            try
            {
                if (DynelManager.LocalPlayer.Profession == Profession.Agent)
                {
                    if (Spell.GetSpellsForNanoline(NanoLine.DOTAgentStrainA).Any(s => s.Id == spellID))
                    {
                        LoadedNukeSpells.Add(spellID);

                        RegisterSpellProcessor(_settings["DOTAgentStrainA"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        Dots(debuffSpell, fightingTarget, ref actionTarget, "DOTAgentStrainAOption"), (CombatActionPriority)_settings["DOTAgentStrainAPriority"].AsInt32(), RuleContext.InCombat);
                    }

                    if (SpellID.CratSpecialNuke.Contains(spellID) && !LoadedCombatBureaucratBuffs.Contains(spellID))
                        LoadedCombatBureaucratBuffs.Add(spellID);

                    if (SpellID.CratNormalNuke.Contains(spellID) && !LoadedCombatBureaucratBuffs.Contains(spellID))
                        LoadedCombatBureaucratBuffs.Add(spellID);

                    if (SpellID.DoctorCombatNukes.Contains(spellID) && !LoadedCombatDoctorBuffs.Contains(spellID))
                        LoadedCombatDoctorBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.DOT_LineA).Any(s => s.Id == spellID) && !LoadedCombatDoctorBuffs.Contains(spellID))
                        LoadedCombatDoctorBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.DOT_LineB).Any(s => s.Id == spellID) && !LoadedCombatDoctorBuffs.Contains(spellID))
                        LoadedCombatDoctorBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.DOTStrainC).Any(s => s.Id == spellID) && !LoadedCombatDoctorBuffs.Contains(spellID))
                        LoadedCombatDoctorBuffs.Add(spellID);

                    if (SpellID.MPMindDamage.Contains(spellID) && !LoadedCombatMetaphysicistBuffs.Contains(spellID))
                        LoadedCombatMetaphysicistBuffs.Add(spellID);

                    if (SpellID.MPNormalNuke.Contains(spellID) && !LoadedCombatMetaphysicistBuffs.Contains(spellID))
                        LoadedCombatMetaphysicistBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.DOTNanotechnicianStrainA).Any(s => s.Id == spellID) && !LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                        LoadedCombatNanoTechnicianBuffs.Add(spellID);

                    if (SpellID.NTNukes.Contains(spellID) && !LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                        LoadedCombatNanoTechnicianBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.DOTNanotechnicianStrainB).Any(s => s.Id == spellID) && !LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                        LoadedCombatNanoTechnicianBuffs.Add(spellID);

                    if (SpellID.NTRKAOENukes.Contains(spellID) && !LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                        LoadedCombatNanoTechnicianBuffs.Add(spellID);

                    if (SpellID.NTSLAOENukes.Contains(spellID) && !LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                        LoadedCombatNanoTechnicianBuffs.Add(spellID);
                }

                switch ((Profession)DynelManager.LocalPlayer.GetStat(Stat.VisualProfession))
                {
                    case Profession.Bureaucrat:
                        if (SpellID.CratSpecialNuke.Contains(spellID))
                        {
                            if (!LoadedNukeBureaucrat.Contains(spellID))
                                LoadedNukeBureaucrat.Add(spellID);

                            RegisterSpellProcessor(_settings["SpecialNuke"].AsInt32(), WorkplaceDepressionTargetDebuff, (CombatActionPriority)_settings["SpecialNukePriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (SpellID.CratNormalNuke.Contains(spellID))
                        {
                            if (!LoadedNukeBureaucrat.Contains(spellID))
                                LoadedNukeBureaucrat.Add(spellID);

                            RegisterSpellProcessor(_settings["NormalNuke"].AsInt32(), CratSingleTargetNuke, (CombatActionPriority)_settings["NormalNukePriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Bureaucrat)
                            LoadedNukeSpells.UnionWith(LoadedNukeBureaucrat);

                        break;

                    case Profession.Doctor:
                        if (SpellID.DoctorCombatNukes.Contains(spellID))
                        {
                            if (!LoadedNukeDoctor.Contains(spellID))
                                LoadedNukeDoctor.Add(spellID);

                            RegisterSpellProcessor(_settings["Nuke"].AsInt32(), DocSingleTargetNuke, (CombatActionPriority)_settings["NukePriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.DOT_LineA).Any(s => s.Id == spellID))
                        {
                            if (!LoadedNukeDoctor.Contains(spellID))
                                LoadedNukeDoctor.Add(spellID);

                            RegisterSpellProcessor(_settings["DOT_LineA"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            Dots(debuffSpell, fightingTarget, ref actionTarget, "DOT_LineAOption"), (CombatActionPriority)_settings["DOT_LineAPriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.DOT_LineB).Any(s => s.Id == spellID))
                        {
                            if (!LoadedNukeDoctor.Contains(spellID))
                                LoadedNukeDoctor.Add(spellID);

                            RegisterSpellProcessor(_settings["DOT_LineB"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            Dots(debuffSpell, fightingTarget, ref actionTarget, "DOT_LineBOption"), (CombatActionPriority)_settings["DOT_LineBPriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.DOTStrainC).Any(s => s.Id == spellID))
                        {
                            if (!LoadedNukeDoctor.Contains(spellID))
                                LoadedNukeDoctor.Add(spellID);

                            RegisterSpellProcessor(_settings["DOTStrainC"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            Dots(debuffSpell, fightingTarget, ref actionTarget, "DOTStrainCOption"), (CombatActionPriority)_settings["DOTStrainCPriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Doctor)
                            LoadedNukeSpells.UnionWith(LoadedNukeDoctor);

                        break;

                    case Profession.Metaphysicist:
                        if (SpellID.MPMindDamage.Contains(spellID))
                        {
                            if (!LoadedNukeMetaphysicist.Contains(spellID))
                                LoadedNukeMetaphysicist.Add(spellID);

                            RegisterSpellProcessor(_settings["MindDamage"].AsInt32(), WarmUpNuke, (CombatActionPriority)_settings["MindDamagePriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (SpellID.MPNormalNuke.Contains(spellID))
                        {
                            if (!LoadedNukeMetaphysicist.Contains(spellID))
                                LoadedNukeMetaphysicist.Add(spellID);

                            RegisterSpellProcessor(_settings["MPNormalNuke"].AsInt32(), MPSingleTargetNuke, (CombatActionPriority)_settings["MPNormalNukePriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                            LoadedNukeSpells.UnionWith(LoadedNukeMetaphysicist);

                        break;

                    case Profession.NanoTechnician:
                        if (Spell.GetSpellsForNanoline(NanoLine.DOTNanotechnicianStrainA).Any(s => s.Id == spellID))
                        {
                            if (!LoadedNukeNanoTechnician.Contains(spellID))
                                LoadedNukeNanoTechnician.Add(spellID);

                            RegisterSpellProcessor(_settings["DOTNanotechnicianStrainA"].AsInt32(), DOTADebuffTarget, (CombatActionPriority)_settings["DOTNanotechnicianStrainAPriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (SpellID.NTNukes.Contains(spellID))
                        {
                            if (!LoadedNukeNanoTechnician.Contains(spellID))
                                LoadedNukeNanoTechnician.Add(spellID);

                            RegisterSpellProcessor(_settings["NTNukesA"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SingleTargetNuke(spell, fightingTarget, ref actionTarget, "NTNukesA"), (CombatActionPriority)_settings["NTNukesAPriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.DOTNanotechnicianStrainB).Any(s => s.Id == spellID))
                        {
                            if (!LoadedNukeNanoTechnician.Contains(spellID))
                                LoadedNukeNanoTechnician.Add(spellID);

                            RegisterSpellProcessor(_settings["DOTNanotechnicianStrainB"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SingleTargetNuke(spell, fightingTarget, ref actionTarget, "DOTNanotechnicianStrainB"), (CombatActionPriority)_settings["DOTNanotechnicianStrainBPriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (SpellID.NTNukes.Contains(spellID))
                        {
                            if (!LoadedNukeNanoTechnician.Contains(spellID))
                                LoadedNukeNanoTechnician.Add(spellID);

                            RegisterSpellProcessor(_settings["NTNukesB"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SingleTargetNuke(spell, fightingTarget, ref actionTarget, "NTNukesB"), (CombatActionPriority)_settings["NTNukesBPriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (SpellID.NTRKAOENukes.Contains(spellID))
                        {
                            if (!LoadedNukeNanoTechnician.Contains(spellID))
                                LoadedNukeNanoTechnician.Add(spellID);

                            RegisterSpellProcessor(_settings["NTRKAOENukes"].AsInt32(), (Spell aoeNuke, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            AOENuke(aoeNuke, fightingTarget, ref actionTarget, "NTRKAOENukes"), (CombatActionPriority)_settings["NTRKAOENukesPriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (SpellID.NTSLAOENukes.Contains(spellID))
                        {
                            if (!LoadedNukeNanoTechnician.Contains(spellID))
                                LoadedNukeNanoTechnician.Add(spellID);

                            RegisterSpellProcessor(_settings["NTSLAOENukes"].AsInt32(), (Spell aoeNuke, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            AOENuke(aoeNuke, fightingTarget, ref actionTarget, "NTSLAOENukes"), (CombatActionPriority)_settings["NTSLAOENukesPriority"].AsInt32(), RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician)
                            LoadedNukeSpells.UnionWith(LoadedNukeNanoTechnician);

                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Bureaucrat

        private bool WorkplaceDepressionTargetDebuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Nuking"].AsBool()) return false;
            if (_settings["SpecialNukeOption"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;

            switch (_settings["NanoDeltaDebuffOption"].AsInt32())
            {
                case 1:
                    if (spell.Id == 273631 && Spell.List.Any(s => s.Id == 222687 && s.IsReady) && !fightingTarget.Buffs.Contains(NanoLine.NanoDeltaDebuff))
                        return false;
                    break;

                case 2:
                    if (fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))
                    {
                        if (_settings["SpecialNukeOption"].AsInt32() == 2 && spell.Id == 273631 && Spell.List.Any(s => s.Id == 222687 && s.IsReady) && !fightingTarget.Buffs.Contains(NanoLine.NanoDeltaDebuff))
                            return false;
                    }
                    break;
            }

            if (spell.Id == 273631 && fightingTarget.Buffs.Find(new[] { 273632, 301842 }, out Buff targetDebuff) && targetDebuff.RemainingTime > 10) return false;
            if (spell.Id == 270250 && fightingTarget.Buffs.Contains(NanoLine.NanoResistanceDebuff_LineA)) return false;

            if (_settings["SpecialNukeOption"].AsInt32() == 2 && !(fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))) return false;

            return true;
        }

        private bool CratSingleTargetNuke(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Nuking"].AsBool()) return false;
            if (_settings["NormalNukeOption"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;

            switch (_settings["SpecialNukeOption"].AsInt32())
            {
                case 1:
                    if (spell.Id == 273307 && Spell.List.Any(s => s.Id == 273631 && s.IsReady) && !fightingTarget.Buffs.Contains(NanoLine.GeneralNanoACDebuff))
                        return false;
                    break;

                case 2:
                    if (fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))
                    {
                        if (_settings["NormalNukeOption"].AsInt32() == 2 && spell.Id == 273307 && Spell.List.Any(s => s.Id == 273631 && s.IsReady) && !fightingTarget.Buffs.Contains(NanoLine.GeneralNanoACDebuff))
                            return false;
                    }
                    break;
            }

            if (_settings["NormalNukeOption"].AsInt32() == 2 && fightingTarget.MaxHealth < 1000000) return false;

            return true;
        }

        #endregion

        #region Doctor

        private bool DocSingleTargetNuke(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Nuking"].AsBool()) return false;
            if (_settings["NukeOption"].AsInt32() == 0) return false;
            if (DynelManager.LocalPlayer.NanoPercent < 40) return false;
            if (fightingTarget == null) return false;

            switch (_settings["NukeOption"].AsInt32())
            {
                case 1:
                    return TargetDebuff(spell, spell.Nanoline, fightingTarget, ref actionTarget);
                case 2:
                    if (fightingTarget?.MaxHealth < 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;
                    return TargetDebuff(spell, spell.Nanoline, fightingTarget, ref actionTarget);
                default:
                    return false;
            }
        }

        private bool Dots(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string option)
        {
            try
            {
                if (!_settings["Nuking"].AsBool()) return false;
                if (_settings[option] == null) return false;
                if (_settings[option].AsInt32() == 0) return false;
                if (!CanCast(spell)) return false;
                if (NeedsReload()) return false;
                if (fightingTarget == null) return false;
                if (debuffAreaTargetsToIgnore.Contains(fightingTarget?.Name)) return false;

                switch (_settings[option].AsInt32())
                {
                    case 1:
                        return TargetDebuff(spell, spell.Nanoline, fightingTarget, ref actionTarget);
                    case 2:
                        if (fightingTarget?.MaxHealth < 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;
                        return TargetDebuff(spell, spell.Nanoline, fightingTarget, ref actionTarget);
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

        #endregion

        #region Metaphysicist
        private bool WarmUpNuke(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!_settings["Nuking"].AsBool()) return false;
                if (_settings["MindDamageOption"] == null) return false;
                if (_settings["MindDamageOption"].AsInt32() == 0) return false;
                if (!CanCast(spell)) return false;
                if (fightingTarget == null) return false;

                if (_settings["MindDamageOption"].AsInt32() == 2)
                    if (fightingTarget?.MaxHealth <= 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;

                if (fightingTarget.Buffs.Find(NanoLine.MetaphysicistMindDamageNanoDebuffs, out Buff debuff) && debuff.RemainingTime > 3) return false;

                actionTarget = (fightingTarget, true);
                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        private bool MPSingleTargetNuke(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!_settings["Nuking"].AsBool()) return false;
                if (_settings["MPNormalNukeOption"] == null) return false;
                if (_settings["MPNormalNukeOption"].AsInt32() == 0) return false;
                if (!CanCast(spell)) return false;
                if (fightingTarget == null) return false;

                if (_settings["MPNormalNukeOption"].AsInt32() == 2)
                    if (fightingTarget?.MaxHealth <= 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;

                if (_settings["MindDamageOption"] != null)
                {
                    switch (_settings["MindDamageOption"].AsInt32())
                    {
                        case 1:
                            if (fightingTarget.Buffs.Find(NanoLine.MetaphysicistMindDamageNanoDebuffs, out Buff debuff) && debuff.RemainingTime <= 3) return false;
                            break;
                        case 2:
                            if (fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))
                            {
                                if (fightingTarget.Buffs.Find(NanoLine.MetaphysicistMindDamageNanoDebuffs, out Buff debuffBoss) && debuffBoss.RemainingTime <= 3) return false;
                            }
                            break;
                    }
                }

                actionTarget = (fightingTarget, true);
                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        #endregion

        #region NanoTechnician

        private bool DOTADebuffTarget(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["DOTNanotechnicianStrainAOption"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (DynelManager.LocalPlayer.NanoPercent < 40) return false;
            if (fightingTarget == null) return false;
            if (debuffAreaTargetsToIgnore.Contains(fightingTarget?.Name)) return false;
            if (fightingTarget.Buffs.Contains(spell.Nanoline)) return false;

            if (spell.Id == 28596)
            {
                if (!_settings["AOE"].AsBool())
                    return false;
            }

            if (_settings["DOTNanotechnicianStrainAOption"].AsInt32() == 2)
                if (fightingTarget.MaxHealth < 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool AOENuke(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!_settings["AOE"].AsBool()) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;

            if (_settings[setting + "Option"].AsInt32() == 2)
                if (fightingTarget.MaxHealth < 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool SingleTargetNuke(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;

            if (_settings[setting + "Option"].AsInt32() == 2)
                if (fightingTarget.MaxHealth < 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        #endregion
    }
}
