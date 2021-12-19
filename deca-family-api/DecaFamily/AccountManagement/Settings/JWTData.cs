using System;

namespace AccountManagement.Settings
{
    public class JWTData
    {
        public const string Data = "JWTConfigurations";

        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}
