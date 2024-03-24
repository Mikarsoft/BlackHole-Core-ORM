using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Web
{
    internal class BHMiddleware
    {

        //public class RequestRoutingMiddleware
        //{
        //    private readonly RequestDelegate _next;
        //    private readonly CustomController _customController;

        //    public RequestRoutingMiddleware(RequestDelegate next, CustomController customController)
        //    {
        //        _next = next;
        //        _customController = customController;
        //    }

        //    public async Task Invoke(HttpContext context)
        //    {
        //        // Inspect request path and route to custom controller as needed
        //        var requestPath = context.Request.Path;

        //        if (requestPath.StartsWithSegments("/custom"))
        //        {
        //            // Route to custom controller for handling
        //            await _customController.HandleRequest(context);
        //        }
        //        else
        //        {
        //            // Pass request to next middleware in the pipeline
        //            await _next(context);
        //        }
        //    }
        //}

    }
}
