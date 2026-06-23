using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Facility_Management_CEI.Services
{
    public interface IIfcProcessingService
    {
        Task<IfcProcessingResult> SaveSplitAndConvertAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}
