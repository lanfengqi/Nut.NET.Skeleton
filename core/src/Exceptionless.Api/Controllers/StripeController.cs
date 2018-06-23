﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Exceptionless.Core;
using Exceptionless.Core.Billing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Exceptionless.Api.Controllers {
    [Route(API_PREFIX + "/stripe")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class StripeController : ExceptionlessApiController {
        private readonly StripeEventHandler _stripeEventHandler;
        private readonly ILogger _logger;

        public StripeController(StripeEventHandler stripeEventHandler, ILogger<StripeController> logger) {
            _stripeEventHandler = stripeEventHandler;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostAsync() {
            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            using (_logger.BeginScope(new ExceptionlessState().SetHttpContext(HttpContext).Property("event", json))) {
                if (String.IsNullOrEmpty(json)) {
                    _logger.LogWarning("Unable to get json of incoming event.");
                    return BadRequest();
                }

                StripeEvent stripeEvent;
                try {
                    stripeEvent = StripeEventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], Settings.Current.StripeWebHookSigningSecret);
                } catch (Exception ex) {
                    _logger.LogError(ex, "Unable to parse incoming event with {Signature}: {Message}", Request.Headers["Stripe-Signature"], ex.Message);
                    return BadRequest();
                }

                if (stripeEvent == null) {
                    _logger.LogWarning("Null stripe event.");
                    return BadRequest();
                }

                await _stripeEventHandler.HandleEventAsync(stripeEvent);
                return Ok();
            }
        }
    }
}