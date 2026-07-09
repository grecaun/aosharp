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
        public static List<bool> LoadedPetSpells = new List<bool> { false, false, false };
        public static HashSet<int> LoadedPetBuffSpells = new HashSet<int>();

        private bool AttackPetNCU = true;
        private bool SupportPetNCU = true;

        private bool AttackPetIncreaseAggressiveness = true;
        private bool SupportPetIncreaseAggressiveness = true;

        private bool AttackPetAggressiveDefensive = true;
        private bool SupportPetAggressiveDefensive = true;

        private bool AttackPetDamageModifier = true;
        private bool SupportPetDamageModifier = true;

        private int[] MPUrns = { 305555, 206256, 206255, 204709 };

        private void RegisterPetBuffSpells(int spellID)
        {
            try
            {
                #region Pet Cleanse

                if (SpellID.PetCleanse.Contains(spellID))
                {
                    LoadedPetBuffSpells.Add(spellID);

                    RegisterSpellProcessor(SpellID.PetCleanse, PetCleanse);
                }

                #endregion

                #region Pet Warp

                if (SpellID.PetWarps.Contains(spellID))
                {
                    LoadedPetBuffSpells.Add(spellID);

                    RegisterSpellProcessor(SpellID.PetWarps, PetWarp);
                }

                #endregion

                #region Pet Procs

                if (SpellID.SiphonBox683.Contains(spellID))
                {
                    LoadedPetBuffSpells.Add(spellID);
                    if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !PetBuffWindowController.Support.Any(c => c.SettingKey == "SupportPetProc"))
                        PetBuffWindowController.Support.Add(
                        new PetBuffUiConfig
                        {
                            SpellIds = SpellID.SiphonBox683,
                            SettingKey = "SupportPetProc",
                            Label = "Offensive Proc",
                            UiType = UiType.Checkbox
                        });

                    RegisterSpellProcessor(SpellID.SiphonBox683, PetProcBuff, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                #endregion

                #region Generic

                if (Spell.GetSpellsForNanoline(NanoLine.PetDamageOverTimeResistNanos).Any(s => s.Id == spellID))
                {
                    LoadedPetBuffSpells.Add(spellID);
                    if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist && !PetBuffWindowController.Support.Any(c => c.SettingKey == "SupportPetPetDamageOverTimeResistNanos"))
                        PetBuffWindowController.Support.Add(
                            new PetBuffUiConfig
                            {
                                SpellIds = Spell.GetSpellsForNanoline(NanoLine.PetDamageOverTimeResistNanos).Select(s => s.Id).ToArray(),
                                SettingKey = "SupportPetPetDamageOverTimeResistNanos",
                                Label = "Pet DoT Resist Buffs",
                                UiType = UiType.DropDownWOption
                            });

                    RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.PetDamageOverTimeResistNanos).OrderByStackingOrder(),
                        (Spell buffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        PetSettingBuff(buffSpell, fightingTarget, ref actionTarget, "PetDamageOverTimeResistNanos"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                if (SpellID.PetShortTermDamageBuffs.Contains(spellID))
                {
                    LoadedPetBuffSpells.Add(spellID);

                    if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !PetBuffWindowController.Support.Any(c => c.SettingKey == "SupportPetPetShortTermDamageBuffs"))
                        PetBuffWindowController.Support.Add(
                            new PetBuffUiConfig
                            {
                                SpellIds = SpellID.PetShortTermDamageBuffs,
                                SettingKey = "SupportPetPetShortTermDamageBuffs",
                                Label = "Pet Short Term Damage Buffs",
                                UiType = UiType.DropDownWOption
                            });

                    RegisterSpellProcessor(SpellID.PetShortTermDamageBuffs, (Spell buffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            PetSettingBuff(buffSpell, fightingTarget, ref actionTarget, "PetShortTermDamageBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                if (SpellID.PetDefensiveNanos.Contains(spellID))
                {
                    LoadedPetBuffSpells.Add(spellID);

                    if ((DynelManager.LocalPlayer.Profession == Profession.Metaphysicist || DynelManager.LocalPlayer.Profession == Profession.Engineer) &&
                        !PetBuffWindowController.Support.Any(c => c.SettingKey == "SupportPetPetDefensiveNanos"))
                        PetBuffWindowController.Support.Add(
                            new PetBuffUiConfig
                            {
                                SpellIds = SpellID.PetDefensiveNanos,
                                SettingKey = "SupportPetPetDefensiveNanos",
                                Label = "Pet Defensive Buffs",
                                UiType = UiType.DropDownWOption
                            });

                    if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist && !PetBuffWindowController.Heal.Any(c => c.SettingKey == "HealPetPetDefensiveNanos"))
                        PetBuffWindowController.Heal.Add(
                            new PetBuffUiConfig
                            {
                                SpellIds = SpellID.PetDefensiveNanos,
                                SettingKey = "HealPetPetDefensiveNanos",
                                Label = "Pet Defensive Buffs",
                                UiType = UiType.DropDownWOption
                            });

                    RegisterSpellProcessor(SpellID.PetDefensiveNanos, (Spell buffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            PetSettingBuff(buffSpell, fightingTarget, ref actionTarget, "PetDefensiveNanos"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.MPPetInitiativeBuffs).Any(s => s.Id == spellID))
                {
                    LoadedPetBuffSpells.Add(spellID);

                    if (DynelManager.LocalPlayer.Profession == Profession.Engineer && !PetBuffWindowController.Support.Any(c => c.SettingKey == "SupportPetMPPetInitiativeBuffs"))
                        PetBuffWindowController.Support.Add(
                            new PetBuffUiConfig
                            {
                                SpellIds = Spell.GetSpellsForNanoline(NanoLine.MPPetInitiativeBuffs).Select(s => s.Id).ToArray(),
                                SettingKey = "SupportPetMPPetInitiativeBuffs",
                                Label = "MP Pet Initiative Buffs",
                                UiType = UiType.DropDownWOption
                            });


                    RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.MPPetInitiativeBuffs).OrderByStackingOrder(),
                        (Spell buffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            PetSettingBuff(buffSpell, fightingTarget, ref actionTarget, "MPPetInitiativeBuffs"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                }

                #endregion

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Bureaucrat:

                        #region Pet Spawners

                        if (CratPetsList.Pets.Any(x => x.Value.PetType == PetType.Attack && x.Key == spellID))
                        {
                            LoadedPetSpells[0] = true;
                            LoadedPetBuffSpells.Add(spellID);

                            if (!PetBuffWindowController.Attack.Any(c => c.SettingKey == "AttackSpareShell"))
                                PetBuffWindowController.Attack.Add(new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeNano }, SettingKey = "AttackSpareShell", Label = "Cast Spare Shell", UiType = UiType.Checkbox });

                            RegisterSpellProcessor(_settings["SpawnAttackPet"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            ShellSpawner(CratPetsList.Pets, PetType.Attack, spell, fightingTarget, ref actionTarget, "SpawnAttackPet"), CombatActionPriority.NonCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.SupportPets).Any(s => s.Id == spellID))
                        {
                            LoadedPetSpells[1] = true;
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(_settings["SpawnSupportPet"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            NoShellPetSpawner(PetType.Support, spell, fightingTarget, ref actionTarget, "SpawnSupportPet"), CombatActionPriority.NonCombat);
                        }

                        #endregion

                        #region Pet Buffs

                        if (Spell.GetSpellsForNanoline(NanoLine.PetTauntBuff).Any(s => s.Id == spellID))
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.PetTauntBuff).OrderByStackingOrder(),
                                 (Spell buffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                     PetSettingBuff(buffSpell, fightingTarget, ref actionTarget, "PetTauntBuff"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.DroidDamageMatrix == spellID)// 
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.DroidDamageMatrix, DroidDamageMatrix, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        #endregion

                        break;

                    case Profession.Engineer:
                        #region Pet Spawners

                        if (EngiePetsList.Pets.Any(c => c.Value.PetType == PetType.Attack && c.Key == spellID))
                        {
                            LoadedPetSpells[0] = true;
                            LoadedPetBuffSpells.Add(spellID);

                            if (!PetBuffWindowController.Attack.Any(c => c.SettingKey == "AttackSpareShell"))
                                PetBuffWindowController.Attack.Add(new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeNano }, SettingKey = "AttackSpareShell", Label = "Cast Spare Shell", UiType = UiType.Checkbox });

                            RegisterSpellProcessor(_settings["SpawnAttackPet"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            ShellSpawner(EngiePetsList.Pets, PetType.Attack, spell, fightingTarget, ref actionTarget, "SpawnAttackPet"), CombatActionPriority.NonCombat);

                        }

                        if (EngiePetsList.Pets.Any(c => c.Value.PetType == PetType.Support && c.Key == spellID))
                        {
                            LoadedPetSpells[1] = true;
                            LoadedPetBuffSpells.Add(spellID);

                            if (!PetBuffWindowController.Support.Any(c => c.SettingKey == "SupportSpareShell"))
                                PetBuffWindowController.Support.Add(new PetBuffUiConfig { SpellIds = new[] { SpellID.CompositeNano }, SettingKey = "SupportSpareShell", Label = "Cast Spare Shell", UiType = UiType.Checkbox });

                            RegisterSpellProcessor(_settings["SpawnSupportPet"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            ShellSpawner(EngiePetsList.Pets, PetType.Support, spell, fightingTarget, ref actionTarget, "SpawnSupportPet"), CombatActionPriority.NonCombat);

                        }

                        #endregion

                        #region Pet Buffs

                        if (Spell.GetSpellsForNanoline(NanoLine.EngineerMiniaturization).Any(s => s.Id == spellID))
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.EngineerMiniaturization).OrderByStackingOrder(), EngineerMiniaturization, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (SpellID.ShieldOfObedientServant.Contains(spellID))
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.ShieldOfObedientServant, ShieldOfTheObedientServant, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        #endregion

                        break;
                    case Profession.Agent:
                    case Profession.Metaphysicist:

                        #region Pets

                        if (Spell.GetSpellsForNanoline(NanoLine.AttackPets).Any(s => s.Id == spellID))
                        {
                            LoadedPetSpells[0] = true;
                            LoadedPetBuffSpells.Add(spellID);

                            if (Spell.GetSpellsForNanoline(NanoLine.AttackPets).Any(s => s.Id == _settings["SpawnAttackPet"].AsInt32()))
                                RegisterSpellProcessor(_settings["SpawnAttackPet"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                                NoShellPetSpawner(PetType.Attack, spell, fightingTarget, ref actionTarget, "SpawnAttackPet"), CombatActionPriority.NonCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.SupportPets).Any(s => s.Id == spellID))
                        {
                            LoadedPetSpells[1] = true;
                            LoadedPetBuffSpells.Add(spellID);
                            LoadedHolds.Add(spellID);
                            RegisterSpellProcessor(_settings["SpawnSupportPet"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            NoShellPetSpawner(PetType.Support, spell, fightingTarget, ref actionTarget, "SpawnSupportPet"), CombatActionPriority.NonCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.HealPets).Any(s => s.Id == spellID))
                        {
                            LoadedPetSpells[2] = true;
                            LoadedPetBuffSpells.Add(spellID);
                            LoadedHeals.Add(spellID);
                            RegisterSpellProcessor(_settings["SpawnHealPet"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            NoShellPetSpawner(PetType.Heal, spell, fightingTarget, ref actionTarget, "SpawnHealPet"), CombatActionPriority.NonCombat);
                        }

                        #endregion

                        if (SpellID.InstillDamageBuffs.Contains(spellID))
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(SpellID.InstillDamageBuffs, InstillDamage, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.MesmerizationConstructEmpowerment).Any(s => s.Id == spellID))
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.MesmerizationConstructEmpowerment).OrderByStackingOrder(), MezzPetSeed, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.HealingConstructEmpowerment).Any(s => s.Id == spellID))
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.HealingConstructEmpowerment).OrderByStackingOrder(), HealPetSeed, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.AggressiveConstructEmpowerment).Any(s => s.Id == spellID))
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.AggressiveConstructEmpowerment).OrderByStackingOrder(), AttackPetSeed, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.MPAttackPetDamageType).Any(s => s.Id == spellID))
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.MPAttackPetDamageType).OrderByStackingOrder(), DamageTypePet, CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.PetHealDelta843).Any(s => s.Id == spellID))
                        {
                            LoadedPetBuffSpells.Add(spellID);
                            RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.PetHealDelta843).OrderByStackingOrder(), (Spell buffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            PetSettingBuff(buffSpell, fightingTarget, ref actionTarget, "PetHealDelta843"), CombatActionPriority.NonCombat, RuleContext.OutOfCombat);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Pets

        protected bool NoShellPetSpawner(PetType petType, Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting = null)
        {
            if (!CanCast(spell)) return false;
            if (_settings["NanoSpellBuffs"].AsBool())
            {
                if (!DynelManager.LocalPlayer.Buffs.Contains(SpellID.MPCompositeNano))
                    return false;
            }

            if (_settings["WrangelBuffs"].AsBool())
            {
                if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.TraderTeamSkillWranglerBuff))
                    return false;
            }

            if (!spell.MeetsSelfUseReqs()) return false;
            if (setting != null)
            {
                if (!_settings[setting + "CheckBox"].AsBool()) return false;
                if (_settings[setting].AsInt32() != spell.Id) return false;
            }

            if (!CanSpawnPets(petType)) return false;
            if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) > 1) return false;
            var debuffs = new NanoLine[] { NanoLine.TraderSkillTransferTargetDebuff_Deprive, NanoLine.TraderSkillTransferTargetDebuff_Ransack, NanoLine.NanoShutdownDebuff, NanoLine.TraderShutdownSkillDebuff };
            if (DynelManager.LocalPlayer.Buffs.Contains(debuffs)) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }

        private bool ShellSpawner(Dictionary<int, PetSpellData> petData, PetType petType, Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!CanCast(spell)) return false;

            if (_settings["NanoSpellBuffs"].AsBool())
            {
                if (!DynelManager.LocalPlayer.Buffs.Contains(SpellID.MPCompositeNano))
                    return false;
            }

            if (_settings["WrangelBuffs"].AsBool())
            {
                if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.TraderTeamSkillWranglerBuff))
                    return false;
            }

            if (!spell.MeetsSelfUseReqs()) return false;
            if (Item.HasPendingUse) return false;
            if (PerkAction.List.Any(p => p.IsExecuting)) return false;

            if (!_settings[setting + "CheckBox"].AsBool()) return false;
            if (_settings[setting].AsInt32() != spell.Id) return false;
            
            if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) > 1) return false;
            
            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.TraderSkillTransferTargetDebuff_Deprive, NanoLine.TraderSkillTransferTargetDebuff_Ransack, NanoLine.NanoShutdownDebuff, NanoLine.TraderShutdownSkillDebuff })) return false;
            
            if (!petData.ContainsKey(spell.Id)) return false;

            if (Inventory.Find(petData[spell.Id].ShellId, out Item shell))
            {
                if (!CanSpawnPets(petData[spell.Id].PetType)) return false;
                //if (Item.HasPendingUse) return false;

                if (petType == PetType.Attack)
                {
                    AttackMechEngi = 0.0;
                    AttackElecEngi = 0.0;
                    //AttackDivertEnergyToAvoidance = 0.0;
                    //AttackDivertEnergyToHitpoints = 0.0;
                    AttackPetNCU = false;
                    AttackPetIncreaseAggressiveness = false;
                    AttackPetAggressiveDefensive = false;
                    AttackPetDamageModifier = false;
                }

                if (petType == PetType.Support)
                {
                    SupportMechEngi = 0.0;
                    SupportElecEngi = 0.0;
                    //SupportDivertEnergyToAvoidance = 0.0;
                    //SupportDivertEnergyToHitpoints = 0.0;
                    SupportPetNCU = false;
                    SupportPetIncreaseAggressiveness = false;
                    SupportPetAggressiveDefensive = false;
                    SupportPetDamageModifier = false;
                }

                shell?.Use();
            }

            if (Inventory.Items.Any(i => petData.Values.Any(p => p.PetType == petType && p.ShellId == i.Id))) return false;
            if (Inventory.NumFreeSlots < 2) return false;

            if (_settings[petType + "SpareShell"] != null && _settings[petType + "SpareShell"].AsBool())
            {
                actionTarget.ShouldSetTarget = false;
                return true;
            }

            if (DynelManager.LocalPlayer.Pets.Where(c => c.Type == petData[spell.Id].PetType || c.Type == PetType.Unknown).Count() >= 1) return false;

            if (!CanSpawnPets(petType)) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }

        private bool Urns(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (Now < _lastZonedTime) return false;
            if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) >= 1) return false;
            if (Item.HasPendingUse) return false;
            if (PerkAction.List.Any(perk => perk.IsExecuting)) return false;

            if (!CanSpawnPets(PetType.Attack)) return false;

            if (_settings["NanoSpellBuffs"].AsBool())
            {
                if (!DynelManager.LocalPlayer.Buffs.Contains(SpellID.MPCompositeNano))
                    return false;
            }

            if (_settings["WrangelBuffs"].AsBool())
            {
                if (!DynelManager.LocalPlayer.Buffs.Contains(NanoLine.TraderTeamSkillWranglerBuff))
                    return false;
            }

            if (!item.MeetsSelfUseReqs()) return false;

            string setting = "SpawnAttackPet";

            if (setting != null)
            {
                if (!_settings[setting + "CheckBox"].AsBool()) return false;
                if (_settings[setting].AsInt32() != item.Id) return false;
            }

            return true;
        }

        private bool CanSpawnPets(PetType petType)
        {
            return _settings[$"Spawn{petType}Pet"].AsBool() && CanLookupPetsAfterZone() && !PetAlreadySpawned(petType);
        }

        private bool PetAlreadySpawned(PetType petType)
        {
            return DynelManager.LocalPlayer.Pets.Any(c => (c.Type == PetType.Unknown || c.Type == petType));
        }

        protected bool CanLookupPetsAfterZone()
        {
            if (Game.IsZoning) return false;
            return Now > _lastZonedTime + 2.0;
        }

        #endregion

        #region Items

        private bool Robot_NCU_Upgrade(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (Game.IsZoning) return false;
            if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) >= 1) return false;
            if (fightingTarget != null || Attacked) return false;
            if (Item.HasPendingUse) return false;
            if (PerkAction.List.Any(perk => perk.IsExecuting)) return false;
            if (DynelManager.LocalPlayer.Pets == null || !DynelManager.LocalPlayer.Pets.Any()) return false;

            var robotPet = DynelManager.LocalPlayer.Pets.FirstOrDefault(pet => pet.Character != null && pet.Character.Breed == Breed.HumanMonster && pet.Character.GetStat(Stat.NPCFamily) == 95 && ((pet.Type == PetType.Attack && !AttackPetNCU) || (pet.Type == PetType.Support && !SupportPetNCU)));

            if (robotPet == null) return false;

            if (robotPet.Character.GetStat(Stat.QuantumFT) < 1)
            {
                if (robotPet.Type == PetType.Attack)
                    AttackPetNCU = true;
                else if (robotPet.Type == PetType.Support)
                    SupportPetNCU = true;
                return false;
            }

            if (_settings["AttackPetNCU"].AsBool() && robotPet.Type == PetType.Attack)
            {
                actionTarget.Target = robotPet.Character;
                if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
            }

            if (_settings["SupportPetNCU"].AsBool() && robotPet.Type == PetType.Support)
            {
                actionTarget.Target = robotPet.Character;
                if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
            }

            return false;
        }

        private bool PetItems()
        {

            return false;
        }

        #endregion

        #region Perks

        private bool ChannelRagePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!_settings["ChannelRage"].AsBool()) return false;
            if (!CanLookupPetsAfterZone()) return false;
            if (!CanUsePerk(perkAction)) return false;

            var attackPet = DynelManager.LocalPlayer.Pets.FirstOrDefault(pet => pet.Character != null && pet.Type == PetType.Attack && !pet.Character.Buffs.Contains(NanoLine.ChannelRage))?.Character;

            if (attackPet == null) return false;

            actionTarget = (attackPet, true);
            return true;
        }

        private bool PetCombatBuffPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string settingBool)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!_settings[settingBool].AsBool()) return false;
            if (!CanLookupPetsAfterZone()) return false;
            if (!CanUsePerk(perkAction)) return false;

            var buff = new int[] { };

            switch (perkAction.Hash)
            {
                case PerkHash.KenFi:
                    buff = new int[] { 271007, 271006, 271005, 271004, 271003, 271002, 271001, 271000, 270999, 252294 };
                    break;
                case PerkHash.Puppeteer:
                    buff = new int[] { 251295 };
                    break;
                case PerkHash.OptimizeBotProtocol:
                    buff = new int[] { 251903 };
                    break;
            }

            var attackPet = DynelManager.LocalPlayer.Pets.FirstOrDefault(pet => pet.Character != null && pet.Type == PetType.Attack && !pet.Character.Buffs.Contains(buff))?.Character;

            if (attackPet == null) return false;

            actionTarget = (attackPet, true);
            return true;
        }

        private bool GenericPetPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!CanLookupPetsAfterZone()) return false;

            actionTarget.Target = null;

            if (_settings["AttackPetGadgeteerCheckBox"].AsBool())
            {
                if (_settings["AttackPetGadgeteer"].AsInt32() == (int)perkAction.Hash)
                {
                    switch (perkAction.Hash)
                    {
                        case PerkHash.TauntBox:
                            if (!CanUsePerk(perkAction)) return false;
                            break;
                        case PerkHash.ChaoticEnergy:
                        case PerkHash.SiphonBox:
                            if (!CanUsePerk(perkAction)) return false;
                            break;
                    }

                    actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Character != null && p.Type == PetType.Attack && !p.Character.Buffs.Contains(NanoLine.GadgeteerPetProcs))?.Character;

                    if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                }
            }

            if (_settings["SupportPetGadgeteerCheckBox"].AsBool())
            {
                if (_settings["SupportPetGadgeteer"].AsInt32() == (int)perkAction.Hash)
                {
                    switch (perkAction.Hash)
                    {
                        case PerkHash.TauntBox:
                            if (!CanUsePerk(perkAction)) return false;
                            break;
                        case PerkHash.ChaoticEnergy:
                        case PerkHash.SiphonBox:
                            if (!CanUsePerk(perkAction)) return false;
                            break;
                    }

                    actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Character != null && p.Type == PetType.Support && !p.Character.Buffs.Contains(NanoLine.GadgeteerPetProcs))?.Character;

                    if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                }
            }

            return false;
        }

        #endregion

        #region Warp

        private bool PetWarp(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["WarpPets"].AsBool() || !CanCast(spell) || !CanLookupPetsAfterZone()) return false;

            return DynelManager.LocalPlayer.Pets.Any(c => c.Character == null);
        }

        #endregion

        #region Cleanse

        protected bool PetCleanse(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!CanCast(spell)) return false;
                if (!CanLookupPetsAfterZone()) return false;

                actionTarget.Target = null;

                actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(p => p.Character != null && p.Character.Buffs != null && !p.Character.Buffs.Contains(224391)
                && p.Character.Buffs.Contains(new NanoLine[] { NanoLine.Root, NanoLine.Snare, NanoLine.Mezz }))?.Character;

                if (actionTarget.Target == null) return false;

                actionTarget = (actionTarget.Target, true);
                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        #endregion

        #region Procs

        private bool PetProcBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!_settings["BuffPets"].AsBool()) return false;
                if (!CanCast(spell)) return false;
                if (DynelManager.LocalPlayer.Pets == null || !DynelManager.LocalPlayer.Pets.Any()) return false;
                if (!CanLookupPetsAfterZone()) return false;

                actionTarget.Target = null;

                if (_settings["AttackPetProc"].AsBool())
                {
                    actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(ap => ap.Character != null && ap.Type == PetType.Attack
                    && ap.Character.GetStat(Stat.NPCFamily) == (int)NpcClan.EngineerAttackPet && !ap.Character.Buffs.Contains(NanoLine.SiphonBox683))?.Character;
                    if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                }

                if (_settings["SupportPetProc"].AsBool())
                {
                    actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(ap => ap.Character != null && ap.Type == PetType.Support
                    && ap.Character.GetStat(Stat.NPCFamily) == (int)NpcClan.EngineerAttackPet && !ap.Character.Buffs.Contains(NanoLine.SiphonBox683))?.Character;
                    if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
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

        #region Agent

        #endregion

        #region Bureaucrat

        private bool DroidDamageMatrix(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!_settings["AttackPetDroidDamageMatrix"].AsBool()) return false;
            if (!CanCast(spell)) return false;
            if (!CanLookupPetsAfterZone()) return false;

            var pet = DynelManager.LocalPlayer.Pets.FirstOrDefault(a => a.Type == PetType.Attack);

            if (pet?.Character == null) return false;
            if (pet.Character.Buffs.Contains(285696)) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }
        #endregion

        #region Engineer

        private bool EngineerMiniaturization(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!CanCast(spell)) return false;
            if (!CanLookupPetsAfterZone()) return false;

            actionTarget.Target = null;

            if (_settings["AttackPetEngineerMiniaturizationCheckBox"].AsBool() && spell.Id == _settings["AttackPetEngineerMiniaturization"].AsInt32())
            {
                actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(a => a.Character != null && a.Character.GetStat(Stat.NPCFamily) == 95 && a.Type == PetType.Attack && !a.Character.Buffs.Contains(NanoLine.EngineerMiniaturization))?.Character;
                if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
            }

            if (_settings["SupportPetEngineerMiniaturizationCheckBox"].AsBool() && spell.Id == _settings["SupportPetEngineerMiniaturization"].AsInt32())
            {
                actionTarget.Target = DynelManager.LocalPlayer.Pets.FirstOrDefault(a => a.Character != null && a.Character.GetStat(Stat.NPCFamily) == 95 && a.Type == PetType.Support && !a.Character.Buffs.Contains(NanoLine.EngineerMiniaturization))?.Character;
                if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
            }

            return false;
        }

        private bool ShieldOfTheObedientServant(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!CanCast(spell)) return false;
            if (!CanLookupPetsAfterZone()) return false;

            if (_settings["AttackPetShieldOfObedientServantCheckBox"].AsBool())
            {
                var pet = DynelManager.LocalPlayer.Pets.FirstOrDefault(p =>
                    p.Type == PetType.Attack &&
                    p.Character != null &&
                    !p.Character.Buffs.Contains(NanoLine.ShieldoftheObedientServant))?.Character;

                if (pet != null) { actionTarget = (pet, true); return true; }
            }

            if (_settings["SupportPetShieldOfObedientServantCheckBox"].AsBool())
            {
                var pet = DynelManager.LocalPlayer.Pets.FirstOrDefault(p =>
                    p.Type == PetType.Support &&
                    p.Character != null &&
                    !p.Character.Buffs.Contains(NanoLine.ShieldoftheObedientServant))?.Character;

                if (pet != null) { actionTarget = (pet, true); return true; }
            }

            return false;
        }

        #endregion

        #region Metaphysicist

        private bool SupportPetSpawner(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["SpawnSupportPet"].AsBool()) return false;

            if (!CanCast(spell)) return false;
            if (!CanLookupPetsAfterZone()) return false;
            if (DynelManager.LocalPlayer.Pets.Any(c => c.Type == PetType.Support)) return false;

            actionTarget.ShouldSetTarget = false;
            return true;
        }

        private bool MezzPetSeed(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!_settings["MesmerizationConstructEmpowermentCheckBox"].AsBool()) return false;
            if (spell.Id != _settings["MesmerizationConstructEmpowerment"].AsInt32()) return false;

            return PetTargetBuff(NanoLine.MesmerizationConstructEmpowerment, PetType.Support, spell, fightingTarget, ref actionTarget);
        }

        private bool HealPetSeed(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!_settings["HealingConstructEmpowermentCheckBox"].AsBool()) return false;
            if (spell.Id != _settings["HealingConstructEmpowerment"].AsInt32()) return false;

            return PetTargetBuff(NanoLine.HealingConstructEmpowerment, PetType.Heal, spell, fightingTarget, ref actionTarget);
        }

        private bool AttackPetSeed(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!_settings["AttackPetAggressiveConstructEmpowermentCheckBox"].AsBool()) return false;
            if (spell.Id != _settings["AttackPetAggressiveConstructEmpowerment"].AsInt32()) return false;

            return PetTargetBuff(NanoLine.AggressiveConstructEmpowerment, PetType.Attack, spell, fightingTarget, ref actionTarget);
        }

        private bool DamageTypePet(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!_settings["AttackPetMPAttackPetDamageTypeCheckBox"].AsBool()) return false;
            if (spell.Id != _settings["AttackPetMPAttackPetDamageType"].AsInt32()) return false;

            return PetTargetBuff(NanoLine.MPAttackPetDamageType, PetType.Attack, spell, fightingTarget, ref actionTarget);
        }

        private bool InstillDamage(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!_settings["AttackPetInstillDamageBuffsCheckBox"].AsBool()) return false;
            if (spell.Id != _settings["AttackPetInstillDamageBuffs"].AsInt32()) return false;

            return PetTargetBuff(NanoLine.MPPetDamageBuffs, PetType.Attack, spell, fightingTarget, ref actionTarget);
        }

        #endregion

        #region Trader

        #endregion

        #region Generic Pet Buff

        private bool PetSettingBuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!_settings["BuffPets"].AsBool()) return false;
            if (!CanCast(spell)) return false;
            if (!CanLookupPetsAfterZone()) return false;

            actionTarget.Target = null;

            //Chat.WriteLine($"{spell.Name}, {spell.Id}, {spell.Nanoline}, {_settings[$"AttackPet{setting}"].AsInt32()}");

            if ((actionTarget.Target = PetBuffCheck(spell, PetType.Attack, $"AttackPet{setting}CheckBox", $"AttackPet{setting}")) != null)
            { actionTarget = (actionTarget.Target, true); return true; }

            if ((actionTarget.Target = PetBuffCheck(spell, PetType.Heal, $"HealPet{setting}CheckBox", $"HealPet{setting}")) != null)
            { actionTarget = (actionTarget.Target, true); return true; }

            if ((actionTarget.Target = PetBuffCheck(spell, PetType.Support, $"SupportPet{setting}CheckBox", $"SupportPet{setting}")) != null)
            { actionTarget = (actionTarget.Target, true); return true; }

            return false;
        }

        protected bool PetTargetBuff(NanoLine buffNanoLine, PetType petType, Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!_settings["BuffPets"].AsBool()) return false;
                if (!CanCast(spell)) return false;
                if (!CanLookupPetsAfterZone()) return false;

                var target = DynelManager.LocalPlayer.Pets.FirstOrDefault(c => c.Character != null && c.Type == petType && !c.Character.Buffs.Contains(buffNanoLine));

                if (target == null) return false;
                actionTarget = (target.Character, true);
                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        private SimpleChar PetBuffCheck(Spell spell, PetType type, string checkBoxKey, string procKey)
        {
            try
            {
                var pet = DynelManager.LocalPlayer.Pets.Where(p => p.Character != null && p.Type == type && !p.Character.Buffs.Contains(spell.Nanoline) && spell.MeetsUseReqs(p.Character)).FirstOrDefault()?.Character;
                if (pet == null) return null;
                if (_settings[checkBoxKey] == null) return null;
                if (!_settings[checkBoxKey].AsBool()) return null;

                if (_settings[procKey].AsInt32() == 0) return null;
                if (_settings[procKey].AsInt32() != spell.Id) return null;

                if (SpellID.PetDefensiveNanos.Contains(spell.Id))
                    if (pet.Buffs.Contains(SpellID.PetDefensiveNanos)) return null;

                if (SpellID.PetShortTermDamageBuffs.Contains(spell.Id))
                    if (pet.Buffs.Contains(NanoLine.PetShortTermDamageBuffs)) return null;

                if (pet.Buffs.Contains(_settings[procKey].AsInt32())) return null;

                //Chat.WriteLine($"{spell.Name}, Spell Id = {spell.Id}, Spell NanoLine = {spell.Nanoline}, Setting = {_settings[procKey].AsInt32()}, Pet = {pet.Name}, Pet Type = {type}");

                return pet;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return null;
            }
        }

        #endregion
    }
}