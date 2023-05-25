using Quiz.Models;
using Quiz.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;

namespace Quiz.Repository
{
	public class SQLiteDataAccess
	{
		public static void CreateQuizz(CreateViewModel model)
		{
			using (var connection = new SQLiteConnection(LoadConnectionString()))
			{
				connection.Open();

				//Adding a quiz
				string insertQuizSql = "INSERT INTO Quizzes (Name) VALUES (@name);";
				var insertQuizCommand = new SQLiteCommand(insertQuizSql, connection);
				insertQuizCommand.Parameters.AddWithValue("@name", model.Name);
				insertQuizCommand.ExecuteNonQuery();

				//Getting ID of added quiz
				string selectQuizSql = "SELECT ID FROM Quizzes WHERE Name = @name;";
				var selectQuizCommand = new SQLiteCommand(selectQuizSql, connection);
				selectQuizCommand.Parameters.AddWithValue("@name", model.Name);
				int quizId = Convert.ToInt32(selectQuizCommand.ExecuteScalar());

				//Adding questions and answers
				foreach (var question in model.NewQuestionList)
				{
					//Adding questions
					string insertQuestionSql = "INSERT INTO Questions (Text, QuizID) VALUES (@text, @quizId);";
					var insertQuestionCommand = new SQLiteCommand(insertQuestionSql, connection);
					insertQuestionCommand.Parameters.AddWithValue("@text", question.Question);
					insertQuestionCommand.Parameters.AddWithValue("@quizId", quizId);
					insertQuestionCommand.ExecuteNonQuery();

					//Getting ID of added questions
					string selectQuestionSql = "SELECT last_insert_rowid();";
					var selectQuestionCommand = new SQLiteCommand(selectQuestionSql, connection);
					int questionId = Convert.ToInt32(selectQuestionCommand.ExecuteScalar());

					//Adding answers
					foreach (var answer in question.Answers)
					{
						string insertAnswerSql = "INSERT INTO Answers (Text, IsCorrect, QuestionID) VALUES (@text, @isCorrect, @questionId);";
						var insertAnswerCommand = new SQLiteCommand(insertAnswerSql, connection);
						insertAnswerCommand.Parameters.AddWithValue("@text", answer.Answer);
						insertAnswerCommand.Parameters.AddWithValue("@isCorrect", answer.IsCorrect);
						insertAnswerCommand.Parameters.AddWithValue("@questionId", questionId);
						insertAnswerCommand.ExecuteNonQuery();
					}
				}
				connection.Close();
			}
		}

		public static void RemoveQuiz(int quizId)
		{

			RemoveQuestions(quizId);
			using (var connection = new SQLiteConnection(LoadConnectionString()))
			{
				connection.Open();

				//Removing Quiz
				using (var command = new SQLiteCommand("DELETE FROM Quizzes WHERE ID = @quizId", connection))
				{
					command.Parameters.AddWithValue("@quizId", quizId);
					command.ExecuteNonQuery();
				}

				connection.Close();
			}
		}

		public static void RemoveQuestions(int quizId)
		{
			using (var connection = new SQLiteConnection(LoadConnectionString()))
			{
				connection.Open();

				//Removing Answers
				using (var command = new SQLiteCommand("DELETE FROM Answers WHERE QuestionID IN (SELECT ID FROM Questions WHERE QuizID = @quizId)", connection))
				{
					command.Parameters.AddWithValue("@quizId", quizId);
					command.ExecuteNonQuery();
				}

				//Removing Questions
				using (var command = new SQLiteCommand("DELETE FROM Questions WHERE QuizID = @quizId", connection))
				{
					command.Parameters.AddWithValue("@quizId", quizId);
					command.ExecuteNonQuery();
				}

				connection.Close();
			}
		}

