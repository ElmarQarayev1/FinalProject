﻿using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Medical.Data.Repositories.Interfaces
{
	public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void Delete(TEntity entity);
        TEntity Get(Expression<Func<TEntity, bool>> predicate, params string[] includes);
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, params string[] includes);
        bool Exists(Expression<Func<TEntity, bool>> predicate, params string[] includes);
        int Save();
        void DeleteRange(IEnumerable<TEntity> entities);
       
        

    }
}

