using System;

namespace GameOfTanks
{
    class MyException : Exception
    {
        public MyException(string mess) : base(mess)
        { }
    }
    class IntException : MyException
    {
        public int Return { get; private set; }
        public IntException(string mess, int ret) : base(mess)
        {
            Return = ret;
        }
    }
}
