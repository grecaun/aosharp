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
        public static HashSet<int> OwnedPerks = new HashSet<int>();

        public static HashSet<int> DamagePerks = new HashSet<int>();
        //public static HashSet<int> DotPerks = new HashSet<int>();
        public static HashSet<int> DebuffPerks = new HashSet<int>();
        public static HashSet<int> RemoverPerks = new HashSet<int>();
        public static HashSet<int> CombatBuffPerks = new HashSet<int>();

        public void RegisterPerkProcessors()
        {
            foreach (var perk in PerkAction.List)
            {
                #region Heals

                if (perk.Hash == PerkHash.Heal)
                {
                    LoadedHeals.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Heal, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetHealPerk(perkAction, fightingTarget, ref actionTarget, "HealValue"), CombatActionPriority.High);
                }

                if (perk.Hash == PerkHash.HarmonizeBodyAndMind)
                {
                    LoadedHeals.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.HarmonizeBodyAndMind, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        SelfHealPerk(perkAction, fightingTarget, ref actionTarget, "HarmonizeBodyAndMindValue"), CombatActionPriority.High);
                }

                if (perk.Hash == PerkHash.ReconstructDNA)
                {
                    LoadedHeals.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.ReconstructDNA, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        SelfHealPerk(perkAction, fightingTarget, ref actionTarget, "ReconstructDNAValue"), CombatActionPriority.High);
                }


                if (perk.Hash == PerkHash.RegainNano)
                {
                    LoadedHeals.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.RegainNano, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NanoHealSelf(perkAction, fightingTarget, ref actionTarget, "RegainNanoValue"), CombatActionPriority.High);
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Adventurer || DynelManager.LocalPlayer.Profession == Profession.Enforcer || DynelManager.LocalPlayer.Profession == Profession.Engineer || DynelManager.LocalPlayer.Profession == Profession.Keeper)
                {
                    if (perk.Hash == PerkHash.BioRejuvenation)
                    {
                        LoadedHeals.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.BioRejuvenation, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "BioRejuvenationValue"), CombatActionPriority.High, RuleContext.InCombat);
                    }
                    if (perk.Hash == PerkHash.BioRegrowth)
                    {
                        LoadedHeals.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.BioRegrowth, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetHealPerk(perkAction, fightingTarget, ref actionTarget, "BioRegrowthValue"), CombatActionPriority.High);
                    }
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Enforcer || DynelManager.LocalPlayer.Profession == Profession.Soldier)
                {
                    if (perk.Hash == PerkHash.DrawBlood)
                    {
                        LoadedHeals.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.DrawBlood, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LifeTapPerk(perkAction, fightingTarget, ref actionTarget, "DrawBloodValue"), CombatActionPriority.High);
                    }
                    if (perk.Hash == PerkHash.Lifeblood)
                    {
                        LoadedHeals.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Lifeblood, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SelfHealPerk(perkAction, fightingTarget, ref actionTarget, "LifebloodValue"), CombatActionPriority.High);
                    }
                    if (perk.Hash == PerkHash.BlessingOfLife)
                    {
                        LoadedHeals.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.BlessingOfLife, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "BlessingOfLifeValue"), CombatActionPriority.High);
                    }
                }

                if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician || DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                {
                    if (perk.Hash == PerkHash.AccessNotumSource)
                    {
                        LoadedHeals.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.AccessNotumSource, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        NanoHealTeam(perkAction, fightingTarget, ref actionTarget, "AccessNotumSourceValue"), CombatActionPriority.High);
                    }
                }

                if (DynelManager.LocalPlayer.Breed == Breed.Solitus)
                {
                    if (perk.Hash == PerkHash.Survival)
                    {
                        LoadedHeals.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Survival, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SelfHealPerk(perkAction, fightingTarget, ref actionTarget, "SurvivalValue"), CombatActionPriority.High);
                    }
                }

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Adventurer:
                        if (perk.Hash == PerkHash.Devour)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Devour, DevourLifeTapPerk, CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.Awakening)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Awakening, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "AwakeningValue"), CombatActionPriority.High);
                        }
                        break;
                    case Profession.Doctor:
                        if (perk.Hash == PerkHash.EnhancedHeal)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.EnhancedHeal, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                SelfHealPerk(perkAction, fightingTarget, ref actionTarget, "EnhancedHealValue"), CombatActionPriority.High, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.BattlegroupHeal1)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BattlegroupHeal1, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "BattlegroupHeal1Value"), CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.BattlegroupHeal2)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BattlegroupHeal2, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "BattlegroupHeal2Value"), CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.BattlegroupHeal3)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BattlegroupHeal3, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "BattlegroupHeal3Value"), CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.BattlegroupHeal4)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BattlegroupHeal4, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "BattlegroupHeal4Value"), CombatActionPriority.High, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.TeamHeal)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TeamHeal, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "TeamHealValue"), CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.CloseCall)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.CloseCall, CloseCall, CombatActionPriority.Low);
                        }

                        break;
                    case Profession.Engineer:
                        if (perk.Hash == PerkHash.Reconstruction)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Reconstruction, PetHealPerk, CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.RepairPet)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.RepairPet, PetHealPerk, CombatActionPriority.High);
                        }
                        break;
                    case Profession.Keeper:
                        if (perk.Hash == PerkHash.CuringTouch)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.CuringTouch, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TargetHealPerk(perkAction, fightingTarget, ref actionTarget, "CuringTouchValue"), CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.LayOnHands)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.LayOnHands, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TargetHealPerk(perkAction, fightingTarget, ref actionTarget, "LayOnHandsValue"), CombatActionPriority.High);
                        }
                        break;

                    case Profession.MartialArtist:

                        if (perk.Hash == PerkHash.BalanceOfYinAndYang)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BalanceOfYinAndYang, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                SelfHealPerk(perkAction, fightingTarget, ref actionTarget, "BalanceOfYinAndYangValue"), CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.RedDawn)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.RedDawn, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                SelfHealPerk(perkAction, fightingTarget, ref actionTarget, "RedDawnValue"), CombatActionPriority.High);
                        }
                        break;
                    case Profession.Metaphysicist:
                        if (perk.Hash == PerkHash.SpiritOfPurity)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SpiritOfPurity, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                NanoHealTeam(perkAction, fightingTarget, ref actionTarget, "SpiritOfPurityValue"), CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.SpiritOfBlessing)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SpiritOfBlessing, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "SpiritOfBlessingValue"), CombatActionPriority.High);
                        }
                        break;
                    case Profession.NanoTechnician:
                        if (perk.Hash == PerkHash.NanoHeal)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.NanoHeal, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                NanoHealTeam(perkAction, fightingTarget, ref actionTarget, "NanoHealValue"), CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.TapNotumSource)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TapNotumSource, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                NanoHealTarget(perkAction, fightingTarget, ref actionTarget, "TapNotumSourceValue"), CombatActionPriority.High);
                        }
                        break;
                    case Profession.Shade:

                        if (perk.Hash == PerkHash.ConsumeTheSoul)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ConsumeTheSoul, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                LifeTapPerk(perkAction, fightingTarget, ref actionTarget, "ConsumeTheSoulValue"), CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.Exultation)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Exultation, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                LifeTapPerk(perkAction, fightingTarget, ref actionTarget, "ExultationValue"), CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.Diffuse)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Diffuse, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                LifeTapPerk(perkAction, fightingTarget, ref actionTarget, "DiffuseValue"), CombatActionPriority.High);
                        }
                        break;
                    case Profession.Soldier:
                        if (perk.Hash == PerkHash.FieldBandage)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.FieldBandage, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                SelfHealPerk(perkAction, fightingTarget, ref actionTarget, "FieldBandageValue"), CombatActionPriority.High);
                        }

                        break;
                    case Profession.Trader:
                        if (perk.Hash == PerkHash.ReapLife)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ReapLife, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                LifeTapPerk(perkAction, fightingTarget, ref actionTarget, "ReapLifeValue"), CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.TapVitae)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TapVitae, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                LifeTapPerk(perkAction, fightingTarget, ref actionTarget, "TapVitaeValue"), CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.VitalShock)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.VitalShock, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                LifeTapPerk(perkAction, fightingTarget, ref actionTarget, "VitalShockValue"), CombatActionPriority.High);
                        }
                        if (perk.Hash == PerkHash.PurpleHeart)
                        {
                            LoadedHeals.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.PurpleHeart, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                TeamHealPerk(perkAction, fightingTarget, ref actionTarget, "PurpleHeartValue"), CombatActionPriority.High, RuleContext.InCombat);
                        }
                        break;
                }

                #endregion

                #region Genome Perks

                switch (DynelManager.LocalPlayer.Breed)
                {
                    case Breed.Atrox:

                        if (perk.Hash == PerkHash.BodyTackle)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BodyTackle, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;

                    case Breed.Nanomage:

                        if (perk.Hash == PerkHash.Reject)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Reject, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;

                    case Breed.Opifex:

                        if (perk.Hash == PerkHash.DizzyingHeights)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.DizzyingHeights, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Opening)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Opening, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;

                    case Breed.Solitus:

                        if (perk.Hash == PerkHash.Feel)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Feel, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;
                }

                #endregion

                #region Profession

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Adventurer:

                        if (perk.Hash == PerkHash.BleedingWounds)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BleedingWounds, BleedingWounds, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.GuttingBlow)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.GuttingBlow, GuttingBlow, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.LightBullet)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.LightBullet, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.LightKiller)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.LightKiller, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.ShadowStab)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ShadowStab, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.ShadowKiller)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ShadowKiller, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.NocturnalStrike)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.NocturnalStrike, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Agent:

                        if (perk.Hash == PerkHash.Tranquilizer)
                        {
                            DebuffPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Tranquilizer, DebuffPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.ChaoticModulation)
                        {
                            CombatBuffPerks.Add((int)perk.Hash);

                            RegisterPerkProcessor(PerkHash.ChaoticModulation, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "ChaoticModulationSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.PinpointStrike)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.PinpointStrike, PinpointStrike, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.DeathStrike)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.DeathStrike, DeathStrike, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.ShadowBullet)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ShadowBullet, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.SnipeShot1)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SnipeShot1, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.SnipeShot2)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SnipeShot2, SnipeShot2, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Recalibrate)
                        {
                            CombatBuffPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Recalibrate, CombatSelfBuffPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.SilentPlague)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SilentPlague, DotPerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Bureaucrat:

                        if (perk.Hash == PerkHash.Antitrust)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Antitrust, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Doctor:

                        if (perk.Hash == PerkHash.ViralCombination)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ViralCombination, ViralCombinationPerk, CombatActionPriority.Low);
                        }

                        if (perk.Hash == PerkHash.Cure1)
                        {
                            RemoverPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Cure1, AAO_Dots_CleansePerk, CombatActionPriority.VeryHigh);
                        }

                        if (perk.Hash == PerkHash.Vaccinate1)
                        {
                            RemoverPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Vaccinate1, Trader_Debuff_CleansePerk, CombatActionPriority.VeryHigh);
                        }

                        if (perk.Hash == PerkHash.Cure2)
                        {
                            RemoverPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Cure2, AAO_Dots_CleansePerk, CombatActionPriority.VeryHigh);
                        }

                        if (perk.Hash == PerkHash.Vaccinate2)
                        {
                            RemoverPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Vaccinate2, Trader_Debuff_CleansePerk, CombatActionPriority.VeryHigh);
                        }

                        if (perk.Hash == PerkHash.HaleAndHearty)
                        {
                            RemoverPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.HaleAndHearty, HaleandHearty, CombatActionPriority.VeryHigh);
                        }

                        if (perk.Hash == PerkHash.TeamHaleAndHearty)
                        {
                            RemoverPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TeamHaleAndHearty, TeamHaleandHearty, CombatActionPriority.VeryHigh);
                        }

                        if (perk.Hash == PerkHash.Mistreatment)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Mistreatment, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.NanoTransmission)
                        {
                            CombatBuffPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.NanoTransmission, NanoTransmission);
                        }

                        break;

                    case Profession.Enforcer:
                        if (perk.Hash == PerkHash.Avalanche)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Avalanche, AOE_Combat_Perk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.DisableNaturalHealing)
                        {
                            DebuffPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.DisableNaturalHealing, DebuffPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.StoneFist)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.StoneFist, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209857), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.OverwhelmingMight)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.OverwhelmingMight, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 226028), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Pulverize)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Pulverize, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.SeismicSmash)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SeismicSmash, AOE_Combat_Perk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Charge)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Charge, ChargePerk, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Headbutt)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Headbutt, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Engineer:

                        if (perk.Hash == PerkHash.InstallExplosiveDevice)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.InstallExplosiveDevice, DirectDamagePerk, CombatActionPriority.Medium);
                        }

                        if (perk.Hash == PerkHash.Medallion)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Medallion, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Fixer:
                        if (perk.Hash == PerkHash.NCUBooster)
                        {
                            LoadedNonCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.NCUBooster, NCUBoosterPerk, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (perk.Hash == PerkHash.ECM1)
                        {
                            RemoverPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ECM1, NanoShutdown_TraderDebuff_CleansePerk, CombatActionPriority.VeryHigh);
                        }

                        if (perk.Hash == PerkHash.ECM2)
                        {
                            RemoverPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ECM2, NanoShutdown_TraderDebuff_CleansePerk, CombatActionPriority.VeryHigh);
                        }

                        if (perk.Hash == PerkHash.TriggerHappy)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TriggerHappy, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.EatBullets)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.EatBullets, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.PowerBolt)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.PowerBolt, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Numb)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Numb, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Cripple)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Cripple, CripplePerk, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Keeper:

                        if (perk.Hash == PerkHash.MarkOfSufferance)
                        {
                            RemoverPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.MarkOfSufferance, AAO_Dots_CleansePerk, CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.DeepCuts)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.DeepCuts, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.HonoringTheAncients)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.HonoringTheAncients, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.SeppukuSlash)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SeppukuSlash, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.ForceOpponent)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ForceOpponent, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Purify)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Purify, AOE_Combat_Perk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        switch (DynelManager.LocalPlayer.Side)
                        {
                            case Side.Clan:
                                RegisterPerkProcessor(PerkHash.RighteousWrath, WrathAttack, CombatActionPriority.Low);
                                break;
                            case Side.Neutral:
                                RegisterPerkProcessor(PerkHash.SpectatorWrath, WrathAttack, CombatActionPriority.Low);
                                break;
                            case Side.OmniTek:
                                RegisterPerkProcessor(PerkHash.UnhallowedWrath, WrathAttack, CombatActionPriority.Low);
                                break;
                        }

                        break;

                    case Profession.MartialArtist:

                        if (perk.Hash == PerkHash.ChiConductor)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ChiConductor, DirectDamagePerk, CombatActionPriority.Medium);
                        }

                        if (perk.Hash == PerkHash.FleshQuiver)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.FleshQuiver, DirectDamagePerk, CombatActionPriority.Medium);
                        }

                        if (perk.Hash == PerkHash.Obliterate)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Obliterate, DirectDamagePerk, CombatActionPriority.High);
                        }

                        break;

                    case Profession.Metaphysicist:

                        if (perk.Hash == PerkHash.KenSi)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.KenSi, DotPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.KaMon)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.KaMon, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;

                    case Profession.NanoTechnician:
                        if (perk.Hash == PerkHash.BreachDefenses)
                        {
                            DebuffPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BreachDefenses, DebuffPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.FadeAnger)//detaunt
                        {

                            RegisterPerkProcessor(PerkHash.FadeAnger, DetauntPerk, CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.Utilize)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Utilize, UtilizePerk, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Shade:

                        if (perk.Hash == PerkHash.DimensionalFist)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.DimensionalFist, DimensionalFist, CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.Symbiosis)
                        {
                            CombatBuffPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Symbiosis, SymbiosisPerk, CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.Atrophy)
                        {
                            DebuffPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Atrophy, DebuffPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.SpiritDissolution)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SpiritDissolution, SpiritDissolutionPerk, CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.ChaosRitual)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ChaosRitual, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.CaptureVigor)
                        {
                            CombatBuffPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.CaptureVigor, SpiritPhylacteryPerkStart, CombatActionPriority.VeryLow, RuleContext.InCombat);
                        }

                        if (SpiritPhylactery.Contains(perk.Hash))
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(perk.Hash, SpiritPhylacteryPerk, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.RitualOfDevotion)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.RitualOfDevotion, TotemicRitesPerkStart, CombatActionPriority.VeryLow, RuleContext.InCombat);
                        }

                        if (TotemicRites.Contains(perk.Hash))
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(perk.Hash, TotemicRitesPerk, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Stab)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Stab, PiercingMasteryPerkStartStab, CombatActionPriority.VeryLow, RuleContext.InCombat);
                        }

                        //if (perk.Hash == PerkHash.Perforate)
                        //{
                        //    DamagePerks.Add((int)perk.Hash);
                        //    RegisterPerkProcessor(PerkHash.Perforate, PiercingMasteryPerkStartPerforate, CombatActionPriority.VeryLow, RuleContext.InCombat);
                        //}

                        if (PiercingMastery.Contains(perk.Hash))
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(perk.Hash, PiercingMasteryPerk, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Soldier:

                        if (perk.Hash == PerkHash.ContainedBurst)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ContainedBurst, ContainedBursPerk, CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.Guardian)
                        {
                            CombatBuffPerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Guardian, GuardianPerk, CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.WeaponBash)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.WeaponBash, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Clipfever)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Clipfever, AOE_Combat_Perk, CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.MuzzleOverload)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.MuzzleOverload, DirectDamagePerk, CombatActionPriority.High);
                        }

                        break;

                    case Profession.Trader:

                        if (perk.Hash == PerkHash.Bloodletting)
                        {
                            DamagePerks.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Bloodletting, DotPerk, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        break;
                }

                #endregion

                #region Group Perks

                #region Alchemist

                if (DynelManager.LocalPlayer.Profession == Profession.Trader ||
                    DynelManager.LocalPlayer.Profession == Profession.Engineer ||
                    DynelManager.LocalPlayer.Profession == Profession.Doctor)
                {
                    if (perk.Hash == PerkHash.TaintWounds)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.TaintWounds, DotPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.PoisonSprinkle)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.PoisonSprinkle, DotPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.SealWounds)
                    {
                        RemoverPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.SealWounds, SealWoundsPerk, CombatActionPriority.VeryHigh);
                    }
                }

                #endregion

                #region BluntMastery

                if (DynelManager.LocalPlayer.Profession == Profession.Enforcer ||
                    DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                {
                    if (perk.Hash == PerkHash.QuickBash)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.QuickBash, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.DevastatingBlow)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.DevastatingBlow, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209811), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region Brawler

                if (DynelManager.LocalPlayer.Profession == Profession.Enforcer ||
                    DynelManager.LocalPlayer.Profession == Profession.MartialArtist)
                {
                    if (perk.Hash == PerkHash.BigSmash)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.BigSmash, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.FollowupSmash)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.FollowupSmash, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209815), CombatActionPriority.High, RuleContext.InCombat);
                    }
                }

                #endregion

                #region Counterweight

                if (DynelManager.LocalPlayer.Profession == Profession.Adventurer ||
                    DynelManager.LocalPlayer.Profession == Profession.Enforcer ||
                    DynelManager.LocalPlayer.Profession == Profession.Shade)
                {
                    if (perk.Hash == PerkHash.Confinement)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Confinement, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 252397), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region EdgedMastery

                if (DynelManager.LocalPlayer.Profession == Profession.Adventurer ||
                    DynelManager.LocalPlayer.Profession == Profession.Enforcer)
                {
                    if (perk.Hash == PerkHash.QuickCut)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.QuickCut, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.Flay)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Flay, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209843), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.FlurryOfCuts)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.FlurryOfCuts, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209843), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.RibbonFlesh)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.RibbonFlesh, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209843), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region IllogicalPatterns

                if (DynelManager.LocalPlayer.Profession == Profession.Trader ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer ||
                    DynelManager.LocalPlayer.Profession == Profession.Engineer)
                {
                    if (perk.Hash == PerkHash.MemoryScrabble)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.MemoryScrabble, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 252447), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region Mountaineer

                if (DynelManager.LocalPlayer.Profession == Profession.Adventurer ||
                    DynelManager.LocalPlayer.Profession == Profession.Agent ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer ||
                    DynelManager.LocalPlayer.Profession == Profession.Enforcer ||
                    DynelManager.LocalPlayer.Profession == Profession.Soldier ||
                    DynelManager.LocalPlayer.Profession == Profession.Keeper)
                {
                    if (perk.Hash == PerkHash.DetonateStoneworks)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.DetonateStoneworks, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 227164), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region NanoDoctorate

                if (DynelManager.LocalPlayer.Profession == Profession.Doctor ||
                    DynelManager.LocalPlayer.Profession == Profession.Agent ||
                    DynelManager.LocalPlayer.Profession == Profession.Bureaucrat ||
                    DynelManager.LocalPlayer.Profession == Profession.Trader ||
                    DynelManager.LocalPlayer.Profession == Profession.Metaphysicist ||
                    DynelManager.LocalPlayer.Profession == Profession.NanoTechnician)
                {
                    if (perk.Hash == PerkHash.SkillDrainRemoval)
                    {
                        RemoverPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.SkillDrainRemoval, Trader_Debuff_CleansePerk, CombatActionPriority.VeryHigh);
                    }

                    if (perk.Hash == PerkHash.ShutdownRemoval)
                    {
                        RemoverPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.ShutdownRemoval, ShutdownRemovalPerk, CombatActionPriority.VeryHigh);
                    }
                }

                #endregion

                #region NotumSource && TheoreticalResearch

                if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician ||
                    DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                {
                    if (perk.Hash == PerkHash.BlastNano)
                    {
                        DebuffPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.BlastNano, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209903), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.AccelerateDecayingQuarks)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.AccelerateDecayingQuarks, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffAndTargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 274391, 211367), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                }

                #endregion

                #region PistolMastery

                if (DynelManager.LocalPlayer.Profession == Profession.Doctor ||
                    DynelManager.LocalPlayer.Profession == Profession.Adventurer ||
                    DynelManager.LocalPlayer.Profession == Profession.Engineer ||
                    DynelManager.LocalPlayer.Profession == Profession.Bureaucrat ||
                    DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                {
                    if (perk.Hash == PerkHash.QuickShot)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.QuickShot, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.DoubleShot)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.DoubleShot, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209924), CombatActionPriority.High, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.Deadeye)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Deadeye, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region PowerUp

                if (DynelManager.LocalPlayer.Profession == Profession.Doctor ||
                    DynelManager.LocalPlayer.Profession == Profession.Trader ||
                    DynelManager.LocalPlayer.Profession == Profession.Soldier ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer ||
                    DynelManager.LocalPlayer.Profession == Profession.Engineer ||
                    DynelManager.LocalPlayer.Profession == Profession.Agent)
                {

                    if (perk.Hash == PerkHash.PowerVolley)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.PowerVolley, DirectDamagePerk, CombatActionPriority.Medium, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.PowerShock)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.PowerShock, DirectDamagePerk, CombatActionPriority.Medium, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.PowerBlast)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.PowerBlast, DirectDamagePerk, CombatActionPriority.Medium, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.PowerCombo)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.PowerCombo, DirectDamagePerk, CombatActionPriority.Medium, RuleContext.InCombat);
                    }
                }

                #endregion

                #region Ranger

                if (DynelManager.LocalPlayer.Profession == Profession.Agent ||
                    DynelManager.LocalPlayer.Profession == Profession.MartialArtist ||
                    DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                {
                    if (perk.Hash == PerkHash.Clearshot)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Clearshot, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.Popshot)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Popshot, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.Clearsight)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Clearsight, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region Reaver

                if (DynelManager.LocalPlayer.Profession == Profession.Enforcer ||
                    DynelManager.LocalPlayer.Profession == Profession.Keeper)
                {
                    if (perk.Hash == PerkHash.Cleave)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Cleave, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.Transfix)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Transfix, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 226024), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.PainLance)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.PainLance, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 226024), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.SliceAndDice)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.SliceAndDice, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 226024), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region RifleMastery

                if (DynelManager.LocalPlayer.Profession == Profession.Soldier ||
                    DynelManager.LocalPlayer.Profession == Profession.Agent)
                {

                    if (perk.Hash == PerkHash.FindTheFlaw)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.FindTheFlaw, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.CalledShot)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.CalledShot, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region ShotgunMastery

                if (DynelManager.LocalPlayer.Profession == Profession.Trader ||
                    DynelManager.LocalPlayer.Profession == Profession.Soldier ||
                    DynelManager.LocalPlayer.Profession == Profession.Engineer)
                {
                    if (perk.Hash == PerkHash.EasyShot)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.EasyShot, WeaponTypePerk, CombatActionPriority.Low);
                    }

                    if (perk.Hash == PerkHash.PointBlank)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.PointBlank, WeaponTypePerk, CombatActionPriority.Low);
                    }
                }

                #endregion

                #region SMGMastery

                if (DynelManager.LocalPlayer.Profession == Profession.Soldier ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer)
                {

                    if (perk.Hash == PerkHash.SolidSlug)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.SolidSlug, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209945), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.NeutroniumSlug)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.NeutroniumSlug, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209945), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region SpatialDisplacement

                if (DynelManager.LocalPlayer.Profession == Profession.Shade ||
                    DynelManager.LocalPlayer.Profession == Profession.Keeper ||
                    DynelManager.LocalPlayer.Profession == Profession.MartialArtist ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer ||
                    DynelManager.LocalPlayer.Profession == Profession.Adventurer)
                {
                    if (perk.Hash == PerkHash.Removal1)
                    {
                        RemoverPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Removal1, Removal1Perk, CombatActionPriority.VeryHigh);
                    }

                    if (perk.Hash == PerkHash.Removal2)
                    {
                        RemoverPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Removal2, Removal2Perk, CombatActionPriority.VeryHigh);
                    }

                    if (perk.Hash == PerkHash.Purge1)
                    {
                        RemoverPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Purge1, Root_Snare_CleansePerk, CombatActionPriority.VeryHigh);
                    }

                    if (perk.Hash == PerkHash.Purge2)
                    {
                        RemoverPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Purge2, Root_Snare_CleansePerk, CombatActionPriority.VeryHigh);
                    }

                    if (perk.Hash == PerkHash.GreatPurge)
                    {
                        RemoverPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.GreatPurge, Root_Snare_CleansePerk, CombatActionPriority.VeryHigh);
                    }
                }

                #endregion

                #region Starfall

                if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician ||
                    DynelManager.LocalPlayer.Profession == Profession.Metaphysicist ||
                    DynelManager.LocalPlayer.Profession == Profession.Doctor ||
                    DynelManager.LocalPlayer.Profession == Profession.Bureaucrat)
                {

                    if (perk.Hash == PerkHash.Combust)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Combust, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209973), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.ThermalDetonation)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.ThermalDetonation, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209974), CombatActionPriority.Low, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.Supernova)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Supernova, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209975), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region TheUnknownFactor

                if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist ||
                    DynelManager.LocalPlayer.Profession == Profession.NanoTechnician ||
                    DynelManager.LocalPlayer.Profession == Profession.Doctor ||
                    DynelManager.LocalPlayer.Profession == Profession.Agent ||
                    DynelManager.LocalPlayer.Profession == Profession.Trader ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer ||
                    DynelManager.LocalPlayer.Profession == Profession.Engineer)
                {


                    if (perk.Hash == PerkHash.ChaoticAssumption)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.ChaoticAssumption, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 252460), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #region Tinkerer

                if (DynelManager.LocalPlayer.Profession == Profession.Trader ||
                    DynelManager.LocalPlayer.Profession == Profession.Engineer)
                {
                    if (perk.Hash == PerkHash.SabotageQuarkField)
                    {
                        DebuffPerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.SabotageQuarkField, DebuffPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.IgnitionFlare)
                    {
                        DamagePerks.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.IgnitionFlare, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                #endregion

                #endregion

                #region General

                #region ChampionofHeavyArtillery

                if (perk.Hash == PerkHash.FireFrenzy)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.FireFrenzy, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (perk.Hash == PerkHash.Fuzz)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Fuzz, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 253089), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region ChampionofHeavyInfantry

                if (perk.Hash == PerkHash.Bluntness)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Bluntness, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (perk.Hash == PerkHash.Break)
                {
                    DebuffPerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Break, DebuffPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                }

                #endregion

                #region ChampionofLightArtillery

                if (perk.Hash == PerkHash.Collapser)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Collapser, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (perk.Hash == PerkHash.Implode)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Implode, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region ChampionofLightInfantry

                if (perk.Hash == PerkHash.Crave)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Crave, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (perk.Hash == PerkHash.Bore)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Bore, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region ChampionofNanoCombat

                if (perk.Hash == PerkHash.NanoFeast)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.NanoFeast, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (perk.Hash == PerkHash.BotConfinement)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.BotConfinement, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region CombatKnowledge

                if (perk.Hash == PerkHash.InitialStrike)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.InitialStrike, DirectDamagePerk, CombatActionPriority.High, RuleContext.InCombat);
                }

                //if (perk.Hash == PerkHash.RedeemLastWish)
                //{
                //    OwnedPerks.Add((int)perk.Hash);
                //    RegisterPerkProcessor(PerkHash.RedeemLastWish, ...);
                //}

                #endregion

                #region EnhanceDNA

                if (perk.Hash == PerkHash.ViralWipe)
                {
                    RemoverPerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.ViralWipe, ViralWipePerk, CombatActionPriority.VeryHigh);
                }

                #endregion

                #region FirstAid



                #endregion

                #region Genius



                #endregion

                #region KungFuMaster

                if (perk.Hash == PerkHash.TremorHand)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.TremorHand, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region Nanomorph

                if (perk.Hash == PerkHash.AssumeTarget)
                {
                    DebuffPerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.AssumeTarget, DebuffPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                }

                //if (perk.Hash == PerkHash.PeelLayers)
                //{
                //    OwnedPerks.Add((int)perk.Hash);
                //    RegisterPerkProcessor(PerkHash.PeelLayers, ...);
                //}

                #endregion

                #region NotumRepulsor

                if (perk.Hash == PerkHash.StripNano)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.StripNano, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        LocalBuffAndTargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209903, 209910), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (perk.Hash == PerkHash.AnnihilateNotumMolecules)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.AnnihilateNotumMolecules, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        TargetBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 209912), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region Opportunist

                if (perk.Hash == PerkHash.OpportunityKnocks)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.OpportunityKnocks, DirectDamagePerk, CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (perk.Hash == PerkHash.ControlledChance)
                {
                    DamagePerks.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.ControlledChance, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        LocalBuffCheckPerk(perkAction, fightingTarget, ref actionTarget, 252474), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #endregion

                #region LEPocs

                if (ProcType1Map.ContainsKey(perk.Hash))
                {
                    OwnedProcsType1.Add((int)perk.Hash);
                    RegisterPerkProcessor(perk.Hash, LEProc1, CombatActionPriority.Low);
                }

                if (ProcType2Map.ContainsKey(perk.Hash))
                {
                    OwnedProcsType2.Add((int)perk.Hash);
                    RegisterPerkProcessor(perk.Hash, LEProc2, CombatActionPriority.Low);
                }

                #endregion

                #region CombatBuff

                switch (DynelManager.LocalPlayer.Breed)
                {
                    case Breed.Atrox:

                        if (perk.Hash == PerkHash.MongoRage)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.MongoRage, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "MongoRageSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.WitOfTheAtrox)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.WitOfTheAtrox, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "WitOfTheAtroxValue", true), CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.MyOwnFortress)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.MyOwnFortress, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "MyOwnFortressValue", true), CombatActionPriority.High);
                        }

                        break;

                    case Breed.Nanomage:

                        if (perk.Hash == PerkHash.NotumShield)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.NotumShield, NotumShield, CombatActionPriority.High, RuleContext.InCombat);
                        }

                        break;

                    case Breed.Solitus:

                        if (perk.Hash == PerkHash.TackyHack)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TackyHack, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "TackyHackSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Sphere)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Sphere, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                DelayedCombatTeamBuff(perkAction, fightingTarget, ref actionTarget, "SphereValue"), CombatActionPriority.High);
                        }

                        break;
                }

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Adventurer:
                        if (perk.Hash == PerkHash.PowerOfLight)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.PowerOfLight, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "PowerOfLightSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.BladeOfNight)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BladeOfNight, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "BladeOfNightSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Beckoning)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Beckoning, Beckoning, CombatActionPriority.Medium);
                        }
                        break;

                    case Profession.Agent:

                        if (perk.Hash == PerkHash.ToxicShock)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ToxicShock, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "ToxicShockSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.NightKiller)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(perk.Hash, NightKillerPerk, CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Bureaucrat:
                        if (perk.Hash == PerkHash.DodgeTheBlame)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.DodgeTheBlame, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "DodgeTheBlameValue", true), CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.Overrule)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Overrule, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "OverruleSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Enforcer:
                        if (perk.Hash == PerkHash.TrollForm)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TrollForm, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "TrollFormValue", true), CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.HammerAndAnvil)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.HammerAndAnvil, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "HammerAndAnvilSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Highway)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Highway, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "HighwaySelection"), CombatActionPriority.High);
                        }

                        break;

                    case Profession.Engineer:
                        if (perk.Hash == PerkHash.FreakShield)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.FreakShield, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "FreakShieldSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        break;

                    case Profession.Keeper:
                        if (perk.Hash == PerkHash.DevotionalArmor)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.DevotionalArmor, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "DevotionalArmorSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.BladeWhirlwind)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BladeWhirlwind, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "BladeWhirlwindSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Insight)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Insight, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "InsightSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        break;

                    case Profession.MartialArtist:
                        if (perk.Hash == PerkHash.Moonmist)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Moonmist, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "MoonmistSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }


                        break;

                    case Profession.NanoTechnician:
                        if (perk.Hash == PerkHash.FlimFocus)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.FlimFocus, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "FlimFocusSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.ProgramOverload)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ProgramOverload, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "ProgramOverloadSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }
                        break;

                    case Profession.Soldier:
                        if (perk.Hash == PerkHash.Violence)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Violence, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "ViolenceSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.SupressiveHorde)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SupressiveHorde, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "SupressiveHordeSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }


                        break;

                    case Profession.Trader:
                        if (perk.Hash == PerkHash.Sacrifice)
                        {
                            LoadedCombatBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Sacrifice, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "SacrificeSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        break;
                }

                #region Acrobat

                if (DynelManager.LocalPlayer.Profession == Profession.Fixer ||
                    DynelManager.LocalPlayer.Profession == Profession.MartialArtist ||
                    DynelManager.LocalPlayer.Profession == Profession.Shade ||
                    DynelManager.LocalPlayer.Profession == Profession.Adventurer)
                {
                    if (perk.Hash == PerkHash.Limber)
                    {
                        LoadedCombatBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Limber, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "LimberValue", false), CombatActionPriority.High);
                    }

                    if (perk.Hash == PerkHash.DanceOfFools)
                    {
                        LoadedCombatBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.DanceOfFools, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "DanceOfFoolsValue", false), CombatActionPriority.High);
                    }
                }

                #endregion

                #region BioShielding

                if (DynelManager.LocalPlayer.Profession == Profession.Adventurer ||
                    DynelManager.LocalPlayer.Profession == Profession.Enforcer ||
                    DynelManager.LocalPlayer.Profession == Profession.Engineer ||
                    DynelManager.LocalPlayer.Profession == Profession.Keeper)
                {
                    if (perk.Hash == PerkHash.BioShield)
                    {
                        LoadedCombatBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.BioShield, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "BioShieldValue", true), CombatActionPriority.High);
                    }

                    if (perk.Hash == PerkHash.BioCocoon)
                    {
                        LoadedCombatBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.BioCocoon, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "BioCocoonValue", true), CombatActionPriority.High);
                    }
                }

                #endregion

                #region CarefulinBattle

                if (DynelManager.LocalPlayer.Profession == Profession.Shade ||
                    DynelManager.LocalPlayer.Profession == Profession.MartialArtist ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer ||
                    DynelManager.LocalPlayer.Profession == Profession.Bureaucrat)
                {
                    if (perk.Hash == PerkHash.EvasiveStance)
                    {
                        LoadedCombatBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.EvasiveStance, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "EvasiveStanceValue", false), CombatActionPriority.High);
                    }
                }

                #endregion

                #region Mountaineer

                if (DynelManager.LocalPlayer.Profession == Profession.Adventurer ||
                    DynelManager.LocalPlayer.Profession == Profession.Agent ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer ||
                    DynelManager.LocalPlayer.Profession == Profession.Enforcer ||
                    DynelManager.LocalPlayer.Profession == Profession.Soldier ||
                    DynelManager.LocalPlayer.Profession == Profession.Keeper)
                {
                    if (perk.Hash == PerkHash.EncaseInStone)
                    {
                        LoadedCombatBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.EncaseInStone, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SettingCombatSelfBuffPerk(perkAction, fightingTarget, ref actionTarget, "EncaseInStoneValue", true), CombatActionPriority.High);
                    }
                }

                #endregion

                #region NotumSource && TheoreticalResearch

                if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician ||
                    DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                {
                    if (perk.Hash == PerkHash.KnowledgeEnhancer)
                    {
                        LoadedCombatBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.KnowledgeEnhancer, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "KnowledgeEnhancerSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                    }
                }

                #endregion

                #region PowerUp

                if (DynelManager.LocalPlayer.Profession == Profession.Doctor ||
                    DynelManager.LocalPlayer.Profession == Profession.Trader ||
                    DynelManager.LocalPlayer.Profession == Profession.Soldier ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer ||
                    DynelManager.LocalPlayer.Profession == Profession.Engineer ||
                    DynelManager.LocalPlayer.Profession == Profession.Agent)
                {
                    if (perk.Hash == PerkHash.Energize)
                    {
                        LoadedCombatBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Energize, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "EnergizeSelection", false), CombatActionPriority.Medium, RuleContext.InCombat);
                    }
                }

                #endregion

                #region SMGMastery

                if (DynelManager.LocalPlayer.Profession == Profession.Soldier ||
                    DynelManager.LocalPlayer.Profession == Profession.Fixer)
                {
                    if (perk.Hash == PerkHash.ReinforceSlugs)
                    {
                        LoadedCombatBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.ReinforceSlugs, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            EnumSelfPerk(perkAction, fightingTarget, ref actionTarget, "ReinforceSlugsSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                    }
                }

                #endregion


                #endregion

                #region Debuffs

                switch (DynelManager.LocalPlayer.Breed)
                {
                    case Breed.Opifex:
                        if (perk.Hash == PerkHash.Derivate)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Derivate, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "DerivateSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.BlindedByDelights)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.BlindedByDelights, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "BlindedByDelightsSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        break;

                    case Breed.Nanomage:
                        if (perk.Hash == PerkHash.Sword)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Sword, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "SwordSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.Pen)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Pen, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "PenSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        break;
                }
                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Agent:
                        if (perk.Hash == PerkHash.FadeArmor)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.FadeArmor, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "FadeArmorSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        break;
                    case Profession.Bureaucrat:
                        if (perk.Hash == PerkHash.Succumb)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Succumb, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "SuccumbSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        break;

                    case Profession.Doctor:
                        if (perk.Hash == PerkHash.MaliciousProhibition)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.MaliciousProhibition, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "MaliciousProhibitionSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        break;
                    case Profession.Engineer:
                        if (perk.Hash == PerkHash.Deconstruction)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Deconstruction, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "DeconstructionSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.InstallNotumDepletionDevice)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.InstallNotumDepletionDevice, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "InstallNotumDepletionDeviceSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.SuppressivePrimer)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SuppressivePrimer, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "SuppressivePrimerSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.ThermalPrimer)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ThermalPrimer, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "ThermalPrimerSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;
                    case Profession.Keeper:
                        if (perk.Hash == PerkHash.MarkOfVengeance)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.MarkOfVengeance, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "MarkOfVengeanceSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.MarkOfTheUnclean)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.MarkOfTheUnclean, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "MarkOfTheUncleanSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.MarkOfTheUnhallowed)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.MarkOfTheUnhallowed, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "MarkOfTheUnhallowedSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        break;
                    case Profession.MartialArtist:

                        if (perk.Hash == PerkHash.Dragonfire)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Dragonfire, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "DragonfireSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.Incapacitate)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Incapacitate, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "IncapacitateSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.RedDusk)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.RedDusk, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "RedDuskSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        break;
                    case Profession.Shade:
                        if (perk.Hash == PerkHash.EtherealTouch)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.EtherealTouch, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "EtherealTouchSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.ConvulsiveTremor)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ConvulsiveTremor, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "ConvulsiveTremorSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.DoomTouch)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.DoomTouch, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "DoomTouchSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.Blur)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Blur, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "BlurSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        break;
                    case Profession.Soldier:
                        if (perk.Hash == PerkHash.Tracer)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Tracer, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "TracerSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.LaserPaintTarget)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.LaserPaintTarget, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "LaserPaintTargetSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.TriangulateTarget)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TriangulateTarget, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "TriangulateTargetSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        if (perk.Hash == PerkHash.NapalmSpray)
                        {
                            LoadedDeBuffs.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.NapalmSpray, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "NapalmSpraySelection"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        break;
                }


                if (DynelManager.LocalPlayer.Profession == Profession.Trader || DynelManager.LocalPlayer.Profession == Profession.Engineer || DynelManager.LocalPlayer.Profession == Profession.Doctor)
                {
                    if (perk.Hash == PerkHash.ChemicalBlindness)
                    {
                        LoadedDeBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.ChemicalBlindness, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "ChemicalBlindnessSelection"), CombatActionPriority.Medium, RuleContext.InCombat);
                    }
                }

                if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician || DynelManager.LocalPlayer.Profession == Profession.Metaphysicist || DynelManager.LocalPlayer.Profession == Profession.Doctor || DynelManager.LocalPlayer.Profession == Profession.Bureaucrat)
                {
                    if (perk.Hash == PerkHash.DazzleWithLights)
                    {
                        LoadedDeBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.DazzleWithLights, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "DazzleWithLightsSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist || DynelManager.LocalPlayer.Profession == Profession.NanoTechnician || DynelManager.LocalPlayer.Profession == Profession.Doctor
                    || DynelManager.LocalPlayer.Profession == Profession.Agent || DynelManager.LocalPlayer.Profession == Profession.Trader || DynelManager.LocalPlayer.Profession == Profession.Fixer || DynelManager.LocalPlayer.Profession == Profession.Engineer)
                {
                    if (perk.Hash == PerkHash.HostileTakeover)
                    {
                        LoadedDeBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.HostileTakeover, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "HostileTakeoverSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }
                if (DynelManager.LocalPlayer.Profession == Profession.Soldier || DynelManager.LocalPlayer.Profession == Profession.Agent)
                {
                    if (perk.Hash == PerkHash.ArmorPiercingShot)
                    {
                        LoadedDeBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.ArmorPiercingShot, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "ArmorPiercingShotSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }
                if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician || DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                {
                    if (perk.Hash == PerkHash.StopNotumFlow)
                    {
                        LoadedDeBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.StopNotumFlow, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "StopNotumFlowSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                    if (perk.Hash == PerkHash.NotumOverflow)
                    {
                        LoadedDeBuffs.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.NotumOverflow, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "NotumOverflowSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                    }
                }

                if (perk.Hash == PerkHash.Tick)
                {
                    LoadedDeBuffs.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Tick, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "TickSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (perk.Hash == PerkHash.ZapNano)
                {
                    LoadedDeBuffs.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.ZapNano, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        EnumTargetPerk(perkAction, fightingTarget, ref actionTarget, "ZapNanoSelection"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region Holds

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Adventurer:
                        if (perk.Hash == PerkHash.Stoneworks)
                        {
                            LoadedHolds.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Stoneworks, AOE_Root_Perk, CombatActionPriority.Medium, RuleContext.InCombat);
                        }
                        break;
                    case Profession.Agent:
                        if (perk.Hash == PerkHash.TheShot)
                        {
                            LoadedHolds.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TheShot, TheShot, CombatActionPriority.Low);
                        }
                        if (perk.Hash == PerkHash.Assassinate)
                        {
                            LoadedHolds.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Assassinate, AssassinatePerk, CombatActionPriority.High);
                        }

                        if (perk.Hash == PerkHash.ConcussiveShot)
                        {
                            LoadedHolds.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ConcussiveShot, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkConcussiveShot"), CombatActionPriority.Medium, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.SoftenUp)
                        {
                            LoadedHolds.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SoftenUp, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkSoftenUp"), CombatActionPriority.Medium, RuleContext.InCombat);

                        }

                        break;

                    case Profession.Bureaucrat:
                        if (perk.Hash == PerkHash.ConfoundWithRules)
                        {
                            LoadedHolds.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ConfoundWithRules, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkConfoundWithRules"), CombatActionPriority.Medium, RuleContext.InCombat);

                        }
                        break;

                    case Profession.Enforcer:
                        if (perk.Hash == PerkHash.GroinKick)
                        {
                            LoadedHolds.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.GroinKick, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkGroinKick"), CombatActionPriority.Medium, RuleContext.InCombat);

                        }
                        break;
                    case Profession.Shade:
                        if (perk.Hash == PerkHash.Disorientate)
                        {
                            LoadedHolds.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Disorientate, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkDisorientate"), CombatActionPriority.Medium, RuleContext.InCombat);

                        }
                        break;
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Enforcer || DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                {
                    if (perk.Hash == PerkHash.CrushBone)
                    {
                        LoadedHolds.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.CrushBone, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkCrushBone"), CombatActionPriority.Medium, RuleContext.InCombat);
                    }

                    if (perk.Hash == PerkHash.BringThePain)
                    {
                        LoadedHolds.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.BringThePain, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkBringThePain"), CombatActionPriority.Medium, RuleContext.InCombat);

                    }
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Enforcer || DynelManager.LocalPlayer.Profession == Profession.MartialArtist)
                {
                    if (perk.Hash == PerkHash.BlindsideBlow)
                    {
                        LoadedHolds.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.BlindsideBlow, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkBlindsideBlow"), CombatActionPriority.Medium, RuleContext.InCombat);

                    }
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Adventurer || DynelManager.LocalPlayer.Profession == Profession.Enforcer || DynelManager.LocalPlayer.Profession == Profession.Shade)
                {
                    if (perk.Hash == PerkHash.FullFrontal)
                    {
                        LoadedHolds.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.FullFrontal, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkFullFrontal"), CombatActionPriority.Medium, RuleContext.InCombat);

                    }
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Trader || DynelManager.LocalPlayer.Profession == Profession.Fixer || DynelManager.LocalPlayer.Profession == Profession.Engineer)
                {
                    if (perk.Hash == PerkHash.Guesstimate)
                    {
                        LoadedHolds.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.Guesstimate, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkGuesstimate"), CombatActionPriority.Medium, RuleContext.InCombat);

                    }
                }
                if (DynelManager.LocalPlayer.Profession == Profession.NanoTechnician || DynelManager.LocalPlayer.Profession == Profession.Metaphysicist)
                {
                    if (perk.Hash == PerkHash.QuarkContainmentField)
                    {
                        LoadedHolds.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.QuarkContainmentField, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkQuarkContainmentField"));

                    }
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Soldier || DynelManager.LocalPlayer.Profession == Profession.Fixer)
                {
                    if (perk.Hash == PerkHash.JarringBurst)
                    {
                        LoadedHolds.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.JarringBurst, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkJarringBurst"), CombatActionPriority.Medium, RuleContext.InCombat);

                    }
                }

                if (DynelManager.LocalPlayer.Profession == Profession.Trader || DynelManager.LocalPlayer.Profession == Profession.Soldier || DynelManager.LocalPlayer.Profession == Profession.Engineer)
                {
                    if (perk.Hash == PerkHash.LegShot)
                    {
                        LoadedHolds.Add((int)perk.Hash);
                        RegisterPerkProcessor(PerkHash.LegShot, LegShot, CombatActionPriority.Medium, RuleContext.InCombat);
                    }
                }

                if (perk.Hash == PerkHash.NanoShakes)
                {
                    LoadedHolds.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.NanoShakes, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    Targeted_Hold(perkAction, fightingTarget, ref actionTarget, "PerkNanoShakes"), CombatActionPriority.Medium, RuleContext.InCombat);

                }

                #region FreakStrength

                if (perk.Hash == PerkHash.Grasp)
                {
                    LoadedHolds.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Grasp, GraspPerk, CombatActionPriority.Low);
                }

                if (perk.Hash == PerkHash.Bearhug)
                {
                    LoadedHolds.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.Bearhug, BearhugPerks, CombatActionPriority.High);
                }

                if (perk.Hash == PerkHash.GripOfColossus)
                {
                    LoadedHolds.Add((int)perk.Hash);
                    RegisterPerkProcessor(PerkHash.GripOfColossus, GripOfColossusPerk, CombatActionPriority.High);
                }

                #endregion


                #endregion

                #region Pets

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Metaphysicist:

                        if (perk.Hash == PerkHash.KenFi)
                        {
                            LoadedPetBuffSpells.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.KenFi, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
                                => PetCombatBuffPerk(perkAction, fightingTarget, ref actionTarget, "KenFi"), CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.ChannelRage)
                        {
                            LoadedPetBuffSpells.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ChannelRage, ChannelRagePerk, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }
                        break;
                    case Profession.Bureaucrat:
                        if (perk.Hash == PerkHash.Puppeteer)
                        {
                            LoadedPetBuffSpells.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Puppeteer, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
                            => PetCombatBuffPerk(perkAction, fightingTarget, ref actionTarget, "Puppeteer"), CombatActionPriority.Low, RuleContext.InCombat);
                        }
                        break;
                    case Profession.Engineer:

                        if (perk.Hash == PerkHash.OptimizeBotProtocol)
                        {
                            LoadedPetBuffSpells.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.OptimizeBotProtocol, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
                            => PetCombatBuffPerk(perkAction, fightingTarget, ref actionTarget, "OptimizeBotProtocol"), CombatActionPriority.Low, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.TauntBox)
                        {
                            LoadedPetBuffSpells.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.TauntBox, GenericPetPerk, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (perk.Hash == PerkHash.ChaoticEnergy)
                        {
                            LoadedPetBuffSpells.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ChaoticEnergy, GenericPetPerk, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (perk.Hash == PerkHash.SiphonBox)
                        {
                            LoadedPetBuffSpells.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.SiphonBox, GenericPetPerk, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }
                        break;
                }

                #endregion

                #region Taunts

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Enforcer:
                        if (perk.Hash == PerkHash.Hatred)
                        {
                            LoadedTaunts.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Hatred, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TauntPerk(perkAction, fightingTarget, ref actionTarget, "TauntPerkHatred"), CombatActionPriority.VeryHigh, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.Taunt)
                        {
                            LoadedTaunts.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.Taunt, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TauntPerk(perkAction, fightingTarget, ref actionTarget, "TauntPerkTaunt"), CombatActionPriority.VeryHigh, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.ArouseAnger)
                        {
                            LoadedTaunts.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.ArouseAnger, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TauntPerk(perkAction, fightingTarget, ref actionTarget, "TauntPerkArouseAnger"), CombatActionPriority.VeryHigh, RuleContext.InCombat);
                        }

                        if (perk.Hash == PerkHash.CauseOfAnger)
                        {
                            LoadedTaunts.Add((int)perk.Hash);
                            RegisterPerkProcessor(PerkHash.CauseOfAnger, (PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            TauntPerk(perkAction, fightingTarget, ref actionTarget, "TauntPerkCauseOfAnger"), CombatActionPriority.Ultra, RuleContext.InCombat);
                        }

                        break;
                }

                #endregion
            }

            OwnedPerks.UnionWith(DamagePerks);
            OwnedPerks.UnionWith(DebuffPerks);
            OwnedPerks.UnionWith(RemoverPerks);
            OwnedPerks.UnionWith(CombatBuffPerks);

            OwnedProcs.UnionWith(OwnedProcsType1);
            OwnedProcs.UnionWith(OwnedProcsType2);

            if (OwnedProcs != null && OwnedProcs.Count > 0 && !_buttonDefinitions.Any(x => x.Label == "Procs"))
                _buttonDefinitions.Add(("Procs", Proc_Button_Click));

        }

        #region Profession

        #region Adventurer
        private bool CanUseMorphPerk()
        {
            if (!DynelManager.LocalPlayer.Buffs.Contains(new int[] { 217670, 25994, 263278, 82834, 275005, 85062, 217680, 85070, 229666, 229884, 229887, 229889 })) return false;
            return true;
        }

        private bool DevourLifeTapPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (!CanUseMorphPerk()) return false;
            if (fightingTarget == null) return false;
            if (DynelManager.LocalPlayer.HealthPercent > _settings["DevourValue"].AsInt32()) return false;
            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool BleedingWounds(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (!CanUseMorphPerk()) return false;
            if (fightingTarget == null) return false;
            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool GuttingBlow(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (!CanUseMorphPerk()) return false;
            if (fightingTarget == null) return false;
            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool Beckoning(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["BeckoningSelection"].AsInt32() == 0) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (!CanUseMorphPerk()) return false;
            if (fightingTarget == null) return false;
            if (_settings["BeckoningSelection"].AsInt32() == 2 && !(fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }
        #endregion

        #region Agent

        private bool SnipeShot2(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!fightingTarget.Buffs.Contains(new int[] { 209941, 211388 })) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool PinpointStrike(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;

            if (PerkAction.List.Any(p => p.Hash == PerkHash.ArmorPiercingShot && p.IsAvailable))
            {
                if (!fightingTarget.Buffs.Contains(209883)) return false;
            }

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool DeathStrike(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (fightingTarget.HealthPercent > 50) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool NightKillerPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;

            switch (_settings[$"EvasionDebuffs_AgentOption"].AsInt32())
            {
                case 1://target
                    if (!fightingTarget.Buffs.Contains(NanoLine.EvasionDebuffs_Agent)) return false;
                    break;
                case 2://boss
                    if (fightingTarget?.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))
                    {
                        if (!fightingTarget.Buffs.Contains(NanoLine.EvasionDebuffs_Agent)) return false;
                    }
                    break;
            }

            actionTarget = (fightingTarget, true);
            return true;
        }

        #endregion

        #region Doctor
        private bool ViralCombinationPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!fightingTarget.Buffs.Contains(new NanoLine[] { NanoLine.DOT_LineA, NanoLine.DOT_LineB })) return false;
            if (Spell.List.Any(x => x.Nanoline == NanoLine.DOTStrainC))
                if (!fightingTarget.Buffs.Contains(NanoLine.DOTStrainC)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        public static bool HaleandHearty(PerkAction perk, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!perk.IsAvailable) return false;

            var target = Team.Members.FirstOrDefault(p => p.Character != null && HaleandHeartyDebuffs(p.Character))?.Character;

            if (target == null) return false;

            actionTarget.Target = target;
            actionTarget.ShouldSetTarget = true;
            return true;
        }

        public static bool TeamHaleandHearty(PerkAction perk, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!perk.IsAvailable) return false;

            if (PerkAction.Find("Hale and Hearty", out PerkAction _HaleandHearty) && _HaleandHearty.IsAvailable) return false;

            return Team.Members.Any(m => m.Character != null && HaleandHeartyDebuffs(m.Character));
        }

        static bool HaleandHeartyDebuffs(SimpleChar c)
        {
            return c != null && c.Buffs.Contains(new[] { NanoLine.AAODebuffs, NanoLine.TraderAAODrain, NanoLine.TraderSkillTransferTargetDebuff_Deprive, NanoLine.TraderSkillTransferTargetDebuff_Ransack,
            NanoLine.DOT_LineA, NanoLine.DOT_LineB, NanoLine.DOTNanotechnicianStrainA, NanoLine.DOTAgentStrainA, NanoLine.DOTNanotechnicianStrainB, NanoLine.DOTStrainC,
                NanoLine.PainLanceDoT, NanoLine.MINIDoT, NanoLine.InitiativeDebuffs });
        }

        private bool NanoTransmission(PerkAction perk, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["NanoTransmission"].AsBool()) return false;

            return CombatSelfBuffPerk(perk, fightingTarget, ref actionTarget);
        }

        private bool CloseCall(PerkAction perk, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!perk.IsAvailable || Spell.List.Any(spell => spell.IsReady) || !InCombat()) return false;

            SimpleChar target;

            if (Team.IsInTeam)
            {
                target = Team.Members.Where(t => t.Character != null && t.Character.IsInLineOfSight && t.Character.IsAlive &&
                t.Character.HealthPercent < 50 && perk.IsInRange(t.Character)).OrderBy(t => t.Character.HealthPercent).FirstOrDefault()?.Character;
            }
            else
            {
                if (DynelManager.LocalPlayer.HealthPercent > 50) return false;
                target = DynelManager.LocalPlayer;
            }

            if (target == null) return false;

            actionTarget.Target = target;
            actionTarget.ShouldSetTarget = true;
            return true;
        }

        private bool BattleGroupHeal(PerkAction perk, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, int healthPercentage)
        {
            if (!perk.IsAvailable || Spell.List.Any(spell => spell.IsReady) || !InCombat()) return false;

            if ((perk.Hash == PerkHash.BattlegroupHeal2 && PerkAction.List.Any(p => p.Hash == PerkHash.BattlegroupHeal1 && p.IsAvailable)) ||
                (perk.Hash == PerkHash.BattlegroupHeal3 && (PerkAction.List.Any(p => p.Hash == PerkHash.BattlegroupHeal1 && p.IsAvailable) || PerkAction.List.Any(p => p.Hash == PerkHash.BattlegroupHeal2 && p.IsAvailable))) ||
                (perk.Hash == PerkHash.BattlegroupHeal4 && (PerkAction.List.Any(p => p.Hash == PerkHash.BattlegroupHeal1 && p.IsAvailable) || PerkAction.List.Any(p => p.Hash == PerkHash.BattlegroupHeal2 && p.IsAvailable) || PerkAction.List.Any(p => p.Hash == PerkHash.BattlegroupHeal3 && p.IsAvailable))))
            {
                return false;
            }

            if (Team.IsInTeam)
            {
                var dyingTeamMembersCount = Team.Members.Count(m => m.Character != null && m.Character.Health > 0 && m.Character.HealthPercent <= healthPercentage);

                if (dyingTeamMembersCount < 2) return false;

                actionTarget.Target = DynelManager.LocalPlayer;
                actionTarget.ShouldSetTarget = true;
                return true;
            }
            else if (DynelManager.LocalPlayer.HealthPercent <= healthPercentage)
            {
                actionTarget.Target = DynelManager.LocalPlayer;
                actionTarget.ShouldSetTarget = true;
                return true;

            }
            return false;
        }

        #endregion

        #region Enforcer
        private bool ChargePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            var lp = DynelManager.LocalPlayer;
            if (!lp.IsMoving) return false;
            if (fightingTarget == null) return false;
            if (lp.Position.DistanceFrom(fightingTarget.Position) > 10 && lp.Position.DistanceFrom(fightingTarget.Position) < 3) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        #endregion

        #region Engineer
        #endregion

        #region Fixer
        private bool NCUBoosterPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;

            var NCU_Boosters = new int[] { 211394, 211393, 211392, 211395 };

            SimpleChar target = null;

            if (Team.IsInTeam)
            {
                var local = Team.Members.Find(x => x.Identity == DynelManager.LocalPlayer.Identity);
                if (local == null)
                    return false;

                target = Team.Members.Find(tm => tm.Character != null && (!Team.IsRaid || tm.TeamIndex == local.TeamIndex) && tm.Character.Health > 0
                    && tm.Character.IsInLineOfSight && RemainingNCU.ContainsKey(tm.Identity) && (perkAction.AttackRange == 0 || TargetInPerkRange(perkAction, tm.Character)) && !tm.Character.Buffs.Contains(NCU_Boosters))?.Character;
            }
            else if (!DynelManager.LocalPlayer.Buffs.Contains(NCU_Boosters))
                target = DynelManager.LocalPlayer;

            if (target == null) return false;
            actionTarget = (target, true);
            return true;
        }

        private bool CripplePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (!DynelManager.LocalPlayer.Buffs.Contains(248959)) return false;
            if (fightingTarget == null) return false;
            actionTarget = (fightingTarget, true);
            return true;
        }
        #endregion

        #region Keeper
        #endregion

        #region MartialArtist

        #endregion

        #region Metaphysicist
        #endregion

        #region NanoTechnician
        private bool DetauntPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["DeTaunt"].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (!Team.IsInTeam) return false;
            actionTarget.Target = null;

            actionTarget.Target = DynelManager.NPCs.FirstOrDefault(npc => npc.IsAttacking && npc.FightingTarget.Identity == DynelManager.LocalPlayer.Identity);

            if (actionTarget.Target == null) return false;
            actionTarget.ShouldSetTarget = true;
            return true;
        }

        private bool UtilizePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (!DynelManager.LocalPlayer.Buffs.Contains(302292)) return false;
            if (fightingTarget == null) return false;
            actionTarget = (fightingTarget, true);
            return true;
        }
        #endregion

        #region Shade

        private bool DimensionalFist(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!fightingTarget.Buffs.Contains(209982)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool SymbiosisPerk(PerkAction perk, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (fightingTarget == null || PerkAction.List.Any(p => p.Hash == PerkHash.RitualOfDevotion && !p.IsAvailable)) return false;

            if (PerkAction.List.Any(a => a.IsExecuting)) return false;

            return true;
        }

        private bool SpiritDissolutionPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!fightingTarget.Buffs.Contains(209997)) return false;

            if (PerkAction.List.Any(p => p.Hash == PerkHash.FleshQuiver))
                if (fightingTarget.Buffs.Contains(209971)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        #endregion

        #region Soldier
        private bool ContainedBursPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (fightingTarget.Buffs.Contains(209950)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool ViolencePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (DynelManager.LocalPlayer.HealthPercent >= 40) return false;

            actionTarget = (DynelManager.LocalPlayer, true);
            return true;
        }

        private bool GuardianPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (!DynelManager.LocalPlayer.Buffs.Contains(209952)) return false;
            if (!DynelManager.LocalPlayer.Buffs.Contains(new NanoLine[] { NanoLine.TotalMirrorShield, NanoLine.AugmentedMirrorShieldNano })) return false;

            if (Team.IsInTeam)
            {
                var local = Team.Members.Find(x => x.Identity == DynelManager.LocalPlayer.Identity);
                if (local == null)
                    return false;
                actionTarget.Target = Team.Members.FirstOrDefault(m => m.Character != null && (!Team.IsRaid || m.TeamIndex == local.TeamIndex) && RemainingNCU.ContainsKey(m.Identity) && !m.Character.Buffs.Contains(209953) && DynelManager.NPCs.Any(n => n.IsAttacking && n.FightingTarget?.Identity == m.Identity)).Character;
            }
            else if (DynelManager.NPCs.Any(n => n != null && n.IsAttacking && fightingTarget.Identity == DynelManager.LocalPlayer.Identity) && !DynelManager.LocalPlayer.Buffs.Contains(209953))
                actionTarget.Target = DynelManager.LocalPlayer;

            actionTarget.ShouldSetTarget = true;
            return true;
        }
        #endregion

        #region Trader
        #endregion

        #endregion

        #region AOE Combat

        private bool AOE_Combat_Perk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["AOE"].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (PVPDistance(11)) return false;
            if (fightingTarget == null) return false;

            switch (perkAction.Hash)
            {
                case PerkHash.Avalanche:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(209857)) return false;
                    break;
                case PerkHash.SeismicSmash:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(226028)) return false;
                    break;
            }

            actionTarget = (fightingTarget, true);
            return true;
        }

        #endregion

        #region Snares, Stuns, Roots





        #endregion

        #region damage/attack

        private bool DirectDamagePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (perkAction.AttackRange != 0)
            {
                if (!TargetInPerkRange(perkAction, fightingTarget)) return false;
            }

            switch (perkAction.Hash)
            {
                case PerkHash.LightKiller:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(281525)) return false;
                    break;
                case PerkHash.Headbutt:
                    if (!fightingTarget.IsFacing(DynelManager.LocalPlayer)) return false;
                    break;
                case PerkHash.InitialStrike:
                    if (fightingTarget.HealthPercent < 95) return false;
                    break;
                case PerkHash.PowerVolley:
                case PerkHash.PowerShock:
                case PerkHash.PowerBlast:
                case PerkHash.PowerCombo:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(209927)) return false;
                    break;
                case PerkHash.QuickShot:
                    if (!perkAction.MeetsSelfUseReqs()) return false;
                    break;
                case PerkHash.TremorHand:
                    if (PerkAction.List.Any(p => p.Hash == PerkHash.EtherealTouch) && !fightingTarget.Buffs.Contains(209982)) return false;
                    break;
                case PerkHash.FleshQuiver:
                    if (PerkAction.List.Any(x => x.Hash == PerkHash.TremorHand) && !fightingTarget.Buffs.Contains(209886)) return false;
                    break;
                case PerkHash.ChiConductor:
                    if (PerkAction.List.Any(x => x.Hash == PerkHash.FleshQuiver) && !fightingTarget.Buffs.Contains(209971)) return false;
                    break;
                case PerkHash.Obliterate:
                    if (fightingTarget.HealthPercent > 14) return false;
                    if (PerkAction.List.Any(x => x.Hash == PerkHash.Obliterate) && !fightingTarget.Buffs.Contains(209972)) return false;
                    break;
            }

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool WeaponTypePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (perkAction.AttackRange != 0)
            {
                if (!TargetInPerkRange(perkAction, fightingTarget)) return false;
            }

            if (!perkAction.MeetsSelfUseReqs()) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool LocalBuffCheckPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, int Buff)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (perkAction.AttackRange != 0)
            {
                if (!TargetInPerkRange(perkAction, fightingTarget)) return false;
            }
            if (!DynelManager.LocalPlayer.Buffs.Contains(Buff)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool TargetBuffCheckPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, int Buff)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (perkAction.AttackRange != 0)
            {
                if (!TargetInPerkRange(perkAction, fightingTarget)) return false;
            }
            if (!fightingTarget.Buffs.Contains(Buff)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool LocalBuffAndTargetBuffCheckPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, int localBuff, int targetBuff)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (!DynelManager.LocalPlayer.Buffs.Contains(localBuff)) return false;
            if (fightingTarget == null) return false;
            if (perkAction.AttackRange != 0)
            {
                if (!TargetInPerkRange(perkAction, fightingTarget)) return false;
            }
            if (!fightingTarget.Buffs.Contains(targetBuff)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool DotPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (perkAction.AttackRange != 0)
            {
                if (!TargetInPerkRange(perkAction, fightingTarget)) return false;
            }
            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool DebuffPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (perkAction.AttackRange != 0)
            {
                if (!TargetInPerkRange(perkAction, fightingTarget)) return false;
            }
            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool WrathAttack(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.Skill2hEdged)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool TotemicRitesPerkStart(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!perkAction.IsAvailable || perkAction.IsPending || perkAction.IsExecuting) return false;
            if (_actionQueue.Any(x => x.CombatAction is PerkAction a && a == perkAction)) return false;

            if (!PerkAction.List.Where(p => TotemicRites.Contains(p.Hash)).All(p => p.IsAvailable)) return false;

            return true;
        }

        readonly List<PerkHash> TotemicRites = new List<PerkHash> { PerkHash.DevourVigor, PerkHash.RitualOfZeal, PerkHash.DevourEssence, PerkHash.RitualOfSpirit, PerkHash.DevourVitality, PerkHash.RitualOfBlood };

        private bool TotemicRitesPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (_actionQueue.Any(x => x.CombatAction is PerkAction a && a == perkAction)) return false;

            switch (perkAction.Hash)
            {
                case PerkHash.DevourVigor:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.RitualofDevotion)) return false;
                    return true;

                case PerkHash.RitualOfZeal:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedDevourVigor)) return false;
                    return true;

                case PerkHash.DevourEssence:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.RitualofZeal)) return false;
                    return true;

                case PerkHash.RitualOfSpirit:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedDevourEssence)) return false;
                    return true;

                case PerkHash.DevourVitality:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.RitualofSpirit)) return false;
                    return true;

                case PerkHash.RitualOfBlood:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedDevourVitality)) return false;
                    return true;
            }

            return false;
        }

        private bool SpiritPhylacteryPerkStart(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!perkAction.IsAvailable || perkAction.IsPending || perkAction.IsExecuting) return false;
            if (_actionQueue.Any(x => x.CombatAction is PerkAction a && a == perkAction)) return false;

            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.PerformedUnsealedBlight, NanoLine.PerformedCaptureEssence, NanoLine.PerformedUnsealedPestilence, NanoLine.PerformedCaptureSpirit,
                NanoLine.PerformedUnsealedContagion, NanoLine.PerformedCaptureVitality })) return false;
            return true;
        }

        readonly List<PerkHash> SpiritPhylactery = new List<PerkHash> { PerkHash.UnsealedBlight, PerkHash.CaptureEssence, PerkHash.UnsealedPestilence, PerkHash.CaptureSpirit, PerkHash.UnsealedContagion, PerkHash.CaptureVitality };

        private bool SpiritPhylacteryPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (_actionQueue.Any(x => x.CombatAction is PerkAction a && a == perkAction)) return false;

            switch (perkAction.Hash)
            {
                case PerkHash.UnsealedBlight:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedCaptureVigor)) return false;
                    return true;

                case PerkHash.CaptureEssence:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedUnsealedBlight)) return false;
                    return true;

                case PerkHash.UnsealedPestilence:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedCaptureEssence)) return false;
                    return true;

                case PerkHash.CaptureSpirit:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedUnsealedPestilence)) return false;
                    return true;

                case PerkHash.UnsealedContagion:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedCaptureSpirit)) return false;
                    return true;

                case PerkHash.CaptureVitality:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedUnsealedContagion)) return false;
                    return true;
            }

            return false;
        }

        private bool PiercingMasteryPerkStartStab(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["ShouldMoveBehindTarget"].AsInt32() == 0) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (_settings["ShouldMoveBehindTarget"].AsInt32() == 2 && !(fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))) return false;
            if (!perkAction.IsAvailable || perkAction.IsPending || perkAction.IsExecuting) return false;
            if (_actionQueue.Any(x => x.CombatAction is PerkAction a && a == perkAction)) return false;

            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.PerformedDoubleStab, NanoLine.PerformedPerforate, NanoLine.PerformedLacerate, NanoLine.PerformedImpale,
                NanoLine.PerformedGore, NanoLine.PerformedHecatomb })) return false;

            return true;
        }

        private bool PiercingMasteryPerkStartPerforate(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;

            switch (_settings["ShouldMoveBehindTarget"].AsInt32())
            {
                case 0:
                    break;
                case 1:
                    return false;
                case 2:
                    if (fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name)) return false;
                    break;
            }

            if (!perkAction.IsAvailable || perkAction.IsPending || perkAction.IsExecuting) return false;
            if (_actionQueue.Any(x => x.CombatAction is PerkAction a && a == perkAction)) return false;

            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.PerformedLacerate, NanoLine.PerformedImpale, NanoLine.PerformedGore, NanoLine.PerformedHecatomb })) return false;

            return true;
        }

        readonly List<PerkHash> PiercingMastery = new List<PerkHash> { PerkHash.DoubleStab, PerkHash.Perforate, PerkHash.Lacerate, PerkHash.Impale, PerkHash.Gore, PerkHash.Hecatomb };

        private bool PiercingMasteryPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (_actionQueue.Any(x => x.CombatAction is PerkAction a && a == perkAction)) return false;

            switch (perkAction.Hash)
            {
                case PerkHash.DoubleStab:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedStab)) return false;
                    return true;

                case PerkHash.Perforate:
                    switch (_settings["ShouldMoveBehindTarget"].AsInt32())
                    {
                        case 0:
                            return true;
                        case 1:
                            return DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedDoubleStab);
                        case 2:
                            if (fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))
                                return DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedDoubleStab);

                            return true;
                    }
                    return false;
                case PerkHash.Lacerate:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedPerforate)) return false;
                    return true;

                case PerkHash.Impale:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedLacerate)) return false;
                    return true;

                case PerkHash.Gore:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedImpale)) return false;
                    return true;

                case PerkHash.Hecatomb:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.PerformedGore)) return false;
                    return true;
            }

            return false;
        }

        #endregion

        #region Buffs

        private bool SettingCombatSelfBuffPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting, bool RequiresTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null && !Attacked) return false;
            var lp = DynelManager.LocalPlayer;
            if (lp.Buffs.Any(b => b.Name == perkAction.Name)) return false;
            if (lp.HealthPercent > _settings[setting].AsInt32()) return false;

            actionTarget = (lp, RequiresTarget);
            return true;
        }

        private bool NotumShield(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (DynelManager.LocalPlayer.Buffs.Any(b => b.Name == perkAction.Name)) return false;
            if (DynelManager.LocalPlayer.Nano <= 2000) return false;
            actionTarget.Target = null;

            switch (_settings["NotumShieldSelection"].AsInt32())
            {
                case 0:
                    return false;
                case 1:
                    actionTarget.Target = DynelManager.LocalPlayer;
                    break;
                case 2:
                    if (fightingTarget != null && !(fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))) return false;
                    actionTarget.Target = DynelManager.LocalPlayer;
                    break;
            }

            if (actionTarget.Target == null) return false;

            actionTarget.ShouldSetTarget = true;
            return true;
        }
        private bool CombatSelfBuffPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            var lp = DynelManager.LocalPlayer;
            if (lp.Buffs.Any(b => b.Name == perkAction.Name)) return false;

            actionTarget = (lp, true);
            return true;
        }

        private bool DelayedCombatTeamBuff(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null && !Attacked) return false;
            if (Time.AONormalTime < FightTimeStamp + _settings[settingName].AsInt32()) return false;

            actionTarget = (DynelManager.LocalPlayer, true);
            return true;
        }

        private bool EnumSelfPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null, bool TargetSelf = true)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null && !Attacked) return false;
            var lp = DynelManager.LocalPlayer;
            if (lp.Buffs.Any(b => b.Name == perkAction.Name)) return false;

            if (perkAction.Hash == PerkHash.Violence)
            {
                if (DynelManager.LocalPlayer.HealthPercent > 39) return false;
            }

            actionTarget.Target = null;

            if (settingName != null)
            {
                switch (_settings[settingName].AsInt32())
                {
                    case 0:
                        return false;
                    case 1:
                        actionTarget.Target = lp;
                        break;
                    case 2:
                        if (fightingTarget != null && !(fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))) return false;
                        actionTarget.Target = lp;
                        break;
                }
            }

            if (actionTarget.Target == null) return false;

            actionTarget.ShouldSetTarget = TargetSelf;
            return true;
        }

        private bool EnumTargetPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingName = null)
        {
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            actionTarget.Target = null;

            if (settingName != null)
            {
                switch (_settings[settingName].AsInt32())
                {
                    case 0:
                        return false;
                    case 1:
                        actionTarget.Target = fightingTarget;
                        break;
                    case 2:
                        if (fightingTarget != null && !(fightingTarget.MaxHealth > 1000000 || BossNames.Contains(fightingTarget.Name))) return false;
                        actionTarget.Target = fightingTarget;
                        break;
                }
            }

            if (actionTarget.Target == null) return false;
            actionTarget.ShouldSetTarget = true;
            return true;
        }

        #endregion

        #region debuff removers

        private bool NanoShutdown_TraderDebuff_CleansePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (perkAction.IsExecuting) return false;
            if (!perkAction.IsAvailable) return false;

            var player = DynelManager.LocalPlayer;
            var debuffs = new NanoLine[] { NanoLine.TraderSkillTransferTargetDebuff_Deprive, NanoLine.TraderSkillTransferTargetDebuff_Ransack, NanoLine.NanoShutdownDebuff };
            if (!player.Buffs.Contains(debuffs)) return false;
            actionTarget.Target = player;
            actionTarget.ShouldSetTarget = true;
            return true;
        }

        private bool Trader_Debuff_CleansePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (perkAction.IsExecuting) return false;
            if (!perkAction.IsAvailable) return false;

            var player = DynelManager.LocalPlayer;

            if (!player.Buffs.Contains(NanoLine.TraderSkillTransferTargetDebuff_Deprive) || !player.Buffs.Contains(NanoLine.TraderSkillTransferTargetDebuff_Ransack)) return false;

            actionTarget.Target = player;
            actionTarget.ShouldSetTarget = true;
            return true;
        }

        private bool AAO_Dots_CleansePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (perkAction.IsExecuting) return false;
            if (!perkAction.IsAvailable) return false;

            var debuffs = new NanoLine[]
            {
                NanoLine.AAODebuffs, NanoLine.TraderAAODrain, NanoLine.DOT_LineA, NanoLine.DOTNanotechnicianStrainA, NanoLine.DOTAgentStrainA, NanoLine.DOTNanotechnicianStrainB,
                NanoLine.DOTStrainC, NanoLine.PainLanceDoT, NanoLine.MINIDoT
            };

            actionTarget.Target = null;

            if (Team.IsInTeam)
                actionTarget.Target = Team.Members.FirstOrDefault(t => t.Character != null && t.Character.IsInLineOfSight && t.Character.IsAlive && (perkAction.AttackRange == 0 || TargetInPerkRange(perkAction, t.Character)) && t.Character.Buffs.Contains(debuffs))?.Character;

            else if (DynelManager.LocalPlayer.Buffs.Contains(debuffs))
                actionTarget.Target = DynelManager.LocalPlayer;

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        private bool Root_Snare_CleansePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (perkAction.IsExecuting) return false;
            if (!perkAction.IsAvailable) return false;

            var roots = new NanoLine[] { NanoLine.Root, NanoLine.Snare };
            actionTarget.Target = null;

            if (Team.IsInTeam)
                actionTarget.Target = Team.Members.FirstOrDefault(c => c.Character != null && c.Character.IsInLineOfSight && c.Character.IsAlive && (perkAction.AttackRange == 0 || TargetInPerkRange(perkAction, c.Character)) && c.Character.Buffs.Contains(roots))?.Character;
            else if (DynelManager.LocalPlayer.Buffs.Contains(roots))
                actionTarget.Target = DynelManager.LocalPlayer;

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        private bool Removal1Perk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)// target only
        {
            if (Game.IsZoning) return false;
            if (perkAction.IsExecuting) return false;
            if (!perkAction.IsAvailable) return false;

            if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Snare)) return false;

            actionTarget.ShouldSetTarget = true;
            actionTarget.Target = DynelManager.LocalPlayer;
            return true;
        }

        private bool Removal2Perk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)// target only
        {
            if (Game.IsZoning) return false;
            if (perkAction.IsExecuting) return false;
            if (!perkAction.IsAvailable) return false;

            if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Root)) return false;

            actionTarget.ShouldSetTarget = true;
            actionTarget.Target = DynelManager.LocalPlayer;
            return true;
        }

        private bool ViralWipePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (perkAction.IsExecuting) return false;
            if (!perkAction.IsAvailable) return false;

            var debuffs = new NanoLine[] { NanoLine.DOT_LineA, NanoLine.DOT_LineB, NanoLine.DOTNanotechnicianStrainA, NanoLine.DOTAgentStrainA,
                NanoLine.DOTNanotechnicianStrainB, NanoLine.DOTStrainC, NanoLine.PainLanceDoT, NanoLine.MINIDoT };

            if (!DynelManager.LocalPlayer.Buffs.Contains(debuffs)) return false;

            actionTarget.ShouldSetTarget = true;
            actionTarget.Target = DynelManager.LocalPlayer;
            return true;
        }
        private bool SealWoundsPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (perkAction.IsExecuting) return false;
            if (!perkAction.IsAvailable) return false;

            var dots = new NanoLine[] { NanoLine.DOT_LineA, NanoLine.DOT_LineB, NanoLine.DOTNanotechnicianStrainA, NanoLine.DOTAgentStrainA,
                NanoLine.DOTNanotechnicianStrainB, NanoLine.DOTStrainC, NanoLine.PainLanceDoT, NanoLine.MINIDoT };

            actionTarget.Target = null;

            if (Team.IsInTeam)
                actionTarget.Target = Team.Members.FirstOrDefault(t => t.Character != null && t.Character.IsInLineOfSight && t.Character.IsAlive && (perkAction.AttackRange == 0 || TargetInPerkRange(perkAction, t.Character)) && t.Character.Buffs.Contains(dots))?.Character;
            else if (DynelManager.LocalPlayer.Buffs.Contains(dots))
                actionTarget.Target = DynelManager.LocalPlayer;

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        private bool ShutdownRemovalPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (perkAction.IsExecuting) return false;
            if (!perkAction.IsAvailable) return false;

            actionTarget.Target = null;

            if (Team.IsInTeam)
                actionTarget.Target = Team.Members.FirstOrDefault(t => t.Character != null && t.Character.IsInLineOfSight && t.Character.IsAlive && (perkAction.AttackRange == 0 || TargetInPerkRange(perkAction, t.Character)) && t.Character.Buffs.Contains(NanoLine.NanoShutdownDebuff))?.Character;
            else if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.NanoShutdownDebuff))
                actionTarget.Target = DynelManager.LocalPlayer;

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        #endregion

        #region Pets Perks

        private bool PetHealPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (fightingTarget == null) return false;
            if (perkAction.IsExecuting) return false;

            foreach (Pet pet in DynelManager.LocalPlayer.Pets)
            {
                if (pet.Character == null) continue;

                if (pet.Character.HealthPercent <= 90)
                {
                    actionTarget.ShouldSetTarget = true;
                    actionTarget.Target = pet.Character;
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Checks

        private bool TargetInPerkRange(PerkAction perk, SimpleChar target)
        {
            try
            {
                if (perk == null) return false;
                if (!perk.IsAvailable) return false;
                if (perk.IsPending) return false;
                if (perk.IsExecuting) return false;
                if (perk.AttackRange == 0) return false;
                if (target == null) return false;
                if (DynelManager.LocalPlayer.Position == null) return false;
                if (DynelManager.LocalPlayer.Velocity > 0) return false;
                if (DynelManager.LocalPlayer.Position.DistanceFrom(target.Position) > perk.AttackRange) return false;

                try { if (!perk.MeetsUseReqs(target)) return false; }
                catch { return false; }

                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        protected bool CanUsePerk(PerkAction perkAction)
        {
            try
            {
                if (Game.IsZoning) return false;
                if (Now < _lastZonedTime) return false;
                if (Item.HasPendingUse) return false;
                if (!perkAction.IsAvailable) return false;
                if (perkAction.IsPending) return false;
                if (perkAction.IsExecuting) return false;

                    if (_actionQueue.Any(x => x.CombatAction is PerkAction action && action == perkAction)) return false;
                if (IsPlayerFlyingOrFalling()) return false;

                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        #endregion
    }
}