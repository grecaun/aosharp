using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class PetBuffUiConfig : IUiConfig
{
    public int[] SpellIds;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => SpellIds ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
}

public class PetBuffWindowController
{
    private GenericWindowController<PetBuffUiConfig> _PetBuffWindow;
    public Window CurrentWindow => _PetBuffWindow.CurrentWindow;

    #region Lists

    public static List<PetBuffUiConfig> Attack = new List<PetBuffUiConfig>
    {
        new PetBuffUiConfig {SpellIds = SpellID.All_Attack_Pets, SettingKey ="SpawnAttackPet", Label = "Cast Attack Pet", UiType = UiType.DropDownWOption},

        new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeNano }, SettingKey = "AttackPetCompositeNano", Label = "Composite Nano", UiType = UiType.Checkbox},
        new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeMartialProwess }, SettingKey = "AttackPetCompositeMartialProwess", Label = "Composite Martial Prowess", UiType = UiType.Checkbox },
        new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeMelee }, SettingKey = "AttackPetCompositeMelee", Label = "Composite Melee", UiType = UiType.Checkbox },
        new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositePhysicalSpecial }, SettingKey = "AttackPetCompositePhysicalSpecial", Label = "Composite Physical Special", UiType = UiType.Checkbox },

        new PetBuffUiConfig { SpellIds = new[] { (int)PerkHash.KenFi }, SettingKey = "KenFi", Label = "Ken Fi", UiType = UiType.Checkbox },
        new PetBuffUiConfig { SpellIds = new[] { (int)PerkHash.ChannelRage }, SettingKey = "ChannelRage", Label = "Channel Rage", UiType = UiType.Checkbox },

        new PetBuffUiConfig { SpellIds = new[] { (int)PerkHash.Puppeteer }, SettingKey = "Puppeteer", Label = "Puppeteer", UiType = UiType.Checkbox },
        new PetBuffUiConfig { SpellIds = new[] { SpellID.DroidDamageMatrix }, SettingKey = "AttackPetDroidDamageMatrix", Label = "Droid Damage Matrix", UiType = UiType.Checkbox },

        new PetBuffUiConfig { SpellIds = new[] { (int)PerkHash.OptimizeBotProtocol }, SettingKey = "OptimizeBotProtocol", Label = "Optimize Bot Protocol", UiType = UiType.Checkbox },

        new PetBuffUiConfig { SpellIds = SpellID.SiphonBox683, SettingKey = "AttackPetProc", Label = "Offensive Proc", UiType = UiType.Checkbox },

        new PetBuffUiConfig { SpellIds = SpellID.GadgeteerPerkLine, SettingKey = "AttackPetGadgeteer", Label = "Gadgeteer", UiType = UiType.DropDownWOption },
       
        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.ArmorBuff).Select(s => s.Id).ToArray(), SettingKey = "AttackPetArmorBuff", Label = "Armor Buff", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = SpellID.TargetDamageBuffs_LineAAgent, SettingKey = "AttackPetDamageBuffs_LineAAgent", Label = "Damage Buffs Line A Agent", UiType = UiType.DropDownWOption },
        new PetBuffUiConfig { SpellIds = SpellID.TargetDamageBuffs_LineAEngineer, SettingKey = "AttackPetDamageBuffs_LineAEngineer", Label = "Damage Buffs Line A Engineer", UiType = UiType.DropDownWOption },
        //new PetBuffUiConfig { SpellIds = SpellID.TargetDamageBuffs_LineAFixer, SettingKey = "AttackPetDamageBuffs_LineAFixer", Label = "Damage Buffs Line A Fixer", UiType = UiType.DropDownWOption },
        //new PetBuffUiConfig { SpellIds = SpellID.TargetDamageBuffs_LineASoldier, SettingKey = "AttackPetDamageBuffs_LineASoldier", Label = "Damage Buffs Line A Soldier", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.CriticalDecreaseBuff).Select(s => s.Id).ToArray(), SettingKey = "AttackPetCriticalDecreaseBuff", Label = "Critical Decrease Buff", UiType = UiType.DropDownWOption },
        
        new PetBuffUiConfig { SpellIds = SpellID.TargetMajorEvasionBuffsMetaphysicist, SettingKey = "AttackPetAnticipationofRetaliation", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption },
          
        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.PetDamageOverTimeResistNanos).Select(s => s.Id).ToArray(), SettingKey = "AttackPetPetDamageOverTimeResistNanos", Label = "Pet DoT Resist Buffs", UiType = UiType.DropDownWOption },
        
        new PetBuffUiConfig { SpellIds = SpellID.PetDefensiveNanos, SettingKey = "AttackPetPetDefensiveNanos", Label = "Pet Defensive Buffs", UiType = UiType.DropDownWOption },
        
        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.PetTauntBuff).Select(s => s.Id).ToArray(), SettingKey = "AttackPetPetTauntBuff", Label = "Pet Taunt Buffs", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.EngineerMiniaturization).Select(s => s.Id).ToArray(), SettingKey = "AttackPetEngineerMiniaturization", Label = "Engineer Miniaturization", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = SpellID.PetShortTermDamageBuffs, SettingKey = "AttackPetPetShortTermDamageBuffs", Label = "Pet Short Term Damage Buffs", UiType = UiType.DropDownWOption },
       
        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.MPPetInitiativeBuffs).Select(s => s.Id).ToArray(), SettingKey = "AttackPetMPPetInitiativeBuffs", Label = "MP Pet Initiative Buffs", UiType = UiType.DropDownWOption },
        
        new PetBuffUiConfig { SpellIds = SpellID.ShieldOfObedientServant, SettingKey = "AttackPetShieldOfObedientServant", Label = "Shield of the Obedient Servant", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = SpellID.InstillDamageBuffs, SettingKey = "AttackPetInstillDamageBuffs", Label = "Instill Damage Buffs", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.AggressiveConstructEmpowerment).Select(s => s.Id).ToArray(), SettingKey = "AttackPetAggressiveConstructEmpowerment", Label = "Aggressive Construct Empowerment", UiType = UiType.DropDownWOption },
        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.MPAttackPetDamageType).Select(s => s.Id).ToArray(), SettingKey = "AttackPetMPAttackPetDamageType", Label = "MP Attack Pet Damage Type", UiType = UiType.DropDownWOption },
       
        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.PetHealDelta843).Select(s => s.Id).ToArray(), SettingKey = "AttackPetPetHealDelta843", Label = "Pet Heal Delta 843", UiType = UiType.DropDownWOption },
        
        new PetBuffUiConfig { SpellIds = SpellID.MPCompositeNano, SettingKey = "AttackPetMPCompositeNano", Label = "MP Composite Nano", UiType = UiType.DropDownWOption },

    };

    public static List<PetBuffUiConfig> Support = new List<PetBuffUiConfig>
    {
        new PetBuffUiConfig {SpellIds = SpellID.All_Support_Pets, SettingKey ="SpawnSupportPet", Label = "Cast Support Pet", UiType = UiType.DropDownWOption},
       
        new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeNano }, SettingKey = "SupportPetCompositeNano", Label = "Composite Nano", UiType = UiType.Checkbox},
        new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeMartialProwess }, SettingKey = "SupportPetCompositeMartialProwess", Label = "Composite Martial Prowess", UiType = UiType.Checkbox },
        new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeMelee }, SettingKey = "SupportPetCompositeMelee", Label = "Composite Melee", UiType = UiType.Checkbox },
        new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositePhysicalSpecial }, SettingKey = "SupportPetCompositePhysicalSpecial", Label = "Composite Physical Special", UiType = UiType.Checkbox },

        new PetBuffUiConfig { SpellIds = SpellID.GadgeteerPerkLine, SettingKey = "SupportPetGadgeteer", Label = "Gadgeteer", UiType = UiType.DropDownWOption },
        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.ArmorBuff).Select(s => s.Id).ToArray(), SettingKey = "SupportPetArmorBuff", Label = "Armor Buff", UiType = UiType.DropDownWOption },

        //new PetBuffUiConfig { SpellIds = SpellID.TargetDamageBuffs_LineAAgent, SettingKey = "SupportPetDamageBuffs_LineAAgent", Label = "Damage Buffs Line A Agent", UiType = UiType.DropDownWOption },
        new PetBuffUiConfig { SpellIds = SpellID.TargetDamageBuffs_LineAEngineer, SettingKey = "SupportPetDamageBuffs_LineAEngineer", Label = "Damage Buffs Line A Engineer", UiType = UiType.DropDownWOption },
        //new PetBuffUiConfig { SpellIds = SpellID.TargetDamageBuffs_LineAFixer, SettingKey = "SupportPetDamageBuffs_LineAFixer", Label = "Damage Buffs Line A Fixer", UiType = UiType.DropDownWOption },
        //new PetBuffUiConfig { SpellIds = SpellID.TargetDamageBuffs_LineASoldier, SettingKey = "SupportPetDamageBuffs_LineASoldier", Label = "Damage Buffs Line A Soldier", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.CriticalDecreaseBuff).Select(s => s.Id).ToArray(), SettingKey = "SupportPetCriticalDecreaseBuff", Label = "Critical Decrease Buff", UiType = UiType.DropDownWOption },
        
        new PetBuffUiConfig { SpellIds = SpellID.TargetMajorEvasionBuffsMetaphysicist, SettingKey = "SupportPetAnticipationofRetaliation", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.EngineerMiniaturization).Select(s => s.Id).ToArray(), SettingKey = "SupportPetEngineerMiniaturization", Label = "Engineer Miniaturization", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = SpellID.ShieldOfObedientServant, SettingKey = "SupportPetShieldOfObedientServant", Label = "Shield of the Obedient Servant", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.MesmerizationConstructEmpowerment).Select(s => s.Id).ToArray(), SettingKey = "MesmerizationConstructEmpowerment", Label = "Mesmerization Construct Empowerment", UiType = UiType.DropDownWOption },
        
        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.PetHealDelta843).Select(s => s.Id).ToArray(), SettingKey = "SupportPetPetHealDelta843", Label = "Pet Heal Delta 843", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.NPCostBuff).Select(s => s.Id).ToArray(), SettingKey = "SupportPetCostBuffs", Label = "Cost Buffs", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = SpellID.MPCompositeNano, SettingKey = "SupportPetMPCompositeNano", Label = "MP Composite Nano", UiType = UiType.DropDownWOption },

        new PetBuffUiConfig { SpellIds = SpellID.SenImpBuffs, SettingKey = "SupportPetMPSenImpBuffs", Label = "Sensory improvement Nano", UiType = UiType.DropDownWOption },
        new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.PsyModBuff).Select(s => s.Id).ToArray(), SettingKey = "SupportPetMPPsyModBuff", Label = "Psychological modifications Nano", UiType = UiType.DropDownWOption },

    };

    public static List<PetBuffUiConfig> Heal = new List<PetBuffUiConfig>
    {
       new PetBuffUiConfig {SpellIds = Spell.GetSpellsForNanoline(NanoLine.HealPets).Select(s => s.Id).ToArray(), SettingKey ="SpawnHealPet", Label = "Cast Heal Pet", UiType = UiType.DropDownWOption},

       new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeNano }, SettingKey = "HealPetCompositeNano", Label = "Composite Nano", UiType = UiType.Checkbox},
       new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeMartialProwess }, SettingKey = "HealPetCompositeMartialProwess", Label = "Composite Martial Prowess", UiType = UiType.Checkbox },

       new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.ArmorBuff).Select(s => s.Id).ToArray(), SettingKey = "HealPetArmorBuff", Label = "Armor Buff", UiType = UiType.DropDownWOption },
       
       new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.CriticalDecreaseBuff).Select(s => s.Id).ToArray(), SettingKey = "HealPetCriticalDecreaseBuff", Label = "Critical Decrease Buff", UiType = UiType.DropDownWOption },
       
       new PetBuffUiConfig { SpellIds = SpellID.TargetMajorEvasionBuffsMetaphysicist, SettingKey = "HealPetAnticipationofRetaliation", Label = "Major Evasion Buffs", UiType = UiType.DropDownWOption },

       new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.HealingConstructEmpowerment).Select(s => s.Id).ToArray(), SettingKey = "HealingConstructEmpowerment", Label = "Healing Construct Empowerment", UiType = UiType.DropDownWOption },
       new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.PetDamageOverTimeResistNanos).Select(s => s.Id).ToArray(), SettingKey = "HealPetPetDamageOverTimeResistNanos", Label = "Pet DoT Resist Buffs", UiType = UiType.DropDownWOption },
       
       new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.PetHealDelta843).Select(s => s.Id).ToArray(), SettingKey = "HealPetPetHealDelta843", Label = "Pet Heal Delta 843", UiType = UiType.DropDownWOption },

       new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.NPCostBuff).Select(s => s.Id).ToArray(), SettingKey = "HealPetCostBuffs", Label = "Cost Buffs", UiType = UiType.DropDownWOption },

       new PetBuffUiConfig { SpellIds = SpellID.MPCompositeNano, SettingKey = "HealPetMPCompositeNano", Label = "MP Composite Nano", UiType = UiType.DropDownWOption },

       new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.BioMetBuff).Select(s => s.Id).ToArray(), SettingKey = "HealPetMPBioMetBuff", Label = "Biological metamorphosis Nano", UiType = UiType.DropDownWOption },
       new PetBuffUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.MatMetBuff).Select(s => s.Id).ToArray(), SettingKey = "HealPetMPMatMetBuff", Label = "Matter metamorphosis Nano", UiType = UiType.DropDownWOption },

    };

    #endregion

    public PetBuffWindowController(HandlerSettings settings)
    {
        _PetBuffWindow = new GenericWindowController<PetBuffUiConfig>(settings);
    }

    public void PetBuffShowWindow(string pluginDirectory)
    {
        try
        {
            var columns = new List<(List<(string, IEnumerable<PetBuffUiConfig>)>, HashSet<int>, HashSet<int>)>();

            if (LoadedPetSpells[0])
            {
                columns.Add((
                    new List<(string, IEnumerable<PetBuffUiConfig>)>
                    { ("Attack", Attack
                    .OrderBy(c =>
                        c.SettingKey == "SpawnAttackPet" ? 0 :
                        c.UiType == UiType.Checkbox ? 1 :
                        c.UiType == UiType.DropDownWOption ? 2 :
                        3)
                    .ThenBy(c => c.Label)) },
                    LoadedPetBuffSpells,
                    LoadedPetBuffSpells
                ));
            }

            if (LoadedPetSpells[1])
            {
                columns.Add((
                    new List<(string, IEnumerable<PetBuffUiConfig>)>
                    { ("Support", Support
                    .OrderBy(c =>
                        c.SettingKey == "SpawnSupportPet" ? 0 :
                        c.UiType == UiType.Checkbox ? 1 :
                        c.UiType == UiType.DropDownWOption ? 2 :
                        3)
                    .ThenBy(c => c.Label)) },
                    LoadedPetBuffSpells,
                    LoadedPetBuffSpells
                ));
            }

            if (LoadedPetSpells[2])
            {
                columns.Add((
                    new List<(string, IEnumerable<PetBuffUiConfig>)>
                    { ("Heal", Heal
                    .OrderBy(c =>
                        c.SettingKey == "SpawnHealPet" ? 0 :
                        c.UiType == UiType.Checkbox ? 1 :
                        c.UiType == UiType.DropDownWOption ? 2 :
                        3)
                    .ThenBy(c => c.Label)) },
                    LoadedPetBuffSpells,
                    LoadedPetBuffSpells
                ));
            }

            _PetBuffWindow.ShowWindow(
                title: "Pets",
                xmlPath: pluginDirectory + "\\UI\\PetWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                columns: columns,
                extraColumn: ("/petstats for pet stats", null, Option.Button),
                pluginDirectory: pluginDirectory,
                options: null
            );
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
