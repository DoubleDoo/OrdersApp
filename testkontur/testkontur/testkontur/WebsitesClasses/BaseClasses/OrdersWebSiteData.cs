using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;
using testkontur.OrderClasses;
using System;

namespace testkontur.WebsitesClasses
{
    class OrdersWebSiteData
    {

        private OrdersWebSiteDataElements websiteData;

        public OrdersWebSiteData(OrdersWebSiteDataElements _websiteData)
        {

            websiteData = _websiteData;
        }

       
        public void FillOrderWithData(Order order)
        {/*
            Thread.Sleep(1000);
            SileniumFunctions.OpenLink(order.link);
            order.type = websiteData.GetTypeData();
            order.orderer = websiteData.GetOrdererData();
            order.federal = ParseFederalFromAddress(websiteData.GetCityData());
            order.city = websiteData.GetCityData();
            order.date = websiteData.GetDateData();
            order.info = websiteData.GetInfoData();
            order.price = websiteData.GetPriceData();*/
        }

        static string ParseFederalFromAddress(string addr)
        {
            string District = "";
            string[] centr = new string[] { "Калуг", "Белгород", "Брянск", "Владимирск", "Воронежск", "Ивановск", "Калужск", "Костромск", "Курск", "Липецк", "Московск", "Орловск", "Рязанск", "Смоленск", "Тамбовск", "Тверск", "Тульск", "Ярославск", "Москв" };
            string[] zapd = new string[] { "Карел", "Коми", "Архангельск", "Вологодск", "Калининградск", "Ленинградск", "Мурманск", "Новгородск", "Псковск", "Санкт-Петер", "Ненецк", "Петербург" };
            string[] yzn = new string[] { "Адыг", "Калмык", "Крым", "Краснодарск", "Астраханск", "Волгоградск", "Ростовск", "Севастоп" };
            string[] kavkaz = new string[] { "Дагест", "Ингушет", "Кабардин", "Карач", "Осети", "Чеченск", "Ставропольск" };
            string[] privolz = new string[] { "Нижнекамск", "Башкортост", "Марий Эл", "Мордов", "Татарст", "Удмуртск", "Чувашск", "Пермск", "Кировск", "Нижегородск", "Оренбургск", "Пензенск", "Самарск", "Саратовск", "Ульяновск", "Казан", "Самара" };
            string[] ural = new string[] { "Курганск", "Свердловск", "Тюменск", "Челябинск", "Мансийск", "Ненецк", "Екатеринбург" };
            string[] sibir = new string[] { "Алтай", "Бурят", "Тыв", "Хакас", "Алтайск", "Забайкальск", "Краснояр", "Иркутск", "Кемеровск", "Новосибирск", "Омск", "Томск" };
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
            return District;
        }


    }
}
