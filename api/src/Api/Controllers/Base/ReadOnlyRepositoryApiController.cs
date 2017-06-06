﻿using AutoMapper;
using Foundatio.Skeleton.Core.Models;
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

        public virtual async Task<IHttpActionResult> GetById(string id) {
            TModel model = await GetModel(id);
            if (model == null)
                return NotFound();

            return await OkModel(model);
        }

        protected async Task<IHttpActionResult> OkModel(TModel model) {
            if (typeof(TViewModel) == typeof(TModel) && _mapper.ConfigurationProvider.FindTypeMapFor<TModel, TViewModel>() == null)
                return Ok(model);

            return Ok(await Map<TViewModel>(model, true));
        }

        protected virtual async Task<TModel> GetModel(string id, bool useCache = true) {
            if (String.IsNullOrEmpty(id))
                return null;

            TModel model = await _repository.GetByIdAsync(id, useCache);
            if (_isOwnedByOrganization && model != null && ((IOwnedByOrganization)model).OrganizationId != GetSelectedOrganizationId())
                return null;

            return model;
        }

        protected virtual async Task<IReadOnlyCollection<TModel>> GetModels(string[] ids, bool useCache = true) {
            if (ids == null || ids.Length == 0)
                return new List<TModel>();

            IReadOnlyCollection<TModel> models = await _repository.GetByIdsAsync(ids, useCache: useCache);
            var selectedOrganizationId = GetSelectedOrganizationId();
            if (_isOwnedByOrganization)
                models = models?.Where(m => ((IOwnedByOrganization)m).OrganizationId == selectedOrganizationId).ToList();

            return models;
        }
        
        #endregion

        #region Mapping

        protected async Task<TDestination> Map<TDestination>(object source, bool isResult = false) {
            var destination = _mapper.Map<TDestination>(source);

            if (isResult)
                await AfterResultMap(new List<TDestination>(new[] { destination }));
            return destination;
        }

        protected async Task<TDestination> Map<TDestination>(object source, TDestination destination, bool isResult = false) {
            destination = _mapper.Map(source, destination);

            if (isResult)
                await AfterResultMap(new List<TDestination>(new[] { destination }));
            return destination;
        }

        protected async Task<IReadOnlyCollection<TDestination>> MapCollection<TDestination>(object source, bool isResult = false) {
            var destination = _mapper.Map<IReadOnlyCollection<TDestination>>(source);

            if (isResult)
                await AfterResultMap<TDestination>(destination);
            return destination;
        }

        protected virtual async Task AfterResultMap<TDestination>(IReadOnlyCollection<TDestination> models) {
            foreach (var model in models.OfType<IHaveData>())
                model.Data.RemoveSensitiveData();
        }

        #endregion
    }
}
