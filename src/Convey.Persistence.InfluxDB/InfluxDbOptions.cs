namespace Convey.Persistence.InfluxDB
{
    public class InfluxDbOptions
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Organization { get; set; }
        public bool Seed { get; set; }
    }
}