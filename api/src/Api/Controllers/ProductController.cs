using AutoMapper;
using Foundatio.Logging;
using Foundatio.Messaging;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Foundatio.Skeleton.Repositories.Model;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/products")]
    //[Authorize(Roles = AuthorizationRoles.User)]
    public class ProductController :  RepositoryApiController<IProductRepository, Product, ViewProduct, NewProduct, ViewProduct> {
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _messagePublisher;

        public ProductController(IProductRepository productRepository,
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            IMessagePublisher messagePublisher,
            IMapper mapper)
            : base(loggerFactory, productRepository, mapper) {
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
        }

        [HttpGet]
        [Route("admin")]
        //[Authorize(Roles = AuthorizationRoles.GlobalAdmin)]
        public async Task<IHttpActionResult> GetForAdminsAsync(int page = 1, int limit = 10) {

            page = GetPage(page);
            limit = GetLimit(limit);

            var products = await _repository.FindAsync(x => x.Code != "", new PagingOptions { Page = page, Limit = limit });

            return OkWithResourceLinks(products, products.TotalPages > page, page, products.TotalCount);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewProduct))]
        [HttpGet]
        [Route("{id:objectid}", Name = "GetProductById")]
        public override async Task<IHttpActionResult> GetByIdAsync(string id) {
            var product = await GetModelAsync(id);
            if (product == null)
                return NotFound();

            var viewPorduct = await Map<ViewProduct>(product);
            return Ok(viewPorduct);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewProduct))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(NewProduct value) {
            return base.PostAsync(value);
        }


        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewProduct))]
        [HttpPut]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PutAsync(string id, ViewProduct product, long? version = null) {
            return base.PutAsync(id, product, version);
        }

        [HttpDelete]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> DeleteAsync(string id) {
            return base.DeleteAsync(id);
        }

        protected override Task<Product> AddModelAsync(Product value) {
            value.Id = Guid.NewGuid().ToString("N");
            value.CreatedUtc = value.UpdatedUtc = DateTime.UtcNow;

            return base.AddModelAsync(value);
        }
    }
}
