using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Components.Analytical;
using Components.Db;
using Components.Db.Maps;
using Components.Markets;
using Components.Markets.AdaptersApi;
using NHibernate.Linq;
using Trader.Helpers;


namespace Trader
{
    public class Trader
    {
        private readonly OrdersTrader _ordersTrader;
        public static int? Mode;
        public static int? StopLoss;
        private readonly List<CatchPairs> _catchPairs;
        private readonly OrderBook _orderBook;
        private readonly StatesMachine _statesMachine;
        private readonly BalancesManager _balancesManager;
        private readonly PushMachine _pushMachine;
        private readonly PushMirror.Push _pM;
        private readonly PushMirror.Push _pMr;
        private readonly OrderBookMachine _orderBookMachine;
        private readonly OrdersManager _ordersManager;

        public Trader(OrdersTrader ordersTrader, List<OrdersTrader> ordersTraders, List<CatchPairs> catchPairs,
            List<OpenOrders> openOrders, List<Balances> balances)
        {
            _ordersTrader = ordersTrader;
            Mode = _ordersTrader.ModeTrade;
//            StopLoss = (StopLoss == null) ? CheckStopLoss() : StopLoss;
            _catchPairs = catchPairs;
            _orderBook = ProHub.MarketApi.GetOrderBook(_ordersTrader.PairName);
            _balancesManager = new BalancesManager(balances, _ordersTrader, ordersTraders);
            _pushMachine = new PushMachine(_ordersTrader, _orderBook, _balancesManager);
            _pM = _pushMachine.PM;
            _pMr = _pushMachine.PMr;
            _orderBookMachine = new OrderBookMachine(_orderBook);
            _orderBookMachine.WriteOrderSteps(ordersTrader);
            _statesMachine = new StatesMachine(_ordersTrader, _catchPairs);
            _ordersManager = new OrdersManager(openOrders, _ordersTrader, _orderBookMachine, _balancesManager);
        }


        public void RunWatcher()
        {
            WatchingMachine();
        }

        public void RunTrader()
        {
            TradingMachine();
        }

        private void WatchingMachine()
        {
            if (_ordersTrader.SafeDelete == 1 || _ordersTrader.ForceDelete == 1) DeleteOrdersAndPair();
            else if (_pM.BPercent < _pM.BPercentPush - _pM.PercentPrism || _pM.Run == 1 || _pM.Force == 1)
            {
                var balanceLimitBuy = _balancesManager.GetBalanceLimitBuy();
                if (balanceLimitBuy != null &&
                    balanceLimitBuy < _balancesManager.MainCurrency.MinOrderQuantityLimit) DeleteOrdersAndPair();
                else if (CheckStopLoss()) DeleteOrdersAndPair();
                else if (_pM.Run == 1 || _pM.Force == 1 || (ProHub.MarketSettings.AutoTrade == 1 ||
                                                            _ordersTrader.AutoTrade == 1))
                    ChangeMode(balanceLimitBuy);
                else if (_pM.PercentPrismMove < 50 || _statesMachine.Market.Sma60 < -0.2M) DeleteOrdersAndPair();
                else DeleteOrdersAndPair();
            }
        }

        private void TradingMachine()
        {
            switch (Mode)
            {
                case 1:
                    if (CheckStopLoss() || _ordersTrader.SafeDelete == 1 || _ordersTrader.ForceDelete == 1)
                    {
                        if (!DeleteOrdersAndPair()) ChangeMode(_balancesManager.Second?.Common);
                    }
                    else if (_balancesManager.ModeBalance < _balancesManager.MainCurrency.MinOrderQuantityLimit &&
                             _ordersManager.MainOrders.Count == 0) ChangeMode(_balancesManager.Second?.Common);
                    else if (_pM.Force == 1) ForceTrade();
                    else ChooseTraderCharacter();
                    break;
                case 2:
                    if (_ordersTrader.ForceDelete == 1)
                        if (!DeleteOrdersAndPair()) ChangeMode(_balancesManager.Second?.Common);
                    if (_pM.BPercent < _pM.BPercentPush - _pM.PercentPrism || _pM.Run == 1 || _pM.Force == 1)
                    {
                        if (!DeleteOrdersAndPair()) ChangeMode(_balancesManager.Second?.Common);
                        else ChangeMode(_balancesManager.Second?.Common);
                    }
                    break;
                case 3:
                    DeleteOrdersAndPair();
                    if (_pM.Force == 1 || _ordersTrader.ForceDelete == 1) ForceTrade();
                    else ChooseTraderCharacter();
                    break;
                default:
                    throw new Exception("Dont satisfactory Mode!");
                case null:
                    throw new Exception("Dont satisfactory null Mode!");
            }
        }


