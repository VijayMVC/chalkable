using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IFundService
    {
        decimal GetSchoolReserve(Guid schoolId);
        decimal GetRoleBalance(int roleId, Guid schoolId);
        FundRequest RequestByPurchaseOrder(Guid? schoolId, Guid? userId, decimal amount, string purchaseOrder, List<Pair<int, decimal>> roleDist, byte[] signature);
        FundRequest RequestByPurchaseOrder(Guid? schoolId, Guid? userId, decimal amount, decimal adminAmount, decimal teacherAmount, decimal studentAmount, decimal parentAmount, string purchaseOrder, byte[] signature);
        void AddFundsToRole(Guid schoolId, int roleId, decimal amount, Guid? fundRequestId, string description);
        void AddFundsToClass(Guid classId, decimal amount, string discription);
        void AddFundsToPerson(Guid userId, decimal amount, string discription);
        IList<FundRequest> GetFundRequests();

        decimal GetUserBalance(Guid userId, bool? privateMoney = null);
        decimal GetClassBalance(Guid classId);
        Fund AppInstallPersonPayment(Guid appInstallId, decimal amount, DateTime performedDateTime, string descrption);
        IList<ApplicationInstallAction> GetSchoolPersonHistory(Guid userId);
        decimal GetToSchoolPeyment(Guid schoolId);
        decimal GetPeymentForApps(Guid schoolId);
        bool ApproveReject(Guid fundRequestId, bool isApprove);   
    }

    public class FundService : MasterServiceBase, IFundService
    {
        public FundService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        //TODO: need to figure out something about admin roles
        private IList<Fund> GetFundsByRole(int roleId, Guid schoolId)
        {
            using (var uow = Read())
            {
                var da = new FundDataAccess(uow);
                IList<Fund> funds;
                if (roleId == CoreRoles.ADMIN_VIEW_ROLE.Id || roleId == CoreRoles.ADMIN_EDIT_ROLE.Id || roleId == CoreRoles.ADMIN_GRADE_ROLE.Id)
                    funds = da.GetRoleFunds(schoolId, new[] { CoreRoles.ADMIN_VIEW_ROLE.Id, CoreRoles.ADMIN_EDIT_ROLE.Id, CoreRoles.ADMIN_GRADE_ROLE.Id });
                else
                    funds = da.GetRoleFunds(schoolId, new[] { roleId });
                return funds;

            }
        }

        private void AddFundsToGroup(IList<Guid> schoolPersons, IList<Fund> funds, decimal amount, Guid schoolId, Guid? fundRequestId, string description)
        {
            if (GetSchoolReserve(schoolId) < amount)
                throw new ChalkableException(ChlkResources.ERR_FUND_RESERVE_NOT_ENOUGH_MONEY);
            if (schoolPersons.Count == 0 && amount != 0)
                throw new ChalkableException(ChlkResources.ERR_FUND_INVALID_GROUP);
            if (amount == 0) return;
            var currentAmounts = schoolPersons.ToDictionary(schoolPerson => schoolPerson, schoolPerson => (decimal)0);
            foreach (var fund in funds)
            {
                if (fund.FromUserRef.HasValue)
                    currentAmounts[fund.FromUserRef.Value] = currentAmounts[fund.FromUserRef.Value] - fund.Amount;
                if (fund.ToUserRef.HasValue)
                    currentAmounts[fund.ToUserRef.Value] = currentAmounts[fund.ToUserRef.Value] + fund.Amount;
            }
            var amountByPerson = currentAmounts.Select(x => new Pair<Guid, decimal>(x.Key, x.Value)).ToList();
            amountByPerson.Sort((x, y) => x.Second > y.Second ? 1 : x.Second == y.Second ? 0 : -1);
            for (int i = 0; i < amountByPerson.Count; i++)
            {
                decimal leftToAdd = amount / (amountByPerson.Count - i);
                leftToAdd = Math.Max(-Math.Max(amountByPerson[i].Second, 0), leftToAdd);
                leftToAdd = (long)(leftToAdd * 100);
                leftToAdd /= 100;
                amountByPerson[i].Second += leftToAdd;
                amount -= leftToAdd;
            }
            if (amount != 0)
                throw new ChalkableException(ChlkResources.ERR_FUND_ROLE_NOT_ENOUGH_MONEY);

            using (var uow = Update())
            {
                var da = new FundDataAccess(uow);
                var fs = new List<Fund>();
                for (int i = 0; i < amountByPerson.Count; i++)
                {
                    var diff = amountByPerson[i].Second - currentAmounts[amountByPerson[i].First];
                    Fund f = new Fund
                        {
                            Id = Guid.NewGuid(),
                            FromSchoolRef = schoolId,
                            SchoolRef = schoolId,
                            ToUserRef = amountByPerson[i].First,
                            IsPrivate = false,
                            FundRequestRef = fundRequestId,
                            Description = description,
                            Amount = Math.Abs(diff)
                        };
                    fs.Add(f);
                }
                da.Insert(fs);
                uow.Commit();
            }
        }

        private Fund Send(Guid? fromSchoolId, Guid? toSchoolId, Guid? fromUserId, Guid? toUserId, Guid? applicationInstallactionId, decimal amount, DateTime performedDateTime, string description)
        {
            if (fromUserId.HasValue && GetUserBalance(fromUserId.Value) - amount < 0)
                throw new ChalkableException(ChlkResources.ERR_FUND_PERSON_NOT_ENOUGH_MONEY);
            if (fromSchoolId.HasValue && GetSchoolReserve(fromSchoolId.Value) - amount < 0)
                throw new ChalkableException(ChlkResources.ERR_FUND_SCHOOL_NOT_ENOUGH_MONEY);
            if (fromSchoolId.HasValue && toUserId.HasValue)
                throw new NotImplementedException(ChlkResources.ERR_FUND_CANT_TRANSFER_MONEY_BETWEEN_SCHOOLS);
            var schoolId = fromSchoolId ?? toSchoolId;
            if (!schoolId.HasValue && fromUserId.HasValue)
            {
                throw new NotImplementedException();
            }
                
            if (!schoolId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_FUND_CANT_DETERMINE_SCHOOL);
            var fund = new Fund
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                PerformedDateTime = performedDateTime,
                Description = description,
                FromUserRef = fromUserId,
                ToUserRef = toUserId,
                FromSchoolRef = fromSchoolId,
                ToSchoolRef = toSchoolId,
                SchoolRef = schoolId.Value,
                AppInstallActionRef = applicationInstallactionId,
            };
            using (var uow = Update())
            {
                var da = new FundDataAccess(uow);
                da.Insert(fund);
                uow.Commit();
            }
            return fund;
        }

        public decimal GetSchoolReserve(Guid schoolId)
        {
            if (!(BaseSecurity.IsAdminViewer(Context) || BaseSecurity.IsSysAdmin(Context)))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new FundDataAccess(uow);

                var toschoolFunds = da.GetAll(new AndQueryCondition { { Fund.TO_SCHOOL_REF_FIELD, schoolId } });
                var fromschoolFunds = da.GetAll(new AndQueryCondition { { Fund.FROM_SCHOOL_REF_FIELD, schoolId } });
                var amountToschool = toschoolFunds.Sum(x => x.Amount);
                var amountFromschool = fromschoolFunds.Sum(x => x.Amount);
                return (amountToschool - amountFromschool);
            }
        }

        public decimal GetRoleBalance(int roleId, Guid schoolId)
        {
            if (!(BaseSecurity.IsAdminViewer(Context) || BaseSecurity.IsSysAdmin(Context)))
                throw new ChalkableSecurityException();
            var funds = GetFundsByRole(roleId, schoolId);

            var to = funds.Where(x => x.ToUserRef.HasValue).Sum(x => x.Amount);
            var from = funds.Where(x => x.FromUserRef.HasValue).Sum(x => x.Amount);
            var res = to - from;
            return res;
        }

        public FundRequest RequestByPurchaseOrder(Guid? schoolId, Guid? userId, decimal amount, string purchaseOrder, List<Pair<int, decimal>> roleDist, byte[] signature)
        {
            if (!(BaseSecurity.IsAdminViewer(Context) || BaseSecurity.IsSysAdmin(Context)))
                throw new ChalkableSecurityException();
            if (amount <= 0)
                throw new ChalkableException(ChlkResources.ERR_FUND_INVALID_AMOUNT);
            var fr = new FundRequest
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Created = DateTime.UtcNow,
                CreatedByRef = Context.UserId,
                PurchaseOrder = purchaseOrder,
                SchoolRef = schoolId,
                UserRef = userId,
                State = FundRequestState.Created
            };
            if (signature != null)
            {
                fr.SignatureRef = Guid.NewGuid();
                ServiceLocator.FundRequestPictureService.UploadPicture(fr.SignatureRef.Value, signature);
            }
            if (schoolId.HasValue)
            {
                using (var uow = Update())
                {
                    var da = new FundRequestRoleDistributionDataAccess(uow);
                    new FundRequestDataAccess(uow).Insert(fr);
                    decimal reserveAmount = amount;
                    var rds = new List<FundRequestRoleDistribution>();
                    foreach (var pair in roleDist)
                    {
                        if (pair.Second != 0)
                        {
                            var rd = new FundRequestRoleDistribution
                            {
                                Id = Guid.NewGuid(),
                                Amount = pair.Second,
                                RoleRef = pair.First,
                                FundRequestRef = fr.Id
                            };
                            rds.Add(rd);
                            if (rd.Amount < 0)
                                throw new ChalkableException(ChlkResources.ERR_FUND_INVALID_ROLE_DISTRIBUTION_AMOUNT);
                            reserveAmount -= rd.Amount;
                        }
                    }
                    if(rds.Count > 0) da.Insert(rds);      
                    if (reserveAmount < 0)
                        throw new ChalkableException(ChlkResources.ERR_FUND_ROLE_DISTRIBUTION_TOO_HIGH);
                    uow.Commit();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return fr;
        }

        public FundRequest RequestByPurchaseOrder(Guid? schoolId, Guid? userId, decimal amount, decimal adminAmount,
                                                  decimal teacherAmount, decimal studentAmount, decimal parentAmount,
                                                  string purchaseOrder, byte[] signature)
        {
            var roleDist = new List<Pair<int, decimal>>
                               {
                                   new Pair<int, decimal>(CoreRoles.ADMIN_VIEW_ROLE.Id, adminAmount),
                                   new Pair<int, decimal>(CoreRoles.TEACHER_ROLE.Id, teacherAmount),
                                   new Pair<int, decimal>(CoreRoles.STUDENT_ROLE.Id, studentAmount),
                                   new Pair<int, decimal>(CoreRoles.PARENT_ROLE.Id, parentAmount)
                               };
            return RequestByPurchaseOrder(schoolId, userId, amount, purchaseOrder, roleDist, signature);
        }

        public void AddFundsToRole(Guid schoolId, int roleId, decimal amount, Guid? fundRequestId, string description)
        {
            if (!(BaseSecurity.IsAdminViewer(Context) || BaseSecurity.IsSysAdmin(Context)))
                throw new ChalkableSecurityException();
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            var schoolSl = ServiceLocator.SchoolServiceLocator(Context.SchoolId.Value);
            if (amount != 0)
            {
                var funds = GetFundsByRole(roleId, schoolId);
                var sps = schoolSl.PersonService.GetPersons();
                if (roleId == CoreRoles.ADMIN_EDIT_ROLE.Id || roleId == CoreRoles.ADMIN_GRADE_ROLE.Id || roleId == CoreRoles.ADMIN_VIEW_ROLE.Id)
                {
                    sps = sps.Where(x => x.RoleRef == CoreRoles.ADMIN_EDIT_ROLE.Id || x.RoleRef == CoreRoles.ADMIN_VIEW_ROLE.Id || x.RoleRef == CoreRoles.ADMIN_GRADE_ROLE.Id).ToList();
                }
                else
                    sps = sps.Where(x => x.RoleRef == roleId).ToList();
                AddFundsToGroup(sps.Select(x=>x.Id).ToList(), funds, amount, schoolId, fundRequestId, description);
            }
        }

        public void AddFundsToClass(Guid classId, decimal amount, string discription)
        {
            if (amount != 0)
            {
                if (!Context.SchoolId.HasValue)
                    throw new UnassignedUserException();
                var schoolSl = ServiceLocator.SchoolServiceLocator(Context.SchoolId.Value);
                var students = schoolSl.PersonService.GetPaginatedPersons(new PersonQuery
                    {
                        ClassId = classId,
                        RoleId = CoreRoles.STUDENT_ROLE.Id
                    });
                if (students.Count == 0)
                    throw new ChalkableException(ChlkResources.ERR_NO_STUDENTS_IN_CLASS);
                var schoolId = Context.SchoolId.Value;
                if (GetSchoolReserve(schoolId) < amount)
                    throw new ChalkableException(ChlkResources.ERR_FUND_RESERVE_NOT_ENOUGH_MONEY);
                var funds = new List<Fund>();
                using (var uow = Read())
                {
                    var da = new FundDataAccess(uow);
                    for (int i = 0; i < students.Count; i++)
                    {
                        var fs = da.GetAll(new AndQueryCondition
                            {
                                {Fund.TO_USER_REF_FIELD, students[i].Id},
                                {Fund.FROM_USER_REF_FIELD, students[i].Id},
                                {Fund.IS_PRIVATE_FIELD, false},
                            });
                        funds.AddRange(fs);
                    }
                }
                AddFundsToGroup(students.Select(x => x.Id).ToList(), funds.ToList(), amount, schoolId, null, discription);
            }
        }

        public void AddFundsToPerson(Guid userId, decimal amount, string discription)
        {
            if (amount != 0)
            {
                if (!Context.SchoolId.HasValue)
                    throw new UnassignedUserException();
                using (var uow = Read())
                {
                    var da = new FundDataAccess(uow);
                    var fs = da.GetAll(new AndQueryCondition
                            {
                                {Fund.TO_USER_REF_FIELD, userId},
                                {Fund.FROM_USER_REF_FIELD, userId},
                                {Fund.IS_PRIVATE_FIELD, false},
                            });
                    AddFundsToGroup(new List<Guid> { userId }, fs, amount, Context.SchoolId.Value, null, discription);
                }
            }
        }

        public IList<FundRequest> GetFundRequests()
        {
            var ps = new AndQueryCondition();
            if (BaseSecurity.IsSysAdmin(Context)){}
            else if (BaseSecurity.IsAdminViewer(Context))
                ps.Add(FundRequest.SCHOOL_REF_FIELD, Context.SchoolId);
            else
            {
                ps.Add(FundRequest.USER_REF_FIELD, Context.UserId);
                ps.Add(FundRequest.CREATED_BY_REF_FIELD, Context.UserId);
            }
            using (var uow = Read())
            {
                return new FundRequestDataAccess(uow).GetAll(ps);
            }
        }

        public decimal GetUserBalance(Guid userId, bool? privateMoney = null)
        {
            var schoolPerson = ServiceLocator.UserService.GetById(userId);
            if (!BaseSecurity.IsAdminTeacherOrExactStudent(schoolPerson, Context))
                throw new ChalkableSecurityException();
            var psFrom = new AndQueryCondition { { Fund.FROM_USER_REF_FIELD, userId } };
            var psTo = new AndQueryCondition { { Fund.TO_USER_REF_FIELD, userId } };
            if (privateMoney.HasValue)
            {
                psFrom.Add(Fund.IS_PRIVATE_FIELD, privateMoney);
                psTo.Add(Fund.IS_PRIVATE_FIELD, privateMoney);
            }
            using (var uow = Read())
            {
                var da = new FundDataAccess(uow);
                var toPersonFunds = da.GetAll(psTo);
                var fromPersonFunds = da.GetAll(psFrom);
                var amountToPeron = toPersonFunds.Sum(x => x.Amount);
                var amountFromPerson = fromPersonFunds.Sum(x => x.Amount);
                return (amountToPeron - amountFromPerson);    
            }
        }

        public decimal GetClassBalance(Guid classId)
        {
            using (var uow = Read())
            {
                var clazz = new ClassDataAccess(uow).GetById(classId);
                if (!BaseSecurity.IsAdminViewerOrClassTeacher(clazz, Context))
                    throw new ChalkableSecurityException();
                if (!Context.SchoolId.HasValue)
                    throw new UnassignedUserException();
                var schoolSl = ServiceLocator.SchoolServiceLocator(Context.SchoolId.Value);
                var students = schoolSl.PersonService.GetPaginatedPersons(new PersonQuery
                {
                    ClassId = classId,
                    RoleId = CoreRoles.STUDENT_ROLE.Id
                });
                decimal res = 0;
                foreach (var person in students)
                {
                    res = res + GetUserBalance(person.Id, false);
                }
                return res;
            }
            
        }
        
        public Fund AppInstallPersonPayment(Guid appInstallId, decimal amount, DateTime performedDateTime, string descrption)
        {
            return Send(null, null, Context.UserId, null, appInstallId, amount, performedDateTime, descrption);
        }

        public IList<ApplicationInstallAction> GetSchoolPersonHistory(Guid userId)
        {
            using (var uow = Read())
            {
                return new ApplicationInstallActionDataAccess(uow).GetAll(new AndQueryCondition
                        {
                            {ApplicationInstallAction.OWNER_REF_FIELD, userId}
                        });
            }
        }

        public decimal GetToSchoolPeyment(Guid schoolId)
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();

            using (var uow = Read())
            {
                var da = new FundDataAccess(uow);
                return da.GetAll(new AndQueryCondition { { Fund.TO_SCHOOL_REF_FIELD, schoolId } }).Sum(x => x.Amount);
            }
        }

        public decimal GetPeymentForApps(Guid schoolId)
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException(ChlkResources.ERR_FUND_PERSON_BALANCE_INVALID_PERMISSION);

            using (var uow = Read())
            {
                var da = new FundDataAccess(uow);
                return da.GetPaymentsForApps(schoolId);
            }
        }

        public bool ApproveReject(Guid fundRequestId, bool isApprove)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException(ChlkResources.ERR_FUND_MANAGE_REQUEST_ILLEGAL_RIGHTS);
            using (var uow = Update())
            {
                var da = new FundRequestDataAccess(uow);
                var fundRequest = da.GetById(fundRequestId);
                if (fundRequest.State == FundRequestState.Created)
                {
                    if (isApprove)
                    {
                        fundRequest.State = FundRequestState.Confirmed;
                        var schoolId = fundRequest.SchoolRef;
                        var f = new Fund
                        {
                            Id = Guid.NewGuid(),
                            Amount = fundRequest.Amount,
                            IsPrivate = false,
                            Description = ChlkResources.FUND_ADDED_MONEY_TO_REQUEST,
                            FundRequestRef = fundRequest.Id,
                            PerformedDateTime = fundRequest.Created
                        };
                        if (schoolId.HasValue)
                        {
                            f.ToSchoolRef = schoolId.Value;
                            f.SchoolRef = schoolId.Value;
                            new FundDataAccess(uow).Insert(f);
                        
                            var fundRequestRoles =
                                new FundRequestRoleDistributionDataAccess(uow).GetAll(new AndQueryCondition
                                    {
                                        {FundRequestRoleDistribution.FUND_REQUEST_REF_FIELD, fundRequest.Id}
                                    });
                            da.Update(fundRequest);
                            uow.Commit();
                            foreach (var fundRequestRole in fundRequestRoles)
                            {
                                var role = fundRequestRole.RoleRef;
                                AddFundsToRole(schoolId.Value, role, fundRequestRole.Amount, fundRequest.Id, string.Format(ChlkResources.FUND_ADDED_MONEY_TO_ROLE, role));
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                    }
                    else
                    {
                        fundRequest.State = FundRequestState.Rejected;
                        da.Update(fundRequest);
                        uow.Commit();
                    }
                    return true;
                }
            }
            return false;
        }
    }
}