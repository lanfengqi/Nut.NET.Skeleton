namespace Foundatio.Skeleton.Repositories.Model {
    public interface IVersioned {
        /// <summary>
        /// Current modification version for the document.
        /// </summary>
        long Version { get; set; }
    }
}
