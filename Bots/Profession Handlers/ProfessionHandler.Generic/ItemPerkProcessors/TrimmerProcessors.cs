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
        public static HashSet<int> OwnedTrimmers = new HashSet<int>();

        private float DelayBetweenFiveMinTrims = 301;
        //private float DelayBetweenActuatorTrims = 3610;

        private double AttackMechEngi;
        private double SupportMechEngi;

        private double AttackElecEngi;
        private double SupportElecEngi;

        //private double AttackDivertEnergyToAvoidance;
        //private double SupportDivertEnergyToAvoidance;

        //private double AttackDivertEnergyToHitpoints;
        //private double SupportDivertEnergyToHitpoints;

        //private int Aggressiveness = 60; //Increase Aggressiveness
        //private int AggDef = 0; //Aggressive-Defensive

        public static readonly int[] IncreaseAggressiveness = { 154940, 154939 };

        public static readonly int[] AggressiveDefensiveSelectionArray =
        {
            88386, 88385, // NegativeAggressiveDefensive
            88384, 88383 // PositiveAggressiveDefensive
        };

        //Locks Mech Engi
        public static readonly int[] DmgChangeSelectionArray =
        {
            249107, // ColdDamageModifier
            249109, // FireDamageModifier
            249110, // EnergyDamageModifier
        };

        //Locks Mech Engi
        public static readonly int[] MechEngiSelectionArray =
        {
            87936, 87893, // DivertEnergyToDefense
            88378, 88377, // DivertEnergyToOffense
            //253189, 253188 // ImproveActuators
        };

        //Locks Elec Engi
        public static readonly int[] ElecEngiSelectionArray =
        {
            88380, 88379, // DivertEnergyToAvoidance
            88382, 88381 // DivertEnergyToHitpoints
        };

        private bool TrimmerMethod(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (item == null) return false;
            if (Game.IsZoning) return false;
            if (Now < _lastZonedTime) return false;
            if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) >= 1) return false;
            if (Item.HasPendingUse) return false;
            if (PerkAction.List.Any(perk => perk.IsExecuting)) return false;
            if (DynelManager.LocalPlayer.Pets == null || !DynelManager.LocalPlayer.Pets.Any()) return false;
            if (!item.MeetsSelfUseReqs()) return false;

            actionTarget.Target = null;

            foreach (var pet in DynelManager.LocalPlayer.Pets)
            {
                if (actionTarget.Target != null) continue;
                if (pet.Character == null) continue;
                if (pet.Character.Breed != Breed.HumanMonster) continue;
                if (pet.Character.GetStat(Stat.NPCFamily) != 95) continue;

                switch (item.Id)
                {
                    #region Permanent non-combat buffs

                    case 154940: // IncreaseAggressiveness
                    case 154939: // IncreaseAggressiveness
                        if (fightingTarget == null)
                        {
                            switch (pet.Type)
                            {
                                case PetType.Attack:
                                    if (_settings[$"AttackIncreaseAggressivenessCheckBox"].AsBool() && !AttackPetIncreaseAggressiveness)
                                    {
                                        actionTarget.Target = pet.Character;
                                        item.Use(actionTarget.Target, true);
                                        AttackPetIncreaseAggressiveness = true;
                                        continue;
                                    }
                                    break;
                                case PetType.Support:
                                    if (_settings[$"SupportIncreaseAggressivenessCheckBox"].AsBool() && !SupportPetIncreaseAggressiveness)
                                    {
                                        actionTarget.Target = pet.Character;
                                        item.Use(actionTarget.Target, true);
                                        SupportPetIncreaseAggressiveness = true;
                                        continue;
                                    }
                                    break;
                            }
                        }
                        break;

                    case 88386:  // NegativeAggressiveDefensive
                    case 88385:  // NegativeAggressiveDefensive
                    case 88384:  // PositiveAggressiveDefensive
                    case 88383:  // PositiveAggressiveDefensive
                        if (fightingTarget == null)
                        {
                            switch (pet.Type)
                            {
                                case PetType.Attack:
                                    if (_settings["AttackAggressiveDefensiveSelectionArrayCheckBox"].AsBool() && !AttackPetAggressiveDefensive && item.Id == _settings["AttackAggressiveDefensiveSelectionArray"].AsInt32())
                                    {
                                        actionTarget.Target = pet.Character;
                                        item.Use(actionTarget.Target, true);
                                        AttackPetAggressiveDefensive = true;
                                        continue;
                                    }
                                    break;
                                case PetType.Support:
                                    if (_settings["SupportAggressiveDefensiveSelectionArrayCheckBox"].AsBool() && !SupportPetAggressiveDefensive && item.Id == _settings["SupportAggressiveDefensiveSelectionArray"].AsInt32())
                                    {
                                        actionTarget.Target = pet.Character;
                                        item.Use(actionTarget.Target, true);
                                        SupportPetAggressiveDefensive = true;
                                        continue;
                                    }
                                    break;
                            }
                        }
                        break;

                    //Target Lock Mechanical engineering 300s
                    case 249107: // ColdDamageModifier
                    case 249109: // FireDamageModifier
                    case 249110: // EnergyDamageModifier
                        if (fightingTarget == null)
                        {
                            switch (pet.Type)
                            {
                                case PetType.Attack:
                                    if (_settings["AttackDmgChangeSelectionArrayCheckBox"].AsBool() && !pet.Character.Buffs.Contains(NanoLine.DamageChangeBuffs) && item.Id == _settings["AttackDmgChangeSelectionArray"].AsInt32())
                                    {
                                        actionTarget.Target = pet.Character;
                                        item.Use(actionTarget.Target, true);
                                        AttackMechEngi = Time.AONormalTime + 301;
                                        continue;
                                    }
                                    break;

                                case PetType.Support:
                                    if (_settings["SupportDmgChangeSelectionArrayCheckBox"].AsBool() && !pet.Character.Buffs.Contains(NanoLine.DamageChangeBuffs) && item.Id == _settings["SupportDmgChangeSelectionArray"].AsInt32())
                                    {
                                        actionTarget.Target = pet.Character;
                                        item.Use(actionTarget.Target, true);
                                        SupportMechEngi = Time.AONormalTime + 301;
                                        continue;
                                    }
                                    break;
                            }
                        }
                        break;

                    #endregion

                    #region Combat buffs (5 min lockouts)

                    //Target Lock Mechanical engineering 300s
                    case 87936:  // DivertEnergyToDefense
                    case 87893:  // DivertEnergyToDefense
                    case 88378:  // DivertEnergyToOffense
                    case 88377:  // DivertEnergyToOffense
                        if (fightingTarget != null)
                        {
                            switch (pet.Type)
                            {
                                case PetType.Attack:
                                    if (_settings["AttackMechEngiSelectionArrayCheckBox"].AsBool() && item.Id == _settings["AttackMechEngiSelectionArray"].AsInt32())
                                    {
                                        if (Time.AONormalTime > AttackMechEngi)
                                        {
                                            actionTarget.Target = pet.Character;
                                            item.Use(actionTarget.Target, true);
                                            AttackMechEngi = Time.AONormalTime + 301;
                                            continue;
                                        }
                                    }
                                    break;
                                case PetType.Support:
                                    if (_settings["SupportMechEngiSelectionArrayCheckBox"].AsBool() && item.Id == _settings["SupportMechEngiSelectionArray"].AsInt32())
                                    {
                                        if (Time.AONormalTime > SupportMechEngi)
                                        {
                                            actionTarget.Target = pet.Character;
                                            item.Use(actionTarget.Target, true);
                                            SupportMechEngi = Time.AONormalTime + 301;
                                            continue;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;

                    //User Lock	Mechanical engineering 3600s
                    case 253189: // ImproveActuators
                    case 253188: // ImproveActuators
                        break;

                    //Target Lock Electrical engineering 300s
                    case 88380:  // DivertEnergyToAvoidance
                    case 88379:  // DivertEnergyToAvoidance
                        if (fightingTarget != null)
                        {
                            switch (pet.Type)
                            {
                                case PetType.Attack:
                                    if (_settings["AttackElecEngiSelectionArrayCheckBox"].AsBool() && item.Id == _settings["AttackElecEngiSelectionArray"].AsInt32())
                                    {
                                        if (Time.AONormalTime > AttackElecEngi)
                                        {
                                            actionTarget.Target = pet.Character;
                                            item.Use(actionTarget.Target, true);
                                            AttackElecEngi = Time.AONormalTime + 301;
                                            continue;
                                        }
                                    }
                                    break;

                                case PetType.Support:
                                    if (_settings["SupportElecEngiSelectionArrayCheckBox"].AsBool() && item.Id == _settings["SupportElecEngiSelectionArray"].AsInt32())
                                    {
                                        if (Time.AONormalTime > SupportElecEngi)
                                        {
                                            actionTarget.Target = pet.Character;
                                            item.Use(actionTarget.Target, true);
                                            SupportElecEngi = Time.AONormalTime + 301;
                                            continue;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;

                    //Target Lock Electrical engineering 300s
                    case 88382:  // DivertEnergyToHitpoints
                    case 88381:  // DivertEnergyToHitpoints
                        if (fightingTarget != null)
                        {
                            switch (pet.Type)
                            {
                                case PetType.Attack:
                                    if (_settings["AttackElecEngiSelectionArrayCheckBox"].AsBool() && item.Id == _settings["AttackElecEngiSelectionArray"].AsInt32())
                                    {
                                        if (Time.AONormalTime > AttackElecEngi)
                                        {
                                            actionTarget.Target = pet.Character;
                                            item.Use(actionTarget.Target, true);
                                            AttackElecEngi = Time.AONormalTime + 301;
                                            continue;
                                        }
                                    }
                                    break;

                                case PetType.Support:
                                    if (_settings["SupportElecEngiSelectionArrayCheckBox"].AsBool() && item.Id == _settings["SupportElecEngiSelectionArray"].AsInt32())
                                    {
                                        if (Time.AONormalTime > SupportElecEngi)
                                        {
                                            actionTarget.Target = pet.Character;
                                            item.Use(actionTarget.Target, true);
                                            SupportElecEngi = Time.AONormalTime + 301;
                                            continue;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;

                        #endregion
                }
            }

            return false;
        }

        //private bool CheckTrimmer(Item item, Pet pet)
        //{
        //    switch (item.Id)
        //    {
        //        #region Permanent non-combat buffs

        //        case 154940: // IncreaseAggressiveness
        //        case 154939: // IncreaseAggressiveness
        //            if (pet.Type == PetType.Attack)
        //            {
        //                AttackPetIncreaseAggressiveness = true;
        //                return true;
        //            }
        //            else if (pet.Type == PetType.Support)
        //            {
        //                SupportPetIncreaseAggressiveness = true;
        //                return true;
        //            }
        //            return false;
        //        case 88386:  // NegativeAggressiveDefensive
        //        case 88385:  // NegativeAggressiveDefensive
        //        case 88384:  // PositiveAggressiveDefensive
        //        case 88383:  // PositiveAggressiveDefensive
        //            if (pet.Type == PetType.Attack)
        //            {
        //                if (_settings["AttackIncreaseAggressivenessCheckBox"].AsBool() && !AttackPetIncreaseAggressiveness)
        //                    return false;

        //                AttackPetAggressiveDefensive = true;
        //                return true;
        //            }
        //            else if (pet.Type == PetType.Support)
        //            {
        //                if (_settings["SupportIncreaseAggressivenessCheckBox"].AsBool() && !SupportPetIncreaseAggressiveness)
        //                    return false;

        //                SupportPetAggressiveDefensive = true;
        //                return true;
        //            }
        //            return false;
        //        case 249107: // ColdDamageModifier
        //        case 249109: // FireDamageModifier
        //        case 249110: // EnergyDamageModifier
        //            if (pet.Type == PetType.Attack)
        //            {
        //                if (pet.Character.Buffs.Contains(NanoLine.DamageChangeBuffs))
        //                {
        //                    AttackMechEngi = Time.AONormalTime;
        //                    AttackPetDamageModifier = true;
        //                    return false;
        //                }

        //                if (Time.AONormalTime < AttackMechEngi + DelayBetweenFiveMinTrims) return false;
        //                return true;
        //            }
        //            else if (pet.Type == PetType.Support)
        //            {
        //                if (pet.Character.Buffs.Contains(NanoLine.DamageChangeBuffs))
        //                {
        //                    SupportMechEngi = Time.AONormalTime;
        //                    SupportPetDamageModifier = true;
        //                }

        //                if (Time.AONormalTime < SupportMechEngi + DelayBetweenFiveMinTrims) return false;
        //                return true;
        //            }
        //            return false;

        //        #endregion

        //        #region Combat buffs (5 min lockouts)

        //        case 87936:  // DivertEnergyToDefense
        //        case 87893:  // DivertEnergyToDefense
        //        case 88378:  // DivertEnergyToOffense
        //        case 88377:  // DivertEnergyToOffense
        //            if (pet.Type == PetType.Attack)
        //            {
        //                if (Time.AONormalTime < AttackMechEngi + DelayBetweenFiveMinTrims) return false;
        //                AttackMechEngi = Time.AONormalTime;
        //                return true;
        //            }
        //            else if (pet.Type == PetType.Support)
        //            {
        //                if (Time.AONormalTime < SupportMechEngi + DelayBetweenFiveMinTrims) return false;
        //                SupportMechEngi = Time.AONormalTime;
        //                return true;
        //            }
        //            return false;

        //        //case 253189: // ImproveActuators
        //        //case 253188: // ImproveActuators
        //        //break;

        //        case 88380:  // DivertEnergyToAvoidance
        //        case 88379:  // DivertEnergyToAvoidance
        //            if (pet.Type == PetType.Attack)
        //            {
        //                if (_settings["AttackMechEngiSelectionArrayCheckBox"].AsBool() &&
        //                    !(AttackMechEngi > 0 && Time.AONormalTime < AttackMechEngi + DelayBetweenFiveMinTrims))
        //                    return false;

        //                if (Time.AONormalTime < AttackDivertEnergyToAvoidance + DelayBetweenFiveMinTrims) return false;
        //                AttackDivertEnergyToAvoidance = Time.AONormalTime;
        //                return true;
        //            }
        //            else if (pet.Type == PetType.Support)
        //            {
        //                if (_settings["SupportMechEngiSelectionArrayCheckBox"].AsBool() &&
        //                    !(SupportMechEngi > 0 && Time.AONormalTime < SupportMechEngi + DelayBetweenFiveMinTrims))
        //                    return false;

        //                if (Time.AONormalTime < SupportDivertEnergyToAvoidance + DelayBetweenFiveMinTrims) return false;
        //                SupportDivertEnergyToAvoidance = Time.AONormalTime;
        //                return true;
        //            }
        //            return false;

        //        case 88382:  // DivertEnergyToHitpoints
        //        case 88381:  // DivertEnergyToHitpoints
        //            if (pet.Type == PetType.Attack)
        //            {
        //                if ((_settings["AttackMechEngiSelectionArrayCheckBox"].AsBool() &&
        //                     !(AttackMechEngi > 0 && Time.AONormalTime < AttackMechEngi + DelayBetweenFiveMinTrims)) ||
        //                    (_settings["AttackElecEngiSelectionArrayCheckBox"].AsBool() &&
        //                     !(AttackDivertEnergyToAvoidance > 0 && Time.AONormalTime < AttackDivertEnergyToAvoidance + DelayBetweenFiveMinTrims)))
        //                    return false;

        //                if (Time.AONormalTime < AttackDivertEnergyToHitpoints + DelayBetweenFiveMinTrims) return false;
        //                AttackDivertEnergyToHitpoints = Time.AONormalTime;
        //                return true;
        //            }
        //            else if (pet.Type == PetType.Support)
        //            {
        //                if ((_settings["SupportMechEngiSelectionArrayCheckBox"].AsBool() &&
        //                     !(SupportMechEngi > 0 && Time.AONormalTime < SupportMechEngi + DelayBetweenFiveMinTrims)) ||
        //                    (_settings["SupportElecEngiSelectionArrayCheckBox"].AsBool() &&
        //                     !(SupportDivertEnergyToAvoidance > 0 && Time.AONormalTime < SupportDivertEnergyToAvoidance + DelayBetweenFiveMinTrims)))
        //                    return false;

        //                if (Time.AONormalTime < SupportDivertEnergyToHitpoints + DelayBetweenFiveMinTrims) return false;
        //                SupportDivertEnergyToHitpoints = Time.AONormalTime;
        //                return true;
        //            }
        //            return false;

        //            #endregion
        //    }

        //    return false;
        //}
    }
}