using LoadShedder.Common;
using Newtonsoft.Json.Linq;

namespace LoadShedder.Models
{
    public class Position
    {
        public Position() { }
        /*
        public Position(string id)
        {
            if (string.IsNullOrEmpty(id))
                Id = Guid.NewGuid().ToString();
            else
                Id = id;
        }*/
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Nickname of the device
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Device Id
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// Channel on the device input
        /// </summary>
        public string ChannelId { get; set; } = string.Empty;
        /// <summary>
        /// Gameboard Id which unnder this position belongs
        /// </summary>
        public string GameBoardId { get; set; } = string.Empty;
        /// <summary>
        /// Channel number as element in Device RawData array
        /// </summary>
        public int ChannelInputNumber { get; set; } = 0;
        /// <summary>
        /// Actual placed Game piece on position
        /// </summary>
        public GamePiece? ActualPlacedGamePiece { get; set; } = null;
        /// <summary>
        /// All allowed gamepieces on the position
        /// </summary>
        public Dictionary<string, GamePiece> AllowedGamePieces { get; set; } = new Dictionary<string, GamePiece>();

        public void AddGamePiece(GamePiece gm)
        {
            if (gm != null)
            {
                if (!AllowedGamePieces.ContainsKey(gm.ExpectedVoltage.ToString()))
                    AllowedGamePieces.Add(gm.ExpectedVoltage.ToString(), gm);
            }
        }

        public void RemoveGamePiece(double voltage)
        {
            if (voltage > 0.0 + MainDataContext.ADCVoltageTolerance)
            {
                if (AllowedGamePieces.ContainsKey(voltage.ToString()))
                    AllowedGamePieces.Remove(voltage.ToString());
            }
        }

        /// <summary>
        /// Check if the specific piece by the voltage (which is used as dictionary) is allowed on this position
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns></returns>
        public GamePiece? IsGamePieceAllowed(int voltage)
        {
            if (voltage > 0.0 + MainDataContext.ADCVoltageTolerance)
            {
                var piece = AllowedGamePieces.Values.FirstOrDefault(p => p.IsVoltageMatch(voltage));
                if (piece != null)
                    return piece;

                //if (AllowedGamePieces.TryGetValue(voltage.ToString(), out var gm))
                    //return gm;
            }

            return null;
        }

        public GamePiece? FindMatchGamePiece(int value)
        {
            foreach (var gm in AllowedGamePieces.Values)
                if (gm.IsVoltageMatch(value))
                    return gm;

            return null;
        }

        public bool TryToPlacePiece(int voltage)
        {
            if (voltage > 0.0  + MainDataContext.ADCVoltageTolerance)
            {
                //if (IsGamePieceAllowed(voltage) != null)
                {
                    var gm = FindMatchGamePiece(voltage);

                    if (gm != null)
                    {
                        var piece = new GamePiece()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = gm.Name,
                            Description = gm.Description,
                            EnergyValue = gm.EnergyValue,
                            GamePieceType = gm.GamePieceType,
                            ExpectedVoltage = gm.ExpectedVoltage,
                            ResistorsCombo = ResistorsCombos.None
                        };

                        ActualPlacedGamePiece = piece;

                        return true;
                    }
                }
            }
            else if (voltage <= 0.0 + MainDataContext.ADCVoltageTolerance)
            {
                // for the case that position is empty
                RemoveGamePiece(voltage);
                ActualPlacedGamePiece = null;
                return true;
            }

            return false;
        }
    }
}
