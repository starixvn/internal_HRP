namespace HRP.Models.Report
{
    using System.Collections.Generic;
    using HRP.Core.Constants;

    public static partial class ColumnsConfirguration
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
                Name = "Work Location - Country",
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
            },
            new DynamicReportItem
            {
                Id = 89,
                Name = "P-File Admin",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 90,
                Name = "Paymaster - BVI Salary",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 91,
                Name = "Paymaster - BVI Allowance",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 92,
                Name = "Paymaster - BVI Local Salary",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 93,
                Name = "EPA - Administrator",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 94,
                Name = "EPA - Evaluator #1",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 95,
                Name = "EPA - Evaluator #2",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 96,
                Name = "EPA - Reviewer",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 97,
                Name = "EPA - Management",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 98,
                Name = "EPA - Increment",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 99,
                Name = "EPA - BVI Bonus",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 100,
                Name = "EPA - Local Bonus",
                Group = DynamicReportItemGroup.Employement
            },
            new DynamicReportItem
            {
                Id = 101,
                Name = "PDF Password",
                Group = DynamicReportItemGroup.Employement
            }
        };

        public static IEnumerable<DynamicReportItem> DynamicReportItems => dynamicReportItems;
    }
}