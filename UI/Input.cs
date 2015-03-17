using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if WINDOWS8
using System.Linq;
#else
using System.Runtime.InteropServices;
#endif

namespace XNA8DFramework.UI
{
#if WINDOWS
    public class CharacterEventArgs : EventArgs
    {
        private readonly char _character;
#if !WINDOWS8
        private readonly int _lParam;
#endif
        public CharacterEventArgs(char character
#if !WINDOWS8
            , int lParam
#endif
            )
        {
            _character = character;
#if !WINDOWS8
            _lParam = lParam;
#endif
        }
        public char Character
        {
            get
            {
                return _character;
            }
        }
#if !WINDOWS8
        public int Param
        {
            get
            {
                return _lParam;
            }
        }
        public int RepeatCount
        {
            get
            {
                return _lParam & 0xffff;
            }
        }
        public bool ExtendedKey
        {
            get
            {
                return (_lParam & (1 << 24)) > 0;
            }
        }
        public bool AltPressed
        {
            get
            {
                return (_lParam & (1 << 29)) > 0;
            }
        }
        public bool PreviousState
        {
            get
            {
                return (_lParam & (1 << 30)) > 0;
            }
        }
        public bool TransitionState
        {
            get
            {
                return (_lParam & (1 << 31)) > 0;
            }
        }
#endif
    }
    public class KeyEventArgs : EventArgs
    {
        private readonly Keys _keyCode;
        public KeyEventArgs(Keys keyCode)
        {
            _keyCode = keyCode;
        }
        public Keys KeyCode
        {
            get
            {
                return _keyCode;
            }
        }
    }
    public static class EventInput
    {
        /// <summary>		
        /// Event raised when a character has been entered.		
        /// </summary>		
        public static event EventHandler<CharacterEventArgs> CharEntered;
        /// <summary>		
        /// Event raised when a key has been pressed down. May fire multiple times due to keyboard repeat.		
        /// </summary>		
        public static event EventHandler<KeyEventArgs> KeyDown;
        /// <summary>		
        /// Event raised when a key has been released.		
        /// </summary>		
        public static event EventHandler<KeyEventArgs> KeyUp;

