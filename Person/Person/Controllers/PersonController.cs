using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Person.Models;
using Person.Elastic;
using System.Web.Script.Serialization;
using Nest;

namespace Person.Controllers
{
    public class PersonController : Controller
    {
        ElasticConnection elastic = new ElasticConnection();
        // GET: Person
        public void Index()
        {
            List<PersonModel> person = new List<PersonModel>();
            var person1 = new PersonModel
            {
                Id = "3",
                Firstname = "Dewa",
                Lastname = "Pandu"
            };

            var person2 = new PersonModel
            {
                Id = "2",
                Firstname = "Rusdy",
                Lastname = "Hafid"
            };
            person.Add(person1);
            person.Add(person2);
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //string json = js.Serialize(person);
            //return json;
            var index = elastic.client().IndexMany(person);
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public string Search(string searchEl)
        {
            var searchResults = elastic.client().Search<PersonModel>(s => s
                .AllIndices()
                .AllTypes()
                .From(0)
                .Size(10)
                .Query(q => q
                     .Match(m => m
                     .Field(f => f.Firstname)
                     .Query(searchEl)
                      )                
                )
            );
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(searchResults.Documents);
            return json;
        }
    }
}