using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json.Linq;
using System.Collections;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambdaAuthorizer
{
    public class Function
    {
        /// <summary>
        /// For the purpose of demo, these values are hardcoded. You will get them from Secret Manager, OAuth2/OIDC, S3, Database, etc.
        /// </summary>
        const string clientCertSubjectDN = "C=US,ST=Texas,L=Dallas,O=Aspnet4you,OU=InfoSec,CN=My mTls Client";
        const string clientCertIssuerDN = "C=US,ST=Texas,L=Dallas,O=Aspnet4you,OU=InfoSec,CN=My Private Root CA";
        const string tobereplacedToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJhc3BuZXQ0eW91LmNvbSIsImF1ZCI6InBldHN0b3JlIiwic3ViIjoicGV0c3RvcmVAcGV0c3RvcmUuY29tIiwiaWF0IjoxNjE1Nzc3MDkxLCJleHAiOjE2MTkzNzcwOTF9.LW9rXnMbx735xWxUSzGZtYYnaqIwVRKbMVJSWd_W9s-VZISaGQ6gUKgT_sAiM_1-1am0rt13YXz7sGQU13feqRAsrbenuyGWJbLS_nCyHog2J2jJlQVyfenH2BsF45egO2ctM3AuGMAUugxotrimWdg42m4reatt46EzVRjpQ-6SqlUdAPGu8QwinrjR-9MwYUhBQjS5Mbp1TQcgZj-hsYtFtl_lBoNP7jVkXxjfhjW5X8kSiF5FEq0T5xg_yMK1YiFutnbufS9ezufbn-RBi4MKnZvoVVHwVmR5-sCC62DWrW6CwOXsM2B_dGi7evQOrYFsnq6jgcm5ZF3rbalLYA";

        /// <summary>
        /// Credits :
        /// https://aws.amazon.com/blogs/compute/introducing-mutual-tls-authentication-for-amazon-api-gateway/
        /// https://medium.com/contino-engineering/mutual-tls-auth-with-aws-api-gateway-part-2-check-certificate-revocation-8b86538afe0
        /// https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/lambda-creating-project-in-visual-studio.html
        /// https://docs.aws.amazon.com/apigateway/latest/developerguide/http-api-lambda-authorizer.html   (v1.0 schema used)
        /// https://docs.aws.amazon.com/apigateway/latest/developerguide/apigateway-use-lambda-authorizer.html
        /// https://stackoverflow.com/questions/45631758/how-to-pass-api-gateway-authorizer-context-to-a-http-integration
        /// https://tools.aspnet4you.com/#/jwt (generate jwt token)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ApiGatewayLambdaAuthorizerResponse> FunctionHandler(ApiGatewayLambdaAuthorizerRequest request, ILambdaContext context)
        {
            var authorized = false;
            string issuerDN = "";
            string subjectDN = "";

            try
            {
                Clientcert clientCert = request?.requestContext?.identity?.clientCert;
                
                if(clientCert !=null)
                {
                    context.Logger.Log($"serialNumber = {clientCert.serialNumber}");
                    context.Logger.Log($"subjectDN = {clientCert.subjectDN}");
                    context.Logger.Log($"issuerDN = {clientCert.issuerDN}");
                    context.Logger.Log($"validity = {clientCert.validity.notAfter}");

                    if(clientCert.subjectDN == clientCertSubjectDN &&clientCert.issuerDN == clientCertIssuerDN)
                    {
                        subjectDN = clientCert.subjectDN;
                        issuerDN = clientCert.issuerDN;
                        authorized = true;
                    }
                }

            }
            catch (Exception ex)
            {
                context.Logger.Log(ex.Message);
            }

            return CreateResponse(request.methodArn, authorized, subjectDN, issuerDN);
        }

       
        private static ApiGatewayLambdaAuthorizerResponse CreateResponse(string methodArn, bool authorized, string subjectDN, string issuerDN)
        {
            Statement statement = new Statement()
            {
                Action = "execute-api:Invoke",
                Effect = authorized ? "Allow" : "Deny",
                Resource = methodArn
            };

            Policydocument policy = new Policydocument
            {
                Version = "2012-10-17",
                Statement = new [] { statement}
            };

            return new ApiGatewayLambdaAuthorizerResponse
            {
                principalId = subjectDN,
                policyDocument = policy,
                context = new Context() 
                { 
                    JwtTokenForBackendApi = authorized ? tobereplacedToken : "",
                    IssuerDN = issuerDN,
                    SubjectDN = subjectDN
                }
            };
        }
    }
}
