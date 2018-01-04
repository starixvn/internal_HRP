namespace HRP.Services
{
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.EmployeeManagement;
    using HRP.Models.EmployeeManagement;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;

    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly IBaseService baseService;

        public EmployeeManagementService(
            IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> CreateEmployeeAsync(EmployeePersonalDetail employee)
        {
            var criteria = new CreateEmployeeCriteria
            {
                FullName = employee.FullName,
                NickName = employee.NickName
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public CommandResult CheckValidPicture(IFormFile file)
        {
            var result = new CommandResult();

            if (file != null)
            {
                if (file.Length / (1024 * 1024) > 5)
                {
                    result.Code = 1;
                    result.Message = Message.FileTooLarge;
                }
                else if (!file.ContentType.Contains("image"))
                {
                    result.Code = 1;
                    result.Message = Message.InvalidImageType;
                }
                else
                {
                    result.Code = 0;
                    result.Message = string.Empty;
                }
            }
            else
            {
                result.Code = 0;
                result.Message = string.Empty;
            }

            return result;
        }

        private void SaveProfilePicture(string fileName)
        {
            //To do
        }
    }
}