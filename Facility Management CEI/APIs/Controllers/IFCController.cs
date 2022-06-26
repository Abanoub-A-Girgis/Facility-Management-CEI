using Facility_Management_CEI.IdentityDb;
using API.Enums;
using API.Models;
using API.Moldels_DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xbim.Common;
using Xbim.Ifc;
////////////////////////////////////////////
//using Xbim.Ifc4.BuildingControlsDomain;
//using Xbim.Ifc4.HvacDomain;
//using Xbim.Ifc4.Interfaces;
//using Xbim.Ifc4.Kernel;
//using Xbim.Ifc4.ProductExtension;
//using Xbim.Ifc4.SharedBldgElements;
//using Xbim.Ifc4.SharedBldgServiceElements;
/////////////////////////////////////////////
using Xbim.Ifc2x3.BuildingcontrolsDomain;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.SharedBldgServiceElements;
using System.Net.Http;
using System.IO;

namespace API.Controllers
{
    [Route("api/[controller]")]//the route that the request and response will take 
    [ApiController]//Controller type 
    public class IFCController : Controller
    {
        public ApplicationDBContext _Context { get; set; }
        public IFCController(ApplicationDBContext context)
        {
            _Context = context;
        }

        [Route("AddBuildingDataFromIFC")]
        [HttpPost]
        public void AddBuildingDataFromIFC(string fileName)
        {
            string FilePath = Path.Combine("wwwroot\\data", fileName);
            using (IfcStore Model = IfcStore.Open($"{FilePath}"))
            {
                //stepModel.Instances: retreiving all the elements inside our model
                var building = Model.Instances;

                #region BuildingData
                Building Mybuilding = new Building();
                Mybuilding.Id = building.OfType<IfcBuilding>().FirstOrDefault().EntityLabel;
                Mybuilding.Name = building.OfType<IfcBuilding>().FirstOrDefault().Name.Value;
                Mybuilding.Path = "data/" + fileName;
                _Context.Buildings.Add(Mybuilding);
                _Context.SaveChanges();
                #endregion

                #region FloorData

                var Floors = building.OfType<IfcBuildingStorey>();
                List<Floor> BuildingFloor = new List<Floor>();
                foreach (var Floor in Floors)
                {
                    BuildingFloor.Add(
                    new Floor()
                    {
                        Id = Floor.EntityLabel,
                        FloorName = Floor.Name.Value,
                        BuildingId = Mybuilding.Id,
                    });
                }
                _Context.Floors.AddRange(BuildingFloor);
                _Context.SaveChanges();
                #endregion

                #region SpacesData

                var Spaces = building.OfType<IfcSpace>();
                List<Space> FloorSpaces = new List<Space>();
                foreach (var Space in Spaces)
                {
                    var SpaceDecomposition = Space.Decomposes;//Return a single IfcRelAggregate (which Contains all Space Compoenets)
                    var SpaceIfcRelAggregate = SpaceDecomposition.FirstOrDefault();//get this IfcRelAggregate
                    var SpaceRelatingObject = SpaceIfcRelAggregate.RelatingObject;//we need the relating object from this space this Object return object defintion
                    var SpacesStorey = SpaceRelatingObject as IfcBuildingStorey;// Finally get the Story as we need to unbox this object to our IfcBuildingStorey
                    FloorSpaces.Add(
                    new Space()
                    {
                        Id = Space.EntityLabel,
                        Name = Space.LongName.Value,
                        FloorId = SpacesStorey.EntityLabel,
                    });
                }
                /////////////////code for the dummy floor and space 
                var FloorsIds = new List<int>();
                var EmptySpacesToBeAdded = new List<Space>();
                foreach (Floor floor in BuildingFloor)
                {
                    FloorsIds.Add(floor.Id);

                }
                for (int i = 0; i < FloorsIds.Count; i++)
                {

                    EmptySpacesToBeAdded.Add(
                    new Space()
                    {
                        Id = FloorSpaces.Max(i => i.Id) + (i + 1),
                        Name = "NaN",
                        FloorId = FloorsIds[i],
                    });
                };
                FloorSpaces.AddRange(EmptySpacesToBeAdded);
                /////////////////code for the dummy floor and space
                _Context.Spaces.AddRange(FloorSpaces);
                _Context.SaveChanges();
                #endregion

                #region AssetData
                var RelatedAssets = building.OfType<IfcRelContainedInSpatialStructure>();//getting all the assets within the Ifc file (IIFc interface is more general for Ifc Version than Ifc that will need tp specify a certain compatable formate)
                var Doors = building.OfType<IfcDoor>().ToList();//For adding the Dooors
                var Windows = building.OfType<IfcWindow>().ToList();//Forr adding the Windows
                var RelSpace = building.OfType<IfcRelSpaceBoundary>();//For adding elements that are related to a space to give it a space id
                var PipesAndDucts = building.OfType<IfcFlowSegment>();//Get Pipes & Ducts
                var Columns = building.OfType<IfcColumn>();//To get Columns 
                var Slabs = building.OfType<IfcSlab>();//To get Columns 
                var Walls = building.OfType<IfcWall>();//To get Columns 
                var beams = building.OfType<IfcBeam>();//To get Columns 
                List<Asset> AssetsList = new List<Asset>();//Our List that hass all the assets
                //Doors Loop
                foreach (var Door in Doors)
                {
                    if (Door.IsContainedIn == null)//making sure that the Door is contained in a floor some times it gives null exception
                        continue;
                    AssetsList.Add(new Asset
                    {
                        Id = Door.EntityLabel,
                        Name = $"{Door.ObjectType.Value}:{Door.Name.Value}",
                        SpaceId = Door.ProvidesBoundaries.FirstOrDefault()?.RelatingSpace.EntityLabel,
                        FloorId = Door.IsContainedIn.EntityLabel
                    });
                }
                //Windows Loop
                foreach (var Window in Windows)
                {
                    AssetsList.Add(new Asset
                    {
                        Id = Window.EntityLabel,
                        Name = $"{Window.ObjectType.Value}:{Window.Name.Value}",
                        /////////////////code for the dummy floor and space
                        SpaceId = EmptySpacesToBeAdded.FirstOrDefault(i => i.FloorId == Window.IsContainedIn.EntityLabel).Id /*null*/,
                        /////////////////code for the dummy floor and space
                        FloorId = Window.IsContainedIn.EntityLabel
                    });
                }
                //Ducts and pipe Loop
                foreach (var item in PipesAndDucts)
                {
                    if (item.IsContainedIn == null)
                        continue;
                    AssetsList.Add(new Asset
                    {
                        Id = item.EntityLabel,
                        Name = $"{item.ObjectType.Value}:{item.Name.Value}",
                        /////////////////code for the dummy floor and space
                        SpaceId = EmptySpacesToBeAdded.FirstOrDefault(i => i.FloorId == item.IsContainedIn.EntityLabel).Id /*null*/,
                        /////////////////code for the dummy floor and space
                        FloorId = item.IsContainedIn.EntityLabel
                    });
                }
                //getting Furniture located in a space 
                foreach (var Space in FloorSpaces)
                {
                    var x = RelatedAssets.Where(con => con.RelatingStructure.EntityLabel == Space.Id);//List Contains Single element
                    var SpaceRelAssets = x.FirstOrDefault();//return the space that is a relating object to the furniture    
                    if (SpaceRelAssets == null)
                        continue;

                    var RelAssetsInSpace = SpaceRelAssets.RelatedElements.ToList();//getting the furniture within our space 

                    foreach (var Asset in RelAssetsInSpace)//getting each asset in our selected space 
                    {
                        AssetsList.Add(//feedign our Asset object with the data needed for database  
                        new Asset
                        {
                            Id = Asset.EntityLabel,
                            Name = $"{Asset.ObjectType.Value} : {Asset.Name.Value}",
                            SpaceId = Space.Id,//SpaceId = Assets.IsContainedIn.EntityLabel
                            FloorId = Space.FloorId
                        });
                    }
                }
                //getting the columns 
                foreach (var column in Columns)
                {
                    if (column.IsContainedIn == null)
                        continue;
                    AssetsList.Add(new Asset
                    {
                        Id = column.EntityLabel,
                        Name = $"{column.ObjectType.Value}:{column.Name.Value}",
                        /////////////////code for the dummy floor and space
                        SpaceId = EmptySpacesToBeAdded.FirstOrDefault(i => i.FloorId == column.IsContainedIn.EntityLabel).Id /*null*/,
                        /////////////////code for the dummy floor and space
                        FloorId = column.IsContainedIn.EntityLabel
                    });
                }
                //getting the Slabs 
                foreach (var slab in Slabs)
                {
                    if (slab.IsContainedIn == null)
                        continue;
                    AssetsList.Add(new Asset
                    {
                        Id = slab.EntityLabel,
                        Name = $"{slab.ObjectType.Value}:{slab.Name.Value}",
                        /////////////////code for the dummy floor and space
                        SpaceId = EmptySpacesToBeAdded.FirstOrDefault(i => i.FloorId == slab.IsContainedIn.EntityLabel).Id /*null*/,
                        /////////////////code for the dummy floor and space
                        FloorId = slab.IsContainedIn.EntityLabel
                    });
                }
                //getting the Walls 
                foreach (var wall in Walls)
                {
                    if (wall.IsContainedIn == null)
                        continue;
                    AssetsList.Add(new Asset
                    {
                        Id = wall.EntityLabel,
                        Name = $"{wall.ObjectType.Value}:{wall.Name.Value}",
                        /////////////////code for the dummy floor and space
                        SpaceId = EmptySpacesToBeAdded.FirstOrDefault(i => i.FloorId == wall.IsContainedIn.EntityLabel).Id /*null*/,
                        /////////////////code for the dummy floor and space
                        FloorId = wall.IsContainedIn.EntityLabel
                    });
                }
                //getting the Beams
                foreach (var beam in beams)
                {
                    if (beam.IsContainedIn == null)
                        continue;
                    AssetsList.Add(new Asset
                    {
                        Id = beam.EntityLabel,
                        Name = $"{beam.ObjectType.Value}:{beam.Name.Value}",
                        /////////////////code for the dummy floor and space
                        SpaceId = EmptySpacesToBeAdded.FirstOrDefault(i => i.FloorId == beam.IsContainedIn.EntityLabel).Id /*null*/,
                        /////////////////code for the dummy floor and space
                        FloorId = beam.IsContainedIn.EntityLabel
                    });
                }

                //IFcRelSpace Boundary Elements (the elements that are connected to a physical Space) 
                //this must be the last loop as it might includes a duplicate element which is investigated in this loop
                foreach (var item in RelSpace)
                {
                    if (item.RelatedBuildingElement == null)//in case that the Boundary are virtual then it wont contain any elements
                        continue;
                    if (AssetsList.Any(ele => ele.Id == item.RelatedBuildingElement.EntityLabel))
                        continue;
                    //getting all the structural elements+ Door With spaces Id
                    AssetsList.Add(
                        new Asset
                        {
                            Id = item.RelatedBuildingElement.EntityLabel,
                            Name = $"{item.RelatedBuildingElement.ObjectType.Value} : {item.RelatedBuildingElement.Name.Value}",
                            SpaceId = item.RelatingSpace.EntityLabel,
                            FloorId = FloorSpaces.Where(sp => sp.Id == item.RelatingSpace.EntityLabel).FirstOrDefault().FloorId
                        });
                }

                _Context.Assets.AddRange(AssetsList);
                _Context.SaveChanges();
                #endregion

                #region SensorData

                var GettingSensorsOfElementProxy = building.OfType<IfcBuildingElementProxy>().Where(s => s.ObjectType.Value.ToString().Contains("ensor")).ToList();
                var GettingSensorOfIfcAlarm = building.OfType<IfcDistributionControlElement>().ToList();
                var GettingSensorOfIfcValve = building.OfType<IfcFlowController>().ToList();
                List<Sensor> sensors = new List<Sensor>();
                //1st Loop Collection
                foreach (var sensor in GettingSensorsOfElementProxy)
                {
                    if (sensor is null)//ensure that ths list Contains no values 
                        continue;
                    sensors.Add(new Sensor()
                    {
                        Id = sensor.EntityLabel,
                        Name = sensor.Name.Value,
                        SpaceId = sensor.IsContainedIn.EntityLabel,//represents the space that contains this sensor
                        SensorType = Enums.SensorType.SpaceSensor,
                        AssetId = null
                    });
                }
                //2nd Loop Collection
                foreach (var sensor in GettingSensorOfIfcAlarm)
                {
                    if (sensor is null)
                        continue;
                    sensors.Add(new Sensor()
                    {
                        Id = sensor.EntityLabel,
                        Name = sensor.Name.Value,
                        SpaceId = sensor.IsContainedIn.EntityLabel,//represents the space that contains this sensor
                        SensorType = Enums.SensorType.SpaceSensor,//Default value can be changed later
                        AssetId = null

                    });
                }
                _Context.Sensors.AddRange(sensors);
                _Context.SaveChanges();
                #endregion
            }
        }

