using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Convey.Types;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

namespace Convey.Persistence.InfluxDB
{
    public interface IInfluxBucket<TEntity, in TIdentifiable> where TEntity : IIdentifiable<TIdentifiable>
    { 
        InfluxDBClient Client { get; }
        string BucketName { get; }
        FluxQueryBuilder GetFluxQueryBuilder();
        Task<IReadOnlyList<TEntity>> GetAsync(TIdentifiable id);
        Task<IReadOnlyList<TEntity>> GetAsync(TIdentifiable id, Predicate<TEntity> predicate);
        Task<IReadOnlyList<TEntity>> GetAsync(FluxQueryBuilder fluxQuery);
        Task<IReadOnlyList<TEntity>> GetAsync(string fluxQuery);
        Task<IReadOnlyList<TEntity>> FindAsync(FluxQueryBuilder fluxQuery);
        Task<IReadOnlyList<TEntity>> FindAsync(string fluxQuery);
        Task<PagedResult<TEntity>> BrowseAsync<TQuery>(FluxQueryBuilder fluxQuery, TQuery query) where TQuery : IPagedQuery;
        Task<PagedResult<TEntity>> BrowseAsync<TQuery>(string fluxQuery, TQuery query) where TQuery : IPagedQuery;
        Task<PagedResult<TEntity>> BrowseAsync<TQuery>(TIdentifiable id, TQuery query) where TQuery : IPagedQuery;
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task DeleteAsync(TIdentifiable id);
        Task DeleteAsync(TIdentifiable id, DateTime start, DateTime end);
        Task DeleteAsync(DeletePredicateRequest request);
        Task<bool> ExistsAsync(TIdentifiable id);
    }
}