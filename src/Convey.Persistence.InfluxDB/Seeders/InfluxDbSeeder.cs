using System.Threading.Tasks;
using InfluxDB.Client;

namespace Convey.Persistence.InfluxDB.Seeders
{
    public class InfluxDbSeeder : IInfluxDbSeeder
    {
        public async Task SeedAsync(InfluxDBClient client, InfluxDbOptions options)
        {
            await CustomSeedAsync(client, options);
        }

        protected virtual async Task CustomSeedAsync(InfluxDBClient client, InfluxDbOptions options)
        {
            await Task.CompletedTask;
        }
    }
}