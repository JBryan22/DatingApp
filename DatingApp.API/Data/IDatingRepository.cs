using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        // this is using a generic type so we don't have to create an Add etc methods for Users and Photos.
        // instead we can just say its of type T which is a class. Type T in our case will be a User or Photo
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         // if we can save, will return true. if there is nothing to save or there was an issue/error it will return false
         Task<bool> SaveAll();
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);
         Task<Photo> GetPhoto(int id);
    }
}