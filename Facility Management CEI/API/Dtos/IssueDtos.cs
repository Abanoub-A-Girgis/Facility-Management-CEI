using System;
using System.ComponentModel.DataAnnotations;
using API.Enums;

namespace Facility_Management_CEI.ApiLayer.Dtos
{
    public class IssueDto
    {
        public int Id { get; set; }
        public IncidentStatus Status { get; set; }
        public Priority Severity { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public DateTime ReportingTime { get; set; }
        public int? ElementId { get; set; }
        public string ElementName { get; set; }
        public int SpaceId { get; set; }
        public string SpaceName { get; set; }
        public int ReporterId { get; set; }
        public string ReporterName { get; set; }
        public int? AssignedWorkerId { get; set; }
        public string AssignedWorkerName { get; set; }
    }

    public class CreateIssueRequest
    {
        [Required]
        [StringLength(2000, MinimumLength = 3)]
        public string Description { get; set; }

        [Required]
        public Priority Severity { get; set; }

        [Required]
        public int ReporterId { get; set; }

        public int? SpaceId { get; set; }

        [StringLength(2000)]
        public string Comment { get; set; }
    }

    public class UpdateIssueRequest
    {
        [Required]
        [StringLength(2000, MinimumLength = 3)]
        public string Description { get; set; }

        [Required]
        public Priority Severity { get; set; }

        [Required]
        public IncidentStatus Status { get; set; }

        [StringLength(2000)]
        public string Comment { get; set; }
    }

    public class AssignWorkerRequest
    {
        [Required]
        public int WorkerId { get; set; }

        public int? AssignedById { get; set; }
    }

    public class WorkerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public int? BuildingId { get; set; }
    }

    public class LookupValueDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
