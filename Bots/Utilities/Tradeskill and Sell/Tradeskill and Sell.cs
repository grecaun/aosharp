using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;
using AOSharp.Common.GameData.UI;

namespace TradeSkillAndSell
{
    public class TradeSkillAndSell : AOPluginEntry
    {
        protected Settings _settings;

        static string SourceName;
        static string TargetName;
        static string SellItem;

        static int itemCount;

        static State CurrentState = new State();
        //static State CheckState = new State();
        TradeState LastTradeState = new TradeState();

        static bool Start;


        public override void Run()
        {
            base.Run();

            _settings = new Settings("TradeSkillAndSell");

            foreach (var bag in Inventory.Items.Where(b => b.UniqueIdentity.Type == IdentityType.Container))
            {
                bag.Use();
                bag.Use();
            }

            Network.N3MessageReceived += N3MessageReceived;
            Trade.TradeStateChanged += TradeStateChanged;

            _settings.AddVariable("SellToggle", false);

            RegisterSettingsWindow("TradeSkillAndSell", "TradeskillAndSellSettingWindow.xml");

            SettingsController.settingsWindow = Window.Create(new Rect(50, 50, 300, 300), "TradeSkillAndSell", "Settings", WindowStyle.Default, WindowFlags.AutoScale);

            if (SettingsController.settingsWindow != null && !SettingsController.settingsWindow.IsVisible)
            {
                SettingsController.AppendSettingsTab("TradeSkillAndSell", SettingsController.settingsWindow);

                if (SettingsController.settingsWindow.FindView("Start/Stop-Button", out Button ssButton))
                {
                    ssButton.Clicked += Start_Stop_Button_Clicked;
                }
                if (SettingsController.settingsWindow.FindView("SetButton", out Button setButton))
                {
                    setButton.Clicked += Set_Button_Clicked;
                }
            }

            Chat.RegisterCommand("source", (string command, string[] param, ChatWindow chatWindow) =>
            {
                SourceName = string.Join(" ", param.Skip(0));
                Chat.WriteLine($"Source Name = {string.Join(" ", param.Skip(0))}");
            });

            Chat.RegisterCommand("target", (string command, string[] param, ChatWindow chatWindow) =>
            {
                TargetName = string.Join(" ", param.Skip(0));
                Chat.WriteLine($"Target Name = {string.Join(" ", param.Skip(0))}");
            });

            Chat.RegisterCommand("saleitem", (string command, string[] param, ChatWindow chatWindow) =>
            {
                SellItem = string.Join(" ", param.Skip(0));
                Chat.WriteLine($"Item to sell name = {string.Join(" ", param.Skip(0))}");
            });

            Chat.RegisterCommand("enable", (string command, string[] param, ChatWindow chatWindow) =>
            {
                StartProcess();
            });

            Chat.RegisterCommand("disable", (string command, string[] param, ChatWindow chatWindow) =>
            {
                StopProcess();
            });


            if (!Game.IsNewEngine)
            {
                Chat.WriteLine("Tradeskill And Sell loaded!");
                Chat.WriteLine("/tsns for the UI");
                Chat.WriteLine("/enable to start, /disable to stop");
                Chat.WriteLine("/source name");
                Chat.WriteLine("/target name");
                Chat.WriteLine("/saleitem name");
            }
            else
            {
                Chat.WriteLine("Does not work on this engine!");
            }
        }
        public void Set_Button_Clicked(object sender, ButtonBase e)
        {
            SettingsController.settingsWindow.FindView("Source", out TextInputView sourceTextInput);
            SettingsController.settingsWindow.FindView("Target", out TextInputView targetTextInput);
            SettingsController.settingsWindow.FindView("Sell", out TextInputView sellTextInput);

            SourceName = sourceTextInput.Text;
            TargetName = targetTextInput.Text;

            Chat.WriteLine($"Source = {SourceName}", ChatColor.Red);
            Chat.WriteLine($"Target = {TargetName}", ChatColor.White);

            if (!_settings["SellToggle"].AsBool()) { return; }
            SellItem = sellTextInput.Text;
            Chat.WriteLine($"Sell = {SellItem}", ChatColor.DarkBlue);
        }

        public void Start_Stop_Button_Clicked(object sender, ButtonBase e)
        {
            if (!Start)
            {
                StartProcess();
            }
            else
            {
                StopProcess();
            }
        }

        private void StartProcess()
        {
            Chat.WriteLine($"Started");

            if (_settings["SellToggle"].AsBool())
            {
                CurrentState = State.Open;
            }
            else
            {
                CurrentState = State.Combine;
            }

            Game.OnUpdate += OnUpdate;
            Start = true;
        }
        private void StopProcess()
        {
            Chat.WriteLine($"Stopped");
            CurrentState = State.Stop;
            Game.OnUpdate -= OnUpdate;
            Start = false;
            if (_settings["SellToggle"].AsBool())
            {
                Trade.Accept();
            }
            return;
        }

        public override void Teardown()
        {
            SettingsController.CleanUp();
        }

        protected void RegisterSettingsWindow(string settingsName, string xmlName)
        {
            SettingsController.RegisterSettingsWindow(settingsName, PluginDirectory + "\\UI\\" + xmlName, _settings);
        }

