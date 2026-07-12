using System;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using Microsoft.Win32;
using static ProfessionHandler.Generic.GenericProfessionHandler;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public static class SpellID
        {
            #region Generic

            public static readonly int[] NanoSkillsInoperative = { 157743 };

            public static readonly int[] MeleeInitiativeBuffs = { 29240, 29649, 28875, 29641, 29221 };
            public static readonly int[] RangedInitiativeBuffs = { 29240, 29779, 29641, 29778, 29221, 29248, 29188 };
            public static readonly int[] NanoInitiativeBuffs = { 29240, 222856, 28669, 29641, 29221 };

            public static readonly int[] PVPEnabled = { 202732, 214879, 216382, 284620 };

            public static int[] Polymorph223_MajorEvasionBuffs144_RunspeedBuffs150_PerceptionBuffs191 = { 275005, 85062 };
            public static int[] Polymorph223_HPBuff151_ArmorBuff3 = { 217670, 25994 };
            public static int[] Polymorph223_AAOBuffs836 = { 217680, 85070 };
            public static int[] Polymorph223_RunspeedBuffs150_ConcealmentBuff193_CriticalIncreaseBuff182 = { 263278, 82834 };

            public const int FountainOfLife = 302907;

            public const int DanceOfFools = 210159;
            public const int Limber = 210158;

            public const int CompositeAttribute = 223372;
            public const int CompositeNano = 223380;
            public const int CompositeUtility = 287046;
            public const int CompositeTradeskill = 287040;
            public const int CompositeMartialProwess = 302158;
            public const int CompositeMelee = 223360;
            public const int CompositePhysicalSpecial = 215264;
            public const int CompositeRanged = 223348;
            public const int CompositeRangedSpecial = 223364;

            public const int InnerSanctumDebuff = 206387;
            public static int[] Energize = new[] { 226851, 226850, 226849, 226848, 226847, 226846, 226845, 226844, 226843, 226842 };

            public const int InsightIntoSL = 268610;

            public const int BlightedFlesh = 305492;
            public const int WeepingFlesh = 204594;

            public static readonly int[] ShrinkingGrowingflesh = new[] { 302535, 302534, 302544, 302542, 302540, 302538, 302532, 302530 };
            public static readonly int[] AAOTransfer = new[] { 301524, 301520, 267263, 267265 };
            public static readonly int[] KeeperStrStamAgiBuff = new[] { 211158, 211160, 211162, 273365 };

            public static readonly int[] Hoverboards = { 270634, 270632, 270636, 270327, 277712, 288804, 270643, 270641, 270431, 270540, 270542, 274272, 288808, 281684, 288814, 270538, 281668, 288812, 270544, 270546, };

            public static readonly int[] PetWarps = { 209488, 210206, 275167 };

            public const int BioCocoon = 209802;

            public static readonly int[] SiphonBox683 = { 302247, 301888, 302254 };

            public static readonly int[] PetCleanse = { 269869, 269870 };

            public static readonly int[] GadgeteerPerkLine = { (int)PerkHash.ChaoticEnergy, (int)PerkHash.SiphonBox, (int)PerkHash.TauntBox };

            public static readonly int[] PetDefensiveNanos = { 267608, 267607, 267606, 267605, 267604, 267603, 285106, 267601, 267600, 267599 };

            public static readonly int[] PetShortTermDamageBuffs = { 151827, 151829, 230388, 267611, 205193, 205249, 267533, 267598, 205303, 205247, 230386, 151828, 205189, 205245, 205301, 230382, 205243, 151824, 205299, 205187, 205241, 230384, 205297, 205239,
                205185, 151830, 230380, 205295, 230378, 205237, 205293, 205183, 230376, 205235, 205291, 151826, 205191, 205233, 205289, 230374, 205231, 151825, 205195, 205287, 230372, 205197, 205229, 151831, 267278,};

            public static readonly int[] AOECalms = { 100422, 100424, 100426 };
            public static readonly int[] AOESnares = { 95382, 95383, 85314, 85315, 85313, 85316, 85317, 85318, 85216, 85221, 85224, 85223, 85217, 85220, 85222, 85218, 85219 };
            public static readonly int[] AOERoots = { 224129, 224127, 224125, 224123, 224121, 82166, 82164, 82163, 224119, 82161, 82160, 82159, 82158, 82157, 82156, 30719, 82242, 30748 };

            public static readonly int[] SingleTargetTaunts = { 275014, 223123, 223121, 223119, 223117, 223115, 100209, 100210, 100212, 100211, 100213, 301936, 100214, 100216, 100215, 100217, 28866, 223209, 223207,
                223205, 223203, 223201, 29242, 100207, 29218, 100205, 100206, 100208, 29228, 161690 };

            public static readonly int[] TargetMatCreaBuff = { 29300, 151762, 29294, 151765 };

            public static readonly int[] TargetPistolMastery = { 29246 };

            public static readonly int[] UserSneakAttackBuffs = { 210797, 210795, 210793 };
            public static readonly int[] TargetSneakAttackBuffs = { 210791, 210789, 210787 };

            public static readonly int[] TargetPsy_IntBuff = { 220345 };

            #endregion

            #region Adventurer

            public static readonly int[] AdventureSingleTargetHealing = new[] { 223167, 252008, 252006, 136674, 136673, 143908, 82059, 136675, 136676, 82060, 136677, 136678, 136679, 136682, 82061, 136681, 136680, 136683, 136684, 136685, 82062, 136686,
                136689, 82063, 136688, 136687, 26695 };
            public static readonly int[] AdventurerCompleteHealing = new[] { 136672 };
            public static readonly int[] AdventurerTeamHealing = new[] { 270770, 273285, 82051, 82052, 82053, 82054, 82055, 82056, 82057, 82058, 26696 };

            public static readonly int[] PoisonousBite = { 273288 };

            public static readonly int[] DragonMorph = { 217670, 25994 };
            public static readonly int[] LeetMorph = { 263278, 82834 };
            public static readonly int[] WolfMorph = { 275005, 85062 };
            public static readonly int[] SaberMorph = { 217680, 85070 };
            public static readonly int[] TreeMorph = { 229666, 229884, 229887, 229889 };
            public static readonly int[] Morphs = { 217670, 25994, 263278, 82834, 275005, 85062, 217680, 85070, 229666, 229884, 229887, 229889 };
            public static readonly int[] BirdMorph = { 25997, 85066, 82835 };

            public static readonly int[] TargetAimedShotBuffsAdventurer = { 26702 };

            public static readonly int[] TargetArmorBuffAdventurer = { 74173, 74174, 74175, 74176, 74177 };

            public static readonly int[] UserDamageBuffs_LineAAdventurer = { 273288 };

            public static readonly int[] UserDamageShieldsAdventurer = { 161571, 161589, 161595, 161569, 161539, 161553, 161535, 161567, 161517, 161505, 161499, 161565, 161549, 161551, 161533, 161497, 161591, 161519, 161555, 161593, 161501, 161521, 161507, 161537, 161523, 161503 };
            public static readonly int[] TargetDamageShieldsAdventurer = { 55837, 55813, 55815, 55814, 55817, 55818, 55816, 55820, 55819, 55822, 55821, 55823, 55824, 55825, 55827, 55826, 55828, 55829, 55830, 55832, 55831, 55834, 55833, 55835, 55836, 55812 };

            public static readonly int[] TeamExperienceConstructs_XPBonusAdventurer = { 263277 };

            public static readonly int[] TargetFirstAidAndTreatmentBuffAdventurer = { 26703, 26704 };

            public static readonly int[] TeamFortifyAdventurer = { 302229, 302235, 302217, 302243, 302223, 302232, 302220, 302226, 302240, 302214 };

            public static readonly int[] TeamFortifyLeet = { 302229, 302226 };
            public static readonly int[] TeamFortifyWolf = { 302235, 302232 };
            public static readonly int[] TeamFortifyPitLizard = { 302217, 302214 };
            public static readonly int[] TeamFortifySabretooth = { 302243, 302240 };
            public static readonly int[] TeamFortifyTree = { 302223, 302220 };

            public static readonly int[] AdventurerMorphBuffLeet = { 275817, 162121, 162119, 162117 };
            public static readonly int[] AdventurerMorphBuffPitLizard = { 269441 };
            public static readonly int[] AdventurerMorphBuffSabretooth = { 162321, 162319, 162317, 162315, 162313 };
            public static readonly int[] AdventurerMorphBuffWolf = { 275821, 162260, 162258, 162256, 162254 };

            public static readonly int[] DragonAOE = { 161692 };
            public static readonly int[] DragonTargetTaunt = { 161690 };

            public static readonly int[] UserNanoResistanceBuffsAdventurer = { 268027 };

            public static readonly int[] TargetPerceptionBuffsAdventurer = { 26107 };

            public static readonly int[] UserPistolBuffAdventurer = { 161159, 161157, 161155, 161153 };

            public static readonly int[] TargetRunspeedBuffsAdventurer = { 26237, 26705 };

            public static readonly int[] UserMultiwieldBuffAdventurer = { 161167, 161165, 161163, 161161 };

            public static readonly int[] AdventurerMezzTarget = { 163396, 163394, 163388 };

            #endregion

            #region Agent

            public static readonly int[] UserAgilityBuffAgent = { 263243, 263240 };
            public static readonly int[] TargetAgilityBuff = { 25989 };

            public static readonly int[] FalseProfession_Adventurer = { 117214, 117225, 32030 };
            public static readonly int[] FalseProfession_Bureaucrat = { 117209, 117220, 32032 };
            public static readonly int[] FalseProfession_Doctor = { 117210, 117221, 32033 };
            public static readonly int[] FalseProfession_Enforcer = { 117217, 117228, 32041 };
            public static readonly int[] FalseProfession_Engineer = { 117213, 117224, 32034 };
            public static readonly int[] FalseProfession_Fixer = { 117212, 117223, 32039 };
            public static readonly int[] FalseProfession_MartialArtist = { 117215, 117226, 32035 };
            public static readonly int[] FalseProfession_Metaphysicist = { 117208, 117219, 32036 };
            public static readonly int[] FalseProfession_NanoTechnician = { 117207, 117218, 32037 };
            public static readonly int[] FalseProfession_Soldier = { 117216, 117227, 32038 };
            public static readonly int[] FalseProfession_Trader = { 117211, 117222, 32040 };

            public static readonly int[] TeamCritBuffs = { 160791, 160789, 160787 };
            public const int AssassinsAimedShot = 275007;
            public const int SteadyNerves = 160795;
            public const int SuperiorHoldVictim = 270249;
            public const int GreaterDelayPursuers = 85316;
            public const int GreaterDelayTheInevitable = 82545;

            public static readonly int[] UserAimedShotBuffsAgent = { 275007, 160795 };
            public static readonly int[] TargetAimedShotBuffsAgent = { 26702 };

            public static readonly int[] UserConcealmentBuffAgent = { 273296, 32061, 32060, 32059 };
            public static readonly int[] TargetConcealmentBuffAgent = { 32062, 29153 };
            public static readonly int[] TeamConcealmentBuffAgent = { 160897, 160895, 160901, 160899 };

            public static readonly int[] UserCriticalIncreaseBuffAgent = { 273293, 160793 };
            public static readonly int[] TargetCriticalIncreaseBuffAgent = { 160791, 160789, 160787 };

            public static readonly int[] TargetDamageBuffs_LineAAgent = { 222838, 222837, 81856, 81851, 222835, 81852, 222833, 81853, 81854 };

            public static readonly int[] TargetRifleBuffsAgent = { 32067, 32064 };

            public static readonly int[] AgentDetaunt = { 226447, 226445, 226443, 226441, 226439 };

            public static readonly int[] AgentMezzTarget = { 25980 };

            public static readonly int[] AgentSnareArea = { 85316, 85317, 85318 };
            public static readonly int[] AgentSnareTarget = { 82545, 82539, 82542, 82537 };

            public static readonly int[] AgentRootTarget = { 270249, 56207, 56208, 56209, 56213, 56211, 56212, 56210, 56214, 56215, 56217, 56216, 56218 };

            #endregion

            #region Bureaucrat

            public static readonly int[] BureaucratPetHeal = { 116798, 30071, 30072 };

            public static readonly int[] CratAttackPets = { 273300, 235386, 46391, 46381, 46356, 46410, 46373, 46368, 46404, 46393, 46383, 46358, 46412, 46375, 46360, 46395, 46387, 46377, 46352, 46406, 46369, 46364, 
                46400, 46394, 46384, 46350, 46398, 46376, 46361, 46396, 46388, 46378, 46353, 46407, 46370, 46365, 46401, 46389, 46379, 46354, 46408, 46371, 46366, 46402, 46390, 46380, 46355, 46409, 46372, 46367, 46403,
                46392, 46382, 46357, 46411, 46374, 46359, 46405, 46386, 46385, 46351, 46399, 46363, 46362, 46397 };

            public static readonly int[] CratNormalNuke = { 273307, 82000, 78396, 78397, 30091, 78399, 81996, 30083, 81997, 30068, 81998, 78398, 29618 };
            public static readonly int[] CratSpecialNuke = { 273631, 270250, 78400, 30082, 78394, 78395 };

            public const int WorkplaceDepression = 273631;

            public const int CorporateStrategy = 267611;

            
            public const int SkilledGunSlinger = 263251;
            public const int GreaterGunSlinger = 263250;

            public const int DroidDamageMatrix = 267916;

            public const int PuissantVoidInertia = 224129;
            public const int ShacklesofObedience = 82463;

            public static readonly int[] PistolBuffsSelf = { 263250, 263251 };

            public static readonly int[] SingleTargetNukes = { 273307, WorkplaceDepression, 270250, 78400, 30082, 78394, 78395, 82000, 78396, 78397, 30091, 78399, 81996, 30083, 81997, 30068, 81998, 78398, 81999, 29618 };

            public static readonly int[] AadBuffAuras = { 270783, 155807, 155806, 155805, 155809, 155808 };
            public static readonly int[] CritBuffAuras = { 157503, 157499 };
            public static readonly int[] NanoResBuffAuras = { 157504, 157500, 157501, 157502 };
            public static readonly int[] NanoPointsDebuffAuras = { 275826, 157524, 157534, 157533, 157532, 157531 };

            public static readonly int[] CritDebuffAuras = { 157530, 157529, 157528 };
            public static readonly int[] NanoResDebuffAuras = { 157527, 157526, 157525, 157535 };
            public static readonly int[] GeneralRadACDebuff = { 302143, 302142 };
            public static readonly int[] GeneralProjACDebuff = { 302150, 302152 };

            public static readonly int[] _CorporateLeadership = { 205439, 205437, 205435, 205433 };

            public static readonly int[] CratOtherRootReduction = { 203956, 203954, 203952, 203950, 203948 };// target root
            public static readonly int[] CratOtherSnareReduction = { 203964, 203962, 203960, 203958 }; // target snare
            public static readonly int[] CratPetReduction = { 203859, 203857, 203855, 203852, 203850 };//target HumanMonster, snare root
            public static readonly int[] CratSelfRootReduction = { 203846, 203844, 203842, 203839, 203837, 203835, 203831 };// self root
            public static readonly int[] CratSelfSnareReduction = { 203665, 203663, 203661, 203659, 203657 }; // self snare

            public static readonly int[] TargetConcealmentBuffBureaucrat = { 30074 };

            public static readonly int[] TeamExperienceConstructs_XPBonusBureaucrat = { 90448, 90449, 90450 };

            public static readonly int[] UserPistolBureaucrat = { 263251, 263250 };
            public static readonly int[] TargetPistolBureaucrat = { 29246, 30077 };

            public static readonly int[] TakeTheBullet = { 267917 };

            public static readonly int[] CratInitDebuffs = { 275824, 236514, 236508, 236506 };

            public static readonly int[] LastMinNegotiations = { 267535, 267538 };

            public static readonly int[] BureaucratMezzTarget = { 224143, 224141, 224139, 224149, 224147, 224145, 224137, 155577, 224135, 100428, 100429, 100430, 224133, 100431, 224131, 100432, 30093, 219020, 30056, 30065 };
            public static readonly int[] BureaucratMezzArea = { 100422, 100424, 100426 };

            public static readonly int[] BureaucratMezzStunTarget = { 30079, 30060, 29199 };

            public static readonly int[] BureaucratSnareTarget = { 82463, 82466, 82469, 82472, 82474, 82480, 82482, 82528 };
            public static readonly int[] BureaucratSnareArea = { 95382, 95383, 85314, 85315, 85313 };

            public static readonly int[] BureaucratRootArea = { 224129, 224127, 224125, 224123, 224121, 82166, 82164, 82163, 82161, 82160, 82159, 82157, 82156 };
            public static readonly int[] BureaucratRootTarget = { 55993, 55980, 55981, 55982, 55983, 55984, 55985, 55986, 55988, 224117, 55987, 55989, 55990, 224115, 55991, 55992, 43368 };

            #endregion

            #region Doctor

            public static readonly int[] DoctorHPBuffs = new[] { 95709, 28662, 95720, 95712, 95710, 95711, 28649, 95713, 28660, 95715, 95714, 95718, 95716, 95717, 95719, 42397 };

            public static readonly int[] TargetFirstAidAndTreatmentBuffDoctor = { 28675, 28674, 28657 };

            public static readonly int[] TargetInitiativeBuffsDoctor = { 222856, 28669 };

            public static readonly int[] TargetNanoResistanceBuffs = { 222823, 28671 };

            public static readonly int[] TargetStrengthBuffDoctor = { 42400, 28658 };

            public static readonly int[] DoctorHealOverTime = { 269455, 43852, 43868, 43870, 43872, 43873, 43871, 42396, 43869, 43867, 43877, 43876, 43875, 43879, 42399, 43882, 43874, 43880, 42401 };

            public static readonly int[] DoctorCombatNukes = { 275701, 82009, 82008, 82007, 28667 };

            public const int _EpsilonPurge = 28659;

            public static readonly int _teamImprovedLifeChanneler = 275011;

            public static readonly int[] IndividualShortMaxHealths = { 96247, 96259, 96258, 96256, 96257, 96255, 96254, 96253, 96262, 96251, 96250, 96249, 96248 };

            public static readonly int[] DocBreakableInitDebuffs = { 99578 };
            public static readonly int[] DocUnbreakableInitDebuffs = { 99577, 301845, 99583, 99582 };

            public static readonly int[] DoctorSingleTargetHealing =  { 223299, 223297, 223295, 223293, 223291, 223289, 223287, 223285, 223281, 43878, 43881, 43886, 43885, 43887, 43890, 43884, 43808, 43888, 43889, 43883, 43811, 43809, 43810, 28645, 43816, 43817, 43825,
                43815, 43814, 43821, 43820, 28648, 43812, 43824, 43822, 43819, 43818, 43823, 28677, 43813, 43826, 43838, 43835, 28672, 43836, 28676, 43827, 43834, 28681, 43837, 43833, 43828, 28654, 43829, 43832, 28665 };

            public static readonly int[] DoctorCompleteTargetHealing = { 270747, 28650 };

            public static readonly int[] DoctorTeamHealing = { 273312, 273315, 270349, 43891, 43892, 43893, 43894, 43895, 43896, 43897, 43898, 43899, 43900, 43901, 43903, 43902, 42404, 43905, 43904, 42395, 43907, 43908, 43906, 42398, 43910,
                43909, 42402, 43911, 43913, 42405, 43912, 43914, 43915, 27804, 43916, 43917, 42408 };

            public static readonly int _alphaAndOmega = 42409;


            #endregion

            #region Enforcer

            public static readonly int[] UserAbsorbACBuffEnforcer = { 273320 };
            public static readonly int[] TargetAbsorbACBuffEnforcer = { 270350, 117686, 117688, 117682, 117687, 117685, 117684, 117683, 117680, 117681 };

            public static readonly int[] UserDamageShieldsEnforcer = { 269460, };
            public static readonly int[] TargetDamageShieldsEnforcer = { 55751, 55750, 29646, 55748, 29658, 55749, 55747, 55745, 29645, 55746, 55743, 29642, 55744, 55742, 55741 };

            public static readonly int[] UserHPBuffEnforcer = { 273322, 223113, 223111, 223109, 223107, 95699, 95703, 95705, 95698 };
            public static readonly int[] TargetHPBuffEnforcer = { 273629, 95708, 95700, 95701, 95702, 95704, 95706, 95707 };

            public static readonly int[] TargetInitiativeBuffsEnforcer = { 29649, 29641 };

            public static readonly int[] TargetStrengthBuffEnforcer = { 29652 };


            #endregion

            #region Engineer

            public static readonly int[] EngineerPetHeal = { 116791, 116795, 116796, 116792, 116797, 116794, 116793 };
            public static readonly int[] EngineerAOEPetHeal = { 270351 };

            public static readonly int[] TargetArmorBuffEngineer = { 227680, 227678, 227676, 227674, 29789, 29788, 70557, 70529, 70530, 70531, 70532, 70533, 70534, 70535, 70536, 70537, 70538, 70539, 70547, 70549, 70550, 70552,
            70551, 70553, 70554, 70555, 70556, 70540, 70541, 70543, 70544, 70545, 70546, 70548, 29743, 70542 };

            public static readonly int[] UserDamageBuffs_LineAEngineer = { 269463 };
            public static readonly int[] TargetDamageBuffs_LineAEngineer = { 29760, 81922, 29758, 29764 };

            public static readonly int[] TargetDamageShieldsEngineer = { 29774, 29763, 55767, 55768, 55769, 55771, 55770, 29773, 55772, 29781, 55773, 55774, 29782 };

            public static readonly int[] TargetInitiativeBuffsEngineer = { 29779, 29778, 29188 };

            public static readonly int[] TargetReflectShieldEngineer = { 70295, 70299, 70296, 70297, 70298, 70294 };

            public static readonly int[] SlayerdroidTransference = { 31593 };


            public static readonly int[] EngieAttackPets = { 223323, 223321, 223319, 223317, 223315, 223313, 45671, 45673, 45674, 45672, 45685, 45679, 45729, 45690, 45670, 45678, 45735, 45667, 45669, 45728, 45722, 45708,
                45688, 45696, 45716, 45689, 45684, 45677, 45734, 45666, 45668, 45727, 45721, 45707, 45687, 45695, 45715, 45683, 45700, 45733, 45665, 45702, 45726, 45720, 45706, 45686, 45694, 45714, 45682, 45699, 45732,
                45664, 45701, 45725, 45719, 45705, 45711, 45693, 45713, 45680, 45697, 45730, 45675, 45736, 45723, 45717, 45703, 45709, 45691, 45712, 45681, 45698, 45731, 45676, 45737, 45718, 45724, 45704, 45710, 45692, 43325 };

            public static readonly int[] EngieSupportPets = { 275815, 223337, 223335, 223333, 223331, 223329, 223327, 301855, 223325 };

            public const int SympatheticReactiveCocoon = 154550;
            public const int IntrusiveAuraCancellation = 204372;

            public static readonly int[] PerkTauntBox = { 229131, 229130, 229129, 229128, 229127, 229126 };
            public static readonly int[] PerkSiphonBox = { 229657, 229656, 229655, 229654 };
            public static readonly int[] PerkChaoticBox = { 227787 };

            public static readonly int[] AllOff_Blinds = { 154715, 154716, 154717, 154718, 154719 };
            public static readonly int[] ReflectDebuff = { 154725, 154726, 154727, 154728 };
            public static readonly int[] ShieldDebuff = { 154720, 154721, 154722, 154723, 154724 };

            public static readonly int[] PetSnare = { 275835, 204370, 204368, 204366, 204364, 204362 };

            public static readonly int[] ShieldOfObedientServant = { 270790, 202260 };
            public static readonly int[] EngineeringBuff = { 273346, 227667, 227657 };

            public static readonly int[] UserEngineerSpecialAttackAbsorber = { 223311, 223309, 204355, 204353, 223307, 204351, 204349, 204347, 204345 };
            public static readonly int[] TeamEngineerSpecialAttackAbsorber = { 204422, 204420, 204418 };

            #endregion

            #region Fixer

            public static readonly int[] TargetConcealmentBuffFixer = { 32062, 29153 };

            public static readonly int[] TargetDamageBuffs_LineAFixer = { 222838, 81879, 81880, 81881, 222837, 81882, 81883, 81884, 222835, 31448, 81885, 31411, 81886, 222833, 31384, 81887, 31401 };

            public static readonly int[] TargetMajorEvasionBuffs_RunspeedBuffsFixer = { 93132, 93126, 93127, 93128, 93129, 93130, 93131, 93125 };

            public static readonly int[] ShadowlandsRunspeed = { 223125, 223131, 223129, 215718, 223127 };

            public static readonly int[] TargetPerceptionBuffsFixer = { 31380 };

            public static readonly int[] TeamRunspeedBuffsFixer = { 162595, 162589, 162603, 162593, 162899, 162591, 162597, 162601 };

            public static readonly int[] TargetSneakAttackBuffsFixer = { 31409 };

            public const int _WakeUpCall = 279374;
            public const int _RefactorNCUMatrix = 275680;

            public const int SpinNanoweb = 85216;
            public const int IntenseAgglutinativeNanoweb = 223143;

            public static readonly int[] Grid = { 155189, 155187, 155188, 155186 };
            public static readonly int[] ShadowwebSpinner = { 273349, 224422, 224420, 224418, 224416, 224414, 224412, 224410, 224408, 224405, 224403 };

            public static readonly int[] FixerNCUBuffs = { 275043, 163095, 163094, 163087, 163085, 163083, 163081, 163079, 162995 };

            public const int GreaterPreservationMatrix = 275679;
            public static readonly int[] UserFixerLongHoT = { 275679 };

            public static readonly int[] TargetFixerLongHoT = { 252050, 252048, 252046, 162734, 162732, 162730, 162728, 162726, 162724, 162722, 162720, 162718, 162716, 162714 };

            public static readonly int[] FixerShortHealOverTime = { 273352, 270352, 85226, 85225, 85227, 85229, 85228, 85230, 85231, 85232, 85233, 85234, 85215 };

            public static readonly int[] FixerSnareTarget = { 223143, 223141, 223139, 223137, 223135, 82502, 82504, 223133, 82507, 82510, 217039, 82513, 82515, 82518 };
            public static readonly int[] FixerSnareArea = { 85216, 85221, 85224, 85223, 85217, 85220, 85222, 85218, 85219 };

            public static readonly int[] FixerRootTarget = { 56219, 56220, 56221, 56223, 56222, 56224, 56225, 56226 };

            public static readonly int[] FixerSelfRootRemoval = { 203813, 203811 };
            public static readonly int[] FixerSelfSnareRemoval = { 203603, 203601, 203599, 203595, 203597, 203593 };
            public static readonly int[] FixerTargetSnareReduction = { 203984, 203982, 203980, 203978, 203976 };


            #endregion

            #region Keeper

            public static readonly int[] TargetAAOBuffsKeeper = { 301602 };

            public static readonly int[] TeamFortifyKeeper = { 275036, 223022, 210518, 210516, 210514, 210512 };

            public const int CourageOfTheJust = 279380;

            #endregion

            #region MartialArtist

            public static readonly int[] MartialArtistSingleTargetHealing = { 275698, 252054, 252052, 270353, 82030, 82031, 82032, 82036, 82035, 82033, 82034, 28901 };
            public static readonly int[] MartialArtistTeamHealing = { 273367, 301493, 82046, 82047, 82048, 82049, 82050 };

            public static readonly int[] UserArmorBuffMartialArtist = { 275700, 28879 };
            public static readonly int[] TargetArmorBuffMartialArtist = { 75351, 75336, 75338, 28869, 75337, 75339, 75340, 75342, 75341, 75343, 28907, 75344, 75345, 75347, 75346, 28905, 75348, 75350 };

            public static readonly int[] UserCriticalIncreaseBuffMartialArtist = { 95527, 95528, 95529, 95526 };
            public static readonly int[] TargetCriticalIncreaseBuffMartialArtist = { 160574, 160575, 160576 };

            public static readonly int[] UserDamageBuffs_LineAMartialArtist = { 270798, 81827, 81825, 81822, 81824, 81823, 28876, 81826, 81830, 81829, 28892 };

            public static readonly int[] UserDamageBuff_LineCMartialArtist = { 269470 };

            public static readonly int[] UserInitiativeBuffsMartialArtist = { 273372 };
            public static readonly int[] TargetInitiativeBuffsMartialArtist = { 28875 };

            public static readonly int[] UserMajorEvasionBuffsMartialArtist = { 218070, 218068, 218066, 218064, 218062, 218060 };
            public static readonly int[] TargetMajorEvasionBuffsMartialArtist = { 28903, 28878, 28872 };

            public static readonly int[] MartialArtistBowBuffsMartialArtist = { 273370, 263246, 263249 };

            public static readonly int[] UserMartialArtsBuffMartialArtist = { 28880 };
            public static readonly int[] TargetMartialArtsBuffMartialArtist = { 28895 };

            public static readonly int[] TargetRunspeedBuffsMartialArtist = { 28862 };

            public static readonly int[] TargetStrengthBuffMartialArtist = { 28898, 28899 };

            public const int LimboMastery = 28894;

            public static readonly int[] Horde = { 28890 };
            public static readonly int[] Cohort = { 28868 };

            #endregion

            #region Metaphysicist

            public static readonly int[] UserPsy_IntBuffMetaphysicist = { 273379, 29309 };

            public static readonly int[] UserMajorEvasionBuffsMetaphysicist = { 302188 };
            public static readonly int[] TargetMajorEvasionBuffsMetaphysicist = { 29272 };

            public static readonly int[] MartialArtistBowBuffsMetaphysicist = { 302257 };

            public static readonly int[] TargetNPCostBuffMetaphysicist = { 95409, 29307, 95411, 95408, 95410 };

            public static readonly int[] MPNormalNuke = { 267878, 125763, 125760, 125765, 125764 };
            public static readonly int[] MPMindDamage = { 270355, 125761, 29297, 125762, 29298, 29114 };

            //public static readonly int[] HealPets = { 225902, 125746, 125739, 125740, 125741, 125742, 125743, 125744, 125745, 125738 };
            //public static readonly int[] AttackPets = { 254859, 225900, 225898, 225896, 225894, 43737, 43731, 43732, 43735, 43734, 43733, 43324 };

            public static readonly int[] PetShortTermDamage = { 267598, 205193, 151827, 205189, 205187, 151828, 205185, 151824, 205183, 151830, 205191, 151826, 205195, 151825, 205197, 151831 };

            public static readonly int[] InstillDamageBuffs = { 270800, 285101, 116814, 116817, 116812, 116816, 116821, 116815, 116813 };
            public static readonly int[] ChantBuffs = { 116819, 116818, 116811, 116820 };

            public static readonly int[] SenImpBuffs = { 29304, 151757, 29315, 151764 }; //Composites count as SenseImp buffs. Have to be excluded

            public static readonly int[] MPCompositeNano = { 220343, 220341, 220339, 220337, 220335, 220333, 220331 };

            public static readonly int[] SacrificialBond = { 267281, 300506 };
            public static readonly int[] SacrificialBondTransfer = { 300505 };

            public static readonly int[] SacrificialPower = { 267391, 267531 };
            public static readonly int[] SacrificalShield = { 267278 };

            public static readonly int[] TwoHanded =
            {
                154981, //Azure Cobra of Orma
                154982, //Wixel's Notum Python
                154983, //Asp of Semol
                154984, //Viper Staff
            };
            public static readonly int[] OneHanded =
            {
                275849, //Asp of Titaniush
                154977, //Gold Acantophis
                154978, //Bitis Striker
                154979, //Coplan's Hand Taipan
                154980, //The Crotalus
            };
            public static readonly int[] Shield =
            {
                273376, //Shield of Zset
                275852, //Shield of Esa
                154971, //Shield of Asmodian
                154974, //Mocham's Guard"
                154972, //Death Ward
                154968, //Belthior's Flame Ward
                154975, //Wave Breaker
                154973, //Solar Guard"
                154976, //Notum Defender
                154970, //Vital Buckler
                154969, //Living Shield of Evernan
            };

            public static readonly int[] MetaPhysicistNanoResistanceDebuff_LineA = { 266299 };
            public static readonly int[] MetaPhysicistMezzTarget = { 29285 };

            #endregion

            #region NanoTechnician

            public static readonly int[] UserAbsorbACBuffNanoTechnician = { 273386 };
            public static readonly int[] TargetAbsorbACBuffNanoTechnician = { 270356, 117676, 117675, 117677, 117678, 117679 };

            public static readonly int[] UserFortifyNanoTechnician = { 150631, 150621, 150622, 150623, 150624, 150625, 150626, 150627, 150628, 150629, 150630, 150620 };

            public static readonly int[] UserMajorEvasionBuffsNanoTechnician = { 270802 };
            public static readonly int[] TargetMajorEvasionBuffsNanoTechnician = { 28603 };

            public static readonly int[] UserPsy_IntBuffNanoTechnician = { 220349, 220347 };

            public static readonly int[] UserMatCreaBuff = { 302274 };

            public static readonly int[] TargetNPCostBuffNanoTechnician = { 95417, 95413, 95412, 95414, 95415, 95416, 95407 };

            public static readonly int[] UserReflectShieldNanoTechnician = { 273388, 263265 };

            public static readonly int[] NTRKAOENukes = { 28620, 28638, 28637, 28594, 45922, 45906, 45884, 28635, 28593, 45925, 45940, 45900, 28629, 45917, 45937, 28599, 45894, 45943, 28633, 28631 };
            public static readonly int[] NTSLAOENukes = { 266293, 266294, 266295, 266296, 266297, 266298 };

            public static readonly int[] NTNukes = { 218168, 275692, 218164, 218162, 218160, 218158, 218156, 218154, 218152, 218150, 218148, 218146, 218144, 218142, 218140,
                218138, 218136, 269473, 218134, 218132, 218124, 218130, 218122, 218120, 218128, 218118, 218126, 218116, 218114, 218112, 218104, 218102, 218110, 218108,
                218100, 218098, 218106, 218096, 218094, 218092,
                201935, 202262, 201933, 28618, 45226, 45192, 45230, 28619, 28623, 28604, 28616, 28597, 45210, 45236, 45197,
                45233, 45247, 45199, 45235, 45234, 45258, 45217, 28600, 45198, 28613, 45919, 45195, 45225, 45260, 45891, 45254, 45890, 45213, 45215, 45915, 45252,
                45214, 45929, 45251, 45220, 45920, 45222, 45911, 28598, 45237, 45216, 45913, 45901, 45212, 45912, 45206, 45883, 45245, 45904, 45140, 45218, 28626,
                45261, 45909, 45203, 45903, 45228, 45200, 45939, 28592, 45242, 45885, 45926, 45241, 45908, 44538, 45934, 45250, 45138, 45932, 28632, 45205, 28609,
                45209, 45246, 45935, 45921, 45227, 45207, 45942, 45924, 45191, 28610, 45914, 45893, 45208, 28621, 45933, 45916, 45211, 45240, 45941, 45259, 45910,
                45253, 28614, 45221, 28634, 45204, 45886, 45196, 45928, 45201, 45193, 45323, 45889, 45895, 45244, 28605, 45219, 45938, 45223, 28628, 45232, 45248,
                45898, 45923, 45202, 45229, 45907, 45139, 45887, 45231, 45882, 28627, 45936, 45194, 28639, 45931, 45243, 28630, 45137, 28607, 45257, 45880, 45256,
                45249, 45888, 45881, 45255, 45927, 42543, 45902, 42540, 42541, 45899, 45905, 28611, 45897, 28601, 28608, 45918, 45892, 45930, 45896, 28612 };


            public const int NanobotAegis = 302074;
            public const int IzgimmersWealth = 275024;
            public const int IzgimmersUltimatum = 218168;

            public static readonly int[] HaloNanoDebuff = { 45239, 45238, 45224 };
            public const int Stun = 28625;
            public static readonly int[] Calm = { 259365, 259364, 259362, 259363, 259366, 259367, 100443, 100441, 259335, 259336, 100442, 100440 };
            public static int LickofthePest = 201937;

            public const int SuperiorFleetingImmunity = 273386;

            public static readonly int[] AOEBlinds = { 83959, 83960, 83961, 83962, 83963, 83964 };

            public static readonly int[] NanoTechnicianAAODebuffs_Target = { 275697, 83947, 83948, 83949, 83950, 83951, 83952, 83953, 83954, 83955, 83956, 83957, 83958, 83942, 83943, 83944 };
            public static readonly int[] NanoTechnicianAAODebuffs_Area = { 83959, 83960, 83961, 83962, 83963, 83964 };

            public const int DefensiveFocus = 266314;
            public const int OffensiveFocus = 266315;

            public static readonly int[] NanoTechnicianNanoPointHeals = { 275024 };

            public static readonly int[] NanoTechnicianMezzStunTarget = { 28625 };
            public static readonly int[] NanoTechnicianMezzCalmTarget = { 259365, 259364, 259363, 259362, 259366, 259367, 100443, 100441, 259336, 259335, 100442, 100440 };

            public static readonly int[] NanoTechnicianMezzHackedBlindTarget = { 253384, 253382, 253380 };

            public static readonly int[] NanoTechnicianRootTarget = { 259376, 259377, 259374, 259375, 259372, 259373, 56028, 56022, 56023, 56024, 259338, 259337, 56025, 56026, 56027, 42110 };

            #endregion

            #region Shade

            public static readonly int[] UserAgilityBuffShade = { 273393, 210752, 210750, 210748, 210746 };

            public static readonly int[] UserConcealmentBuffShade = { 273395 };

            public static readonly int[] UserMartialArtsBuffShade = { 210744, 210742, 210740, 210738, 210736, 210734, 210732 };

            public static readonly int[] UserMultiwieldBuffShade = { 275843, 210812, 210810, 210808, 210806, 210804 };

            public static readonly int[] TargetRunspeedBuffsShade = { 272371 };

            public const int ShadesCaress = 266300;

            public const int SpiritSiphon = 297342;

            public static readonly int[] ShadeHealthDrain = { 273390, 301895 };

            #endregion

            #region Soldier

            public static readonly int[] TargetAAOBuffsSoldier = { 270248, 29222 };

            public static readonly int[] TargetArmorBuffSoldier = { 75401, 75402, 75403, 75404, 75405, 75406, 75407, 75408, 29223, 75409, 75410, 75411, 75412, 75413 };

            public static readonly int[] UserAssaultRifleBuffs = { 275027, 203119, 203121 };
            public static readonly int[] TargetAssaultRifleBuffs = { 29220 };

            public static readonly int[] CompositeHeavyArtillery = { 269482 };

            public static readonly int[] UserBurstBuff = { 203129, 203131, 203133 };
            public static readonly int[] TargetBurstBuff = { 29251 };

            public static readonly int[] TargetDamageBuffs_LineASoldier = { 222838, 222837, 29232, 222835, 29243, 222833, 29225, 29244 };

            public static readonly int[] UserHPBuffSoldier = { 273398 };
            public static readonly int[] TargetHPBuffSoldier = { 95697, 95693, 29254, 95694, 95695, 95696, 29091 };

            public static readonly int[] TargetInitiativeBuffsSoldier = { 29240, 29221, 29248 };

            public static readonly int[] UserMajorEvasionBuffsSoldier = { 275844 };
            public static readonly int[] TargetMajorEvasionBuffsSoldier = { 29247 };

            public static readonly int[] UserRangedEnergyWeaponBuffs = { 275905, 203135 };
            public static readonly int[] TargetRangedEnergyWeaponBuffs = { 29230 };

            public static readonly int[] UserReflectShieldSoldier = { 273400, 223185, 223183, 223181, 223229, 70300, 70305, 70302, 70301, 70304, 70303, 70307, 70306, 70309, 70308 };
            public static readonly int[] TargetReflectShieldSoldier = { 70311, 70314, 70325, 70324, 70333, 70332, 70310, 70312, 70329, 70328, 70319, 70322, 70315, 70318, 70313, 70316, 70327, 70326, 70335, 70334,
            70336, 70337, 70331, 70330, 70321, 70323, 70317, 70320 };

            public static readonly int[] TargetRifleBuffsSoldier = { 29250 };

            public static readonly int[] SOLDTimedTauntBuffs = { 229104, 229102, 229100, 229098, 229096, 229094, 229092, 229090 };

            public static readonly int[] DeTaunt = { 223221, 223219, 223217, 223215, 223213, 223211 };

            public static readonly int[] SoldierDrainHeal = { 301897, 29241 };
            //public static readonly int[] SoldierHeal = { 301897, 29241 };
            public static readonly int[] SoldierCompleteHealing = new int[] { 204305, 204303 };
            public const int Phalanx = 29245;
            public const int Precognition = 29247;

            public static readonly int[] SiphonBox683Soldier = { 302399, 302395 };

            #endregion

            #region Trader

            public static readonly int[] UserDamageBuff_LineC = { 275846 };
            public static readonly int[] TargetDamageBuff_LineC = { 30746, 30718 };
            public static readonly int[] TeamDamageBuff_LineC = { 30721, 30747, 81928 };

            public static readonly int[] UserMajorEvasionBuffsTrader = { 270808 };
            public static readonly int[] TargetMajorEvasionBuffsTrader = { 30745 };

            public static readonly int[] UmbralWrangler = { 235291, 235289, 235287, 235283, 235281, 235279, 235277, 235275, 235273, 235271 };

            //public static readonly int[] HealthDrain = { 273390, 301895, 293941, 270357, 77195, 76478, 76475, 76487, 76481, 76484, 76491, 76494, 76499, 76571, 76503, 76651, 76614, 76656, 76653, 76679, 76681, 76684, 76686, 76691, 76688, 76717, 76715, 76720, 76722, 76724, 76727, 76729, 76732, 76742 };

            //public static readonly int[] _hagglerHeals =      { 273410, 252155, 121496, 121500, 121501, 121499, 121502, 121495, 121492, 121506, 121494, 121493, 121504, 121498, 121503, 76653, 76679, 76681, 76684, 76686, 76691, 76688, 76717, 76715, 121497, 121505 };
            public static readonly int[] TraderHealthTarget = { 273410, 252155, 121496, 121500, 121499, 121502, 121495, 121492, 121506, 121494, 121493, 121504, 121498, 121503, 121497, 121505 };

            //public static readonly int[] TraderTeamHeals =  { 118245, 118230, 118232, 118231, 118235, 118233, 118234, 118238, 118236, 118237, 118241, 118239, 118240, 118243, 43374 };
            public static readonly int[] TraderHealthTeam = { 118245, 118230, 118232, 118231, 118235, 118233, 118234, 118238, 118236, 118237, 118241, 118239, 118240, 118243, 43374 };

            public static readonly int[] TraderDrainHeal = { 302401 };
            public static readonly int[] TraderHealthDrain = { 270357, 77195, 76478, 76475, 76487, 76481, 76484, 76491, 76494, 76499, 76571, 76503, 76651, 76614, 76656, 76653, 76679, 76681, 76684, 76686, 76691, 76688, 76717, 76715, 76720, 76722, 76724, 76727, 76742 };

            public const int UnstoppableKiller = 275846;

            public const int DivestDamageTransfer = 273408;

            public const int FlowofTime = 30719;
            public const int DecisionbyCommittee = 258659;

            public static readonly int[] TraderNanoPointHeals = { 223257, 223255, 223253, 223251, 223249, 223247, 223241 };

            public static readonly int[] TraderNanoResistanceDebuff_LineA = { 226746, 226742, 226738, 226734, 226728, 226726 };

            public static readonly int[] TraderMezzTarget = { 252057, 301899, 100439, 100434, 100435, 100436, 100437, 100438, 100433 };

            public static readonly int[] TraderRootTarget = { 56235, 56228, 56229, 56230, 56231, 56232, 56233, 56227, 56234 };
            public static readonly int[] TraderRootArea = { 30719, 82242, 30748 };

            public static readonly int[] TraderSelfRootDurationReduction = { 203792, 203790, 203788, 203786, 203784 };
            public static readonly int[] TraderTargetRootDurationReduction = { 203970, 203968, 203966 };

            #endregion

            public static readonly int[] All_Attack_Pets = (CratAttackPets ?? Array.Empty<int>()).Concat(EngieAttackPets ?? Array.Empty<int>()).Concat((Spell.GetSpellsForNanoline(NanoLine.AttackPets) ?? Array.Empty<Spell>()).Select(s => s.Id).ToArray()).ToArray();
            public static readonly int[] All_Support_Pets = (EngieSupportPets ?? Array.Empty<int>()).Concat((Spell.GetSpellsForNanoline(NanoLine.SupportPets) ?? Array.Empty<Spell>()).Select(s => s.Id).ToArray()).ToArray();
        }
    }
}
