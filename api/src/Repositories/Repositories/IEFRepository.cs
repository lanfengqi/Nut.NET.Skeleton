using Foundatio.Skeleton.Repositories.Model;

namespace Foundatio.Skeleton.Repositories {
    public interface IEFRepository<T> : IEFReadOnlyRepository<T>, IRepository<T> where T : class, IIdentity, new() {
    }
}
