using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using testkontur.OrderClasses;
using HtmlAgilityPack;
using System;
using Fizzler.Systems.HtmlAgilityPack;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

using testkontur.Threads;

namespace testkontur.WebsitesClasses
{
    public class Rosatom
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


        public static void RosatomStatic(string _startSearchDate, string _endSearchDate, string _startSearchPrice, string _endSearchPrice, ProgressBar _bar, Label _lbl, TextBox _tb)
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
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".even"))
            {
                if ((node.QuerySelector("p")).QuerySelector("a") != null)
                {
                    ordersForKey.Add("http://zakupki.rosatom.ru" + (node.QuerySelector("p")).QuerySelector("a").GetAttributeValue("href", null));
                }
            }
           // tb.Invoke(new Action(() => { tb.AppendText("Find.Count:" + ordersForKey.Count + Environment.NewLine); }));
            return ordersForKey;
        }

        public static string GenerateSearchLinkByPage(string keyWord, int page)
        {
            return "http://zakupki.rosatom.ru" +
           "/Web.aspx?node=currentorders&tso=1&tsl=1&sbflag=0&pricemon=0&os=" + keyWord + "&ostate=P&pricef=" + startSearchPrice + "&pricet=" +
           endSearchPrice + "&pubdates=" + startSearchDate + "&pubdatef=" + endSearchDate + "&pform=a&page="+page;
        }


   


        public static string GetType(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetTypeRosatom(data);
        }
        public static string GetTypeRosatom(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Способ закупки"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        if (node.QuerySelector("td").InnerText.Contains("Способ закупки"))
                        {
                            return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText);
                        }
                    }
                }
            }
            return "Закупка";
        }

      

        public static string GetOrderer(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetOrdererRosatom(data);
        }
        public static string GetOrdererRosatom(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Наименование организации") || data.Text.Contains("Официальное наименование"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        if (node.QuerySelector("td").InnerText.Contains("Наименование организации"))
                        {
                            return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                        }
                        else if (node.QuerySelector("td").InnerText.Contains("Официальное наименование"))
                        {
                            return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }

        


        public static string GetCity(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetCityRosatom(data);
        }
        public static string GetCityRosatom(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Место нахождения") || data.Text.Contains("Почтовый адрес"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        if (node.QuerySelector("td").InnerText.Contains("Место нахождения"))
                        {
                            return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                        }
                        else if (node.QuerySelector("td").InnerText.Contains("Почтовый адрес"))
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
            string linkl = "";
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".property-table"))
            {
                if (node.InnerHtml.Contains("Лоты"))
                {
                    if (!linkl.Contains("lot"))
                        linkl = "http://zakupki.rosatom.ru" + node.QuerySelector(".odd").QuerySelector("a").GetAttributeValue("href", null);
                }
            }
            if (linkl != "")
            {
                GetDateRosatomLots(linkl);
            }
            return GetDateRosatom(data);
        }
        public static string GetDateRosatom(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Дата и время окончания подачи предложений") || data.Text.Contains("Дата и время окончания подачи заявок"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        if (node.QuerySelector("td").InnerText.Contains("Дата и время окончания подачи предложений"))
                        {
                            return DataParser.ParseDate(node.QuerySelectorAll("td").ToArray()[1].InnerText);
                        }
                        else if (node.QuerySelector("td").InnerText.Contains("Дата и время окончания подачи заявок"))
                        {
                            return DataParser.ParseDate(node.QuerySelectorAll("td").ToArray()[1].InnerText);
                        }
                    }
                }
            }
            return "None";
        }
        public static string GetDateRosatomLots(string link)
        {
            HtmlAgilityPack.HtmlDocument document = WebRequests.LoadPage(link);
            if (document.Text.Contains("Дата и время окончания подачи предложений") || document.Text.Contains("Дата и время окончания подачи заявок"))
            {
                foreach (HtmlNode node in document.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        if (node.QuerySelector("td").InnerText.Contains("Дата и время окончания подачи предложений"))
                        {
                            return DataParser.ParseDate(node.QuerySelectorAll("td").ToArray()[1].InnerText);
                        }
                        else if (node.QuerySelector("td").InnerText.Contains("Дата и время окончания подачи заявок"))
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
            return GetInformationRosatom(data);
        }
        public static string GetInformationRosatom(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Наименование закупки"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    if (node.QuerySelector("td") != null)
                    {
                        if (node.QuerySelector("td").InnerText.Contains("Наименование закупки"))
                        {
                            return DataParser.FixQuots(node.QuerySelectorAll("td").ToArray()[1].InnerText).Trim();
                        }
                    }
                }
            }

            if (data.DocumentNode.QuerySelector("h1") != null)
                return DataParser.FixQuots(data.DocumentNode.QuerySelector("h1").InnerText).Trim();
            return "None";
        }

       

        public static string GetPrice(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetPriceForRosatom(data);
        }


        public static string GetPriceForRosatom(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("цена договора в рублях") || data.Text.Contains("Начальная цена лота"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr"))
                {
                    
                        if (node.QuerySelector("td").InnerText.Contains("цена договора в рублях"))
                        {
                            return DataParser.ParsePrice(node.QuerySelectorAll("td").ToArray()[1].InnerText);
                        } 
                        else if(node.QuerySelector("td").InnerText.Contains("Начальная цена лота")) 
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
            bool qq = noNewOrders;
            //tb.Invoke(new Action(() => { tb.AppendText("Pre:" + qq.ToString() + Environment.NewLine); }));
            List<string> ordersForKey = new List<string>();
            ordersForKey = GetLinks(document);
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


        public static void NextSearch(HtmlAgilityPack.HtmlDocument document, string keyword, int id, int page,ref bool noNewOrders)
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