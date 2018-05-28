using Colore;
using Colore.Effects.Keyboard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ColoreColor = Colore.Data.Color;
namespace RenaissanceGEChroma
{
    class Program
    {
        static readonly int[] Name = new[] { 0x54F150, 0x54F307, 0x54F4BE };
        static readonly int[] CurrentHP = new[] { 0x54F244, 0x54F3FB, 0x54F5B2 };
        static readonly int[] MaxHP = new[] { 0x54F248, 0x54F3FF, 0x54F5B6 };
        static readonly int[] CurrentMP = new[] { 0x54F24C, 0x54F403, 0x54F5BA };
        static readonly int[] MaxMP = new[] { 0x54F24E, 0x54F405, 0x54F5BC };
        static readonly int FamLevel = 0x558200;
        static readonly Key[][] charRenderGrid = new[] {
            new Key[] { Key.Q, Key.W, Key.E, Key.R, Key.T, Key.Y, Key.U, Key.I, Key.O, Key.P },
            new Key[] { Key.A, Key.S, Key.D, Key.F, Key.G, Key.H, Key.J, Key.K, Key.L, Key.OemSemicolon },
            new Key[] { Key.Z, Key.X, Key.C, Key.V, Key.B, Key.N, Key.M, Key.OemComma, Key.OemPeriod, Key.OemSlash }
        };
        /*
        static readonly Key[][] charHPRows = new[] {
            new Key[] { Key.Q, Key.W, Key.E, Key.R, Key.T,  },
            new Key[] { Key.A, Key.S, Key.D, Key.F, Key.G,  },
            new Key[] { Key.Z, Key.X, Key.C, Key.V, Key.B,  }
        };
        */
        /*
        static readonly Key[][] charMPRows = new[] {
            new Key[] { Key.P, Key.O, Key.I, Key.U, Key.Y },
            new Key[] { Key.OemSemicolon,Key.L, Key.K, Key.J, Key.H },
            new Key[] { Key.OemSlash, Key.OemPeriod, Key.OemComma,  Key.M, Key.N}
        };
        */
        static readonly Key[] NumpadKeys = new Key[] { Key.Num0, Key.Num1, Key.Num2, Key.Num3, Key.Num4, Key.Num5, Key.Num6, Key.Num7, Key.Num8, Key.Num9 };
        static readonly Key[] FunctionKeys = new Key[] { Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12 };
        static readonly ColoreColor fadePurple = new ColoreColor((byte)64, (byte)0, (byte)123);
        static readonly ColoreColor fadeRed = new ColoreColor((byte)24, (byte)0, (byte)0);
        static readonly ColoreColor fadeBlue = new ColoreColor((byte)0, (byte)0, (byte)32);
        static readonly ColoreColor fadeWhite = new ColoreColor((byte)32, (byte)32, (byte)32);
        static readonly ColoreColor contrastOrange = new ColoreColor((byte)255, (byte)40, 0);
        static bool dangerousState = false;
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
        {
            string char1Name, char2Name, char3Name;
            int char1CurrentHP, char1MaxHP, char1CurrentMP, char1MaxMP, char2CurrentHP, char2MaxHP, char2CurrentMP, char2MaxMP, char3CurrentHP, char3MaxHP, char3CurrentMP, char3MaxMP;
            int familyLevel = 0;
            uint bufferSize = 255;
            var keyboard = KeyboardCustom.Create();
            var chroma = await ColoreProvider.CreateNativeAsync();
            await chroma.SetAllAsync(fadeRed);
            WaitGame:
            try
            {
                Process GameProcess = Process.GetProcessesByName("rGE").FirstOrDefault();
                VAMemory vam = new VAMemory("rGE");
                vam.ReadInt32(GameProcess.MainModule.BaseAddress);
                while (true)
                {
                    Console.Clear();
                    //HP
                    char1CurrentHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + CurrentHP[0]));
                    char1MaxHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + MaxHP[0]));
                    char2CurrentHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + CurrentHP[1]));
                    char2MaxHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + MaxHP[1]));
                    char3CurrentHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + CurrentHP[2]));
                    char3MaxHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + MaxHP[2]));
                    //MP
                    char1CurrentMP = vam.ReadInt16((IntPtr)(vam.getBaseAddress + CurrentMP[0]));
                    char1MaxMP = vam.ReadInt16((IntPtr)(vam.getBaseAddress + MaxMP[0]));
                    char2CurrentMP = vam.ReadInt16((IntPtr)(vam.getBaseAddress + CurrentMP[1]));
                    char2MaxMP = vam.ReadInt16((IntPtr)(vam.getBaseAddress + MaxMP[1]));
                    char3CurrentMP = vam.ReadInt16((IntPtr)(vam.getBaseAddress + CurrentMP[2]));
                    char3MaxMP = vam.ReadInt16((IntPtr)(vam.getBaseAddress + MaxMP[2]));

                    var HP1Point = Math.Round((double)char1CurrentHP / char1MaxHP, 2);
                    var HP2Point = Math.Round((double)char2CurrentHP / char2MaxHP, 2);
                    var HP3Point = Math.Round((double)char3CurrentHP / char3MaxHP, 2);
                    var MP1Point = Math.Round((double)char1CurrentMP / char1MaxMP, 2);
                    var MP2Point = Math.Round((double)char2CurrentMP / char2MaxMP, 2);
                    var MP3Point = Math.Round((double)char3CurrentMP / char3MaxMP, 2);
                    renderKeyboard(ref keyboard, new[] { (int)(HP1Point * 10), (int)(HP2Point * 10), (int)(HP3Point * 10) }, new[] { (int)(MP1Point * 10), (int)(MP2Point * 10), (int)(MP3Point * 10) });
                    await chroma.Keyboard.SetCustomAsync(keyboard);
                    //char1Name = vam.ReadStringASCII((IntPtr)(vam.getBaseAddress + Name[0]), bufferSize);
                    //char2Name = vam.ReadStringASCII((IntPtr)(vam.getBaseAddress + Name[1]), bufferSize);
                    //char3Name = vam.ReadStringASCII((IntPtr)(vam.getBaseAddress + Name[2]), bufferSize);
                    //char1Name = char1Name.Substring(0, char1Name.IndexOf("\0"));
                    //char2Name = char2Name.Substring(0, char2Name.IndexOf("\0"));
                    //char3Name = char3Name.Substring(0, char3Name.IndexOf("\0"));
                    //Console.WriteLine($"\rName : {char1Name} {char1CurrentMP}/{char1MaxMP}[{HP1Point * 100}]% ");
                    //Console.WriteLine($"\rName : {char2Name} {char2CurrentMP}/{char2MaxMP}[{HP2Point * 100}]% ");
                    //Console.WriteLine($"\rName : {char3Name} {char3CurrentMP}/{char3MaxMP}[{HP3Point * 100}]% ");
                    Thread.Sleep(500);
                }
            }

            catch
            {
                keyboard.Set(fadeRed);
                await chroma.Keyboard.SetCustomAsync(keyboard);
                Console.Write("\rWaiting for character info to allocate into memory...");
                Thread.Sleep(500);
                goto WaitGame;
            }
        }

        private static void renderKeyboard(ref KeyboardCustom keyboard, int[] characterHPPoint, int[] characterMPPoint)
        {
            //reset
            keyboard.Set(fadeRed);
            var currentTime = DateTime.Now;
            var am_pm = currentTime.ToString("tt", CultureInfo.InvariantCulture);
            keyboard[FunctionKeys[currentTime.Hour == 0 ? 11 : (currentTime.Hour % 12) - 1]] = am_pm == "AM" ? ColoreColor.White : ColoreColor.Blue;
            var decMin = (currentTime.Minute - (currentTime.Minute % 10)) / 10;
            keyboard[NumpadKeys[decMin]] = contrastOrange;
            keyboard[NumpadKeys[currentTime.Minute % 10]] = ColoreColor.Yellow;
            for (var i = 0; i < charRenderGrid[0].Length; i++)
            {
                keyboard[charRenderGrid[0][i]] = fadeWhite;
                keyboard[charRenderGrid[1][i]] = fadeWhite;
                keyboard[charRenderGrid[2][i]] = fadeWhite;
                //keyboard[charMPRows[0][i]] = fadeWhite;
                //keyboard[charMPRows[1][i]] = fadeWhite;
                //keyboard[charMPRows[2][i]] = fadeWhite;
            }
            for (var charIndex = 0; charIndex < characterHPPoint.Length; charIndex++)
            {
                ColoreColor stateColor = GetHPStateColor(characterHPPoint[charIndex]);
                for (var i = 0; i < characterMPPoint[charIndex]; i++)
                    keyboard[charRenderGrid[charIndex][i]] = ColoreColor.Blue;
                for (var i = 0; i < characterHPPoint[charIndex]; i++)
                    keyboard[charRenderGrid[charIndex][i]] = stateColor;

            }
            dangerousState = !dangerousState;

        }

        private static ColoreColor GetHPStateColor(double hp)
        {
            var stateColor = ColoreColor.Green;
            if (hp < 7)
            {
                stateColor = ColoreColor.Yellow;
            }
            if (hp < 5)
            {
                if (dangerousState) //RED-BLACK blinking
                {
                    stateColor = ColoreColor.Red;
                }
                else
                {
                    stateColor = ColoreColor.Black;
                }
            }
            return stateColor;
        }
    }
}

