using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using static MalisDamageMeter.MainWindow;

public class Mode
{
    public ModeEnum Current;

    private Dictionary<ModeEnum, string> _modeEnumValues = new Dictionary<ModeEnum, string>
    {
        { ModeEnum.Damage , "Damage" },
        { ModeEnum.Healing , "Healing" },
    };

    private void NextModeEnum() => Current = Current == ModeEnum.Damage ? ModeEnum.Healing : ModeEnum.Damage;

    public KeyValuePair<ModeEnum, string> GetNext()
    {
        NextModeEnum();

        foreach (var value in _modeEnumValues)
        {
            if (Current != value.Key)
                continue;

            return value;
        }

        return new KeyValuePair<ModeEnum, string>();
    }
}

public enum ModeEnum
{
    Damage,
    Healing,
}