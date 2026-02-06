using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AssignmentProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("Signup")]
        public IActionResult Signup([FromBody] RegisterModel model)
        {
            string conStr = _config.GetConnectionString("DefaultConnection");

            string message = "";
            string uniqueId = "";

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_MainApi", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "SIGNUP");
                cmd.Parameters.AddWithValue("@FullName", model.FullName);
                cmd.Parameters.AddWithValue("@Username", model.Username);

                string passwordHash = PasswordHasher.HashPassword(model.Password);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    message = dr["Message"].ToString();
                    uniqueId = dr["UniqueId"] == DBNull.Value ? "" : dr["UniqueId"].ToString();
                }
            }

            if (message == "Username already exists!")
                return BadRequest(new { message = message });

            return Ok(new { message = message, uniqueId = uniqueId });
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            string conStr = _config.GetConnectionString("DefaultConnection");

            int userId = 0;
            string fullName = "";
            string uniqueId = "";
            string storedHash = "";

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_MainApi", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "LOGIN");
                cmd.Parameters.AddWithValue("@Username", model.Username);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                {
                    return Unauthorized(new { message = "Invalid Username or Password" });
                }

                storedHash = dr["PasswordHash"].ToString();

                bool isValidPassword = PasswordHasher.VerifyPassword(model.Password, storedHash);

                if (!isValidPassword)
                {
                    return Unauthorized(new { message = "Invalid Username or Password" });
                }

                userId = Convert.ToInt32(dr["UserId"]);
                fullName = dr["FullName"].ToString();
                uniqueId = dr["UniqueId"].ToString();
            }

            var token = GenerateToken(userId.ToString(), model.Username);

            return Ok(new
            {
                token = token,
                username = model.Username,
                fullName = fullName,
                uniqueId = uniqueId
            });
        }

        private string GenerateToken(string userId, string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static class PasswordHasher
        {
            public static string HashPassword(string password)
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }

            public static bool VerifyPassword(string enteredPassword, string storedHash)
            {
                return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
            }
        }
    }
    
}

