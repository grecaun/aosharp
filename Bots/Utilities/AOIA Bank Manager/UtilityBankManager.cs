using AOSharp.Core;
using AOSharp.Core.Inventory;
using SmokeLounge.AOtomation.Messaging.Messages;
using AOSharp.Common.GameData;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using System.Collections.Generic;
using System.Linq;

namespace AOIA_Bank_Manager
{
    public class UtilityBankManager : AOPluginEntry
    {
        List<int> InventoryBags = new List<int>();
        List<int> BankBags = new List<int>();
        List<int> OpenedBags = new List<int>();

        double openDelay;
        bool BankOpened;

        public override void Run()
        {
            Game.OnUpdate += OnUpdate;
            Network.N3MessageReceived += N3MessageReceived;
            Inventory.ContainerOpened += ContainerOpened;

            foreach (var bag in Inventory.Backpacks.Where(c => c.Slot.Type == IdentityType.Inventory))
            {
                if (InventoryBags.Contains(bag.Identity.Instance)) { continue; }
                InventoryBags.Add(bag.Identity.Instance);
            }

            Chat.WriteLine("AOIA Bank Manager Loaded!");
        }

        private void ContainerOpened(object sender, Container container)
        {
            switch (container.Identity.Type)
            {
                case IdentityType.Container:
                    if (OpenedBags.Contains(container.Identity.Instance)) { return; }
                    var openedBag = Inventory.Items.FirstOrDefault(c => c.UniqueIdentity.Instance == container.Identity.Instance);
                    OpenedBags.Add(container.Identity.Instance);
                    openedBag?.Use();// close the opened bag
                    break;
            }
        }

        private void N3MessageReceived(object sender, N3Message e)
        {
            switch (e.N3MessageType)
            {
                case N3MessageType.Bank:
                    var bankMsg = (BankMessage)e;
                    BankOpened = true;
                    foreach (var bankSlot in bankMsg.BankSlots)
                    {
                        if (bankSlot.Identity.Type != IdentityType.Container) { continue; }
                        if (BankBags.Contains(bankSlot.Identity.Instance)) { continue; }
                        BankBags.Add(bankSlot.Identity.Instance);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnUpdate(object sender, float e)
        {
            if (Time.AONormalTime < openDelay) { return; }

            var InventoryBagToOpen = InventoryBags.FirstOrDefault(bagID => !OpenedBags.Contains(bagID));

            if (InventoryBagToOpen != 0)
            {
                Inventory.Items.FirstOrDefault(item => item.UniqueIdentity.Instance == InventoryBagToOpen)?.Use();
            }
            else if (BankOpened)
            {
                var InvBankBag = Inventory.Items.FirstOrDefault(c => BankBags.Contains(c.UniqueIdentity.Instance));

                var BankBag = Inventory.Bank.Items.FirstOrDefault(c => BankBags.Contains(c.UniqueIdentity.Instance)
                && !OpenedBags.Contains(c.UniqueIdentity.Instance));

                if (InvBankBag != null)
                {
                    if (OpenedBags.Contains(InvBankBag.UniqueIdentity.Instance))
                    {
                        InvBankBag?.MoveToBank();
                    }
                    else
                    {
                        InvBankBag?.Use();
                    }
                }
                else
                {
                    BankBag?.MoveToInventory();
                }
            }
            else
            {
                var Bank = DynelManager.AllDynels.FirstOrDefault(c => c.Name.Contains("Bank") && c.Identity.Type == IdentityType.Terminal
                && DynelManager.LocalPlayer.Position.DistanceFrom(c.Position) < 8);

                Bank?.Use();
            }

            openDelay = Time.AONormalTime + 1.0;
        }
    }
}
