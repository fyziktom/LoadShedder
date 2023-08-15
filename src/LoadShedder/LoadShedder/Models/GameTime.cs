namespace LoadShedder.Models
{
    public class GameTime
    {
        /// <summary>
        /// Start time of the game
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// End time if the game has already ended. Otherwise there is the max value
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.MaxValue;
        /// <summary>
        /// Elapsed time of the game
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get => EndTime == DateTime.MaxValue ? DateTime.UtcNow - StartTime : EndTime - StartTime;
        }
    }
}
