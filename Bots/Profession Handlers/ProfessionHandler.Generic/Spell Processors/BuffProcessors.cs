using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public static HashSet<int> LoadedNonCombatBuffs = new HashSet<int>();

        public static HashSet<int> LoadedNonCombatAdventurerBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatBureaucratBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatDoctorBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatEnforcerBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatEngineerBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatFixerBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatKeeperBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatMartialArtistBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatMetaphysicistBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatNanoTechnicianBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatShadeBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatSoldierBuffs = new HashSet<int>();
        public static HashSet<int> LoadedNonCombatTraderBuffs = new HashSet<int>();

        private HashSet<int> LoadedWeaponSpells = new HashSet<int>();

        private void RegisterBuffSpells(int spellID)
        {
            try
            {
                #region Generic

                if (spellID == SpellID.CompositeAttribute)
                {
                    LoadedNonCombatBuffs.Add(spellID);
                    RegisterSpellProcessor(SpellID.CompositeAttribute, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        CompositeBuff(spell, fightingTarget, ref actionTarget, "CompositeAttribute"), CombatActionPriority.Ultra, RuleContext.OutOfCombat);
                }

                if (spellID == SpellID.CompositeNano)
                {
                    LoadedNonCombatBuffs.Add(spellID);
                    LoadedPetBuffSpells.Add(spellID);
                    RegisterSpellProcessor(SpellID.CompositeNano, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        CompositeBuff(spell, fightingTarget, ref actionTarget, "CompositeNano"), CombatActionPriority.VeryHigh, RuleContext.OutOfCombat);
                }

                if (spellID == SpellID.CompositeUtility)
                {
                    LoadedNonCombatBuffs.Add(spellID);
                    RegisterSpellProcessor(SpellID.CompositeUtility, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        CompositeBuff(spell, fightingTarget, ref actionTarget, "CompositeUtility"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (spellID == SpellID.CompositeMartialProwess)
                {
                    LoadedNonCombatBuffs.Add(spellID);
                    LoadedPetBuffSpells.Add(spellID);
                    RegisterSpellProcessor(SpellID.CompositeMartialProwess, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        CompositeBuff(spell, fightingTarget, ref actionTarget, "CompositeMartialProwess"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (spellID == SpellID.CompositeMelee)
                {
                    LoadedNonCombatBuffs.Add(spellID);
                    LoadedPetBuffSpells.Add(spellID);
                    RegisterSpellProcessor(SpellID.CompositeMelee, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        CompositeBuff(spell, fightingTarget, ref actionTarget, "CompositeMelee"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (spellID == SpellID.CompositePhysicalSpecial)
                {
                    LoadedNonCombatBuffs.Add(spellID);
                    LoadedPetBuffSpells.Add(spellID);
                    RegisterSpellProcessor(SpellID.CompositePhysicalSpecial, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        CompositeBuff(spell, fightingTarget, ref actionTarget, "CompositePhysicalSpecial"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (spellID == SpellID.CompositeRanged)
                {
                    LoadedNonCombatBuffs.Add(spellID);
                    RegisterSpellProcessor(SpellID.CompositeRanged, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        CompositeBuff(spell, fightingTarget, ref actionTarget, "CompositeRanged"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (spellID == SpellID.CompositeRangedSpecial)
                {
                    LoadedNonCombatBuffs.Add(spellID);
                    RegisterSpellProcessor(SpellID.CompositeRangedSpecial, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        CompositeBuff(spell, fightingTarget, ref actionTarget, "CompositeRangedSpecial"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (spellID == SpellID.InsightIntoSL)
                {
                    LoadedNonCombatBuffs.Add(spellID);
                    RegisterSpellProcessor(SpellID.InsightIntoSL, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    TargetBuff(spell, fightingTarget, ref actionTarget, "SLMap"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }
                #endregion

                #region Agent
                if (DynelManager.LocalPlayer.Profession == Profession.Agent)
                {
                    if (SpellID.UserAgilityBuffAgent.Contains(spellID))
                    {
                        if (!BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserAgilityBuffAgent"))
                            BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserAgilityBuffAgent, SettingKey = "UserAgilityBuffAgent", Label = "Agility Buff", UiType = UiType.DropDownWOption });

                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["UserAgilityBuffAgent"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        UserBuff(spell, fightingTarget, ref actionTarget, "UserAgilityBuffAgent"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                    }

                    if (SpellID.TargetAgilityBuff.Contains(spellID))
                    {
                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["AgilityBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetBuff(spell, fightingTarget, ref actionTarget, "AgilityBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                    }

                    if (Spell.GetSpellsForNanoline(NanoLine.SenseBuff).Any(s => s.Id == spellID))
                    {
                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["SenseBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetBuff(spell, fightingTarget, ref actionTarget, "SenseBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                    }

                    if (SpellID.UserAimedShotBuffsAgent.Contains(spellID))
                    {
                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["AimedShotBuffsSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        UserBuff(spell, fightingTarget, ref actionTarget, "AimedShotBuffsSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                    }

                    if (SpellID.TargetAimedShotBuffsAgent.Contains(spellID))
                    {
                        if (!BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetAimedShotBuffsAgent"))
                            BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetAimedShotBuffsAdventurer, SettingKey = "TargetAimedShotBuffsAgent", Label = "Aimed Shot Buffs", UiType = UiType.DualDropDown });

                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["TargetAimedShotBuffsAgent"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetBuff(spell, fightingTarget, ref actionTarget, "TargetAimedShotBuffsAgent"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                    }

                    if (Spell.GetSpellsForNanoline(NanoLine.AgentDamageProc_DamageInflictSegment).Any(s => s.Id == spellID))
                    {
                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["AgentDamageProc_DamageInflictSegment"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        UserBuff(spell, fightingTarget, ref actionTarget, "AgentDamageProc_DamageInflictSegment"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                    }

                    if (Spell.GetSpellsForNanoline(NanoLine.AgentProcBuff).Any(s => s.Id == spellID))
                    {
                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["AgentProcBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        UserBuff(spell, fightingTarget, ref actionTarget, "AgentProcBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                    }

                    if (Spell.GetSpellsForNanoline(NanoLine.ExecutionerBuff).Any(s => s.Id == spellID))
                    {
                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["ExecutionerBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        UserBuff(spell, fightingTarget, ref actionTarget, "ExecutionerBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                    }

                    if (SpellID.UserConcealmentBuffAgent.Contains(spellID))
                    {
                        if (!BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserConcealmentBuffAgent"))
                            BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserConcealmentBuffAgent, SettingKey = "UserConcealmentBuffAgent", Label = "Concealment Buff", UiType = UiType.DropDownWOption });

                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["UserConcealmentBuffAgent"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        UserBuff(spell, fightingTarget, ref actionTarget, "UserConcealmentBuffAgent"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                    }

                    if (SpellID.TargetConcealmentBuffAgent.Contains(spellID))
                    {
                        if (!BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetConcealmentBuffAgent"))
                            BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetConcealmentBuffAgent, SettingKey = "TargetConcealmentBuffAgent", Label = "Concealment Buff", UiType = UiType.DualDropDown });

                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["TargetConcealmentBuffAgent"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetBuff(spell, fightingTarget, ref actionTarget, "TargetConcealmentBuffAgent"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                    }

                    if (SpellID.TeamConcealmentBuffAgent.Contains(spellID))
                    {
                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["ConcealmentBuffTeam"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TeamBuff(spell, fightingTarget, ref actionTarget, "ConcealmentBuffTeam"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                    }

                    if (SpellID.UserCriticalIncreaseBuffAgent.Contains(spellID))
                    {
                        if (!BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserCriticalIncreaseBuffAgent"))
                            BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserCriticalIncreaseBuffAgent, SettingKey = "UserCriticalIncreaseBuffAgent", Label = "Critical Increase Buff", UiType = UiType.DropDownWOption });

                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["UserCriticalIncreaseBuffAgent"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        UserBuff(spell, fightingTarget, ref actionTarget, "UserCriticalIncreaseBuffAgent"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                    }

                    if (SpellID.TargetCriticalIncreaseBuffAgent.Contains(spellID))
                    {
                        if (!BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetCriticalIncreaseBuffAgent"))
                            BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetCriticalIncreaseBuffAgent, SettingKey = "TargetCriticalIncreaseBuffAgent", Label = "Critical Increase Buff", UiType = UiType.DualDropDown });

                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["TargetCriticalIncreaseBuffAgent"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetBuff(spell, fightingTarget, ref actionTarget, "TargetCriticalIncreaseBuffAgent"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                    }

                    if (SpellID.TargetDamageBuffs_LineAAgent.Contains(spellID))
                    {
                        if (!BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageBuffs_LineAAgent"))
                            BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetDamageBuffs_LineAAgent, SettingKey = "TargetDamageBuffs_LineAAgent", Label = "Damage Buffs - Line A", UiType = UiType.DualDropDown });

                        LoadedNonCombatBuffs.Add(spellID);
                        if (!LoadedPetBuffSpells.Contains(spellID))
                            LoadedPetBuffSpells.Add(spellID);
                        RegisterSpellProcessor(SpellID.TargetDamageBuffs_LineAAgent, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetBuff(spell, fightingTarget, ref actionTarget, "TargetDamageBuffs_LineAAgent"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                    }

                    if (SpellID.TargetRifleBuffsAgent.Contains(spellID))
                    {
                        if (!BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetRifleBuffsAgent"))
                            BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetRifleBuffsAgent, SettingKey = "TargetRifleBuffsAgent", Label = "Rifle Buffs", UiType = UiType.DualDropDown });

                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["TargetRifleBuffsAgent"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetBuff(spell, fightingTarget, ref actionTarget, "TargetRifleBuffsAgent"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                    }

                    if (Spell.GetSpellsForNanoline(NanoLine.ShadowlandReflectBase).Any(s => s.Id == spellID))
                    {
                        LoadedNonCombatBuffs.Add(spellID);
                        RegisterSpellProcessor(_settings["ShadowlandReflectBaseTeam"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TeamBuff(spell, fightingTarget, ref actionTarget, "ShadowlandReflectBaseTeam"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                    }

                    if (SpellID.FalseProfession_Adventurer.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "Adventurer"))
                    {
                        _FPDefinitions.Add(("Adventurer", AdventurerBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_Bureaucrat.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "Bureaucrat"))
                    {
                        _FPDefinitions.Add(("Bureaucrat", BureaucratBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_Doctor.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "Doctor"))
                    {
                        _FPDefinitions.Add(("Doctor", DoctorBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_Enforcer.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "Enforcer"))
                    {
                        _FPDefinitions.Add(("Enforcer", EnforcerBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_Engineer.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "Engineer"))
                    {
                        _FPDefinitions.Add(("Engineer", EngineerBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_Fixer.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "Fixer"))
                    {
                        _FPDefinitions.Add(("Fixer", FixerBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_MartialArtist.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "MartialArtist"))
                    {
                        _FPDefinitions.Add(("MartialArtist", MartialArtistBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_Metaphysicist.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "Metaphysicist"))
                    {
                        _FPDefinitions.Add(("Metaphysicist", MetaphysicistBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_NanoTechnician.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "NanoTechnician"))
                    {
                        _FPDefinitions.Add(("NanoTechnician", NanoTechnicianBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_Soldier.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "Soldier"))
                    {
                        _FPDefinitions.Add(("Soldier", SoldierBuffs_Button_Clicked));
                    }
                        

                    if (SpellID.FalseProfession_Trader.Contains(spellID) && !_FPDefinitions.Any(x => x.Label == "Trader"))
                    {
                        _FPDefinitions.Add(("Trader", TraderBuffs_Button_Clicked));
                    }
                        
                }

                #endregion

                switch ((Profession)DynelManager.LocalPlayer.GetStat(Stat.VisualProfession))
                {
                    #region Adventurer
                        case Profession.Adventurer:
                        if (Spell.GetSpellsForNanoline(NanoLine.DamageShieldUpgrades).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["DamageShieldUpgrades"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "DamageShieldUpgrades"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SelfRoot_SnareResistBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "SelfRoot_SnareResistBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.OtherRoot_SnareResistBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["OtherRoot_SnareResistBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "OtherRoot_SnareResistBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine._1HEdgedBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["_1HEdgedBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "_1HEdgedBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetRunspeedBuffsAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetRunspeedBuffsAdventurer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetRunspeedBuffsAdventurer, SettingKey = "TargetRunspeedBuffsAdventurer", Label = "Runspeed Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetRunspeedBuffsAdventurer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetRunspeedBuffsAdventurer, SettingKey = "TargetRunspeedBuffsAdventurer", Label = "Runspeed Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetRunspeedBuffsAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetRunspeedBuffsAdventurer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.TeamRunSpeedBuffs).Any(s => s.Id == spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamRunSpeedBuffs"))
                                BuffWindowController.TeamBuff.Add(new BuffUiConfig { BuffIDs = Spell.GetSpellsForNanoline(NanoLine.TeamRunSpeedBuffs).Select(s => s.Id).ToArray(), SettingKey = "TeamRunSpeedBuffs", Label = "Team Run Speed Buffs", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamRunSpeedBuffs"))
                                FPBuffWindowController.TeamBuff.Add(new FPBuffUiConfig { FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.TeamRunSpeedBuffs).Select(s => s.Id).ToArray(), SettingKey = "TeamRunSpeedBuffs", Label = "Team Run Speed Buffs", UiType = UiType.DropDownWOption });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TeamRunSpeedBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "TeamRunSpeedBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserSneakAttackBuffs.Contains(spellID))
                        {
                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SneakAttackBuffsSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "SneakAttackBuffsSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetSneakAttackBuffs.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetSneakAttackBuffs"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetSneakAttackBuffs, SettingKey = "TargetSneakAttackBuffs", Label = "Sneak Attack Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetSneakAttackBuffs"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetSneakAttackBuffs, SettingKey = "TargetSneakAttackBuffs", Label = "Sneak Attack Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetSneakAttackBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetSneakAttackBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetAimedShotBuffsAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetAimedShotBuffsAdventurer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetAimedShotBuffsAgent, SettingKey = "TargetAimedShotBuffsAdventurer", Label = "Aimed Shot Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetAimedShotBuffsAdventurer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetAimedShotBuffsAgent, SettingKey = "TargetAimedShotBuffsAdventurer", Label = "Aimed Shot Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetAimedShotBuffsAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetAimedShotBuffsAdventurer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetArmorBuffAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetArmorBuffAdventurer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetArmorBuffAdventurer, SettingKey = "TargetArmorBuffAdventurer", Label = "Armor Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetArmorBuffAdventurer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetArmorBuffAdventurer, SettingKey = "TargetArmorBuffAdventurer", Label = "Armor Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetArmorBuffAdventurer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetArmorBuffAdventurer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserDamageBuffs_LineAAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageBuffs_LineAAdventurer"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserDamageBuffs_LineAAdventurer, SettingKey = "UserDamageBuffs_LineAAdventurer", Label = "Damage Buffs - Line A", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageBuffs_LineAAdventurer"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserDamageBuffs_LineAAdventurer, SettingKey = "UserDamageBuffs_LineAAdventurer", Label = "Damage Buffs - Line A", UiType = UiType.DropDownWOption });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserDamageBuffs_LineAAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserDamageBuffs_LineAAdventurer"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserDamageShieldsAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageShieldsAdventurer"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserDamageShieldsAdventurer, SettingKey = "UserDamageShieldsAdventurer", Label = "Damage Shields", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageShieldsAdventurer"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserDamageShieldsAdventurer, SettingKey = "UserDamageShieldsAdventurer", Label = "Damage Shields", UiType = UiType.DropDownWOption });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserDamageShieldsAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserDamageShieldsAdventurer"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetDamageShieldsAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageShieldsAdventurer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetDamageShieldsAdventurer, SettingKey = "TargetDamageShieldsAdventurer", Label = "Damage Shields", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageShieldsAdventurer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetDamageShieldsAdventurer, SettingKey = "TargetDamageShieldsAdventurer", Label = "Damage Shields", UiType = UiType.DualDropDown });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetDamageShieldsAdventurer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetDamageShieldsAdventurer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TeamExperienceConstructs_XPBonusAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamExperienceConstructs_XPBonusAdventurer"))
                                BuffWindowController.TeamBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TeamExperienceConstructs_XPBonusAdventurer, SettingKey = "TeamExperienceConstructs_XPBonusAdventurer", Label = "Experience Constructs - XP Bonus", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamExperienceConstructs_XPBonusAdventurer"))
                                FPBuffWindowController.TeamBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TeamExperienceConstructs_XPBonusAdventurer, SettingKey = "TeamExperienceConstructs_XPBonusAdventurer", Label = "Experience Constructs - XP Bonus", UiType = UiType.DropDownWOption });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TeamExperienceConstructs_XPBonusAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "TeamExperienceConstructs_XPBonusAdventurer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetFirstAidAndTreatmentBuffAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetFirstAidAndTreatmentBuffAdventurer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetFirstAidAndTreatmentBuffAdventurer, SettingKey = "TargetFirstAidAndTreatmentBuffAdventurer", Label = "First Aid And Treatment Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetFirstAidAndTreatmentBuffAdventurer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetFirstAidAndTreatmentBuffAdventurer, SettingKey = "TargetFirstAidAndTreatmentBuffAdventurer", Label = "First Aid And Treatment Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetFirstAidAndTreatmentBuffAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetFirstAidAndTreatmentBuffAdventurer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TeamFortifyAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamFortifyAdventurer"))
                                BuffWindowController.TeamBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TeamFortifyAdventurer, SettingKey = "TeamFortifyAdventurer", Label = "Fortify", UiType = UiType.Checkbox });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamFortifyAdventurer"))
                                FPBuffWindowController.TeamBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TeamFortifyAdventurer, SettingKey = "TeamFortifyAdventurer", Label = "Fortify", UiType = UiType.Checkbox });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(SpellID.TeamFortifyAdventurer, MorphFortifyBuff, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserNanoResistanceBuffsAdventurer.Contains(spellID))
                        {
                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserNanoResistanceBuffsAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserNanoResistanceBuffsAdventurer"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetPerceptionBuffsAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetPerceptionBuffsAdventurer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetPerceptionBuffsAdventurer, SettingKey = "TargetPerceptionBuffsAdventurer", Label = "Perception Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetPerceptionBuffsAdventurer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetPerceptionBuffsAdventurer, SettingKey = "TargetPerceptionBuffsAdventurer", Label = "Perception Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetPerceptionBuffsAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetPerceptionBuffsAdventurer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserPistolBuffAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserPistolBuffAdventurer"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserPistolBuffAdventurer, SettingKey = "UserPistolBuffAdventurer", Label = "Pistol Buff", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserPistolBuffAdventurer"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserPistolBuffAdventurer, SettingKey = "UserPistolBuffAdventurer", Label = "Pistol Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserPistolBuffAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserPistolBuffAdventurer"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMultiwieldBuffAdventurer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Adventurer && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMultiwieldBuffAdventurer"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserMultiwieldBuffAdventurer, SettingKey = "UserMultiwieldBuffAdventurer", Label = "Multiwield Buff", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMultiwieldBuffAdventurer"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserMultiwieldBuffAdventurer, SettingKey = "UserMultiwieldBuffAdventurer", Label = "Multiwield Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatAdventurerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserMultiwieldBuffAdventurer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserMultiwieldBuffAdventurer"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Adventurer)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatAdventurerBuffs);
                        break;
                    #endregion

                    #region Bureaucrat
                    case Profession.Bureaucrat:
                        if (SpellID.TargetPsy_IntBuff.Contains(spellID))
                        {
                            LoadedNonCombatBureaucratBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["Psy_IntBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "Psy_IntBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetConcealmentBuffBureaucrat.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Bureaucrat && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetConcealmentBuffBureaucrat"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetConcealmentBuffBureaucrat, SettingKey = "TargetConcealmentBuffBureaucrat", Label = "Concealment Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetConcealmentBuffBureaucrat"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetConcealmentBuffBureaucrat, SettingKey = "TargetConcealmentBuffBureaucrat", Label = "Concealment Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatBureaucratBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetConcealmentBuffBureaucrat"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetConcealmentBuffBureaucrat"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TeamExperienceConstructs_XPBonusBureaucrat.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Bureaucrat && !BuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamExperienceConstructs_XPBonusBureaucrat"))
                                BuffWindowController.TeamBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TeamExperienceConstructs_XPBonusBureaucrat, SettingKey = "TeamExperienceConstructs_XPBonusBureaucrat", Label = "Experience Constructs - XP Bonus", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamExperienceConstructs_XPBonusBureaucrat"))
                                FPBuffWindowController.TeamBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TeamExperienceConstructs_XPBonusBureaucrat, SettingKey = "TeamExperienceConstructs_XPBonusBureaucrat", Label = "Experience Constructs - XP Bonus", UiType = UiType.DropDownWOption });

                            LoadedNonCombatBureaucratBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TeamExperienceConstructs_XPBonusBureaucrat"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "TeamExperienceConstructs_XPBonusBureaucrat"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.NanoDeltaBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatBureaucratBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["NanoDeltaBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "NanoDeltaBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.PsychologyBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatBureaucratBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["PsychologyBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "PsychologyBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.CriticalDecreaseBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatBureaucratBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(_settings["CriticalDecreaseBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "CriticalDecreaseBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.SpeechLine).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatBureaucratBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SpeechLine"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "SpeechLine"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserPistolBureaucrat.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Bureaucrat && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserPistolBureaucrat"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserPistolBureaucrat, SettingKey = "UserPistolBureaucrat", Label = "Pistol Buff", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserPistolBureaucrat"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserPistolBureaucrat, SettingKey = "UserPistolBureaucrat", Label = "Pistol Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatBureaucratBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserPistolBureaucrat"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserPistolBureaucrat"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetPistolBureaucrat.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Bureaucrat && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetPistolBureaucrat"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetPistolBureaucrat, SettingKey = "TargetPistolBureaucrat", Label = "Pistol Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetPistolBureaucrat"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetPistolBureaucrat, SettingKey = "TargetPistolBureaucrat", Label = "Pistol Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatBureaucratBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetPistolBureaucrat"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetPistolBureaucrat"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Bureaucrat)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatBureaucratBuffs);
                        break;
                    #endregion

                    #region Doctor
                    case Profession.Doctor:
                        if (SpellID.TargetStrengthBuffDoctor.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Doctor && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetStrengthBuffDoctor"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetStrengthBuffDoctor, SettingKey = "TargetStrengthBuffDoctor", Label = "Strength Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetStrengthBuffDoctor"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetStrengthBuffDoctor, SettingKey = "TargetStrengthBuffDoctor", Label = "Strength Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatDoctorBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetStrengthBuffDoctor"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetStrengthBuffDoctor"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.DoctorHPBuffs.Contains(spellID))
                        {
                            LoadedNonCombatDoctorBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["DoctorHPBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "DoctorHPBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetFirstAidAndTreatmentBuffDoctor.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Doctor && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetFirstAidAndTreatmentBuffDoctor"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetFirstAidAndTreatmentBuffDoctor, SettingKey = "TargetFirstAidAndTreatmentBuffDoctor", Label = "First Aid And Treatment Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetFirstAidAndTreatmentBuffDoctor"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetFirstAidAndTreatmentBuffDoctor, SettingKey = "TargetFirstAidAndTreatmentBuffDoctor", Label = "First Aid And Treatment Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatDoctorBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetFirstAidAndTreatmentBuffDoctor"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetFirstAidAndTreatmentBuffDoctor"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.HealDeltaBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatDoctorBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["HealDeltaBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "HealDeltaBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetInitiativeBuffsDoctor.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Doctor && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsDoctor"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetInitiativeBuffsDoctor, SettingKey = "TargetInitiativeBuffsDoctor", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsDoctor"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetInitiativeBuffsDoctor, SettingKey = "TargetInitiativeBuffsDoctor", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatDoctorBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetInitiativeBuffsDoctor, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetInitiativeBuffsDoctor"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetNanoResistanceBuffs.Contains(spellID))
                        {
                            LoadedNonCombatDoctorBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetNanoResistanceBuffs, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetNanoResistanceBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetPistolMastery.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Doctor && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "PistolBuffTarget"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetPistolMastery, SettingKey = "PistolBuffTarget", Label = "Pistol Mastery", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "PistolBuffTarget"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetPistolMastery, SettingKey = "PistolBuffTarget", Label = "Pistol Mastery", UiType = UiType.DualDropDown });

                            LoadedNonCombatDoctorBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["PistolBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "PistolBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Doctor)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatDoctorBuffs);

                        break;
                    #endregion

                    #region Enforcer
                    case Profession.Enforcer:
                        if (SpellID.TargetStrengthBuffEnforcer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Enforcer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetStrengthBuffEnforcer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetStrengthBuffEnforcer, SettingKey = "TargetStrengthBuffEnforcer", Label = "Strength Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetStrengthBuffEnforcer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetStrengthBuffEnforcer, SettingKey = "TargetStrengthBuffEnforcer", Label = "Strength Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetStrengthBuffEnforcer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetStrengthBuffEnforcer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine._1HBluntBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["_1HBluntBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "_1HBluntBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.DamageChangeBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["DamageChangeBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "DamageChangeBuffs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserDamageShieldsEnforcer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Enforcer && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageShieldsEnforcer"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserDamageShieldsEnforcer, SettingKey = "UserDamageShieldsEnforcer", Label = "Damage Shields", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageShieldsEnforcer"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserDamageShieldsEnforcer, SettingKey = "UserDamageShieldsEnforcer", Label = "Damage Shields", UiType = UiType.DropDownWOption });

                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserDamageShieldsEnforcer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserDamageShieldsEnforcer"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetDamageShieldsEnforcer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Enforcer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageShieldsEnforcer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetDamageShieldsEnforcer, SettingKey = "TargetDamageShieldsEnforcer", Label = "Damage Shields", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageShieldsEnforcer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetDamageShieldsEnforcer, SettingKey = "TargetDamageShieldsEnforcer", Label = "Damage Shields", UiType = UiType.DualDropDown });

                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetDamageShieldsEnforcer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetDamageShieldsEnforcer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.EnforcerMeleeEnergyBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["EnforcerMeleeEnergyBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "EnforcerMeleeEnergyBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.EnforcerPiercingBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["EnforcerPiercingBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "EnforcerPiercingBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.EnforcerTauntProcs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["EnforcerTauntProcs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "EnforcerTauntProcs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.FastAttackBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["FastAttackBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "FastAttackBuffs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserHPBuffEnforcer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Enforcer && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == ".UserHPBuffEnforcer"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserHPBuffEnforcer, SettingKey = ".UserHPBuffEnforcer", Label = "HP Buff", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == ".UserHPBuffEnforcer"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserHPBuffEnforcer, SettingKey = ".UserHPBuffEnforcer", Label = "HP Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings[".UserHPBuffEnforcer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, ".UserHPBuffEnforcer"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetHPBuffEnforcer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Enforcer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetHPBuffEnforcer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetHPBuffEnforcer, SettingKey = "TargetHPBuffEnforcer", Label = "HP Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetHPBuffEnforcer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetHPBuffEnforcer, SettingKey = "TargetHPBuffEnforcer", Label = "HP Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetHPBuffEnforcer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetHPBuffEnforcer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetInitiativeBuffsEnforcer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Enforcer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsEnforcer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetInitiativeBuffsEnforcer, SettingKey = "TargetInitiativeBuffsEnforcer", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsEnforcer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetInitiativeBuffsEnforcer, SettingKey = "TargetInitiativeBuffsEnforcer", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetInitiativeBuffsEnforcer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetInitiativeBuffsEnforcer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.MeleeWeaponBuffLine).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEnforcerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["MeleeWeaponBuffLine"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "MeleeWeaponBuffLine"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Enforcer)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatEnforcerBuffs);
                        break;
                    #endregion

                    #region Engineer
                    case Profession.Engineer:
                        if (SpellID.TargetArmorBuffEngineer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetArmorBuffEngineer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetArmorBuffEngineer, SettingKey = "TargetArmorBuffEngineer", Label = "Armor Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetArmorBuffEngineer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetArmorBuffEngineer, SettingKey = "TargetArmorBuffEngineer", Label = "Armor Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetArmorBuffEngineer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetArmorBuffEngineer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserDamageBuffs_LineAEngineer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageBuffs_LineAEngineer"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserDamageBuffs_LineAEngineer, SettingKey = "UserDamageBuffs_LineAEngineer", Label = "Damage Buffs - Line A", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageBuffs_LineAEngineer"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserDamageBuffs_LineAEngineer, SettingKey = "UserDamageBuffs_LineAEngineer", Label = "Damage Buffs - Line A", UiType = UiType.DropDownWOption });

                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserDamageBuffs_LineAEngineer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserDamageBuffs_LineAEngineer"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetDamageBuffs_LineAEngineer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageBuffs_LineAEngineer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetDamageBuffs_LineAEngineer, SettingKey = "TargetDamageBuffs_LineAEngineer", Label = "Damage Buffs - Line A", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageBuffs_LineAEngineer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetDamageBuffs_LineAEngineer, SettingKey = "TargetDamageBuffs_LineAEngineer", Label = "Damage Buffs - Line A", UiType = UiType.DualDropDown });

                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetDamageBuffs_LineAEngineer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetDamageBuffs_LineAEngineer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetDamageShieldsEngineer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageShieldsEngineer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetDamageShieldsEngineer, SettingKey = "TargetDamageShieldsEngineer", Label = "Damage Shields", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageShieldsEngineer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetDamageShieldsEngineer, SettingKey = "TargetDamageShieldsEngineer", Label = "Damage Shields", UiType = UiType.DualDropDown });

                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetDamageShieldsEngineer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetDamageShieldsEngineer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.EngineeringBuff.Contains(spellID))
                        {
                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["EngineeringBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "EngineeringBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.EngineerAuras).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["EngineerAuras"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "EngineerAuras"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserEngineerSpecialAttackAbsorber.Contains(spellID))
                        {
                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["EngineerSpecialAttackAbsorberSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "EngineerSpecialAttackAbsorberSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TeamEngineerSpecialAttackAbsorber.Contains(spellID))
                        {
                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["EngineerSpecialAttackAbsorberTeam"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "EngineerSpecialAttackAbsorberTeam"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.GrenadeBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["GrenadeBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "GrenadeBuffs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetInitiativeBuffsEngineer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsEngineer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetInitiativeBuffsEngineer, SettingKey = "TargetInitiativeBuffsEngineer", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsEngineer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetInitiativeBuffsEngineer, SettingKey = "TargetInitiativeBuffsEngineer", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetInitiativeBuffsEngineer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetInitiativeBuffsEngineer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetPistolMastery.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "PistolBuffTarget"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetPistolMastery, SettingKey = "PistolBuffTarget", Label = "Pistol Mastery", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "PistolBuffTarget"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetPistolMastery, SettingKey = "PistolBuffTarget", Label = "Pistol Mastery", UiType = UiType.DualDropDown });

                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["PistolBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "PistolBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetReflectShieldEngineer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetReflectShieldEngineer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetReflectShieldEngineer, SettingKey = "TargetReflectShieldEngineer", Label = "Reflect Shield", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetReflectShieldEngineer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetReflectShieldEngineer, SettingKey = "TargetReflectShieldEngineer", Label = "Reflect Shield", UiType = UiType.DualDropDown });

                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetReflectShieldEngineer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetReflectShieldEngineer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.ShadowlandReflectBase).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["ShadowlandReflectBaseTeam"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "ShadowlandReflectBaseTeam"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.SlayerdroidTransference.Contains(spellID))
                        {
                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SlayerdroidTransference"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "SlayerdroidTransference"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.SpecialAttackAbsorberBase).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatEngineerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SpecialAttackAbsorberBase"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "SpecialAttackAbsorberBase"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Engineer)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatEngineerBuffs);
                        break;
                    #endregion

                    #region Fixer
                    case Profession.Fixer:
                        if (SpellID.TargetConcealmentBuffFixer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Fixer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetConcealmentBuffFixer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetConcealmentBuffFixer, SettingKey = "TargetConcealmentBuffFixer", Label = "Concealment Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetConcealmentBuffFixer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetConcealmentBuffFixer, SettingKey = "TargetConcealmentBuffFixer", Label = "Concealment Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetConcealmentBuffFixer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetConcealmentBuffFixer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetDamageBuffs_LineAFixer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Fixer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageBuffs_LineAFixer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetDamageBuffs_LineAFixer, SettingKey = "TargetDamageBuffs_LineAFixer", Label = "Damage Buffs - Line A", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageBuffs_LineAFixer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetDamageBuffs_LineAFixer, SettingKey = "TargetDamageBuffs_LineAFixer", Label = "Damage Buffs - Line A", UiType = UiType.DualDropDown });

                            LoadedNonCombatFixerBuffs.Add(spellID);
                            if (!LoadedPetBuffSpells.Contains(spellID))
                                LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetDamageBuffs_LineAFixer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetDamageBuffs_LineAFixer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.FixerDodgeBuffLine).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["FixerDodgeBuffLine"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "FixerDodgeBuffLine"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.FixerNCUBuffs.Contains(spellID))
                        {
                            LoadedNonCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["FixerNCUBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "FixerNCUBuffs"), CombatActionPriority.High, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.FixerSuppressorBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["FixerSuppressorBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "FixerSuppressorBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.Break_EntryBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["Break_EntryBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "Break_EntryBuffs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Fixer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffs_RunspeedBuffsFixer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer, SettingKey = "TargetMajorEvasionBuffs_RunspeedBuffsFixer", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffs_RunspeedBuffsFixer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer, SettingKey = "TargetMajorEvasionBuffs_RunspeedBuffsFixer", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatFixerBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetMajorEvasionBuffs_RunspeedBuffsFixer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.ShadowlandsRunspeed.Contains(spellID))
                        {
                            LoadedNonCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["ShadowlandsRunspeed"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "ShadowlandsRunspeed"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetPerceptionBuffsFixer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Fixer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetPerceptionBuffsFixer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetPerceptionBuffsFixer, SettingKey = "TargetPerceptionBuffsFixer", Label = "Perception Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetPerceptionBuffsFixer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetPerceptionBuffsFixer, SettingKey = "TargetPerceptionBuffsFixer", Label = "Perception Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetPerceptionBuffsFixer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetPerceptionBuffsFixer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TeamRunspeedBuffsFixer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Fixer && !BuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamRunspeedBuffsFixer"))
                                BuffWindowController.TeamBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TeamRunspeedBuffsFixer, SettingKey = "TeamRunspeedBuffsFixer", Label = "Team Run Speed Buffs", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TeamBuff.Any(c => c.SettingKey == "TeamRunspeedBuffsFixer"))
                                FPBuffWindowController.TeamBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TeamRunspeedBuffsFixer, SettingKey = "TeamRunspeedBuffsFixer", Label = "Team Run Speed Buffs", UiType = UiType.DropDownWOption });

                            LoadedNonCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TeamRunspeedBuffsFixer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "TeamRunspeedBuffsFixer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetSneakAttackBuffsFixer.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Fixer && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetSneakAttackBuffsFixer"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetSneakAttackBuffsFixer, SettingKey = "TargetSneakAttackBuffsFixer", Label = "Sneak Attack Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetSneakAttackBuffsFixer"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetSneakAttackBuffsFixer, SettingKey = "TargetSneakAttackBuffsFixer", Label = "Sneak Attack Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatFixerBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetSneakAttackBuffsFixer"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetSneakAttackBuffsFixer"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.Grid.Contains(spellID))
                        {
                            int highestGrid = SpellID.Grid.FirstOrDefault(id => Spell.List.Any(s => s.Id == id));
                            RegisterSpellProcessor(highestGrid, Grid, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.ShadowwebSpinner.Contains(spellID))
                        {
                            int highestShadowwebSpinner = SpellID.ShadowwebSpinner.FirstOrDefault(id => Spell.List.Any(s => s.Id == id));
                            RegisterSpellProcessor(highestShadowwebSpinner, ShadowwebSpinner, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        #region Spawn Armor

                        if (SpellID.Grid.Contains(spellID))
                        {
                            int highestGrid = SpellID.Grid.FirstOrDefault(id => Spell.List.Any(s => s.Id == id));
                            RegisterSpellProcessor(highestGrid, Grid, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.ShadowwebSpinner.Contains(spellID))
                        {
                            int highestShadowwebSpinner = SpellID.ShadowwebSpinner.FirstOrDefault(id => Spell.List.Any(s => s.Id == id));
                            RegisterSpellProcessor(highestShadowwebSpinner, ShadowwebSpinner, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        #endregion

                        if (DynelManager.LocalPlayer.Profession == Profession.Fixer)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatFixerBuffs);
                        break;
                    #endregion

                    #region Keeper
                    case Profession.Keeper:
                        if (Spell.GetSpellsForNanoline(NanoLine.KeeperStr_Stam_AgiBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["KeeperStr_Stam_AgiBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "KeeperStr_Stam_AgiBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine._2HEdgedBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["_2HEdgedBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "_2HEdgedBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetAAOBuffsKeeper.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Keeper && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetAAOBuffsKeeper"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetAAOBuffsKeeper, SettingKey = "TargetAAOBuffsKeeper", Label = "AAO Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetAAOBuffsKeeper"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetAAOBuffsKeeper"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.KeeperAura_Absorb_Reflect_AMSBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["KeeperAura_Absorb_Reflect_AMSBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "KeeperAura_Absorb_Reflect_AMSBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.KeeperAura_Damage_SnareReductionBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["KeeperAura_Damage_SnareReductionBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "KeeperAura_Damage_SnareReductionBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.KeeperAura_HPandNPHeal).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["KeeperAura_HPandNPHeal"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "KeeperAura_HPandNPHeal"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.KeeperDeflect_RiposteBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["KeeperDeflect_RiposteBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "KeeperDeflect_RiposteBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.KeeperEvade_Dodge_DuckBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["KeeperEvade_Dodge_DuckBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "KeeperEvade_Dodge_DuckBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.KeeperProcBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["KeeperProcBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "KeeperProcBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.Fury).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["Fury"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "Fury"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserSneakAttackBuffs.Contains(spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SneakAttackBuffsSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "SneakAttackBuffsSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetSneakAttackBuffs.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Keeper && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetSneakAttackBuffs"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetSneakAttackBuffs, SettingKey = "TargetSneakAttackBuffs", Label = "Sneak Attack Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetSneakAttackBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetSneakAttackBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.FastAttackBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatKeeperBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["FastAttackBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "FastAttackBuffs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Keeper)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatKeeperBuffs);

                        break;
                    #endregion

                    #region MartialArtist
                    case Profession.MartialArtist:
                        if (SpellID.UserArmorBuffMartialArtist.Contains(spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserArmorBuffMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserArmorBuffMartialArtist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetArmorBuffMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetArmorBuffMartialArtist"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetArmorBuffMartialArtist, SettingKey = "TargetArmorBuffMartialArtist", Label = "Armor Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetArmorBuffMartialArtist"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetArmorBuffMartialArtist, SettingKey = "TargetArmorBuffMartialArtist", Label = "Armor Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetArmorBuffMartialArtist, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetArmorBuffMartialArtist"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.BrawlBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["BrawlBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "BrawlBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.ControlledRageBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["ControlledRageBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "ControlledRageBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserCriticalIncreaseBuffMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserCriticalIncreaseBuffMartialArtist"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserCriticalIncreaseBuffMartialArtist, SettingKey = "UserCriticalIncreaseBuffMartialArtist", Label = "Critical Increase Buff", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserCriticalIncreaseBuffMartialArtist"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserCriticalIncreaseBuffMartialArtist, SettingKey = "UserCriticalIncreaseBuffMartialArtist", Label = "Critical Increase Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserCriticalIncreaseBuffMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserCriticalIncreaseBuffMartialArtist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetCriticalIncreaseBuffMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetCriticalIncreaseBuffMartialArtist"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetCriticalIncreaseBuffMartialArtist, SettingKey = "TargetCriticalIncreaseBuffMartialArtist", Label = "Critical Increase Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetCriticalIncreaseBuffMartialArtist"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetCriticalIncreaseBuffMartialArtist, SettingKey = "TargetCriticalIncreaseBuffMartialArtist", Label = "Critical Increase Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetCriticalIncreaseBuffMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetCriticalIncreaseBuffMartialArtist"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserDamageBuffs_LineAMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageBuffs_LineAMartialArtist"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserDamageBuffs_LineAMartialArtist, SettingKey = "UserDamageBuffs_LineAMartialArtist", Label = "Damage Buffs - Line A", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserDamageBuffs_LineAMartialArtist"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserDamageBuffs_LineAMartialArtist, SettingKey = "UserDamageBuffs_LineAMartialArtist", Label = "Damage Buffs - Line A", UiType = UiType.DropDownWOption });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserDamageBuffs_LineAMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserDamageBuffs_LineAMartialArtist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.FastAttackBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["FastAttackBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "FastAttackBuffs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserInitiativeBuffsMartialArtist.Contains(spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserInitiativeBuffsMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserInitiativeBuffsMartialArtist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetInitiativeBuffsMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsMartialArtist"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetInitiativeBuffsMartialArtist, SettingKey = "TargetInitiativeBuffsMartialArtist", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsMartialArtist"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetInitiativeBuffsMartialArtist, SettingKey = "TargetInitiativeBuffsMartialArtist", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetInitiativeBuffsMartialArtist, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetInitiativeBuffsMartialArtist"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMajorEvasionBuffsMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsMartialArtist"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserMajorEvasionBuffsMartialArtist, SettingKey = "UserMajorEvasionBuffsMartialArtist", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsMartialArtist"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserMajorEvasionBuffsMartialArtist, SettingKey = "UserMajorEvasionBuffsMartialArtist", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserMajorEvasionBuffsMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserMajorEvasionBuffsMartialArtist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetMajorEvasionBuffsMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsMartialArtist"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetMajorEvasionBuffsMartialArtist, SettingKey = "TargetMajorEvasionBuffsMartialArtist", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsMartialArtist"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetMajorEvasionBuffsMartialArtist, SettingKey = "TargetMajorEvasionBuffsMartialArtist", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetMajorEvasionBuffsMartialArtist, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetMajorEvasionBuffsMartialArtist"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.MartialArtistBowBuffsMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "MartialArtistBowBuffsMartialArtist"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.MartialArtistBowBuffsMartialArtist, SettingKey = "MartialArtistBowBuffsMartialArtist", Label = "Martial Artist Bow Buffs", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "MartialArtistBowBuffsMartialArtist"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.MartialArtistBowBuffsMartialArtist, SettingKey = "MartialArtistBowBuffsMartialArtist", Label = "Martial Artist Bow Buffs", UiType = UiType.DropDownWOption });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["MartialArtistBowBuffsMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "MartialArtistBowBuffsMartialArtist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.MartialArtistZazenStance).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["MartialArtistZazenStance"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "MartialArtistZazenStance"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMartialArtsBuffMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMartialArtsBuffMartialArtist"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserMartialArtsBuffMartialArtist, SettingKey = "UserMartialArtsBuffMartialArtist", Label = "Martial Arts Buff", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMartialArtsBuffMartialArtist"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserMartialArtsBuffMartialArtist, SettingKey = "UserMartialArtsBuffMartialArtist", Label = "Martial Arts Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserMartialArtsBuffMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserMartialArtsBuffMartialArtist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetMartialArtsBuffMartialArtist.Contains(spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["MartialArtsBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "MartialArtsBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.NanoResistBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["NanoResistBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "NanoResistBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.RiposteBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["RiposteBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "RiposteBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetRunspeedBuffsMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetRunspeedBuffsMartialArtist"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetRunspeedBuffsMartialArtist, SettingKey = "TargetRunspeedBuffsMartialArtist", Label = "Runspeed Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetRunspeedBuffsMartialArtist"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetRunspeedBuffsMartialArtist, SettingKey = "TargetRunspeedBuffsMartialArtist", Label = "Runspeed Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetRunspeedBuffsMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetRunspeedBuffsMartialArtist"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetStrengthBuffMartialArtist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetStrengthBuffMartialArtist"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetStrengthBuffMartialArtist, SettingKey = "TargetStrengthBuffMartialArtist", Label = "Strength Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetStrengthBuffMartialArtist"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetStrengthBuffMartialArtist, SettingKey = "TargetStrengthBuffMartialArtist", Label = "Strength Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetStrengthBuffMartialArtist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetStrengthBuffMartialArtist"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.LimboMastery == spellID)
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["LimboMastery"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "LimboMastery"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.Horde.Contains(spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["Horde"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "Horde"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.Cohort.Contains(spellID))
                        {
                            LoadedNonCombatMartialArtistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["Cohort"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "Cohort"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.MartialArtist)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatMartialArtistBuffs);
                        break;
                    #endregion

                    #region Metaphysicist
                    case Profession.Metaphysicist:
                        if (SpellID.UserPsy_IntBuffMetaphysicist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserPsy_IntBuffMetaphysicist"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserPsy_IntBuffMetaphysicist, SettingKey = "UserPsy_IntBuffMetaphysicist", Label = "Psy/Int Buff", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserPsy_IntBuffMetaphysicist"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserPsy_IntBuffMetaphysicist, SettingKey = "UserPsy_IntBuffMetaphysicist", Label = "Psy/Int Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserPsy_IntBuffMetaphysicist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserPsy_IntBuffMetaphysicist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetPsy_IntBuff.Contains(spellID))
                        {
                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["Psy_IntBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "Psy_IntBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMajorEvasionBuffsMetaphysicist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsMetaphysicist"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserMajorEvasionBuffsMetaphysicist, SettingKey = "UserMajorEvasionBuffsMetaphysicist", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsMetaphysicist"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserMajorEvasionBuffsMetaphysicist, SettingKey = "UserMajorEvasionBuffsMetaphysicist", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserMajorEvasionBuffsMetaphysicist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserMajorEvasionBuffsMetaphysicist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetMajorEvasionBuffsMetaphysicist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsMetaphysicist"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetMajorEvasionBuffsMetaphysicist, SettingKey = "TargetMajorEvasionBuffsMetaphysicist", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsMetaphysicist"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetMajorEvasionBuffsMetaphysicist, SettingKey = "TargetMajorEvasionBuffsMetaphysicist", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetMajorEvasionBuffsMetaphysicist, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetMajorEvasionBuffsMetaphysicist"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.MartialArtistBowBuffsMetaphysicist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "MartialArtistBowBuffsMetaphysicist"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.MartialArtistBowBuffsMetaphysicist, SettingKey = "MartialArtistBowBuffsMetaphysicist", Label = "Martial Artist Bow Buffs", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "MartialArtistBowBuffsMetaphysicist"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.MartialArtistBowBuffsMetaphysicist, SettingKey = "MartialArtistBowBuffsMetaphysicist", Label = "Martial Artist Bow Buffs", UiType = UiType.DropDownWOption });

                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["MartialArtistBowBuffsMetaphysicist"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "MartialArtistBowBuffsMetaphysicist"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetNPCostBuffMetaphysicist.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetNPCostBuffMetaphysicist"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetNPCostBuffMetaphysicist, SettingKey = "TargetNPCostBuffMetaphysicist", Label = "NP Cost Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetNPCostBuffMetaphysicist"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetNPCostBuffMetaphysicist, SettingKey = "TargetNPCostBuffMetaphysicist", Label = "NP Cost Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetNPCostBuffMetaphysicist, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetNPCostBuffMetaphysicist"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetPistolMastery.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "PistolBuffTarget"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetPistolMastery, SettingKey = "PistolBuffTarget", Label = "Pistol Mastery", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "PistolBuffTarget"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetPistolMastery, SettingKey = "PistolBuffTarget", Label = "Pistol Mastery", UiType = UiType.DualDropDown });

                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["PistolBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "PistolBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.MPCompositeNano.Contains(spellID))
                        {
                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.MPCompositeNano, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "MPCompositeNano"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.BioMetBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.BioMetBuff), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "BioMetBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetMatCreaBuff.Contains(spellID))
                        {
                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["MatCreaBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "MatCreaBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.MatMetBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.MatMetBuff), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "MatMetBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.PsyModBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.PsyModBuff), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "PsyModBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.SenImpBuffs.Contains(spellID))
                        {
                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.SenImpBuffs, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "SenseImpBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.MatLocBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["MatLocBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "MatLocBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.InterruptModifier).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatMetaphysicistBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["InterruptModifier"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "InterruptModifier"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        #region Weapons

                        if (SpellID.TwoHanded.Contains(spellID))
                        {
                            LoadedWeaponSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TwoHanded, TwoHandedWeapon, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.OneHanded.Contains(spellID))
                        {
                            LoadedWeaponSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.OneHanded, OneHandedWeapon, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.Shield.Contains(spellID))
                        {
                            LoadedWeaponSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.Shield, ShieldWeapon, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        #endregion

                        if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatMetaphysicistBuffs);
                        break;
                    #endregion

                    #region NanoTechnician
                    case Profession.NanoTechnician:
                        if (SpellID.UserPsy_IntBuffNanoTechnician.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserPsy_IntBuffNanoTechnician"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserPsy_IntBuffNanoTechnician, SettingKey = "UserPsy_IntBuffNanoTechnician", Label = "Psy/Int Buff", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserPsy_IntBuffNanoTechnician"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserPsy_IntBuffNanoTechnician, SettingKey = "UserPsy_IntBuffNanoTechnician", Label = "Psy/Int Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserPsy_IntBuffNanoTechnician"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserPsy_IntBuffNanoTechnician"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetPsy_IntBuff.Contains(spellID))
                        {
                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["Psy_IntBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "Psy_IntBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserFortifyNanoTechnician.Contains(spellID))
                        {
                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserFortifyNanoTechnician"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserFortifyNanoTechnician"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMajorEvasionBuffsNanoTechnician.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsNanoTechnician"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserMajorEvasionBuffsNanoTechnician, SettingKey = "UserMajorEvasionBuffsNanoTechnician", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsNanoTechnician"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserMajorEvasionBuffsNanoTechnician, SettingKey = "UserMajorEvasionBuffsNanoTechnician", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserMajorEvasionBuffsNanoTechnician"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserMajorEvasionBuffsNanoTechnician"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetMajorEvasionBuffsNanoTechnician.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsNanoTechnician"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetMajorEvasionBuffsNanoTechnician, SettingKey = "TargetMajorEvasionBuffsNanoTechnician", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsNanoTechnician"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetMajorEvasionBuffsNanoTechnician, SettingKey = "TargetMajorEvasionBuffsNanoTechnician", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetMajorEvasionBuffsNanoTechnician, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetMajorEvasionBuffsNanoTechnician"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMatCreaBuff.Contains(spellID))
                        {
                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["MatCreaBuffSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "MatCreaBuffSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetMatCreaBuff.Contains(spellID))
                        {
                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["MatCreaBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "MatCreaBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.NanoDamageMultiplierBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["NanoDamageMultiplierBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "NanoDamageMultiplierBuffs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.NanoOverTime_LineA).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["NanoOverTime_LineA"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "NanoOverTime_LineA"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.NFRangeBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["NFRangeBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "NFRangeBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetNPCostBuffNanoTechnician.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetNPCostBuffNanoTechnician"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetNPCostBuffNanoTechnician, SettingKey = "TargetNPCostBuffNanoTechnician", Label = "NP Cost Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetNPCostBuffNanoTechnician"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetNPCostBuffNanoTechnician, SettingKey = "TargetNPCostBuffNanoTechnician", Label = "NP Cost Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetNPCostBuffNanoTechnician, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetNPCostBuffNanoTechnician"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserReflectShieldNanoTechnician.Contains(spellID))
                        {
                            LoadedNonCombatNanoTechnicianBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["ReflectShieldNanoTechnicianSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "ReflectShieldNanoTechnicianSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatNanoTechnicianBuffs);
                        break;
                    #endregion

                    #region Shade
                    case Profession.Shade:
                        if (SpellID.UserAgilityBuffShade.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Shade && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserAgilityBuffShade"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserAgilityBuffShade, SettingKey = "UserAgilityBuffShade", Label = "Agility Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserAgilityBuffShade"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserAgilityBuffShade"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserConcealmentBuffShade.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Shade && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserConcealmentBuffShade"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserConcealmentBuffShade, SettingKey = "UserConcealmentBuffShade", Label = "Concealment Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserConcealmentBuffShade"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserConcealmentBuffShade"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.FastAttackBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["FastAttackBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "FastAttackBuffs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMartialArtsBuffShade.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Shade && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMartialArtsBuffShade"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserMartialArtsBuffShade, SettingKey = "UserMartialArtsBuffShade", Label = "Martial Arts Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserMartialArtsBuffShade"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserMartialArtsBuffShade"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMultiwieldBuffShade.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Shade && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMultiwieldBuffShade"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserMultiwieldBuffShade, SettingKey = "UserMultiwieldBuffShade", Label = "Multiwield Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserMultiwieldBuffShade"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserMultiwieldBuffShade"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetRunspeedBuffsShade.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Shade && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetRunspeedBuffsShade"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetRunspeedBuffsShade, SettingKey = "TargetRunspeedBuffsShade", Label = "Runspeed Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetRunspeedBuffsShade"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetRunspeedBuffsShade"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.ShadePiercingBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["ShadePiercingBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "ShadePiercingBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserSneakAttackBuffs.Contains(spellID))
                        {
                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SneakAttackBuffsSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "SneakAttackBuffsSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetSneakAttackBuffs.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Shade && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetSneakAttackBuffs"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetSneakAttackBuffs, SettingKey = "TargetSneakAttackBuffs", Label = "Sneak Attack Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetSneakAttackBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetSneakAttackBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.AADBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["AADBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "AADBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.ShadeProcBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["ShadeProcBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "ShadeProcBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.WeaponEffectAdd_On2).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatShadeBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["WeaponEffectAdd_On2"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "WeaponEffectAdd_On2"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Shade)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatShadeBuffs);
                        break;
                    #endregion

                    #region Soldier
                    case Profession.Soldier:
                        if (SpellID.TargetAAOBuffsSoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetAAOBuffsSoldier"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetAAOBuffsSoldier, SettingKey = "TargetAAOBuffsSoldier", Label = "AAO Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetAAOBuffsSoldier"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetAAOBuffsSoldier, SettingKey = "TargetAAOBuffsSoldier", Label = "AAO Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetAAOBuffsSoldier"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetAAOBuffsSoldier"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetArmorBuffSoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetArmorBuffSoldier"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetArmorBuffSoldier, SettingKey = "TargetArmorBuffSoldier", Label = "Armor Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetArmorBuffSoldier"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetArmorBuffSoldier, SettingKey = "TargetArmorBuffSoldier", Label = "Armor Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetArmorBuffSoldier, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetArmorBuffSoldier"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserAssaultRifleBuffs.Contains(spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["AssaultRifleBuffsSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "AssaultRifleBuffsSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetAssaultRifleBuffs.Contains(spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["AssaultRifleBuffsTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "AssaultRifleBuffsTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.CompositeHeavyArtillery.Contains(spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["CompositeHeavyArtillery"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "CompositeHeavyArtillery"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserBurstBuff.Contains(spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["BurstBuffSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "BurstBuffSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetBurstBuff.Contains(spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["BurstBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "BurstBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.SoldierDamageBase).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SoldierDamageBase"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "SoldierDamageBase"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetDamageBuffs_LineASoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageBuffs_LineASoldier"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetDamageBuffs_LineASoldier, SettingKey = "TargetDamageBuffs_LineASoldier", Label = "Damage Buffs - Line A", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetDamageBuffs_LineASoldier"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetDamageBuffs_LineASoldier, SettingKey = "TargetDamageBuffs_LineASoldier", Label = "Damage Buffs - Line A", UiType = UiType.DualDropDown });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            if (!LoadedPetBuffSpells.Contains(spellID))
                                LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetDamageBuffs_LineASoldier, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetDamageBuffs_LineASoldier"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.HeavyWeaponsBuffs).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["HeavyWeaponsBuffs"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "HeavyWeaponsBuffs"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserHPBuffSoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserHPBuffSoldier"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserHPBuffSoldier, SettingKey = "UserHPBuffSoldier", Label = "HP Buff", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserHPBuffSoldier"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserHPBuffSoldier, SettingKey = "UserHPBuffSoldier", Label = "HP Buff", UiType = UiType.DropDownWOption });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserHPBuffSoldier"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserHPBuffSoldier"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetHPBuffSoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetHPBuffSoldier"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetHPBuffSoldier, SettingKey = "TargetHPBuffSoldier", Label = "HP Buff", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetHPBuffSoldier"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetHPBuffSoldier, SettingKey = "TargetHPBuffSoldier", Label = "HP Buff", UiType = UiType.DualDropDown });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetHPBuffSoldier, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetHPBuffSoldier"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetInitiativeBuffsSoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsSoldier"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetInitiativeBuffsSoldier, SettingKey = "TargetInitiativeBuffsSoldier", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetInitiativeBuffsSoldier"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetInitiativeBuffsSoldier, SettingKey = "TargetInitiativeBuffsSoldier", Label = "Initiative Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetInitiativeBuffsSoldier, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetInitiativeBuffsSoldier"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMajorEvasionBuffsSoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsSoldier"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserMajorEvasionBuffsSoldier, SettingKey = "UserMajorEvasionBuffsSoldier", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsSoldier"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserMajorEvasionBuffsSoldier, SettingKey = "UserMajorEvasionBuffsSoldier", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserMajorEvasionBuffsSoldier"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserMajorEvasionBuffsSoldier"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetMajorEvasionBuffsSoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsSoldier"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetMajorEvasionBuffsSoldier, SettingKey = "TargetMajorEvasionBuffsSoldier", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsSoldier"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetMajorEvasionBuffsSoldier, SettingKey = "TargetMajorEvasionBuffsSoldier", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetMajorEvasionBuffsSoldier, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetMajorEvasionBuffsSoldier"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserRangedEnergyWeaponBuffs.Contains(spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["RangedEnergyWeaponBuffsSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "RangedEnergyWeaponBuffsSelf"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetRangedEnergyWeaponBuffs.Contains(spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["RangedEnergyWeaponBuffsTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "RangedEnergyWeaponBuffsTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.ShadowlandReflectBase).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["ShadowlandReflectBaseTeam"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "ShadowlandReflectBaseTeam"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetReflectShieldSoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetReflectShieldSoldier"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetReflectShieldSoldier, SettingKey = "TargetReflectShieldSoldier", Label = "Reflect Shield", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetReflectShieldSoldier"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetReflectShieldSoldier, SettingKey = "TargetReflectShieldSoldier", Label = "Reflect Shield", UiType = UiType.DualDropDown });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetReflectShieldSoldier, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetReflectShieldSoldier"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetRifleBuffsSoldier.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetRifleBuffsSoldier"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetRifleBuffsSoldier, SettingKey = "TargetRifleBuffsSoldier", Label = "Rifle Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetRifleBuffsSoldier"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetRifleBuffsSoldier, SettingKey = "TargetRifleBuffsSoldier", Label = "Rifle Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TargetRifleBuffsSoldier"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetRifleBuffsSoldier"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.SoldierFullAutoBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SoldierFullAutoBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "SoldierFullAutoBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.SoldierShotgunBuff).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SoldierShotgunBuff"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "SoldierShotgunBuff"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.TotalFocus).Any(s => s.Id == spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["TotalFocus"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "TotalFocus"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.Phalanx == spellID)
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["Phalanx"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "Phalanx"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.SiphonBox683Soldier.Contains(spellID))
                        {
                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["SiphonBox683"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "SiphonBox683"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetPistolMastery.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Soldier && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "PistolBuffTarget"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetPistolMastery, SettingKey = "PistolBuffTarget", Label = "Pistol Mastery", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "PistolBuffTarget"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetPistolMastery, SettingKey = "PistolBuffTarget", Label = "Pistol Mastery", UiType = UiType.DualDropDown });

                            LoadedNonCombatSoldierBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["PistolBuffTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "PistolBuffTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Soldier)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatSoldierBuffs);
                        break;
                    #endregion

                    #region Trader
                    case Profession.Trader:
                        if (SpellID.UserDamageBuff_LineC.Contains(spellID))
                        {
                            LoadedNonCombatTraderBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["DamageBuff_LineCSelf"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "DamageBuff_LineCSelf"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetDamageBuff_LineC.Contains(spellID))
                        {
                            LoadedNonCombatTraderBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["DamageBuff_LineCTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "DamageBuff_LineCTarget"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TeamDamageBuff_LineC.Contains(spellID))
                        {
                            LoadedNonCombatTraderBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["DamageBuff_LineCTeam"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "DamageBuff_LineCTeam"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UserMajorEvasionBuffsTrader.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Trader && !BuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsTrader"))
                                BuffWindowController.SelfBuff.Add(new BuffUiConfig { BuffIDs = SpellID.UserMajorEvasionBuffsTrader, SettingKey = "UserMajorEvasionBuffsTrader", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.SelfBuff.Any(c => c.SettingKey == "UserMajorEvasionBuffsTrader"))
                                FPBuffWindowController.SelfBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.UserMajorEvasionBuffsTrader, SettingKey = "UserMajorEvasionBuffsTrader", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption });

                            LoadedNonCombatTraderBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UserMajorEvasionBuffsTrader"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            UserBuff(spell, fightingTarget, ref actionTarget, "UserMajorEvasionBuffsTrader"), CombatActionPriority.VeryLow, RuleContext.OutOfCombat);
                        }

                        if (SpellID.TargetMajorEvasionBuffsTrader.Contains(spellID))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Trader && !BuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsTrader"))
                                BuffWindowController.TargetBuff.Add(new BuffUiConfig { BuffIDs = SpellID.TargetMajorEvasionBuffsTrader, SettingKey = "TargetMajorEvasionBuffsTrader", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            if (DynelManager.LocalPlayer.Profession == Profession.Agent && !FPBuffWindowController.TargetBuff.Any(c => c.SettingKey == "TargetMajorEvasionBuffsTrader"))
                                FPBuffWindowController.TargetBuff.Add(new FPBuffUiConfig { FPBuffIDs = SpellID.TargetMajorEvasionBuffsTrader, SettingKey = "TargetMajorEvasionBuffsTrader", Label = "Major Evasion Buffs", UiType = UiType.DualDropDown });

                            LoadedNonCombatTraderBuffs.Add(spellID);
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.TargetMajorEvasionBuffsTrader, (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuff(spell, fightingTarget, ref actionTarget, "TargetMajorEvasionBuffsTrader"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.UmbralWrangler.Contains(spellID))
                        {
                            LoadedNonCombatTraderBuffs.Add(spellID);
                            RegisterSpellProcessor(_settings["UmbralWrangler"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamBuff(spell, fightingTarget, ref actionTarget, "UmbralWrangler"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (DynelManager.LocalPlayer.Profession == Profession.Trader)
                            LoadedNonCombatBuffs.UnionWith(LoadedNonCombatTraderBuffs);
                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Generic

        private bool UserBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string v)
        {
            if (!_settings[v + "CheckBox"].AsBool()) return false;
            if (!_settings["Buffing"].AsBool()) return false;
            if (!CanCast(spell)) return false;

            if (_settings[v].AsInt32() != spell.Id) return false;

            if (!SpellCheckSelf(spell)) return false;

            if (spell.Nanoline == NanoLine.SiphonBox683)
            {
                if (!_settings["AOE"].AsBool())
                    return false;
            }

            switch (spell.Nanoline)
            {
                case NanoLine.RunspeedBuffs:
                case NanoLine.MajorEvasionBuffs:
                    if (DynelManager.LocalPlayer.Buffs.Contains(SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer)) return false;
                    if (IsInsideInnerSanctum()) return false;
                    break;
            }

            actionTarget = (DynelManager.LocalPlayer, true);
            return true;
        }

        private bool TargetBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string v)
        {
            if (!CanCast(spell)) return false;

            actionTarget.Target = null;

            if (_settings["BuffPets"] != null && _settings["BuffPets"].AsBool() && CanLookupPetsAfterZone())
            {
                string settingPrefix = null;

                switch (spell.Nanoline)
                {
                    case NanoLine.ArmorBuff:
                        settingPrefix = "ArmorBuff";
                        break;
                    case NanoLine.DamageBuffs_LineA:
                        if (SpellID.TargetDamageBuffs_LineAAgent.Contains(spell.Id))
                            settingPrefix = "DamageBuffs_LineAAgent";
                        else if (SpellID.TargetDamageBuffs_LineAEngineer.Contains(spell.Id))
                            settingPrefix = "DamageBuffs_LineAEngineer";
                        else if (SpellID.TargetDamageBuffs_LineAFixer.Contains(spell.Id))
                            settingPrefix = "DamageBuffs_LineAFixer";
                        else if (SpellID.TargetDamageBuffs_LineASoldier.Contains(spell.Id))
                            settingPrefix = "DamageBuffs_LineASoldier";
                        break;
                    case NanoLine.MajorEvasionBuffs:
                        settingPrefix = "AnticipationofRetaliation";
                        break;
                    case NanoLine.CriticalDecreaseBuff:
                        settingPrefix = "CriticalDecreaseBuff";
                        break;
                    case NanoLine.PsyModBuff:
                        settingPrefix = "MPPsyModBuff";
                        break;
                    case NanoLine.BioMetBuff:
                        settingPrefix = "MPBioMetBuff";
                        break;
                    case NanoLine.MatMetBuff:
                        settingPrefix = "MPMatMetBuff";
                        break;
                    case NanoLine.NPCostBuff:
                        settingPrefix = "CostBuffs";
                        break;
                    case NanoLine.DamageShields:
                        settingPrefix = "DamageShields";
                        break;
                    case NanoLine.InitiativeBuffs:
                        settingPrefix = "InitiativeBuffs";
                        break;
                    case NanoLine.ReflectShield:
                        settingPrefix = "ReflectShield";
                        break;
                }

                if (SpellID.SenImpBuffs.Contains(spell.Id))
                    settingPrefix = "MPSenImpBuffs";

                if (SpellID.MPCompositeNano.Contains(spell.Id))
                    settingPrefix = "MPCompositeNano";

                if (settingPrefix != null)
                {
                    if ((actionTarget.Target = PetBuffCheck(spell, PetType.Attack, $"AttackPet{settingPrefix}CheckBox", $"AttackPet{settingPrefix}")) != null)
                    { actionTarget = (actionTarget.Target, true); return true; }

                    if ((actionTarget.Target = PetBuffCheck(spell, PetType.Support, $"SupportPet{settingPrefix}CheckBox", $"SupportPet{settingPrefix}")) != null)
                    { actionTarget = (actionTarget.Target, true); return true; }

                    if ((actionTarget.Target = PetBuffCheck(spell, PetType.Heal, $"HealPet{settingPrefix}CheckBox", $"HealPet{settingPrefix}")) != null)
                    { actionTarget = (actionTarget.Target, true); return true; }
                }
            }

            if (!_settings["Buffing"].AsBool()) return false;
            if (_settings[v + "Option"].AsInt32() == 0) return false;
            if (_settings[v].AsInt32() != spell.Id) return false;

            //Chat.WriteLine($"setting id = {_settings[v].AsInt32()}, spell id = {spell.Id}");
            switch (_settings[v + "Option"].AsInt32())
            {
                case 1:// self
                    //Chat.WriteLine($"{_settings[v + "Option"].AsInt32()}");
                    if (SpellCheckSelf(spell)) { actionTarget = (DynelManager.LocalPlayer, true); return true; }
                    return false;
                case 2: // team
                        //Chat.WriteLine($"{_settings[v + "Option"].AsInt32()}");
                    if (Team.IsInTeam)
                    {

                        switch (spell.Nanoline)
                        {
                            case NanoLine.InitiativeBuffs:

                                if (SpellID.MeleeInitiativeBuffs.Contains(spell.Id))
                                    return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.MartialArts, CharacterWieldedWeapon.Fists, CharacterWieldedWeapon.Edged1H, CharacterWieldedWeapon.Blunt1H, CharacterWieldedWeapon.Edged2H,
                                        CharacterWieldedWeapon.Blunt2H, CharacterWieldedWeapon.Piercing, CharacterWieldedWeapon.Melee);

                                if (SpellID.RangedInitiativeBuffs.Contains(spell.Id))
                                    return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.Pistol, CharacterWieldedWeapon.AssaultRifle, CharacterWieldedWeapon.Rifle,
                                        CharacterWieldedWeapon.Shotgun, CharacterWieldedWeapon.Smg, CharacterWieldedWeapon.Bow, CharacterWieldedWeapon.Energy,
                                        CharacterWieldedWeapon.Grenade, CharacterWieldedWeapon.Grenade2, CharacterWieldedWeapon.HeavyWeapons, CharacterWieldedWeapon.Ranged);

                                break;

                            case NanoLine.MartialArtsBuff:
                            case NanoLine.ControlledRageBuff:
                                return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.MartialArts, CharacterWieldedWeapon.Fists);

                            case NanoLine.BrawlBuff:
                                return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.MartialArts, CharacterWieldedWeapon.Fists, CharacterWieldedWeapon.Edged1H, CharacterWieldedWeapon.Blunt1H, CharacterWieldedWeapon.Edged2H,
                                        CharacterWieldedWeapon.Blunt2H, CharacterWieldedWeapon.Piercing, CharacterWieldedWeapon.Melee);

                            case NanoLine.GrenadeBuffs:
                                return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.Pistol, CharacterWieldedWeapon.Grenade);

                            case NanoLine.PistolBuff:
                                return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.Pistol);

                            case NanoLine.RangedEnergyWeaponBuffs:
                                return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.Ranged, CharacterWieldedWeapon.Energy);

                            case NanoLine.RifleBuffs:
                                return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.Rifle);

                            case NanoLine.BurstBuff:
                                return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.AssaultRifle, CharacterWieldedWeapon.Pistol, CharacterWieldedWeapon.Smg, CharacterWieldedWeapon.Shotgun);

                            case NanoLine.AssaultRifleBuffs:
                                if (SpellID.CompositeHeavyArtillery.Contains(spell.Id))
                                {
                                    var arMember = Team.Members.FirstOrDefault(tm => tm.Character != null && tm.Character.Health > 0 &&
                                        InCastRange(spell, tm.Character) && SpellCheckLocalTeam(spell, tm.Character) && HeavyCompWeaponChecks(tm.Character))?.Character;
                                    if (arMember != null) { actionTarget = (arMember, true); return true; }
                                    return false;
                                }
                                else
                                {
                                    return TeamBuffWeaponCheck(spell, ref actionTarget, CharacterWieldedWeapon.AssaultRifle);
                                }

                            default:
                                {
                                    var fallbackMember = Team.Members.Where(tm => tm.Character != null && tm.Character.Health > 0 && InCastRange(spell, tm.Character) && SpellCheckLocalTeam(spell, tm.Character)).FirstOrDefault()?.Character;

                                    if (fallbackMember != null)
                                    {
                                        //Chat.WriteLine($"{fallbackMember.Name} missing buff, spell. name={spell.Name}, spell.Nanoline={(int)spell.Nanoline}. Buffs: {string.Join(", ", fallbackMember.Buffs.Select(b => $"{b.Name} ({(int)b.Nanoline})"))}");
                                        actionTarget = (fallbackMember, true);
                                        return true;
                                    }
                                    return false;
                                }
                        }
                    }

                    else if (SpellCheckSelf(spell)) { actionTarget = (DynelManager.LocalPlayer, true); return true; }
                    return false;
                default:
                    return false;
            }
        }

        private bool TeamBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string v)
        {
            if (!_settings["Buffing"].AsBool()) return false;
            if (!_settings[v + "CheckBox"].AsBool()) return false;
            if (!CanCast(spell)) return false;

            if (Team.IsInTeam)
            {
                NanoLine teamspell = NanoLine.NOSTACKING;

                switch (spell.Nanoline)
                {
                    case NanoLine.ExperienceConstructs_XPBonus:
                        teamspell = NanoLine.XPBonus;
                        break;
                    case NanoLine.TeamRunSpeedBuffs:
                        teamspell = NanoLine.RunspeedBuffs;
                        break;
                        //case NanoLine.ShadowlandReflectBase:
                        //    teamspell = NanoLine.ReflectShield;
                        //    break;
                }

                if (SpellID.FixerNCUBuffs.Contains(spell.Id))
                    teamspell = NanoLine.FixerNCUBuff;


                if (SpellID.DoctorHPBuffs.Contains(spell.Id))
                    teamspell = NanoLine.DoctorHPBuffs;

                if (SpellID.TeamConcealmentBuffAgent.Contains(spell.Id))
                    teamspell = NanoLine.ConcealmentBuff;

                if (teamspell != NanoLine.NOSTACKING)
                {
                    var local = Team.Members.Find(x => x.Identity == DynelManager.LocalPlayer.Identity);
                    if (local == null)
                        return false;

                    var member = Team.Members.Find(tm => tm.Character != null && tm.Character.Identity != DynelManager.LocalPlayer.Identity && (!Team.IsRaid || tm.TeamIndex == local.TeamIndex) && tm.Character.Health > 0
                    && tm.Character.IsInLineOfSight && InCastRange(spell, tm.Character) && SpellCheckLocalTeam(spell, tm.Character) && !tm.Character.Buffs.Contains(teamspell))?.Character;

                    if (member != null)
                    {
                        //Chat.WriteLine($"{member.Name} missing {teamspell}");

                        //foreach (var buff in member.Buffs)
                        //    Chat.WriteLine($"{buff.Name}, {buff.Nanoline}");

                        if (teamspell == NanoLine.RunspeedBuffs && member.Buffs.Contains(SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer)) return false;

                        actionTarget = (DynelManager.LocalPlayer, true);
                        return true;
                    }
                }
            }

            switch (spell.Nanoline)
            {
                case NanoLine.ExperienceConstructs_XPBonus:
                    if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.XPBonus)) return false;
                    break;
                case NanoLine.TeamRunSpeedBuffs:
                    if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.RunspeedBuffs)) return false;
                    if (DynelManager.LocalPlayer.Buffs.Contains(SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer)) return false;
                    break;
                case NanoLine.RunspeedBuffs:
                    if (DynelManager.LocalPlayer.Buffs.Contains(SpellID.TargetMajorEvasionBuffs_RunspeedBuffsFixer)) return false;
                    break;
                case NanoLine.EngineerAuras:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(spell.Id))
                    {
                        DynelManager.LocalPlayer.Buffs.Find(Spell.GetSpellsForNanoline(spell.Nanoline).Select(b => b.Id).ToArray(), out Buff buff);
                        buff?.Remove();
                    }
                    break;
            }

            if (SpellCheckSelf(spell)) { actionTarget = (DynelManager.LocalPlayer, true); return true; }

            return false;
        }

        private bool MorphFortifyBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["TeamFortifyAdventurer"].AsBool()) return false;
            if (!CanCast(spell)) return false;

            var buffs = DynelManager.LocalPlayer.Buffs;
            if (!buffs.Any(b => b.Nanoline == NanoLine.Polymorph)) return false;

            if (buffs.FirstOrDefault(b => b.Nanoline == spell.Nanoline)?.StackingOrder >= spell.StackingOrder) return false;

            if (buffs.Any(b => SpellID.LeetMorph.Contains(b.Id)) && SpellID.TeamFortifyLeet.Contains(spell.Id) && !buffs.Any(b => b.Id == spell.Id))
            {
                actionTarget = (DynelManager.LocalPlayer, true);
                return true;
            }

            if (buffs.Any(b => SpellID.WolfMorph.Contains(b.Id)) && SpellID.TeamFortifyWolf.Contains(spell.Id) && !buffs.Any(b => b.Id == spell.Id))
            {
                actionTarget = (DynelManager.LocalPlayer, true);
                return true;
            }

            if (buffs.Any(b => SpellID.DragonMorph.Contains(b.Id)) && SpellID.TeamFortifyPitLizard.Contains(spell.Id) && !buffs.Any(b => b.Id == spell.Id))
            {
                actionTarget = (DynelManager.LocalPlayer, true);
                return true;
            }

            if (buffs.Any(b => SpellID.SaberMorph.Contains(b.Id)) && SpellID.TeamFortifySabretooth.Contains(spell.Id) && !buffs.Any(b => b.Id == spell.Id))
            {
                actionTarget = (DynelManager.LocalPlayer, true);
                return true;
            }

            if (buffs.Any(b => SpellID.TreeMorph.Contains(b.Id)) && SpellID.TeamFortifyTree.Contains(spell.Id) && !buffs.Any(b => b.Id == spell.Id))
            {
                actionTarget = (DynelManager.LocalPlayer, true);
                return true;
            }

            return false;
        }

        protected bool CompositeBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            try
            {
                if (!CanCast(spell)) return false;
                if (spell.Id == SpellID.CompositeMartialProwess && IsInsideInnerSanctum()) return false;

                actionTarget.Target = null;

                if (_settings["Composites"].AsBool())
                {
                    if (_settings[setting].AsBool())
                        if (SpellCheckSelf(spell))
                        {
                            actionTarget.Target = DynelManager.LocalPlayer;

                            if (actionTarget.Target != null)
                            {
                                actionTarget = (actionTarget.Target, true);
                                return true;
                            }
                        }
                }

                if (actionTarget.Target == null)
                {
                    if (_settings["BuffPets"] != null && _settings["BuffPets"].AsBool())
                    {
                        if (DynelManager.LocalPlayer.Pets == null || !DynelManager.LocalPlayer.Pets.Any()) return false;
                        if (!CanLookupPetsAfterZone()) return false;

                        switch (spell.Id)
                        {
                            case SpellID.CompositeNano:
                                if (_settings["AttackPetCompositeNano"].AsBool() &&
                                    (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Attack && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }

                                if (_settings["SupportPetCompositeNano"].AsBool() &&
                                    (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Support && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }

                                if (_settings["HealPetCompositeNano"].AsBool() &&
                                    (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Heal && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }
                                break;

                            case SpellID.CompositeMartialProwess:
                                if (_settings["AttackPetCompositeMartialProwess"].AsBool() &&
                                    (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Attack && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }

                                if (_settings["SupportPetCompositeMartialProwess"].AsBool() &&
                                    (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Support && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }

                                if (_settings["HealPetCompositeMartialProwess"].AsBool() &&
                                   (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Heal && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }
                                break;

                            case SpellID.CompositeMelee:
                                if (_settings["AttackPetCompositeMelee"].AsBool() &&
                                    (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Attack && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }

                                if (_settings["SupportPetCompositeMelee"].AsBool() &&
                                    (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Support && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }
                                break;

                            case SpellID.CompositePhysicalSpecial:
                                if (_settings["AttackPetCompositePhysicalSpecial"].AsBool() &&
                                    (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Attack && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }

                                if (_settings["SupportPetCompositePhysicalSpecial"].AsBool() &&
                                    (actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Type == PetType.Support && p.Character != null && !p.Character.Buffs.Contains(spell.Id))?.Character) != null)
                                { actionTarget = (actionTarget.Target, true); return true; }
                                break;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        #endregion

        #region Polymorphs

        private void MorphSwitch()
        {
            switch (_settings["MorphSelection"].AsInt32())
            {
                case 0://None
                    MorhpSpellArray = new int[0];
                    if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Polymorph))
                        CancelBuffs(SpellID.Morphs);
                    break;
                case 1://Dragon
                    MorhpSpellArray = SpellID.DragonMorph;
                    break;
                case 2://Saber
                    MorhpSpellArray = SpellID.SaberMorph;
                    break;
                case 3://Wolf
                    MorhpSpellArray = SpellID.WolfMorph;
                    break;
                case 4://Leet
                    MorhpSpellArray = SpellID.LeetMorph;
                    break;
                case 5://Tree
                    MorhpSpellArray = SpellID.TreeMorph;
                    break;
            }
        }

        private void Morphs()
        {
            var localPlayer = DynelManager.LocalPlayer;

            if (localPlayer.GetStat(Stat.VisualProfession) != 6) { return; }
            if (localPlayer.Buffs.Contains(SpellID.BirdMorph)) { return; }

            MorphSwitch();

            if (MorhpSpellArray.Length == 0) { return; }
            if (localPlayer.Buffs.Contains(MorhpSpellArray)) { return; }

            var MorphSpell = Spell.List.FirstOrDefault(h => MorhpSpellArray.Contains(h.Id));
            if (MorphSpell == null) { return; }

            if (!Spell.HasPendingCast && MorphSpell.MeetsUseReqs() && localPlayer.MovementStatePermitsCasting)
                MorphSpell.Cast(localPlayer, true);
        }

        #endregion

        #region Soldier

        private bool HeavyCompWeaponChecks(SimpleChar _target)
        {
            return GetWieldedWeapons(_target).HasFlag(CharacterWieldedWeapon.AssaultRifle)
                //|| (GetWieldedWeapons(_target).HasFlag(CharacterWieldedWeapon.Grenade) && _target.Profession != Profession.Engineer)
                || GetWieldedWeapons(_target).HasFlag(CharacterWieldedWeapon.HeavyWeapons)
                || (GetWieldedWeapons(_target).HasFlag(CharacterWieldedWeapon.Rifle) && _target.Profession != Profession.Agent)
                || (GetWieldedWeapons(DynelManager.LocalPlayer).HasFlag(CharacterWieldedWeapon.Ranged) && GetWieldedWeapons(_target).HasFlag(CharacterWieldedWeapon.Energy))
                || (GetWieldedWeapons(_target).HasFlag(CharacterWieldedWeapon.Smg) && _target.Profession != Profession.Fixer);
        }

        #endregion

        #region Fixer

        private bool Grid(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Buffing"].AsBool()) return false;
            if (_settings["ArmorSelection"].AsInt32() != 2) return false;
            if (!CanCast(spell)) return false;

            return !Inventory.Items.Any(x => RelevantGenericItems.Grid.Contains(x.HighId));
        }

        private bool ShadowwebSpinner(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Buffing"].AsBool()) return false;
            if (_settings["ArmorSelection"].AsInt32() != 1) return false;
            if (!CanCast(spell)) return false;

            return !Inventory.Items.Any(x => RelevantGenericItems.ShadowwebSpinner.Contains(x.HighId));
        }

        #endregion

        #region Keeper

        public static double weaponCheckDelay;

        public static List<string> allWeaponNames = new List<string>
            {
                "Azure Cobra of Orma",
                "Wixel's Notum Python",
                "Asp of Semol",
                "Viper Staff",
                "Asp of Titaniush",
                "Gold Acantophis",
                "Bitis Striker",
                "Coplan's Hand Taipan",
                "The Crotalus",
            };

        public static List<string> allShieldNames = new List<string>
        {
                "Shield of Zset",
                "Shield of Esa",
                "Shield of Asmodian",
                "Mocham's Guard",
                "Death Ward",
                "Belthior's Flame Ward",
                "Wave Breaker",
                "Solar Guard",
                "Notum Defender",
                "Vital Buckler",
                "Living Shield of Evernan",
        };

        protected bool TwoHandedWeapon(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["SummonedWeaponSelection"].AsInt32() != 1) return false;

            if (Time.AONormalTime < weaponCheckDelay && DynelManager.LocalPlayer.IsAttacking && Spell.HasPendingCast && Spell.List.Any(s => !s.IsReady)) return false;

            if (HasWeapon() || HasShield()) return false;

            return true;
        }
        protected bool OneHandedWeapon(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["SummonedWeaponSelection"].AsInt32() != 2) return false;

            if (Time.AONormalTime < weaponCheckDelay && DynelManager.LocalPlayer.IsAttacking && Spell.HasPendingCast && Spell.List.Any(s => !s.IsReady)) return false;

            if (HasWeapon()) return false;

            return true;
        }
        protected bool ShieldWeapon(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["SummonedWeaponSelection"].AsInt32() < 2) return false;

            if (_settings["SummonedWeaponSelection"].AsInt32() == 2 && spell.Id == 273376) return false;

            if (Time.AONormalTime < weaponCheckDelay && DynelManager.LocalPlayer.IsAttacking && Spell.HasPendingCast && Spell.List.Any(s => !s.IsReady)) return false;

            if (HasShield()) return false;

            return true;
        }

        public static bool HasWeapon()
        {
            foreach (Item weapon in Inventory.Items)
            {
                if (allWeaponNames.Contains(weapon.Name))
                    return true;
            }
            return false;
        }

        public static bool HasShield()
        {
            foreach (Item weapon in Inventory.Items)
            {
                if (allShieldNames.Contains(weapon.Name))
                    return true;
            }
            return false;
        }

        #endregion
    }
}
