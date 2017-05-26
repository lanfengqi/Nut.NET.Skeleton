using System;

namespace Foundatio.Skeleton.Repositories.Model {
    public interface IHaveDates : IHaveCreatedDate {
        DateTime UpdatedUtc { get; set; }
    }

    public interface IHaveCreatedDate {
        DateTime CreatedUtc { get; set; }
    }
}
