using System;

namespace Trader.Helpers
{
    public static class Pmr
    {
        public static decimal? X(decimal? firstOperand, decimal? secondOperand)
        {
            if (Trader.Mode == 0 || Trader.Mode == 1) return firstOperand + secondOperand;
            if (Trader.Mode == 2 || Trader.Mode == 3) return firstOperand - secondOperand;
            throw new Exception("Dont get validate mode!");
        }

        public static int? X(int? firstOperand, int? secondOperand)
        {
            if (Trader.Mode == 0 || Trader.Mode == 1) return firstOperand + secondOperand;
            if (Trader.Mode == 2 || Trader.Mode == 3) return firstOperand - secondOperand;
            throw new Exception("Dont get validate mode!");
        }

        public static float? X(float? firstOperand, float? secondOperand)
        {
            if (Trader.Mode == 0 || Trader.Mode == 1) return firstOperand + secondOperand;
            if (Trader.Mode == 2 || Trader.Mode == 3) return firstOperand - secondOperand;
            throw new Exception("Dont get validate mode!");
        }
    }
}