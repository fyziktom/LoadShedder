using LoadShedder.Models;
using System.Collections.Concurrent;

namespace LoadShedder.Common
{
    public static class MainDataContext
    {
        public static string BaseAddress { get; set; } = "http://localhost:5000";
        /// <summary>
        /// Main ADC voltage supplied as reference voltage for all resistors
        /// </summary>
        public static double ADCMainVoltage { get; set; } = 5000; //5000mV
        /// <summary>
        /// Main ADC resolution
        /// </summary>
        public static double ADCResolution { get; set; } = 4096; //12-bit ADC
        /// <summary>
        /// ADC main dividing resistor. The resistor combos of the game pieces are connected in series to this resistor
        /// </summary>
        public static double ADCMainDividingResistor { get; set; } = 10000; //100kOhm
        /// <summary>
        /// ADC voltage tolerance
        /// </summary>
        public static double ADCVoltageTolerance { get; set; } = 20; //20mV
        /// <summary>
        /// Settings of the game, constants, etc. It is loaded from the appsettings.json
        /// </summary>
        public static GameSettings GameSettings { get; set; } = new GameSettings();
        /// <summary>
        /// Dictionary of all the devices
        /// </summary>
        public static ConcurrentDictionary<string, Device> Devices { get; set; } = new ConcurrentDictionary<string, Device>();
        /// <summary>
        /// Dictionary of all the games
        /// </summary>
        public static ConcurrentDictionary<string, Game> Games { get; set; } = new ConcurrentDictionary<string, Game>();
        /// <summary>
        /// Players across the games
        /// </summary>
        public static ConcurrentDictionary<string, Player> Players { get; set; } = new ConcurrentDictionary<string, Player>();
        /// <summary>
        /// Dictionary of all available GamePieces
        /// </summary>
        public static Dictionary<ResistorsCombos, GamePiece> GamePieces { get; set; } = new Dictionary<ResistorsCombos, GamePiece>();
        /// <summary>
        /// Dictionary of all gameboards.
        /// </summary>
        public static ConcurrentDictionary<string, GameBoard> GameBoards { get; set; } = new ConcurrentDictionary<string, GameBoard>();
        /// <summary>
        /// Place for storing all the game actions before the UI will dequeue them.
        /// </summary>
        public static ConcurrentDictionary<string, ConcurrentQueue<GameResponseActionEventArgs>> GameResponseActions { get; set; } = new ConcurrentDictionary<string, ConcurrentQueue<GameResponseActionEventArgs>>();
    }
}
