﻿
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
                    MainDataContext.Devices.TryAdd("test", new Models.Device()
                    {
                        Id = "test",
                        Name = "Test Device"
                    });
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

                    var gamepieces = new List<GamePiece>()
                    {
                       new GamePiece() { Name = "Solar 2", ExpectedVoltage = 3000, EnergyValue = 10000, GamePieceType = GamePieceTypes.Source },
                       new GamePiece() { Name = "Solar 2", ExpectedVoltage = 0, EnergyValue = 7500, GamePieceType = GamePieceTypes.Source},
                       new GamePiece() { Name = "Solar 2", ExpectedVoltage = 7500, EnergyValue = 5000, GamePieceType = GamePieceTypes.Source},
                    };

                    gameboard.AddPosition("3_Solar 2", "test", 0, gamepieces);

                    gamepieces.Clear();
                    gamepieces = new List<GamePiece>()
                    {
                       new GamePiece() { Name = "westpunt", ExpectedVoltage = 2100, EnergyValue = 10000, GamePieceType = GamePieceTypes.Consumer },
                       new GamePiece() { Name = "westpunt", ExpectedVoltage = 1654, EnergyValue = 7500, GamePieceType = GamePieceTypes.Consumer},
                       new GamePiece() { Name = "westpunt", ExpectedVoltage = 700, EnergyValue = 5000, GamePieceType = GamePieceTypes.Consumer},
                    };

                    gameboard.AddPosition("1-westpunt", "test", 12, gamepieces);


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
