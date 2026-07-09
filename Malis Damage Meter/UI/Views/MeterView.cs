using AOSharp.Common.GameData;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MalisDamageMeter
{
    public class MeterView
    {
        public View Root;
        public BitmapView Icon;
        public TextView LeftTextView;
        public TextView RightTextView;
        public List<PowerBarViewData> PowerBars;
        private Profession _currentProfession;

        public MeterView()
        {
            Root = View.CreateFromXml($"{DamageMeter.PluginDir}\\UI\\Views\\MeterView.xml");

            if (Root.FindChild("Icon", out Icon)) { }

            if (Root.FindChild("Meter", out PowerBarView Meter)) { Meter.Value = 0f; }

            if (Root.FindChild("Meter2", out PowerBarView Meter2)) { Meter2.Value = 0f; }

            if (Root.FindChild("Meter3", out PowerBarView Meter3)) { Meter3.Value = 0f; }

            if (Root.FindChild("Meter4", out PowerBarView Meter4)) { Meter4.Value = 0f; }

            if (Root.FindChild("Meter5", out PowerBarView Meter5)) { Meter5.Value = 0f; }

            if (Root.FindChild("RightText", out RightTextView)) { }

            if (Root.FindChild("LeftText", out LeftTextView)) { }

            PowerBars = new List<PowerBarViewData> {
                new PowerBarViewData { PowerBarView = Meter } ,
                new PowerBarViewData { PowerBarView = Meter2 },
                new PowerBarViewData { PowerBarView = Meter3 },
                new PowerBarViewData { PowerBarView = Meter4 },
                new PowerBarViewData { PowerBarView = Meter5 }
            };
        }

        public void InitMeterData(SimpleCharMeterData simpleCharMeterData, float highestValue)
        {
            SetIcon(simpleCharMeterData.SimpleCharData.Profession);
            SetMeterData(simpleCharMeterData.MeterViewData, highestValue);
        }

        public void SetMeterData(List<MeterViewData> meterViewData, float highestValue)
        {
            int meterValue = 0;

            for (int i = 0; i < meterViewData.Count; i++)
            {
                meterValue += meterViewData[i].Total;

                PowerBars[i].PowerBarView.Value = meterValue / highestValue;

                if (PowerBars[i].Color != meterViewData[i].Color)
                {
                    PowerBars[i].PowerBarView.SetBarColor(meterViewData[i].Color);
                    PowerBars[i].Color = meterViewData[i].Color;
                }
            }
        }

        public void SetIcon(Profession profession)
        {
            if (profession == (Profession)Const.UnkProf || profession == _currentProfession)
                return;

            _currentProfession = profession;
            Icon.SetBitmap($"GFX_GUI_ICON_PROFESSION_{(int)profession}");
        }

        public void ResetMeter()
        {
            if (DamageMeter.Window.ViewSettings.Mode.Current == ModeEnum.Damage)
            {
            }
            else
            {
                foreach (var s in PowerBars.Skip(2))
                    s.PowerBarView.Value = 0;
            }
        }
    }
}

public class PowerBarViewData
{
    public PowerBarView PowerBarView;
    public uint Color;
}

public static class MeterViewColors
{
    public const int HealUser= 0xbb5448;
    public const int HealPet = 0x9c623c;
    public const int DamageAutoAttack = 0x2e788f;
    public const int DamageSpecials = 0x81389b;
    public const int DamageNanobots = 0x4ea220;
    public const int DamagePet = 0x2c32a2;
    public const int DamageDeflect = 0x29985f;
}

public class MeterViewData
{
    public int Total;
    public uint Color;
}