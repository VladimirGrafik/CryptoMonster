using System;
using System.Linq;
using Components.Db;
using Components.Db.Maps;
using Components.Helpers;
using Components.Markets.AdaptersApi;
using Cmp = Trader.Helpers.Cmp;
using Dvr = Trader.Helpers.Dvr;
using Mnr = Trader.Helpers.Mnr;


namespace Trader
{
    public class PushMachine
    {
        private readonly OrdersTrader _ordersTrader;
        private readonly OrderBook _orderBook;
        private readonly BalancesManager _balancesManager;
        private readonly int? _mode = Trader.Mode;
        public readonly PushMirror.Push PM;

        public readonly PushMirror.Push PMr;

        public PushMachine(OrdersTrader ordersTrader, OrderBook orderBook, BalancesManager balancesManager)
        {
            _ordersTrader = ordersTrader;
            _orderBook = orderBook;
            _balancesManager = balancesManager;
            var pushMirror = new PushMirror(ordersTrader, orderBook);
            PM = pushMirror.PushM;
            PMr = pushMirror.PushMr;
            RunPushMachine();
        }

        private void RunPushMachine()
        {
            if (PM.PercentStart == null || PM.PercentStart == 0) PM.PercentStart = 2;
            if (PM.PercentLimit == null || PM.PercentLimit == 0) PM.PercentLimit = 3;
            if (PM.PercentEnd == null || PM.PercentEnd == 0) PM.PercentEnd = 0.5M;
            MovePricePush();
            GetPercentPushAndPercent();
            if (PM.BalanceLimit == null)
            {
                var balanceLimitTemp = (_balancesManager.ModeBalance != null)
                    ? (_mode == 0) ? _balancesManager.ModeBalance / 3 : _balancesManager.ModeBalance
                    : 0;
                PM.BalanceLimit = balanceLimitTemp;
            }
            GetBPriceAndBPriceTrade();
            MoveBPricePush();
            GetBPercentPushAndBPercent();
            BuildBPercentPrism();
            WritePushMachine();
        }

        private void MovePricePush()
        {
            PM.Price = PMr.OrderBook[0].Rate;
            if ((_mode == 0 || _mode == 1) && PM.PriceBegining == null) PM.PriceBegining = PM.Price;
            if (PM.PricePush == null) PM.PricePush = PM.Price;
            else PM.PricePush = Cmp.X(PM.PricePush, PM.Price) ? PM.Price : PM.PricePush;
        }

        private void GetPercentPushAndPercent()
        {
            var dif = Mnr.X(PM.PriceBegining, PM.Price);
            if (PM.PriceBegining == 0 || PM.Price == 0) throw new Exception("Zero parameters!");
            PM.Percent = (dif != 0) ? dif / PM.Price * 100 : 0;

            var dif2 = Mnr.X(PM.PriceBegining, PM.PricePush);
            if (PM.PriceBegining == 0 || PM.PricePush == 0) throw new Exception("Zero parameters!");
            PM.PercentPush = (dif2 != 0) ? dif2 / PM.PricePush * 100 : 0;
        }

        private void GetBPriceAndBPriceTrade()
        {
            decimal enoughQuantity = 0;
            decimal rateQuantity = 0;
            foreach (var orderBook in PMr.OrderBook ?? throw new Exception("No rows in orderBook!"))
            {
                enoughQuantity += orderBook.Quantity;
                rateQuantity += orderBook.Rate * orderBook.Quantity;
                if (enoughQuantity >= PM.BalanceLimit)
                {
                    PM.BPriceTrede = orderBook.Rate;
                    PM.BPrice = rateQuantity / enoughQuantity;
                    break;
                }
            }
        }

        private void MoveBPricePush()
        {
            if ((_mode == 0 || _mode == 1) && PM.BPriceBegining == null) PM.BPriceBegining = PM.BPrice;
            if (PM.BPricePush == null) PM.BPricePush = PM.BPrice;
            else PM.BPricePush = Cmp.X(PM.BPricePush, PM.BPrice) ? PM.BPrice : PM.BPricePush;
        }