        [Route("BuilingDTOData")]
        [HttpGet]
        public BuildingDTO BuilingDTOData()
        {
            var BuildingList = _Context.Buildings.FirstOrDefault();
            BuildingDTO buildingDTO = new BuildingDTO();
            buildingDTO.Id = BuildingList.Id;
            buildingDTO.Name = BuildingList.Name;
            buildingDTO.Path = BuildingList.Path;
            return buildingDTO;
        }
        [Route("FloorDTO")]
        [HttpGet]
        public IEnumerable<FloorDTO> FloorDTO()
        {
            var floors = _Context.Floors;
            List<FloorDTO> floorDTOs = new List<FloorDTO>();
            foreach (var floor in floors)
            {
                floorDTOs.Add(
                   new FloorDTO()
                   {
                       Id = floor.Id,
                       FloorName = floor.FloorName,
                       BuildingId = floor.BuildingId,
                   });
            }
            return floorDTOs;
        }
        [Route("SpaceDTO")]
        [HttpGet]
        public IEnumerable<SpaceDTO> SpaceDTO()
        {
            var spaces = _Context.Spaces;
            List<SpaceDTO> SpaceDTOs = new List<SpaceDTO>();
            foreach (var space in spaces)
            {
                SpaceDTOs.Add(new SpaceDTO()
                {
                    Id = space.Id,
                    Name = space.Name,
                    FloorId = space.FloorId
                });
            }
            return SpaceDTOs;
        }

