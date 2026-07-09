using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using static MalisDamageMeter.MainWindow;

public class Scope
{
    public ScopeEnum Current;

    private Dictionary<ScopeEnum, int> _icons = new Dictionary<ScopeEnum, int>
    {
        { ScopeEnum.Solo , Textures.SoloScopeButton },
        { ScopeEnum.Team , Textures.TeamScopeButton },
        { ScopeEnum.All , Textures.AllScopeButton },
    };

    public bool Check(int instance)
    {
        if (Current == ScopeEnum.Solo && instance != DynelManager.LocalPlayer.Identity.Instance)
            return false;

        if (Current == ScopeEnum.Team && !Team.Members.Any(x => x.Character?.Identity.Instance == instance))
            return false;

        return true;
    }

    private bool IsAttacking() => DynelManager.LocalPlayer.IsAttacking;

    private bool IsTeamAttacking() => DynelManager.LocalPlayer.IsInTeam() && Team.Members.Any(x => x.Character != null && x.Character.IsAttacking);

    private bool IsAnyAttacking() => DynelManager.Players.Any(x => x.IsAttacking);

    private bool IsSoloAndAttacking() => Current == ScopeEnum.Solo && IsAttacking();

    private bool IsSoloAndNotAttacking() => Current == ScopeEnum.Solo && !IsAttacking();

    private bool IsTeamAndAttacking() => Current == ScopeEnum.Team && IsTeamAttacking();

    private bool IsTeamAndNotAttacking() => Current == ScopeEnum.Team && !IsTeamAttacking();

    private bool IsAllAndAttacking() => Current == ScopeEnum.All && IsAnyAttacking();

    private bool IsAllAndNotAttacking() => Current == ScopeEnum.All && !IsAnyAttacking();

    public bool IsInCombat => IsSoloAndAttacking() || IsTeamAndAttacking() || IsAllAndAttacking();

    public bool IsNotInCombat => IsSoloAndNotAttacking() || IsTeamAndNotAttacking() || IsAllAndNotAttacking();

    public void Next() => Current = Current == ScopeEnum.Solo ? ScopeEnum.Team : Current == ScopeEnum.Team ? ScopeEnum.All : ScopeEnum.Solo;

    public int SetIcon() => _icons[Current];
}

public enum ScopeEnum
{
    Solo,
    Team,
    All

}
