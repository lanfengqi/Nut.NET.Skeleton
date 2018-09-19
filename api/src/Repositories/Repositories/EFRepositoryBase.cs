using FluentValidation;
using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories {
    public class EFRepositoryBase<T> : EFReadOnlyRepositoryBase<T>, IEFRepository<T> where T : class, IIdentity, new() {
        protected readonly IValidator<T> _validator;

        public EFRepositoryBase(IEFRepositoryContext eFRepositoryContext, IValidator<T> validator = null)
            : base(eFRepositoryContext) {

            this._validator = validator;
        }


        public async Task<T> AddAsync(T document) {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            await AddAsync(new[] { document });

            return document;
        }

        public async Task AddAsync(IEnumerable<T> documents) {

            var docs = documents?.ToList();
            if (docs == null || docs.Any(d => d == null))
                throw new ArgumentNullException(nameof(documents));

            if (docs.Count == 0)
                return;
            if (_validator != null)
                foreach (var doc in docs)
                    await _validator.ValidateAndThrowAsync(doc);

            foreach (var doc in docs) {
                _context.Set<T>().Add(doc);
            }

            
            await _context.SaveChangesAsync();
        }

        public async Task<T> SaveAsync(T document) {

            if (document == null)
                throw new ArgumentNullException(nameof(document));

            ////RemoveHoldingEntityInContext(document);
            ////AttachIfNot(document);
            ////_context.Entry(document).State = EntityState.Modified;
            //((IObjectContextAdapter)_context).ObjectContext.Detach(document);

            _context.Entry(document).State = EntityState.Modified;
               
            await _context.SaveChangesAsync();

            return document;
        }

        private Boolean RemoveHoldingEntityInContext(T entity) {
            var objContext = ((IObjectContextAdapter)_context).ObjectContext;
            var objSet = objContext.CreateObjectSet<T>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);

            if (exists) {
                objContext.Detach(foundEntity);
            }

            return (exists);
        }

        protected virtual void AttachIfNot(T entity) {
            if (!_context.Set<T>().Local.Contains(entity)) {
                _context.Set<T>().Attach(entity);
            }
        }

        public async Task SaveAsync(IEnumerable<T> documents) {

            var docs = documents?.ToList();
            if (docs == null || docs.Any(d => d == null))
                throw new ArgumentNullException(nameof(documents));

            if (docs.Count == 0)
                return;

            foreach (var doc in docs) {
                _context.Entry(doc).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(string id) {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            await RemoveAsync(new[] { id });
        }

        public async Task RemoveAsync(IEnumerable<string> ids) {

            var docs = ids?.ToList();
            if (docs == null || docs.Any(d => d == null))
                throw new ArgumentNullException(nameof(ids));

            if (docs.Count == 0)
                return;

            var entitys = await GetByIdsAsync(ids);
            if (entitys != null && entitys.Any()) {
                _context.Set<T>().RemoveRange(entitys);

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(T document) {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            await RemoveAsync(new[] { document });

        }

        public async Task RemoveAsync(IEnumerable<T> documents) {

            var docs = documents?.ToList();
            if (docs == null || docs.Any(d => d == null))
                throw new ArgumentNullException(nameof(documents));

            if (docs.Count == 0)
                return;

            foreach (var doc in docs) {
                _context.Set<T>().Remove(doc);
            }
            await _context.SaveChangesAsync();
        }

    }
}
