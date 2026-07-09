using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public static HashSet<int> OwnedProcs = new HashSet<int>();

        public static HashSet<int> OwnedProcsType1 = new HashSet<int>();
        public static HashSet<int> OwnedProcsType2 = new HashSet<int>();

        bool type1 = false;
        bool type2 = false;

        #region LE Proc Bools

        protected bool LEProc1(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!CanUsePerk(perkAction)) return false;

                var lp = DynelManager.LocalPlayer;

                if (!type1)
                {
                    if (_settings["ProcType1Selection1"].AsInt32() == (int)perkAction.Hash)
                    {
                        if (!lp.Buffs.Contains(NanoLine.ResearchAbility1)) return true;
                        if (lp.Buffs.FirstOrDefault(b => b.Nanoline == NanoLine.ResearchAbility1) is Buff a1 && !string.Equals(a1.Name, perkAction.Name, StringComparison.OrdinalIgnoreCase)) return true;
                    }
                }
                else if (_settings["ProcType1Selection2"].AsInt32() == (int)perkAction.Hash)
                {
                    if (!lp.Buffs.Contains(NanoLine.ResearchAbility1)) return true;
                    if (lp.Buffs.FirstOrDefault(b => b.Nanoline == NanoLine.ResearchAbility1) is Buff a2 && !string.Equals(a2.Name, perkAction.Name, StringComparison.OrdinalIgnoreCase)) return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }
        protected bool LEProc2(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!CanUsePerk(perkAction)) return false;

                var lp = DynelManager.LocalPlayer;

                if (!type2)
                {
                    if (_settings["ProcType2Selection1"].AsInt32() == (int)perkAction.Hash)
                    {
                        if (!lp.Buffs.Contains(NanoLine.ResearchAbility2)) return true;
                        if (lp.Buffs.FirstOrDefault(b => b.Nanoline == NanoLine.ResearchAbility2) is Buff a1 && a1.Name != perkAction.Name) return true;
                    }
                }
                else if (_settings["ProcType2Selection2"].AsInt32() == (int)perkAction.Hash)
                {
                    if (!lp.Buffs.Contains(NanoLine.ResearchAbility2)) return true;
                    if (lp.Buffs.FirstOrDefault(b => b.Nanoline == NanoLine.ResearchAbility2) is Buff a2 && a2.Name != perkAction.Name) return true;
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

        #region Map

        Dictionary<PerkHash, (int, int)> ProcType1Map = new Dictionary<PerkHash, (int, int)>()
        {
            // Example entries: PerkHash, (Perk spell id, Add proc spell id)
            { PerkHash.LEProcAdventurerAesirAbsorption, (263361, 263362) },
            { PerkHash.LEProcAdventurerMacheteFlurry, (263329, 266732) },
            { PerkHash.LEProcAdventurerSelfPreservation, (263369, 266730) },
            { PerkHash.LEProcAdventurerSkinProtection, (263357, 266727) },
            { PerkHash.LEProcAdventurerFerociousHits, (263366, 266724) },

            { PerkHash.LEProcAgentGrimReaper, (263393, 266790) },
            { PerkHash.LEProcAgentDisableCuffs, (263409, 266787) },
            { PerkHash.LEProcAgentNoEscape, (263405, 266786) },
            { PerkHash.LEProcAgentIntenseMetabolism, (263381, 266749) },
            { PerkHash.LEProcAgentMinorNanobotEnhance, (263377, 266747) },

            { PerkHash.LEProcBureaucratPleaseHold, (263453, 267024) },
            { PerkHash.LEProcBureaucratFormsInTriplicate, (263426, 266798) },
            { PerkHash.LEProcBureaucratSocialServices, (263461, 267041) },
            { PerkHash.LEProcBureaucratNextWindowOver, (263465, 266794) },
            { PerkHash.LEProcBureaucratWaitInThatQueue, (263429, 267023) },

            { PerkHash.LEProcDoctorDangerousCulture, (263477, 266816) },
            { PerkHash.LEProcDoctorAntiseptic, (263518, 266815) },
            { PerkHash.LEProcDoctorMuscleMemory, (263514, 267034) },
            { PerkHash.LEProcDoctorBloodTransfusion, (263481, 266810) },
            { PerkHash.LEProcDoctorRestrictiveBandaging, (263504, 266804) },

            { PerkHash.LEProcEnforcerVortexOfHate, (263625, 302209) },
            { PerkHash.LEProcEnforcerRagingBlow, (263597, 266829) },
            { PerkHash.LEProcEnforcerShieldOfTheOgre, (263637, 266827) },
            { PerkHash.LEProcEnforcerInspireRage, (263633, 263634) },
            { PerkHash.LEProcEnforcerAirOfHatred, (263617, 302208) },
            { PerkHash.LEProcEnforcerTearLigaments, (263593, 266823) },
            { PerkHash.LEProcEnforcerVileRage, (263629, 266821) },

            { PerkHash.LEProcEngineerReactiveArmor, (263767, 266867) },
            { PerkHash.LEProcEngineerDestructiveTheorem, (263779, 266868) },
            { PerkHash.LEProcEngineerEnergyTransfer, (263755, 266988) },
            { PerkHash.LEProcEngineerEndureBarrage, (263751, 266864) },
            { PerkHash.LEProcEngineerDestructiveSignal, (263775, 66862) },
            { PerkHash.LEProcEngineerSplinterPreservation, (263759, 266863) },
            { PerkHash.LEProcEngineerCushionBlows, (263747, 266859) },

            { PerkHash.LEProcFixerLucksCalamity, (265980, 267042) },
            { PerkHash.LEProcFixerDirtyTricks, (265958, 266852) },
            { PerkHash.LEProcFixerEscapeTheSystem, (265951, 266850) },
            { PerkHash.LEProcFixerIntenseMetabolism, (263381, 266749) },
            { PerkHash.LEProcFixerFishInABarrel, (265962, 267043) },

            { PerkHash.LEProcKeeperRighteousSmite, (266115, 266890) },
            { PerkHash.LEProcKeeperSymbioticBypass, (265995, 266888) },
            { PerkHash.LEProcKeeperVirtuousReaper, (266112, 266884) },
            { PerkHash.LEProcKeeperIgnoreTheUnrepentant, (266163, 266881) },
            { PerkHash.LEProcKeeperPureStrike, (266160, 266878) },
            { PerkHash.LEProcKeeperEschewTheFaithless, (266148, 266877) },
            { PerkHash.LEProcKeeperRighteousStrike, (266109, 266874) },

            { PerkHash.LEProcMartialArtistAbsoluteFist, (266004, 266972) },
            { PerkHash.LEProcMartialArtistStrengthenKi, (266151, 266971) },
            { PerkHash.LEProcMartialArtistDisruptKi, (266071, 266969) },
            { PerkHash.LEProcMartialArtistSmashingFist, (266001, 266963) },
            { PerkHash.LEProcMartialArtistStrengthenSpirit, (266169, 266961) },
            { PerkHash.LEProcMartialArtistStingingFist, (265998, 266957) },

            { PerkHash.LEProcMetaPhysicistNanobotContingentArrest, (266207, 267037) },
            { PerkHash.LEProcMetaPhysicistAnticipatedEvasion, (266178, 266933) },
            { PerkHash.LEProcMetaPhysicistThoughtfulMeans, (266077, 302211) },
            { PerkHash.LEProcMetaPhysicistRegainFocus, (266010, 266928) },
            { PerkHash.LEProcMetaPhysicistEconomicNanobotUse, (266007, 266926) },

            { PerkHash.LEProcNanoTechnicianThermalReprieve, (266986, 266944) },
            { PerkHash.LEProcNanoTechnicianHarvestEnergy, (266210, 266943) },
            { PerkHash.LEProcNanoTechnicianLayeredAmnesty, (266987, 266940) },
            { PerkHash.LEProcNanoTechnicianSourceTap, (266019, 266939) },
            { PerkHash.LEProcNanoTechnicianCircularLogic, (266016, 266936) },

            { PerkHash.LEProcShadeBlackenedLegacy, (266091, 266975) },
            { PerkHash.LEProcShadeSiphonBeing, (266059, 266970) },
            { PerkHash.LEProcShadeShadowedGift, (266088, 266966) },
            { PerkHash.LEProcShadeDrainEssence, (266056, 266964) },
            { PerkHash.LEProcShadeElusiveSpirit, (266026, 266962) },
            { PerkHash.LEProcShadeToxicConfusion, (266186, 266960) },
            { PerkHash.LEProcShadeSapLife, (266053, 266954) },

            { PerkHash.LEProcSoldierFuriousAmmunition, (266041, 266905) },
            { PerkHash.LEProcSoldierTargetAcquired, (266097, 266901) },
            { PerkHash.LEProcSoldierReconditioned, (266142, 266900) },
            { PerkHash.LEProcSoldierConcussiveShot, (266032, 266898) },
            { PerkHash.LEProcSoldierEmergencyBandages, (266035, 266897) },
            { PerkHash.LEProcSoldierSuccessfulTargeting, (266189, 266894) },

            { PerkHash.LEProcTraderDebtCollection, (266068, 266923) },
            { PerkHash.LEProcTraderAccumulatedInterest, (266228, 267032) },
            { PerkHash.LEProcTraderExchangeProduct, (266106, 266918) },
            { PerkHash.LEProcTraderUnforgivenDebts, (266103, 267028) },
            { PerkHash.LEProcTraderUnexpectedBonus, (266044, 266912) },
            { PerkHash.LEProcTraderRebate, (266062, 266906) },
        };

        Dictionary<PerkHash, (int, int)> ProcType2Map = new Dictionary<PerkHash, (int, int)>()
        {
            { PerkHash.LEProcAdventurerHealingHerbs, (263347, 266736) },
            { PerkHash.LEProcAdventurerCombustion, (263373, 266735) },
            { PerkHash.LEProcAdventurerCharringBlow, (263333, 266731) },
            { PerkHash.LEProcAdventurerRestoreVigor, (263353, 266728) },
            { PerkHash.LEProcAdventurerMacheteSlice, (263337, 266726) },
            { PerkHash.LEProcAdventurerSoothingHerbs, (263341, 266725) },
            { PerkHash.LEProcAdventurerBasicDressing, (263349, 266723) },

            { PerkHash.LEProcAgentNotumChargedRounds, (263421, 266789) },
            { PerkHash.LEProcAgentLaserAim, (263413, 266788) },
            { PerkHash.LEProcAgentNanoEnhancedTargeting, (263401, 266752) },
            { PerkHash.LEProcAgentPlasteelPiercingRounds, (263417, 266751) },
            { PerkHash.LEProcAgentCellKiller, (263389, 266750) },
            { PerkHash.LEProcAgentImprovedFocus, (263385, 266748) },
            { PerkHash.LEProcAgentBrokenAnkle, (263397, 302293) },

            { PerkHash.LEProcBureaucratMobilityEmbargo, (263433, 266802) },
            { PerkHash.LEProcBureaucratWrongWindow, (263469, 266800) },
            { PerkHash.LEProcBureaucratTaxAudit, (263457, 266799) },
            { PerkHash.LEProcBureaucratLostPaperwork, (263445, 266796) },
            { PerkHash.LEProcBureaucratDeflation, (263441, 266795) },
            { PerkHash.LEProcBureaucratInflationAdjustment, (263437, 266791) },
            { PerkHash.LEProcBureaucratPapercut, (263449, 266792) },

            { PerkHash.LEProcDoctorMassiveVitaePlan, (263510, 266817) },
            { PerkHash.LEProcDoctorAnatomicBlight, (263516, 267035) },
            { PerkHash.LEProcDoctorHealingCare, (263512, 266813) },
            { PerkHash.LEProcDoctorPathogen, (263503, 266811) },
            { PerkHash.LEProcDoctorAnesthetic, (263506, 266809) },
            { PerkHash.LEProcDoctorAstringent, (263473, 263505) },
            { PerkHash.LEProcDoctorInflammation, (263508, 266805) },

            { PerkHash.LEProcEnforcerViolationBuffer, (263621, 266826) },
            { PerkHash.LEProcEnforcerInspireIre, (263609, 266825) },
            { PerkHash.LEProcEnforcerShrugOffHits, (263601, 266820) },
            { PerkHash.LEProcEnforcerBustKneecaps, (263613, 266819) },
            { PerkHash.LEProcEnforcerIgnorePain, (263605, 266818) },

            { PerkHash.LEProcEngineerAssaultForceRelief, (263783, 266873) },
            { PerkHash.LEProcEngineerDroneMissiles, (263771, 266871) },
            { PerkHash.LEProcEngineerDroneExplosives, (263791, 266865) },
            { PerkHash.LEProcEngineerCongenialEncasement, (263787, 266860) },
            { PerkHash.LEProcEngineerPersonalProtection, (263763, 266858) },

            { PerkHash.LEProcFixerBootlegRemedies, (265981, 266857) },
            { PerkHash.LEProcFixerSlipThemAMickey, (265952, 266856) },
            { PerkHash.LEProcFixerBendingTheRules, (265973, 266854) },
            { PerkHash.LEProcFixerBackyardBandages, (265968, 266853) },
            { PerkHash.LEProcFixerFightingChance, (265965, 266851) },
            { PerkHash.LEProcFixerContaminatedBullets, (265972, 266847) },
            { PerkHash.LEProcFixerUndergroundSutures, (265979, 266846) },

            { PerkHash.LEProcKeeperHonorRestored, (266219, 266891) },
            { PerkHash.LEProcKeeperAmbientPurification, (266201, 266885) },
            { PerkHash.LEProcKeeperBenevolentBarrier, (265992, 266882) },
            { PerkHash.LEProcKeeperSubjugation, (265989, 266879) },
            { PerkHash.LEProcKeeperFaithfulReconstruction, (266157, 266875) },

            { PerkHash.LEProcMartialArtistSelfReconstruction, (266124, 266974) },
            { PerkHash.LEProcMartialArtistDebilitatingStrike, (266204, 266967) },
            { PerkHash.LEProcMartialArtistHealingMeditation, (266121, 266965) },
            { PerkHash.LEProcMartialArtistAttackLigaments, (266166, 266959) },
            { PerkHash.LEProcMartialArtistMedicinalRemedy, (266118, 266955) },

            { PerkHash.LEProcMetaPhysicistSuperEgoStrike, (266133, 266934) },
            { PerkHash.LEProcMetaPhysicistSuppressFury, (266175, 267040) },
            { PerkHash.LEProcMetaPhysicistEgoStrike, (266013, 266931) },
            { PerkHash.LEProcMetaPhysicistMindWail, (266130, 266929) },
            { PerkHash.LEProcMetaPhysicistSowDoubt, (266172, 267039) },
            { PerkHash.LEProcMetaPhysicistSowDespair, (266074, 266925) },
            { PerkHash.LEProcMetaPhysicistDiffuseRage, (266127, 267038) },

            { PerkHash.LEProcNanoTechnicianOptimizedLibrary, (266702, 266946) },
            { PerkHash.LEProcNanoTechnicianAcceleratedReality, (266225, 266945) },
            { PerkHash.LEProcNanoTechnicianLoopingService, (266700, 266942) },
            { PerkHash.LEProcNanoTechnicianPoweredNanoFortress, (266050, 266941) },
            { PerkHash.LEProcNanoTechnicianIncreaseMomentum, (266136, 266938) },
            { PerkHash.LEProcNanoTechnicianUnstableLibrary, (266080, 266937) },

            { PerkHash.LEProcShadeBlackheart, (266029, 266973) },
            { PerkHash.LEProcShadeTwistedCaress, (266154, 266968) },
            { PerkHash.LEProcShadeConcealedSurprise, (266085, 266958) },
            { PerkHash.LEProcShadeMisdirection, (266183, 266956) },
            { PerkHash.LEProcShadeDeviousSpirit, (266023, 266953) },

            { PerkHash.LEProcSoldierFuseBodyArmor, (266216, 266904) },
            { PerkHash.LEProcSoldierOnTheDouble, (266038, 266903) },
            { PerkHash.LEProcSoldierGrazeJugularVein, (266213, 266902) },
            { PerkHash.LEProcSoldierGearAssaultAbsorption, (266192, 266899) },
            { PerkHash.LEProcSoldierDeepSixInitiative, (266139, 266895) },
            { PerkHash.LEProcSoldierShootArtery, (266094, 266893) },

            { PerkHash.LEProcTraderUnopenedLetter, (266047, 267033) },
            { PerkHash.LEProcTraderRigidLiquidation, (266198, 266919) },
            { PerkHash.LEProcTraderDepleteAssets, (266065, 67025) },
            { PerkHash.LEProcTraderEscrow, (266145, 266914) },
            { PerkHash.LEProcTraderRefinanceLoans, (266195, 267030) },
            { PerkHash.LEProcTraderPaymentPlan, (266100, 266101) },
        };

        #endregion

        public static string ProcDescription()
        {
            var lines = new List<string>();

            switch (DynelManager.LocalPlayer.Profession)
            {
                case Profession.Adventurer:
                    lines.Add("Type 1");
                    lines.Add("Aesir Absorption (Self Modify Add All Def. 50 30s)");
                    lines.Add("Machete Flurry (Self Modify +Damage 75 60s)");
                    lines.Add("Self Preservation (Self Modify ShieldAC 52, Self Modify AbsorbAC 255 60s)");
                    lines.Add("Skin Protection (Self Modify ShieldAC 31, Self Modify AbsorbAC 150 60s)");
                    lines.Add("Ferocious Hits (Self Modify +Damage 15 30s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Healing Herbs (Self Hit Health 697 .. 1193)");
                    lines.Add("Combustion (Target Hit Health Fire -1294 .. -2415)");
                    lines.Add("Charring Blow (Target Hit Health Fire -533 .. -1434)");
                    lines.Add("Restore Vigor (Team Hit Health 356 .. 746)");
                    lines.Add("Machete Slice (Target Hit Health Fire -137 .. -350)");
                    lines.Add("Soothing Herbs (Self Hit Health 186 .. 391)");
                    lines.Add("Basic Dressing (Self Hit Health 15 .. 25)");
                    break;

                case Profession.Agent:
                    lines.Add("Type 1");
                    lines.Add("Grim Reaper (Target Hit Health Melee 500, 10 hits, 1s delay)");
                    lines.Add("Disable Cuffs (Self Reduce Snare 1083s, Self Reduce Root 1083s, Resist root/snares 20% 15s)");
                    lines.Add("No Escape! (Target Restrict Action Movement, 6s delay 6s)");
                    lines.Add("Intense Metabolism (Self Modify Nano init 250 60s)");
                    lines.Add("Minor Nanobot Enhance (Self Modify +Damage 15 60s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Notum-Charged Rounds (Self Modify +Damage 200 60s)");
                    lines.Add("Laser Aim (Self Modify CriticalIncrease 30 60s)");
                    lines.Add("Nano-Enhanced Targeting (Self Modify CriticalIncrease 22 15s)");
                    lines.Add("Plasteel Piercing Rounds (Self Modify +Damage 75 60s)");
                    lines.Add("Cell Killer (Target Hit Health Melee 75, 20 hits, 1s delay 10s)");
                    lines.Add("Improved Focus (Self Modify CriticalIncrease 15 15s)");
                    lines.Add("Broken Ankle (Target Restrict Action Movement 3s)");
                    break;

                case Profession.Bureaucrat:
                    lines.Add("Type 1");
                    lines.Add("Please Hold (Target Modify Run speed -1500 30s)");
                    lines.Add("Forms in Triplicate (Self Hit Nano 20%)");
                    lines.Add("Social Services (Target Restrict Action Movement 6s)");
                    lines.Add("Next Window Over (Self Hit Nano 10%)");
                    lines.Add("Wait In That Queue (Target Modify Run speed -600 15s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Mobility Embargo (AOE 10m Restrict Action Movement 8s)");
                    lines.Add("Wrong Window (Self Modify Nano attack damage 50% 30s)");
                    lines.Add("Tax Audit (Target Hit Health Energy -1600 .. -3750)");
                    lines.Add("Lost Paperwork (Target Hit Health Melee -264 .. -532)");
                    lines.Add("Deflation (Self Modify Nano attack damage 25% 45s)");
                    lines.Add("Inflation Adjustment (Self Modify Nano attack damage modifier 10% 60s)");
                    lines.Add("Papercut (Target Hit Health Cold -10 .. -23)");
                    break;

                case Profession.Doctor:
                    lines.Add("Type 1");
                    lines.Add("Dangerous Culture (DOT 11 250 Poison-damage [15x750 every 2sec] 30s)");
                    lines.Add("Antiseptic (Healing 1133 - 1533)");
                    lines.Add("Muscle Memory (Self Nano Init Buff 250 60s)");
                    lines.Add("Blood Transfusion (Healing 327-551)");
                    lines.Add("Restrictive Bandaging (Healing 21-37)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Massive Vitae Plan (Self HealEff 25% 60s)");
                    lines.Add("Anatomic Blight (Init Debuff -569 60s)");
                    lines.Add("Healing Care (Healing [Team] 434-820)");
                    lines.Add("Pathogen (DOT 1 3375 Poison-damage [15x225 every 2sec] 30s)");
                    lines.Add("Anesthetic (Self HealEff +15% 60s)");
                    lines.Add("Astringent (Init Debuff -350 20s) ");
                    lines.Add("Inflammation (DOT 300 Poison-damage [20x15 every 2sec] 30s)");
                    break;

                case Profession.Enforcer:
                    lines.Add("Type 1");
                    lines.Add("Vortex Of Hate (AOE Taunt, Self HOT 224 x10, 2s delay 20s)");
                    lines.Add("Raging Blow (AR/Dmg Buff [Challenger] +255 Damage +111 AAO 60s)");
                    lines.Add("Shield Of The Ogre (Absorbshield 745 60s)");
                    lines.Add("Inspire Rage (Taunt 1600)");
                    lines.Add("Air Of Hatred (20m AOE taunt and self HOT, Heal 79 x10, 2s delay 20s)");
                    lines.Add("Tear Ligaments (AR/Dmg Buff [Challenger] +170 Damage +70 AAO +34% Scale 60s)");
                    lines.Add("Vile Rage (Rage +350 Runspeed +200 NR +250 Inits 60s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Violation Buffer (Damageshield +479 Max HP +75 Shield damage +240 Energy AC, Self heal 479 60s) ");
                    lines.Add("Inspire Ire (Taunt 4750)");
                    lines.Add("Shrug Off Hits (Absorbshield 280 60s)");
                    lines.Add("Bust Kneecaps (AR/Dmg Buff [Challenger] +27 Damage +12 AAO +16% Scale 42s)");
                    lines.Add("Ignore Pain (Damageshield +25 Max HP +10 Shield damage, self heal 25 60s)");
                    break;

                case Profession.Engineer:
                    lines.Add("Type 1");
                    lines.Add("Reactive Armor (Absorbshield 675 60s)");
                    lines.Add("Destructive Theorem (Melee/Phys/ranged Init Buff +150, Add All DMG +35 60s)");
                    lines.Add("Energy Transfer (Damageshield +75 60s)");
                    lines.Add("Endure Barrage (AC Buff +500 60s)");
                    lines.Add("Destructive Signal (Melee/Phys/ranged Init Buff +80, Add All DMG +20 60s)");
                    lines.Add("Splinter Preservation (Absorbshield 375 60s)");
                    lines.Add("Cushion Blows (Damageshield +10 Damageshield +40 Melee AC 60s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Assault Force Relief (AC Buff (Team) +2500 60s)");
                    lines.Add("Drone Missiles (Target Hit 1375 - 3211)");
                    lines.Add("Drone Explosives (Target Hit 497 - 1016 Projectiledamage)");
                    lines.Add("Congenial Encasement (Reflectshield +13% Reflect 7 Max Reflected Damage 60s) ");
                    lines.Add("Personal Protection (AC Buff +130 60s)");
                    break;

                case Profession.Fixer:
                    lines.Add("Type 1");
                    lines.Add("Luck's Calamity (Evade Debuff -170 60s)");
                    lines.Add("Dirty Tricks (Dodge Buff +100 60s)");
                    lines.Add("Escape The System (Root reducer -45 sec [10% change for proc])");
                    lines.Add("Intense Metabolism (Self Modify Nano init 250 60s)");
                    lines.Add("Fish In A Barrel (Evade Debuff -85 60s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Bootleg Remedies (HOT 2436 - 2634 [6x406-439 every 10sec] 60s)");
                    lines.Add("Slip Them A Mickey (Damage Boost +130 60s)");
                    lines.Add("Bending The Rules (Damage Boost +85 60s)");
                    lines.Add("Backyard Bandages (HOT 2172 - 2220 [6x362-370 every 10sec] 60s)");
                    lines.Add("Fighting Chance (Damage Boost +50 60s)");
                    lines.Add("Contaminated Bullets (Damage Boost +3 60s)");
                    lines.Add("Underground Sutures (HOT 180 - 216 [12x 15-18 every 5 sec] 60s)");
                    break;

                case Profession.Keeper:
                    lines.Add("Type 1");
                    lines.Add("Righteous Smite (Damage Boost [Team] +200 60s)");
                    lines.Add("Symbiotic Bypass (Evade Buff [Team] +140 Evade ClsC +40 Dodge Ranged +40 Duck Exp 60s) ");
                    lines.Add("Virtuous Reaper (Damage Boost [Team] +90 60s");
                    lines.Add("Ignore The Unrepentant (Evade Buff +110 Evade ClsC +30 Dodge Ranged +30 Duck Exp 60s)");
                    lines.Add("Pure Strike (Damage Boost +65 60s)");
                    lines.Add("Eschew the Faithless (Self Modify Duck explosives 14 Dodge ranged 14 Evade close 50 60s)");
                    lines.Add("Righteous Strike (Self Modify Damage modifier 20 60s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Honor Restored (AAO +50 AAD +120 [Team] 60s)");
                    lines.Add("Ambient Purification (Healing [Team] 481-948)");
                    lines.Add("Benevolent Barrier (Reflect Shield +4% 600s)");
                    lines.Add("Subjugation (AAO +20 AAD +45 [Team] 60s)");
                    lines.Add("Faithful Reconstruction (Team Hit Health 42 .. 53)");
                    break;

                case Profession.MartialArtist:
                    lines.Add("Type 1");
                    lines.Add("Absolute Fist (+111 Dmg 60s)");
                    lines.Add("Strengthen Ki (+40 Strength, +676 Melee/ma AC, +574 Other AC 60s) ");
                    lines.Add("Disrupt Ki (Evade Buff +85 60s)");
                    lines.Add("Smashing Fist (+63 Dmg 60s)");
                    lines.Add("Strengthen Spirit (+269 Melee/ma AC, +229 Other AC 60s)");
                    lines.Add("Stinging Fist (+19 Dmg 60s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Self Reconstruction (Healing 980 - 1803)");
                    lines.Add("Debilitating Strike (Crit Increase +19% 60s)");
                    lines.Add("Healing Meditation (Healing 443 - 981)");
                    lines.Add("Attack Ligaments (Crit Increase +8% 60s)");
                    lines.Add("Medicinal Remedy (Healing 34 - 59  60s)");
                    break;

                case Profession.Metaphysicist:
                    lines.Add("Type 1");
                    lines.Add("Nanobot Contingent Arrest (Fight Target: -750 NanoInit, %Add nano cost +100%, Nano cast interrupt -25% 60s) ");
                    lines.Add("Anticipated Evasion (Evade Buff +250 60s)");
                    lines.Add("Thoughtful Means (Reduces Nano Cost by 25% 60s)");
                    lines.Add("Regain Focus (Evade Buff +100 60s)");
                    lines.Add("Economic Nanobot Use (Nanocost Reducer -12% 60s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Super-Ego Strike (Target Hit 1500 - 3000 Cold-damage)");
                    lines.Add("Suppress Fury (Damage Debuff -75 Damage -261 Inits 60s)");
                    lines.Add("Ego Strike (Target Hit 802 - 1468 Cold-damage)");
                    lines.Add("Mind Wail (Target Hit 314-699 Cold-damage)");
                    lines.Add("Sow Doubt (Damage Debuff -35 Damage -156 Inits 60s)");
                    lines.Add("Sow Despair (Target Hit 30-65 Poison-damage)");
                    lines.Add("Diffuse Rage (Damage Debuff -7 Damage -35 Inits 60s)");
                    break;

                case Profession.NanoTechnician:
                    lines.Add("Type 1");
                    lines.Add("Thermal Reprieve (Reflectshield +10% Reflect 10 Max Reflect 60s)");
                    lines.Add("Harvest Energy (Nano-HOT 5220 [12x435 every 5sec] 60s)");
                    lines.Add("Layered Amnesty (Reflectshield +4% 60s Defensive)");
                    lines.Add("Source Tap (Nano-HOT 1224 [12x102 every 5sec] 60s)");
                    lines.Add("Circular Logic (Nano-HOT 60 [12x5 every 5sec] 60s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Optimized Library (AC/HP/NR Buff +331 AC +350 Max HP +140 NR 60s)");
                    lines.Add("Accelerated Reality (Nanoinit Buff +750 60s)");
                    lines.Add("Looping Service (Absorbshield 680 60s)");
                    lines.Add("Powered Nano Fortress (AC/HP/NR Buff +167 AC +246 Max HP +111 NR 60s) ");
                    lines.Add("Increase Momentum (Nanoinit Buff +200 60s)");
                    lines.Add("Unstable Library (AC/HP/NR Buff +31 AC +50 Max HP +32 NR 60s)");
                    break;

                case Profession.Shade:
                    lines.Add("Type 1");
                    lines.Add("Blackened Legacy (Evade Buff EvadeClsC 100 DuckExp 50 DodgeRange 50 60s) ");
                    lines.Add("Siphon Being (HP Drain 580 Energy-damage 577 Healing)");
                    lines.Add("Shadowed Gift (DOT 975 Poison-damage [5x195 hits every 1 sec] 6s)");
                    lines.Add("Drain Essence (HP Drain 382 Energy-damage 310 Healing)");
                    lines.Add("Elusive Spirit (Evade Buff EvadeClsC 56 DuckExp 32 DodgeRange 32 60s)");
                    lines.Add("Toxic Confusion (DOT 425 Poison-damage [5x85 hits every 1sec] 6s)");
                    lines.Add("Sap Life (HP Drain 17 Energy-damage 7 HP Gain)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Blackheart (Target Hit 767 Melee-damage)");
                    lines.Add("Twisted Caress (Target Hit 550 Melee-damage)");
                    lines.Add("Concealed Surprise (Target Hit 234 Melee-damage)");
                    lines.Add("Misdirection (Evade Buff EvadeClsC 40 DuckExp 25 DodgeRange 25 60s)");
                    lines.Add("Devious Spirit (Target Hit 23 Melee-damage)");
                    break;

                case Profession.Soldier:
                    lines.Add("Type 1");
                    lines.Add("Furious Ammunition (Damage Buff +99 60s)");
                    lines.Add("Target Acquired (AR Buff +35 AAO 60s)");
                    lines.Add("Reconditioned (Self Buff +361 Max HP, HOT 650 [13x50 every 5s] 60s)");
                    lines.Add("Concussive Shot (Damage Buff +35 damage 60s)");
                    lines.Add("Emergency Bandages (Self Buff +200 Max HP, HOT 325 [13x25 every 5s] 60s) ");
                    lines.Add("Successful Targeting (AR Buff +23 AAO 60s)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Fuse Body Armor (Max Reflect DMG +75 60s)");
                    lines.Add("On The Double (Init Buff +150 60s)");
                    lines.Add("Graze Jugular Vein (Damage Buff +70 60s)");
                    lines.Add("Gear Assault Absorption (Max Reflect DMG +25 60s)");
                    lines.Add("Deep Six Initiative (Init Buff +50 60s)");
                    lines.Add("Shoot Artery (Damage Buff +15 all damage 60s)");
                    break;

                case Profession.Trader:
                    lines.Add("Type 1");
                    lines.Add("Debt Collection (HP Drain 1100 - 1200 Energy-damage 1100-1300 Healing)");
                    lines.Add("Accumulated Interest (Skilldrain 204 Weapon- and Nanoskills Drain [10s] Gain 102 AAO Drain [PVM] [30s] 30s)");
                    lines.Add("Exchange Product (HP Drain 990 Energy-damage 997 Healing)");
                    lines.Add("Unforgiven Debts (Skilldrain 136 Weapon- and Nanoskills Drain [10s] Gain 68 AAO Drain [PVM] [30s] 30s)");
                    lines.Add("Unexpected Bonus (HP Drain 300 Energydamage 222 Healing)");
                    lines.Add("Rebate (HP Drain 43 Energydamage 18 Healing)");
                    lines.Add("");
                    lines.Add("Type 2");
                    lines.Add("Unopened Letter (AC Drain +2067 AC -2098 AC 60s)");
                    lines.Add("Rigid Liquidation (Nanodrain 1842 Drain [6x307 every 5sec] 1626 Gain [6x271 every 5sec] 30s) ");
                    lines.Add("Deplete Assets (AC Drain +1394 AC -1449 AC 60s)");
                    lines.Add("Escrow (Nanodrain 798 Drain [6x133 every 5sec] 600 Gain [6x100 every 5sec] 30s)");
                    lines.Add("Refinance Loans (AC Drain -200 AC +200 AC [On FightingTarget] 60s)");
                    lines.Add("Payment Plan (Skilldrain 9 Weapon- and Nanoskills Drain [10s] Gain 5 AAO Drain [PVM] [30s] 30s)");
                    break;
            }

            return string.Join("\n", lines);
        }
    }
}