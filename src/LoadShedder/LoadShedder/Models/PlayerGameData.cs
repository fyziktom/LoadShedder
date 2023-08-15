namespace LoadShedder.Models
{
    public class PlayerBilanceData
    {
        public double BilanceSources { get; set; } = 0.0;
        public double BilanceConsumers { get; set; } = 0.0;
        public double Bilance { get; set; } = 0.0;
    }

    public enum GameTimePenalty
    {
        NONE = 0,
        TEN_SECONDS = 10,
        FIFTHTEEN_SECONDS = 15
    }

    public class PlayerGameData : GameTime
    {
        public string PlayerId { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public GamePlayStage ActualGamePlayStage { get; set; } = GamePlayStage.None;
        public GameResponseActions ActualGameResponseAction { get; set; } = GameResponseActions.None;

        private GameTimePenalty actualGameTimePenalty = GameTimePenalty.NONE;
        /// <summary>
        /// If the player cause some faul they can get the penalty
        /// </summary>
        public GameTimePenalty ActualGameTimePenalty
        {
            get
            {
                return actualGameTimePenalty;
            }
            set
            {
                actualGameTimePenalty = value;
                if (value != GameTimePenalty.NONE)
                    GamePenaltyStartTime = DateTime.UtcNow;
                else
                    GamePenaltyStartTime = DateTime.MaxValue;
            }
        }
        /// <summary>
        /// If the penalty is 
        /// </summary>
        public DateTime GamePenaltyStartTime { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// History of the bilance data of the player
        /// </summary>
        public Dictionary<DateTime, PlayerBilanceData> BilanceDataHistory { get; set; } = new Dictionary<DateTime, PlayerBilanceData>();
        /// <summary>
        /// When the stage of the game is changed for the player it is recorded here
        /// </summary>
        public Dictionary<DateTime, GamePlayStage> GamePlayStagesHistory { get; set; } = new Dictionary<DateTime, GamePlayStage>();
    
        /// <summary>
        /// Set new stage of the game for this player
        /// </summary>
        /// <param name="newStage"></param>
        public void ChangePlayStage(GamePlayStage newStage)
        {
            ActualGamePlayStage = newStage;
            GamePlayStagesHistory.TryAdd(DateTime.UtcNow, newStage);
        }
        /// <summary>
        /// Load new bilance data to the history of the player game data
        /// </summary>
        /// <param name="bilanceSources"></param>
        /// <param name="bilanceConsumers"></param>
        /// <param name="bilance"></param>
        public void LoadNewBilances(double bilanceSources, double bilanceConsumers, double bilance)
        {
            BilanceDataHistory.TryAdd(DateTime.UtcNow, new PlayerBilanceData()
            {
                BilanceSources = bilanceSources,
                BilanceConsumers = bilanceConsumers,
                Bilance = bilance,
            });
        }
        /// <summary>
        /// Clear the data history
        /// </summary>
        public void ClearHistories()
        {
            BilanceDataHistory.Clear();
            GamePlayStagesHistory.Clear();
        }
    }
}
