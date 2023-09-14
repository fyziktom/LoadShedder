namespace LoadShedder.Models.Dto
{
    public class GameBoardDto
    {
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
    }
}
