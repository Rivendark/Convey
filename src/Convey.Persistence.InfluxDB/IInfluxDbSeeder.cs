using System.Threading.Tasks;
using InfluxDB.Client;

namespace Convey.Persistence.InfluxDB
{
    public interface IInfluxDbSeeder
    {
        Task SeedAsync(InfluxDBClient client, InfluxDbOptions options);
    }
}