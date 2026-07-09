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
        public static HashSet<int> OwnedItems = new HashSet<int>();
        public static HashSet<int> MaAttacks = new HashSet<int>();
        public static HashSet<(Item, int)> SkillLock = new HashSet<(Item, int)>();

        public void RegisterItemProcessors()
        {
            foreach (var item in Inventory.Items.Concat(Inventory.Backpacks.Where(b => b.Name != null && b.Name.IndexOf("fannypack", StringComparison.OrdinalIgnoreCase) >= 0).SelectMany(bp => Inventory.GetContainerItems(bp.Identity))))
            {
                #region Heal

                if ((_wieldedWeapons & (CharacterWieldedWeapon.Fists | CharacterWieldedWeapon.MartialArts)) != 0 && RelevantGenericItems.FlowerOfLife.Contains(item.Id))
                {
                    MaAttacks.Add(item.Id);
                    RegisterItemProcessor(RelevantGenericItems.FlowerOfLife, FlowerOfLife, CombatActionPriority.High, RuleContext.Both);
                }
                if (RelevantGenericItems.SitKits.Contains(item.Id))
                {
                    LoadedHeals.Add(item.Id);
                    RegisterItemProcessor(RelevantGenericItems.SitKits, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        SitItemUsage(id, fightingTarget, ref actionTarget, "NanoKitsValue", "HealthKitsValue"), CombatActionPriority.High, RuleContext.OutOfCombat);
                }

                if (RelevantGenericItems.PremiumNanoRecharger.Contains(item.Id))
                {
                    LoadedHeals.Add(item.Id);
                    RegisterItemProcessor(RelevantGenericItems.PremiumNanoRecharger, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        SitItemUsage(id, fightingTarget, ref actionTarget, "PremiumNanoRechargerValue", null), CombatActionPriority.High, RuleContext.OutOfCombat);
                }

                if (RelevantGenericItems.HealthAndNanoStims.Contains(item.Id))
                {
                    LoadedHeals.Add(item.Id);
                    RegisterItemProcessor(RelevantGenericItems.HealthAndNanoStims, HealthAndNanoStim, CombatActionPriority.High, RuleContext.Both);
                }

                if (item.Id == RelevantGenericItems.DeathsDoor)
                {
                    LoadedHeals.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "DeathsDoorValue"), CombatActionPriority.High, RuleContext.Both);
                }

                if (RelevantGenericItems.FreeStims.Contains(item.Id))
                {
                    LoadedHeals.Add(item.Id);
                    RegisterItemProcessor(item.Id, FreeStim, CombatActionPriority.High, RuleContext.Both);
                }

                if (item.Id == RelevantGenericItems.ViralCommunicationsLarvae)
                {
                    LoadedHeals.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetNanoValue(id, fightingTarget, ref actionTarget, "ViralCommunicationsLarvaeValue"), CombatActionPriority.High, RuleContext.Both);
                }

                if (item.Id == RelevantGenericItems.NotumFocus)
                {
                    LoadedHeals.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetNanoValue(id, fightingTarget, ref actionTarget, "NotumFocusValue"), CombatActionPriority.High, RuleContext.Both);
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Doctor && RelevantGenericItems.TOTW_Doc_Books.Contains(item.Id))
                {
                    LoadedHeals.Add(item.Id);
                    int highestTOTWDocBook = RelevantGenericItems.TOTW_Doc_Books.FirstOrDefault(id => Inventory.Items.Any(i => i.Id == id) || Inventory.Backpacks.Any(bp => Inventory.GetContainerItems(bp.Identity).Any(i => i.Id == id)));
                    RegisterItemProcessor(highestTOTWDocBook, TOTWHeal, CombatActionPriority.High, RuleContext.Both);
                }

                if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician && RelevantGenericItems.TOTWWrists.Contains(item.Id))
                {
                    LoadedHeals.Add(item.Id);
                    int highestTOTWWrist = RelevantGenericItems.TOTWWrists.FirstOrDefault(id => Inventory.Items.Any(i => i.Id == id) || Inventory.Backpacks.Any(bp => Inventory.GetContainerItems(bp.Identity).Any(i => i.Id == id)));
                    RegisterItemProcessor(highestTOTWWrist, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetNanoValue(id, fightingTarget, ref actionTarget, "TOTWWristsValue"), CombatActionPriority.High, RuleContext.Both);
                }

                if (RelevantGenericItems.SanguisugentBodyArmor.Contains(item.Id))
                {
                    LoadedHeals.Add(item.Id);
                    RegisterItemProcessor(RelevantGenericItems.SanguisugentBodyArmor, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetHealthValue(id, fightingTarget, ref actionTarget, "SanguisugentBodyArmorValue"), CombatActionPriority.High, RuleContext.Both);
                }

                if (RelevantGenericItems.StaffofCleansing.Contains(item.Id))
                {
                    LoadedHeals.Add(item.Id);
                    int highestStaffofCleansing = RelevantGenericItems.StaffofCleansing.FirstOrDefault(id => Inventory.Items.Any(i => i.Id == id) || Inventory.Backpacks.Any(bp => Inventory.GetContainerItems(bp.Identity).Any(i => i.Id == id)));
                    RegisterItemProcessor(highestStaffofCleansing, StaffofCleansing, CombatActionPriority.High);
                }

                if (RelevantGenericItems.PerniciousBodyArmor.Contains(item.Id))
                {
                    LoadedHeals.Add(item.Id);
                    int highestPerniciousBodyArmor = RelevantGenericItems.PerniciousBodyArmor.FirstOrDefault(id => Inventory.Items.Any(i => i.Id == id) || Inventory.Backpacks.Any(bp => Inventory.GetContainerItems(bp.Identity).Any(i => i.Id == id)));
                    RegisterItemProcessor(highestPerniciousBodyArmor, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetNanoValue(id, fightingTarget, ref actionTarget, "PerniciousBodyArmorValue"), CombatActionPriority.High);
                }
                #endregion

                #region Absorb\Shield\ACs

                if ((DynelManager.LocalPlayer.Breed == Breed.Nanomage || DynelManager.LocalPlayer.Profession == Profession.Enforcer) && RelevantGenericItems.DreadlochEnduranceBooster.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestDreadlochEnduranceBooster = RelevantGenericItems.DreadlochEnduranceBooster.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestDreadlochEnduranceBooster, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "DreadlochEnduranceBoosterValue"), CombatActionPriority.High, RuleContext.Both);
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Enforcer && item.Id == RelevantGenericItems.AssaultClassTank)
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "AssaultClassTankValue"), CombatActionPriority.High, RuleContext.Both);
                }

                if (RelevantGenericItems.TotwShieldShoulders.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestTotwShieldShoulders = RelevantGenericItems.TotwShieldShoulders.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestTotwShieldShoulders, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "TotwShieldShouldersValue"), CombatActionPriority.High, RuleContext.Both);
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Shade && item.Id == RelevantGenericItems.SharlsCyberneticTattoo)
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "SharlsCyberneticTattooValue"), CombatActionPriority.High, RuleContext.Both);
                }
                #endregion

                #region Buffs

                if (RelevantGenericItems.ReanimatedCloaks.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestReanimatedCloak = RelevantGenericItems.ReanimatedCloaks.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestReanimatedCloak, ReanimatedCloaks, CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.TotwDmgShoulders.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestTotwDmgShoulder = RelevantGenericItems.TotwDmgShoulders.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestTotwDmgShoulder, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "TotwDmgShouldersSelection", false, false), CombatActionPriority.Medium, RuleContext.InCombat);
                }

                if (RelevantGenericItems.GnuffsEternalRiftCrystal.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestGnuffsEternalRiftCrystal = RelevantGenericItems.GnuffsEternalRiftCrystal.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestGnuffsEternalRiftCrystal, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "GnuffsEternalRiftCrystalSelection", false, false), CombatActionPriority.Medium, RuleContext.InCombat);
                }

                if (item.Id == RelevantGenericItems.SteamingHotCupOfEnhancedCoffee)
                {
                    LoadedNonCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalOptionWitchBuffCheck(id, fightingTarget, ref actionTarget, "CoffeeOption", false, false, NanoLine.FoodandDrinkBuffs), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.FlurryOfBlows.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestFlurryOfBlows = RelevantGenericItems.FlurryOfBlows.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestFlurryOfBlows, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "FlurryOfBlowsSelection", false, false), CombatActionPriority.Medium, RuleContext.InCombat);
                }

                if (RelevantGenericItems.EyeOfTheHunter.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestEyeOfTheHunter = RelevantGenericItems.EyeOfTheHunter.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestEyeOfTheHunter, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "EyeOfTheHunterSelection", false, false), CombatActionPriority.Medium, RuleContext.InCombat);
                }

                if (RelevantGenericItems.MuscularStim.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestMuscularStim = RelevantGenericItems.MuscularStim.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestMuscularStim, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "MuscularStimValue"), CombatActionPriority.High);
                }

                if (item.Id == RelevantGenericItems.PolymerizingStim)
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "PolymerizingStimValue"), CombatActionPriority.High);
                }

                if (item.Id == RelevantGenericItems.MutatedSlitherBlood)
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "MutatedSlitherBloodValue"), CombatActionPriority.High);
                }

                if (RelevantGenericItems.BacchantesAnunWings.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestBacchantesAnunWings = RelevantGenericItems.BacchantesAnunWings.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestBacchantesAnunWings, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "BacchantesAnunWingsValue"), CombatActionPriority.High);
                }

                if (item.Id == RelevantGenericItems.CombatAssistWenWen)
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "CombatAssistWenWenValue"), CombatActionPriority.High, RuleContext.InCombat);
                }

                if (RelevantGenericItems.BoostedStim.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestBoostedStim = RelevantGenericItems.BoostedStim.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestBoostedStim, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "BoostedStimValue"), CombatActionPriority.High);
                }

                if (item.Id == RelevantGenericItems.BoltarBrainBlaster)
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "BoltarBrainBlasterValue"), CombatActionPriority.High);
                }

                if (item.Id == RelevantGenericItems.BioremediationStim)
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NoTargetHealthValue(id, fightingTarget, ref actionTarget, "BioremediationStimValue"), CombatActionPriority.High);
                }

                if (RelevantGenericItems.AncientGenerationDevices.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(RelevantGenericItems.AncientGenerationDevices, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "AncientGenerationDevices", false, true), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region combat

                if (RelevantGenericItems.SharpObjects.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["SharpObjects"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "SharpObjectsOption", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.ThrowingGrenades.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["ThrowingGrenades"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "ThrowingGrenadesOption", true, true), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.RingOfFleshes.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["RingOfFleshes"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "RingOfFleshesOption", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.WenWen.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["WenWen"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "WenWenOption", true, true), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.Manta.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["Manta"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "MantaOption", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.MantaAOE.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["MantaAOE"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "MantaAOEOption", false, true), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.ToTWBloodRings.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["ToTWBloodRings"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "ToTWBloodRingsOption", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.ToTWFlameRings.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["ToTWFlameRings"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "ToTWFlameRingsOption", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.ICCDrone.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["ICCDrone"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "ICCDroneOption", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.SpecialArrows.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["SpecialArrows"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "SpecialArrowsOption", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.AOESpecialArrows.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["AOESpecialArrows"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "AOESpecialArrowsOption", true, true), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.SuppressiveBurstItem.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["SuppressiveBurstItem"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "SuppressiveBurstItemOption", false, true), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.ClusterBullets.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["ClusterBullets"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "ClusterBulletsOption", false, true), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.HomingPermorphaBullets.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(_settings["HomingPermorphaBullets"].AsInt32(), (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "HomingPermorphaBulletsOption", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (item.Id == RelevantGenericItems.RingofSisterPestilence)
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "RingofSisterPestilenceSelection", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (item.Id == RelevantGenericItems.RingofSisterMerciless)
                {
                    LoadedNukeSpells.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "RingofSisterMercilessSelection", true, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (RelevantGenericItems.KamikazeRobotShell.Contains(item.Id))
                {
                    LoadedNukeSpells.Add(item.Id);
                    int highestKamikazeRobotShell = RelevantGenericItems.KamikazeRobotShell.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestKamikazeRobotShell, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "KamikazeRobotShellSelection", false, false), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if ((_wieldedWeapons & (CharacterWieldedWeapon.Fists | CharacterWieldedWeapon.MartialArts)) != 0)
                {
                    if (RelevantGenericItems.Enigma == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.Enigma, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "EnigmaOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.StingoftheViper == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.StingoftheViper, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "StingoftheViperOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.ApeFistofKhalum == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.ApeFistofKhalum, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "ApeFistofKhalumOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.KarmicFist == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.KarmicFist, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "KarmicFistOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.Shen.Contains(item.Id))
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.Shen, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "ShenOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.Sappo == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.Sappo, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "SappoOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.BrightBlueCloudlessSky.Contains(item.Id))
                    {
                        MaAttacks.Add(item.Id);
                        int highestBrightBlueCloudlessSky = RelevantGenericItems.BrightBlueCloudlessSky.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                        RegisterItemProcessor(highestBrightBlueCloudlessSky, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                           MAItem(id, fightingTarget, ref actionTarget, "BrightBlueCloudlessSkyOption"), CombatActionPriority.Low, RuleContext.InCombat);

                    }

                    if (RelevantGenericItems.BlessedWithThunder.Contains(item.Id))
                    {
                        MaAttacks.Add(item.Id);
                        int highestBlessedWithThunder = RelevantGenericItems.BlessedWithThunder.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                        RegisterItemProcessor(highestBlessedWithThunder, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                              MAItem(id, fightingTarget, ref actionTarget, "BlessedWithThunderOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.BirdOfPrey.Contains(item.Id))
                    {
                        MaAttacks.Add(item.Id);
                        int highestBirdOfPrey = RelevantGenericItems.BirdOfPrey.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                        RegisterItemProcessor(highestBirdOfPrey, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                             MAItem(id, fightingTarget, ref actionTarget, "BirdOfPreyOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.AttackOfTheSnake.Contains(item.Id))
                    {
                        MaAttacks.Add(item.Id);
                        int highestAttackOfTheSnake = RelevantGenericItems.AttackOfTheSnake.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                        RegisterItemProcessor(highestAttackOfTheSnake, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "AttackOfTheSnakeOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.AngelOfNight.Contains(item.Id))
                    {
                        MaAttacks.Add(item.Id);
                        int highestAngelOfNight = RelevantGenericItems.AngelOfNight.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                        RegisterItemProcessor(highestAngelOfNight, AngelOfNight, CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.TouchOfSaiFung == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.TouchOfSaiFung, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "TouchOfSaiFungOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.TheWizdomOfHuzzum == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.TheWizdomOfHuzzum, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "TheWizdomOfHuzzumOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.TreeOfEnlightenment == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.TreeOfEnlightenment, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "TreeOfEnlightenmentOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.UponAWaveOfSummer.Contains(item.Id))
                    {
                        MaAttacks.Add(item.Id);
                        int highestUponAWaveOfSummer = RelevantGenericItems.UponAWaveOfSummer.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                        RegisterItemProcessor(highestUponAWaveOfSummer, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "UponAWaveOfSummerOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.StampedeOfTheBoar == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.StampedeOfTheBoar, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "StampedeOfTheBoarOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.Delirium == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.Delirium, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "DeliriumOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (RelevantGenericItems.InnerBalance == item.Id)
                    {
                        MaAttacks.Add(item.Id);
                        RegisterItemProcessor(RelevantGenericItems.InnerBalance, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            MAItem(id, fightingTarget, ref actionTarget, "InnerBalanceOption"), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region Non Combat

                if (item.Id == RelevantGenericItems.ReflectGraft)
                {
                    LoadedNonCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NonCombatUserAndEquipped(id, fightingTarget, ref actionTarget, "ReflectGraftOption", new NanoLine[] { NanoLine.ReflectShield }), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (item.Id == RelevantGenericItems.BootsOfGridspaceDistortion)
                {
                    LoadedNonCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NonCombatUserAndEquipped(id, fightingTarget, ref actionTarget, "BootsOfGridspaceDistortionOption", new int[] { 305996 }), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (RelevantGenericItems.RingofKnowledge.Contains(item.Id))
                {
                    LoadedNonCombatBuffs.Add(item.Id);
                    int highestRingofKnowledge = RelevantGenericItems.RingofKnowledge.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestRingofKnowledge, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NonCombatItemBuff(id, fightingTarget, ref actionTarget, "RingofKnowledgeOption"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (item.Id == RelevantGenericItems.IskopsAscendancy)
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NonCombatItemBuff(id, fightingTarget, ref actionTarget, "IskopsAscendancyOption"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (RelevantGenericItems.BootsofInfiniteSpeed.Contains(item.Id))
                {
                    LoadedNonCombatBuffs.Add(item.Id);
                    int highestBootsofInfiniteSpeed = RelevantGenericItems.BootsofInfiniteSpeed.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestBootsofInfiniteSpeed, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NonCombatItemBuff(id, fightingTarget, ref actionTarget, "BootsofInfiniteSpeedOption"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (RelevantGenericItems.BurstofSpeedStim.Contains(item.Id))
                {
                    LoadedCombatBuffs.Add(item.Id);
                    int highestBurstofSpeedStim = RelevantGenericItems.BurstofSpeedStim.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(highestBurstofSpeedStim, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NonCombatItemBuff(id, fightingTarget, ref actionTarget, "BurstofSpeedStimOption"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (item.Id == RelevantGenericItems.CurseofMalahde)
                {
                    LoadedCombatBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NonCombatItemBuff(id, fightingTarget, ref actionTarget, "CurseofMalahdeOption"), CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                if (RelevantGenericItems.ExpCans.Contains(item.Id))
                {
                    LoadedNonCombatBuffs.Add(item.Id);
                    //int highestExpCan = RelevantGenericItems.ExpCans.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(item.Id, ExpCan, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                if (RelevantGenericItems.InsuranceCans.Contains(item.Id))
                {
                    LoadedNonCombatBuffs.Add(item.Id);
                    //int highestInsuranceCan = RelevantGenericItems.InsuranceCans.FirstOrDefault(id => id == item.Id && item.MeetsSelfUseReqs());
                    RegisterItemProcessor(item.Id, InsuranceCan, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                if (RelevantGenericItems.RezCans.Contains(item.Id))
                {
                    LoadedNonCombatBuffs.Add(item.Id);
                    //int highestRezCan = RelevantGenericItems.RezCans.FirstOrDefault(id => id == item.Id);
                    RegisterItemProcessor(item.Id, RezCan, CombatActionPriority.Ultra, RuleContext.Both);
                }

                #endregion

                #region Debuffs

                if (item.Id == RelevantGenericItems.BracerofBrotherMalevolence)
                {
                    LoadedDeBuffs.Add(item.Id);
                    RegisterItemProcessor(item.Id, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "BracerofBrotherMalevolenceSelection", true, false), CombatActionPriority.Medium, RuleContext.InCombat);
                }

                if (RelevantGenericItems.TotwBlindRings.Contains(item.Id))
                {
                    LoadedDeBuffs.Add(item.Id);
                    int highestTotwBlindRing = RelevantGenericItems.TotwBlindRings.FirstOrDefault(id => Inventory.Items.Any(i => i.Id == id) || Inventory.Backpacks.Any(bp => Inventory.GetContainerItems(bp.Identity).Any(i => i.Id == id)));
                    RegisterItemProcessor(highestTotwBlindRing, (Item id, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        ConditionalAttackSelection(id, fightingTarget, ref actionTarget, "TotwBlindRingsSelection", true, false), CombatActionPriority.Medium, RuleContext.InCombat);
                }

                #endregion

                #region Pets

                if (RelevantGenericItems.AndroidNCU.Contains(item.Id))
                {
                    LoadedPetBuffSpells.Add(item.Id);

                    if (DynelManager.LocalPlayer.Profession == Profession.Engineer)
                    {
                        if (!PetBuffWindowController.Attack.Any(c => c.SettingKey == "AttackPetNCU"))
                            PetBuffWindowController.Attack.Add(new PetBuffUiConfig { SpellIds = RelevantGenericItems.AndroidNCU, SettingKey = "AttackPetNCU", Label = "Upgrade NCUs", UiType = UiType.Checkbox });

                        if (!PetBuffWindowController.Support.Any(c => c.SettingKey == "SupportPetNCU"))
                            PetBuffWindowController.Support.Add(new PetBuffUiConfig { SpellIds = RelevantGenericItems.AndroidNCU, SettingKey = "SupportPetNCU", Label = "Upgrade NCUs", UiType = UiType.Checkbox });
                    }

                    if (DynelManager.LocalPlayer.Profession == Profession.Bureaucrat)
                    {
                        if (!PetBuffWindowController.Attack.Any(c => c.SettingKey == "AttackPetNCU"))
                            PetBuffWindowController.Attack.Add(new PetBuffUiConfig { SpellIds = RelevantGenericItems.AndroidNCU, SettingKey = "AttackPetNCU", Label = "Upgrade NCUs", UiType = UiType.Checkbox });
                    }

                    RegisterItemProcessor(RelevantGenericItems.AndroidNCU, Robot_NCU_Upgrade, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                if (MPUrns.Contains(item.Id))
                {
                    LoadedPetSpells[0] = true;
                    LoadedPetBuffSpells.Add(item.Id);

                    if (MPUrns.Contains(_settings["SpawnAttackPet"].AsInt32()))
                        RegisterItemProcessor(_settings["SpawnAttackPet"].AsInt32(), Urns, CombatActionPriority.NonCombat);
                }

                #endregion

                #region Taunts

                if (RelevantGenericItems.TauntTools.Contains(item.Id))
                {
                    LoadedTaunts.Add(item.Id);
                    RegisterItemProcessor(_settings["TauntItemTauntTools"].AsInt32(), TauntItem, CombatActionPriority.High, RuleContext.InCombat);
                }

                #endregion

                #region Pet Trimmers

                if (IncreaseAggressiveness.Contains(item.Id))
                {
                    OwnedTrimmers.Add(item.Id);

                    if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !TrimmersWindowController.SupportPet.Any(c => c.SettingKey == "SupportIncreaseAggressivenessCheckBox"))
                        TrimmersWindowController.SupportPet.Add(new TrimmerUiConfig
                        {
                            ItemIds = IncreaseAggressiveness,
                            SettingKey = "SupportIncreaseAggressivenessCheckBox",
                            Label = "Increase Aggressiveness",
                            UiType = UiType.Checkbox
                        });

                    RegisterItemProcessor(IncreaseAggressiveness, (Item itemToUse, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    TrimmerMethod(itemToUse, fightingTarget, ref actionTarget, "IncreaseAggressiveness"), CombatActionPriority.High, RuleContext.OutOfCombat);
                }

                if (AggressiveDefensiveSelectionArray.Contains(item.Id))
                {
                    OwnedTrimmers.Add(item.Id);
                    if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !TrimmersWindowController.SupportPet.Any(c => c.SettingKey == "SupportAggressiveDefensiveSelectionArray"))
                        TrimmersWindowController.SupportPet.Add(new TrimmerUiConfig
                        {
                            ItemIds = AggressiveDefensiveSelectionArray,
                            SettingKey = "SupportAggressiveDefensiveSelectionArray",
                            Label = "Aggressive/Defensive",
                            UiType = UiType.DropDownWOption
                        });

                    RegisterItemProcessor(AggressiveDefensiveSelectionArray, (Item itemToUse, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    TrimmerMethod(itemToUse, fightingTarget, ref actionTarget, "AggressiveDefensiveSelectionArray"), CombatActionPriority.High, RuleContext.Both);
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Engineer)
                {
                    if (DmgChangeSelectionArray.Contains(item.Id))
                    {
                        OwnedTrimmers.Add(item.Id);
                        RegisterItemProcessor(DmgChangeSelectionArray, (Item itemToUse, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TrimmerMethod(itemToUse, fightingTarget, ref actionTarget, "DmgChangeSelectionArray"), CombatActionPriority.High, RuleContext.OutOfCombat);
                    }
                }

                if (MechEngiSelectionArray.Contains(item.Id))
                {
                    OwnedTrimmers.Add(item.Id);

                    if (DynelManager.LocalPlayer.Profession == Profession.Engineer &&
                        !TrimmersWindowController.SupportPet.Any(c => c.SettingKey == "SupportMechEngiSelectionArray"))
                        TrimmersWindowController.SupportPet.Add(
                        new TrimmerUiConfig
                        {
                            ItemIds = MechEngiSelectionArray,
                            SettingKey = "SupportMechEngiSelectionArray",
                            Label = "Mech. Engi",
                            UiType = UiType.DropDownWOption
                        });

                    RegisterItemProcessor(MechEngiSelectionArray, (Item itemToUse, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    TrimmerMethod(itemToUse, fightingTarget, ref actionTarget, "MechEngiSelectionArray"), CombatActionPriority.High, RuleContext.InCombat);
                }

                if (ElecEngiSelectionArray.Contains(item.Id))
                {
                    OwnedTrimmers.Add(item.Id);

                    if (DynelManager.LocalPlayer.Profession == Profession.Engineer &&
                        !TrimmersWindowController.SupportPet.Any(c => c.SettingKey == "SupportElecEngiSelectionArray"))
                        TrimmersWindowController.SupportPet.Add(
                        new TrimmerUiConfig
                        {
                            ItemIds = ElecEngiSelectionArray,
                            SettingKey = "SupportElecEngiSelectionArray",
                            Label = "Elec. Engi",
                            UiType = UiType.DropDownWOption
                        });

                    RegisterItemProcessor(ElecEngiSelectionArray, (Item itemToUse, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    TrimmerMethod(itemToUse, fightingTarget, ref actionTarget, "ElecEngiSelectionArray"), CombatActionPriority.High, RuleContext.InCombat);
                }

                #endregion

            }

            if (OwnedItems != null && OwnedItems.Count > 0 && !_buttonDefinitions.Any(x => x.Label == "Items"))
                _buttonDefinitions.Add(("Items", Item_Button_Click));

            if (MaAttacks != null && MaAttacks.Count > 0 && !_buttonDefinitions.Any(x => x.Label == "MA Attacks"))
                _buttonDefinitions.Add(("MA Attacks", MA_Attack_Buttion_Clicked));
        }

        #region Items Bools

        private bool StaffofCleansing(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;

            if (!DynelManager.LocalPlayer.Buffs.Contains(new NanoLine[] { NanoLine.DOT_LineA, NanoLine.DOT_LineB, NanoLine.DOTNanotechnicianStrainA,
            NanoLine.DOTAgentStrainA, NanoLine.DOTNanotechnicianStrainB, NanoLine.Snare, NanoLine.Root, NanoLine.Mezz, NanoLine.InitiativeDebuffs,
            NanoLine.DOTStrainC})) return false;

            actionTarget = (DynelManager.LocalPlayer, false);
            return true;
        }

        private bool ReanimatedCloaks(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["CloakoftheReanimatedOption"].AsBool()) return false;
            if (Game.IsZoning) return false;
            if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) >= 1) return false;
            if (fightingTarget == null) return false;
            if (fightingTarget.IsPlayer) return false;
            if (Item.HasPendingUse) return false;
            if (!item.IsEquipped) return false;
            if (DynelManager.LocalPlayer.Buffs.Contains(274736) || DynelManager.LocalPlayer.Buffs.Contains(NanoLine.ReanimatedCloakBuffs)) return false;
            if (fightingTarget?.MaxHealth < 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;
            if (DynelManager.LocalPlayer.Buffs.Contains(274736)) return false;
            actionTarget = (null, false);
            return true;
        }

        private bool TOTWHeal(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["TotwDocBooksValue"].AsInt32() == 0) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;

            actionTarget.Target = null;

            if (Team.IsInTeam)
                actionTarget.Target = Team.Members.Where(t => t.Character != null && t.Character.IsInLineOfSight && t.Character.IsAlive
                && t.Character.HealthPercent <= _settings["TotwDocBooksValue"].AsInt32() && (item.AttackRange == 0 || TargetInItemRange(item, t.Character))).OrderBy(t => t.Character.HealthPercent).FirstOrDefault()?.Character;
            else if (DynelManager.LocalPlayer.HealthPercent <= _settings["TotwDocBooksValue"].AsInt32())
                actionTarget.Target = DynelManager.LocalPlayer;

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        private bool NonCombatItemBuff(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName)
        {
            if (!_settings[settingName].AsBool()) return false;
            if (!CanUseItem(item)) return false;

            actionTarget = (DynelManager.LocalPlayer, true);
            return true;
        }

        private bool HealthAndNanoStim(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["StimsOption"].AsBool()) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;

            var lp = DynelManager.LocalPlayer;

            if (lp.Buffs.Contains(new NanoLine[] { NanoLine.Root, NanoLine.Snare }) || lp.Buffs.Contains(new int[] { 280470, 258231 })) return false;

            actionTarget.Target = null;

            if (Team.IsInTeam)
                actionTarget.Target = Team.Members.Where(m => m.Character != null && m.Character.IsInLineOfSight && m.Character.IsAlive && (item.AttackRange == 0 || TargetInItemRange(item, m.Character))
                && (m.Character.HealthPercent <= _settings["HealthStimsValue"].AsInt32() || m.Character.NanoPercent <= _settings["NanoStimsValue"].AsInt32()))
                    .OrderByDescending(c => c.Profession == Profession.Doctor || c.Profession == Profession.Enforcer || c.Profession == Profession.Soldier).ThenBy(c => c.Character.HealthPercent).FirstOrDefault()?.Character;
            else if (lp.HealthPercent <= _settings["HealthStimsValue"].AsInt32() || (lp.NanoPercent <= _settings["NanoStimsValue"].AsInt32()))
                actionTarget.Target = lp;

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        private bool FreeStim(Item item, SimpleChar fightingtarget, ref (SimpleChar Target, bool ShouldSetTarget) actiontarget)
        {
            if (Game.IsZoning) return false;
            if (!CanUseItem(item)) return false;

            actiontarget.Target = null;

            if (Team.IsInTeam)
                actiontarget.Target = Team.Members.FirstOrDefault(m => m.Character != null && m.Character.IsInLineOfSight && m.Character.IsAlive && (item.AttackRange == 0 || TargetInItemRange(item, m.Character))
                && (m.Character.Buffs.Contains(new NanoLine[] { NanoLine.Root, NanoLine.Snare, NanoLine.Mezz })))?.Character;
            if (DynelManager.LocalPlayer.Buffs.Contains(new NanoLine[] { NanoLine.Root, NanoLine.Snare, NanoLine.Mezz }))
                actiontarget.Target = DynelManager.LocalPlayer;

            if (actiontarget.Target == null) return false;

            actiontarget = (actiontarget.Target, true);
            return true;
        }

        protected bool ConditionalOption(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null, bool requireTarget = true, bool requireCombat = true)
        {
            if (!_settings[settingName].AsBool()) return false;

            if (requireCombat)
            {
                if (fightingTarget == null) return false;
                if (!CanUseItem(item)) return false;
            }
            else
                if (!CanUseItem(item)) return false;

            if (requireTarget)
            {
                actionTarget = (fightingTarget, true);
                return true;
            }
            else
            {
                actionTarget = (null, true);
                return true;
            }
        }

        protected bool ConditionalOptionWitchBuffCheck(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null, bool requireTarget = true, bool requireCombat = true, NanoLine nanoLine = NanoLine.NOSTACKING)
        {
            if (!_settings[settingName].AsBool()) return false;

            if (DynelManager.LocalPlayer.Buffs.Contains(nanoLine)) return false;

            if (requireCombat)
            {
                if (fightingTarget == null) return false;
                if (!CanUseItem(item)) return false;
            }
            else
                if (!CanUseItem(item)) return false;

            if (requireTarget)
            {
                actionTarget = (fightingTarget, true);
                return true;
            }
            else
            {
                actionTarget = (null, true);
                return true;
            }
        }

        protected bool TargetHealthValue(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null)
        {
            if (_settings[settingName].AsInt32() == 0) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;

            actionTarget.Target = null;

            if (Team.IsInTeam)
            {
                actionTarget.Target = Team.Members.Where(p => p.Character != null && p.Character.Health > 0 && p.Character.IsInLineOfSight && (item.AttackRange == 0 || TargetInItemRange(item, p.Character)) && p.Character.HealthPercent <= _settings[settingName].AsInt32())
                   .OrderBy(p => p.Character.MissingHealth).FirstOrDefault()?.Character;
            }
            else if (DynelManager.LocalPlayer.HealthPercent <= _settings[settingName].AsInt32())
            {

                actionTarget.Target = DynelManager.LocalPlayer;
            }

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        protected bool TargetNanoValue(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null)
        {
            if (_settings[settingName].AsInt32() == 0) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;

            actionTarget.Target = null;

            if (Team.IsInTeam)
            {
                actionTarget.Target = Team.Members.Where(p => p.Character != null && p.Character.Health > 0 && p.Character.IsInLineOfSight && (item.AttackRange == 0 || TargetInItemRange(item, p.Character)) && p.Character.NanoPercent <= _settings[settingName].AsInt32())
                   .OrderBy(p => p.Character.MissingNano).FirstOrDefault()?.Character;
            }
            else if (DynelManager.LocalPlayer.NanoPercent <= _settings[settingName].AsInt32())
            {

                actionTarget.Target = DynelManager.LocalPlayer;
            }

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        protected bool NoTargetHealthValue(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null)
        {
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null && !Attacked) return false;

            if (DynelManager.LocalPlayer.HealthPercent > _settings[settingName].AsInt32()) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }

        protected bool NoTargetNanoValue(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null)
        {
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;

            if (DynelManager.LocalPlayer.NanoPercent > _settings[settingName].AsInt32()) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }

        protected bool ConditionalAttackSelection(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null, bool requireTarget = true, bool aoeRequired = false)
        {
            if (settingName != null && _settings[settingName].AsInt32() == 0) return false;
            if (aoeRequired && !_settings["AOE"].AsBool()) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;

            if (_settings[settingName].AsInt32() == 2 && !(fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))) return false;

            if (requireTarget)
            {
                if (item.AttackRange != 0)
                {
                    if (!TargetInItemRange(item, fightingTarget)) return false;
                }
                else
                {
                    Chat.WriteLine($"{item.Name} as no attack range");
                }

                actionTarget = (fightingTarget, true);
            }
            else
                actionTarget = (null, false);

            return true;
        }

        private bool NonCombatUserAndEquipped(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName, int[] buffIds = null)
        {
            if (fightingTarget != null) return false;
            return _settings[settingName].AsBool() && CanUseItem(item) && item.IsEquipped && (buffIds == null || !DynelManager.LocalPlayer.Buffs.Contains(buffIds));
        }

        private bool NonCombatUserAndEquipped(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName, NanoLine[] buffNanoLines = null)
        {
            if (fightingTarget != null) return false;
            return _settings[settingName].AsBool() && CanUseItem(item) && item.IsEquipped && (buffNanoLines == null || !DynelManager.LocalPlayer.Buffs.Contains(buffNanoLines));
        }

        private bool RezCan(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actiontarget)
        {
            if (Game.IsZoning) return false;
            if (fightingTarget != null) return false;
            if (Attacked) return false;
            if (Item.HasPendingUse) return false;

            if (DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.FirstAid)) return false;

            actiontarget.ShouldSetTarget = false;
            return true;
        }

        private bool InsuranceCan(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (fightingTarget != null) return false;
            if (!CanUseItem(item)) return false;
            if (DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.FirstAid) || DynelManager.LocalPlayer.GetStat(Stat.UnsavedXP) == 0 || DynelManager.LocalPlayer.Buffs.Contains(300727)) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }

        private bool ExpCan(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)//xp buff check?
        {
            if (fightingTarget != null) return false;
            if (!CanUseItem(item)) return false;
            if (DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.FirstAid)) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }

        private bool SitItemUsage(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string nanoSetting = null, string healthSetting = null)
        {
            if (!_settings["KitsOption"].AsBool()) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget != null) return false;
            if (Attacked) return false;
            if (DynelManager.LocalPlayer.MovementState != MovementState.Sit) return false;

            if (!((nanoSetting != null && _settings[nanoSetting].AsInt32() != 0 && DynelManager.LocalPlayer.NanoPercent < _settings[nanoSetting].AsInt32()) ||
                (healthSetting != null && _settings[healthSetting].AsInt32() != 0 && DynelManager.LocalPlayer.HealthPercent < _settings[healthSetting].AsInt32()))) return false;

            actionTarget = (DynelManager.LocalPlayer, true);
            return true;
        }

        #endregion

        #region MA Attacks

        private bool MAItem(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingBool)
        {
            if (!_settings[settingBool].AsBool()) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;
            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool FlowerOfLife(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["FlowerOfLifeOption"].AsBool()) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;

            if (DynelManager.LocalPlayer.HealthPercent > _settings["FlowerOfLifesValue"].AsInt32()) return false;

            return true;
        }

        private bool AngelOfNight(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["AOE"].AsBool()) return false;
            if (!_settings["AngelOfNightOption"].AsBool()) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;
            if (PVPDistance(11)) return false;

            return true;
        }

        #endregion

        #region Checks

        protected bool TargetInItemRange(Item item, SimpleChar target)
        {
            try
            {
                if (item == null) return false;
                if (item.AttackRange == 0) return false;
                if (target == null) return false;
                if (target.Position == null) return false;
                if (DynelManager.LocalPlayer.Position == null) return false;
                if (item.AttackRange == 0) return false;
                if (!item.MeetsUseReqs(target)) return false;
                if (DynelManager.LocalPlayer.Velocity > 0) return false;

                return DynelManager.LocalPlayer.Position.DistanceFrom(target.Position) <= item?.AttackRange;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        protected bool CanUseItem(Item item)
        {
            try
            {
                if (Game.IsZoning) return false;
                if (Now < _lastZonedTime) return false;
                if (_settings["WaitForRez"].AsBool())
                {
                    if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) > 1)
                        return false;
                }
                if (Item.HasPendingUse) return false;
                if (PerkAction.List.Any(perk => perk.IsExecuting)) return false;

                var skillLockStat = item.UseModifiers.FirstOrDefault(m => m.Function == SpellFunction.LockSkill)?.Properties.TryGetValue(SpellPropertyOperator.Stat, out var v) == true ? v : (int?)null;
                if (skillLockStat.HasValue)
                    if (DynelManager.LocalPlayer.Cooldowns.ContainsKey((Stat)skillLockStat.Value)) return false;

                if (SkillLock.Any(x => x.Item1 == item)) return false;

                if (!item.MeetsSelfUseReqs()) return false;

                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        private void UseItems()
        {
            try
            {
                if (Game.IsZoning) return;
                if (Now < _lastZonedTime) return;
                if (Item.HasPendingUse) return;
                if (Spell.Find("Composite Attribute Boost", out Spell spell) && !spell.IsReady) return;
                if (PerkAction.List.Any(p => p.IsExecuting)) return;
                if (DynelManager.LocalPlayer.IsAttacking) return;

                var itemsToUse = new HashSet<string> { "Cell Templates", "Plasmid Cultures", "Mitochondria Samples", "Protein Mapping Data",
                "Clan Bravery Token", "Clan Mission Token", "Omni-Tek Bravery Token", "Omni-Tek Mission Token", "Illegal Notum Chunk" };

                if (_settings["UseCapsule"].AsBool())
                    itemsToUse.Add("Soul Capsule");

                foreach (var item in Inventory.Items.Where(c => c.Slot.Type == IdentityType.Inventory || c.UniqueIdentity.Type == IdentityType.Container))
                {
                    if (itemsToUse.Contains(item.Name))
                    {
                        item.Use();
                        continue;
                    }

                    if (item.UniqueIdentity.Type != IdentityType.Container)
                        continue;

                    foreach (var containerItem in Inventory.GetContainerItems(item.UniqueIdentity))
                    {
                        if (itemsToUse.Contains(containerItem.Name))
                            containerItem.Use();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }
        public static class RelevantGenericItems
        {
            //locks via spell
            //lock buff Will of the Reanimator 274736, buff nanoline 851
            public const int CloakoftheReanimatedGladiator = 274702; // casts self buff
            public const int CloakoftheReanimatedHealer = 274707; // heal eff and nano int and nano delta
            public const int CloakoftheReanimatedIllusionist = 274717; // nano attack 
            public const int CloakoftheReanimatedJester = 274722; // int and evades
            public const int CloakoftheReanimatedRanger = 274696; // add dmg
            public const int CloakoftheReanimatedSummoner = 274712; // heal and heal eff and int


            public static readonly int[] ReanimatedCloaks = new[]
            {
                CloakoftheReanimatedGladiator,
                CloakoftheReanimatedHealer,
                CloakoftheReanimatedIllusionist,
                CloakoftheReanimatedJester,
                CloakoftheReanimatedRanger,
                CloakoftheReanimatedSummoner
            };

            //skillLock=Agg%2FDef
            public static int[] FlurryOfBlows = new[] { 85908, 85907 };//buff
            public static int[] EyeOfTheHunter = new[] { 162400, 162399 };//buff

            //skillLock=Biological+metamorphosis *26 pages*
            public static int[] TOTW_Doc_Books = new[] {
                305514, //Sacred Text of the Immortal One
                206242, //Teachings of the Immortal One
                204765,//Holy Book of the Immortal
            };

            public static int[] RingOfFleshes = new[] {
                305491, //Ring of Blighted Flesh
                206202, //Ring of Putrescent Flesh
                204595, //Ring of Weeping Flesh
            };

            public const int SharlsCyberneticTattoo = 269511; //shade only

            //skillLock=Body+development
            public static int[] TotwShieldShoulders = new[] {
                305476, //Desecrated Flesh
                206015, //Corrupted Flesh
                204698, //Withered Flesh
            };

            public static int[] ToTWBloodRings = new[] {
            305495, //Bloodthrall Ring
            206201, //Bloodslave Ring
            204596  //Bloodleech Ring 
            };

            public static int[] MuscularStim = new[] { 154665, 154664 };

            //skillLock=Bow+special+attack
            public static int[] SpecialArrows = new[]
            {
                158589, //Poison Bamboo - Special Arrows, 177.78 dmg/min
                159064, //Pestering Gadfly - Special Arrows, 75 dmg/min
                158591, //Maple Blaze - Special Arrows, 50 dmg/min
                158590, //Maple Blaze - Special Arrows, 25 dmg/min
                158588, //Poison Bamboo - Special Arrows, 6.67 dmg/min
            };

            public static int[] AOESpecialArrows = new[]
           {
                158597, //Apple Seed - Special Arrows aoe, 200 dmg/min
                158593, //Needle of Pine - Special Arrows aoe, 130 dmg/min
                158595, //Frozen Cherry Blossom - Special Arrows aoe, 116.67 dmg/min
                158594, //Frozen Cherry Blossom - Special Arrows aoe, 41.67 dmg/min
                158596, //Apple Seed - Special Arrows aoe, 8 dmg/min
                158592, //Needle of Pine - Special Arrows aoe, 4 dmg/min
            };

            //skillLock=Burst
            public static int[] SuppressiveBurstItem = new[] { 155708, 155707 };//aoe

            //skillLock=Chemical+AC
            public const int PolymerizingStim = 245167;

            //skillLock=Cold+AC
            public const int MutatedSlitherBlood = 245322;

            //skillLock=Dimach
            public const int TouchOfSaiFung = 275018;
            public const int TheWizdomOfHuzzum = 303056;
            public const int TreeOfEnlightenment = 204607;

            //skillLock=Duck+explosives
            public const int AssaultClassTank = 156576;

            //skillLock=Experience
            public const int IskopsAscendancy = 206063;

            //skillLock=Fire+damage+modifier
            public static int[] BacchantesAnunWings = new[] { 248934, 248933 };

            //skillLock=First+aid *more to add*
            public const int CombatAssistWenWen = 305975;

            public static int[] HealthAndNanoStims = new[] { 291045, 291044, 291043 };
            public const int DeathsDoor = 303071;
            public static int[] FreeStims = new[] { 204107, 204106, 204105, 204104, 204103 };
            public static readonly int[] ExpCans = new[] { 303376, 288772, 288771, 288769, 288788, 288787, 288786, 288792, 288791, 288790 };
            public static int[] RezCans = new[] { 301070, 303390 };
            public static int[] InsuranceCans = new[] { 303389, 300728 };

            //skillLock=Grenade
            public static readonly int[] ThrowingGrenades = new[]
            {
                164781, //HSR Hedgehog 23 Throwing Grenade
                165117, //May Fly Throwing Grenade
                164780, //HSR Hedgehog 23 Throwing Grenade
                165116, //May Fly Throwing Grenade
            };

            //skillLock=Heavy+weapons
            public static int[] MantaAOE = new[] {
                204157, //Manta - Sportline
                203832, //Manta - Sportline
            };
            public static int[] Manta = new[] {
                203841, //Manta - A.V. 
                203830, //Manta - Neo Concept
                203829, //Manta - Neo Concept
                203834, //Manta - A.V.
            };

            //skillLock=Intelligence
            public static int[] PerniciousBodyArmor = new[] { 226458, 226457 };
            public const int RingofSisterPestilence = 267909;
            public const int Enigma = 267522;

            //skillLock=IP
            public static int[] RingofKnowledge = new[] { 70255, 70256 };

            //skillLock=Map+navigation
            public static int[] GnuffsEternalRiftCrystal = new[] { 303179, 303180 };

            //skillLock=Martial+arts
            public const int Sappo = 267525;
            public const int StingoftheViper = 305542;
            public const int ApeFistofKhalum = 204605;
            public const int KarmicFist = 206191;
            public static int[] Shen = new[] { 201281, 20125, 70616, 70618 };
            public static int[] FlowerOfLife = new[] { 204326, 201280, 70614, 70615 };
            public static int[] BrightBlueCloudlessSky = new[] { 201279, 204328, 70617, 70611 };
            public static int[] BlessedWithThunder = new[] { 201278, 204327, 70612, 70613 };
            public static int[] BirdOfPrey = new[] { 201277, 204329, 70609, 70610 };
            public static int[] AttackOfTheSnake = new[] { 201276, 204330, 43146, 43145 };
            public static int[] AngelOfNight = new[] { 201282, 204331, 70607, 70608 };

            //skillLock=Matter+creation
            public static int[] ToTWFlameRings = new[] {

                305493, //Ring of Purifying Flame
                206203, //Ring of Wilting Flame
                204593  //Ring of Tattered Flame
            };
            public static int[] ICCDrone = new[] {
                303189, //ICC Surveillance Drone
                303192, //ICC Arbitration Drone
                303188, //ICC Pacification Drone
            };

            //skillLock=Max+health
            public static int[] BoostedStim = new[] { 55696, 55697 };

            //skillLock=Max+nano
            public static readonly int[] TOTWWrists = { 305513, 204649 };//nt only

            //skillLock=Mechanical+engineering
            public static int[] KamikazeRobotShell = new[] { 156281, 156282 };

            //skillLock=Nano+pool *debuf remover*
            public static int[] StaffofCleansing = new[] { 305526, 303465 };
            public static int[] Rod = new[] { 206017, 204608 };
            public const int PurificationStim = 305978;

            //skillLock=Nano+programming
            public static int[] PremiumNanoRecharger = new[] { 289218, 289217, 289216, 289215, 289214 }; //nano kit
            public static int[] PurgeNanoKit = new[] { 204266, 204265, 204264, 204263, 204262 }; //Free Movement, non combat

            //skillLock=Parry *2 pages*

            //skillLock=Perception
            public static int[] SanguisugentBodyArmor = new[] { 226830, 226829 };
            public const int RingofSisterMerciless = 267907;

            //skillLock=Pharmaceuticals *70 pages*

            //skillLock=Psychic
            public const int BracerofBrotherMalevolence = 301679;
            public const int ViralCommunicationsLarvae = 288281;
            public const int NotumFocus = 158914;

            //skillLock=Psychological+modifications *78 pages*

            //skillLock=Psychology
            public static readonly int[] TauntTools = new[]
           {
                244655,  // Scorpio's Aim of Anger (QL 250) – 8000 taunt, lock: 11s, TPM: 43636
                263255,  // Ephemeral Annoyance (QL 300) – 3600 taunt, lock: 10s, TPM: 21600
                152028,  // Aggression Multiplier (Jealousy Augmented) (QL 200)  – 1800 taunt, lock: 6s, TPM: 18000
                253187,  // Codex of the Insulting Emerto (High) – 1100 taunt, lock:  6s, TPM: 11000
                165111,  // Library of Foul Language (QL 200) – 850 taunt, lock: 6s, TPM: 8500
                83919,   // Aggression Multiplier – 800 taunt, lock: 6s, TPM: 8000
                151693,  // Modified Aggression Enhancer (High) – 800 taunt, lock: 6s, TPM: 8000
                158046,  // Da Taunter! (QL 200) – 800 taunt, lock: 6s, TPM: 8000
                152029,  // Aggression Enhancer (Jealousy Augmented) (QL 1) – 75 taunt, lock: 6s, TPM: 750
                253186,  // Codex of the Insulting Emerto (Low) – 50 taunt, lock: 6s, TPM: 500
                263254,  // Ephemeral Annoyance (QL 1) – 50 taunt, lock: 10s, TPM: 300
                165112,  // Library of Foul Language (QL 1) – 30 taunt, lock: 6s, TPM: 300
                151692,  // Modified Aggression Enhancer (Low) – 25 taunt, lock: 6s, TPM: 250
                83920,   // Aggression Enhancer – 25 taunt, lock: 6s, TPM: 250
                158045   // Da Taunter! (QL 1) – 25 taunt, lock: 6s, TPM: 250
            };

            public const int EmotionalSponge = 152031; //Detaunt

            public static int[] TheRepressor = new[] { 159197, 159196 };//Detaunt

            //skillLock=Quantum+physics
            public const int BoltarBrainBlaster = 254425; //user reflect buff

            //skillLock=Radiation+AC
            public const int BioremediationStim = 245156; //ca buff

            //skillLock=Ranged+energy
            public static int[] WenWen = new[] { 129656, 129655, 129654, 129653, 129652, 129651, 129650, 129649, 129648, 129647, 129646, 129645, 129644, 129643, 129642, 129641, 129640, 129639, 129638 };

            //skillLock=Riposte
            public static int[] DamageShieldProjector = new[] { 70251, 70254 }; //Shield, equiped
            public static int[] UponAWaveOfSummer = new[] { 205406, 205405, 205404 };
            public const int StampedeOfTheBoar = 305554;

            //skillLock=Run+speed
            public const int BootsOfGridspaceDistortion = 305995;
            public static int[] BootsofInfiniteSpeed = new[] { 202723, 202722 };
            public static int[] BurstofSpeedStim = new[] { 85369, 85368 };
            public const int CurseofMalahde = 245903;
            public const int SteamingHotCupOfEnhancedCoffee = 157296;

            //skillLock=Sensory+improvement
            public static int[] TotwBlindRings = new[] { 206204, 204598 };

            //skillLock=Sharp+objects
            public static readonly int[] SharpObjects = new[]
            {
               244216, //Tear of Oedipus
               245990, //Lava capsule
               245323, //Kizzermole Gumboil
               244215, //Heroes Discus
               244214, //Fallen Star
               244211, //Koan Shuriken
               244206, //Chunk of Eternal Ice
               244208, //Poison Darts of the Deceptor
               244209, //Capsule of Fulminating Novictum
               244210, //Everburning Coal
               244205, //Electric Bolts
               244204, //Meteorite Spikes
               244987, //Circus Throwing Dagger
               244986, //Circus Throwing Dagger
               164632, //Aluminum Throwing Dagger
               164633, //Aluminum Throwing Dagger
               164779, //Aluminum Throwing Dagger
               164778, //Aluminum Throwing Dagger
            };

            //skillLock=Smg
            public static readonly int[] ClusterBullets = { 300944, 158952, 158951 };
            public static readonly int[] HomingPermorphaBullets = { 246840, 246839 };

            //skillLock=Stamina
            public const int InnerBalance = 267523;

            //skillLock=Strength
            public static int[] DreadlochEnduranceBooster = new[] { 267168, 267167 };
            public static int[] TotwDmgShoulders = new[] { 305478, 206013, 204653 };
            public const int Delirium = 267922;

            //skillLock=Time+and+space *5 pages*
            public const int ReflectGraft = 95225; //Hacked Boosted-Graft: Lesser Deflection Shield (Extended) 

            public static readonly int[] AncientGenerationDevices = new[]
            {
                267720, // Ancient Fire Generation Device
                267721, // Ancient Cold Generation Device
                267722, // Ancient Chemical Generation Device
                267723, // Ancient Radiation Generation Device
                267724, // Ancient Poison Generation Device
                267725, // Ancient Damage Generation Device
            };

            //skillLock=Treatment *4 pagees*
            public static int[] SitKits = new[] { 297274, 293297, 293296, 291084, 291083, 291082, 292256 };


            public static readonly int[] AndroidNCU = new[] { 269840, 150307, 150306 };


            public static readonly int[] Grid = { 155172, 155173, 155174, 155150 };
            public static readonly int[] ShadowwebSpinner = { 273350, 224400, 224399, 224398, 224397, 224396, 224395, 224394, 224393, 224392, 224390 };
        };

        #endregion
    }
}
