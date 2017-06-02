using FluentValidation;
using Foundatio.Caching;
using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories.Repositories {
    public class EFRepositoryBase<T> : EFReadOnlyRepositoryBase<T>, IEFRepository<T> where T : class, IIdentity, new() {
        protected readonly IValidator<T> _validator;

        public EFRepositoryBase(IEFRepositoryContext efRepositoryContext, ICacheClient cacheClient, IValidator<T> validator = null)
            : base(efRepositoryContext, cacheClient) {

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

            var doc = _context.Entry(document);
            doc.State = System.Data.Entity.EntityState.Modified;

            await _context.SaveChangesAsync();

            return document;
        }

        public async Task SaveAsync(IEnumerable<T> documents) {

            var docs = documents?.ToList();
            if (docs == null || docs.Any(d => d == null))
                throw new ArgumentNullException(nameof(documents));

            if (docs.Count == 0)
                return;

            foreach (var doc in docs) {
                var document = _context.Entry(doc);
                document.State = System.Data.Entity.EntityState.Modified;
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
