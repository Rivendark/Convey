using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Convey.Types;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

namespace Convey.Persistence.InfluxDB.Buckets
{
    internal class InfluxBucket<TEntity, TIdentifiable> : IInfluxBucket<TEntity, TIdentifiable>
        where TEntity : IIdentifiable<TIdentifiable>
    {
        public InfluxDBClient Client { get; }
        public Organization Organization { get; }
        public string BucketName { get; }
        
        public InfluxBucket(InfluxDBClient client, Organization organization, string bucketName)
        {
            Client = client;
            Organization = organization;
            BucketName = bucketName;
        }
        
        public FluxQueryBuilder GetFluxQueryBuilder()
        {
            return new FluxQueryBuilder(BucketName);
        }

        public async Task<IReadOnlyList<TEntity>> GetAsync(TIdentifiable id)
        {
            var queryBuilder = GetFluxQueryBuilder();
            queryBuilder.AddFilter("id", FluxEquationOperators.Equals, id.ToString());
            return await Client.GetQueryApi().QueryAsync<TEntity>(queryBuilder.GetQuery(), Organization.Id);
        }

        public async Task<IReadOnlyList<TEntity>> GetAsync(TIdentifiable id, Predicate<TEntity> predicate)
        {
            var queryBuilder = GetFluxQueryBuilder();
            queryBuilder.AddFilter("id", FluxEquationOperators.Equals, id.ToString());
            var collection = await Client.GetQueryApi()
                .QueryAsync<TEntity>(queryBuilder.GetQuery(), Organization.Id);
            return collection.FindAll(predicate);
        }

        public async Task<IReadOnlyList<TEntity>> GetAsync(FluxQueryBuilder fluxQuery)
            => await Client.GetQueryApi().QueryAsync<TEntity>(fluxQuery.GetQuery(), Organization.Id);
        
        public async Task<IReadOnlyList<TEntity>> GetAsync(string fluxQuery)
            => await Client.GetQueryApi().QueryAsync<TEntity>(fluxQuery, Organization.Id);

        public async Task<IReadOnlyList<TEntity>> FindAsync(FluxQueryBuilder fluxQuery)
            => await Client.GetQueryApi().QueryAsync<TEntity>(fluxQuery.GetQuery(), Organization.Id);

        public async Task<IReadOnlyList<TEntity>> FindAsync(string fluxQuery)
            => await Client.GetQueryApi().QueryAsync<TEntity>(fluxQuery, Organization.Id);

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(FluxQueryBuilder fluxQuery, TQuery query)
            where TQuery : IPagedQuery
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(string fluxQuery, TQuery query)
            where TQuery : IPagedQuery
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(TIdentifiable id, TQuery query)
            where TQuery : IPagedQuery
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(TEntity entity)
            => await Client.GetWriteApiAsync()
                .WriteMeasurementAsync(BucketName, Organization.Id, WritePrecision.Ns, entity);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
            => await Client.GetWriteApiAsync()
                .WriteMeasurementsAsync(BucketName, Organization.Id, WritePrecision.Ns, entities);

        public async Task DeleteAsync(TIdentifiable id)
            => await Client.GetDeleteApi().Delete(
                new DeletePredicateRequest(DateTime.MinValue, DateTime.UtcNow, $"id = {id.ToString()}"),
                BucketName,
                Organization.Id
            );

        public async Task DeleteAsync(TIdentifiable id, DateTime start, DateTime end)
            => await Client.GetDeleteApi().Delete(
                new DeletePredicateRequest(start, end, $"id = {id.ToString()}"),
                BucketName,
                Organization.Id
            );
        
        public async Task DeleteAsync(DeletePredicateRequest request)
            => await Client.GetDeleteApi().Delete(request, BucketName, Organization.Id);

        public async Task<bool> ExistsAsync(TIdentifiable id)
            => (await Client.GetQueryApi().QueryAsync<TEntity>(
                GetFluxQueryBuilder().AddFilter("id", FluxEquationOperators.Equals, id.ToString())
                    .GetQuery(), Organization.Id
                )
            ).Count > 0;
    }
}