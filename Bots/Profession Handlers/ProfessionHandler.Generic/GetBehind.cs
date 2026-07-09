using AOSharp.Common.GameData;
using AOSharp.Core.Movement;
using AOSharp.Core;
using System.Linq;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {

        public const float BehindDistOffset = 1.25f;
        public const float RaycastYOffset = 0.1f;

        int[] PFIgnores = new [] { 8050, 6055};

        public void MoveBehindFightingtarget()
        {
            if (PFIgnores.Contains(Playfield.ModelIdentity.Instance)) return;

            if (!DynelManager.LocalPlayer.IsAttacking || DynelManager.LocalPlayer.FightingTarget == null) return;

            if (DynelManager.LocalPlayer.FightingTarget.FightingTarget == null) return;

            if (DynelManager.LocalPlayer.Profession != Profession.Enforcer)
            {
                if (DynelManager.LocalPlayer.FightingTarget.FightingTarget.Identity == DynelManager.LocalPlayer.Identity && !DynelManager.LocalPlayer.FightingTarget.Buffs.Contains(NanoLine.Stun))
                    return;
            }

            if (DynelManager.LocalPlayer.Velocity != 0) return;

            Vector3 posBehindTarget = GetPositionBehindTarget(DynelManager.LocalPlayer.FightingTarget);

            if (posBehindTarget == null) return;

            if (DynelManager.LocalPlayer.Position.Distance2DFrom(posBehindTarget) < BehindDistOffset) return;

            MovementController.Instance.SetDestination(posBehindTarget);
        }

        private Vector3 GetPositionBehindTarget(SimpleChar target)
        {
            Vector3 posBehind = target.Position + Quaternion.AngleAxis(-180, target.Rotation.Forward).VectorRepresentation() * BehindDistOffset;

            return Playfield.Raycast(target.Position + Vector3.Up * RaycastYOffset, posBehind + Vector3.Up * RaycastYOffset, out Vector3 hitPos, out _) ? hitPos : posBehind;
        }
    }
}
