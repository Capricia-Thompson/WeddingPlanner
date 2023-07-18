using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;

public class WeddingController : Controller
{
    private readonly ILogger<WeddingController> _logger;
    private MyContext _context;

    public WeddingController(ILogger<WeddingController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [SessionCheck]
    [HttpGet("weddings")]
    public IActionResult Weddings()
    {
        List<Wedding> AllWeddings = _context.Weddings.Include(w => w.Guests).ThenInclude(g => g.User).ToList();
        return View(AllWeddings);
    }

    [SessionCheck]
    [HttpGet("weddings/new")]
    public IActionResult WeddingForm()
    {
        return View();
    }

    [SessionCheck]
    [HttpPost("weddings/create")]
    public IActionResult NewWedding(Wedding newWedding)
    {
        if (ModelState.IsValid)
        {
            int Uid = (int)HttpContext.Session.GetInt32("UserId");
            newWedding.UserId = Uid;
            _context.Add(newWedding);
            _context.SaveChanges();
            int WeddingId = newWedding.WeddingId;
            return Redirect($"/weddings/{WeddingId}");
        }
        else
        {
            return View("WeddingForm", newWedding);
        }
    }

    [SessionCheck]
    [HttpGet("weddings/{WedId}")]
    public IActionResult WeddingDetails(int WedId)
    {
        Wedding? Event = _context.Weddings.Include(w => w.Guests).ThenInclude(g => g.User).FirstOrDefault(w => w.WeddingId == WedId);

        if (Event == null)
        {
            return RedirectToAction("Weddings");
        }
        return View("WeddingDetails", Event);
    }

    [SessionCheck]
    [HttpPost("weddings/{WedId}/destroy")]
    public IActionResult Delete(int WedId)
    {
        Wedding? wedToDelete = _context.Weddings.FirstOrDefault(w => w.WeddingId == WedId);
        _context.Weddings.Remove(wedToDelete);
        _context.SaveChanges();
        return RedirectToAction("Weddings");
    }

    [SessionCheck]
    [HttpPost("/weddings/{WedId}/RSVP")]
    public IActionResult RSVP(int WedId)
    {
        int userId = (int)HttpContext.Session.GetInt32("UserId");

        Attendee? existingRSVP = _context.Attendees.FirstOrDefault(a => a.UserId == userId && a.WeddingId == WedId);

        if (existingRSVP != null)
        {
            _context.Attendees.Remove(existingRSVP);
            _context.SaveChanges();
            return RedirectToAction("Weddings");
        }

        Attendee newRSVP = new Attendee()
        {
            WeddingId = WedId,
            UserId = userId
        };
        _context.Attendees.Add(newRSVP);
        _context.SaveChanges();
        return RedirectToAction("Weddings");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}