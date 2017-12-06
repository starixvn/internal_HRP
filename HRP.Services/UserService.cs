namespace HRP.Services
{
    using System;
    using System.Data;
    using Core.Model;
    using HRP.Criteria;
    using Core.Services;
    using HRP.Models;
    using Fanex.Data;

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