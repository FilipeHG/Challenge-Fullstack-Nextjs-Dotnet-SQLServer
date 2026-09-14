using Challenge.SupportRequests.Domain.Enums;
using Challenge.SupportRequests.Domain.Exceptions;

namespace Challenge.SupportRequests.Domain.Entities;

public sealed class SupportRequest
{
    public long Id { get; private set; }
    public string Title { get; }
    public string Description { get; }
    public string Requester { get; }
    public Priority Priority { get; private set; }
    public RequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? CompletedAt { get; private set; }

    private SupportRequest(long id, string title, string description, string requester, Priority priority,
        RequestStatus status, DateTime createdAt, DateTime? completedAt)
    {
        Title = RequiredText(title, 200);
        Description = RequiredText(description, 4000);
        Requester = RequiredText(requester, 200);
        if (!Enum.IsDefined(priority) || !Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(priority));
        if ((status == RequestStatus.Completed) != completedAt.HasValue)
            throw new ArgumentException("Completion timestamp must match status.");
        Id = id;
        Priority = priority;
        Status = status;
        CreatedAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
        CompletedAt = completedAt.HasValue ? DateTime.SpecifyKind(completedAt.Value, DateTimeKind.Utc) : null;
    }

    public static SupportRequest Create(string title, string description, string requester, Priority priority, DateTime utcNow)
        => new(0, title, description, requester, priority, RequestStatus.Open, utcNow, null);

    public static SupportRequest Rehydrate(long id, string title, string description, string requester, Priority priority,
        RequestStatus status, DateTime createdAt, DateTime? completedAt)
        => new(id, title, description, requester, priority, status, createdAt, completedAt);

    public void AssignId(long id)
    {
        if (Id != 0 || id <= 0) throw new InvalidOperationException("Identity may only be assigned once.");
        Id = id;
    }

    public void ChangePriority(Priority priority)
    {
        if (!Enum.IsDefined(priority)) throw new ArgumentOutOfRangeException(nameof(priority));
        Priority = priority;
    }

    public void ChangeStatus(RequestStatus status, DateTime utcNow)
    {
        if (!Enum.IsDefined(status)) throw new ArgumentOutOfRangeException(nameof(status));
        if (Status == RequestStatus.Completed && status == RequestStatus.Open)
            throw new InvalidStatusTransitionException();
        if (Status == status) return;
        Status = status;
        CompletedAt = status == RequestStatus.Completed ? DateTime.SpecifyKind(utcNow, DateTimeKind.Utc) : null;
    }

    public void EnsureCanBeDeleted()
    {
        if (Status != RequestStatus.Open) throw new SupportRequestDeletionNotAllowedException();
    }

    private static string RequiredText(string value, int maxLength)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Length > maxLength) throw new ArgumentException("Text exceeds its maximum length.");
        return value.Trim();
    }
}
