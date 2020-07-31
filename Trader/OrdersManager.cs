using System.Collections.Generic;
using System.Linq;
using Components;
using Components.Analytical;
using Components.Db.Maps;
using Components.Markets.AdaptersApi;

namespace Trader
{
    public class OrdersManager
    {
        private readonly OrdersTrader _ordersTrader;
        private OrderBookMachine _orderBookMachine;
        private readonly List<OpenOrders> _openOrders;
        public List<OpenOrders> AllOrders { get; private set; }
        public List<OpenOrders> MainOrders { get; private set; }
        public List<OpenOrders> TracerOrders { get; private set; }

        public OrdersManager(List<OpenOrders> openOrders, OrdersTrader ordersTrader, OrderBookMachine orderBookMachine,
            BalancesManager balancesManager)
        {
            _ordersTrader = ordersTrader;
            _orderBookMachine = orderBookMachine;
            _openOrders = openOrders;
            AllOrders = openOrders.Where(x => x.PairName == _ordersTrader.PairName).ToList();
            MainOrders = AllOrders.Where(x => x.Quantity >= balancesManager.MainCurrency.MaxTracerQuantityLimit)
                .ToList();
            TracerOrders = AllOrders.Where(x => x.Quantity < balancesManager.MainCurrency.MaxTracerQuantityLimit)
                .ToList();
        }

        private List<OpenOrders> GetActualOrdersPosition()
        {
            var ordersPosition = new List<OpenOrders>();

            return ordersPosition;
        }


        private bool CheckOrdersPosition()
        {
            return true;
        }

        public bool SetOrdersPair(decimal? rate, decimal? quantity, int count = 1)
        {
            if (AllOrders == null) return false;
            foreach (var openOrder in AllOrders)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_ordersTrader.BuyOrSell(Trader.Mode, _ordersTrader.PairName, rate, quantity) != null) break;
                }
            }
            return true;
        }

        public bool CancelAllOrdersPair()
        {
            if (AllOrders == null) return true;
            foreach (var openOrder in AllOrders)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (ProHub.MarketApi.Cancel(openOrder.OrderId)) break;
                }
            }
            return true;
        }

        public bool CancelAllOrdersPair(int orderGroup)
        {
            if (AllOrders == null) return false;
            var openOrdersPairTradingStyle = AllOrders.Where(x => x.orderGroup == orderGroup).ToList();
            if (openOrdersPairTradingStyle.Count == 0) return false;
            foreach (var openOrder in AllOrders)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (ProHub.MarketApi.Cancel(openOrder.OrderId)) break;
                }
            }
            return true;
        }

        public bool DeleteAllOpenOrdersMarket()
        {
            if (_openOrders == null) return false;
            foreach (var openOrder in _openOrders)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (ProHub.MarketApi.Cancel(openOrder.OrderId)) break;
                }
            }
            return true;
        }
    }
}