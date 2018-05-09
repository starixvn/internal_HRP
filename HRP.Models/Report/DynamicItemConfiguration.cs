namespace HRP.Models.Report
{
    using System.Collections.Generic;
    using HRP.Core.Constants;

    public static partial class ColumnsConfirguration
    {
        private static readonly IEnumerable<DynamicReportGroup> dynamicReportGroups = new List<DynamicReportGroup>
        {
            new DynamicReportGroup
            {
                GroupdId = 1,
                Name = DynamicReportItemGroup.PersonalDetails,
                Items = new List<DynamicReportItem>
                {
                    new DynamicReportItem
                    {
                        Id = 1,
                        Name = "Full Name",
                    },
                    new DynamicReportItem
                    {
                        Id = 2,
                        Name = "Nick Name",
                    },
                    new DynamicReportItem
                    {
                        Id = 3,
                        Name = "Gender",
                    },
                    new DynamicReportItem
                    {
                        Id = 4,
                        Name = "Date of Birth",
                    },
                    new DynamicReportItem
                    {
                        Id = 5,
                        Name = "Place of Birth",
                    },
                    new DynamicReportItem
                    {
                        Id = 6,
                        Name = "Nationality",
                    },
                    new DynamicReportItem
                    {
                        Id = 7,
                        Name = "NRIC No",
                    },
                    new DynamicReportItem
                    {
                        Id = 8,
                        Name = "Marital Status",
                    },
                    new DynamicReportItem
                    {
                        Id = 9,
                        Name = "No Of Children",
                    },
                    new DynamicReportItem
                    {
                        Id = 10,
                        Name = "Telephone Overseas",
                    },
                    new DynamicReportItem
                    {
                        Id = 11,
                        Name = "Telephone Local",
                    },
                    new DynamicReportItem
                    {
                        Id = 12,
                        Name = "Telephone Other",
                    },
                    new DynamicReportItem
                    {
                        Id = 13,
                        Name = "Passport No",
                    },
                    new DynamicReportItem
                    {
                        Id = 14,
                        Name = "Passport Date of Issue",
                    },
                    new DynamicReportItem
                    {
                        Id = 15,
                        Name = "Passport Expired Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 16,
                        Name = "Passport Place of Issue",
                    },
                    new DynamicReportItem
                    {
                        Id = 17,
                        Name = "Skype Account",
                    },
                    new DynamicReportItem
                    {
                        Id = 18,
                        Name = "Personal Email Address",
                    },
                    new DynamicReportItem
                    {
                        Id = 19,
                        Name = "Permanent Address (Overseas)",
                    },
                    new DynamicReportItem
                    {
                        Id = 20,
                        Name = "Resident Address (Local)",
                    },
                    new DynamicReportItem
                    {
                        Id = 21,
                        Name = "Mailing Address",
                    },
                    new DynamicReportItem
                    {
                        Id = 22,
                        Name = "Emergency Contact Name",
                    },
                    new DynamicReportItem
                    {
                        Id = 23,
                        Name = "Emergency Contact Relationship",
                    },
                    new DynamicReportItem
                    {
                        Id = 24,
                        Name = "Emergency Contact Address",
                    },
                    new DynamicReportItem
                    {
                        Id = 25,
                        Name = "Emergency Contact Telephone No1",
                    },
                    new DynamicReportItem
                    {
                        Id = 26,
                        Name = "Emergency Contact Telephone No2",
                    }
                }
            },
            new DynamicReportGroup
            {
                GroupdId = 2,
                Name = DynamicReportItemGroup.Employement,
                Items = new List<DynamicReportItem>
                {
                    new DynamicReportItem
                    {
                        Id = 27,
                        Name = "Company BVI",
                    },
                    new DynamicReportItem
                    {
                        Id = 28,
                        Name = "Company BVI - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 29,
                        Name = "Company BVI - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 30,
                        Name = "Company Local",
                    },
                    new DynamicReportItem
                    {
                        Id = 31,
                        Name = "Company Local - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 32,
                        Name = "Company Local - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 33,
                        Name = "Job Title BVI",
                    },
                    new DynamicReportItem
                    {
                        Id = 34,
                        Name = "Job Title BVI - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 35,
                        Name = "Job Title BVI - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 36,
                        Name = "Job Title Local",
                    },
                    new DynamicReportItem
                    {
                        Id = 37,
                        Name = "Job Title Local - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 38,
                        Name = "Job Title Local - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 39,
                        Name = "Date Commencement Group - Start Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 40,
                        Name = "Date Commencement Group - End Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 41,
                        Name = "Date Commencement Group - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 42,
                        Name = "Date Commencement Company - Start",
                    },
                    new DynamicReportItem
                    {
                        Id = 43,
                        Name = "Date Commencement Company - End",
                    },
                    new DynamicReportItem
                    {
                        Id = 44,
                        Name = "Date Commencement Company - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 45,
                        Name = "Length Of Service - Group",
                    },
                    new DynamicReportItem
                    {
                        Id = 46,
                        Name = "Length Of Service - Company",
                    },
                    new DynamicReportItem
                    {
                        Id = 48,
                        Name = "ID No - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 49,
                        Name = "ID No - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 50,
                        Name = "Employment Type",
                    },
                    new DynamicReportItem
                    {
                        Id = 51,
                        Name = "Employment Status",
                    },
                    new DynamicReportItem
                    {
                        Id = 52,
                        Name = "Employment Status - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 53,
                        Name = "Job Category",
                    },
                    new DynamicReportItem
                    {
                        Id = 54,
                        Name = "Job Category - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 55,
                        Name = "Job Category - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 56,
                        Name = "Grade Level",
                    },
                    new DynamicReportItem
                    {
                        Id = 57,
                        Name = "Job Grade",
                    },
                    new DynamicReportItem
                    {
                        Id = 58,
                        Name = "Grade Level - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 59,
                        Name = "Grade Level - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 60,
                        Name = "Division",
                    },
                    new DynamicReportItem
                    {
                        Id = 61,
                        Name = "Division - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 62,
                        Name = "Division - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 63,
                        Name = "Department",
                    },
                    new DynamicReportItem
                    {
                        Id = 64,
                        Name = "Department - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 65,
                        Name = "Department - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 66,
                        Name = "Cost Center",
                    },
                    new DynamicReportItem
                    {
                        Id = 67,
                        Name = "Cost Center - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 68,
                        Name = "Cost Center - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 69,
                        Name = "Section",
                    },
                    new DynamicReportItem
                    {
                        Id = 70,
                        Name = "Section - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 71,
                        Name = "Section - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 72,
                        Name = "Email Address",
                    },
                    new DynamicReportItem
                    {
                        Id = 73,
                        Name = "Work Location - Country",
                    },
                    new DynamicReportItem
                    {
                        Id = 74,
                        Name = "Work Location - City",
                    },
                    new DynamicReportItem
                    {
                        Id = 75,
                        Name = "Work Location - Office",
                    },
                    new DynamicReportItem
                    {
                        Id = 76,
                        Name = "Work Permit - Company",
                    },
                    new DynamicReportItem
                    {
                        Id = 77,
                        Name = "Work Permit - Type 1",
                    },
                    new DynamicReportItem
                    {
                        Id = 78,
                        Name = "Work Permit - Type 1 Start Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 79,
                        Name = "Work Permit - Type 1 End Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 80,
                        Name = "Work Permit - Type 2",
                    },
                    new DynamicReportItem
                    {
                        Id = 81,
                        Name = "Work Permit - Type 2 Start Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 82,
                        Name = "Work Permit - Type 2 End Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 83,
                        Name = "Tax Declaration - DST",
                    },
                    new DynamicReportItem
                    {
                        Id = 84,
                        Name = "Tax Declaration - DBT",
                    },
                    new DynamicReportItem
                    {
                        Id = 85,
                        Name = "Local Statutory - Tax ID No",
                    },
                    new DynamicReportItem
                    {
                        Id = 86,
                        Name = "Local Statutory - SSS No",
                    },
                    new DynamicReportItem
                    {
                        Id = 87,
                        Name = "Local Statutory - Health Ins No",
                    },
                    new DynamicReportItem
                    {
                        Id = 88,
                        Name = "Local Statutory - HDMF No",
                    },
                    new DynamicReportItem
                    {
                        Id = 89,
                        Name = "P-File Admin",
                    },
                    new DynamicReportItem
                    {
                        Id = 90,
                        Name = "Paymaster - BVI Salary",
                    },
                    new DynamicReportItem
                    {
                        Id = 91,
                        Name = "Paymaster - BVI Allowance",
                    },
                    new DynamicReportItem
                    {
                        Id = 92,
                        Name = "Paymaster - BVI Local Salary",
                    },
                    new DynamicReportItem
                    {
                        Id = 93,
                        Name = "EPA - Administrator",
                    },
                    new DynamicReportItem
                    {
                        Id = 94,
                        Name = "EPA - Evaluator #1",
                    },
                    new DynamicReportItem
                    {
                        Id = 95,
                        Name = "EPA - Evaluator #2",
                    },
                    new DynamicReportItem
                    {
                        Id = 96,
                        Name = "EPA - Reviewer",
                    },
                    new DynamicReportItem
                    {
                        Id = 97,
                        Name = "EPA - Management",
                    },
                    new DynamicReportItem
                    {
                        Id = 98,
                        Name = "EPA - Increment",
                    },
                    new DynamicReportItem
                    {
                        Id = 99,
                        Name = "EPA - BVI Bonus",
                    },
                    new DynamicReportItem
                    {
                        Id = 100,
                        Name = "EPA - Local Bonus",
                    },
                    new DynamicReportItem
                    {
                        Id = 101,
                        Name = "PDF Password",
                    }
                }
            },
            new DynamicReportGroup
            {
                GroupdId = 3,
                Name = DynamicReportItemGroup.Payroll,
                Items = new List<DynamicReportItem>
                {
                    new DynamicReportItem
                    {
                        Id = 102,
                        Name = "Basic Salary",
                    },
                    new DynamicReportItem
                    {
                        Id = 103,
                        Name = "Basic Salary - Effective Date",
                    },
                    new DynamicReportItem
                    {
                        Id = 104,
                        Name = "Basic Salary - Reason",
                    },
                    new DynamicReportItem
                    {
                        Id = 105,
                        Name = "Standard Working Day",
                    },
                    new DynamicReportItem
                    {
                        Id = 106,
                        Name = "Basic Salary - E-banking",
                    },
                    new DynamicReportItem
                    {
                        Id = 107,
                        Name = "Basic Salary - Direct Banking 1",
                    },
                    new DynamicReportItem
                    {
                        Id = 108,
                        Name = "Basic Salary - Direct Banking 2",
                    },
                    new DynamicReportItem
                    {
                        Id = 109,
                        Name = "Basic Salary - Cash",
                    },
                    new DynamicReportItem
                    {
                        Id = 110,
                        Name = "Allowance",
                    },
                    new DynamicReportItem
                    {
                        Id = 111,
                        Name = "Allowance - Direct Banking 1",
                    },
                    new DynamicReportItem
                    {
                        Id = 112,
                        Name = "Allowance - Direct Banking 2",
                    },
                    new DynamicReportItem
                    {
                        Id = 113,
                        Name = "Allowance - Cash",
                    },
                    new DynamicReportItem
                    {
                        Id = 114,
                        Name = "Subsistence",
                    },
                    new DynamicReportItem
                    {
                        Id = 115,
                        Name = "Home Passage",
                    },
                    new DynamicReportItem
                    {
                        Id = 116,
                        Name = "Housing",
                    },
                    new DynamicReportItem
                    {
                        Id = 117,
                        Name = "Mobile Phone",
                    },
                    new DynamicReportItem
                    {
                        Id = 118,
                        Name = "HPI",
                    },
                    new DynamicReportItem
                    {
                        Id = 119,
                        Name = "Duty Meal",
                    },
                    new DynamicReportItem
                    {
                        Id = 120,
                        Name = "Secondment",
                    },
                    new DynamicReportItem
                    {
                        Id = 121,
                        Name = "Others",
                    },
                    new DynamicReportItem
                    {
                        Id = 122,
                        Name = "Rooming Charge",
                    },
                    new DynamicReportItem
                    {
                        Id = 123,
                        Name = "Fitness Club Membership",
                    }
                }
            }
        };

        public static IEnumerable<DynamicReportGroup> DynamicReportGroups => dynamicReportGroups;
    }
}