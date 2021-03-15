using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambdaAuthorizer
{
    public class ApiGatewayLambdaAuthorizerRequest
    {
        public string version { get; set; }
        public string type { get; set; }
        public string methodArn { get; set; }
        public string identitySource { get; set; }
        public string authorizationToken { get; set; }
        public string resource { get; set; }
        public string path { get; set; }
        public string httpMethod { get; set; }
        public Headers headers { get; set; }
        public Querystringparameters queryStringParameters { get; set; }
        public Pathparameters pathParameters { get; set; }
        public Stagevariables stageVariables { get; set; }
        public Requestcontext requestContext { get; set; }
    }

    public class Headers
    {
        public string XAMZDate { get; set; }
        public string Accept { get; set; }
        public string HeaderAuth1 { get; set; }
        public string CloudFrontViewerCountry { get; set; }
        public string CloudFrontForwardedProto { get; set; }
        public string CloudFrontIsTabletViewer { get; set; }
        public string CloudFrontIsMobileViewer { get; set; }
        public string UserAgent { get; set; }
    }

    public class Querystringparameters
    {
        public string QueryString1 { get; set; }
    }

    public class Pathparameters
    {
    }

    public class Stagevariables
    {
        public string StageVar1 { get; set; }
    }

    public class Requestcontext
    {
        public string path { get; set; }
        public string accountId { get; set; }
        public string resourceId { get; set; }
        public string stage { get; set; }
        public string requestId { get; set; }
        public Identity identity { get; set; }
        public string resourcePath { get; set; }
        public string httpMethod { get; set; }
        public string apiId { get; set; }
    }

    public class Identity
    {
        public string apiKey { get; set; }
        public string sourceIp { get; set; }
        public Clientcert clientCert { get; set; }
    }

    public class Clientcert
    {
        public string clientCertPem { get; set; }
        public string subjectDN { get; set; }
        public string issuerDN { get; set; }
        public string serialNumber { get; set; }
        public Validity validity { get; set; }
    }

    public class Validity
    {
        public string notBefore { get; set; }
        public string notAfter { get; set; }
    }

}
