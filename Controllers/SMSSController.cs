using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesCommissionsAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;



namespace SalesCommissionsAPI.Controllers
{
    [Authorize(Policy = "RequireWindowsAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class SMSSController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly LocalDbContext _context;
        private readonly ApplicationDbContext _APPcontext;
        private readonly ILogger<SMSSController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


//ILogger<SMSSController> logger, IHttpContextAccessor httpContextAccessor
        public SMSSController(LocalDbContext context, ApplicationDbContext aPPcontext)
        {
            _context = context;
           // _logger = logger;
           // _httpContextAccessor = httpContextAccessor;
            _APPcontext = aPPcontext;
        }

        // GET: api/SMSS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SMSS>>> GetAllSMSS()
        {
            return await _context.SMSS.ToListAsync();
        }

        // GET: api/SMSS/{Salesrep}/{Year}/{Cmth}
        [HttpGet("{Salesrep}/{Year}/{Cmth}")]
        public async Task<ActionResult<SMSS>> GetSMSS(string Salesrep, int Year, int Cmth)
        {
            var smss = await _context.SMSS.FindAsync(Salesrep, Year, Cmth);

            if (smss == null)
            {
                return NotFound();
            }

            return smss;
        }

        
        [HttpPost]
        public async Task<ActionResult<SMSS>> CreateSMSS(SMSS smss)
        {
            smss.Fmth = ((smss.Cmth % 12) + 3) % 12 != 0 ? ((smss.Cmth % 12) + 3) % 12 : 12;

            smss.Site = _APPcontext.SalesRep.FirstOrDefault(s => s.SlsRepId == smss.Salesrep)?.site ?? "";
            SalesRep salesRep = _APPcontext.SalesRep.FirstOrDefault(s => s.SlsRepId == smss.Salesrep);
            string salesRepSite = _APPcontext.SalesRep.FirstOrDefault(s => s.SlsRepId == smss.Salesrep)?.site ?? "";
            Console.WriteLine(salesRep+ "" + salesRepSite);
            _context.SMSS.Add(smss);
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Ok("Credentials Expired. please login");
            }

            // Log the create action before saving to the database
            var logEntry = new ActionLog
            {
                ActionName = "CreateSMSS",
                PerformedBy = userId,
                Entity = "SMSS",
                Data = $"Salesrep: {smss.Salesrep}, Year: {smss.Year}, Cmth: {smss.Cmth}",
                Timestamp = DateTime.UtcNow
            };
            _context.ActionLogs.Add(logEntry);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                Console.WriteLine($"Error: {innerException}");
                throw; // Re-throw the exception if necessary
            }

            return CreatedAtAction(nameof(GetSMSS), new { Salesrep = smss.Salesrep, Year = smss.Year, Cmth = smss.Cmth }, smss);
        }

        // PUT: api/SMSS/{Salesrep}/{Year}/{Cmth}
        [HttpPut("{Salesrep}/{Year}/{Cmth}")]
        public async Task<IActionResult> UpdateSMSS(string Salesrep, int Year, int Cmth, SMSS smss)
        {
            var existingSMSS = await _context.SMSS.FindAsync(Salesrep, Year, Cmth);
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Ok("Credentials Expired. please login");
            }
            if (existingSMSS == null)
            {
                smss.Fmth = ((smss.Cmth % 12) + 3) % 12 != 0 ? ((smss.Cmth % 12) + 3) % 12 : 12;
                smss.Site = _APPcontext.SalesRep.FirstOrDefault(s => s.SlsRepId == smss.Salesrep)?.site ?? "";
                _context.SMSS.Add(smss);
                await _context.SaveChangesAsync();
                

                // Log the create action
                var logEntry = new ActionLog
                {
                    ActionName = "CreateSMSS",
                    PerformedBy = userId,
                    Entity = "SMSS",
                    Data = $"Salesrep: {smss.Salesrep}, Year: {smss.Year}, Cmth: {smss.Cmth}",
                    Timestamp = DateTime.UtcNow
                };
                _context.ActionLogs.Add(logEntry);

                return CreatedAtAction(nameof(GetSMSS), new { Salesrep = smss.Salesrep, Year = smss.Year, Cmth = smss.Cmth }, smss);
                //return NotFound();
            }

            existingSMSS.Salesrep = smss.Salesrep;
            existingSMSS.Year = smss.Year;
            existingSMSS.Fmth = ((smss.Cmth % 12) + 3) % 12 != 0 ? ((smss.Cmth % 12) + 3) % 12 : 12;
            existingSMSS.Cmth = smss.Cmth;
            existingSMSS.GPBudget = smss.GPBudget;
            existingSMSS.Site = _APPcontext.SalesRep.FirstOrDefault(s => s.SlsRepId == existingSMSS.Salesrep)?.site ?? "";

            _context.SMSS.Update(existingSMSS);
            

