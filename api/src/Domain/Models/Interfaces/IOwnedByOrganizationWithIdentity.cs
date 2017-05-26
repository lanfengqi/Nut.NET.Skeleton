using Foundatio.Skeleton.Repositories.Model;

namespace Foundatio.Skeleton.Domain.Models
{
    public interface IOwnedByOrganizationWithIdentity : IOwnedByOrganization, IIdentity
    {
    }
}