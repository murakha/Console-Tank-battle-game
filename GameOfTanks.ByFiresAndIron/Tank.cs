using System;
using System.Threading;
using System.Runtime.Serialization;

using static System.Console;

namespace GameOfTanks
{
    public enum types { ShermanType, PanteraType };
    /// <summary>
    /// Basic abstract class Tank
    /// </summary>
    [Serializable]
    abstract class Tank : ISerializable
    {
        /// <summary>
        /// Tank coord (top left point - for the left army, top right - for the right)
        /// </summary>
        public int coordX, coordY;
        public string Name { get; set; }
        public double Shells { get; set; }
        public double Armor { get; set; }
        public double Maneuve { get; set; }
        /// <summary>
        /// Mutex variable for setting battlefieald
        /// </summary>
        protected static Mutex mute = new Mutex();
        /// <summary>
        /// Operator * are using for both tanks
        /// </summary>
        /// <param name="t1"> First tank </param>
        /// <param name="t2"> Second tank </param>
        /// <returns> Return result of battle (-1 - first win, 0 - draw, 1 - victory for second)</returns>
        static public int operator *(Tank t1, Tank t2)
        {
            int score = 0;
            if (t1.Shells > t2.Shells)
                score++;
            if (t1.Armor > t2.Armor)
                score++;
            if (t1.Maneuve > t2.Maneuve)
                score++;

            if (score >= 2)
                return -1;
            if ((t1.Shells == t2.Shells && t1.Armor == t2.Armor)
                || (t1.Shells == t2.Shells && t1.Maneuve == t2.Maneuve)
                || (t1.Maneuve == t2.Maneuve && t1.Armor == t2.Armor))
                return 0;
            return 1;
        }
        /// <summary>
        /// Basic abstract class Tank
        /// </summary>
        public Tank(string Name, int x, int y)
        {
            this.Name = Name;
            Shells = Program.rand.Next(10, 101) + Program.rand.Next(0, 100) * 0.01;
            Armor = Program.rand.Next(10, 101) + Program.rand.Next(0, 100) * 0.01;
            Maneuve = Program.rand.Next(10, 101) + Program.rand.Next(0, 100) * 0.01;
            coordX = x;
            coordY = y;
        }
        protected Tank(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Shells = info.GetDouble("Shells");
            Armor = info.GetDouble("Armor");
            Maneuve = info.GetDouble("Maneuve");
            coordX = info.GetInt32("CoordX");
            coordY = info.GetInt32("CoordY");
        }
        /// <summary>
        /// Print tant on the battlefield
        /// </summary>
        /// <param name="x"> The real Х-coord of picter </param>
        /// <param name="y"> The real Y-coord of picter </param>
        public abstract void Print(int x, int y);
        /// <summary>
        /// Print destroyed tank
        /// </summary>
        public abstract void Destroied();
        /// <summary>
        /// Mooving tank on the battle position
        /// </summary>
        public abstract void Move();
        public override string ToString()
        {
            return $" Name: {Name}\tShells => {Shells}\tArmor => {Armor}\t Maneuve => {Maneuve}";
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Shells", Shells);
            info.AddValue("Armor", Armor);
            info.AddValue("Maneuve", Maneuve);
            info.AddValue("CoordX", coordX);
            info.AddValue("CoordY", coordY);
        }
    }
    /// <summary>
    /// "Sherman" type tank
    /// </summary>
    [Serializable]
    class Sherman : Tank, ISerializable
    {
        public Sherman(string Name, int x, int y) : base(Name, x, y)
        { }
        private Sherman(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
        /// <summary>
        /// Print tant on the battlefield
        /// </summary>
        /// <param name="x"> The real Х-coord of picter </param>
        /// <param name="y"> The real Y-coord of picter </param>
        public override void Print(int x, int y)
        {
            ForegroundColor = ConsoleColor.Green;
            SetCursorPosition(x, y);
            Write(" ****");
            SetCursorPosition(x, y + 1);
            Write("######");
            SetCursorPosition(x, y + 2);
            Write("#####====");
            SetCursorPosition(x, y + 3);
            Write("######");
            SetCursorPosition(x, y + 4);
            WriteLine(" ****");
            ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Print destroyed tank
        /// </summary>
        public override void Destroied()
        {
            ForegroundColor = ConsoleColor.Yellow;
            SetCursorPosition(coordX + 2, coordY);
            Write("&&");
            SetCursorPosition(coordX + 1, coordY + 1);
            Write("&&&&");
            SetCursorPosition(coordX, coordY + 2);
            Write("&&&&&&");
            SetCursorPosition(coordX + 1, coordY + 3);
            Write("&&&&");
            SetCursorPosition(coordX + 2, coordY + 4);
            Write("&&");
        }
        /// <summary>
        /// Mooving tank on the battle position
        /// </summary>
        public override void Move()
        {
            int realX = 0;
            mute.WaitOne();
            Print(realX, coordY);
            mute.ReleaseMutex();
            while (realX != coordX)
            {
                mute.WaitOne();
                for (int i = 0; i < 5; i++)
                {
                    SetCursorPosition(realX, coordY + i);
                    Write(" ");
                }
                realX++;
                Print(realX, coordY);
                mute.ReleaseMutex();
                Thread.Sleep(200);
            }
        }
        public override string ToString()
        {
            return " Type: Sherman\t" + base.ToString();
        }
    }
    /// <summary>
    /// "Pantera" type tank
    /// </summary>
    [Serializable]
    class Pantera : Tank, ISerializable
    {
        public Pantera(string Name, int x, int y) : base(Name, x, y)
        { }
        private Pantera(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
        /// <summary>
        /// Print tant on the battlefield
        /// </summary>
        /// <param name="x"> The real Х-coord of picter </param>
        /// <param name="y"> The real Y-coord of picter </param>
        public override void Print(int x, int y)
        {
            ForegroundColor = ConsoleColor.Red;
            SetCursorPosition(x - 5, y);
            Write(" **** ");
            SetCursorPosition(x - 5, y + 1);
            Write("######");
            SetCursorPosition(x - 8, y + 2);
            Write("====#####");
            SetCursorPosition(x - 5, y + 3);
            Write("######");
            SetCursorPosition(x - 5, y + 4);
            WriteLine(" **** ");
            ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Print destroyed tank
        /// </summary>
        public override void Destroied()
        {
            ForegroundColor = ConsoleColor.Yellow;
            SetCursorPosition(coordX - 3, coordY);
            Write("&&");
            SetCursorPosition(coordX - 4, coordY + 1);
            Write("&&&&");
            SetCursorPosition(coordX - 5, coordY + 2);
            Write("&&&&&&");
            SetCursorPosition(coordX - 4, coordY + 3);
            Write("&&&&");
            SetCursorPosition(coordX - 3, coordY + 4);
            Write("&&");
        }
        /// <summary>
        /// Mooving tank on the battle position
        /// </summary>
        public override void Move()
        {
            int realX = 99;
            mute.WaitOne();
            Print(realX, coordY);
            mute.ReleaseMutex();
            while (realX != coordX)
            {
                mute.WaitOne();
                for (int i = 0; i < 5; i++)
                {
                    SetCursorPosition(realX, coordY + i);
                    Write(" ");
                }
                realX--;
                Print(realX, coordY);
                mute.ReleaseMutex();
                Thread.Sleep(200);
            }
        }
        public override string ToString()
        {
            return " Type: Pantera\t" + base.ToString();
        }
    }
}
