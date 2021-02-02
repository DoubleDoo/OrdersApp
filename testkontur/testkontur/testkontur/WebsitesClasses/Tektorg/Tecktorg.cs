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
    class Tecktorg
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


        public static void TecktorgStatic(string _startSearchDate, string _endSearchDate, string _startSearchPrice, string _endSearchPrice, ProgressBar _bar, Label _lbl, TextBox _tb)
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


        private static List<string> GetLinks(HtmlAgilityPack.HtmlDocument data, int curPage)
        {
            List<string> ordersForKey = new List<string>();
            if (data.DocumentNode.QuerySelector(".section-procurement__number") != null)
                if (int.Parse(data.DocumentNode.QuerySelector(".section-procurement__number").InnerText.Split(':')[1]) / 25 + 1 >= curPage)
                    //if (curPage*25<=)
                    //tb.Invoke(new Action(() => { tb.AppendText("search" + Environment.NewLine); }));////
                    foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".section-procurement__item-title"))
                    {
                        //if (node.QuerySelector("a").GetAttributeValue("href", null).Contains("epz"))
                        //{
                        //tb.Invoke(new Action(() => { tb.AppendText("+" + Environment.NewLine); }));////
                        ordersForKey.Add("https://www.tektorg.ru" + node.GetAttributeValue("href", null));
                        //}
                        //else ordersForKey.Add(node.QuerySelector("a").GetAttributeValue("href", null).Split('&')[0]);
                    }
            return ordersForKey;
        }

       

        public static string GenerateSearchLinkByPage(string keyWord, int page)
        {
            return "https://www.tektorg.ru" +
                "/procedures?lang=ru&q=" + keyWord + "&dpfrom=" + startSearchDate +
                "&dpto=" + endSearchDate + "&startpricefrom=" + startSearchPrice + "&startpriceto=" + endSearchPrice + "&page=" + page;
        }

       

        public static string GetType(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetTypeTecktorg(data);
        }
        public static string GetTypeTecktorg(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Способ закупки") || data.Text.Contains("Тип процедуры"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        if (node.QuerySelector("td").InnerText.Contains("Способ закупки"))
                        {
                            return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                        }
                        else if (node.QuerySelector("td").InnerText.Contains("Тип процедуры"))
                        {
                            return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                        }
                    }
                }
            }
            return "Процедура";
        }
      

        public static string GetOrderer(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetOrdererTecktorg(data);
        }
        public static string GetOrdererTecktorg(HtmlAgilityPack.HtmlDocument data)
        {
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
            {
                if (node.QuerySelector("td") != null)
                    if (node.QuerySelector("td").InnerText.Contains("Наименование организатора:"))
                    {
                        return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                    }
            }
            return "None";
        }

       
        public static string GetCity(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetCityTecktorg(data);
        }
        public static string GetCityTecktorg(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Почтовый адрес") || data.Text.Contains("Адрес поставки"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        if (node.QuerySelector("td").InnerText.Contains("Почтовый адрес"))
                        {
                            return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                        }
                        else if (node.QuerySelector("td").InnerText.Contains("Адрес поставки"))
                        {
                            return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }

   

        public static string GetDate(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetDateTecktorg(data);
        }

        public static string GetDateTecktorg(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Дата и время окончания") || data.Text.Contains("Дата окончания срока подачи") || data.Text.Contains("Дата окончания приема заявок"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        if (node.QuerySelector("td").InnerText.Contains("Дата и время окончания"))
                        {
                            return DataParser.ParseDate(node.QuerySelectorAll("td").ToArray()[1].InnerText);
                        }
                        else if (node.QuerySelector("td").InnerText.Contains("Дата окончания срока подачи"))
                        {
                            return DataParser.ParseDate(node.QuerySelectorAll("td").ToArray()[1].InnerText);
                        }
                        else if (node.QuerySelector("td").InnerText.Contains("Дата окончания приема заявок"))
                        {
                            return DataParser.ParseDate(node.QuerySelectorAll("td").ToArray()[1].InnerText);
                        }
                    }
                }
            }
            return "None";
        }
      


        public static string GetInformation(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetInformationTecktorg(data);
        }
        public static string GetInformationTecktorg(HtmlAgilityPack.HtmlDocument data)
        {
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".procedure__item-name"))
            {
                return DataParser.FixQuots(node.InnerText).Trim();
            }
            return "None";
        }


      

        public static string GetPrice(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetPriceForTecktorg(data);
        }


        public static string GetPriceForTecktorg(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Начальная (максимальная) цена контракта"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {

                    if (node.QuerySelector("td").InnerText.Contains("Начальная (максимальная) цена контракта"))
                    {
                        return DataParser.ParsePrice(node.QuerySelectorAll("td").ToArray()[1].InnerText);
                    }
                }
            }
            return "0";
        }
   

        public static HtmlAgilityPack.HtmlDocument OpenLinkOrder(string link)
        {
            tb.Invoke(new Action(() => { tb.AppendText(link + Environment.NewLine); }));
            return WebRequests.LoadPageStealth(link);
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
            return WebRequests.LoadPageStealth(GenerateSearchLinkByPage(keyWord, page));
        }

        public static void DownloadDataSearch(HtmlAgilityPack.HtmlDocument document, int page, ref bool noNewOrders)
        {
            bool qq = noNewOrders;
            //tb.Invoke(new Action(() => { tb.AppendText("Pre:" + qq.ToString() + Environment.NewLine); }));
            List<string> ordersForKey = new List<string>();
            ordersForKey = GetLinks(document, page);
            //tb.Invoke(new Action(() => { tb.AppendText("Search_Count:" + ordersForKey.Count + Environment.NewLine); }));
            noNewOrders = true;
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
                    //t.Start();
                }
                noNewOrders &= isExist;
            }
            qq = noNewOrders;
            // tb.Invoke(new Action(() => { tb.AppendText("After:" + qq.ToString() + Environment.NewLine); }));
        }


        public static void NextSearch(HtmlAgilityPack.HtmlDocument document, string keyword, int id, int page, ref bool noNewOrders)
        {
            if (!noNewOrders)
            {
                SearchTread sTread = new SearchTread(
                    keyword,
                    ++page,
                    OpenLinkSearch,
                    DownloadDataSearch,
                    NextSearch
                    );
                Thread t = new Thread(new ThreadStart(sTread.ThreadProc));
                lock (threadsSearchLock) { SearchThreads.Add(t); }
                //t.Start();
            }
            // tb.Invoke(new Action(() => { tb.AppendText("Search_finish:" + id + ":" + keyword + Environment.NewLine); }));
        }






    }
}
