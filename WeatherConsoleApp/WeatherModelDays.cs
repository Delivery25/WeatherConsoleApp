using Newtonsoft.Json;

namespace WeatherConsoleApp
{

    public class WeatherModelDays
    {
        public string Cod { get; set; }
        public int Message { get; set; }
        public int Cnt { get; set; }
        public List[] List { get; set; }
        public City City { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Coord Coord { get; set; }
        public string Country { get; set; }
        public int Population { get; set; }
        
        [JsonProperty("timezone")]
        public int TimeZone { get; set; }
        public int Sunrise { get; set; }
        public int Sunset { get; set; }
    }

    public class List
    {
        public int Dt { get; set; }
        public Root Main { get; set; }
        public Weather[] Weather { get; set; }
        public Clouds Clouds { get; set; }
        public Wind Wind { get; set; }
        public int Visibility { get; set; }
        public float Pop { get; set; }
        public System Sys { get; set; }

        [JsonProperty("dt_txt")]
        public string DtTxt { get; set; }
    }

    public class Root
    {
        public float Temp { get; set; }
        [JsonProperty("feels_like")]
        public float FeelsLike { get; set; }

        [JsonProperty("temp_min")]
        public float TempMin { get; set; }

        [JsonProperty("temp_max")]
        public float TempMax { get; set; }
        public int Pressure { get; set; }
        
        [JsonProperty("sea_level")]
        public int SeaLevel { get; set; }

        [JsonProperty("grnd_level")]
        public int GrndLevel { get; set; }
        public int Humidity { get; set; }
        
        [JsonProperty("temp_kf")]
        public float TempKf { get; set; }
    }


    public class System
    {
        public string Pod { get; set; }
    }
}
