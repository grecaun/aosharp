using System;
using System.Runtime.InteropServices;
using AOSharp.Common.GameData.UI;
using AOSharp.Common.Unmanaged.Imports;
using AOSharp.Core.UI;

namespace ManagerHelp
{
    public static class Extensions
    {
        public static void SetStateColors(this Button button, uint fullColor = 0xffffff, uint raisedColor = 0xffffff, uint pressedColor = 0xffffff)
        {
            ButtonState[] buttonStates = new ButtonState[3] { ButtonState.Raised, ButtonState.Pressed, ButtonState.Hover };

            button.SetAlpha(2);

            foreach (var buttonState in buttonStates)
            {
                button.SetColorOverride(fullColor);

                if (buttonState == ButtonState.Raised)
                {
                    button.SetBorderColor(raisedColor, buttonState);
                }
                else if (buttonState == ButtonState.Pressed)
                {
                    button.SetBorderColor(pressedColor, buttonState);
                }
            }
        }

        [DllImport("GUI.dll", EntryPoint = "?GetBorderView@Button_c@@QAEPAVBorderView_c@@W4StateID_e@1@@Z", CallingConvention = CallingConvention.ThisCall)]
        public static extern IntPtr GetBorderView(IntPtr pThis, ButtonState buttonState);
        public static void SetBorderColor(this Button button, uint color, ButtonState state)
        {
            IntPtr ptr = GetBorderView(button.Pointer, state);

            if (ptr == IntPtr.Zero)
                return;

            View_c.SetColor(ptr, color);
        }
    }
}
