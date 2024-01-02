using IdentityServer;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class CustomProfileService : IProfileService
    {

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var nameClaim = context.Subject.Claims.FirstOrDefault(x => x.Type == "name");
            var userId = nameClaim.Value;
            if (EmailClaimStorage.EmailClaims.TryGetValue(userId, out string email))
            {
                context.IssuedClaims.Add(new Claim("email", email));
            }


            if (nameClaim != null)
            {
                context.IssuedClaims.Add(new Claim("name", nameClaim.Value));
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = !string.IsNullOrEmpty(context.Subject.GetSubjectId());
            return Task.CompletedTask;
        }
    }
}
