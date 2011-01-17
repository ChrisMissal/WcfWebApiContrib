﻿using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SelfhostedServer.ServiceContracts {

    [ServiceContract]
    public class FooService {
        private readonly ILogger _Logger;

        public FooService(ILogger logger) {
            _Logger = logger;
        }

        [WebGet(UriTemplate = "root")]
        [OperationContract]
        public void GetServiceRoot(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage) {
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            var content = new StringContent(@"<html>
                                                <a href='foo'>Get plain text response by returning a string</a><br/>
                                                <a href='foo2'>Get plain text response by modifiing a passed HttpResponseMessage</a>
                                              </html>");
            content.Headers.ContentType.MediaType = "text/html";
            httpResponseMessage.Content = content;
        }


        // Recommended way to impact the httpResponseMessage directly
        [WebGet(UriTemplate = "Foo2")]
        [OperationContract]
        public void GetFoo2(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage) {
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            httpResponseMessage.Content = new StringContent("Hello world", System.Text.Encoding.UTF8, "text/plain");
        }

        
        [WebGet(UriTemplate = "Foo")]
        [OperationContract]
        public string GetFoo() {
            
            return "If you use fiddler and put text/plain it will return as text/plain, otherwise it will come back as an xml serialized string";
        }


    }
}
