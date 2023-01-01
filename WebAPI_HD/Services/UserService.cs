using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_HD.Helper;
using WebAPI_HD.Model;
using WebAPI_HD.Repository;

namespace WebAPI_HD.Services
{
    public interface IUserService
    {
        LoginResponse Login([FromBody] LoginModel model);
        IEnumerable<User> GetAll();
        User GetById(int id);
        void Register(RegisterRequest model);
        void Update(int id, UpdateRequest model);
        void Delete(int id);
    }
    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        private JwtAuthenticationManager _jwtAuth;
        private readonly IMapper _mapper;

        public UserService(
            ApplicationDbContext context,
            JwtAuthenticationManager jwtAuth,
            IMapper mapper)
        {
            _context = context;
            _jwtAuth = jwtAuth;
            _mapper = mapper;
        }
        public LoginResponse Login([FromBody] LoginModel model)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == model.Username);

            if ((user == null) || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                throw new AppException("Username or password is incorrect");
            }
            var response = _mapper.Map<LoginResponse>(user);
            response.Token = _jwtAuth.GenerateAccessToken(user);
            return response;
            //sign your token here here..
        }
        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return getUser(id);
        }
        public void Register(RegisterRequest model)
        {
            // validate
            if (_context.Users.Any(x => x.Username == model.Username))
                throw new AppException("Username '" + model.Username + "' is already taken");

            // map model to new user object
            var user = _mapper.Map<User>(model);

            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // save user
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        // GET: api/Users
        public void Update(int id, UpdateRequest model)
        {
            var user = getUser(id);

            // validate
            if (model.Username != user.Username && _context.Users.Any(x => x.Username == model.Username))
                throw new AppException("Username '" + model.Username + "' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // copy model to user and save
            _mapper.Map(model, user);
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = getUser(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        // helper methods

        private User getUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            } 
                
            return user;
        }

    }
}
