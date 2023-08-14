namespace LoadShedder.Models
{
    public class Player
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Score { get; set; } = 0;
    }
}
