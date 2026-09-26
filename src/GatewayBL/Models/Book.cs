using GatewayBL.Enums;

namespace GatewayBL.Models;

public record Book(Guid BookUid, string Name, string? Author, string? Genre, Condition Condition, int AvailableCount);