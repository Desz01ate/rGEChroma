using Colore;
using Colore.Effects.Keyboard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //Mana is not static variable/pointer so I can't find the offset for it.
        static readonly Key[][] charHPRows = new[] {
            new Key[] { Key.Q, Key.W, Key.E, Key.R, Key.T, Key.Y, Key.U, Key.I, Key.O, Key.P },
            new Key[] { Key.A, Key.S, Key.D, Key.F, Key.G, Key.H, Key.J, Key.K, Key.L, Key.OemSemicolon },
            new Key[] { Key.Z, Key.X, Key.C, Key.V, Key.B, Key.N, Key.M, Key.OemComma, Key.OemPeriod, Key.OemSlash }
        };
        static readonly ColoreColor fadePurple = new ColoreColor((byte)64, (byte)0, (byte)123);
        static readonly ColoreColor fadeRed = new ColoreColor((byte)32, (byte)0, (byte)0);
        static readonly ColoreColor fadeBlue = new ColoreColor((byte)0, (byte)0, (byte)32);
        static readonly ColoreColor fadeWhite = new ColoreColor((byte)32, (byte)32, (byte)32);
        static bool dangerousState = false;
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
        {
            string char1Name, char2Name, char3Name;
            int char1CurrentHP, char1MaxHP, char2CurrentHP, char2MaxHP, char3CurrentHP, char3MaxHP;
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
                    char1CurrentHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + CurrentHP[0]));
                    char1MaxHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + MaxHP[0]));
                    char2CurrentHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + CurrentHP[1]));
                    char2MaxHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + MaxHP[1]));
                    char3CurrentHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + CurrentHP[2]));
                    char3MaxHP = vam.ReadInt32((IntPtr)(vam.getBaseAddress + MaxHP[2]));
                    var HP1Point = Math.Round((double)char1CurrentHP / char1MaxHP, 2);
                    var HP2Point = Math.Round((double)char2CurrentHP / char2MaxHP, 2);
                    var HP3Point = Math.Round((double)char3CurrentHP / char3MaxHP, 2);
                    renderKeyboard(ref keyboard, (int)(HP1Point * 10), (int)(HP2Point * 10), (int)(HP3Point * 10));
                    await chroma.Keyboard.SetCustomAsync(keyboard);
                    //char1Name = vam.ReadStringASCII((IntPtr)(vam.getBaseAddress + Name[0]), bufferSize);
                    //char2Name = vam.ReadStringASCII((IntPtr)(vam.getBaseAddress + Name[1]), bufferSize);
                    //char3Name = vam.ReadStringASCII((IntPtr)(vam.getBaseAddress + Name[2]), bufferSize);
                    //char1Name = char1Name.Substring(0, char1Name.IndexOf("\0"));
                    //char2Name = char2Name.Substring(0, char2Name.IndexOf("\0"));
                    //char3Name = char3Name.Substring(0, char3Name.IndexOf("\0"));
                    //Console.WriteLine($"\rName : {char1Name} {char1CurrentHP}/{char1MaxHP}[{HP1Point * 100}]% ");
                    //Console.WriteLine($"\rName : {char2Name} {char2CurrentHP}/{char2MaxHP}[{HP2Point * 100}]% ");
                    //Console.WriteLine($"\rName : {char3Name} {char3CurrentHP}/{char3MaxHP}[{HP3Point * 100}]% ");
                    Thread.Sleep(500);
                }
            }

            catch
            {
                keyboard.Set(fadeRed);
                await chroma.Keyboard.SetCustomAsync(keyboard);
                Console.Write("\rRenaissance Granado Espada is not running...");
                Thread.Sleep(500);
                goto WaitGame;
            }
        }

        private static void renderKeyboard(ref KeyboardCustom keyboard, params int[] characterHPPoint)
        {
            keyboard.Clear();
            keyboard.Set(fadeRed);
            for (var i = 0; i < charHPRows[0].Length; i++)
            {
                keyboard[charHPRows[0][i]] = fadeWhite;
                keyboard[charHPRows[1][i]] = fadeWhite;
                keyboard[charHPRows[2][i]] = fadeWhite;
            }
            for (var charIndex = 0; charIndex < characterHPPoint.Length; charIndex++)
            {
                if (characterHPPoint[charIndex] > 0)
                {
                    ColoreColor stateColor = GetHPStateColor(characterHPPoint[charIndex]);
                    for (var i = 0; i < characterHPPoint[charIndex]; i++)
                        keyboard[charHPRows[charIndex][i]] = stateColor;
                }
            }
            dangerousState = !dangerousState;

        }

        private static ColoreColor GetHPStateColor(double hP1Point)
        {
            var stateColor = ColoreColor.Green;
            if (hP1Point < 5)
            {
                if (dangerousState)
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

