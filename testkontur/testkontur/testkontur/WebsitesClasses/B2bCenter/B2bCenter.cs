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
    public class B2bCenter
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


        public static void B2bCenterStatic(string _startSearchDate, string _endSearchDate, string _startSearchPrice, string _endSearchPrice, ProgressBar _bar, Label _lbl, TextBox _tb)
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
                        0,
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
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".search-results-title"))
            {
                ordersForKey.Add("https://www.b2b-center.ru/" + node.GetAttributeValue("href", null).Split('#')[0]);
            }
            return ordersForKey;
        }



        public static string GenerateSearchLinkByPage(string keyWord, int page)
        {
            return "https://www.b2b-center.ru" +
                 "/market/?f_keyword=" + keyWord + "&searching=1&company_type=2&price_start=" + startSearchPrice + "&price_end=" + endSearchPrice +
                 "&price_currency=0&date=1&date_start_dmy=" + startSearchDate + "&date_end_dmy=" + endSearchDate + "&trade=buy&lot_type=0&from=" + page * 20 + "#search-result";
        }


        public static string GetType(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetTypeB2B(data);
        }
        public static string GetTypeB2B(HtmlAgilityPack.HtmlDocument data)
        {
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("h1.h3"))
            {
                if (node.InnerText.Contains("№"))
                {
                    return DataParser.FixQuots(node.InnerText).Split('#')[0].Trim();
                }
            }
            return "Закупка";
        }


    
        public static string GetOrderer(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetOrdererB2B(data);
        }
        public static string GetOrdererB2B(HtmlAgilityPack.HtmlDocument data)
        {
            HtmlNode node = data.DocumentNode.SelectSingleNode("//tr[@id='trade-info-organizer-name']");
            if (node != null)
                return DataParser.FixQuots(node.QuerySelector("span").InnerText).Trim();
            return "None";
        }
 
       

        public static string GetCity(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetCityB2B(data);
        }
        public static string GetCityB2B(HtmlAgilityPack.HtmlDocument data)
        {
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr.c1"))
            {
                if (node.QuerySelector("td.fname") != null)
                    if (node.QuerySelector("td.fname").InnerText.Contains("Адрес места поставки"))
                    {
                        return DataParser.FixQuots(node.QuerySelectorAll("td").ToList()[1].InnerText).Trim();
                    }

            }
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll("tr.c2"))
            {
                if (node.QuerySelector("td.fname") != null)
                    if (node.QuerySelector("td.fname").InnerText.Contains("Адрес места поставки"))
                    {
                        return DataParser.FixQuots(node.QuerySelectorAll("td").ToList()[1].InnerText).Trim();
                    }

            }
            return "None";
        }


     



        public static string GetDate(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetDateB2B(data);
        }
        public static string GetDateB2B(HtmlAgilityPack.HtmlDocument data)
        {
            HtmlNode node = data.DocumentNode.SelectSingleNode("//tr[@id='trade_info_date_end']");
            if (node != null)
            {
                return DataParser.ParseDate(node.QuerySelectorAll("td").ToList()[1].InnerText);
            }
            return "None";
        }
 
        public static string GetInformation(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetInformationB2B(data);
        }
        public static string GetInformationB2B(HtmlAgilityPack.HtmlDocument data)
        {
            foreach (HtmlNode node in data.DocumentNode.QuerySelectorAll(".expandable-text.full"))
            {
                if (node.QuerySelector("span") != null)
                {

                    return DataParser.FixQuots(node.QuerySelector("span").InnerText).Trim();
                }

            }

            return "None";
        }
      
        public static string GetPrice(HtmlAgilityPack.HtmlDocument data, string link)
        {
            return GetPriceForB2B(data);
        }

        public static string GetPriceForB2B(HtmlAgilityPack.HtmlDocument data)
        {
            HtmlNode node = data.DocumentNode.SelectSingleNode("//tr[@id='trade-info-lot-price']");
            if (node != null)
            {
                DataParser.ParsePrice(node.QuerySelectorAll("td").ToList()[1].InnerText).Trim();
            }
            return "0";
        }
      
        public static HtmlAgilityPack.HtmlDocument OpenLinkOrder(string link)
        {
            tb.Invoke(new Action(() => { tb.AppendText(link + Environment.NewLine); }));
            return WebRequests.LoadPageStealth(link);
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
             //  tb.Invoke(new Action(() => { tb.AppendText("   Order_finish:"+ id+":"+link+ Environment.NewLine); }));
        }



        public static HtmlAgilityPack.HtmlDocument OpenLinkSearch(string keyWord, int page)
        {
            return WebRequests.LoadPageStealth(GenerateSearchLinkByPage(keyWord, page));
            //return WebRequests.LoadPage(GenerateSearchLinkByPage(keyWord, page));
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
            //tb.Invoke(new Action(() => { tb.AppendText("Search_finish:" + id + ":" + keyword + Environment.NewLine); }));
        }


    }
}
/*
      public static List<Order> Process(List<string> keyWords)
      {
          foreach (string key in keyWords)
          {
              SearchTread sTread = new SearchTread(
                      key,
                      0,
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

              List<Thread> or = SearchThreads.Take(1).ToList<Thread>();
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

          // tb.Invoke(new Action(() => { tb.AppendText("Orders.Count:" + OrdersThreads.Count  + Environment.NewLine); }));
          /*
          int ocount = OrdersThreads.Count();
          while (ocount > 0)
          {

              List<Thread> or = OrdersThreads.Take(50).ToList<Thread>();
                  foreach (Thread q in or)
                  {
                      q.Start();
                  }
                  foreach (Thread q in or)
                  {
                      q.Join();
                  }
              lock (threadsOrdersLock)
              {
                  foreach (Thread q in or)
                  {
                      OrdersThreads.Remove(q);
                  }
              }
                  ocount = OrdersThreads.Count();
              tb.Invoke(new Action(() => { tb.AppendText("Orders.Count:" + ocount + Environment.NewLine); }));

          }
          int pointer = 0;
          int activecounter = 0;


          while (true)
          {
              activecounter = 0;
              foreach (Thread q in OrdersThreads)
              {
                  if (q.IsAlive) activecounter++;
              }
              if (pointer == OrdersThreads.Count)
              {
                  break;
              }
              if (activecounter < 50)
              {
                  OrdersThreads[pointer].Start();
                  pointer++;
              }
              else
              {

              }
              tb.Invoke(new Action(() => { tb.AppendText("Pointer.Count:" + pointer + "/" + OrdersThreads.Count + Environment.NewLine); }));
          }
          bool isFinished = false;
          while (!isFinished)
          {
              Thread.Sleep(3000);
              isFinished = true;
              lock (threadsOrdersLock)
              {
                  foreach (Thread q in OrdersThreads)
                  {
                      isFinished &= !q.IsAlive;
                  }

              }
          }


          tb.Invoke(new Action(() => { tb.AppendText(Environment.NewLine + Environment.NewLine + "Skiped:" + linksSkip.Count + Environment.NewLine); }));
          return orderstest;
      }*/