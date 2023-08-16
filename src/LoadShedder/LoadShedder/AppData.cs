using LoadShedder.Models;
using System.Collections.Concurrent;

namespace LoadShedder
{
    public class AppData
    {
        public string ActualRunningGameId { get; set; } = "testGame";
        public string ActualRunningGameBoardId { get; set; } = "testBoard";
        public string ActualRunningDeviceId { get; set; } = "test";
        public string ActualRunningPlayerId { get; set; } = "fyziktom";
        public string ActualRunningPlayerName { get; set; } = "fyziktom";
        public ConcurrentDictionary<string, GameResponseActionEventArgs> GameResponseDataHistory { get; set; } = new ConcurrentDictionary<string, GameResponseActionEventArgs>();

    }
}
