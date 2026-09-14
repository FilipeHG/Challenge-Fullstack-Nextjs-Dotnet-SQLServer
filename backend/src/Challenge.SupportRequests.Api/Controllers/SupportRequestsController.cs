using System.ComponentModel.DataAnnotations;
using Challenge.SupportRequests.Application.Common.Models;
using Challenge.SupportRequests.Application.SupportRequests;
using Challenge.SupportRequests.Application.SupportRequests.Create;
using Challenge.SupportRequests.Application.SupportRequests.Delete;
using Challenge.SupportRequests.Application.SupportRequests.GetById;
using Challenge.SupportRequests.Application.SupportRequests.List;
using Challenge.SupportRequests.Application.SupportRequests.UpdatePriority;
using Challenge.SupportRequests.Application.SupportRequests.UpdateStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.SupportRequests.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/support-requests")]
[ProducesResponseType<ProblemDetails>(400)]
[ProducesResponseType<ProblemDetails>(401)]
[ProducesResponseType<ProblemDetails>(500)]
public sealed class SupportRequestsController(CreateSupportRequestUseCase createUseCase, GetSupportRequestByIdUseCase getUseCase,
    ListSupportRequestsUseCase listUseCase, UpdateSupportRequestPriorityUseCase priorityUseCase,
    UpdateSupportRequestStatusUseCase statusUseCase, DeleteSupportRequestUseCase deleteUseCase) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a support request")]
    [EndpointDescription("Creates an open request. Creation and completion timestamps are controlled by the server.")]
    [ProducesResponseType<SupportRequestResponse>(201)]
    public async Task<ActionResult<SupportRequestResponse>> CreateAsync(CreateSupportRequestRequest input,
        CancellationToken cancellationToken)
    {
        var result = await createUseCase.ExecuteAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get a support request")]
    [EndpointDescription("Returns the request identified by its positive numeric ID.")]
    [ProducesResponseType<SupportRequestResponse>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    public async Task<ActionResult<SupportRequestResponse>> GetByIdAsync([Range(1, long.MaxValue)] long id,
        CancellationToken cancellationToken)
        => Ok(await getUseCase.ExecuteAsync(id, cancellationToken));

    [HttpGet]
    [EndpointSummary("List support requests")]
    [EndpointDescription("Filters by status/priority and searches title, description and requester. Defaults: page 1, limit 100. Newest first, then descending ID.")]
    [ProducesResponseType<PagedResult<SupportRequestResponse>>(200)]
    public async Task<ActionResult<PagedResult<SupportRequestResponse>>> ListAsync([FromQuery] ListSupportRequestsRequest input,
        CancellationToken cancellationToken)
        => Ok(await listUseCase.ExecuteAsync(input, cancellationToken));

    [HttpPatch("{id}/priority")]
    [EndpointSummary("Update priority")]
    [EndpointDescription("Accepts low, medium or high and returns the updated request.")]
    [ProducesResponseType<SupportRequestResponse>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    public async Task<ActionResult<SupportRequestResponse>> UpdatePriorityAsync([Range(1, long.MaxValue)] long id,
        UpdateSupportRequestPriorityRequest input, CancellationToken cancellationToken)
        => Ok(await priorityUseCase.ExecuteAsync(id, input, cancellationToken));

    [HttpPatch("{id}/status")]
    [EndpointSummary("Update status")]
    [EndpointDescription("Completion sets completedAt. Completed to open is forbidden; completed to inProgress clears completedAt.")]
    [ProducesResponseType<SupportRequestResponse>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(409)]
    public async Task<ActionResult<SupportRequestResponse>> UpdateStatusAsync([Range(1, long.MaxValue)] long id,
        UpdateSupportRequestStatusRequest input, CancellationToken cancellationToken)
        => Ok(await statusUseCase.ExecuteAsync(id, input, cancellationToken));

    [HttpDelete("{id}")]
    [EndpointSummary("Delete an open request")]
    [EndpointDescription("Only requests whose current status is open can be deleted.")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(409)]
    public async Task<IActionResult> DeleteAsync([Range(1, long.MaxValue)] long id,
        CancellationToken cancellationToken)
    {
        await deleteUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