        static bool _initialized;
#if WINDOWS8
        static KeyboardState _oldkbs;
#else
        delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        static IntPtr _prevWndProc;
        static WndProc _hookProcDelegate;
        static IntPtr _hImc;
        //various Win32 constants that we need		
        const int GwlWndproc = -4;
        const int WmKeydown = 0x100;
        const int WmKeyup = 0x101;
        const int WmChar = 0x102;
        const int WmImeSetcontext = 0x0281;
        const int WmInputlangchange = 0x51;
        const int WmGetdlgcode = 0x87;
        //const int WM_IME_COMPOSITION = 0x10f;
        const int DlgcWantallkeys = 4;
        //Win32 functions that we're using		
        [DllImport("Imm32.dll")]
        static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hImc);
        [DllImport("user32.dll")]
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
#endif
        /// <summary>		
        /// Initialize the TextInput with the given GameWindow.		
        /// </summary>		
        /// <param name="game">The XNA game to which text input should be linked.</param>		
        public static void Initialize(Game game)
        {
            if (_initialized)
                throw new InvalidOperationException("TextInput.Initialize can only be called once!");

#if WINDOWS8
            _oldkbs = Keyboard.GetState();
#else
            GameWindow window = game.Window;
            _hookProcDelegate = HookProc;
            _prevWndProc = (IntPtr)SetWindowLong(window.Handle, GwlWndproc, (int)Marshal.GetFunctionPointerForDelegate(_hookProcDelegate));
            _hImc = ImmGetContext(window.Handle);
#endif
            _initialized = true;
        }
#if WINDOWS8
        public static void Update(GameTime gameTime)
        {
            KeyboardState kbs = Keyboard.GetState();

            var keys = kbs.GetPressedKeys().Concat(_oldkbs.GetPressedKeys());

            var keysList = keys as IList<Keys> ?? keys.ToList();

            bool shiftPressed = keysList.Contains(Keys.LeftShift) || keysList.Contains(Keys.RightShift);

            foreach (Keys key in keysList)
            {
                if (kbs.IsKeyDown(key) && _oldkbs.IsKeyUp(key))
                {
                    if (KeyDown != null)
                        KeyDown(null, new KeyEventArgs(key));
                }
                if (_oldkbs.IsKeyDown(key) && kbs.IsKeyUp(key))
                {
                    if (KeyUp != null)
                        KeyUp(null, new KeyEventArgs(key));
                    if (CharEntered != null)
                    {
                        char ch;
                        switch (key)
                        {
                            case Keys.A:
                            case Keys.B:
                            case Keys.C:
                            case Keys.D:
                            case Keys.E:
                            case Keys.F:
                            case Keys.G:
                            case Keys.H:
                            case Keys.I:
                            case Keys.J:
                            case Keys.K:
                            case Keys.L:
                            case Keys.M:
                            case Keys.N:
                            case Keys.O:
                            case Keys.P:
                            case Keys.Q:
                            case Keys.R:
                            case Keys.S:
                            case Keys.T:
                            case Keys.U:
                            case Keys.V:
                            case Keys.W:
                            case Keys.X:
                            case Keys.Y:
                            case Keys.Z:
                                ch = (char)key;
                                if (!shiftPressed)
                                {
                                    ch += (char)('a' - 'A');
                                }
                                CharEntered(null, new CharacterEventArgs(ch));
                                break;
                            case Keys.NumPad0:
                            case Keys.NumPad1:
                            case Keys.NumPad2:
                            case Keys.NumPad3:
                            case Keys.NumPad4:
                            case Keys.NumPad5:
                            case Keys.NumPad6:
                            case Keys.NumPad7:
                            case Keys.NumPad8:
                            case Keys.NumPad9:
                                ch = (char)('0' + key - (int)Keys.NumPad0);
                                CharEntered(null, new CharacterEventArgs(ch));
                                break;
                            case Keys.D0:
                                HandleSpecialDigit(shiftPressed, key, ')');
                                break;
                            case Keys.D1:
                                HandleSpecialDigit(shiftPressed, key, '!');
                                break;
                            case Keys.D2:
                                HandleSpecialDigit(shiftPressed, key, '@');
                                break;
                            case Keys.D3:
                                HandleSpecialDigit(shiftPressed, key, '#');
                                break;
                            case Keys.D4:
                                HandleSpecialDigit(shiftPressed, key, '$');
                                break;
                            case Keys.D5:
                                HandleSpecialDigit(shiftPressed, key, '%');
                                break;
                            case Keys.D6:
                                HandleDigit(key);
                                break;
                            case Keys.D7:
                                HandleSpecialDigit(shiftPressed, key, '&');
                                break;
                            case Keys.D8:
                                HandleSpecialDigit(shiftPressed, key, '*');
                                break;
                            case Keys.D9:
                                HandleSpecialDigit(shiftPressed, key, '(');
                                break;
                            case Keys.OemPlus:
                                HandleSpecialCharacter(shiftPressed, '=', '+');
                                break;
                            case Keys.OemComma:
                                HandleSpecialCharacter(shiftPressed, ',', '<');
                                break;
                            case Keys.OemMinus:
                                HandleSpecialCharacter(shiftPressed, '-', '_');
                                break;
                            case Keys.OemQuotes:
                                HandleSpecialCharacter(shiftPressed, '"', '\'');
                                break;
                            case Keys.OemQuestion:
                                //HandleSpecialCharacter(shiftPressed, '/', '?');
                                HandleSpecialCharacter(shiftPressed, ';', ':');
                                break;
                            case Keys.OemTilde:
                                HandleSpecialCharacter(shiftPressed, '`', '~');
                                break;
                            case Keys.OemCloseBrackets:
                                HandleSpecialCharacter(shiftPressed, ']', '}');
                                break;
                            case Keys.OemOpenBrackets:
                                HandleSpecialCharacter(shiftPressed, '[', '{');
                                break;
                            case Keys.OemPipe:
                                HandleSpecialCharacter(shiftPressed, '\\', '|');
                                break;
                            case Keys.OemPeriod:
                                HandleSpecialCharacter(shiftPressed, '.', '>');
                                break;
                            //^ ? /
                            case Keys.Enter:
                            case Keys.Space:
                                CharEntered(null, new CharacterEventArgs((char)key));
                                break;
                        }
                    }
                }
            }

            _oldkbs = kbs;
        }

        private static void HandleSpecialDigit(bool shiftPressed, Keys key, char specialChar)
        {
            if (shiftPressed)
                CharEntered(null, new CharacterEventArgs(specialChar));
            else
                HandleDigit(key);
        }

        private static void HandleSpecialCharacter(bool shiftPressed, char specialChar1, char specialChar2)
        {
            CharEntered(null, new CharacterEventArgs(shiftPressed ? specialChar2 : specialChar1));
        }

        private static void HandleDigit(Keys key)
        {
            CharEntered(null, new CharacterEventArgs((char) ('0' + key - (int) Keys.D0)));
        }
#else
        static IntPtr HookProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr returnCode = CallWindowProc(_prevWndProc, hWnd, msg, wParam, lParam);
            switch (msg)
            {
                case WmGetdlgcode:
                    returnCode = (IntPtr)(returnCode.ToInt32() | DlgcWantallkeys); break;
                case WmKeydown:
                    if (KeyDown != null)
                        KeyDown(null, new KeyEventArgs((Keys)wParam));
                    break;
                case WmKeyup:
                    if (KeyUp != null)
                        KeyUp(null, new KeyEventArgs((Keys)wParam));
                    break;
                case WmChar:
                    if (CharEntered != null)
                        CharEntered(null, new CharacterEventArgs((char)wParam, lParam.ToInt32()));
                    break;
                case WmImeSetcontext:
                    if (wParam.ToInt32() == 1)
                        ImmAssociateContext(hWnd, _hImc);
                    break;
                case WmInputlangchange:
                    ImmAssociateContext(hWnd, _hImc);
                    returnCode = (IntPtr)1;
                    break;
            }
            return returnCode;
        }
#endif
    }
#endif
}