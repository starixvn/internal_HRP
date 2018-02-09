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
            { (int)FieldType.BasicSalaryChangeReason, new Field   {
                    FieldId = (int)FieldType.BasicSalaryChangeReason,
                    FieldName = ViewLang.BasicSalaryChangeReason,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllRefResource,
                    LinkCreateItem =  DataConfigurationActionLink.CreateRefResource,
                    LinkUpdateItem =  DataConfigurationActionLink.UpdateRefResource,
                    RefName = ReferenceResource.BasicSalaryChangeReason }
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
            }
        };

        public static Dictionary<int, Field> Fields { get => fields; }
    }
}