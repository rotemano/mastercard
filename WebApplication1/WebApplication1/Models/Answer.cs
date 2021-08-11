using System;
using System.Collections.Generic;
using System.Data;

namespace WebApplication1.Models
{
    public class Answer
    {
        int id;
        string label;

        public Answer() { }
        public Answer(int id, string label)
        {
            Id = id;
            Label = label;
        }

        public int Id { get => id; set => id = value; }
        public string Label { get => label; set => label = value; }

        public static bool SetAnswers(List<Question> questions, int user_id)
        {

            DataTable users_answerDT = new DataTable();
            users_answerDT.Columns.Add("user_id", typeof(int));
            users_answerDT.Columns.Add("question_id", typeof(int));
            users_answerDT.Columns.Add("answer_id", typeof(int));
            users_answerDT.Columns.Add("submit_datetime", typeof(DateTime));

            DataTable users_answer_commentsDT = new DataTable();
            users_answer_commentsDT.Columns.Add("user_id", typeof(int));
            users_answer_commentsDT.Columns.Add("question_id", typeof(int));
            users_answer_commentsDT.Columns.Add("submit_datetime", typeof(DateTime));
            users_answer_commentsDT.Columns.Add("comment", typeof(string));

            var submit_datetime = DateTime.Now;

            try
            {
               
                foreach (Question question in questions)
                {
                    var question_id = question.Id;
                    var comment = question.Comment;
                    //add answers into users_answers table
                    foreach (Answer answer in question.Answers)
                    {
                        var answer_id = answer.Id;
                        users_answerDT.Rows.Add(user_id, question_id, answer_id, submit_datetime);
                    }
                    //add question comments into users_answer_comments table,
                    //each row have unique (user_id, question_id, submit_datetime)
                    if (!String.IsNullOrEmpty(comment))
                    {
                        users_answer_commentsDT.Rows.Add(user_id, question_id, submit_datetime, comment);
                    }
                }
                bool success = DAL.BulkInsert(users_answerDT, "users_answers");
                if (success)
                    success = DAL.BulkInsert(users_answer_commentsDT, "users_answer_comments");
                return success;

            }
            catch (Exception ex)
            {

                ExceptionsLogger.LogException(ex);
                return false;
            }


        }

        public static int GetScore(int user_id)
        {
            try
            {
                Dictionary<string, string> queryParams = new Dictionary<string, string>();
                queryParams.Add("@user_id", user_id.ToString());
                var score=DAL.ExecuteScalar("sp_calc_score", queryParams, true);

                if (score is null)
                    return -1;
                return Convert.ToInt32(score);
            }
            catch (Exception ex)
            {
                ExceptionsLogger.LogException(ex);
                return -1;
            }

        }
    }
}