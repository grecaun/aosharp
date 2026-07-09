using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Common.Unmanaged.DataTypes;
using AOSharp.Common.Unmanaged.Imports;
using AOSharp.Common.Unmanaged.Interfaces;
using AOSharp.Core;
using AOSharp.Core.UI;
using MalisDamageMeter;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MalisDamageMeter
{
    public class Const
    {
        public static uint UnkProf = 4294967295;
    }

    public class Utils
    {

        public static void LoadCustomTextures(string path, int startId)
        {
            DirectoryInfo textureDir = new DirectoryInfo(path);

            foreach (var file in textureDir.GetFiles("*.png").OrderBy(x => x.Name))
            {
                GuiResourceManager.CreateGUITexture(file.Name.Replace(".png", "").Remove(0, 4), startId++, file.FullName);
            }
        }

        public static void InfoPacket(Identity identity)
        {
            Network.Send(new CharacterActionMessage()
            {
                Action = CharacterActionType.InfoRequest,
                Identity = DynelManager.LocalPlayer.Identity,
                Target = identity,
            });
        }

        public static string FindScriptFolder()
        {
            string path = Preferences.GetCharacterPath();

            for (int i = 0; i < 3; i++)
                path = Path.GetDirectoryName(path);

            return Path.Combine(path, $"Scripts\\");
        }

        public static Dictionary<Stat, int> SetAcStats() => new Dictionary<Stat, int>
        {
            { Stat.ChemicalAC , 0 },
            { Stat.ColdAC , 0 },
            { Stat.EnergyAC , 0 },
            { Stat.FireAC , 0 },
            { Stat.MeleeAC , 0 },
            { Stat.PoisonAC , 0 },
            { Stat.ProjectileAC , 0 },
            { Stat.RadiationAC, 0 },
        };

        public static Dictionary<Stat, int> SetSpecialsStats() => new Dictionary<Stat, int>
        {
            { Stat.FlingShot , 0 },
            { Stat.Burst , 0 },
            { Stat.FullAuto , 0 },
            { Stat.AimedShot , 0 },
            { Stat.Brawl , 0 },
            { Stat.Dimach , 0 },
            { Stat.Backstab , 0 },
            { Stat.FastAttack , 0 },
            { Stat.SneakAttack , 0 },
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        private static bool WriteInt(IntPtr memoryAddress, int value, out IntPtr intPtrBytesWrote)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            bool result = WriteProcessMemory(Kernel32.GetCurrentProcess(), memoryAddress, buffer, sizeof(int), out intPtrBytesWrote);
            return result;
        }

        public static void SetScriptMaxFileSize(int maxSize)
        {
            IntPtr baseAddress = Kernel32.GetModuleHandle("GUI.dll") + 0xb197d;
            WriteInt(baseAddress, maxSize, out _);
        }
    }
}