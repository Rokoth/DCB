using Contract.Interface;
using Contract.Model;
using DCB.Db.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.GameService
{
    internal class MoveService : IMoveService
    {
        private readonly IRepository<DCB.DB.Model.Person> _personRepository;
        private readonly IRepository<DCB.DB.Model.Location> _locationRepository;
        private readonly IRepository<DCB.DB.Model.Characteristics> _characteristicsRepository;
        private readonly IRepository<DCB.DB.Model.Inventory> _inventoryRepository;
        private readonly IRepository<DCB.DB.Model.UserRole> _userRoleRepository;
        private readonly IRepository<DCB.DB.Model.Role> _roleRepository;
        private readonly IRepository<DCB.DB.Model.Map> _mapRepository;
        private readonly IRepository<DCB.DB.Model.Npc> _npcRepository;
        private readonly ILockService _lockService;

        public MoveService(IRepository<DCB.DB.Model.Person> personRepository,
            IRepository<DCB.DB.Model.Location> locationRepository,
            IRepository<DCB.DB.Model.Characteristics> characteristicsRepository,
            IRepository<DCB.DB.Model.Inventory> inventoryRepository,
            IRepository<DCB.DB.Model.UserRole> userRoleRepository,
            IRepository<DCB.DB.Model.Role> roleRepository,
            IRepository<DCB.DB.Model.Map> mapRepository,
            IRepository<DCB.DB.Model.Npc> npcRepository,
            ILockService lockService
            )
        {
            _personRepository = personRepository;
            _locationRepository = locationRepository;
            _characteristicsRepository = characteristicsRepository;
            _inventoryRepository = inventoryRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _mapRepository = mapRepository;
            _npcRepository = npcRepository;
            _lockService = lockService;
        }

        public async Task<Response<Location>> MoveNorth(Guid personId, bool isPerson, Guid? userId, CancellationToken token)
        {
            Guid? personLockId = null;
            Guid? curLocationLockId = null;
            Guid? newLocationLockId = null;
            try
            {
                if (isPerson)
                {
                    if (userId == null)
                        throw new ArgumentNullException("userId");

                    var person = await _personRepository.GetAsync(personId, token);
                    if (person == null)
                        return Response<Location>.Error("Персонаж не найден");

                    var access = await CheckAccess(userId.Value, person.UserId, token);
                    if (!access)
                        return Response<Location>.Error("Нет доступа к персонажу");

                    if(!await _lockService.Lock(person.Id))
                        return Response<Location>.Error("Персонаж заблокирован");

                    personLockId = person.Id;

                    var currentLocation = await _locationRepository.GetAsync(person.LocationId, token);
                    if(currentLocation == null)
                        return Response<Location>.Error("Локация не найдена");

                    if (!await _lockService.Lock(currentLocation.Id))
                        return Response<Location>.Error("Текущая локация заблокирована");

                    curLocationLockId = currentLocation.Id;

                    if (currentLocation.Y == 0)
                    {
                        var currentMap = await _mapRepository.GetAsync(currentLocation.MapId, token);
                        if(currentMap.Y == 0)
                            return Response<Location>.Error("Вы дошли до конца карты");

                        var newMap = (await _mapRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Map>()
                        {
                            Selector = (s) => s.X == currentMap.X && s.Y == currentMap.Y - 1
                        }, token))?.Data?.FirstOrDefault();

                        if(newMap == null)
                            return Response<Location>.Error("Карта не найдена");

                        var newLocation = (await _locationRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Location>()
                        {
                            Selector = (s) => s.MapId == newMap.Id && s.X == currentLocation.X && s.Y == currentLocation.Y
                        }, token)).Data.FirstOrDefault();

                        if (newLocation == null)
                            return Response<Location>.Error("Карта не найдена");

                        if (!await _lockService.Lock(newLocation.Id))
                            return Response<Location>.Error("Новая локация заблокирована");

                        newLocationLockId = newLocation.Id;
                                                
                    }
                    else
                    {

                    }


                }
                else
                {

                }
            }
            finally
            {
                if (personLockId != null)
                    await _lockService.Unlock(personLockId.Value);

                if (curLocationLockId != null)
                    await _lockService.Unlock(curLocationLockId.Value);

                if (newLocationLockId != null)
                    await _lockService.Unlock(newLocationLockId.Value);
            }
        }

        private async Task<bool> CheckAccess(Guid userId, Guid requestUserId, CancellationToken token)
        {
            var userRoles = await _userRoleRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.UserRole>()
            {
                Selector = (s) => s.UserId == userId
            }, token);

            if (userRoles.AllCount == 0)
                return false;

            var rolesId = userRoles.Data.Select(x => x.RoleId).ToList();

            var roles = await _roleRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Role>()
            {
                Selector = (s) => rolesId.Contains(s.Id)
            }, token);

            if (roles.AllCount == 0) return false;

            if (roles.Data.Any(s => s.IsAdmin))
                return true;

            if (requestUserId != userId)
                return false;

            return true;
        }
    }

}
