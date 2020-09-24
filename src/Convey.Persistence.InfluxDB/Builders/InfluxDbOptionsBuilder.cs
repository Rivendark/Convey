namespace Convey.Persistence.InfluxDB.Builders
{
    internal sealed class InfluxDbOptionsBuilder : IInfluxDbOptionsBuilder
    {
        private readonly InfluxDbOptions _options = new InfluxDbOptions();

        public IInfluxDbOptionsBuilder WithUrl(string url)
        {
            _options.Url = url;
            return this;
        }

        public IInfluxDbOptionsBuilder WithUsername(string username)
        {
            _options.Username = username;
            return this;
        }

        public IInfluxDbOptionsBuilder WithPassword(string password)
        {
            _options.Password = password;
            return this;
        }

        public IInfluxDbOptionsBuilder WithDatabase(string database)
        {
            _options.Database = database;
            return this;
        }

        public IInfluxDbOptionsBuilder WithSeed(bool seed)
        {
            _options.Seed = seed;
            return this;
        }

        public InfluxDbOptions Build()
            => _options;
    }
}