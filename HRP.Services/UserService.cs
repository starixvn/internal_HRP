namespace HRP.Services
{
    using Core.Services;
    using HRP.Criteria;
    using HRP.Models;

    public class UserService : BaseService, IUserService
    {
        public User GetUserByUsername(string username)
        {
            GetUserByUsernameCriteria criteria = new GetUserByUsernameCriteria
            {
                Username = username
            };

            User user = new User();

            if (criteria != null && criteria.IsValid())
            {
                user = GetDataEntity<User>(criteria);
            }

            return user;
        }
    }
}