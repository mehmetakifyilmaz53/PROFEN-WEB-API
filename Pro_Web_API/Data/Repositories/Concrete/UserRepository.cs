using Microsoft.EntityFrameworkCore;
using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Data.Contexts;
using Pro_Web_API.Data.Repositories.Abstract;

namespace Pro_Web_API.Data.Repositories.Concrete
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.user_Name == username);
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task AddAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı ekleme işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task UpdateAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı güncelleme işlemi sırasında hata oluştu.", 500);
            }

        }

        public async Task DeleteAsync(User user)
        {

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı silme işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {

            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.email == email);
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı işlemi sırasında hata oluştu.", 500);
            }
        }
    }
}
