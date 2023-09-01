using LoadShedder.Common;
using LoadShedder.Components;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;
using VEDriversLite.EntitiesBlocks.Blocks;
using VEDriversLite.EntitiesBlocks.Consumers;
using VEDriversLite.EntitiesBlocks.Entities;
using VEDriversLite.EntitiesBlocks.Handlers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LoadShedder.Models
{
    public class GameBoard
    {
        public GameBoard(string id = "", string deviceId = "", string name = "New Board")
        {
            if (!string.IsNullOrEmpty(id))
                Id = id;
            else
                Id = Guid.NewGuid().ToString();

            DeviceId = deviceId;
            Name = name;
            start = new DateTime(2023, 8, 13);
            end = start + new TimeSpan(1, 0, 0);

            ResetBoard();
        }
        /// <summary>
        /// ID of the board
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Name of the board
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Game device ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// Player Id
        /// </summary>
        public string PlayerId { get; set; } = string.Empty;
        /// <summary>
        /// Positions on the gameboard
        /// </summary>
        public Dictionary<string, Position> Positions { get; set; } = new Dictionary<string, Position>();
        /// <summary>
        /// Energetic Grid handler
        /// </summary>
        [JsonIgnore]
        public BaseEntitiesHandler eGrid { get; set; } = new BaseEntitiesHandler();
        /// <summary>
        /// Main entity for the board in the eGrid
        /// </summary>
        [JsonIgnore]
        public IEntity Root { get; set; } = new BaseEntity();

        public event EventHandler<string> NewDataLoaded;

        [JsonIgnore]
        private DateTime start = new DateTime(2023, 8, 13);
        [JsonIgnore]
        private DateTime end = new DateTime(2023, 8, 13, 1, 0, 0);

        public void ResetBoard()
        {
            Root = new BaseEntity();
            eGrid = new BaseEntitiesHandler();
            eGrid.AddEntity(Root, "Root", "Grid");
        }

        /// <summary>
        /// This function will add block to the entity. This block is created from the game piece.
        /// It copy the direction of the current (production vs. consumption) and it has length one hour.
        /// The calculation of network bilance use the same start/end time as these blocks. 
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public bool AddGamePieceToBoard(GamePiece? piece)
        {
            if (piece == null)
                return false;

            if (string.IsNullOrEmpty(piece.Id))
                return false;
            var res = (false, "");

            if (piece.GamePieceType == GamePieceTypes.Consumer)
            {
                double[] acRun = new double[24];
                for (int i = 0; i < acRun.Length; i++)
                    acRun[i] = 1.0;

                var deviceSim = new DeviceSimulator(acRun, piece.EnergyValue);
                deviceSim.Name = piece.Name;
                res = eGrid.AddSimulatorToEntity(Root.Id, deviceSim);
            }
            else
            {
                var Block = new BaseBlock();

                IBlock block = Block.GetBlockByPower(BlockType.Simulated,
                                                     BlockDirection.Created,
                                                     start,
                                                     end - start,
                                                     piece.EnergyValue,
                                                     Root.Name,
                                                     Root.Id);

                res = eGrid.AddBlockToEntity(Root.Id, block);
            }

            if (res.Item1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Add game pieces to the board
        /// </summary>
        /// <param name="gamePieces"></param>
        /// <returns></returns>
        public bool AddGamePiecesToBoard(int[]? latestData = null)
        {
            eGrid.RemoveAllEntityBlocks(Root.Id);
            if(eGrid.Entities.TryGetValue(Root.Id, out var root))
                root.Simulators.Clear();                

            if (latestData == null && !string.IsNullOrEmpty(DeviceId))
            {
                if (MainDataContext.Devices.TryGetValue(DeviceId, out var dev))
                    latestData = dev.RawData;
                else
                    return false;
            }

            if (latestData != null)
            {
                foreach (var position in Positions)
                {
                    if (latestData.Length > position.Value.ChannelInputNumber)
                    {
                        if (position.Value.TryToPlacePiece(latestData[position.Value.ChannelInputNumber]))
                        {
                            if (position.Value.ActualPlacedGamePiece != null)
                                AddGamePieceToBoard(position.Value.ActualPlacedGamePiece);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public bool RefreshBoardStatusFromDeviceData()
        {
            if(AddGamePiecesToBoard())
            {
                NewDataLoaded?.Invoke(this, Id); 
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get Actual Bilance of the Root entity
        /// </summary>
        /// <returns></returns>
        public double GetActualBilance()
        {
            var consumption = eGrid.GetConsumptionOfEntity(Root.Id,
                                                            BlockTimeframe.Hour,
                                                            start,
                                                            start + new TimeSpan(24, 0, 0),
                                                            true,
                                                            true,
                                                            null,
                                                            null,
                                                            true);
            

            if (consumption != null)
                return consumption.First().Amount;
            
            return 0.0;
        }

        /// <summary>
        /// Get Actual Bilance of the Root entity just for the sources
        /// </summary>
        /// <returns></returns>
        public double GetActualBilanceForSources()
        {
            var consumption = eGrid.GetConsumptionOfEntity(Root.Id,
                                                            BlockTimeframe.Hour,
                                                            start,
                                                            start + new TimeSpan(24, 0, 0),
                                                            true,
                                                            true,
                                                            new List<BlockDirection>() { BlockDirection.Created },
                                                            null,
                                                            true);


            if (consumption != null)
                return consumption.First().Amount;
            
            return 0.0;
        }

        /// <summary>
        /// Get Actual Bilance of the Root entity just for the consumers
        /// </summary>
        /// <returns></returns>
        public double GetActualBilanceForConsumers()
        {
            var consumption = eGrid.GetConsumptionOfEntity(Root.Id,
                                                            BlockTimeframe.Hour,
                                                            start,
                                                            start + new TimeSpan(24, 0, 0),
                                                            true,
                                                            true,
                                                            new List<BlockDirection>() { BlockDirection.Consumed },
                                                            null,
                                                            true);


            if (consumption != null)
                return consumption.First().Amount;
            
            return 0.0;
        }

        public string AddPosition(string? id, string name, string deviceId, string? channelId, int channelNumber, List<GamePiece>? gamePieces)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(deviceId))
                return "NO_NAME_OR_DEVICE_ID";
            if (MainDataContext.Devices.TryGetValue(deviceId, out var device))
            {
                if (device.Channels.TryGetValue(channelNumber, out var channel))
                {
                    var pos = new Position()
                    {
                        Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString(),
                        ChannelId = !string.IsNullOrEmpty(channelId) ? channelId : channelNumber.ToString(),
                        Name = name,
                        ChannelInputNumber = channelNumber,
                        DeviceId = deviceId,
                        GameBoardId = Id
                    };

                    channel.PositionId = pos.Id;

                    if (gamePieces != null)
                    {
                        foreach (var piece in gamePieces)
                            pos.AllowedGamePieces.TryAdd(piece.ExpectedVoltage.ToString(), piece);
                    }

                    Positions.TryAdd(pos.Id, pos);

                    return pos.Id;
                }
            }

            return "ERROR";
        }

        public void RemovePosition(string id)
        {
            if (Positions.ContainsKey(id))
                Positions.Remove(id);
        }

    }
}
