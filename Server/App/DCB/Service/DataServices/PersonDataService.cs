using Contract.Interface;
using Contract.Model;
using DCB.Db.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.DataServices
{
    public class PersonDataService: IPersonDataService
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

        public PersonDataService(IRepository<DCB.DB.Model.Person> personRepository,
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

        public async Task<Response<Person>> GetAsync(Guid id, Guid userId, CancellationToken token)
        {
            var model = await _personRepository.GetAsync(id, token);
            if (model == null)
                return Response<Person>.Error("Персонаж не найден");

            var access = await CheckAccess(userId, model.UserId, token);
            if (!access)
                return Response<Person>.Error("Нет доступа к персонажу");
            
            return await Map(model, token);
        }

        public async Task<Response<EntityList<Person>>> GetAsync(PersonFilter filter, Guid userId, CancellationToken token)
        {
            var models = await _personRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Person>()
            {
                Page = filter.Page,
                Size = filter.Size,
                Sort = filter.Sort,
                Selector = (f) => f.Name == filter.Name
            }, token);

            return Response<EntityList<Person>>.Ok(new EntityList<Person>(models.Data.Select(s => new Person()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
            }
            ).ToList(), models.AllCount, filter.Page ?? 0, filter.Size ?? models.AllCount));
        }

        public async Task<Response<Person>> UpdateAsync(PersonUpdater model, Guid userId, CancellationToken token)
        {
            try
            {
                if(!await _lockService.Lock(model.Id))
                    return Response<Person>.Error("Персонаж заблокирован, попробуйте позже");

                var currentModel = await _personRepository.GetAsync(model.Id, token);

                if (currentModel == null)
                    return Response<Person>.Error("Персонаж не найден");

                var access = await CheckAccess(userId, currentModel.UserId, token);
                if (!access)
                    return Response<Person>.Error("Нет доступа к персонажу");

                var result = await _personRepository.UpdateAsync(new DCB.DB.Model.Person()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description
                }, true, token);

                return await Map(result, token);
            }
            finally
            {
                await _lockService.Unlock(model.Id);
            }
        }

        public async Task<Response<Person>> AddAsync(PersonCreator model, Guid userId, CancellationToken token)
        {
            var access = await CheckAccess(userId, model.UserId, token);
            if (!access)
                return Response<Person>.Error("Нет доступа к пользователю");

            var location = await GetFreeLocation(token);
            if(!location.IsSuccess)
                return Response<Person>.Error(location.Message);

            try
            {
                if (!await _lockService.Lock(location.Value.Id))
                    return Response<Person>.Error("Локация заблокирована, попробуйте позже");

                var result = await _personRepository.AddAsync(new DCB.DB.Model.Person()
                {
                    Name = model.Name,
                    Description = model.Description,
                    LocationId = location.Value.Id
                }, false, token);

                await _characteristicsRepository.AddAsync(new DCB.DB.Model.Characteristics()
                {
                    PersonId = result.Id
                }, false, token);

                await _inventoryRepository.AddAsync(new DCB.DB.Model.Inventory()
                {
                    PersonId = result.Id
                }, false, token);

                await _personRepository.SaveAsync(token);

                return await Map(result, token);
            }
            finally
            {
                await _lockService.Unlock(location.Value.Id);
            }
        }

        private async Task<Response<Location>> GetFreeLocation(CancellationToken token)
        {
            var maps = await _mapRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Map>()
            {
                Selector = (s) => s.Level == 1
            }, token);

            var map = maps.Data.FirstOrDefault();
            if (map == null)
                return Response<Location>.Error("Не удалось разместить персонажа на локации");

            var locations = await _locationRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Location>()
            {
                Selector = (s) => s.MapId == map.Id && s.IsEnter
            }, token);

            if(locations == null || !locations.Data.Any())
                return Response<Location>.Error("Не удалось разместить персонажа на локации");

            foreach(var location in locations.Data)
            {
                if (await _lockService.Lock(location.Id))
                {
                    try
                    {
                        var persons = await _personRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Person>()
                        {
                            Selector = (s) => s.LocationId == location.Id
                        }, token);

                        if(persons.AllCount > 0)
                            continue;

                        var npcs = await _npcRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Npc>()
                        {
                            Selector = (s) => s.LocationId == location.Id
                        }, token);

                        if (npcs.AllCount > 0)
                            continue;

                        return Response<Location>.Ok(new Location 
                        {
                            Id = location.Id
                        });
                    }
                    finally
                    {
                        await _lockService.Unlock(location.Id);
                    }
                }
            }

            return Response<Location>.Error("Не удалось разместить персонажа на локации");
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

        private async Task<Response<Person>> Map(DCB.DB.Model.Person model, CancellationToken token)
        {
            var result = new Person()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            };

            var location = await _locationRepository.GetAsync(model.LocationId, token);
            if (location != null)
            {
                result.Location = new Location()
                {
                    Id = location.Id
                };
            }

            var characteristics = await _characteristicsRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Characteristics>()
            {
                Selector = (ch) => ch.PersonId == model.Id
            }, token);
            if (characteristics != null && characteristics.AllCount > 0)
            {
                var characteristic = characteristics.Data.FirstOrDefault();
                result.Characteristics = new Characteristics()
                {
                    Id = characteristic.Id
                };
            }

            var inventories = await _inventoryRepository.GetAsync(new DCB.DB.Model.Filter<DCB.DB.Model.Inventory>()
            {
                Selector = (ch) => ch.PersonId == model.Id
            }, token);
            if (inventories != null && inventories.AllCount > 0)
            {
                var inventory = inventories.Data.FirstOrDefault();
                result.Inventory = new Inventory()
                {
                    Id = inventory.Id
                };
            }

            return Response<Person>.Ok(result);
        }
    }
}
