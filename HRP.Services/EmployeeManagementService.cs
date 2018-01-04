namespace HRP.Services
{
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Employee;
    using HRP.Models.EmployeeManagement;
    using HRP.Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly IBaseService baseService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ISessionService sessionService;

        public EmployeeManagementService(
            IHostingEnvironment hostingEnvironment,
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.hostingEnvironment = hostingEnvironment;
            this.sessionService = sessionService;
        }

        public async Task<CommandResult> CreateEmployeeAsync(EmployeePersonalDetail employee)
        {
            var pictureExtension = employee.ProfilePicture != null ? Path.GetExtension(employee.ProfilePicture.FileName) : string.Empty;
            var criteria = new CreateEmployeeCriteria
            {
                FullName = employee.FullName,
                NickName = employee.NickName,
                PictureExtension = pictureExtension,
                CreatedBy = sessionService.UserId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            await SaveProfilePictureAsync(employee.ProfilePicture, criteria.EmployeeId.ToString()).ConfigureAwait(false);

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

        private async Task SaveProfilePictureAsync(IFormFile image, string fileName)
        {
            if (image != null && image.Length != 0)
            {
                var directoryPath = Path.Combine(hostingEnvironment.WebRootPath, "ProfilePicture");
                Directory.CreateDirectory(directoryPath);

                var fullFileName = fileName + Path.GetExtension(image.FileName);
                var path = Path.Combine(directoryPath, fullFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream).ConfigureAwait(false);
                }
            }
        }
    }
}