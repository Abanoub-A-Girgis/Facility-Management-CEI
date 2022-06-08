//using API.Models;
//using API.Moldels_DTOs;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xbim.Ifc;
//using Xbim.Ifc4.ProductExtension;
//using Xbim.Ifc4.SharedBldgElements;

//namespace API.Controllers
//{
//    [Route("api/[controller]")]//the route that the request and response will take 
//    [ApiController]//Controller type 
//    public class Send_IFC_Controller : Controller
//    {
//        #region PostAPI 
//        [Route("AddNewBuilding")]
//        [HttpPost]
//        public void AddNewBuilding(BuildingDTO Bui)
//        {
//            var Building = new Building()
//            {
//                Id = Bui.Id,
//                Name = Bui.Name,
//            };
//            _Context.Buildings.Add(Building);
//            _Context.SaveChanges();
//        }
//        [Route("AddNewFloor")]
//        [HttpPost]
//        public void AddNewFloor(FloorDTO Flo)
//        {
//            var Floor = new Floor()
//            {
//                Id = Flo.Id,
//                FloorNumber = Flo.FloorNumber,
//                BuildingId = Flo.BuildingId,
//            };
//            _Context.Floors.Add(Floor);
//            _Context.SaveChanges();
//        }
//        [Route("AddNewSpace")]
//        [HttpPost]
//        public void AddNewSpace(SpaceDTO Spa)
//        {
//            var Space = new Space()
//            {
//                Id = Spa.Id,
//                Name = Spa.Name,
//                FloorId = Spa.FloorId,
//            };
//            _Context.Spaces.Add(Space);
//            _Context.SaveChanges();
//        }
//        [Route("AddNewAsset")]
//        [HttpPost]
//        public void AddNewAsset(AssetDTO Asse)
//        {
//            var Asset = new Asset()
//            {
//                Id = Asse.Id,
//                Name = Asse.Name,
//                SpaceId = Asse.SpaceId,
//            };
//            _Context.Assets.Add(Asset);
//            _Context.SaveChanges();
//        }
//        [Route("AddNewSensor")]
//        [HttpPost]
//        public void AddNewSensor(SensorDTO Sen)
//        {
//            var Sensor = new Sensor()
//            {
//                Id = Sen.Id,
//                Name = Sen.Name,
//                SpaceId = Sen.SpaceId,
//            };
//            _Context.Sensors.Add(Sensor);
//            _Context.SaveChanges();
//        }
//        [Route("AddNewSensorWarning")]
//        [HttpPost]
//        public void AddNewSensorWarning(SensorWarningDTO SenWar)
//        {
//            var SensorWarning = new SensorWarning()
//            {
//                //Id = SenWar.Id,//Auto generated will make error if added 
//                SensorId = SenWar.SensorId,
//                Description = SenWar.Description,
//                Investigated = SenWar.Investigated,
//                Priority = SenWar.Priority,
//                ReportingTime = SenWar.ReportingTime,
//                IssueDate = SenWar.IssueDate,
//                UserId = SenWar.UserId,//make Put Api To add the User Id 
//            };
//            _Context.SensorWarnings.Add(SensorWarning);
//            _Context.SaveChanges();
//        }
//        [Route("AddNewUser")]
//        [HttpPost]
//        public void AddNewUser(UserDTO Use)
//        {
//            var User = new User()
//            {
//                //Id = Use.Id,//Auto generated will make error if added
//                UserName = Use.UserName,
//                PassWord = Use.PassWord,
//                Type = Use.Type,
//                BuildingId = Use.BuildingId,
//                SuperId = Use.SuperId,
//            };
//            _Context.Users.Add(User);
//            _Context.SaveChanges();
//        }
//        [Route("AddNewIncident")]
//        [HttpPost]
//        public void AddNewIncident(IncidentDTO Inc)
//        {
//            var Incident = new Incident()
//            {
//                //Id = Inc.Id,//Auto generated will make error if added
//                Status = Inc.Status,
//                Priority = Inc.Priority,
//                Description = Inc.Description,
//                ReportingTime = Inc.ReportingTime,
//                AssetId = Inc.AssetId,
//                UserId = Inc.UserId,
//                SensorWarningId = Inc.SensorWarningId,
//            };
//            _Context.Incidents.Add(Incident);
//            _Context.SaveChanges();
//        }
//        [Route("AddNewTask")]
//        [HttpPost]
//        public void AddNewTask(TaskDTO Tas)
//        {
//            var Task = new Task()
//            {
//                //Id = Tas.Id,//Auto generated will make error if added
//                Type = Tas.Type,
//                Description = Tas.Description,
//                Status = Tas.Status,
//                Priority = Tas.Priority,
//                Cost = Tas.Cost,
//                FixingTime = Tas.FixingTime,
//                IncidentId = Tas.IncidentId,
//                AssignedToId = Tas.AssignedToId,
//                AssignedById = Tas.AssignedById,
//                CreatedById = Tas.CreatedById,
//            };
//            _Context.Tasks.Add(Task);
//            _Context.SaveChanges();
//        }
//        #endregion
//        #region GetAPI
//        [Route("GetAllBuildings")]
//        [HttpGet]
//        public IEnumerable<BuildingDTO> GetAllBuildings()
//        {
//            return _Context.Buildings.ToList().Select(
//                Bui => new BuildingDTO()
//                {
//                    Id = Bui.Id,
//                    Name = Bui.Name,
//                }).ToList();
//        }
//        [Route("GetAllFloors")]
//        [HttpGet]
//        public IEnumerable<FloorDTO> GetAllFloors()
//        {
//            return _Context.Floors.ToList().Select(
//                Flo => new FloorDTO()
//                {
//                    Id = Flo.Id,
//                    BuildingId = Flo.BuildingId,
//                    FloorNumber = Flo.FloorNumber,
//                }).ToList();
//        }
//        [Route("GetAllSpaces")]
//        [HttpGet]
//        public IEnumerable<SpaceDTO> GetAllSpaces()
//        {
//            return _Context.Spaces.ToList().Select(
//                Spa => new SpaceDTO()
//                {
//                    Id = Spa.Id,
//                    Name = Spa.Name,
//                    FloorId = Spa.FloorId,
//                }).ToList();
//        }
//        [Route("GetAllAssets")]
//        [HttpGet]
//        public IEnumerable<AssetDTO> GetAllAssets()
//        {
//            return _Context.Assets.ToList().Select(
//                Asse => new AssetDTO()
//                {
//                    Id = Asse.Id,
//                    Name = Asse.Name,
//                    SpaceId = Asse.SpaceId,
//                }).ToList();
//        }
//        [Route("GetAllSensors")]
//        [HttpGet]
//        public IEnumerable<SensorDTO> GetAllSensors()
//        {
//            return _Context.Sensors.ToList().Select(
//                Sen => new SensorDTO()
//                {
//                    Id = Sen.Id,
//                    Name = Sen.Name,
//                    SpaceId = Sen.SpaceId,
//                }).ToList();
//        }
//        [Route("GetAllSensorWarnings")]
//        [HttpGet]
//        public IEnumerable<SensorWarningDTO> GetAllSensorWarnings()
//        {
//            return _Context.SensorWarnings.ToList().Select(
//                SenWar => new SensorWarningDTO()
//                {
//                    Id = SenWar.Id,
//                    SensorId = SenWar.SensorId,
//                    Description = SenWar.Description,
//                    Investigated = SenWar.Investigated,
//                    Priority = SenWar.Priority,
//                    IssueDate = SenWar.IssueDate,
//                    ReportingTime = SenWar.ReportingTime,
//                    UserId = SenWar.UserId,
//                }).ToList();
//        }
//        [Route("GetAllUsers")]
//        [HttpGet]
//        public IEnumerable<UserDTO> GetAllUsers()
//        {
//            return _Context.Users.ToList().Select(
//                Use => new UserDTO()
//                {
//                    Id = Use.Id,
//                    UserName = Use.UserName,
//                    PassWord = Use.PassWord,
//                    Type = Use.Type,
//                    BuildingId = Use.BuildingId,
//                    SuperId = Use.SuperId,
//                }).ToList();
//        }
//        [Route("GetAllIncidents")]
//        [HttpGet]
//        public IEnumerable<IncidentDTO> GetAllIncidents()
//        {
//            return _Context.Incidents.ToList().Select(
//                Inc => new IncidentDTO()
//                {
//                    Id = Inc.Id,
//                    Status = Inc.Status,
//                    Priority = Inc.Priority,
//                    Description = Inc.Description,
//                    ReportingTime = Inc.ReportingTime,
//                    AssetId = Inc.AssetId,
//                    UserId = Inc.UserId,
//                    SensorWarningId = Inc.SensorWarningId,
//                }).ToList();
//        }
//        [Route("GetAllTasks")]
//        [HttpGet]
//        public IEnumerable<TaskDTO> GetAllTasks()
//        {
//            return _Context.Tasks.ToList().Select(
//                Tas => new TaskDTO()
//                {
//                    Id = Tas.Id,
//                    Type = Tas.Type,
//                    Description = Tas.Description,
//                    Status = Tas.Status,
//                    Priority = Tas.Priority,
//                    Cost = Tas.Cost,
//                    FixingTime = Tas.FixingTime,
//                    IncidentId = Tas.IncidentId,
//                    AssignedToId = Tas.AssignedToId,
//                    AssignedById = Tas.AssignedById,
//                    CreatedById = Tas.CreatedById,
//                }).ToList();
//        }
//        #endregion
//        #region PutAPI
//        [Route("SensorMonitoringAsset")]
//        [HttpPut]
//        public void SensorMonitoringAsset(int SensorID, int AssetID)
//        {
//            Asset AssetToBeMonitored = _Context.Assets.FirstOrDefault(Asse => Asse.Id == AssetID);
//            Sensor SensorToMonitor = _Context.Sensors.FirstOrDefault(Sen => Sen.Id == SensorID);
//            AssetToBeMonitored.Sensor.Add(SensorToMonitor);
//            SensorToMonitor.Asset.Add(AssetToBeMonitored);
//            _Context.SaveChanges();
//        }
//        #endregion
//        #region DeleteAPI
//        [Route("DeleteBuildingById")]
//        [HttpDelete]
//        public void DeleteBuildingById(int Id)
//        {
//            _Context.Buildings.Remove(_Context.Buildings.ToList()
//                .Where(Bui => Bui.Id == Id).FirstOrDefault());
//            _Context.SaveChanges();
//        }
//        [Route("DeleteFloorById")]
//        [HttpDelete]
//        public void DeleteFloorById(int Id)
//        {
//            _Context.Floors.Remove(_Context.Floors.ToList()
//                .Where(Flo => Flo.Id == Id).FirstOrDefault());
//            _Context.SaveChanges();
//        }
//        [Route("DeleteSpaceById")]
//        [HttpDelete]
//        public void DeleteSpaceById(int Id)
//        {
//            _Context.Spaces.Remove(_Context.Spaces.ToList()
//                .Where(Spa => Spa.Id == Id).FirstOrDefault());
//            _Context.SaveChanges();
//        }
//        [Route("DeleteAssetById")]
//        [HttpDelete]
//        public void DeleteAssetById(int Id)
//        {
//            _Context.Assets.Remove(_Context.Assets.ToList()
//                .Where(Asse => Asse.Id == Id).FirstOrDefault());
//            _Context.SaveChanges();
//        }
//        [Route("DeleteSensorById")]
//        [HttpDelete]
//        public void DeleteSensorById(int Id)
//        {
//            _Context.Sensors.Remove(_Context.Sensors.ToList()
//                .Where(Sen => Sen.Id == Id).FirstOrDefault());
//            _Context.SaveChanges();
//        }
//        [Route("DeleteSensorWarningById")]
//        [HttpDelete]
//        public void DeleteSensorWarningById(int Id)
//        {
//            _Context.SensorWarnings.Remove(_Context.SensorWarnings.ToList()
//                .Where(SenWar => SenWar.Id == Id).FirstOrDefault());
//            _Context.SaveChanges();
//        }
//        [Route("DeleteUserById")]
//        [HttpDelete]
//        public void DeleteUserById(int Id)
//        {
//            _Context.Users.Remove(_Context.Users.ToList()
//                .Where(Use => Use.Id == Id).FirstOrDefault());
//            _Context.SaveChanges();
//        }
//        [Route("DeleteIncidentById")]
//        [HttpDelete]
//        public void DeleteIncidentById(int Id)
//        {
//            _Context.Incidents.Remove(_Context.Incidents.ToList()
//                .Where(Inc => Inc.Id == Id).FirstOrDefault());
//            _Context.SaveChanges();
//        }
//        [Route("DeleteTaskById")]
//        [HttpDelete]
//        public void DeleteTaskById(int Id)
//        {
//            _Context.Tasks.Remove(_Context.Tasks.ToList()
//                .Where(Tas => Tas.Id == Id).FirstOrDefault());
//            _Context.SaveChanges();
//        }
//        [Route("ClearDatabase")]
//        [HttpDelete]
//        public string ClearDatabase(string PassWord)
//        {
//            if (PassWord == "Jimmy")
//            {
//                Building[] Bui = _Context.Buildings.ToArray();
//                _Context.Buildings.RemoveRange(Bui);

