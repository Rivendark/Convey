using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

namespace Convey.Persistence.InfluxDB.Initializers
{
    internal sealed class InfluxDbInitializer : IInfluxDbInitializer
    {
        private static int _initialized;
        private readonly InfluxDbOptions _options;
        private readonly InfluxDBClient _client;
        private readonly IInfluxDbSeeder _seeder;

        public InfluxDbInitializer(InfluxDBClient client, IInfluxDbSeeder seeder, InfluxDbOptions options)
        {
            _client = client;
            _seeder = seeder;
            _options = options;
        }

        public async Task InitializeAsync()
        {
            if (Interlocked.Exchange(ref _initialized, 1) == 1)
            {
                return;
            }

            var organizationManager = _client.GetOrganizationsApi();
            var organizations = await organizationManager.FindOrganizationsAsync();
            if (!organizations.Exists(o => o.Name.Equals(_options.Organization)))
            {
                await organizationManager.CreateOrganizationAsync(_options.Organization);
            }
        }
    }
}