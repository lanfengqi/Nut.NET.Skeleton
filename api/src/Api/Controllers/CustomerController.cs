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
    [RoutePrefix(API_PREFIX + "/customers")]
    //[Authorize(Roles = AuthorizationRoles.User)]
    public class CustomerController :  RepositoryApiController<ICustomerRepository, Customer, ViewCustomer, NewCustomer, ViewCustomer> {

        public CustomerController(ICustomerRepository customerRepository,
            ILoggerFactory loggerFactory,
            IMessagePublisher messagePublisher,
            IMapper mapper)
            : base(loggerFactory, customerRepository, mapper) {

        }

        [HttpGet]
        [Route("admin")]
        public async Task<IHttpActionResult> GetForAdminsAsync(int page = 1, int limit = 10) {

            page = GetPage(page);
            limit = GetLimit(limit);

            var customers = await _repository.FindAsync(x => x.Code != "", new PagingOptions { Page = page, Limit = limit });

            return OkWithResourceLinks(customers, customers.TotalPages > page, page, customers.TotalCount);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewCustomer))]
        [HttpGet]
        [Route("{id:objectid}", Name = "GetCustomerById")]
        public override async Task<IHttpActionResult> GetByIdAsync(string id) {
            var customer = await GetModelAsync(id);
            if (customer == null)
                return NotFound();

            var viewCustomer = await Map<ViewCustomer>(customer);
            return Ok(viewCustomer);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewCustomer))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(NewCustomer value) {
            return base.PostAsync(value);
        }


        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewCustomer))]
        [HttpPut]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PutAsync(string id, ViewCustomer customer, long? version = null) {
            return base.PutAsync(id, customer, version);
        }

        [HttpDelete]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> DeleteAsync(string id) {
            return base.DeleteAsync(id);
        }

        protected override Task<Customer> AddModelAsync(Customer value) {
            value.Id = Guid.NewGuid().ToString("N");
            value.CreatedUtc = value.UpdatedUtc = DateTime.UtcNow;

            return base.AddModelAsync(value);
        }
    }
}
