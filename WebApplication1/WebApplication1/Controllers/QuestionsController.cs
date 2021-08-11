using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class QuestionsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<Question> Get()
        {
            try
            {
               // this.Request.Headers.
                List<Question> questionList = Question.GetQuestions();
                return questionList;
            }
            catch (Exception ex)
            {
                ExceptionsLogger.LogException(ex);
                throw new Exception("An error has occurred while processing the request.");
            }
        }


        // POST api/<controller>
        public int Post([FromBody]JObject stuff)
        {
            List<Question> questions = stuff["questions"].ToObject<List<Question>>();
            int user_id = stuff["user_id"].ToObject<int>();

            bool success = Answer.SetAnswers(questions, user_id);
            if (success)
                return Answer.GetScore(user_id);
            return -1;
        }

    }
}