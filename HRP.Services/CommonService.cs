namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.BankAccount;
    using HRP.Criteria.Currency;
    using HRP.Models;
    using HRP.Models.BankAccount;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CommonService(
        IBaseService baseService,
        ISessionService sessionService) : ICommonService
    {
        public async Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] refNames, bool? isActive = true)
        {
            IEnumerable<ReferenceResourceModel> refResources = new List<ReferenceResourceModel>();
            if (refNames != null)
            {
                var criteria = new GetRefResourceByNamesCriteria
                {
                    RefNames = string.Join(",", refNames),
                    IsActive = isActive
                };

                refResources = await baseService.GetDataAsync<ReferenceResourceModel>(criteria);
            }

            return refResources;
        }

        public IEnumerable<ReferenceResourceModel> FilterReferenceResources(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            var referenceResources = new List<ReferenceResourceModel>();
            if (refResources != null && !string.IsNullOrWhiteSpace(refName))
            {
                referenceResources = refResources.Where(x => x.RefName == refName).ToList();
            }

            return referenceResources;
        }

        public async Task<IEnumerable<Company>> GetCompanyByFunctionKeyAsync(string functionKey)
        {
            var criteria = new GetCompanyByFunctionKeyCriteria
            {
                UserId = sessionService.UserId,
                FunctionKey = functionKey
            };

            return await baseService.GetDataAsync<Company>(criteria);
        }

        public async Task<IEnumerable<Division>> GetDivisionByCompanyIdAsync(int companyId, bool? isActive = true)
        {
            var criteria = new GetDivisionByCompanyIdCriteria { CompanyId = companyId, IsActive = isActive };
            return await baseService.GetDataAsync<Division>(criteria);
        }

        public async Task<IEnumerable<Department>> GetDepartmentByDivisionIdAsync(int divisionId, bool? isActive = true)
        {
            var criteria = new GetDepartmentByDivisionIdCriteria { DivisionId = divisionId, IsActive = isActive };
            return await baseService.GetDataAsync<Department>(criteria);
        }

        public async Task<IEnumerable<CostCenter>> GetCostCenterByDepartmentIdAsync(int departmentId, bool? isActive = true)
        {
            var criteria = new GetCostCenterByDepartmentIdCriteria { DepartmentId = departmentId, IsActive = isActive };
            return await baseService.GetDataAsync<CostCenter>(criteria);
        }

        public async Task<IEnumerable<Section>> GetSectionByCostCenterIdAsync(int costCenterId, bool? isActive = true)
        {
            var criteria = new GetSectionByCostCenterIdCriteria { CostCenterId = costCenterId, IsActive = isActive };
            return await baseService.GetDataAsync<Section>(criteria);
        }

        public async Task<IEnumerable<GradeLevel>> GetGradeLevelByJobCategoryIdAsync(int jobCategoryId, bool? isActive = true)
        {
            var criteria = new GetGradeLevelByJobCategoryIdCriteria { JobCategoryId = jobCategoryId, IsActive = isActive };
            return await baseService.GetDataAsync<GradeLevel>(criteria);
        }

        public async Task<IEnumerable<JobGrade>> GetJobGradeByGradeLevelIdAsync(int gradeLevelId, bool? isActive = true)
        {
            var criteria = new GetJobGradeByGradeLevelIdCriteria { GradeLevelId = gradeLevelId, IsActive = true };
            return await baseService.GetDataAsync<JobGrade>(criteria);
        }

        public async Task<IEnumerable<JobCategory>> GetAllJobCategoryAsync(bool? isActive = true)
        {
            var criteria = new GetAllJobCategoryCriteria { IsActive = isActive };
            return await baseService.GetDataAsync<JobCategory>(criteria);
        }

        public async Task<IEnumerable<BankAccount>> GetEmployeeBankAccountAsync(int employeeId, int bankTypeId, bool? isActive = true)
        {
            var criteria = new SearchBankAccountCriteria
            {
                EmployeeId = employeeId,
                BankTypeId = bankTypeId,
                IsActive = isActive
            };

            return await baseService.GetDataAsync<BankAccount>(criteria);
        }

        public SelectList GetSelectList<T>(IEnumerable<T> items, string dataValueField, string dataTextField)
        {
            return new SelectList(items, dataValueField, dataTextField);
        }

        public async Task<IEnumerable<Currency>> GetCurrencyAsync(bool? isActive = true)
        {
            var criteria = new GetAllCurrencyCriteria
            {
                IsActive = isActive
            };
            return await baseService.GetDataAsync<Currency>(criteria);
        }
    }
}