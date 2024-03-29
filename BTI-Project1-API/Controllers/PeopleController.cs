﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BTI_Project1_API.Context;
using BTI_Project1_API.Models;
using BTI_Project1_API.Helper;
using BTI_Project1_API.Attributes;

namespace BTI_Project1_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PeopleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<_Person>>> GetPerson()
        {
            return await Helper.Convert.DbToPersonListAsync(_context);
        }

        // GET: api/People/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<_Person>>> GetAllPerson()
        {
            return await Helper.Convert.DbToPersonListAsync(_context, true);
        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<_Person>> GetPerson(int id)
        {
            var person = await _context.Person.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            if (!person.IsActive)
            {
                return NotFound();
            }

            return await Helper.Convert.DbToPersonAsync(person, _context);
            //return person;
        }

        // PUT: api/People/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            Helper.PutMethod.Person(_context, person);

            Person dbperson = await _context.Person.FindAsync(id);

            Helper.Copy.Action(person, dbperson);

            dbperson.ProjectIds = person.ProjectIds;
            dbperson.IsActive = person.IsActive;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetPerson", new { id = person.Id }, await Helper.Convert.DbToPersonAsync(person, _context));
            //return NoContent();
        }

        // PUT: api/People/pass
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("pass/{id}")]
        public async Task<ActionResult<bool>> ChangePassword(int id, PasswordStruct passwordStruct)
        {
            Person person = await _context.Person.FindAsync(id);

            int DbId = 0;

            foreach (var tempperson in _context.Person)
            {
                if (tempperson.UserName.Equals(passwordStruct.username))
                {
                    DbId = tempperson.Id;
                }
            }

            if (!person.Password.Equals(passwordStruct.oldPassword))
            {
                return false;
            }

            if(DbId != id)
            {
                return false;
            }

            person.Password = passwordStruct.newPassword;

            try
            {
                await _context.SaveChangesAsync();

                return true;
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/People
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            _context.Person.Add(Helper.Convert.PersonToDb(person, _context));
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }

        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            //foreach (var project in _context.Project)
            //{
            //    if (person.Id.ToString().Contains(project.PersonIds))
            //    {
            //        List<string> personIds = project.PersonIds.Split('-').ToList();
            //        personIds.Remove(person.Id.ToString());
            //        project.PersonIds = personIds.Count == 1 ? personIds[0] : String.Join('-', personIds);
            //    }
            //}

            person.IsActive = false;
            _context.Entry(person).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.Id == id);
        }


        public struct PasswordStruct
        {
            public string username { get; set; }
            public string oldPassword { get; set; }
            public string newPassword { get; set; }
        }
    }
}
