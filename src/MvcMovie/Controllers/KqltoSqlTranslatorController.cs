using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Test;
using Kusto.Language;
namespace MvcMovie.Controllers
{
    public class KqltoSqlTranslatorController : Controller
    {
        // 
        // GET: /HelloWorld/

        public string Index()
        {
            string input = "T";
            KqltoSqlTranslatorClass t = new KqltoSqlTranslatorClass();
            string output;
            output = t.gettingSqlQuery(input);
            return output; 
        }
       
        public ActionResult GenerateSqlQuery()
        {
            return View();
            
        }
        [HttpPost]
        // GET: /HelloWorld/GenerateSqlQuery
        public ActionResult GenerateSqlQuery(string kqlQuery)
        {
            KqltoSqlTranslatorClass t = new KqltoSqlTranslatorClass();
            string output;
            output = t.gettingSqlQuery(kqlQuery);

            //return Content("Received kqlQuery:  " + kqlQuery);
            return Content("Converted SQL query : \n " + output);
        }

        // 
        // GET: /HelloWorld/Welcome/ 

        

    }
}