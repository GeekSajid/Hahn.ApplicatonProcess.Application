using Hahn.ApplicatonProcess.December2020.Data.Entity;
using Hahn.ApplicatonProcess.December2020.Data.UnitOfWork;
using Hahn.ApplicatonProcess.December2020.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Hahn.ApplicatonProcess.December2020.Domain.Services.ApplicantService;

namespace Hahn.ApplicatonProcess.December2020.Domain.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IUoW uow;
        public ApplicantService(IUoW _uow)
        {
            uow = _uow;
        }

        public List<ApplicantDTO> GetAllApplicant()
        {
            return uow.ApplicantRepository.Get().Select(x => new ApplicantDTO
            {
                Id = x.Id,
                Name = x.Name,
                FamilyName = x.FamilyName,
                Address = x.Address,
                CountryOfOrigin = x.CountryOfOrigin,
                Age = x.Age,
                EmailAddress = x.EmailAddress,
                Hired = x.Hired
            }).ToList();
        }

        public ApplicantDTO GetApplicantById(int ApplicantId)
        {
            return uow.ApplicantRepository.Get(x => x.Id == ApplicantId).
             Select(x => new ApplicantDTO
             {
                 Id = x.Id,
                 Name = x.Name,
                 FamilyName = x.FamilyName,
                 Address = x.Address,
                 CountryOfOrigin = x.CountryOfOrigin,
                 Age = x.Age,
                 EmailAddress = x.EmailAddress,
                 Hired = x.Hired
             }).FirstOrDefault();
        }

        public bool CreateOrUpdateApplicant(ApplicantDTO applicant)
        {
            try
            {
                //Used for Checking if email is already being used by another applicant
                var checkAvailabilityEmail = uow.ApplicantRepository.Get(x => x.EmailAddress == applicant.EmailAddress).FirstOrDefault();

                if (applicant.Id > 0)
                {

                    if ((checkAvailabilityEmail != null && checkAvailabilityEmail.Id == applicant.Id) || checkAvailabilityEmail == null)
                    {
                        checkAvailabilityEmail = new Applicant
                        {
                            Id = applicant.Id,
                            Name = applicant.Name,
                            FamilyName = applicant.FamilyName,
                            Address = applicant.Address,
                            CountryOfOrigin = applicant.CountryOfOrigin,
                            Age = applicant.Age,
                            EmailAddress = applicant.EmailAddress,
                            Hired = applicant.Hired
                        };

                        uow.ApplicantRepository.Update(checkAvailabilityEmail);
                        uow.Commit();
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (checkAvailabilityEmail == null)
                    {
                        var InsertApplicant = new Applicant
                        {
                            Id = applicant.Id,
                            Name = applicant.Name,
                            FamilyName = applicant.FamilyName,
                            Address = applicant.Address,
                            CountryOfOrigin = applicant.CountryOfOrigin,
                            Age = applicant.Age,
                            EmailAddress = applicant.EmailAddress,
                            Hired = applicant.Hired
                        };

                        uow.ApplicantRepository.Insert(InsertApplicant);
                        uow.Commit();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception er)
            {
                var message = er.Message;
                return false;
            }

        }

        public bool DeleteApplicant(int ApplicantId)
        {
            try
            {
                var checkAvalaibility = uow.ApplicantRepository.Get(x => x.Id == ApplicantId).FirstOrDefault();

                if (checkAvalaibility != null)
                {
                    uow.ApplicantRepository.Delete(checkAvalaibility);
                    uow.Commit();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception er)
            {
                var Message = er.Message;
                return false;
            }
        }

        public interface IApplicantService
        {
            ApplicantDTO GetApplicantById(int ApplicantId);
            List<ApplicantDTO> GetAllApplicant();
            bool DeleteApplicant(int ApplicantId);

            bool CreateOrUpdateApplicant(ApplicantDTO Applicant);
        }

    }
}
