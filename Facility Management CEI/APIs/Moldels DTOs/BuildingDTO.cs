﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class BuildingDTO
    {
        #region EntityProperties
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        #endregion

        #region Link with other entities
       
        #endregion
    }
}
