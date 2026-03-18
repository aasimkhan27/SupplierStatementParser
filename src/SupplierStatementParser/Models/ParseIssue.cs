namespace SupplierStatementParser.Models;

/// <summary>
/// Represents a warning or error generated during parsing.
/// </summary>
/// <param name="Code">Machine-readable issue code.</param>
/// <param name="Message">Human-readable message.</param>
/// <param name="Severity">Issue severity.</param>
/// <param name="Source">Optional source component.</param>
public sealed record ParseIssue(string Code, string Message, ParseSeverity Severity, string? Source = null);
