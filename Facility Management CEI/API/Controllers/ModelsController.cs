using API.Enums;
using Facility_Management_CEI.ApiLayer.Dtos;
using Facility_Management_CEI.IdentityDb;
using Facility_Management_CEI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Facility_Management_CEI.ApiLayer.Controllers
{
    [ApiController]
    [Route("api/models")]
    public class ModelsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IIfcProcessingService _ifcProcessingService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<ModelsController> _logger;

        public ModelsController(
            ApplicationDBContext context,
            IIfcProcessingService ifcProcessingService,
            IStringLocalizer<SharedResource> localizer,
            ILogger<ModelsController> logger)
        {
            _context = context;
            _ifcProcessingService = ifcProcessingService;
            _localizer = localizer;
            _logger = logger;
        }

        [HttpPost("upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        [ProducesResponseType(typeof(ApiResponse<ModelUploadResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _ifcProcessingService.SaveSplitAndConvertAsync(file, cancellationToken);
                new global::API.Controllers.IFCController(_context).AddBuildingDataFromIFC(
                    result.FileName,
                    result.GeneratedWexBimPaths,
                    Path.GetDirectoryName(result.FullPath));

                var model = await _context.Buildings
                    .OrderByDescending(b => b.Id)
                    .FirstOrDefaultAsync(b => b.Path == "data/" + result.FileName, cancellationToken);

                var response = new ModelUploadResponseDto
                {
                    ModelId = model?.Id,
                    FileName = result.FileName,
                    RelativePath = result.RelativePath,
                    GeneratedIfcFiles = result.GeneratedIfcFiles,
                    GeneratedWexBimPaths = result.GeneratedWexBimPaths
                };

                return CreatedAtAction(nameof(GetById), new { modelId = model?.Id }, ApiResponse<ModelUploadResponseDto>.Ok(response, _localizer["Server.UploadProcessed"]));
            }
            catch (InvalidDataException ex)
            {
                _logger.LogWarning(ex, "IFC upload validation failed.");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IFC upload processing failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(_localizer["Server.UploadFailed"]));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BuildingModelDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var models = await _context.Buildings
                .Select(b => new BuildingModelDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Path = b.Path,
                    FloorCount = b.Floors.Count,
                    ElementCount = _context.Assets.Count(a => a.Floor.BuildingId == b.Id) + _context.Spaces.Count(s => s.Floor.BuildingId == b.Id),
                    OpenIssueCount = _context.Incidents.Count(i => i.Status != IncidentStatus.Completed && i.Space.Floor.BuildingId == b.Id)
                })
                .ToListAsync(cancellationToken);

            return Ok(ApiResponse<IReadOnlyList<BuildingModelDto>>.Ok(models));
        }

        [HttpGet("{modelId:int}")]
        [ProducesResponseType(typeof(ApiResponse<BuildingModelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int modelId, CancellationToken cancellationToken)
        {
            var model = await _context.Buildings
                .Where(b => b.Id == modelId)
                .Select(b => new BuildingModelDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Path = b.Path,
                    FloorCount = b.Floors.Count,
                    ElementCount = _context.Assets.Count(a => a.Floor.BuildingId == b.Id) + _context.Spaces.Count(s => s.Floor.BuildingId == b.Id),
                    OpenIssueCount = _context.Incidents.Count(i => i.Status != IncidentStatus.Completed && i.Space.Floor.BuildingId == b.Id)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                return NotFound(ApiResponse<object>.Fail("Model was not found."));
            }

            return Ok(ApiResponse<BuildingModelDto>.Ok(model));
        }

        [HttpGet("{modelId:int}/floors")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<FloorDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFloors(int modelId, CancellationToken cancellationToken)
        {
            var floors = await _context.Floors
                .Where(f => f.BuildingId == modelId)
                .OrderBy(f => f.Path)
                .Select(f => new FloorDto
                {
                    Id = f.Id,
                    ModelId = f.BuildingId,
                    Name = f.FloorName,
                    WexBimPath = f.Path
                })
                .ToListAsync(cancellationToken);

            return Ok(ApiResponse<IReadOnlyList<FloorDto>>.Ok(floors));
        }

        [HttpGet("{modelId:int}/elements")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ElementDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetElements(int modelId, CancellationToken cancellationToken)
        {
            var elements = await ElementsQuery(modelId, null).ToListAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<ElementDto>>.Ok(elements));
        }

        [HttpGet("{modelId:int}/floors/{floorId:int}/elements")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ElementDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFloorElements(int modelId, int floorId, CancellationToken cancellationToken)
        {
            var elements = await ElementsQuery(modelId, floorId).ToListAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<ElementDto>>.Ok(elements));
        }

        [HttpGet("{modelId:int}/viewer-highlights")]
        [ProducesResponseType(typeof(ApiResponse<ViewerHighlightDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetViewerHighlights(int modelId, CancellationToken cancellationToken)
        {
            var issues = await _context.Incidents
                .Where(i => i.Status != IncidentStatus.Completed && i.Space.Floor.BuildingId == modelId)
                .Select(i => new { i.AssetId, i.SpaceId, i.Priority })
                .ToListAsync(cancellationToken);

            var result = new ViewerHighlightDto();
            foreach (var issue in issues)
            {
                if (issue.AssetId == null)
                {
                    result.Rooms.Add(issue.SpaceId);
                    continue;
                }

                switch (issue.Priority)
                {
                    case Priority.Severe:
                        result.Severe.Add(issue.AssetId.Value);
                        break;
                    case Priority.High:
                        result.High.Add(issue.AssetId.Value);
                        break;
                    case Priority.Medium:
                        result.Medium.Add(issue.AssetId.Value);
                        break;
                    case Priority.Low:
                        result.Low.Add(issue.AssetId.Value);
                        break;
                }
            }

            return Ok(ApiResponse<ViewerHighlightDto>.Ok(result));
        }

        private IQueryable<ElementDto> ElementsQuery(int modelId, int? floorId)
        {
            var assets = _context.Assets
                .Where(a => a.Floor.BuildingId == modelId && (floorId == null || a.FloorId == floorId.Value))
                .Select(a => new ElementDto
                {
                    Id = a.Id,
                    ElementType = "Asset",
                    Name = a.Name,
                    Materials = a.Materials,
                    FloorId = a.FloorId,
                    FloorName = a.Floor.FloorName,
                    SpaceId = a.SpaceId,
                    SpaceName = a.Space != null ? a.Space.Name : null,
                    OpenIssueCount = a.Incident.Count(i => i.Status != IncidentStatus.Completed)
                });

            var spaces = _context.Spaces
                .Where(s => s.Floor.BuildingId == modelId && (floorId == null || s.FloorId == floorId.Value))
                .Select(s => new ElementDto
                {
                    Id = s.Id,
                    ElementType = "Space",
                    Name = s.Name,
                    Materials = null,
                    FloorId = s.FloorId,
                    FloorName = s.Floor.FloorName,
                    SpaceId = s.Id,
                    SpaceName = s.Name,
                    OpenIssueCount = _context.Incidents.Count(i => i.AssetId == null && i.SpaceId == s.Id && i.Status != IncidentStatus.Completed)
                });

            return assets.Concat(spaces).OrderBy(e => e.FloorName).ThenBy(e => e.Name);
        }
    }
}
