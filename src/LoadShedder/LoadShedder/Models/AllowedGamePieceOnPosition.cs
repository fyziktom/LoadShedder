namespace LoadShedder.Models
{
    public class AllowedGamePieceOnPosition
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Nickname of the device
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Expected voltage based on the config which identify the gamepiece in the readable form
        /// </summary>
        public double ExpectedVoltage { get; set; } = 0.0;

    }
}
