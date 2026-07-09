using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.IPC;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using ManagerHelp.IPCMessages;
using Shared;
using Debug = AOSharp.Core.Debug;

namespace ManagerHelp
{
    public partial class ManagerHelp : AOPluginEntry
    {
        const string PluginName = "ManagerHelp";
        private string Version_Number = "2.0.7";

        private IPCChannel IPCChannel;

        private double _sitPetUpdateTimer;

        private double _shapeUsedTimer;
        private double _morphPathingTimer;
        private double _bellyPathingTimer;
        private double _zixMorphTimer;

        private Window settingsWindow;
        private Window _infoWindow;
        private Window _eumenidesWindow;

        private Vector3 returnPosition = Vector3.Zero;

        private Settings _settings;

        private List<Settings> settingsToSave = new List<Settings>();

        List<Vector3> MorphBird = new List<Vector3>
        {
            new Vector3(75.5, 29.0, 58.6),
            new Vector3(37.3, 29.0, 59.0),
            new Vector3(35.6, 29.3, 30.5),
            new Vector3(37.3, 29.0, 59.0),
            new Vector3(75.5, 29.0, 58.6),
            new Vector3(75.5, 29.0, 58.6)
            //new Vector3(76.1, 29.0, 28.3)
        };

        List<Vector3> MorphHorse = new List<Vector3>
        {
            new Vector3(128.4, 29.0, 59.6),
            new Vector3(161.9, 29.0, 59.5),
            new Vector3(163.9, 29.4, 29.6),
            new Vector3(161.9, 29.0, 59.5),
            new Vector3(128.4, 29.0, 59.6),
            new Vector3(128.4, 29.0, 59.6)
            //new Vector3(76.1, 29.0, 28.3)
        };

        List<Vector3> OutBellyPath = new List<Vector3>
        {
            new Vector3(214.8f, 100.6f, 126.5f),
            new Vector3(211.0f, 100.3f, 135.1f)
        };

        public static readonly int[] ZixMorph = { 288532, 302212 };
        public static int[] GridNanos = { 296546 };
        private int[] GridCans = { 288822, 303392 };

        Kits kitsInstance = new Kits();
        private float _arrowHeight = 10f;

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

                _settings.AddVariable("IPCChannel", 2);
                _settings["IPCChannel"] = 2;
                _settings.AddVariable("KitNanoPercentageBox", 90);
                _settings.AddVariable("KitHealthPercentageBox", 90);

                _settings.AddVariable("AutoSit", false);
                _settings.AddVariable("Traps", false);
                _settings.AddVariable("RookieArrow", false);

                _settings.AddVariable("MorphPathing", false);
                _settings.AddVariable("BellyPathing", false);
                _settings.AddVariable("Eumenides", false);
                _settings.AddVariable("Db3Shapes", false);

                _settings.AddVariable("RKYalm", 0);
                _settings.AddVariable("SLYalm", 0);

                _settings.AddVariable("Positions", 0);

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                settingsToSave.Add(_settings);

                IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

                IPCChannel.RegisterCallback((int)IPCOpcode.fly, FlyMessageReceived);
                IPCChannel.RegisterCallback((int)IPCOpcode.UISettings, BroadcastSettingsReceived);
                IPCChannel.RegisterCallback((int)IPCOpcode.Grid, IPC_Grid_Message_Received);
                IPCChannel.RegisterCallback((int)IPCOpcode.YalmSelection, Yalm_Selection_Message_Received);

                Chat.RegisterCommand(PluginName, ManagerCommand);
                Chat.RegisterCommand("ManagerHelpchannel", ChannelCommand);
                Chat.RegisterCommand("autosit", AutoSitSwitch);
                Chat.RegisterCommand("gridall", GridCommand);
                Chat.RegisterCommand("fly", FlyCommand);

                Game.OnUpdate += OnUpdate;
                UIController.WindowDeleted += Windowclosed;

