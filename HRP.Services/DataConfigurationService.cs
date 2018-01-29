namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Company;
    using HRP.Criteria.Division;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Resources;
    using HRP.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DataConfigurationService : IDataConfigurationService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public DataConfigurationService(IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public IEnumerable<Field> GetDataForIndexPage()
        {
            var fieldList = new List<Field>();
            fieldList.Add(new Field
            {
                FieldId = (int)FieldType.CompanyBVI,
                FieldName = ViewLang.CompanyBVI,
                LinkGetAllItem = DataConfigurationActionLink.GetAllCompanyBVI
            });

            fieldList.Add(new Field
            {
                FieldId = (int)FieldType.CompanyLocal,
                FieldName = ViewLang.CompanyLocal,
                LinkGetAllItem = DataConfigurationActionLink.GetAllCompanyLocal
            });

            fieldList.Add(new Field
            {
                FieldId = (int)FieldType.Division,
                FieldName = ViewLang.Division,
                LinkGetAllItem = DataConfigurationActionLink.GetAllDivision
            });

            return fieldList.OrderBy(x => x.FieldName);
        }

        #region CompanyBVI

        public async Task<IEnumerable<SimpleItem>> GetAllCompanyBVIAsync()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria()).ConfigureAwait(false);

            if (companies.Any())
            {
                var result = companies.Select(c => new SimpleItem
                {
                    ItemId = c.CompanyId,
                    ItemName = c.Code,
                    Description = c.Name,
                    IsActive = c.IsActive
                });

                return result;
            }

            return new List<SimpleItem>();
        }

        public async Task<bool> IsExistedCompanyBVIAsync(SimpleItem item)
        {
            var isExisted = (await baseService.GetDataAsync<int>(new CheckCompanyExistsCriteria { CompanyId = item.ItemId, Code = item.ItemName }).ConfigureAwait(false)).Any();

            return isExisted;
        }

        public async Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item)
        {
            var criteria = new CreateCompanyCriteria
            {
                Code = item.ItemName,
                Name = item.Description,
                IsActive = item.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Code);
            }

            return commandResult;
        }

        public async Task<SimpleItem> GetCompanyBVIByIdAsync(int id)
        {
            var criteria = new GetCompanyByIdCriteria
            {
                CompanyId = id
            };

            var company = await baseService.GetDataEntityAsync<Company>(criteria).ConfigureAwait(false);

            if (company != null)
            {
                return new SimpleItem
                {
                    ItemId = id,
                    ItemName = company.Code,
                    Description = company.Name,
                    IsActive = company.IsActive
                };
            }

            return null;
        }

        public async Task<CommandResult> UpdateCompanyBVIAsync(SimpleItem item)
        {
            var criteria = new UpdateCompanyCriteria
            {
                CompanyId = item.ItemId,
                Code = item.ItemName,
                Name = item.Description,
                IsActive = item.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion CompanyBVI

        public async Task<DivisionTableViewModel> GetAllDivisionAsync()
        {
            var criteria = new GetAllDivisionCriteria();

            if (criteria.IsValid())
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    using (var multi = db.QueryMultiple(criteria))
                    {
                        var divisions = await multi.ReadAsync<DivisionItem>().ConfigureAwait(false);
                        var companies = await multi.ReadAsync<Company>().ConfigureAwait(false);

                        var result = new DivisionTableViewModel
                        {
                            Divisions = divisions,
                            CompanyList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code))
                        };

                        return result;
                    }
                }
            }

            return new DivisionTableViewModel();
        }
    }
}