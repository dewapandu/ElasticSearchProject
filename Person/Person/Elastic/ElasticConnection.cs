using Nest;
using System;
using Person.Models;
using Person.Elastic;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Person.Elastic
{
    public class ElasticConnection
    {
        public ElasticClient client()
        {
            var node = new Uri("http://localhost:9200");

            var settings = new ConnectionSettings(
                node
            ).DefaultIndex("personmodels");

            var client = new ElasticClient(settings);
            return client;
        }

        public void Index(PersonModel person)
        {
            var index = client().Index(person, i => i
                .Index("personmodels")
                .Type("personmodel")
                .Id(person.Id)
                .Ttl("1m")
            );
        }
    }
}