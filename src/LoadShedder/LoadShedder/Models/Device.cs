using LoadShedder.Common;
using Newtonsoft.Json;
using System.Collections.Concurrent;
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
        [JsonIgnore]
        public Dictionary<DateTime, int[]> DeviceRawDataHistory { get; set; } = new Dictionary<DateTime, int[]>();
        /// <summary>
        /// Input Channels on the device
        /// </summary>
        public ConcurrentDictionary<int, Channel> Channels { get; set; } = new ConcurrentDictionary<int, Channel>();
        /// <summary>
        /// Log all new received data to the file. It is identified by the ID of the device and placed to separated folder in the root folder/Devices of the app
        /// </summary>
        public bool StoreTheData { get; set; } = false;

        /// <summary>
        /// Iterate the channels and if it has value in data array add new value to the channel
        /// </summary>
        /// <param name="data"></param>
        public void LoadDataToChannels(int[]? data)
        {
            if (data == null || data.Length == 0)
                return;

            foreach (var channel in Channels)
            {
                if (channel.Key < data.Length)
                    channel.Value.AddNewValue(data[channel.Key]);
            }
        }

        public bool LoadNewRawData(int[]? data)
        {
            if (data == null || data.Length == 0)
                return false;
            
            // Add to the main object
            RawData = data;

            // update data in channels 
            LoadDataToChannels(data);

            var obj = new
            {
                time = DateTime.UtcNow.ToString(),
                data = data
            };

            if (StoreTheData)
            {

                FileHelpers.CheckOrCreateTheFolder(AppDomain.CurrentDomain.BaseDirectory, "Devices");
                FileHelpers.CheckOrCreateTheFolder(AppDomain.CurrentDomain.BaseDirectory, $"Devices/Device-{Id}");
                FileHelpers.WriteTextToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Devices/Device-{Id}/Device-{Id}-data-{DateTime.UtcNow.ToString("yyyy-MM-dd_hh-mm-ss-ff")}.json"),
                                                                JsonConvert.SerializeObject(obj, Formatting.Indented));
            }

            if (DeviceRawDataHistory.Count > MainDataContext.MaximumHistoryStepsInRAM)
            {
                var oldest = DeviceRawDataHistory.Keys.Order().FirstOrDefault();
                if (DeviceRawDataHistory.ContainsKey(oldest))
                    DeviceRawDataHistory.Remove(oldest);
            }
            DeviceRawDataHistory.TryAdd(DateTime.UtcNow, data);

            return true;
        }


        public IEnumerable<int> GetChannelDataHistory(int channelNumber)
        {
            if (channelNumber < RawData.Length)
            {
                foreach (var step in DeviceRawDataHistory.Values)
                    yield return step[channelNumber];
            }
        }

        /// <summary>
        /// Clear all device raw data history
        /// </summary>
        public void ClearDeviceDataHistory()
        {
            DeviceRawDataHistory.Clear();
        }
        
        /// <summary>
        /// Add channel to the device
        /// </summary>
        /// <param name="channelNumber"></param>
        /// <param name="channelName"></param>
        /// <param name="positionId"></param>
        public void AddChannel(int channelNumber, string channelName, string positionId = null)
        {
            if (!string.IsNullOrEmpty(channelName))
            {
                if (!Channels.ContainsKey(channelNumber))
                {
                    Channels.TryAdd(channelNumber, new Channel()
                    {
                        ChannelInputNumber = channelNumber,
                        DeviceId = Id,
                        Name = channelName,
                        PositionId = positionId
                    });
                }
            }
        }
        /// <summary>
        /// Add position ID to the channel on the device
        /// </summary>
        /// <param name="channelNumber"></param>
        /// <param name="positionId"></param>
        public void AddPositionIDToChannel(int channelNumber, string positionId)
        {
            if (string.IsNullOrEmpty(positionId))
                return;

            if (channelNumber < 0)
                return;
            if (!Channels.TryGetValue(channelNumber, out var channel))
                channel.PositionId = positionId;
        }

        /// <summary>
        /// Remove channel from the device
        /// </summary>
        /// <param name="channelNumber"></param>
        /// <returns></returns>
        public bool RemoveChannel(int channelNumber)
        {
            if (channelNumber >= 0)
                return Channels.TryRemove(channelNumber, out var ch);
            else
                return false;

        }
    }
}