        [Route("AssetDTO")]
        [HttpGet]
        public IEnumerable<AssetDTO> AssetDTO()
        {
            var Assets = _Context.Assets;
            List<AssetDTO> assetDTOs = new List<AssetDTO>();
            foreach (var asset in Assets)
            {
                assetDTOs.Add(new AssetDTO()
                {
                    Id = asset.Id,
                    Name = asset.Name,
                    SpaceId = asset.SpaceId

                });
            }
            return assetDTOs;
        }

        [Route("SensorDTO")]
        [HttpGet]
        public IEnumerable<SensorDTO> SensorDTO()
        {
            var Sensors = _Context.Sensors;
            List<SensorDTO> sensorDTOs = new List<SensorDTO>();
            foreach (var sensor in Sensors)
            {
                sensorDTOs.Add(new SensorDTO()
                {
                    Id = sensor.Id,
                    Name = sensor.Name,
                    SpaceId = sensor.SpaceId,
                    SensorType = sensor.SensorType,
                    AssetId = sensor.AssetId
                });

            }
            return sensorDTOs;
        }

        //Get the data from the DB to the user
        // we need to get List of tasks that is assigned to the incidents 
        [Route("GetTaskData")]
        [HttpGet]
        public IEnumerable<TaskDTO> GetTaskData()
        {
            List<TaskDTO> Tasks = new List<TaskDTO>();

            foreach (var Task in _Context.Tasks)
            {
                Tasks.Add(new TaskDTO()
                {
                    Type = Task.Type,
                    Description = Task.Description,
                    Status = Task.Status,
                    Priority = Task.Priority,
                    Cost = Task.Cost,
                    FixingTime = Task.FixingTime,
                    IncidentId = Task.IncidentId,
                    AssignedToId = Task.AssignedToId,
                    AssignedById = Task.AssignedById,
                    CreatedById = Task.CreatedById,
                });
            }
            return Tasks;
        }

