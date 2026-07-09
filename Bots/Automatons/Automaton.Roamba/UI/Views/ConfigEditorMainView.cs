using AOSharp.Core;
using AOSharp.Core.UI;
using Stateless.Graph;
using System;
using System.Runtime.InteropServices;

namespace AutomatonRoamba
{
    public enum AttackRangeMode
    {
        Manual,
        Automatic
    }

    public class ConfigEditorMainView : RoambaView
    {
        private TextInputView _tauntRangeTextView;
        private TextInputView _pathRangeTextView;
        private TextInputView _wanderLimitTextView;
        private TextInputView _fightTimeoutTextView;
        private TextInputView _lootTimeoutTextView;
        private TextInputView _hpPercentTextView;
        private TextInputView _npPercentTextView;
        private TextInputView _followTarget;
        private TextInputView _attackInput;
        private TextView _attackName;
        private Checkbox _followTargetCheckbox;
        private Checkbox _disableIfCheckbox;
        private Checkbox _attackMobsCheckbox;
        private Checkbox _useTauntItemCheckbox;
        private Checkbox _pathToMobsCheckbox;
        private Checkbox _pathToCorpsesCheckbox;
        private Checkbox _priorityNamesOnlyCheckbox;
        private Checkbox _disableIfHpCheckbox;
        private Checkbox _disableIfNpCheckbox;
        private Checkbox _disableIfAttackedCheckbox;
        private Checkbox _disableIfNearbyPlayersCheckBox;
        private Checkbox _manualAttackRangeModeCheckbox;
        private Checkbox _automaticAttackRangeModeCheckbox;
        private TextInputView _fileName;
        private int _manualAttackRange;
        private int _automaticAttackPadding;
        private View _attackMobsOptions;
        private View _pathToCorpsesOptions;
        private View _tauntItemOptions;
        private View _disableIfOptions;
        private View _followTargetOptions;
        private View _pathToMobsOptions;
        public Action<string, bool> OnSave;

