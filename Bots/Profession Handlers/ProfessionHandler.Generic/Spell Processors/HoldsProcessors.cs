using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public static HashSet<int> LoadedHolds = new HashSet<int>();
        private void RegisterHoldsSpells(int spellID)
        {
            try
            {
                var visualProfession = (Profession)DynelManager.LocalPlayer.GetStat(Stat.VisualProfession);

                #region Adventurer

                if (SpellID.AdventurerMezzTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Adventurer)
                        RegisterSpellProcessor(_settings["AdventurerMezzTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "AdventurerMezzTarget"), CombatActionPriority.Medium);
                }

                #endregion

                #region Agent

                if (SpellID.AgentMezzTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (DynelManager.LocalPlayer.Profession == Profession.Agent)
                        RegisterSpellProcessor(_settings["AgentMezzTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "AgentMezzTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.AgentRootTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (DynelManager.LocalPlayer.Profession == Profession.Agent)
                        RegisterSpellProcessor(_settings["AgentRootTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "AgentRootTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.AgentSnareTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (DynelManager.LocalPlayer.Profession == Profession.Agent)
                        RegisterSpellProcessor(_settings["AgentSnareTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "AgentSnareTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.AgentSnareArea.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (DynelManager.LocalPlayer.Profession == Profession.Agent)
                        RegisterSpellProcessor(_settings["AgentSnareArea"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "AgentSnareArea"), CombatActionPriority.Medium);
                }

                #endregion

                #region Bureaucrat

                if (SpellID.BureaucratMezzTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["BureaucratMezzTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "BureaucratMezzTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.BureaucratMezzArea.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["BureaucratMezzArea"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "BureaucratMezzArea"), CombatActionPriority.Medium);
                }

                if (SpellID.BureaucratMezzStunTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["BureaucratMezzStunTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "BureaucratMezzStunTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.BureaucratRootTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["BureaucratRootTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "BureaucratRootTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.BureaucratRootArea.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["BureaucratRootArea"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "BureaucratRootArea"), CombatActionPriority.Medium);
                }

                if (SpellID.BureaucratSnareTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["BureaucratSnareTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "BureaucratSnareTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.BureaucratSnareArea.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["BureaucratSnareArea"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "BureaucratSnareArea"), CombatActionPriority.Medium);
                }

                if (SpellID.LastMinNegotiations.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["LastMinNegotiations"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        Holds(spell, fightingTarget, ref actionTarget, "LastMinNegotiations"), CombatActionPriority.Medium);
                }

                #endregion

                #region Engineer

                if (SpellID.PetSnare.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Engineer)
                        RegisterSpellProcessor(_settings["EngineerPetAOESnareBuff"].AsInt32(), PetAOESnare, CombatActionPriority.Medium, RuleContext.InCombat);
                }

                #endregion

                #region Fixer

                if (SpellID.FixerSnareTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Fixer)
                        RegisterSpellProcessor(_settings["FixerSnareTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "FixerSnareTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.FixerSnareArea.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Fixer)
                        RegisterSpellProcessor(_settings["FixerSnareArea"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "FixerSnareArea"), CombatActionPriority.Medium);
                }

                if (SpellID.FixerRootTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Fixer)
                        RegisterSpellProcessor(_settings["FixerRootTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "FixerRootTarget"), CombatActionPriority.Medium);
                }

                #endregion

                #region Metaphysicist

                if (SpellID.MetaPhysicistMezzTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["MetaPhysicistMezzTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "MetaPhysicistMezzTarget"), CombatActionPriority.Medium);
                }

                #endregion

                #region NanoTechnician

                if (SpellID.NanoTechnicianMezzStunTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.NanoTechnician)
                        RegisterSpellProcessor(_settings["NanoTechnicianMezzStunTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "NanoTechnicianMezzStunTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.NanoTechnicianMezzCalmTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.NanoTechnician)
                        RegisterSpellProcessor(_settings["NanoTechnicianMezzCalmTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "NanoTechnicianMezzCalmTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.NanoTechnicianMezzHackedBlindTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.NanoTechnician)
                        RegisterSpellProcessor(_settings["NanoTechnicianMezzHackedBlindTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "NanoTechnicianMezzHackedBlindTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.NanoTechnicianRootTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.NanoTechnician)
                        RegisterSpellProcessor(_settings["NanoTechnicianRootTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "NanoTechnicianRootTarget"), CombatActionPriority.Medium);
                }

                #endregion

                #region Trader

                if (SpellID.TraderMezzTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderMezzTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "TraderMezzTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.TraderRootTarget.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderRootTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "TraderRootTarget"), CombatActionPriority.Medium);
                }

                if (SpellID.TraderRootArea.Contains(spellID))
                {
                    LoadedHolds.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderRootArea"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                       Holds(spell, fightingTarget, ref actionTarget, "TraderRootArea"), CombatActionPriority.Medium);
                }

                #endregion

            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Spells

        private bool Holds(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!CanCast(spell)) return false;

            actionTarget.Target = null;

            if (_settings["RaidRoot"] != null && _settings["RaidRoot"].AsBool())
            {
                if (spell.Id == SpellID.SuperiorHoldVictim || spell.Id == SpellID.PuissantVoidInertia || spell.Id == SpellID.FlowofTime)//root, aoe root, aoe root.
                {
                    actionTarget.Target = DynelManager.Characters.Where(c => c.IsInLineOfSight && IsMoving(c) && !c.Buffs.Contains(NanoLine.Root)
                    && (c.Name == "Flaming Vengeance" || c.Name == "Hand of the Colonel" || c.Name == "Alien Seeker")).OrderBy(c => c.DistanceFrom(DynelManager.LocalPlayer)).FirstOrDefault();

                    if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                }

                if (spell.Id == SpellID.GreaterDelayTheInevitable || spell.Id == SpellID.ShacklesofObedience || spell.Id == SpellID.IntenseAgglutinativeNanoweb)//snare, snare, snare
                {
                    actionTarget.Target = DynelManager.Characters.Where(c => c.IsInLineOfSight && c.IsMoving && !c.Buffs.Contains(NanoLine.Snare) && c.Name == "Alien Heavy Patroller").OrderBy(c => c.DistanceFrom(DynelManager.LocalPlayer)).FirstOrDefault();

                    if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                }
            }

            if (_settings["AOESnare"] != null && _settings["AOESnare"].AsBool())
            {
                if (spell.Id == SpellID.GreaterDelayPursuers || spell.Id == SpellID.SpinNanoweb)// aoe snare, aoe snare
                {
                    actionTarget.Target = DynelManager.Characters.Where(c => c.IsInLineOfSight && IsMoving(c) && !c.Buffs.Contains(NanoLine.Snare) &&
                    (c.Name == "Flaming Vengeance" || c.Name == "Hand of the Colonel" || c.Name == "Alien Seeker")).OrderBy(c => c.DistanceFrom(DynelManager.LocalPlayer)).FirstOrDefault();

                    if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                }
            }

            if (_settings["Calm12Man"] != null && _settings["Calm12Man"].AsBool())
            {
                if (Playfield.ModelIdentity.Instance == 6015)
                {
                    if (SpellID.LastMinNegotiations.Contains(spell.Id))
                    {
                        actionTarget.Target = DynelManager.NPCs.FirstOrDefault(c => c != null && c.IsAlive && InCastRange(spell, c) && (c.Name == "Right Hand of Madness" || c.Name == "Deranged Xan") && !c.Buffs.Contains(new int[] { 267535, 267536, 267538 }));

                        if (actionTarget.Target != null) { actionTarget = (actionTarget.Target, true); return true; }
                    }
                }
            }

            if (_settings[$"{setting}Option"].AsInt32() == 0) return false;

            var line = NanoLine.NOSTACKING;

            if (Spell.GetSpellsForNanoline(NanoLine.Mezz).Any(s => s.Id == spell.Id))
            {
                if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
                line = NanoLine.Mezz;
            }

            if (SpellID.AOECalms.Contains(spell.Id))
            {
                if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
                if (!_settings["AOE"].AsBool()) return false;
                line = NanoLine.Mezz;
            }

            if (SpellID.LastMinNegotiations.Contains(spell.Id))
            {
                if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
                if (!_settings["AOE"].AsBool()) return false;
            }

            if (Spell.GetSpellsForNanoline(NanoLine.Snare).Any(s => s.Id == spell.Id))
            {
                if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
                line = NanoLine.Snare;
            }

            if (SpellID.AOESnares.Contains(spell.Id))
            {
                if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
                if (!_settings["AOE"].AsBool()) return false;
                line = NanoLine.Snare;
            }

            if (Spell.GetSpellsForNanoline(NanoLine.Root).Any(s => s.Id == spell.Id))
            {
                if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
                line = NanoLine.Root;
            }

            if (SpellID.AOERoots.Contains(spell.Id))
            {
                if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
                if (!_settings["AOE"].AsBool()) return false;
                line = NanoLine.Root;
            }

            if (line != NanoLine.NOSTACKING)
                actionTarget.Target = DynelManager.NPCs.Where(c => c != null && !debuffAreaTargetsToIgnore.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight &&
                !c.Buffs.Contains(line) && InCastRange(spell, c) && c.MaxHealth < 1000000).OrderBy(c => c.DistanceFrom(DynelManager.LocalPlayer)).ThenBy(c => c.Health).FirstOrDefault();
            else
                actionTarget.Target = DynelManager.NPCs.Where(c => c != null && !debuffAreaTargetsToIgnore.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight &&
                    !c.Buffs.Contains(SpellID.LastMinNegotiations) && InCastRange(spell, c) && c.MaxHealth < 1000000).OrderBy(c => c.DistanceFrom(DynelManager.LocalPlayer)).ThenBy(c => c.Health).FirstOrDefault();


            if (actionTarget.Target == null) return false;

            switch (_settings[$"{setting}Option"].AsInt32())
            {
                case 1:// adds
                    var add = actionTarget.Target.IsAttacking && actionTarget.Target.FightingTarget != null && !AttackingMob(actionTarget.Target) && AttackingTeam(actionTarget.Target);
                    if (add) { actionTarget = (actionTarget.Target, true); return true; }
                    break;

                case 2:// all
                    actionTarget = (actionTarget.Target, true); return true;
            }

            return false;
        }

        private bool PetAOESnare(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["AOESnare"] != null && _settings["AOESnare"].AsBool())
            {
                var raidTargets = DynelManager.Characters
                    .Where(c => c.IsInLineOfSight && IsMoving(c) && !c.Buffs.Contains(NanoLine.Snare) &&
                                (c.Name == "Flaming Vengeance" || c.Name == "Hand of the Colonel"))
                    .ToList();

                SimpleChar closestPet = null;
                float closestDist = float.MaxValue;

                foreach (var p in DynelManager.LocalPlayer.Pets)
                {
                    foreach (var target in raidTargets)
                    {
                        float d = p.Character.Position.Distance2DFrom(target.Position);
                        if (d < 15 && d < closestDist)
                        {
                            closestDist = d;
                            closestPet = p.Character;
                        }
                    }
                }

                if (closestPet != null)
                {
                    actionTarget = (closestPet, true);
                    return true;
                }
            }


            if (_settings["EngineerPetAOESnareBuffOption"].AsInt32() == 0) return false;
            if (!_settings["AOE"].AsBool()) return false;
            if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
            if (!CanCast(spell)) return false;
            if (fightingTarget == null) return false;
            if (PVPDistance(16)) return false;

            switch (_settings["EngineerPetAOESnareBuffOption"].AsInt32())
            {
                case 1:
                    {
                        var pet = DynelManager.LocalPlayer.Pets.Where(p => p.Character != null && !p.Character.Buffs.Contains(NanoLine.EngineerPetAOESnareBuff)).FirstOrDefault()?.Character;

                        if (pet == null) return false;

                        actionTarget = (pet, true);
                        return true;
                    }
                case 2:
                    {
                        SimpleChar closestPet = null;
                        float closestDist = float.MaxValue;

                        var targets = DynelManager.NPCs
                            .Where(npc => npc != null && !npc.IsPet && !npc.Buffs.Contains(NanoLine.Snare))
                            .ToList();

                        foreach (var p in DynelManager.LocalPlayer.Pets)
                        {
                            foreach (var t in targets)
                            {
                                float d = p.Character.Position.Distance2DFrom(t.Position);
                                if (d < 15 && d < closestDist)
                                {
                                    closestDist = d;
                                    closestPet = p.Character;
                                }
                            }
                        }

                        if (closestPet == null)
                            return false;

                        actionTarget = (closestPet, true);
                        return true;
                    }
                case 3:
                    {
                        var pet = DynelManager.LocalPlayer.Pets.Where(p => p.Character != null && !p.Character.Buffs.Contains(NanoLine.EngineerPetAOESnareBuff)).FirstOrDefault()?.Character;

                        if (pet == null) return false;

                        actionTarget = (pet, true);
                        return true;
                    }
                default:
                    return false;
            }
        }

        public static bool IsMoving(SimpleChar target)
        {
            if (Playfield.Identity.Instance == 4021) return true;
            if (target == null) return false;
            return target.Velocity > 0;
        }

        #endregion

        #region Perks

        #region Generic

        private bool LegShot(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
            if (!_settings["PerkLegShot"].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (fightingTarget?.Buffs.Where(c => c.Name.ToLower().Contains(perkAction.Name.ToLower()) && c.RemainingTime > 3).Any() == true) return false;
            if (!TargetInPerkRange(perkAction, fightingTarget)) return false;

            return DebuffPerk(perkAction, fightingTarget, ref actionTarget);
        }

        private bool GraspPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
            if (!_settings["PerkGrasp"].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            //if (fightingTarget.IsAttacking && fightingTarget.FightingTarget.Identity == DynelManager.LocalPlayer.Identity) return false;
            if (!fightingTarget.Buffs.Contains(NanoLine.Stun)) return false;
            if (!TargetInPerkRange(perkAction, fightingTarget)) return false;

            MoveBehindFightingtarget();

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool BearhugPerks(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
            if (!_settings["PerkBearhug"].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            //if (fightingTarget.IsAttacking && fightingTarget.FightingTarget.Identity == DynelManager.LocalPlayer.Identity) return false;
            if (!fightingTarget.Buffs.Contains(209861)) return false;
            if (!TargetInPerkRange(perkAction, fightingTarget)) return false;

            MoveBehindFightingtarget();

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool GripOfColossusPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
            if (!_settings["PerkGripOfColossus"].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!TargetInPerkRange(perkAction, fightingTarget)) return false;
            //if (fightingTarget.IsAttacking && fightingTarget.FightingTarget.Identity == DynelManager.LocalPlayer.Identity) return false;
            if (!fightingTarget.Buffs.Contains(209862)) return false;

            MoveBehindFightingtarget();

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool Targeted_Hold(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
            if (!_settings[setting].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!TargetInPerkRange(perkAction, fightingTarget)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        #endregion

        #region Adventurer
        private bool AOE_Root_Perk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["AOE"].AsBool()) return false;
            if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
            if (!_settings["PerkStoneworks"].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (PVPDistance(21)) return false;
            if (fightingTarget == null) return false;
            if (!TargetInPerkRange(perkAction, fightingTarget)) return false;

            actionTarget = (DynelManager.LocalPlayer, true);
            return true;
        }

        #endregion

        #region Agent

        private bool TheShot(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
            if (!_settings["PerkTheShot"].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (fightingTarget?.Buffs.Where(c => c.Name.ToLower().Contains(perkAction.Name.ToLower()) && c.RemainingTime > 3).Any() == true) return false;
            if (!TargetInPerkRange(perkAction, fightingTarget)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        private bool AssassinatePerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["Roots/Snares/Stuns"].AsBool()) return false;
            if (!_settings["PerkAssassinate"].AsBool()) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!fightingTarget.Buffs.Contains(209794)) return false;
            if (!TargetInPerkRange(perkAction, fightingTarget)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        #endregion

        #endregion

        #region Items

        #endregion
    }
}