		public static void AddQuestions(int quizId, List<SingleQuestionViewModel> questions)
		{
			using (var connection = new SQLiteConnection(LoadConnectionString()))
			{
				connection.Open();

				//Adding questions and answers
				foreach (var question in questions)
				{
					//Adding questions
					string insertQuestionSql = "INSERT INTO Questions (Text, QuizID) VALUES (@text, @quizId);";
					var insertQuestionCommand = new SQLiteCommand(insertQuestionSql, connection);
					insertQuestionCommand.Parameters.AddWithValue("@text", question.QuestionModel.Question);
					insertQuestionCommand.Parameters.AddWithValue("@quizId", quizId);
					insertQuestionCommand.ExecuteNonQuery();

					//Getting ID of added questions
					string selectQuestionSql = "SELECT last_insert_rowid();";
					var selectQuestionCommand = new SQLiteCommand(selectQuestionSql, connection);
					int questionId = Convert.ToInt32(selectQuestionCommand.ExecuteScalar());

					//Adding answers
					foreach (var answer in question.QuestionModel.Answers)
					{
						string insertAnswerSql = "INSERT INTO Answers (Text, IsCorrect, QuestionID) VALUES (@text, @isCorrect, @questionId);";
						var insertAnswerCommand = new SQLiteCommand(insertAnswerSql, connection);
						insertAnswerCommand.Parameters.AddWithValue("@text", answer.Answer);
						insertAnswerCommand.Parameters.AddWithValue("@isCorrect", answer.IsCorrect);
						insertAnswerCommand.Parameters.AddWithValue("@questionId", questionId);
						insertAnswerCommand.ExecuteNonQuery();
					}
				}
				connection.Close();
			}
		}

		public static List<SingleQuizModel> GetQuizzes()
		{
			List<SingleQuizModel> quizzesFound = new List<SingleQuizModel>();
			using (var connection = new SQLiteConnection(LoadConnectionString()))
			{
				connection.Open();

				string selectQuizzesSql = "SELECT ID, Name FROM Quizzes;";
				var selectQuizzesCommand = new SQLiteCommand(selectQuizzesSql, connection);

				using (var reader = selectQuizzesCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						int quizId = reader.GetInt32(0);
						string quizName = reader.GetString(1);
						var quizInfo = new SingleQuizModel()
						{
							ID = quizId,
							QuizName = quizName,
						};
						quizzesFound.Add(quizInfo);
					}
				}
				connection.Close();
			}
			return quizzesFound;
		}

		public static List<QuestionModel> GetQuestions(int quizID)
		{
			List<QuestionModel> questions = new List<QuestionModel>();
			using (var connection = new SQLiteConnection(LoadConnectionString()))
			{
				connection.Open();

				string selectQuestions = "SELECT ID, Text FROM Questions Where QuizID = @quizID";
				SQLiteCommand selectQuestionsCommand = new SQLiteCommand(selectQuestions, connection);
				selectQuestionsCommand.Parameters.AddWithValue("@quizID", quizID);

				SQLiteDataReader reader = selectQuestionsCommand.ExecuteReader();
				int questionNumber = 1;
				while (reader.Read())
				{
					questions.Add(new QuestionModel()
					{
						ID = reader.GetInt32(0),
						QuestionNumber = questionNumber,
						Question = reader.GetString(1),
						Answers = new List<AnswerModel>(),
					});
					questionNumber++;
				}
				reader.Close();

				foreach (var question in questions)
				{
					int questionId = question.ID;
					string selectAnswers = "SELECT Text, IsCorrect FROM Answers Where QuestionID = @questionID";
					SQLiteCommand selectAnswersCommand = new SQLiteCommand(selectAnswers, connection);
					selectAnswersCommand.Parameters.AddWithValue("@questionID", questionId);

					SQLiteDataReader answerReader = selectAnswersCommand.ExecuteReader();
					int answerID = 0;
					while (answerReader.Read())
					{
						question.Answers.Add(new AnswerModel()
						{
							ID = answerID,
							Answer = answerReader.GetString(0),
							IsCorrect = answerReader.GetBoolean(1),
						}); ;
						answerID++;
					}
					answerReader.Close();
				}
				connection.Close();
			}
			return questions;
		}

		private static string LoadConnectionString(string id = "Default")
		{
			return ConfigurationManager.ConnectionStrings[id].ConnectionString;
		}
	}
}
