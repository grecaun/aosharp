using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class HoldUiConfig : IUiConfig, IOptionsConfig
{
    public int[] HoldsIDs;
    public string SettingKey;
    public string Label;
    public UiType UiType;
    public List<(string Name, int Value)> Options { get; set; } = new List<(string, int)>();

    public int[] GetIds() => HoldsIDs ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
}

public class HoldWindowController
{
    private readonly GenericWindowController<HoldUiConfig> _HoldWindow;
    public Window CurrentWindow => _HoldWindow.CurrentWindow;

    #region Lists

    public static readonly List<(string, int)> HoldTargetOptions = new List<(string, int)> { ("Off", 0), ("Adds", 1), ("All", 2) };
    

    public static List<HoldUiConfig> Holds = new List<HoldUiConfig>
    {
        new HoldUiConfig { HoldsIDs = Spell.GetSpellsForNanoline(NanoLine.SupportPets).OrderByStackingOrder().Select(s => s.Id).ToArray(), SettingKey ="PetMezzing", Label ="Mezz Pet Options", UiType = UiType.DropDown, Options = new List<(string Name, int Value)>{ ("Off", 0), ("Target", 1), ("Adds", 2) } },

        new HoldUiConfig{ HoldsIDs = SpellID.AdventurerMezzTarget, SettingKey = "AdventurerMezzTarget", Label = "Adventurer Mezz Target", UiType = UiType.DualDropDown },

        new HoldUiConfig{ HoldsIDs = SpellID.AgentMezzTarget, SettingKey = "AgentMezzTarget", Label = "Agent Mezz Target", UiType = UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.AgentRootTarget, SettingKey = "AgentRootTarget", Label = "Agent Root Target", UiType = UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.AgentSnareTarget, SettingKey = "AgentSnareTarget", Label = "Agent Snare Target", UiType = UiType.DualDropDown },

        new HoldUiConfig{ HoldsIDs = SpellID.BureaucratMezzTarget, SettingKey = "BureaucratMezzTarget", Label = "Bureaucrat Mezz Target", UiType = UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.BureaucratMezzStunTarget, SettingKey = "BureaucratMezzStunTarget", Label = "Bureaucrat Mezz Stun Target", UiType = UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.BureaucratSnareTarget, SettingKey = "BureaucratSnareTarget", Label = "Bureaucrat Snare Target", UiType = UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.BureaucratRootTarget, SettingKey = "BureaucratRootTarget", Label = "Bureaucrat Root Target", UiType = UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.LastMinNegotiations, SettingKey ="LastMinNegotiations", Label = "Last Min Negotiations", UiType=UiType.DualDropDown },

        new HoldUiConfig{ HoldsIDs = SpellID.FixerSnareTarget, SettingKey ="FixerSnareTarget", Label = "Fixer Snare Target", UiType=UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.FixerRootTarget, SettingKey ="FixerRootTarget", Label = "Fixer Root Target", UiType=UiType.DualDropDown },

        new HoldUiConfig{ HoldsIDs = SpellID.MetaPhysicistMezzTarget, SettingKey ="MetaPhysicistMezzTarget", Label = "Meta Physicist Mezz Target", UiType=UiType.DualDropDown },

        new HoldUiConfig{ HoldsIDs = SpellID.NanoTechnicianMezzStunTarget, SettingKey ="NanoTechnicianMezzStunTarget", Label = "Nano Technician Mezz Stun Target", UiType=UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.NanoTechnicianMezzCalmTarget, SettingKey ="NanoTechnicianMezzCalmTarget", Label = "Nano Technician Mezz Calm Target", UiType=UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.NanoTechnicianMezzHackedBlindTarget, SettingKey ="NanoTechnicianMezzHackedBlindTarget", Label = "Nano Technician Mezz Hacked Blind Target", UiType=UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.NanoTechnicianRootTarget, SettingKey ="NanoTechnicianRootTarget", Label = "Nano Technician Root Target", UiType=UiType.DualDropDown },

        new HoldUiConfig{ HoldsIDs = SpellID.TraderMezzTarget, SettingKey ="TraderMezzTarget", Label = "Trader Mezz Target", UiType=UiType.DualDropDown },
        new HoldUiConfig{ HoldsIDs = SpellID.TraderRootTarget, SettingKey ="TraderRootTarget", Label = "Trader Root Target", UiType=UiType.DualDropDown },

        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.TheShot }, SettingKey = "PerkTheShot", Label = "The Shot", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.Assassinate}, SettingKey = "PerkAssassinate", Label = "Assassinate", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.ConcussiveShot}, SettingKey = "PerkConcussiveShot", Label = "Concussive Shot", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.SoftenUp}, SettingKey = "PerkSoftenUp", Label = "Soften Up", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.ConfoundWithRules}, SettingKey = "PerkConfoundWithRules", Label = "Confound With Rules", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.GroinKick}, SettingKey = "PerkGroinKick", Label = "Groin Kick", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.Disorientate}, SettingKey = "PerkDisorientate", Label = "Disorientate", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.CrushBone}, SettingKey = "PerkCrushBone", Label = "Crush Bone", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.BringThePain}, SettingKey = "PerkBringThePain", Label = "Bring The Pain", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.BlindsideBlow}, SettingKey = "PerkBlindsideBlow", Label = "Blindside Blow", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.FullFrontal}, SettingKey = "PerkFullFrontal", Label = "Full Frontal", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.Guesstimate}, SettingKey = "PerkGuesstimate", Label = "Guesstimate", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.QuarkContainmentField}, SettingKey = "PerkQuarkContainmentField", Label = "Quark Containment Field", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.JarringBurst}, SettingKey = "PerkJarringBurst", Label = "Jarring Burst", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.LegShot}, SettingKey = "PerkLegShot", Label = "Leg Shot", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.NanoShakes}, SettingKey = "PerkNanoShakes", Label = "Nano Shakes", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.Grasp}, SettingKey = "PerkGrasp", Label = "Grasp", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.Bearhug}, SettingKey = "PerkBearhug", Label = "Bearhug", UiType = UiType.Checkbox },
        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.GripOfColossus}, SettingKey = "PerkGripOfColossus", Label = "Grip Of Colossus", UiType = UiType.Checkbox },

    };

    public static List<HoldUiConfig> AOEHolds = new List<HoldUiConfig>
    {
        new HoldUiConfig{HoldsIDs = SpellID.AgentSnareArea, SettingKey = "AgentSnareArea", Label = "Agent Snare Area", UiType = UiType.DualDropDown},

        new HoldUiConfig{HoldsIDs = SpellID.BureaucratMezzArea, SettingKey = "BureaucratMezzArea", Label = "Bureaucrat Mezz Area", UiType = UiType.DualDropDown},
        new HoldUiConfig{HoldsIDs = SpellID.BureaucratSnareArea, SettingKey = "BureaucratSnareArea", Label = "Bureaucrat Snare Area", UiType = UiType.DualDropDown},
        new HoldUiConfig{HoldsIDs = SpellID.BureaucratRootArea, SettingKey = "BureaucratRootArea", Label = "Bureaucrat Root Area", UiType = UiType.DualDropDown},

        new HoldUiConfig{HoldsIDs = Spell.GetSpellsForNanoline(NanoLine.EngineerPetAOESnareBuff).Select(s=>s.Id).ToArray(), SettingKey = "EngineerPetAOESnareBuff", Label ="Engineer Pet AOE Snare Buff", UiType = UiType.DualDropDown, Options = new List<(string, int)> { ("Off", 0), ("Combat Only", 1), ("Spam", 2), ("On", 3) }},

        new HoldUiConfig{HoldsIDs = SpellID.FixerSnareArea, SettingKey = "FixerSnareArea", Label = "Fixer Snare Area", UiType = UiType.DualDropDown},

        new HoldUiConfig{HoldsIDs = SpellID.TraderRootArea, SettingKey = "TraderRootArea", Label = "Trader Root Area", UiType = UiType.DualDropDown},

        new HoldUiConfig{ HoldsIDs = new []{(int)PerkHash.Stoneworks}, SettingKey = "PerkStoneworks", Label = "Stoneworks", UiType = UiType.Checkbox }

    };

    #endregion

    public HoldWindowController(HandlerSettings settings)
    {
        _HoldWindow = new GenericWindowController<HoldUiConfig>(settings);
    }

    public void HoldShowWindow(string pluginDirectory)
    {
        try
        {
            _HoldWindow.ShowWindow(
                title: "Holds",
                xmlPath: pluginDirectory + "\\UI\\GenericWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                sections: new List<(string, IEnumerable<HoldUiConfig>)>
                {
                ("Holds", Holds),
                ("AOE Holds", AOEHolds),
                },
                 extraSection: (null),
                sectionIds: LoadedHolds,
                entryIds: LoadedHolds,
                pluginDirectory: pluginDirectory,
                options: HoldTargetOptions);
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
