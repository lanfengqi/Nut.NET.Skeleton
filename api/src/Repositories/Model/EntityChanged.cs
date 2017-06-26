using Foundatio.Skeleton.Core.Collections;

namespace Foundatio.Skeleton.Repositories.Model {
    public class EntityChanged {

        public string Type { get; set; }
        public string Id { get; set; }
        public ChangeType ChangeType { get; set; }
        public DataDictionary Data { get; set; }
    }

    public enum ChangeType {
        Added = 0,
        Saved = 1,
        Removed = 2
    }
}
