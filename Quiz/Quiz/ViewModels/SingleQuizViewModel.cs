using Quiz.Core;
using Quiz.Models;
using Quiz.Repository;
using System;

namespace Quiz.ViewModels
{
	public class SingleQuizViewModel : ViewModel
	{
		public SingleQuizModel FoundSingleQuizModel { get; set; }

		public RelayCommand PlayQuizzCommand { get; set; }
		public RelayCommand RemoveQuizzCommand { get; set; }
		public RelayCommand EditQuizzCommand { get; set; }

		public static event Action<int> StartQuizEvent;
		public static event Action<int, string> EditQuizEvent;
		public SingleQuizViewModel()
		{
			PlayQuizzCommand = new RelayCommand(o =>
			{
				if (FoundSingleQuizModel != null)
				{
					StartQuizEvent?.Invoke(FoundSingleQuizModel.ID);
				}
			},
			o => true
			);

			RemoveQuizzCommand = new RelayCommand(o =>
			{
				if (FoundSingleQuizModel != null)
				{
					SQLiteDataAccess.RemoveQuiz(FoundSingleQuizModel.ID);
					SearchViewModel.FoundedQuizzes.Remove(this);
				}
			},
			o => true
			);

			EditQuizzCommand = new RelayCommand(o =>
			{
				if (FoundSingleQuizModel != null)
				{
					EditQuizEvent?.Invoke(FoundSingleQuizModel.ID, FoundSingleQuizModel.QuizName);
				}
			},
			o => true
			);
		}

		public void PlayQuiz()
		{
			Console.WriteLine($"{FoundSingleQuizModel.QuizName} started...");
		}
	}
}
