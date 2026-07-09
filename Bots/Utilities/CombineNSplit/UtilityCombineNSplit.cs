using AOSharp.Common.GameData;
using AOSharp.Common.Unmanaged.Imports;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CombineNSplit
{
    public class UtilityCombineNSplit : AOPluginEntry
    {
        public static string previousErrorMessage = string.Empty;

        Dictionary<Identity, Item> filteredItems = new Dictionary<Identity, Item>();

        State CurrentState = new State();
        //State CompareState = new State();

        int QL;
        int Quantity;
        string StringName;

        public override void Run()
        {
            base.Run();

            Chat.WriteLine("CombineNSplit Loaded!");
            Chat.WriteLine("Commands");
            Chat.WriteLine("/combineall");
            Chat.WriteLine("/combine Quality Level Item Name");
            Chat.WriteLine("/split Amount Quality Level Item Name");

            Chat.RegisterCommand("combineall", (string command, string[] param, ChatWindow chatWindow) =>
            {
                CurrentState = State.CombineAll;
            });

            Chat.RegisterCommand("combine", (string command, string[] param, ChatWindow chatWindow) =>
            {
                if (param.Length < 2 || !int.TryParse(param[0], out int qualityLevel))
                {
                    chatWindow.WriteLine("Usage: /combine 'quality level' 'item name'", ChatColor.LightBlue);
                    return;
                }

                StringName = string.Join(" ", param.Skip(1));
                QL = qualityLevel;
                CurrentState = State.Combine;
            });

            Chat.RegisterCommand("split", (string command, string[] param, ChatWindow chatWindow) =>
            {
                if (param.Length < 3 || !int.TryParse(param[0], out int parsedAmount) || !int.TryParse(param[1], out int qualityLevel))
                {
                    chatWindow.WriteLine("Usage: /split 'amount' 'quality level' 'item name'", ChatColor.LightBlue);
                    return;
                }

                StringName = string.Join(" ", param.Skip(2));
                Quantity = parsedAmount;
                QL = qualityLevel;
                CurrentState = State.Split;
            });

            Chat.WriteLine("Ready");
            CurrentState = State.Waiting;
            //CompareState = State.Done;
            Game.OnUpdate += OnUpdate;
        }

        private void OnUpdate(object sender, float e)
        {
            //if (CurrentState != CompareState)
            //{
            //    Chat.WriteLine($"{CurrentState}");
            //    CompareState = CurrentState;
            //}

            switch (CurrentState)
            {
                case State.CombineAll:

                    HandleCombineAll(Inventory.Items);

                    var duplicateItems = Inventory.Items.Where(item => item.Slot.Type == IdentityType.Inventory)
                     .GroupBy(item => new { item.Name, item.QualityLevel }).Where(group => group.Count() > 1)
                     .ToList();

                    if (duplicateItems.Any()) { return; }

                    if (Inventory.Bank.IsOpen)
                    {
                        HandleCombineAll(Inventory.Bank.Items, (int)IdentityType.BankByRef);
                    }

                    foreach (var item in filteredItems)
                    {
                        if (Inventory.NumFreeSlots < 2) { return; }
                        if (item.Key.Type != IdentityType.BankByRef) { continue; }

                        var itemToMovetoInventory = Inventory.Bank.Items.FirstOrDefault(c => c.Slot == item.Key);
                        if (itemToMovetoInventory == null) { continue; }
                        itemToMovetoInventory?.MoveToInventory();
                        filteredItems.Remove(item.Key);
                        return;
                    }

                    CurrentState = State.Done;
                    break;
                case State.Combine:

                    HandleCombineQL(Inventory.Items, QL, StringName);

                    if (Inventory.Items.Where(c => c.Name == StringName && c.QualityLevel == QL).ToList().Count > 1) { return; }

                    if (Inventory.Bank.IsOpen)
                    {
                        HandleCombineQL(Inventory.Bank.Items, QL, StringName, (int)IdentityType.BankByRef);
                    }

                    foreach (var item in Inventory.Bank.Items.Where(x => x.Name == StringName && x.QualityLevel == QL))
                    {
                        if (Inventory.NumFreeSlots < 2) { return; }

                        item.MoveToInventory();
                        return;
                    }

                    CurrentState = State.Done;
                    break;
                case State.Split:
                    Split(Inventory.Items, Quantity, QL, StringName);

                    if (Inventory.Bank.IsOpen)
                    {
                        Split(Inventory.Bank.Items, Quantity, QL, StringName);
                    }

                    CurrentState = State.Done;
                    break;
            }
        }

        private void Split(IReadOnlyList<Item> items, int amount, int qualityLevel, string name)
        {
            var ItemToSplit = items.FirstOrDefault(c => c.Name == name && c.Charges > amount && c.QualityLevel == qualityLevel);

            if (ItemToSplit != null)
            {
                SplitItem(ItemToSplit.Slot, amount);
            }
        }

        private void HandleCombineAll(IReadOnlyList<Item> items, int location = (int)IdentityType.Inventory)
        {
            foreach (var item in items)
            {
                var canFlags = (CanFlags)item.GetStat(Stat.Can);

                if (!canFlags.HasFlag(CanFlags.Stackable)) { continue; }
                if (canFlags.HasFlag(CanFlags.CantSplit)) { continue; }

                var matchingItems = Inventory.Items.Where(i => i.Name == item.Name && i.QualityLevel == item.QualityLevel).ToList();

                if (matchingItems.Count > 1)
                {
                    var targetItem = matchingItems.FirstOrDefault();
                    var parameterItem = matchingItems?.FirstOrDefault(i => i.Slot != targetItem?.Slot);

                    Network.Send(new CharacterActionMessage
                    {
                        Action = (CharacterActionType)53,
                        Unknown1 = 0,
                        Target = targetItem.Slot,
                        Parameter1 = location,
                        Parameter2 = parameterItem.Slot.Instance,
                        Unknown2 = 0,
                    });
                }
            }

            foreach (var item in items)
            {
                if (filteredItems.ContainsKey(item.Slot)) { filteredItems.Remove(item.Slot); }

                var canFlags = (CanFlags)item.GetStat(Stat.Can);

                if (canFlags.HasFlag(CanFlags.Stackable) && !canFlags.HasFlag(CanFlags.CantSplit))
                {
                    filteredItems[item.Slot] = item;
                }
            }
        }

        private void HandleCombineQL(IReadOnlyList<Item> receivedlist, int qualityLevel, string name, int location = (int)IdentityType.Inventory)
        {
            foreach (var item in receivedlist)
            {
                var canFlags = (CanFlags)item.GetStat(Stat.Can);

                if (!canFlags.HasFlag(CanFlags.Stackable)) { continue; }

                var matchingItems = Inventory.Items.Where(i => i.Name == name && i.QualityLevel == qualityLevel).ToList();

                if (matchingItems.Count > 1)
                {
                    var targetItem = matchingItems.FirstOrDefault();
                    var parameterItem = matchingItems?.FirstOrDefault(i => i.Slot != targetItem?.Slot);

                    Network.Send(new CharacterActionMessage
                    {
                        Action = (CharacterActionType)53,
                        Unknown1 = 0,
                        Target = targetItem.Slot,
                        Parameter1 = location,
                        Parameter2 = parameterItem.Slot.Instance,
                        Unknown2 = 0,
                    });
                }
            }
        }

        private enum State
        {
            CombineAll, Combine, Split, Done, Waiting,
        }

        public override void Teardown()
        {
            base.Teardown();
        }

        [DllImport("Gamecode.dll", EntryPoint = "?N3Msg_SplitItem@n3EngineClientAnarchy_t@@QAE_NABVIdentity_t@@H@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern bool SplitItem(IntPtr pThis, ref Identity identity, int count);

        public static bool SplitItem(Identity identity, int count)
        {
            return SplitItem(N3Engine_t.GetInstance(), ref identity, count);
        }
    }
}
