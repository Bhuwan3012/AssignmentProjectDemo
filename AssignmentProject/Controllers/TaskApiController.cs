using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;

namespace AssignmentProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskApiController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TaskApiController(IConfiguration config)
        {
            _config = config;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private string GetUserName()
        {
            return User.FindFirstValue(ClaimTypes.Name) ?? "User";
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            List<TaskModel> tasks = new List<TaskModel>();
            string conStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_MainApi", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "GET_ALL_TASKS");

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    tasks.Add(new TaskModel
                    {
                        TaskId = Convert.ToInt32(dr["TaskId"]),
                        TaskTitle = dr["TaskTitle"].ToString(),
                        TaskDescription = dr["TaskDescription"].ToString(),
                        TaskDueDate = Convert.ToDateTime(dr["TaskDueDate"]),
                        TaskStatus = dr["TaskStatus"].ToString(),
                        TaskRemarks = dr["TaskRemarks"].ToString(),
                        CreatedOn = Convert.ToDateTime(dr["CreatedOn"]),

                        LastUpdatedOn = dr["LastUpdatedOn"] == DBNull.Value ? null : Convert.ToDateTime(dr["LastUpdatedOn"]),

                        CreatedByName = dr["CreatedByName"].ToString(),
                        CreatedById = dr["CreatedById"].ToString(),

                        LastUpdatedByName = dr["LastUpdatedByName"] == DBNull.Value ? null : dr["LastUpdatedByName"].ToString(),
                        LastUpdatedById = dr["LastUpdatedById"] == DBNull.Value ? null : dr["LastUpdatedById"].ToString()
                    });
                }
            }

            return Ok(tasks);
        }


        [HttpPost("Create")]
        public IActionResult Create([FromBody] TaskModel task)
        {
            string conStr = _config.GetConnectionString("DefaultConnection");

            string userId = task.CreatedById;
            string userName = task.CreatedByName;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_MainApi", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "CREATE_TASK");
                cmd.Parameters.AddWithValue("@TaskTitle", task.TaskTitle);
                cmd.Parameters.AddWithValue("@TaskDescription", task.TaskDescription ?? "");
                cmd.Parameters.AddWithValue("@TaskDueDate", task.TaskDueDate);
                cmd.Parameters.AddWithValue("@TaskStatus", task.TaskStatus);
                cmd.Parameters.AddWithValue("@TaskRemarks", task.TaskRemarks ?? "");

                cmd.Parameters.AddWithValue("@CreatedByName", userName);
                cmd.Parameters.AddWithValue("@CreatedById", userId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return Ok(new { message = "Task Created Successfully" });
        }


        [HttpPut("Update/{id}")]
        public IActionResult Update(int id, [FromBody] TaskModel task)
        {
            string conStr = _config.GetConnectionString("DefaultConnection");

            task.LastUpdatedOn = DateTime.Now;

            string userId2 = task.CreatedById;
            string userId = task.LastUpdatedById;
            string userName = task.LastUpdatedByName;

            string message = "";

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_MainApi", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "UPDATE_TASK");
                cmd.Parameters.AddWithValue("@TaskId", id);
                cmd.Parameters.AddWithValue("@TaskTitle", task.TaskTitle);
                cmd.Parameters.AddWithValue("@TaskDescription", task.TaskDescription ?? "");
                cmd.Parameters.AddWithValue("@TaskDueDate", task.TaskDueDate);
                cmd.Parameters.AddWithValue("@TaskStatus", task.TaskStatus);
                cmd.Parameters.AddWithValue("@TaskRemarks", task.TaskRemarks ?? "");

                cmd.Parameters.AddWithValue("@LastUpdatedByName", userName ?? "");
                cmd.Parameters.AddWithValue("@LastUpdatedById", userId ?? "");
                cmd.Parameters.AddWithValue("@CreatedById", userId2);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    message = dr["Message"].ToString();
                }
            }

            if (message == "You can update only your own task!")
                return Unauthorized(new { message = message });

            return Ok(new { message = message });
        }


        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id, [FromQuery] string createdById)
        {
            string conStr = _config.GetConnectionString("DefaultConnection");
            string message = "";

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_MainApi", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "DELETE_TASK");
                cmd.Parameters.AddWithValue("@TaskId", id);
                cmd.Parameters.AddWithValue("@CreatedById", createdById);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    message = dr["Message"].ToString();
                }
            }

            if (message == "You can delete only your own task!")
                return Unauthorized(new { message = message });

            return Ok(new { message = message });
        }


        // SEARCH TASKS
        [HttpGet("Search/{text}")]
        public IActionResult Search(string text)
        {
            List<TaskModel> tasks = new List<TaskModel>();
            string conStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_MainApi", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "SEARCH_TASKS");
                cmd.Parameters.AddWithValue("@SearchText", text);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    tasks.Add(new TaskModel
                    {
                        TaskId = Convert.ToInt32(dr["TaskId"]),
                        TaskTitle = dr["TaskTitle"].ToString(),
                        TaskDescription = dr["TaskDescription"].ToString(),
                        TaskDueDate = Convert.ToDateTime(dr["TaskDueDate"]),
                        TaskStatus = dr["TaskStatus"].ToString(),
                        TaskRemarks = dr["TaskRemarks"].ToString(),
                        CreatedOn = Convert.ToDateTime(dr["CreatedOn"]),

                        LastUpdatedOn = dr["LastUpdatedOn"] == DBNull.Value ? null : Convert.ToDateTime(dr["LastUpdatedOn"]),

                        CreatedByName = dr["CreatedByName"].ToString(),
                        CreatedById = dr["CreatedById"].ToString(),

                        LastUpdatedByName = dr["LastUpdatedByName"] == DBNull.Value ? null : dr["LastUpdatedByName"].ToString(),
                        LastUpdatedById = dr["LastUpdatedById"] == DBNull.Value ? null : dr["LastUpdatedById"].ToString()
                    });
                }
            }

            return Ok(tasks);
        }
    }
  
}

