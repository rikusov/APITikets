using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using DocsVision.Platform.StorageServer.Extensibility;

namespace APITikets{
    public class APITiketsMoney : StorageServerExtension {
        //реквест запрос + парс
        private decimal GetPriceTo(DateTime DateRaice, string Kod_port_out,string Kod_port_in) {
            string begin_month = (new DateTime(DateRaice.Year,DateRaice.Month,1)).ToString("yyyy-MM-dd");

            string string_url = "http://map.aviasales.ru/prices.json?origin_iata={0}&period={1}:month&direct=true&one_way=true&no_visa=true&schengen=true&need_visa=false&locale=ru";

            HttpWebRequest request = WebRequest.Create(String.Format(string_url,Kod_port_out,begin_month)) as HttpWebRequest;

            request.Method = "GET";
            request.Accept = "application/json";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string output = new StreamReader(response.GetResponseStream()).ReadToEnd();

            IEnumerable<TiketsJson> ListTikets = JsonConvert.DeserializeObject<IEnumerable<TiketsJson>>(output);

            ListTikets = (ListTikets.Where(x => x.destination == Kod_port_in && x.depart_date == DateRaice));

            if (ListTikets.Count() == 0) return 0.0m;

            return ListTikets.Select(y => y.value).Min();
        }
        //метод в ДВ
        [ExtensionMethod]
        public decimal GetTikets(string KodCityDistant, DateTime DateDeparture, DateTime DateArrival) {
            decimal PriceThere = GetPriceTo(DateDeparture, "LED", KodCityDistant);

            if (Decimal.Compare(PriceThere, 0.0m) <= 0) return 0.0m; 

            decimal PriceFrom = GetPriceTo(DateArrival, KodCityDistant, "LED");

            if (Decimal.Compare(PriceFrom, 0.0m) <= 0) return 0.0m;

            return PriceThere+PriceFrom;
        }
    
    }
}
