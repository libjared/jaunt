//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SFML;
//using SFML.Graphics;
//using SFML.Window;
//using SFML.Audio;
//using Lidgren.Network;

//namespace Jaunt
//{
//    public class TextWriter
//    {
//        public String s = "";
//        public bool active = true;
//        public int maxChar = 250;
//        public int bsTimer = 0;

//        public TextWriter()
//        {

//        }

//        public void clear()
//        {
//            s = "";
//        }

//        private static void LoadContentInitialize()
//        {
//            window = new RenderWindow(
//                new VideoMode(640, 480), "new project!");

         

//            //window.TextEntered += TextEnteredHandler;

//            // ...
//        }

//        //private static void TextEnteredHandler(object sender, TextEventArgs e) {
//        //    mychatstring += e.Unicode;
//        //}

//        public void update()
//        {



//            if (Main.ks.IsKeyDown(Keys.Back) && Main.oks.IsKeyUp(Keys.Back) && active)
//            {
//                if (s.Length > 0)
//                    s = s.Remove(s.Length - 1);
//            }
//            if (Main.ks.IsKeyDown(Keys.Back))
//                bsTimer++;
//            else
//                bsTimer = 0;

//            if (bsTimer > 35)
//                if (s.Length > 0)
//                    s = s.Remove(s.Length - 1);

//            if (s.Length < maxChar && active)
//            {
//                if (Main.ks.IsKeyDown(Keys.A) && Main.oks.IsKeyUp(Keys.A))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "A" : "a");
//                }
//                if (Main.ks.IsKeyDown(Keys.B) && Main.oks.IsKeyUp(Keys.B))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "B" : "b");
//                }
//                if (Main.ks.IsKeyDown(Keys.C) && Main.oks.IsKeyUp(Keys.C))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "C" : "c");
//                }
//                if (Main.ks.IsKeyDown(Keys.D) && Main.oks.IsKeyUp(Keys.D))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "D" : "d");
//                }
//                if (Main.ks.IsKeyDown(Keys.E) && Main.oks.IsKeyUp(Keys.E))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "E" : "e");
//                }
//                if (Main.ks.IsKeyDown(Keys.F) && Main.oks.IsKeyUp(Keys.F))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "F" : "f");
//                }
//                if (Main.ks.IsKeyDown(Keys.G) && Main.oks.IsKeyUp(Keys.G))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "G" : "g");
//                }
//                if (Main.ks.IsKeyDown(Keys.H) && Main.oks.IsKeyUp(Keys.H))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "H" : "h");
//                }
//                if (Main.ks.IsKeyDown(Keys.I) && Main.oks.IsKeyUp(Keys.I))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "I" : "i");
//                }
//                if (Main.ks.IsKeyDown(Keys.J) && Main.oks.IsKeyUp(Keys.J))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "J" : "j");
//                }
//                if (Main.ks.IsKeyDown(Keys.K) && Main.oks.IsKeyUp(Keys.K))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "K" : "k");
//                }
//                if (Main.ks.IsKeyDown(Keys.L) && Main.oks.IsKeyUp(Keys.L))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "L" : "l");
//                }
//                if (Main.ks.IsKeyDown(Keys.M) && Main.oks.IsKeyUp(Keys.M))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "M" : "m");
//                }
//                if (Main.ks.IsKeyDown(Keys.N) && Main.oks.IsKeyUp(Keys.N))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "N" : "n");
//                }
//                if (Main.ks.IsKeyDown(Keys.O) && Main.oks.IsKeyUp(Keys.O))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "O" : "o");
//                }
//                if (Main.ks.IsKeyDown(Keys.P) && Main.oks.IsKeyUp(Keys.P))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "P" : "p");
//                }
//                if (Main.ks.IsKeyDown(Keys.Q) && Main.oks.IsKeyUp(Keys.Q))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "Q" : "q");
//                }
//                if (Main.ks.IsKeyDown(Keys.R) && Main.oks.IsKeyUp(Keys.R))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "R" : "r");
//                }
//                if (Main.ks.IsKeyDown(Keys.S) && Main.oks.IsKeyUp(Keys.S))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "S" : "s");
//                }
//                if (Main.ks.IsKeyDown(Keys.T) && Main.oks.IsKeyUp(Keys.T))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "T" : "t");
//                }
//                if (Main.ks.IsKeyDown(Keys.U) && Main.oks.IsKeyUp(Keys.U))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "U" : "u");
//                }
//                if (Main.ks.IsKeyDown(Keys.V) && Main.oks.IsKeyUp(Keys.V))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "V" : "v");
//                }
//                if (Main.ks.IsKeyDown(Keys.W) && Main.oks.IsKeyUp(Keys.W))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "W" : "w");
//                }
//                if (Main.ks.IsKeyDown(Keys.X) && Main.oks.IsKeyUp(Keys.X))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "X" : "x");
//                }
//                if (Main.ks.IsKeyDown(Keys.Y) && Main.oks.IsKeyUp(Keys.Y))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "Y" : "y");
//                }
//                if (Main.ks.IsKeyDown(Keys.Z) && Main.oks.IsKeyUp(Keys.Z))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "Z" : "z");
//                }
//                if (Main.ks.IsKeyDown(Keys.D1) && Main.oks.IsKeyUp(Keys.D1))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "!" : "1");
//                }
//                if (Main.ks.IsKeyDown(Keys.D2) && Main.oks.IsKeyUp(Keys.D2))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "@" : "2");
//                }
//                if (Main.ks.IsKeyDown(Keys.D3) && Main.oks.IsKeyUp(Keys.D3))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "#" : "3");
//                }
//                if (Main.ks.IsKeyDown(Keys.D4) && Main.oks.IsKeyUp(Keys.D4))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "$" : "4");
//                }
//                if (Main.ks.IsKeyDown(Keys.D5) && Main.oks.IsKeyUp(Keys.D5))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "%" : "5");
//                }
//                if (Main.ks.IsKeyDown(Keys.D6) && Main.oks.IsKeyUp(Keys.D6))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "^" : "6");
//                }
//                if (Main.ks.IsKeyDown(Keys.D7) && Main.oks.IsKeyUp(Keys.D7))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "&" : "7");
//                }
//                if (Main.ks.IsKeyDown(Keys.D8) && Main.oks.IsKeyUp(Keys.D8))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "*" : "8");
//                }
//                if (Main.ks.IsKeyDown(Keys.D9) && Main.oks.IsKeyUp(Keys.D9))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "(" : "9");
//                }
//                if (Main.ks.IsKeyDown(Keys.D0) && Main.oks.IsKeyUp(Keys.D0))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? ")" : "10");
//                }
//                if (Main.ks.IsKeyDown(Keys.OemQuestion) && Main.oks.IsKeyUp(Keys.OemQuestion))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? "?" : "/");
//                }
//                if (Main.ks.IsKeyDown(Keys.OemSemicolon) && Main.oks.IsKeyUp(Keys.OemSemicolon))
//                {
//                    s += (Main.ks.IsKeyDown(Keys.LeftShift) ? ":" : ";");
//                }

//                if (Main.ks.IsKeyDown(Keys.Space) && Main.oks.IsKeyUp(Keys.Space))
//                {
//                    s += " ";
//                }

//            }

//        }

//        public string getText()
//        {
//            return s;
//        }
//    }
//}
