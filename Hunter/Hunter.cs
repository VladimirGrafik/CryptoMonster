using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Components.Db;
using Components.Db.Maps;
using Components.Helpers;
using NHibernate.Linq;
using Components.Markets;
using Components.Markets.AdaptersApi;

namespace Hunter
{
    public class Hunter
    {
        private const int CountPeriod = 20;
        private const int CountPeriodMinimal = 10;
        private readonly MarketApi _marketApi;
        private readonly string _pairConnector;
        private readonly List<SumPairs> _sumPairs;
        private readonly List<DefaultPairs> _savedPairs;
        private readonly bool _updatePairs;
        private readonly List<DefaultPairs> _enabledPairs;
        private readonly int _uTimeIdNow;
        private readonly bool _deleteOldPairs;
        private readonly List<CatchPairs> _catchPairs;
        private readonly List<List<CatchPairs>> _catchPairs15;
        private readonly List<List<CatchPairs>> _catchPairs60;


        public Hunter(List<CatchPairs> catchPairs)
        {
            _marketApi = ProHub.MarketApi;
            _sumPairs = _marketApi.GetSumPairs();
            _savedPairs = GetSavedPairs();
            _updatePairs = UpdateDifferencePairs();
            _enabledPairs = GetEnabledPairs();
            _uTimeIdNow = UnixTime.Now();
            _deleteOldPairs = DeleteOldPairs();
            _catchPairs = catchPairs;
            _catchPairs15 = GetCatchPairsPeriod(900);
            _catchPairs60 = GetCatchPairsPeriod(3600);
        }


        private List<DefaultPairs> GetSavedPairs()
        {
            using (var cmSe = CmSetting.OpenSession())
            {
                var savedPairs = cmSe.QueryOver<DefaultPairs>()
                    .Where(x => x.MarketName == _marketApi.MarketName).List();
                if (savedPairs == null)
                    throw new Exception("Нет сохраненных валютных пар для биржи!");
                return savedPairs.ToList();
            }
        }

        private bool UpdateDifferencePairs()
        {
            if (_marketApi.UpdateMarketPairs == false) return false;
            var missingPairsNames = _sumPairs.Select(x => x.PairName).Except(_savedPairs.Select(x => x.PairName))
                .ToList();
            var missingPairs = new List<SumPairs>();
            foreach (var missingPairsName in missingPairsNames)
                missingPairs.Add(_sumPairs.SingleOrDefault(x => x.PairName == missingPairsName));
            if (missingPairs.Count != 0)
            {
                using (var cmSe = CmSetting.OpenSession())
                {
                    using (var trans = cmSe.BeginTransaction())
                    {
                        foreach (var missingPair in missingPairs)
                        {
                            var unSavedPairs = new DefaultPairs()
                            {
                                MainCurrency = missingPair.MainCurrency,
                                SecondCurrency = missingPair.SecondCurrency,
                                MarketName = _marketApi.MarketName,
                                PairName = missingPair.PairName,
                                PairLabel = missingPair.MainCurrency + "_" + missingPair.SecondCurrency + "_" +
                                            _marketApi.MarketName
                            };
                            cmSe.Save(unSavedPairs);
                        }
                        trans.Commit();
                        return true;
                    }
                }
            }
            var nonexistentPairs = _savedPairs.Select(x => x.PairName).Except(_sumPairs.Select(x => x.PairName))
                .ToList();
            if (nonexistentPairs.Count != 0)
            {
                using (var cmSe = CmSetting.OpenSession())
                {
                    using (var trans = cmSe.BeginTransaction())
                    {
                        foreach (var nonexistentPair in nonexistentPairs)
                        {
                            var deletedPairs = cmSe.QueryOver<DefaultPairs>().List().Where(x =>
                                    x.PairName == nonexistentPair && x.MarketName == _marketApi.MarketName)
                                .ToList();
                            foreach (var deletedPair in deletedPairs)
                                cmSe.Delete(deletedPair);
                        }
                        trans.Commit();
                        return true;
                    }
                }
            }
            return false;
        }


        private List<DefaultPairs> GetEnabledPairs()
        {
            var enabledPairs = _savedPairs.Where(x => x.Enable == 1).ToList();
            if (enabledPairs.Count == 0)
                throw new Exception("Нет включенных валютных пар!");
            return enabledPairs;
        }


        private void FilteringPairs()
        {
            throw new NotImplementedException();
        }

        private bool DeleteOldPairs()
        {
            using (var cmTh = CmThinking.OpenSession())
            {
                using (var trans = cmTh.BeginTransaction())
                {
                    if (_uTimeIdNow != 0)
                    {
                        var oldPairsTimeId = _uTimeIdNow - 3700;
                        var oldPairs = cmTh.Query<CatchPairs>().Where(x => x.UTime < oldPairsTimeId).ToList();
                        foreach (var oldPair in oldPairs)
                        {
                            cmTh.Delete(oldPair);
                        }
                    }
                    trans.Commit();
                }
            }
            return true;
        }