        public ConfigEditorMainView() : base(FilePath.ConfigEditorMainView)
        {
            if (Root.FindChild("AttackMobsInfo", out Button attackMobsInfo))
            {
                SetGfx(attackMobsInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                attackMobsInfo.Clicked += (sender, e) =>
                {
                    Chat.WriteLine("Attack Mobs - Toggles the option of attacking mobs\n" +
                        "Fight Timeout - How long until I ignore a mob that I am engaging with (s)\n" +
                        "Override Attack Range - Manually sets attack range\n" +
                        "Attack Range - Value of the overriden attack range (m)\n" +
                        "Automatic Attack Range - Automatically calculates attack ranged based of weapons\n" +
                        "Padding - Weapon range - padding = final attack range (m)");
                };
            }

            if (Root.FindChild("PathToCorpsesInfo", out Button pathToCorpsesInfo))
            {
                SetGfx(pathToCorpsesInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                pathToCorpsesInfo.Clicked += (sender, e) =>
                {
                    Chat.WriteLine("Path To Corpses - Toggles the option of pathing to corpses\n" +
                        "Loot Timeout - How long should I stand by a corpse (s)");
                };
            }

            if (Root.FindChild("PathToMobsInfo", out Button pathToMobsInfo))
            {
                SetGfx(pathToMobsInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                pathToMobsInfo.Clicked += (sender, e) =>
                {
                    Chat.WriteLine("Path To Mobs - Toggles the option of pathing to mobs\n" +
                        "Path Range - From which distance should I start pathing to mobs (m)");
                };
            }

            if (Root.FindChild("TauntItemInfo", out Button tauntItemInfo))
            {
                SetGfx(tauntItemInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                tauntItemInfo.Clicked += (sender, e) =>
                {
                    Chat.WriteLine("Use Taunt Item - Toggles the option of using taunt items\n" +
                        "Pull Range - From which distance should I use taunting devices (m)");
                };
            }

            if (Root.FindChild("FollowTargetInfo", out Button followTargetInfo))
            {
                SetGfx(followTargetInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                followTargetInfo.Clicked += (sender, e) =>
                {
                    Chat.WriteLine("Follow Targets - Toggles the option of following targets while pathing\n" +
                        "Names - Each line represents a name to be followed (first available name will be picked)");
                };
            }

            if (Root.FindChild("DisableIfInfo", out Button disableIfInfo))
            {
                SetGfx(disableIfInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                disableIfInfo.Clicked += (sender, e) =>
                {
                    Chat.WriteLine("Disable Pathing If - Toggles the option of disabling pathing under certain conditions\n" +
                        "Health (%) < - If health drops below given threshold\n" +
                        "Nano (%) < - If nano drops below given threshold\n" +
                        "Being Attacked < - If player is under attack\n" +
                        "Players Nearby - If there is players nearby (ignoring Follow Targets)");
                };
            }

            if (Root.FindChild("PriorityNamesOnlyInfo", out Button priorityNamesOnlyInfo))
            {
                SetGfx(priorityNamesOnlyInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                priorityNamesOnlyInfo.Clicked += (sender, e) =>
                {
                    Chat.WriteLine("Priority Names Only - Toggles the option of targeting Priority Names (only Priority Names will be targeted)");
                };
            }

            if (Root.FindChild("ExportSaveInfo", out Button exportSaveInfo))
            {
                SetGfx(exportSaveInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                exportSaveInfo.Clicked += (sender, e) =>
                {
                    Chat.WriteLine("Save or export your config. Configs can be accessed across multiple characters.");
                };
            }

            if (Root.FindChild("AttackMobs", out _attackMobsCheckbox)) { _attackMobsCheckbox.Toggled += OnAttackMobsToggle; }
            if (Root.FindChild("PathToCorpses", out _pathToCorpsesCheckbox)) { _pathToCorpsesCheckbox.Toggled += PathToCorpsesToggle; }
            if (Root.FindChild("TauntItem", out _useTauntItemCheckbox)) { _useTauntItemCheckbox.Toggled += UseTauntItemToggle; }
            if (Root.FindChild("PathToMobs", out _pathToMobsCheckbox)) { _pathToMobsCheckbox.Toggled += PathToMobsToggle; }
            if (Root.FindChild("TauntRange", out _tauntRangeTextView)) { }
            if (Root.FindChild("PathRange", out _pathRangeTextView)) { }
            if (Root.FindChild("WanderLimit", out _wanderLimitTextView)) { }
            if (Root.FindChild("FightTimeout", out _fightTimeoutTextView)) { }
            if (Root.FindChild("LootTimeout", out _lootTimeoutTextView)) { }
            if (Root.FindChild("DisableIf", out _disableIfCheckbox)) { _disableIfCheckbox.Toggled += DisableIfToggle; }
            if (Root.FindChild("DisableIfHp", out _disableIfHpCheckbox)) { }
            if (Root.FindChild("DisableIfNp", out _disableIfNpCheckbox)) { }
            if (Root.FindChild("DisableIfAttacked", out _disableIfAttackedCheckbox)) { }
            if (Root.FindChild("DisableIfPlayersNearby", out _disableIfNearbyPlayersCheckBox)) { }
            if (Root.FindChild("HpPercent", out _hpPercentTextView)) { }
            if (Root.FindChild("NpPercent", out _npPercentTextView)) { }
            if (Root.FindChild("FollowTargetList", out _followTarget)) { }
            if (Root.FindChild("FollowTarget", out _followTargetCheckbox)) { _followTargetCheckbox.Toggled += FollowTargetToggle; }
            if (Root.FindChild("ManualAttackRangeMode", out _manualAttackRangeModeCheckbox)) { _manualAttackRangeModeCheckbox.Toggled += OnManualAttackRangeModeToggle; }
            if (Root.FindChild("AutomaticAttackRangeMode", out _automaticAttackRangeModeCheckbox)) { _automaticAttackRangeModeCheckbox.Toggled += OnAutomaticAttackRangeModeToggle; }
            if (Root.FindChild("AttackInput", out _attackInput)) { }
            if (Root.FindChild("AttackName", out _attackName)) { }
            if (Root.FindChild("PriorityNamesOnly", out _priorityNamesOnlyCheckbox)) { }
            if (Root.FindChild("Save", out Button save)) { save.Clicked += SaveClick; }
            if (Root.FindChild("Close", out Button close)) { close.Clicked += CloseClick; }
            if (Root.FindChild("FileName", out _fileName)) { }


            if (Root.FindChild("AttackMobsOptions", out _attackMobsOptions)) { _attackMobsOptions.Show(_attackMobsCheckbox.IsChecked, true); }
            if (Root.FindChild("PathToCorpsesOptions", out _pathToCorpsesOptions)) { _pathToCorpsesOptions.Show(_pathToCorpsesCheckbox.IsChecked, true); }
            if (Root.FindChild("PathToMobsOptions", out _pathToMobsOptions)) { _pathToMobsOptions.Show(_pathToMobsCheckbox.IsChecked, true); }
            if (Root.FindChild("TauntItemOptions", out _tauntItemOptions)) { _tauntItemOptions.Show(_useTauntItemCheckbox.IsChecked, true); }
            if (Root.FindChild("DisableIfOptions", out _disableIfOptions)) { _disableIfOptions.Show(_disableIfCheckbox.IsChecked, true); }
            if (Root.FindChild("FollowTargetOptions", out _followTargetOptions)) { _followTargetOptions.Show(_followTargetCheckbox.IsChecked, true); }
        }

        private void SetGfx(Button button, string gfx)
        {
            button.SetGfx(AOSharp.Common.GameData.UI.ButtonState.Raised, gfx);
            button.SetGfx(AOSharp.Common.GameData.UI.ButtonState.Hover, gfx);
            button.SetGfx(AOSharp.Common.GameData.UI.ButtonState.Pressed, gfx);
        }

        private void CloseClick(object sender, ButtonBase e)
        {
            AutomatonRoamba.ConfigEditorWindow.Close();
        }

        private void OnAttackMobsToggle(object sender, bool e)
        {
            _attackMobsOptions.Show(e, true);
        }

        private void PathToCorpsesToggle(object sender, bool e)
        {
            _pathToCorpsesOptions.Show(e, true);
        }

        private void UseTauntItemToggle(object sender, bool e)
        {
            _tauntItemOptions.Show(e, true);
        }
        private void FollowTargetToggle(object sender, bool e)
        {
            _followTargetOptions.Show(e, true);
        }

        private void PathToMobsToggle(object sender, bool e)
        {
            _pathToMobsOptions.Show(e, true);
        }

        private void DisableIfToggle(object sender, bool e)
        {
            _disableIfOptions.Show(e, true);
        }

        private void OnAutomaticAttackRangeModeToggle(object sender, bool state)
        {
            _attackName.Text = state ? "Padding:" : "Attack Range:";
            _attackInput.Text = state ? _automaticAttackPadding.ToString() : _manualAttackRange.ToString();
            _manualAttackRangeModeCheckbox.SetValue(!state);
        }

        private void OnManualAttackRangeModeToggle(object sender, bool state)
        {
            _attackName.Text = state ? "Attack Range:" : "Padding:";
            _attackInput.Text = state ? _manualAttackRange.ToString() : _automaticAttackPadding.ToString();
            _automaticAttackRangeModeCheckbox.SetValue(!state);
        }

        private void SaveClick(object sender, ButtonBase e)
        {
            if (string.IsNullOrEmpty(_fileName.Text))
            {
                AutomatonRoamba.Log.Warning("Please provide a file name");
                return;
            }

            if (!_attackMobsCheckbox.IsChecked && DynelManager.LocalPlayer.IsAttacking)
            {
                DynelManager.LocalPlayer.StopAttack();
            }

            var savePath = $"{FilePath.ConfigFolderPath}\\{_fileName.Text}.json";

            GetData().Save(savePath);
            OnSave?.Invoke(savePath, true);
            AutomatonRoamba.MainWindow.UpdateActiveConfig(_fileName.Text);
            AutomatonRoamba.ConfigEditorWindow.Close();
        }

        private int GetValue(TextInputView textView)
        {
            return int.TryParse(textView.Text, out int range) ? range : 0;
        }

        public ConfigEditorConfig GetData()
        {
            ConfigEditorConfig pathingConfig = new ConfigEditorConfig
            {
                AttackMobs = _attackMobsCheckbox.IsChecked,
                UseTauntItem = _useTauntItemCheckbox.IsChecked,
                PathToMobs = _pathToMobsCheckbox.IsChecked,
                PathToCorpses = _pathToCorpsesCheckbox.IsChecked,
                TauntRange = GetValue(_tauntRangeTextView),
                PathRange = GetValue(_pathRangeTextView),
                WanderLimit = GetValue(_wanderLimitTextView),
                FightTimeoutPeriod = GetValue(_fightTimeoutTextView),
                LootTimeoutPeriod = GetValue(_lootTimeoutTextView),
                DisableIf = _disableIfCheckbox.IsChecked,
                DisableIfHp = _disableIfHpCheckbox.IsChecked,
                DisableIfNp = _disableIfNpCheckbox.IsChecked,
                DisableIfAttacked = _disableIfAttackedCheckbox.IsChecked,
                DisableIfPlayersNearby = _disableIfNearbyPlayersCheckBox.IsChecked,
                HealthPercent = GetValue(_hpPercentTextView),
                NanoPercent = GetValue(_npPercentTextView),
                FollowTarget = _followTargetCheckbox.IsChecked,
                FollowTargetName = _followTarget.Text,
                AttackRangeMode = _manualAttackRangeModeCheckbox.IsChecked ? AttackRangeMode.Manual: AttackRangeMode.Automatic,
                ManualModeAttackRange = _manualAttackRange,
                AutomaticModeAttackPadding = _automaticAttackPadding,
                AttackPriorityOnly = _priorityNamesOnlyCheckbox.IsChecked,

            };

            return pathingConfig;
        }

        public void SetData(ConfigEditorConfig config, string path)
        {
            Chat.WriteLine(path);
            _attackMobsCheckbox.SetValue(config.AttackMobs);
            _useTauntItemCheckbox.SetValue(config.UseTauntItem);
            _pathToMobsCheckbox.SetValue(config.PathToMobs);
            _pathToCorpsesCheckbox.SetValue(config.PathToCorpses);
            _tauntRangeTextView.Text = config.TauntRange.ToString();
            _pathRangeTextView.Text = config.PathRange.ToString();
            _wanderLimitTextView.Text = config.WanderLimit.ToString();
            _fightTimeoutTextView.Text = config.FightTimeoutPeriod.ToString();
            _lootTimeoutTextView.Text = config.LootTimeoutPeriod.ToString();
            _disableIfCheckbox.SetValue(config.DisableIf);
            _disableIfHpCheckbox.SetValue(config.DisableIfHp);
            _disableIfNpCheckbox.SetValue(config.DisableIfNp);
            _disableIfAttackedCheckbox.SetValue(config.DisableIfAttacked);
            _disableIfNearbyPlayersCheckBox.SetValue(config.DisableIfPlayersNearby);
            _hpPercentTextView.Text = config.HealthPercent.ToString();
            _npPercentTextView.Text = config.NanoPercent.ToString();
            _followTargetCheckbox.SetValue(config.FollowTarget);
            _followTarget.Text = config.FollowTargetName;
            _manualAttackRangeModeCheckbox.SetValue(config.AttackRangeMode == AttackRangeMode.Manual);
            _automaticAttackRangeModeCheckbox.SetValue(config.AttackRangeMode == AttackRangeMode.Automatic);
            _priorityNamesOnlyCheckbox.SetValue(config.AttackPriorityOnly);
            _fileName.Text = string.IsNullOrEmpty(path) ? "" : System.IO.Path.GetFileNameWithoutExtension(path);
            _manualAttackRange = config.ManualModeAttackRange;
            _automaticAttackPadding = config.AutomaticModeAttackPadding;
            _attackInput.Text = config.AttackRangeMode == AttackRangeMode.Manual ? config.ManualModeAttackRange.ToString() : config.AutomaticModeAttackPadding.ToString();
            _attackName.Text = config.AttackRangeMode == AttackRangeMode.Manual ? "Attack Range:" : "Padding:";
        }
    }
}