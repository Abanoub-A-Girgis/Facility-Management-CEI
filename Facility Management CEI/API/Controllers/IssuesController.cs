using API.Enums;
using API.Models;
using Facility_Management_CEI.ApiLayer.Dtos;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainTask = API.Models.Task;

namespace Facility_Management_CEI.ApiLayer.Controllers
{
    [ApiController]
    [Route("api/issues")]
    public class IssuesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<IssuesController> _logger;

        public IssuesController(ApplicationDBContext context, ILogger<IssuesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("severities")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LookupValueDto>>), StatusCodes.Status200OK)]
        public IActionResult GetSeverities()
        {
            return Ok(ApiResponse<IReadOnlyList<LookupValueDto>>.Ok(EnumValues<Priority>()));
        }

        [HttpGet("statuses")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LookupValueDto>>), StatusCodes.Status200OK)]
        public IActionResult GetStatuses()
        {
            return Ok(ApiResponse<IReadOnlyList<LookupValueDto>>.Ok(EnumValues<IncidentStatus>()));
        }

        [HttpPut("{issueId:int}")]
        [ProducesResponseType(typeof(ApiResponse<IssueDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int issueId, UpdateIssueRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Issue request is invalid."));
            }

            var issue = await _context.Incidents.FirstOrDefaultAsync(i => i.Id == issueId, cancellationToken);
            if (issue == null)
            {
                return NotFound(ApiResponse<object>.Fail("Issue was not found."));
            }

            issue.Description = request.Description;
            issue.Priority = request.Severity;
            issue.Status = request.Status;
            issue.Comment = request.Comment;

            var tasks = await _context.Tasks.Where(t => t.IncidentId == issueId).ToListAsync(cancellationToken);
            foreach (var task in tasks)
            {
                task.Priority = request.Severity;
                if (request.Status == IncidentStatus.Completed)
                {
                    task.Status = API.Enums.TaskStatus.Completed;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            var dto = await ProjectIssue(_context.Incidents.Where(i => i.Id == issueId)).FirstAsync(cancellationToken);
            return Ok(ApiResponse<IssueDto>.Ok(dto, "Issue updated."));
        }

        [HttpDelete("{issueId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int issueId, CancellationToken cancellationToken)
        {
            var issue = await _context.Incidents.FirstOrDefaultAsync(i => i.Id == issueId, cancellationToken);
            if (issue == null)
            {
                return NotFound(ApiResponse<object>.Fail("Issue was not found."));
            }

            var tasks = await _context.Tasks.Where(t => t.IncidentId == issueId).ToListAsync(cancellationToken);
            _context.Tasks.RemoveRange(tasks);
            _context.Incidents.Remove(issue);
            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpPut("{issueId:int}/assign-worker")]
        [ProducesResponseType(typeof(ApiResponse<IssueDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignWorker(int issueId, AssignWorkerRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Assignment request is invalid."));
            }

            var issue = await _context.Incidents.FirstOrDefaultAsync(i => i.Id == issueId, cancellationToken);
            if (issue == null)
            {
                return NotFound(ApiResponse<object>.Fail("Issue was not found."));
            }

            var worker = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == request.WorkerId && u.Type == UserType.Agent, cancellationToken);
            if (worker == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Worker was not found or is not an agent."));
            }

            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.IncidentId == issueId, cancellationToken);
            if (task == null)
            {
                task = new DomainTask
                {
                    IncidentId = issue.Id,
                    CreatedById = request.AssignedById ?? issue.AppUserId,
                    Description = issue.Description,
                    Priority = issue.Priority,
                    Type = TaskType.Other
                };
                _context.Tasks.Add(task);
            }

            task.AssignedToId = request.WorkerId;
            task.AssignedById = request.AssignedById;
            task.Status = API.Enums.TaskStatus.WorkInProgress;
            task.Priority = issue.Priority;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Assigned issue {IssueId} to worker {WorkerId}", issueId, request.WorkerId);

            var dto = await ProjectIssue(_context.Incidents.Where(i => i.Id == issueId)).FirstAsync(cancellationToken);
            return Ok(ApiResponse<IssueDto>.Ok(dto, "Issue assigned to worker."));
        }

        public static IQueryable<IssueDto> ProjectIssue(IQueryable<Incident> query)
        {
            return query.Select(i => new IssueDto
            {
                Id = i.Id,
                Status = i.Status,
                Severity = i.Priority,
                Description = i.Description,
                Comment = i.Comment,
                ReportingTime = i.ReportingTime,
                ElementId = i.AssetId,
                ElementName = i.Asset != null ? i.Asset.Name : i.Space.Name,
                SpaceId = i.SpaceId,
                SpaceName = i.Space.Name,
                ReporterId = i.AppUserId,
                ReporterName = i.AppUser.FirstName + " " + i.AppUser.LastName,
                AssignedWorkerId = i.Task.Select(t => t.AssignedToId).FirstOrDefault(),
                AssignedWorkerName = i.Task.Select(t => t.AssignedTo.FirstName + " " + t.AssignedTo.LastName).FirstOrDefault()
            });
        }

        private static IReadOnlyList<LookupValueDto> EnumValues<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(value => new LookupValueDto
                {
                    Id = Convert.ToInt32(value),
                    Name = value.ToString()
                })
                .ToList();
        }
    }
}
