using System;
using System.Collections.Generic;
using Components.Db.Maps;
using Components.Markets.AdaptersApi;

namespace Trader
{
    public class PushMirror
    {
        public Push PushM { get; }
        public Push PushMr { get; }

        public PushMirror(OrdersTrader ordersTrader, OrderBook orderBook)
        {
            var pushMBuy = new Push
            {
                PriceBegining = ordersTrader.PriceStart,
                BPriceBegining = ordersTrader.BPriceStart,
                Run = ordersTrader.RunBuy,
                Force = ordersTrader.ForceBuy,
                Price = ordersTrader.PriceBuy,
                Percent = ordersTrader.PercentBuy,
                PricePush = ordersTrader.PricePushBuy,
                PercentPush = ordersTrader.PercentPushBuy,
                BPriceTrede = ordersTrader.BPriceTradeBuy,
                BPrice = ordersTrader.BPriceBuy,
                BPercent = ordersTrader.BPercentBuy,
                BPricePush = ordersTrader.BPricePushBuy,
                BPercentPush = ordersTrader.BPercentPushBuy,
                PercentStart = ordersTrader.PercentStartBuy,
                PercentLimit = ordersTrader.PercentLimitBuy,
                PercentEnd = ordersTrader.PercentEndBuy,
                PercentPrismMove = ordersTrader.PercentPrismMoveBuy,
                PercentPrism = ordersTrader.PercentPrismBuy,
                BalanceLimit = ordersTrader.BalanceLimitBuy,
                OrderBook = orderBook.Bids
            };
            var pushMSell = new Push
            {
                PriceBegining = ordersTrader.PriceBuy,
                BPriceBegining = ordersTrader.BPriceBuy,
                Run = ordersTrader.RunSell,
                Force = ordersTrader.ForceSell,
                Price = ordersTrader.PriceSell,
                Percent = ordersTrader.PercentSell,
                PricePush = ordersTrader.PricePushSell,
                PercentPush = ordersTrader.PercentPushSell,
                BPriceTrede = ordersTrader.BPriceTradeSell,
                BPrice = ordersTrader.BPriceSell,
                BPercent = ordersTrader.BPercentSell,
                BPricePush = ordersTrader.BPricePushSell,
                BPercentPush = ordersTrader.BPercentPushSell,
                PercentStart = ordersTrader.PercentStartSell,
                PercentLimit = ordersTrader.PercentLimitSell,
                PercentEnd = ordersTrader.PercentEndSell,
                PercentPrismMove = ordersTrader.PercentPrismMoveSell,
                PercentPrism = ordersTrader.PercentPrismSell,
                BalanceLimit = ordersTrader.BalanceLimitSell,
                OrderBook = orderBook.Asks
            };
            switch (Trader.Mode)
            {
                case 0:
                    PushM = pushMBuy;
                    PushMr = pushMSell;
                    break;
                case 1:
                    PushM = pushMBuy;
                    PushMr = pushMSell;
                    break;
                case 2:
                    PushM = pushMSell;
                    PushMr = pushMBuy;
                    break;
                case 3:
                    PushM = pushMSell;
                    PushMr = pushMBuy;
                    break;
                case null:
                    throw new Exception("Dont satisfactory Mode = null!");
                default:
                    throw new Exception("Dont satisfactory Mode!");
            }
        }

        public class Push
        {
            public decimal? PriceBegining { get; set; }
            public decimal? BPriceBegining { get; set; }
            public decimal? Run { get; set; }
            public decimal? Force { get; set; }
            public decimal? Price { get; set; }
            public decimal? Percent { get; set; }
            public decimal? PricePush { get; set; }
            public decimal? PercentPush { get; set; }
            public decimal? BPriceTrede { get; set; }
            public decimal? BPrice { get; set; }
            public decimal? BPercent { get; set; }
            public decimal? BPricePush { get; set; }
            public decimal? BPercentPush { get; set; }
            public decimal? PercentStart { get; set; }
            public decimal? PercentLimit { get; set; }
            public decimal? PercentEnd { get; set; }
            public decimal? PercentPrismMove { get; set; }
            public decimal? PercentPrism { get; set; }
            public decimal? BalanceLimit { get; set; }
            public List<BidsAsks> OrderBook { get; set; }
        }
    }
}