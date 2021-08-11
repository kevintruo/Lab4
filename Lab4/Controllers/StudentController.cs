using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;

namespace Lab4.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolCommunityContext _context;

        public StudentController(SchoolCommunityContext context)
        {
            _context = context;
        }

        // GET: Student
        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new CommunityViewModel();
            viewModel.Students = await _context.Students
                  .Include(i => i.Membership)
                  .ThenInclude(i => i.Community)
                  .AsNoTracking()
                  .OrderBy(i => i.id)
                  .ToListAsync();

            if (id != null)
            {
                ViewData["StudentID"] = id;
                viewModel.CommunityMemberships = viewModel.Students.Where(
                    x => x.id == id).Single().Membership;
            }
            return View(viewModel);
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> EditMemberships(int? id)
        {
            //Create a local variable for student membership view model
            var viewModel = new StudentMembershipViewModel();
            //Get the student with the parameter ID
            viewModel.Student = _context.Students.Where(x => x.id == id).Single();

            //Get a list of all communities
            List<Community> commList = await _context.Communities
                .Include(i => i.Membership)
                .AsNoTracking()
                .OrderBy(i => i.Title)
                .ToListAsync();

            //Create a new list of community membership model
            List<CommunityMembershipViewModel> list = new List<CommunityMembershipViewModel>();
            //Iterate through all the communities and initialize a new view model and add them into our new list of view models
            foreach(var comm in commList)
            {
                //Create a temporary new view model
                var commModel = new CommunityMembershipViewModel();
                //Initialize data
                commModel.CommunityId = comm.Id;
                commModel.Title = comm.Title;
                //Find out if any students are a member of this community
                foreach (var member in comm.Membership)
                {
                    if (member.StudentId == id)
                        commModel.IsMember = true;
                }
                //Add new view model to the list of view models
                list.Add(commModel);
            }
            //Query to sort descending by isMember (because false is 0 and true is 1), and then sort ascending by Title
            IEnumerable<CommunityMembershipViewModel> orderedByTitle = list.OrderByDescending(l => l.IsMember).ThenBy(l => l.Title);
            //Execute query
            list = orderedByTitle.ToList<CommunityMembershipViewModel>();
            
            //Set memberships to list of community membership view models and return the student membership view model
            viewModel.Memberships = list;
            return View(viewModel);
        }

        public async Task<IActionResult> RemoveMembership(int? studentId, string commId)
        {
            //Create a temporary community membership
            CommunityMembership temp = new CommunityMembership();
            //Look up that membership
            temp = _context.CommunityMemberships.Where(x => x.StudentId == studentId && x.CommunityId == commId).Single();
            if(temp == null)
            {
                return NotFound();
            }
            //remove if found
            _context.CommunityMemberships.Remove(temp);
            await _context.SaveChangesAsync();
            //redirect to editmemberships.cshtml
            return RedirectToAction(nameof(EditMemberships), new { id = studentId });
        }

        public async Task<IActionResult> AddMembership(int? studentId, string commId)
        {
            //Create a temporary community membership
            CommunityMembership temp = new CommunityMembership();
            //Initialize variable 
            temp.CommunityId = commId;
            temp.StudentId = (int)studentId;
            //Add membership
            _context.CommunityMemberships.Add(temp);
            await _context.SaveChangesAsync();
            //redirect to editmemberships.cshtml
            return RedirectToAction(nameof(EditMemberships), new { id = studentId });
        }

        // POST: Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,LastName,FirstName,EnrollmentDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,LastName,FirstName,EnrollmentDate")] Student student)
        {
            if (id != student.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.id == id);
        }
    }
}