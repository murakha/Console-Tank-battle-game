using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml;

using static System.Console;

namespace GameOfTanks
{
    class Program
    {
        public static Random rand = new Random();
        private static Army Left;
        private static Army Right;
        public static int WinsLeft, WinsRight, Draw;
        static void Main(string[] arg)
        {
            SetWindowSize(100, 40);
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
            LoadStat();
            CreateArmies(true);
            string[] menu = { "Create armies", "View armies", "Start battle", "Stat" };
            bool work = true;
            int choose = 0;

            for (int i = 0; i < menu.Length; i++)
            {
                SetCursorPosition(10, 7 + 2 * i);
                Write(menu[i]);
            }

            while (work)
            {
                ForegroundColor = ConsoleColor.Magenta;
                SetCursorPosition(10, 7 + 2 * choose);
                Write(menu[choose]);
                ForegroundColor = ConsoleColor.White;

                switch (ReadKey().Key)
                {
                    case ConsoleKey.Escape:
                        work = false;
                        break;
                    case ConsoleKey.UpArrow:
                        SetCursorPosition(10, 7 + 2 * choose);
                        Write(menu[choose]);
                        if (choose > 0)
                            choose--;
                        else
                            choose = menu.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        SetCursorPosition(10, 7 + 2 * choose);
                        Write(menu[choose]);
                        if (choose < menu.Length - 1)
                            choose++;
                        else
                            choose = 0;
                        break;
                    case ConsoleKey.Enter:
                        switch (choose)
                        {
                            case 0:
                                CreateArmies();
                                break;
                            case 1:
                                ShowArmies();
                                break;
                            case 2:
                                Clear();
                                Army.Fight(Left, Right);
                                ReadKey();
                                Clear();
                                break;
                            case 3:
                                ShowStat();
                                break;
                        }
                        ForegroundColor = ConsoleColor.White;
                        for (int i = 0; i < menu.Length; i++)
                        {
                            SetCursorPosition(10, 7 + 2 * i);
                            Write(menu[i]);
                        }
                        break;
                }
            }
            Clear();
        }

        static void LoadStat()
        {
            if (File.Exists("Stat.xml"))
            {

                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load("Stat.xml");
                    WinsLeft = Int32.Parse(doc.ChildNodes.Item(1).ChildNodes.Item(0).InnerText);
                    WinsRight = Int32.Parse(doc.ChildNodes.Item(1).ChildNodes.Item(1).InnerText);
                    Draw = Int32.Parse(doc.ChildNodes.Item(1).ChildNodes.Item(2).InnerText);
                }
                catch (Exception e)
                {
                    WriteLine(e);
                    WriteLine("Stat file read error");
                    ReadKey();
                    WinsLeft = 0;
                    WinsRight = 0;
                    Draw = 0;
                    SaveStat();
                    Clear();
                }
            }
            else
            {
                WinsLeft = 0;
                WinsRight = 0;
                Draw = 0;
                SaveStat();
            }
        }
        public static void SaveStat()
        {
            XmlTextWriter wr = null;
            try
            {
                wr = new XmlTextWriter("Stat.xml", System.Text.Encoding.Unicode);
                wr.Formatting = Formatting.Indented;
                wr.WriteStartDocument();
                wr.WriteStartElement("Stat");
                wr.WriteElementString("Left", WinsLeft.ToString());
                wr.WriteElementString("Right", WinsRight.ToString());
                wr.WriteElementString("Draw", Draw.ToString());
                wr.WriteEndElement();
            }
            catch (Exception e)
            {
                Clear();
                WriteLine("Stat file write error");
                ReadKey();
            }
            finally
            {
                if (wr != null)
                    wr.Close();
            }
        }
        static void CreateArmies(bool CatchSave = false)
        {
            if (CatchSave == true)
            {
                try
                {
                    Directory.CreateDirectory("Saves");
                    Directory.CreateDirectory(@"Saves\Left");
                    Directory.CreateDirectory(@"Saves\Right");
                    SoapFormatter sf = new SoapFormatter();
                    if (Directory.GetFiles(@"Saves\Left").Length != 0)
                    {
                        using (Stream s = File.OpenRead(@"Saves\Left\ArmyInfo.soap"))
                        {
                            Left = (Army)sf.Deserialize(s);
                        }
                        using (Stream s = File.OpenRead(@"Saves\Left\Tanks.soap"))
                        {
                            Tank[] arr = (Tank[])sf.Deserialize(s);
                            Left.TankBrigade.AddRange(arr);
                        }
                    }
                    else
                    {
                        Left = new Army("Left", 5, types.ShermanType);
                        Left.SerializingArmy(@"Saves\Left");
                    }
                    if (Directory.GetFiles(@"Saves\Right").Length != 0)
                    {
                        using (Stream s = File.OpenRead(@"Saves\Right\ArmyInfo.soap"))
                        {
                            Right = (Army)sf.Deserialize(s);
                        }
                        using (Stream s = File.OpenRead(@"Saves\Right\Tanks.soap"))
                        {
                            Tank[] arr = (Tank[])sf.Deserialize(s);
                            Right.TankBrigade.AddRange(arr);
                        }
                    }
                    else
                    {
                        Right = new Army("Right", 5, types.PanteraType);
                        Right.SerializingArmy(@"Saves\Right");
                    }
                }
                catch (Exception e)
                {
                    Clear();
                    WriteLine(e);
                    ReadKey();
                    Clear();
                }
            }
            else
            {
                Left = new Army("Left", 5, types.ShermanType);
                Right = new Army("Right", 5, types.PanteraType);
                Left.SerializingArmy(@"Saves\Left");
                Right.SerializingArmy(@"Saves\Right");
                Clear();
                WriteLine("\tNew armies created");
                ReadKey();
                Clear();
            }
        }
        static void ShowArmies()
        {
            Clear();
            SetCursorPosition(0, 3);
            Left.Info();
            WriteLine();
            Right.Info();
            ReadKey();
            Clear();
        }

        static void ShowStat()
        {
            Clear();
            SetCursorPosition(10, 3);
            ForegroundColor = ConsoleColor.White;
            Write("Left army's victories: ");
            ForegroundColor = ConsoleColor.Green;
            Write($"{WinsLeft}");
            SetCursorPosition(10, 5);
            ForegroundColor = ConsoleColor.White;
            Write("Right army's victories: ");
            ForegroundColor = ConsoleColor.Red;
            Write($"{WinsRight}");
            SetCursorPosition(10, 7);
            ForegroundColor = ConsoleColor.White;
            Write("Battles that didn't find a winner: ");
            ForegroundColor = ConsoleColor.Yellow;
            Write($"{Draw}");
            ReadKey();
            Clear();
        }
    }
}
