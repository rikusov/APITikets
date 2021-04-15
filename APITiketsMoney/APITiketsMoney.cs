using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using DocsVision.Platform.StorageServer.Extensibility;
using APITikets.Module;

namespace APITikets{
    public class APITiketsMoney : StorageServerExtension {

        /// <summary>
        /// Класс для запроса минимальной ценны билета по заданому направлению
        /// </summary>
        /// <param name="DateRaice"> дата вылета</param>
        /// <param name="Kod_port_out"> локация вылета</param>
        /// <param name="Kod_port_in"> локация прилета</param>
        /// <returns>
        ///     возращает 0.0m если билетов нет
        ///     возвращает -1.0m если произошла ошибка
        ///     в случае если все ок возвращает цену
        /// </returns>
        private decimal GetPriceTo(DateTime DateRaice, string Kod_port_out, string Kod_port_in) {
            string begin_month = (new DateTime(DateRaice.Year, DateRaice.Month, 1)).ToString("yyyy-MM-dd");

            string string_url = "http://api.travelpayouts.com/v2/prices/latest?currency=rub&origin={0}&destination={1}&beginning_of_period={2}&page={3}&limit=1000&show_to_affiliates=false&token=21a863d2f1b7329e55be16d22f588f51";

            List<TiketsJson> ListTikets = new List<TiketsJson>();

            int i = 1;

            while (true) {
                HttpWebRequest request = WebRequest.Create(String.Format(string_url, Kod_port_out, Kod_port_in, begin_month, i)) as HttpWebRequest;
                request.Method = "GET";
                request.Accept = "application/json";

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                try
                {
                    string output = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    GetDataJson getdata = JsonConvert.DeserializeObject<GetDataJson>(output);

                    if (getdata.Success){
                        if (getdata.DataJson.Count != 0) ListTikets.AddRange(getdata.DataJson);

                        if (getdata.DataJson.Count < 1000) break;

                        i++;
                    }
                    else { return -1.0m; }
                }
                catch { return -1.0m; }
            }

            if (ListTikets.Count != 0) {
                IEnumerable<decimal> return_tikets = ListTikets.Where(x => x.DateDepart == DateRaice).Select(p => p.ValuePrice);

                if (return_tikets.Count() == 0) return 0.0m;
                else return return_tikets.Min();
            }

            return 0.0m;
        }
        /// <summary>
        /// Класс для возврвщения суммы билетов полета туда обратно
        /// </summary>
        /// <param name="KodCityDistant">Код локации куда летим</param>
        /// <param name="DateDeparture">дата вылета</param>
        /// <param name="DateArrival">дата возвращения</param>
        /// <returns>
        ///     возвращает -4.0m если при запросе билета произошла ошибка
        ///     возвращает -1.0m если нет билетов туда
        ///     возвращает -2.0m если нет билетов обратно
        ///     возвращает -3.0m если нет билетов туда-обратно
        ///     в случае если все ок возвращает сумму цену
        /// </returns>
        [ExtensionMethod]
        public decimal GetTikets(string KodCityDistant, DateTime DateDeparture, DateTime DateArrival){

            decimal output = 0.0m;

            decimal a = GetPriceTo(DateDeparture, "LED", KodCityDistant);

            if (Decimal.Compare(a, -1.0m) == 0) return -4.0m;
            else if (Decimal.Compare(a, 0.0m) == 0) output = -1.0m;
            else output += a;

            decimal b = GetPriceTo(DateArrival, KodCityDistant, "LED");

            if (Decimal.Compare(b, -1.0m) == 0) return -4.0m;
            else if (Decimal.Compare(b, 0.0m) == 0)
                    if (Decimal.Compare(output, -1.0m) == 0) output += -2.0m;
                    else output = -2.0m;
            else if (Decimal.Compare(output, -1.0m) != 0) output += b;

            return output;
        }
    
    }
}
