using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;

using static System.Console;

namespace GameOfTanks
{
    /// <summary>
    /// Army including tank brigade and methods for battle manage
    /// </summary>
    [Serializable]
    class Army : ISerializable
    {
        public string ArmyName { get; set; }
        public List<Tank> TankBrigade;
        types type;
        /// <summary>
        /// Basic constructor for army
        /// </summary>
        /// Army type <see cref="types"/>
        /// (Sherman або PanteraType)
        /// </param>
        /// <exception cref="MyException">
        /// </exception>
        public Army(string Name, int Count, types t)
        {
            ArmyName = Name;
            type = t;
            try
            {
                if (Count < 0)
                    throw new MyException("Create army error. Negative count of units is impossible");
                else if (Count == 0)
                    throw new MyException("Create army error. Null count of units is impossible");
                TankBrigade = new List<Tank>();
                for (int i = 0; i < Count; i++)
                {

                    if (type == types.ShermanType)
                        TankBrigade.Add(new Sherman($"GreenUnit_{i}", 20, 7 + 7 * i));
                    else if (type == types.PanteraType)
                        TankBrigade.Add(new Pantera($"RedUnit_{i}", 79, 7 + 7 * i));
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
        private Army(SerializationInfo info, StreamingContext context)
        {
            ArmyName = info.GetString("Name");
            if (info.GetString("Type") == "ShermanType")
                type = types.ShermanType;
            else
                type = types.PanteraType;
            TankBrigade = new List<Tank>();
        }
        /// <summary>
        /// Print info about army
        /// </summary>
        public void Info()
        {
            ForegroundColor = ConsoleColor.White;
            WriteLine($"\tInfo about army *{ArmyName}*");
            WriteLine();
            if (type == types.ShermanType)
                ForegroundColor = ConsoleColor.Green;
            else
                ForegroundColor = ConsoleColor.Red;
            foreach (Tank t in TankBrigade)
                WriteLine(t);
            ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Serialization and saving army
        /// </summary>
        /// <param name="adress"> Path (directory) for saving</param>
        public void SerializingArmy(string adress)
        {
            try
            {
                SoapFormatter sf = new SoapFormatter();
                using (Stream s = File.Create($"{adress}\\ArmyInfo.soap"))
                {
                    sf.Serialize(s, this);
                }
                using (Stream s = File.Create($"{adress}\\Tanks.soap"))
                {
                    sf.Serialize(s, TankBrigade.ToArray());
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
        /// <summary>
        /// Static method for the start of battle. Two armies mooving on the battle position
        /// </summary>
        static public void ToPosition(Army arm1, Army arm2)
        {
            try
            {
                List<Tank> MixedList = new List<Tank>();
                MixedList.AddRange(arm1.TankBrigade);
                MixedList.AddRange(arm2.TankBrigade);
                MixedList = MixedList.OrderBy(x => Program.rand.Next()).ToList();

                Thread th = null;
                foreach (Tank t in MixedList)
                {
                    th = new Thread(new ThreadStart(t.Move));
                    th.Start();
                    Thread.Sleep(500);
                }
                th.Join();
            }
            catch (Exception e)
            {
                Clear();
                WriteLine(e);
                ReadKey();
                Clear();
            }
        }
        /// <summary>
        /// Static method for the battle manage
        /// </summary>
        /// <returns> Returns battle result <see cref="int"/>
        /// Return result of battle (-1 - first win, 0 - draw, 1 - victory for second)
        /// </returns>
        static public int Fight(Army a1, Army a2)
        {
            try
            {
                if (a1.TankBrigade.Count > a2.TankBrigade.Count)
                    throw new IntException("The first army outnumbers of the second army", -1);
                if (a2.TankBrigade.Count > a1.TankBrigade.Count)
                    throw new IntException("The second army outnumbers of the first army", -1);

                ToPosition(a1, a2);
                Dictionary<Tank, Tank> FightsPair = new Dictionary<Tank, Tank>();
                for (int i = 0; i < a1.TankBrigade.Count; i++)
                    FightsPair.Add(a1.TankBrigade[i], a2.TankBrigade[i]);
                List<KeyValuePair<Tank,Tank>> FightsQueq = FightsPair.OrderBy(x => Program.rand.Next()).ToList();
                FightsPair.Clear();

                SetCursorPosition(20, 3);
                ForegroundColor = ConsoleColor.Yellow;
                Write("Log:");
                ForegroundColor = ConsoleColor.White;
                Write(" Battle started");
                Thread.Sleep(2000);

                int pointLeft = 0, pointRight = 0;
                foreach (KeyValuePair<Tank,Tank> it in FightsQueq)
                {
                    SetCursorPosition(25, 3);
                    Write("                                ");
                    SetCursorPosition(25, 3);
                    switch(it.Key * it.Value)
                    {
                        case -1:
                            ForegroundColor = ConsoleColor.Green;
                            Write($" {it.Key.Name} ");
                            ForegroundColor = ConsoleColor.White;
                            Write(" destroy ");
                            ForegroundColor = ConsoleColor.Red;
                            Write($" {it.Value.Name}");
                            pointLeft++;
                            it.Value.Destroied();
                            break;
                        case 0:
                            ForegroundColor = ConsoleColor.White;
                            Write(" the duel beetween ");
                            ForegroundColor = ConsoleColor.Green;
                            Write($"{it.Key.Name}");
                            ForegroundColor = ConsoleColor.White;
                            Write(" and ");
                            ForegroundColor = ConsoleColor.Red;
                            Write($"{it.Value.Name}");
                            ForegroundColor = ConsoleColor.White;
                            Write(" is over without winner");
                            break;
                        case 1:
                            ForegroundColor = ConsoleColor.Red;
                            Write($" {it.Value.Name} ");
                            ForegroundColor = ConsoleColor.White;
                            Write(" destroy ");
                            ForegroundColor = ConsoleColor.Green;
                            Write($" {it.Key.Name}");
                            pointRight++;
                            it.Key.Destroied();
                            break;
                    }
                    Thread.Sleep(2000);
                }
                ForegroundColor = ConsoleColor.White;
                SetCursorPosition(25, 3);
                Write("                                ");
                SetCursorPosition(25, 3);
                if (pointLeft > pointRight)
                {
                    if (pointLeft < 4)
                        Write($"The tank battle is ovet by winner {a1.ArmyName}!");
                    else
                        Write($" Army {a1.ArmyName} defeated army {a2.ArmyName}!");
                    Program.WinsLeft++;
                    Program.SaveStat();
                    return -1;
                }
                else if (pointRight > pointLeft)
                {
                    if (pointRight < 4)
                        Write($"The tank battle is ovet by winner {a2.ArmyName}!");
                    else
                        Write($" Army {a1.ArmyName} was defeated by {a2.ArmyName}!");
                    Program.WinsRight++;
                    Program.SaveStat();
                    return 1;
                }
                else
                {
                    Write("Tank duel is over without winner!");
                    Program.Draw++;
                    Program.SaveStat();
                    return 0;
                }
            }
            catch (IntException i)
            {
                Clear();
                WriteLine(i.Message);
                ReadKey();
                Clear();
                return i.Return;
            }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", ArmyName);
            info.AddValue("Type", type.ToString());
        }
    }
}
