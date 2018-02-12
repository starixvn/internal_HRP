namespace HRP.Criteria.Employee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;

    public class GetEmployeeBasicInfoByIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetBasicInfoById";

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
