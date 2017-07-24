using Foundatio.Skeleton.Repositories.Model;

namespace Foundatio.Skeleton.Domain.Models.Messaging {
    public class AppEntityChanged : EntityChanged {
        public string OrganizationId { get; set; }
        public string ContactId { get; set; }

        public static AppEntityChanged Create(EntityChanged entityChanged) {
            var appEntityChanged = new AppEntityChanged {
                Id = entityChanged.Id,
                Type = entityChanged.Type,
                ChangeType = entityChanged.ChangeType,
            };

            return appEntityChanged;
        }

        public class KnownKeys {
            public const string OrganizationId = "organization_id";
            public const string ContactId = "contact_id";
        }
    }
}