        private void GetBPercentPushAndBPercent()
        {
            var dif = Mnr.X(PM.BPriceBegining, PM.BPrice);
            if (PM.BPriceBegining == 0 || PM.BPrice == 0) throw new Exception("Zero parameters!");
            PM.BPercent = (dif != 0) ? dif / PM.BPrice * 100 : 0;

            var dif2 = Mnr.X(PM.BPriceBegining, PM.BPricePush);
            if (PM.BPriceBegining == 0 || PM.BPricePush == 0) throw new Exception("Zero parameters!");
            PM.BPercentPush = (dif2 != 0) ? dif2 / PM.BPricePush * 100 : 0;
        }

        private void BuildBPercentPrism()
        {
            if (PM.BPercentPush == null || PM.PercentLimit == null) return;
            if (PM.BPercentPush != 0 && PM.PercentLimit != 0)
                PM.PercentPrismMove = PM.BPercentPush / PM.PercentLimit * 100;
            else PM.PercentPrismMove = 0;
            if (PM.PercentPrismMove < 0) PM.PercentPrismMove = 0;
            else if (PM.PercentPrismMove > 100) PM.PercentPrismMove = 100;

            if (PM.PercentPrismMove <= 0) PM.PercentPrism = PM.PercentStart;
            else if (PM.PercentPrismMove >= 100) PM.PercentPrism = PM.PercentEnd;
            else
            {
                var percentPrismRange = PM.PercentEnd - PM.PercentStart;
                if (percentPrismRange == 0) PM.PercentPrism = PM.PercentStart;
                else
                {
                    var percentInPrism = PM.PercentPrismMove / 100 * percentPrismRange;
                    PM.PercentPrism = PM.PercentStart +
                                      ((percentInPrism * PM.PercentPrismMove / 100) * PM.PercentPrismMove / 100);
                }
            }
        }

        private void WritePushMachine()
        {
            using (var cmTr = CmTrading.OpenSession())
            {
                using (var trans = cmTr.BeginTransaction())
                {
                    var orTr = cmTr.QueryOver<OrdersTrader>().Where(x =>
                            x.PairName == _ordersTrader.PairName)
                        .List().FirstOrDefault();
                    if (orTr != null)
                    {
                        if (orTr.DateStart == null) orTr.DateStart = DateTime.Now;
                        if (orTr.UTimeStart == null) orTr.UTimeStart = UnixTime.Now();
                        if (orTr.PriceStart == null) orTr.PriceStart = PM.PriceBegining;
                        if (orTr.BPriceStart == null) orTr.BPriceStart = PM.BPriceBegining;
                        if (PM.BalanceLimit == null) orTr.BalanceLimit(orTr, _mode, PM.BalanceLimit);
                        orTr.Characters = "xxxxx";
                        orTr.Price(orTr, _mode, PM.Price);
                        orTr.Percent(orTr, _mode, PM.Percent);
                        orTr.PricePush(orTr, _mode, PM.PricePush);
                        orTr.PercentPush(orTr, _mode, PM.PercentPush);
                        orTr.BPriceTrede(orTr, _mode, PM.BPriceTrede);
                        orTr.BPrice(orTr, _mode, PM.BPrice);
                        orTr.BPercent(orTr, _mode, PM.BPercent);
                        orTr.BPricePush(orTr, _mode, PM.BPricePush);
                        orTr.BPercentPush(orTr, _mode, PM.BPercentPush);
                        orTr.PercentStart(orTr, _mode, PM.PercentStart);
                        orTr.PercentLimit(orTr, _mode, PM.PercentLimit);
                        orTr.PercentEnd(orTr, _mode, PM.PercentEnd);
                        orTr.PercentPrismMove(orTr, _mode, PM.PercentPrismMove);
                        orTr.PercentPrism(orTr, _mode, PM.PercentPrism);
                        cmTr.Update(orTr);
                    }

                    else throw new Exception("Dont got ordersTrader from db!");
                    trans.Commit();
                }
            }
        }
    }
}