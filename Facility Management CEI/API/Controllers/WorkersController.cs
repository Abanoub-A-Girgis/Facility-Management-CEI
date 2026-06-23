using API.Enums;
using Facility_Management_CEI.ApiLayer.Dtos;
using Facility_Management_CEI.IdentityDb;
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
    [Route("api/workers")]
    public class WorkersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public WorkersController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<WorkerDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int? buildingId, CancellationToken cancellationToken)
        {
            var workers = await _context.AppUsers
                .Where(u => u.Type == UserType.Agent && (buildingId == null || u.BuildingId == buildingId.Value))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new WorkerDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FirstName + " " + u.LastName,
                    Type = u.Type.ToString(),
                    BuildingId = u.BuildingId
                })
                .ToListAsync(cancellationToken);

            return Ok(ApiResponse<IReadOnlyList<WorkerDto>>.Ok(workers));
        }
    }
}
