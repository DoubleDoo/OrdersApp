using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using testkontur.OrderClasses;
using System.Text.RegularExpressions;
using System;
using System.Windows.Forms;

using testkontur.Threads;

namespace testkontur.WebsitesClasses
{
    class ZakupkiGov
    {
        static ProgressBar bar;
        static Label lbl;
        static TextBox tb;
        private static string startSearchDate;
        private static string endSearchDate;
        private static string startSearchPrice;
        private static string endSearchPrice;

        public static List<Order> orderstest = new List<Order>();
        private static object ListLock = new object();

        public static HashSet<string> linksSettest = new HashSet<string>();
        private static object SetLock = new object();

        private static List<Thread> SearchThreads = new List<Thread>();
        private static List<Thread> OrdersThreads = new List<Thread>();

        private static object threadsSearchLock = new object();
        private static object threadsOrdersLock = new object();


        public static HashSet<string> linksSkip = new HashSet<string>();
        private static object SkipLock = new object();


        public static void ZakupkiGovStatic(string _startSearchDate, string _endSearchDate, string _startSearchPrice, string _endSearchPrice, ProgressBar _bar, Label _lbl, TextBox _tb)
        {
            startSearchDate = _startSearchDate;
            endSearchDate = _endSearchDate;
            startSearchPrice = _startSearchPrice;
            endSearchPrice = _endSearchPrice;
            bar = _bar;
            lbl = _lbl;
            tb = _tb;
        }

        public static List<Thread> Process(List<string> keyWords)
        {
            foreach (string key in keyWords)
            {
                SearchTread sTread = new SearchTread(
                        key,
                        1,
                        OpenLinkSearch,
                        DownloadDataSearch,
                        NextSearch
                        );
                Thread t = new Thread(new ThreadStart(sTread.ThreadProc));
                lock (threadsSearchLock) { SearchThreads.Add(t); }
                //t.Start();
            }
            int scount = SearchThreads.Count();
            while (scount > 0)
            {

                List<Thread> or = SearchThreads.Take(10).ToList<Thread>();
                foreach (Thread q in or)
                {
                    q.Start();
                }
                foreach (Thread q in or)
                {
                    q.Join();
                    Thread.Sleep(100);
                }
                lock (threadsSearchLock)
                {
                    foreach (Thread q in or)
                    {
                        SearchThreads.Remove(q);
                    }
                }
                scount = SearchThreads.Count();
                tb.Invoke(new Action(() => { tb.AppendText("Search.Count:" + scount + Environment.NewLine); }));

            }

            return OrdersThreads;
        }



