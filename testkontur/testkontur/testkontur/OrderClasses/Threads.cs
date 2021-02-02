
using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace testkontur.Threads
{


    public delegate HtmlAgilityPack.HtmlDocument OpenLinkS(string keyWord, int page);
    public delegate void DownloadDataS(HtmlAgilityPack.HtmlDocument document, int page, ref bool noNewOrders);
    public delegate void NextS(HtmlAgilityPack.HtmlDocument document, string keyword, int id, int page,ref bool noNewOrders);

    public delegate HtmlAgilityPack.HtmlDocument OpenLinkO(string link);
    public delegate void DownloadDataO(HtmlAgilityPack.HtmlDocument document, string link);
    public delegate void NextO(int id, string link);




    public class OrdersTread
    {
        private static object threadidLock = new object();
        static int threadId;
        private string link;
        private int id;

        private OpenLinkO OpenLinkCallback;
        private DownloadDataO DownloadDataCallback;
        private NextO NeaxtCallback;


        public OrdersTread(string _link, OpenLinkO callback1, DownloadDataO callback2, NextO callback3)
        {
            lock (threadidLock)
            {
                id = threadId++;
            }
            OpenLinkCallback = callback1;
            DownloadDataCallback = callback2;
            NeaxtCallback = callback3;
            link = _link;
        }

        public void ThreadProc()
        {
            HtmlAgilityPack.HtmlDocument document = null;
            if (OpenLinkCallback != null)
                document = OpenLinkCallback(link);
            if (DownloadDataCallback != null)
                DownloadDataCallback(document, link);
            if (NeaxtCallback != null)
                NeaxtCallback(id, link);
        }
    }


    public class SearchTread
    {
        private static object threadidLock = new object();
        static int threadId;
        private string keyword;
        private int page;
        private int id;
        private bool noNewOrders;

        private OpenLinkS OpenLinkCallback;
        private DownloadDataS DownloadDataCallback;
        private NextS NeaxtCallback;


        public SearchTread(string _keyword, int _page, OpenLinkS callback1, DownloadDataS callback2, NextS callback3)
        {
            lock (threadidLock)
            {
                id = threadId++;
            }
            page = _page;
            OpenLinkCallback = callback1;
            DownloadDataCallback = callback2;
            NeaxtCallback = callback3;
            keyword = _keyword;
            noNewOrders = false;
        }

        public void ThreadProc()
        {
            HtmlAgilityPack.HtmlDocument document = null;
            if (OpenLinkCallback != null)
                document = OpenLinkCallback(keyword, page);
            if (DownloadDataCallback != null)
                DownloadDataCallback(document,page,ref noNewOrders);
            if (NeaxtCallback != null)
                NeaxtCallback(document, keyword, id, page,ref noNewOrders);
        }
    }

    public static class DataParser
    {
        public static string ParseDate(string data)
        {
            Regex regexForDate = new Regex(@"(\d){2}.(\d){2}.(\d){4}");
            data = FixQuots(data);
            Match match = (regexForDate).Match(data);
            string res = match.Value.Trim();
            return res;
        }

        public static string ParsePrice(string data)
        {
            //Regex regexForPrice = new Regex(@"((\d)|(\s))+,(\d){2}");((\d)|(\s))+(,|.)?(\d){0,2}
            Regex regexForPrice = new Regex(@"((\d)|(\s))+,(\d){0,2}");
            data = FixQuots(data);
            Match match = (regexForPrice).Match(data);
            string res = match.Value.Trim();
            return res;
        }

        
        public static string ParsePriceCustom(string data)
        {
            //Regex regexForPrice = new Regex(@"((\d)|(\s))+,(\d){2}");((\d)|(\s))+(,|.)?(\d){0,2}
            Regex regexForPrice = new Regex(@"((\d)|(\s))+(,|.)?(\d){0,2}");
            data = FixQuots(data).Replace(" ", "").Replace(".", ",");
            Match match = (regexForPrice).Match(data);
            string res = match.Value.Trim();
            //if (data.Substring(data.Length - 1, 1) == ".") data = data.Substring(0, data.Length - 1);
            //if (data.Length == 0) data = "0";
            return res;
        }
        
        public static string FixQuots(string data)
        {
            string res = data.Replace("&#160;", "").Replace("&quot;", "\"").Replace("&nbsp;", "").Replace("&#034;", "\"").Replace("&#187;", "\"").Replace("&#171;", "\"");
            return res;
        }

        public static string GetFederal(string addr)
        {
            string District = "";
            string[] centr = new string[] { "Тверь", "Тула", "Калуг", "Белгород", "Брянск", "Владимирск", "Воронежск", "Ивановск", "Калужск", "Костромск", "Курск", "Липецк", "Московск", "Орловск", "Рязанск", "Смоленск", "Тамбовск", "Тверск", "Тульск", "Ярославск", "Москв" };
            string[] zapd = new string[] { "Кемерово", "Салехард", "Карел", "Коми", "Архангельск", "Вологодск", "Калининградск", "Ленинградск", "Мурманск", "Новгородск", "Псковск", "Санкт-Петер", "Ненецк", "Петербург" };
            string[] yzn = new string[] { "Астрахань", "Ростов", "Краснод", "Сочи", "Адыг", "Калмык", "Крым", "Краснодарск", "Астраханск", "Волгоградск", "Ростовск", "Севастоп" };
            string[] kavkaz = new string[] { "Дагест", "Ингушет", "Кабардин", "Карач", "Осети", "Чеченск", "Ставропольск" };
            string[] privolz = new string[] { "Пермь", "Оренбург", "Ижевск", "Уфа", "Нижнекамск", "Башкортост", "Марий Эл", "Мордов", "Татарст", "Удмуртск", "Чувашск", "Пермск", "Кировск", "Нижегородск", "Оренбургск", "Пензенск", "Самарск", "Саратовск", "Ульяновск", "Казан", "Самара", "Новгород" };
            string[] ural = new string[] { "Салехард", "Курганск", "Свердловск", "Тюменск", "Челябинск", "Мансийск", "Ненецк", "Екатеринбург" };
            string[] sibir = new string[] { "Кемеров", "Алтай", "Бурят", "Тыв", "Хакас", "Алтайск", "Забайкальск", "Краснояр", "Иркутск", "Кемеровск", "Новосибирск", "Омск", "Томск" };
            string[] dalnev = new string[] { "Саха", "Якут", "Камчатск", "Приморск", "Хабаровск", "Амурск", "Магаданск", "Сахалинск", "Еврейск", "Чукотск", "Владивост" };
            for (int i = 0; i < centr.Length; i++)
            {
                if (!(addr.IndexOf(centr[i], StringComparison.CurrentCultureIgnoreCase) == -1))
                    District = "Центральный федеральный округ";
            }
            for (int i = 0; i < zapd.Length; i++)
            {
                if (!(addr.IndexOf(zapd[i], StringComparison.CurrentCultureIgnoreCase) == -1))
                    District = "Северо-Западный федеральный округ";
            }
            for (int i = 0; i < yzn.Length; i++)
            {
                if (!(addr.IndexOf(yzn[i], StringComparison.CurrentCultureIgnoreCase) == -1))
                    District = "Южный федеральный округ";
            }
            for (int i = 0; i < kavkaz.Length; i++)
            {
                if (!(addr.IndexOf(kavkaz[i], StringComparison.CurrentCultureIgnoreCase) == -1))
                    District = "Северо-Кавказский федеральный округ";
            }
            for (int i = 0; i < privolz.Length; i++)
            {
                if (!(addr.IndexOf(privolz[i], StringComparison.CurrentCultureIgnoreCase) == -1))
                    District = "Приволжский федеральный округ";
            }
            for (int i = 0; i < ural.Length; i++)
            {
                if (!(addr.IndexOf(ural[i], StringComparison.CurrentCultureIgnoreCase) == -1))
                    District = "Уральский федеральный округ";
            }
            for (int i = 0; i < sibir.Length; i++)
            {
                if (!(addr.IndexOf(sibir[i], StringComparison.CurrentCultureIgnoreCase) == -1))
                    District = "Сибирский федеральный округ";
            }
            for (int i = 0; i < dalnev.Length; i++)
            {
                if (!(addr.IndexOf(dalnev[i], StringComparison.CurrentCultureIgnoreCase) == -1))
                    District = "Дальневосточный федеральный округ";
            }
            if (District == "") return "None";
            return District;
        }
    }

    public static class WebRequests
    {
        public static HtmlWeb browser = new HtmlWeb();
        public static HtmlAgilityPack.HtmlDocument LoadPage(string data)
        {

            if (data.Length < 5) data = "https://zakupki.gov.ru/223/purchase/public/purchase/info/common-info.html?regNumber=32109896663";
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            bool pass = false;
            while (!pass)
            {
                try
                {
                    doc = browser.Load(data);
                    pass = true;
                }
                catch (System.IO.IOException e)
                {
                    pass = false;
                    Thread.Sleep(2000);
                }
                catch (System.Net.WebException e)
                {
                    pass = false;
                    Thread.Sleep(2000);
                }
            }
            return doc;
        }

        public static HtmlAgilityPack.HtmlDocument LoadPageStealth(string data)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            if (data.Length < 5) data = "https://zakupki.gov.ru/223/purchase/public/purchase/info/common-info.html?regNumber=32109896663";
            bool pass = false;
            while (!pass)
            {
                try
                {
                    doc = browser.Load(data, "37.203.242.221", 10183, "dubinich", "7140043");
                    pass = true;
                }
                catch (System.IO.IOException e)
                {
                    pass = false;
                    Thread.Sleep(2000);
                }
                catch (System.Net.WebException e)
                {
                    pass = false;
                    Thread.Sleep(2000);
                }
            }
            return doc;
        }
    }
   



}