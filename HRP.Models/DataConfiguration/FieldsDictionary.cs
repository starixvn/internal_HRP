namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Resources;

    public static class FieldsDictionary
    {
        private static readonly Dictionary<int, Field> fields = new Dictionary<int, Field>
        {
            { (int)FieldType.CompanyBVI, new Field   {
                    FieldId = (int)FieldType.CompanyBVI,
                    FieldName = ViewLang.CompanyBVI,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllCompanyBVI,
                    LinkCreateItem =  DataConfigurationActionLink.CreateCompanyBVI,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateCompanyBVI }
            },
            { (int)FieldType.Division, new Field   {
                    FieldId = (int)FieldType.Division,
                    FieldName = ViewLang.Division,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllDivision ,
                    LinkCreateItem =  DataConfigurationActionLink.CreateDivision,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateDivision }
            },
            { (int)FieldType.Department, new Field   {
                    FieldId = (int)FieldType.Department,
                    FieldName = ViewLang.Department,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllDepartment,
                    LinkCreateItem =  DataConfigurationActionLink.CreateDepartment,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateDepartment }
            },
            { (int)FieldType.CostCenter, new Field   {
                    FieldId = (int)FieldType.CostCenter,
                    FieldName = ViewLang.CostCenter,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllCostCenter,
                    LinkCreateItem =  DataConfigurationActionLink.CreateCostCenter,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateCostCenter }
            },
            { (int)FieldType.Section, new Field   {
                    FieldId = (int)FieldType.Section,
                    FieldName = ViewLang.Section,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllSection,
                    LinkCreateItem =  DataConfigurationActionLink.CreateSection,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateSection }
            },
            { (int)FieldType.JobCategory, new Field   {
                    FieldId = (int)FieldType.JobCategory,
                    FieldName = ViewLang.JobCategory,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllJobCategory,
                    LinkCreateItem =  DataConfigurationActionLink.CreateJobCategory,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateJobCategory }
            },
            { (int)FieldType.GradeLevel, new Field   {
                    FieldId = (int)FieldType.GradeLevel,
                    FieldName = ViewLang.GradeLevel,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllGradeLevel,
                    LinkCreateItem =  DataConfigurationActionLink.CreateGradeLevel,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateGradeLevel }
            },
            { (int)FieldType.JobGrade, new Field   {
                    FieldId = (int)FieldType.JobGrade,
                    FieldName = ViewLang.JobGrade,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllJobGrade,
                    LinkCreateItem =  DataConfigurationActionLink.CreateJobGrade,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateJobGrade }
            },
            { (int)FieldType.Country, new Field   {
                    FieldId = (int)FieldType.Country,
                    FieldName = ViewLang.Country,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllCountry,
                    LinkCreateItem =  DataConfigurationActionLink.CreateCountry,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateCountry }
            },
            { (int)FieldType.City, new Field   {
                    FieldId = (int)FieldType.City,
                    FieldName = ViewLang.City,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllCity,
                    LinkCreateItem =  DataConfigurationActionLink.CreateCity,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateCity }
            },
            { (int)FieldType.Office, new Field   {
                    FieldId = (int)FieldType.Office,
                    FieldName = ViewLang.Office,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllOffice,
                    LinkCreateItem =  DataConfigurationActionLink.CreateOffice,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateOffice }
            },
            { (int)FieldType.CompanyLocal, new Field   {
                    FieldId = (int)FieldType.CompanyLocal,
                    FieldName = ViewLang.CompanyLocal,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.LocalCompany }
            },
            { (int)FieldType.Nationality, new Field   {
                    FieldId = (int)FieldType.Nationality,
                    FieldName = ViewLang.Nationality,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.Nationality }
            },
            { (int)FieldType.NoOfChildren, new Field   {
                    FieldId = (int)FieldType.NoOfChildren,
                    FieldName = ViewLang.NoOfChildren,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.NoOfChildren }
            },
            { (int)FieldType.JobTitleLocal, new Field   {
                    FieldId = (int)FieldType.JobTitleLocal,
                    FieldName = string.Concat(ViewLang.JobTitle, " - ", ViewLang.Local),
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.LocalJobTitle }
            },
            { (int)FieldType.JobTitleChangeReason, new Field   {
                    FieldId = (int)FieldType.JobTitleChangeReason,
                    FieldName = ViewLang.JobTitleChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.JobTitleChangeReason }
            },
            { (int)FieldType.DateCommencementChangeReason, new Field   {
                    FieldId = (int)FieldType.DateCommencementChangeReason,
                    FieldName = ViewLang.DateCommencementChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.DateCommencementChangeReason }
            },
            { (int)FieldType.EmployeeIDChangeReason, new Field   {
                    FieldId = (int)FieldType.EmployeeIDChangeReason,
                    FieldName = ViewLang.EmployeeIDChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.EmployeeIDChangeReason }
            },
            { (int)FieldType.EmploymentType, new Field   {
                    FieldId = (int)FieldType.EmploymentType,
                    FieldName = ViewLang.EmploymentType,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.EmploymentType }
            },
            { (int)FieldType.EmploymentStatus, new Field   {
                    FieldId = (int)FieldType.EmploymentStatus,
                    FieldName = ViewLang.EmploymentStatus,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.EmploymentStatus }
            },
            { (int)FieldType.JobCategoryChangeReason, new Field   {
                    FieldId = (int)FieldType.JobCategoryChangeReason,
                    FieldName = ViewLang.JobCategoryChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.JobCategoryChangeReason }
            },
            { (int)FieldType.JobGradeChangeReason, new Field   {
                    FieldId = (int)FieldType.JobGradeChangeReason,
                    FieldName = ViewLang.JobGradeChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.JobGradeChangeReason }
            },
            { (int)FieldType.DivisionChangeReason, new Field   {
                    FieldId = (int)FieldType.DivisionChangeReason,
                    FieldName = ViewLang.DivisionChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.DivisionChangeReason }
            },
            { (int)FieldType.DepartmentChangeReason, new Field   {
                    FieldId = (int)FieldType.DepartmentChangeReason,
                    FieldName = ViewLang.DepartmentChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.DepartmentChangeReason }
            },
            { (int)FieldType.SectionChangeReason, new Field   {
                    FieldId = (int)FieldType.SectionChangeReason,
                    FieldName = ViewLang.SectionChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.SectionChangeReason }
            },
            { (int)FieldType.CostCenterChangeReason, new Field   {
                    FieldId = (int)FieldType.CostCenterChangeReason,
                    FieldName = ViewLang.CostCenterChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.CostCenterChangeReason }
            },
            { (int)FieldType.WorkPermitType1, new Field   {
                    FieldId = (int)FieldType.WorkPermitType1,
                    FieldName = ViewLang.WorkPermitType1,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.WorkPermitType1 }
            },
            { (int)FieldType.WorkPermitType2, new Field   {
                    FieldId = (int)FieldType.WorkPermitType2,
                    FieldName = ViewLang.WorkPermitType2,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.WorkPermitType2 }
            },
            { (int)FieldType.TypeOfBankAccount, new Field   {
                    FieldId = (int)FieldType.TypeOfBankAccount,
                    FieldName = ViewLang.TypeOfBankAccount,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.TypeOfBankAccount }
            },
            { (int)FieldType.PaymentMethodCash, new Field   {
                    FieldId = (int)FieldType.PaymentMethodCash,
                    FieldName = ViewLang.PaymentMethodCash,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.PaymentMethodCash }
            },
            { (int)FieldType.ContraCoAccount, new Field   {
                    FieldId = (int)FieldType.ContraCoAccount,
                    FieldName = ViewLang.ContraCoAccount,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.ContraCoAccount }
            },
            { (int)FieldType.CompanyBVIChangeReason, new Field   {
                    FieldId = (int)FieldType.CompanyBVIChangeReason,
                    FieldName = ViewLang.CompanyBVIChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.CompanyBVIChangeReason }
            },
            { (int)FieldType.CompanyLocalChangeReason, new Field   {
                    FieldId = (int)FieldType.CompanyLocalChangeReason,
                    FieldName = ViewLang.CompanyLocalChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.CompanyLocalChangeReason }
            },
            { (int)FieldType.Currency, new Field   {
                    FieldId = (int)FieldType.Currency,
                    FieldName = ViewLang.Currency,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllCurrency,
                    LinkCreateItem =  DataConfigurationActionLink.CreateCurrency,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateCurrency}
            }
        };

        public static Dictionary<int, Field> Fields { get => fields; }
    }
}