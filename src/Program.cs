using Newtonsoft.Json;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ZmWatchDog.Zm;

namespace ZmWatchDog
{
    class Program
    {
        private static int ErrorQty { get; set; } = 0;
        private static int OKQty { get; set; } = 0;

        private static int HashrateErrorQty { get; set; } = 0;

        private static void ConsoleOut(string text) => Console.WriteLine($"{DateTime.Now} {text}");

        static void Main(string[] args)
        {
            Config GetConf()
            {
                Config conf;
                try
                {
                    conf =
                        JsonConvert.DeserializeObject<Config>(File.ReadAllText(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json")));
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().Error($"GetConf: {e.Message}");
                    ConsoleOut("Start default settings");
                    conf = new Config();
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"),
                        JsonConvert.SerializeObject(conf));
                }
                return conf;
            }


            LogManager.GetCurrentClassLogger().Info("Start app");
            var config = GetConf();

            double sumHashrate = 0;

            while (true)
            {
                try
                {
                    var response = ZmHelper.GetStat(config.Ip, config.Port);

                    if (response.error != null)
                        ErrorQty++;
                    else
                    {
                        OKQty++;
                        //обнуляем счетчик ошибок
                        ErrorQty = 0;


                        sumHashrate = response.result.Sum(x => x.sol_ps);

                        LogManager.GetCurrentClassLogger().Info($"Hashrate: {sumHashrate}");

                    }

                    ConsoleOut("Get stat OK");
                }
                catch (Exception e)
                {
                    ConsoleOut(e.Message);
                    LogManager.GetCurrentClassLogger().Error($"GetStat: {e.Message}");
                    ErrorQty++;
                }

                if (config.MinHashrate.HasValue)
                {
                    if (sumHashrate < config.MinHashrate.Value)
                        HashrateErrorQty++;
                    else HashrateErrorQty = 0;


                    if (HashrateErrorQty > 5)
                    {
                        ConsoleOut($"Going to reboot. Current hashrate: {sumHashrate}");

                        LogManager.GetCurrentClassLogger().Info($"Going to reboot. Hashrate: {sumHashrate}");
                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last.json"),
                            DateTime.Now.ToString());
                        Process.Start("shutdown", "/r /t 0 /f");
                    }
                }



                if (ErrorQty >= config.ErrorMax)
                {
                    if (OKQty >= 0)
                    {
                        ConsoleOut("Going to reboot");

                        LogManager.GetCurrentClassLogger().Info("Going to reboot");
                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last.json"),
                            DateTime.Now.ToString());
                        Process.Start("shutdown", "/r /t 0 /f");
                    }
                    else
                    {
                        LogManager.GetCurrentClassLogger().Info("OKQty = 0. Rebooting cancelled");
                    }
                }


                Thread.Sleep(TimeSpan.FromSeconds(config.FrequencySec));
            }
        }

    }
}
