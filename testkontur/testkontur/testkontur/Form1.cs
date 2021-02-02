using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using Microsoft.Office.Interop;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;
using HtmlAgilityPack;
using Fizzler;
//using System.Text.Json;
//using System.Text.Json.Serialization;


using testkontur.OrderClasses;
using testkontur.WebsitesClasses;
using Fizzler.Systems.HtmlAgilityPack;
using System.Net;
using Microsoft.Office.Interop.Excel;
using Action = System.Action;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace testkontur
{


    public partial class Form1 : Form
    {
        public string startdate = "";
        public string enddate = "";
        public string startprice = "";
        public string endprice = "";
        // public bool skip = false;
        public List<string> keywords;
        // public List<string> links;
        //public int counter = 0;
        //public int timeoutsearch = 0;
        //public int timeoutdatacolect = 0;
        public int MSCindex = 1;
        public int SPBindex = 1;
        public int CHBindex = 1;
        public int VLDindex = 1;
        public int OTHindex = 1;
        public int FLTindex = 1;
        // public int timeoutteh = 0;
        Microsoft.Office.Interop.Excel.Worksheet MSK;
        Microsoft.Office.Interop.Excel.Worksheet SPB;
        Microsoft.Office.Interop.Excel.Worksheet VLD;
        Microsoft.Office.Interop.Excel.Worksheet CHB;
        Microsoft.Office.Interop.Excel.Worksheet OTH;
        Microsoft.Office.Interop.Excel.Worksheet FLT;

        List<Order> BlockedOrders = new List<Order>();
        List<Thread> OrdersThreadsList = new List<Thread>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            Thread mythread = new Thread(thread);
            mythread.Start();



        }
        private static object threadsOrdersLock = new object();


        public void thread()
        {
            label8.Invoke(new Action(() => { label8.Text = "Инициация"; }));

            keywords = new List<string>();
            foreach (string a in textBox1.Lines)
            {
                keywords.Add(a);
            }

            startdate = dateTimePicker1.Value.ToString("dd.MM.yyyy");
            enddate = dateTimePicker2.Value.ToString("dd.MM.yyyy");
            startprice = textBox4.Text;
            endprice = textBox5.Text;



            List<Order> orders = new List<Order>();

            if (TecktorgCheckBox.Checked)
            {
                Tecktorg.TecktorgStatic(startdate, enddate, startprice, endprice, progressBar1, label8, textBox8);
                OrdersThreadsList.AddRange(Tecktorg.Process(keywords));
                textBox8.Invoke(new Action(() => { textBox8.AppendText("Tecktorg" + " завершено"+ Environment.NewLine); }));
            }
            if (EtpgpbCheckBox.Checked)
            {
                Etpgpb.EtpgpbStatic(startdate, enddate, startprice, endprice, progressBar1, label8, textBox8);
                OrdersThreadsList.AddRange(Etpgpb.Process(keywords));
                textBox8.Invoke(new Action(() => { textBox8.AppendText("Etpgpb" + " завершено" + Environment.NewLine); }));
            }
            if (B2BCenterCheckBox.Checked)
            {
                B2bCenter.B2bCenterStatic(startdate, enddate, startprice, endprice, progressBar1, label8, textBox8);
                OrdersThreadsList.AddRange(B2bCenter.Process(keywords));
                textBox8.Invoke(new Action(() => { textBox8.AppendText("B2bCenter" + " завершено" + Environment.NewLine); }));
            }
            if (ZakupkiGovCheckBox.Checked)
            {
                ZakupkiGov.ZakupkiGovStatic(startdate, enddate, startprice, endprice, progressBar1, label8, textBox8);
                OrdersThreadsList.AddRange(ZakupkiGov.Process(keywords));
                textBox8.Invoke(new Action(() => { textBox8.AppendText("ZakupkiGov" + " завершено" + Environment.NewLine); }));
            }
            if (RosatomCheckBox.Checked)
            {
                string strdate = startdate.Split('.')[2] + startdate.Split('.')[1] + startdate.Split('.')[0];
                string nddate = enddate.Split('.')[2] + enddate.Split('.')[1] + enddate.Split('.')[0];

                Rosatom.RosatomStatic(strdate, nddate, startprice, endprice, progressBar1, label8, textBox8);
                OrdersThreadsList.AddRange(Rosatom.Process(keywords));
                textBox8.Invoke(new Action(() => { textBox8.AppendText("Rosatom" + " завершено" + Environment.NewLine); }));
            }
            /*
            int ocount = OrdersThreadsList.Count();

            while (ocount > 0)
            {

                List<Thread> or = OrdersThreadsList.Take(50).ToList<Thread>();
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
                        OrdersThreadsList.Remove(q);
                    }
                }
                ocount = OrdersThreadsList.Count();
                textBox8.Invoke(new Action(() => { textBox8.AppendText("Orders.Count:" + ocount + Environment.NewLine); }));

            }*/
            int pointer = 0;
            int activecounter = 0;


            while (true)
            {
                activecounter = 0;
                foreach (Thread q in OrdersThreadsList)
                {
                    if (q.IsAlive) activecounter++;
                }
                if (pointer == OrdersThreadsList.Count)
                {
                    break;
                }
                if (activecounter < 50)
                {
                    OrdersThreadsList[pointer].Start();
                    pointer++;
                    textBox8.Invoke(new Action(() => { textBox8.AppendText("Pointer.Count:" + pointer + "/" + OrdersThreadsList.Count + Environment.NewLine); }));
                }
                else
                {

                }
                
            }
            bool isFinished = false;
            while (!isFinished)
            {
                Thread.Sleep(3000);
                isFinished = true;
                lock (threadsOrdersLock)
                {
                    foreach (Thread q in OrdersThreadsList)
                    {
                        isFinished &= !q.IsAlive;
                    }

                }
            }


            if (TecktorgCheckBox.Checked)
            {
                orders.AddRange(Tecktorg.orderstest);
                textBox8.Invoke(new Action(() => { textBox8.AppendText("Tecktorg" + " пропущено:" + Tecktorg .linksSkip.Count+ Environment.NewLine); }));
            }
            if (EtpgpbCheckBox.Checked)
            {
                orders.AddRange(Etpgpb.orderstest);
                textBox8.Invoke(new Action(() => { textBox8.AppendText("Etpgpb" + " пропущено:" + Etpgpb.linksSkip.Count + Environment.NewLine); }));
            }
            if (B2BCenterCheckBox.Checked)
            {
                orders.AddRange(B2bCenter.orderstest);
                textBox8.Invoke(new Action(() => { textBox8.AppendText("B2bCenter" + " пропущено:" + B2bCenter.linksSkip.Count + Environment.NewLine); }));
            }
            if (ZakupkiGovCheckBox.Checked)
            {
                orders.AddRange(ZakupkiGov.orderstest);
                textBox8.Invoke(new Action(() => { textBox8.AppendText("ZakupkiGov" + " пропущено:" + ZakupkiGov.linksSkip.Count + Environment.NewLine); }));
            }
            if (RosatomCheckBox.Checked)
            {
                orders.AddRange(Rosatom.orderstest);
                textBox8.Invoke(new Action(() => { textBox8.AppendText("Rosatom" + " пропущено:" + Rosatom.linksSkip.Count + Environment.NewLine); }));
            }


            //tb.Invoke(new Action(() => { tb.AppendText(Environment.NewLine + Environment.NewLine + "Skiped:" + linksSkip.Count + Environment.NewLine); }));


            // textBox8.Invoke(new Action(() => { textBox8.AppendText("end" + Environment.NewLine); }));
            orders = obrabotcaNew(orders);




            label8.Invoke(new Action(() => { label8.Text = "Экспорт в EXCEL"; }));



            Microsoft.Office.Interop.Excel.Application ObjExcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook ObjWorkBook;
            ObjExcel.SheetsInNewWorkbook = 6;
            ObjWorkBook = ObjExcel.Workbooks.Add(System.Reflection.Missing.Value);
            MSK = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1];
            MSK.Name = "МСК";
            SPB = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[2];
            SPB.Name = "СПБ";
            VLD = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[3];
            VLD.Name = "ВЛД";
            CHB = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[4];
            CHB.Name = "ЧЛБ";
            OTH = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[5];
            OTH.Name = "Прочее";
            FLT = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[6];
            FLT.Name = "Фильрованое";



            progressBar1.Invoke(new Action(() => { progressBar1.Value = 0; progressBar1.Maximum = orders.Count; }));
            foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in ObjWorkBook.Sheets)
            {
                sheet.Columns.RowHeight = 12;
                sheet.Cells.Style.WrapText = true;
                sheet.Columns[1].ColumnWidth = 6;
                sheet.Cells[1, 1] = "Номер";
                sheet.Cells[1, 1].EntireRow.Font.Bold = true;
                sheet.Columns[2].ColumnWidth = 20;
                sheet.Cells[1, 2] = "Тип";
                sheet.Cells[1, 2].EntireRow.Font.Bold = true;
                sheet.Columns[3].ColumnWidth = 20;
                sheet.Cells[1, 3] = "Заказчик";
                sheet.Cells[1, 3].EntireRow.Font.Bold = true;
                sheet.Columns[4].ColumnWidth = 20;
                sheet.Cells[1, 4] = "Федеральный округ";
                sheet.Cells[1, 4].EntireRow.Font.Bold = true;
                sheet.Columns[5].ColumnWidth = 20;
                sheet.Cells[1, 5] = "Город";
                sheet.Cells[1, 5].EntireRow.Font.Bold = true;
                sheet.Columns[6].ColumnWidth = 10;
                sheet.Cells[1, 6] = "Дата окончания приема заявок";
                sheet.Cells[1, 6].EntireRow.Font.Bold = true;
                sheet.Columns[7].ColumnWidth = 45;
                sheet.Cells[1, 7] = "Краткая информация";
                sheet.Cells[1, 7].EntireRow.Font.Bold = true;
                sheet.Columns[8].ColumnWidth = 15;
                sheet.Cells[1, 8] = "Цена";
                sheet.Cells[1, 8].EntireRow.Font.Bold = true;
                sheet.Columns[9].ColumnWidth = 20;
                sheet.Cells[1, 9] = "Ссылка исходного размещения";
                sheet.Cells[1, 9].EntireRow.Font.Bold = true;
            }

            /*
           Regex regexabriviaturs = new Regex(@"(((\s|^)нку)|((\s|^)що-70)|((\s|^)кру)|((\s|^)крун)|((\s|^)зру)|((\s|^)ору)|
           ((\s|^)ру)|((\s|^)гру)|((\s|^)гпп)|((\s|^)бртп)|((\s|^)ктпсн)|((\s|^)русн)|((\s|^)дэс)|((\s|^)ксо)|((\s|^)бмз)|((\s|^)ктп)|
           ((\s|^)бктп)|((\s|^)ктпн)|((\s|^)рза)|((\s|^)пс)|((\s|^)ртзо)|((\s|^)ктпсн)|((\s|^)опу)|((\s|^)тп)|((\s|^)ртп)|((\s|^)мрз)|
           ((\s|^)асу))|((распределит)|(устройств)|(щит)|(подстанц)|(электростанц)|(дизел)|(панел)|(управл)|(энергоцентр)|(общеподстанцион)|
           (пункт)|(шкаф )|(ячейк))", RegexOptions.IgnoreCase);
          */
            //Regex regexcutwords = new Regex(@"((распределит) | (устройств) | (щит) | (подстанц) | (электростанц) | (дизел) | (панел) | (управл) | (энергоцентр) | (общеподстанцион) | (пункт) | (шкаф ) | (ячейк))", RegexOptions.IgnoreCase);


            int i = 1;
            Microsoft.Office.Interop.Excel.Worksheet curWorkSheet;
            foreach (Order ord in orders)
            {
                i = getindexforSheet(ord.federal) + 1;
                progressBar1.Invoke(new Action(() => { progressBar1.Value++; }));
                curWorkSheet = getSheetFromHTML(ord.federal);
                curWorkSheet.Cells[i, 7] = ord.info;
                if (
                ord.info.ToLower().Contains(" нку ") || ord.info.ToLower().Contains(" що-70 ") ||
                ord.info.ToLower().Contains(" кру ") || ord.info.ToLower().Contains(" крун ") ||
                ord.info.ToLower().Contains(" зру ") || ord.info.ToLower().Contains(" ору ") ||
                ord.info.ToLower().Contains(" гру ") || ord.info.ToLower().Contains(" гпп ") ||
                ord.info.ToLower().Contains(" бртп ") || ord.info.ToLower().Contains(" ктпсн ") ||
                ord.info.ToLower().Contains(" русн ") || ord.info.ToLower().Contains(" дэс ") ||
                ord.info.ToLower().Contains(" ксо ") || ord.info.ToLower().Contains(" бмз ") ||
                ord.info.ToLower().Contains(" ктп ") || ord.info.ToLower().Contains(" бктп ") ||
                ord.info.ToLower().Contains(" ктпн ") || ord.info.ToLower().Contains(" рза ") ||
                ord.info.ToLower().Contains(" ртзо ") || ord.info.ToLower().Contains(" ктпсн ") ||
                ord.info.ToLower().Contains(" опу ") || ord.info.ToLower().Contains(" тп ") ||
                ord.info.ToLower().Contains(" ртп ") || ord.info.ToLower().Contains(" мрз ") ||
                //ord.info.ToLower().Contains("асу") || ord.info.ToLower().Contains("распределит") 
                ord.info.ToLower().Contains("щит") || ord.info.ToLower().Contains("подстанц") ||
                ord.info.ToLower().Contains("дизел") || ord.info.ToLower().Contains("панел") ||
                ord.info.ToLower().Contains("энергоцентр") || ord.info.ToLower().Contains("общеподстанцион") ||
                ord.info.ToLower().Contains("шкаф") || ord.info.ToLower().Contains("ячейк") || ord.info.ToLower().Contains("ячеек") ||
                ord.info.ToLower().Contains("генератор") || ord.info.ToLower().Contains("элегазов") ||
                ord.info.ToLower().Contains("распределитель") || ord.info.ToLower().Contains("энергетическ") ||
                ord.info.ToLower().Contains("трансформатор") || ord.info.ToLower().Contains("крун") ||
                ord.info.ToLower().Contains("двигател") || ord.info.ToLower().Contains("ктп") || ord.info.ToLower().Contains("рза") ||
                ord.info.ToLower().Contains("электростанц") || ord.info.ToLower().Contains("энергоблок") ||
                ord.info.ToLower().Contains("кл-") || ord.info.ToLower().Contains("рп-") ||
                ord.info.ToLower().Contains("ртп-") || ord.info.ToLower().Contains("тп-") ||
                ord.info.ToLower().Contains("асу тп") || ord.info.ToLower().Contains(" пс ") ||
                ord.info.ToLower().Contains("кл-") || ord.info.ToLower().Contains(" тэц ") ||
                ord.info.ToLower().Contains(" гэс ") || ord.info.ToLower().Contains("электростанци") ||
                ord.info.ToLower().Contains("що-70") || ord.info.ToLower().Contains("гпп") ||
                ord.info.ToLower().Contains("ртзо") || ord.info.ToLower().Contains("ктпсн") ||
                ord.info.ToLower().Contains("вл-") || ord.info.ToLower().Contains("вли-") || ord.info.ToLower().Contains(" вл ")
                //ord.info.ToLower().Contains("") || ord.info.ToLower().Contains("") ||
                )
                {
                    curWorkSheet.Cells[i, 7].Font.Bold = true;
                }
                curWorkSheet.Cells[i, 6] = ord.date;
                curWorkSheet.Cells[i, 5] = ord.city;
                decimal tst = 0;
                // textBox8.Invoke(new Action(() => { textBox8.AppendText("Price" + ord.price+":" + ord.price.Length + Environment.NewLine); }));
                if (Decimal.TryParse(ord.price,out tst))
                curWorkSheet.Cells[i, 8] = Math.Round(Decimal.Parse(ord.price));
                 else
                 {
                    FLT.Cells[i, 8] = "?";
                    textBox8.Invoke(new Action(() => { textBox8.AppendText("Price" + ord.price + ":" + ord.price.Length + Environment.NewLine); }));
                }
                //curWorkSheet.Cells[i, 8] = Math.Round(Decimal.Parse(ord.price));
                curWorkSheet.Cells[i, 9] = ord.link;
                curWorkSheet.Cells[i, 3] = ord.orderer;
                curWorkSheet.Cells[i, 1] = (i - 1).ToString();
                curWorkSheet.Cells[i, 2] = ord.type;
                curWorkSheet.Cells[i, 4] = ord.federal;



            }
            progressBar1.Invoke(new Action(() => { progressBar1.Value = 0; progressBar1.Maximum = BlockedOrders.Count; }));
            i = 1;
            foreach (Order ord in BlockedOrders)
            {
                i++;
                progressBar1.Invoke(new Action(() => { progressBar1.Value++; }));
                FLT.Cells[i, 7] = ord.info;
                if (
                    ord.info.ToLower().Contains(" нку ") || ord.info.ToLower().Contains(" що-70 ") ||
                    ord.info.ToLower().Contains(" кру ") || ord.info.ToLower().Contains(" крун ") ||
                    ord.info.ToLower().Contains(" зру ") || ord.info.ToLower().Contains(" ору ") ||
                    ord.info.ToLower().Contains(" гру ") || ord.info.ToLower().Contains(" гпп ") ||
                    ord.info.ToLower().Contains(" бртп ") || ord.info.ToLower().Contains(" ктпсн ") ||
                    ord.info.ToLower().Contains(" русн ") || ord.info.ToLower().Contains(" дэс ") ||
                    ord.info.ToLower().Contains(" ксо ") || ord.info.ToLower().Contains(" бмз ") ||
                    ord.info.ToLower().Contains(" ктп ") || ord.info.ToLower().Contains(" бктп ") ||
                    ord.info.ToLower().Contains(" ктпн ") || ord.info.ToLower().Contains(" рза ") ||
                    ord.info.ToLower().Contains(" ртзо ") || ord.info.ToLower().Contains(" ктпсн ") ||
                    ord.info.ToLower().Contains(" опу ") || ord.info.ToLower().Contains(" тп ") ||
                    ord.info.ToLower().Contains(" ртп ") || ord.info.ToLower().Contains(" мрз ") ||
                    //ord.info.ToLower().Contains("асу") || ord.info.ToLower().Contains("распределит") 
                    ord.info.ToLower().Contains("щит") || ord.info.ToLower().Contains("подстанц") ||
                    ord.info.ToLower().Contains("дизел") || ord.info.ToLower().Contains("панел") ||
                    ord.info.ToLower().Contains("энергоцентр") || ord.info.ToLower().Contains("общеподстанцион") ||
                    ord.info.ToLower().Contains("шкаф") || ord.info.ToLower().Contains("ячейк") || ord.info.ToLower().Contains("ячеек") ||
                    ord.info.ToLower().Contains("генератор") || ord.info.ToLower().Contains("элегазов") ||
                    ord.info.ToLower().Contains("распределитель") || ord.info.ToLower().Contains("энергетическ") ||
                    ord.info.ToLower().Contains("трансформатор") || ord.info.ToLower().Contains("крун") ||
                    ord.info.ToLower().Contains("двигател") || ord.info.ToLower().Contains("ктп") || ord.info.ToLower().Contains("рза") ||
                    ord.info.ToLower().Contains("электростанц") || ord.info.ToLower().Contains("энергоблок") ||
                    ord.info.ToLower().Contains("кл-") || ord.info.ToLower().Contains("рп-") ||
                    ord.info.ToLower().Contains("ртп-") || ord.info.ToLower().Contains("тп-") ||
                    ord.info.ToLower().Contains("асу тп") || ord.info.ToLower().Contains(" пс ") ||
                    ord.info.ToLower().Contains("кл-") || ord.info.ToLower().Contains(" тэц ") ||
                    ord.info.ToLower().Contains(" гэс ") || ord.info.ToLower().Contains("электростанци") ||
                    ord.info.ToLower().Contains("що-70") || ord.info.ToLower().Contains("гпп") ||
                    ord.info.ToLower().Contains("ртзо") || ord.info.ToLower().Contains("ктпсн") ||
                    ord.info.ToLower().Contains("вл-") || ord.info.ToLower().Contains("вли-") || ord.info.ToLower().Contains(" вл ")
                    //ord.info.ToLower().Contains("") || ord.info.ToLower().Contains("") ||
                    )
                {
                    FLT.Cells[i, 7].Font.Bold = true;
                }
                FLT.Cells[i, 6] = ord.date;
                FLT.Cells[i, 5] = ord.city;


                // textBox8.Invoke(new Action(() => { textBox8.AppendText("Price" + ord.price+":" + ord.price.Length + Environment.NewLine); }));
                decimal tst = 0;
                if (Decimal.TryParse(ord.price,out tst))
                FLT.Cells[i, 8] = Math.Round(Decimal.Parse(ord.price));
               else
                {
                    FLT.Cells[i, 8] = "?";
                    textBox8.Invoke(new Action(() => { textBox8.AppendText("Price" + ord.price + ":" + ord.price.Length + Environment.NewLine); }));
                }
                FLT.Cells[i, 9] = ord.link;
                FLT.Cells[i, 3] = ord.orderer;
                FLT.Cells[i, 1] = (i - 1).ToString();
                FLT.Cells[i, 2] = ord.type;
                FLT.Cells[i, 4] = ord.federal;
            }



            label8.Invoke(new Action(() => { label8.Text = "Работа завершена"; }));
            ObjExcel.Visible = true;
            ObjExcel.UserControl = true;
            // browser.Quit();

        }


        public Microsoft.Office.Interop.Excel.Worksheet getSheetFromHTML(string html)
        {
            if (html == "Северо-Западный федеральный округ")
            { SPBindex++; return SPB; }
            else if (html == "Дальневосточный федеральный округ")
            { VLDindex++; return VLD; }
            else if (html == "Приволжский федеральный округ")
            { CHBindex++; return CHB; }
            else if (html == "Центральный федеральный округ")
            { MSCindex++; return MSK; }
            else
            { OTHindex++; return OTH; }
        }

        public int getindexforSheet(string name)
        {
            if (name == "Северо-Западный федеральный округ")
            { return SPBindex; }
            else if (name == "Дальневосточный федеральный округ")
            { return VLDindex; }
            else if (name == "Приволжский федеральный округ")
            { return CHBindex; }
            else if (name == "Центральный федеральный округ")
            { return MSCindex; }
            else
            { return OTHindex; }

        }
        public List<Order> obrabotcaNew(List<Order> AllOrders)
        {
            List<Order> FilteredOrders = new List<Order>();
            List<Order> AcseptedOrders = new List<Order>();
            label8.Invoke(new Action(() => { label8.Text = "Обработка: удаление по словам исключениям"; }));

            foreach (Order ord in AllOrders)
            {
                if (ord.info.ToLower().Contains("крыльц") || ord.info.ToLower().Contains("крыш") ||
                    ord.info.ToLower().Contains("лекарств") || ord.info.ToLower().Contains("медецин") ||
                    ord.info.ToLower().Contains("реагент") || ord.info.ToLower().Contains("грузов") ||
                    ord.info.ToLower().Contains("костюм") || ord.info.ToLower().Contains("грузопасажир") ||
                    ord.info.ToLower().Contains("автозапчаст") || ord.info.ToLower().Contains("автомобил") ||
                    ord.info.ToLower().Contains("инвалид") || ord.info.ToLower().Contains("кровл") ||
                    ord.info.ToLower().Contains("питания") || ord.info.ToLower().Contains("хозяйствен") ||
                    ord.info.ToLower().Contains("шприц") || ord.info.ToLower().Contains("перчат") ||
                    ord.info.ToLower().Contains("маска") || ord.info.ToLower().Contains("масок") ||
                    ord.info.ToLower().Contains("охрана") || ord.info.ToLower().Contains("маски") ||
                    ord.info.ToLower().Contains("медицин") || ord.info.ToLower().Contains("реклам") ||
                    ord.info.ToLower().Contains("смазочн") || ord.info.ToLower().Contains("порошок") ||
                    ord.info.ToLower().Contains("одежд") || ord.info.ToLower().Contains("вирус") ||
                    ord.info.ToLower().Contains("лечебн") || ord.info.ToLower().Contains("грузоподъ") ||
                    ord.info.ToLower().Contains("вытяжн") || ord.info.ToLower().Contains("опухол") ||
                    ord.info.ToLower().Contains("видео") || ord.info.ToLower().Contains("пожар") ||
                    ord.info.ToLower().Contains("говяд") || ord.info.ToLower().Contains("курин") ||
                    ord.info.ToLower().Contains("канцеляр") || ord.info.ToLower().Contains("порошк") ||
                    ord.info.ToLower().Contains("обув") || ord.info.ToLower().Contains("халат") ||
                    ord.info.ToLower().Contains("кроват") || ord.info.ToLower().Contains("лифт") ||
                    ord.info.ToLower().Contains("стекл") || ord.info.ToLower().Contains("мебел") ||
                    ord.info.ToLower().Contains("плитк") || ord.info.ToLower().Contains("жидкост") ||
                    ord.info.ToLower().Contains("водовод") || ord.info.ToLower().Contains("игрушк") ||
                    ord.info.ToLower().Contains("библиоте") || ord.info.ToLower().Contains("творог") ||
                    ord.info.ToLower().Contains("цветоч") || ord.info.ToLower().Contains("цветок") ||
                    ord.info.ToLower().Contains("краск") || ord.info.ToLower().Contains("фрукт") ||
                    ord.info.ToLower().Contains("сувенир") || ord.info.ToLower().Contains("маломобил") ||
                    ord.info.ToLower().Contains("инсулин") || ord.info.ToLower().Contains("туалет") ||
                    ord.info.ToLower().Contains("транспорт") || ord.info.ToLower().Contains("водоснабж") ||
                    ord.info.ToLower().Contains("нефт") || ord.info.ToLower().Contains("бензин") ||
                    ord.info.ToLower().Contains("теплогенератор") || ord.info.ToLower().Contains("кроват") ||
                    ord.info.ToLower().Contains("халат") || ord.info.ToLower().Contains("обув") ||
                    ord.info.ToLower().Contains("молочн") || ord.info.ToLower().Contains("молоко") ||
                    ord.info.ToLower().Contains("компьютер") || ord.info.ToLower().Contains("репчат") ||
                    ord.info.ToLower().Contains("программн") || ord.info.ToLower().Contains("физкульт") ||
                    ord.info.ToLower().Contains("ректив") || ord.info.ToLower().Contains("погрузоч") ||
                    ord.info.ToLower().Contains("больным") || ord.info.ToLower().Contains("лошад") ||
                    ord.info.ToLower().Contains("аромат") || ord.info.ToLower().Contains("подводн") ||
                    ord.info.ToLower().Contains("подарок") || ord.info.ToLower().Contains("подарка") ||
                    ord.info.ToLower().Contains("скот") || ord.info.ToLower().Contains("корона") ||
                    ord.info.ToLower().Contains("снег") || ord.info.ToLower().Contains("кино") ||
                    ord.info.ToLower().Contains("кондиционир") || ord.info.ToLower().Contains("пандус") ||
                    ord.info.ToLower().Contains("канализац") || ord.info.ToLower().Contains("вентиляц") ||
                    ord.info.ToLower().Contains("водоответ") || ord.info.ToLower().Contains("обледен") ||
                    ord.info.ToLower().Contains("марлев") || ord.info.ToLower().Contains("лабаратор") ||
                    ord.info.ToLower().Contains("перевозк") || ord.info.ToLower().Contains("погрузчик") ||
                    ord.info.ToLower().Contains("инвентар") || ord.info.ToLower().Contains("полиграф") ||
                    ord.info.ToLower().Contains("бакалей") || ord.info.ToLower().Contains("инкубатор") ||
                    ord.info.ToLower().Contains("водопровод") || ord.info.ToLower().Contains("гидроагрегат") ||
                    ord.info.ToLower().Contains("имлапнт") || ord.info.ToLower().Contains("травма") ||
                    ord.info.ToLower().Contains("дисплей") || ord.info.ToLower().Contains("рольстав") ||
                    ord.info.ToLower().Contains("коммуникац") || ord.info.ToLower().Contains("светодиод") ||
                    ord.info.ToLower().Contains("гараж") || ord.info.ToLower().Contains("лаборатор") ||
                    ord.info.ToLower().Contains("водоотведен") || ord.info.ToLower().Contains("носителей") ||
                    ord.info.ToLower().Contains("насос") || ord.info.ToLower().Contains("тонометр") ||
                    ord.info.ToLower().Contains("грузик") || ord.info.ToLower().Contains("офтальмолог") ||
                    ord.info.ToLower().Contains("плесен") || ord.info.ToLower().Contains("гриб") ||
                    ord.info.ToLower().Contains("оптоволокн") || ord.info.ToLower().Contains("сантех") ||
                    ord.info.ToLower().Contains("туман") || ord.info.ToLower().Contains("бухгалтер") ||
                    ord.info.ToLower().Contains("хакатон") || ord.info.ToLower().Contains("наркоз") ||
                    ord.info.ToLower().Contains("песчан") || ord.info.ToLower().Contains("грунта") ||
                    ord.info.ToLower().Contains("бумага") || ord.info.ToLower().Contains("косилка") ||
                    ord.info.ToLower().Contains("хирург") || ord.info.ToLower().Contains("игруш") ||
                    ord.info.ToLower().Contains("препарат") || ord.info.ToLower().Contains("грудк") ||
                    ord.info.ToLower().Contains("овощ") || ord.info.ToLower().Contains("инфекц") ||
                    ord.info.ToLower().Contains("аэрозол") || ord.info.ToLower().Contains("антибио") ||
                    ord.info.ToLower().Contains("спутник") || ord.info.ToLower().Contains("антисепт") ||
                    ord.info.ToLower().Contains("жарочн") || ord.info.ToLower().Contains("аудио") ||
                    ord.info.ToLower().Contains("смесь") || ord.info.ToLower().Contains("смеси") ||
                    ord.info.ToLower().Contains("композит") || ord.info.ToLower().Contains("раствор") ||
                    ord.info.ToLower().Contains("кровь") || ord.info.ToLower().Contains("крови") ||
                    ord.info.ToLower().Contains("кровля") || ord.info.ToLower().Contains("кровли") ||
                    ord.info.ToLower().Contains("моноклон") || ord.info.ToLower().Contains("навес") ||
                    ord.info.ToLower().Contains("бактерио") || ord.info.ToLower().Contains("бахил") ||
                    ord.info.ToLower().Contains("реставрац") || ord.info.ToLower().Contains("холодильн") ||
                    ord.info.ToLower().Contains("атестац") || ord.info.ToLower().Contains("благоустройст") ||
                    ord.info.ToLower().Contains("клиент") || ord.info.ToLower().Contains("конюш") ||
                    ord.info.ToLower().Contains("метролог") || ord.info.ToLower().Contains("водопропуск") ||
                    ord.info.ToLower().Contains("бассейн") || ord.info.ToLower().Contains("межпанельн") ||
                    ord.info.ToLower().Contains("лестнич") || ord.info.ToLower().Contains("кабинет") ||
                    ord.info.ToLower().Contains("флюорограф") || ord.info.ToLower().Contains("инкубац") ||
                    ord.info.ToLower().Contains("метеоролог") || ord.info.ToLower().Contains("фасад") ||
                    ord.info.ToLower().Contains("сейф") || ord.info.ToLower().Contains("жалюзи") ||
                    ord.info.ToLower().Contains("акций") || ord.info.ToLower().Contains("промо") ||
                    ord.info.ToLower().Contains("питьев") || ord.info.ToLower().Contains("аутсорс") ||
                    ord.info.ToLower().Contains("землерой") || ord.info.ToLower().Contains("санитар") ||
                    ord.info.ToLower().Contains("топлив") || ord.info.ToLower().Contains("вывоз") ||
                    ord.info.ToLower().Contains("холодил") || ord.info.ToLower().Contains("сертификац") ||
                    ord.info.ToLower().Contains("уборке") || ord.info.ToLower().Contains("уборка") ||
                    ord.info.ToLower().Contains("автобус") || ord.info.ToLower().Contains("пропуск") ||
                    ord.info.ToLower().Contains("дорожн") || ord.info.ToLower().Contains("осветитель") ||
                    ord.info.ToLower().Contains("анализатор") || ord.info.ToLower().Contains("молоко") ||
                    ord.info.ToLower().Contains("молока") || ord.info.ToLower().Contains("хлеб") ||
                    ord.info.ToLower().Contains("лакокрасоч") || ord.info.ToLower().Contains("масло") ||
                    ord.info.ToLower().Contains("теплообмен") || ord.info.ToLower().Contains("индивид") ||
                    ord.info.ToLower().Contains("подшипник") || ord.info.ToLower().Contains("топлив") ||
                    ord.info.ToLower().Contains("проживан") || ord.info.ToLower().Contains("аттестац") ||
                    ord.info.ToLower().Contains("водоотвод") || ord.info.ToLower().Contains("музыка") ||
                    ord.info.ToLower().Contains("левады") || ord.info.ToLower().Contains("левада") ||
                    ord.info.ToLower().Contains("манежа") || ord.info.ToLower().Contains("гигрометр") ||
                    ord.info.ToLower().Contains("зернов") || ord.info.ToLower().Contains("информацион") ||
                    ord.info.ToLower().Contains("радио") || ord.info.ToLower().Contains("учител") ||
                    ord.info.ToLower().Contains("язык") || ord.info.ToLower().Contains("очистн") ||
                    ord.info.ToLower().Contains("труба") || ord.info.ToLower().Contains("трубы") ||
                    ord.info.ToLower().Contains("кондитер") || ord.info.ToLower().Contains("конференц") ||
                    ord.info.ToLower().Contains("жилет") || ord.info.ToLower().Contains("реактив") ||
                    ord.info.ToLower().Contains("табачн") || ord.info.ToLower().Contains("табак") ||
                    ord.info.ToLower().Contains("бумаг") || ord.info.ToLower().Contains("телефон") ||
                    ord.info.ToLower().Contains("тампон") || ord.info.ToLower().Contains("трубопровод") ||
                    ord.info.ToLower().Contains("кондиционер") || ord.info.ToLower().Contains("бытовой") ||
                    ord.info.ToLower().Contains("подгузник") || ord.info.ToLower().Contains("офис") ||
                    ord.info.ToLower().Contains("охранять") || ord.info.ToLower().Contains("охранн") ||
                    ord.info.ToLower().Contains("продовольств") || ord.info.ToLower().Contains("отображен") ||
                    ord.info.ToLower().Contains("воды") || ord.info.ToLower().Contains("вода") ||
                    ord.info.ToLower().Contains("научн") || ord.info.ToLower().Contains("публик") ||
                    ord.info.ToLower().Contains(" охране") || ord.info.ToLower().Contains(" охран") ||
                    ord.info.ToLower().Contains("скобян") || ord.info.ToLower().Contains("теплоснабж") ||
                    ord.info.ToLower().Contains("термометр") || ord.info.ToLower().Contains("страхован") ||
                    ord.info.ToLower().Contains("сервер") || ord.info.ToLower().Contains("захоронен") ||
                    ord.info.ToLower().Contains("светофор") || ord.info.ToLower().Contains("паркомат") ||
                    ord.info.ToLower().Contains("оргтехник") || ord.info.ToLower().Contains("помидор") ||
                    ord.info.ToLower().Contains("шлагбаум") || ord.info.ToLower().Contains("гидротехническ") ||
                    ord.info.ToLower().Contains("подписк") || ord.info.ToLower().Contains("мероприяти") ||
                    ord.info.ToLower().Contains("сигнализац") || ord.info.ToLower().Contains("сушилк") ||
                    ord.info.ToLower().Contains("пищеблок") || ord.info.ToLower().Contains("дезинсекц") ||
                    ord.info.ToLower().Contains("загрязняющ") || ord.info.ToLower().Contains("бандаж") ||
                    ord.info.ToLower().Contains("металлополимер") || ord.info.ToLower().Contains("гофро") ||
                    ord.info.ToLower().Contains("шпатель") || ord.info.ToLower().Contains("подарков") ||
                    ord.info.ToLower().Contains("подарки") || ord.info.ToLower().Contains("посуды") ||
                    ord.info.ToLower().Contains("посуда") || ord.info.ToLower().Contains("коммунальн") ||
                    ord.info.ToLower().Contains("водогрей") || ord.info.ToLower().Contains("здоровь") ||
                    ord.info.ToLower().Contains("хроматограф") || ord.info.ToLower().Contains("вычислитель") ||
                    ord.info.ToLower().Contains("летатель") || ord.info.ToLower().Contains("рециркулятор") ||
                    ord.info.ToLower().Contains("Cisco") || ord.info.ToLower().Contains("полимер") ||
                    ord.info.ToLower().Contains("дверей") || ord.info.ToLower().Contains("страховочн") ||
                    ord.info.ToLower().Contains("калиток") || ord.info.ToLower().Contains("калитка") ||
                    ord.info.ToLower().Contains("дверь") || ord.info.ToLower().Contains("вредоносн") ||
                    ord.info.ToLower().Contains("обучающ") || ord.info.ToLower().Contains("огнестрел") ||
                    ord.info.ToLower().Contains("эколог") || ord.info.ToLower().Contains("нормативо") ||
                    ord.info.ToLower().Contains("аудит") || ord.info.ToLower().Contains("рельсов") ||
                    ord.info.ToLower().Contains("экскаватор") || ord.info.ToLower().Contains("микроклимат") ||
                    ord.info.ToLower().Contains("рейтинг") || ord.info.ToLower().Contains("маммограф") ||
                    ord.info.ToLower().Contains("персонал") || ord.info.ToLower().Contains("моторвагон") ||
                    ord.info.ToLower().Contains("ламинарн") || ord.info.ToLower().Contains("химическ") ||
                    ord.info.ToLower().Contains("атмосфер") || ord.info.ToLower().Contains("вентилятор") ||
                    ord.info.ToLower().Contains("эвм") || ord.info.ToLower().Contains("протез") ||
                    ord.info.ToLower().Contains("арматура") || ord.info.ToLower().Contains("работник") ||
                    ord.info.ToLower().Contains("радиацион") || ord.info.ToLower().Contains("инструмент") ||
                    ord.info.ToLower().Contains("лейкопластыр") || ord.info.ToLower().Contains("скальпел") ||
                    ord.info.ToLower().Contains("впитывающ") || ord.info.ToLower().Contains("автозимник") ||
                    ord.info.ToLower().Contains("климатич") || ord.info.ToLower().Contains("разрушител") ||
                    ord.info.ToLower().Contains("ноутбук") || ord.info.ToLower().Contains("тестирован") ||
                    ord.info.ToLower().Contains("педагог") || ord.info.ToLower().Contains("кухонн")
                    )
                {
                    FilteredOrders.Add(ord);
                }
                else
                {
                    AcseptedOrders.Add(ord);
                }


            }
            List<Order> BuferOrders = new List<Order>();
            BuferOrders.AddRange(FilteredOrders);
            FilteredOrders.Clear();
            foreach (Order ord in BuferOrders)
            {
                if ((
                    ord.info.ToLower().Contains(" нку ") || ord.info.ToLower().Contains(" що-70 ") ||
                    ord.info.ToLower().Contains(" кру ") || ord.info.ToLower().Contains(" крун ") ||
                    ord.info.ToLower().Contains(" зру ") || ord.info.ToLower().Contains(" ору ") ||
                    ord.info.ToLower().Contains(" гру ") || ord.info.ToLower().Contains(" гпп ") ||
                    ord.info.ToLower().Contains(" бртп ") || ord.info.ToLower().Contains(" ктпсн ") ||
                    ord.info.ToLower().Contains(" русн ") || ord.info.ToLower().Contains(" дэс ") ||
                    ord.info.ToLower().Contains(" ксо ") || ord.info.ToLower().Contains(" бмз ") ||
                    ord.info.ToLower().Contains(" ктп ") || ord.info.ToLower().Contains(" бктп ") ||
                    ord.info.ToLower().Contains(" ктпн ") || ord.info.ToLower().Contains(" рза ") ||
                    ord.info.ToLower().Contains(" ртзо ") || ord.info.ToLower().Contains(" ктпсн ") ||
                    ord.info.ToLower().Contains(" опу ") || ord.info.ToLower().Contains(" тп ") ||
                    ord.info.ToLower().Contains(" ртп ") || ord.info.ToLower().Contains(" мрз ") ||
                    //ord.info.ToLower().Contains("асу") || ord.info.ToLower().Contains("распределит") 
                    ord.info.ToLower().Contains("щит") || ord.info.ToLower().Contains("подстанц") ||
                    ord.info.ToLower().Contains("дизел") || ord.info.ToLower().Contains("панел") ||
                    ord.info.ToLower().Contains("энергоцентр") || ord.info.ToLower().Contains("общеподстанцион") ||
                    ord.info.ToLower().Contains("шкаф") || ord.info.ToLower().Contains("ячейк") || ord.info.ToLower().Contains("ячеек") ||
                    ord.info.ToLower().Contains("генератор") || ord.info.ToLower().Contains("элегазов") ||
                    ord.info.ToLower().Contains("распределитель") || ord.info.ToLower().Contains("энергетическ") ||
                    ord.info.ToLower().Contains("трансформатор") || ord.info.ToLower().Contains("крун") ||
                    ord.info.ToLower().Contains("двигател") || ord.info.ToLower().Contains("ктп") || ord.info.ToLower().Contains("рза") ||
                    ord.info.ToLower().Contains("электростанц") || ord.info.ToLower().Contains("энергоблок") ||
                    ord.info.ToLower().Contains("кл-") || ord.info.ToLower().Contains("рп-") ||
                    ord.info.ToLower().Contains("ртп-") || ord.info.ToLower().Contains("тп-") ||
                    ord.info.ToLower().Contains("асу тп") || ord.info.ToLower().Contains(" пс ") ||
                    ord.info.ToLower().Contains("кл-") || ord.info.ToLower().Contains(" тэц ") ||
                    ord.info.ToLower().Contains(" гэс ") || ord.info.ToLower().Contains("электростанци") ||
                    ord.info.ToLower().Contains("що-70") || ord.info.ToLower().Contains("гпп") ||
                    ord.info.ToLower().Contains("ртзо") || ord.info.ToLower().Contains("ктпсн") ||
                    ord.info.ToLower().Contains("вл-") || ord.info.ToLower().Contains("вли-") || ord.info.ToLower().Contains(" вл ")
                    //ord.info.ToLower().Contains("") || ord.info.ToLower().Contains("") ||
                    ))
                {
                    FilteredOrders.Add(ord);
                }

            }

            BuferOrders = new List<Order>();
            BuferOrders.AddRange(AcseptedOrders);
            AcseptedOrders.Clear();
            foreach (Order ord in BuferOrders)
            {
                if (!(
                    ord.info.ToLower().Contains(" нку ") || ord.info.ToLower().Contains(" що-70 ") ||
                    ord.info.ToLower().Contains(" кру ") || ord.info.ToLower().Contains(" крун ") ||
                    ord.info.ToLower().Contains(" зру ") || ord.info.ToLower().Contains(" ору ") ||
                    ord.info.ToLower().Contains(" гру ") || ord.info.ToLower().Contains(" гпп ") ||
                    ord.info.ToLower().Contains(" бртп ") || ord.info.ToLower().Contains(" ктпсн ") ||
                    ord.info.ToLower().Contains(" русн ") || ord.info.ToLower().Contains(" дэс ") ||
                    ord.info.ToLower().Contains(" ксо ") || ord.info.ToLower().Contains(" бмз ") ||
                    ord.info.ToLower().Contains(" ктп ") || ord.info.ToLower().Contains(" бктп ") ||
                    ord.info.ToLower().Contains(" ктпн ") || ord.info.ToLower().Contains(" рза ") ||
                    ord.info.ToLower().Contains(" ртзо ") || ord.info.ToLower().Contains(" ктпсн ") ||
                    ord.info.ToLower().Contains(" опу ") || ord.info.ToLower().Contains(" тп ") ||
                    ord.info.ToLower().Contains(" ртп ") || ord.info.ToLower().Contains(" мрз ") ||
                    //ord.info.ToLower().Contains("асу") || ord.info.ToLower().Contains("распределит") 
                    ord.info.ToLower().Contains("щит") || ord.info.ToLower().Contains("подстанц") ||
                    ord.info.ToLower().Contains("дизел") || ord.info.ToLower().Contains("панел") ||
                    ord.info.ToLower().Contains("энергоцентр") || ord.info.ToLower().Contains("общеподстанцион") ||
                    ord.info.ToLower().Contains("шкаф") || ord.info.ToLower().Contains("ячейк") || ord.info.ToLower().Contains("ячеек") ||
                    ord.info.ToLower().Contains("генератор") || ord.info.ToLower().Contains("элегазов") ||
                    ord.info.ToLower().Contains("распределитель") || ord.info.ToLower().Contains("энергетическ") ||
                    ord.info.ToLower().Contains("трансформатор") || ord.info.ToLower().Contains("крун") ||
                    ord.info.ToLower().Contains("двигател") || ord.info.ToLower().Contains("ктп") || ord.info.ToLower().Contains("рза") ||
                    ord.info.ToLower().Contains("электростанц") || ord.info.ToLower().Contains("энергоблок") ||
                    ord.info.ToLower().Contains("кл-") || ord.info.ToLower().Contains("рп-") ||
                    ord.info.ToLower().Contains("ртп-") || ord.info.ToLower().Contains("тп-") ||
                    ord.info.ToLower().Contains("асу тп") || ord.info.ToLower().Contains(" пс ") ||
                    ord.info.ToLower().Contains("кл-") || ord.info.ToLower().Contains(" тэц ") ||
                    ord.info.ToLower().Contains(" гэс ") || ord.info.ToLower().Contains("электростанци") ||
                    ord.info.ToLower().Contains("що-70") || ord.info.ToLower().Contains("гпп") ||
                    ord.info.ToLower().Contains("ртзо") || ord.info.ToLower().Contains("ктпсн") ||
                    ord.info.ToLower().Contains("вл-") || ord.info.ToLower().Contains("вли-") || ord.info.ToLower().Contains(" вл ")
                    //ord.info.ToLower().Contains("") || ord.info.ToLower().Contains("") ||
                    ))
                {
                    FilteredOrders.Add(ord);
                }
                else
                {
                    AcseptedOrders.Add(ord);
                }
            }
            BlockedOrders.AddRange(FilteredOrders);
            return AcseptedOrders;
        }

        /*
         Regex regexabriviaturs = new Regex(@"(((\s|^)нку)|((\s|^)що-70)|((\s|^)кру)|((\s|^)крун)|((\s|^)зру)|((\s|^)ору)|
         ((\s|^)ру)|((\s|^)гру)|((\s|^)гпп)|((\s|^)бртп)|((\s|^)ктпсн)|((\s|^)русн)|((\s|^)дэс)|((\s|^)ксо)|((\s|^)бмз)|((\s|^)ктп)|
         ((\s|^)бктп)|((\s|^)ктпн)|((\s|^)рза)|((\s|^)пс)|((\s|^)ртзо)|((\s|^)ктпсн)|((\s|^)опу)|((\s|^)тп)|((\s|^)ртп)|((\s|^)мрз)|
         ((\s|^)асу))|((распределит)|(устройств)|(щит)|(подстанц)|(электростанц)|(дизел)|(панел)|(управл)|(энергоцентр)|(общеподстанцион)|
         (пункт)|(шкаф )|(ячейк))", RegexOptions.IgnoreCase);
        */
        //Regex regexcutwords = new Regex(@"((распределит) | (устройств) | (щит) | (подстанц) | (электростанц) | (дизел) | (панел) | (управл) | (энергоцентр) | (общеподстанцион) | (пункт) | (шкаф ) | (ячейк))", RegexOptions.IgnoreCase);


    }
  

}