        //Get the Data From the User To DB
        [Route("StoreTaskData")]
        [HttpPost]
        public void StoreTaskData(TaskDTO task)
        {
            Models.Task Task = new Models.Task()
            {
                Id = task.Id,
                Type = task.Type,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                Cost = task.Cost,
                FixingTime = task.FixingTime,
                IncidentId = task.IncidentId,
                AssignedToId = task.AssignedToId,
                AssignedById = task.AssignedById,
                CreatedById = task.CreatedById
            };
            _Context.Tasks.Add(Task);
            _Context.SaveChanges();
        }

        [Route("ClearDatabase")]
        [HttpDelete]
        public void ClearDatabase()
        {

            Building[] Bui = _Context.Buildings.ToArray();
            _Context.Buildings.RemoveRange(Bui);

            Floor[] Flo = _Context.Floors.ToArray();
            _Context.Floors.RemoveRange(Flo);

            Space[] Spa = _Context.Spaces.ToArray();
            _Context.Spaces.RemoveRange(Spa);

            Asset[] Asse = _Context.Assets.ToArray();
            _Context.Assets.RemoveRange(Asse);

            Sensor[] Sen = _Context.Sensors.ToArray();
            _Context.Sensors.RemoveRange(Sen);

            SensorWarning[] SenWar = _Context.SensorWarnings.ToArray();
            _Context.SensorWarnings.RemoveRange(SenWar);

            AppUser[] Use = _Context.AppUsers.ToArray();
            _Context.AppUsers.RemoveRange(Use);

            Incident[] Inc = _Context.Incidents.ToArray();
            _Context.Incidents.RemoveRange(Inc);

            Models.Task[] Tas = _Context.Tasks.ToArray();
            _Context.Tasks.RemoveRange(Tas);

            _Context.SaveChanges();
        }

    }
}
