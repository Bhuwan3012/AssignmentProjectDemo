namespace AssignmentProject
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public DateTime TaskDueDate { get; set; }
        public string TaskStatus { get; set; }
        public string TaskRemarks { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }   // nullable

        public string CreatedByName { get; set; }
        public string CreatedById { get; set; }

        public string? LastUpdatedByName { get; set; } // nullable
        public string? LastUpdatedById { get; set; }   // nullable
    }

    public class RegisterModel
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
