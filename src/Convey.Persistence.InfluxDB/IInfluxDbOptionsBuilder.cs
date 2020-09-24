namespace Convey.Persistence.InfluxDB
{
    public interface IInfluxDbOptionsBuilder
    {
        IInfluxDbOptionsBuilder WithUrl(string url);
        IInfluxDbOptionsBuilder WithUsername(string username);
        IInfluxDbOptionsBuilder WithPassword(string password);
        IInfluxDbOptionsBuilder WithDatabase(string database);
        IInfluxDbOptionsBuilder WithSeed(bool seed);
        InfluxDbOptions Build();
    }
}