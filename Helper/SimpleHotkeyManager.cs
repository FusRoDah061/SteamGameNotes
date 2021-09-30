using log4net;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SteamGameNotes.Helper
{
    public static class SimpleHotkeyManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SimpleHotkeyManager));

        private static Dictionary<Key, Hotkey> _hotkeys = new Dictionary<Key, Hotkey>();
        private static KeyboardListener _keyboardListener;

        private static Key _lastKeyPressed = Key.None;

        public static void AddHotkey(Key key, EventHandler<EventArgs> handler)
        {
            AddHotkey(key, Key.None, handler);
        }

        public static void AddHotkey(Key key1, Key key2, EventHandler<EventArgs> handler)
        {
            if(_keyboardListener == null)
            {
                _keyboardListener = new KeyboardListener();
                _keyboardListener.OnKeyPressed += OnKeyboardKeyDown;

                _keyboardListener.HookKeyboard();
            }

            _hotkeys.Add(key1, new Hotkey(key1, key2, handler));
        }

        private static void OnKeyboardKeyDown(object sender, KeyPressedArgs e)
        {
            bool handled = false;

            try
            {

                if (_hotkeys.ContainsKey(e.KeyPressed))
                {
                    if (_hotkeys[e.KeyPressed].AdditionalKey.Equals(Key.None))
                    {
                        var hotkey = _hotkeys[e.KeyPressed];
                        hotkey.Handler(hotkey, new EventArgs());
                        handled = true;
                    }
                }

                if (_lastKeyPressed != Key.None && !handled)
                {
                    if (_hotkeys.ContainsKey(_lastKeyPressed))
                    {
                        var hotkey = _hotkeys[_lastKeyPressed];

                        if (hotkey.AdditionalKey != Key.None)
                        {
                            if (hotkey.AdditionalKey.Equals(e.KeyPressed))
                            {
                                hotkey.Handler(hotkey, new EventArgs());
                            }
                        }
                        else
                        {
                            hotkey.Handler(hotkey, new EventArgs());
                        }

                        _lastKeyPressed = Key.None;
                    }
                }  
            }
            catch (Exception ex)
            {
                log.Error("Error handling hotkey: ", ex);
            }
            finally
            {
                _lastKeyPressed = e.KeyPressed;
            }            
        }

        private class Hotkey
        {
            public Key MainKey { get; set; }

            public Key AdditionalKey { get; set; }

            public EventHandler<EventArgs> Handler { get; set; }

            public Hotkey(Key mainKey, Key additionalKey, EventHandler<EventArgs> handler)
            {
                MainKey = mainKey;
                AdditionalKey = additionalKey;
                Handler = handler;
            }
        }
    }
}
