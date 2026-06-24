using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Facility_Management_CEI.ApiLayer.Dtos
{
    public class DashboardDto
    {
        public int ModelCount { get; set; }
        public int FloorCount { get; set; }
        public int ElementCount { get; set; }
        public int OpenIssueCount { get; set; }
        public int WorkerCount { get; set; }
        public int OpenWorkOrderCount { get; set; }
        public int UninvestigatedWarningCount { get; set; }
    }

    public class WorkOrderDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
        public int IncidentId { get; set; }
        public string AssignedTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? FixingTime { get; set; }
        public double? Cost { get; set; }
    }

    public class SensorWarningDto
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public string Description { get; set; }
        public bool Investigated { get; set; }
        public string Priority { get; set; }
        public string SensorName { get; set; }
        public string SpaceName { get; set; }
        public string AssignedUser { get; set; }
    }

    public class BuildingInformationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int FloorCount { get; set; }
        public int SpaceCount { get; set; }
        public int AssetCount { get; set; }
        public int SensorCount { get; set; }
        public int OpenIssueCount { get; set; }
    }

    public class UserListItemDto
    {
        public int? AppUserId { get; set; }
        public string IdentityUserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public int? BuildingId { get; set; }
    }

    public class CreateUserRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }

        public int? BuildingId { get; set; }
        public int? SupervisorId { get; set; }
    }

    public class OperationsPayloadDto
    {
        public DashboardDto Summary { get; set; }
        public IReadOnlyList<IssueDto> Incidents { get; set; }
        public IReadOnlyList<WorkOrderDto> WorkOrders { get; set; }
        public IReadOnlyList<SensorWarningDto> SensorWarnings { get; set; }
        public IReadOnlyList<BuildingInformationDto> Buildings { get; set; }
    }
}