        private List<List<CatchPairs>> GetCatchPairsPeriod(int period)
        {
            var step = period / CountPeriod;
            if (_catchPairs == null) return null;
            var timeCounter = _uTimeIdNow;
            var catchPairsPeriod = new List<List<CatchPairs>>();
            for (var i = 0; i < CountPeriod; i++)
            {
                var uTimeId = _catchPairs.Select(x => x.UTime)
                    .FirstOrDefault(x => x < timeCounter && x > timeCounter - 60);
                if (uTimeId == null) return catchPairsPeriod;
                var catchPairsPeriodTemp = _catchPairs.Where(x => x.UTime == uTimeId).ToList();
                if (catchPairsPeriodTemp.Count == 0) return catchPairsPeriod;
                catchPairsPeriod.Add(catchPairsPeriodTemp);
                timeCounter -= step;
            }
            return catchPairsPeriod;
        }

        private void GetStatistic()
        {
            if (_sumPairs.Count == 0) return;
            var mainCurrencies = ProHub.MarketApi.MainCurrencies;
            mainCurrencies.Add(new MainCurrencies()
            {
                CurrencyName = "STATISTIC",
                MainPairName = "STATISTIC",
                MainCurrency = "STATISTIC",
                SecondCurrency = "STATISTIC"
            });
            foreach (var mainCurrency in mainCurrencies)
            {
                var sumPairsTemp = mainCurrency.CurrencyName == "STATISTIC"
                    ? _sumPairs
                    : _sumPairs.Where(x => x.MainCurrency == mainCurrency.CurrencyName);

                decimal? priceP15 = 0,
                    priceP60 = 0,
                    pricePDay = 0,
                    priceP15Sma = 0,
                    priceP60Sma = 0,
                    priceP15Ema = 0,
                    priceP60Ema = 0,
                    prismPDay = 0,
                    prismPDayHeight = 0,
                    volumeP15 = 0;
                foreach (var sumPair in sumPairsTemp)
                {
                    priceP15 += sumPair.PriceP15;
                    priceP60 += sumPair.PriceP60;
                    pricePDay += sumPair.PricePDay;
                    priceP15Sma += sumPair.PriceP15Sma;
                    priceP60Sma += sumPair.PriceP60Sma;
                    priceP15Ema += sumPair.PriceP15Ema;
                    priceP60Ema += sumPair.PriceP60Ema;
                    prismPDay += sumPair.PrismPDay;
                    prismPDayHeight += sumPair.PrismPDayHeihgt;
                    volumeP15 += sumPair.VolumeP15;
                }
                _sumPairs.Add(new SumPairs()
                    {
                        PairName = mainCurrency.CurrencyName,
                        MainCurrency = "STATISTIC",
                        SecondCurrency = "STATISTIC",
                        PriceP15 = priceP15 / _sumPairs.Count,
                        PriceP60 = priceP60 / _sumPairs.Count,
                        PricePDay = pricePDay / _sumPairs.Count,
                        PriceP15Sma = priceP15Sma / _sumPairs.Count,
                        PriceP60Sma = priceP60Sma / _sumPairs.Count,
                        PriceP15Ema = priceP15Ema / _sumPairs.Count,
                        PriceP60Ema = priceP60Ema / _sumPairs.Count,
                        PrismPDay = prismPDay / _sumPairs.Count,
                        PrismPDayHeihgt = prismPDayHeight / _sumPairs.Count,
                        VolumeP15 = volumeP15 / _sumPairs.Count
                    }
                );
            }
        }

        public void RunHunter()
        {
            BuildPriceVolumePercent();
            GetStatistic();
            WriteCatchPairs();
        }