        private void ChooseTraderCharacter()
        {
            var marketState = new[]
                {
                    _statesMachine.Market?.VectorSma15X60,
                    _statesMachine.Market?.PercentDay != 0 ? _statesMachine.Market?.PercentDay / 24 / 4 : 0,
                    _statesMachine.Market?.Sma60 != 0 ? _statesMachine.Market?.Sma60 / 4 : 0,
                    _statesMachine.Market?.Sma15
                }
                .Average();
            var pairState = new[]
                {
                    _statesMachine.Pair?.VectorSma15X60,
                    _statesMachine.Pair?.PercentDay != 0 ? _statesMachine.Pair?.PercentDay / 24 / 4 : 0,
                    _statesMachine.Pair?.Sma60 != 0 ? _statesMachine.Market?.Sma60 / 4 : 0,
                    _statesMachine.Pair?.Sma15
                }
                .Average();

            var summariesState = new[] {marketState, pairState != 0 ? pairState / 2 : 0}.Average() * 1200;
            summariesState = 0;

            if (Mode == 1)
            {
                if (summariesState > 100) ForceTrade();
                else if (summariesState > 40) BorderStealer();
                else if (summariesState > -40) StepToFirst();
                else if (summariesState > -80) BaseTrader();
                else if (summariesState < -80) NoTrade();
            }
            else if (Mode == 3)
            {
                if (summariesState > 100) NoTrade();
                else if (summariesState > 40) BaseTrader();
                else if (summariesState > -40) StepToFirst();
                else if (summariesState > -80) BorderStealer();
                else if (summariesState < -80) ForceTrade();
            }
            else throw new Exception("Dont satisfactory Mode!");
        }

        private bool CheckStopLoss()
        {
            
            
            return false;
        }

        private void UpdateStopLoss()
        {
        }

        private bool DeleteOrdersAndPair()
        {
            if (_balancesManager.Second == null ||
                _balancesManager.Second.Common <
                _balancesManager.MainCurrency.MinOrderQuantityLimit / _orderBook.Bids[0].Rate)
            {
                _ordersManager.CancelAllOrdersPair();
                if (Mode == 3 && !WriteTradeHistory()) throw new Exception("Dont write trade history!");
                using (var cmTr = CmTrading.OpenSession())
                using (var trans = cmTr.BeginTransaction())
                {
                    var currentPair = cmTr.Query<OrdersTrader>().SingleOrDefault(x => x.Id == _ordersTrader.Id);
                    cmTr.Delete(currentPair);
                    trans.Commit();
                }
                return true;
            }
            return false;
        }

        private void ForceTrade()
        {
            if (_ordersManager.MainOrders.Count != 0) _ordersManager.CancelAllOrdersPair();
            _ordersTrader.BuyOrSell(Mode, _ordersTrader.PairName, _pM.BPriceTrede, _balancesManager.ModeBalance);
        }

        private void BorderStealer(decimal percentBorder = 1)
        {
            var priceBorder = Pmr.X(_pM.Price, (_pM.Price / 100 * percentBorder));
            if (_ordersManager.MainOrders.Count > 0) _ordersManager.CancelAllOrdersPair();
            foreach (var orderBook in _pMr.OrderBook)
            {
                if (Cmp.X(orderBook.Rate, priceBorder))
                {
                    return;
                }
                _ordersTrader.BuyOrSell(Mode, _ordersTrader.PairName, priceBorder, _balancesManager.ModeBalance);
                _ordersManager.CancelAllOrdersPair();
                break;
            }
        }

        private void StepToFirst()
        {
            foreach (var orderBook in _pM.OrderBook)
            {
                if (orderBook.Quantity > _balancesManager.ModeBalance * 0.6M)
                {
                    var cupOrderPrice = Pmr.X(orderBook.Rate,
                        _balancesManager.MainCurrency.MinUnitCurrency);
                    if (_ordersManager.AllOrders.Count == 0)
                    {
                        _ordersTrader.BuyOrSell(Mode, _ordersTrader.PairName, cupOrderPrice,
                            _balancesManager.ModeBalance);
                        return;
                    }
                    if (_ordersManager.AllOrders.Any(openOrder => Cmp.X(orderBook.Rate, openOrder.Rate) ||
                                                                  Cmp.X(openOrder.Rate, cupOrderPrice)))
                    {
                        _ordersManager.CancelAllOrdersPair();
                        _ordersTrader.BuyOrSell(Mode, _ordersTrader.PairName, cupOrderPrice,
                            _balancesManager.ModeBalance);
                        return;
                    }
                }
            }
        }

