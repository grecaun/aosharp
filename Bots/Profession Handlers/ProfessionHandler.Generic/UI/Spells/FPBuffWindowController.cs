using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class FPBuffUiConfig : IUiConfig, IOptionsConfig
{
    public int[] FPBuffIDs;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => FPBuffIDs ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
    public List<(string Name, int Value)> Options { get; set; }
}

public class FPBuffWindowController
{
    private GenericWindowController<FPBuffUiConfig> _FPBuffWindow;
    public Window CurrentWindow => _FPBuffWindow.CurrentWindow;

    #region Lists

    public static readonly List<(string Name, int Value)> BuffOptions = new List<(string, int)> { ("Off", 0), ("Self", 1), ("Local Team", 2) };

    public static List<FPBuffUiConfig> SelfBuff = new List<FPBuffUiConfig>
    {
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserArmorBuffMartialArtist, SettingKey ="UserArmorBuffMartialArtist", Label = "Armor Buff", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserFortifyNanoTechnician, SettingKey ="UserFortifyNanoTechnician", Label = "Fortify", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserInitiativeBuffsMartialArtist, SettingKey ="UserInitiativeBuffsMartialArtist", Label = "Initiative Buffs", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserNanoResistanceBuffsAdventurer, SettingKey ="UserNanoResistanceBuffsAdventurer", Label = "Nano Resistance Buffs", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).Select(s=>s.Id).ToArray(), SettingKey ="SelfRoot_SnareResistBuff", Label = "Self Root/Snare Resist Buff", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.DamageShieldUpgrades).Select(s=>s.Id).ToArray(), SettingKey = "DamageShieldUpgrades", Label ="Damage Shield Upgrades", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine._1HEdgedBuff).Select(s=>s.Id).ToArray(), SettingKey = "_1HEdgedBuff", Label ="1HEdged Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserAimedShotBuffsAgent, SettingKey ="AimedShotBuffsSelf", Label = "Aimed Shot Buffs", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserSneakAttackBuffs, SettingKey ="SneakAttackBuffsSelf", Label = "Sneak Attack Buffs", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.AgentDamageProc_DamageInflictSegment).Select(s=>s.Id).ToArray(), SettingKey = "AgentDamageProc_DamageInflictSegment", Label ="Agent Damage Proc-DamageInflictSegment", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.AgentProcBuff).Select(s=>s.Id).ToArray(), SettingKey = "AgentProcBuff", Label ="Agent Proc Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.ExecutionerBuff).Select(s=>s.Id).ToArray(), SettingKey = "ExecutionerBuff", Label ="Executioner Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.DamageChangeBuffs).Select(s=>s.Id).ToArray(), SettingKey = "DamageChangeBuffs", Label ="Damage Change Buffs", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.EnforcerMeleeEnergyBuff).Select(s=>s.Id).ToArray(), SettingKey = "EnforcerMeleeEnergyBuff", Label ="Enforcer Melee Energy Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.EnforcerPiercingBuff).Select(s=>s.Id).ToArray(), SettingKey = "EnforcerPiercingBuff", Label ="Enforcer Piercing Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.EnforcerTauntProcs).Select(s=>s.Id).ToArray(), SettingKey = "EnforcerTauntProcs", Label ="Enforcer Taunt Procs", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.FastAttackBuffs).Select(s=>s.Id).ToArray(), SettingKey = "FastAttackBuffs", Label ="Fast Attack Buffs", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.MeleeWeaponBuffLine).Select(s=>s.Id).ToArray(), SettingKey = "MeleeWeaponBuffLine", Label ="Melee Weapon Buff Line", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = SpellID.EngineeringBuff, SettingKey ="EngineeringBuff", Label = "Engineering Buff", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserEngineerSpecialAttackAbsorber, SettingKey ="EngineerSpecialAttackAbsorberSelf", Label = "Engineer Special Attack Absorber", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.FixerDodgeBuffLine).Select(s=>s.Id).ToArray(), SettingKey = "FixerDodgeBuffLine", Label ="Fixer Dodge Buff Line", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.FixerSuppressorBuff).Select(s=>s.Id).ToArray(), SettingKey ="FixerSuppressorBuff", Label = "Fixer Suppressor Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine._2HEdgedBuff).Select(s=>s.Id).ToArray(), SettingKey ="_2HEdgedBuff", Label = "2H Edged Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperDeflect_RiposteBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperDeflect_RiposteBuff", Label = "Keeper Deflect/Riposte Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperEvade_Dodge_DuckBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperEvade_Dodge_DuckBuff", Label = "Keeper Evade/Dodge/Duck Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperProcBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperProcBuff", Label = "Keeper Proc Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperStr_Stam_AgiBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperStr_Stam_AgiBuff", Label = "Keeper Str/Stam/Agi Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.Fury).Select(s=>s.Id).ToArray(), SettingKey ="Fury", Label = "Fury", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserDamageBuff_LineC, SettingKey ="DamageBuff_LineCSelf", Label = "Damage Buff - Line C", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.MartialArtistZazenStance).Select(s=>s.Id).ToArray(), SettingKey ="MartialArtistZazenStance", Label = "Martial Artist Zazen Stance", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoResistBuff).Select(s=>s.Id).ToArray(), SettingKey ="NanoResistBuff", Label = "Nano Resist Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.RiposteBuff).Select(s=>s.Id).ToArray(), SettingKey ="RiposteBuff", Label = "Riposte Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserMatCreaBuff, SettingKey ="MatCreaBuffSelf", Label = "MatCrea Buff", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoDamageMultiplierBuffs).Select(s=>s.Id).ToArray(), SettingKey ="NanoDamageMultiplierBuffs", Label = "Nano Damage Multiplier Buffs", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.ShadePiercingBuff).Select(s=>s.Id).ToArray(), SettingKey ="ShadePiercingBuff", Label = "Shade Piercing Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.ShadeProcBuff).Select(s=>s.Id).ToArray(), SettingKey ="ShadeProcBuff", Label = "Shade Proc Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.WeaponEffectAdd_On2).Select(s=>s.Id).ToArray(), SettingKey ="WeaponEffectAdd_On2", Label = "Weapon Effect Add-On 2", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserAssaultRifleBuffs, SettingKey ="AssaultRifleBuffsSelf", Label = "Assault Rifle Buffs", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserRangedEnergyWeaponBuffs, SettingKey ="RangedEnergyWeaponBuffsSelf", Label = "Ranged Energy Weapon Buffs", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserBurstBuff, SettingKey ="BurstBuffSelf", Label = "Burst Buff", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.HeavyWeaponsBuffs).Select(s=>s.Id).ToArray(), SettingKey ="HeavyWeaponsBuffs", Label = "Heavy Weapons Buffs", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.SoldierFullAutoBuff).Select(s=>s.Id).ToArray(), SettingKey ="SoldierFullAutoBuff", Label = "Soldier Full Auto Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.SoldierShotgunBuff).Select(s=>s.Id).ToArray(), SettingKey ="SoldierShotgunBuff", Label = "Soldier Shotgun Buff", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.TotalFocus).Select(s=>s.Id).ToArray(), SettingKey ="TotalFocus", Label = "Total Focus", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = SpellID.SiphonBox683Soldier, SettingKey ="SiphonBox683", Label = "Siphon Box: 683", UiType = UiType.DropDownWOption },
        new FPBuffUiConfig{FPBuffIDs = SpellID.SlayerdroidTransference, SettingKey ="SlayerdroidTransference", Label = "Slayerdroid Transference", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserReflectShieldNanoTechnician, SettingKey ="ReflectShieldNanoTechnicianSelf", Label = "Reflect Shield", UiType = UiType.DropDownWOption},
    };

    public static List<FPBuffUiConfig> TargetBuff = new List<FPBuffUiConfig>
    {
        new FPBuffUiConfig{FPBuffIDs = new[]{ SpellID.InsightIntoSL }, SettingKey = "SLMap", Label ="Insight into the Shadowlands", UiType = UiType.DualDropDown },
        new FPBuffUiConfig{FPBuffIDs = SpellID.TargetNanoResistanceBuffs, SettingKey ="TargetNanoResistanceBuffs", Label = "Nano Resistance Buffs", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.OtherRoot_SnareResistBuff).Select(s=>s.Id).ToArray(), SettingKey ="OtherRoot_SnareResistBuff", Label = "Other Root/Snare Resist Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TargetAgilityBuff, SettingKey ="AgilityBuffTarget", Label = "Agility Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.SenseBuff).Select(s=>s.Id).ToArray(), SettingKey ="SenseBuff", Label = "Sense Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoDeltaBuffs).Select(s=>s.Id).ToArray(), SettingKey ="NanoDeltaBuffs", Label = "Nano Delta Buffs", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TargetPsy_IntBuff, SettingKey ="Psy_IntBuffTarget", Label = "Psy/Int Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.PsychologyBuff).Select(s=>s.Id).ToArray(), SettingKey ="PsychologyBuff", Label = "Psychology Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.CriticalDecreaseBuff).Select(s=>s.Id).ToArray(), SettingKey ="CriticalDecreaseBuff", Label = "Critical Decrease Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.HealDeltaBuff).Select(s=>s.Id).ToArray(), SettingKey ="HealDeltaBuff", Label = "Heal Delta Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine._1HBluntBuff).Select(s=>s.Id).ToArray(), SettingKey ="_1HBluntBuff", Label = "1H Blunt Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.GrenadeBuffs).Select(s=>s.Id).ToArray(), SettingKey ="GrenadeBuffs", Label = "Grenade Buffs", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.BrawlBuff).Select(s=>s.Id).ToArray(), SettingKey ="BrawlBuff", Label = "Brawl Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.ControlledRageBuff).Select(s=>s.Id).ToArray(), SettingKey ="ControlledRageBuff", Label = "Controlled Rage Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TargetDamageBuff_LineC, SettingKey ="DamageBuff_LineCTarget", Label = "Damage Buff - Line C", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TargetMartialArtsBuffMartialArtist, SettingKey ="MartialArtsBuffTarget", Label = "Martial Arts Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = new int[]{SpellID.LimboMastery }, SettingKey ="LimboMastery", Label = "Limbo Mastery", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.MPCompositeNano, SettingKey ="MPCompositeNano", Label = "MP Composite Nano", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.BioMetBuff).Select(s=>s.Id).ToArray(), SettingKey ="BioMetBuff", Label = "BioMet Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TargetMatCreaBuff, SettingKey ="MatCreaBuffTarget", Label = "MatCrea Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.MatMetBuff).Select(s=>s.Id).ToArray(), SettingKey ="MatMetBuff", Label = "MatMet Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.PsyModBuff).Select(s=>s.Id).ToArray(), SettingKey ="PsyModBuff", Label = "PsyMod Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.SenImpBuffs, SettingKey ="SenseImpBuff", Label = "SenseImp Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.MatLocBuff).Select(s=>s.Id).ToArray(), SettingKey ="MatLocBuff", Label = "MatLoc Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.InterruptModifier).Select(s=>s.Id).ToArray(), SettingKey ="InterruptModifier", Label = "Interrupt Modifier", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoOverTime_LineA).Select(s=>s.Id).ToArray(), SettingKey ="NanoOverTime_LineA", Label = "Nano Over Time - Line A", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.NFRangeBuff).Select(s=>s.Id).ToArray(), SettingKey ="NFRangeBuff", Label = "NF Range Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.AADBuffs).Select(s=>s.Id).ToArray(), SettingKey ="AADBuffs", Label = "AAD Buffs", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TargetAssaultRifleBuffs, SettingKey ="AssaultRifleBuffsTarget", Label = "Assault Rifle Buffs", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.CompositeHeavyArtillery, SettingKey ="CompositeHeavyArtillery", Label = "Composite Heavy Artillery", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TargetRangedEnergyWeaponBuffs, SettingKey ="RangedEnergyWeaponBuffsTarget", Label = "Ranged Energy Weapon Buffs", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TargetBurstBuff, SettingKey ="BurstBuffTarget", Label = "Burst Buff", UiType = UiType.DualDropDown},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.Break_EntryBuffs).Select(s=>s.Id).ToArray(), SettingKey ="Break_EntryBuffs", Label = "Break & Entry Buffs", UiType = UiType.DualDropDown},
    };

    public static List<FPBuffUiConfig> TeamBuff = new List<FPBuffUiConfig>
    {
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.ShadowlandReflectBase).Select(s=>s.Id).ToArray(), SettingKey ="ShadowlandReflectBaseTeam", Label = "Shadowland Reflect Base", UiType = UiType.DropDownWOption},

        new FPBuffUiConfig{FPBuffIDs = SpellID.TeamConcealmentBuffAgent, SettingKey ="ConcealmentBuffTeam", Label = "Concealment Buff", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.SpeechLine).Select(s=>s.Id).ToArray(), SettingKey ="SpeechLine", Label = "Speech Line", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.DoctorHPBuffs, SettingKey ="DoctorHPBuffs", Label = "Doctor HP Buffs", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.EngineerAuras).Select(s=>s.Id).ToArray(), SettingKey ="EngineerAuras", Label = "Engineer Auras", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TeamEngineerSpecialAttackAbsorber, SettingKey = "EngineerSpecialAttackAbsorberTeam", Label = "Engineer Special Attack Absorber", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.SpecialAttackAbsorberBase).Select(s=>s.Id).ToArray(), SettingKey ="SpecialAttackAbsorberBase", Label = "Special Attack Absorber Base", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.FixerNCUBuffs, SettingKey = "FixerNCUBuffs", Label = "Fixer NCU Buffs", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.ShadowlandsRunspeed, SettingKey = "ShadowlandsRunspeed", Label = "Shadowlands Runspeed", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperAura_Absorb_Reflect_AMSBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperAura_Absorb_Reflect_AMSBuff", Label = "Keeper Aura-Absorb/Reflect/AMS Buff", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperAura_Damage_SnareReductionBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperAura_Damage_SnareReductionBuff", Label = "Keeper Aura-Damage/Snare Reduction Buff", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperAura_HPandNPHeal).Select(s=>s.Id).ToArray(), SettingKey ="KeeperAura_HPandNPHeal", Label = "Keeper Aura-HP and NP Heal", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.TeamDamageBuff_LineC, SettingKey = "DamageBuff_LineCTeam", Label = "Damage Buff - Line C", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.Horde, SettingKey = "Horde", Label = "Horde", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.Cohort, SettingKey = "Cohort", Label = "Cohort", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.SoldierDamageBase).Select(s=>s.Id).ToArray(), SettingKey ="SoldierDamageBase", Label = "Soldier Damage Base", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = new int[] { SpellID.Phalanx}, SettingKey = "Phalanx", Label = "Phalanx", UiType = UiType.DropDownWOption},
        new FPBuffUiConfig{FPBuffIDs = SpellID.UmbralWrangler, SettingKey = "UmbralWrangler", Label = "Umbral Wrangler", UiType = UiType.DropDownWOption},
    };

    public static readonly List<(string Name, int Value)> CombatBuffOptions = new List<(string, int)> { ("Off", 0), ("Target", 1), ("Boss", 2) };
    public static readonly List<(string Name, int Value)> UserCycle = new List<(string Name, int Value)> { ("Off", 0), ("Buff", 1), ("Cycle (HP%)", 2) };
    public static readonly List<(string Name, int Value)> UserCycleTime = new List<(string Name, int Value)> { ("Off", 0), ("Buff", 1), ("Cycle (seconds)", 2) };
    public static readonly List<(string Name, int Value)> TargetCycle = new List<(string Name, int Value)> { ("Off", 0), ("Buff", 1), ("Cycle(HP%)", 2), ("Cycle and Team Buff", 3) };
    public static readonly List<(string Name, int Value)> TargetCycleTime = new List<(string Name, int Value)> { ("Off", 0), ("Buff", 1), ("Cycle(seconds)", 2), ("Cycle and Team Buff", 3) };
    public static readonly List<(string Name, int Value)> On_Off = new List<(string Name, int Value)> { ("Off", 0), ("On (HP%)", 1), };

    public static List<FPBuffUiConfig> FightingTarget = new List<FPBuffUiConfig>()
    {
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.Charge).Select(s=>s.Id).ToArray(), SettingKey ="Charge", Label = "Charge", UiType = UiType.DualDropDown, Options = CombatBuffOptions },
    };

    public static List<FPBuffUiConfig> CombatSelfBuff = new List<FPBuffUiConfig>()
    {
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.ConcentrationCriticalLine).Select(s=>s.Id).ToArray(), SettingKey ="ConcentrationCriticalLine", Label = "Concentration Critical Line", UiType = UiType.DualDropDown, Options = CombatBuffOptions },
        new FPBuffUiConfig{FPBuffIDs = SpellID.TakeTheBullet, SettingKey = "TakeTheBullet", Label = "Take the Bullet", UiType = UiType.DropDownInputBox, Options = On_Off },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.Ransack_DepriveResistBuff).Select(s=>s.Id).ToArray(), SettingKey ="Ransack_DepriveResistBuff", Label = "Ransack/Deprive Resist Buff", UiType = UiType.DualDropDown, Options = CombatBuffOptions },

        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.Challenger).Select(s=>s.Id).ToArray(), SettingKey ="Challenger", Label = "Challenger", UiType = UiType.DropDownInputBoxDropDown, Options = UserCycleTime },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.Rage).Select(s=>s.Id).ToArray(), SettingKey ="Rage", Label = "Rage", UiType = UiType.DropDownInputBoxDropDown, Options = UserCycleTime },

        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.FixerFearImmunity).Select(s=>s.Id).ToArray(), SettingKey ="FixerFearImmunity", Label = "Fixer Fear Immunity", UiType = UiType.DualDropDown, Options = CombatBuffOptions },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.NullitySphereNano).Select(s => s.Id).ToArray(), SettingKey = "NullitySphereNano", Label ="Nullity Sphere Nano", UiType = UiType.DropDownInputBoxDropDown, Options = On_Off },
        new FPBuffUiConfig{FPBuffIDs = new int[] { SpellID.NanobotAegis}, SettingKey = "NanobotAegis", Label ="Nanobot Aegis", UiType = UiType.DropDownInputBoxDropDown, Options = On_Off },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.DamagetoNano).Select(s => s.Id).ToArray(), SettingKey = "DamagetoNano", Label ="Damage to Nano", UiType = UiType.DropDownInputBoxDropDown, Options = On_Off },
        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.ControlledDestructionBuff).Select(s=>s.Id).ToArray(), SettingKey ="ControlledDestructionBuff", Label = "Controlled Destruction Buff", UiType = UiType.DualDropDown, Options = CombatBuffOptions },
        new FPBuffUiConfig{FPBuffIDs = SpellID.UserDamageBuff_LineCMartialArtist, SettingKey ="DamageBuff_LineCSelfMartialArtist", Label = "Damage Buff - Line C", UiType = UiType.DualDropDown, Options = CombatBuffOptions },

        new FPBuffUiConfig{FPBuffIDs = SpellID.UserReflectShieldSoldier, SettingKey ="ReflectShieldSelf", Label = "Reflect Shield", UiType = UiType.DropDownInputBoxDropDown, Options = On_Off},

        new FPBuffUiConfig{FPBuffIDs = Spell.GetSpellsForNanoline(NanoLine.AdventurerMorphBuff).Select(s=>s.Id).ToArray(), SettingKey ="AdventurerMorphBuff", Label="Adventurer Morph Buff", UiType = UiType.DropDown, Options = CombatBuffOptions},

        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.MongoRage }, SettingKey = "MongoRageSelection", Label = "Mongo Rage", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.NotumShield }, SettingKey = "NotumShieldSelection", Label = "Notum Shield", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.TackyHack }, SettingKey = "TackyHackSelection", Label = "Tacky Hack", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.PowerOfLight }, SettingKey = "PowerOfLightSelection", Label = "Power Of Light", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.BladeOfNight }, SettingKey = "BladeOfNightSelection", Label = "Blade Of Night", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Beckoning }, SettingKey = "BeckoningSelection", Label = "Beckoning", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Overrule }, SettingKey = "OverruleSelection", Label = "Overrule", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.FreakShield }, SettingKey = "FreakShieldSelection", Label = "Freak Shield", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.HammerAndAnvil }, SettingKey = "HammerAndAnvilSelection", Label = "Hammer And Anvil", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Highway }, SettingKey = "HighwaySelection", Label = "Highway", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.DevotionalArmor }, SettingKey = "DevotionalArmorSelection", Label = "Devotional Armor", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.BladeWhirlwind }, SettingKey = "BladeWhirlwindSelection", Label = "Blade Whirlwind", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Moonmist }, SettingKey = "MoonmistSelection", Label = "Moonmist", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.FlimFocus }, SettingKey = "FlimFocusSelection", Label = "Flim Focus", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.ProgramOverload }, SettingKey = "ProgramOverloadSelection", Label = "Program Overload", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Violence }, SettingKey = "ViolenceSelection", Label = "Violence", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.SupressiveHorde }, SettingKey = "SupressiveHordeSelection", Label = "Supressive Horde", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Sacrifice }, SettingKey = "SacrificeSelection", Label = "Sacrifice", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Energize }, SettingKey = "EnergizeSelection", Label = "Energize", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.ReinforceSlugs }, SettingKey = "ReinforceSlugsSelection", Label = "Reinforce Slugs", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.KnowledgeEnhancer }, SettingKey = "KnowledgeEnhancerSelection", Label = "Knowledge Enhancer", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Insight }, SettingKey = "InsightSelection", Label = "Insight", UiType = UiType.RadioButtonGroup },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.ToxicShock }, SettingKey = "ToxicShockSelection", Label = "Toxic Shock", UiType = UiType.RadioButtonGroup },

        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Sphere }, SettingKey = "SphereValue", Label = "Sphere Delay", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.MyOwnFortress }, SettingKey = "MyOwnFortressValue", Label = "My Own Fortress", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.WitOfTheAtrox }, SettingKey = "WitOfTheAtroxValue", Label = "Wit Of The Atrox", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.DodgeTheBlame }, SettingKey = "DodgeTheBlameValue", Label = "Dodge The Blame", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.BioShield }, SettingKey = "BioShieldValue", Label = "Bio Shield", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.BioCocoon }, SettingKey = "BioCocoonValue", Label = "Bio Cocoon", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.EncaseInStone }, SettingKey = "EncaseInStoneValue", Label = "Encase In Stone", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.Limber }, SettingKey = "LimberValue", Label = "Limber", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.DanceOfFools }, SettingKey = "DanceOfFoolsValue", Label = "Dance Of Fools", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.EvasiveStance }, SettingKey = "EvasiveStanceValue", Label = "Evasive Stance", UiType = UiType.TextInput },
        new FPBuffUiConfig { FPBuffIDs = new int[]{ (int)PerkHash.TrollForm }, SettingKey = "TrollFormValue", Label = "Troll Form", UiType = UiType.TextInput },

    };

    public static List<FPBuffUiConfig> CombatTargetBuff = new List<FPBuffUiConfig>()
    {

    };

    public static List<FPBuffUiConfig> CombatTeamBuff = new List<FPBuffUiConfig>()
    {

    };





    public static readonly List<(string Name, int Value)> NukeTargetOptions = new List<(string, int)> { ("None", 0), ("Target", 1), ("Boss", 2), };

    public static List<FPBuffUiConfig> Dots = new List<FPBuffUiConfig>
    {
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.DOT_LineA).Select(s => s.Id).ToArray(), SettingKey = "DOT_LineA", Label = "DOT Line A", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.DOT_LineB).Select(s => s.Id).ToArray(), SettingKey = "DOT_LineB", Label = "DOT Line B", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.DOTNanotechnicianStrainA).Select(s => s.Id).ToArray(), SettingKey = "DOTNanotechnicianStrainA", Label = "DOT Nanotechnician Strain A", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.DOTAgentStrainA).Select(s => s.Id).ToArray(), SettingKey = "DOTAgentStrainA", Label = "DOT Agent Strain A", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.DOTStrainC).Select(s => s.Id).ToArray(), SettingKey = "DOTStrainC", Label = "DOT Strain C", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions }
    };

    public static List<FPBuffUiConfig> Nukes = new List<FPBuffUiConfig>
    {
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.Nukes).Select(s => s.Id).ToArray(), SettingKey = "Nukes", Label = "Nukes", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.AlphaNukes).Select(s => s.Id).ToArray(), SettingKey = "AlphaNukes", Label = "Alpha Nukes", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.Nuke).Select(s => s.Id).ToArray(), SettingKey = "Nuke", Label = "Nuke", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.AlphaNuke).Select(s => s.Id).ToArray(), SettingKey = "AlphaNuke", Label = "Alpha Nuke", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.OmegaNuke).Select(s => s.Id).ToArray(), SettingKey = "OmegaNuke", Label = "Omega Nuke", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.SpecialEffectNukes).Select(s => s.Id).ToArray(), SettingKey = "SpecialEffectNukes", Label = "Special Effect Nukes", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= SpellID.NTNukes, SettingKey = "NTNukesA", Label = "NT Nukes A", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.DOTNanotechnicianStrainB).Select(s => s.Id).ToArray(), SettingKey = "DOTNanotechnicianStrainB", Label = "DOT Nanotechnician Strain B", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= SpellID.NTNukes, SettingKey = "NTNukesB", Label = "NT Nukes B", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= SpellID.CratNormalNuke, SettingKey ="NormalNuke", Label ="Normal Nuke", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= SpellID.CratSpecialNuke, SettingKey ="SpecialNuke", Label ="Special Nuke", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= SpellID.MPNormalNuke, SettingKey ="MPNormalNuke", Label ="Normal Nuke", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= SpellID.MPMindDamage, SettingKey ="MindDamage", Label ="Mind Damage", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
    };

    public static List<FPBuffUiConfig> AOENukes = new List<FPBuffUiConfig>
    {
        new FPBuffUiConfig { FPBuffIDs= SpellID.NTSLAOENukes, SettingKey = "NTSLAOENukes", Label = "NT SL AOE Nukes", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= SpellID.NTRKAOENukes, SettingKey = "NTRKAOENukes", Label = "NT RK AOE Nukes", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.NTAreaNukes).Select(s => s.Id).ToArray(), SettingKey = "NTAreaNukes", Label = "NT Area Nukes", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.NTAreaNukes2).Select(s => s.Id).ToArray(), SettingKey = "NTAreaNukes2", Label = "NT Area Nukes 2", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.Nukes).Select(s => s.Id).ToArray(), SettingKey = "Nukes", Label = "Nukes", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
        new FPBuffUiConfig { FPBuffIDs= Spell.GetSpellsForNanoline(NanoLine.AOENuke).Select(s => s.Id).ToArray(), SettingKey = "AOENuke", Label = "AOE Nuke", UiType = UiType.ThreeDropDown, Options = NukeTargetOptions },
    };

    #endregion

    public FPBuffWindowController(HandlerSettings settings)
    {
        _FPBuffWindow = new GenericWindowController<FPBuffUiConfig>(settings);
    }

    public void FPBuffShowWindow(string pluginDirectory, string title, HashSet<int> nonCombatList, HashSet<int> combatList)
    {
        try
        {
            var columns = new List<(List<(string, IEnumerable<FPBuffUiConfig>)>, HashSet<int>, HashSet<int>)>();

            if (nonCombatList != null && nonCombatList.Any())
            {
                columns.Add((
                    new List<(string, IEnumerable<FPBuffUiConfig>)>
                    {
                    ("Self", SelfBuff),
                    ("Target", TargetBuff),
                    ("Team Buffs", TeamBuff)
                    },
                    nonCombatList,
                    nonCombatList
                ));
            }

            if (combatList != null && combatList.Any())
            {
                columns.Add((
                    new List<(string, IEnumerable<FPBuffUiConfig>)>
                    {
                    ("Fighting Target", FightingTarget),
                    ("Combat Self", CombatSelfBuff),
                    ("Combat Target", CombatTargetBuff),
                    ("Combat Team", CombatTeamBuff),
                    ("DOTs", Dots),
                    ("Nukes", Nukes),
                    ("AOE Nukes", AOENukes)
                    },
                    combatList,
                    combatList
                ));
            }

            _FPBuffWindow.ShowWindow(
                title: title,
                xmlPath: pluginDirectory + "\\UI\\BuffWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                columns: columns,
                extraColumn: null,
                pluginDirectory: pluginDirectory,
                options: BuffOptions
            );
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
