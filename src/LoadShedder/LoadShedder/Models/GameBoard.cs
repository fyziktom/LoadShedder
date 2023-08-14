using LoadShedder.Common;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;
using VEDriversLite.EntitiesBlocks.Blocks;
using VEDriversLite.EntitiesBlocks.Consumers;
using VEDriversLite.EntitiesBlocks.Entities;
using VEDriversLite.EntitiesBlocks.Handlers;

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
        /// Energetic Grid handler
        /// </summary>
        [JsonIgnore]
        public BaseEntitiesHandler eGrid { get; set; } = new BaseEntitiesHandler();
        /// <summary>
        /// Main entity for the board in the eGrid
        /// </summary>
        [JsonIgnore]
        public IEntity Root { get; set; } = new BaseEntity();

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
        public bool AddGamePiecesToBoard(List<GamePiece>? gamePieces = null)
        {
            eGrid.RemoveAllEntityBlocks(Root.Id);
            if(eGrid.Entities.TryGetValue(Root.Id, out var root))
                root.Simulators.Clear();                

            if (gamePieces == null && !string.IsNullOrEmpty(DeviceId))
            {
                if (MainDataContext.Devices.TryGetValue(DeviceId, out var dev))
                    gamePieces = dev.GamePieces;
                else
                    return false;
            }

            foreach(var piece in gamePieces)
            {
                if (!AddGamePieceToBoard(piece))
                    return false;
            }
            return true;
        }

        public bool RefreshBoardStatusFromDeviceData()
        {
            return AddGamePiecesToBoard();
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

    }
}