        private bool WriteTradeHistory()
        {
            return true;
        }

        private void BaseTrader()
        {
            if (_pM.BPercent > _pM.BPercentPush - _pM.PercentPrism && _pM.BPercent > 0) ;
        }

        private void NoTrade()
        {
        }

        private void ChangeMode(decimal? balanceLimit, int? mode = null)
        {
            using (var cmTr = CmTrading.OpenSession())
            using (var trans = cmTr.BeginTransaction())
            {
                var ordersTrader = cmTr.Query<OrdersTrader>()
                    .SingleOrDefault(x => x.Id == _ordersTrader.Id);
                if (ordersTrader != null)
                {
                    if (mode != null)
                    {
                        switch (mode)
                        {
                            case 0:
                                if (Mode == 2 && Mode == 3) throw new Exception("Not allow mode change!");
                                ordersTrader.ModeTrade = mode;
                                ordersTrader.BalanceLimitBuy = null;
                                ordersTrader.BalanceLimitSell = null;
                                break;
                            case 1:
                                if (Mode == 2 && Mode == 3) throw new Exception("Not allow mode change!");
                                ordersTrader.ModeTrade = mode;
                                ordersTrader.BalanceLimitBuy = balanceLimit;
                                ordersTrader.BalanceLimitSell = null;
                                ordersTrader.PricePushSell = null;
                                ordersTrader.BPricePushSell = null;
                                break;
                            case 2:
//                                if (Mode == 0) throw new Exception("Not allow mode change!");
                                ordersTrader.ModeTrade = mode;
                                ordersTrader.BalanceLimitBuy = balanceLimit * _pM.BPrice;
                                ordersTrader.BalanceLimitSell = balanceLimit;
                                break;
                            case 3:
                                if (Mode == 0) throw new Exception("Not allow mode change!");
                                ordersTrader.ModeTrade = mode;
                                ordersTrader.BalanceLimitBuy = balanceLimit / _pM.BPrice;
                                ordersTrader.BalanceLimitSell = balanceLimit;
                                ordersTrader.PricePushSell = null;
                                ordersTrader.BPricePushSell = null;
                                break;
                            default: throw new Exception("Dont satisfactory mode take!");
                        }
                    }
                    else
                        switch (Mode)
                        {
                            case 0:
                                ordersTrader.ModeTrade = 1;
                                if (balanceLimit != null) ordersTrader.BalanceLimitBuy = balanceLimit;
                                break;
                            case 1:
                                _ordersManager.CancelAllOrdersPair();
                                if (DeleteOrdersAndPair()) return;
                                ordersTrader.ModeTrade = 2;
                                if (balanceLimit != null)
                                {
                                    ordersTrader.BalanceLimitSell = balanceLimit;
                                    ordersTrader.BalanceLimitBuy = balanceLimit / _pM.BPrice;
                                }
                                break;
                            case 2:
                                ordersTrader.ModeTrade = 3;
                                if (balanceLimit != null)
                                {
                                    ordersTrader.BalanceLimitSell = balanceLimit;
                                    ordersTrader.BalanceLimitBuy = balanceLimit / _pM.BPrice;
                                }
                                break;
                            case 3:
                                _ordersManager.CancelAllOrdersPair();
                                if (DeleteOrdersAndPair()) return;
                                return;
                            case null:
                                throw new Exception("Dont satisfactory Mode = null!");
                            default:
                                throw new Exception("Dont satisfactory Mode!");
                        }
                }
                cmTr.Update(ordersTrader);
                trans.Commit();
            }
        }


        private void TracerExtender()
        {
        }

//        private bool CheckBalance()


//        {


//            _balance = (Mode == 1)


//                ? _balancesManager?.Balances.SingleOrDefault(x => x.CurrencyName == _ordersTrader.MainCurrency)?.Common


//                : (Mode == 2)


//                    ? _balancesManager.Balances.SingleOrDefault(x => x.CurrencyName == _ordersTrader.SecondCurrency)?


//                        .Common


//                    : throw new Exception("Dont satisfactory Mode!");


//            return _balance > ProHub.MarketApi.MainCurrencies.SingleOrDefault(x =>


//                       x.CurrencyName == _ordersTrader.MainCurrency)?.MinOrderQuantityLimit;


//        }
    }
}