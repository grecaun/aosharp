using AOSharp.Core.UI;
using Shared;
using System;

namespace Buddy.Shared.UI
{
    public class BuddyCoreView : CustomView
    {
        internal TextInputView ChannelTextView;
        internal Checkbox AutoEnable;
        public Button EnabledButton;
        public TextView TitleTextView;
        public bool IsButtonEnabled;

        private Button _infoButton;
        private Button _closeButton;
        private View _infoView;
        private string _infoWindowPath;
        public Action OnCloseClick;

        public BuddyCoreView(View infoView, string coreViewPath, string infoWindowPath) : base(coreViewPath)
        {
            try
            {
                XmlPath = coreViewPath;
                _infoWindowPath = infoWindowPath;
                _infoView = infoView;

                if (Root.FindChild("Info", out _infoButton))
                {
                    _infoButton.Clicked = InfoButtonClick;
                }

                if (Root.FindChild("Close", out _closeButton))
                {
                    _closeButton.Clicked = CloseButtonClick;
                }

                if (Root.FindChild("Title", out TitleTextView))
                {
                }

                Root.FindChild("ChannelId", out ChannelTextView);
                Root.FindChild("OnInjectEnable", out AutoEnable);

                if (Root.FindChild("Enabled", out EnabledButton))
                {
                    EnabledButton.SetLabel("Start");
                    EnabledButton.Clicked += EnableClick;
                }
            }
            catch (Exception ex)
            {
                Chat.WriteLine(ex.Message);
            }
        }

        private void CloseButtonClick(object sender, ButtonBase e)
        {
            OnCloseClick?.Invoke();
        }

        private void EnableClick(object sender, ButtonBase e)
        {
            IsButtonEnabled = !IsButtonEnabled;
        }

        public void SetButtonState(bool state)
        {
            string label = state ? "Stop" : "Start";
            (EnabledButton).SetLabel(label);
        }

        public BuddyCoreConfig GetData()
        {
            BuddyCoreConfig config = new BuddyCoreConfig();

            config.OnInjectEnable = AutoEnable.IsChecked;

            if (byte.TryParse(ChannelTextView.Text, out byte result))
                config.ChannelId = result;
            else
                config.ChannelId = 0;

            return config;
        }

        public void SetData(BuddyCoreConfig config)
        {
            ChannelTextView.Text = config.ChannelId.ToString();
            AutoEnable.SetValue(config.OnInjectEnable);
        }

        public void SetChannelId(int id)
        {
            ChannelTextView.Text = id.ToString();   
        }

        public void SetAutoEnable(bool state)
        {
            AutoEnable.SetValue(state);
        }

        private void InfoButtonClick(object sender, ButtonBase e)
        {
            InfoWindow infoWindow = new InfoWindow("BuddyCoreInfoView", _infoWindowPath, _infoView);
            infoWindow.Show();
        }
    }
}