
using LoadShedder.Common;
using LoadShedder.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;
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
            MainDataContext.ADCMainDividingResistor = settings.GetValue<double>("ADCMainDividingResistor", 2000);
            MainDataContext.ADCResolution = settings.GetValue<double>("ADCResolution", 4096);
            MainDataContext.ADCMainVoltage = settings.GetValue<double>("ADCMainVoltage", 5000);

            /*
            settings.GetSection("GamePieces").Bind(MainDataContext.GamePieces);
            var newgpcs = new Dictionary<ResistorsCombos1, GamePiece>();

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GamePiecesResistorsCombosValues.txt")))
            {
                var newrcs = new List<int>();

                var comboValues = new StringBuilder();

                comboValues.AppendLine($"ComboName;Value;Resistors;MainADCResistorVoltage;ComboVoltage;");
                foreach (int rc in Enum.GetValues(typeof(ResistorsCombos1)))
                {
                    if ((ResistorsCombos1)rc != ResistorsCombos1.None)
                    {
                        newrcs.Add((int)rc);
                        var res = GamePiece.DecomposeCombo1IntoResistors((ResistorsCombos1)rc);
                        var r = new StringBuilder();
                        var ix = 0;
                        foreach (var rr in res)
                        {
                            r.Append($"{rr.ToString()}");
                            if (ix < res.Count - 1)
                                r.Append(" + ");
                            //else if (ix == res.Count - 1)
                            //r.Append("\n");
                            ix++;
                        }
                        var tmpgp = new GamePiece() { ResistorsCombo1 = (ResistorsCombos1)rc };

                        comboValues.AppendLine($"{Enum.GetName(typeof(ResistorsCombos1), rc)};{rc};{r.ToString()};{Math.Round(tmpgp.GetMainResistorVoltage(), 0)};{Math.Round(tmpgp.GetGamePieceResistorVoltage(), 0)}");
                    }
                }
                var index = 1;

                newrcs.Sort();

                foreach (var g in MainDataContext.GamePieces.Values)
                {
                    if (index < newrcs.Count)
                    {
                        newgpcs.TryAdd((ResistorsCombos1)newrcs[index], new GamePiece()
                        {
                            Description = g.Description,
                            EnergyValue = g.EnergyValue,
                            GamePieceType = g.GamePieceType,
                            Id = g.Id,
                            Name = g.Name,
                            ResistorsCombo1 = (ResistorsCombos1)newrcs[index]
                        });
                        index++;
                    }
                }

                FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GamePiecesResistorsCombosValues.txt"), comboValues.ToString());
            }

            var obj = JsonConvert.SerializeObject(newgpcs, Formatting.Indented);
            FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GamePieces1.json"), obj);
            */

            
            settings.GetSection("GameSettings").Bind(MainDataContext.GameSettings);

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
                    var dtos = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Models.Dto.GameBoardDto>>(filecontent) ?? new ConcurrentDictionary<string, Models.Dto.GameBoardDto>();
                    if (dtos != null)
                    {
                        foreach (var d in dtos)
                            MainDataContext.GameBoards.TryAdd(d.Key, new GameBoard(d.Value));
                    }
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
                if (!MainDataContext.Players.ContainsKey("fyziktom"))
                {
                    MainDataContext.Players.TryAdd("fyziktom", new Models.Player()
                    {
                        Id = "fyziktom",
                        Name = "Tomas Svoboda"
                    });
                }

                if (!MainDataContext.Devices.ContainsKey("test"))
                {
                    var device = new Models.Device()
                    {
                        Id = "test",
                        Name = "Test Device"
                    };

                    device.AddChannel(11, "1-Westpunt");
                    device.AddChannel(24, "2-Barber");
                    device.AddChannel(9, "3-Soto");
                    device.AddChannel(13, "4-Wind");
                    device.AddChannel(14, "5-TeraKora");
                    device.AddChannel(12, "6-Solar");
                    device.AddChannel(28, "7-St.Maria");
                    device.AddChannel(1, "8-Isla");
                    device.AddChannel(2, "9-Bueheuvel");
                    device.AddChannel(29, "10-Piscadera");
                    device.AddChannel(30, "11-Gas");
                    device.AddChannel(31, "12-Otrobanda");
                    device.AddChannel(25, "13-Buevengat");
                    device.AddChannel(17, "14-Wind");
                    device.AddChannel(18, "15-Solar");
                    device.AddChannel(5, "16-Diesel_1");
                    device.AddChannel(22, "17-Diesel_2");
                    device.AddChannel(6, "18-Diesel_3");
                    device.AddChannel(4, "19-Punda");
                    device.AddChannel(7, "20-Koraalspecht");
                    device.AddChannel(23, "21-Jan_Thiel");
                    device.AddChannel(19, "22-Montana");
                    device.AddChannel(21, "23-Wind");
                    device.AddChannel(20, "24-Fuik");


                    MainDataContext.Devices.TryAdd("test", device);
                }

                if (!MainDataContext.GameBoards.ContainsKey("testBoard"))
                {
                    var gameboard = new Models.GameBoard()
                    {
                        DeviceId = "test",
                        Id = "testBoard",
                        Name = "Test Board",
                        PlayerId = "fyziktom"
                    };

                    gameboard.AddPosition(null, "1-Westpunt", "test", null, 11, null);
                    gameboard.AddPosition(null, "2-Barber", "test", null, 24, null);
                    gameboard.AddPosition(null, "3-Soto", "test", null, 9, null);
                    gameboard.AddPosition(null, "4-Wind", "test", null, 13, null);
                    gameboard.AddPosition(null, "5-TeraKora", "test", null, 14, null);
                    gameboard.AddPosition(null, "6-Solar", "test", null, 12, null);
                    gameboard.AddPosition(null, "7-St.Maria", "test", null, 28, null);
                    gameboard.AddPosition(null, "8-Isla", "test", null, 1, null);
                    gameboard.AddPosition(null, "9-Bueheuvel", "test", null, 2, null);
                    gameboard.AddPosition(null, "10-Piscadera", "test", null, 29, null);
                    gameboard.AddPosition(null, "11-Gas", "test", null, 30, null);
                    gameboard.AddPosition(null, "12-Otrobanda", "test", null, 31, null);
                    gameboard.AddPosition(null, "13-Buevengat", "test", null, 25, null);
                    gameboard.AddPosition(null, "14-Wind", "test", null, 17, null);
                    gameboard.AddPosition(null, "15-Solar", "test", null, 18, null);
                    gameboard.AddPosition(null, "16-Diesel_1", "test", null, 5, null);
                    gameboard.AddPosition(null, "17-Diesel_2", "test", null, 22, null);
                    gameboard.AddPosition(null, "18-Diesel_3", "test", null, 6, null);
                    gameboard.AddPosition(null, "19-Punda", "test", null, 4, null);
                    gameboard.AddPosition(null, "20-Koraalspecht", "test", null, 7, null);
                    gameboard.AddPosition(null, "21-Jan_Thiel", "test", null, 23, null);
                    gameboard.AddPosition(null, "22-Montana", "test", null, 19, null);
                    gameboard.AddPosition(null, "23-Wind", "test", null, 21, null);
                    gameboard.AddPosition(null, "24-Fuik", "test", null, 20, null);
                    MainDataContext.GameBoards.TryAdd("testBoard", gameboard);
                }

                if (!MainDataContext.Games.ContainsKey("testGame"))
                {
                    MainDataContext.Games.TryAdd("testGame", new Models.Game()
                    {
                        Id = "testBoard",
                        GameBoardIds = new List<string>() { "testBoard" }
                    });
                }
                else
                {
                    if (MainDataContext.Games.TryGetValue("testGame", out var game))
                        game.PlayersGameData.Clear();
                }

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Cannot init the basic configuration" + ex);
            }

            await Console.Out.WriteLineAsync("");
            await Console.Out.WriteLineAsync("");
            await Console.Out.WriteLineAsync("Starting main loop...");
            var savetimes = 30;

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
                                if (savetimes <= 0)
                                {
                                    FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Players.json"),
                                                                JsonConvert.SerializeObject(MainDataContext.Players, Formatting.Indented));
                                    FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Games.json"),
                                                                JsonConvert.SerializeObject(MainDataContext.Games, Formatting.Indented));
                                    FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Devices.json"),
                                                                JsonConvert.SerializeObject(MainDataContext.Devices, Formatting.Indented));
                                    FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameBoards.json"),
                                                                JsonConvert.SerializeObject(MainDataContext.GameBoards, Formatting.Indented));
                                    savetimes = 30;
                                }
                                else
                                {
                                    savetimes--;
                                }


                                // recconect the events 
                                foreach(var game in MainDataContext.Games.Values)
                                {
                                    game.GameRespondingAction -= Game_GameRespondingAction;
                                    game.GameRespondingAction += Game_GameRespondingAction;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Cannot save server state to the file.");
                            }

                            await Task.Delay(1000);

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

        private void Game_GameRespondingAction(object? sender, GameResponseActionEventArgs e)
        {
            if (MainDataContext.GameResponseActions.TryGetValue(e.GameId, out var que))
                que.Enqueue(e);
            else
            {
                var q = new ConcurrentQueue<GameResponseActionEventArgs>();
                q.Enqueue(e);
                MainDataContext.GameResponseActions.TryAdd(e.GameId, q);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }
    }
}