        private void BuildPriceVolumePercent()
        {
            if (_catchPairs15 == null || _catchPairs60 == null) return;
            foreach (var sumPair in _sumPairs)
            {
                try
                {
                    sumPair.PriceP15 = GetPricePercent(_catchPairs15, sumPair);
                    sumPair.PriceP60 = GetPricePercent(_catchPairs60, sumPair);
                    sumPair.PriceP15Sma = GetPricePercentSma(_catchPairs15, sumPair);
                    sumPair.PriceP60Sma = GetPricePercentSma(_catchPairs60, sumPair);
                    sumPair.PriceP15Ema = GetPricePercentEma(_catchPairs15, sumPair, 900);
                    sumPair.PriceP60Ema = GetPricePercentEma(_catchPairs60, sumPair, 3600);
                    sumPair.VolumeP15 = GetVolumePercent(_catchPairs15, sumPair);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        private decimal? GetPricePercent(List<List<CatchPairs>> catchPairsPeriod, SumPairs sumPair)
        {
            if (catchPairsPeriod?.Count < CountPeriodMinimal) return null;
            var catchPairLast = catchPairsPeriod?.LastOrDefault()?
                .SingleOrDefault(x => x.PairName == sumPair.PairName);
            if (sumPair?.LastPrice == 0 || catchPairLast?.LastPrice == 0) return null;
            var division = (sumPair?.LastPrice / catchPairLast?.LastPrice) - 1;
            if (division == 0) return 0;
            return division * 100;
        }

        private decimal? GetPricePercentSma(List<List<CatchPairs>> catchPairsPeriod, SumPairs sumPair)
        {
            if (catchPairsPeriod?.Count < CountPeriodMinimal) return null;
            decimal? sum = 0;
            var i = 0;
            for (; i < catchPairsPeriod?.Count; i++)
            {
                var catchPairPast = catchPairsPeriod.ElementAtOrDefault(i)?
                    .SingleOrDefault(x => x.PairName == sumPair.PairName);
                if (sumPair.LastPrice == 0 || catchPairPast?.LastPrice == 0) return null;
                sum += catchPairPast?.LastPrice;
            }
            var division = (sumPair.LastPrice / (sum / i)) - 1;
            if (division == 0) return 0;
            return division * 100;
        }


        private decimal? GetPricePercentEma(List<List<CatchPairs>> catchPairsPeriod, SumPairs sumPair, int period)
        {
            if (catchPairsPeriod?.Count < CountPeriodMinimal) return null;
            var catchPairFirst = catchPairsPeriod?.FirstOrDefault()?
                .SingleOrDefault(x => x.PairName == sumPair.PairName);
            var catchPairLast = catchPairsPeriod?.LastOrDefault()?
                .SingleOrDefault(x => x.PairName == sumPair.PairName);
            decimal? lastPriceEma;
            if (period == 900) lastPriceEma = catchPairLast?.PriceP15Ema;
            else if (period == 3600) lastPriceEma = catchPairLast?.PriceP60Ema;
            else throw new Exception("Dont satisfactory period!");
            if (sumPair.LastPrice == 0 || catchPairLast?.LastPrice == 0) return null;
            var diferencePercent = (sumPair.LastPrice / catchPairLast?.LastPrice) - 1;
            if (diferencePercent != 0) diferencePercent *= 100;
            else diferencePercent = 0;
            if (lastPriceEma == null) return diferencePercent;
            var diferenceEma = diferencePercent - lastPriceEma;
            if (diferenceEma == 0) return lastPriceEma;
            return lastPriceEma + 0.7M * diferenceEma;
        }

        private decimal? GetVolumePercent(List<List<CatchPairs>> catchPairsPeriod, SumPairs sumPair)
        {
            if (catchPairsPeriod?.Count < CountPeriodMinimal) return null;
            var catchPairLast = catchPairsPeriod?.LastOrDefault()?
                .SingleOrDefault(x => x.PairName == sumPair.PairName);
            if (sumPair?.VolumeDay == null || catchPairLast?.VolumeDay == null) return null;
            return ((sumPair.VolumeDay / catchPairLast.VolumeDay) - 1) * 100;
        }

        private void WriteCatchPairs()
        {
            using (var cmTh = CmThinking.OpenSession())
            {
                using (var trans = cmTh.BeginTransaction())
                {
                    var tempVar = 0;
                    foreach (var sumPair in _sumPairs)
                    {
                        tempVar++;
                        var catchPairs = new CatchPairs
                        {
                            PairName = sumPair.PairName,
                            MainCurrency = sumPair.MainCurrency,
                            SecondCurrency = sumPair.SecondCurrency,
                            DateTime = DateTime.Now,
                            LastPrice = sumPair.LastPrice,
                            PriceP15 = sumPair.PriceP15,
                            PriceP60 = sumPair.PriceP60,
                            PricePDay = sumPair.PricePDay,
                            PriceP15Sma = sumPair.PriceP15Sma,
                            PriceP60Sma = sumPair.PriceP60Sma,
                            PriceP15Ema = sumPair.PriceP15Ema,
                            PriceP60Ema = sumPair.PriceP60Ema,
                            MaxBidsDay = sumPair.MaxBidsDay,
                            MinAsksDay = sumPair.MinAsksDay,
                            PrismPDay = sumPair.PrismPDay,
                            PrismPDayHeight = sumPair.PrismPDayHeihgt,
                            VolumeP15 = sumPair.VolumeP15,
                            VolumeDay = sumPair.VolumeDay,
                            UTime = _uTimeIdNow
                        };
                        cmTh.Save(catchPairs);
                    }
                    trans.Commit();
                }
            }
        }
    }
}