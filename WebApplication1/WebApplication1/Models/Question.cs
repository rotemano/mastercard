using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Question
    {
        int id;
        string label;
        int type_id;
        string comment;
        List<Answer> answers;
        public Question() { }
        public Question(int id, string label, int type_id)
        {
            Id = id;
            Label = label;
            Type_id = type_id;
        }

        public Question(int id, string label, int type_id, List<Answer> answers) : this(id, label, type_id)
        {
            Answers = answers;
        }

        public int Id { get => id; set => id = value; }
        public string Label { get => label; set => label = value; }
        public int Type_id { get => type_id; set => type_id = value; }
        public string Comment { get => comment; set => comment = value; }
        public List<Answer> Answers { get => answers; set => answers = value; }

        //Get all the questions with their type and answers
        public static List<Question> GetQuestions()
        {
            List<Question> questionsList = new List<Question>();
            try
            {
                DataTable Q_dt = DAL.FillDataTableFromSp("sp_get_questions", new Dictionary<string, string>());
                DataTable A_dt = DAL.FillDataTableFromSp("sp_get_answers", new Dictionary<string, string>());

                foreach (DataRow dr_q in Q_dt.Rows)
                {
                    Question q = new Question(Convert.ToInt32(dr_q["id"]), dr_q["label"].ToString(), Convert.ToInt32(dr_q["type_id"]), new List<Answer>());
                    IEnumerable<object> answersForQuestion = from answer in A_dt.AsEnumerable()
                                                              where answer["question_id"].ToString() == q.Id.ToString()
                                                              select answer;
                    //add for each question it's answeres
                    foreach (DataRow dr_a in answersForQuestion)
                    {
                        q.Answers.Add(new Answer(Convert.ToInt32(dr_a["id"]), dr_a["Label"].ToString()));
                    }
                    questionsList.Add(q);
                }



            }
            catch (Exception ex)
            {

                ExceptionsLogger.LogException(ex);
            }



            return questionsList;

        }
    }
}