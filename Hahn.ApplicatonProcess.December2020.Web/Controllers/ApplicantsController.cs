using Hahn.ApplicatonProcess.December2020.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Hahn.ApplicatonProcess.December2020.Domain.Services.ApplicantService;

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
            if (id != applicant.Id||!ModelState.IsValid)
            {
                return BadRequest();
            }
            var previousHiredStatus = _applicantService.GetApplicantById(id).Hired;
            if (_applicantService.CreateOrUpdateApplicant(applicant)) 
            {
                return NoContent();
            }
            else
            {
                var applicantJsonStr = JsonConvert.SerializeObject(applicant);
                _logger.Error("Applicant Update failed for following applicant:" + applicantJsonStr);
                return BadRequest();
            }           

        }

        // POST: api/Applicants
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ApplicantDTO>> PostApplicant([FromBody] ApplicantDTO applicant)
        {
            if (ModelState.IsValid)
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
            else
            {
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

    }
}
