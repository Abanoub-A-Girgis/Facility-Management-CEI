using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public TaskType Type { get; set; }
        public string Description { get; set; }
        public Enums.TaskStatus Status { get; set; }
        public Priority Priority { get; set; }
        public double Cost { get; set; }
        //[DataType(DataType.Date)]
        public DateTime FixingTime { get; set; }
        public int IncidentId { get; set; }
        public int? AssignedToId { get; set; }//test the null//need to be tested in api to make sure it add in the list
        public int? AssignedById { get; set; }//test the null//need to be tested in api to make sure it add in the list
        public int CreatedById { get; set; }//we made this for the modelbuilder to select it 
    }
}
