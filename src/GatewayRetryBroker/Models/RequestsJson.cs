using Newtonsoft.Json;

namespace GatewayRetryBroker.Models;

public record RevertTakeBookRequestJson([JsonProperty] Guid ReservationUid);

public record ChangeAvailableCountRequestJson([JsonProperty] Guid LibraryUid, [JsonProperty] Guid BookUid, [JsonProperty] int Count);

public record UpdateRatingRequestJson([JsonProperty] string Username, [JsonProperty] int Stars);
