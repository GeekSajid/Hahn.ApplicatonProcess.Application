using Hahn.ApplicatonProcess.December2020.Data.Entity;
using Hahn.ApplicatonProcess.December2020.Data.UnitOfWork;
using Hahn.ApplicatonProcess.December2020.Domain.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
                var previousHiredStatus = uow.ApplicantRepository.Get(x => x.Id == applicant.Id).FirstOrDefault().Hired;

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

                        // Sends Mail if applicant's hiring status changes
                        if (previousHiredStatus != applicant.Hired)
                        {
                            List<string> recepient = new List<string>() {
                                applicant.EmailAddress
                            };
                            if (applicant.Hired)
                            {
                                var mail = new SendEmail()
                                {
                                    Subject = "Application Status",
                                    Body = "Congratulations " + applicant.Name + "! " + "You have been Hired by Hahn Softwareentwicklung!",
                                    FromMail = "apimailsendergeek@gmail.com",
                                    ToMail = recepient,
                                    Password = "api@123mail",
                                };
                                var mailStatus = SendEmailToUrl(mail);

                            }
                            else
                            {
                                var mail = new SendEmail()
                                {
                                    Subject = "Application Status",
                                    Body = "Sorry " + applicant.Name + ", " + "You're hiring process have been halted or cancelled. Contact our support hotline for details",
                                    FromMail = "apimailsendergeek@gmail.com",
                                    ToMail = recepient,
                                    Password = "api@123mail",
                                };
                                var mailStatus = SendEmailToUrl(mail);
                            }

                        }

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

        #region Email helper
        public static string SendEmailToUrl(SendEmail model)
        {
            try
            {
                string path = /*System.Configuration.ConfigurationManager.AppSettings["email_send_url"]*/"http://103.192.157.43/service/api/customeMailSender";

                string BaseUrl = path;
                var content2 = JsonConvert.SerializeObject(model);
                HttpContent content = new StringContent(content2, Encoding.UTF8, "application/json");

                var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = new Uri(BaseUrl);

                HttpResponseMessage result = client.PostAsync(uri, content).Result;

                var jsonString = result.Content.ReadAsStringAsync();
                jsonString.Wait();

                return jsonString.Result;
            }
            catch (Exception ex)
            {
                return "Error " + ex.ToString();
            }
        }

        public class SendEmail
        {
            public SendEmail()
            {
                secretkey = "OTUxKCUhQCM=";
            }
            public string secretkey { get; set; }
            public string FromMail { get; set; }
            public List<string> ToMail { get; set; }
            public List<string> BccList { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public string Password { get; set; }
        }
        #endregion

        public interface IApplicantService
        {
            ApplicantDTO GetApplicantById(int ApplicantId);
            List<ApplicantDTO> GetAllApplicant();
            bool DeleteApplicant(int ApplicantId);

            bool CreateOrUpdateApplicant(ApplicantDTO Applicant);
        }

    }
}
