using LibraryBL.Enums;

namespace LibraryBL.Models;

public record Book(int Id, Guid BookUid, string Name, string? Author, string? Genre, Condition Condition, int AvailableCount);