        private static List<string> GetLinks(HtmlAgilityPack.HtmlDocument data)
        {
            List<string> ordersForKey = new List<string>();
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".registry-entry__header-mid__number"))
            {
                if (node.QuerySelector("a").GetAttributeValue("href", null).Contains("epz"))
                {
                    ordersForKey.Add("https://zakupki.gov.ru" + node.QuerySelector("a").GetAttributeValue("href", null).Split('&')[0]);
                }
                else ordersForKey.Add(node.QuerySelector("a").GetAttributeValue("href", null).Split('&')[0]);
            }
            return ordersForKey;
        }



        public static string GenerateSearchLinkByPage(string keyWord, int page)
        {
            return "https://zakupki.gov.ru/epz/order/extendedsearch/results.html?" +
                "searchString=" + keyWord + "&morphology=on&" +
                "search-filter=Дате+размещения&pageNumber=" + page + "&sortDirection=false&recordsPerPage=_50&showLotsInfoHidden=false&sortBy=UPDATE_DATE&fz44=on&" +
                "fz223=on&ppRf615=on&fz94=on&af=on&priceFromGeneral=" + startSearchPrice + "&priceToGeneral=" + endSearchPrice + "&currencyIdGeneral=-1&" +
                "publishDateFrom=" + startSearchDate + "&publishDateTo=" + endSearchDate + "&OrderPlacementSmallBusinessSubject=on&OrderPlacementRnpData=on&OrderPlacementExecutionRequirement=on&orderPlacement94_0=0&orderPlacement94_1=0&orderPlacement94_2=0";
        }



        public static string GetType(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("epz")) return GetTypeForEpz(data);
            return GetTypeForNonEpz(data);
        }
        public static string GetTypeForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return EpzTemplate(data, "Способ определения поставщика");
        }
        public static string GetTypeForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return NonEpzTemplate(data, "Способ размещения закупки");
        }


        public static string GetOrderer(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("epz")) return GetOrdererForEpz(data);
            return GetOrdererForNonEpz(data);
        }
        public static string GetOrdererForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return EpzTemplate(data, "Организация, осуществляющая размещение");
        }
        public static string GetOrdererForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return NonEpzTemplate(data, "Наименование организации");
        }


        public static string GetCity(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("epz")) return GetCityForEpz(data);
            return GetCityForNonEpz(data);
        }
        public static string GetCityForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return EpzTemplate(data, "Почтовый адрес");
        }
        public static string GetCityForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return NonEpzTemplate(data, "Почтовый адрес");
        }


        public static string GetDate(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("epz")) return GetDateForEpz(data);
            return GetDateForNonEpz(data);
        }
        public static string GetDateForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Дата и время окончания"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".blockInfo__section"))
                {
                    if (node.QuerySelector(".section__title") != null)
                    {
                        if (node.QuerySelector(".section__title").InnerText.Contains("Дата и время окончания"))
                        {
                            return DataParser.ParseDate(node.QuerySelector(".section__info").InnerText);
                        }
                    }
                }
            }
            return "None";
        }
        public static string GetDateForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return DataParser.ParseDate(NonEpzTemplate(data, "Дата и время окончания"));
        }


        public static string GetInformation(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("epz")) return GetInformationForEpz(data);
            return GetInformationForNonEpz(data);
        }
        public static string GetInformationForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return EpzTemplate(data, "Наименование объекта закупки");
        }
        public static string GetInformationForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return NonEpzTemplate(data, "Наименование закупки");
        }

        public static string GetPrice(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("epz")) return GetPriceForEpz(data);
            return GetPriceForNonEpz(link);
        }
        public static string GetPriceForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            return DataParser.ParsePrice(EpzTemplate(data, "Начальная (максимальная) цена контракта", "Максимальное значение цены контракта"));
        }
        public static string GetPriceForNonEpz(string link)
        {
            HtmlAgilityPack.HtmlDocument document = WebRequests.LoadPage(link.Replace("common-info", "lot-list"));
            if (document.Text.Contains("Начальная (максимальная) цена"))
            {
                foreach (HtmlNode node in document.DocumentNode.QuerySelectorAll("tr"))
                {
                    foreach (HtmlNode node1 in node.QuerySelectorAll("td"))
                    {
                        if (node1.InnerText.Contains("Начальная (максимальная) цена"))
                        {
                            return DataParser.ParsePrice(node1.InnerText);
                        }
                    }
                }
            }
            return "0";
        }




        public static HtmlAgilityPack.HtmlDocument OpenLinkOrder(string link)
        {
            tb.Invoke(new Action(() => { tb.AppendText(link + Environment.NewLine); }));
            return WebRequests.LoadPage(link);
        }

        public static void DownloadDataOrder(HtmlAgilityPack.HtmlDocument document, string link)
        {
            Order ord = new Order(link);
            int i = 0;
            int g = 0;
            bool skiped = false;
            while (GetInformation(document, ord.link) == "None")
            {
                i++;
                Thread.Sleep(1000);
                if (i > 10)
                {
                    /* if (g > 3)
                     { break; }
                     document = WebRequests.LoadPage(link);
                     i = 0;
                     g++;*/
                    lock (SkipLock)
                    {
                        linksSkip.Add(link);
                    }
                    skiped = true;
                    break;
                }

            }
            if (!skiped)
            {
                ord.type = GetType(document, ord.link);
                ord.orderer = GetOrderer(document, ord.link);
                ord.city = GetCity(document, ord.link);
                ord.date = GetDate(document, ord.link);
                ord.info = GetInformation(document, ord.link);
                ord.federal = DataParser.GetFederal(ord.city);
                ord.price = GetPrice(document, ord.link);
                lock (ListLock)
                {
                    orderstest.Add(ord);
                }
            }
        }

        public static void NextOrder(int id, string link)
        {
            //   tb.Invoke(new Action(() => { tb.AppendText("   Order_finish:"+ id+":"+link+ Environment.NewLine); }));
        }
        public static HtmlAgilityPack.HtmlDocument OpenLinkSearch(string keyWord, int page)
        {
            //tb.Invoke(new Action(() => { tb.AppendText("Search_Page:" + page + Environment.NewLine); }));
            return WebRequests.LoadPage(GenerateSearchLinkByPage(keyWord, page));
        }

        public static void DownloadDataSearch(HtmlAgilityPack.HtmlDocument document, int page, ref bool noNewOrders)
        {
            List<string> ordersForKey = new List<string>();
            ordersForKey = GetLinks(document);
            foreach (string linkString in ordersForKey)
            {
                bool isExist = false;
                lock (SetLock)
                {
                    isExist = linksSettest.Contains(linkString);
                }
                if (!isExist)
                {
                    lock (SetLock)
                    {
                        linksSettest.Add(linkString);
                    }
                    OrdersTread oTread = new OrdersTread(
                    linkString,
                    OpenLinkOrder,
                    DownloadDataOrder,
                    NextOrder
                    );
                    Thread t = new Thread(new ThreadStart(oTread.ThreadProc));
                    lock (threadsOrdersLock) { OrdersThreads.Add(t); }
                }
            }
        }


        public static void NextSearch(HtmlAgilityPack.HtmlDocument document, string keyword, int id, int page, ref bool noNewOrders)
        {
            if (GetLinks(document).Count > 0)
            {
                SearchTread sTread = new SearchTread(
                    keyword,
                    ++page,
                    OpenLinkSearch,
                    DownloadDataSearch,
                    NextSearch
                    );
                Thread t = new Thread(new ThreadStart(sTread.ThreadProc));
                lock (threadsSearchLock) { SearchThreads.Add(t);}
            }
        }


        public static string EpzTemplate(HtmlAgilityPack.HtmlDocument data, params string[] keys)
        {
            bool searchAccepted = false;
            foreach (string key in keys)
            {
                if (data.Text.Contains(key))
                {
                    searchAccepted = true;
                }

            }
            if (searchAccepted)
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".blockInfo__section.section"))
                {
                    if (node.QuerySelector(".section__title") != null)
                    {
                        foreach (string key in keys)
                        {
                            if (node.QuerySelector(".section__title").InnerText.Contains(key))
                            {
                                return DataParser.FixQuots(node.QuerySelector(".section__info").InnerText).Trim();
                            }

                        }
                    }
                }
            }
            return "None";
        }


        public static string NonEpzTemplate(HtmlAgilityPack.HtmlDocument data, params string[] keys)
        {
            bool searchAccepted = false;
            foreach (string key in keys)
            {
                if (data.Text.Contains(key))
                {
                    searchAccepted = true;
                }

            }
            if (searchAccepted)
            {

                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        foreach (string key in keys)
                        {
                            if (node.QuerySelector("td").InnerText.Contains(key))
                            {
                                return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                            }
                        }
                    }
                }

            }
            return "None";
        }


    }
}
