using AOSharp.Common.GameData;
using AOSharp.Common.Unmanaged.DataTypes;
using AOSharp.Common.Unmanaged.Imports;
using AOSharp.Core.Inventory;
using AOSharp.Core;
using System;
using System.Collections.Generic;

namespace AutoItemLevel
{
    public static class Extensions
    {
        public unsafe static List<RequirementCriterion> GetReqs(this Item item, ItemActionInfo itemActionInfo = ItemActionInfo.UseCriteria)
        {
            Identity none = Identity.None;
            IntPtr pEngine;

            if ((pEngine = N3Engine_t.GetInstance()) == IntPtr.Zero)
                return new List<RequirementCriterion>();

            IntPtr pCriteria = N3EngineClientAnarchy_t.GetItemActionInfo(item.Pointer, itemActionInfo);

            if (pCriteria == IntPtr.Zero)
                return new List<RequirementCriterion>();

            List<RequirementCriterion> criteria = new List<RequirementCriterion>();

            foreach (IntPtr pReq in ((StdStructVector*)(pCriteria + 0x4))->ToList(0xC))
            {
                criteria.Add(new RequirementCriterion
                {
                    Param1 = *(int*)(pReq),
                    Param2 = *(int*)(pReq + 0x4),
                    Operator = *(UseCriteriaOperator*)(pReq + 0x8)
                });
            }
            return criteria;
        }
    }
}
