using System;
using System.Linq;
using Components;
using Components.Db.Maps;

namespace Trader
{
    internal class TraderStarter : ProHub
    {
        private TraderStarter(string[] args) : base(args)
        {
        }

        public static void RunTraderStarter(string[] args)
        {
            var runStatic = new TraderStarter(args);
            if (!SistemSettings.EnableHunter) throw new Exception("Trader is desibled!");
            var ordersTraders = GetOrdersTrader().ToList();
            var catchPairs = GetCatchPairs(true);
            var openOrders = MarketApi.GetOpenOrders();
            var balances = MarketApi.GetBalances();
            foreach (OrdersTrader ordersTrader in ordersTraders)
            {
                if (ordersTrader.ModeTrade == 1 || ordersTrader.ModeTrade == 2 || ordersTrader.ModeTrade == 3)
                {
                    try
                    {
                        var trader = new Trader(ordersTrader, ordersTraders, catchPairs, openOrders, balances);
                        trader.RunTrader();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }
    }
}