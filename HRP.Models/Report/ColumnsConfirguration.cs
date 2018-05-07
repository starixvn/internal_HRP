namespace HRP.Models.Report
{
    using System.Collections.Generic;
    using HRP.Core.Constants;

    public static class ColumnsConfirguration
    {
        private static readonly IEnumerable<DynamicReportItem> dynamicReportItems = new List<DynamicReportItem>
        {
            new DynamicReportItem
            {
                Id = 1,
                Name = "Full Name",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 2,
                Name = "Nick Name",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 3,
                Name = "Gender",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 4,
                Name = "Date of Birth",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 5,
                Name = "Place of Birth",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 6,
                Name = "Nationality",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 7,
                Name = "NRIC No",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 8,
                Name = "Marital Status",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 9,
                Name = "No Of Children",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 10,
                Name = "Telephone Overseas",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 11,
                Name = "Telephone Local",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 12,
                Name = "Telephone Other",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 13,
                Name = "Passport No",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 14,
                Name = "Passport Date of Issue",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 15,
                Name = "Passport Expired Date",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 16,
                Name = "Passport Place of Issue",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 17,
                Name = "Skype Account",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 18,
                Name = "Personal Email Address",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 19,
                Name = "Permanent Address (Overseas)",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 20,
                Name = "Resident Address (Local)",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 21,
                Name = "Mailing Address",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 22,
                Name = "Emergency Contact Name",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 23,
                Name = "Emergency Contact Relationship",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 24,
                Name = "Emergency Contact Address",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 25,
                Name = "Emergency Contact Telephone No1",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 26,
                Name = "Emergency Contact Telephone No2",
                Group = DynamicReportItemGroup.PersonalDetails
            },
            new DynamicReportItem
            {
                Id = 27,
                Name = "Company BVI",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 28,
                Name = "Company BVI - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 29,
                Name = "Company BVI - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 30,
                Name = "Company Local",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 31,
                Name = "Company Local - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 32,
                Name = "Company Local - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 33,
                Name = "Job Title BVI",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 34,
                Name = "Job Title BVI - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 35,
                Name = "Job Title BVI - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 36,
                Name = "Job Title Local",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 37,
                Name = "Job Title Local - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 38,
                Name = "Job Title Local - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 39,
                Name = "Date Commencement Group - Start Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 40,
                Name = "Date Commencement Group - End Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 41,
                Name = "Date Commencement Group - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 42,
                Name = "Date Commencement Company - Start",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 43,
                Name = "Date Commencement Company - End",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 44,
                Name = "Date Commencement Company - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 45,
                Name = "Length Of Service - Group",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 46,
                Name = "Length Of Service - Company",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 47,
                Name = "ID No",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 48,
                Name = "ID No - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 49,
                Name = "ID No - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 50,
                Name = "Employment Type",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 51,
                Name = "Employment Status",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 52,
                Name = "Employment Status - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 53,
                Name = "Job Category",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 54,
                Name = "Job Category - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 55,
                Name = "Job Category - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 56,
                Name = "Grade Level",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 57,
                Name = "Job Grade",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 58,
                Name = "Grade Level - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 59,
                Name = "Grade Level - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 60,
                Name = "Division",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 61,
                Name = "Division - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 62,
                Name = "Division - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 63,
                Name = "Department",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 64,
                Name = "Department - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 65,
                Name = "Department - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 66,
                Name = "Cost Center",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 67,
                Name = "Cost Center - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 68,
                Name = "Cost Center - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 69,
                Name = "Section",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 70,
                Name = "Section - Effective Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 71,
                Name = "Section - Reason",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 72,
                Name = "Email Address",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 73,
                Name = "Work Location - Company",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 74,
                Name = "Work Location - City",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 75,
                Name = "Work Location - Office",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 76,
                Name = "Work Permit - Company",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 77,
                Name = "Work Permit - Type 1",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 78,
                Name = "Work Permit - Type 1 Start Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 79,
                Name = "Work Permit - Type 1 End Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 80,
                Name = "Work Permit - Type 2",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 81,
                Name = "Work Permit - Type 2 Start Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 82,
                Name = "Work Permit - Type 2 End Date",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 83,
                Name = "Tax Declaration - DST",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 84,
                Name = "Tax Declaration - DBT",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 85,
                Name = "Local Statutory - Tax ID No",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 86,
                Name = "Local Statutory - SSS No",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 87,
                Name = "Local Statutory - Health Ins No",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 88,
                Name = "Local Statutory - HDMF No",
                Group = DynamicReportItemGroup.Employement
            }
        };

        private static readonly Dictionary<int, IEnumerable<ColumnGroup>> columns = new Dictionary<int, IEnumerable<ColumnGroup>>
        {
            {
                1,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 1,
                        GroupName = "ID No",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.EmployeeId),
                               HeaderName = "ID No"
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
                        GroupId = 2,
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
                3,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 3,
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
                4,
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
                5,
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
                6,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 6,
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
                7,
                new List<ColumnGroup>
                {
                    new ColumnGroup
                    {
                        GroupId = 7,
                        GroupName = "Basic Salary",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.Salary),
                               HeaderName = "Actual Basic Salary Amount",
                               IsEncrypted = true
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.CurrencyCode),
                               HeaderName = "Actual Basic Salary Currency",
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
                        GroupId = 12,
                        GroupName = "Subsistence",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SubsistenceAmount),
                               HeaderName = "Subsistence Amount",
                               IsEncrypted = true
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SubsistenceCurrency),
                               HeaderName = "Subsistence Currency",
                           }
                        }
                    },
                    new ColumnGroup
                    {
                        GroupId = 12,
                        GroupName = "HomePassage",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HomePassageAmount),
                               HeaderName = "HomePassage Amount",
                               IsEncrypted = true
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HomePassageCurrency),
                               HeaderName = "HomePassage Currency",
                           }
                        }
                    },
                    new ColumnGroup
                    {
                        GroupId = 12,
                        GroupName = "Housing",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HousingAmount),
                               HeaderName = "Housing Amount",
                               IsEncrypted = true
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HousingCurrency),
                               HeaderName = "HomePassage Currency",
                           }
                        }
                    },
                    new ColumnGroup
                    {
                        GroupId = 12,
                        GroupName = "Mobile Phone",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.MobilePhoneAmount),
                               HeaderName = "Mobile Phone Amount",
                               IsEncrypted = true
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.MobilePhoneCurrency),
                               HeaderName = "Mobile Phone Currency",
                           }
                        }
                    },
                    new ColumnGroup
                    {
                        GroupId = 12,
                        GroupName = "HPI",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HPIAmount),
                               HeaderName = "HPI Amount",
                               IsEncrypted = true
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.HPICurrency),
                               HeaderName = "HPI Currency",
                           }
                        }
                    },
                    new ColumnGroup
                    {
                        GroupId = 12,
                        GroupName = "Duty Meal",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DutyMealAmount),
                               HeaderName = "Duty Meal Amount",
                               IsEncrypted = true
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.DutyMealCurrency),
                               HeaderName = "Duty Meal Currency",
                           }
                        }
                    },
                    new ColumnGroup
                    {
                        GroupId = 12,
                        GroupName = "Secondment",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SecondmentAmount),
                               HeaderName = "Secondment Amount",
                               IsEncrypted = true
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.SecondmentCurrency),
                               HeaderName = "Secondment Currency",
                           }
                        }
                    },
                    new ColumnGroup
                    {
                        GroupId = 12,
                        GroupName = "Others",
                        Columns = new List<MappingColumn>
                        {
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.OthersAmount),
                               HeaderName = "Others Amount",
                               IsEncrypted = true
                           },
                           new MappingColumn
                           {
                               PropertyName = nameof(DynamicReportModel.OthersCurrency),
                               HeaderName = "Others Currency",
                           }
                        }
                    }
                }
            }
        };

        public static Dictionary<int, IEnumerable<ColumnGroup>> Columns { get => columns; }

        public static IEnumerable<DynamicReportItem> DynamicReportItems => dynamicReportItems;
    }
}