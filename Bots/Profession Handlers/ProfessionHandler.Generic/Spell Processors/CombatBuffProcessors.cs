using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public static HashSet<int> LoadedCombatBuffs = new HashSet<int>();

        public static HashSet<int> LoadedCombatAdventurerBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatBureaucratBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatDoctorBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatEnforcerBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatEngineerBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatFixerBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatKeeperBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatMartialArtistBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatMetaphysicistBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatNanoTechnicianBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatShadeBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatSoldierBuffs = new HashSet<int>();
        public static HashSet<int> LoadedCombatTraderBuffs = new HashSet<int>();

        private void RegisterCombatSpells(int spellID)
        {
            try
            {
                if (DynelManager.LocalPlayer.Profession == Profession.Agent)
                {
                    if (Spell.GetSpellsForNanoline(NanoLine.ConcentrationCriticalLine).Any(s => s.Id == spellID))
                    {
                        LoadedCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["ConcentrationCriticalLine"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        UserCombatBuff(spell, fightingTarget, ref actionTarget, "ConcentrationCriticalLine", false), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (Spell.GetSpellsForNanoline(NanoLine.AdventurerMorphBuff).Any(s => s.Id == spellID) && !LoadedCombatAdventurerBuffs.Contains(spellID))
                        LoadedCombatAdventurerBuffs.Add(spellID);

                    if (SpellID.TakeTheBullet.Contains(spellID) && !LoadedCombatBureaucratBuffs.Contains(spellID))
                        LoadedCombatBureaucratBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.Ransack_DepriveResistBuff).Any(s => s.Id == spellID) && !LoadedCombatDoctorBuffs.Contains(spellID))
                        LoadedCombatDoctorBuffs.Add(spellID);

                    if (SpellID.UserAbsorbACBuffEnforcer.Contains(spellID))
                    {
                        if (!FPBuffWindowController.CombatSelfBuff.Any(c => c.SettingKey == "UserAbsorbACBuffEnforcer"))
                            FPBuffWindowController.CombatSelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserAbsorbACBuffEnforcer, SettingKey = "UserAbsorbACBuffEnforcer", Label = "Absorb AC Buff", UiType = UiType.DropDownInputBoxDropDown, Options = FPBuffWindowController.UserCycleTime });

                        LoadedCombatEnforcerBuffs.Add(spellID);
                    }

                    if (SpellID.TargetAbsorbACBuffEnforcer.Contains(spellID))
                    {
                        if (!FPBuffWindowController.CombatTargetBuff.Any(c => c.SettingKey == "TargetAbsorbACBuffEnforcer"))
                            FPBuffWindowController.CombatTargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetAbsorbACBuffEnforcer, SettingKey = "TargetAbsorbACBuffEnforcer", Label = "Absorb AC Buff", UiType = UiType.DropDownInputBoxDropDown, Options = FPBuffWindowController.TargetCycleTime });

                        LoadedCombatEnforcerBuffs.Add(spellID);
                    }

                    if (Spell.GetSpellsForNanoline(NanoLine.Challenger).Any(s => s.Id == spellID) && !LoadedCombatEnforcerBuffs.Contains(spellID))
                        LoadedCombatEnforcerBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.Rage).Any(s => s.Id == spellID) && !LoadedCombatEnforcerBuffs.Contains(spellID))
                        LoadedCombatEnforcerBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.Charge).Any(s => s.Id == spellID) && !LoadedCombatEnforcerBuffs.Contains(spellID))
                        LoadedCombatEnforcerBuffs.Add(spellID);

                    if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !LoadedCombatEngineerBuffs.Contains(spellID))
                        LoadedCombatEngineerBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.FixerFearImmunity).Any(s => s.Id == spellID) && !LoadedCombatFixerBuffs.Contains(spellID))
                        LoadedCombatFixerBuffs.Add(spellID);

                    if (SpellID.UserDamageBuff_LineCMartialArtist.Contains(spellID) && !LoadedCombatMartialArtistBuffs.Contains(spellID))
                        LoadedCombatMartialArtistBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.ControlledDestructionBuff).Any(s => s.Id == spellID) && !LoadedCombatMartialArtistBuffs.Contains(spellID))
                        LoadedCombatMartialArtistBuffs.Add(spellID);

                    if (SpellID.SacrificialBond.Contains(spellID) && !LoadedCombatMetaphysicistBuffs.Contains(spellID))
                        LoadedCombatMetaphysicistBuffs.Add(spellID);

                    if (SpellID.SacrificialPower.Contains(spellID) && !LoadedCombatMetaphysicistBuffs.Contains(spellID))
                        LoadedCombatMetaphysicistBuffs.Add(spellID);

                    if (SpellID.UserAbsorbACBuffNanoTechnician.Contains(spellID))
                    {
                        if (!FPBuffWindowController.CombatSelfBuff.Any(c => c.SettingKey == "UserAbsorbACBuffNanoTechnician"))
                            FPBuffWindowController.CombatSelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserAbsorbACBuffNanoTechnician, SettingKey = "UserAbsorbACBuffNanoTechnician", Label = "Absorb AC Buff", UiType = UiType.DropDownInputBoxDropDown, Options = FPBuffWindowController.UserCycleTime });

                        LoadedCombatNanoTechnicianBuffs.Add(spellID);
                    }

                    if (SpellID.TargetAbsorbACBuffNanoTechnician.Contains(spellID))
                    {
                        if (!FPBuffWindowController.CombatTargetBuff.Any(c => c.SettingKey == "TargetAbsorbACBuffNanoTechnician"))
                            FPBuffWindowController.CombatTargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetAbsorbACBuffNanoTechnician, SettingKey = "TargetAbsorbACBuffNanoTechnician", Label = "Absorb AC Buff", UiType = UiType.DropDownInputBoxDropDown, Options = FPBuffWindowController.TargetCycleTime });

                        LoadedCombatNanoTechnicianBuffs.Add(spellID);
                    }

                    if (Spell.GetSpellsForNanoline(NanoLine.NullitySphereNano).Any(s => s.Id == spellID) && !LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                        LoadedCombatNanoTechnicianBuffs.Add(spellID);

                    if (SpellID.NanobotAegis == spellID && !LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                        LoadedCombatNanoTechnicianBuffs.Add(spellID);

                    if (SpellID.UserReflectShieldSoldier.Contains(spellID) && !LoadedCombatSoldierBuffs.Contains(spellID))
                        LoadedCombatSoldierBuffs.Add(spellID);

                    if (Spell.GetSpellsForNanoline(NanoLine.DamagetoNano).Any(s => s.Id == spellID) && !LoadedCombatTraderBuffs.Contains(spellID))
                        LoadedCombatTraderBuffs.Add(spellID);
                }

                switch ((Profession)DynelManager.LocalPlayer.GetStat(Stat.VisualProfession))
                {
                    case Profession.Adventurer:
                        if (Spell.GetSpellsForNanoline(NanoLine.AdventurerMorphBuff).Any(s => s.Id == spellID))
                        {
                            if (!LoadedCombatAdventurerBuffs.Contains(spellID))
                                LoadedCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.AdventurerMorphBuff), AdventurerMorphBuffMethod, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Adventurer)
                            LoadedCombatBuffs.UnionWith(LoadedCombatAdventurerBuffs);
                        break;

                    case Profession.Bureaucrat:
                        if (SpellID.TakeTheBullet.Contains(spellID))
                        {
                            if (!LoadedCombatBureaucratBuffs.Contains(spellID))
                                LoadedCombatBureaucratBuffs.Add(spellID);

                            RegisterSpellProcessor(SpellID.TakeTheBullet, TakeTheBulletMethod, CombatActionPriority.VeryHigh, RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Bureaucrat)
                            LoadedCombatBuffs.UnionWith(LoadedCombatBureaucratBuffs);
                        break;

                    case Profession.Doctor:
                        if (Spell.GetSpellsForNanoline(NanoLine.Ransack_DepriveResistBuff).Any(s => s.Id == spellID))
                        {
                            if (!LoadedCombatDoctorBuffs.Contains(spellID))
                                LoadedCombatDoctorBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["Ransack_DepriveResistBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                UserCombatBuff(spell, fightingTarget, ref actionTarget, "Ransack_DepriveResistBuff", true), CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Doctor)
                            LoadedCombatBuffs.UnionWith(LoadedCombatDoctorBuffs);
                        break;

                    case Profession.Enforcer:
                        if (SpellID.UserAbsorbACBuffEnforcer.Contains(spellID))
                        {
                            if (!BuffWindowController.CombatSelfBuff.Any(c => c.SettingKey == "UserAbsorbACBuffEnforcer"))
                                BuffWindowController.CombatSelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserAbsorbACBuffEnforcer, SettingKey = "UserAbsorbACBuffEnforcer", Label = "Absorb AC Buff", UiType = UiType.DropDownInputBoxDropDown, Options = BuffWindowController.UserCycleTime });

                            if (!LoadedCombatEnforcerBuffs.Contains(spellID))
                                LoadedCombatEnforcerBuffs.Add(spellID);

                            RegisterSpellProcessor(_settings["UserAbsorbACBuffEnforcer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserCycleCombatBuff(spell, fightingTarget, ref actionTarget, "UserAbsorbACBuffEnforcer"),
                            _settings["UserAbsorbACBuffEnforcerOption"].AsInt32() == 1 ? CombatActionPriority.NonCombat : CombatActionPriority.High, _settings["UserAbsorbACBuffEnforcerOption"].AsInt32() == 1 ? RuleContext.OutOfCombat : RuleContext.Both);

                        }

                        if (SpellID.TargetAbsorbACBuffEnforcer.Contains(spellID))
                        {
                            if (!BuffWindowController.CombatTargetBuff.Any(c => c.SettingKey == "TargetAbsorbACBuffEnforcer"))
                                BuffWindowController.CombatTargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetAbsorbACBuffEnforcer, SettingKey = "TargetAbsorbACBuffEnforcer", Label = "Absorb AC Buff", UiType = UiType.DropDownInputBoxDropDown, Options = BuffWindowController.TargetCycleTime });

                            if (!LoadedCombatEnforcerBuffs.Contains(spellID))
                                LoadedCombatEnforcerBuffs.Add(spellID);

                            RegisterSpellProcessor(_settings["TargetAbsorbACBuffEnforcer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetCycleTimeCombatBuff(spell, fightingTarget, ref actionTarget, "TargetAbsorbACBuffEnforcer"),
                            _settings["TargetAbsorbACBuffEnforcerOption"].AsInt32() == 1 ? CombatActionPriority.NonCombat : CombatActionPriority.High, _settings["TargetAbsorbACBuffEnforcerOption"].AsInt32() == 1 ? RuleContext.OutOfCombat : RuleContext.Both);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.Challenger).Any(s => s.Id == spellID))
                        {
                            if (!BuffWindowController.CombatSelfBuff.Any(c => c.SettingKey == "Challenger"))
                                BuffWindowController.CombatSelfBuff.Add(new BuffUiConfig
                                {
                                    BuffIDs = Spell.GetSpellsForNanoline(NanoLine.Challenger).Select(s => s.Id).ToArray(),
                                    SettingKey = "Challenger",
                                    Label = "Challenger",
                                    UiType = UiType.DropDownInputBoxDropDown,
                                    Options = new List<(string, int)> { ("Off", 0), ("Cycle (seconds)", 2) }
                                });

                            if (!LoadedCombatEnforcerBuffs.Contains(spellID))
                                LoadedCombatEnforcerBuffs.Add(spellID);

                            RegisterSpellProcessor(_settings["Challenger"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                UserCycleCombatBuff(spell, fightingTarget, ref actionTarget, "Challenger"), CombatActionPriority.VeryLow, RuleContext.InCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.Rage).Any(s => s.Id == spellID))
                        {
                            if (!LoadedCombatEnforcerBuffs.Contains(spellID))
                                LoadedCombatEnforcerBuffs.Add(spellID);

                            RegisterSpellProcessor(_settings["Rage"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                UserCycleCombatBuff(spell, fightingTarget, ref actionTarget, "Rage"), CombatActionPriority.VeryLow, RuleContext.Both);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.Charge).Any(s => s.Id == spellID))
                        {
                            if (!LoadedCombatEnforcerBuffs.Contains(spellID))
                                LoadedCombatEnforcerBuffs.Add(spellID);

                            RegisterSpellProcessor(_settings["Charge"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                CastOnFightingTarget(spell, fightingTarget, ref actionTarget, "Charge"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Enforcer)
                            LoadedCombatBuffs.UnionWith(LoadedCombatEnforcerBuffs);
                        break;

                    case Profession.Engineer:
                        if (DynelManager.LocalPlayer.Profession == Profession.Engineer)
                            LoadedCombatBuffs.UnionWith(LoadedCombatEngineerBuffs);
                        break;

                    case Profession.Fixer:
                        if (Spell.GetSpellsForNanoline(NanoLine.FixerFearImmunity).Any(s => s.Id == spellID))
                        {
                            if (!LoadedCombatFixerBuffs.Contains(spellID))
                                LoadedCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["FixerFearImmunity"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                UserCombatBuff(spell, fightingTarget, ref actionTarget, "FixerFearImmunity", true), CombatActionPriority.VeryLow, RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Fixer)
                            LoadedCombatBuffs.UnionWith(LoadedCombatFixerBuffs);
                        break;

                    case Profession.Keeper:

                        if (SpellID.TeamFortifyKeeper.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Keeper && !BuffWindowController.CombatTeamBuff.Any(c => c.SettingKey == "TeamFortifyKeeper"))
                                BuffWindowController.CombatTeamBuff.Add(new BuffUiConfig
                                {
                                    BuffIDs = SpellID.TeamFortifyKeeper,
                                    SettingKey = "TeamFortifyKeeper",
                                    Label = "Fortify",
                                    UiType = UiType.DualDropDown,
                                    Options = new List<(string Name, int Value)> { ("Off", 0), ("Buff", 1), ("Spam", 2) }
                                });

                            LoadedCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TeamFortifyKeeper"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            Spam(spell, fightingTarget, ref actionTarget, "TeamFortifyKeeper"),
                            _settings["TeamFortifyKeeperOption"].AsInt32() == 1 ? CombatActionPriority.NonCombat : CombatActionPriority.High, _settings["TeamFortifyKeeperOption"].AsInt32() == 1 ? RuleContext.OutOfCombat : RuleContext.Both);
                        }

                        RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.KeeperFearImmunity).OrderByStackingOrder(), RecastAntiFear, CombatActionPriority.Ultra, RuleContext.Both);

                        if (DynelManager.LocalPlayer.Profession == Profession.Keeper)
                            LoadedCombatBuffs.UnionWith(LoadedCombatKeeperBuffs);

                        break;

                    case Profession.MartialArtist:
                        if (SpellID.UserDamageBuff_LineCMartialArtist.Contains(spellID))
                        {
                            if (!LoadedCombatMartialArtistBuffs.Contains(spellID))
                                LoadedCombatMartialArtistBuffs.Add(spellID);

                            RegisterSpellProcessor(_settings["DamageBuff_LineCSelfMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserCombatBuff(spell, fightingTarget, ref actionTarget, "DamageBuff_LineCSelfMartialArtist", true), CombatActionPriority.VeryLow, RuleContext.InCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.ControlledDestructionBuff).Any(s => s.Id == spellID))
                        {
                            if (!LoadedCombatMartialArtistBuffs.Contains(spellID))
                                LoadedCombatMartialArtistBuffs.Add(spellID);

                            RegisterSpellProcessor(_settings["ControlledDestructionBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserCombatBuff(spell, fightingTarget, ref actionTarget, "ControlledDestructionBuff", true), CombatActionPriority.VeryLow, RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist)
                            LoadedCombatBuffs.UnionWith(LoadedCombatMartialArtistBuffs);
                        break;

                    case Profession.Metaphysicist:
                        if (SpellID.SacrificialBond.Contains(spellID))
                        {
                            if (!LoadedCombatMetaphysicistBuffs.Contains(spellID))
                                LoadedCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(SpellID.SacrificialBond, SacrificialBond, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (SpellID.SacrificialPower.Contains(spellID))
                        {
                            if (!LoadedCombatMetaphysicistBuffs.Contains(spellID))
                                LoadedCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(SpellID.SacrificialPower, SacrificialBond, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                            LoadedCombatBuffs.UnionWith(LoadedCombatMetaphysicistBuffs);
                        break;

                    case Profession.NanoTechnician:
                        if (SpellID.UserAbsorbACBuffNanoTechnician.Contains(spellID))
                        {
                            if (!BuffWindowController.CombatSelfBuff.Any(c => c.SettingKey == "UserAbsorbACBuffNanoTechnician"))
                                BuffWindowController.CombatSelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserAbsorbACBuffNanoTechnician, SettingKey = "UserAbsorbACBuffNanoTechnician", Label = "Absorb AC Buff", UiType = UiType.DropDownInputBoxDropDown, Options = BuffWindowController.UserCycleHP });

                            if (!LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                                LoadedCombatNanoTechnicianBuffs.Add(spellID);

                            //RegisterSpellProcessor(_settings["UserAbsorbACBuffNanoTechnician"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            //    UserCycleCombatBuff(spell, fightingTarget, ref actionTarget, "UserAbsorbACBuffNanoTechnician"), CombatActionPriority.High, RuleContext.Both);
                            RegisterSpellProcessor(_settings["UserAbsorbACBuffNanoTechnician"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserCycleCombatBuff(spell, fightingTarget, ref actionTarget, "UserAbsorbACBuffNanoTechnician"),
                            _settings["UserAbsorbACBuffNanoTechnicianOption"].AsInt32() == 1 ? CombatActionPriority.NonCombat : CombatActionPriority.High, _settings["UserAbsorbACBuffNanoTechnicianOption"].AsInt32() == 1 ? RuleContext.OutOfCombat : RuleContext.Both);

                        }

                        if (SpellID.TargetAbsorbACBuffNanoTechnician.Contains(spellID))
                        {
                            if (!BuffWindowController.CombatTargetBuff.Any(c => c.SettingKey == "TargetAbsorbACBuffNanoTechnician"))
                                BuffWindowController.CombatTargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetAbsorbACBuffNanoTechnician, SettingKey = "TargetAbsorbACBuffNanoTechnician", Label = "Absorb AC Buff", UiType = UiType.DropDownInputBoxDropDown, Options = BuffWindowController.TargetCycleTime });

                            if (!LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                                LoadedCombatNanoTechnicianBuffs.Add(spellID);

                            //RegisterSpellProcessor(_settings["TargetAbsorbACBuffNanoTechnician"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            //    TargetCycleCombatBuff(spell, fightingTarget, ref actionTarget, "TargetAbsorbACBuffNanoTechnician"), CombatActionPriority.High, RuleContext.Both);
                            RegisterSpellProcessor(_settings["TargetAbsorbACBuffNanoTechnician"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetCycleTimeCombatBuff(spell, fightingTarget, ref actionTarget, "TargetAbsorbACBuffNanoTechnician"),
                            _settings["TargetAbsorbACBuffNanoTechnicianOption"].AsInt32() == 1 ? CombatActionPriority.NonCombat : CombatActionPriority.High, _settings["TargetAbsorbACBuffNanoTechnicianOption"].AsInt32() == 1 ? RuleContext.OutOfCombat : RuleContext.Both);

                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.NullitySphereNano).Any(s => s.Id == spellID))
                        {
                            if (!LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                                LoadedCombatNanoTechnicianBuffs.Add(spellID);

                            RegisterSpellProcessor(_settings["NullitySphereNano"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                CombatWithHPCheck(spell, fightingTarget, ref actionTarget, "NullitySphereNano"), CombatActionPriority.Ultra);
                        }

                        if (SpellID.NanobotAegis == spellID)
                        {
                            if (!LoadedCombatNanoTechnicianBuffs.Contains(spellID))
                                LoadedCombatNanoTechnicianBuffs.Add(spellID);

                            RegisterSpellProcessor(_settings["NanobotAegis"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                CombatWithHPCheck(spell, fightingTarget, ref actionTarget, "NanobotAegis"), CombatActionPriority.Ultra);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician)
                            LoadedCombatBuffs.UnionWith(LoadedCombatNanoTechnicianBuffs);
                        break;

                    case Profession.Shade:

                        if (Spell.GetSpellsForNanoline(NanoLine.EmergencySneak).Any(s => s.Id == spellID))
                        {
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.EmergencySneak).OrderByStackingOrder(), SmokeBombNano, CombatActionPriority.High);
                        }

                        if (SpellID.SpiritSiphon == spellID)
                        {
                            RegisterSpellProcessor(SpellID.SpiritSiphon, SpiritSiphon, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Soldier:
                        if (SpellID.UserReflectShieldSoldier.Contains(spellID))
                        {
                            if (!LoadedCombatSoldierBuffs.Contains(spellID))
                                LoadedCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["ReflectShieldSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                CombatWithHPCheck(spell, fightingTarget, ref actionTarget, "ReflectShieldSelf"), CombatActionPriority.High, RuleContext.Both);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Soldier)
                            LoadedCombatBuffs.UnionWith(LoadedCombatSoldierBuffs);
                        break;

                    case Profession.Trader:
                        if (Spell.GetSpellsForNanoline(NanoLine.DamagetoNano).Any(s => s.Id == spellID))
                        {
                            if (!LoadedCombatTraderBuffs.Contains(spellID))
                                LoadedCombatTraderBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["DamagetoNano"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                CombatWithHPCheck(spell, fightingTarget, ref actionTarget, "DamagetoNano"), CombatActionPriority.Ultra);
                        }

                        if (SpellID.DecisionbyCommittee == spellID)
                        {
                            //LoadedPetSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.DecisionbyCommittee, PetSpawner, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Trader)
                            LoadedCombatBuffs.UnionWith(LoadedCombatTraderBuffs);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Generic

        private bool UserCombatBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting, bool TargetSelf)
        {
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;

            if (spell.Nanoline == NanoLine.Ransack_DepriveResistBuff && !DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.TraderSkillTransferTargetDebuff_Ransack, NanoLine.TraderSkillTransferTargetDebuff_Deprive })) return false;

            switch (_settings[setting + "Option"].AsInt32())
            {
                case 1:
                    if (fightingTarget != null && SpellCheckSelf(spell))
                    {
                        actionTarget = (DynelManager.LocalPlayer, TargetSelf);
                        return true;
                    }
                    return false;
                case 2:
                    if (fightingTarget != null && (fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name)) && SpellCheckSelf(spell))
                    {
                        actionTarget = (DynelManager.LocalPlayer, TargetSelf);
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        private bool UserCycleCombatBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;

            switch (_settings[setting + "Option"].AsInt32())
            {
                case 1:// buff
                    if (fightingTarget == null)
                    {
                        if (SpellCheckSelf(spell))
                        {
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                    }
                    return false;
                case 2:
                    if (fightingTarget != null)
                    {
                        if (!DynelManager.LocalPlayer.Buffs.Find(spell.Nanoline, out Buff buff))
                        {
                            if (SpellCheckSelf(spell))
                            {
                                actionTarget = (DynelManager.LocalPlayer, true);
                                return true;
                            }
                        }
                        else if (buff.TotalTime - buff.RemainingTime >= _settings[setting + "Value"].AsInt32())
                        {
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                    }
                    else if (SpellCheckSelf(spell) && spell.Nanoline != NanoLine.Challenger)
                    {
                        actionTarget = (DynelManager.LocalPlayer, true);
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        private bool TargetCycleTimeCombatBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;

            switch (_settings[setting + "Option"].AsInt32())
            {
                case 1:// buff self
                    if (fightingTarget == null)
                    {
                        if (SpellCheckSelf(spell))
                        {
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                    }
                    return false;
                case 2:// buff team
                    if (fightingTarget == null)
                    {
                        if (Team.IsInTeam)
                        {
                            var member = Team.Members.FirstOrDefault(tm => tm.Character != null && tm.Character.Health > 0 && InCastRange(spell, tm.Character) && SpellCheckLocalTeam(spell, tm.Character))?.Character;

                            if (member != null)
                            {
                                actionTarget = (member, true);
                                return true;
                            }
                        }
                        else if (SpellCheckSelf(spell))
                        {
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                    }
                    return false;
                case 3:// cycle
                    if (fightingTarget != null)
                    {
                        if (!DynelManager.LocalPlayer.Buffs.Find(spell.Nanoline, out Buff buff))
                        {
                            if (SpellCheckSelf(spell))
                            {
                                actionTarget = (DynelManager.LocalPlayer, true);
                                return true;
                            }
                        }
                        else if (buff.TotalTime - buff.RemainingTime >= _settings[setting + "Value"].AsInt32())
                        {
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                    }
                    else if (SpellCheckSelf(spell))
                    {
                        actionTarget = (DynelManager.LocalPlayer, true);
                        return true;
                    }
                    return false;
                case 4:// cycle & team buff
                    if (fightingTarget != null)
                    {
                        if (!DynelManager.LocalPlayer.Buffs.Find(spell.Nanoline, out Buff buff))
                        {
                            if (SpellCheckSelf(spell))
                            {
                                actionTarget = (DynelManager.LocalPlayer, true);
                                return true;
                            }
                        }
                        else if (buff.TotalTime - buff.RemainingTime >= _settings[setting + "Value"].AsInt32())
                        {
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                    }
                    else if (Team.IsInTeam)
                    {
                        var member = Team.Members.FirstOrDefault(tm => tm.Character != null && tm.Character.Health > 0 && InCastRange(spell, tm.Character) && SpellCheckLocalTeam(spell, tm.Character))?.Character;

                        if (member != null)
                        {
                            actionTarget = (member, true);
                            return true;
                        }
                    }
                    else if (SpellCheckSelf(spell))
                    {
                        actionTarget = (DynelManager.LocalPlayer, true);
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        private bool CastOnFightingTarget(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string v)
        {
            if (_settings[v + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;

            switch (_settings[v + "Option"].AsInt32())
            {
                case 1:
                    if (fightingTarget != null)
                    {
                        actionTarget = (fightingTarget, true);
                        return true;
                    }
                    return false;
                case 2:
                    if (fightingTarget != null && (fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name)))
                    {
                        actionTarget = (fightingTarget, true);
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        private bool CombatWithHPCheck(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string v)
        {
            if (_settings[v + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;
            if (DynelManager.LocalPlayer.HealthPercent > _settings[v + "Value"].AsInt32()) return false;
            if (IsPlayerFlyingOrFalling()) return false;

            switch (spell.Nanoline)
            {
                case NanoLine.NullitySphereNano:
                    if (DynelManager.LocalPlayer.Buffs.Contains(SpellID.NanobotAegis)) return false;
                    break;
                case NanoLine.ReflectShield:
                    if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.NullitySphereNano)) return false;
                    break;

            }
            return spell.Cost < DynelManager.LocalPlayer.Nano;
        }

        #endregion

        #region Adventurer

        private bool AdventurerMorphBuffMethod(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["AdventurerMorphBuffOption"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;
            var buffs = DynelManager.LocalPlayer.Buffs;
            if (!buffs.Any(b => b.Nanoline == NanoLine.Polymorph)) return false;

            switch (_settings["AdventurerMorphBuffOption"].AsInt32())
            {
                case 1:
                    return MorphBuffCheck(spell, buffs, ref actionTarget);
                case 2:
                    if (fightingTarget.MaxHealth >= 1000000 || BossNames.Contains(fightingTarget.Name))
                        return MorphBuffCheck(spell, buffs, ref actionTarget);
                    return false;
                default:
                    return false;
            }
        }

        private bool MorphBuffCheck(Spell spell, IEnumerable<Buff> buffs, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (buffs.Any(b => SpellID.LeetMorph.Contains(b.Id)) && SpellID.AdventurerMorphBuffLeet.Contains(spell.Id) && !buffs.Any(b => b.Id == spell.Id))
            {
                actionTarget = (DynelManager.LocalPlayer, true);
                return true;
            }

            if (buffs.Any(b => SpellID.WolfMorph.Contains(b.Id)) && SpellID.AdventurerMorphBuffWolf.Contains(spell.Id) && !buffs.Any(b => b.Id == spell.Id))
            {
                actionTarget = (DynelManager.LocalPlayer, true);
                return true;
            }

            if (buffs.Any(b => SpellID.DragonMorph.Contains(b.Id)) && SpellID.AdventurerMorphBuffPitLizard.Contains(spell.Id) && !buffs.Any(b => b.Id == spell.Id))
            {
                actionTarget = (DynelManager.LocalPlayer, true);
                return true;
            }

            if (buffs.Any(b => SpellID.SaberMorph.Contains(b.Id)) && SpellID.AdventurerMorphBuffSabretooth.Contains(spell.Id) && !buffs.Any(b => b.Id == spell.Id))
            {
                actionTarget = (DynelManager.LocalPlayer, true);
                return true;
            }

            return false;
        }

        #endregion

        #region Bureaucrat

        private bool TakeTheBulletMethod(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["TakeTheBulletOption"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (DynelManager.LocalPlayer.Buffs.Contains(267918)) return false;
            if (!CanLookupPetsAfterZone()) return false;
            if (DynelManager.LocalPlayer.Pets.Any(c => c.Character.GetStat(Stat.NPCFamily) == 95)) return false;
            if (_settings["TakeTheBulletValue"].AsInt32() > DynelManager.LocalPlayer.HealthPercent) return false;

            actionTarget = (DynelManager.LocalPlayer.Pets.First(c => c.Character.GetStat(Stat.NPCFamily) == 95).Character, true);
            return true;
        }

        #endregion

        #region Keeper

        private bool Spam(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;

            switch (_settings[setting + "Option"].AsInt32())
            {
                case 1://buff
                    if (fightingTarget == null)
                    {
                        //if (Team.IsInTeam)
                        //{
                        //    var local = Team.Members.Find(x => x.Identity == DynelManager.LocalPlayer.Identity);
                        //    if (local == null)
                        //        return false;

                        //    var member = Team.Members.Find(tm => tm.Character != null && tm.Character.Identity != DynelManager.LocalPlayer.Identity && (!Team.IsRaid || tm.TeamIndex == local.TeamIndex) && tm.Character.Health > 0
                        //    && tm.Character.IsInLineOfSight && InCastRange(spell, tm.Character) && SpellCheckLocalTeam(spell, tm.Character) && !tm.Character.Buffs.Contains(NanoLine.KeeperAbsorbAura_Team))?.Character;

                        //    if (member != null)
                        //    {
                        //        actionTarget = (DynelManager.LocalPlayer, false);
                        //        return true;
                        //    }
                        //}
                        //else 
                        if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Fortify))
                        {
                            actionTarget = (DynelManager.LocalPlayer, false);
                            return true;
                        }
                    }
                    return false;
                case 2://spam
                    if (fightingTarget != null)
                    {
                        if (Spell.HasPendingCast) return false;
                        if (spell.Cost > DynelManager.LocalPlayer.Nano) return false;
                        actionTarget = (DynelManager.LocalPlayer, false);
                        return true;
                    }
                    else
                    {
                        //if (Team.IsInTeam)
                        //{
                        //    var local = Team.Members.Find(x => x.Identity == DynelManager.LocalPlayer.Identity);
                        //    if (local == null)
                        //        return false;

                        //    var member = Team.Members.Find(tm => tm.Character != null && tm.Character.Identity != DynelManager.LocalPlayer.Identity && (!Team.IsRaid || tm.TeamIndex == local.TeamIndex) && tm.Character.Health > 0
                        //    && tm.Character.IsInLineOfSight && InCastRange(spell, tm.Character) && SpellCheckLocalTeam(spell, tm.Character) && !tm.Character.Buffs.Contains(NanoLine.KeeperAbsorbAura_Team))?.Character;

                        //    if (member != null)
                        //    {
                        //        actionTarget = (DynelManager.LocalPlayer, false);
                        //        return true;
                        //    }
                        //}
                        //else 
                        if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Fortify))
                        {
                            actionTarget = (DynelManager.LocalPlayer, false);
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
        }

        private bool RecastAntiFear(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["RecastAntiFear"].AsBool()) return false;
            if (spell == null) return false;
            if (!spell.IsReady) return false;
            if (!DynelManager.LocalPlayer.MovementStatePermitsCasting) return false;
            if (spell.Cost > DynelManager.LocalPlayer.Nano) return false;

            switch (Playfield.ModelIdentity.Instance)
            {
                case 6015:
                    if (!DynelManager.NPCs.Any(c => c.Health > 0 && c.Name == "Left Hand of Insanity")) return false;
                    return true;
                case 9070:
                    if (!DynelManager.NPCs.Any(c => c.Health > 0 && c.Name == "Queen of the Slums")) return false;
                    return true;
                case 8020:
                    if (!DynelManager.NPCs.Any(c => c.Health > 0 && c.Name == "The Cybernetic Daemon")) return false;
                    return true;
            }

            return false;
        }

        #endregion

        #region Metaphysicist

        private bool SacrificialBond(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            return false;
        }

        #endregion

        #region Shade

        private bool SmokeBombNano(Spell spell, SimpleChar fightingtarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["SmokeBomb"].AsBool()) return false;
            if (DynelManager.LocalPlayer.HealthPercent > 30) return false;
            actionTarget.ShouldSetTarget = false;
            return true;
        }
        private bool SpiritSiphon(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["SpiritSiphon"].AsBool()) return false;
            if (fightingTarget == null) return false;
            if (fightingTarget.Level < _settings["SpiritSiphonLvl"].AsInt32()) return false;
            if (DynelManager.LocalPlayer.Nano < spell.Cost) return false;
            if (fightingTarget.HealthPercent > 21) return false;

            spell.Cast(fightingTarget, true);

            return false;
        }

        #endregion

        #region Soldier
        private bool AMS(Spell spell, SimpleChar fightingtarget, ref (SimpleChar Target, bool ShouldSetTarget) actiontarget)
        {
            if (!_settings["Buffing"].AsBool()) return false;
            if (!CanCast(spell)) return false;

            return DynelManager.LocalPlayer.HealthPercent <= _settings["AMSPercentage"].AsInt32();

        }
        #endregion

        #region Trader


        private bool PetSpawner(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["PetSelection"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;

            switch (_settings["PetSelection"].AsInt32())
            {
                case 1:
                    return NoShellPetSpawner(PetType.Attack, spell, fightingTarget, ref actionTarget);
                case 2:
                    if (fightingTarget.MaxHealth < 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;
                    return NoShellPetSpawner(PetType.Attack, spell, fightingTarget, ref actionTarget);
                default:
                    return false;

            }
        }

        #endregion

    }
}
