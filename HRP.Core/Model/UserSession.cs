namespace HRP.Core.Model
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class UserSession
    {
        public UserSession()
        {
            MenuItems = new List<MenuItem>();
        }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public IEnumerable<MenuItem> MenuItems { get; set; }

        [JsonIgnore]
        public IEnumerable<FunctionModel> Functions { get; set; }
    }
}