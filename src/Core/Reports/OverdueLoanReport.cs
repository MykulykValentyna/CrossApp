namespace Core.Reports;

public sealed record OverdueLoanReport(string Id, string BookTitle, string ReaderId, DateTime DueDate);