
using LoadShedder.Common;
using LoadShedder.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using VEDriversLite.Common;

namespace LoadShedder
{
    public class LoadShedderCoreService : BackgroundService
    {
        private IConfiguration settings;
        private IHostApplicationLifetime lifetime;

        public LoadShedderCoreService(IConfiguration settings, IHostApplicationLifetime lifetime)
        {
            this.settings = settings; //startup configuration in appsettings.json
            this.lifetime = lifetime;

            // Load custom configuration from AppSettings.json
            #region LoadConfigData

            MainDataContext.ADCVoltageTolerance = settings.GetValue<double>("ADCVoltageTolerance", 5);
            MainDataContext.ADCMainDividingResistor = settings.GetValue<double>("ADCMainDividingResistor", 100000);
            MainDataContext.ADCResolution = settings.GetValue<double>("ADCResolution", 4096);
            MainDataContext.ADCMainVoltage = settings.GetValue<double>("ADCMainVoltage", 5000);

            settings.GetSection("GamePieces").Bind(MainDataContext.GamePieces);

            #endregion
       }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            await Task.Delay(1);

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Devices.json")))
            {
                var filecontent = FileHelpers.ReadTextFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Devices.json"));
                try
                {
                    MainDataContext.Devices = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Device>>(filecontent) ?? new ConcurrentDictionary<string, Device>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot deserialize the list of the devices. Please check the file consitency.");
                }
            }
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Games.json")))
            {
                var filecontent = FileHelpers.ReadTextFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Games.json"));
                try
                {
                    MainDataContext.Games = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Game>>(filecontent) ?? new ConcurrentDictionary<string, Game>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot deserialize the list of the gameboards. Please check the file consitency.");
                }
            }
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameBoards.json")))
            {
                var filecontent = FileHelpers.ReadTextFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameBoards.json"));
                try
                {
                    MainDataContext.GameBoards = JsonConvert.DeserializeObject<ConcurrentDictionary<string, GameBoard>>(filecontent) ?? new ConcurrentDictionary<string, GameBoard>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot deserialize the list of the gameboards. Please check the file consitency.");
                }
            }
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Players.json")))
            {
                var filecontent = FileHelpers.ReadTextFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Players.json"));
                try
                {
                    MainDataContext.Players = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Player>>(filecontent) ?? new ConcurrentDictionary<string, Player>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot deserialize the list of the players. Please check the file consitency.");
                }
            }

            try
            {

                if (!MainDataContext.Devices.ContainsKey("test"))
                {
                    MainDataContext.Devices.TryAdd("test", new Models.Device()
                    {
                        Id = "test",
                        Name = "Test Device"
                    });
                }

                if (!MainDataContext.GameBoards.ContainsKey("testBoard"))
                {
                    MainDataContext.GameBoards.TryAdd("testBoard", new Models.GameBoard()
                    {
                        DeviceId = "test",
                        Id = "testBoard",
                        Name = "Test Board"
                    });
                }
                
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Cannot init the basic configuration" + ex);
            }

            await Console.Out.WriteLineAsync("");
            await Console.Out.WriteLineAsync("");
            await Console.Out.WriteLineAsync("Starting main loop...");
            try
            {
                _ = Task.Run(async () =>
                {
                    while (!stopToken.IsCancellationRequested)
                    {
                        try
                        {
                            try
                            {
                                FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Players.json"), 
                                                            JsonConvert.SerializeObject(MainDataContext.Players, Formatting.Indented));
                                FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Games.json"),
                                                            JsonConvert.SerializeObject(MainDataContext.Games, Formatting.Indented));
                                FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Devices.json"),
                                                            JsonConvert.SerializeObject(MainDataContext.Devices, Formatting.Indented));
                                FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameBoards.json"),
                                                            JsonConvert.SerializeObject(MainDataContext.GameBoards, Formatting.Indented));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Cannot save server state to the file.");
                            }

                            await Task.Delay(15000);

                        }
                        catch (Exception ex)
                        {
                            await Console.Out.WriteLineAsync("Error occured in LoadShedderCoreService." + ex.Message);
                            await Task.Delay(1000);
                        }
                    }
                    await Console.Out.WriteLineAsync($"Core Service handler task stopped");
                });

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Cannot start Core service:" + ex.Message);
                lifetime.StopApplication();
            }

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }
    }
}
