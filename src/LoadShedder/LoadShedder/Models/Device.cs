using LoadShedder.Common;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using VEDriversLite.Common;

namespace LoadShedder.Models
{
    public class Device
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Nickname of the device
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Raw data from the module. These are values captured by ADC in the milivolts
        /// </summary>
        public int[]? RawData { get; set; } = null;
        /// <summary>
        /// Device Raw data history. 
        /// Based on the datetime in the game you can search through the history of the data of specific device.
        /// </summary>
        public Dictionary<DateTime, int[]> DeviceRawDataHistory { get; set; } = new Dictionary<DateTime, int[]>();

        /// <summary>
        /// List of actual placed GamePieces
        /// </summary>
        public List<GamePiece> GamePieces { get; set; } = new List<GamePiece>();

        public bool LoadNewRawData(int[]? data)
        {
            if (data == null || data.Length == 0)
                return false;
            
            // Add to the main object
            RawData = data;

            var obj = new
            {
                time = DateTime.UtcNow.ToString(),
                data = data
            };

            FileHelpers.CheckOrCreateTheFolder(AppDomain.CurrentDomain.BaseDirectory, "Devices");
            FileHelpers.CheckOrCreateTheFolder(AppDomain.CurrentDomain.BaseDirectory, $"Devices/Device-{Id}");
            FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Devices/Device-{Id}/Device-{Id}-data-{DateTime.UtcNow.ToString("yyyy-MM-dd_hh-mm-ss-ff")}.json"),
                                                            JsonConvert.SerializeObject(obj, Formatting.Indented));

            //DeviceRawDataHistory.TryAdd(DateTime.UtcNow, data);

            GamePieces.Clear();

            foreach(var value in data)
            {
                if (value > 0)
                {
                    var pieceType = GamePiece.GetGamePieceTypeBasedOnVoltage(value);
                    if (MainDataContext.GamePieces.TryGetValue(pieceType, out var sourcePiece))
                    {
                        var piece = new GamePiece()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = sourcePiece.Name,
                            Description = sourcePiece.Description,
                            EnergyValue = sourcePiece.EnergyValue,
                            GamePieceType = sourcePiece.GamePieceType,
                            ResistorsCombo = pieceType
                        };

                        GamePieces.Add(piece);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Clear all device raw data history
        /// </summary>
        public void ClearDeviceDataHistory()
        {
            DeviceRawDataHistory.Clear();
        }
    }
}
