using System;
using Newtonsoft.Json;

namespace APITikets.Module
{
    /// <summary>
    /// Класс для десереализации билета
    /// </summary>
    public class TiketsJson{
        [JsonProperty(PropertyName = "value")]
        public decimal ValuePrice { get; set; }

        [JsonProperty(PropertyName = "depart_date")]
        public DateTime DateDepart { get; set; }
    }

}
