using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using Shared;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace Manager.POH
{
    public partial class ManagerPOH : AOPluginEntry
    {
        private const string PluginName = "ManagerPOH";
        private string Version_Number = "0.0.2";

        private IPCChannel IPCChannel;

        public SMovementController SMovementController { get; set; }

        private Settings _settings;

        private List<Settings> settingsToSave = new List<Settings>();
        private List<string> ErrorMessages = new List<string>();

        private Kits kitsInstance = new Kits();

        private Window _mainWindow;
        private Window _infoWindow;
        private Window _pohWindow;
        private Window _phoBuffWindow;

        private Vector3 returnPosition;
        private Vector3 alterReturnPosition = new Vector3( 64.8f, 1.0f, 276.7f );

        private bool[] AtPOS = new bool[4];
        private bool InPos;
        private bool CheckPOS;

        private Vector3 Waypoint = Vector3.Zero;
        private Vector3 NextRiftPosition = Vector3.Zero;
        
        private enum Dance
        {
            DanceBallet = 7,
            DanceChicken = 11,
            DanceDisco = 16,
            DanceFlamenco = 21,
            DancePulp = 42,
            DanceYmca = 63,
        }

        public override void Run()
        {
            try
            {
                if (Game.IsNewEngine)
                {
                    Chat.WriteLine("Does not work on this engine!");
                    return;
                }

                _settings = new Settings(PluginName);

                SMovementControllerSettings mSettings = new SMovementControllerSettings
                {
                    NavMeshSettings = new SNavMeshSettings { DrawNavMesh = false, DrawDistance = 30 },

                    PathSettings = new SPathSettings
                    {
                        DrawPath = false,
                        MinRotSpeed = 10,
                        MaxRotSpeed = 30,
                        UnstuckUpdate = 5000,
                        UnstuckThreshold = 2f,
                        RotUpdate = 10,
                        MovementUpdate = 200,
                        PathRadius = 0.26f,
                        Extents = new Vector3(3.0f, 3.0f, 3.0f)
                    }
                };

                SMovementController.Set(mSettings);
                SMovementController.AutoLoadNavmeshes($"{PluginDirectory}\\NavMeshes");

                _settings.AddVariable("IPCChannel", 7);
                _settings["IPCChannel"] = 7;

                _settings.AddVariable("KitNanoPercentageBox", 90);
                _settings.AddVariable("KitHealthPercentageBox", 90);

                _settings.AddVariable("AutoSit", false);

                _settings.AddVariable("Rift", false);
                _settings.AddVariable("Alters", false);

                int pos = 0;

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.Enforcer: pos = 1; break;
                    case Profession.Bureaucrat: pos = 2; break;
                    case Profession.Engineer: pos = 3; break;
                    case Profession.Metaphysicist: pos = 4; break;
                }

                _settings.AddVariable("POHPositions", pos);

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                _settings.AddVariable("PhoMainWindowTopLeftX", 50f);
                _settings.AddVariable("PhoMainWindowTopLeftY", 50f);

                settingsToSave.Add(_settings);

                IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

                IPCChannel.RegisterCallback((int)IPCOpcode.POHPathing, POHPathingReceived);
                IPCChannel.RegisterCallback((int)IPCOpcode.POHBool, POHBoolReceived);
                IPCChannel.RegisterCallback((int)IPCOpcode.UISettings, BroadcastSettingsReceived);

                Chat.RegisterCommand(PluginName, ManagerCommand);

                Game.OnUpdate += OnUpdate;
                Network.N3MessageSent += N3MessageSent;
                Network.N3MessageReceived += N3MessageReceived;
                UIController.WindowDeleted += Windowclosed;
                
                kitsInstance = new Kits();

                Chat.WriteLine($"{PluginName} Loaded!");
                Chat.WriteLine($"/{PluginName} for UI.");
                Chat.WriteLine($"/macro {PluginName} /{PluginName}");
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region IPC message Received

        private void BroadcastSettingsReceived(int arg1, IPCMessage message)
        {
            if (message is UISettings uISettings)
            {
                _settings["Alters"] = uISettings.Alters;
                Save();
            }
        }

        private void POHPathingReceived(int arg1, IPCMessage message)
        {
            if (Playfield.ModelIdentity.Instance != 8020) return;

            var Pos = message as POHPathing;

            CheckPOS = Pos.ShouldPath;

            if (CheckPOS)
            {
                Array.Clear(AtPOS, 0, AtPOS.Length);
                InPos = false;

                if (returnPosition == Vector3.Zero)
                {
                    returnPosition = Pos.Position;
                    HandlePathingToPOS();
                }
            }
            else if (returnPosition != Vector3.Zero)
            {
                Network.Send(new SocialActionCmdMessage { Unknown5 = 0x3E, Unknown = 1, Action = SocialAction.DanceBallet });
                SMovementController.SetNavDestination(returnPosition);
                returnPosition = Vector3.Zero;
            }
        }

        private void POHBoolReceived(int arg1, IPCMessage message)
        {
            POHBool pos = message as POHBool;
            Chat.WriteLine(pos);
            if (AtPOS[0] != pos.AtPOS1)
                AtPOS[0] = pos.AtPOS1;
            if (AtPOS[1] != pos.AtPOS2)
                AtPOS[1] = pos.AtPOS2;
            if (AtPOS[2] != pos.AtPOS3)
                AtPOS[2] = pos.AtPOS3;
            if (AtPOS[3] != pos.AtPOS4)
                AtPOS[3] = pos.AtPOS4;

            Chat.WriteLine($"AtPOS1: {AtPOS[0]}, AtPOS2: {AtPOS[1]}, AtPOS3: {AtPOS[2]}, AtPOS4: {AtPOS[3]}");

        }

        #endregion

        #region Chat Commands

        private void ManagerCommand(string arg1, string[] arg2, ChatWindow window)
        {
            try
            {
                if (_mainWindow?.IsValid == true)
                {
                    Window_Closed_helper();

                    _mainWindow.Close();
                    _mainWindow = null;
                    return;
                }

                _mainWindow = Window.CreateFromXml(PluginName, PluginDirectory + "\\UI\\ManagerPOHWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                _mainWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

                if (_mainWindow.FindView("InfoView", out Button infoView))
                    infoView.Clicked = Info_Button_Clicked;

                if (_mainWindow.FindView("KitHealthPercentageBox", out TextInputView kitHealthInput))
                    kitHealthInput.Text = $"{_settings["KitHealthPercentageBox"].AsInt32()}";

                if (_mainWindow.FindView("KitNanoPercentageBox", out TextInputView kitNanoInput))
                    kitNanoInput.Text = $"{_settings["KitNanoPercentageBox"].AsInt32()}";

                if (_mainWindow.FindView("KitSave", out Button kitSaveButton))
                    kitSaveButton.Clicked = Save_Button_Clicked;

                if (_mainWindow.FindView("BroadcastSettingsView", out Button settingsButton))
                    settingsButton.Clicked = UISettingsButtonClicked;

                if (_mainWindow.FindView("POHPathButton", out Button pathButton))
                    pathButton.Clicked = POHPathButtonClicked;

                if (_mainWindow.FindView("POHView", out Button pohView))
                    pohView.Clicked = POHView;

                if (_mainWindow.FindView("PhoBuff", out Button phoButton))
                    phoButton.Clicked = PhoButtonClicked;

                if (_mainWindow.FindView("VersionNumber", out TextView version))
                    version.Text = $"Version {Version_Number}";

                _mainWindow.Show(true);

            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #endregion

        #region Button Clicked

        private void Info_Button_Clicked(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
                return;
            }

            _infoWindow = Window.CreateFromXml("Info", PluginDirectory + "\\UI\\ManagerPOHInfoWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            _infoWindow.Show(true);
        }

        private void PhoButtonClicked(object sender, ButtonBase e)
        {
            if (_phoBuffWindow?.IsValid == true)
            {
                Pho_Window_Closed();

                _phoBuffWindow.Close();
                _phoBuffWindow = null;
                return;
            }

            _phoBuffWindow = Window.CreateFromXml("Mirror", PluginDirectory + "\\UI\\PhoBuffWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _phoBuffWindow.MoveTo(_settings["PhoMainWindowTopLeftX"].AsFloat(), _settings["PhoMainWindowTopLeftY"].AsFloat());

            if (_phoBuffWindow.FindView("BuffName", out TextView buffNameText))
                buffNameText.Text = $"         ";

            if (_phoBuffWindow.FindView("Buff", out TextView buffText))
                buffText.Text = $"  ";

            _phoBuffWindow.Show(true);
        }

        private void POHView(object s, ButtonBase button)
        {
            if (_pohWindow?.IsValid == true)
            {
                _pohWindow.Close();
                _pohWindow = null;
                return;
            }

            _pohWindow = Window.CreateFromXml("POH", PluginDirectory + "\\UI\\POHPosWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _pohWindow.MoveTo(390, 220);

            _pohWindow.Show(true);
        }

        private void POHPathButtonClicked(object s, ButtonBase button)
        {
            if (Playfield.ModelIdentity.Instance != 8020) return;

            CheckPOS = !CheckPOS;

            IPCChannel.Broadcast(new POHPathing
            {
                Position = DynelManager.LocalPlayer.Position,
                ShouldPath = CheckPOS
            });

            if (CheckPOS)
            {
                Array.Clear(AtPOS, 0, AtPOS.Length);
                InPos = false;

                if (returnPosition == Vector3.Zero)
                {
                    returnPosition = DynelManager.LocalPlayer.Position;
                    HandlePathingToPOS();
                }
            }
            else
            {
                if (returnPosition != Vector3.Zero)
                    SMovementController.SetNavDestination(returnPosition);

                returnPosition = Vector3.Zero;
            }
        }

        private void Save_Button_Clicked(object sender, ButtonBase e)
        {
            if (_mainWindow.FindView("KitHealthPercentageBox", out TextInputView kitHealthInput) &&
            int.TryParse(kitHealthInput.Text, out int kitHealthValue))
            {
                if (_settings["KitHealthPercentageBox"].AsInt32() != kitHealthValue)
                    _settings["KitHealthPercentageBox"] = kitHealthValue;
            }

            if (_mainWindow.FindView("KitNanoPercentageBox", out TextInputView kitNanoInput) &&
                int.TryParse(kitNanoInput.Text, out int kitNanoValue))
            {
                if (_settings["KitNanoPercentageBox"].AsInt32() != kitNanoValue)
                    _settings["KitNanoPercentageBox"] = kitNanoValue;
            }
            Save();
        }

        private void UISettingsButtonClicked(object s, ButtonBase button)
        {
            IPCChannel.Broadcast(new UISettings()
            {
                Alters = _settings["Alters"].AsBool()
            });

            Save();
        }

        #endregion

        #region Events
        private void N3MessageReceived(object sender, N3Message e)
        {
            if (Playfield.ModelIdentity.Instance != 8050) return;
            if (e.N3MessageType != N3MessageType.FollowTarget) return;
            var followTaretMessage = (FollowTargetMessage)e;
            var pho = DynelManager.NPCs.FirstOrDefault(n => n != null && n.Health > 0 && n.Name == "The Awoken Nightmare, Phobettor");
            if (pho == null) return;
            if (followTaretMessage.Identity == pho?.Identity) return;
            if (!(followTaretMessage.Info is FollowTargetMessage.PathInfo pathInfo)) return;

            foreach (var waypoint in pathInfo.Waypoints)
            {
                if (Waypoint != waypoint)
                    Waypoint = waypoint;
            }
        }

        private void N3MessageSent(object sender, N3Message e)
        {
            if (e.N3MessageType != N3MessageType.CharacterAction) return;
            var charAction = (CharacterActionMessage)e;
            if (charAction.Action != CharacterActionType.Logout) return;

            Save();

            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            UIController.WindowDeleted -= Windowclosed;
            return;
        }

        private void Windowclosed(object sender, Window e)
        {
            switch (e.Name)
            {
                case PluginName:
                    Window_Closed_helper();
                    break;
                case "Mirror":
                    Pho_Window_Closed();
                    break;
            }
        }

        private void OnUpdate(object s, float deltaTime)
        {
            try
            {
                switch (Playfield.ModelIdentity.Instance)
                {
                    case 8020:
                        if (DynelManager.LocalPlayer.Room.Name == "Mutant King Boss Room")
                        {
                            var Azd = DynelManager.NPCs.FirstOrDefault(n => n != null && n.Health > 0 && n.Name == "Azdaja the Joyous");

                            if (Azd != null)
                            {
                                if (_settings["Alters"].AsBool())
                                {
                                    foreach (var altar in DynelManager.Terminals.Where(t => t.Name == "Altar of Purification"))
                                    {
                                        float distance = Vector3.Distance(DynelManager.LocalPlayer.Position, altar.Position);

                                        float speedPerSecond = 1f + (DynelManager.LocalPlayer.GetStat(Stat.RunSpeed) / 100f);

                                        float timeLeft = altar.GetStat(Stat.TimeExist);

                                        float estimatedTime = distance / speedPerSecond;

                                        bool hasDebuff = DynelManager.LocalPlayer.Buffs.Contains(new int[] { 302702, 270011, 302677 });

                                        bool atAltarXY = DynelManager.LocalPlayer.Position.Distance2DFrom(altar.Position) < 2.0f;

                                        if (!atAltarXY)
                                        {
                                            if (hasDebuff)
                                            {
                                                if (estimatedTime < timeLeft)
                                                {
                                                    if (distance <= 10.0f)
                                                    {
                                                        SMovementController.SetMovement(MovementAction.JumpStart);
                                                        SMovementController.SetDestination(altar.Position);
                                                    }
                                                    else if (!SMovementController.IsNavigating())
                                                        SMovementController.SetNavDestination(altar.Position);
                                                }
                                                else
                                                    SMovementController.SetNavDestination(alterReturnPosition);
                                            }
                                        }
                                        else if (!hasDebuff || timeLeft < 2.0f)
                                        {
                                            SMovementController.SetMovement(MovementAction.JumpStart);
                                            SMovementController.SetDestination(alterReturnPosition);
                                        }
                                    }
                                }
                            }
                        }
                        else if (CheckPOS)
                            AtPos();
                        break;
                    case 8050:

                        var pho = DynelManager.NPCs.FirstOrDefault(n => n != null && n.Health > 0 && n.Name == "The Awoken Nightmare, Phobettor");

                        if (pho != null)
                        {
                            if (Waypoint != Vector3.Zero)
                                Debug.DrawLine(pho.Position, Waypoint, DebuggingColor.Blue);

                            if (_phoBuffWindow?.IsValid == true)
                            {
                                var buff = pho.Buffs.FirstOrDefault(b => b.Id == 302810);

                                _phoBuffWindow.FindView("Buff", out TextView buffText);
                                _phoBuffWindow.FindView("BuffName", out TextView buffNameText);

                                if (buff != null)
                                {
                                    buffNameText.Text = $"Shattered";

                                    if (buff.RemainingTime > 10)
                                        buffText.SetDefaultColor(Color.Green);
                                    else
                                        buffText.SetDefaultColor(Color.Red);

                                    buffText.Text = buff.RemainingTime.ToString();
                                }
                                else
                                {
                                    buffNameText.Text = $"Mirrored";
                                    buffText.Text = "  ";
                                }
                            }

                            if (_settings["Rift"].AsBool())
                            {
                                foreach (var rift in DynelManager.AllDynels.Where(d => d.Name == "Unstable Rift" && d.Identity.Type == IdentityType.Terminal))
                                {
                                    var timeremaining = rift.GetStat(Stat.TimeExist);

                                    if (timeremaining < 1000)
                                    {
                                        Debug.DrawSphere(rift.Position, 5, DebuggingColor.Red);
                                        if (NextRiftPosition != Vector3.Zero)
                                            Debug.DrawLine(DynelManager.LocalPlayer.Position, NextRiftPosition, DebuggingColor.White);
                                    }
                                    else if (timeremaining < 2000)
                                        Debug.DrawSphere(rift.Position, 10, DebuggingColor.Yellow);
                                    else if (timeremaining < 3000)
                                        Debug.DrawSphere(rift.Position, 20, DebuggingColor.Green);
                                }
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #endregion

        #region Helpers

        private void AtPos()
        {
            //floor 0ne
            var pos1 = new Vector3(204.1, 7.8, 99.1);
            var pos2 = new Vector3(228.2, 7.8, 112.3);

            //floor two
            var pos3 = new Vector3(71.2, 6.0, 117.2);
            var pos4 = new Vector3(50.2, 6.0, 48.9);
            var pos5 = new Vector3(122.8, 6.0, 37.1);

            //floor three
            var pos6 = new Vector3(291.5, 9.0, 223.1);
            var pos7 = new Vector3(275.1, 6.0, 180.9);
            var pos8 = new Vector3(336.6, 6.0, 134.1);
            var pos9 = new Vector3(357.0, 6.0, 30.2);

            var playerPos = DynelManager.LocalPlayer.Position;

            if (!InPos)
            {
                if (playerPos.DistanceFrom(pos1) < 1 || playerPos.DistanceFrom(pos3) < 1 || playerPos.DistanceFrom(pos6) < 1)
                {
                    AtPOS[0] = true;
                    IPCChannel.Broadcast(new POHBool { AtPOS1 = AtPOS[0], AtPOS2 = AtPOS[1], AtPOS3 = AtPOS[2], AtPOS4 = AtPOS[3] });
                    InPos = true;
                }
                else if (playerPos.DistanceFrom(pos2) < 1 || playerPos.DistanceFrom(pos4) < 1 || playerPos.DistanceFrom(pos7) < 1)
                {
                    AtPOS[1] = true;
                    IPCChannel.Broadcast(new POHBool { AtPOS1 = AtPOS[0], AtPOS2 = AtPOS[1], AtPOS3 = AtPOS[2], AtPOS4 = AtPOS[3] });
                    InPos = true;
                }
                else if (playerPos.DistanceFrom(pos5) < 1 || playerPos.DistanceFrom(pos8) < 1)
                {
                    AtPOS[2] = true;
                    IPCChannel.Broadcast(new POHBool { AtPOS1 = AtPOS[0], AtPOS2 = AtPOS[1], AtPOS3 = AtPOS[2], AtPOS4 = AtPOS[3] });
                    InPos = true;
                }
                else if (playerPos.DistanceFrom(pos9) < 1)
                {
                    AtPOS[3] = true;
                    IPCChannel.Broadcast(new POHBool { AtPOS1 = AtPOS[0], AtPOS2 = AtPOS[1], AtPOS3 = AtPOS[2], AtPOS4 = AtPOS[3] });
                    InPos = true;
                }
            }

            CheckReadyAndHandle();
        }

        private void CheckReadyAndHandle()
        {
            int posCount = AtPOS.Count(x => x);

            int required = 0;

            if (DynelManager.LocalPlayer.Room.Instance >= 0 && DynelManager.LocalPlayer.Room.Instance <= 6)
                required = 2;
            else if (DynelManager.LocalPlayer.Room.Instance >= 8 && DynelManager.LocalPlayer.Room.Instance <= 19)
                required = 3;
            else if (DynelManager.LocalPlayer.Room.Instance >= 20 && DynelManager.LocalPlayer.Room.Instance <= 44)
                required = 4;
            
            if (required > 0 && posCount >= required)
            {
                
                var portalKeepers = DynelManager.Characters.FirstOrDefault(c => c.Name.Contains("Portal") && c.Health > 0 && c.IsInLineOfSight);

                if (portalKeepers != null)
                {
                    if (!DynelManager.LocalPlayer.IsAttacking)
                    DynelManager.LocalPlayer.Attack(portalKeepers, false);
                }
                else
                {
                    if (returnPosition != Vector3.Zero)
                    {
                        Network.Send(new SocialActionCmdMessage { Unknown5 = 0x3E, Unknown = 1, Action = SocialAction.DanceYmca });
                        SMovementController.SetNavDestination(returnPosition);
                    }

                    returnPosition = Vector3.Zero;
                    InPos = false;
                    Array.Clear(AtPOS, 0, AtPOS.Length);
                    CheckPOS = false;
                }
            }
        }


        private void HandlePathingToPOS()
        {
            switch (DynelManager.LocalPlayer.Room.Instance)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    switch (_settings["POHPositions"].AsInt32())
                    {
                        case 1:
                            HandleMovement(204.1, 7.8, 99.1);
                            break;
                        case 2:
                            HandleMovement(228.2, 7.8, 112.3);
                            break;
                        case 3:
                            returnPosition = Vector3.Zero;
                            break;
                        case 4:
                            returnPosition = Vector3.Zero;
                            break;
                    }
                    break;

                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                    switch (_settings["POHPositions"].AsInt32())
                    {
                        case 1:
                            HandleMovement(71.2, 6.0, 117.2);
                            break;
                        case 2:
                            HandleMovement(50.2, 6.0, 48.9);
                            break;
                        case 3:
                            HandleMovement(122.8, 6.0, 37.1);
                            break;
                        case 4:
                            returnPosition = Vector3.Zero;
                            break;
                    }
                    break;

                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 34:
                case 35:
                case 36:
                case 37:
                case 39:
                case 40:
                case 42:
                case 43:
                case 44:
                    switch (_settings["POHPositions"].AsInt32())
                    {
                        case 1:
                            HandleMovement(291.5, 9.0, 223.1);
                            break;
                        case 2:
                            HandleMovement(275.1, 6.0, 180.9);
                            break;
                        case 3:
                            HandleMovement(336.6, 6.0, 134.1);
                            break;
                        case 4:
                            HandleMovement(357.0, 6.0, 30.2);
                            break;
                    }
                    break;

                default:
                    break;
            }
        }

        private void HandleMovement(double x, double z, double y)
        {
            if (SMovementController.IsNavigating()) return;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(new Vector3(x, z, y)) > 1)
            {
                Network.Send(new SocialActionCmdMessage { Unknown5 = 0x3E, Unknown = 1, Action = SocialAction.DanceFlamenco });
                SMovementController.SetNavDestination(new Vector3(x, z, y));
            }
        }

        private void Window_Closed_helper()
        {
            if (_mainWindow?.IsValid == true)
            {
                Rect frame = _mainWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }

        private void Pho_Window_Closed()
        {
            if (_phoBuffWindow?.IsValid == true)
            {
                Rect frame = _phoBuffWindow.GetFrame();
                _settings["PhoMainWindowTopLeftX"] = frame.MinX;
                _settings["PhoMainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }

        public override void Teardown()
        {
            Save();
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            UIController.WindowDeleted -= Windowclosed;
            _mainWindow?.Close();
        }

        #endregion

        #region Misc.

        private void Save()
        {
            settingsToSave.ForEach(settings => settings.Save());
        }

        private void ErrorCatch(Exception ex)
        {
            var output = ex.Message + Environment.NewLine + "   at " + ex.TargetSite?.DeclaringType?.FullName + "." + ex.TargetSite?.Name;

            if (!ErrorMessages.Contains(output))
                ErrorMessages.Add(output);

            if (_mainWindow != null && _mainWindow.IsValid && _mainWindow.FindView("Errors", out View errorView))
                PopulateErrorView(errorView);
        }

        private void PopulateErrorView(View errorView)
        {
            errorView.DeleteAllChildren();

            if (ErrorMessages != null && ErrorMessages.Count > 0)
            {
                foreach (var error in ErrorMessages)
                {
                    var parts = error.Split(new[] { "   at " }, StringSplitOptions.None);

                    View xmlRoot = View.CreateFromXml($"{PluginDirectory}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                    xmlRoot.FindChild("TextLabel", out TextView labelView);
                    labelView.Text = parts[0];
                    labelView.SetColor(Color.Red);
                    errorView.AddChild(xmlRoot, true);

                    if (parts.Length > 1)
                    {
                        View methodRoot = View.CreateFromXml($"{PluginDirectory}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                        methodRoot.FindChild("TextLabel", out TextView methodLabel);
                        methodLabel.Text = "at " + parts[1];
                        errorView.AddChild(methodRoot, true);
                    }
                }
            }
        }

        #endregion
    }
}

