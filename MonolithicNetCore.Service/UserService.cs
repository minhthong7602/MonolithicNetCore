using System.Collections.Generic;
using System.Linq;
using MonolithicNetCore.Data.Repository;
using MonolithicNetCore.Entity;

namespace MonolithicNetCore.Service
{
    public interface IUserService
    {
        List<Users> GetAll();
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) => _userRepository = userRepository;

        public List<Users> GetAll()
        {
            return _userRepository.GetAll().ToList();
        }
    }
}
