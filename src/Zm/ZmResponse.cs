using System.Collections.Generic;

namespace ZmWatchDog.Zm
{
    public class Result
    {
        public int gpu_id { get; set; }
        public int temperature { get; set; }
        public double sol_ps { get; set; }
        public double avg_sol_ps { get; set; }
        public double sol_pw { get; set; }
        public double avg_sol_pw { get; set; }
        public double power_usage { get; set; }
        public double avg_power_usage { get; set; }
        public int accepted_shares { get; set; }
        public int rejected_shares { get; set; }
        public int latency { get; set; }
    }

    public class ZmResponse
    {
        public int id { get; set; }
        public List<Result> result { get; set; }
        public int uptime { get; set; }
        public int contime { get; set; }
        public string server { get; set; }
        public int port { get; set; }
        public string user { get; set; }
        public string version { get; set; }
        public object error { get; set; }
    }
}
