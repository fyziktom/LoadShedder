using LoadShedder.Common;
using LoadShedder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;
using VEDriversLite.Common;

namespace LoadShedder.Controllers
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // We'd normally just use "*" for the allow-origin header, 
            // but Chrome (and perhaps others) won't allow you to use authentication if
            // the header is set to "*".
            // TODO: Check elsewhere to see if the origin is actually on the list of trusted domains.
            var ctx = filterContext.HttpContext;
            //var origin = ctx.Request.Headers["Origin"];
            //var allowOrigin = !string.IsNullOrWhiteSpace(origin) ? origin : "*";
            ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            ctx.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            ctx.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            base.OnActionExecuting(filterContext);
        }
    }

    [Route("api")]
    [ApiController]
    public class HomeController : Controller
    {

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetCombosResistanceValues")]
        public Dictionary<string,double> GetCombosResistanceValues()
        {
            Dictionary<string,double> result = new Dictionary<string, double>();
            foreach (ResistorsCombos combo in Enum.GetValues(typeof(ResistorsCombos)))
            {
                var resistorsFromCombo = GamePiece.DecomposeComboIntoResistors(combo);
                double totalResistance = 0;

                foreach (var resistor in resistorsFromCombo)
                {
                    totalResistance += (int)resistor;
                }

                result.TryAdd(Enum.GetName(typeof(ResistorsCombos), combo) ?? "UNKNOWN", totalResistance);
            }
            return result;
        }
        
        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGamePieces")]
        public Dictionary<string, GamePiece> GetGamePieces()
        {
            var gps = new Dictionary<string, GamePiece>();

            foreach (var b in MainDataContext.GameBoards)
                foreach (var p in b.Value.Positions)
                    foreach (var g in p.Value.AllowedGamePieces)
                        gps.TryAdd(g.Key, g.Value);

            return gps;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGamePiecesResistorVoltages")]
        public Dictionary<string,double> GetGamePiecesResistorVoltage()
        {
            var result = new Dictionary<string, double>();
            foreach(var piece in MainDataContext.GamePieces)
                result.TryAdd(Enum.GetName(typeof(ResistorsCombos), piece.Value.ResistorsCombo) ?? piece.Value.Id, 
                    piece.Value.GetGamePieceResistorVoltage());
            
            return result;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGameBoards")]
        public IDictionary<string, GameBoard> GetGameBoards()
        {
            return MainDataContext.GameBoards;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetDevices")]
        public IDictionary<string, Device> GetDevices()
        {
            return MainDataContext.Devices;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGames")]
        public IDictionary<string, Game> GetGames()
        {
            return MainDataContext.Games;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetPlayers")]
        public IDictionary<string, Player> GetPlayers()
        {
            return MainDataContext.Players;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGame/{id}")]
        public Game GetGame(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return new Game();

                if (MainDataContext.Games.TryGetValue(id, out var game))
                    return game;
                else
                    return new Game();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get game id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetPlayer/{id}")]
        public Player GetPlayer(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return new Player();

                if (MainDataContext.Players.TryGetValue(id, out var player))
                    return player;
                else
                    return new Player();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get player id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGameBoard/{id}")]
        public GameBoard GetGameBoard(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return new GameBoard();

                if (MainDataContext.GameBoards.TryGetValue(id, out var board))
                    return board;
                else
                    return new GameBoard();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get board id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetDevice/{id}")]
        public Device GetDevice(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return new Device();

                if (MainDataContext.Devices.TryGetValue(id, out var device))
                    return device;
                else
                    return new Device();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get device id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetDeviceDataRaw/{id}")]
        public string GetDeviceDataRaw(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return string.Empty;

                if (MainDataContext.Devices.TryGetValue(id, out var device))
                    return JsonConvert.SerializeObject(device.RawData);
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get raw data for device id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGameBoardGamePieces/{id}")]
        public List<GamePiece> GetGameBoardGamePieces(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return new List<GamePiece>();

                if (MainDataContext.GameBoards.TryGetValue(id, out var gb))
                {
                    var gps = new List<GamePiece>();
                    foreach (var pos in gb.Positions)
                        foreach (var g in pos.Value.AllowedGamePieces)
                            gps.Add(g.Value);

                    return gps;
                }
                else
                    return new List<GamePiece>();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get data for device id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGameBoardPositions/{id}")]
        public Dictionary<string,Position> GetGameBoardPositions(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return new Dictionary<string, Position>();

                if (MainDataContext.GameBoards.TryGetValue(id, out var gb))
                    return gb.Positions;
                else
                    return new Dictionary<string, Position>();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get data for device id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetBoardStatus/{id}")]
        public GameBoard? GetBoardStatus(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;

                if (MainDataContext.GameBoards.TryGetValue(id, out var board))
                    return board;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get board status of board id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetBoardBilance/{id}")]
        public double GetBoardBilance(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return 0.0;
                
                if (MainDataContext.GameBoards.TryGetValue(id, out var board))
                    return board.GetActualBilance();              
                else
                    return 0.0;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get board bilance of board id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetBoardSourcesBilance/{id}")]
        public double GetBoardSourcesBilance(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return 0.0;

                if (MainDataContext.GameBoards.TryGetValue(id, out var board))
                    return board.GetActualBilanceForSources();
                else
                    return 0.0;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get board sources bilance of board id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetBoardConsumersBilance/{id}")]
        public double GetBoardConsumersBilance(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return 0.0;

                if (MainDataContext.GameBoards.TryGetValue(id, out var board))
                    return board.GetActualBilanceForConsumers();
                else
                    return 0.0;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get board bilance of board id: {id}!");
            }
        }

        public class NewDeviceDataRequest
        {
            public string id { get; set; } = string.Empty;
            public int[]? data { get; set; } = null;
        }

        /// <summary>
        /// Post of the new device data
        /// </summary>
        /// <returns>OK if accepted</returns>
        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("NewDeviceData")]
        public async Task<string> NewDeviceData([FromBody] NewDeviceDataRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.id))
                    return "ERROR_NO_ID";

                if (data.data == null)
                    return "ERROR_NO_DATA";

                if (MainDataContext.Devices.TryGetValue(data.id, out var device))
                {
                    if (device.LoadNewRawData(data.data))
                    {
                        var board = MainDataContext.GameBoards.Values.FirstOrDefault(b => b.DeviceId == data.id);
                        if (board != null)
                            board.RefreshBoardStatusFromDeviceData();
                        
                        return "OK";
                    }
                    return "ERROR_INVALID_DATA";
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Accept the device data.! Exception: " + ex.Message);
            }

            return "ERROR_UNKNOWN";
        }


        public class AddPlayerRequest
        {
            public string name { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
            public string boardId { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("AddPlayer")]
        public async Task<string> AddPlayer([FromBody] AddPlayerRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.name))
                    return "ERROR_NO_NAME";

                var player = new Player() { Id = Guid.NewGuid().ToString(), Name = data.name, Description = data.description };

                if (!MainDataContext.Players.TryAdd(player.Id, player))
                {
                    return "ERROR_CANNOT_ADD_PLAYER";
                }

                if (!string.IsNullOrEmpty(data.boardId))
                {
                    if (MainDataContext.GameBoards.TryGetValue(data.boardId, out var board))
                        board.PlayerId = player.Id;
                }

                return player.Id;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add player! Exception: " + ex.Message);
            }
        }

        public class AddGameBoardRequest
        {
            public string name { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
            public string playerId { get; set; } = string.Empty;
            public string deviceId { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("AddGameBoard")]
        public async Task<string> AddGameBoard([FromBody] AddGameBoardRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.name))
                    return "ERROR_NO_NAME";

                var gameboard = new GameBoard()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = data.name,
                    PlayerId = data.playerId,
                    DeviceId = data.deviceId,
                };

                if (!MainDataContext.GameBoards.TryAdd(gameboard.Id, gameboard))
                {
                    return "ERROR_CANNOT_ADD_GAMEBOARD";
                }

                return gameboard.Id;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add GameBoard! Exception: " + ex.Message);
            }
        }

        public class AddPositionRequest
        {
            public string id { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string gameboardId { get; set; } = string.Empty;
            public string deviceId { get; set; } = string.Empty;
            public string channelId { get; set; } = string.Empty;
            public int channelInputNumber { get; set; } = -1;
            public List<GamePiece> gamePieces { get; set; } = new List<GamePiece>();
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("AddPosition")]
        public async Task<string> AddPosition([FromBody] AddPositionRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.name))
                    return "ERROR_NO_NAME";
                if (string.IsNullOrEmpty(data.deviceId))
                    return "ERROR_NO_DEVICE_ID";
                if (string.IsNullOrEmpty(data.gameboardId))
                    return "ERROR_NO_GAMEBOARD_ID";
                if (string.IsNullOrEmpty(data.channelId))
                    return "ERROR_NO_CHANNEL_ID";
                if (data.channelInputNumber < 0)
                    return "ERROR_NO_CHANNEL_INPUT_NUMBER";

                if (MainDataContext.GameBoards.TryGetValue(data.gameboardId, out var gameboard))
                { 
                    var res = gameboard.AddPosition(data.id, data.name, data.deviceId, data.channelId, data.channelInputNumber, data.gamePieces);
                    return res;
                }
                else
                {
                    throw new HttpResponseException((HttpStatusCode)501, $"Cannot Find GameBoard!");
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add GameBoard! Exception: " + ex.Message);
            }
        }


        public class RemovePositionRequest
        {
            public string id { get; set; } = string.Empty;
            public string gameboardId { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("RemovePosition")]
        public async Task<string> RemovePosition([FromBody] RemovePositionRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.id))
                    return "ERROR_NO_ID";
                if (string.IsNullOrEmpty(data.gameboardId))
                    return "ERROR_NO_GAMEBOARD_ID";

                if (MainDataContext.GameBoards.TryGetValue(data.gameboardId, out var gameboard))
                    gameboard.RemovePosition(data.id);
                else
                    throw new HttpResponseException((HttpStatusCode)501, $"Cannot Find GameBoard!");

                return "OK";
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot remove position from gameboard GameBoard! Exception: " + ex.Message);
            }
        }

        public class TestMatchOfGamePieceInPositionRequest 
        { 
            public string gameboardId { get; set; } = string.Empty;
            public string positionId { get; set; } = string.Empty;
            public int measuredVoltage { get; set; } = -1;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("TestMatchOfGamePieceInPosition")]
        public async Task<GamePiece> TestMatchOfGamePieceInPosition([FromBody] TestMatchOfGamePieceInPositionRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.gameboardId))
                    throw new HttpResponseException((HttpStatusCode)501, $"ERROR_NO_GAMEBOARD_ID");
                if (string.IsNullOrEmpty(data.positionId))
                    throw new HttpResponseException((HttpStatusCode)501, $"ERROR_NO_POSITION_ID");
                if (data.measuredVoltage < 0)
                    throw new HttpResponseException((HttpStatusCode)501, $"ERROR_IN_MEASURED_VALUE");

                if (MainDataContext.GameBoards.TryGetValue(data.gameboardId, out var gameboard))
                {
                    if (gameboard.Positions.TryGetValue(data.positionId, out var position))
                    {
                        var gp = position.FindMatchGamePiece(data.measuredVoltage);
                        if (gp != null)
                            return gp;
                        else
                            return new GamePiece();
                    }
                    else
                    {
                        throw new HttpResponseException((HttpStatusCode)501, $"Cannot Find Position in GameBoard, Add position first!");
                    }

                    throw new HttpResponseException((HttpStatusCode)501, $"ERROR!");
                }
                else
                {
                    throw new HttpResponseException((HttpStatusCode)501, $"Cannot Find GameBoard!");
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add GameBoard! Exception: " + ex.Message);
            }
        }

        public class AddGamePieceToPositionRequest
        {
            public string name { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
            public string gameboardId { get; set; } = string.Empty;
            public string positionId { get; set; } = string.Empty;
            public int expectedVoltage { get; set; } = -1;
            public int energyvalue { get; set; } = -1;
            public int typeOfGamePiece { get; set; } = 0;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("AddGamePieceToPosition")]
        public async Task<string> AddGamePieceToPosition([FromBody] AddGamePieceToPositionRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.name))
                    return "ERROR_NO_NAME";
                if (string.IsNullOrEmpty(data.gameboardId))
                    return "ERROR_NO_GAMEBOARD_ID";
                if (string.IsNullOrEmpty(data.positionId))
                    return "ERROR_NO_POSITION_ID";
                if (data.energyvalue < 0)
                    return "ERROR_NO_ENERGY_VALUE";
                if (data.expectedVoltage < 0)
                    return "ERROR_NO_EXPECTED_VOLTAGE";

                if (MainDataContext.GameBoards.TryGetValue(data.gameboardId, out var gameboard))
                {
                    if (gameboard.Positions.TryGetValue(data.positionId, out var position))
                    {
                        if (position.IsGamePieceAllowed(data.expectedVoltage) == null)
                        {
                            var gamepiece = new GamePiece()
                            {
                                Id = data.expectedVoltage.ToString(),
                                Name = data.name,
                                Description = data.description,
                                EnergyValue = data.energyvalue,
                                ExpectedVoltage = data.expectedVoltage,
                                GamePieceType = data.typeOfGamePiece == 0 ? GamePieceTypes.Source : GamePieceTypes.Consumer
                            };

                            position.AddGamePiece(gamepiece);

                            return gamepiece.Id;
                        }
                    }
                    else
                    {
                        throw new HttpResponseException((HttpStatusCode)501, $"Cannot Find Position in GameBoard, Add position first!");
                    }

                    throw new HttpResponseException((HttpStatusCode)501, $"ERROR!");
                }
                else
                {
                    throw new HttpResponseException((HttpStatusCode)501, $"Cannot Find GameBoard!");
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add GameBoard! Exception: " + ex.Message);
            }
        }

        public class AddDeviceRequest
        {
            public string name { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("AddDevice")]
        public async Task<string> AddDevice([FromBody] AddDeviceRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.name))
                    return "ERROR_NO_NAME";

                var device = new Device()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = data.name,
                };

                if (!MainDataContext.Devices.TryAdd(device.Id, device))
                {
                    return "ERROR_CANNOT_ADD_DEVICE";
                }

                return device.Id;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add Devices! Exception: " + ex.Message);
            }
        }

        public class AddGameRequest
        {
            public string name { get; set; } = string.Empty;
            public List<string> gameboards { get; set; } = new List<string>();
            public GameType gametype { get; set; } = GameType.SINGLEPLAYER_TIME_SCORE;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("AddGame")]
        public async Task<string> AddGame([FromBody] AddGameRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.name))
                    return "ERROR_NO_NAME";

                var game = new Game()
                {
                    Id = Guid.NewGuid().ToString(),
                    GameBoardIds = data.gameboards,
                    GameType = data.gametype
                };

                if (!MainDataContext.Games.TryAdd(game.Id, game))
                {
                    return "ERROR_CANNOT_ADD_DEVICE";
                }

                return game.Id;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add Game! Exception: " + ex.Message);
            }
        }

        public class AddDeviceToGameboardRequest
        {
            public string gameboardid { get; set; } = string.Empty;
            public string deviceid { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("AddDeviceToGameboard")]
        public async Task<string> AddDeviceToGameboard([FromBody] AddDeviceToGameboardRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.gameboardid) || string.IsNullOrEmpty(data.deviceid))
                    return "ERROR_NO_ID";


                if (MainDataContext.GameBoards.TryGetValue(data.gameboardid, out var gameboard))
                {
                    if (MainDataContext.Devices.TryGetValue(data.deviceid, out var device))
                    {
                        gameboard.DeviceId = device.Id;
                        return "OK";
                    }
                    else
                        return "ERROR_CANNOT_FIND_DEVICE";
                }
                else
                    return "ERROR_CANNOT_FIND_GAMEBOARD";
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add Device to the Gameboard! Exception: " + ex.Message);
            }

            return "ERROR";
        }

        public class RemoveDeviceFromGameboardRequest
        {
            public string gameboardid { get; set; } = string.Empty;
            public string deviceid { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("RemoveDeviceFromGameboard")]
        public async Task<string> RemoveDeviceFromGameboard([FromBody] RemoveDeviceFromGameboardRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.gameboardid) || string.IsNullOrEmpty(data.deviceid))
                    return "ERROR_NO_ID";


                if (MainDataContext.GameBoards.TryGetValue(data.gameboardid, out var gameboard))
                {
                    gameboard.DeviceId = string.Empty;
                        return "OK";
                }
                else
                    return "ERROR_CANNOT_FIND_GAMEBOARD";
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Remove Device from Gameboard! Exception: " + ex.Message);
            }
        }

        public class AddGameBoardToGameRequest
        {
            public string gameid { get; set; } = string.Empty;
            public string gameboardid { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("AddGameBoardToGame")]
        public async Task<string> AddGameBoardToGame([FromBody] AddGameBoardToGameRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.gameid) || string.IsNullOrEmpty(data.gameboardid))
                    return "ERROR_NO_ID";

                
                if (MainDataContext.Games.TryGetValue(data.gameid, out var game))
                {
                    if (MainDataContext.GameBoards.TryGetValue(data.gameboardid, out var gameboard))
                    {
                        if (game.AddBoardToGame(gameboard.Id))
                            return "OK";
                        else
                            return "ERROR_ALREADY_ADDED";
                    }
                    else
                        return "ERROR_CANNOT_FIND_GAMEBOARD";
                }
                else
                    return "ERROR_CANNOT_FIND_GAME";
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add Gameboad to the Game! Exception: " + ex.Message);
            }

            return "ERROR";
        }

        public class RemoveGameBoardFromGameRequest
        {
            public string gameid { get; set; } = string.Empty;
            public string gameboardid { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("RemoveGameBoardFromGame")]
        public async Task<string> RemoveGameBoardFromGame([FromBody] RemoveGameBoardFromGameRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.gameid) || string.IsNullOrEmpty(data.gameboardid))
                    return "ERROR_NO_ID";


                if (MainDataContext.Games.TryGetValue(data.gameid, out var game))
                {
                    if (game.RemoveBoardFromGame(data.gameboardid))
                        return "OK";
                    else
                        return "ERROR_ALREADY_REMOVED";
                }
                else
                    return "ERROR_CANNOT_FIND_GAME";
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Remove Gameboard from Game! Exception: " + ex.Message);
            }

            return "ERROR";
        }

        public class AddPlayerToGameBoardRequest
        {
            public string gameboardid { get; set; } = string.Empty;
            public string playerid { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("AddPlayerToGameBoard")]
        public async Task<string> AddPlayerToGameBoard([FromBody] AddPlayerToGameBoardRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.gameboardid) || string.IsNullOrEmpty(data.playerid))
                    return "ERROR_NO_ID";


                if (MainDataContext.GameBoards.TryGetValue(data.gameboardid, out var gameboard))
                {
                    if (MainDataContext.Players.TryGetValue(data.playerid, out var player))
                    {
                        gameboard.PlayerId = data.playerid;
                        return "OK";
                    }
                    else
                    {
                        return "ERROR_CANNOT_FIND_PLAYER";
                    }
                }
                else
                {
                    return "ERROR_CANNOT_FIND_GAMEBOARD";
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Add Gameboad to the Game! Exception: " + ex.Message);
            }

            return "ERROR";
        }

        public class RemovePlayerFromGameBoardRequest
        {
            public string gameboardid { get; set; } = string.Empty;
            public string playerid { get; set; } = string.Empty;
        }

        [AllowCrossSiteJsonAttribute]
        [HttpPost]
        [Route("RemovePlayerFromGameBoard")]
        public async Task<string> RemovePlayerFromGameBoard([FromBody] RemovePlayerFromGameBoardRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.gameboardid) || string.IsNullOrEmpty(data.playerid))
                    return "ERROR_NO_ID";


                if (MainDataContext.GameBoards.TryGetValue(data.gameboardid, out var gameboard))
                {
                    gameboard.PlayerId = string.Empty;
                    return "OK";
                }
                else
                {
                    return "ERROR_CANNOT_FIND_GAMEBOARD";
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot Remove Player from GameBoard! Exception: " + ex.Message);
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("RemoveGame/{id}")]
        public string RemoveGame(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return string.Empty;

                if (MainDataContext.Games.TryRemove(id, out var game))
                    return "OK";
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot remove game: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("RemoveGameBoard/{id}")]
        public string RemoveGameBoard(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return string.Empty;

                if (MainDataContext.GameBoards.TryRemove(id, out var board))
                {
                    foreach(var game in MainDataContext.Games.Values.Where(g => g.GameBoardIds.Contains(board.Id)))
                        game.RemoveBoardFromGame(board.Id);
                    
                    return "OK";
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot remove gameboard: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("RemovePlayer/{id}")]
        public string RemovePlayer(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return string.Empty;

                if (MainDataContext.Players.TryRemove(id, out var player))
                {
                    foreach (var gameboard in MainDataContext.GameBoards.Values.Where(g => g.PlayerId == player.Id))
                        gameboard.PlayerId = string.Empty;

                    return "OK";
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot remove player: {id}!");
            }
        }

        #region GamePlay

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("StartGame/{id}")]
        public string StartGame(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "ERROR_NO_ID";

                if (MainDataContext.Games.TryGetValue(id, out var game))
                {
                    game.StartGame();
                    return "OK";
                }
                else
                    return "ERROR_CANNOT_FIND_GAME";
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot start game id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("StopGame/{id}")]
        public string StopGame(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "ERROR_NO_ID";

                if (MainDataContext.Games.TryGetValue(id, out var game))
                {
                    game.EndGame();
                    return "OK";
                }
                else
                    return "ERROR_CANNOT_FIND_GAME";
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot end game id: {id}!");
            }
        }


        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGameActualRunningTime/{id}")]
        public TimeSpan GetGameActualRunningTime(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                    return TimeSpan.Zero;

                if (MainDataContext.Games.TryGetValue(id, out var game))
                    return game.ElapsedTime;
                else
                    return TimeSpan.Zero;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot end game id: {id}!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGameResponseAction/{gameId}")]
        public GameResponseActionEventArgs? GetGameResponseAction(string gameId)
        {
            try
            {
                if (MainDataContext.GameResponseActions.Count > 0)
                {
                    if (MainDataContext.GameResponseActions.TryGetValue(gameId, out var que))
                    {
                        if (que.Count > 0)
                        {
                            que.TryDequeue(out var action);
                            return action;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get Game Response Action!");
            }
        }        
        
        
        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGamePlayerDataByDeviceId/{deviceId}")]
        public PlayerGameData? GetGamePlayerDataByDeviceId(string deviceId)
        {
            try
            {
                var gb = MainDataContext.GameBoards.Values.Where(gb => gb.DeviceId == deviceId).FirstOrDefault();
                if (gb != null)
                {
                    if (!string.IsNullOrEmpty(gb.PlayerId))
                    {
                        var game = MainDataContext.Games.Values.Where(g => g.PlayersIds.Contains(gb.PlayerId)).FirstOrDefault();
                        if (game != null)
                        {

                            if (game.PlayersGameData.TryGetValue(gb.PlayerId, out var pgd))
                            {
                                return pgd;
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get Game Player data by deviceId Action!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("ActualGamePlayStageByDeviceId/{deviceId}")]
        public int BlackoutStatusByDeviceId(string deviceId)
        {
            try
            {
                var gb = MainDataContext.GameBoards.Values.Where(gb => gb.DeviceId == deviceId).FirstOrDefault();
                if (gb != null)
                {
                    if (!string.IsNullOrEmpty(gb.PlayerId))
                    {
                        var game = MainDataContext.Games.Values.Where(g => g.PlayersIds.Contains(gb.PlayerId)).FirstOrDefault();
                        if (game != null)
                        {
                            if (game.PlayersGameData.TryGetValue(gb.PlayerId, out var pgd))
                            {
                                return (int)pgd.ActualGamePlayStage;
                            }
                        }
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get Game Play Stage by deviceId Action!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GameResponseActionsByDeviceId/{deviceId}")]
        public int GameResponseActionsByDeviceId(string deviceId)
        {
            try
            {
                var gb = MainDataContext.GameBoards.Values.Where(gb => gb.DeviceId == deviceId).FirstOrDefault();
                if (gb != null)
                {
                    if (!string.IsNullOrEmpty(gb.PlayerId))
                    {
                        var game = MainDataContext.Games.Values.Where(g => g.PlayersIds.Contains(gb.PlayerId)).FirstOrDefault();
                        if (game != null)
                        {
                            if (game.PlayersGameData.TryGetValue(gb.PlayerId, out var pgd))
                            {
                                return (int)pgd.ActualGameResponseAction;
                            }
                        }
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get Game Response Action by deviceId Action!");
            }
        }

        [AllowCrossSiteJsonAttribute]
        [HttpGet]
        [Route("GetGameStoredData")]
        public Dictionary<string, GameStoredData> GetGameStoredData()
        {
            try
            {
                var response = new Dictionary<string, GameStoredData>();

                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Players");
                if (Directory.Exists(path))
                {
                    foreach(var dir in Directory.EnumerateDirectories(path))
                    {
                        foreach(var file in Directory.EnumerateFiles(dir))
                        {
                            try
                            {
                                var filecontent = FileHelpers.ReadTextFromFile(file);
                                if (!string.IsNullOrEmpty(filecontent))
                                {
                                    var gsd = JsonConvert.DeserializeObject<GameStoredData>(filecontent);
                                    if (gsd != null)
                                        response.TryAdd(gsd.GameId, gsd);
                                }
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine("Cannot get the file from game stored data. \n" + ex.Message);
                            }
                        }
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException((HttpStatusCode)501, $"Cannot get Game Stored Data!");
            }
        }

        #endregion
    }
}
