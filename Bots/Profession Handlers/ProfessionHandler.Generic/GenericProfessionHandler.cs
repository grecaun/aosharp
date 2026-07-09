using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Common.SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using AOSharp.Common.Unmanaged.Imports;
using AOSharp.Common.Unmanaged.Interfaces;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using Newtonsoft.Json.Linq;
using Shared;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.ChatMessages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using static System.Net.Mime.MediaTypeNames;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler : Combat.ProfessionHandler
    {
        public Dictionary<LocalGroupMessageType, int> ChatChannels = new Dictionary<LocalGroupMessageType, int>();
        public static int ChatChannelId;
        public static byte ChatChannelType;
        public int AMSPrintStop;

        public static Window _mainWindow;
        public static Window _infoWindow;
        public static Window _petCommandWindow;
        public static Window _morphWindow;
        public static Window _procWindow;
        public static Window _weaponWindow;
        public static Window _falseProfWindow;

        public static List<string> ErrorMessages = new List<string>();

        public static Dictionary<Identity, int> RemainingNCU = new Dictionary<Identity, int>();

        public int EvadeCycleTimeoutSeconds = 180;

        private static readonly long _freq = Stopwatch.Frequency;
        private static readonly long _start = Stopwatch.GetTimestamp();
        public static double Now => (Stopwatch.GetTimestamp() - _start) / (double)_freq;

        public static double _lastZonedTime = 0.0;
        protected double _lastCombatTime = 0.0;

        private double petAttackDelay = 0.0;

        public List<HandlerSettings> settingsToSave = new List<HandlerSettings>();

        public static string StimTargetName = string.Empty;

        public double CycleXpPerks = 0;

        private double FightTimeStamp = 0;

        public static List<int> AdvyMorphs = new List<int> { 217670, 25994, 263278, 82834, 275005, 85062, 217680, 85070 };

        public AttackInfoMessage lastAttackInfoMessage;

        protected static string PluginDir;

        public static HandlerSettings _settings;

        #region targets to not debuff

        protected static HashSet<string> debuffAreaTargetsToIgnore = new HashSet<string>
        {
                    "Dogmatic Pestilence",
                    //"Slayerdroid XXIV Turbo",
                    "Technological Officer Darwelsi",
                    "Immortal Guardian",
                    "Mature Abyss Orchid",
                    "Abyss Orchid Sprout",
                    "Tower of Astodan",
                    "Unicorn Commander Labbe",
                    "Calan-Cur",
                    "Spirit of Judgement",
                    "Wandering Spirit",
                    "Altar of Torture",
                    "Altar of Purification",
                    "Unicorn Coordinator Magnum Blaine",
                    "Watchful Spirit",
                    "Amesha Vizaresh",
                    "Guardian Spirit of Purification",
                    "Tibor 'Rocketman' Nagy",
                    "One Who Obeys Precepts",
                    "The Retainer Of Ergo",
                    "Green Tower",
                    "Blue Tower",
                    "Alien Cocoon",
                    "Alien Coccoon",
                    "Outzone Supplier",
                    "Hollow Island Weed",
                    "Sheila Marlene",
                    "Unicorn Advance Sentry",
                    "Unicorn Technician",
                    "Basic Tools Merchant",
                    "Container Supplier",
                    "Basic Quality Pharmacist",
                    "Basic Quality Armorer",
                    "Basic Quality Weaponsdealer",
                    "Tailor",
                    "Unicorn Commander Rufus",
                    "Ergo, Inferno Guardian of Shadows",
                    "Unicorn Trooper",
                    "Unicorn Squadleader",
                    "Rookie Alien Hunter",
                    "Unicorn Service Tower Alpha",
                    "Unicorn Service Tower Delta",
                    "Unicorn Service Tower Gamma",
                    "Sean Powell",
                    "Xan Spirit",
                    "Unicorn Guard",
                    "Essence Fragment",
                    "Scalding Flames",
                    "Guide",
                    "Guard",
                    "Awakened Xan",
                    "Fanatic",
                    "Peacekeeper Coursey",
                    "Harbinger of Pestilence",
                    "Pandemonium Idol",
                    "Laser Drone",
                    "Heatbeam",
                    "Thermal Detonator",
                    "Unstable Sentry Drone",
                    "Stasis Containment Field",
                    "Assault Drone",
                    "Scalding Flame",
                    "Automated Defense System",
                    "Medical Drone",
                    "Ju-Ju Doll",
                    "Temporal Vortex",
                    "Gateway to the Past",
                    "Gateway to the Present",
                    "Gateway to the Future",
                    "Masked Eleet",
                    "Dust Brigade Security Drone",
                    "Nanovoider",
                    "Punishment",
                    "Flaming Chaos",
                    "Flaming Punishment",
                    "Flaming Vengeance",
                    "Otacustes",
                    "Alien Heavy Patroller",
        };

        #endregion

        public static IPCChannel IPCChannel;

        public static string[] parm;

        public Dictionary<int, PetCommand> CurrentPetCommand = new Dictionary<int, PetCommand>();

        public Identity HealTarget = Identity.None;
        public Identity CurrentHealTarget = Identity.None;
        public Identity MezzTarget = Identity.None;
        public Identity CurrentMezzTarget = Identity.None;

        private List<(Identity Identity, string Name, int MaxNCU)> Pets = new List<(Identity, string, int)>();

        public List<Identity> MezzTargets = new List<Identity>();

        AttackState state;

        private double ReloadDelay;

        public bool Attacked = false;

        public int petColor;
        public string SyncPetsString;

        public int[] MorhpSpellArray;

        public ProcsWindowController ProcWindow = new ProcsWindowController(_settings);
        public PerksWindowController PerkWindow = new PerksWindowController(_settings);
        public ItemsWindowController ItemWindow = new ItemsWindowController(_settings);
        public MAAttacksWindowController MAAttackWindow = new MAAttacksWindowController(_settings);
        public NukeWindowController NukeWindow = new NukeWindowController(_settings);
        public PetBuffWindowController PetWindow = new PetBuffWindowController(_settings);
        public static TrimmersWindowController TrimmerWindow = new TrimmersWindowController(_settings);
        public HoldWindowController HoldWindow = new HoldWindowController(_settings);
        public DebuffWindowController DebuffWindow = new DebuffWindowController(_settings);
        public HealWindowController HealWindow = new HealWindowController(_settings);
        public TauntWindowController TauntWindow = new TauntWindowController(_settings);
        public BuffWindowController BuffWindow = new BuffWindowController(_settings);

        public void Proc_Button_Click(object s, ButtonBase button) { ProcWindow.ShowProcsWindow(PluginDir); }
        public void Perk_Button_Click(object s, ButtonBase button) { PerkWindow.ShowPerksWindow(PluginDir); }
        public void Item_Button_Click(object s, ButtonBase button) { ItemWindow.ShowItemsWindow(PluginDir); }
        public void MA_Attack_Buttion_Clicked(object s, ButtonBase button) { MAAttackWindow.MAAttackShowWindow(PluginDir); }
        public void Nuke_Buttion_Clicked(object s, ButtonBase button) { NukeWindow.NukeShowWindow(PluginDir); }
        public void Pet_Button_Clicked(object s, ButtonBase button) { PetWindow.PetBuffShowWindow(PluginDir); }
        public static void Trimmer_Button_Clicked(object s, ButtonBase button) { TrimmerWindow.TrimmerShowWindow(PluginDir); }
        public void Debuff_Button_Clicked(object s, ButtonBase button) { DebuffWindow.DebuffShowWindow(PluginDir); }
        public void Calms_Button_Clicked(object s, ButtonBase button) { HoldWindow.HoldShowWindow(PluginDir); }
        public void Heals_Button_Clicked(object s, ButtonBase button) { HealWindow.HealShowWindow(PluginDir); }
        public void Taunts_Butoon_Clicked(object s, ButtonBase button) { TauntWindow.TauntShowWindow(PluginDir); }
        public void Buffs_Button_Clicked(object s, ButtonBase button) { BuffWindow.BuffShowWindow(PluginDir); }

        public List<(string Label, EventHandler<ButtonBase> Handler)> _buttonDefinitions = new List<(string, EventHandler<ButtonBase>)>();

        public List<(string Label, EventHandler<ButtonBase> Handler)> _FPDefinitions = new List<(string, EventHandler<ButtonBase>)>();

        public FPBuffWindowController AdventurerBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController BureaucratBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController DoctorBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController EnforcerBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController EngineerBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController FixerBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController MartialArtistBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController MetaphysicistBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController NanoTechnicianBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController SoldierBuffWindow = new FPBuffWindowController(_settings);
        public FPBuffWindowController TraderBuffWindow = new FPBuffWindowController(_settings);

        public void AdventurerBuffs_Button_Clicked(object s, ButtonBase button) { AdventurerBuffWindow.FPBuffShowWindow(PluginDir, "Adventurer", LoadedNonCombatAdventurerBuffs, LoadedCombatAdventurerBuffs); }
        public void BureaucratBuffs_Button_Clicked(object s, ButtonBase button) { BureaucratBuffWindow.FPBuffShowWindow(PluginDir, "Bureaucrat", LoadedNonCombatBureaucratBuffs, LoadedCombatBureaucratBuffs); }
        public void DoctorBuffs_Button_Clicked(object s, ButtonBase button) { DoctorBuffWindow.FPBuffShowWindow(PluginDir, "Doctor", LoadedNonCombatDoctorBuffs, LoadedCombatDoctorBuffs); }
        public void EnforcerBuffs_Button_Clicked(object s, ButtonBase button) { EnforcerBuffWindow.FPBuffShowWindow(PluginDir, "Enforcer", LoadedNonCombatEnforcerBuffs, LoadedCombatEnforcerBuffs); }
        public void EngineerBuffs_Button_Clicked(object s, ButtonBase button) { EngineerBuffWindow.FPBuffShowWindow(PluginDir, "Engineer", LoadedNonCombatEngineerBuffs, LoadedCombatEngineerBuffs); }
        public void FixerBuffs_Button_Clicked(object s, ButtonBase button) { FixerBuffWindow.FPBuffShowWindow(PluginDir, "Fixer", LoadedNonCombatFixerBuffs, LoadedCombatFixerBuffs); }
        public void MartialArtistBuffs_Button_Clicked(object s, ButtonBase button) { MartialArtistBuffWindow.FPBuffShowWindow(PluginDir, "MartialArtist", LoadedNonCombatMartialArtistBuffs, LoadedCombatMartialArtistBuffs); }
        public void MetaphysicistBuffs_Button_Clicked(object s, ButtonBase button) { MetaphysicistBuffWindow.FPBuffShowWindow(PluginDir, "Metaphysicist", LoadedNonCombatMetaphysicistBuffs, LoadedCombatMetaphysicistBuffs); }
        public void NanoTechnicianBuffs_Button_Clicked(object s, ButtonBase button) { NanoTechnicianBuffWindow.FPBuffShowWindow(PluginDir, "NanoTechnician", LoadedNonCombatNanoTechnicianBuffs, LoadedCombatNanoTechnicianBuffs); }
        public void SoldierBuffs_Button_Clicked(object s, ButtonBase button) { SoldierBuffWindow.FPBuffShowWindow(PluginDir, "Soldier", LoadedNonCombatSoldierBuffs, LoadedCombatSoldierBuffs); }
        public void TraderBuffs_Button_Clicked(object s, ButtonBase button) { TraderBuffWindow.FPBuffShowWindow(PluginDir, "Trader", LoadedNonCombatTraderBuffs, LoadedCombatTraderBuffs); }

        public CharacterWieldedWeapon _wieldedWeapons;

        public static GenericProfessionHandler Handler { get; private set; }

        JObject bossJson;
        List<string> BossNames = new List<string>();

        public GenericProfessionHandler(string pluginDir)
        {
            try
            {
                Handler = this;

                foreach (var bag in Inventory.Backpacks.Where(b => b.Name != null && b.Name.IndexOf("fannypack", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    var bagAsItem = Inventory.Items.FirstOrDefault(i => i.UniqueIdentity.Instance == bag.Identity.Instance);
                    bagAsItem?.Use();
                }

                Game.OnUpdate += WaitForFannypacks;

                _settings = new HandlerSettings("ProfessionHandler");

                ItemWindow = new ItemsWindowController(_settings);
                MAAttackWindow = new MAAttacksWindowController(_settings);
                NukeWindow = new NukeWindowController(_settings);
                PerkWindow = new PerksWindowController(_settings);
                ProcWindow = new ProcsWindowController(_settings);
                PetWindow = new PetBuffWindowController(_settings);
                TrimmerWindow = new TrimmersWindowController(_settings);
                HoldWindow = new HoldWindowController(_settings);
                DebuffWindow = new DebuffWindowController(_settings);
                HealWindow = new HealWindowController(_settings);
                TauntWindow = new TauntWindowController(_settings);
                BuffWindow = new BuffWindowController(_settings);

                PluginDir = pluginDir;

                _wieldedWeapons = GetWieldedWeapons(DynelManager.LocalPlayer);

                _settings.AddVariable("Version_Number", 2.43f);
                _settings.AddVariable("MAX_CONCURRENT_PERKS", 1);

                #region Handler Settings

                _settings.AddVariable("SyncPets", true);
                SyncPetsString = _settings["SyncPets"].AsBool() ? "SyncPets Disable" : "SyncPets Enable";

                _settings.AddVariable("Enable", true);
                _settings["Enable"] = true;

                _settings.AddVariable("IPCChannel", 0);
                _settings["IPCChannel"] = 0;

                _settings.AddVariable("Reserve NCU", 0);

                _settings.AddVariable("AOE", false);
                _settings.AddVariable("Roots/Snares/Stuns", false);
                _settings.AddVariable("Specials", true);

                _settings.AddVariable("ProcType1Selection1", 0);
                _settings.AddVariable("ProcType1Selection2", 0);
                _settings.AddVariable("ProcType2Selection1", 0);
                _settings.AddVariable("ProcType2Selection2", 0);

                _settings.AddVariable("Buffing", false);
                _settings.AddVariable("Composites", false);
                _settings.AddVariable("WaitForRez", false);

                _settings.AddVariable("Nuking", false);

                _settings.AddVariable("RecastAntiFear", false);
                _settings.AddVariable("ReleaseMeNow", false);
                _settings.AddVariable("OSTMongo", false);

                _settings.AddVariable("UseCapsule", false);

                #region Spells

                #region Heals

                _settings.AddVariable("FountainOfLifeHeal", 0);
                _settings.AddVariable("FountainOfLifeHealValue", 40);
                _settings.AddVariable("FountainOfLifeHealOption", 2);

                _settings.AddVariable("AdventureSingleTargetHealing", 0);
                _settings.AddVariable("AdventureSingleTargetHealingValue", 75);
                _settings.AddVariable("AdventureSingleTargetHealingOption", 2);

                _settings.AddVariable("DoctorSingleTargetHealing", 0);
                _settings.AddVariable("DoctorSingleTargetHealingValue", 70);
                _settings.AddVariable("DoctorSingleTargetHealingOption", 2);

                _settings.AddVariable("MartialArtistSingleTargetHealing", 0);
                _settings.AddVariable("MartialArtistSingleTargetHealingValue", 75);
                _settings.AddVariable("MartialArtistSingleTargetHealingOption", 2);

                _settings.AddVariable("TraderHealthTarget", 0);
                _settings.AddVariable("TraderHealthTargetValue", 0);
                _settings.AddVariable("TraderHealthTargetOption", 0);

                _settings.AddVariable("ShadeHealthDrain", 0);
                _settings.AddVariable("ShadeHealthDrainValue", 0);
                _settings.AddVariable("ShadeHealthDrainOption", 0);

                _settings.AddVariable("TraderHealthDrain", 0);
                _settings.AddVariable("TraderHealthDrainValue", 0);
                _settings.AddVariable("TraderHealthDrainOption", 0);

                _settings.AddVariable("AdventurerCompleteHealing", 0);
                _settings.AddVariable("AdventurerCompleteHealingValue", 35);
                _settings.AddVariable("AdventurerCompleteHealingOption", 2);

                _settings.AddVariable("DoctorCompleteTargetHealing", 0);
                _settings.AddVariable("DoctorCompleteTargetHealingValue", 40);
                _settings.AddVariable("DoctorCompleteTargetHealingOption", 2);

                _settings.AddVariable("SoldierCompleteHealing", 0);
                _settings.AddVariable("SoldierCompleteHealingValue", 0);
                _settings.AddVariable("SoldierCompleteHealingOption", 0);

                _settings.AddVariable("CompleteTeamHealingLine", 0);
                _settings.AddVariable("CompleteTeamHealingLineValue", 40);
                _settings.AddVariable("CompleteTeamHealingLineOption", 1);

                _settings.AddVariable("SoldierDrainHeal", 0);
                _settings.AddVariable("SoldierDrainHealValue", 0);
                _settings.AddVariable("SoldierDrainHealOption", 0);

                _settings.AddVariable("TraderDrainHeal", 0);
                _settings.AddVariable("TraderDrainHealValue", 0);
                _settings.AddVariable("TraderDrainHealOption", 0);

                _settings.AddVariable("DoctorHealOverTime", 0);
                _settings.AddVariable("DoctorHealOverTimeValue", 0);
                _settings.AddVariable("DoctorHealOverTimeOption", 0);

                _settings.AddVariable("FixerShortHealOverTime", 0);
                _settings.AddVariable("FixerShortHealOverTimeValue", 0);
                _settings.AddVariable("FixerShortHealOverTimeOption", 0);

                _settings.AddVariable("MorphHeal", 0);
                _settings.AddVariable("MorphHealValue", 0);
                _settings.AddVariable("MorphHealOption", 0);

                _settings.AddVariable("NemesisNanoPrograms", true);

                _settings.AddVariable("HealPets", 0);
                _settings.AddVariable("HealPetsValue", 70);
                _settings.AddVariable("HealPetsOption", 1);

                _settings.AddVariable("AOEHealPets", 0);
                _settings.AddVariable("AOEHealPetsValue", 70);
                _settings.AddVariable("AOEHealPetsOption", 1);

                _settings.AddVariable("AdventurerTeamHealing", 0);
                _settings.AddVariable("AdventurerTeamHealingValue", 80);
                _settings.AddVariable("AdventurerTeamHealingOption", 1);

                _settings.AddVariable("DoctorTeamHealing", 0);
                _settings.AddVariable("DoctorTeamHealingValue", 70);
                _settings.AddVariable("DoctorTeamHealingOption", 1);

                _settings.AddVariable("MartialArtistTeamHealing", 0);
                _settings.AddVariable("MartialArtistTeamHealingValue", 75);
                _settings.AddVariable("MartialArtistTeamHealingOption", 1);

                _settings.AddVariable("DoctorShortHPBuffs", 0);
                _settings.AddVariable("DoctorShortHPBuffsValue", 0);
                _settings.AddVariable("DoctorShortHPBuffsOption", 4);

                _settings.AddVariable("TeamImprovedLifeChanneler", 0);
                _settings.AddVariable("TeamImprovedLifeChannelerValue", 80);
                _settings.AddVariable("TeamImprovedLifeChannelerOption", 1);

                _settings.AddVariable("NanoTechnicianNanoPointHeals", 0);
                _settings.AddVariable("NanoTechnicianNanoPointHealsValue", 0);
                _settings.AddVariable("NanoTechnicianNanoPointHealsOption", 0);

                _settings.AddVariable("TraderNanoPointHeals", 0);
                _settings.AddVariable("TraderNanoPointHealsValue", 0);
                _settings.AddVariable("TraderNanoPointHealsOption", 0);

                _settings.AddVariable("PetHealingValue", 80);
                _settings.AddVariable("PetHealingOption", 2);

                _settings.AddVariable("FixerLongHoTSelf", 0);
                _settings.AddVariable("FixerLongHoTSelfCheckBox", 0);

                _settings.AddVariable("FixerLongHoTTarget", 0);
                _settings.AddVariable("FixerLongHoTTargetOption", 0);

                _settings.AddVariable("TraderTeamHeals", 0);
                _settings.AddVariable("TraderTeamHealsValue", 0);
                _settings.AddVariable("TraderTeamHealsOption", 0);

                #endregion

                #region Debuffs

                _settings.AddVariable("PetMezzingOption", 2);

                _settings.AddVariable("AAODebuffs", 0);
                _settings.AddVariable("AAODebuffsOption", 0);
                _settings.AddVariable("BioMetDebuff", 0);
                _settings.AddVariable("BioMetDebuffOption", 0);
                _settings.AddVariable("DamageDrain", 0);
                _settings.AddVariable("DamageDrainOption", 0);
                _settings.AddVariable("DebuffNanoACHeavy", 0);
                _settings.AddVariable("DebuffNanoACHeavyOption", 0);
                _settings.AddVariable("EngineerDebuffAuras", 0);
                _settings.AddVariable("EngineerDebuffAurasOption", 0);
                _settings.AddVariable("EvasionDebuffs", 0);
                _settings.AddVariable("EvasionDebuffsOption", 0);
                _settings.AddVariable("EvasionDebuffs_Agent", 0);
                _settings.AddVariable("EvasionDebuffs_AgentOption", 0);
                _settings.AddVariable("General1HandBluntDebuff", 0);
                _settings.AddVariable("General1HandBluntDebuffOption", 0);
                _settings.AddVariable("General1HEdgedDebuff", 0);
                _settings.AddVariable("General1HEdgedDebuffOption", 0);
                _settings.AddVariable("General2HBluntDebuff", 0);
                _settings.AddVariable("General2HBluntDebuffOption", 0);
                _settings.AddVariable("General2HEdgedDebuff", 0);
                _settings.AddVariable("General2HEdgedDebuffOption", 0);
                _settings.AddVariable("GeneralAgilityDebuff", 0);
                _settings.AddVariable("GeneralAgilityDebuffOption", 0);
                _settings.AddVariable("GeneralAssaultRifleDebuff", 0);
                _settings.AddVariable("GeneralAssaultRifleDebuffOption", 0);
                _settings.AddVariable("GeneralBioMetDebuff", 0);
                _settings.AddVariable("GeneralBioMetDebuffOption", 0);
                _settings.AddVariable("GeneralBowDebuff", 0);
                _settings.AddVariable("GeneralBowDebuffOption", 0);
                _settings.AddVariable("GeneralBowSpecialDebuff", 0);
                _settings.AddVariable("GeneralBowSpecialDebuffOption", 0);
                _settings.AddVariable("GeneralBrawlDebuff", 0);
                _settings.AddVariable("GeneralBrawlDebuffOption", 0);
                _settings.AddVariable("GeneralBurstDebuff", 0);
                _settings.AddVariable("GeneralBurstDebuffOption", 0);
                _settings.AddVariable("GeneralChemicalACDebuff", 0);
                _settings.AddVariable("GeneralChemicalACDebuffOption", 0);
                _settings.AddVariable("GeneralColdACDebuff", 0);
                _settings.AddVariable("GeneralColdACDebuffOption", 0);
                _settings.AddVariable("GeneralDeflectDebuff", 0);
                _settings.AddVariable("GeneralDeflectDebuffOption", 0);
                _settings.AddVariable("GeneralDimachDebuff", 0);
                _settings.AddVariable("GeneralDimachDebuffOption", 0);
                _settings.AddVariable("GeneralEnergyACDebuff", 0);
                _settings.AddVariable("GeneralEnergyACDebuffOption", 0);
                _settings.AddVariable("GeneralEnergyMeleeDebuff", 0);
                _settings.AddVariable("GeneralEnergyMeleeDebuffOption", 0);
                _settings.AddVariable("GeneralFlingShotDebuff", 0);
                _settings.AddVariable("GeneralFlingShotDebuffOption", 0);
                _settings.AddVariable("GeneralFullAutoDebuff", 0);
                _settings.AddVariable("GeneralFullAutoDebuffOption", 0);
                _settings.AddVariable("GeneralGrenadeDebuff", 0);
                _settings.AddVariable("GeneralGrenadeDebuffOption", 0);
                _settings.AddVariable("GeneralIntelligenceDebuff", 0);
                _settings.AddVariable("GeneralIntelligenceDebuffOption", 0);
                _settings.AddVariable("GeneralKnifeDebuff", 0);
                _settings.AddVariable("GeneralKnifeDebuffOption", 0);
                _settings.AddVariable("GeneralLREnergyWeaponDebuff", 0);
                _settings.AddVariable("GeneralLREnergyWeaponDebuffOption", 0);
                _settings.AddVariable("GeneralMartialArtsDebuff", 0);
                _settings.AddVariable("GeneralMartialArtsDebuffOption", 0);
                _settings.AddVariable("GeneralMatCreaDebuff", 0);
                _settings.AddVariable("GeneralMatCreaDebuffOption", 0);
                _settings.AddVariable("GeneralMatLocDebuff", 0);
                _settings.AddVariable("GeneralMatLocDebuffOption", 0);
                _settings.AddVariable("GeneralMatMetDebuff", 0);
                _settings.AddVariable("GeneralMatMetDebuffOption", 0);
                _settings.AddVariable("GeneralMeleeACDebuff", 0);
                _settings.AddVariable("GeneralMeleeACDebuffOption", 0);
                _settings.AddVariable("GeneralNanoACDebuff", 0);
                _settings.AddVariable("GeneralNanoACDebuffOption", 0);
                _settings.AddVariable("GeneralPiercingDebuff", 0);
                _settings.AddVariable("GeneralPiercingDebuffOption", 0);
                _settings.AddVariable("GeneralPistoDebuff", 0);
                _settings.AddVariable("GeneralPistoDebuffOption", 0);
                _settings.AddVariable("GeneralPoisonACDebuff", 0);
                _settings.AddVariable("GeneralPoisonACDebuffOption", 0);
                _settings.AddVariable("GeneralProjectileACDebuff", 0);
                _settings.AddVariable("GeneralProjectileACDebuffOption", 0);
                _settings.AddVariable("GeneralPsyModDebuff", 0);
                _settings.AddVariable("GeneralPsyModDebuffOption", 0);
                _settings.AddVariable("GeneralPsychicDebuff", 0);
                _settings.AddVariable("GeneralPsychicDebuffOption", 0);
                _settings.AddVariable("GeneralRadiationACDebuff", 0);
                _settings.AddVariable("GeneralRadiationACDebuffOption", 0);
                _settings.AddVariable("GeneralRiposteDebuff", 0);
                _settings.AddVariable("GeneralRiposteDebuffOption", 0);
                _settings.AddVariable("GeneralRifleDebuff", 0);
                _settings.AddVariable("GeneralRifleDebuffOption", 0);
                _settings.AddVariable("GeneralSenseDebuff", 0);
                _settings.AddVariable("GeneralSenseDebuffOption", 0);
                _settings.AddVariable("GeneralSenseImpDebuff", 0);
                _settings.AddVariable("GeneralSenseImpDebuffOption", 0);
                _settings.AddVariable("GeneralShotgunDebuff", 0);
                _settings.AddVariable("GeneralShotgunDebuffOption", 0);
                _settings.AddVariable("GeneralSMGDebuff", 0);
                _settings.AddVariable("GeneralSMGDebuffOption", 0);
                _settings.AddVariable("GeneralSneakAttackDebuff", 0);
                _settings.AddVariable("GeneralSneakAttackDebuffOption", 0);
                _settings.AddVariable("GeneralStaminaDebuff", 0);
                _settings.AddVariable("GeneralStaminaDebuffOption", 0);
                _settings.AddVariable("GeneralStrengthDebuff", 0);
                _settings.AddVariable("GeneralStrengthDebuffOption", 0);
                _settings.AddVariable("HealDeltaDebuff", 0);
                _settings.AddVariable("HealDeltaDebuffOption", 0);
                _settings.AddVariable("HealReactivityMultiplierDebuff", 0);
                _settings.AddVariable("HealReactivityMultiplierDebuffOption", 0);
                _settings.AddVariable("HealthandNanoOverTimeDrain", 0);
                _settings.AddVariable("HealthandNanoOverTimeDrainOption", 0);
                _settings.AddVariable("HopeDebuff", 0);
                _settings.AddVariable("HopeDebuffOption", 0);

                _settings.AddVariable("InitiativeDebuffs", 0);
                _settings.AddVariable("InitiativeDebuffsOption", 0);

                _settings.AddVariable("DocBreakableInitDebuffs", 0);
                _settings.AddVariable("DocBreakableInitDebuffsOption", 0);

                _settings.AddVariable("IntelligenceDebuff", 0);
                _settings.AddVariable("IntelligenceDebuffOption", 0);

                _settings.AddVariable("MatCreaDebuff", 0);
                _settings.AddVariable("MatCreaDebuffOption", 0);

                _settings.AddVariable("PsyModDebuff", 0);
                _settings.AddVariable("PsyModDebuffOption", 0);

                _settings.AddVariable("MatLocDebuff", 0);
                _settings.AddVariable("MatLocDebuffOption", 0);
                _settings.AddVariable("MatMetDebuff", 0);
                _settings.AddVariable("MatMetDebuffOption", 0);
                _settings.AddVariable("MetaPhysicistDamageDebuff", 0);
                _settings.AddVariable("MetaPhysicistDamageDebuffOption", 0);
                _settings.AddVariable("MiseryDebuff", 0);
                _settings.AddVariable("MiseryDebuffOption", 0);
                _settings.AddVariable("MPDamageDebuffLineA", 0);
                _settings.AddVariable("MPDamageDebuffLineAOption", 0);
                _settings.AddVariable("MPDamageDebuffLineB", 0);
                _settings.AddVariable("MPDamageDebuffLineBOption", 0);
                _settings.AddVariable("NanoDeltaDebuff", 0);
                _settings.AddVariable("NanoDeltaDebuffOption", 0);
                _settings.AddVariable("NanoDrain_LineA", 0);
                _settings.AddVariable("NanoDrain_LineAOption", 0);
                _settings.AddVariable("NanoDrain_LineB", 0);
                _settings.AddVariable("NanoDrain_LineBOption", 0);

                _settings.AddVariable("TraderNanoResistanceDebuff_LineA", 0);
                _settings.AddVariable("TraderNanoResistanceDebuff_LineAOption", 0);

                _settings.AddVariable("MetaPhysicistNanoResistanceDebuff_LineA", 0);
                _settings.AddVariable("MetaPhysicistNanoResistanceDebuff_LineAOption", 0);

                _settings.AddVariable("NanoResistDebuffProc", 0);
                _settings.AddVariable("NanoResistDebuffProcOption", 0);
                _settings.AddVariable("NanoShutdownDebuff", 0);
                _settings.AddVariable("NanoShutdownDebuffOption", 0);
                _settings.AddVariable("NPCostDebuff", 0);
                _settings.AddVariable("NPCostDebuffOption", 0);
                _settings.AddVariable("PathofDarknessDebuff", 0);
                _settings.AddVariable("PathofDarknessDebuffOption", 0);
                _settings.AddVariable("ProximityRangeDebuff", 0);
                _settings.AddVariable("ProximityRangeDebuffOption", 0);
                _settings.AddVariable("PsychicDebuff", 0);
                _settings.AddVariable("PsychicDebuffOption", 0);
                _settings.AddVariable("RoadToDarknessDebuff", 0);
                _settings.AddVariable("RoadToDarknessDebuffOption", 0);
                _settings.AddVariable("SenseImpDebuff", 0);
                _settings.AddVariable("SenseImpDebuffOption", 0);
                _settings.AddVariable("ShadeInitDebuffProc", 0);
                _settings.AddVariable("ShadeInitDebuffProcOption", 0);
                _settings.AddVariable("SilenceDebuff", 0);
                _settings.AddVariable("SilenceDebuffOption", 0);
                _settings.AddVariable("SkillLockModifierDebuff1053", 0);
                _settings.AddVariable("SkillLockModifierDebuff1053Option", 0);
                _settings.AddVariable("SkillLockModifierDebuff847", 0);
                _settings.AddVariable("SkillLockModifierDebuff847Option", 0);
                _settings.AddVariable("TraderDebuffACNanos", 0);
                _settings.AddVariable("TraderDebuffACNanosOption", 0);
                _settings.AddVariable("TraderShutdownSkillDebuff", 0);
                _settings.AddVariable("TraderShutdownSkillDebuffOption", 0);
                _settings.AddVariable("TraderAADDrain", 0);
                _settings.AddVariable("TraderAADDrainOption", 0);
                _settings.AddVariable("TraderAAODrain", 0);
                _settings.AddVariable("TraderAAODrainOption", 0);
                _settings.AddVariable("TraderACTransferTargetDebuff_Draw", 0);
                _settings.AddVariable("TraderACTransferTargetDebuff_DrawOption", 0);
                _settings.AddVariable("TraderACTransferTargetDebuff_Siphon", 0);
                _settings.AddVariable("TraderACTransferTargetDebuff_SiphonOption", 0);
                _settings.AddVariable("TraderSkillTransferTargetDebuff_Deprive", 0);
                _settings.AddVariable("TraderSkillTransferTargetDebuff_DepriveOption", 0);
                _settings.AddVariable("TraderSkillTransferTargetDebuff_Ransack", 0);
                _settings.AddVariable("TraderSkillTransferTargetDebuff_RansackOption", 0);

                _settings.AddVariable("SLNanopointDrain", 0);
                _settings.AddVariable("SLNanopointDrainOption", 0);

                _settings.AddVariable("HaloNanoDebuff", 0);
                _settings.AddVariable("HaloNanoDebuffOption", 0);

                _settings.AddVariable("LickofthePest", 0);
                _settings.AddVariable("LickofthePestOption", 0);

                _settings.AddVariable("AOEBlinds", 0);
                _settings.AddVariable("AOEBlindsOption", 0);

                _settings.AddVariable("DemotivationalSpeeches", 0);
                _settings.AddVariable("DemotivationalSpeechesOption", 0);

                #endregion

                #region Non Combat Buffs

                _settings.AddVariable("SLMap", 0);
                _settings.AddVariable("SLMapOption", 0);

                _settings.AddVariable("CompositeAttribute", false);
                _settings.AddVariable("CompositeNano", false);
                _settings.AddVariable("CompositeUtility", false);
                _settings.AddVariable("CompositeTradeskill", false);
                _settings.AddVariable("CompositeMartialProwess", false);
                _settings.AddVariable("CompositeMelee", false);
                _settings.AddVariable("CompositePhysicalSpecial", false);
                _settings.AddVariable("CompositeRanged", false);
                _settings.AddVariable("CompositeRangedSpecial", false);

                _settings.AddVariable("TargetStrengthBuffDoctor", 0);
                _settings.AddVariable("TargetStrengthBuffDoctorOption", 0);

                _settings.AddVariable("TargetStrengthBuffEnforcer", 0);
                _settings.AddVariable("TargetStrengthBuffEnforcerOption", 0);

                _settings.AddVariable("TargetStrengthBuffMartialArtist", 0);
                _settings.AddVariable("TargetStrengthBuffMartialArtistOption", 0);

                _settings.AddVariable("UserDamageShieldsAdventurer", 0);
                _settings.AddVariable("UserDamageShieldsAdventurerCheckBox", false);

                _settings.AddVariable("UserDamageShieldsEnforcer", 0);
                _settings.AddVariable("UserDamageShieldsEnforcerCheckBox", false);

                _settings.AddVariable("TargetDamageShieldsAdventurer", 0);
                _settings.AddVariable("TargetDamageShieldsAdventurerOption", 0);

                _settings.AddVariable("TargetDamageShieldsEnforcer", 0);
                _settings.AddVariable("TargetDamageShieldsEnforcerOption", 0);

                _settings.AddVariable("TargetDamageShieldsEngineer", 0);
                _settings.AddVariable("TargetDamageShieldsEngineerOption", 0);

                _settings.AddVariable("UserMajorEvasionBuffsMartialArtist", 0);
                _settings.AddVariable("UserMajorEvasionBuffsMartialArtistCheckBox", false);

                _settings.AddVariable("UserMajorEvasionBuffsMetaphysicist", 0);
                _settings.AddVariable("UserMajorEvasionBuffsMetaphysicistCheckBox", false);

                _settings.AddVariable("UserMajorEvasionBuffsNanoTechnician", 0);
                _settings.AddVariable("UserMajorEvasionBuffsNanoTechnicianCheckBox", false);

                _settings.AddVariable("UserMajorEvasionBuffsSoldier", 0);
                _settings.AddVariable("UserMajorEvasionBuffsSoldierCheckBox", false);

                _settings.AddVariable("UserMajorEvasionBuffsTrader", 0);
                _settings.AddVariable("UserMajorEvasionBuffsTraderCheckBox", false);

                _settings.AddVariable("TargetMajorEvasionBuffs_RunspeedBuffsFixer", 0);
                _settings.AddVariable("TargetMajorEvasionBuffs_RunspeedBuffsFixerOption", 0);

                _settings.AddVariable("TargetMajorEvasionBuffsMartialArtist", 0);
                _settings.AddVariable("TargetMajorEvasionBuffsMartialArtistOption", 0);

                _settings.AddVariable("TargetMajorEvasionBuffsMetaphysicist", 0);
                _settings.AddVariable("TargetMajorEvasionBuffsMetaphysicistOption", 0);

                _settings.AddVariable("TargetMajorEvasionBuffsNanoTechnician", 0);
                _settings.AddVariable("TargetMajorEvasionBuffsNanoTechnicianOption", 0);

                _settings.AddVariable("TargetMajorEvasionBuffsSoldier", 0);
                _settings.AddVariable("TargetMajorEvasionBuffsSoldierOption", 0);

                _settings.AddVariable("TargetMajorEvasionBuffsTrader", 0);
                _settings.AddVariable("TargetMajorEvasionBuffsTraderOption", 0);

                //_settings.AddVariable("MajorEvasionBuffsTeam", 0);
                //_settings.AddVariable("MajorEvasionBuffsTeamCheckBox", false);

                _settings.AddVariable("UserArmorBuffMartialArtist", 0);
                _settings.AddVariable("UserArmorBuffMartialArtistCheckBox", false);

                _settings.AddVariable("TargetArmorBuffAdventurer", 0);
                _settings.AddVariable("TargetArmorBuffAdventurerOption", 0);

                _settings.AddVariable("TargetArmorBuffEngineer", 0);
                _settings.AddVariable("TargetArmorBuffEngineerOption", 0);

                _settings.AddVariable("TargetArmorBuffMartialArtist", 0);
                _settings.AddVariable("TargetArmorBuffMartialArtistOption", 0);

                _settings.AddVariable("TargetArmorBuffSoldier", 0);
                _settings.AddVariable("TargetArmorBuffSoldierOption", 0);

                _settings.AddVariable("UserInitiativeBuffsMartialArtist", 0);
                _settings.AddVariable("UserInitiativeBuffsMartialArtistCheckBox", false);

                _settings.AddVariable("TargetInitiativeBuffsDoctor", 0);
                _settings.AddVariable("TargetInitiativeBuffsDoctorOption", 0);

                _settings.AddVariable("TargetInitiativeBuffsEnforcer", 0);
                _settings.AddVariable("TargetInitiativeBuffsEnforcerOption", 0);

                _settings.AddVariable("TargetInitiativeBuffsEngineer", 0);
                _settings.AddVariable("TargetInitiativeBuffsEngineerOption", 0);

                _settings.AddVariable("TargetInitiativeBuffsMartialArtist", 0);
                _settings.AddVariable("TargetInitiativeBuffsMartialArtistOption", 0);

                _settings.AddVariable("TargetInitiativeBuffsSoldier", 0);
                _settings.AddVariable("TargetInitiativeBuffsSoldierOption", 0);

                _settings.AddVariable("UserFortifyNanoTechnician", 0);
                _settings.AddVariable("UserFortifyNanoTechnicianCheckBox", false);

                _settings.AddVariable("TeamFortifyKeeper", 0);
                _settings.AddVariable("TeamFortifyKeeperOption", 0);

                _settings.AddVariable("TeamFortifyAdventurer", false);

                _settings.AddVariable("ShadowlandReflectBaseTeam", 0);
                _settings.AddVariable("ShadowlandReflectBaseTeamCheckBox", false);

                _settings.AddVariable("UserDamageBuffs_LineAAdventurer", 0);
                _settings.AddVariable("UserDamageBuffs_LineAAdventurerCheckBox", false);

                _settings.AddVariable("UserDamageBuffs_LineAEngineer", 0);
                _settings.AddVariable("UserDamageBuffs_LineAEngineerCheckBox", false);

                _settings.AddVariable("UserDamageBuffs_LineAMartialArtist", 0);
                _settings.AddVariable("UserDamageBuffs_LineAMartialArtistCheckBox", false);

                _settings.AddVariable("TargetDamageBuffs_LineAAgent", 0);
                _settings.AddVariable("TargetDamageBuffs_LineAAgentOption", 0);

                _settings.AddVariable("TargetDamageBuffs_LineAEngineer", 0);
                _settings.AddVariable("TargetDamageBuffs_LineAEngineerOption", 0);

                _settings.AddVariable("TargetDamageBuffs_LineAFixer", 0);
                _settings.AddVariable("TargetDamageBuffs_LineAFixerOption", 0);

                _settings.AddVariable("TargetDamageBuffs_LineASoldier", 0);
                _settings.AddVariable("TargetDamageBuffs_LineASoldierOption", 0);

                _settings.AddVariable("TeamExperienceConstructs_XPBonusAdventurer", 0);
                _settings.AddVariable("TeamExperienceConstructs_XPBonusAdventurerCheckBox", false);

                _settings.AddVariable("TeamExperienceConstructs_XPBonusBureaucrat", 0);
                _settings.AddVariable("TeamExperienceConstructs_XPBonusBureaucratCheckBox", false);

                _settings.AddVariable("TargetFirstAidAndTreatmentBuffAdventurer", 0);
                _settings.AddVariable("TargetFirstAidAndTreatmentBuffAdventurerOption", 0);

                _settings.AddVariable("TargetFirstAidAndTreatmentBuffDoctor", 0);
                _settings.AddVariable("TargetFirstAidAndTreatmentBuffDoctorOption", 0);

                _settings.AddVariable("UserMultiwieldBuffAdventurer", 0);
                _settings.AddVariable("UserMultiwieldBuffAdventurerCheckBox", false);

                _settings.AddVariable("UserMultiwieldBuffShade", 0);
                _settings.AddVariable("UserMultiwieldBuffShadeCheckBox", false);

                _settings.AddVariable("UserNanoResistanceBuffsAdventurer", 0);
                _settings.AddVariable("UserNanoResistanceBuffsAdventurerCheckBox", false);

                _settings.AddVariable("TargetNanoResistanceBuffs", 0);
                _settings.AddVariable("TargetNanoResistanceBuffsOption", 0);

                _settings.AddVariable("TargetRunspeedBuffsAdventurer", 0);
                _settings.AddVariable("TargetRunspeedBuffsAdventurerOption", 0);

                _settings.AddVariable("TargetRunspeedBuffsMartialArtist", 0);
                _settings.AddVariable("TargetRunspeedBuffsMartialArtistOption", 0);

                _settings.AddVariable("TargetRunspeedBuffsShade", 0);
                _settings.AddVariable("TargetRunspeedBuffsShadeOption", 0);

                _settings.AddVariable("TeamRunSpeedBuffs", 0);
                _settings.AddVariable("TeamRunSpeedBuffsCheckBox", false);

                _settings.AddVariable("TeamRunspeedBuffsFixer", 0);
                _settings.AddVariable("TeamRunspeedBuffsFixerCheckBox", false);

                _settings.AddVariable("SelfRoot_SnareResistBuff", 0);
                _settings.AddVariable("SelfRoot_SnareResistBuffCheckBox", false);

                _settings.AddVariable("OtherRoot_SnareResistBuff", 0);
                _settings.AddVariable("OtherRoot_SnareResistBuffOption", 0);

                _settings.AddVariable("DamageShieldUpgrades", 0);
                _settings.AddVariable("DamageShieldUpgradesCheckBox", false);

                _settings.AddVariable("_1HEdgedBuff", 0);
                _settings.AddVariable("_1HEdgedBuffCheckBox", false);

                _settings.AddVariable("UserPistolBuffAdventurer", 0);
                _settings.AddVariable("UserPistolBuffAdventurerCheckBox", false);

                _settings.AddVariable("UserPistolBureaucrat", 0);
                _settings.AddVariable("UserPistolBureaucratCheckBox", false);

                _settings.AddVariable("PistolBuffTarget", 0);
                _settings.AddVariable("PistolBuffTargetOption", 0);

                _settings.AddVariable("TargetPistolBureaucrat", 0);
                _settings.AddVariable("TargetPistolBureaucratOption", 0);

                _settings.AddVariable("AimedShotBuffsSelf", 0);
                _settings.AddVariable("AimedShotBuffsSelfCheckBox", false);

                _settings.AddVariable("TargetAimedShotBuffsAgent", 0);
                _settings.AddVariable("TargetAimedShotBuffsAgentOption", 0);

                _settings.AddVariable("TargetAimedShotBuffsAdventurer", 0);
                _settings.AddVariable("TargetAimedShotBuffsAdventurerOption", 0);

                _settings.AddVariable("TargetPerceptionBuffsAdventurer", 0);
                _settings.AddVariable("TargetPerceptionBuffsAdventurerOption", 0);

                _settings.AddVariable("TargetPerceptionBuffsFixer", 0);
                _settings.AddVariable("TargetPerceptionBuffsFixerOption", 0);

                _settings.AddVariable("SneakAttackBuffsSelf", 0);
                _settings.AddVariable("SneakAttackBuffsSelfCheckBox", false);

                _settings.AddVariable("TargetSneakAttackBuffs", 0);
                _settings.AddVariable("TargetSneakAttackBuffsOption", 0);

                _settings.AddVariable("TargetSneakAttackBuffsFixer", 0);
                _settings.AddVariable("TargetSneakAttackBuffsFixerOption", 0);

                _settings.AddVariable("AgentDamageProc_DamageInflictSegment", 0);
                _settings.AddVariable("AgentDamageProc_DamageInflictSegmentCheckBox", false);

                _settings.AddVariable("AgentProcBuff", 0);
                _settings.AddVariable("AgentProcBuffCheckBox", false);

                _settings.AddVariable("UserAgilityBuffAgent", 0);
                _settings.AddVariable("UserAgilityBuffAgentCheckBox", false);

                _settings.AddVariable("UserAgilityBuffShade", 0);
                _settings.AddVariable("UserAgilityBuffShadeCheckBox", false);

                _settings.AddVariable("AgilityBuffTarget", 0);
                _settings.AddVariable("AgilityBuffTargetOption", 0);

                _settings.AddVariable("SenseBuff", 0);
                _settings.AddVariable("SenseBuffOption", 0);

                _settings.AddVariable("UserCriticalIncreaseBuffAgent", 0);
                _settings.AddVariable("UserCriticalIncreaseBuffAgentCheckBox", false);

                _settings.AddVariable("UserCriticalIncreaseBuffMartialArtist", 0);
                _settings.AddVariable("UserCriticalIncreaseBuffMartialArtistCheckBox", false);

                _settings.AddVariable("TargetCriticalIncreaseBuffAgent", 0);
                _settings.AddVariable("TargetCriticalIncreaseBuffAgentOption", 0);

                _settings.AddVariable("TargetCriticalIncreaseBuffMartialArtist", 0);
                _settings.AddVariable("TargetCriticalIncreaseBuffMartialArtistOption", 0);

                _settings.AddVariable("ExecutionerBuff", 0);
                _settings.AddVariable("ExecutionerBuffCheckBox", false);

                _settings.AddVariable("TargetRifleBuffsAgent", 0);
                _settings.AddVariable("TargetRifleBuffsAgentOption", 0);

                _settings.AddVariable("TargetRifleBuffsSoldier", 0);
                _settings.AddVariable("TargetRifleBuffsSoldierOption", 0);

                _settings.AddVariable("UserConcealmentBuffAgent", 0);
                _settings.AddVariable("UserConcealmentBuffAgentCheckBox", false);

                _settings.AddVariable("UserConcealmentBuffShade", 0);
                _settings.AddVariable("UserConcealmentBuffShadeCheckBox", false);

                _settings.AddVariable("TargetConcealmentBuffAgent", 0);
                _settings.AddVariable("TargetConcealmentBuffAgentOption", 0);

                _settings.AddVariable("TargetConcealmentBuffBureaucrat", 0);
                _settings.AddVariable("TargetConcealmentBuffBureaucratOption", 0);

                _settings.AddVariable("TargetConcealmentBuffFixer", 0);
                _settings.AddVariable("TargetConcealmentBuffFixerOption", 0);

                _settings.AddVariable("ConcealmentBuffTeam", 0);
                _settings.AddVariable("ConcealmentBuffTeamCheckBox", false);

                _settings.AddVariable("SpeechLine", 0);
                _settings.AddVariable("SpeechLineCheckBox", false);

                _settings.AddVariable("NanoDeltaBuffs", 0);
                _settings.AddVariable("NanoDeltaBuffsOption", 0);

                _settings.AddVariable("UserPsy_IntBuffMetaphysicist", 0);
                _settings.AddVariable("UserPsy_IntBuffMetaphysicistCheckBox", false);

                _settings.AddVariable("UserPsy_IntBuffNanoTechnician", 0);
                _settings.AddVariable("UserPsy_IntBuffNanoTechnicianCheckBox", false);

                _settings.AddVariable("Psy_IntBuffTarget", 0);
                _settings.AddVariable("Psy_IntBuffTargetOption", 0);

                _settings.AddVariable("PsychologyBuff", 0);
                _settings.AddVariable("PsychologyBuffOption", 0);

                _settings.AddVariable("CriticalDecreaseBuff", 0);
                _settings.AddVariable("CriticalDecreaseBuffOption", 0);

                _settings.AddVariable("DoctorHPBuffs", 0);
                _settings.AddVariable("DoctorHPBuffsCheckBox", false);

                _settings.AddVariable("HealDeltaBuff", 0);
                _settings.AddVariable("HealDeltaBuffOption", 0);

                _settings.AddVariable("_1HBluntBuff", 0);
                _settings.AddVariable("_1HBluntBuffOption", 0);

                _settings.AddVariable("DamageChangeBuffs", 0);
                _settings.AddVariable("DamageChangeBuffsCheckBox", false);

                _settings.AddVariable("EnforcerMeleeEnergyBuff", 0);
                _settings.AddVariable("EnforcerMeleeEnergyBuffCheckBox", false);

                _settings.AddVariable("EnforcerPiercingBuff", 0);
                _settings.AddVariable("EnforcerPiercingBuffCheckBox", false);

                _settings.AddVariable("EnforcerTauntProcs", 0);
                _settings.AddVariable("EnforcerTauntProcsCheckBox", false);

                _settings.AddVariable("FastAttackBuffs", 0);
                _settings.AddVariable("FastAttackBuffsCheckBox", false);

                _settings.AddVariable(".UserHPBuffEnforcer", 0);
                _settings.AddVariable(".UserHPBuffEnforcerCheckBox", false);

                _settings.AddVariable("UserHPBuffSoldier", 0);
                _settings.AddVariable("UserHPBuffSoldierCheckBox", false);

                _settings.AddVariable("TargetHPBuffEnforcer", 0);
                _settings.AddVariable("TargetHPBuffEnforcerOption", 0);

                _settings.AddVariable("TargetHPBuffSoldier", 0);
                _settings.AddVariable("TargetHPBuffSoldierOption", 0);

                _settings.AddVariable("MeleeWeaponBuffLine", 0);
                _settings.AddVariable("MeleeWeaponBuffLineCheckBox", false);

                _settings.AddVariable("EngineeringBuff", 0);
                _settings.AddVariable("EngineeringBuffCheckBox", false);

                _settings.AddVariable("EngineerAuras", 0);
                _settings.AddVariable("EngineerAurasCheckBox", false);

                _settings.AddVariable("EngineerSpecialAttackAbsorberSelf", 0);
                _settings.AddVariable("EngineerSpecialAttackAbsorberSelfCheckBox", false);
                _settings.AddVariable("EngineerSpecialAttackAbsorberTeam", 0);
                _settings.AddVariable("EngineerSpecialAttackAbsorberTeamCheckBox", false);

                _settings.AddVariable("GrenadeBuffs", 0);
                _settings.AddVariable("GrenadeBuffsOption", 0);

                _settings.AddVariable("SpecialAttackAbsorberBase", 0);
                _settings.AddVariable("SpecialAttackAbsorberBaseCheckBox", false);

                _settings.AddVariable("TargetReflectShieldEngineer", 0);
                _settings.AddVariable("TargetReflectShieldEngineerOption", 0);

                _settings.AddVariable("TargetReflectShieldSoldier", 0);
                _settings.AddVariable("TargetReflectShieldSoldierOption", 0);

                _settings.AddVariable("FixerDodgeBuffLine", 0);
                _settings.AddVariable("FixerDodgeBuffLineCheckBox", false);

                _settings.AddVariable("FixerNCUBuffs", 0);
                _settings.AddVariable("FixerNCUBuffsCheckBox", false);

                _settings.AddVariable("FixerSuppressorBuff", 0);
                _settings.AddVariable("FixerSuppressorBuffCheckBox", false);

                _settings.AddVariable("ShadowlandsRunspeed", 0);
                _settings.AddVariable("ShadowlandsRunspeedCheckBox", false);

                _settings.AddVariable("_2HEdgedBuff", 0);
                _settings.AddVariable("_2HEdgedBuffCheckBox", false);

                _settings.AddVariable("TargetAAOBuffsKeeper", 0);
                _settings.AddVariable("TargetAAOBuffsKeeperOption", 0);

                _settings.AddVariable("TargetAAOBuffsSoldier", 0);
                _settings.AddVariable("TargetAAOBuffsSoldierOption", 0);

                _settings.AddVariable("KeeperAura_Absorb_Reflect_AMSBuff", 0);
                _settings.AddVariable("KeeperAura_Absorb_Reflect_AMSBuffCheckBox", false);

                _settings.AddVariable("KeeperAura_Damage_SnareReductionBuff", 0);
                _settings.AddVariable("KeeperAura_Damage_SnareReductionBuffCheckBox", false);

                _settings.AddVariable("KeeperAura_HPandNPHeal", 0);
                _settings.AddVariable("KeeperAura_HPandNPHealCheckBox", false);

                _settings.AddVariable("KeeperDeflect_RiposteBuff", 0);
                _settings.AddVariable("KeeperDeflect_RiposteBuffCheckBox", false);

                _settings.AddVariable("KeeperEvade_Dodge_DuckBuff", 0);
                _settings.AddVariable("KeeperEvade_Dodge_DuckBuffCheckBox", false);

                _settings.AddVariable("KeeperProcBuff", 0);
                _settings.AddVariable("KeeperProcBuffCheckBox", false);

                _settings.AddVariable("KeeperStr_Stam_AgiBuff", 0);
                _settings.AddVariable("KeeperStr_Stam_AgiBuffCheckBox", false);

                _settings.AddVariable("Fury", 0);
                _settings.AddVariable("FuryCheckBox", false);

                _settings.AddVariable("BrawlBuff", 0);
                _settings.AddVariable("BrawlBuffOption", 0);

                _settings.AddVariable("ControlledRageBuff", 0);
                _settings.AddVariable("ControlledRageBuffOption", 0);

                _settings.AddVariable("DamageBuff_LineCSelf", 0);
                _settings.AddVariable("DamageBuff_LineCSelfCheckBox", false);

                _settings.AddVariable("DamageBuff_LineCTarget", 0);
                _settings.AddVariable("DamageBuff_LineCTargetOption", 0);
                _settings.AddVariable("DamageBuff_LineCTeam", 0);
                _settings.AddVariable("DamageBuff_LineCTeamCheckBox", false);

                _settings.AddVariable("MartialArtistBowBuffsMartialArtist", 0);
                _settings.AddVariable("MartialArtistBowBuffsMartialArtistCheckBox", false);

                _settings.AddVariable("MartialArtistBowBuffsMetaphysicist", 0);
                _settings.AddVariable("MartialArtistBowBuffsMetaphysicistCheckBox", false);

                _settings.AddVariable("MartialArtistZazenStance", 0);
                _settings.AddVariable("MartialArtistZazenStanceCheckBox", false);

                _settings.AddVariable("UserMartialArtsBuffMartialArtist", 0);
                _settings.AddVariable("UserMartialArtsBuffMartialArtistCheckBox", false);

                _settings.AddVariable("UserMartialArtsBuffShade", 0);
                _settings.AddVariable("UserMartialArtsBuffShadeCheckBox", false);

                _settings.AddVariable("MartialArtsBuffTarget", 0);
                _settings.AddVariable("MartialArtsBuffTargetOption", 0);

                _settings.AddVariable("NanoResistBuff", 0);
                _settings.AddVariable("NanoResistBuffCheckBox", false);

                _settings.AddVariable("RiposteBuff", 0);
                _settings.AddVariable("RiposteBuffCheckBox", false);

                _settings.AddVariable("LimboMastery", 0);
                _settings.AddVariable("LimboMasteryOption", 0);

                _settings.AddVariable("Horde", 0);
                _settings.AddVariable("HordeCheckBox", false);

                _settings.AddVariable("Cohort", 0);
                _settings.AddVariable("CohortCheckBox", false);

                _settings.AddVariable("MPCompositeNano", 0);
                _settings.AddVariable("MPCompositeNanoOption", 0);

                _settings.AddVariable("BioMetBuff", 0);
                _settings.AddVariable("BioMetBuffOption", 0);

                _settings.AddVariable("MatCreaBuffSelf", 0);
                _settings.AddVariable("MatCreaBuffSelfCheckBox", false);
                _settings.AddVariable("MatCreaBuffTarget", 0);
                _settings.AddVariable("MatCreaBuffTargetOption", 0);

                _settings.AddVariable("MatMetBuff", 0);
                _settings.AddVariable("MatMetBuffOption", 0);

                _settings.AddVariable("PsyModBuff", 0);
                _settings.AddVariable("PsyModBuffOption", 0);

                _settings.AddVariable("SenseImpBuff", 0);
                _settings.AddVariable("SenseImpBuffOption", 0);

                _settings.AddVariable("MatLocBuff", 0);
                _settings.AddVariable("MatLocBuffOption", 0);

                _settings.AddVariable("InterruptModifier", 0);
                _settings.AddVariable("InterruptModifierOption", 0);

                _settings.AddVariable("TargetNPCostBuffMetaphysicist", 0);
                _settings.AddVariable("TargetNPCostBuffMetaphysicistOption", 0);

                _settings.AddVariable("TargetNPCostBuffNanoTechnician", 0);
                _settings.AddVariable("TargetNPCostBuffNanoTechnicianOption", 0);

                _settings.AddVariable("NanoDamageMultiplierBuffs", 0);
                _settings.AddVariable("NanoDamageMultiplierBuffsCheckBox", false);

                _settings.AddVariable("NanoOverTime_LineA", 0);
                _settings.AddVariable("NanoOverTime_LineAOption", 0);

                _settings.AddVariable("NFRangeBuff", 0);
                _settings.AddVariable("NFRangeBuffOption", 0);

                _settings.AddVariable("ShadePiercingBuff", 0);
                _settings.AddVariable("ShadePiercingBuffCheckBox", false);

                _settings.AddVariable("AADBuffs", 0);
                _settings.AddVariable("AADBuffsOption", 0);

                _settings.AddVariable("ShadeProcBuff", 0);
                _settings.AddVariable("ShadeProcBuffCheckBox", false);

                _settings.AddVariable("WeaponEffectAdd_On2", 0);
                _settings.AddVariable("WeaponEffectAdd_On2CheckBox", false);

                _settings.AddVariable("AssaultRifleBuffsSelf", 0);
                _settings.AddVariable("AssaultRifleBuffsSelfCheckBox", false);
                _settings.AddVariable("AssaultRifleBuffsTarget", 0);
                _settings.AddVariable("AssaultRifleBuffsTargetOption", 0);

                _settings.AddVariable("CompositeHeavyArtillery", 0);
                _settings.AddVariable("CompositeHeavyArtilleryOption", 0);

                _settings.AddVariable("BurstBuffSelf", 0);
                _settings.AddVariable("BurstBuffSelfCheckBox", false);
                _settings.AddVariable("BurstBuffTarget", 0);
                _settings.AddVariable("BurstBuffTargetOption", 0);

                _settings.AddVariable("SoldierDamageBase", 0);
                _settings.AddVariable("SoldierDamageBaseCheckBox", false);

                _settings.AddVariable("HeavyWeaponsBuffs", 0);
                _settings.AddVariable("HeavyWeaponsBuffsCheckBox", false);

                _settings.AddVariable("RangedEnergyWeaponBuffsSelf", 0);
                _settings.AddVariable("RangedEnergyWeaponBuffsSelfCheckBox", false);
                _settings.AddVariable("RangedEnergyWeaponBuffsTarget", 0);
                _settings.AddVariable("RangedEnergyWeaponBuffsTargetOption", 0);

                _settings.AddVariable("SoldierFullAutoBuff", 0);
                _settings.AddVariable("SoldierFullAutoBuffCheckBox", false);

                _settings.AddVariable("SoldierShotgunBuff", 0);
                _settings.AddVariable("SoldierShotgunBuffCheckBox", false);

                _settings.AddVariable("TotalFocus", 0);
                _settings.AddVariable("TotalFocusCheckBox", false);

                _settings.AddVariable("Phalanx", 0);
                _settings.AddVariable("PhalanxCheckBox", false);

                _settings.AddVariable("SiphonBox683", 0);
                _settings.AddVariable("SiphonBox683CheckBox", false);

                _settings.AddVariable("UmbralWrangler", 0);
                _settings.AddVariable("UmbralWranglerCheckBox", false);

                _settings.AddVariable("SlayerdroidTransference", 0);
                _settings.AddVariable("SlayerdroidTransferenceCheckBox", false);

                _settings.AddVariable("Break_EntryBuffs", 0);
                _settings.AddVariable("Break_EntryBuffsOption", 0);

                #endregion

                #region Combat Buffs

                _settings.AddVariable("ConcentrationCriticalLine", 0);
                _settings.AddVariable("ConcentrationCriticalLineOption", 0);

                _settings.AddVariable("TakeTheBulletValue", 0);
                _settings.AddVariable("TakeTheBulletOption", 0);

                _settings.AddVariable("Ransack_DepriveResistBuff", 0);
                _settings.AddVariable("Ransack_DepriveResistBuffOption", 0);

                _settings.AddVariable("UserAbsorbACBuffEnforcer", 0);
                _settings.AddVariable("UserAbsorbACBuffEnforcerValue", 0);
                _settings.AddVariable("UserAbsorbACBuffEnforcerOption", 0);

                _settings.AddVariable("UserAbsorbACBuffNanoTechnician", 0);
                _settings.AddVariable("UserAbsorbACBuffNanoTechnicianValue", 0);
                _settings.AddVariable("UserAbsorbACBuffNanoTechnicianOption", 0);

                _settings.AddVariable("TargetAbsorbACBuffEnforcer", 0);
                _settings.AddVariable("TargetAbsorbACBuffEnforcerValue", 0);
                _settings.AddVariable("TargetAbsorbACBuffEnforcerOption", 0);

                _settings.AddVariable("TargetAbsorbACBuffNanoTechnician", 0);
                _settings.AddVariable("TargetAbsorbACBuffNanoTechnicianValue", 0);
                _settings.AddVariable("TargetAbsorbACBuffNanoTechnicianOption", 0);

                _settings.AddVariable("Challenger", 0);
                _settings.AddVariable("ChallengerValue", 0);
                _settings.AddVariable("ChallengerOption", 0);

                _settings.AddVariable("Rage", 0);
                _settings.AddVariable("RageValue", 0);
                _settings.AddVariable("RageOption", 0);

                _settings.AddVariable("Charge", 0);
                _settings.AddVariable("ChargeOption", 0);

                _settings.AddVariable("FixerFearImmunity", 0);
                _settings.AddVariable("FixerFearImmunityOption", 0);

                _settings.AddVariable("NullitySphereNano", 0);
                _settings.AddVariable("NullitySphereNanoOption", 0);
                _settings.AddVariable("NullitySphereNanoValue", 0);

                _settings.AddVariable("NanobotAegis", 0);
                _settings.AddVariable("NanobotAegisOption", 0);
                _settings.AddVariable("NanobotAegisValue", 0);

                _settings.AddVariable("DamagetoNano", 0);
                _settings.AddVariable("DamagetoNanoOption", 0);
                _settings.AddVariable("DamagetoNanoValue", 0);

                _settings.AddVariable("ReflectShieldSelf", 0);
                _settings.AddVariable("ReflectShieldSelfOption", 0);
                _settings.AddVariable("ReflectShieldSelfValue", 0);

                _settings.AddVariable("DamageBuff_LineCSelfMartialArtist", 0);
                _settings.AddVariable("DamageBuff_LineCSelfMartialArtistOption", false);

                _settings.AddVariable("ControlledDestructionBuff", 0);
                _settings.AddVariable("ControlledDestructionBuffOption", false);

                _settings.AddVariable("AdventurerMorphBuffOption", 0);

                _settings.AddVariable("ReflectShieldNanoTechnicianSelf", 0);
                _settings.AddVariable("ReflectShieldNanoTechnicianSelfCheckBox", false);

                #endregion

                #region Pets

                _settings.AddVariable("NanoSpellBuffs", false);
                _settings.AddVariable("WrangelBuffs", false);
                _settings.AddVariable("BuffPets", false);
                _settings.AddVariable("WarpPets", false);

                #region Attack Pets
                _settings.AddVariable("SpawnAttackPet", 0);//
                _settings.AddVariable("SpawnAttackPetCheckBox", false);

                _settings.AddVariable("AttackSpareShell", false);

                _settings.AddVariable("AttackPetCompositeNano", false);//
                _settings.AddVariable("AttackPetCompositeMartialProwess", false);//
                _settings.AddVariable("AttackPetCompositeMelee", false);//
                _settings.AddVariable("AttackPetCompositePhysicalSpecial", false);//

                _settings.AddVariable("KenFi", false);
                _settings.AddVariable("ChannelRage", false);//

                _settings.AddVariable("Puppeteer", false);//

                _settings.AddVariable("OptimizeBotProtocol", false);//

                _settings.AddVariable("AttackPetGadgeteer", 0);//
                _settings.AddVariable("AttackPetGadgeteerCheckBox", false);//

                _settings.AddVariable("AttackPetArmorBuff", 0);
                _settings.AddVariable("AttackPetArmorBuffCheckBox", false);

                _settings.AddVariable("AttackPetDamageBuffs_LineAAgent", 0);
                _settings.AddVariable("AttackPetDamageBuffs_LineAAgentCheckBox", false);

                _settings.AddVariable("AttackPetDamageBuffs_LineAEngineer", 0);
                _settings.AddVariable("AttackPetDamageBuffs_LineAEngineerCheckBox", false);

                _settings.AddVariable("AttackPetDamageBuffs_LineAFixer", 0);
                _settings.AddVariable("AttackPetDamageBuffs_LineAFixerCheckBox", false);

                _settings.AddVariable("AttackPetDamageBuffs_LineASoldier", 0);
                _settings.AddVariable("AttackPetDamageBuffs_LineASoldierCheckBox", false);

                _settings.AddVariable("AttackPetCriticalDecreaseBuff", 0);//
                _settings.AddVariable("AttackPetCriticalDecreaseBuffCheckBox", false);//

                _settings.AddVariable("AttackPetAnticipationofRetaliation", 0);//
                _settings.AddVariable("AttackPetAnticipationofRetaliationCheckBox", false);//

                _settings.AddVariable("AttackPetProc", false);//

                _settings.AddVariable("AttackPetPetDamageOverTimeResistNanos", 0);//
                _settings.AddVariable("AttackPetPetDamageOverTimeResistNanosCheckBox", false);//

                _settings.AddVariable("AttackPetPetDefensiveNanos", 0);//
                _settings.AddVariable("AttackPetPetDefensiveNanosCheckBox", false);//

                _settings.AddVariable("AttackPetPetTauntBuff", 0);//
                _settings.AddVariable("AttackPetPetTauntBuffCheckBox", false);//

                _settings.AddVariable("AttackPetDroidDamageMatrix", false);//

                _settings.AddVariable("AttackPetEngineerMiniaturization", 0);
                _settings.AddVariable("AttackPetEngineerMiniaturizationCheckBox", false);

                _settings.AddVariable("AttackPetPetShortTermDamageBuffs", 0);//
                _settings.AddVariable("AttackPetPetShortTermDamageBuffsCheckBox", false);//

                _settings.AddVariable("AttackPetMPPetInitiativeBuffs", 0);//
                _settings.AddVariable("AttackPetMPPetInitiativeBuffsCheckBox", false);//

                _settings.AddVariable("AttackPetShieldOfObedientServant", 0);//
                _settings.AddVariable("AttackPetShieldOfObedientServantCheckBox", false);//

                _settings.AddVariable("AttackPetInstillDamageBuffs", 0);//
                _settings.AddVariable("AttackPetInstillDamageBuffsCheckBox", false);//

                _settings.AddVariable("AttackPetAggressiveConstructEmpowerment", 0);//
                _settings.AddVariable("AttackPetAggressiveConstructEmpowermentCheckBox", false);//

                _settings.AddVariable("AttackPetMPAttackPetDamageType", 0);//
                _settings.AddVariable("AttackPetMPAttackPetDamageTypeCheckBox", false);//

                _settings.AddVariable("AttackPetPetHealDelta843", 0);//
                _settings.AddVariable("AttackPetPetHealDelta843CheckBox", false);//

                _settings.AddVariable("AttackPetMPCompositeNano", 0);//
                _settings.AddVariable("AttackPetMPCompositeNanoCheckBox", false);//

                #endregion

                #region Support Pet

                _settings.AddVariable("SpawnSupportPet", 0);//
                _settings.AddVariable("SpawnSupportPetCheckBox", false);

                _settings.AddVariable("SupportSpareShell", false);

                _settings.AddVariable("SupportPetCompositeNano", false);//
                _settings.AddVariable("SupportPetCompositeMartialProwess", false);//
                _settings.AddVariable("SupportPetCompositeMelee", false);//
                _settings.AddVariable("SupportPetCompositePhysicalSpecial", false);//

                _settings.AddVariable("SupportPetGadgeteer", 0);//
                _settings.AddVariable("SupportPetGadgeteerCheckBox", false);//

                _settings.AddVariable("SupportPetArmorBuff", 0);//
                _settings.AddVariable("SupportPetArmorBuffCheckBox", false);//

                _settings.AddVariable("SupportPetDamageBuffs_LineAAgent", 0);
                _settings.AddVariable("SupportPetDamageBuffs_LineAAgentCheckBox", false);

                _settings.AddVariable("SupportPetDamageBuffs_LineAEngineer", 0);
                _settings.AddVariable("SupportPetDamageBuffs_LineAEngineerCheckBox", false);

                _settings.AddVariable("SupportPetDamageBuffs_LineAFixer", 0);
                _settings.AddVariable("SupportPetDamageBuffs_LineAFixerCheckBox", false);

                _settings.AddVariable("SupportPetDamageBuffs_LineASoldier", 0);
                _settings.AddVariable("SupportPetDamageBuffs_LineASoldierCheckBox", false);

                _settings.AddVariable("SupportPetCriticalDecreaseBuff", 0);//
                _settings.AddVariable("SupportPetCriticalDecreaseBuffCheckBox", false);//

                _settings.AddVariable("SupportPetAnticipationofRetaliation", 0);//
                _settings.AddVariable("SupportPetAnticipationofRetaliationCheckBox", false);//

                _settings.AddVariable("SupportPetEngineerMiniaturization", 0);
                _settings.AddVariable("SupportPetEngineerMiniaturizationCheckBox", false);

                _settings.AddVariable("SupportPetPetShortTermDamageBuffs", 0);//
                _settings.AddVariable("SupportPetPetShortTermDamageBuffsCheckBox", false);//

                _settings.AddVariable("SupportPetProc", false);//

                _settings.AddVariable("SupportPetMPPetInitiativeBuffs", 0);//
                _settings.AddVariable("SupportPetMPPetInitiativeBuffsCheckBox", false);//

                _settings.AddVariable("SupportPetPetDefensiveNanos", 0);//
                _settings.AddVariable("SupportPetPetDefensiveNanosCheckBox", false);//

                _settings.AddVariable("SupportPetShieldOfObedientServant", 0);//
                _settings.AddVariable("SupportPetShieldOfObedientServantCheckBox", false);//

                _settings.AddVariable("MesmerizationConstructEmpowerment", 0);//
                _settings.AddVariable("MesmerizationConstructEmpowermentCheckBox", false);//

                _settings.AddVariable("SupportPetPetDamageOverTimeResistNanos", 0);//
                _settings.AddVariable("SupportPetPetDamageOverTimeResistNanosCheckBox", false);//

                _settings.AddVariable("SupportPetPetHealDelta843", 0);//
                _settings.AddVariable("SupportPetPetHealDelta843CheckBox", false);//

                _settings.AddVariable("SupportPetCostBuffs", 0);//
                _settings.AddVariable("SupportPetCostBuffsCheckBox", false);//

                _settings.AddVariable("SupportPetMPCompositeNano", 0);//
                _settings.AddVariable("SupportPetMPCompositeNanoCheckBox", false);//

                _settings.AddVariable("SupportPetMPSenImpBuffs", 0);//
                _settings.AddVariable("SupportPetMPSenImpBuffsCheckBox", false);//

                _settings.AddVariable("SupportPetMPPsyModBuff", 0);//
                _settings.AddVariable("SupportPetMPPsyModBuffCheckBox", false);//

                #endregion

                #region Heal Pet

                _settings.AddVariable("SpawnHealPet", 0);//
                _settings.AddVariable("SpawnHealPetCheckBox", false);

                _settings.AddVariable("HealPetCompositeNano", false);//
                _settings.AddVariable("HealPetCompositeMartialProwess", false);//

                _settings.AddVariable("HealPetArmorBuff", 0);//
                _settings.AddVariable("HealPetArmorBuffCheckBox", false);//

                _settings.AddVariable("HealPetDamageBuffs_LineA", 0);//
                _settings.AddVariable("HealPetDamageBuffs_LineACheckBox", false);//

                _settings.AddVariable("HealPetCriticalDecreaseBuff", 0);//
                _settings.AddVariable("HealPetCriticalDecreaseBuffCheckBox", false);//

                _settings.AddVariable("HealPetAnticipationofRetaliation", 0);//
                _settings.AddVariable("HealPetAnticipationofRetaliationCheckBox", false);//

                _settings.AddVariable("HealPetPetDefensiveNanos", 0);//
                _settings.AddVariable("HealPetPetDefensiveNanosCheckBox", false);//

                _settings.AddVariable("HealingConstructEmpowerment", 0);//
                _settings.AddVariable("HealingConstructEmpowermentCheckBox", false);//

                _settings.AddVariable("HealPetPetDamageOverTimeResistNanos", 0);//
                _settings.AddVariable("HealPetPetDamageOverTimeResistNanosCheckBox", false);//

                _settings.AddVariable("HealPetPetHealDelta843", 0);//
                _settings.AddVariable("HealPetPetHealDelta843CheckBox", false);//

                _settings.AddVariable("HealPetCostBuffs", 0);//
                _settings.AddVariable("HealPetCostBuffsCheckBox", false);//

                _settings.AddVariable("HealPetMPCompositeNano", 0);//
                _settings.AddVariable("HealPetMPCompositeNanoCheckBox", false);//

                _settings.AddVariable("HealPetMPMatMetBuff", 0);//
                _settings.AddVariable("HealPetMPMatMetBuffCheckBox", false);//

                _settings.AddVariable("HealPetMPBioMetBuff", 0);//
                _settings.AddVariable("HealPetMPBioMetBuffCheckBox", false);//

                #endregion

                #endregion

                #region Nukes

                #region DOTs
                _settings.AddVariable("DOT_LineA", 0);
                _settings.AddVariable("DOT_LineAPriority", 20);
                _settings.AddVariable("DOT_LineAOption", 2);

                _settings.AddVariable("DOT_LineB", 0);
                _settings.AddVariable("DOT_LineBPriority", 20);
                _settings.AddVariable("DOT_LineBOption", 2);

                _settings.AddVariable("DOTNanotechnicianStrainA", 0);
                _settings.AddVariable("DOTNanotechnicianStrainAPriority", 20);
                _settings.AddVariable("DOTNanotechnicianStrainAOption", 2);

                _settings.AddVariable("DOTAgentStrainA", 0);
                _settings.AddVariable("DOTAgentStrainAPriority", 0);
                _settings.AddVariable("DOTAgentStrainAOption", 0);

                _settings.AddVariable("DOTStrainC", 0);
                _settings.AddVariable("DOTStrainCPriority", 20);
                _settings.AddVariable("DOTStrainCOption", 2);
                #endregion

                #region Nukes
                _settings.AddVariable("Nukes", 0);
                _settings.AddVariable("NukesPriority", 0);
                _settings.AddVariable("NukesOption", 0);

                _settings.AddVariable("AlphaNukes", 0);
                _settings.AddVariable("AlphaNukesPriority", 0);
                _settings.AddVariable("AlphaNukesOption", 0);

                _settings.AddVariable("Nuke", 0);
                _settings.AddVariable("NukePriority", 0);
                _settings.AddVariable("NukeOption", 0);

                _settings.AddVariable("AlphaNuke", 0);
                _settings.AddVariable("AlphaNukePriority", 0);
                _settings.AddVariable("AlphaNukeOption", 0);

                _settings.AddVariable("OmegaNuke", 0);
                _settings.AddVariable("OmegaNukePriority", 0);
                _settings.AddVariable("OmegaNukeOption", 0);

                _settings.AddVariable("SpecialEffectNukes", 0);
                _settings.AddVariable("SpecialEffectNukesPriority", 0);
                _settings.AddVariable("SpecialEffectNukesOption", 0);

                _settings.AddVariable("NTNukesA", 0);
                _settings.AddVariable("NTNukesAPriority", 20);
                _settings.AddVariable("NTNukesAOption", 1);

                _settings.AddVariable("DOTNanotechnicianStrainB", 0);
                _settings.AddVariable("DOTNanotechnicianStrainBPriority", 30);
                _settings.AddVariable("DOTNanotechnicianStrainBOption", 2);

                _settings.AddVariable("NTNukesB", 0);
                _settings.AddVariable("NTNukesBPriority", 40);
                _settings.AddVariable("NTNukesBOption", 1);

                _settings.AddVariable("NormalNuke", 0);
                _settings.AddVariable("NormalNukePriority", 40);
                _settings.AddVariable("NormalNukeOption", 2);

                _settings.AddVariable("SpecialNuke", 0);
                _settings.AddVariable("SpecialNukePriority", 0);
                _settings.AddVariable("SpecialNukeOption", 0);

                _settings.AddVariable("MPNormalNuke", 0);
                _settings.AddVariable("MPNormalNukePriority", 40);
                _settings.AddVariable("MPNormalNukeOption", 2);

                _settings.AddVariable("MindDamage", 0);
                _settings.AddVariable("MindDamagePriority", 30);
                _settings.AddVariable("MindDamageOption", 2);

                #endregion

                #region AOENukes
                _settings.AddVariable("NTSLAOENukes", 0);
                _settings.AddVariable("NTSLAOENukesPriority", 0);
                _settings.AddVariable("NTSLAOENukesOption", 0);
                _settings.AddVariable("NTRKAOENukes", 0);
                _settings.AddVariable("NTRKAOENukesPriority", 20);
                _settings.AddVariable("NTRKAOENukesOption", 1);
                _settings.AddVariable("NTAreaNukes", 0);
                _settings.AddVariable("NTAreaNukesPriority", 0);
                _settings.AddVariable("NTAreaNukesOption", 0);
                _settings.AddVariable("NTAreaNukes2", 0);
                _settings.AddVariable("NTAreaNukes2Priority", 0);
                _settings.AddVariable("NTAreaNukes2Option", 0);
                _settings.AddVariable("AOENuke", 0);
                _settings.AddVariable("AOENukePriority", 0);
                _settings.AddVariable("AOENukeOption", 0);
                #endregion

                #endregion

                #region Holds

                _settings.AddVariable("RaidRoot", false);
                _settings.AddVariable("AOESnare", false);

                #region Target

                _settings.AddVariable("AdventurerMezzTarget", 0);
                _settings.AddVariable("AdventurerMezzTargetOption", 0);

                _settings.AddVariable("AgentMezzTarget", 0);
                _settings.AddVariable("AgentMezzTargetOption", 0);

                _settings.AddVariable("AgentRootTarget", 0);
                _settings.AddVariable("AgentRootTargetOption", 0);

                _settings.AddVariable("AgentSnareTarget", 0);
                _settings.AddVariable("AgentSnareTargetOption", 0);

                _settings.AddVariable("BureaucratMezzTarget", 0);
                _settings.AddVariable("BureaucratMezzTargetOption", 0);

                _settings.AddVariable("BureaucratMezzStunTarget", 0);
                _settings.AddVariable("BureaucratMezzStunTargetOption", 0);

                _settings.AddVariable("BureaucratRootTarget", 0);
                _settings.AddVariable("BureaucratRootTargetOption", 0);

                _settings.AddVariable("BureaucratSnareTarget", 0);
                _settings.AddVariable("BureaucratSnareTargetOption", 0);

                _settings.AddVariable("LastMinNegotiations", 0);
                _settings.AddVariable("LastMinNegotiationsOption", 0);

                _settings.AddVariable("FixerSnareTarget", 0);
                _settings.AddVariable("FixerSnareTargetOption", 0);

                _settings.AddVariable("FixerRootTarget", 0);
                _settings.AddVariable("FixerRootTargetOption", 0);

                _settings.AddVariable("MetaPhysicistMezzTarget", 0);
                _settings.AddVariable("MetaPhysicistMezzTargetOption", 0);

                _settings.AddVariable("NanoTechnicianMezzStunTarget", 0);
                _settings.AddVariable("NanoTechnicianMezzStunTargetOption", 0);

                _settings.AddVariable("NanoTechnicianMezzCalmTarget", 0);
                _settings.AddVariable("NanoTechnicianMezzCalmTargetOption", 0);

                _settings.AddVariable("NanoTechnicianMezzHackedBlindTarget", 0);
                _settings.AddVariable("NanoTechnicianMezzHackedBlindTargetOption", 0);

                _settings.AddVariable("NanoTechnicianRootTarget", 0);
                _settings.AddVariable("NanoTechnicianRootTargetOption", 0);

                _settings.AddVariable("TraderMezzTarget", 0);
                _settings.AddVariable("TraderMezzTargetOption", 0);

                _settings.AddVariable("TraderRootTarget", 0);
                _settings.AddVariable("TraderRootTargetOption", 0);

                _settings.AddVariable("PerkTheShot", false);
                _settings.AddVariable("PerkAssassinate", false);
                _settings.AddVariable("PerkConcussiveShot", false);
                _settings.AddVariable("PerkSoftenUp", false);

                _settings.AddVariable("PerkConfoundWithRules", false);
                _settings.AddVariable("PerkGroinKick", false);
                _settings.AddVariable("PerkDisorientate", false);
                _settings.AddVariable("PerkCrushBone", false);
                _settings.AddVariable("PerkBringThePain", false);
                _settings.AddVariable("PerkBlindsideBlow", false);
                _settings.AddVariable("PerkFullFrontal", false);
                _settings.AddVariable("PerkGuesstimate", false);
                _settings.AddVariable("PerkQuarkContainmentField", false);
                _settings.AddVariable("PerkJarringBurst", false);
                _settings.AddVariable("PerkLegShot", false);
                _settings.AddVariable("PerkNanoShakes", false);
                _settings.AddVariable("PerkGrasp", false);
                _settings.AddVariable("PerkBearhug", false);
                _settings.AddVariable("PerkGripOfColossus", false);

                #endregion

                #region AOE

                _settings.AddVariable("AgentSnareArea", 0);
                _settings.AddVariable("AgentSnareAreaOption", 0);

                _settings.AddVariable("BureaucratMezzArea", 0);
                _settings.AddVariable("BureaucratMezzAreaOption", 0);

                _settings.AddVariable("BureaucratRootArea", 0);
                _settings.AddVariable("BureaucratRootAreaOption", 0);

                _settings.AddVariable("BureaucratSnareArea", 0);
                _settings.AddVariable("BureaucratSnareAreaOption", 0);

                _settings.AddVariable("EngineerPetAOESnareBuff", 0);
                _settings.AddVariable("EngineerPetAOESnareBuffOption", 0);

                _settings.AddVariable("FixerSnareArea", 0);
                _settings.AddVariable("FixerSnareAreaOption", 0);

                _settings.AddVariable("TraderRootArea", 0);
                _settings.AddVariable("TraderRootAreaOption", 0);

                _settings.AddVariable("PerkStoneworks", false);

                #endregion

                #endregion

                #endregion

                #region Taunts

                // Taunts
                _settings.AddVariable("TauntSpellSingleTarget", 0);
                _settings.AddVariable("TauntSpellSingleTargetOption", 2);
                _settings.AddVariable("TauntSpellSingleTargetValue", 5);

                _settings.AddVariable("TauntSpellTimedSingleTarget", 0);
                _settings.AddVariable("TauntSpellTimedSingleTargetOption", 2);
                _settings.AddVariable("TauntSpellTimedSingleTargetValue", 4);

                _settings.AddVariable("TauntPerkHatred", 0);
                _settings.AddVariable("TauntPerkHatredOption", 1);

                _settings.AddVariable("TauntPerkTaunt", 0);
                _settings.AddVariable("TauntPerkTauntOption", 1);

                _settings.AddVariable("TauntPerkArouseAnger", 0);
                _settings.AddVariable("TauntPerkArouseAngerOption", 1);

                _settings.AddVariable("TauntPerkCauseOfAnger", 0);
                _settings.AddVariable("TauntPerkCauseOfAngerOption", 1);

                //Item
                _settings.AddVariable("TauntItemTauntTools", 0);
                _settings.AddVariable("TauntItemTauntToolsCheckBox", false);

                // AOETaunts
                _settings.AddVariable("TauntSpellAOETaunt", 0);
                _settings.AddVariable("TauntSpellAOETauntOption", 2);
                _settings.AddVariable("TauntSpellAOETauntValue", 4);

                #endregion

                #region Perk HandlerSettings

                #region HealthHealPerks

                _settings.AddVariable("AwakeningValue", 40);
                _settings.AddVariable("BalanceOfYinAndYangValue", 40);
                _settings.AddVariable("BattlegroupHeal1Value", 40);
                _settings.AddVariable("BattlegroupHeal2Value", 40);
                _settings.AddVariable("BattlegroupHeal3Value", 40);
                _settings.AddVariable("BattlegroupHeal4Value", 40);
                _settings.AddVariable("BioRegrowthValue", 40);
                _settings.AddVariable("BioRejuvenationValue", 40);
                _settings.AddVariable("ConsumeTheSoulValue", 40);
                _settings.AddVariable("DiffuseValue", 40);
                _settings.AddVariable("CuringTouchValue", 40);
                _settings.AddVariable("DevourValue", 40);
                _settings.AddVariable("DrawBloodValue", 40);
                _settings.AddVariable("BlessingOfLifeValue", 40);
                _settings.AddVariable("EnhancedHealValue", 40);
                _settings.AddVariable("ExultationValue", 40);
                _settings.AddVariable("FieldBandageValue", 40);
                _settings.AddVariable("HarmonizeBodyAndMindValue", 40);
                _settings.AddVariable("LayOnHandsValue", 40);
                _settings.AddVariable("LifebloodValue", 40);
                _settings.AddVariable("SurvivalValue", 40);
                _settings.AddVariable("HealValue", 40);

                _settings.AddVariable("SpiritOfBlessingValue", 40);

                _settings.AddVariable("ReapLifeValue", 40);
                _settings.AddVariable("ReconstructDNAValue", 40);
                _settings.AddVariable("ReconstructionValue", 40);
                _settings.AddVariable("RedDawnValue", 40);

                _settings.AddVariable("TeamHealValue", 40);
                _settings.AddVariable("TapVitaeValue", 40);
                _settings.AddVariable("VitalShockValue", 40);
                _settings.AddVariable("PurpleHeartValue", 40);

                #endregion

                #region NanoHealPerks

                _settings.AddVariable("NanoHealValue", 40);
                _settings.AddVariable("RegainNanoValue", 40);
                _settings.AddVariable("SpiritOfPurityValue", 40);
                _settings.AddVariable("TapNotumSourceValue", 40);
                _settings.AddVariable("AccessNotumSourceValue", 40);

                #endregion

                #region BuffPerks

                _settings.AddVariable("MongoRageSelection", 1);
                _settings.AddVariable("NotumShieldSelection", 1);
                _settings.AddVariable("TackyHackSelection", 1);
                _settings.AddVariable("SphereValue", 1);
                _settings.AddVariable("PowerOfLightSelection", 1);
                _settings.AddVariable("BladeOfNightSelection", 1);
                _settings.AddVariable("BeckoningSelection", 1);
                _settings.AddVariable("OverruleSelection", 1);
                _settings.AddVariable("FreakShieldSelection", 1);
                _settings.AddVariable("HammerAndAnvilSelection", 1);
                _settings.AddVariable("HighwaySelection", 1);
                _settings.AddVariable("DevotionalArmorSelection", 1);
                _settings.AddVariable("BladeWhirlwindSelection", 1);
                _settings.AddVariable("MoonmistSelection", 1);
                _settings.AddVariable("FlimFocusSelection", 1);
                _settings.AddVariable("ProgramOverloadSelection", 1);
                _settings.AddVariable("ViolenceSelection", 1);
                _settings.AddVariable("SupressiveHordeSelection", 1);
                _settings.AddVariable("SacrificeSelection", 1);
                _settings.AddVariable("EnergizeSelection", 1);
                _settings.AddVariable("ReinforceSlugsSelection", 1);
                _settings.AddVariable("KnowledgeEnhancerSelection", 1);
                _settings.AddVariable("InsightSelection", 1);
                _settings.AddVariable("ToxicShockSelection", 1);

                _settings.AddVariable("MyOwnFortressValue", 40);
                _settings.AddVariable("WitOfTheAtroxValue", 40);
                _settings.AddVariable("DodgeTheBlameValue", 40);
                _settings.AddVariable("BioShieldValue", 40);
                _settings.AddVariable("BioCocoonValue", 40);
                _settings.AddVariable("EncaseInStoneValue", 40);
                _settings.AddVariable("LimberValue", 40);
                _settings.AddVariable("DanceOfFoolsValue", 40);
                _settings.AddVariable("EvasiveStanceValue", 40);
                _settings.AddVariable("TrollFormValue", 40);

                #endregion

                #region DebuffPerks

                _settings.AddVariable("SwordSelection", 1);
                _settings.AddVariable("PenSelection", 1);
                _settings.AddVariable("DerivateSelection", 1);
                _settings.AddVariable("BlindedByDelightsSelection", 1);
                _settings.AddVariable("FadeArmorSelection", 1);
                _settings.AddVariable("SuccumbSelection", 1);
                _settings.AddVariable("MaliciousProhibitionSelection", 1);
                _settings.AddVariable("DeconstructionSelection", 1);
                _settings.AddVariable("InstallNotumDepletionDeviceSelection", 1);
                _settings.AddVariable("SuppressivePrimerSelection", 1);
                _settings.AddVariable("ThermalPrimerSelection", 1);
                _settings.AddVariable("MarkOfVengeanceSelection", 1);
                _settings.AddVariable("MarkOfTheUncleanSelection", 1);
                _settings.AddVariable("MarkOfTheUnhallowedSelection", 1);
                _settings.AddVariable("DragonfireSelection", 1);
                _settings.AddVariable("IncapacitateSelection", 1);
                _settings.AddVariable("RedDuskSelection", 1);
                _settings.AddVariable("EtherealTouchSelection", 1);
                _settings.AddVariable("ConvulsiveTremorSelection", 1);
                _settings.AddVariable("DoomTouchSelection", 1);
                _settings.AddVariable("BlurSelection", 1);
                _settings.AddVariable("TracerSelection", 1);
                _settings.AddVariable("LaserPaintTargetSelection", 1);
                _settings.AddVariable("TriangulateTargetSelection", 1);
                _settings.AddVariable("NapalmSpraySelection", 1);
                _settings.AddVariable("ChemicalBlindnessSelection", 1);
                _settings.AddVariable("DazzleWithLightsSelection", 1);
                _settings.AddVariable("HostileTakeoverSelection", 1);
                _settings.AddVariable("ArmorPiercingShotSelection", 1);
                _settings.AddVariable("StopNotumFlowSelection", 1);
                _settings.AddVariable("NotumOverflowSelection", 1);
                _settings.AddVariable("TickSelection", 1);
                _settings.AddVariable("ZapNanoSelection", 1);

                #endregion

                #endregion

                #region Item HandlerSettings

                #region Item Heals

                _settings.AddVariable("TotwDocBooksValue", 40);

                _settings.AddVariable("KitsOption", true);
                _settings.AddVariable("HealthKitsValue", 90);
                _settings.AddVariable("NanoKitsValue", 90);
                _settings.AddVariable("StimsOption", false);
                _settings.AddVariable("HealthStimsValue", 30);
                _settings.AddVariable("NanoStimsValue", 0);
                _settings.AddVariable("DeathsDoorOption", false);
                _settings.AddVariable("DeathsDoorValue", 40);
                _settings.AddVariable("TOTWWristsValue", 40);
                _settings.AddVariable("PremiumNanoRechargerValue", 40);
                _settings.AddVariable("ViralCommunicationsLarvaeValue", 40);
                _settings.AddVariable("NotumFocusValue", 40);
                _settings.AddVariable("SanguisugentBodyArmorValue", 50);
                _settings.AddVariable("PerniciousBodyArmorValue", 1);

                #endregion

                #region Item Damage

                _settings.AddVariable("SharpObjects", 0);
                _settings.AddVariable("SharpObjectsOption", 0);

                _settings.AddVariable("ThrowingGrenades", 0);
                _settings.AddVariable("ThrowingGrenadesOption", 0);

                _settings.AddVariable("RingOfFleshes", 0);
                _settings.AddVariable("RingOfFleshesOption", 0);

                _settings.AddVariable("WenWen", 0);
                _settings.AddVariable("WenWenOption", 0);

                _settings.AddVariable("Manta", 0);
                _settings.AddVariable("MantaOption", 0);

                _settings.AddVariable("MantaAOE", 0);
                _settings.AddVariable("MantaAOEOption", 0);

                _settings.AddVariable("ToTWBloodRings", 0);
                _settings.AddVariable("ToTWBloodRingsOption", 0);

                _settings.AddVariable("ToTWFlameRings", 0);
                _settings.AddVariable("ToTWFlameRingsOption", 0);

                _settings.AddVariable("ICCDrone", 0);
                _settings.AddVariable("ICCDroneOption", 0);

                _settings.AddVariable("SpecialArrows", 0);
                _settings.AddVariable("SpecialArrowsOption", 0);

                _settings.AddVariable("AOESpecialArrows", 0);
                _settings.AddVariable("AOESpecialArrowsOption", 0);

                _settings.AddVariable("SuppressiveBurstItem", 0);
                _settings.AddVariable("SuppressiveBurstItemOption", 0);

                _settings.AddVariable("ClusterBullets", 0);
                _settings.AddVariable("ClusterBulletsOption", 0);

                _settings.AddVariable("HomingPermorphaBullets", 0);
                _settings.AddVariable("HomingPermorphaBulletsOption", 0);

                _settings.AddVariable("RingofSisterPestilenceSelection", 1);
                _settings.AddVariable("RingofSisterMercilessSelection", 1);

                #endregion

                #region MA Attacks

                _settings.AddVariable("SappoOption", false);
                _settings.AddVariable("StingoftheViperOption", false);
                _settings.AddVariable("ApeFistofKhalumOption", false);
                _settings.AddVariable("KarmicFistOption", false);
                _settings.AddVariable("ShenOption", false);
                _settings.AddVariable("FlowerOfLifeOption", false);
                _settings.AddVariable("FlowerOfLifesValue", 40);
                _settings.AddVariable("BrightBlueCloudlessSkyOption", false);
                _settings.AddVariable("BlessedWithThunderOption", false);
                _settings.AddVariable("BirdOfPreyOption", false);
                _settings.AddVariable("AttackOfTheSnakeOption", false);
                _settings.AddVariable("AngelOfNightOption", false);

                _settings.AddVariable("TouchOfSaiFungOption", false);
                _settings.AddVariable("TheWizdomOfHuzzumOption", false);
                _settings.AddVariable("TreeOfEnlightenmentOption", false);

                _settings.AddVariable("UponAWaveOfSummerOption", false);
                _settings.AddVariable("StampedeOfTheBoarOption", false);

                _settings.AddVariable("DeliriumOption", false);

                _settings.AddVariable("InnerBalanceOption", false);

                _settings.AddVariable("EnigmaOption", false);

                #endregion

                #region Item Buffs

                _settings.AddVariable("FlurryOfBlowsSelection", 1);
                _settings.AddVariable("EyeOfTheHunterSelection", 1);
                _settings.AddVariable("SharlsCyberneticTattooValue", 40);
                _settings.AddVariable("GnuffsEternalRiftCrystalSelection", 1);
                _settings.AddVariable("ReflectGraftOption", false);
                _settings.AddVariable("DreadlochEnduranceBoosterValue", 40);
                _settings.AddVariable("TotwDmgShouldersSelection", 1);
                _settings.AddVariable("TotwShieldShouldersValue", 40);
                _settings.AddVariable("MuscularStimValue", 40);
                _settings.AddVariable("AssaultClassTankValue", 40);
                _settings.AddVariable("BootsOfGridspaceDistortionOption", false);
                _settings.AddVariable("BootsofInfiniteSpeedOption", false);
                _settings.AddVariable("BurstofSpeedStimOption", false);
                _settings.AddVariable("CurseofMalahdeOption", false);
                _settings.AddVariable("PolymerizingStimValue", 40);
                _settings.AddVariable("MutatedSlitherBloodValue", 40);
                _settings.AddVariable("IskopsAscendancyOption", false);
                _settings.AddVariable("BacchantesAnunWingsValue", 40);
                _settings.AddVariable("CombatAssistWenWenValue", 40);
                _settings.AddVariable("RingofKnowledgeOption", false);
                _settings.AddVariable("BoostedStimValue", 40);
                _settings.AddVariable("KamikazeRobotShellSelection", 1);
                _settings.AddVariable("BoltarBrainBlasterValue", 40);
                _settings.AddVariable("BioremediationStimValue", 40);

                _settings.AddVariable("CloakoftheReanimatedOption", true);
                _settings.AddVariable("CoffeeOption", true);

                _settings.AddVariable("AncientGenerationDevices", 0);

                #endregion

                #region Item DeBuff

                _settings.AddVariable("BracerofBrotherMalevolenceSelection", 1);
                _settings.AddVariable("TotwBlindRingsSelection", 1);

                #endregion

                #region Pets

                _settings.AddVariable("AttackPetNCU", false);
                _settings.AddVariable("SupportPetNCU", false);

                #region Trimmers

                #region AttackPet

                _settings.AddVariable("AttackIncreaseAggressivenessCheckBox", false);

                _settings.AddVariable("AttackDmgChangeSelectionArray", 0);
                _settings.AddVariable("AttackDmgChangeSelectionArrayCheckBox", false);

                _settings.AddVariable("AttackMechEngiSelectionArray", 0);
                _settings.AddVariable("AttackMechEngiSelectionArrayCheckBox", false);

                _settings.AddVariable("AttackElecEngiSelectionArray", 0);
                _settings.AddVariable("AttackElecEngiSelectionArrayCheckBox", false);

                _settings.AddVariable("AttackAggressiveDefensiveSelectionArray", 0);
                _settings.AddVariable("AttackAggressiveDefensiveSelectionArrayCheckBox", false);

                #endregion

                #region SupportPet

                _settings.AddVariable("SupportIncreaseAggressivenessCheckBox", false);

                _settings.AddVariable("SupportDmgChangeSelectionArray", 0);
                _settings.AddVariable("SupportDmgChangeSelectionArrayCheckBox", false);

                _settings.AddVariable("SupportMechEngiSelectionArray", 0);
                _settings.AddVariable("SupportMechEngiSelectionArrayCheckBox", false);

                _settings.AddVariable("SupportElecEngiSelectionArray", 0);
                _settings.AddVariable("SupportElecEngiSelectionArrayCheckBox", false);

                _settings.AddVariable("SupportAggressiveDefensiveSelectionArray", 0);
                _settings.AddVariable("SupportAggressiveDefensiveSelectionArrayCheckBox", false);

                #endregion

                #endregion

                #endregion

                #endregion

                #region UI

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                _settings.AddVariable("BuffsTopLeftX", 50f);
                _settings.AddVariable("BuffsTopLeftY", 50f);
                _settings.AddVariable("BuffsWidth", 300f);
                _settings.AddVariable("BuffsHeight", 300f);

                _settings.AddVariable("DebuffsTopLeftX", 50f);
                _settings.AddVariable("DebuffsTopLeftY", 50f);
                _settings.AddVariable("DebuffsWidth", 300f);
                _settings.AddVariable("DebuffsHeight", 300f);

                _settings.AddVariable("HealsTopLeftX", 50f);
                _settings.AddVariable("HealsTopLeftY", 50f);
                _settings.AddVariable("HealsWidth", 300f);
                _settings.AddVariable("HealsHeight", 300f);

                _settings.AddVariable("HoldsTopLeftX", 50f);
                _settings.AddVariable("HoldsTopLeftY", 50f);
                _settings.AddVariable("HoldsWidth", 300f);
                _settings.AddVariable("HoldsHeight", 300f);

                _settings.AddVariable("NukesTopLeftX", 50f);
                _settings.AddVariable("NukesTopLeftY", 50f);
                _settings.AddVariable("NukesWidth", 300f);
                _settings.AddVariable("NukesHeight", 300f);

                _settings.AddVariable("PerksTopLeftX", 50f);
                _settings.AddVariable("PerksTopLeftY", 50f);
                _settings.AddVariable("PerksWidth", 300f);
                _settings.AddVariable("PerksHeight", 300f);

                _settings.AddVariable("PetsTopLeftX", 50f);
                _settings.AddVariable("PetsTopLeftY", 50f);
                _settings.AddVariable("PetsWidth", 300f);
                _settings.AddVariable("PetsHeight", 300f);

                _settings.AddVariable("TauntsTopLeftX", 50f);
                _settings.AddVariable("TauntsTopLeftY", 50f);
                _settings.AddVariable("TauntsWidth", 300f);
                _settings.AddVariable("TauntsHeight", 300f);

                _settings.AddVariable("MorphsTopLeftX", 50f);
                _settings.AddVariable("MorphsTopLeftY", 50f);
                _settings.AddVariable("MorphsWidth", 300f);
                _settings.AddVariable("MorphsHeight", 300f);

                _settings.AddVariable("PetComsTopLeftX", 50f);
                _settings.AddVariable("PetComsTopLeftY", 50f);
                _settings.AddVariable("PetComsWidth", 300f);
                _settings.AddVariable("PetComsHeight", 300f);

                _settings.AddVariable("ProcsTopLeftX", 50f);
                _settings.AddVariable("ProcsTopLeftY", 50f);
                _settings.AddVariable("ProcsWidth", 300f);
                _settings.AddVariable("ProcsHeight", 300f);

                _settings.AddVariable("ItemsTopLeftX", 50f);
                _settings.AddVariable("ItemsTopLeftY", 50f);
                _settings.AddVariable("ItemsWidth", 300f);
                _settings.AddVariable("ItemsHeight", 300f);

                _settings.AddVariable("InfoTopLeftX", 50f);
                _settings.AddVariable("InfoTopLeftY", 50f);
                _settings.AddVariable("InfoWidth", 300f);
                _settings.AddVariable("InfoHeight", 300f);

                _settings.AddVariable("WeaponsTopLeftX", 50f);
                _settings.AddVariable("WeaponsTopLeftY", 50f);
                _settings.AddVariable("WeaponsWidth", 300f);
                _settings.AddVariable("WeaponsHeight", 300f);

                _settings.AddVariable("FPTopLeftX", 50f);
                _settings.AddVariable("FPTopLeftY", 50f);
                _settings.AddVariable("FPWidth", 300f);
                _settings.AddVariable("FPHeight", 300f);

                #endregion

                #region Profession

                switch (DynelManager.LocalPlayer.Profession)
                {
                    #region Adventurer
                    case Profession.Adventurer:
                        _settings.AddVariable("MorphSelection", 0);
                        _settings.AddVariable("MorphSelectionCheckBox", false);

                        _settings.AddVariable("ShouldMoveBehindTarget", 0);

                        break;
                    #endregion

                    #region Agent
                    case Profession.Agent:
                        _settings.AddVariable("DeTaunt", false);

                        _settings.AddVariable("CastFP", false);
                        _settings.AddVariable("Alternate", false);
                        _settings.AddVariable("FalseProfSelection1", 0);
                        _settings.AddVariable("FalseProfSelection2", 0);
                        _settings.AddVariable("LastFP", 0);

                        _settings.AddVariable("ChaoticModulationSelection", 0);

                        #region Adv

                        _settings.AddVariable("CatDamage", false);
                        _settings.AddVariable("MorphSelection", 0);
                        _settings.AddVariable("MorphSelectionCheckBox", false);
                        #endregion

                        #region Doc

                        _settings.AddVariable("EpsilonPurge", false);
                        _settings.AddVariable("LockCH", false);

                        #endregion
                        break;
                    #endregion

                    #region Bureaucrat
                    case Profession.Bureaucrat:
                        _settings.AddVariable("Exoneration", true);

                        _settings.AddVariable("Calm12Man", false);
                        break;
                    #endregion

                    #region Doctor
                    case Profession.Doctor:
                        _settings.AddVariable("NanoTransmission", false);

                        _settings.AddVariable("EpsilonPurge", false);

                        _settings.AddVariable("LockCH", false);

                        break;
                    #endregion

                    #region Engineer
                    case Profession.Engineer:

                        break;
                    #endregion

                    #region Enforcer
                    case Profession.Enforcer:

                        break;
                    #endregion

                    #region Fixer
                    case Profession.Fixer:
                        _settings.AddVariable("ArmorSelection", 0);
                        _settings.AddVariable("BulletsSelection", 0);

                        break;
                    #endregion

                    #region Keeper
                    case Profession.Keeper:

                        break;
                    #endregion

                    #region MartialArtist
                    case Profession.MartialArtist:

                        break;
                    #endregion

                    #region Metaphysicist
                    case Profession.Metaphysicist:
                        _settings.AddVariable("SummonedWeaponSelection", 0);

                        break;
                    #endregion

                    #region NanoTechnician
                    case Profession.NanoTechnician:
                        _settings.AddVariable("DeTaunt", false);

                        break;
                    #endregion

                    #region Shade
                    case Profession.Shade:

                        _settings.AddVariable("SpiritSiphon", false);
                        _settings.AddVariable("SpiritSiphonLvl", DynelManager.LocalPlayer.Level);

                        _settings.AddVariable("SmokeBomb", false);

                        _settings.AddVariable("ShouldMoveBehindTarget", 0);

                        break;
                    #endregion

                    #region Soldier
                    case Profession.Soldier:
                        _settings.AddVariable("DeTaunt", false);
                        _settings.AddVariable("DeTaunt_If_AMS_False", false);
                        _settings.AddVariable("Print_AMS", false);
                        _settings["Print_AMS"] = false;
                        _settings.AddVariable("AMSPercentage", 0);

                        break;
                    #endregion

                    #region Trader
                    case Profession.Trader:
                        _settings["SpawnAttackPet"] = true;

                        _settings.AddVariable("PetSelection", 2);

                        break;
                        #endregion
                }

                #endregion

                settingsToSave.Add(_settings);
                _settings.Prune();

                #endregion

                IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

                bossJson = JObject.Parse(File.ReadAllText(PluginDir + "\\Bosses.json"));

                BossNames = bossJson[Playfield.ModelIdentity.Instance.ToString()] is JArray a ? a.Select(x => (string)x["Name"]).ToList() : new List<string>();

                Chat.RegisterCommand("morphs", MorphsCommand);
                Chat.RegisterCommand("rebuff", Rebuff);
                Chat.RegisterCommand("handlerchannel", ChannelCommand);
                Chat.RegisterCommand("petstats", PetStats);
                Chat.RegisterCommand("HandlerSettings", PlayerSettings);
                Chat.RegisterCommand("updatesettings", ReloadProcessors);
                Chat.RegisterCommand("HandlerEnable", (c, p, w) =>
                {
                    _settings["Enable"] = !_settings["Enable"].AsBool();
                    Chat.WriteLine($"HandlerEnable is now: {_settings["Enable"].AsBool()}");
                });
                Chat.RegisterCommand("resetui", ResetUI);

                Chat.RegisterCommand("que", (c, p, w) =>
                {
                    Chat.WriteLine($"{_actionQueue.Count} queued");

                    foreach (var rule in _actionQueue)
                    {
                        if (rule.CombatAction is Item i)
                            Chat.WriteLine($"Item {i.Name}. Timeout = {rule.Timeout}, Current Time = {Time.AONormalTime}, Timeout left = {rule.Timeout - Time.AONormalTime:F2}");

                        if (rule.CombatAction is PerkAction perk)
                            Chat.WriteLine($"Perk {perk.Name}. Timeout = {rule.Timeout}, Current Time = {Time.AONormalTime}, Timeout left = {rule.Timeout - Time.AONormalTime:F2}");
                    }
                });

                Chat.RegisterCommand("check", (c, p, w) =>
                {
                    var name = string.Join(" ", p);

                    foreach (var item in Inventory.Items.Concat(Inventory.Backpacks.Where(b => b.Name != null && b.Name.IndexOf("fannypack", StringComparison.OrdinalIgnoreCase) >= 0)
                        .SelectMany(bp => Inventory.GetContainerItems(bp.Identity))).Where(i => i.Name == name))
                    {
                        Chat.WriteLine(item.Name);

                        foreach (var prob in item.UseModifiers)
                        {
                            Chat.WriteLine($"Function {prob.Function}");

                            foreach (var kvp in prob.Properties)
                            {
                                Chat.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                            }
                        }
                    }
                });

                Chat.RegisterCommand("dumpstats", (c, p, w) =>
                {
                    var target = Targeting.Target;
                    if (target == null) return;

                    IntPtr engine = N3Engine_t.GetInstance();
                    if (engine == IntPtr.Zero) return;

                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "StatDump.txt");

                    using (var sw = new StreamWriter(filePath, true))
                    {
                        sw.WriteLine($"==== {DateTime.Now:yyyy-MM-dd HH:mm:ss} ====");
                        sw.WriteLine($"Target: {target.Name} {target.Identity}");
                        sw.WriteLine($"Local:  {DynelManager.LocalPlayer.Identity}");
                        sw.WriteLine();

                        Identity dynel = target.Identity;

                        for (int statId = 0; statId <= 0x3000; statId++)
                        {
                            var unk = default(Identity);

                            int value = N3EngineClientAnarchy_t.GetSkill(engine, ref dynel, (Stat)statId, 2, ref unk);

                            if (value == 0 || value == 6 || value == 1234567890) continue;

                            if (Enum.IsDefined(typeof(Stat), (Stat)statId))
                                sw.WriteLine($"{(Stat)statId} = {value}");
                            else
                                sw.WriteLine($"0x{statId:X4} = {value}");
                        }

                        sw.WriteLine();
                    }

                    Chat.WriteLine($"Dump complete: {filePath}");
                });

                Chat.RegisterCommand("channels", (c, p, w) =>
                {
                    foreach (var channel in ChatChannels)
                        Chat.WriteLine($"Name:{channel.Key}, ID: {channel.Value}");
                });

                Network.ChatMessageReceived += ChatMessageReceived;
                
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }
        public enum LocalGroupMessageType : byte
        {
            Org = 3,
            Team = 130,
            Raid = 143
        }

        private void ChatMessageReceived(object sender, ChatMessageBody e)
        {
            if (e.PacketType != ChatMessageType.GroupMessage) return;
            var msg = (GroupMsgMessage)e;
            if (msg.MessageType != (GroupMessageType)130 && msg.MessageType != (GroupMessageType)143) return;
            ChatChannels[(LocalGroupMessageType)(byte)msg.MessageType] = msg.ChannelId;
        }

        private void Windowclosed(object sender, Window e)
        {
            if (e.Name.Contains("PH"))
            {
                Window_Closed_helper();
            }

            switch (e.Name)
            {
                case "Buffs":
                    SaveWindowBounds("Buffs", BuffWindow.CurrentWindow);
                    Save();
                    break;

                case "Debuffs":
                    SaveWindowBounds("Debuffs", DebuffWindow.CurrentWindow);
                    Save();
                    break;

                case "Heals":
                    SaveWindowBounds("Heals", HealWindow.CurrentWindow);
                    Save();
                    break;

                case "Holds":
                    SaveWindowBounds("Holds", HoldWindow.CurrentWindow);
                    Save();
                    break;

                case "Items":
                    SaveWindowBounds("Items", ItemWindow.CurrentWindow);
                    Save();
                    break;

                case "MA Attacks":
                    SaveWindowBounds("MA Attacks", MAAttackWindow.CurrentWindow);
                    Save();
                    break;

                case "Nukes":
                    SaveWindowBounds("Nukes", NukeWindow.CurrentWindow);
                    Save();
                    break;

                case "Perks":
                    SaveWindowBounds("Perks", PerkWindow.CurrentWindow);
                    Save();
                    break;

                case "Pets":
                    SaveWindowBounds("Pets", PetWindow.CurrentWindow);
                    Save();
                    break;

                case "Taunts":
                    SaveWindowBounds("Taunts", TauntWindow.CurrentWindow);
                    Save();
                    break;

                case "Trimmers":
                    SaveWindowBounds("Trimmers", TrimmerWindow.CurrentWindow);
                    Save();
                    break;

                case "Morphs":
                    SaveWindowBounds("Morphs", _morphWindow);
                    Save();
                    break;

                case "PetComs":
                    SaveWindowBounds("PetComs", _petCommandWindow);
                    Save();
                    break;

                case "Procs":
                    SaveWindowBounds("Procs", _procWindow);
                    Save();
                    break;

                case "Info":
                    SaveWindowBounds("Info", _infoWindow);
                    Save();
                    break;

                case "Weapons":
                    SaveWindowBounds("Weapons", _weaponWindow);
                    Save();
                    break;

                case "FP":
                    SaveWindowBounds("FP", _falseProfWindow);
                    Save();
                    break;
            }
        }

        private void WaitForFannypacks(object s, float dt)
        {
            foreach (var bag in Inventory.Backpacks.Where(b => b.Name != null && b.Name.IndexOf("fannypack", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                var items = Inventory.GetContainerItems(bag.Identity);
                if (items.Count == 0) return;

                var bagAsItem = Inventory.Items.FirstOrDefault(i => i.UniqueIdentity.Instance == bag.Identity.Instance);
                bagAsItem?.Use();
            }

            Game.OnUpdate -= WaitForFannypacks;

            RegisterPerkProcessors();
            RegisterItemProcessors();
            RegisterSpellProcessors();

            Network.N3MessageReceived += Network_N3MessageReceived;
            Network.N3MessageSent += N3MessageSent;
            Game.TeleportStarted += TeleportStarted;
            Game.TeleportEnded += TeleportEnded;
            Game.PlayfieldInit += PlayfieldInit;
            UIController.WindowDeleted += Windowclosed;
            Game.OnUpdate += Update;
            DynelManager.DynelSpawned += DynelSpawned;
        }

        private void RegisterProcessors()
        {
            _itemRules.Clear();
            _perkRules.Clear();
            _spellRules.Clear();

            LoadedTaunts.Clear();
            LoadedCombatBuffs.Clear();
            LoadedNonCombatBuffs.Clear();
            LoadedDebuffRemoverSpells.Clear();
            LoadedDeBuffs.Clear();
            LoadedHolds.Clear();
            LoadedHeals.Clear();
            LoadedNukeSpells.Clear();
            LoadedPetBuffSpells.Clear();

            LoadedPetSpells[0] = false;
            LoadedPetSpells[1] = false;
            LoadedPetSpells[2] = false;

            OwnedItems.Clear();
            OwnedPerks.Clear();
            OwnedProcs.Clear();
            OwnedTrimmers.Clear();

            RegisterPerkProcessors();
            RegisterItemProcessors();
            RegisterSpellProcessors();
        }

        private void N3MessageSent(object sender, N3Message e)
        {
            switch (e.N3MessageType)
            {
                case N3MessageType.CharacterAction:
                    var charAction = (CharacterActionMessage)e;
                    if (charAction.Action == CharacterActionType.Logout)
                    {
                        Save();

                        Network.N3MessageSent -= N3MessageSent;
                        UIController.WindowDeleted -= Windowclosed;
                        Network.ChatMessageReceived -= ChatMessageReceived;
                        return;
                    }
                    break;
                case N3MessageType.PetCommand:
                    var petMsg = (PetCommandMessage)e;

                    foreach (var pet in petMsg.Pets)
                    {
                        if (!CurrentPetCommand.ContainsKey(pet.Identity.Instance))
                            CurrentPetCommand.Add(pet.Identity.Instance, petMsg.Command);
                        else
                            CurrentPetCommand[pet.Identity.Instance] = petMsg.Command;
                    }
                    break;
            }
        }

        enum AttackState { None, Attack, Stop, Follow, Zone }

        private void Network_N3MessageReceived(object sender, N3Message e)
        {
            var localPlayer = DynelManager.LocalPlayer;
            var attackPet = localPlayer.Pets.FirstOrDefault(a => a.Type == PetType.Attack);
            var supportPet = localPlayer.Pets.FirstOrDefault(a => a.Type == PetType.Support);

            switch (e.N3MessageType)
            {
                case N3MessageType.AttackInfo:
                    var attackInfoMessage = (AttackInfoMessage)e;
                    lastAttackInfoMessage = attackInfoMessage;
                    break;
                case N3MessageType.Attack:
                    var attack = (AttackMessage)e;
                    if (attack.Identity != localPlayer.Identity) return;
                    if (_settings["SyncPets"] == null || !_settings["SyncPets"].AsBool()) return;
                    state = AttackState.Attack;
                    break;
                case N3MessageType.StopFight:
                    var stop = (StopFightMessage)e;
                    if (stop.Identity != localPlayer.Identity) return;

                    if (state == AttackState.Stop) return;
                    attackPet?.Follow();
                    supportPet?.Follow();
                    state = AttackState.Stop;
                    break;
                case N3MessageType.Buff:
                    var buff = (BuffMessage)e;
                    var coccoon = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && (c.Name == "Alien Coccoon" || c.Name == "Alien Cocoon"));

                    if (localPlayer.Buffs.Contains(NanoLine.Cocoon) && coccoon != null)
                        attackPet?.Attack(coccoon.Identity);
                    break;
                case N3MessageType.CastNanoSpell:
                    var castNanoSpell = (CastNanoSpellMessage)e;

                    if (_settings["Print_AMS"] != null && _settings["Print_AMS"].AsBool())
                    {
                        if (castNanoSpell.Identity != localPlayer.Identity) return;
                        if (SpellID.UserReflectShieldSoldier.Contains(castNanoSpell.NanoId))
                        {
                            AMSPrintStop = 1;
                            SendChannelMsg((GroupMessageType)ChatChannelType, ChatChannelId, "AMS Casted");
                        }
                    }

                    if (ProcType1Map.Any(s => s.Value.Item2 == castNanoSpell.NanoId))
                    {
                        var ProcType1MatchingEntry = ProcType1Map.FirstOrDefault(x => x.Value.Item2 == castNanoSpell.NanoId);

                        if (_settings["ProcType1Selection1"].AsInt32() == (int)ProcType1MatchingEntry.Key)
                            type1 = true;

                        else if (_settings["ProcType1Selection2"].AsInt32() == (int)ProcType1MatchingEntry.Key)
                            type1 = false;
                    }
                    else if (ProcType2Map.Any(s => s.Value.Item2 == castNanoSpell.NanoId))
                    {
                        var ProcType2MatchingEntry = ProcType2Map.FirstOrDefault(x => x.Value.Item2 == castNanoSpell.NanoId);

                        if (_settings["ProcType2Selection1"].AsInt32() == (int)ProcType2MatchingEntry.Key)
                            type2 = true;
                        else if (_settings["ProcType2Selection2"].AsInt32() == (int)ProcType2MatchingEntry.Key)
                            type2 = false;
                    }
                    break;
            }
        }

        private void TeleportStarted(object sender, EventArgs e)
        {
            _lastZonedTime = Now + 300.0;
        }

        private void TeleportEnded(object sender, EventArgs e)
        {
            _lastZonedTime = Now + 1.0;
            _lastCombatTime = 0.0;
            state = AttackState.Zone;

            InitializeFannyPack();

            if (Team.IsInTeam)
                foreach (var member in Team.Members)
                {
                    if (member.Character != null)
                        Targeting.SetTarget(member.Identity);
                }
            //UpdatePets();
        }

        private void PlayfieldInit(object sender, uint e)
        {
            CurrentHealTarget = Identity.None;
            _lastZonedTime = Now + 1.0;
            _lastCombatTime = 0.0;
            BossNames = bossJson[Playfield.ModelIdentity.Instance.ToString()] is JArray a ? a.Select(x => (string)x["Name"]).ToList() : new List<string>();

            InitializeFannyPack();
        }

        private void DynelSpawned(object sender, Dynel e)
        {
            if (Game.IsZoning) return;
            if (Now < _lastZonedTime) return;

            if (Team.IsInTeam)
            {
                if (Team.Members.Any(m => m.Character != null && m.Identity == e.Identity && m.Identity != DynelManager.LocalPlayer.Identity))
                    Targeting.SetTarget(e.Identity);
            }
        }

        //private void UpdatePets()
        //{
        //    Network.N3MessageReceived += handler;

        //    void handler(object s, N3Message m)
        //    {
        //        switch (m.N3MessageType)
        //        {
        //            case N3MessageType.SimpleCharFullUpdate:
        //                {
        //                    var update = (SimpleCharFullUpdateMessage)m;

        //                    if (update.Owner == DynelManager.LocalPlayer.Identity)
        //                    {
        //                        if (!Pets.Any(p => p.Identity == update.Identity))
        //                            Pets.Add((update.Identity, update.Name, 0));

        //                        Network.Send(new PetCommandMessage() { Command = PetCommand.Report });
        //                        Network.N3MessageReceived -= handler;
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //}

        protected override void OnUpdate(float deltaTime)
        {
            try
            {
                if (Game.IsZoning) return;

                if (Now < _lastZonedTime) return;

                CancelHostileAuras();

                UseItems();
                Ammo.CrateOfAmmo();

                if ((Profession)DynelManager.LocalPlayer.GetStat(Stat.VisualProfession) == Profession.Adventurer)
                    Morphs();

                switch (state)
                {
                    case AttackState.Zone:
                        foreach (var pet in DynelManager.LocalPlayer.Pets)
                            pet?.Follow();
                        state = AttackState.Follow;
                        break;
                }

                if (DynelManager.LocalPlayer.GetStat(Stat.VisualProfession) == (int)Profession.Doctor)
                    if (_settings["LockCH"]?.AsBool() == true && Spell.Find(SpellID._alphaAndOmega, out Spell ch))
                    {
                        if (ch.IsReady)
                            switch (Playfield.ModelIdentity.Instance)
                            {
                                case 6015: if (DynelManager.NPCs.Any(c => c.IsAlive && c.Name == "Deranged Xan")) ch.Cast(DynelManager.LocalPlayer, true); break;
                                case 8020: if (DynelManager.NPCs.Any(c => c.IsAlive && (c.Name == "The Maiden" || c.Name == "Azdaja the Joyous"))) ch.Cast(DynelManager.LocalPlayer, true); break;
                                case 9070: if (DynelManager.LocalPlayer.Room.Name == "Shopping Dead-end" && DynelManager.NPCs.Any(c => c.IsAlive && c.Name == "Eumenides")) ch.Cast(DynelManager.LocalPlayer, true); break;
                            }
                    }

                var lp = DynelManager.LocalPlayer;

                if (lp.IsAttacking && lp.FightingTarget != null)
                {
                    if (lp.FightingTarget.IsPlayer)
                    {
                        lp.StopAttack(false);
                        return;
                    }

                    if (FightTimeStamp == 0)
                        FightTimeStamp = Time.AONormalTime;

                    PerformSpecialAttacks(lp.FightingTarget);

                    if (_settings["SyncPets"] == null || _settings["SyncPets"].AsBool())
                        SyncPetAttacking(lp.FightingTarget);

                    _lastCombatTime = Time.AONormalTime;
                }
                else
                    FightTimeStamp = 0;

                Attacked = DynelManager.NPCs.Any(n => n.IsAttacking && n.FightingTarget != null && n.FightingTarget.Identity == lp.Identity);

                base.OnUpdate(deltaTime);
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Checks

        public bool IsPlayerFlyingOrFalling()
        {
            var localPlayer = DynelManager.LocalPlayer;

            return localPlayer.Buffs.Contains(NanoLine.Vehicles) || localPlayer.MovementState == MovementState.Fly || localPlayer.IsFalling || DynelManager.LocalPlayer.Buffs.Contains(SpellID.Hoverboards);
        }

        public static void CancelLocalBuffs()
        {
            foreach (Buff buff in DynelManager.LocalPlayer.Buffs)
            {
                if (Spell.List.Any(s => s.Id == buff.Id))
                    buff.Remove();
            }
        }

        public static void CancelAllBuffs()
        {
            foreach (Buff buff in DynelManager.LocalPlayer.Buffs)
                buff.Remove();
        }

        public static void CancelBuffs(int[] buffsToCancel)
        {
            foreach (Buff buff in DynelManager.LocalPlayer.Buffs)
            {
                if (buffsToCancel.Contains(buff.Id))
                    buff.Remove();
            }
        }

        protected void CancelHostileAuras()
        {
            switch ((Profession)DynelManager.LocalPlayer.GetStat(Stat.VisualProfession))
            {
                case Profession.Bureaucrat:
                    if ((_settings["DemotivationalSpeechesOption"].AsInt32() != 3 && Time.AONormalTime > _lastCombatTime + 5) || !_settings["AOE"].AsBool() || PVPDistance(11))
                        CancelBuffs(Spell.GetSpellsForNanoline(NanoLine.DemotivationalSpeeches).Select(s => s.Id).ToArray());
                    break;
                case Profession.Engineer:
                    if ((_settings["EngineerDebuffAurasOption"].AsInt32() != 3 && Time.AONormalTime > _lastCombatTime + 5) || !_settings["AOE"].AsBool() || PVPDistance(11))
                        CancelBuffs(Spell.GetSpellsForNanoline(NanoLine.EngineerDebuffAuras).Select(s => s.Id).ToArray());
                    break;
            }
        }

        protected bool IsInsideInnerSanctum()
        {
            if (Game.IsZoning) return false;
            return DynelManager.LocalPlayer.Buffs.Any(buff => buff.Id == SpellID.InnerSanctumDebuff);
        }

        public bool AttackingMob(SimpleChar mob)
        {
            if (Game.IsZoning) return false;
            if (Team.IsInTeam)
                return Team.Members.Any(c => c.Character != null && c.Character.FightingTarget?.Identity == mob.Identity);

            return DynelManager.LocalPlayer.FightingTarget?.Identity == mob.Identity;
        }

        public bool AttackingTeam(SimpleChar mob)
        {
            if (Game.IsZoning || mob.FightingTarget == null)
                return false;

            var target = mob.FightingTarget;

            if (Team.IsInTeam)
                return Team.Members.Any(m => m.Identity == target.Identity || (target.IsPet && target.PetOwnerId == m.Identity.Instance));

            return target.Identity == DynelManager.LocalPlayer.Identity ||
                   (target.IsPet && target.PetOwnerId == DynelManager.LocalPlayer.Identity.Instance);
        }

        public static bool InCombat()
        {
            if (Game.IsZoning) return false;
            var localPlayer = DynelManager.LocalPlayer;

            if (Team.IsInTeam)
            {
                return Team.Members.Any(member => member.Character != null && member.Character.IsAttacking)
                    || DynelManager.NPCs.Any(mob => mob.FightingTarget != null && Team.Members.Contains(mob.FightingTarget.Identity));
            }

            return localPlayer.IsAttacking || (localPlayer.Pets != null && localPlayer.Pets.Any(pet => pet.Character != null && pet.Character.IsAttacking)) ||
                   DynelManager.NPCs.Any(npc => npc.FightingTarget != null && (npc.FightingTarget.Identity == localPlayer.Identity ||
                                                 (localPlayer.Pets != null && localPlayer.Pets.Any(pet => pet.Character != null && npc.FightingTarget.Identity == pet.Character.Identity))));
        }

        public static CharacterWieldedWeapon GetWieldedWeapons(SimpleChar local) => (CharacterWieldedWeapon)local.GetStat(Stat.EquippedWeapons);

        #endregion

        #region Chat Commands

        private void ReloadProcessors(string arg1, string[] arg2, ChatWindow window)
        {
            RegisterProcessors();
        }

        private void ChannelCommand(string arg1, string[] arg2, ChatWindow window)
        {
            if (arg2 == null || arg2.Length == 0)
            {
                Chat.WriteLine($"Current IPC Channel: {_settings["IPCChannel"].AsInt32()}");
                return;
            }

            if (int.TryParse(arg2[0], out int newChannel))
            {
                if (newChannel < 1 || newChannel > 255)
                {
                    Chat.WriteLine("Invalid channel. Please enter a number between 1 and 255.");
                    return;
                }

                _settings["IPCChannel"] = newChannel;
                IPCChannel.SetChannelId(Convert.ToByte(_settings["IPCChannel"].AsInt32()));
                Chat.WriteLine($"IPC Channel set to: {_settings["IPCChannel"].AsInt32()}");
                Save();
            }
            else
                Chat.WriteLine("Invalid input. Please enter a number between 1 and 255.");
        }

        private void PetStats(string arg1, string[] arg2, ChatWindow window)
        {
            foreach (var pet in DynelManager.LocalPlayer.Pets)
            {
                switch (pet.Type)
                {
                    case PetType.Attack:
                        petColor = (int)ChatColor.Red;
                        break;
                    case PetType.Heal:
                        petColor = (int)ChatColor.LightBlue;
                        break;
                    case PetType.Support:
                        petColor = (int)ChatColor.Green;
                        break;
                    case PetType.Social:
                        petColor = (int)ChatColor.Yellow;
                        break;
                    default:
                        petColor = (int)ChatColor.White;
                        break;
                }

                var petChar = pet.Character;

                Chat.WriteLine($"{petChar.Name} lvl {petChar.Level} type {pet.Type}", (ChatColor)petColor);
                Chat.WriteLine("--------------------");
                Chat.WriteLine($"AMS = {petChar.GetStat(Stat.AMS)}", (ChatColor)petColor);
                Chat.WriteLine($"AggDef = {petChar.GetStat(Stat.AggDef)}", (ChatColor)petColor);
                Chat.WriteLine($"Aggressiveness = {petChar.GetStat(Stat.Aggressiveness)}", (ChatColor)petColor);
                Chat.WriteLine($"AddAllOff = {petChar.GetStat(Stat.AddAllOff)}", (ChatColor)petColor);
                Chat.WriteLine($"Taunt = {petChar.GetStat(Stat.Taunt)}", (ChatColor)petColor);
                Chat.WriteLine("--------------------");
                Chat.WriteLine($"MaxNCU = {petChar.GetStat(Stat.MaxNCU)}", (ChatColor)petColor);
                Chat.WriteLine($"CurrentNCU = {petChar.GetStat(Stat.CurrentNCU)}", (ChatColor)petColor);
                Chat.WriteLine($"QuantumFT = {petChar.GetStat(Stat.QuantumFT)}", (ChatColor)petColor);
                Chat.WriteLine("--------------------");
                Chat.WriteLine($"FireDamageModifier = {petChar.GetStat(Stat.FireDamageModifier)}", (ChatColor)petColor);
                Chat.WriteLine($"EnergyDamageModifier = {petChar.GetStat(Stat.EnergyDamageModifier)}", (ChatColor)petColor);
                Chat.WriteLine($"ColdDamageModifier) = {petChar.GetStat(Stat.ColdDamageModifier)}", (ChatColor)petColor);
                Chat.WriteLine("--------------------");
                Chat.WriteLine($"MaxHealth = {petChar.GetStat(Stat.MaxHealth)}", (ChatColor)petColor);
                Chat.WriteLine($"EvadeClsC = {petChar.GetStat(Stat.EvadeClsC)}", (ChatColor)petColor);
                Chat.WriteLine($"DodgeRanged = {petChar.GetStat(Stat.DodgeRanged)}", (ChatColor)petColor);
                Chat.WriteLine($"NanoResist = {petChar.GetStat(Stat.NanoResist)}", (ChatColor)petColor);

                Chat.WriteLine("--------------------");
                Chat.WriteLine($"CriticalIncrease = {petChar.GetStat(Stat.CriticalIncrease)}", (ChatColor)petColor);
                Chat.WriteLine($"MeleeInit = {petChar.GetStat(Stat.MeleeInit)}", (ChatColor)petColor);
                Chat.WriteLine($"NanoCInit = {petChar.GetStat(Stat.NanoCInit)}", (ChatColor)petColor);
                Chat.WriteLine($"PhysicalInit = {petChar.GetStat(Stat.PhysicalInit)}", (ChatColor)petColor);
                Chat.WriteLine("--------------------");

                int npcFamily = petChar.GetStat(Stat.NPCFamily);
                string npcFamilyName;

                if (Enum.IsDefined(typeof(NpcFamily), npcFamily))
                    npcFamilyName = ((NpcFamily)npcFamily).ToString();
                else if (Enum.IsDefined(typeof(NpcClan), npcFamily))
                    npcFamilyName = ((NpcClan)npcFamily).ToString();
                else
                    npcFamilyName = "Unmapped";
                Chat.WriteLine($"NPCFamily = {npcFamily} ({npcFamilyName})", (ChatColor)petColor);

                Breed breed = petChar.Breed;
                Chat.WriteLine($"Breed = {breed} ({(int)breed})", (ChatColor)petColor);
                Chat.WriteLine("--------------------");
                int wielded = petChar.GetStat(Stat.EquippedWeapons);
                Chat.WriteLine($"EquippedWeapons = {(CharacterWieldedWeapon)wielded} ({wielded})", (ChatColor)petColor);

                Chat.WriteLine("-- Melee Skills --", (ChatColor)petColor);
                Chat.WriteLine($"MartialArts = {petChar.GetStat(Stat.MartialArts)}", (ChatColor)petColor);
                Chat.WriteLine($"1h Blunt = {petChar.GetStat(Stat._1hBlunt)}", (ChatColor)petColor);
                Chat.WriteLine($"1h Edged = {petChar.GetStat(Stat._1hEdged)}", (ChatColor)petColor);
                Chat.WriteLine($"2h Blunt = {petChar.GetStat(Stat._2hBlunt)}", (ChatColor)petColor);
                Chat.WriteLine($"2h Edged = {petChar.GetStat(Stat.Skill2hEdged)}", (ChatColor)petColor);
                Chat.WriteLine($"Piercing = {petChar.GetStat(Stat.Piercing)}", (ChatColor)petColor);
                Chat.WriteLine($"MeleeEnergy = {petChar.GetStat(Stat.MeleeEnergy)}", (ChatColor)petColor);
                Chat.WriteLine("--------------------");

                Chat.WriteLine("-- Specials --", (ChatColor)petColor);
                Chat.WriteLine($"Brawl = {petChar.GetStat(Stat.Brawl)}", (ChatColor)petColor);
                Chat.WriteLine($"Dimach = {petChar.GetStat(Stat.Dimach)}", (ChatColor)petColor);
                Chat.WriteLine($"FastAttack = {petChar.GetStat(Stat.FastAttack)}", (ChatColor)petColor);
                Chat.WriteLine("--------------------");
            }
        }

        private void PlayerSettings(string arg1, string[] arg2, ChatWindow window)
        {
            string path = System.IO.Path.Combine(Preferences.GetCharacterPath(), "AOSharp");

            if (Directory.Exists(path))
            {
                string normalizedPath = path.Replace('/', '\\'); // ensure backslashes
                System.Diagnostics.Process.Start("explorer.exe", $"\"{normalizedPath}\"");
            }
            else
                Chat.WriteLine($"HandlerSettings folder not found: {path}");
        }

        #endregion

        #region Misc

        public static void SendChannelMsg(GroupMessageType msgType, int channelId, string text)
        {
            Network.Send(new GroupMsgMessage
            {
                MessageType = msgType,
                ChannelId = channelId,
                SenderId = (uint)Game.ClientInst,
                Text = text,
            });
        }

        public static readonly List<(string Name, int Value)> Priorities = new List<(string, int)> { ("High", 40), ("Medium", 50), ("Low", 60) };

        public static bool PVPDistance(int distance)
        {
            return DynelManager.Players.Any(p => p != null && DynelManager.LocalPlayer.Position.Distance2DFrom(p.Position) <= distance && p.Buffs.Contains(SpellID.PVPEnabled));
        }

        private void Rebuff(string command, string[] param, ChatWindow chatWindow)
        {
            try
            {
                if (param.Length < 1)
                {
                    CancelLocalBuffs();
                    IPCChannel.Broadcast(new ClearBuffsMessage() { Type = 0 });
                    return;
                }

                switch (param[0].ToLower())
                {
                    case "all":
                        CancelAllBuffs();
                        IPCChannel.Broadcast(new ClearBuffsMessage() { Type = 1 });
                        break;
                    default:
                        Chat.WriteLine("/rebuff - Removes only buffs the local player can cast.", ChatColor.LightBlue);
                        Chat.WriteLine("/rebuff all - Removes all buffs.", ChatColor.LightBlue);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public bool NeedsReload()
        {
            if (Game.IsZoning) return false;

            if (lastAttackInfoMessage == null) return false;

            return DynelManager.LocalPlayer.Weapons.Any(w => w.Value.GetStat(Stat.RangedInit) > 0 && lastAttackInfoMessage.AmmoCount == 0);
        }

        private void SyncPetAttacking(SimpleChar fightingTarget)
        {

            if (Time.AONormalTime > petAttackDelay)
            {
                var localPlayer = DynelManager.LocalPlayer;
                var attackPet = localPlayer.Pets.FirstOrDefault(a => a.Type == PetType.Attack);
                var supportPet = localPlayer.Pets.FirstOrDefault(a => a.Type == PetType.Support);

                switch (localPlayer.Profession)
                {
                    case Profession.Metaphysicist:
                    case Profession.Agent:
                        if (attackPet?.Character != null && !attackPet.Character.IsAttacking && attackPet.Character.Velocity == 0)
                            attackPet?.Attack(fightingTarget.Identity);
                        if (_settings["PetMezzingOption"].AsInt32() == 1)
                        {
                            if (supportPet?.Character != null && !supportPet.Character.IsAttacking && supportPet.Character.Velocity == 0)
                                supportPet?.Attack(fightingTarget.Identity);
                        }
                        break;
                    case Profession.Bureaucrat:
                    case Profession.Engineer:
                        if (attackPet?.Character != null && !attackPet.Character.IsAttacking && attackPet.Character.Velocity == 0)
                            attackPet?.Attack(fightingTarget.Identity);

                        if (supportPet?.Character != null && !supportPet.Character.IsAttacking && supportPet.Character.Velocity == 0)
                            supportPet?.Attack(fightingTarget.Identity);
                        break;
                }
                petAttackDelay = Time.AONormalTime + 0.2;
            }
        }

        [Flags]
        public enum CharacterWieldedWeapon
        {
            Fists = 0x1,
            MartialArts = 0x3,
            Melee = 0x2,
            Ranged = 0x4,
            Bow = 0x8,
            Smg = 0x10,
            Edged1H = 0x20,
            Blunt1H = 0x40,
            Edged2H = 0x80,
            Blunt2H = 0x100,
            Piercing = 0x200,
            Pistol = 0x400,
            AssaultRifle = 0x800,
            Rifle = 0x1000,
            Shotgun = 0x2000,
            Energy = 0x4000,
            Grenade = 0x8000,
            Grenade2 = 0x20000,
            HeavyWeapons = 0x40000
        }

        public class PetSpellData
        {
            public int ShellId;
            public int ShellId2;
            public PetType PetType;

            public PetSpellData(int shellId, PetType petType)
            {
                ShellId = shellId;
                PetType = petType;
            }
            public PetSpellData(int shellId, int shellId2, PetType petType)
            {
                ShellId = shellId;
                ShellId2 = shellId2;
                PetType = petType;
            }
        }

        public void Save()
        {
            try
            {
                RegisterProcessors();

                settingsToSave.ForEach(HandlerSettings => HandlerSettings.Save());
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public void Send_Pets_Wait()
        {
            if (DynelManager.LocalPlayer.Pets.Length > 0)
            {
                foreach (Pet pet in DynelManager.LocalPlayer.Pets)
                    pet.Wait();
            }
        }

        public void Send_Pets_Follow()
        {
            if (DynelManager.LocalPlayer.Pets.Length > 0)
            {
                foreach (Pet pet in DynelManager.LocalPlayer.Pets)
                    pet.Follow();
            }
        }

        public void Warp_Pets()
        {
            if (DynelManager.LocalPlayer.Pets.Length > 0)
            {
                Spell warp = Spell.List.FirstOrDefault(x => SpellID.PetWarps.Contains(x.Id));

                warp?.Cast(DynelManager.LocalPlayer, false);
            }
        }

        private void InitializeFannyPack()
        {
            foreach (var bag in Inventory.Backpacks.Where(b => b.Name != null && b.Name.IndexOf("fannypack", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                if (bag.Items.Count > 0) continue;
                var bagAsItem = Inventory.Items.FirstOrDefault(i => i.UniqueIdentity.Instance == bag.Identity.Instance);
                bagAsItem?.Use();
                bagAsItem?.Use();
            }
        }

        #endregion

        #region UI

        private void MorphsCommand(string arg1, string[] arg2, ChatWindow window)
        {
            Morphs_Window_Helper();
        }

        public void HandleMorphViewClick(object s, ButtonBase button)
        {
            Morphs_Window_Helper();
        }

        private void Morphs_Window_Helper()
        {
            if (_morphWindow?.IsValid == true)
            {
                SaveWindowBounds("Morphs", _morphWindow);
                _morphWindow.Close();
                _morphWindow = null;
                return;
            }

            _morphWindow = Window.CreateFromXml("Morphs", PluginDir + "\\UI\\HandlerMainWindow\\AdvMorphView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _morphWindow.MoveTo(_settings["MorphsTopLeftX"].AsFloat(), _settings["MorphsTopLeftY"].AsFloat());
            _morphWindow.ResizeTo(new Vector2(_settings["MorphsWidth"].AsFloat(), _settings["MorphsHeight"].AsFloat()));

            _morphWindow.Show(true);
        }

        public void Window_Closed_helper()
        {
            if (_mainWindow?.IsValid == true)
            {
                Rect frame = _mainWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }

        public void SaveWindowBounds(string title, Window _window)
        {
            if (_window?.IsValid != true) return;
            //Chat.WriteLine($"{title} saved");
            var frame = _window.GetFrame();
            _settings[$"{title}TopLeftX"] = frame.MinX;
            _settings[$"{title}TopLeftY"] = frame.MinY;
            _settings[$"{title}Width"] = frame.MaxX - frame.MinX;
            _settings[$"{title}Height"] = frame.MaxY - frame.MinY;
        }

        public Window Generic_Button_Clicked(Window _window, string windowName, string xmlPath)
        {
            try
            {
                if (_window?.IsValid == true)
                {
                    SaveWindowBounds(windowName, _window);
                    _window.Close();
                    return null;
                }

                _window = Window.CreateFromXml(windowName, xmlPath, windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                _window.MoveTo(_settings[windowName + "TopLeftX"].AsFloat(), _settings[windowName + "TopLeftY"].AsFloat());
                _window.ResizeTo(new Vector2(_settings[windowName + "Width"].AsFloat(), _settings[windowName + "Height"].AsFloat()));

                _window.Show(true);
                return _window;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return null;
            }
        }

        public void BuildDynamicButtons(View dynamicButtonsView)
        {
            dynamicButtonsView.DeleteAllChildren();

            _buttonDefinitions = _buttonDefinitions.OrderBy(x => x.Label).ToList();

            int index = 0;
            int count = _buttonDefinitions.Count;

            while (index < count)
            {
                if (index + 1 < count)
                {
                    var row = View.CreateFromXml(PluginDir + "\\UI\\HandlerMainWindow\\DynamicButtons\\DualButtonRow.xml");
                    if (row == null) break;

                    if (row.FindChild("ButtonLeft", out Button buttonLeft))
                    {
                        buttonLeft.SetLabel(_buttonDefinitions[index].Label);
                        buttonLeft.Clicked = _buttonDefinitions[index].Handler;
                    }

                    if (row.FindChild("ButtonRight", out Button buttonRight))
                    {
                        buttonRight.SetLabel(_buttonDefinitions[index + 1].Label);
                        buttonRight.Clicked = _buttonDefinitions[index + 1].Handler;
                    }

                    dynamicButtonsView.AddChild(row, false);
                    index += 2;
                }
                else
                {
                    var row = View.CreateFromXml(PluginDir + "\\UI\\HandlerMainWindow\\DynamicButtons\\SingleButtonRow.xml");
                    if (row == null) break;

                    if (row.FindChild("ButtonCenter", out Button buttonCenter))
                    {
                        buttonCenter.SetLabel(_buttonDefinitions[index].Label);
                        buttonCenter.Clicked = _buttonDefinitions[index].Handler;
                    }

                    dynamicButtonsView.AddChild(row, false);
                    index++;
                }
            }
        }

        public static void ErrorCatch(Exception ex)
        {
            var output = ex.Message + Environment.NewLine + "   at " + ex.TargetSite?.DeclaringType?.FullName + "." + ex.TargetSite?.Name + Environment.NewLine + "   at " + ex.StackTrace;

            if (!ErrorMessages.Contains(output))
                ErrorMessages.Add(output);

            if (_mainWindow != null && _mainWindow.IsValid && _mainWindow.FindView("Errors", out View errorView))
                PopulateErrorView(errorView);
        }

        public static void PopulateErrorView(View errorView)
        {
            errorView.DeleteAllChildren();

            if (ErrorMessages != null && ErrorMessages.Count > 0)
            {
                foreach (var error in ErrorMessages)
                {
                    var parts = error.Split(new[] { "   at " }, StringSplitOptions.None);

                    View xmlRoot = View.CreateFromXml($"{PluginDir}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                    xmlRoot.FindChild("TextLabel", out TextView labelView);
                    labelView.Text = parts[0];
                    labelView.SetColor(Color.Red);
                    errorView.AddChild(xmlRoot, true);

                    if (parts.Length > 1)
                    {
                        View methodRoot = View.CreateFromXml($"{PluginDir}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                        methodRoot.FindChild("TextLabel", out TextView methodLabel);
                        methodLabel.Text = "at " + parts[1];
                        errorView.AddChild(methodRoot, true);
                    }

                    if (parts.Length > 2)
                    {
                        View traceRoot = View.CreateFromXml($"{PluginDir}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                        traceRoot.FindChild("TextLabel", out TextView traceLabel);
                        traceLabel.Text = "at " + parts[2];
                        errorView.AddChild(traceRoot, true);
                    }
                }
            }
        }

        public void Sync_Pets_Toggle()
        {
            _settings["SyncPets"] = !_settings["SyncPets"].AsBool();
            SyncPetsString = _settings["SyncPets"].AsBool() ? "SyncPets Disable" : "SyncPets Enable";

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("PetSyncButton", out Button SyncPetsButton))
                SyncPetsButton.SetLabel(SyncPetsString);

            if (_settings["SyncPets"].AsBool())
                Chat.WriteLine("SyncPets enabled.");
            else
                Chat.WriteLine("SyncPets disabled");

            Save();
        }

        #endregion
    }
}