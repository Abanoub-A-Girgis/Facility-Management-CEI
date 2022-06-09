using API.DB;
using API.Models;
using API.Moldels_DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.BuildingControlsDomain;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.Ifc4.SharedBldgServiceElements;

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
        public void AddBuildingDataFromIFC(string FilePath)
        {
            using (IfcStore Model = IfcStore.Open($"{FilePath}"))
            {
                //stepModel.Instances: retreiving all the elements inside our model
                var building = Model.Instances;

                #region BuildingData
                Building Mybuilding = new Building();
                Mybuilding.Id = building.OfType<IfcBuilding>().FirstOrDefault().EntityLabel;
                Mybuilding.Name = building.OfType<IfcBuilding>().FirstOrDefault().Name.Value;
                Mybuilding.Path = FilePath;
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
                _Context.Spaces.AddRange(FloorSpaces);
                _Context.SaveChanges();
                #endregion

                #region AssetData
                var Assets = building.OfType<IIfcRelContainedInSpatialStructure>();//getting all the assets within the Ifc file (IIFc interface is more general for Ifc Version than Ifc that will need tp specify a certain compatable formate)

                List<Asset> SpacesAssets = new List<Asset>();

                    foreach (var Space in FloorSpaces)
                    {
                        var SpaceAssets = Assets.Where(con => con.RelatingStructure.EntityLabel == Space.Id).FirstOrDefault();//return the space that is a relating object to the furniture
                        if (SpaceAssets == null)
                            continue;
                        var AssetsInSpace = SpaceAssets.RelatedElements;//getting the furniture within our space 
                        foreach (var Asset in AssetsInSpace)//getting each asset in our selected space 
                        {
                            SpacesAssets.Add(//feedign our Asset object with the data needed for database  
                            new Asset
                            {
                                Id = Asset.EntityLabel,
                                Name = Asset.Name.Value,
                                SpaceId = Space.Id,//SpaceId = Assets.IsContainedIn.EntityLabel
                                FloorId=Space.FloorId
                        });
                        }
                    
                
                //    var Pipes = building.OfType<IfcPipeSegment>();
                //    var SpacePipe = Pipes.Where(con => con.IsContainedIn.EntityLabel == Space.Id).FirstOrDefault();//return the space that is a relating object to the furniture
                //    if (SpacePipe == null)
                //        continue;
                //    foreach (var Pipe in Pipes)
                //    {
                //        SpacesAssets.Add(//feedign our Asset object with the data needed for database  
                //        new Asset
                //        {
                //            Id = Pipe.EntityLabel,
                //            Name = Pipe.Name.Value,
                //            SpaceId = Space.Id,//SpaceId = Assets.IsContainedIn.EntityLabel
                //        });
                //    }
                //    var Ducts = building.OfType<IfcDuctSegment>();
                //    var SpaceDuct = Ducts.Where(con => con.IsContainedIn.EntityLabel == Space.Id).FirstOrDefault();//return the space that is a relating object to the furniture
                //    if (SpaceDuct == null)
                //        continue;
                //    foreach (var duct in Ducts)
                //    {
                //        SpacesAssets.Add(//feedign our Asset object with the data needed for database  
                //        new Asset
                //        {
                //            Id = duct.EntityLabel,
                //            Name = duct.Name.Value,
                //            SpaceId = Space.Id,//SpaceId = Assets.IsContainedIn.EntityLabel
                //        });
                //    }
                }
                _Context.Assets.AddRange(SpacesAssets);
                _Context.SaveChanges();
                #endregion

                #region SensorData

                var GettingSensorsOfElementProxy = building.OfType<IfcBuildingElementProxy>().Where(s => s.Name.Value.ToString().Contains("ensor")).ToList();
                var GettingSensorsOfIFCAlarm = building.OfType<IfcAlarm>().ToList();
                var GettingSensorOfIfcSensor = building.OfType<IfcSensor>();
                var GettingSensorOfIfcValve = building.OfType<IfcValve>();
                List<Sensor> sensors = new List<Sensor>();
                //1st Loop Collection
                foreach (var sensor in GettingSensorsOfElementProxy)
                {
                    if (sensor is null)//ensure that ths list Contains no values 
                        continue;
                    //var x = sensor.ConnectedTo;
                    //if (x.Count() == 0)
                    //    continue;
                    sensors.Add(new Sensor()
                    {
                        Id = sensor.EntityLabel,
                        Name = sensor.Name.Value,
                        SpaceId = sensor.IsContainedIn.EntityLabel,//represents the space that contains this sensor
                        SensorType = Enums.SensorType.SpaceSensor,
                        AssetId =null
                    });
                }
                //2nd Loop Collection
                foreach (var sensor in GettingSensorsOfIFCAlarm)
                {
                    if (sensor is null)
                        continue;
                    //var x = sensor.ConnectedTo;
                    //if (x.Count() == 0)
                    //    continue;
                    sensors.Add(new Sensor()
                    {
                        Id = sensor.EntityLabel,
                        Name = sensor.Name.Value,
                        SpaceId = sensor.IsContainedIn.EntityLabel,//represents the space that contains this sensor
                        SensorType = Enums.SensorType.SpaceSensor,//Default value can be changed later
                        AssetId = null

                    });
                }
                //3rd Loop Collection

                foreach (var sensor in GettingSensorOfIfcSensor)
                {
                    if (sensor is null)
                        continue;

                    sensors.Add(new Sensor()
                    {
                        Id = sensor.EntityLabel,
                        Name = sensor.Name.Value,
                        SpaceId = sensor.IsContainedIn.EntityLabel,//represents the space that contains this sensor
                        SensorType = Enums.SensorType.SpaceSensor,//Default value can be changed later
                        AssetId =null

                    });
                }
                //4th Loop Collection
                foreach (var sensor in GettingSensorOfIfcValve)
                {
                    
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
            buildingDTO.Id=BuildingList.Id;
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
        public string ClearDatabase(string PassWord)
        {
            if (PassWord == "Jimmy")
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

                User[] Use = _Context.Users.ToArray();
                _Context.Users.RemoveRange(Use);

                Incident[] Inc = _Context.Incidents.ToArray();
                _Context.Incidents.RemoveRange(Inc);

                Models.Task[] Tas = _Context.Tasks.ToArray();
                _Context.Tasks.RemoveRange(Tas);

                _Context.SaveChanges();
                return "Done";
            }
            else return "Wrong PassWord";
        }

        
    }
}

