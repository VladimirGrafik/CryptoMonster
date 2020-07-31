using System;

namespace Trader.Helpers
{
    public static class Cmp
    {
        public static bool X(decimal? firstOperand, decimal? secondOperand)
        {
            if (Trader.Mode == 0 || Trader.Mode == 1) return firstOperand > secondOperand;
            if (Trader.Mode == 2 || Trader.Mode == 3) return firstOperand < secondOperand;
            throw new Exception("Dont get validate mode!");
        }

        public static bool X(int? firstOperand, int? secondOperand)
        {
            if (Trader.Mode == 0 || Trader.Mode == 1) return firstOperand > secondOperand;
            if (Trader.Mode == 2 || Trader.Mode == 3) return firstOperand < secondOperand;
            throw new Exception("Dont get validate mode!");
        }

        public static bool X(float? firstOperand, float? secondOperand)
        {
            if (Trader.Mode == 0 || Trader.Mode == 1) return firstOperand > secondOperand;
            if (Trader.Mode == 2 || Trader.Mode == 3) return firstOperand < secondOperand;
            throw new Exception("Dont get validate mode!");
        }
    }
}