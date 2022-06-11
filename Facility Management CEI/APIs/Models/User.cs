using API.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class User: IdentityUser
    {
        #region Entity Properties
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public UserType Type { get; set; }
        #endregion

        #region Link with other
        public List<SensorWarning> SensorWarnings { get; set; }
        public int? BuildingId { get; set; } //new//may may user one to many can be null
        public Building Building { get; set; }//if we put a FK we will have a error
        //public int BuildingId { get; set; }//if we put a FK we will have a error
        public List<Incident> Incidents { get; set; }
        ////[InverseProperty("CreatedBy")] //we made this with fluent api look AplicationDBContext 
        public List<Models.Task> TasksCreated { get; set; }
        [InverseProperty("AssignedBy")]
        public List<Models.Task> TasksAssigned { get; set; }
        [InverseProperty("AssignedTo")]
        public List<Models.Task> TasksReceived { get; set; }
        // represents the Unary RelationShip
        public int? SuperId { get; set; }//user SuperVisor Id
        public User Super { get; set; } // supervisor object
        [InverseProperty("Super")]//new//will not make anny difference if we remove it 
        public List<User> SuperUnder { get; set; } // supervised personal 
        #endregion
    }
}
