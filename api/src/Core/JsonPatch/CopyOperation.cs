using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Foundatio.Skeleton.Core.JsonPatch {
    public class CopyOperation : Operation {
        public string FromPath { get; set; }

        public override void Write(JsonWriter writer) {
            writer.WriteStartObject();

            WriteOp(writer, "copy");
            WritePath(writer, Path);
            WriteFromPath(writer, FromPath);

            writer.WriteEndObject();
        }

        public override void Read(JObject jOperation) {
            Path = jOperation.Value<string>("path");
            FromPath = jOperation.Value<string>("from");
        }
    }
}