using Foundatio.Caching;
using Foundatio.Logging;
using Foundatio.Metrics;
using Foundatio.Skeleton.Api.Models.Media;
using Foundatio.Skeleton.Core.Utility;
using Foundatio.Skeleton.Domain;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Storage;
using Swashbuckle.Swagger.Annotations;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/media")]
    public class MediaController : AppApiController {
        private readonly IFileStorage _fileStorage;
        private readonly IMetricsClient _metricsClient;
        private readonly ILogger _logger;
        private readonly ICacheClient _cacheClinet;

        public MediaController(ILoggerFactory loggerFactory, IFileStorage fileStorage, IMetricsClient metricsClient
            , ICacheClient cacheClient) {
            _logger = loggerFactory?.CreateLogger<MediaController>() ?? NullLogger.Instance;
            _fileStorage = fileStorage;
            _metricsClient = metricsClient;
            _cacheClinet = cacheClient;
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FileResponseModel))]
        [HttpPost]
        [Route("fileUpload")]
        public async Task<IHttpActionResult> FileUploadAsync() {
            var sessionId = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");

            var file = HttpContext.Current.Request.Files[0];
            if (file == null)
                throw new ArgumentNullException("Not file uploaded");

            var stream = file.InputStream;
            var fileName = Path.GetFileName(file.FileName);
            var fileExtension = Path.GetExtension(fileName);

            var fileStorageName = string.Format("{0}{1}", sessionId, fileExtension);
            var path = PathHelper.ExpandPath($"{Settings.Current.StorageFolder}\\public\\");

            await _fileStorage.SaveFileAsync(string.Format("{0}{1}", path, fileStorageName), stream);

            return Ok(new FileResponseModel { FileName = fileStorageName, FileUrl = StorageHelper.GetPictureUrl(fileStorageName) });
        }


    }
}
