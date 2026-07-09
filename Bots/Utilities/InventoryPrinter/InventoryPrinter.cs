using System.Linq;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;

namespace InventoryPrinter
{
    public class InventoryPrinter : AOPluginEntry
    {
        public override void Run()
        {
            Chat.RegisterCommand("print", Print_Inventory);
            Chat.WriteLine("/print to print all inventory items");
        }

        private void Print_Inventory(string arg1, string[] arg2, ChatWindow window)
        {
            if (arg2.Length == 0)
                foreach (var item in Inventory.Items)
                {
                    Chat.WriteLine(
                        $"Name {item.Name ?? "null"}, " +
                        $"Slot {item.Slot}, " +
                        $"ID {item.Id}, " +
                        $"UniqueIdentity {item.UniqueIdentity}");
                }

            if (arg2.Length > 0)
                foreach (var item in Inventory.Items.Where(i => i.Name.Contains(arg2[0])))
                {
                    Chat.WriteLine(
                        $"Name {item.Name ?? "null"}, " +
                        $"Slot {item.Slot}, " +
                        $"ID {item.Id}, " +
                        $"UniqueIdentity {item.UniqueIdentity}");
                }
        }
    }
}