//                Floor[] Flo = _Context.Floors.ToArray();
//                _Context.Floors.RemoveRange(Flo);

//                Space[] Spa = _Context.Spaces.ToArray();
//                _Context.Spaces.RemoveRange(Spa);

//                Asset[] Asse = _Context.Assets.ToArray();
//                _Context.Assets.RemoveRange(Asse);

//                Sensor[] Sen = _Context.Sensors.ToArray();
//                _Context.Sensors.RemoveRange(Sen);

//                SensorWarning[] SenWar = _Context.SensorWarnings.ToArray();
//                _Context.SensorWarnings.RemoveRange(SenWar);

//                User[] Use = _Context.Users.ToArray();
//                _Context.Users.RemoveRange(Use);

//                Incident[] Inc = _Context.Incidents.ToArray();
//                _Context.Incidents.RemoveRange(Inc);

//                Task[] Tas = _Context.Tasks.ToArray();
//                _Context.Tasks.RemoveRange(Tas);

//                _Context.SaveChanges();
//                return "Done";
//            }
//            else return "Wrong PassWord";
//        }
//        #endregion
//        [Route("GetBuildingData")]
//        [HttpGet]
//        public BuildingDTO GetBuildingData(string FilePath)
//        {
//            BuildingDTO MyBuilding = new BuildingDTO();
//            using (var Model = IfcStore.Open($"{FilePath}"))
//            {
//                var building = Model.Instances;
//                MyBuilding.Id = building.OfType<IfcBuilding>().FirstOrDefault().EntityLabel;
//                MyBuilding.Name = FilePath;
//            }
//            return MyBuilding;
//        }
//        [Route("GetFloorData")]
//        [HttpGet]
//        public IEnumerable<FloorDTO> GetFloorData(String FilePath)
//        {
//            FloorDTO MyBuilding = new FloorDTO();

//            using (var Model = IfcStore.Open($"{FilePath}"))
//            {
//                var building = Model.Instances;
//                var Floors = building.OfType<IfcSlab>();
//                List<int> FloorsId = new List<int>();
//                List<string> FloorsName = new List<string>();
//                foreach (var Floor in Floors)
//                {
//                    FloorsId.Add(Floor.EntityLabel);
//                    FloorsName.Add(Floor.Name.Value);
//                }
//            }
//        }
//    }
//}
