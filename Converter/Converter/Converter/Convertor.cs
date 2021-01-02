using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Converter
{
    public class Convertor : INotifyPropertyChanged
    {
        private double fEntry { get; set; }
        public double firstEntry //певрое поле ввода
        {
            get
            {
                return fEntry;
            }
            set
            {
                if (fEntry != value )
                {
                    fEntry = value;
                    if (!lockerRashet && !checkLoad)
                    {
                        lockerRashet = true;
                        secoundEntry = rashet1();
                        save();
                    }
                    OnPropertyChanged("firstEntry");
                    lockerRashet = false;
                }
            }
        }
        private double sEntry { get; set; }
        public double secoundEntry
        {
            get
            {
                return sEntry;
            }
            set
            {
                if (sEntry != value)
                {
                    sEntry = value;
                    if (!lockerRashet && !checkLoad)
                    {
                        lockerRashet = true;
                        firstEntry = rashet2();
                        save();
                    }
                    OnPropertyChanged("secoundEntry");
                    lockerRashet = false;
                }
            }
        }

        public bool lockRash;
        public DateTime _date { get; set; }
        public DateTime date //хранение даты
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged("date");
                }
            }
        }
        private string fItem { get; set; }
        public string firstItem //первое поле выбора валюты
        {
            get
            {
                return fItem;
            }
            set
            {
                if (fItem != value)
                {
                    fItem = value;
                    OnPropertyChanged("firstItem");
                }
            }
        }
        private string sItem { get; set; }
        public string secoundItem
        {
            get
            {
                return sItem;
            }
            set
            {
                if (sItem != value)
                {
                    sItem = value;
                    OnPropertyChanged("secoundItem");
                }
            }
        }
        public ObservableCollection<ValuteInfoView> list { get; set; }

        string url = "https://www.cbr-xml-daily.ru/daily_json.js";

        private string firstIndex;

        private string secoundIndex;

        public bool lockerRashet;

        static object locker = new object();
        private DateTime lastDate { get; set; }

        public bool checkLoad;
        public Convertor()
        {
            date = DateTime.UtcNow;
            list = new ObservableCollection<ValuteInfoView>();
            load();
            addToList(request());
            lastDate = date;
            firstItem = firstIndex;
            secoundItem = secoundIndex;
            Task.Run(convert);        
        }
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        List<JToken> request() //запрос на получение информации о валюте
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = httpClient.GetAsync(url).Result;
            while (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                response = httpClient.GetAsync(url).Result;
                Thread.Sleep(50);
            }
            var content = response.Content.ReadAsStringAsync();
            
            JObject jObject = JObject.Parse(content.Result);
            var values = jObject.Values().ToList();
            while (date.Date < values[0].ToObject<DateTime>().Date)
            {
                response = httpClient.GetAsync("https:" + values[2].ToString()).Result;
                while (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var url2 = "https://www.cbr-xml-daily.ru/archive/"
                        + values[0].ToObject<DateTime>().Year
                        + "/"
                        + values[0].ToObject<DateTime>().Month
                        + "/"
                        + values[0].ToObject<DateTime>().Day
                        + "/daily_json.js";
                    response = httpClient.GetAsync(new Uri(url2)).Result;
                    values[0] = values[0].ToObject<DateTime>().AddDays(-1);
                    Thread.Sleep(50);
                }
                Thread.Sleep(50);
                content = response.Content.ReadAsStringAsync();
                jObject = JObject.Parse(content.Result);
                values = jObject.Values().ToList();
            }
            var str = jObject.SelectToken(@"$.Valute").Values();
            return str.ToList<JToken>();
        }
        void addToList(List<JToken> yes)//обработка полученного списка валют
        {
            if (list.Count == 0)
            {
                foreach (var item in yes)
                {
                    var itemInfo = JsonConvert.DeserializeObject<ValuteInfo>(item.ToString());
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        list.Add(new ValuteInfoView
                        {
                            ID = itemInfo.ID,
                            Name = itemInfo.Name,
                            ChacCode = itemInfo.ChacCode,
                            Nominal = itemInfo.Nominal,
                            NumCode = itemInfo.NumCode,
                            Previous = itemInfo.Previous,
                            Value = itemInfo.Value
                        });
                    });
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    list.Add(new ValuteInfoView
                    {
                        Value = 1,
                        Name = "Российский рубль",
                        ChacCode = "RUB",
                        Nominal = 1
                    });
                });
                return;
            }

            for (var i = 0; i < yes.Count - 1; i++)
            {
                var itemInfo = JsonConvert.DeserializeObject<ValuteInfo>(yes[i].ToString());
                lock (locker)
                { 
                    Device.BeginInvokeOnMainThread(() =>
                    {
                         list[i] = new ValuteInfoView
                         {
                             ID = itemInfo.ID,
                             Name = itemInfo.Name,
                             ChacCode = itemInfo.ChacCode,
                             Nominal = itemInfo.Nominal,
                             NumCode = itemInfo.NumCode,
                             Previous = itemInfo.Previous,
                             Value = itemInfo.Value
                         };
                    });
                    Thread.Sleep(50);
                }
            }
                
            Device.BeginInvokeOnMainThread(() =>
            {
                list[yes.Count] = new ValuteInfoView
                {
                    Value = 1,
                    Name = "Российский рубль",
                    ChacCode = "RUB",
                    Nominal = 1
                };
                Thread.Sleep(50);
            });

        }
        void  save()
        {
            CrossSettings.Current.AddOrUpdateValue("firstEntry",    firstEntry);
            CrossSettings.Current.AddOrUpdateValue("secoundEntry",  secoundEntry);
            CrossSettings.Current.AddOrUpdateValue("date",          date);
            CrossSettings.Current.AddOrUpdateValue("firstItem",     firstItem);
            CrossSettings.Current.AddOrUpdateValue("secoundItem",   secoundItem);
        }
        void load()//загрузка настроек
        {
            checkLoad = true;
            firstEntry      =   CrossSettings.Current.GetValueOrDefault("firstEntry",   firstEntry);
            secoundEntry    =   CrossSettings.Current.GetValueOrDefault("secoundEntry", secoundEntry);
            date            =   CrossSettings.Current.GetValueOrDefault("date",         date);
            firstItem       =   CrossSettings.Current.GetValueOrDefault("firstItem",    firstItem);
            secoundItem     =   CrossSettings.Current.GetValueOrDefault("secoundItem",  secoundItem);
            firstIndex          = firstItem;
            secoundIndex        = secoundItem;
            checkLoad = false;
        }
        double rashet1() //расчёт второго поля на основании первого
        {
            double secoundEntry = list[Convert.ToInt32(firstItem)].Value * firstEntry
                          / (list[Convert.ToInt32(secoundItem)].Value * list[Convert.ToInt32(firstItem)].Nominal);
            secoundEntry = Math.Round(secoundEntry, 2);
            return secoundEntry;
        }
        double rashet2() // наоборот 
        {
            double firstEntry = list[Convert.ToInt32(secoundItem)].Value * secoundEntry
                            / (list[Convert.ToInt32(firstItem)].Value * list[Convert.ToInt32(secoundItem)].Nominal);
            firstEntry = Math.Round(firstEntry, 2);
            return firstEntry;
        }
        void convert()
        {
            while(true)
            {
                if (lastDate.Date != date.Date)
                {
                    firstIndex = firstItem;
                    secoundIndex = secoundItem;
                    var val = request();
                    checkLoad = true;
                    addToList(val);
                    checkLoad = false;
                    lastDate = date;
                    firstItem = firstIndex;
                    secoundItem = secoundIndex;
                    secoundEntry = rashet1();
                    save();
                }

                if (firstItem != firstIndex)
                {
                    firstIndex = firstItem;
                    checkLoad = true;
                    secoundEntry = rashet1();
                    checkLoad = false;
                }
                if (secoundItem != secoundIndex)
                {
                    secoundIndex = secoundItem;
                    checkLoad = true;
                    secoundEntry = rashet1();
                    checkLoad = false;
                }
            }
        }
    }
}
