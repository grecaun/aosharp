using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class NukeUiConfig : IUiConfig
{
    public int[] SpellIds;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => SpellIds ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;

    public List<(string Name, int Value)> Options { get; set; }
}

public class NukeWindowController
{
    private GenericWindowController<NukeUiConfig> _NukeWindow;
    public Window CurrentWindow => _NukeWindow.CurrentWindow;

    #region Lists

    public static readonly List<(string Name, int Value)> NukeTargetOptions = new List<(string, int)> { ("None", 0), ("Target", 1), ("Boss", 2), };

    public static List<NukeUiConfig> Dots = new List<NukeUiConfig>
    {
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.DOT_LineA).Select(s => s.Id).ToArray(), SettingKey = "DOT_LineA", Label = "DOT Line A", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.DOT_LineB).Select(s => s.Id).ToArray(), SettingKey = "DOT_LineB", Label = "DOT Line B", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.DOTNanotechnicianStrainA).Select(s => s.Id).ToArray(), SettingKey = "DOTNanotechnicianStrainA", Label = "DOT Nanotechnician Strain A", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.DOTAgentStrainA).Select(s => s.Id).ToArray(), SettingKey = "DOTAgentStrainA", Label = "DOT Agent Strain A", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.DOTStrainC).Select(s => s.Id).ToArray(), SettingKey = "DOTStrainC", Label = "DOT Strain C", UiType = UiType.ThreeDropDown }
    };

    public static List<NukeUiConfig> Nukes = new List<NukeUiConfig>
    {
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.Nukes).Select(s => s.Id).ToArray(), SettingKey = "Nukes", Label = "Nukes", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.AlphaNukes).Select(s => s.Id).ToArray(), SettingKey = "AlphaNukes", Label = "Alpha Nukes", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = SpellID.DoctorCombatNukes, SettingKey = "Nuke", Label = "Nuke", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.AlphaNuke).Select(s => s.Id).ToArray(), SettingKey = "AlphaNuke", Label = "Alpha Nuke", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.OmegaNuke).Select(s => s.Id).ToArray(), SettingKey = "OmegaNuke", Label = "Omega Nuke", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.SpecialEffectNukes).Select(s => s.Id).ToArray(), SettingKey = "SpecialEffectNukes", Label = "Special Effect Nukes", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = SpellID.NTNukes, SettingKey = "NTNukesA", Label = "NT Nukes A", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.DOTNanotechnicianStrainB).Select(s => s.Id).ToArray(), SettingKey = "DOTNanotechnicianStrainB", Label = "DOT Nanotechnician Strain B", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = SpellID.NTNukes, SettingKey = "NTNukesB", Label = "NT Nukes B", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = SpellID.CratNormalNuke, SettingKey ="NormalNuke", Label ="Normal Nuke", UiType = UiType.ThreeDropDown},
        new NukeUiConfig { SpellIds = SpellID.CratSpecialNuke, SettingKey ="SpecialNuke", Label ="Special Nuke", UiType = UiType.ThreeDropDown},
        new NukeUiConfig { SpellIds = SpellID.MPNormalNuke, SettingKey ="MPNormalNuke", Label ="Normal Nuke", UiType = UiType.ThreeDropDown},
        new NukeUiConfig { SpellIds = SpellID.MPMindDamage, SettingKey ="MindDamage", Label ="Mind Damage", UiType = UiType.ThreeDropDown},

        new NukeUiConfig { SpellIds = RelevantGenericItems.SpecialArrows, SettingKey = "SpecialArrows", Label = "Special Arrows", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
        new NukeUiConfig { SpellIds = RelevantGenericItems.SharpObjects, SettingKey = "SharpObjects", Label = "Sharp Objects", UiType = UiType.DualDropDown, Options = NukeTargetOptions  },

        new NukeUiConfig { SpellIds = RelevantGenericItems.RingOfFleshes, SettingKey = "RingOfFleshes", Label = "Ring of Fleshes(Locks Bio Met)", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
        new NukeUiConfig { SpellIds = RelevantGenericItems.WenWen, SettingKey = "WenWen", Label = "WenWen", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
        new NukeUiConfig { SpellIds = RelevantGenericItems.Manta, SettingKey = "Manta", Label = "Manta", UiType = UiType.DualDropDown, Options = NukeTargetOptions },

        new NukeUiConfig { SpellIds = RelevantGenericItems.ToTWBloodRings, SettingKey = "ToTWBloodRings", Label = "ToTW Blood Rings(Locks body dev)", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
        new NukeUiConfig { SpellIds = RelevantGenericItems.ToTWFlameRings, SettingKey = "ToTWFlameRings", Label = "ToTW Flame Rings(Locks Mat Crea)", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
        new NukeUiConfig { SpellIds = RelevantGenericItems.ICCDrone, SettingKey = "ICCDrone", Label = "ICC Drone(Locks Mat Crea)", UiType = UiType.DualDropDown, Options = NukeTargetOptions },

        new NukeUiConfig { SpellIds = RelevantGenericItems.HomingPermorphaBullets, SettingKey = "HomingPermorphaBullets", Label = "Homing Permorpha Bullets", UiType = UiType.DualDropDown, Options = NukeTargetOptions },

        new NukeUiConfig { SpellIds = new[]{RelevantGenericItems.RingofSisterMerciless}, SettingKey = "RingofSisterMercilessSelection", Label = "Ring of Sister Merciless", UiType = UiType.RadioButtonGroup },
        new NukeUiConfig { SpellIds = new []{RelevantGenericItems.RingofSisterPestilence }, SettingKey = "RingofSisterPestilenceSelection", Label = "Ring of Sister Pestilence", UiType = UiType.RadioButtonGroup },
    };

    public static List<NukeUiConfig> AOENukes = new List<NukeUiConfig>
    {
        new NukeUiConfig { SpellIds = SpellID.NTSLAOENukes, SettingKey = "NTSLAOENukes", Label = "NT SL AOE Nukes", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = SpellID.NTRKAOENukes, SettingKey = "NTRKAOENukes", Label = "NT RK AOE Nukes", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.NTAreaNukes).Select(s => s.Id).ToArray(), SettingKey = "NTAreaNukes", Label = "NT Area Nukes", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.NTAreaNukes2).Select(s => s.Id).ToArray(), SettingKey = "NTAreaNukes2", Label = "NT Area Nukes 2", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.Nukes).Select(s => s.Id).ToArray(), SettingKey = "Nukes", Label = "Nukes", UiType = UiType.ThreeDropDown },
        new NukeUiConfig { SpellIds = Spell.GetSpellsForNanoline(NanoLine.AOENuke).Select(s => s.Id).ToArray(), SettingKey = "AOENuke", Label = "AOE Nuke", UiType = UiType.ThreeDropDown },

        new NukeUiConfig { SpellIds = RelevantGenericItems.ThrowingGrenades, SettingKey = "ThrowingGrenades", Label = "Throwing Grenades", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
        new NukeUiConfig { SpellIds = RelevantGenericItems.MantaAOE, SettingKey = "MantaAOE", Label = "Manta AOE", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
        new NukeUiConfig { SpellIds = RelevantGenericItems.SuppressiveBurstItem, SettingKey = "SuppressiveBurstItem", Label = "Suppressive Burst Item", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
        new NukeUiConfig { SpellIds = RelevantGenericItems.ClusterBullets, SettingKey = "ClusterBullets", Label = "Cluster Bullets", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
        new NukeUiConfig { SpellIds = RelevantGenericItems.AOESpecialArrows, SettingKey = "AOESpecialArrows", Label = "Special Arrows", UiType = UiType.DualDropDown, Options = NukeTargetOptions },
    };

    #endregion

    public NukeWindowController(HandlerSettings settings)
    {
        _NukeWindow = new GenericWindowController<NukeUiConfig>(settings);
    }

    public void NukeShowWindow(string pluginDirectory)
    {
        try
        {
            var unconfiguredPerks = new List<NukeUiConfig>();

            foreach (var perk in DamagePerks)
            {
                var hash = (PerkHash)perk;

                unconfiguredPerks.Add(new NukeUiConfig
                {
                    SpellIds = new[] { perk },
                    SettingKey = null,
                    Label = PerkAction.List.FirstOrDefault(x => x.Hash == hash)?.Name ?? hash.ToString(),
                    UiType = UiType.LabelOnly
                });
            }

            _NukeWindow.ShowWindow(
                title: "Nukes",
                xmlPath: pluginDirectory + "\\UI\\GenericWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                sections: new List<(string, IEnumerable<NukeUiConfig>)>
                {
                ("Dots", Dots),
                ("Nukes", Nukes),
                ("AOE Nukes", AOENukes),
                },
                extraSection: unconfiguredPerks.Count > 0  ? ("Perks used without options.", unconfiguredPerks, Option.DropDown) : ((string, IEnumerable<NukeUiConfig>, Option)?)null,
                sectionIds: LoadedNukeSpells,
                entryIds: LoadedNukeSpells,
                pluginDirectory: pluginDirectory,
                options: NukeTargetOptions);
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
