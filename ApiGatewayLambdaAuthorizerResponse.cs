using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambdaAuthorizer
{
    
    public class ApiGatewayLambdaAuthorizerResponse
    {
        public string principalId { get; set; }
        public Policydocument policyDocument { get; set; }
        public Context context { get; set; }
    }

    public class Policydocument
    {
        public string Version { get; set; }
        public Statement[] Statement { get; set; }
    }

    public class Statement
    {
        public string Action { get; set; }
        public string Effect { get; set; }
        public string Resource { get; set; }
    }

    public class Context
    {
        public string JwtTokenForBackendApi { get; set; }
        public string SubjectDN { get; set; }
        public string IssuerDN { get; set; }
    }

}
