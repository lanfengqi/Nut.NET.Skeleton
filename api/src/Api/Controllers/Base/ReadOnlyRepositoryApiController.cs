using AutoMapper;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    public abstract class ReadOnlyRepositoryApiController<TRepository, TModel, TViewModel> : AppApiController
            where TRepository : IReadOnlyRepository<TModel>
            where TModel : class, new()
            where TViewModel : class, new() {
        protected readonly TRepository _repository;
        protected static readonly bool _isOwnedByOrganization = typeof(IOwnedByOrganization).IsAssignableFrom(typeof(TModel));
        protected static readonly bool _isOrganization = typeof(TModel) == typeof(Organization);
        protected static readonly bool _supportsSoftDeletes = typeof(ISupportSoftDeletes).IsAssignableFrom(typeof(TModel));
        protected static readonly bool _isVersioned = typeof(IVersioned).IsAssignableFrom(typeof(TModel));
        protected readonly IMapper _mapper;

        public ReadOnlyRepositoryApiController(TRepository repository, IMapper mapper) {
            _repository = repository;
            _mapper = mapper;
        }

        #region Get

        public virtual async Task<IHttpActionResult> GetByIdAsync(string id) {
            TModel model = await GetModelAsync(id);
            if (model == null)
                return NotFound();

            return await OkModel(model);
        }

        protected async Task<IHttpActionResult> OkModel(TModel model) {
            if (typeof(TViewModel) == typeof(TModel) && _mapper.ConfigurationProvider.FindTypeMapFor<TModel, TViewModel>() == null)
                return Ok(model);

            return Ok(await Map<TViewModel>(model));
        }

        protected virtual async Task<TModel> GetModelAsync(string id) {
            if (String.IsNullOrEmpty(id))
                return null;

            TModel model = await _repository.GetByIdAsync(id);
            if (_isOwnedByOrganization && model != null && ((IOwnedByOrganization)model).OrganizationId != GetSelectedOrganizationId())
                return null;

            return model;
        }

        protected virtual async Task<IReadOnlyCollection<TModel>> GetModelsAsync(string[] ids) {
            if (ids == null || ids.Length == 0)
                return new List<TModel>();

            IReadOnlyCollection<TModel> models = await _repository.GetByIdsAsync(ids);
            var selectedOrganizationId = GetSelectedOrganizationId();
            if (_isOwnedByOrganization)
                models = models?.Where(m => ((IOwnedByOrganization)m).OrganizationId == selectedOrganizationId).ToList();

            return models;
        }
        
        #endregion

        #region Mapping

        protected async Task<TDestination> Map<TDestination>(object source) {
            var destination = _mapper.Map<TDestination>(source);
            return destination;
        }

        protected async Task<TDestination> Map<TDestination>(object source, TDestination destination) {
            destination = _mapper.Map(source, destination);
            return destination;
        }

        protected async Task<IReadOnlyCollection<TDestination>> MapCollection<TDestination>(object source) {
            var destination = _mapper.Map<IReadOnlyCollection<TDestination>>(source);
            return destination;
        }

        #endregion
    }
}
