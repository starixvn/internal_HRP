namespace HRP.Website.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.PaymentProcess;
    using HRP.Services.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [HRPAuthorize]
    public class PaymentProcessController : BaseController
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IPaymentProcessService paymentProcessService;

        public PaymentProcessController(
            IBaseService baseService,
            ISessionService sessionService,
            IHostingEnvironment hostingEnvironment,
            IPaymentProcessService paymentProcessService)
            : base(baseService, sessionService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.paymentProcessService = paymentProcessService;
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public IActionResult Index()
        {
            var companies = baseService.GetData<Company>(new GetAllCompanyCriteria { IsActive = true });
            var viewModel = new SelectWorkingMonthCompanyViewModel
            {
                SelectedTime = DateTime.Today,
                Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code))
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> SelectWorkingMonthAndCompany(SelectWorkingMonthCompanyViewModel viewModel)
        {
            var completedDate = DateTime.Today.AddMonths(3);
            if (ModelState.IsValid)
            {
                if (viewModel.SelectedTime.Month == DateTime.Today.Month && viewModel.SelectedTime < completedDate)
                {
                    var process = new PaymentProcess
                    {
                        CompanyId = viewModel.CompanyId,
                        PaymentMonth = viewModel.SelectedTime,
                        CurrentStep = (int)PaymentSteps.SelectWorkingMonthAndCompany
                    };

                    await paymentProcessService.UpdatePaymentProcessAsync(process);

                    return RedirectToAction("UploadAttendanceAndSalary");
                }

                if (viewModel.SelectedTime.Month < DateTime.Today.Month || viewModel.SelectedTime > completedDate)
                {
                    return RedirectToAction("EmployeeMonthlyPayroll");
                }
            }

            viewModel.Companies = new SelectList(baseService.GetData<Company>(new GetAllCompanyCriteria { IsActive = true }), nameof(Company.CompanyId), nameof(Company.Code));
            return View("Index", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> DownloadAttendance()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\\Template - Attendance (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> DownloadDSTNetSalary()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\\Template - DST Salary (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public IActionResult UploadAttendanceAndSalary()
        {
            return View(new UploadAttendanceAndSalaryViewModel());
        }
    }
}