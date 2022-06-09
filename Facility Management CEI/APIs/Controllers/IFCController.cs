using API;
using API.DB;
using API.Enums;
using API.Models;
using API.Moldels_DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.BuildingControlsDomain;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.SharedBldgElements;

namespace Facility_Management_CEI.APIs.Controllers
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
                Mybuilding.Name = building.OfType<IfcBuilding>().FirstOrDefault().LongName.Value;
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
                            SpaceId = Space.Id,
                            FloorId = Space.FloorId
                        });
                    }
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
                        SensorType = SensorType.SpaceSensor.ToString(),
                        AssetId = null
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
                        SensorType = SensorType.SpaceSensor.ToString(),//Default value can be changed later
                        AssetId = null

                    });
                }
                //3rd Loop Collection

                foreach (var sensor in GettingSensorOfIfcSensor)
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
                        SensorType = SensorType.SpaceSensor.ToString(),//Default value can be changed later
                        AssetId = null

                    });
                }
                //4th Loop Collection

                //foreach (var sensor in GettingSensorOfIfcValve)
                //{
                //    if (sensor is null)
                //        continue;
                //    //var x = sensor.ConnectedTo;
                //    //if (x.Count() == 0)
                //    //continue;
                //    //var x = sensor.ReferencedBy.OfType<IfcPipeFitting>().FirstOrDefault();
                //    sensors.Add(new Sensor()
                //    {
                //        Id = sensor.EntityLabel,
                //        Name = sensor.Name.Value,
                //        SpaceId = sensor.IsContainedIn.EntityLabel,//represents the space that contains this sensor
                //        SensorType = SensorType.AssetSensor,//Default value can be changed later
                //        //AssetId = sensor.Nests.FirstOrDefault(x=>x.EntityLabel==Assets.FirstOrDefault().EntityLabel).EntityLabel
                //    });
                //}

                _Context.Sensors.AddRange(sensors);
                _Context.SaveChanges();

                #endregion

                #region SensorWarnigData
                //List<SensorWarning> SensorWarnings=new List<SensorWarning>();
                ////List<User>users=new List<User>();//we have to delete this line when users are created

                //foreach (var sensor in sensors)
                //{
                //    var x=0;
                //    if ( x<(int)Priority.Medium) {

                //        break;
                //        else if(){ }
                //        SensorWarnings.Add(new SensorWarning()
                //        {

                //            SensorId = sensor.Id,
                //            Description = "",
                //            Investigated = false,
                //            //??
                //            Priority = Priority.Low,
                //            //UserId=users.FirstOrDefault().Id,
                //            IssueDate = DateTime.Now,

                //        });
                //    }

                //_Context.SensorWarnings.AddRange(SensorWarnings);
                //_Context.SaveChanges();

                #endregion

                #region Incident_Data

                //List<Incident> incidents=new List<Incident>();
                //foreach(var user in _Context.Users)
                //{
                //      incidents.Add(new Incident() {
                //      Status=Enums.Status.NotAssigned,
                //      Priority=Priority.Low,
                //      /*Description=user. \\warning.Description*/,
                //      AssetId=warning.Sensor.Asset.Id,
                //      SpaceId=warning.Sensor.SpaceId,
                //      UserId= _Context.Users.Select(x=>x.Id==warning.User/*.Select(y=>y.AssignedById).FirstOrDefault()*/),

                //    });
                //}

                #endregion
            }

        }
        #region Delete_DataBase


        [Route("ClearDatabase")]
        [HttpDelete]
        public string ClearDatabase(/*string PassWord*/)
        {
            //if (PassWord == "Jimmy")
            //{
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

            //SensorWarning[] SenWar = _Context.SensorWarnings.ToArray();
            //_Context.SensorWarnings.RemoveRange(SenWar);

            //User[] Use = _Context.Users.ToArray();
            //_Context.Users.RemoveRange(Use);

            //Incident[] Inc = _Context.Incidents.ToArray();
            //_Context.Incidents.RemoveRange(Inc);

            //Models.Task[] Tas = _Context.Tasks.ToArray();
            //_Context.Tasks.RemoveRange(Tas);

            _Context.SaveChanges();
            return "Done";
            //}
            //else return "Wrong PassWord";
        }
        #endregion

        #region retrieve_building_Data

        [Route("Retrieve_Builing_Data")]
        [HttpGet]
        public BuildingDTO Retrieve_Builing_Data()
        {
            BuildingDTO buildingDTO = new BuildingDTO()
            {
                Id = _Context.Buildings.FirstOrDefault().Id,
                Name = _Context.Buildings.FirstOrDefault().Name,
                Path = _Context.Buildings.FirstOrDefault().Path,
            };
            return buildingDTO;
        }
        #endregion

        #region retrieve_Floor_Data
        [Route("FloorDTO")]
        [HttpGet]
        public IEnumerable<FloorDTO> FloorDTO()
        {
            List<FloorDTO> floorDTOs = new List<FloorDTO>();
            foreach (var floor in _Context.Floors)
            {
                new FloorDTO()
                {
                    Id = floor.Id,
                    FloorName = floor.FloorName,
                    BuildingId = floor.BuildingId,
                };
            }
            return floorDTOs;
        }
        #endregion

        #region retireve_Space_Data
        [Route("SpaceDTO")]
        [HttpGet]
        public IEnumerable<SpaceDTO> SpaceDTO()
        {
            List<SpaceDTO> SpaceDTOs = new List<SpaceDTO>();
            foreach (var space in _Context.Spaces)
            {
                new SpaceDTO()
                {
                    Id = space.Id,
                    Name = space.Name,
                    FloorId = space.FloorId
                };
            }
            return SpaceDTOs;
        }
        #endregion

        #region retrieve_Asset_Data
        [Route("AssetDTO")]
        [HttpGet]
        public IEnumerable<AssetDTO> AssetDTO()
        {
            List<AssetDTO> assetDTOs = new List<AssetDTO>();
            foreach (var asset in _Context.Assets)
            {
                new AssetDTO()
                {
                    Id = asset.Id,
                    Name = asset.Name,
                    SpaceId = asset.SpaceId
                };
            }
            return assetDTOs;
        }
        #endregion

        #region retrieve_Sensor_Data
        [Route("sensorDTOs")]
        [HttpGet]
        public IEnumerable<SensorDTO> sensorDTOs()
        {
            List<SensorDTO> SensorDTO = new List<SensorDTO>();
            foreach (var sensor in _Context.Sensors)
            {
                SensorDTO.Add(new SensorDTO
                {
                    Id = sensor.Id,
                    Name = sensor.Name,
                    SpaceId = sensor.SpaceId,
                    SensorType = sensor.SensorType,
                    AssetId = sensor.AssetId


                });

            }

            return SensorDTO;
        }
        #endregion

        #region Sensor_Warning_Data
        [Route("SensorWarningDTO")]
        [HttpGet]
        public IEnumerable<SensorWarningDTO> SensorWarningDTO()
        {
            List<SensorWarningDTO> sensorWarningDTO = new List<SensorWarningDTO>();
            foreach (var sensorwarning in _Context.SensorWarnings)
            {
                sensorWarningDTO.Add(new SensorWarningDTO
                {
                    Id = sensorwarning.Id,
                    Description = sensorwarning.Description,
                    InvestigatDate = sensorwarning.InvestigatDate,
                    Investigated = sensorwarning.Investigated,
                    IssueDate = sensorwarning.IssueDate,
                    Priority = sensorwarning.Priority,
                    SensorId = sensorwarning.SensorId,
                });
            }
            return sensorWarningDTO;
        }

        #endregion

        #region Incident_Data
        [Route("IncidentDTO")]
        [HttpGet]
        public IEnumerable<IncedentDTO> IncidentDTO()
        {
            List<IncedentDTO> Incidents = new List<IncedentDTO>();
            foreach (var Incident in _Context.Incidents)
            {
                Incidents.Add(new IncedentDTO
                {
                    Id = Incident.Id,
                    AssetId = Incident.AssetId,
                    Description = Incident.Description,
                    Priority = Incident.Priority,
                    ReportingTime = Incident.ReportingTime,
                    SensorWarningId = Incident.SensorWarningId,
                    SpaceId = Incident.SpaceId,
                    Status = Incident.Status,
                    UserId = Incident.UserId,
                });
            }
            return Incidents;
        }
        #endregion

    }
}

