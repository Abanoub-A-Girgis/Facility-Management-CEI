using API.Enums;
using API.Models;
using Facility_Management_CEI.ApiLayer.Dtos;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Facility_Management_CEI.ApiLayer.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/operations")]
    public class OperationsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<LogUser> _userManager;

        public OperationsController(ApplicationDBContext context, UserManager<LogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
        {
            return Ok(ApiResponse<DashboardDto>.Ok(await BuildSummary(cancellationToken)));
        }

        [Authorize(Roles = "AccountManager,Supervisor,Manager,Inspector")]
        [HttpGet("incidents")]
        public async Task<IActionResult> Incidents(CancellationToken cancellationToken)
        {
            var incidents = await IssuesController.ProjectIssue(_context.Incidents)
                .OrderByDescending(issue => issue.ReportingTime)
                .ToListAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<IssueDto>>.Ok(incidents));
        }

        [Authorize(Roles = "AccountManager,Supervisor,Manager,Inspector,Agent")]
        [HttpGet("work-orders")]
        public async Task<IActionResult> WorkOrders(CancellationToken cancellationToken)
        {
            var tasks = await _context.Tasks
                .AsNoTracking()
                .Include(task => task.AssignedTo)
                .Include(task => task.CreatedBy)
                .OrderByDescending(task => task.Id)
                .ToListAsync(cancellationToken);
            var orders = tasks.Select(task => new WorkOrderDto
                {
                    Id = task.Id,
                    Type = task.Type.ToString(),
                    Status = task.Status.ToString(),
                    Priority = task.Priority.ToString(),
                    Description = task.Description,
                    IncidentId = task.IncidentId,
                    AssignedTo = task.AssignedTo == null ? null : task.AssignedTo.FirstName + " " + task.AssignedTo.LastName,
                    CreatedBy = task.CreatedBy.FirstName + " " + task.CreatedBy.LastName,
                    FixingTime = task.FixingTime,
                    Cost = task.Cost
                })
                .ToList();
            return Ok(ApiResponse<IReadOnlyList<WorkOrderDto>>.Ok(orders));
        }

        [Authorize(Roles = "AccountManager,Supervisor,Manager,Inspector")]
        [HttpGet("sensor-warnings")]
        public async Task<IActionResult> SensorWarnings(CancellationToken cancellationToken)
        {
            var warningEntities = await _context.SensorWarnings
                .AsNoTracking()
                .Include(warning => warning.Sensor)
                    .ThenInclude(sensor => sensor.Space)
                .Include(warning => warning.AppUser)
                .OrderByDescending(warning => warning.IssueDate)
                .ToListAsync(cancellationToken);
            var warnings = warningEntities.Select(warning => new SensorWarningDto
                {
                    Id = warning.Id,
                    IssueDate = warning.IssueDate,
                    Description = warning.Description,
                    Investigated = warning.Investigated,
                    Priority = warning.Priority.ToString(),
                    SensorName = warning.Sensor.Name,
                    SpaceName = warning.Sensor.Space.Name,
                    AssignedUser = warning.AppUser == null ? null : warning.AppUser.FirstName + " " + warning.AppUser.LastName
                })
                .ToList();
            return Ok(ApiResponse<IReadOnlyList<SensorWarningDto>>.Ok(warnings));
        }

        [Authorize(Roles = "AccountManager,Owner")]
        [HttpGet("buildings")]
        public async Task<IActionResult> Buildings(CancellationToken cancellationToken)
        {
            var buildings = await _context.Buildings
                .OrderBy(building => building.Name)
                .Select(building => new BuildingInformationDto
                {
                    Id = building.Id,
                    Name = building.Name,
                    Path = building.Path,
                    FloorCount = building.Floors.Count,
                    SpaceCount = building.Floors.SelectMany(floor => floor.Spaces).Count(),
                    AssetCount = _context.Assets.Count(asset => asset.Floor.BuildingId == building.Id),
                    SensorCount = _context.Sensors.Count(sensor => sensor.Space.Floor.BuildingId == building.Id),
                    OpenIssueCount = _context.Incidents.Count(issue =>
                        issue.Space.Floor.BuildingId == building.Id && issue.Status != IncidentStatus.Completed)
                })
                .ToListAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<BuildingInformationDto>>.Ok(buildings));
        }

        [Authorize(Roles = "AccountManager,Owner,Manager")]
        [HttpGet("report")]
        public async Task<IActionResult> Report(CancellationToken cancellationToken)
        {
            var payload = new OperationsPayloadDto
            {
                Summary = await BuildSummary(cancellationToken),
                Incidents = await IssuesController.ProjectIssue(_context.Incidents)
                    .OrderByDescending(issue => issue.ReportingTime)
                    .Take(10)
                    .ToListAsync(cancellationToken),
                WorkOrders = (await _context.Tasks
                    .AsNoTracking()
                    .Include(task => task.AssignedTo)
                    .Include(task => task.CreatedBy)
                    .OrderByDescending(task => task.Id)
                    .Take(10)
                    .ToListAsync(cancellationToken))
                    .Select(task => new WorkOrderDto
                    {
                        Id = task.Id,
                        Type = task.Type.ToString(),
                        Status = task.Status.ToString(),
                        Priority = task.Priority.ToString(),
                        Description = task.Description,
                        IncidentId = task.IncidentId,
                        AssignedTo = task.AssignedTo == null ? null : task.AssignedTo.FirstName + " " + task.AssignedTo.LastName,
                        CreatedBy = task.CreatedBy.FirstName + " " + task.CreatedBy.LastName,
                        FixingTime = task.FixingTime,
                        Cost = task.Cost
                    })
                    .ToList(),
                SensorWarnings = (await _context.SensorWarnings
                    .AsNoTracking()
                    .Include(warning => warning.Sensor)
                        .ThenInclude(sensor => sensor.Space)
                    .Include(warning => warning.AppUser)
                    .OrderByDescending(warning => warning.IssueDate)
                    .Take(10)
                    .ToListAsync(cancellationToken))
                    .Select(warning => new SensorWarningDto
                    {
                        Id = warning.Id,
                        IssueDate = warning.IssueDate,
                        Description = warning.Description,
                        Investigated = warning.Investigated,
                        Priority = warning.Priority.ToString(),
                        SensorName = warning.Sensor.Name,
                        SpaceName = warning.Sensor.Space.Name,
                        AssignedUser = warning.AppUser == null ? null : warning.AppUser.FirstName + " " + warning.AppUser.LastName
                    })
                    .ToList(),
                Buildings = await _context.Buildings.Select(building => new BuildingInformationDto
                {
                    Id = building.Id,
                    Name = building.Name,
                    Path = building.Path,
                    FloorCount = building.Floors.Count,
                    SpaceCount = building.Floors.SelectMany(floor => floor.Spaces).Count(),
                    AssetCount = _context.Assets.Count(asset => asset.Floor.BuildingId == building.Id),
                    SensorCount = _context.Sensors.Count(sensor => sensor.Space.Floor.BuildingId == building.Id),
                    OpenIssueCount = _context.Incidents.Count(issue =>
                        issue.Space.Floor.BuildingId == building.Id && issue.Status != IncidentStatus.Completed)
                }).ToListAsync(cancellationToken)
            };
            return Ok(ApiResponse<OperationsPayloadDto>.Ok(payload));
        }

        [Authorize(Roles = "AccountManager")]
        [HttpGet("users")]
        public async Task<IActionResult> Users(CancellationToken cancellationToken)
        {
            var userEntities = await _context.AppUsers
                .AsNoTracking()
                .Include(user => user.LogUser)
                .OrderBy(user => user.FirstName)
                .ToListAsync(cancellationToken);
            var users = userEntities.Select(user => new UserListItemDto
                {
                    AppUserId = user.Id,
                    IdentityUserId = user.LogUserId,
                    UserName = user.LogUser.UserName,
                    FullName = user.FirstName + " " + user.LastName,
                    Role = user.Type.ToString(),
                    BuildingId = user.BuildingId
                })
                .ToList();
            return Ok(ApiResponse<IReadOnlyList<UserListItemDto>>.Ok(users));
        }

        [Authorize(Roles = "AccountManager")]
        [HttpPost("users")]
        [ProducesResponseType(typeof(ApiResponse<UserListItemDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser(CreateUserRequestDto request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<UserType>(request.Role, true, out var userType))
            {
                return BadRequest(ApiResponse<object>.Fail("The selected role is invalid."));
            }

            if (await _userManager.FindByNameAsync(request.UserName) != null)
            {
                return Conflict(ApiResponse<object>.Fail("A user with this username already exists."));
            }

            if (request.BuildingId.HasValue &&
                !await _context.Buildings.AnyAsync(building => building.Id == request.BuildingId.Value, cancellationToken))
            {
                return BadRequest(ApiResponse<object>.Fail("The selected building does not exist."));
            }

            var identityUser = new LogUser
            {
                UserName = request.UserName.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim()
            };
            var createResult = await _userManager.CreateAsync(identityUser, request.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(ApiResponse<object>.Fail(string.Join(" ", createResult.Errors.Select(error => error.Description))));
            }

            try
            {
                await _userManager.AddToRoleAsync(identityUser, userType.ToString());
                var appUser = new AppUser
                {
                    FirstName = identityUser.FirstName,
                    LastName = identityUser.LastName,
                    Type = userType,
                    BuildingId = request.BuildingId,
                    SuperId = request.SupervisorId,
                    LogUserId = identityUser.Id
                };
                _context.AppUsers.Add(appUser);
                await _context.SaveChangesAsync(cancellationToken);

                var response = new UserListItemDto
                {
                    AppUserId = appUser.Id,
                    IdentityUserId = identityUser.Id,
                    UserName = identityUser.UserName,
                    FullName = appUser.FirstName + " " + appUser.LastName,
                    Role = appUser.Type.ToString(),
                    BuildingId = appUser.BuildingId
                };
                return Created("/api/operations/users/" + appUser.Id, ApiResponse<UserListItemDto>.Ok(response, "User created."));
            }
            catch
            {
                await _userManager.DeleteAsync(identityUser);
                throw;
            }
        }

        private async Task<DashboardDto> BuildSummary(CancellationToken cancellationToken)
        {
            return new DashboardDto
            {
                ModelCount = await _context.Buildings.CountAsync(cancellationToken),
                FloorCount = await _context.Floors.CountAsync(cancellationToken),
                ElementCount = await _context.Assets.CountAsync(cancellationToken) + await _context.Spaces.CountAsync(cancellationToken),
                OpenIssueCount = await _context.Incidents.CountAsync(issue => issue.Status != IncidentStatus.Completed, cancellationToken),
                WorkerCount = await _context.AppUsers.CountAsync(user => user.Type == UserType.Agent, cancellationToken),
                OpenWorkOrderCount = await _context.Tasks.CountAsync(task => task.Status != API.Enums.TaskStatus.Completed, cancellationToken),
                UninvestigatedWarningCount = await _context.SensorWarnings.CountAsync(warning => !warning.Investigated, cancellationToken)
            };
        }
    }
}
