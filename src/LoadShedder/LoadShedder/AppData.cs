using LoadShedder.Models;
using System.Collections.Concurrent;

namespace LoadShedder
{
    public class AppData
    {
        public ConcurrentDictionary<string, GameResponseActionEventArgs> GameResponseDataHistory { get; set; } = new ConcurrentDictionary<string, GameResponseActionEventArgs>();

    }
}
