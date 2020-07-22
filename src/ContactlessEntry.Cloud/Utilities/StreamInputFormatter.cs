using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Utilities
{
    public class StreamInputFormatter : InputFormatter
    {
        public StreamInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Octet));
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            if (null == context)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return ReadRequestBodyAsyncImplementation(context);
        }

        private static async Task<InputFormatterResult> ReadRequestBodyAsyncImplementation(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();

                await request.Body.DrainAsync(CancellationToken.None);
                request.Body.Seek(0, SeekOrigin.Begin);
            }

            return InputFormatterResult.Success(request.Body);
        }
    }
}
