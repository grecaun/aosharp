using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public static HashSet<int> LoadedHeals = new HashSet<int>();

        private void RegisterHealSpells(Spell HealSpell)
        {
            try
            {
                var visualProfession = (Profession)DynelManager.LocalPlayer.GetStat(Stat.VisualProfession);

                if (HealSpell.Id == SpellID.FountainOfLife && HealSpell.MeetsSelfUseReqs())
                {
                    LoadedHeals.Add(HealSpell.Id);
                    RegisterSpellProcessor(SpellID.FountainOfLife, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "FountainOfLifeHeal"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                #region Adventurer

                if (SpellID.AdventureSingleTargetHealing.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "AdventureSingleTargetHealing"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.AdventureSingleTargetHealing, SettingKey = "AdventureSingleTargetHealing", Label = "Adventure Single Target Healing", UiType = UiType.DropDownInputBoxDropDown });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Adventurer)
                        RegisterSpellProcessor(_settings["AdventureSingleTargetHealing"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "AdventureSingleTargetHealing"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (SpellID.AdventurerCompleteHealing.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "AdventurerCompleteHealing"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.AdventurerCompleteHealing, SettingKey = "AdventurerCompleteHealing", Label = "Adventurer Complete Healing", UiType = UiType.DropDownInputBoxDropDown });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Adventurer)
                        RegisterSpellProcessor(_settings["AdventurerCompleteHealing"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "AdventurerCompleteHealing"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (SpellID.AdventurerTeamHealing.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.AOEHeal.Any(c => c.SettingKey == "AdventurerTeamHealing"))
                        HealWindowController.AOEHeal.Add(new HealUiConfig { HealIds = SpellID.AdventurerTeamHealing, SettingKey = "AdventurerTeamHealing", Label = "Adventurer Team Healing", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Adventurer)
                        RegisterSpellProcessor(_settings["AdventurerTeamHealing"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TeamHealing(spell, fightingTarget, ref actionTarget, "AdventurerTeamHealing"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.MorphHeal).Any(s => s.Id == HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "MorphHeal"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = Spell.GetSpellsForNanoline(NanoLine.MorphHeal).Select(s => s.Id).ToArray(), SettingKey = "MorphHeal", Label = "Morph Heal", UiType = UiType.DropDownInputBoxDropDown });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Adventurer)
                        RegisterSpellProcessor(_settings["MorphHeal"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "MorphHeal"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                #endregion

                #region Bureaucrat

                if (SpellID.BureaucratPetHeal.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "HealPets"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.BureaucratPetHeal, SettingKey = "HealPets", Label = "Pet Heal", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["HealPets"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => HealPet(spell, fightingTarget, ref actionTarget, "HealPets"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                #endregion

                #region Engineer

                if (SpellID.EngineerPetHeal.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "HealPets"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.EngineerPetHeal, SettingKey = "HealPets", Label = "Pet Heal", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Engineer)
                        RegisterSpellProcessor(_settings["HealPets"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => HealPet(spell, fightingTarget, ref actionTarget, "HealPets"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (HealSpell.Id == 270351)
                {
                    if (!HealWindowController.AOEHeal.Any(c => c.SettingKey == "AOEHealPets"))
                        HealWindowController.AOEHeal.Add(new HealUiConfig { HealIds = SpellID.EngineerAOEPetHeal, SettingKey = "AOEHealPets", Label = "AOE Pet Heal", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Engineer)
                        RegisterSpellProcessor(_settings["AOEHealPets"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => HealPet(spell, fightingTarget, ref actionTarget, "AOEHealPets"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                #endregion

                #region Doctor

                if (SpellID.DoctorSingleTargetHealing.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "DoctorSingleTargetHealing"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.DoctorSingleTargetHealing, SettingKey = "DoctorSingleTargetHealing", Label = "Doctor Single Target Healing", UiType = UiType.DropDownInputBoxDropDown });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Doctor)
                        RegisterSpellProcessor(_settings["DoctorSingleTargetHealing"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "DoctorSingleTargetHealing"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (SpellID.DoctorCompleteTargetHealing.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "DoctorCompleteTargetHealing"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.DoctorCompleteTargetHealing, SettingKey = "DoctorCompleteTargetHealing", Label = "Doctor Complete Healing", UiType = UiType.DropDownInputBoxDropDown });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Doctor)
                        RegisterSpellProcessor(_settings["DoctorCompleteTargetHealing"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "DoctorCompleteTargetHealing"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (SpellID.DoctorTeamHealing.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.AOEHeal.Any(c => c.SettingKey == "DoctorTeamHealing"))
                        HealWindowController.AOEHeal.Add(new HealUiConfig { HealIds = SpellID.DoctorTeamHealing, SettingKey = "DoctorTeamHealing", Label = "Doctor Team Healing", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Doctor)
                        RegisterSpellProcessor(_settings["DoctorTeamHealing"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TeamHealing(spell, fightingTarget, ref actionTarget, "DoctorTeamHealing"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (HealSpell.Id == SpellID._alphaAndOmega)
                {
                    if (!HealWindowController.AOEHeal.Any(c => c.SettingKey == "CompleteTeamHealingLine"))
                        HealWindowController.AOEHeal.Add(new HealUiConfig { HealIds = new[] { SpellID._alphaAndOmega }, SettingKey = "CompleteTeamHealingLine", Label = "Team Complete Healing", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Doctor)
                        RegisterSpellProcessor(SpellID._alphaAndOmega, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TeamHealing(spell, fightingTarget, ref actionTarget, "CompleteTeamHealingLine"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (SpellID._teamImprovedLifeChanneler == HealSpell.Id)
                {
                    if (!HealWindowController.AOEHeal.Any(c => c.SettingKey == "TeamImprovedLifeChanneler"))
                        HealWindowController.AOEHeal.Add(new HealUiConfig { HealIds = new[] { SpellID._teamImprovedLifeChanneler }, SettingKey = "TeamImprovedLifeChanneler", Label = "Team Improved Life Channeler", UiType = UiType.DropDownInputBoxDropDown, Options = new List<(string Name, int Value)> { ("Off", 0), ("Team Heal (HP%)", 1), ("Buff", 2) } });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Doctor)
                        RegisterSpellProcessor(_settings["TeamImprovedLifeChanneler"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TeamHealing(spell, fightingTarget, ref actionTarget, "TeamImprovedLifeChanneler"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.DoctorShortHPBuffs).Any(s => s.Id == HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "DoctorShortHPBuffs"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = Spell.GetSpellsForNanoline(NanoLine.DoctorShortHPBuffs).Select(s => s.Id).ToArray(), SettingKey = "DoctorShortHPBuffs", Label = "Doctor Short HP Buffs", UiType = UiType.DropDownInputBoxDropDown, Options = new List<(string Name, int Value)> { ("Off", 0), ("Self (HP%)", 1), ("Team/Raid (HP%)", 2), ("All Players (HP%)", 3), ("Buff", 4) } });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Doctor)
                        RegisterSpellProcessor(_settings["DoctorShortHPBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "DoctorShortHPBuffs"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (SpellID.DoctorHealOverTime.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.HOT.Any(c => c.SettingKey == "DoctorHealOverTime"))
                        HealWindowController.HOT.Add(new HealUiConfig { HealIds = SpellID.DoctorHealOverTime, SettingKey = "DoctorHealOverTime", Label = "Doctor Heal Over Time", UiType = UiType.DropDownInputBoxDropDown, Options = new List<(string Name, int Value)> { ("Off", 0), ("Heal (HP%)", 1), ("Buff", 2) } });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Doctor)
                        RegisterSpellProcessor(_settings["DoctorHealOverTime"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => HealOverTime(spell, fightingTarget, ref actionTarget, "DoctorHealOverTime"), _settings["DoctorHealOverTimeOption"].AsInt32() == 2 ? CombatActionPriority.VeryLow : CombatActionPriority.Ultra, _settings["DoctorHealOverTimeOption"].AsInt32() == 2 ? RuleContext.OutOfCombat : RuleContext.Both);
                }

                #endregion

                #region Fixer

                if (SpellID.UserFixerLongHoT.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.HOT.Any(c => c.SettingKey == "FixerLongHoTSelf"))
                        HealWindowController.HOT.Add(new HealUiConfig { HealIds = SpellID.UserFixerLongHoT, SettingKey = "FixerLongHoTSelf", Label = "Self Long Heal Over Time", UiType = UiType.DropDownWOption });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Fixer)
                        RegisterSpellProcessor(_settings["FixerLongHoTSelf"].AsInt32(), (Spell buffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => UserBuff(buffSpell, fightingTarget, ref actionTarget, "FixerLongHoTSelf"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                if (SpellID.TargetFixerLongHoT.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.HOT.Any(c => c.SettingKey == "FixerLongHoTTarget"))
                        HealWindowController.HOT.Add(new HealUiConfig { HealIds = SpellID.TargetFixerLongHoT, SettingKey = "FixerLongHoTTarget", Label = "Target Long Heal Over Time", UiType = UiType.DualDropDown, Options = BuffWindowController.BuffOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Fixer)
                        RegisterSpellProcessor(_settings["FixerLongHoTTarget"].AsInt32(), (Spell buffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetBuff(buffSpell, fightingTarget, ref actionTarget, "FixerLongHoTTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                if (SpellID.FixerShortHealOverTime.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.HOT.Any(c => c.SettingKey == "FixerShortHealOverTime"))
                        HealWindowController.HOT.Add(new HealUiConfig { HealIds = SpellID.FixerShortHealOverTime, SettingKey = "FixerShortHealOverTime", Label = "Fixer Short Heal Over Time", UiType = UiType.DropDownInputBoxDropDown, Options = new List<(string Name, int Value)> { ("Off", 0), ("Heal (HP%)", 1), ("Buff", 2) } });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Fixer)
                        RegisterSpellProcessor(_settings["FixerShortHealOverTime"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => HealOverTime(spell, fightingTarget, ref actionTarget, "FixerShortHealOverTime"), _settings["FixerShortHealOverTimeOption"].AsInt32() == 2 ? CombatActionPriority.VeryLow : CombatActionPriority.Ultra, _settings["FixerShortHealOverTimeOption"].AsInt32() == 2 ? RuleContext.OutOfCombat : RuleContext.Both);
                }

                #endregion

                #region MartialArtist

                if (SpellID.MartialArtistSingleTargetHealing.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "MartialArtistSingleTargetHealing"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.MartialArtistSingleTargetHealing, SettingKey = "MartialArtistSingleTargetHealing", Label = "Martial Artist Single Target Healing", UiType = UiType.DropDownInputBoxDropDown });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.MartialArtist)
                        RegisterSpellProcessor(_settings["MartialArtistSingleTargetHealing"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "MartialArtistSingleTargetHealing"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (SpellID.MartialArtistTeamHealing.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.AOEHeal.Any(c => c.SettingKey == "MartialArtistTeamHealing"))
                        HealWindowController.AOEHeal.Add(new HealUiConfig { HealIds = SpellID.MartialArtistTeamHealing, SettingKey = "MartialArtistTeamHealing", Label = "Martial Artist Team Healing", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.MartialArtist)
                        RegisterSpellProcessor(_settings["MartialArtistTeamHealing"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TeamHealing(spell, fightingTarget, ref actionTarget, "MartialArtistTeamHealing"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                #endregion

                #region NanoTechnician

                if (SpellID.NanoTechnicianNanoPointHeals.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Nano.Any(c => c.SettingKey == "NanoTechnicianNanoPointHeals"))
                        HealWindowController.Nano.Add(new HealUiConfig { HealIds = SpellID.NanoTechnicianNanoPointHeals, SettingKey = "NanoTechnicianNanoPointHeals", Label = "Nano Technician Nano Point Heals", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.NanoTechnician)
                        RegisterSpellProcessor(_settings["NanoTechnicianNanoPointHeals"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => NanoHeal(spell, fightingTarget, ref actionTarget, "NanoTechnicianNanoPointHeals"), CombatActionPriority.VeryHigh, RuleContext.InCombat);
                }

                #endregion

                #region Shade

                if (Spell.GetSpellsForNanoline(NanoLine.NemesisNanoPrograms).Any(s => s.Id == HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "NemesisNanoPrograms"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = Spell.GetSpellsForNanoline(NanoLine.NemesisNanoPrograms).Select(s => s.Id).ToArray(), SettingKey = "NemesisNanoPrograms", Label = "Shade's Caress", UiType = UiType.Checkbox });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Shade)
                        RegisterSpellProcessor(HealSpell.Id, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ShadesCaress(spell, fightingTarget, ref actionTarget), CombatActionPriority.Ultra, RuleContext.InCombat);
                }

                if (SpellID.ShadeHealthDrain.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.HOT.Any(c => c.SettingKey == "ShadeHealthDrain"))
                        HealWindowController.HOT.Add(new HealUiConfig { HealIds = SpellID.ShadeHealthDrain, SettingKey = "ShadeHealthDrain", Label = "Shade Health Drain", UiType = UiType.DropDownInputBoxDropDown, Options = new List<(string Name, int Value)> { ("Off", 0), ("Hot", 1), ("Heal (HP%)", 2), ("Dot", 3), ("Spam", 4) } });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Shade)
                        RegisterSpellProcessor(_settings["ShadeHealthDrain"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => HealthDrainDot(spell, fightingTarget, ref actionTarget, "ShadeHealthDrain"), _settings["ShadeHealthDrainOption"].AsInt32() == 2 ? CombatActionPriority.VeryLow : CombatActionPriority.Ultra);
                }

                #endregion

                #region Soldier

                if (SpellID.SoldierDrainHeal.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "SoldierDrainHeal"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.SoldierDrainHeal, SettingKey = "SoldierDrainHeal", Label = "Soldier Drain Heal", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Soldier)
                        RegisterSpellProcessor(_settings["SoldierDrainHeal"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => DrainHeal(spell, fightingTarget, ref actionTarget, "SoldierDrainHeal"), CombatActionPriority.Ultra, RuleContext.InCombat);
                }

                #endregion

                #region Trader

                if (SpellID.TraderDrainHeal.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "TraderDrainHeal"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.TraderDrainHeal, SettingKey = "TraderDrainHeal", Label = "Trader Drain Heal", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderDrainHeal"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => DrainHeal(spell, fightingTarget, ref actionTarget, "TraderDrainHeal"), CombatActionPriority.Ultra, RuleContext.InCombat);
                }

                if (SpellID.TraderHealthDrain.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "TraderHealthDrain"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.TraderHealthDrain, SettingKey = "TraderHealthDrain", Label = "Trader Health Drain", UiType = UiType.DropDownInputBoxDropDown, Options = new List<(string Name, int Value)> { ("Off", 0), ("Heal (HP%)", 1), ("Damage", 2) } });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderHealthDrain"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => HealthDrain(spell, fightingTarget, ref actionTarget, "TraderHealthDrain"), _settings["TraderHealthDrainOption"].AsInt32() == 2 ? CombatActionPriority.VeryLow : CombatActionPriority.Ultra);
                }

                if (SpellID.TraderHealthTarget.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Heals.Any(c => c.SettingKey == "TraderHealthTarget"))
                        HealWindowController.Heals.Add(new HealUiConfig { HealIds = SpellID.TraderHealthTarget, SettingKey = "TraderHealthTarget", Label = "Trader Health Target", UiType = UiType.DropDownInputBoxDropDown });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderHealthTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "TraderHealthTarget"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (SpellID.TraderHealthTeam.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.AOEHeal.Any(c => c.SettingKey == "TraderTeamHeals"))
                        HealWindowController.AOEHeal.Add(new HealUiConfig { HealIds = SpellID.TraderHealthTeam, SettingKey = "TraderTeamHeals", Label = "Trader Team Heals", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderTeamHeals"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TeamHealing(spell, fightingTarget, ref actionTarget, "TraderTeamHeals"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                if (SpellID.TraderNanoPointHeals.Contains(HealSpell.Id))
                {
                    if (!HealWindowController.Nano.Any(c => c.SettingKey == "TraderNanoPointHeals"))
                        HealWindowController.Nano.Add(new HealUiConfig { HealIds = SpellID.TraderNanoPointHeals, SettingKey = "TraderNanoPointHeals", Label = "Trader Nano Point Heals", UiType = UiType.DropDownInputBoxDropDown, Options = HealWindowController.AOEHealOptions });

                    LoadedHeals.Add(HealSpell.Id);

                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderNanoPointHeals"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => NanoHeal(spell, fightingTarget, ref actionTarget, "TraderNanoPointHeals"), CombatActionPriority.VeryHigh, RuleContext.InCombat);
                }

                #endregion

            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region target

        private bool TargetHealing(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[$"{setting}Option"].AsInt32() == 0) return false;
            if (_settings[$"{setting}Value"].AsInt32() == 0) return false;
            if (spell.Id != _settings[setting].AsInt32()) return false;
            if (!CanCast(spell)) return false;

            if (spell.Nanoline == NanoLine.MorphHeal)
                if (!DynelManager.LocalPlayer.Buffs.Contains(new[] { 217670, 25994 })) return false;

            actionTarget.Target = null;

            if ((spell.Id == SpellID.FountainOfLife || spell.Nanoline == NanoLine.CompleteHealingLine) && fightingTarget == null) return false;

            switch (_settings[$"{setting}Option"].AsInt32())
            {
                case 1:
                    if (DynelManager.LocalPlayer.Profession == Profession.Trader) return false;
                    if (DynelManager.LocalPlayer.HealthPercent <= _settings[$"{setting}Value"].AsInt32())
                        actionTarget.Target = DynelManager.LocalPlayer;

                    if (actionTarget.Target != null && (!((spell.Id == SpellID.FountainOfLife || spell.Nanoline == NanoLine.CompleteHealingLine) && actionTarget.Target.Buffs.Contains(204305))))
                    {
                        actionTarget = (actionTarget.Target, true);
                        return true;
                    }

                    return false;

                case 2:
                    if (Team.IsInTeam)
                    {
                        actionTarget.Target = Team.Members.FirstOrDefault(m => m.Character != null && m.Character.Health > 0 && m.Character.IsInLineOfSight && spell.IsInRange(m.Character) &&
                        !m.Character.Buffs.Contains(SpellID.PVPEnabled) && !(DynelManager.LocalPlayer.Profession == Profession.Trader && m.Character == DynelManager.LocalPlayer)
                        && m.Character.HealthPercent <= _settings[$"{setting}Value"].AsInt32())?.Character;

                        if (actionTarget.Target != null && (!((spell.Id == SpellID.FountainOfLife || spell.Nanoline == NanoLine.CompleteHealingLine) && actionTarget.Target.Buffs.Contains(204305))))
                        {
                            actionTarget = (actionTarget.Target, true);
                            return true;
                        }
                    }
                    else if (DynelManager.LocalPlayer.Profession != Profession.Trader && DynelManager.LocalPlayer.HealthPercent <= _settings[$"{setting}Value"].AsInt32())
                    {
                        actionTarget.Target = DynelManager.LocalPlayer;

                        if (actionTarget.Target != null && (!((spell.Id == SpellID.FountainOfLife || spell.Nanoline == NanoLine.CompleteHealingLine) && actionTarget.Target.Buffs.Contains(204305))))
                        {
                            actionTarget = (actionTarget.Target, true);
                            return true;
                        }
                    }

                    return false;

                case 3:
                    actionTarget.Target = DynelManager.Players.FirstOrDefault(p => p != null && p.Health > 0 && p.IsInLineOfSight && spell.IsInRange(p) && !p.Buffs.Contains(SpellID.PVPEnabled)
                    && !(DynelManager.LocalPlayer.Profession == Profession.Trader && p == DynelManager.LocalPlayer) && p.HealthPercent <= _settings[$"{setting}Value"].AsInt32());

                    if (actionTarget.Target != null && (!((spell.Id == SpellID.FountainOfLife || spell.Nanoline == NanoLine.CompleteHealingLine) && actionTarget.Target.Buffs.Contains(204305))))
                    {
                        actionTarget = (actionTarget.Target, true);
                        return true;
                    }

                    return false;
                case 4:
                    if (Team.IsInTeam)
                    {
                        actionTarget.Target = Team.Members.FirstOrDefault(m => m.Character != null && m.Character.Health > 0 && m.Character.IsInLineOfSight && spell.IsInRange(m.Character)
                        && SpellCheckLocalTeam(spell, m.Character) && !m.Character.Buffs.Contains(SpellID.PVPEnabled) && !m.Character.Buffs.Contains(spell.Nanoline))?.Character;
                        if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                    }
                    else if (!DynelManager.LocalPlayer.Buffs.Contains(spell.Nanoline))
                    {
                        actionTarget.Target = DynelManager.LocalPlayer;
                        actionTarget = (actionTarget.Target, true);
                        return true;
                    }

                    return false;

                default:
                    return false;
            }
        }

        private bool HealPet(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Value"].AsInt32() == 0) return false;
            if (!CanLookupPetsAfterZone()) return false;
            if (!CanCast(spell)) return false;

            var pet = DynelManager.LocalPlayer.Pets.Where(p => p.Character != null && p.Character.Health > 0 && p.Character.Breed == Breed.HumanMonster
            && p.Character.HealthPercent <= _settings[setting + "Value"].AsInt32() && InCastRange(spell, p.Character) && p.Character.IsInLineOfSight).FirstOrDefault()?.Character;

            if (pet != null)
            {
                actionTarget = (pet, true);
                return true;
            }

            return false;
        }

        private bool DrainHeal(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Value"].AsInt32() == 0) return false;
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (!Attacked) return false;

            if (DynelManager.LocalPlayer.HealthPercent > _settings[setting + "Value"].AsInt32()) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }

        private bool HealthDrain(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Value"].AsInt32() == 0) return false;
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;

            switch (_settings[setting + "Option"].AsInt32())
            {
                case 1:
                    if (_settings[setting + "Value"].AsInt32() == 0) return false;
                    if (DynelManager.LocalPlayer.HealthPercent > _settings[setting + "Value"].AsInt32()) return false;
                    actionTarget = (fightingTarget, true);
                    return true;
                case 2:
                    if (Spell.HasPendingCast) return false;
                    actionTarget = (fightingTarget, true);
                    return true;
                default:
                    return false;
            }
        }

        private bool HealthDrainDot(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;

            int Hot = 0;

            switch (spell.Id)
            {
                case 273390://Sneaking Health Drain
                    Hot = 273391;
                    break;

                case 301895://Dissolving Vitality
                    Hot = 301894;
                    break;
            }

            if (Hot == 0) return false;

            switch (_settings[setting + "Option"].AsInt32())
            {
                case 1:// Hot
                    if (DynelManager.LocalPlayer.Buffs.Contains(Hot)) return false;
                    actionTarget = (fightingTarget, true);
                    return true;
                case 2:// Heal at HP
                    if (DynelManager.LocalPlayer.HealthPercent > _settings[setting + "Value"].AsInt32()) return false;
                    actionTarget = (fightingTarget, true);
                    return true;
                case 3:// Dot
                    if (fightingTarget.Buffs.Contains(spell.Id)) return false;
                    actionTarget = (fightingTarget, true);
                    return true;
                case 4:// Spam
                    actionTarget = (fightingTarget, true);
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Team

        private bool TeamHealing(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[$"{setting}Option"].AsInt32() == 0) return false;
            if (_settings[$"{setting}Value"].AsInt32() == 0) return false;
            if (spell.Id != _settings[setting].AsInt32()) return false;
            if (!CanCast(spell)) return false;

            switch (_settings[$"{setting}Option"].AsInt32())
            {
                case 1:
                    if (Team.IsInTeam)
                    {
                        var local = Team.Members.Find(x => x.Identity == DynelManager.LocalPlayer.Identity);
                        if (local == null) return false;

                        var members = Team.Members.Where(m => m.Character != null && m.Character.Health > 0 && m.Character.IsInLineOfSight && spell.IsInRange(m.Character) && (!Team.IsRaid || m.TeamIndex == local.TeamIndex)
                        && !m.Character.Buffs.Contains(SpellID.PVPEnabled) && m.Character.HealthPercent <= _settings[$"{setting}Value"].AsInt32()).ToList();

                        if (members != null && members.Count >= 2)
                        {
                            //Chat.WriteLine($"[{spell.Name}] triggered: {members.Count} members below {_settings[$"{setting}Value"].AsInt32()}%");
                            // foreach (var m in members)
                            // Chat.WriteLine($" - {m.Character.Name} ({m.Character.HealthPercent}%)");
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                    }

                    else if (DynelManager.LocalPlayer.HealthPercent <= _settings[$"{setting}Value"].AsInt32())
                    { actionTarget = (DynelManager.LocalPlayer, true); return true; }

                    return false;

                case 2:
                    if (fightingTarget == null) return false;
                    if (Team.IsInTeam)
                    {
                        var local = Team.Members.Find(x => x.Identity == DynelManager.LocalPlayer.Identity);
                        if (local == null) return false;

                        var member = Team.Members.Find(m => m.Character != null && m.Character.Health > 0 && m.Character.IsInLineOfSight && spell.IsInRange(m.Character) &&
                            (!Team.IsRaid || m.TeamIndex == local.TeamIndex) && SpellCheckLocalTeam(spell, m.Character) && !m.Character.Buffs.Contains(NanoLine.DoctorShortHPBuffs))?.Character;

                        if (member != null)
                        {
                            //Chat.WriteLine($"[TeamHealing method] {member.Name} missing buff, {spell.Nanoline}");
                            actionTarget = (DynelManager.LocalPlayer, true);
                            return true;
                        }
                    }

                    else if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.DoctorShortHPBuffs))
                    { actionTarget = (DynelManager.LocalPlayer, true); return true; }

                    return false;

                default:
                    return false;

            }
        }

        #endregion

        #region HOT

        private bool HealOverTime(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;

            actionTarget.Target = null;

            if (spell.Id == 273352 || spell.Id == 269455)
                actionTarget.Target = DynelManager.LocalPlayer;

            switch (_settings[setting + "Option"].AsInt32())
            {
                case 1://heal
                    {
                        if (_settings[setting + "Value"].AsInt32() != 0)
                        {
                            if (Team.IsInTeam)
                            {
                                var member = Team.Members.Where(m => m.Character != null && m.Character.Health > 0 && m.Character.IsInLineOfSight
                                && spell.IsInRange(m.Character) 
                                && !m.Character.Buffs.Contains(SpellID.PVPEnabled) && !m.Character.Buffs.Contains(new[] { NanoLine.HealOverTime, NanoLine.MongoBuff })
                                && m.Character.HealthPercent <= _settings[setting + "Value"].AsInt32()).FirstOrDefault()?.Character;

                                if (member != null)
                                {
                                    if (actionTarget.Target != null)
                                        actionTarget = (actionTarget.Target, false);
                                    else
                                    {
                                        actionTarget = (member, true);
                                        return true;
                                    }
                                }
                            }
                            else if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.HealOverTime) && DynelManager.LocalPlayer.HealthPercent <= _settings[setting + "Value"].AsInt32())
                            {
                                actionTarget.Target = DynelManager.LocalPlayer;
                                if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                            }
                        }
                    }
                    return false;
                case 2://buff
                    {
                        if (Team.IsInTeam)
                        {
                            var local = Team.Members.Find(x => x.Identity == DynelManager.LocalPlayer.Identity);
                            if (local == null) return false;

                            var member = Team.Members.Find(m => m.Character != null && m.Character.Health > 0 && m.Character.IsInLineOfSight && spell.IsInRange(m.Character) && (!Team.IsRaid || m.TeamIndex == local.TeamIndex)
                            && SpellCheckLocalTeam(spell, m.Character) && !m.Character.Buffs.Contains(SpellID.PVPEnabled) && !m.Character.Buffs.Contains(NanoLine.MongoBuff))?.Character;

                            if (member != null)
                            {
                                if (actionTarget.Target != null)
                                    actionTarget = (actionTarget.Target, false);
                                else
                                    actionTarget = (member, true);
                                return true;
                            }
                        }
                        else if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.HealOverTime))
                        {
                            actionTarget.Target = DynelManager.LocalPlayer;
                            if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                        }
                    }
                    return false;
                default:
                    return false;
            }
        }

        #endregion

        private bool NanoHeal(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[setting + "Option"].AsInt32() == 0) return false;
            if (_settings[setting + "Value"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;

            if (spell.Id != 275024)//Izgimmer's Wealth
                if (Team.IsInTeam)
                {
                    var local = Team.Members.Find(x => x.Identity == DynelManager.LocalPlayer.Identity);
                    if (local == null) return false;

                    var member = Team.Members.Find(m => m.Character != null && m.Character.Health > 0 && m.Character.IsInLineOfSight && spell.IsInRange(m.Character) && (!Team.IsRaid || m.TeamIndex == local.TeamIndex)
                    && SpellCheckLocalTeam(spell, m.Character) && !m.Character.Buffs.Contains(SpellID.PVPEnabled) && !m.Character.Buffs.Contains(NanoLine.NanoPointHeals)
                    && m.Character.NanoPercent <= _settings[setting + "Value"].AsInt32())?.Character;

                    if (member != null) { actionTarget = (DynelManager.LocalPlayer, true); return true; }
                }

            if (DynelManager.LocalPlayer.NanoPercent <= _settings[setting + "Value"].AsInt32() && !DynelManager.LocalPlayer.Buffs.Contains(NanoLine.NanoPointHeals))
            {
                actionTarget.Target = DynelManager.LocalPlayer;

                if (actionTarget.Target != null) { actionTarget = (DynelManager.LocalPlayer, true); return true; }
            }

            return false;
        }

        #region Health Heal

        private bool SelfHealPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!CanUsePerk(perkAction)) return false;

            if (DynelManager.LocalPlayer.HealthPercent > _settings[setting].AsInt32()) return false;

            actionTarget = (DynelManager.LocalPlayer, false);
            return true;
        }

        private bool TargetHealPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!CanUsePerk(perkAction)) return false;

            actionTarget.Target = null;

            if (Team.IsInTeam)
                actionTarget.Target = Team.Members.FirstOrDefault(m => m.Character != null && m.Character?.HealthPercent <= _settings[setting].AsInt32())?.Character;
            else if (DynelManager.LocalPlayer.HealthPercent <= _settings[setting].AsInt32())
                actionTarget.Target = DynelManager.LocalPlayer;

            if (actionTarget.Target == null) return false;

            actionTarget.ShouldSetTarget = true;
            return true;
        }

        private bool TeamHealPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!CanUsePerk(perkAction)) return false;

            bool shouldCast = false;

            switch (perkAction.Hash)
            {
                case PerkHash.BioRejuvenation:
                    if (!_settings["AOE"].AsBool()) return false;
                    break;
            }
            if (Team.IsInTeam)
                shouldCast = Team.Members.Any(m => m != null && m.Character?.HealthPercent <= _settings[setting].AsInt32());
            else
                shouldCast = DynelManager.LocalPlayer.HealthPercent <= _settings[setting].AsInt32();

            actionTarget.ShouldSetTarget = false;
            return shouldCast;
        }

        private bool LifeTapPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;

            if (DynelManager.LocalPlayer.HealthPercent > _settings[setting].AsInt32()) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        #endregion

        #region Nano Heal

        private bool NanoHealSelf(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (DynelManager.LocalPlayer.NanoPercent > _settings[setting].AsInt32()) return false;

            actionTarget = (DynelManager.LocalPlayer, true);
            return true;
        }

        private bool NanoHealTarget(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!CanUsePerk(perkAction)) return false;

            actionTarget.Target = null;

            if (Team.IsInTeam)
                actionTarget.Target = Team.Members.FirstOrDefault(m => m.Character != null && m.Character.NanoPercent <= _settings[setting].AsInt32())?.Character;
            else if (DynelManager.LocalPlayer.NanoPercent <= _settings[setting].AsInt32())
                actionTarget.Target = DynelManager.LocalPlayer;

            if (actionTarget.Target == null) return false;

            actionTarget.ShouldSetTarget = true;
            return true;
        }

        private bool NanoHealTeam(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!CanUsePerk(perkAction)) return false;

            bool shouldUsePerk = false;

            if (Team.IsInTeam)
                shouldUsePerk = Team.Members.Any(m => m.Character != null && m.Character.NanoPercent <= _settings[setting].AsInt32());
            else
                shouldUsePerk = DynelManager.LocalPlayer.NanoPercent <= _settings[setting].AsInt32();

            if (!shouldUsePerk) return false;

            actionTarget = (DynelManager.LocalPlayer, true);
            return true;
        }

        #endregion

        #region Shade

        private bool ShadesCaress(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, int value = 50)
        {
            if (!_settings["NemesisNanoPrograms"].AsBool()) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;
            if (fightingTarget.HealthPercent < 5) return false;
            if (fightingTarget.Buffs.Contains(275242)) return false;

            if (DynelManager.LocalPlayer.HealthPercent <= value)
            {
                var healPerks = new List<PerkHash> { PerkHash.DevourVigor, PerkHash.DevourEssence, PerkHash.DevourVitality, PerkHash.Diffuse, PerkHash.ConsumeTheSoul, PerkHash.Exultation };

                if (PerkAction.List.Any(p => healPerks.Contains(p.Hash) && p.IsAvailable)) return false;

                actionTarget = (fightingTarget, true);
                return true;
            }

            if (Team.IsInTeam)
            {
                var MemberCountNeedingHealth = Team.Members.Count(m => m.Character != null && m.Character.HealthPercent <= value);

                if (MemberCountNeedingHealth > 2)
                {
                    actionTarget = (fightingTarget, true);
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
