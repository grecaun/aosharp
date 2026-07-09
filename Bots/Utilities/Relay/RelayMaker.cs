using System;
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Inventory;
using System.Text.RegularExpressions;

namespace RelayMaker
{
    public class Main : AOPluginEntry
    {
        public static bool Toggle = false;
        public static bool stack = false;

        public static double delay;
        bool Isopen = false;

        Backpack relayBag;

        public static string previousErrorMessage = string.Empty;

        public override void Run()
        {
            try
            {
                Chat.WriteLine("Relay Bot loaded!");
                Chat.WriteLine("Name Bag 'relay'.");
                Chat.WriteLine("/relay to start combination.");

                Game.OnUpdate += OnUpdate;
                Inventory.ContainerOpened += ContainerOpened;

                Chat.RegisterCommand("relay", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    Toggle = !Toggle;
                    Chat.WriteLine($"relay : {Toggle}");
                });
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        private void ContainerOpened(object sender, Container e)
        {
            if (e?.Identity.Instance == relayBag?.Identity.Instance)
            {
                Isopen = true;
            }
        }

        public static int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;

            var lineMatch = Regex.Match(ex.StackTrace ?? "", @":line (\d+)$", RegexOptions.Multiline);

            if (lineMatch.Success)
            {
                lineNumber = int.Parse(lineMatch.Groups[1].Value);
            }

            return lineNumber;
        }

        private void OnUpdate(object s, float deltaTime)
        {
            relayBag = Inventory.Backpacks.FirstOrDefault(b => b.Name == "relay");

            if (relayBag == null) { return; }
            if (!Toggle) { return; }
            if (Time.AONormalTime < delay) { return; }

            var powerlessKyrozchSignalAmplifier = Inventory.Items.FirstOrDefault(c => c.Id == 284283);
            var poweredKyrozchTeleportationRelay = Inventory.Items.FirstOrDefault(c => c.Id == 284289);

            if (!Isopen)
            {
                foreach (var bag in Inventory.Items)
                {
                    if (bag.UniqueIdentity.Instance != relayBag.Identity.Instance) { continue; }
                    bag.Use();
                }
            }
            else
            {
                if (powerlessKyrozchSignalAmplifier != null && poweredKyrozchTeleportationRelay != null)
                {
                    powerlessKyrozchSignalAmplifier.CombineWith(poweredKyrozchTeleportationRelay);
                }

                if (powerlessKyrozchSignalAmplifier == null)
                {
                    ProcessBrokenandPowerlessSignalAmplifier();
                }

                if (poweredKyrozchTeleportationRelay == null)
                {
                    PoweredKyrozchTeleportationRelay();
                }
            }

            delay = Time.AONormalTime + 1.0;
        }

        void ProcessBrokenandPowerlessSignalAmplifier()
        {
            var one = Inventory.Items.FirstOrDefault(c => c.Id == 284280);
            var starting = Inventory.Items.FirstOrDefault(c => c.Id == 284280 && c.Slot.Instance != one?.Slot.Instance);
            var two = Inventory.Items.FirstOrDefault(c => c.Id == 284281);
            var three = Inventory.Items.FirstOrDefault(c => c.Id == 284282);

            var PowerlessKyrozchSignalAmplifier = Inventory.Items.FirstOrDefault(c => c.Id == 284283);

            if (PowerlessKyrozchSignalAmplifier != null) { return; }

            if (one != null)
            {
                if (three != null)
                {
                    three?.CombineWith(one);
                    return;
                }
                else if (two != null)
                {
                    two?.CombineWith(one);
                    return;
                }
                else if (one != null && starting != null)
                {
                    starting?.CombineWith(one);
                    return;
                }
                else
                {
                    MoveItem(284280, relayBag);
                    return;
                }
            }
            else
            {
                MoveItem(284280, relayBag);
                return;
            }
        }

        void PoweredKyrozchTeleportationRelay()
        {
            var KyrozchFuelGland = Inventory.Items.FirstOrDefault(c => c.Id == 284284);

            var InertKyrozchTeleportationRelayOne = Inventory.Items.FirstOrDefault(c => c.Id == 284285);
            var InertKyrozchTeleportationRelayTwo = Inventory.Items.FirstOrDefault(c => c.Id == 284286);
            var InertKyrozchTeleportationRelaythree = Inventory.Items.FirstOrDefault(c => c.Id == 284287);
            var InertKyrozchTeleportationRelayFour = Inventory.Items.FirstOrDefault(c => c.Id == 284288);

            var PoweredKyrozchTeleportationRelay = Inventory.Items.FirstOrDefault(c => c.Id == 284289);

            if (PoweredKyrozchTeleportationRelay != null) { return; }

            if (KyrozchFuelGland != null)
            {
                if (InertKyrozchTeleportationRelayFour != null)
                {
                    KyrozchFuelGland.CombineWith(InertKyrozchTeleportationRelayFour);
                }
                if (InertKyrozchTeleportationRelaythree != null)
                {
                    KyrozchFuelGland.CombineWith(InertKyrozchTeleportationRelaythree);
                }
                else if (InertKyrozchTeleportationRelayTwo != null)
                {
                    KyrozchFuelGland.CombineWith(InertKyrozchTeleportationRelayTwo);
                }
                else if (InertKyrozchTeleportationRelayOne != null)
                {
                    KyrozchFuelGland.CombineWith(InertKyrozchTeleportationRelayOne);
                }
                else
                {
                    MoveItem(284285, relayBag);
                }
            }
            else
            {
                MoveItem(284284, relayBag);
            }
        }

        void MoveItem(int itemId, Backpack bag)
        {
            var item = bag.Items.FirstOrDefault(c => c.Id == itemId);

            item?.MoveToInventory();
            return;
        }

        public override void Teardown()
        {
        }
    }
}