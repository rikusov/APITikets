using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace APITikets.Module{
    /// <summary>
    /// Класс для десереализации ответа
    /// </summary>
    public class GetDataJson
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "data")]
        public List<TiketsJson> DataJson { get; set; }
    }
}
