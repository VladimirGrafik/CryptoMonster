using System;
using System.Collections.Generic;
using System.Linq;
using Components.Db;
using Components.Db.Maps;
using Components.Markets.AdaptersApi;
using Newtonsoft.Json;
using NHibernate.Linq;

namespace Components.Analytical
{
    public class OrderBookMachine
    {
        private const decimal PercentOrderStep = 0.5M;
        private const decimal PercentForWinner = 6 / PercentOrderStep;
        private const decimal MainMultiplier = 2;
        private const decimal WallMultiplier = 4;
        private const decimal RoadStepWidth = 4 / PercentOrderStep;
        private const decimal RoadMultiplier = 4 / PercentOrderStep;
        private readonly OrderBook _orderBook;
        private decimal _marginPercent;
        private readonly int _uTimeIdNow;

        public CatchPairs CatchPair { get; }
        public OrderBook OrderBookSteps { get; }
        public OrderBookState OrderBookState { get; private set; }

        public OrderBookMachine(OrderBook orderBook, CatchPairs catchPair = null, int uTimeIdNow = 0)
        {
            CatchPair = catchPair;
            _orderBook = orderBook;
            OrderBookSteps = BuildOrderBookSteps();
            OrderBookState = GetOrderBookState(OrderBookSteps);
            _marginPercent = OrderBookState.MarginPercent;
            _uTimeIdNow = uTimeIdNow;
        }

        public bool RunAnalytic()
        {
            if (CatchPair == null && _uTimeIdNow == 0) throw new Exception("Dont get catchPair or uTimeId!");
            WriteAnalytic(CatchPair);


            return true;
        }

        private decimal GetMarginPercent()
        {
            var firstBids = _orderBook.Bids.FirstOrDefault();
            var firstAsks = _orderBook.Asks.FirstOrDefault();
            if (firstAsks == null || firstBids == null) throw new Exception("Dont got correct data from orderBook!");
            if (firstAsks.Rate == 0 || firstBids.Rate == 0) throw new Exception("Got null Rate from orderBook!");
            var margin = firstBids.Rate - firstAsks.Rate;
            _marginPercent = firstBids.Rate / margin;
            var marginRateCenter = firstBids.Rate - (margin / 2);
            return marginRateCenter;
        }

        private OrderBook BuildOrderBookSteps()
        {
            var asksStepRate = _orderBook.Asks.FirstOrDefault() ?? throw new Exception("orderBook.Asks is null!");
            var bidsStepRate = _orderBook.Bids.FirstOrDefault() ?? throw new Exception("orderBook.Bids is null!");
            var orderBookSteps = new OrderBook {Asks = new List<BidsAsks>(), Bids = new List<BidsAsks>()};
            var pStepValue = asksStepRate.Rate * (PercentOrderStep / 100);
            var countAsksBids = _orderBook.Asks.Count < _orderBook.Bids.Count
                ? _orderBook.Asks.Count
                : _orderBook.Bids.Count;
            decimal asksSumQuantity = 0;
            decimal bidsSumQuantity = 0;
            var asksStep = asksStepRate.Rate + pStepValue;
            var bidsStep = bidsStepRate.Rate - pStepValue;
            var j = 0;
            var k = 0;
            for (var i = 0; i < countAsksBids; i++)
            {
                var dualChecker = 0;
                for (; j < _orderBook.Asks.Count; j++)
                {
                    asksSumQuantity += _orderBook.Asks[j].Quantity;
                    if (_orderBook.Asks[j].Rate > asksStep)
                    {
                        asksStep += pStepValue;
                        dualChecker += 1;
                        break;
                    }
                }

                for (; k < _orderBook.Bids.Count; k++)
                {
                    bidsSumQuantity += _orderBook.Bids[k].Quantity;
                    if (_orderBook.Bids[k].Rate < bidsStep)
                    {
                        bidsStep -= pStepValue;
                        dualChecker += 1;
                        break;
                    }
                }
                if (dualChecker == 2)
                {
                    orderBookSteps.Asks.Add(new BidsAsks()
                    {
                        Rate = _orderBook.Asks[j].Rate,
                        Quantity = asksSumQuantity
                    });
                    orderBookSteps.Bids.Add(new BidsAsks()
                    {
                        Rate = _orderBook.Bids[k].Rate,
                        Quantity = bidsSumQuantity
                    });
                }
            }
            return orderBookSteps;
        }