            // Log the update action
            var updateLogEntry = new ActionLog
            {
                ActionName = "UpdateSMSS",
                PerformedBy = userId,
                Entity = "SMSS",
                Data = $"Updated Salesrep: {smss.Salesrep}, Year: {smss.Year}, Cmth: {smss.Cmth}",
                Timestamp = DateTime.UtcNow
            };
            _context.ActionLogs.Add(updateLogEntry);

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SMSSExists(Salesrep, Year, Cmth))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(existingSMSS);
        }

        // DELETE: api/SMSS/{Salesrep}/{Year}/{Cmth}
        [HttpDelete("{Salesrep}/{Year}/{Cmth}")]
        public async Task<IActionResult> DeleteSMSS(string Salesrep, int Year, int Cmth)
        {
            var smss = await _context.SMSS.FindAsync(Salesrep, Year, Cmth);
            if (smss == null)
            {
                return NotFound();
            }

            _context.SMSS.Remove(smss);
            // Log request headers
            var headers = Request.Headers;
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Ok("Credentials Expired. please login");
            }
            
            var DeleteLogEntry = new ActionLog
            {
                ActionName = "DeleteSMSS",
                PerformedBy = userId,
                Entity = "SMSS",
                Data = $"Delete Salesrep: {smss.Salesrep}, Year: {smss.Year}, Cmth: {smss.Cmth}",
                Timestamp = DateTime.UtcNow
            };
            _context.ActionLogs.Add(DeleteLogEntry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/SMSS/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateOrUpdateMultipleSMSS(List<SMSS> smssList)
        {
            if (smssList == null || !smssList.Any())
            {
                return BadRequest("No SMSS data provided.");
            }

            foreach (var smss in smssList)
            {
                var existingSMSS = await _context.SMSS
                    .FirstOrDefaultAsync(e => e.Salesrep == smss.Salesrep && e.Year == smss.Year && e.Cmth == smss.Cmth);

                if (existingSMSS == null)
                {
                    // If the SMSS does not exist, add it to the database
                    smss.Fmth = ((smss.Cmth % 12) + 3) % 12 != 0 ? ((smss.Cmth % 12) + 3) % 12 : 12;
                    smss.Site = _APPcontext.SalesRep.FirstOrDefault(s => s.SlsRepId == smss.Salesrep)?.site ?? "";
                    var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    var CreteLogEntry = new ActionLog
                    {
                        ActionName = "UpdateSMSS",
                        PerformedBy = userId,
                        Entity = "SMSS",
                        Data = $"Updated for Salesrep: {smss.Salesrep}, Year: {smss.Year}, Cmth: {smss.Cmth}",
                        Timestamp = DateTime.UtcNow
                    };
                    _context.ActionLogs.Add(CreteLogEntry);
                    _context.SMSS.Add(smss);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error updating database.");
                    }
                }
                else
                {
                    // If it exists, update the existing record
                    existingSMSS.Salesrep = smss.Salesrep;
                    existingSMSS.Year = smss.Year;
                    existingSMSS.Fmth = ((existingSMSS.Cmth % 12) + 3) % 12 != 0 ? ((existingSMSS.Cmth % 12) + 3) % 12 : 12;
                    existingSMSS.Cmth = smss.Cmth;
                    existingSMSS.GPBudget = smss.GPBudget;
                    existingSMSS.Site = _APPcontext.SalesRep.FirstOrDefault(s => s.SlsRepId == existingSMSS.Salesrep)?.site ?? "";

                    _context.SMSS.Update(existingSMSS);
                    var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ;
                   
                    var UpdateLogEntry = new ActionLog
                    {

                        ActionName = "UpdateSMSS",
                        PerformedBy = userId,
                        Entity = "SMSS",
                        Data = $"Update Salesrep: {existingSMSS.Salesrep}, Year: {existingSMSS.Year}, Cmth: {existingSMSS.Cmth}",
                        Timestamp = DateTime.UtcNow
                    };
                    _context.ActionLogs.Add(UpdateLogEntry);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error updating database.");
                    }
                }
                
            }


            return Ok("SMSS data successfully created/updated.");
        }

        // DELETE: api/SMSS/bulk
        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteMultipleSMSS( List<SMSS> smssList)
        {
            if (smssList == null || !smssList.Any())
            {
                return BadRequest("No SMSS data provided.");
            }

            foreach (var smss in smssList)
            {
                var existingSMSS = await _context.SMSS
                    .FirstOrDefaultAsync(e => e.Salesrep == smss.Salesrep && e.Year == smss.Year && e.Cmth == smss.Cmth);

                if (existingSMSS != null)
                {
                    _context.SMSS.Remove(existingSMSS);
                    // Log the update action
                    var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    var DeleteLogEntry = new ActionLog
                    {
                        ActionName = "DeleteSMSS",
                        PerformedBy = userId,
                        Entity = "SMSS",
                        Data = $"Delete Salesrep: {existingSMSS.Salesrep}, Year: {existingSMSS.Year}, Cmth: {existingSMSS.Cmth}",
                        Timestamp = DateTime.UtcNow
                    };
                    _context.ActionLogs.Add(DeleteLogEntry);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting from database.");
                    }
                }
                
            }

            return NoContent();
        }
    

    private bool SMSSExists(string Salesrep, int Year, int Cmth)
        {
            return _context.SMSS.Any(e => e.Salesrep == Salesrep && e.Year == Year && e.Cmth == Cmth);
        }
    }
}

