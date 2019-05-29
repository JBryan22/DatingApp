using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class EFAuthRepository : IAuthRepository
    {
        private readonly DataContext _db;
        public EFAuthRepository(DataContext db)
        {
            _db = db;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                //if we return null, we can return a proper http response in our controller (maybe a 401 unauthorized)
                return null;
            }

            //password check. calls method using salt and compares hash (after unencoding the salted hash). if they are the same the user will be authorized
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            //using out to send as a reference so we can change the password variables created above
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //anything within the using method will be disposed of after it is used. 
            //it boils down to try{stuff in user method}, finally{obj.Dispose()}
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _db.Users.AnyAsync(x => x.Username == username))
            {
                return true;
            }
            return false;
        }
    }
}