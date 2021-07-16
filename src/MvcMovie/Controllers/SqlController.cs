using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test;

namespace KQLtoSQLtranslator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SqlController : ControllerBase
    {
        [HttpPost]
        public Query PostSql(Query request)
        {
            string query = request.query;
            Console.WriteLine(query);
            if (query.Trim() == "")
            {
                return null;
            }

            Response.Headers.TryAdd("Access-Control-Allow-Origin", "*");

            KqltoSqlTranslatorClass t = new KqltoSqlTranslatorClass();
            string output;
            output = t.gettingSqlQuery(query);

            return new Query
            {
                query = output
            };
        }
    }


    public class Query
    {
        public string query { get; set; }
    }
}
