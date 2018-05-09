namespace HRP.Models.Report
{
    using System.Collections.Generic;

    public static partial class ColumnsConfirguration
    {
        private static readonly Dictionary<int, IEnumerable<ColumnGroup>> columns = new Dictionary<int, IEnumerable<ColumnGroup>>
        {
            {
                1,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Full Name",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullName),
                               HeaderName = "Full Name"
                           }
                        }
                    }
                }
            },
            {
                2,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Nick Name",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Nickname),
                               HeaderName = "Nick Name"
                           }
                        }
                    }
                }
            },
            {
                3,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Gender",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Gender),
                               HeaderName = "Gender"
                           }
                        }
                    }
                }
            },
            {
                4,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Date of Birth",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DateOfBirth),
                               HeaderName = "Date of Birth"
                           }
                        }
                    }
                }
            },
            {
                5,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Place of Birth",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.PlaceOfBirth),
                               HeaderName = "Place of Birth"
                           }
                        }
                    }
                }
            },
            {
                6,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Nationality",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Nationality),
                               HeaderName = "Nationality"
                           }
                        }
                    }
                }
            },
            {
                7,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "NRIC No",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NRICNo),
                               HeaderName = "NRIC No"
                           }
                        }
                    }
                }
            },
            {
                8,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Marital Status",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.MaritalStatus),
                               HeaderName = "Marital Status"
                           }
                        }
                    }
                }
            },
            {
                9,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "No Of Children",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NoOfChildren),
                               HeaderName = "No Of Children"
                           }
                        }
                    }
                }
            },
            {
                10,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Telephone Overseas",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.OverseasPhoneNo),
                               HeaderName = "Telephone Overseas"
                           }
                        }
                    }
                }
            },
            {
                11,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Telephone Local",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.LocalPhoneNo),
                               HeaderName = "Telephone Local"
                           }
                        }
                    }
                }
            },
            {
                12,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Telephone Other",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.OtherPhoneNo),
                               HeaderName = "Telephone Other"
                           }
                        }
                    }
                }
            },
            {
                13,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Passport No",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.PassportNo),
                               HeaderName = "Passport No"
                           }
                        }
                    }
                }
            },
            {
                14,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Passport Date of Issue",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DateIssue),
                               HeaderName = "Passport Date of Issue"
                           }
                        }
                    }
                }
            },
            {
                15,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Passport Expired Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.ExpiredDate),
                               HeaderName = "Passport Expired Date"
                           }
                        }
                    }
                }
            },
            {
                16,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Passport Place Of Issue",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.PassportNo),
                               HeaderName = "Passport Place of Issue"
                           }
                        }
                    }
                }
            },
            {
                17,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Skype Account",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Skype),
                               HeaderName = "Skype Account"
                           }
                        }
                    }
                }
            },
            {
                18,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Personal Email Address",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.PersonalEmail),
                               HeaderName = "Personal Email Address"
                           }
                        }
                    }
                }
            },
            {
                19,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Permanent Address (Overseas)",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.PermanentAddress),
                               HeaderName = "Permanent Address (Overseas)"
                           }
                        }
                    }
                }
            },
            {
                20,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Resident Address (Local)",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.ResidenceAddress),
                               HeaderName = "Resident Address (Local)"
                           }
                        }
                    }
                }
            },
            {
                21,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Mailing Address",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.MailingAddress),
                               HeaderName = "Mailing Address"
                           }
                        }
                    }
                }
            },
            {
                22,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Emergency Contact Name",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EmergencyContactName),
                               HeaderName = "Emergency Contact Name"
                           }
                        }
                    }
                }
            },
            {
                23,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Emergency Contact Relationship",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EmergencyContactRelationship),
                               HeaderName = "Emergency Contact Relationship"
                           }
                        }
                    }
                }
            },
            {
                24,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Emergency Contact Address",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EmergencyContactAddress),
                               HeaderName = "Emergency Contact Address"
                           }
                        }
                    }
                }
            },
            {
                25,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Emergency Contact Telephone No1",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EmergencyContactPhoneNo1),
                               HeaderName = "Emergency Contact Telephone No1"
                           }
                        }
                    }
                }
            },
            {
                26,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Emergency Contact Telephone No2",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EmergencyContactPhoneNo2),
                               HeaderName = "Emergency Contact Telephone No2"
                           }
                        }
                    }
                }
            },
            {
                27,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Company BVI",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Company),
                               HeaderName = "Emergency Contact Telephone No2"
                           }
                        }
                    }
                }
            },
            {
                28,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Company BVI - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CompanyChangeDate),
                               HeaderName = "Company BVI - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                29,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Company BVI - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CompanyReason),
                               HeaderName = "Company BVI - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                30,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Company Local",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.LocalCompany),
                               HeaderName = "Company Local"
                           }
                        }
                    }
                }
            },
            {
                31,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Company Local - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.LocalCompanyChangeDate),
                               HeaderName = "Company Local - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                32,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Company Local - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.LocalCompanyReason),
                               HeaderName = "Company Local - Reason"
                           }
                        }
                    }
                }
            },
            {
                33,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Title BVI",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.JobTitle),
                               HeaderName = "Job Title BVI"
                           }
                        }
                    }
                }
            },
            {
                34,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Title BVI - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.JobTitleChangeDate),
                               HeaderName = "Job Title BVI - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                35,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Title BVI - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.JobTitleReason),
                               HeaderName = "Job Title BVI - Reason"
                           }
                        }
                    }
                }
            },
            {
                36,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Title Local",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.LocalJobTitle),
                               HeaderName = "Job Title Local"
                           }
                        }
                    }
                }
            },
            {
                37,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Title Local - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.LocalJobTitleChangeDate),
                               HeaderName = "Job Title Local - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                38,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Title Local - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.LocalJobTitleReason),
                               HeaderName = "Job Title Local - Reason"
                           }
                        }
                    }
                }
            },
            {
                39,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Date Commencement Group - Start Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.GroupStartDate),
                               HeaderName = "Date Commencement Group - Start Date"
                           }
                        }
                    }
                }
            },
            {
                40,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Date Commencement Group - End Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.GroupEndDate),
                               HeaderName = "Date Commencement Group - End Date"
                           }
                        }
                    }
                }
            },
            {
                41,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Date Commencement Group - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.GroupDateReason),
                               HeaderName = "Date Commencement Group - Reason"
                           }
                        }
                    }
                }
            },
            {
                42,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Date Commencement Company - Start Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CompanyStartDate),
                               HeaderName = "Date Commencement Company - Start Date"
                           }
                        }
                    }
                }
            },
            {
                43,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Date Commencement Company - End Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CompanyEndDate),
                               HeaderName = "Date Commencement Company - End Date"
                           }
                        }
                    }
                }
            },
            {
                44,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Date Commencement Company - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CompanyReason),
                               HeaderName = "Date Commencement Company - Reason"
                           }
                        }
                    }
                }
            },
            {
                45,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Length Of Service - Group",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.GroupLengthOfService),
                               HeaderName = "Length Of Service - Group"
                           }
                        }
                    }
                }
            },
            {
                46,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Length Of Service - Company",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CompanyLengthOfService),
                               HeaderName = "Length Of Service - Company"
                           }
                        }
                    }
                }
            },
            {
                48,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "ID No - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.IDNoChangeDate),
                               HeaderName = "ID No - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                49,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "ID No - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.IDNoChangeReason),
                               HeaderName = "ID No - Reason"
                           }
                        }
                    }
                }
            },
            {
                50,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Employment Type",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EmploymentType),
                               HeaderName = "Employment Type"
                           }
                        }
                    }
                }
            },
            {
                51,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Employment Status",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EmploymentStatus),
                               HeaderName = "Employment Status"
                           }
                        }
                    }
                }
            },
            {
                52,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Employment Status - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EmploymentStatusChangeDate),
                               HeaderName = "Employment Status - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                53,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Category",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.JobCategory),
                               HeaderName = "Job Category"
                           }
                        }
                    }
                }
            },
            {
                54,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Category - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.JobCategoryChangeDate),
                               HeaderName = "Job Category - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                55,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Category - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.JobCategoryReason),
                               HeaderName = "Job Category - Reason"
                           }
                        }
                    }
                }
            },
            {
                56,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Grade Level",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.GradeLevel),
                               HeaderName = "Grade Level"
                           }
                        }
                    }
                }
            },
            {
                57,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Job Grade",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.JobGrade),
                               HeaderName = "Job Grade"
                           }
                        }
                    }
                }
            },
            {
                58,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Grade Level - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.JobGradeChangeDate),
                               HeaderName = "Grade Level - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                59,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Grade Level - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.JobGradeReason),
                               HeaderName = "Grade Level - Reason"
                           }
                        }
                    }
                }
            },
            {
                60,
                new List<ColumnGroup>
                {
                   new ColumnGroup
                   {
                       GroupId = 4,
                       GroupName = "Division",
                       Columns = new List<MappingColumn>
                       {
                          new MappingColumn{
                              PropertyName = nameof(DynamicReportModel.Division),
                              HeaderName = "Division"
                          }
                       }
                   }
                }
            },
            {
                61,
                new List<ColumnGroup>
                {
                   new ColumnGroup
                   {
                       GroupId = 4,
                       GroupName = "Division - Effective Date",
                       Columns = new List<MappingColumn>
                       {
                          new MappingColumn{
                              PropertyName = nameof(DynamicReportModel.DivisionChangeDate),
                              HeaderName = "Division - Effective Date"
                          }
                       }
                   }
                }
            },
            {
                62,
                new List<ColumnGroup>
                {
                   new ColumnGroup
                   {
                       GroupId = 4,
                       GroupName = "Division - Reason",
                       Columns = new List<MappingColumn>
                       {
                          new MappingColumn{
                              PropertyName = nameof(DynamicReportModel.DivisionReason),
                              HeaderName = "Division - Reason"
                          }
                       }
                   }
                }
            },
            {
                63,
                new List<ColumnGroup>
                {
                   new ColumnGroup
                   {
                       GroupId = 5,
                       GroupName = "Department",
                       Columns = new List<MappingColumn>
                       {
                          new MappingColumn
                          {
                              PropertyName = nameof(DynamicReportModel.Department),
                              HeaderName = "Department"
                          }
                       }
                   }
                }
            },
            {
                64,
                new List<ColumnGroup>
                {
                   new ColumnGroup
                   {
                       GroupId = 5,
                       GroupName = "Department - Effective Date",
                       Columns = new List<MappingColumn>
                       {
                          new MappingColumn
                          {
                              PropertyName = nameof(DynamicReportModel.DepartmentChangeDate),
                              HeaderName = "Department - Effective Date"
                          }
                       }
                   }
                }
            },
            {
                65,
                new List<ColumnGroup>
                {
                   new ColumnGroup
                   {
                       GroupId = 5,
                       GroupName = "Department - Reason",
                       Columns = new List<MappingColumn>
                       {
                          new MappingColumn
                          {
                              PropertyName = nameof(DynamicReportModel.DepartmentReason),
                              HeaderName = "Department - Reason"
                          }
                       }
                   }
                }
            },
            {
                66,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Cost Center",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CostCenter),
                               HeaderName = "Cost Center"
                           }
                        }
                    }
                }
            },
            {
                67,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Cost Center - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CostCenterChangeDate),
                               HeaderName = "Cost Center - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                68,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Cost Center - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CostCenterReason),
                               HeaderName = "Cost Center - Reason"
                           }
                        }
                    }
                }
            },
            {
                69,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Section",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Section),
                               HeaderName = "Section"
                           }
                        }
                    }
                }
            },
            {
                70,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Section - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SectionChangeDate),
                               HeaderName = "Section - Effective Date"
                           }
                        }
                    }
                }
            },
            {
                71,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Section - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SectionReason),
                               HeaderName = "Section - Reason"
                           }
                        }
                    }
                }
            },
            {
                72,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Email Address",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Email),
                               HeaderName = "Email Address"
                           }
                        }
                    }
                }
            },
            {
                73,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Location - Country",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Country),
                               HeaderName = "Work Location - Country"
                           }
                        }
                    }
                }
            },
            {
                74,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Location - City",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.City),
                               HeaderName = "Work Location - City"
                           }
                        }
                    }
                }
            },
            {
                75,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Location - Office",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Office),
                               HeaderName = "Work Location - Office"
                           }
                        }
                    }
                }
            },
            {
                76,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Permit - Company",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.WorkPermitCompany),
                               HeaderName = "Work Permit - Company"
                           }
                        }
                    }
                }
            },
            {
                77,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Permit - Type 1",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.WorkPermitType1),
                               HeaderName = "Work Permit - Type 1"
                           }
                        }
                    }
                }
            },
            {
                78,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Permit - Type 1 Start Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.StartDateType1),
                               HeaderName = "Work Permit - Type 1 Start Date"
                           }
                        }
                    }
                }
            },
            {
                79,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Permit - Type 1 End Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EndDateType1),
                               HeaderName = "Work Permit - Type 1 End Date"
                           }
                        }
                    }
                }
            },
            {
                80,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Permit - Type 2",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.WorkPermitType2),
                               HeaderName = "Work Permit - Type 2"
                           }
                        }
                    }
                }
            },
            {
                81,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Permit - Type 2 Start Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.StartDateType2),
                               HeaderName = "Work Permit - Type 1 Start Date"
                           }
                        }
                    }
                }
            },
            {
                82,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Work Permit - Type 2 End Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EndDateType2),
                               HeaderName = "Work Permit - Type 2 End Date"
                           }
                        }
                    }
                }
            },
            {
                83,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Tax Declaration - DST",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DSTCompany),
                               HeaderName = "Tax Declaration - DST"
                           }
                        }
                    }
                }
            },
            {
                84,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Tax Declaration - DBT",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DBTCompany),
                               HeaderName = "Tax Declaration - DBT"
                           }
                        }
                    }
                }
            },
            {
                85,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Local Statutory - Tax ID No",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.TaxIDNo),
                               HeaderName = "Local Statutory - Tax ID No"
                           }
                        }
                    }
                }
            },
            {
                86,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Local Statutory - SSS No",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SSSNo),
                               HeaderName = "Local Statutory - SSS No"
                           }
                        }
                    }
                }
            },
            {
                87,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Local Statutory - Health Ins No",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HealthInsNo),
                               HeaderName = "Local Statutory - Health Ins No"
                           }
                        }
                    }
                }
            },
            {
                88,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Local Statutory - HDMF No",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HDMFNo),
                               HeaderName = "Local Statutory - HDMF No"
                           }
                        }
                    }
                }
            },
            {
                89,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "P-File Admin",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNamePFileAdmin),
                               HeaderName = "P-File Admin - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NicknamePFileAdmin),
                               HeaderName = "P-File Admin - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                90,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Paymaster - BVI Salary",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameBVISalary),
                               HeaderName = "Paymaster - BVI Salary - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NickNameBVISalary),
                               HeaderName = "Paymaster - BVI Salary - Nick Name"
                           }

                        }
                    }
                }
            },
            {
                91,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Paymaster - BVI Allowance",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameBVIAllowance),
                               HeaderName = "Paymaster - BVI Allowance - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NickNameBVIAllowance),
                               HeaderName = "Paymaster - BVI Allowance - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                92,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Paymaster - Local Salary",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameLocalSalary),
                               HeaderName = "Paymaster - Local Salary - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NickNameLocalSalary),
                               HeaderName = "Paymaster - Local Salary - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                93,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "EPA - Administrator",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameAdministrator),
                               HeaderName = "EPA - Administrator - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NicknameAdministrator),
                               HeaderName = "EPA - Administrator - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                94,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "EPA - Evaluator #1",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameEvaluator1),
                               HeaderName = "EPA - Evaluator #1 - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NicknameEvaluator1),
                               HeaderName = "EPA - Evaluator #1 - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                95,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "EPA - Evaluator #2",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameEvaluator2),
                               HeaderName = "EPA - Evaluator #2 - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NicknameEvaluator2),
                               HeaderName = "EPA - Evaluator #2 - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                96,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "EPA - Reviewer",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameReviewer),
                               HeaderName = "EPA - Reviewer - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NicknameReviewer),
                               HeaderName = "EPA - Reviewer - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                97,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "EPA - Management",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameManagement),
                               HeaderName = "EPA - Management - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NicknameManagement),
                               HeaderName = "EPA - Management - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                98,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "EPA - Increment",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameIncrement),
                               HeaderName = "EPA - Increment - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NicknameIncrement),
                               HeaderName = "EPA - Increment - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                99,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "EPA - BVI Bonus",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameBVIBonus),
                               HeaderName = "EPA - BVI Bonus - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NicknameBVIBonus),
                               HeaderName = "EPA - BVI Bonus - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                100,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "EPA - Local Bonus",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FullNameLocalBonus),
                               HeaderName = "EPA - Local Bonus - Full Name"
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.NicknameLocalBonus),
                               HeaderName = "EPA - Local Bonus - Nick Name"
                           }
                        }
                    }
                }
            },
            {
                101,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "PDF Password",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.PDFPassword),
                               HeaderName = "PDF Password",
                               IsEncrypted = true
                           }
                        }
                    }
                }
            },
            {
                102,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Basic Salary",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CurrencyCode),
                               HeaderName = "Basic Salary - Currency",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Salary),
                               HeaderName = "Basic Salary - Amount",
                           }
                        }
                    }
                }
            },
            {
                103,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Basic Salary - Effective Date",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EffectiveDate),
                               HeaderName = "Basic Salary - Effective Date",
                           }
                        }
                    }
                }
            },
            {
                104,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Basic Salary - Reason",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Reason),
                               HeaderName = "Basic Salary - Reason",
                           }
                        }
                    }
                }
            },
            {
                105,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Standard Working Day",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.WorkingDays),
                               HeaderName = "Standard Working Day",
                           }
                        }
                    }
                }
            },
            {
                106,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Basic Salary - E-banking",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EbankingCurrencyCode),
                               HeaderName = "Basic Salary - E-banking Currency",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EbankingAmount),
                               HeaderName = "Basic Salary - E-banking Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EbankingAccount),
                               HeaderName = "Basic Salary - E-banking Account",
                           }
                        }
                    }
                }
            },
            {
                107,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Basic Salary - Direct Banking 1",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DirectBanking1CurrencyCode),
                               HeaderName = "Basic Salary - Direct Banking 1 Currency",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DirectBanking1Amount),
                               HeaderName = "Basic Salary - Direct Banking 1 Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DirectBanking1Account),
                               HeaderName = "Basic Salary - Direct Banking 1 Account",
                           }
                        }
                    }
                }
            },
            {
                108,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Basic Salary - Direct Banking 2",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DirectBanking2CurrencyCode),
                               HeaderName = "Basic Salary - Direct Banking 2 Currency",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DirectBanking2Amount),
                               HeaderName = "Basic Salary - Direct Banking 2 Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DirectBanking2Account),
                               HeaderName = "Basic Salary - Direct Banking 1 Account",
                           }
                        }
                    }
                }
            },
            {
                109,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Basic Salary - Cash",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CashCurrencyCode),
                               HeaderName = "Basic Salary - Cash Currency",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CashAmount),
                               HeaderName = "Basic Salary - Cash Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CashAccount),
                               HeaderName = "Basic Salary - Cash Account",
                           }
                        }
                    }
                }
            },
            {
                110,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Allowance",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceCurrencyCode),
                               HeaderName = "Allowance Currency",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceAmount),
                               HeaderName = "Allowance Amount",
                           }
                        }
                    }
                }
            },
            {
                111,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Allowance- Direct Banking 1",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceDirectBanking1CurrencyCode),
                               HeaderName = "Allowance - Direct Banking 1 Currency",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceDirectBanking1Amount),
                               HeaderName = "Allowance - Direct Banking 1 Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceDirectBanking1Account),
                               HeaderName = "Allowance - Direct Banking 1 Account",
                           }
                        }
                    }
                }
            },
            {
                112,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Allowance - Direct Banking 2",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceDirectBanking2CurrencyCode),
                               HeaderName = "Allowance - Direct Banking 2 Currency",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceDirectBanking2Amount),
                               HeaderName = "Allowance - Direct Banking 2 Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceDirectBanking2Account),
                               HeaderName = "Allowance - Direct Banking 1 Account",
                           }
                        }
                    }
                }
            },
            {
                113,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Allowance - Cash",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceCashCurrencyCode),
                               HeaderName = "Allowance - Cash Currency",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceCashAmount),
                               HeaderName = "Allowance - Cash Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.AllowanceCashAccount),
                               HeaderName = "Allowance - Cash Account",
                           }
                        }
                    }
                }
            },
            {
                114,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Subsistence",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SubsistenceAmount),
                               HeaderName = "Subsistence Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SubsistenceCurrencyCode),
                               HeaderName = "Subsistence Currency",
                           }
                        }
                    }
                }
            },
            {
                115,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "HomePassage",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HomePassageAmount),
                               HeaderName = "HomePassage Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HomePassageCurrencyCode),
                               HeaderName = "HomePassage Currency",
                           }
                        }
                    }
                }
            },
            {
                116,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Housing",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HousingAmount),
                               HeaderName = "Housing Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HousingCurrencyCode),
                               HeaderName = "Housing Currency",
                           }
                        }
                    }
                }
            },
            {
                117,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 11,
                        GroupName = "Mobile Phone",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.MobilePhoneAmount),
                               HeaderName = "Mobile Phone Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.MobilePhoneCurrencyCode),
                               HeaderName = "Mobile Phone Currency",
                           }
                        }
                    }
                }
            },
            {
                118,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "HPI",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HPIAmount),
                               HeaderName = "HPI Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HPICurrencyCode),
                               HeaderName = "HPI Currency",
                           }
                        }
                    }
                }
            },
            {
                119,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Duty Meal",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DutyMealAmount),
                               HeaderName = "Duty Meal Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DutyMealCurrencyCode),
                               HeaderName = "Duty Meal Currency",
                           }
                        }
                    }
                }
            },
            {
                120,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Secondment",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SecondmentAmount),
                               HeaderName = "Secondment Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SecondmentCurrencyCode),
                               HeaderName = "Secondment Currency",
                           }
                        }
                    }
                }
            },
            {
                121,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Others",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.OthersAmount),
                               HeaderName = "Others Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.OthersCurrencyCode),
                               HeaderName = "Others Currency",
                           }
                        }
                    }
                }
            },
            {
                122,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Rooming Charge",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.RoomingChargesAmount),
                               HeaderName = "Rooming Charge Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.RoomingChargesCurrencyCode),
                               HeaderName = "Rooming Charge Currency",
                           }
                        }
                    }
                }
            },
            {
                123,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "Fitness Club Membership",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FitnessFirstAmount),
                               HeaderName = "Fitness Club Membership Amount",
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.FitnessFirstCurrencyCode),
                               HeaderName = "Fitness Club Membership Currency",
                           }
                        }
                    }
                }
            }
        };

        public static Dictionary<int, IEnumerable<ColumnGroup>> Columns { get => columns; }
    }
}