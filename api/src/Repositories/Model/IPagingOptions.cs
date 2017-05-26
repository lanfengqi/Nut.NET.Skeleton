namespace Foundatio.Skeleton.Repositories.Model {
    public interface IPagingOptions {
        int? Limit { get; set; }
        int? Page { get; set; }
    }
}
