using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Safecheck.Models;
using System.Linq;

namespace Safecheck.Controllers
{
    [Route("api/forms")]
    public class SafecheckController : Controller
    {
        
        private List<FormList> forms;


        public SafecheckController()
        {
            forms = new List<FormList>();
            FormList basic = new FormList();
            basic.Id = 1;
            basic.Name = "Basic Form";
            forms.Add(basic);

        }   

        //Generate some routes..
        [HttpGet]
        public IEnumerable<FormList> Get()
        {
            return forms.ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById(long id)
        {
            var form = buildForm();
            if (form == null)
            {
                return NotFound();
            }
            return new ObjectResult(form);
        }

        private Form buildForm()
        {
            Form form = new Form();
            form.Id = 2;
            form.Name = "Basic form";

            Item name = new Item();
            name.name = "Name";
            name.type = "TEXT";
        
            Item dob = new Item();
            dob.name = "DOB";
            dob.type = "DATE";
        
            Item contactPhone = new Item();
            contactPhone.name = "Phone";
            contactPhone.type = "TEXT";
        
            Item allergies = new Item();
            allergies.name = "Allergies";
            allergies.type = "TEXT";
            
        
            Item note = new Item();
            note.name="Note that goes somewhere.";
            note.type = "NOTE";
            
            Section parent = new Section();
            parent.name ="Parent";
            parent.addItem(name);
            parent.addItem(dob);
            parent.addItem(contactPhone);
            
        
            form.addSection(parent);
        
        
        
            Section child = new Section();
            child.name = "Child";
            child.repeatable = true;
            child.addItem(name);
            child.addItem(dob);
            child.addItem(allergies);
        
    

            form.addSection(child);
        

            return form;


        }



    }
}