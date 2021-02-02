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
    public class Etpgpb
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


        public static void EtpgpbStatic(string _startSearchDate, string _endSearchDate, string _startSearchPrice, string _endSearchPrice, ProgressBar _bar, Label _lbl, TextBox _tb)
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
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".procedure__link.procedure__infoTitle"))
            {
                if (node.GetAttributeValue("href", null).Contains("gos.etpgpb.ru"))
                {
                    ordersForKey.Add(node.GetAttributeValue("href", null));
                }
                else
                {
                    ordersForKey.Add("https://etpgpb.ru/" + node.GetAttributeValue("href", null));
                }
            }
            return ordersForKey;
        }

       

        public static string GenerateSearchLinkByPage(string keyWord, int page)
        {
            return "https://" +
           "etpgpb.ru/procedures/page/" + page + "/?procedure%5Bcategory%5D=actual&procedure%5Bsection%5D%5B0%5D=common&procedure%5Bsection%5D%5B1%5D=gazprom" +
           "&procedure%5Bsection%5D%5B2%5D=nelikvid&procedure%5Bsection%5D%5B3%5D=pao_gazpromneft&procedure%5Bsection%5D%5B5%5D=airports_russia&" +
           "procedure%5Bsection%5D%5B6%5D=fz44&procedure%5Bsection%5D%5B7%5D=nonresident&procedure%5Bsection%5D%5B4%5D=all&procedure%5Bpublished_from%5D=" + startSearchDate +
           "&procedure%5Bpublished_to%5D=" + endSearchDate + "&procedure%5Btype_procedure%5D%5B1%5D=%D0%97%D0%B0%D0%BF%D1%80%D0%BE%D1%81+%D0%BA%D0%BE%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%BE%D0%BA&" +
           "procedure%5Btype_procedure%5D%5B0%5D=%D0%9A%D0%BE%D0%BD%D0%BA%D1%83%D1%80%D1%81&procedure%5Btype_procedure%5D%5B7%5D=%D0%90%D1%83%D0%BA%D1%86%D0%B8%D0%BE%D0%BD&" +
           "procedure%5Btype_procedure%5D%5B2%5D=%D0%90%D1%83%D0%BA%D1%86%D0%B8%D0%BE%D0%BD+%D0%BD%D0%B0+%D0%BF%D0%BE%D0%B2%D1%8B%D1%88%D0%B5%D0%BD%D0%B8%D0%B5&" +
           "procedure%5Btype_procedure%5D%5B3%5D=%D0%90%D1%83%D0%BA%D1%86%D0%B8%D0%BE%D0%BD+%D0%BD%D0%B0+%D0%BF%D0%BE%D0%BD%D0%B8%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5&" +
           "procedure%5Btype_procedure%5D%5B4%5D=%D0%9F%D0%BE%D0%BF%D0%BE%D0%B7%D0%B8%D1%86%D0%B8%D0%BE%D0%BD%D0%BD%D1%8B%D0%B5+%D1%82%D0%BE%D1%80%D0%B3%D0%B8&" +
           "procedure%5Btype_procedure%5D%5B6%5D=%D0%A6%D0%B5%D0%BD%D0%BE%D0%B2%D0%BE%D0%B9+%D0%B7%D0%B0%D0%BF%D1%80%D0%BE%D1%81&procedure%5Btype_procedure%5D%5B5%5D=%D0%92%D1%81%D0%B5&" +
           "procedure%5Bmin_price%5D=" + startSearchPrice + "&procedure%5Bmax_price%5D=" + endSearchDate + "&procedure%5Bprms%5D%5Bwithout_eds%5D=true&procedure%5Bprms%5D%5Bwith_eds%5D=true&" +
           "procedure%5Bprms%5D%5Bwithout_guarantee_application%5D=true&procedure%5Bprms%5D%5Bwith_guarantee_application%5D=true&procedure%5Bprms%5D%5Ball%5D=true&" +
           "search=" + keyWord;
        }

        
        public static string GetType(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("gos.etpgpb.ru")) return GetTypeForEpz(data);
            return GetTypeForNonEpz(data);
        }
        public static string GetTypeForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Способ определения поставщика"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".form-group"))
                {
                    if (node.QuerySelector(".col-sm-4.control-label") != null)
                    {
                        if (node.QuerySelector(".col-sm-4.control-label").InnerText.Contains("Способ определения поставщика"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".form-control-static.formInfo").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }
        public static string GetTypeForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Способ закупки") || data.Text.Contains("Категория"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".block__docs_container_cell"))
                {
                    if (node.QuerySelector(".block__docs_container_title") != null)
                    {
                        if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Способ закупки"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                        else if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Категория"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }


       

        public static string GetOrderer(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("gos.etpgpb.ru")) return GetOrdererForEpz(data);
            return GetOrdererForNonEpz(data);
        }
        public static string GetOrdererForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Наименование организации"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".form-group"))
                {
                    if (node.QuerySelector(".col-sm-4.control-label") != null)
                    {
                        if (node.QuerySelector(".col-sm-4.control-label").InnerText.Contains("Наименование организации"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".form-control-static.formInfo").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }

        public static string GetOrdererForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Наименование организатора") || data.Text.Contains("Продавец / инициатор продажи"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".block__docs_container_cell"))
                {
                    if (node.QuerySelector(".block__docs_container_title") != null)
                    {
                        if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Наименование организатора"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                        else if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Продавец / инициатор продажи"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }
     
        public static string GetCity(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("gos.etpgpb.ru")) return GetCityForEpz(data);
            return GetCityForNonEpz(data);
        }
        public static string GetCityForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Почтовый адрес"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".form-group"))
                {
                    if (node.QuerySelector(".col-sm-4.control-label") != null)
                    {
                        if (node.QuerySelector(".col-sm-4.control-label").InnerText.Contains("Почтовый адрес"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".form-control-static.formInfo").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }

        public static string GetCityForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Адрес местонахождения") || data.Text.Contains("Почтовый адрес"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".block__docs_container_cell"))
                {
                    if (node.QuerySelector(".block__docs_container_title") != null)
                    {
                        if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Почтовый адрес"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                        else if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Адрес местонахождения"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }

       


        public static string GetDate(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("gos.etpgpb.ru")) return GetDateForEpz(data);
            return GetDateForNonEpz(data);
        }
        public static string GetDateForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Дата и время окончания срока подачи заявок"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".form-group"))
                {
                    if (node.QuerySelector(".col-sm-4.control-label") != null)
                    {
                        if (node.QuerySelector(".col-sm-4.control-label").InnerText.Contains("Дата и время окончания срока подачи заявок"))
                        {
                            return DataParser.ParseDate(node.QuerySelector(".form-control-static.formInfo").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }
        public static string GetDateForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Дата подведения итого") || data.Text.Contains("Дата окончания срока рассмотрения заявок") || data.Text.Contains("Дата предоставления ответа на запрос"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".block__docs_container_cell"))
                {
                    if (node.QuerySelector(".block__docs_container_title") != null)
                    {
                        if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Дата подведения итого"))
                        {
                            return DataParser.ParseDate(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                        else if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Дата окончания срока рассмотрения заявок"))
                        {
                            return DataParser.ParseDate(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                        else if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Дата предоставления ответа на запрос"))
                        {
                            return DataParser.ParseDate(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }

       

        public static string GetInformation(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("gos.etpgpb.ru")) return GetInformationForEpz(data);
            return GetInformationForNonEpz(data);
        }
        public static string GetInformationForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Наименование закупки"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".form-group"))
                {
                    if (node.QuerySelector(".col-sm-4.control-label") != null)
                    {
                        if (node.QuerySelector(".col-sm-4.control-label").InnerText.Contains("Наименование закупки"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".form-control-static.formInfo").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }
        public static string GetInformationForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Наименование закупки") || data.Text.Contains("Наименование процедуры"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".block__docs_container_cell"))
                {
                    if (node.QuerySelector(".block__docs_container_title") != null)
                    {
                        if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Наименование закупки"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                        else if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Наименование процедуры"))
                        {
                            return DataParser.FixQuots(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
                        }
                    }
                }
            }
            return "None";
        }





        public static string GetPrice(HtmlAgilityPack.HtmlDocument data, string link)
        {
            if (link.Contains("gos.etpgpb.ru")) return GetPriceForEpz(data);
            return GetPriceForNonEpz(data);
        }
        public static string GetPriceForEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Начальная (максимальная) цена контракта"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".form-group"))
                {
                    if (node.QuerySelector(".col-sm-4.control-label") != null)
                    {
                        if (node.QuerySelector(".col-sm-4.control-label").InnerText.Contains("Начальная (максимальная) цена контракта"))
                        {
                            return DataParser.ParsePriceCustom(node.QuerySelector(".col-sm-8").InnerText).Trim();
                        }
                    }
                }
            }
            return "0";
        }
        public static string GetPriceForNonEpz(HtmlAgilityPack.HtmlDocument data)
        {
            if (data.Text.Contains("Начальная цена"))
            {
                foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".block__docs_container_cell"))
                {
                    if (node.QuerySelector(".block__docs_container_title") != null)
                    {
                        if (node.QuerySelector(".block__docs_container_title").InnerText.Contains("Начальная цена"))
                        {
                            return DataParser.ParsePriceCustom(node.QuerySelector(".block__docs_container_info").InnerText).Trim();
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
            //tb.Invoke(new Action(() => { tb.AppendText("Search_Count:" + ordersForKey.Count + Environment.NewLine); }));
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
                lock (threadsSearchLock) { SearchThreads.Add(t); }
                //t.Start();
            }
            // tb.Invoke(new Action(() => { tb.AppendText("Search_finish:" + id + ":" + keyword + Environment.NewLine); }));
        }
    }
}
