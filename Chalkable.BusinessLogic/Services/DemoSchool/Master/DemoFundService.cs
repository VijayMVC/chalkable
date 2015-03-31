using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoPersonBalance
    {
        public decimal Balance { get; set; }
        public int PersonId { get; set; }
    }
    public class DemoPersonBalanceStorage : BaseDemoIntStorage<DemoPersonBalance>
    {
        public DemoPersonBalanceStorage()
            : base(x => x.PersonId, false)
        {
        }

        public void AddPersonBalance(DemoPersonBalance balance)
        {
            Add(balance);
        }

        public void UpdatePersonBalance(int personId, decimal balance)
        {
            if (data.ContainsKey(personId))
            {
                data[personId].Balance = balance;
            }
        }
    }
    public class DemoFundService : DemoMasterServiceBase, IFundService
    {
        private DemoPersonBalanceStorage PersonBalanceStorage { get; set; }
        public DemoFundService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            PersonBalanceStorage = new DemoPersonBalanceStorage();
        }

        public decimal GetSchoolReserve(Guid schoolId)
        {
            throw new NotImplementedException();
        }

        public decimal GetRoleBalance(int roleId, Guid schoolId)
        {
            throw new NotImplementedException();
        }

        public FundRequest RequestByPurchaseOrder(Guid? schoolId, Guid? userId, decimal amount, string purchaseOrder, List<Pair<int, decimal>> roleDist, byte[] signature)
        {
            throw new NotImplementedException();
        }

        public FundRequest RequestByPurchaseOrder(Guid? schoolId, Guid? userId, decimal amount, decimal adminAmount, decimal teacherAmount, decimal studentAmount, decimal parentAmount, string purchaseOrder, byte[] signature)
        {
            throw new NotImplementedException();
        }

        public void AddFundsToRole(Guid schoolId, int roleId, decimal amount, Guid? fundRequestId, string description)
        {
            throw new NotImplementedException();
        }

        public void AddFundsToClass(Guid classId, decimal amount, string discription)
        {
            throw new NotImplementedException();
        }

        public void AddFundsToPerson(Guid userId, decimal amount, string discription)
        {
            throw new NotImplementedException();
        }

        public IList<FundRequest> GetFundRequests()
        {
            throw new NotImplementedException();
        }

        public decimal GetUserBalance(int userId, bool? privateMoney = null)
        {
            return PersonBalanceStorage.GetById(userId).Balance;
        }

        public void UpdateUserBalance(int userId, decimal newBalance)
        {
            PersonBalanceStorage.UpdatePersonBalance(userId, newBalance);
        }

        public decimal GetClassBalance(Guid classId)
        {
            throw new NotImplementedException();
        }

        public Fund AppInstallPersonPayment(Guid appInstallId, decimal amount, DateTime performedDateTime, string descrption)
        {
            throw new NotImplementedException();
        }

        public IList<ApplicationInstallAction> GetSchoolPersonHistory(Guid userId)
        {
            throw new NotImplementedException();
        }

        public decimal GetToSchoolPayment(Guid schoolId)
        {
            throw new NotImplementedException();
        }

        public decimal GetPaymentForApps(Guid schoolId)
        {
            throw new NotImplementedException();
        }

        public bool ApproveReject(Guid fundRequestId, bool isApprove)
        {
            throw new NotImplementedException();
        }
    }
}