public static class ReformStateCore
{
    public enum Phase { Disbanding, Inviting, Completed }

    public class ContextData
    {
        public Phase Phase = Phase.Completed;
        public Phase LastPhase;
        public double DisbandDelay;
        public List<Identity> TeamCache = new();
        public List<Identity> InvitedList = new();
    }

    public static void OnEnter(ContextData ctx, double now, Action haltMovement, Action<string> writeLine, double disbandDelaySeconds, out double timeout)
    {
        haltMovement();
        writeLine("Reforming");
        ctx.Phase = Phase.Disbanding;
        ctx.TeamCache.Clear();
        ctx.InvitedList.Clear();
        ctx.DisbandDelay = now + disbandDelaySeconds;
        timeout = now + 10.0;
    }

    public static void Tick(
        ContextData ctx,
        double now,
        Func<bool> isZoning,
        Func<bool> isInTeam,
        Func<IEnumerable<TeamMember>> getMembers,
        Func<bool> settingsSolo,
        Func<Identity> myIdentity,
        Func<bool> isLeader,
        Action<Identity> kick,
        Action<Identity> invite,
        Action<string, ChatColor> writeLine,
        ref double timeout)
    {
        if (isZoning()) return;
        if (ctx.Phase != ctx.LastPhase)
        {
            writeLine(ctx.Phase.ToString(), ChatColor.Green);
            ctx.LastPhase = ctx.Phase;
        }

        switch (ctx.Phase)
        {
            case Phase.Disbanding:
                if (!isInTeam())
                {
                    ctx.Phase = Phase.Inviting;
                }
                else if (getMembers().All(m => m.Character == null) || settingsSolo())
                {
                    if (isLeader() && now >= ctx.DisbandDelay)
                    {
                        foreach (var m in getMembers())
                            if (!ctx.TeamCache.Contains(m.Identity))
                                ctx.TeamCache.Add(m.Identity);

                        foreach (var m in getMembers())
                            if (m.Identity != myIdentity())
                                kick(m.Identity);
                    }
                }
                break;

            case Phase.Inviting:
                if (isInTeam() && ctx.TeamCache.Count == getMembers().Count())
                {
                    ctx.Phase = Phase.Completed;
                }
                else if (isLeader())
                {
                    foreach (var p in ctx.TeamCache.Where(id => !getMembers().Any(m => m.Identity == id) && !ctx.InvitedList.Contains(id) && id != myIdentity()))
                    {
                        ctx.InvitedList.Add(p);
                        invite(p);
                    }
                }
                break;
        }
    }

    public static void OnExit(ContextData ctx, Action<string> writeLine, Action incrementCounter)
    {
        writeLine("Done Reforming");
        ctx.Phase = Phase.Disbanding;
        ctx.TeamCache.Clear();
        ctx.InvitedList.Clear();
        ctx.DisbandDelay = 0;
        incrementCounter();
    }
}
