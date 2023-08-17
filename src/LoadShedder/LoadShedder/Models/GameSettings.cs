namespace LoadShedder.Models
{
    /// <summary>
    /// Settings of thresholds and other constant values for the game. 
    /// All energy values are in kW
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// First step in the game is loading of the sources up to the specified value in kW which is define here
        /// </summary>
        public double LOADING_SOURCES_GOAL_TO_REACH { get; set; } = 75000;

        /// <summary>
        /// During first step they should not connect more consumers than this limit
        /// </summary>
        public double LOADING_SOURCES_MAXIMUM_CONSUMERS { get; set; } = 0.0;

        /// <summary>
        /// During next step they need to add consumers to reach less than this limit
        /// </summary>
        public double LOADING_CONSUMERS_MAXIMUM_BILANCE_GOAL_TO_REACH { get; set; } = 10000;
        /// <summary>
        /// This is lower value of bilance they need to reach to move to the next level.
        /// In this default case they need to plug consumers to be between 0 and 10MW.
        /// </summary>
        public double LOADING_CONSUMERS_MINIMUM_BILANCE_GOAL_TO_REACH { get; set; } = 0;
        /// <summary>
        /// If they will go below this value it will cause the blackout
        /// </summary>
        public double LOADING_CONSUMERS_BLACKOUT_THRESHOLD { get; set; } = 0;
        /// <summary>
        /// In final round they need to balance grid to 0. They should not cause overproduction or overconsumption
        /// </summary>
        public double BALANCING_OVERPRODUCTION_BLACKOUT_THRESHOLD { get; set; } = 15000;
        /// <summary>
        /// In final round they need to balance grid to 0. They should not cause overproduction or overconsumption
        /// </summary>
        public double BALANCING_OVERCONSUMPTION_BLACKOUT_THRESHOLD { get; set; } = -5000;
    }
}
