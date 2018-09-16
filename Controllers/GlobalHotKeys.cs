using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DevSnip
{
    class GlobalHotKeys
    {
        /* FOR GLOBAL HOTKEY EXPLANATION SEE:
         * https://social.technet.microsoft.com/wiki/contents/articles/30568.wpf-implementing-global-hot-keys.aspx
         * 
         * NOTE: to use this in your main window you need to wait for sources to be initialized and the main window is opened.
         * eg.
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //register global hokey hooks
            GlobalHotKeys = new GlobalHotKeys();
            GlobalHotKeys.registerHotkeys(this);
        }
         * NOTE: Remember to remove the hotkey hooks when the window is closed!
         * eg.
        protected override void OnClosed(EventArgs e)
        {
            //remove the hotkey hooks
            GlobalHotKeys.removeHokeys();
            base.OnClosed(e);
        }
         */

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //(none)
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT
        private const uint MOD_WIN = 0x0008; //WINDOWS

        //other keys
        private const uint VK_OEM_3 = 0xC0; //For the US standard keyboard, the '`~' key 
        private const uint VK_CAPITAL = 0x14; //CAPS LOCK
       

        private IntPtr _windowHandle;
        private HwndSource _source;
        private Action callBackMethod;
        public bool registerHotkeys(Window window, Action callBackMethod)
        {
            this.callBackMethod = callBackMethod;

            _windowHandle = new WindowInteropHelper(window).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            var registered = RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_WIN, VK_OEM_3);
            if (!registered) MessageBox.Show("Failed to register hot key."+ Environment.NewLine +" The hotkey might already be in use by another application.");
            return registered;
        }

        //this is the callback executed when the hotkeys are presses.
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);

                            //if you end up registering multiple hotkeys and need to do different actions for each one.
                            if (vkey == VK_CAPITAL)
                            {
                                MessageBox.Show("CapsLock was pressed");
                            }
                            else
                            {
                                //MessageBox.Show("Some other key was pressed"+Environment.NewLine+"it works!");
                                callBackMethod();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        public void removeHokeys()
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
        }
    }
}
