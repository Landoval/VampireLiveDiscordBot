using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DiscordBot1.Database
{
    public class DataManager<TContext> where TContext : DbContext
    {
        private string[] dummystringargs;
        public IDesignTimeDbContextFactory<TContext> dbContextFactory;

        public object threadSync = new object();

        public DataManager(IDesignTimeDbContextFactory<TContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public virtual bool Add<TEntity>(TEntity item) where TEntity : class
        {
            try
            {
                using (var context = dbContextFactory.CreateDbContext(dummystringargs))
                {
                    context.Set<TEntity>().Add(item);
                    context.Entry(item).State = EntityState.Added;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    //SystemContainer.Logger.Fatal(ex, "Add");
                    ex = ex.InnerException;
                }

                return false;
            }

            return true;
        }

        public virtual bool Add<TEntity>(IEnumerable<TEntity> items) where TEntity : class
        {
            try
            {
                if (items.Count() > 0)
                {
                    using (var context = dbContextFactory.CreateDbContext(dummystringargs))
                    {
                        context.Set<TEntity>().AddRange(items);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    //SystemContainer.Logger.Fatal(ex, "Add");
                    ex = ex.InnerException;
                }

                return false;
            }

            return true;
        }

        public virtual bool Update<TEntity>(TEntity item) where TEntity : class
        {
            try
            {
                using (var context = dbContextFactory.CreateDbContext(dummystringargs))
                {

                    //context.Configuration.LazyLoadingEnabled = false;
                    //context.Configuration.ProxyCreationEnabled = false;
                    EntityEntry entry = context.Entry(item);

                    entry.State = EntityState.Modified;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    //SystemContainer.Logger.Fatal(ex, "Update");
                    ex = ex.InnerException;
                }

                return false;
            }

            return true;
        }

        public virtual bool Update<TEntity>(IEnumerable<TEntity> items) where TEntity : class
        {
            try
            {
                if (items.Count() > 0)
                {
                    using (var context = dbContextFactory.CreateDbContext(dummystringargs))
                    {
                        //context.Configuration.LazyLoadingEnabled = false;
                        //context.Configuration.ProxyCreationEnabled = false;
                        //context.Configuration.AutoDetectChangesEnabled = false;

                        foreach (TEntity item in items)
                        {

                            EntityEntry entry = context.Entry(item);

                            entry.State = EntityState.Modified;
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    //SystemContainer.Logger.Fatal(ex, "Update");
                    ex = ex.InnerException;
                }

                return false;
            }

            return true;
        }

        public virtual bool Delete<TEntity>(TEntity item) where TEntity : class
        {
            try
            {
                using (var context = dbContextFactory.CreateDbContext(dummystringargs))
                {
                    context.Entry(item).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    //SystemContainer.Logger.Fatal(ex, "Delete");
                    ex = ex.InnerException;
                }

                return false;
            }

            return true;
        }

        public virtual bool Delete<TEntity>(IEnumerable<TEntity> items) where TEntity : class
        {
            try
            {
                if (items.Count() > 0)
                {
                    using (var context = dbContextFactory.CreateDbContext(dummystringargs))
                    {
                        foreach (TEntity item in items)
                        {
                            context.Entry(item).State = EntityState.Deleted;
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    //SystemContainer.Logger.Fatal(ex, "Delete");
                    ex = ex.InnerException;
                }

                return false;
            }

            return true;
        }

        public virtual IList<TEntity> GetAll<TEntity>(params Expression<Func<TEntity, object>>[] navigationProperties) where TEntity : class
        {
            List<TEntity> list;
            using (var context = dbContextFactory.CreateDbContext(dummystringargs))
            {
                //context.Configuration.LazyLoadingEnabled = false;
                //context.Configuration.ProxyCreationEnabled = false;
                IQueryable<TEntity> dbQuery = context.Set<TEntity>();

                //Apply eager loading
                if (navigationProperties != null)
                    foreach (Expression<Func<TEntity, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<TEntity, object>(navigationProperty);

                list = dbQuery
                    .AsNoTracking()
                    .ToList<TEntity>();
            }
            return list;
        }

        public virtual IList<TEntity> GetList<TEntity>(Func<TEntity, bool> where,
             params Expression<Func<TEntity, object>>[] navigationProperties) where TEntity : class
        {
            List<TEntity> list;
            using (var context = dbContextFactory.CreateDbContext(dummystringargs))
            {
                //context.Configuration.LazyLoadingEnabled = false;
                //context.Configuration.ProxyCreationEnabled = false;
                IQueryable<TEntity> dbQuery = context.Set<TEntity>();

                //Apply eager loading
                foreach (Expression<Func<TEntity, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<TEntity, object>(navigationProperty);

                list = dbQuery
                    .AsNoTracking()
                    .Where(where)
                    .ToList<TEntity>();
            }
            return list;
        }

        public virtual TEntity GetSingle<TEntity>(Func<TEntity, bool> where,
              params Expression<Func<TEntity, object>>[] navigationProperties) where TEntity : class
        {
            TEntity item = null;
            using (var context = dbContextFactory.CreateDbContext(dummystringargs))
            {
                //context.Configuration.LazyLoadingEnabled = false;
                //context.Configuration.ProxyCreationEnabled = false;
                IQueryable<TEntity> dbQuery = context.Set<TEntity>();

                //Apply eager loading
                foreach (Expression<Func<TEntity, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<TEntity, object>(navigationProperty);

                item = dbQuery
                    .AsNoTracking() //Don't track any changes for the selected item
                    .FirstOrDefault(where); //Apply where clause
            }
            return item;
        }
    }
}
