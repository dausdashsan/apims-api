namespace APIMS_Api.Models
{
    public class StationAttributes
    {
        public string STATION_ID { get; set; } = string.Empty;
        public long DATETIME { get; set; }
        public double? API { get; set; }
        public int? API_PM10 { get; set; }
        public string? PARAM_SELECTED { get; set; }
        public string? PARAM_SYMBOL { get; set; }
        public string? PARAM_SYMBOL_PM10 { get; set; }
        public string? CLASS { get; set; }
        public string? STATION_LOCATION { get; set; }
        public double? LONGITUDE { get; set; }
        public double? LATITUDE { get; set; }
        public string? PLACE { get; set; }
        public string? LOT_INFO { get; set; }
        public string? STATION_CATEGORY { get; set; }
        public string? STATE_NAME { get; set; }
        public string? REGION_NAME { get; set; }
    }
}
