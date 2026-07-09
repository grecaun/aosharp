using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using MalisDamageMeter;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MalisDamageMeter
{
    public class PlayerCharData : SimpleCharData
    {
        public PlayerCharData(SimpleChar simpleChar) : base(simpleChar) { }
    }

    public class PetData : SimpleCharData
    {
        public int OwnerId;
        public PetData(SimpleChar simpleChar) : base(simpleChar) { }
    }

    public class SimpleCharData
    {
        public string Name;
        public Profession Profession;
        public RegisterType RegisterType;
        public List<WeaponInfo> WeaponIds = new List<WeaponInfo>();
        public DamageSource DamageSources = new DamageSource();

        public Hits HitReceived = new Hits();
        public HitSource HitSource = new HitSource();
        public HealSource HealSource = new HealSource();
        public AbsorbSource AbsorbSource = new AbsorbSource();

        public DateTime DamageStartTime;
        public DateTime DamageEndTime;

        public SimpleCharData(SimpleChar simpleChar)
        {
            Name = simpleChar.Name;
            Profession = simpleChar.Profession;
            RegisterType = simpleChar.IsPet ? RegisterType.Pet : RegisterType.Player;
            DamageStartTime = DateTime.Now;
            DamageEndTime = DateTime.Now;
        }

        public void TryRegisterWeapon(WeaponInfo weaponInfo)
        {
            if (WeaponIds.Any(x => x.DummyItem.LowId == weaponInfo.DummyItem.LowId && x.Slot == weaponInfo.Slot))
                return;

            WeaponIds.Add(weaponInfo);
            WeaponIds = WeaponIds.OrderBy(x => x.Slot).ToList();
        }
    }

    public class DamageSource : RegisterTypeSource
    {
        public WeaponSource Weapon = new WeaponSource();
        public NanobotsSource Nanobots = new NanobotsSource();
        public DeflectSource DeflectSource = new DeflectSource();

        public void RegisterAttackInfoMessage(Stat stat, int amount, RegisterType type)
        {
            Weapon.RegisterAttackInfoMessage(stat, amount, type);
            RegisterTotal(amount, type);
        }

        public void RegisterSpecialAttackInfoMessage(Stat stat, int amount, RegisterType type)
        {
            Weapon.RegisterSpecialAttackInfoMessage(stat, amount, type);
            RegisterTotal(amount, type);
        }

        public void RegisterHealthDamage(Stat stat, int amount, RegisterType type)
        {
            Nanobots.RegisterNanobots(stat, amount, type);
            RegisterTotal(amount, type);
        }

        public void RegisterReflect(int amount, RegisterType type)
        {
            DeflectSource.RegisterReflect(amount, type);
            RegisterTotal(amount, type);
        }

        public void RegisterShield(int amount, RegisterType type)
        {
            DeflectSource.RegisterShield(amount, type);
            RegisterTotal(amount, type);
        }
    }

    public class WeaponSource : RegisterTypeSource
    {
        public WeaponDamage User = new WeaponDamage();
        public WeaponDamage Pet = new WeaponDamage();

        public void RegisterAttackInfoMessage(Stat stat, int amount, RegisterType type)
        {
            if (type == RegisterType.Player)
                User.RegisterAttackInfoMessage(stat,amount);
            else
                Pet.RegisterAttackInfoMessage(stat,amount);

            RegisterTotal(amount, type);
        }

        public void RegisterSpecialAttackInfoMessage(Stat stat, int amount, RegisterType type)
        {
            if (type == RegisterType.Player)
                User.RegisterSpecialAttackInfoMessage(stat, amount);
            else
                Pet.RegisterSpecialAttackInfoMessage(stat, amount);

            RegisterTotal(amount, type);
        }
    }

    public class NanobotsSource : RegisterTypeSource
    {
        public NanobotsDamage User = new NanobotsDamage();
        public NanobotsDamage Pet = new NanobotsDamage();

        public void RegisterNanobots(Stat stat, int amount, RegisterType type)
        {
            if (type == RegisterType.Player)
                User.Register(stat, amount);
            else
                Pet.Register(stat, amount);

            RegisterTotal(amount, type);
        }
    }

    public class HitSource : RegisterTypeSource
    {
        public Hits User = new Hits();
        public Hits Pet = new Hits();

        public void Register(HitType hitType, RegisterType registerType)
        {
            if (registerType == RegisterType.Player)
                User.Register(hitType);
            else
                Pet.Register(hitType);

            RegisterTotal(1, registerType);
        }
    }

    public class ReflectSource : RegisterTypeSource
    {
        public ReflectDamage User = new ReflectDamage();
        public ReflectDamage Pet = new ReflectDamage();
    }

    public class ShieldSource : RegisterTypeSource
    {
        public ShieldDamage User = new ShieldDamage();
        public ShieldDamage Pet = new ShieldDamage();
    }

    public class AbsorbSource : RegisterTypeSource
    {
        public Absorb User = new Absorb();
        public Absorb Pet = new Absorb();
    }

    public class DeflectSource : RegisterTypeSource
    {
        public ReflectSource Reflect = new ReflectSource();
        public ShieldSource Shield = new ShieldSource();

        public void RegisterReflect(int amount, RegisterType type)
        {
            Reflect.RegisterTotal(amount, type);
            RegisterTotal(amount, type);
        }


        public void RegisterShield(int amount, RegisterType type)
        {
            Shield.RegisterTotal(amount, type);
            RegisterTotal(amount, type);
        }

    }

    public class HealSource : RegisterTypeSource
    {
        public Heal User = new Heal();
        public Heal Pet = new Heal();
    }

    public class WeaponDamage : Damage
    {
        public AutoAttackDamage AutoAttack = new AutoAttackDamage();
        public SpecialsDamage Specials = new SpecialsDamage();

        public void RegisterAttackInfoMessage(Stat stat, int amount)
        {
            AutoAttack.Register(stat, amount);
            RegisterTotal(amount);
        }

        public void RegisterSpecialAttackInfoMessage(Stat stat, int amount)
        {
            Specials.Register(stat, amount);
            RegisterTotal(amount);
        }
    }

    public class AutoAttackDamage : AcDamage
    {
        public AutoAttackDamage() => DamagePerType = Utils.SetAcStats();
    }

    public class SpecialsDamage : AcDamage
    {
        public SpecialsDamage() => DamagePerType = Utils.SetSpecialsStats();
    }

    public class NanobotsDamage : AcDamage
    {
        public NanobotsDamage() => DamagePerType = Utils.SetAcStats();
    }

    public class Absorb : Damage
    {
    }

    public class ReflectDamage : Damage
    {
    }

    public class ShieldDamage : Damage
    {
    }

    public class AcDamage : Damage
    {
        public Dictionary<Stat, int> DamagePerType = new Dictionary<Stat, int>();

        public void Register(Stat type, int amount)
        {
            DamagePerType[type] += amount;
            RegisterTotal(amount);
        }
    }

    public class RegisterTypeSource : Damage
    {
        public int UserTotal;
        public int PetTotal;

        public void RegisterTotal(int amount, RegisterType type)
        {
            if (type == RegisterType.Player)
                UserTotal += amount;
            else
                PetTotal += amount;

            base.RegisterTotal(amount);
        }
    }

    public class Hits : Damage
    {
        public int Normal;
        public int Crit;
        public int Miss;
        public int Glancing;

        public void Register(HitType hitType)
        {
            switch (hitType)
            {
                case HitType.Normal:
                    Normal++;
                    break;
                case HitType.Crit:
                    Crit++;
                    break;
                case HitType.Miss:
                    Miss++;
                    break;
                case HitType.Glancing:
                    Glancing++;
                    break;
            }

            Total++;
        }
    }

    public class Damage
    {
        public int Total;

        protected void RegisterTotal(int amount)
        {
            Total += amount;
        }
    }

    public class Heal : Damage
    {
        public void Register(HealthDamageMessage n3Msg) => Total += n3Msg.Amount;
    }

    public class WeaponInfo
    {
        public WeaponStat DummyItem = new WeaponStat();
        public WeaponSlots Slot;
        public Stat DamageType;
    }

    public class WeaponStat
    {
        public string Name;
        public int Ql;
        public int LowId;
        public int HighId;
    }

    public enum WeaponSlots
    {
        FistOrPet = 0x0,
        MainHand = 0x6,
        Offhand = 0x8,
    }

    public enum HitType
    {
        Miss = 0,
        Glancing = 2,
        Normal = 3,
        Crit = 4,
    }

    public enum RegisterType
    {
        None,
        Player,
        Pet
    }
}