                Chat.WriteLine($"{PluginName} Loaded!");
                Chat.WriteLine($"/{PluginName} for UI.");
                Chat.WriteLine($"/macro mHelp /{PluginName}");
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public override void Teardown()
        {
            Save();
            UIController.WindowDeleted -= Windowclosed;
            Game.OnUpdate -= OnUpdate;
        }

        private void Windowclosed(object sender, Window e)
        {
            switch (e.Name)
            {
                case PluginName:
                    Window_Closed_helper();
                    break;
            }
        }

        private void Window_Closed_helper()
        {
            if (settingsWindow?.IsValid == true)
            {
                Rect frame = settingsWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;

                Save();
            }
        }

        #region IPC Received

        private void BroadcastSettingsReceived(int arg1, IPCMessage message)
        {
            if (message is UISettings uISettings)
            {
                _settings["AutoSit"] = uISettings.AutoSit;
                _settings["MorphPathing"] = uISettings.MorphPathing;
                _settings["BellyPathing"] = uISettings.BellyPathing;
                _settings["Eumenides"] = uISettings.Eumenides;
                _settings["Db3Shapes"] = uISettings.Db3Shapes;

                Save();
            }
        }

        private void IPC_Grid_Message_Received(int arg1, IPCMessage message)
        {
            EnterTheGrid();
        }

        #endregion