        private void TradeStateChanged(Identity identity, TradeState state)
        {
            LastTradeState = state;
            //Chat.WriteLine($"{state}", ChatColor.DarkPink);
            switch (state)
            {
                case TradeState.Opened:
                    if (CurrentState == State.WaitingOnVendingMachine) { CurrentState = State.Combine; }
                    break;
                case TradeState.Finished:

                    break;
            }
        }

        private void N3MessageReceived(object sender, N3Message e)
        {
            switch (e.N3MessageType)
            {
                case N3MessageType.Trade:
                    var tradeMsg = (TradeMessage)e;
                    //Chat.WriteLine(tradeMsg.Action, ChatColor.LightBlue);
                    switch (tradeMsg.Action)
                    {
                        case TradeAction.AddItem:
                            if (itemCount >= 29)
                            {
                                Trade.Accept();
                                itemCount = 0;
                                Chat.WriteLine(Start);
                                if (!Start) { return; };
                                CurrentState = State.Open;
                            }
                            else
                            {
                                CurrentState = State.Combine;
                            }

                            itemCount++;
                            break;

                        case TradeAction.Complete:

                            if (!Start) { return; }

                            CurrentState = State.Open;

                            break;
                    }
                    break;
            }
        }

        private void OnUpdate(object sender, float e)
        {
            if (DynelManager.LocalPlayer.GetStat(Stat.Cash) >= 999999000)
            {
                CurrentState = State.Stop;
                Game.OnUpdate -= OnUpdate;
            }

            //if (CurrentState != CheckState)
            //{
            //    Chat.WriteLine(CurrentState);
            //    CheckState = CurrentState;
            //}

            switch (CurrentState)
            {
                case State.Open:
                    if (LastTradeState != TradeState.Opened)
                    {
                        var vendingMachine = DynelManager.AllDynels.Where(vm => vm.Identity.Type == IdentityType.VendingMachine)
                            .OrderBy(d => d.DistanceFrom(DynelManager.LocalPlayer)).FirstOrDefault(d => d.Position.DistanceFrom(DynelManager.LocalPlayer.Position) < 8);

                        vendingMachine?.Use();

                        CurrentState = State.WaitingOnVendingMachine;
                    }
                    else
                    {
                        CurrentState = State.Combine;
                    }
                    break;
                case State.MoveToTrade:

                    if (SellItem == null) { Chat.WriteLine("No Sell name"); StopProcess(); }

                    foreach (var item in Inventory.Items.Where(i => i.Name == SellItem))
                    {
                        Trade.AddItem(item.Slot);
                    }

                    break;
                case State.Combine:

                    if (SourceName == null) { Chat.WriteLine("No Source name"); StopProcess(); }
                    if (TargetName == null) { Chat.WriteLine("No Target Name"); StopProcess(); }

                    if (!Inventory.Items.Any(c => c.Name == SourceName))
                    {
                        foreach (var bag in Inventory.Backpacks)
                        {
                            foreach (var item in bag.Items)
                            {
                                if (item.Name == SourceName)
                                {
                                    item.MoveToInventory();
                                    return;
                                }
                            }
                        }
                    }

                    var source = Inventory.Items.FirstOrDefault(i => i.Name == SourceName);

                    if (!Inventory.Items.Any(c => c.Name == TargetName && c.Slot != source.Slot))
                    {
                        foreach (var bag in Inventory.Backpacks)
                        {
                            foreach (var item in bag.Items)
                            {
                                if (item.Name == TargetName)
                                {
                                    item.MoveToInventory();
                                    return;
                                }
                            }
                        }
                    }

                    var target = Inventory.Items.FirstOrDefault(i => i.Name == TargetName && i.Slot != source.Slot);

                    if (source == null) { Chat.WriteLine("Source is null"); return; }
                    if (target == null) { Chat.WriteLine("Target is null"); return; }

                    if (target.Slot.Type == IdentityType.Backpack) { target.MoveToInventory(); }
                    if (source.Slot.Type == IdentityType.Backpack) { source.MoveToInventory(); }

                    Network.Send(new CharacterActionMessage
                    {
                        Action = CharacterActionType.UseItemOnItem,
                        Identity = DynelManager.LocalPlayer.Identity,
                        Target = source.Slot,
                        Parameter1 = (int)IdentityType.Inventory,
                        Parameter2 = target.Slot.Instance,
                    });

                    if (_settings["SellToggle"].AsBool())
                    {
                        CurrentState = State.MoveToTrade;
                    }
                    else
                    {
                        CurrentState = State.Combine;
                    }
                    break;
            }
        }

        private enum State
        {
            Open, WaitingOnVendingMachine, Combine, MoveToTrade, Waiting, Stop
        }
    }

    public class Config
    {
        public Dictionary<string, CharacterSettings> CharSettings { get; set; }

        protected string _path;

        public static Config Load(string path)
        {
            Config config;

            try
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));

                config._path = path;
            }
            catch
            {
                Chat.WriteLine($"No config file found.");
                Chat.WriteLine($"Using default settings");

                config = new Config
                {
                    CharSettings = new Dictionary<string, CharacterSettings>()
                    {
                        { DynelManager.LocalPlayer.Name, new CharacterSettings() }
                    }
                };

                config._path = path;

                config.Save();
            }

            return config;
        }

        public void Save()
        {
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\AOSharp\\TradeSkillAndSell\\{DynelManager.LocalPlayer.Name}"))
            {
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\AOSharp\\TradeSkillAndSell\\{DynelManager.LocalPlayer.Name}");
            }

            File.WriteAllText(_path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }

    public class CharacterSettings
    {

    }
}
