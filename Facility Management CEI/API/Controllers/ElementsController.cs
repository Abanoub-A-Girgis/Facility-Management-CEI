using Facility_Management_CEI.ApiLayer.Dtos;
using Facility_Management_CEI.IdentityDb;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Facility_Management_CEI.ApiLayer.Controllers
{
    [ApiController]
    [Route("api/elements")]
    public class ElementsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ElementsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("{elementId:int}")]
        [ProducesResponseType(typeof(ApiResponse<ElementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int elementId, CancellationToken cancellationToken)
        {
            var asset = await _context.Assets
                .Where(a => a.Id == elementId)
                .Select(a => new ElementDto
                {
                    Id = a.Id,
                    ViewerId = a.IfcEntityLabel,
                    ElementKey = "Asset:" + a.Id,
                    ElementType = "Asset",
                    Name = a.Name,
                    Materials = a.Materials,
                    FloorId = a.FloorId,
                    FloorName = a.Floor.FloorName,
                    SpaceId = a.SpaceId,
                    SpaceName = a.Space != null ? a.Space.Name : null,
                    OpenIssueCount = a.Incident.Count(i => i.Status != API.Enums.IncidentStatus.Completed)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (asset != null)
            {
                return Ok(ApiResponse<ElementDto>.Ok(asset));
            }

            var space = await _context.Spaces
                .Where(s => s.Id == elementId)
                .Select(s => new ElementDto
                {
                    Id = s.Id,
                    ViewerId = s.IfcEntityLabel,
                    ElementKey = "Space:" + s.Id,
                    ElementType = "Space",
                    Name = s.Name,
                    Materials = null,
                    FloorId = s.FloorId,
                    FloorName = s.Floor.FloorName,
                    SpaceId = s.Id,
                    SpaceName = s.Name,
                    OpenIssueCount = _context.Incidents.Count(i => i.AssetId == null && i.SpaceId == s.Id && i.Status != API.Enums.IncidentStatus.Completed)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (space == null)
            {
                return NotFound(ApiResponse<object>.Fail("Element was not found."));
            }

            return Ok(ApiResponse<ElementDto>.Ok(space));
        }

        [HttpGet("{elementId:int}/issues")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<IssueDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIssues(int elementId, CancellationToken cancellationToken)
        {
            var issues = await _context.Incidents
                .Where(i => i.AssetId == elementId || (i.AssetId == null && i.SpaceId == elementId))
                .Select(i => new IssueDto
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
                })
                .ToListAsync(cancellationToken);

            return Ok(ApiResponse<IReadOnlyList<IssueDto>>.Ok(issues));
        }

        [HttpPost("{elementId:int}/issues")]
        [ProducesResponseType(typeof(ApiResponse<IssueDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateIssue(int elementId, CreateIssueRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Issue request is invalid."));
            }

            var reporterExists = await _context.AppUsers.AnyAsync(u => u.Id == request.ReporterId, cancellationToken);
            if (!reporterExists)
            {
                return BadRequest(ApiResponse<object>.Fail("Reporter was not found."));
            }

            int? assetId = null;
            int spaceId;

            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == elementId, cancellationToken);
            if (asset != null)
            {
                assetId = asset.Id;
                if (asset.SpaceId == null && request.SpaceId == null)
                {
                    return BadRequest(ApiResponse<object>.Fail("A space id is required for assets without a stored space."));
                }

                spaceId = asset.SpaceId ?? request.SpaceId.Value;
            }
            else
            {
                var space = await _context.Spaces.FirstOrDefaultAsync(s => s.Id == elementId, cancellationToken);
                if (space == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Element was not found."));
                }

                spaceId = space.Id;
            }

            var issue = new Incident
            {
                AssetId = assetId,
                SpaceId = spaceId,
                AppUserId = request.ReporterId,
                Priority = request.Severity,
                Description = request.Description,
                Comment = request.Comment,
                Status = API.Enums.IncidentStatus.Open,
                ReportingTime = System.DateTime.Now
            };

            _context.Incidents.Add(issue);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = await IssuesController.ProjectIssue(_context.Incidents.Where(i => i.Id == issue.Id))
                .FirstAsync(cancellationToken);

            return Created($"/api/elements/{elementId}/issues/{issue.Id}", ApiResponse<IssueDto>.Ok(dto, "Issue created."));
        }
    }
}