        private OrderBookState GetOrderBookState(OrderBook orderBookSteps)
        {
            var winVar = 0;
            var orderBookState = new OrderBookState
            {
                Asks = new OrderBookState.Double() {First = _orderBook.Asks[0].Rate},
                Bids = new OrderBookState.Double() {First = _orderBook.Bids[0].Rate}
            };

            for (var i = 0; i < orderBookSteps.Asks.Count || i < orderBookSteps.Bids.Count; i++)
            {
                if (orderBookSteps.Asks[i].Quantity > orderBookSteps.Bids[i].Quantity * MainMultiplier)
                {
                    orderBookState.Asks.WinsCount += 1;
                    /////////////////////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    if (i > 0)
                    {
                        if (winVar < 0) winVar = 0;
                        winVar += 1;
                        if (orderBookState.Asks.Wall == 0 &&
                            orderBookSteps.Asks[i].Quantity > orderBookSteps.Asks[i - 1].Quantity * WallMultiplier &&
                            i != 0)
                            orderBookState.Asks.Wall = orderBookSteps.Asks[i - 1].Rate;
                        if (i > RoadStepWidth && orderBookSteps.Bids[i].Quantity <
                            orderBookSteps.Bids[i - (int) RoadStepWidth].Quantity * RoadMultiplier)
                            orderBookState.Bids.Road = orderBookSteps.Bids[i - 1].Rate;
                    }
                }
                else if (orderBookSteps.Bids[i].Quantity >
                         orderBookSteps.Asks[i].Quantity * MainMultiplier)
                {
                    orderBookState.Bids.WinsCount += 1;
                    ////////////////////////////////////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    if (i > 0)
                    {
                        if (winVar > 0) winVar = 0;
                        winVar -= 1;
                        if (orderBookState.Bids.Wall == 0 &&
                            orderBookSteps.Bids[i].Quantity > orderBookSteps.Bids[i - 1].Quantity * WallMultiplier &&
                            i != 0)
                            orderBookState.Bids.Wall = orderBookSteps.Bids[i].Quantity;
                        if (i > RoadStepWidth && orderBookSteps.Asks[i].Quantity <
                            orderBookSteps.Asks[i - (int) RoadStepWidth].Quantity * RoadMultiplier)
                            orderBookState.Asks.Road = orderBookSteps.Asks[i - 1].Rate;
                    }
                }

                if (orderBookState.Winner == null)
                {
                    if (winVar <= -PercentForWinner) orderBookState.Winner = "asks";
                    if (winVar >= PercentForWinner) orderBookState.Winner = "bids";
                }
            }
            return orderBookState;
        }

        public void WriteOrderSteps(OrdersTrader ordersTrader)
        {
            using (var cmTr = CmTrading.OpenSession())
            using (var trans = cmTr.BeginTransaction())
            {
                var ordersTraderWrite = cmTr.Query<OrdersTrader>().SingleOrDefault(x => x.Id == ordersTrader.Id);
                var orderBookStepsSerialize = JsonConvert.SerializeObject(OrderBookSteps);
                if (ordersTraderWrite != null) ordersTraderWrite.OrderBookSteps = orderBookStepsSerialize;
                trans.Commit();
            }
        }

        private void WriteAnalytic(CatchPairs catchPair)
        {
            var orderBookStepsJson = JsonConvert.SerializeObject(OrderBookSteps);
            using (var cmTh = CmThinking.OpenSession())
            {
                using (var trans = cmTh.BeginTransaction())
                {
                    var analyticHistory = new AnalyticHistory
                    {
                        PairName = catchPair.PairName,
                        FirstCurrency = catchPair.MainCurrency,
                        SecondCurrency = catchPair.SecondCurrency,
                        DateTime = catchPair.DateTime,
                        PriceP15 = catchPair.PriceP15,
                        PriceP24 = catchPair.PricePDay,
                        LastPrice = catchPair.LastPrice,
                        MaxBids = catchPair.MaxBidsDay,
                        MinAsks = catchPair.MinAsksDay,
                        HighPercent = catchPair.PrismPDayHeight,
                        VolumeDay = catchPair.VolumeDay,
                        VolumeP15 = catchPair.VolumeP15,

                        OrderBook = orderBookStepsJson,

                        Winner = OrderBookState.Winner,
                        AsksWins = OrderBookState.Asks.WinsCount,
                        BidsWins = OrderBookState.Bids.WinsCount,
                        MarginPercent = OrderBookState.MarginPercent,
                        AsksFirst = OrderBookState.Asks.First,
                        BidsFirst = OrderBookState.Bids.First,

                        UTime = _uTimeIdNow
                    };
                    cmTh.Save(analyticHistory);
                    trans.Commit();
                }
            }
        }
    }
}