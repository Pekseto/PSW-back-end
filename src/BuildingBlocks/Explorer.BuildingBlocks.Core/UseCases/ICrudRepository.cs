﻿using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.BuildingBlocks.Core.UseCases;

public interface ICrudRepository<TEntity> where TEntity : Entity
{
    PagedResult<TEntity> GetPaged(int page, int pageSize);
    TEntity Get(long id);
    TEntity Create(TEntity entity);
    TEntity Update(TEntity entity);
    List<TEntity> GetFiltered(Predicate<TEntity> predicate);
    void Delete(long id);
}