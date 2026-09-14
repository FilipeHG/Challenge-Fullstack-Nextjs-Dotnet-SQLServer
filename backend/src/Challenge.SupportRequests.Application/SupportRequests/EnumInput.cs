namespace Challenge.SupportRequests.Application.SupportRequests;

internal static class EnumInput
{
    public static bool IsPriority(string? value) => value is "low" or "medium" or "high";
    public static bool IsStatus(string? value) => value is "open" or "inProgress" or "completed";
}
