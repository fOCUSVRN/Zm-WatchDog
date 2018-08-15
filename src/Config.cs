namespace ZmWatchDog
{
    public class Config
    {
        public int ErrorMax { get; set; } = 5;
        public string Ip { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 2222;

        public int? MinHashrate { get; set; }

        public int FrequencySec { get; set; } = 30;

    }
}
