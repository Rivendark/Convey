using System;
using System.Linq;
using System.Threading.Tasks;
using Convey.Persistence.InfluxDB.Buckets;
using Convey.Persistence.InfluxDB.Builders;
using Convey.Persistence.InfluxDB.Initializers;
using Convey.Types;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.Persistence.InfluxDB
{
    public static class Extensions
    {
        private const string SectionName = "influx";
        private const string RegistryName = "persistance.influxDb";

        public static IConveyBuilder AddInflux(this IConveyBuilder builder, string sectionName = SectionName,
            IInfluxDbSeeder seeder = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var influxOptions = builder.GetOptions<InfluxDbOptions>(sectionName);
            return builder.AddInflux(influxOptions, seeder);
        }

        public static IConveyBuilder AddInflux(this IConveyBuilder builder,
            Func<IInfluxDbOptionsBuilder, IInfluxDbOptionsBuilder> buildOptions, IInfluxDbSeeder seeder = null)
        {
            var influxOptions = buildOptions(new InfluxDbOptionsBuilder()).Build();
            return builder.AddInflux(influxOptions, seeder);
        }

        public static IConveyBuilder AddInflux(this IConveyBuilder builder, InfluxDbOptions influxDbOptions,
            IInfluxDbSeeder seeder = null)
        {
            if (!builder.TryRegister(RegistryName))
            {
                return builder;
            }

            builder.Services.AddSingleton(influxDbOptions);
            builder.Services.AddSingleton(sp =>
            {
                var options = sp.GetService<InfluxDbOptions>();
                var dbOptions = InfluxDBClientOptions.Builder.CreateNew();
                dbOptions.Org(options.Organization);
                dbOptions.Url(options.Url);
                dbOptions.Authenticate(options.Username, options.Password.ToCharArray());

                return InfluxDBClientFactory.Create(dbOptions.Build());
            });

            builder.Services.AddSingleton(sp =>
            {
                var client = sp.GetService<InfluxDBClient>();
                var options = sp.GetService<InfluxDbOptions>();
                var organizationsApi = client.GetOrganizationsApi();
                var organizations = organizationsApi.FindOrganizationsAsync().GetAwaiter().GetResult();
                var organization = organizations.FirstOrDefault(o => o.Name == options.Organization);
                if (organization is null)
                {
                    organization = organizationsApi.CreateOrganizationAsync(options.Organization).GetAwaiter().GetResult();
                }

                return organization;
            });

            builder.Services.AddTransient<IInfluxDbInitializer, InfluxDbInitializer>();

            return builder;
        }

        public static IConveyBuilder AddInfluxBucket<TEntity, TIdentifiable>(this IConveyBuilder builder,
            string bucketName)
            where TEntity : IIdentifiable<TIdentifiable>
        {
            builder.Services.AddTransient(sp =>
            {
                var client = sp.GetService<InfluxDBClient>();
                var organization = sp.GetService<Organization>();
                return InitializeBucket<TEntity, TIdentifiable>(client, organization, bucketName).GetAwaiter().GetResult();
            });

            return builder;
        }

        private static async Task<IInfluxBucket<TEntity, TIdentifiable>> InitializeBucket<TEntity, TIdentifiable>
            (InfluxDBClient client, Organization organization, string bucketName)
            where TEntity : IIdentifiable<TIdentifiable>
        {
            var bucketsApi = client.GetBucketsApi();
            var bucketsList = await bucketsApi.FindBucketsByOrganizationAsync(organization);
            var bucket = bucketsList.Find(b => b.Name == bucketName)
                         ?? await bucketsApi.CreateBucketAsync(bucketName, organization.Id);
            return new InfluxBucket<TEntity, TIdentifiable>(client, organization, bucket.Name);
        }
        
    }
}