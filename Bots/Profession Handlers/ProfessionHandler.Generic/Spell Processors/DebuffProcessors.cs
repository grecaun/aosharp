using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public SimpleChar _drainTarget;

        public static HashSet<int> LoadedDeBuffs = new HashSet<int>();

        private void RegisterDebuffSpells(int spellID)
        {
            try
            {
                #region Generic

                if (Spell.GetSpellsForNanoline(NanoLine.General1HandBluntDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["General1HandBluntDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "General1HandBluntDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.General1HEdgedDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["General1HEdgedDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "General1HEdgedDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.General2HBluntDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["General2HBluntDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "General2HBluntDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.General2HEdgedDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["General2HEdgedDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "General2HEdgedDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralAgilityDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralAgilityDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralAgilityDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralAssaultRifleDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralAssaultRifleDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralAssaultRifleDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralBioMetDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralBioMetDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralBioMetDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralBowDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralBowDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralBowDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralBowSpecialDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralBowSpecialDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralBowSpecialDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralBrawlDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralBrawlDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralBrawlDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralBurstDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralBurstDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralBurstDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.GeneralDeflectDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralDeflectDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralDeflectDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralDimachDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralDimachDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralDimachDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralEnergyACDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralEnergyACDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralEnergyACDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralEnergyMeleeDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralEnergyMeleeDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralEnergyMeleeDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralFlingShotDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralFlingShotDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralFlingShotDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralFullAutoDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralFullAutoDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralFullAutoDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralGrenadeDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralGrenadeDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralGrenadeDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralIntelligenceDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralIntelligenceDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralIntelligenceDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralKnifeDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralKnifeDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralKnifeDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralLREnergyWeaponDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralLREnergyWeaponDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralLREnergyWeaponDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralMartialArtsDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralMartialArtsDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralMartialArtsDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralMatCreaDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralMatCreaDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralMatCreaDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralMatLocDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralMatLocDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralMatLocDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralMatMetDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralMatMetDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralMatMetDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralMeleeACDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralMeleeACDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralMeleeACDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralNanoACDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralNanoACDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralNanoACDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralPiercingDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralPiercingDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralPiercingDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralPistoDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralPistoDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralPistoDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralPoisonACDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralPoisonACDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralPoisonACDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.GeneralPsyModDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralPsyModDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralPsyModDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }


                if (Spell.GetSpellsForNanoline(NanoLine.GeneralRiposteDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralRiposteDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralRiposteDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralRifleDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralRifleDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralRifleDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralSenseDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralSenseDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralSenseDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralSenseImpDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralSenseImpDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralSenseImpDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralShotgunDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralShotgunDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralShotgunDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralSMGDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralSMGDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralSMGDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralSneakAttackDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralSneakAttackDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralSneakAttackDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralStaminaDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralStaminaDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralStaminaDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralStrengthDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["GeneralStrengthDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralStrengthDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.HealDeltaDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["HealDeltaDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "HealDeltaDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.HealReactivityMultiplierDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["HealReactivityMultiplierDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "HealReactivityMultiplierDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.HopeDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["HopeDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "HopeDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.IntelligenceDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["IntelligenceDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "IntelligenceDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }




                if (Spell.GetSpellsForNanoline(NanoLine.MiseryDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["MiseryDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "MiseryDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }


                if (Spell.GetSpellsForNanoline(NanoLine.NanoResistDebuffProc).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["NanoResistDebuffProc"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "NanoResistDebuffProc"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.NPCostDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["NPCostDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "NPCostDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.PathofDarknessDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["PathofDarknessDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "PathofDarknessDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.RoadToDarknessDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["RoadToDarknessDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "RoadToDarknessDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.ShadeInitDebuffProc).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["ShadeInitDebuffProc"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "ShadeInitDebuffProc"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.SilenceDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["SilenceDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "SilenceDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.SkillLockModifierDebuff1053).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    RegisterSpellProcessor(_settings["SkillLockModifierDebuff1053"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "SkillLockModifierDebuff1053"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                var visualProfession = (Profession)DynelManager.LocalPlayer.GetStat(Stat.VisualProfession);

                #region Adventurer

                if (Spell.GetSpellsForNanoline(NanoLine.ProximityRangeDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Adventurer)
                        RegisterSpellProcessor(_settings["ProximityRangeDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "ProximityRangeDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region Agent

                if (Spell.GetSpellsForNanoline(NanoLine.EvasionDebuffs_Agent).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (DynelManager.LocalPlayer.Profession == Profession.Agent)
                        RegisterSpellProcessor(_settings["EvasionDebuffs_Agent"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "EvasionDebuffs_Agent"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region Bureaucrat

                if (Spell.GetSpellsForNanoline(NanoLine.GeneralRadiationACDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["GeneralRadiationACDebuff"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "GeneralRadiationACDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.GeneralProjectileACDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["GeneralProjectileACDebuff"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "GeneralProjectileACDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.SkillLockModifierDebuff847).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["SkillLockModifierDebuff847"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "SkillLockModifierDebuff847"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.NanoDeltaDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["NanoDeltaDebuff"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "NanoDeltaDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.DemotivationalSpeeches).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["DemotivationalSpeeches"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuffingAura(debuffSpellInstance, fightingTarget, ref actionTarget, "DemotivationalSpeeches"), CombatActionPriority.Low, RuleContext.Both);
                }

                if (SpellID.CratInitDebuffs.Contains(spellID))
                {
                    if (visualProfession == Profession.Bureaucrat && !DebuffWindowController.Debuffs.Any(c => c.SettingKey == "InitiativeDebuffs"))
                        DebuffWindowController.Debuffs.Add(new DebuffUiConfig { DebuffIDs = SpellID.CratInitDebuffs, SettingKey = "InitiativeDebuffs", Label = "Initiative Debuffs", UiType = UiType.DualDropDown, Options = DebuffWindowController.Area });

                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["InitiativeDebuffs"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "InitiativeDebuffs"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.GeneralPsychicDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Bureaucrat)
                        RegisterSpellProcessor(_settings["GeneralPsychicDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralPsychicDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region Doctor

                if (SpellID.DocUnbreakableInitDebuffs.Contains(spellID))
                {
                    if (visualProfession == Profession.Doctor && !DebuffWindowController.Debuffs.Any(c => c.SettingKey == "InitiativeDebuffs"))
                        DebuffWindowController.Debuffs.Add(new DebuffUiConfig { DebuffIDs = SpellID.DocUnbreakableInitDebuffs, SettingKey = "InitiativeDebuffs", Label = "Initiative Debuffs", UiType = UiType.DualDropDown, Options = DebuffWindowController.Area });

                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Doctor)
                        RegisterSpellProcessor(_settings["InitiativeDebuffs"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "InitiativeDebuffs"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (SpellID.DocBreakableInitDebuffs.Contains(spellID))
                {
                    if (visualProfession == Profession.Doctor && !DebuffWindowController.Debuffs.Any(c => c.SettingKey == "DocBreakableInitDebuffs"))
                        DebuffWindowController.Debuffs.Add(new DebuffUiConfig { DebuffIDs = SpellID.DocBreakableInitDebuffs, SettingKey = "DocBreakableInitDebuffs", Label = "Breakable Initiative Debuffs", UiType = UiType.DualDropDown, Options = DebuffWindowController.Area });

                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Doctor)
                        RegisterSpellProcessor(_settings["DocBreakableInitDebuffs"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "DocBreakableInitDebuffs"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region Engineer

                if (Spell.GetSpellsForNanoline(NanoLine.EngineerDebuffAuras).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Engineer)
                        RegisterSpellProcessor(_settings["EngineerDebuffAuras"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuffingAura(debuffSpellInstance, fightingTarget, ref actionTarget, "EngineerDebuffAuras"), CombatActionPriority.Low, RuleContext.Both);
                }

                if (SpellID.IntrusiveAuraCancellation == spellID)
                {
                    if (visualProfession == Profession.Engineer)
                        RegisterSpellProcessor(SpellID.IntrusiveAuraCancellation, AuraCancellation, CombatActionPriority.Low, RuleContext.OutOfCombat);
                }

                #endregion

                #region Fixer

                if (Spell.GetSpellsForNanoline(NanoLine.EvasionDebuffs).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Fixer)
                        RegisterSpellProcessor(_settings["EvasionDebuffs"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "EvasionDebuffs"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.PsychicDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Fixer)
                        RegisterSpellProcessor(_settings["PsychicDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "PsychicDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region MetaPhysicist

                if (Spell.GetSpellsForNanoline(NanoLine.MetaPhysicistDamageDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["MetaPhysicistDamageDebuff"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "MetaPhysicistDamageDebuff"), CombatActionPriority.Medium, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.MPDamageDebuffLineA).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["MPDamageDebuffLineA"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "MPDamageDebuffLineA"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.MPDamageDebuffLineB).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["MPDamageDebuffLineB"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "MPDamageDebuffLineB"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.NanoShutdownDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["NanoShutdownDebuff"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "NanoShutdownDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (SpellID.MetaPhysicistNanoResistanceDebuff_LineA.Contains(spellID))
                {
                    LoadedDeBuffs.Add(spellID);

                    if (!DebuffWindowController.Debuffs.Any(c => c.SettingKey == "MetaPhysicistNanoResistanceDebuff_LineA"))
                        DebuffWindowController.Debuffs.Add(new DebuffUiConfig { DebuffIDs = SpellID.MetaPhysicistNanoResistanceDebuff_LineA, SettingKey = "MetaPhysicistNanoResistanceDebuff_LineA", Label = "MetaPhysicist Nano Resistance Debuff Line A", UiType = UiType.DualDropDown, Options = DebuffWindowController.DebuffTargetOptions });

                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["MetaPhysicistNanoResistanceDebuff_LineA"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "MetaPhysicistNanoResistanceDebuff_LineA"), CombatActionPriority.High, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.BioMetDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["BioMetDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "BioMetDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.GeneralChemicalACDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["GeneralChemicalACDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralChemicalACDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }
                if (Spell.GetSpellsForNanoline(NanoLine.GeneralColdACDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["GeneralColdACDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "GeneralColdACDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.MatMetDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["MatMetDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "MatMetDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.MatCreaDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["MatCreaDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "MatCreaDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.PsyModDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["PsyModDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "PsyModDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.SenseImpDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["SenseImpDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "SenseImpDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.MatLocDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Metaphysicist)
                        RegisterSpellProcessor(_settings["MatLocDebuff"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                    GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "MatLocDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region NanoTechnician

                if (SpellID.AOEBlinds.Contains(spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.NanoTechnician)
                        RegisterSpellProcessor(_settings["AOEBlinds"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "AOEBlinds"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (SpellID.HaloNanoDebuff.Contains(spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.NanoTechnician)
                        RegisterSpellProcessor(_settings["HaloNanoDebuff"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "HaloNanoDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (SpellID.LickofthePest == spellID)
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.NanoTechnician)
                        RegisterSpellProcessor(_settings["LickofthePest"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "LickofthePest"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.AAODebuffs).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.NanoTechnician)
                        RegisterSpellProcessor(_settings["AAODebuffs"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "AAODebuffs"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                #endregion

                #region Soldier

                if (SpellID.SoldierCompleteHealing.Contains(spellID))
                {
                    if (!DebuffWindowController.Debuffs.Any(c => c.SettingKey == "SoldierCompleteHealing"))
                        DebuffWindowController.Debuffs.Add(new DebuffUiConfig { DebuffIDs = SpellID.SoldierCompleteHealing, SettingKey = "SoldierCompleteHealing", Label = "Soldier Complete Healing", UiType = UiType.DropDownInputBoxDropDown });

                    LoadedHeals.Add(spellID);

                    if (visualProfession == Profession.Soldier)
                        RegisterSpellProcessor(_settings["SoldierCompleteHealing"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => TargetHealing(spell, fightingTarget, ref actionTarget, "SoldierCompleteHealing"), CombatActionPriority.Ultra, RuleContext.Both);
                }

                #endregion

                #region Trader

                if (Spell.GetSpellsForNanoline(NanoLine.SLNanopointDrain).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["SLNanopointDrain"].AsInt32(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "SLNanopointDrain"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.NanoDrain_LineA).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["NanoDrain_LineA"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "NanoDrain_LineA"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.NanoDrain_LineB).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["NanoDrain_LineB"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "NanoDrain_LineB"), CombatActionPriority.VeryHigh, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.HealthandNanoOverTimeDrain).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["HealthandNanoOverTimeDrain"].AsInt32(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "HealthandNanoOverTimeDrain"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.TraderAADDrain).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderAADDrain"].AsInt32(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "TraderAADDrain"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.TraderAAODrain).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderAAODrain"].AsInt32(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "TraderAAODrain"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.DamageDrain).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["DamageDrain"].AsInt32(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "DamageDrain"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.TraderShutdownSkillDebuff).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderShutdownSkillDebuff"].AsInt32(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "TraderShutdownSkillDebuff"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.TraderSkillTransferTargetDebuff_Deprive).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.TraderSkillTransferTargetDebuff_Deprive).OrderByStackingOrder(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "TraderSkillTransferTargetDebuff_Deprive"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.TraderSkillTransferTargetDebuff_Ransack).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(Spell.GetSpellsForNanoline(NanoLine.TraderSkillTransferTargetDebuff_Ransack).OrderByStackingOrder(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "TraderSkillTransferTargetDebuff_Ransack"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.TraderDebuffACNanos).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderDebuffACNanos"].AsInt32(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "TraderDebuffACNanos"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.TraderACTransferTargetDebuff_Draw).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);
                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderACTransferTargetDebuff_Draw"].AsInt32(), (Spell drain, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(drain, fightingTarget, ref actionTarget, "TraderACTransferTargetDebuff_Draw"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.TraderACTransferTargetDebuff_Siphon).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);

                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderACTransferTargetDebuff_Siphon"].AsInt32(), (Spell debuffSpellInstance, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) => GenericDebuff(debuffSpellInstance, fightingTarget, ref actionTarget, "TraderACTransferTargetDebuff_Siphon"), CombatActionPriority.Low, RuleContext.InCombat);
                }

                if (SpellID.TraderNanoResistanceDebuff_LineA.Contains(spellID))
                {
                    if (!DebuffWindowController.Debuffs.Any(c => c.SettingKey == "TraderNanoResistanceDebuff_LineA"))
                        DebuffWindowController.Debuffs.Add(new DebuffUiConfig { DebuffIDs = SpellID.TraderNanoResistanceDebuff_LineA, SettingKey = "TraderNanoResistanceDebuff_LineA", Label = "Trader Nano Resistance Debuff Line A", UiType = UiType.DualDropDown, Options = DebuffWindowController.DebuffTargetOptions });

                    LoadedDeBuffs.Add(spellID);

                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["TraderNanoResistanceDebuff_LineA"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "TraderNanoResistanceDebuff_LineA"), CombatActionPriority.High, RuleContext.InCombat);
                }

                if (Spell.GetSpellsForNanoline(NanoLine.DebuffNanoACHeavy).Any(s => s.Id == spellID))
                {
                    LoadedDeBuffs.Add(spellID);

                    if (visualProfession == Profession.Trader)
                        RegisterSpellProcessor(_settings["DebuffNanoACHeavy"].AsInt32(), (Spell debuffSpell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                        GenericDebuff(debuffSpell, fightingTarget, ref actionTarget, "DebuffNanoACHeavy"), CombatActionPriority.High, RuleContext.InCombat);
                }

                #endregion
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Generic

        private bool GenericDebuffingAura(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[$"{setting}Option"].AsInt32() == 0) return false;
            if (!_settings["AOE"].AsBool()) return false;
            if (PVPDistance(11)) return false;

            switch (_settings[$"{setting}Option"].AsInt32())
            {
                case 1:
                    if (fightingTarget == null) return false;
                    if (!DynelManager.LocalPlayer.Buffs.Contains(spell.Nanoline) && DynelManager.LocalPlayer.RemainingNCU > spell.NCU)
                        return true;
                    return false;
                case 2:
                    if (fightingTarget == null) return false;
                    var mob = DynelManager.NPCs.FirstOrDefault(npc => npc != null && !npc.IsPet && DynelManager.LocalPlayer.Position.Distance2DFrom(npc.Position) < 9 && !npc.Buffs.Contains(spell.Nanoline));

                    if (mob != null)
                        return true;

                    return false;
                case 3:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(spell.Nanoline) && DynelManager.LocalPlayer.RemainingNCU > spell.NCU)
                        return true;
                    return false;
                default:
                    return false;
            }
        }

        private bool GenericDebuff(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string SelectionName)
        {
            var settingValue = _settings[$"{SelectionName}Option"].AsInt32();

            if (settingValue == 0) return false;
            if (NeedsReload()) return false;
            //if (!CanCast(spell)) return false;

            NanoLine TransferCasterBuff = NanoLine.NOSTACKING;
            NanoLine TargetDrain = NanoLine.NOSTACKING;

            switch (SelectionName)
            {
                case "HealthandNanoOverTimeTransfer":
                    TransferCasterBuff = NanoLine.HealthandNanoOverTimeTransfer;
                    TargetDrain = spell.Nanoline;
                    break;
                case "TraderAAODrain":
                    TransferCasterBuff = NanoLine.AAOBuffs;
                    TargetDrain = NanoLine.TraderNanoTheft1;
                    break;
                case "TraderAADDrain":
                    TransferCasterBuff = NanoLine.AADBuffs;
                    TargetDrain = NanoLine.TraderNanoTheft2;
                    break;
                case "DamageDrain":
                    TransferCasterBuff = NanoLine.DamageBuffs_LineA;
                    TargetDrain = NanoLine.DamageDrain;
                    break;
                case "TraderACTransferTargetDebuff_Draw":
                    TransferCasterBuff = NanoLine.TraderACTransferCasterBuff_Draw;
                    TargetDrain = spell.Nanoline;
                    break;
                case "TraderACTransferTargetDebuff_Siphon":
                    TransferCasterBuff = NanoLine.TraderACTransferCasterBuff_Siphon;
                    TargetDrain = spell.Nanoline;
                    break;
                case "TraderSkillTransferTargetDebuff_Ransack":
                    TransferCasterBuff = NanoLine.TraderSkillTransferCasterBuff_Ransack;
                    TargetDrain = NanoLine.TraderSkillTransferTargetDebuff_Ransack;
                    break;
                case "TraderSkillTransferTargetDebuff_Deprive":
                    TransferCasterBuff = NanoLine.TraderSkillTransferCasterBuff_Deprive;
                    TargetDrain = NanoLine.TraderSkillTransferTargetDebuff_Deprive;
                    break;
                default:
                    TransferCasterBuff = spell.Nanoline;
                    TargetDrain = spell.Nanoline;
                    break;
            }

            if (TargetDrain == NanoLine.NOSTACKING)
            {
                Chat.WriteLine(spell.Name);
                return false;
            }

            switch (spell.Nanoline)
            {
                case NanoLine.DebuffNanoACHeavy:
                    if (!fightingTarget.Buffs.Contains(NanoLine.NanoResistanceDebuff_LineA)) return false;
                    break;
                case NanoLine.NanoDrain_LineB:
                    string[] namesToCheck = new string[] { "Vergil Aeneid", "Abmouth Supremus", "Aztur the Immortal" };
                    if (namesToCheck.Any(name => fightingTarget?.Name.Contains(name) ?? false)) return false;
                    break;
                case NanoLine.MPDamageDebuffLineB:
                    if (!fightingTarget.Buffs.Contains(NanoLine.MPDamageDebuffLineA)) return false;
                    break;
                case NanoLine.HealthandNanoOverTimeDrain:
                    if (!fightingTarget.IsAttacking || fightingTarget.FightingTarget.Identity == null) return false;
                    break;
            }

            //Chat.WriteLine(spell.Name);

            switch (settingValue)
            {
                case 1://Target
                    return TargetDebuff(spell, TargetDrain, fightingTarget, ref actionTarget);
                case 2:// Boss
                    if (fightingTarget?.MaxHealth < 1000000 && !BossNames.Contains(fightingTarget.Name)) return false;
                    return TargetDebuff(spell, TargetDrain, fightingTarget, ref actionTarget);
                case 3:// Area
                    return AreaDebuff(spell, ref actionTarget);
                case 4:// Keep running
                    if (TransferCasterBuff == 0) return false;

                    if (fightingTarget?.MaxHealth < 1000000 && !BossNames.Contains(fightingTarget.Name))
                    {
                        if (!CanCast(spell)) return false;
                        if (DynelManager.LocalPlayer.Buffs.Find(TransferCasterBuff, out Buff transferBuff) && transferBuff.RemainingTime > 5) return false;

                        actionTarget = (fightingTarget, true);
                        return true;
                    }
                    else
                        return TargetDebuff(spell, TargetDrain, fightingTarget, ref actionTarget);

                default:
                    return false;
            }
        }

        protected bool TargetDebuff(Spell spell, NanoLine nanoline, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (!CanCast(spell)) return false;
                if (NeedsReload()) return false;
                if (fightingTarget == null) return false;

                if (debuffAreaTargetsToIgnore.Contains(fightingTarget.Name)) return false;

                if (spell.Nanoline == NanoLine.GeneralPsychicDebuff) return false;

                if (spell.Nanoline == NanoLine.DamageDrain && fightingTarget.Buffs.Contains(NanoLine.MPDamageDebuffLineA)) return false;

                Buff ExistingBuff = null;

                if (int.TryParse(spell.Nanoline.ToString(), out int result) || nanoline == NanoLine.NOSTACKING)
                    ExistingBuff = fightingTarget.Buffs.FirstOrDefault(b => b.Name == spell.Name);
                else
                    ExistingBuff = fightingTarget.Buffs.FirstOrDefault(b => b.Nanoline == nanoline);

                if (ExistingBuff != null)
                {
                    if (spell.StackingOrder < ExistingBuff.StackingOrder) return false;
                    if (spell.StackingOrder == ExistingBuff.StackingOrder && ExistingBuff.RemainingTime > 7f) return false;
                }

                actionTarget = (fightingTarget, true);
                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        private bool AreaDebuff(Spell spell, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            try
            {
                if (Game.IsZoning) return false;
                if (Spell.HasPendingCast) return false;
                if (!spell.IsReady) return false;
                if (!CanCast(spell)) return false;
                if (NeedsReload()) return false;

                var target = DynelManager.NPCs.Where(c => c != null && c.Health > 0 && !c.IsPlayer && !c.IsPet && !debuffAreaTargetsToIgnore.Contains(c.Name) && c.FightingTarget != null && c.IsInLineOfSight
                && !c.Buffs.Contains(new NanoLine[] { spell.Nanoline, NanoLine.Mezz, NanoLine.Root, NanoLine.Snare }) && InCastRange(spell, c) && SpellCheckFightingTarget(spell, c)).OrderBy(c => c.DistanceFrom(DynelManager.LocalPlayer)).FirstOrDefault();

                if (target == null) return false;

                actionTarget = (target, true);
                return true;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
                return false;
            }
        }

        #endregion

        #region Engineer

        private bool AuraCancellation(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!CanCast(spell)) return false;
            if (_settings["EngineerPetAOESnareBuffOption"].AsInt32() == 3) return false;

                if (DynelManager.LocalPlayer.Pets == null) return false;

            var pet = DynelManager.LocalPlayer.Pets.FirstOrDefault(c => c != null && c.Character != null && c.Character.Buffs.Contains(NanoLine.EngineerPetAOESnareBuff));

            if (pet == null) return false;

            actionTarget = (pet.Character, true);
            return true;
        }
        #endregion

        #region NanoTechnician

        private bool NanoResistanceDebuff_LineA(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (_settings["NanoResistSelection"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;

            if (fightingTarget.Buffs.Any(b => b.Nanoline == NanoLine.DebuffNanoACHeavy && b.StackingOrder >= spell.StackingOrder))
                return false;

            int debuff = 0;

            switch (spell.Id)
            {
                case 259339:// Distracting Shower
                    debuff = 259342;// Distracting Shower III
                    break;
                case 269248:// Incessant Flurry
                    debuff = 259346;// Incessant Flurry II
                    break;
                case 259349:// Overwhelming Storm
                    debuff = 259352;// Overwhelming Storm III
                    break;
                case 259354:// Constant Barrage
                    debuff = 259357;// Constant Barrage III
                    break;
            }

            if (debuff == 0) return false;

            if (fightingTarget.Buffs.Find(debuff, out Buff targetDebuff) && targetDebuff.RemainingTime > 10) return false;

            actionTarget.Target = null;

            switch (_settings["NanoResistSelection"].AsInt32())
            {
                case 1:
                    actionTarget.Target = fightingTarget;
                    break;
                case 3:
                    if (fightingTarget.MaxHealth >= 1000000 || BossNames.Contains(fightingTarget.Name))
                        actionTarget.Target = fightingTarget;
                    break;
            }

            if (actionTarget.Target == null) return false;

            actionTarget = (actionTarget.Target, true);
            return true;
        }

        #endregion

    }
}
