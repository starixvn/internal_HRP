namespace HRP.Core.Model
{
    using System.Collections.Generic;

    public class UserSession
    {
        public UserSession()
        {
            Functions = new List<FunctionModel>();
        }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public IEnumerable<FunctionModel> Functions { get; set; }
    }
}