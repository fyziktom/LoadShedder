using LoadShedder.Common;
using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;

namespace LoadShedder.Models
{
    public class Channel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Nickname of the Channel
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Device Id which own this channel
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// Position ID
        /// </summary>
        public string PositionId { get; set; } = string.Empty;
        /// <summary>
        /// Number of the channel as the element of the array from the device
        /// The default (not input set) is -1
        /// </summary>
        public int ChannelInputNumber { get; set; } = -1;

        /// <summary>
        /// Raw data from the module. These are values captured by ADC in the milivolts
        /// </summary>
        public int ActualValue { get; set; } = 0;
        /// <summary>
        /// Channel old data
        /// </summary>
        [JsonIgnore]
        public Dictionary<DateTime, int> ChannelHistory { get; set; } = new Dictionary<DateTime, int>();

        /// <summary>
        /// Clear all channel data history
        /// </summary>
        public void ClearChannelHistory()
        {
            ChannelHistory.Clear();
        }

        /// <summary>
        /// Add new value to the history and refresh value in the ActualValue
        /// </summary>
        /// <param name="value"></param>
        public void AddNewValue(int value)
        {
            ActualValue = value;

            if (value > 0)
            {
                if (ChannelHistory.Count > MainDataContext.MaximumHistoryStepsInRAM)
                {
                    var oldest = ChannelHistory.Keys.Order().FirstOrDefault();
                    if (ChannelHistory.ContainsKey(oldest))
                        ChannelHistory.Remove(oldest);
                }

                ChannelHistory.Add(DateTime.UtcNow, value);
            }
        }
    }
}