        private void OnUpdate(object s, float deltaTime)
        {
            try
            {
                var localPlayer = DynelManager.LocalPlayer;
                var playerRoom = localPlayer.Room;

                if (_settings["AutoSit"].AsBool())
                {
                    kitsInstance.SitAndUseKit(_settings["KitNanoPercentageBox"].AsInt32(), _settings["KitHealthPercentageBox"].AsInt32());

                    if (Time.AONormalTime > _sitPetUpdateTimer + 2)
                    {
                        if (localPlayer.Profession == Profession.Metaphysicist || localPlayer.Profession == Profession.Agent)
                            PetSitKit();

                        _sitPetUpdateTimer = Time.AONormalTime;
                    }
                }

                if (_settings["Traps"].AsBool())
                {
                    foreach (var dynel in DynelManager.AllDynels.Where(d => localPlayer.Position.DistanceFrom(d.Position) < 60))
                    {
                        if (dynel is SimpleChar sc && sc.IsPlayer) continue;
                        
                        if (dynel.Name.Contains("Mine") || dynel.Name.Contains("Trap") || dynel.Name.Contains("Collision Spawn") || dynel.Name.Contains("Unicorn Laser Fence"))
                        {
                            var rad = dynel.Radius;

                            if (rad > 1)
                                Debug.DrawSphere(dynel.Position, rad, DebuggingColor.Red);
                            else
                                Debug.DrawSphere(dynel.Position, 1, DebuggingColor.Red);
                        }
                    }
                }

                if (Time.AONormalTime > _zixMorphTimer + 3)
                {
                    if (localPlayer.Buffs.Contains(new[] { 288532, 302212 }))
                        CancelBuffs(ZixMorph);

                    _zixMorphTimer = Time.AONormalTime;
                }

                switch (Playfield.ModelIdentity.Instance)
                {
                    case 9070:

                        if (playerRoom.Name == "Shopping Dead-end")
                        {
                            if (_settings["Eumenides"].AsBool())
                            {
                                var _eumenides = DynelManager.NPCs.Where(c => c.Name == "Eumenides").FirstOrDefault();

                                if (_eumenides != null)
                                    HandleEumenides();
                            }
                        }

                        if (_settings["BellyPathing"].AsBool() && Time.AONormalTime > _bellyPathingTimer)
                        {
                            var loaclPlayerPosition = localPlayer.Position;

                            var bellyRoom = Playfield.Rooms.FirstOrDefault(c => c.Name == "Abmouth's Stomach");
                            var abbyRoom = Playfield.Rooms.FirstOrDefault(c => c.Name == "Abmouth Showdown");

                            if (playerRoom == bellyRoom)
                            {
                                var Pustule = DynelManager.AllDynels.FirstOrDefault(x => x.Name == "Glowing Pustule");

                                if (Pustule != null)
                                {
                                    if (loaclPlayerPosition.DistanceFrom(Pustule.Position) > 5)
                                    {
                                        if (!MovementController.Instance.IsNavigating)
                                        {
                                            if (loaclPlayerPosition.DistanceFrom(new Vector3(133.3458f, 90.01f, 118.7395f)) < 4f)
                                                MovementController.Instance.SetDestination(new Vector3(131.9f, 90.0f, 104.8f));
                                            else
                                                MovementController.Instance.SetDestination(Pustule.Position);
                                        }
                                    }
                                    else if (MovementController.Instance.IsNavigating)
                                        MovementController.Instance.Halt();
                                    else
                                        Pustule.Use();
                                }
                            }
                            else if (playerRoom == abbyRoom)
                            {
                                if (!MovementController.Instance.IsNavigating)
                                {
                                    if (loaclPlayerPosition.DistanceFrom(new Vector3(217.0f, 94.0f, 148.0f)) < 2f)
                                        MovementController.Instance.SetPath(OutBellyPath);
                                }
                            }

                            _bellyPathingTimer = Time.AONormalTime + 1;
                        }
                        break;
                    case 4021:
                        if (_settings["Db3Shapes"].AsBool() && Time.AONormalTime > _shapeUsedTimer + 0.5)
                        {
                            var shape = DynelManager.AllDynels.FirstOrDefault(x => x.Identity.Type == IdentityType.Terminal && localPlayer.DistanceFrom(x) < 5f
                                && (x.Name == "Triangle of Nano Power" || x.Name == "Cylinder of Speed" || x.Name == "Torus of Aim" || x.Name == "Square of Attack Power"));

                            shape?.Use();

                            _shapeUsedTimer = Time.AONormalTime;
                        }
                        break;
                    case 6015:
                        if (_settings["MorphPathing"].AsBool() && Time.AONormalTime > _morphPathingTimer + 2)
                        {
                            if (!MovementController.Instance.IsNavigating)
                            {
                                if (localPlayer.Buffs.Contains(281109))
                                {
                                    if (returnPosition == Vector3.Zero)
                                        returnPosition = localPlayer.Position;

                                    MovementController.Instance.SetPath(MorphBird);
                                }
                                else if (localPlayer.Buffs.Contains(281108))
                                {
                                    if (returnPosition == Vector3.Zero)
                                        returnPosition = localPlayer.Position;

                                    MovementController.Instance.SetPath(MorphHorse);
                                }
                            }

                            if (!localPlayer.Buffs.Contains(new[] { 281109, 281108 }) && returnPosition != Vector3.Zero)
                            {
                                MovementController.Instance.AppendDestination(returnPosition);
                                returnPosition = Vector3.Zero;
                            }

                            _morphPathingTimer = Time.AONormalTime;
                        }
                        break;
                    case 4366:
                        if (_settings["RookieArrow"].AsBool())
                        {
                            _arrowHeight -= 3.0f * deltaTime;
                            if (_arrowHeight <= 2f)
                                _arrowHeight = 10f;
                            var rookie = DynelManager.NPCs.Where(npc => npc.Name == "Rookie Alien Hunter").FirstOrDefault(h => h.Health > 0);
                            if (rookie != null)
                                DrawArrow(rookie, DebuggingColor.Red);
                        }



                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void EnterTheGrid()
        {
            var spell = Spell.List.FirstOrDefault(s => GridNanos.Contains(s.Id));

            if (spell != null)
                spell.Cast();
            else
            {
                var allItems = Inventory.Items.Concat(Inventory.Backpacks.SelectMany(bp => Inventory.GetContainerItems(bp.Identity)));
                var can = allItems.FirstOrDefault(c => GridCans.Contains(c.Id));
                can?.Use();
            }
        }

        #region Chat Commands

        private void ChannelCommand(string arg1, string[] arg2, ChatWindow window)
        {
            if (arg2 == null || arg2.Length == 0)
            {
                Chat.WriteLine($"Current IPC Channel: {_settings["IPCChannel"].AsInt32()}");
                return;
            }

            if (int.TryParse(arg2[0], out int newChannel))
            {
                if (newChannel < 1 || newChannel > 255)
                {
                    Chat.WriteLine("Invalid channel. Please enter a number between 1 and 255.");
                    return;
                }

                _settings["IPCChannel"] = newChannel;
                IPCChannel.SetChannelId(Convert.ToByte(_settings["IPCChannel"].AsInt32()));
                Chat.WriteLine($"IPC Channel set to: {_settings["IPCChannel"].AsInt32()}");
                Save();
            }
            else
            {
                Chat.WriteLine("Invalid input. Please enter a number between 1 and 255.");
            }
        }

        private void GridCommand(string arg1, string[] arg2, ChatWindow window)
        {
            IPCChannel.Broadcast(new UseGridMessage());
            EnterTheGrid();
        }

        private void AutoSitSwitch(string command, string[] param, ChatWindow chatWindow)
        {
            if (param.Length == 0)
            {
                _settings["AutoSit"] = !_settings["AutoSit"].AsBool();
                Chat.WriteLine($"Auto sit : {_settings["AutoSit"].AsBool()}");
            }
        }

        #endregion

        private void PetSitKit()
        {
            var healpet = DynelManager.LocalPlayer.Pets.Where(x => x.Type == PetType.Heal).FirstOrDefault();
            var kit = Inventory.Items.Where(x => RelevantItems.Kits.Contains(x.Id)).FirstOrDefault();

            if (healpet == null || kit == null) { return; }

            if (kitsInstance.CanUseSitKit() && DynelManager.LocalPlayer.DistanceFrom(healpet.Character) < 30f && healpet.Character.IsInLineOfSight)
            {
                if (healpet.Character.NanoPercent <= 75)
                {
                    if (!DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.Treatment))
                    {
                        if (DynelManager.LocalPlayer.MovementState != MovementState.Sit)
                            MovementController.Instance.SetMovement(MovementAction.SwitchToSit);
                        else
                            kit.Use(healpet.Character, true);
                    }
                    else if (DynelManager.LocalPlayer.MovementState == MovementState.Sit)
                        MovementController.Instance.SetMovement(MovementAction.LeaveSit);
                }
            }
        }

        private void CancelBuffs(int[] buffsToCancel)
        {
            foreach (Buff buff in DynelManager.LocalPlayer.Buffs)
            {
                if (buffsToCancel.Contains(buff.Id))
                    buff.Remove();
            }
        }

        private void Save()
        {
            settingsToSave.ForEach(settings => settings.Save());
        }

        private void DrawArrow(SimpleChar character, Vector3 color)
        {
            if (character == null || !character.IsValid)
                return;

            Vector3 head = character.Position + new Vector3(0, _arrowHeight, 0);
            Vector3 tip = head + new Vector3(0, 0.25f, 0);
            Vector3 start = tip + new Vector3(0, 0.75f, 0);

            IntPtr dbg = AOSharp.Common.Unmanaged.Imports.Debugger_t.GetInstance();
            AOSharp.Common.Unmanaged.Imports.Debugger_t.DrawLine(dbg, start.X, start.Y, start.Z, tip.X, tip.Y, tip.Z, color.X, color.Y, color.Z);

            Vector3 dir = (tip - start).Normalize();
            Vector3 up = Vector3.Up;
            if (Math.Abs(Vector3.Dot(up, dir)) > 0.99f)
                up = Vector3.Right;

            Vector3 side = Vector3.Cross(dir, up).Normalize();
            Vector3 back = tip - dir * 0.3f;

            Vector3 left = back + (side - dir) * 0.2f;
            Vector3 right = back + (-side - dir) * 0.2f;

            AOSharp.Common.Unmanaged.Imports.Debugger_t.DrawLine(dbg, tip.X, tip.Y, tip.Z, left.X, left.Y, left.Z, color.X, color.Y, color.Z);
            AOSharp.Common.Unmanaged.Imports.Debugger_t.DrawLine(dbg, tip.X, tip.Y, tip.Z, right.X, right.Y, right.Z, color.X, color.Y, color.Z);
        }
    }
}