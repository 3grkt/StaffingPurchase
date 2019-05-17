using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace StaffingPurchase.Web.Extensions
{
    public class HttpExtension
    {
        public class FileActionResult : IHttpActionResult
        {
            public FileActionResult(Stream content, string fileName, MediaTypeHeaderValue mediaType)
            {
                this.Content = content;
                this.FileName = fileName;
                this.MediaType = mediaType;
            }

            public FileActionResult(Stream content, string fileName)
            {
                this.Content = content;
                this.FileName = fileName;
            }

            public Stream Content { get; }

            public string FileName { get; }
            public MediaTypeHeaderValue MediaType { get; }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage response = new HttpResponseMessage { Content = new StreamContent(Content) };
                response.Content.Headers.ContentType = MediaType ?? new MediaTypeHeaderValue("application/octet-stream");

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = this.FileName
                };

                return Task.FromResult(response);
            }
        }

        public class StaffPurchaseExceptionActionResult : IHttpActionResult
        {
            public StaffPurchaseExceptionActionResult(string message, HttpStatusCode status = HttpStatusCode.InternalServerError)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                Status = status;
                Message = message;
            }

            public string Message { get; }

            public HttpStatusCode Status { get; }

            public HttpResponseMessage Execute()
            {
                HttpResponseMessage response = new HttpResponseMessage(Status) { Content = new StringContent(Message) };
                return response;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute());
            }
        }
    }
}