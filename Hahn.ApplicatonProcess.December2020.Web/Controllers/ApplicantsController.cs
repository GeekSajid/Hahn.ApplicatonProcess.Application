using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hahn.ApplicatonProcess.December2020.Data.DBContext;
using Hahn.ApplicatonProcess.December2020.Data.Entity;
using static Hahn.ApplicatonProcess.December2020.Domain.Services.ApplicantService;
using Hahn.ApplicatonProcess.December2020.Domain.DTO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Serilog;

namespace Hahn.ApplicatonProcess.December2020.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantsController : ControllerBase
    {
        private readonly IApplicantService _applicantService;
        private readonly ILogger _logger;

        public ApplicantsController(IApplicantService applicantService, ILogger logger)
        {
            _applicantService = applicantService;
            _logger = logger;
        }

        // GET: api/Applicants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicantDTO>>> GetApplicants()
        {
            return Ok(_applicantService.GetAllApplicant());
        }

        // GET: api/Applicants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicantDTO>> GetApplicant([FromRoute] int id)
        {
            var applicant = _applicantService.GetApplicantById(id);

            if (applicant == null)
            {
                return NotFound();
            }

            return Ok(applicant);
        }

        // PUT: api/Applicants/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicant([FromRoute] int id, [FromBody] ApplicantDTO applicant)
        {
            if (id != applicant.Id)
            {
                return BadRequest();
            }
            var previousHiredStatus = _applicantService.GetApplicantById(id).Hired;
            if (_applicantService.CreateOrUpdateApplicant(applicant)) 
            {
                // Sends Mail if applicant's hiring status changes
                if (previousHiredStatus!=applicant.Hired)
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

            }
            else
            {
                var applicantJsonStr = JsonConvert.SerializeObject(applicant);
                _logger.Error("Applicant Update failed for following applicant:" + applicantJsonStr);
                return BadRequest();
            }

            

            return NoContent();
        }

        // POST: api/Applicants
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ApplicantDTO>> PostApplicant([FromBody] ApplicantDTO applicant)
        {
            if (_applicantService.CreateOrUpdateApplicant(applicant)) 
            {
                var savedApplicant = _applicantService.GetAllApplicant().Where(x => x.EmailAddress == applicant.EmailAddress).FirstOrDefault();
                return CreatedAtAction("GetApplicant", new { id = savedApplicant.Id }, applicant);
            }
            else
            {
                var applicantJsonStr = JsonConvert.SerializeObject(applicant);
                _logger.Error("Applicant Creation failed for following applicant:" + applicantJsonStr);
                return BadRequest();
            }
        }

        // DELETE: api/Applicants/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApplicantDTO>> DeleteApplicant([FromRoute] int id)
        {
            var applicant = _applicantService.GetApplicantById(id);
            if (applicant == null)
            {
                return NotFound();
            }

            if (_applicantService.DeleteApplicant(id))
            {
                return Ok();
            }
            else
            {
                var applicantJsonStr = JsonConvert.SerializeObject(applicant);
                _logger.Error("Applicant Delete failed for following applicant:" + applicantJsonStr);
                return BadRequest();
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
    }
}
