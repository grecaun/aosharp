using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class BuffUiConfig : IUiConfig, IOptionsConfig
{
    public int[] BuffIDs;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => BuffIDs ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
    public List<(string Name, int Value)> Options { get; set; }
}

public class BuffWindowController
{
    private GenericWindowController<BuffUiConfig> _BuffWindow;
    public Window CurrentWindow => _BuffWindow.CurrentWindow;

    #region Lists

    public static readonly List<(string Name, int Value)> BuffOptions = new List<(string, int)> { ("Off", 0), ("Self", 1), ("Local Team", 2) };

    public static List<BuffUiConfig> SelfBuff = new List<BuffUiConfig>
    {
        new BuffUiConfig{BuffIDs = new[]{ SpellID.CompositeAttribute }, SettingKey = "CompositeAttribute", Label ="Composite Attribute Boost", UiType = UiType.Checkbox },
        new BuffUiConfig{BuffIDs = new[]{ SpellID.CompositeNano }, SettingKey = "CompositeNano", Label ="Composite Nano Expertise", UiType = UiType.Checkbox },
        new BuffUiConfig{BuffIDs = new[]{ SpellID.CompositeUtility }, SettingKey = "CompositeUtility", Label ="Composite Utility Expertise", UiType = UiType.Checkbox },
        new BuffUiConfig{BuffIDs = new[]{ SpellID.CompositeTradeskill }, SettingKey = "CompositeTradeskill", Label ="Composite Tradeskill Expertise", UiType = UiType.Checkbox },
        new BuffUiConfig{BuffIDs = new[]{ SpellID.CompositeMartialProwess }, SettingKey = "CompositeMartialProwess", Label ="Composite Martial Prowess Expertise", UiType = UiType.Checkbox },
        new BuffUiConfig{BuffIDs = new[]{ SpellID.CompositeMelee }, SettingKey = "CompositeMelee", Label ="Composite Melee Expertise", UiType = UiType.Checkbox },
        new BuffUiConfig{BuffIDs = new[]{ SpellID.CompositePhysicalSpecial }, SettingKey = "CompositePhysicalSpecial", Label ="Composite Physical Special Expertise", UiType = UiType.Checkbox },
        new BuffUiConfig{BuffIDs = new[]{ SpellID.CompositeRanged }, SettingKey = "CompositeRanged", Label ="Composite Ranged Expertise", UiType = UiType.Checkbox },
        new BuffUiConfig{BuffIDs = new[]{ SpellID.CompositeRangedSpecial }, SettingKey = "CompositeRangedSpecial", Label ="Composite Ranged Special Expertise", UiType = UiType.Checkbox },
        new BuffUiConfig{BuffIDs = SpellID.UserArmorBuffMartialArtist, SettingKey ="UserArmorBuffMartialArtist", Label = "Armor Buff", UiType = UiType.DropDownWOption},

        new BuffUiConfig{BuffIDs = SpellID.UserFortifyNanoTechnician, SettingKey ="UserFortifyNanoTechnician", Label = "Fortify", UiType = UiType.DropDownWOption},

        new BuffUiConfig{BuffIDs = SpellID.UserInitiativeBuffsMartialArtist, SettingKey ="UserInitiativeBuffsMartialArtist", Label = "Initiative Buffs", UiType = UiType.DropDownWOption},

        new BuffUiConfig{BuffIDs = SpellID.UserNanoResistanceBuffsAdventurer, SettingKey ="UserNanoResistanceBuffsAdventurer", Label = "Nano Resistance Buffs", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.SelfRoot_SnareResistBuff).Select(s=>s.Id).ToArray(), SettingKey ="SelfRoot_SnareResistBuff", Label = "Self Root/Snare Resist Buff", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.DamageShieldUpgrades).Select(s=>s.Id).ToArray(), SettingKey = "DamageShieldUpgrades", Label ="Damage Shield Upgrades", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine._1HEdgedBuff).Select(s=>s.Id).ToArray(), SettingKey = "_1HEdgedBuff", Label ="1HEdged Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = SpellID.UserAimedShotBuffsAgent, SettingKey ="AimedShotBuffsSelf", Label = "Aimed Shot Buffs", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.UserSneakAttackBuffs, SettingKey ="SneakAttackBuffsSelf", Label = "Sneak Attack Buffs", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.AgentDamageProc_DamageInflictSegment).Select(s=>s.Id).ToArray(), SettingKey = "AgentDamageProc_DamageInflictSegment", Label ="Agent Damage Proc-DamageInflictSegment", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.AgentProcBuff).Select(s=>s.Id).ToArray(), SettingKey = "AgentProcBuff", Label ="Agent Proc Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.ExecutionerBuff).Select(s=>s.Id).ToArray(), SettingKey = "ExecutionerBuff", Label ="Executioner Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.DamageChangeBuffs).Select(s=>s.Id).ToArray(), SettingKey = "DamageChangeBuffs", Label ="Damage Change Buffs", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.EnforcerMeleeEnergyBuff).Select(s=>s.Id).ToArray(), SettingKey = "EnforcerMeleeEnergyBuff", Label ="Enforcer Melee Energy Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.EnforcerPiercingBuff).Select(s=>s.Id).ToArray(), SettingKey = "EnforcerPiercingBuff", Label ="Enforcer Piercing Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.EnforcerTauntProcs).Select(s=>s.Id).ToArray(), SettingKey = "EnforcerTauntProcs", Label ="Enforcer Taunt Procs", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.FastAttackBuffs).Select(s=>s.Id).ToArray(), SettingKey = "FastAttackBuffs", Label ="Fast Attack Buffs", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.MeleeWeaponBuffLine).Select(s=>s.Id).ToArray(), SettingKey = "MeleeWeaponBuffLine", Label ="Melee Weapon Buff Line", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = SpellID.EngineeringBuff, SettingKey ="EngineeringBuff", Label = "Engineering Buff", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.UserEngineerSpecialAttackAbsorber, SettingKey ="EngineerSpecialAttackAbsorberSelf", Label = "Engineer Special Attack Absorber", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.FixerDodgeBuffLine).Select(s=>s.Id).ToArray(), SettingKey = "FixerDodgeBuffLine", Label ="Fixer Dodge Buff Line", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.FixerSuppressorBuff).Select(s=>s.Id).ToArray(), SettingKey ="FixerSuppressorBuff", Label = "Fixer Suppressor Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine._2HEdgedBuff).Select(s=>s.Id).ToArray(), SettingKey ="_2HEdgedBuff", Label = "2H Edged Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperDeflect_RiposteBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperDeflect_RiposteBuff", Label = "Keeper Deflect/Riposte Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperEvade_Dodge_DuckBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperEvade_Dodge_DuckBuff", Label = "Keeper Evade/Dodge/Duck Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperProcBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperProcBuff", Label = "Keeper Proc Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperStr_Stam_AgiBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperStr_Stam_AgiBuff", Label = "Keeper Str/Stam/Agi Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.Fury).Select(s=>s.Id).ToArray(), SettingKey ="Fury", Label = "Fury", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = SpellID.UserDamageBuff_LineC, SettingKey ="DamageBuff_LineCSelf", Label = "Damage Buff - Line C", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.MartialArtistZazenStance).Select(s=>s.Id).ToArray(), SettingKey ="MartialArtistZazenStance", Label = "Martial Artist Zazen Stance", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoResistBuff).Select(s=>s.Id).ToArray(), SettingKey ="NanoResistBuff", Label = "Nano Resist Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.RiposteBuff).Select(s=>s.Id).ToArray(), SettingKey ="RiposteBuff", Label = "Riposte Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = SpellID.UserMatCreaBuff, SettingKey ="MatCreaBuffSelf", Label = "MatCrea Buff", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoDamageMultiplierBuffs).Select(s=>s.Id).ToArray(), SettingKey ="NanoDamageMultiplierBuffs", Label = "Nano Damage Multiplier Buffs", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.ShadePiercingBuff).Select(s=>s.Id).ToArray(), SettingKey ="ShadePiercingBuff", Label = "Shade Piercing Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.ShadeProcBuff).Select(s=>s.Id).ToArray(), SettingKey ="ShadeProcBuff", Label = "Shade Proc Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.WeaponEffectAdd_On2).Select(s=>s.Id).ToArray(), SettingKey ="WeaponEffectAdd_On2", Label = "Weapon Effect Add-On 2", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = SpellID.UserAssaultRifleBuffs, SettingKey ="AssaultRifleBuffsSelf", Label = "Assault Rifle Buffs", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.UserRangedEnergyWeaponBuffs, SettingKey ="RangedEnergyWeaponBuffsSelf", Label = "Ranged Energy Weapon Buffs", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.UserBurstBuff, SettingKey ="BurstBuffSelf", Label = "Burst Buff", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.HeavyWeaponsBuffs).Select(s=>s.Id).ToArray(), SettingKey ="HeavyWeaponsBuffs", Label = "Heavy Weapons Buffs", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.SoldierFullAutoBuff).Select(s=>s.Id).ToArray(), SettingKey ="SoldierFullAutoBuff", Label = "Soldier Full Auto Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.SoldierShotgunBuff).Select(s=>s.Id).ToArray(), SettingKey ="SoldierShotgunBuff", Label = "Soldier Shotgun Buff", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.TotalFocus).Select(s=>s.Id).ToArray(), SettingKey ="TotalFocus", Label = "Total Focus", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = SpellID.SiphonBox683Soldier, SettingKey ="SiphonBox683", Label = "Siphon Box: 683", UiType = UiType.DropDownWOption },
        new BuffUiConfig{BuffIDs = SpellID.SlayerdroidTransference, SettingKey ="SlayerdroidTransference", Label = "Slayerdroid Transference", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.UserReflectShieldNanoTechnician, SettingKey ="ReflectShieldNanoTechnicianSelf", Label = "Reflect Shield", UiType = UiType.DropDownWOption},

        new BuffUiConfig { BuffIDs = new[] { RelevantGenericItems.SteamingHotCupOfEnhancedCoffee }, SettingKey = "CoffeeOption", Label = "Coffee", UiType = UiType.Checkbox },

        new BuffUiConfig { BuffIDs = RelevantGenericItems.RingofKnowledge, SettingKey = "RingofKnowledgeOption", Label = "Ring of Knowledge", UiType = UiType.Checkbox },
        new BuffUiConfig { BuffIDs = new[] { RelevantGenericItems.BootsOfGridspaceDistortion }, SettingKey = "BootsOfGridspaceDistortionOption", Label = "Boots of Gridspace Distortion(Locks RS)", UiType = UiType.Checkbox },
        new BuffUiConfig { BuffIDs = RelevantGenericItems.BootsofInfiniteSpeed, SettingKey = "BootsofInfiniteSpeedOption", Label = "Boots of Infinite Speed(Locks RS)", UiType = UiType.Checkbox },

    };

    public static List<BuffUiConfig> TargetBuff = new List<BuffUiConfig>
    {
        new BuffUiConfig{BuffIDs = new[]{ SpellID.InsightIntoSL }, SettingKey = "SLMap", Label ="Insight into the Shadowlands", UiType = UiType.DualDropDown },
        new BuffUiConfig{BuffIDs = SpellID.TargetNanoResistanceBuffs, SettingKey ="TargetNanoResistanceBuffs", Label = "Nano Resistance Buffs", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.OtherRoot_SnareResistBuff).Select(s=>s.Id).ToArray(), SettingKey ="OtherRoot_SnareResistBuff", Label = "Other Root/Snare Resist Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.TargetAgilityBuff, SettingKey ="AgilityBuffTarget", Label = "Agility Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.SenseBuff).Select(s=>s.Id).ToArray(), SettingKey ="SenseBuff", Label = "Sense Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoDeltaBuffs).Select(s=>s.Id).ToArray(), SettingKey ="NanoDeltaBuffs", Label = "Nano Delta Buffs", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.TargetPsy_IntBuff, SettingKey ="Psy_IntBuffTarget", Label = "Psy/Int Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.PsychologyBuff).Select(s=>s.Id).ToArray(), SettingKey ="PsychologyBuff", Label = "Psychology Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.CriticalDecreaseBuff).Select(s=>s.Id).ToArray(), SettingKey ="CriticalDecreaseBuff", Label = "Critical Decrease Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.HealDeltaBuff).Select(s=>s.Id).ToArray(), SettingKey ="HealDeltaBuff", Label = "Heal Delta Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine._1HBluntBuff).Select(s=>s.Id).ToArray(), SettingKey ="_1HBluntBuff", Label = "1H Blunt Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.GrenadeBuffs).Select(s=>s.Id).ToArray(), SettingKey ="GrenadeBuffs", Label = "Grenade Buffs", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.BrawlBuff).Select(s=>s.Id).ToArray(), SettingKey ="BrawlBuff", Label = "Brawl Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.ControlledRageBuff).Select(s=>s.Id).ToArray(), SettingKey ="ControlledRageBuff", Label = "Controlled Rage Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.TargetDamageBuff_LineC, SettingKey ="DamageBuff_LineCTarget", Label = "Damage Buff - Line C", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.TargetMartialArtsBuffMartialArtist, SettingKey ="MartialArtsBuffTarget", Label = "Martial Arts Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = new int[]{SpellID.LimboMastery }, SettingKey ="LimboMastery", Label = "Limbo Mastery", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.MPCompositeNano, SettingKey ="MPCompositeNano", Label = "MP Composite Nano", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.BioMetBuff).Select(s=>s.Id).ToArray(), SettingKey ="BioMetBuff", Label = "BioMet Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.TargetMatCreaBuff, SettingKey ="MatCreaBuffTarget", Label = "MatCrea Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.MatMetBuff).Select(s=>s.Id).ToArray(), SettingKey ="MatMetBuff", Label = "MatMet Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.PsyModBuff).Select(s=>s.Id).ToArray(), SettingKey ="PsyModBuff", Label = "PsyMod Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.SenImpBuffs, SettingKey ="SenseImpBuff", Label = "SenseImp Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.MatLocBuff).Select(s=>s.Id).ToArray(), SettingKey ="MatLocBuff", Label = "MatLoc Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.InterruptModifier).Select(s=>s.Id).ToArray(), SettingKey ="InterruptModifier", Label = "Interrupt Modifier", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoOverTime_LineA).Select(s=>s.Id).ToArray(), SettingKey ="NanoOverTime_LineA", Label = "Nano Over Time - Line A", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.NFRangeBuff).Select(s=>s.Id).ToArray(), SettingKey ="NFRangeBuff", Label = "NF Range Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.AADBuffs).Select(s=>s.Id).ToArray(), SettingKey ="AADBuffs", Label = "AAD Buffs", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.TargetAssaultRifleBuffs, SettingKey ="AssaultRifleBuffsTarget", Label = "Assault Rifle Buffs", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.CompositeHeavyArtillery, SettingKey ="CompositeHeavyArtillery", Label = "Composite Heavy Artillery", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.TargetRangedEnergyWeaponBuffs, SettingKey ="RangedEnergyWeaponBuffsTarget", Label = "Ranged Energy Weapon Buffs", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = SpellID.TargetBurstBuff, SettingKey ="BurstBuffTarget", Label = "Burst Buff", UiType = UiType.DualDropDown},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.Break_EntryBuffs).Select(s=>s.Id).ToArray(), SettingKey ="Break_EntryBuffs", Label = "Break & Entry Buffs", UiType = UiType.DualDropDown},

        new BuffUiConfig { BuffIDs = new[] { RelevantGenericItems.ReflectGraft }, SettingKey = "ReflectGraftOption", Label = "Reflect Graft", UiType = UiType.Checkbox },
    };

    public static List<BuffUiConfig> TeamBuff = new List<BuffUiConfig>
    {
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.ShadowlandReflectBase).Select(s=>s.Id).ToArray(), SettingKey ="ShadowlandReflectBaseTeam", Label = "Shadowland Reflect Base", UiType = UiType.DropDownWOption},

        new BuffUiConfig{BuffIDs = SpellID.TeamConcealmentBuffAgent, SettingKey ="ConcealmentBuffTeam", Label = "Concealment Buff", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.SpeechLine).Select(s=>s.Id).ToArray(), SettingKey ="SpeechLine", Label = "Speech Line", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.DoctorHPBuffs, SettingKey ="DoctorHPBuffs", Label = "Doctor HP Buffs", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.EngineerAuras).Select(s=>s.Id).ToArray(), SettingKey ="EngineerAuras", Label = "Engineer Auras", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.TeamEngineerSpecialAttackAbsorber, SettingKey = "EngineerSpecialAttackAbsorberTeam", Label = "Engineer Special Attack Absorber", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.SpecialAttackAbsorberBase).Select(s=>s.Id).ToArray(), SettingKey ="SpecialAttackAbsorberBase", Label = "Special Attack Absorber Base", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.FixerNCUBuffs, SettingKey = "FixerNCUBuffs", Label = "Fixer NCU Buffs", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.ShadowlandsRunspeed, SettingKey = "ShadowlandsRunspeed", Label = "Shadowlands Runspeed", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperAura_Absorb_Reflect_AMSBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperAura_Absorb_Reflect_AMSBuff", Label = "Keeper Aura-Absorb/Reflect/AMS Buff", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperAura_Damage_SnareReductionBuff).Select(s=>s.Id).ToArray(), SettingKey ="KeeperAura_Damage_SnareReductionBuff", Label = "Keeper Aura-Damage/Snare Reduction Buff", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.KeeperAura_HPandNPHeal).Select(s=>s.Id).ToArray(), SettingKey ="KeeperAura_HPandNPHeal", Label = "Keeper Aura-HP and NP Heal", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.TeamDamageBuff_LineC, SettingKey = "DamageBuff_LineCTeam", Label = "Damage Buff - Line C", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.Horde, SettingKey = "Horde", Label = "Horde", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.Cohort, SettingKey = "Cohort", Label = "Cohort", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.SoldierDamageBase).Select(s=>s.Id).ToArray(), SettingKey ="SoldierDamageBase", Label = "Soldier Damage Base", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = new int[] { SpellID.Phalanx}, SettingKey = "Phalanx", Label = "Phalanx", UiType = UiType.DropDownWOption},
        new BuffUiConfig{BuffIDs = SpellID.UmbralWrangler, SettingKey = "UmbralWrangler", Label = "Umbral Wrangler", UiType = UiType.DropDownWOption},
    };

    public static readonly List<(string Name, int Value)> CombatBuffOptions = new List<(string, int)> { ("Off", 0), ("Target", 1), ("Boss", 2) };
    public static readonly List<(string Name, int Value)> UserCycleHP = new List<(string Name, int Value)> { ("Off", 0), ("Buff", 1), ("Cycle (HP%)", 2) };
    public static readonly List<(string Name, int Value)> UserCycleTime = new List<(string Name, int Value)> { ("Off", 0), ("Buff", 1), ("Cycle (seconds)", 2) };
    public static readonly List<(string Name, int Value)> TargetCycleHP = new List<(string Name, int Value)> { ("Off", 0), ("Buff Self", 1), ("Buff Team", 2), ("Cycle(HP%)", 3), ("Cycle and Team Buff", 4) };
    public static readonly List<(string Name, int Value)> TargetCycleTime = new List<(string Name, int Value)> { ("Off", 0), ("Buff Self", 1), ("Buff Team", 2), ("Cycle(seconds)", 3), ("Cycle and Team Buff", 4) };
    public static readonly List<(string Name, int Value)> On_Off = new List<(string Name, int Value)> { ("Off", 0), ("On (HP%)", 1), };

    public static List<BuffUiConfig> FightingTarget = new List<BuffUiConfig>()
    {
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.Charge).Select(s=>s.Id).ToArray(), SettingKey ="Charge", Label = "Charge", UiType = UiType.DualDropDown, Options = CombatBuffOptions },
    };

    public static List<BuffUiConfig> CombatSelfBuff = new List<BuffUiConfig>()
    {
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.ConcentrationCriticalLine).Select(s=>s.Id).ToArray(), SettingKey ="ConcentrationCriticalLine", Label = "Concentration Critical Line", UiType = UiType.DualDropDown, Options = CombatBuffOptions },
        new BuffUiConfig{BuffIDs = SpellID.TakeTheBullet, SettingKey = "TakeTheBullet", Label = "Take the Bullet", UiType = UiType.DropDownInputBox, Options = On_Off },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.Ransack_DepriveResistBuff).Select(s=>s.Id).ToArray(), SettingKey ="Ransack_DepriveResistBuff", Label = "Ransack/Deprive Resist Buff", UiType = UiType.DualDropDown, Options = CombatBuffOptions },

        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.Rage).Select(s=>s.Id).ToArray(), SettingKey ="Rage", Label = "Rage", UiType = UiType.DropDownInputBoxDropDown, Options = UserCycleTime },

        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.FixerFearImmunity).Select(s=>s.Id).ToArray(), SettingKey ="FixerFearImmunity", Label = "Fixer Fear Immunity", UiType = UiType.DualDropDown, Options = CombatBuffOptions },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.NullitySphereNano).Select(s => s.Id).ToArray(), SettingKey = "NullitySphereNano", Label ="Nullity Sphere Nano", UiType = UiType.DropDownInputBoxDropDown, Options = On_Off },
        new BuffUiConfig{BuffIDs = new int[] { SpellID.NanobotAegis}, SettingKey = "NanobotAegis", Label ="Nanobot Aegis", UiType = UiType.DropDownInputBoxDropDown, Options = On_Off },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.DamagetoNano).Select(s => s.Id).ToArray(), SettingKey = "DamagetoNano", Label ="Damage to Nano", UiType = UiType.DropDownInputBoxDropDown, Options = On_Off },
        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.ControlledDestructionBuff).Select(s=>s.Id).ToArray(), SettingKey ="ControlledDestructionBuff", Label = "Controlled Destruction Buff", UiType = UiType.DualDropDown, Options = CombatBuffOptions },
        new BuffUiConfig{BuffIDs = SpellID.UserDamageBuff_LineCMartialArtist, SettingKey ="DamageBuff_LineCSelfMartialArtist", Label = "Damage Buff - Line C", UiType = UiType.DualDropDown, Options = CombatBuffOptions },

        new BuffUiConfig{BuffIDs = SpellID.UserReflectShieldSoldier, SettingKey ="ReflectShieldSelf", Label = "Reflect Shield", UiType = UiType.DropDownInputBoxDropDown, Options = On_Off},

        new BuffUiConfig{BuffIDs = Spell.GetSpellsForNanoline(NanoLine.AdventurerMorphBuff).Select(s=>s.Id).ToArray(), SettingKey ="AdventurerMorphBuff", Label="Adventurer Morph Buff", UiType = UiType.DropDown, Options = CombatBuffOptions},

        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.MongoRage }, SettingKey = "MongoRageSelection", Label = "Mongo Rage", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.NotumShield }, SettingKey = "NotumShieldSelection", Label = "Notum Shield", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.TackyHack }, SettingKey = "TackyHackSelection", Label = "Tacky Hack", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.PowerOfLight }, SettingKey = "PowerOfLightSelection", Label = "Power Of Light", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.BladeOfNight }, SettingKey = "BladeOfNightSelection", Label = "Blade Of Night", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Beckoning }, SettingKey = "BeckoningSelection", Label = "Beckoning", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Overrule }, SettingKey = "OverruleSelection", Label = "Overrule", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.FreakShield }, SettingKey = "FreakShieldSelection", Label = "Freak Shield", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.HammerAndAnvil }, SettingKey = "HammerAndAnvilSelection", Label = "Hammer And Anvil", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Highway }, SettingKey = "HighwaySelection", Label = "Highway", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.DevotionalArmor }, SettingKey = "DevotionalArmorSelection", Label = "Devotional Armor", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.BladeWhirlwind }, SettingKey = "BladeWhirlwindSelection", Label = "Blade Whirlwind", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Moonmist }, SettingKey = "MoonmistSelection", Label = "Moonmist", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.FlimFocus }, SettingKey = "FlimFocusSelection", Label = "Flim Focus", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.ProgramOverload }, SettingKey = "ProgramOverloadSelection", Label = "Program Overload", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Violence }, SettingKey = "ViolenceSelection", Label = "Violence", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.SupressiveHorde }, SettingKey = "SupressiveHordeSelection", Label = "Supressive Horde", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Sacrifice }, SettingKey = "SacrificeSelection", Label = "Sacrifice", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Energize }, SettingKey = "EnergizeSelection", Label = "Energize", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.ReinforceSlugs }, SettingKey = "ReinforceSlugsSelection", Label = "Reinforce Slugs", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.KnowledgeEnhancer }, SettingKey = "KnowledgeEnhancerSelection", Label = "Knowledge Enhancer", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Insight }, SettingKey = "InsightSelection", Label = "Insight", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.ToxicShock }, SettingKey = "ToxicShockSelection", Label = "Toxic Shock", UiType = UiType.RadioButtonGroup },

        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.ToxicShock }, SettingKey = "ChaoticModulationSelection", Label = "Chaotic Modulation", UiType = UiType.RadioButtonGroup },

        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Sphere }, SettingKey = "SphereValue", Label = "Sphere Delay", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.MyOwnFortress }, SettingKey = "MyOwnFortressValue", Label = "My Own Fortress", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.WitOfTheAtrox }, SettingKey = "WitOfTheAtroxValue", Label = "Wit Of The Atrox", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.DodgeTheBlame }, SettingKey = "DodgeTheBlameValue", Label = "Dodge The Blame", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.BioShield }, SettingKey = "BioShieldValue", Label = "Bio Shield", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.BioCocoon }, SettingKey = "BioCocoonValue", Label = "Bio Cocoon", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.EncaseInStone }, SettingKey = "EncaseInStoneValue", Label = "Encase In Stone", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.Limber }, SettingKey = "LimberValue", Label = "Limber", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.DanceOfFools }, SettingKey = "DanceOfFoolsValue", Label = "Dance Of Fools", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.EvasiveStance }, SettingKey = "EvasiveStanceValue", Label = "Evasive Stance", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new int[]{ (int)PerkHash.TrollForm }, SettingKey = "TrollFormValue", Label = "Troll Form", UiType = UiType.TextInput },


        new BuffUiConfig { BuffIDs = RelevantGenericItems.TotwDmgShoulders, SettingKey = "TotwDmgShouldersSelection", Label = "ToTW Damage Shoulder(Strengh)", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = RelevantGenericItems.TotwShieldShoulders, SettingKey = "TotwShieldShouldersValue", Label = "ToTW Shield Shoulders(Locks body dev)", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new[] { RelevantGenericItems.AssaultClassTank }, SettingKey = "AssaultClassTankValue", Label = "Assault Class Tank", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new[]{RelevantGenericItems.SharlsCyberneticTattoo}, SettingKey = "SharlsCyberneticTattooValue", Label = "Sharl's Cybernetic Tattoo (Locks Bio Met)", UiType = UiType.TextInput },

        new BuffUiConfig { BuffIDs = RelevantGenericItems.FlurryOfBlows, SettingKey = "FlurryOfBlowsSelection", Label = "Flurry of Blows", UiType = UiType.RadioButtonGroup },
        new BuffUiConfig { BuffIDs = RelevantGenericItems.EyeOfTheHunter, SettingKey = "EyeOfTheHunterSelection", Label = "Eye Of The Hunter", UiType = UiType.RadioButtonGroup },

        new BuffUiConfig { BuffIDs = RelevantGenericItems.DreadlochEnduranceBooster, SettingKey = "DreadlochEnduranceBoosterValue", Label = "Dreadloch Endurance Booster(Locks Strengh)", UiType = UiType.TextInput },

        new BuffUiConfig { BuffIDs = RelevantGenericItems.AncientGenerationDevices, SettingKey = "AncientGenerationDevices", Label = "Ancient Generation Devices", UiType = UiType.RadioButtonGroup },

        new BuffUiConfig { BuffIDs = RelevantGenericItems.ReanimatedCloaks, SettingKey = "CloakoftheReanimatedOption", Label = "Cloak of the Reanimated", UiType = UiType.Checkbox },

        new BuffUiConfig { BuffIDs = RelevantGenericItems.GnuffsEternalRiftCrystal, SettingKey = "GnuffsEternalRiftCrystalSelection", Label = "Gnuff's Eternal Rift Crystal", UiType = UiType.RadioButtonGroup },

        new BuffUiConfig { BuffIDs = RelevantGenericItems.MuscularStim, SettingKey = "MuscularStimValue", Label = "Muscular Stim(Locks body dev)", UiType = UiType.TextInput },

        new BuffUiConfig { BuffIDs = RelevantGenericItems.BurstofSpeedStim, SettingKey = "BurstofSpeedStimOption", Label = "Burst of Speed Stim(Locks RS)", UiType = UiType.Checkbox },

        new BuffUiConfig { BuffIDs = new[]{ RelevantGenericItems.CurseofMalahde}, SettingKey = "CurseofMalahdeOption", Label = "Curse of Malahde(Locks RS)", UiType = UiType.Checkbox },
        new BuffUiConfig { BuffIDs = new[] { RelevantGenericItems.PolymerizingStim }, SettingKey = "PolymerizingStimValue", Label = "Polymerizing Stim", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new[] { RelevantGenericItems.MutatedSlitherBlood }, SettingKey = "MutatedSlitherBloodValue", Label = "Mutated Slither Blood", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new[] { RelevantGenericItems.IskopsAscendancy }, SettingKey = "IskopsAscendancyOption", Label = "Iskop's Ascendancy", UiType = UiType.Checkbox },
        new BuffUiConfig { BuffIDs = RelevantGenericItems.BacchantesAnunWings, SettingKey = "BacchantesAnunWingsValue", Label = "Bacchantes Anun Wings", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new[]{ RelevantGenericItems.CombatAssistWenWen }, SettingKey = "CombatAssistWenWenValue", Label = "Combat Assist Wen-Wen (locks First aid)", UiType = UiType.TextInput },

        new BuffUiConfig { BuffIDs = RelevantGenericItems.BoostedStim, SettingKey = "BoostedStimValue", Label = "Boosted Stim", UiType = UiType.TextInput },

        new BuffUiConfig { BuffIDs = new[]{ RelevantGenericItems.BoltarBrainBlaster}, SettingKey = "BoltarBrainBlasterValue", Label = "Boltar Brain Blaster HP %", UiType = UiType.TextInput },
        new BuffUiConfig { BuffIDs = new[]{ RelevantGenericItems.BioremediationStim}, SettingKey = "BioremediationStimValue", Label = "Bioremediation Stim HP %", UiType = UiType.TextInput },

    };

    public static List<BuffUiConfig> CombatTargetBuff = new List<BuffUiConfig>()
    {

    };

    public static List<BuffUiConfig> CombatTeamBuff = new List<BuffUiConfig>()
    {

    };

    #endregion

    public BuffWindowController(HandlerSettings settings)
    {
        _BuffWindow = new GenericWindowController<BuffUiConfig>(settings);
    }

    public void BuffShowWindow(string pluginDirectory)
    {
        try
        {
            var columns = new List<(List<(string, IEnumerable<BuffUiConfig>)>, HashSet<int>, HashSet<int>)>();

            if (LoadedNonCombatBuffs != null && LoadedNonCombatBuffs.Any())
            {
                columns.Add((
                    new List<(string, IEnumerable<BuffUiConfig>)>
                    {
                    ("Self", SelfBuff),
                    ("Target", TargetBuff),
                    ("Team Buffs", TeamBuff)
                    },
                    LoadedNonCombatBuffs,
                    LoadedNonCombatBuffs
                ));
            }

            if (LoadedCombatBuffs != null && LoadedCombatBuffs.Any())
            {
                columns.Add((
                    new List<(string, IEnumerable<BuffUiConfig>)>
                    {
                    ("Fighting Target", FightingTarget),
                    ("Combat Self", CombatSelfBuff),
                    ("Combat Target", CombatTargetBuff),
                    ("Combat Team", CombatTeamBuff)
                    },
                    LoadedCombatBuffs,
                    LoadedCombatBuffs
                ));
            }

            var unconfiguredPerks = new List<BuffUiConfig>();

            foreach (var perk in CombatBuffPerks)
            {
                var hash = (PerkHash)perk;

                unconfiguredPerks.Add(new BuffUiConfig
                {
                    BuffIDs = new[] { perk },
                    SettingKey = null,
                    Label = PerkAction.List.FirstOrDefault(x => x.Hash == hash)?.Name ?? hash.ToString(),
                    UiType = UiType.LabelOnly
                });
            }

            _BuffWindow.ShowWindow(
                title: "Buffs",
                xmlPath: pluginDirectory + "\\UI\\BuffWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                columns: columns,
                extraColumn: unconfiguredPerks.Count > 0 ? ("Perks used without options.", unconfiguredPerks, Option.DropDown) : ((string, IEnumerable<BuffUiConfig>, Option)?)null,
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
