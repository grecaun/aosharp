using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class DebuffUiConfig : IUiConfig, IOptionsConfig
{
    public int[] DebuffIDs;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => DebuffIDs ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
    public List<(string Name, int Value)> Options { get; set; }
}

public class DebuffWindowController
{
    private GenericWindowController<DebuffUiConfig> _DebuffWindow;
    public Window CurrentWindow => _DebuffWindow.CurrentWindow;

    #region Lists

    public static readonly List<(string Name, int Value)> DebuffTargetOptions = new List<(string, int)> { ("Off", 0), ("Target", 1), ("Boss", 2) };
    public static readonly List<(string Name, int Value)> KeepRunning = new List<(string Name, int Value)> { ("Off", 0), ("Target", 1), ("Boss", 2), ("Keep running", 4) };
    public static readonly List<(string Name, int Value)> Area = new List<(string Name, int Value)> { ("Off", 0), ("Target", 1), ("Boss", 2), ("Area", 3) };
    public static readonly List<(string Name, int Value)> All = new List<(string Name, int Value)> { ("Off", 0), ("Target", 1), ("Boss", 2), ("Area", 3), ("Keep running", 4) };

    public static List<DebuffUiConfig> Debuffs = new List<DebuffUiConfig>
    {
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.AAODebuffs).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "AAODebuffs", Label = "AAO Debuffs", UiType = UiType.DualDropDown, Options = Area},


        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.BioMetDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "BioMetDebuff", Label = "BioMet Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.DebuffNanoACHeavy).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "DebuffNanoACHeavy", Label = "Debuff Nano AC Heavy", UiType = UiType.DualDropDown, Options= Area},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.EvasionDebuffs).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "EvasionDebuffs", Label = "Evasion Debuffs", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.EvasionDebuffs_Agent).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "EvasionDebuffs_Agent", Label = "Evasion Debuffs Agent", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.General1HandBluntDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "General1HandBluntDebuff", Label = "General 1 Hand Blunt Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.General1HEdgedDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "General1HEdgedDebuff", Label = "General 1H Edged Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.General2HBluntDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "General2HBluntDebuff", Label = "General 2H Blunt Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.General2HEdgedDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "General2HEdgedDebuff", Label = "General 2H Edged Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralAgilityDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralAgilityDebuff", Label = "General Agility Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralAssaultRifleDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralAssaultRifleDebuff", Label = "General Assault Rifle Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralBioMetDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralBioMetDebuff", Label = "General Bio Met Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralBowDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralBowDebuff", Label = "General Bow Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralBowSpecialDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralBowSpecialDebuff", Label = "General Bow Special Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralBrawlDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralBrawlDebuff", Label = "General Brawl Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralBurstDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralBurstDebuff", Label = "General Burst Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralChemicalACDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralChemicalACDebuff", Label = "General Chemical AC Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralColdACDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralColdACDebuff", Label = "General Cold AC Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralDeflectDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralDeflectDebuff", Label = "General Deflect Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralDimachDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralDimachDebuff", Label = "General Dimach Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralEnergyACDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralEnergyACDebuff", Label = "General Energy AC Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralEnergyMeleeDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralEnergyMeleeDebuff", Label = "General Energy Melee Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralFlingShotDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralFlingShotDebuff", Label = "General Fling Shot Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralFullAutoDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralFullAutoDebuff", Label = "General Full Auto Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralGrenadeDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralGrenadeDebuff", Label = "General Grenade Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralIntelligenceDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralIntelligenceDebuff", Label = "General Intelligence Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralKnifeDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralKnifeDebuff", Label = "General Knife Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralLREnergyWeaponDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralLREnergyWeaponDebuff", Label = "General LR Energy Weapon Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralMartialArtsDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralMartialArtsDebuff", Label = "General Martial Arts Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralMatCreaDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralMatCreaDebuff", Label = "General MatCrea Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralMatLocDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralMatLocDebuff", Label = "General MatLoc Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralMatMetDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralMatMetDebuff", Label = "General MatMet Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralMeleeACDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralMeleeACDebuff", Label = "General Melee AC Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralNanoACDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralNanoACDebuff", Label = "General Nano AC Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralPiercingDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralPiercingDebuff", Label = "General Piercing Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralPistoDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralPistoDebuff", Label = "General Pistol Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralPoisonACDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralPoisonACDebuff", Label = "General Poison AC Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralProjectileACDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralProjectileACDebuff", Label = "General Projectile AC Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralPsyModDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralPsyModDebuff", Label = "General PsyMod Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralPsychicDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralPsychicDebuff", Label = "General Psychic Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralRadiationACDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralRadiationACDebuff", Label = "General Radiation AC Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralRiposteDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralRiposteDebuff", Label = "General Riposte Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralRifleDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralRifleDebuff", Label = "General Rifle Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralSenseDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralSenseDebuff", Label = "General Sense Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralSenseImpDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralSenseImpDebuff", Label = "General SenseImp Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralShotgunDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralShotgunDebuff", Label = "General Shotgun Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralSMGDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralSMGDebuff", Label = "General SMG Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralSneakAttackDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralSneakAttackDebuff", Label = "General Sneak Attack Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralStaminaDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralStaminaDebuff", Label = "General Stamina Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.GeneralStrengthDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "GeneralStrengthDebuff", Label = "General Strength Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.HealDeltaDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "HealDeltaDebuff", Label = "Heal Delta Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.HealReactivityMultiplierDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "HealReactivityMultiplierDebuff", Label = "Heal Reactivity Multiplier Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.HopeDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "HopeDebuff", Label = "Hope Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.IntelligenceDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "IntelligenceDebuff", Label = "Intelligence Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.MatCreaDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "MatCreaDebuff", Label = "Mat Crea Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.PsyModDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "PsyModDebuff", Label = "Psy Mod Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.MatLocDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "MatLocDebuff", Label = "Mat Loc Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.MatMetDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "MatMetDebuff", Label = "Mat Met Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.MetaPhysicistDamageDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "MetaPhysicistDamageDebuff", Label = "Meta Physicist Damage Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.MiseryDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "MiseryDebuff", Label = "Misery Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.MPDamageDebuffLineA).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "MPDamageDebuffLineA", Label = "MP Damage Debuff Line A", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.MPDamageDebuffLineB).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "MPDamageDebuffLineB", Label = "MP Damage Debuff Line B", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoDeltaDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "NanoDeltaDebuff", Label = "Nano Delta Debuff", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoDrain_LineA).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "NanoDrain_LineA", Label = "Nano Drain_LineA", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoDrain_LineB).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "NanoDrain_LineB", Label = "Nano Drain_LineB", UiType = UiType.DualDropDown },

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoResistDebuffProc).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "NanoResistDebuffProc", Label = "Nano Resist Debuff Proc", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.NanoShutdownDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "NanoShutdownDebuff", Label = "Nano Shutdown Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.NPCostDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "NPCostDebuff", Label = "NPCost Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.PathofDarknessDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "PathofDarknessDebuff", Label = "Path of Darkness Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.ProximityRangeDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "ProximityRangeDebuff", Label = "Proximity Range Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.PsychicDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "PsychicDebuff", Label = "Psychic Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.RoadToDarknessDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "RoadToDarknessDebuff", Label = "Road To Darkness Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.SenseImpDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "SenseImpDebuff", Label = "Sense Imp Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.ShadeInitDebuffProc).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "ShadeInitDebuffProc", Label = "Shade Init Debuff Proc", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.SilenceDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "SilenceDebuff", Label = "Silence Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.SkillLockModifierDebuff1053).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "SkillLockModifierDebuff1053", Label = "Skill Lock Modifier Debuff 1053", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.SkillLockModifierDebuff847).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "SkillLockModifierDebuff847", Label = "Skill Lock Modifier Debuff 847", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.TraderDebuffACNanos).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "TraderDebuffACNanos", Label = "Trader Debuff AC Nanos", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.TraderShutdownSkillDebuff).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "TraderShutdownSkillDebuff", Label = "Trader Shutdown Skill Debuff", UiType = UiType.DualDropDown, Options = Area},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.HealthandNanoOverTimeDrain).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "HealthandNanoOverTimeDrain", Label = "Health and Nano Over Time Drain", UiType = UiType.DualDropDown, Options = KeepRunning},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.TraderAADDrain).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "TraderAADDrain", Label = "Trader AAD Drain", UiType = UiType.DualDropDown, Options = All},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.TraderAAODrain).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "TraderAAODrain", Label = "Trader AAO Drain", UiType = UiType.DualDropDown,Options = All},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.DamageDrain).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "DamageDrain", Label = "Damage Drain", UiType = UiType.DualDropDown, Options = All},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.TraderACTransferTargetDebuff_Draw).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "TraderACTransferTargetDebuff_Draw", Label = "Trader AC Transfer Target Debuff_Draw", UiType = UiType.DualDropDown, Options = All},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.TraderACTransferTargetDebuff_Siphon).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "TraderACTransferTargetDebuff_Siphon", Label = "Trader AC Transfer Target Debuff_Siphon", UiType = UiType.DualDropDown, Options = All},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.TraderSkillTransferTargetDebuff_Deprive).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "TraderSkillTransferTargetDebuff_Deprive", Label = "Trader Skill Transfer Target Debuff_Deprive", UiType = UiType.DualDropDown, Options = All},
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.TraderSkillTransferTargetDebuff_Ransack).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "TraderSkillTransferTargetDebuff_Ransack", Label = "Trader Skill Transfer Target Debuff_Ransack", UiType = UiType.DualDropDown, Options = All},

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.SLNanopointDrain).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "SLNanopointDrain", Label = "SL Nanopoint Drain", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = SpellID.HaloNanoDebuff, SettingKey = "HaloNanoDebuff", Label = "Halo Nano Debuff", UiType = UiType.DualDropDown},
        new DebuffUiConfig { DebuffIDs = new[]{ SpellID.LickofthePest }, SettingKey = "LickofthePest", Label = "Lick of the Pest", UiType = UiType.DualDropDown},

        new DebuffUiConfig { DebuffIDs = new[] { RelevantGenericItems.BracerofBrotherMalevolence }, SettingKey = "BracerofBrotherMalevolenceSelection", Label = "Bracer of Brother Malevolence", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = RelevantGenericItems.TotwBlindRings, SettingKey = "TotwBlindRingsSelection", Label = "ToTW Blind Rings", UiType = UiType.RadioButtonGroup },

        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Sword }, SettingKey = "SwordSelection", Label = "Sword", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Pen }, SettingKey = "PenSelection", Label = "Pen", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Derivate }, SettingKey = "DerivateSelection", Label = "Derivate", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.BlindedByDelights }, SettingKey = "BlindedByDelightsSelection", Label = "Blinded By Delights", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.FadeArmor }, SettingKey = "FadeArmorSelection", Label = "Fade Armor", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Succumb }, SettingKey = "SuccumbSelection", Label = "Succumb", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.MaliciousProhibition }, SettingKey = "MaliciousProhibitionSelection", Label = "Malicious Prohibition", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Deconstruction }, SettingKey = "DeconstructionSelection", Label = "Deconstruction", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.InstallNotumDepletionDevice }    , SettingKey = "InstallNotumDepletionDeviceSelection", Label = "Install Notum Depletion Device", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.SuppressivePrimer }, SettingKey = "SuppressivePrimerSelection", Label = "Suppressive Primer", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.ThermalPrimer }, SettingKey = "ThermalPrimerSelection", Label = "Thermal Primer", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.MarkOfVengeance }, SettingKey = "MarkOfVengeanceSelection", Label = "Mark Of Vengeance", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.MarkOfTheUnclean }, SettingKey = "MarkOfTheUncleanSelection", Label = "Mark Of The Unclean", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.MarkOfTheUnhallowed }, SettingKey = "MarkOfTheUnhallowedSelection", Label = "Mark Of The Unhallowed", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Dragonfire }, SettingKey = "DragonfireSelection", Label = "Dragonfire", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Incapacitate }, SettingKey = "IncapacitateSelection", Label = "Incapacitate", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.RedDusk }, SettingKey = "RedDuskSelection", Label = "Red Dusk", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.EtherealTouch }, SettingKey = "EtherealTouchSelection", Label = "Ethereal Touch", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.ConvulsiveTremor }, SettingKey = "ConvulsiveTremorSelection", Label = "Convulsive Tremor", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.DoomTouch }, SettingKey = "DoomTouchSelection", Label = "Doom Touch", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Blur }, SettingKey = "BlurSelection", Label = "Blur", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Tracer }, SettingKey = "TracerSelection", Label = "Tracer", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.LaserPaintTarget }, SettingKey = "LaserPaintTargetSelection", Label = "Laser Paint Target", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.TriangulateTarget }, SettingKey = "TriangulateTargetSelection", Label = "Triangulate Target", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.NapalmSpray }, SettingKey = "NapalmSpraySelection", Label = "Napalm Spray", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.ChemicalBlindness }, SettingKey = "ChemicalBlindnessSelection", Label = "Chemical Blindness", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.DazzleWithLights }, SettingKey = "DazzleWithLightsSelection", Label = "Dazzle With Lights", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.HostileTakeover }, SettingKey = "HostileTakeoverSelection", Label = "Hostile Takeover", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.ArmorPiercingShot }, SettingKey = "ArmorPiercingShotSelection", Label = "Armor Piercing Shot", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.StopNotumFlow }, SettingKey = "StopNotumFlowSelection", Label = "Stop Notum Flow", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.NotumOverflow }, SettingKey = "NotumOverflowSelection", Label = "Notum Overflow", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.Tick }, SettingKey = "TickSelection", Label = "Tick", UiType = UiType.RadioButtonGroup },
        new DebuffUiConfig { DebuffIDs = new[] {(int) PerkHash.ZapNano }, SettingKey = "ZapNanoSelection", Label = "Zap Nano", UiType = UiType.RadioButtonGroup }

    };

    public static List<DebuffUiConfig> AOEDebuffs = new List<DebuffUiConfig>
    {
        new DebuffUiConfig { DebuffIDs = SpellID.AOEBlinds, SettingKey = "AOEBlinds", Label = "AOE Blinds", UiType = UiType.DualDropDown, Options = new List<(string, int)> { ("Off", 0), ("On", 1), ("Spam", 2) } },

        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.DemotivationalSpeeches).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "DemotivationalSpeeches", Label = "Demotivational Speeches", UiType = UiType.DualDropDown, Options = new List<(string, int)> { ("Off", 0), ("Combat Only", 1), ("Spam", 2), ("On", 3) } },
        new DebuffUiConfig { DebuffIDs = Spell.GetSpellsForNanoline(NanoLine.EngineerDebuffAuras).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey = "EngineerDebuffAuras", Label = "Engineer Debuff Auras", UiType = UiType.DualDropDown, Options = new List<(string, int)> { ("Off", 0), ("Combat Only", 1), ("Spam", 2), ("On", 3) } },
    };

    #endregion

    public DebuffWindowController(HandlerSettings settings)
    {
        _DebuffWindow = new GenericWindowController<DebuffUiConfig>(settings);
    }

    public void DebuffShowWindow(string pluginDirectory)
    {
        try
        {
            var unconfiguredPerks = new List<DebuffUiConfig>();

            foreach (var perk in CombatBuffPerks)
            {
                var hash = (PerkHash)perk;

                unconfiguredPerks.Add(new DebuffUiConfig
                {
                    DebuffIDs = new[] { perk },
                    SettingKey = null,
                    Label = PerkAction.List.FirstOrDefault(x => x.Hash == hash)?.Name ?? hash.ToString(),
                    UiType = UiType.LabelOnly
                });
            }

            _DebuffWindow.ShowWindow(
                title: "Debuffs",
                xmlPath: pluginDirectory + "\\UI\\GenericWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                sections: new List<(string, IEnumerable<DebuffUiConfig>)>
                {
                ("Debuffs", Debuffs),
                ("AOE Debuffs", AOEDebuffs),
                },
                extraSection: unconfiguredPerks.Count > 0  ? ("Perks used without options.", unconfiguredPerks, Option.DropDown) : ((string, IEnumerable<DebuffUiConfig>, Option)?)null,
                sectionIds: LoadedDeBuffs,
                entryIds: LoadedDeBuffs,
                pluginDirectory: pluginDirectory,
                options: DebuffTargetOptions);
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
