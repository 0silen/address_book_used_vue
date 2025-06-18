using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;

namespace ContactManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAllContacts()
        {
            return await _context.Contacts.ToListAsync();
        }

        // GET: api/contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContactById(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();
            return contact;
        }

        // POST: api/contacts
        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
        }

        // PUT: api/contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, Contact contact)
        {
            if (id != contact.Id)
                return BadRequest();

            _context.Entry(contact).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Contacts.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        // DELETE: api/contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return NotFound();
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/contacts/search?query=xxx
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Contact>>> SearchContacts([FromQuery] string query)
        {
            return await _context.Contacts
                .Where(c => c.Name.Contains(query) || c.PhoneNumber.Contains(query) || c.Keywords.Contains(query))
                .ToListAsync();
        }

        // POST: api/contacts/import
        [HttpPost("import")]
        public async Task<IActionResult> ImportContacts([FromBody] List<Contact> contacts)
        {
            _context.Contacts.AddRange(contacts);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/contacts/export
        [HttpGet("export")]
        public async Task<ActionResult<IEnumerable<Contact>>> ExportContacts()
        {
            return await _context.Contacts.ToListAsync();
        }

        // POST: api/contacts/merge
        [HttpPost("merge")]
        public async Task<IActionResult> MergeDuplicateContacts()
        {
            var grouped = await _context.Contacts
                .GroupBy(c => new { c.Name, c.PhoneNumber })
                .ToListAsync();

            var unique = grouped.Select(g => g.First()).ToList();
            _context.Contacts.RemoveRange(_context.Contacts);
            _context.Contacts.AddRange(unique);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}