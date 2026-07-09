using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MalisDamageMeter.DamageMeterWindow;

namespace MalisDamageMeter
{
    public class N3MessageCallbacks
    {
        internal void N3MessageCallback(object sender, N3Message n3Msg)
        {
            if (DamageMeter.Window.ViewSettings.IsPaused)
                return;

            ProcessN3Message(n3Msg);
        }

        public void ProcessN3Message(N3Message n3Msg)
        {
            if (n3Msg is AttackInfoMessage attackMsg)
            {
                if (TryProcessFightTarget(attackMsg.Identity, out SimpleCharData fightTargetData))
                    fightTargetData.HitReceived.Register((HitType)attackMsg.HitType);

                if (!TryProcessSimpleChar(attackMsg.Identity, out RegisterType registerType, out SimpleCharData charData))
                    return;

                var weaponInfo = attackMsg.GetWeaponInfo();
                charData.TryRegisterWeapon(weaponInfo);
                charData.DamageSources.RegisterAttackInfoMessage(weaponInfo.DamageType, attackMsg.Amount, registerType);
                charData.HitSource.Register((HitType)attackMsg.HitType, registerType);
                charData.DamageEndTime = DateTime.Now;
            }
            else if (n3Msg is SpecialAttackInfoMessage specialMsg)
            {
                if (!TryProcessSimpleChar(specialMsg.Identity, out RegisterType registerType, out SimpleCharData charData))
                    return;

                charData.DamageSources.RegisterSpecialAttackInfoMessage(specialMsg.Stat, specialMsg.Amount, registerType);
                charData.DamageEndTime = DateTime.Now;
            }
            else if (n3Msg is HealthDamageMessage healthMsg)
            {
                if (!TryProcessSimpleChar(healthMsg.Target, out RegisterType registerType, out SimpleCharData charData))
                    return;

                if (healthMsg.Amount == 0)
                    return;

                if (healthMsg.IsDamage())
                    charData.DamageSources.RegisterHealthDamage(healthMsg.Stat, -healthMsg.Amount, registerType);
                else
                    charData.HealSource.RegisterTotal(healthMsg.Amount, registerType);

                charData.DamageEndTime = DateTime.Now;
            }
            else if (n3Msg is ReflectAttackMessage reflectMsg)
            {
                if (!TryProcessSimpleChar(reflectMsg.Target, out RegisterType registerType, out SimpleCharData charData))
                    return;

                charData.DamageSources.RegisterReflect(reflectMsg.Amount, registerType);
                charData.DamageEndTime = DateTime.Now;
            }
            else if (n3Msg is ShieldAttackMessage shieldMsg)
            {
                if (!TryProcessSimpleChar(shieldMsg.Target, out RegisterType registerType, out SimpleCharData charData))
                    return;

                charData.DamageSources.RegisterShield(shieldMsg.Amount, registerType);
                charData.DamageEndTime = DateTime.Now;
            }
            else if (n3Msg is AbsorbMessage absorbMsg)
            {
                if (!TryProcessSimpleChar(absorbMsg.Identity, out RegisterType registerType, out SimpleCharData charData))
                    return;

                charData.AbsorbSource.RegisterTotal(absorbMsg.Amount, registerType);
                charData.DamageEndTime = DateTime.Now;
            }
            else if (n3Msg is MissedAttackInfoMessage missMsg)
            {
                if (TryProcessFightTarget(missMsg.Attacker, out SimpleCharData fightTargetData))
                    fightTargetData.HitReceived.Register(HitType.Miss);

                if (!TryProcessSimpleChar(missMsg.Attacker, out RegisterType registerType, out SimpleCharData charData))
                    return;

                charData.HitSource.Register(HitType.Miss, registerType);
                charData.DamageEndTime = DateTime.Now;
            }
        }

        public bool TryProcessFightTarget(Identity msgIdentity, out SimpleCharData fightTargetData)
        {
            fightTargetData = null;

            if (!DynelManager.Find(msgIdentity, out SimpleChar simpleChar))
                return false;

            SimpleChar fightTarget = simpleChar.FightingTarget;

            if (fightTarget == null)
                return false;

            if (!DamageMeter.Window.ViewSettings.Scope.Check(fightTarget.Identity.Instance))
                return false;

            ProcessChar(fightTarget, out fightTargetData);

            return true;
        }

        public bool TryProcessSimpleChar(Identity msgIdentity, out RegisterType registerType, out SimpleCharData simpleCharData)
        {
            simpleCharData = null;
            registerType = RegisterType.None;

            if (!DynelManager.Find(msgIdentity, out SimpleChar simpleChar))
                return false;

            if (simpleChar.IsPlayer || !simpleChar.IsPlayer && DamageMeter.Window.ViewSettings.LogMobs)
            {
                if (!DamageMeter.Window.ViewSettings.Scope.Check(simpleChar.Identity.Instance))
                    return false;

                registerType = RegisterType.Player;
                ProcessChar(simpleChar, out simpleCharData);
            }
            else if (simpleChar.IsPet)
            {
                registerType = RegisterType.Pet;
                ProcessPet(simpleChar, out simpleCharData);
            }
            else
            {
                return false;
            }

            return true;
        }

        private void ProcessChar(SimpleChar simpleChar, out SimpleCharData simpleCharData)
        {
            if (!HitRegisters.Characters.ContainsKey(simpleChar.Identity.Instance))
            {
                Utils.InfoPacket(simpleChar.Identity);
                HitRegisters.Characters.Add(simpleChar.Identity.Instance, new PlayerCharData(simpleChar));
            }

            simpleCharData = HitRegisters.Characters[simpleChar.Identity.Instance];

            if (simpleCharData.Profession == (Profession)Const.UnkProf)
            {
                Utils.InfoPacket(simpleChar.Identity);
                simpleCharData.Profession = simpleChar.Profession;
            }
        }

        private void ProcessPet(SimpleChar simpleChar, out SimpleCharData simpleCharData)
        {
            if (!HitRegisters.Pets.ContainsKey(simpleChar.Identity.Instance))
            {
                Utils.InfoPacket(simpleChar.Identity);
                HitRegisters.Pets.Add(simpleChar.Identity.Instance, new PetData(simpleChar));
            }

            var petChar = HitRegisters.Pets[simpleChar.Identity.Instance];

            if (petChar.OwnerId != 0)
            {
                simpleCharData = HitRegisters.Characters[petChar.OwnerId];

                if (simpleCharData.Profession == (Profession)Const.UnkProf || simpleCharData.Profession == Profession.Unknown)
                {
                    var ownerChar = DynelManager.Characters.FirstOrDefault(x => x.Identity.Instance == petChar.OwnerId);
                    simpleCharData.Profession = ownerChar == null ? (Profession)Const.UnkProf : ownerChar.Profession;
                }
            }
            else
            {
                int playerId = simpleChar.GetOwnerId();

                if (playerId != 0 && !HitRegisters.Characters.ContainsKey(playerId))
                {
                    Utils.InfoPacket(simpleChar.Identity);
                    TryProcessSimpleChar(new Identity { Type = IdentityType.SimpleChar, Instance = playerId }, out _, out _);
                }

                if (HitRegisters.Characters.TryGetValue(playerId, out SimpleCharData charData))
                {
                    HitRegisters.TransferPetData(petChar, playerId);
                    simpleCharData = charData;
                }
                else
                {
                    simpleCharData = HitRegisters.Pets[simpleChar.Identity.Instance];
                }
            }       
        }
    }
}

public enum DamageSourceType
{
    Total,
    Weapon,
    Nano
}

