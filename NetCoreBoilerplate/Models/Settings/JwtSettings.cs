using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace NetCoreBoilerplate.Models.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }

        public string Subject { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }
        
        public int Validity { get; set; }
        
        public DateTime Expiration => IssuedAt.Add(ValidFor);

        public DateTime NotBefore { get; set; } = DateTime.UtcNow;

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public TimeSpan ValidFor { get; set; } = TimeSpan.FromDays(1);

        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
    }
}