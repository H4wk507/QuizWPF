using Quiz.Core;
using Quiz.Models;
using Quiz.Repository;
using Quiz.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quiz.ViewModels
{
	public class EditViewModel : ViewModel
	{
		private INavigationService _navigation;
		public INavigationService Navigation
		{
			get => _navigation;
			set
			{
				_navigation = value;
				OnPropertyChanged();
			}
		}

		public static int DisplayedQuizID { get; set; }
		public static int DisplayedQuestionID { get; set; }
		public static string QuizName { get; set; }
		public QuestionModel DisplayedQuestion { get; set; } = new QuestionModel();
		public static List<QuestionModel> QuestionsList { get; set; } = new List<QuestionModel>();
		public static ObservableCollection<SingleQuestionViewModel> SingleQuestions { get; set; } = new ObservableCollection<SingleQuestionViewModel>();
		public RelayCommand NavigateToSearchViewCommand { get; set; }
		public RelayCommand SaveChangesCommand { get; set; }
		public RelayCommand DiscardChangesCommand { get; set; }
		public RelayCommand AddNewQuestionCommand { get; set; }

		public EditViewModel(INavigationService navigation)
		{
			Navigation = navigation;
			NavigateToSearchViewCommand = new RelayCommand(o => { Navigation.NavigateTo<SearchViewModel>(); }, o => true);
			SaveChangesCommand = new RelayCommand(o => { SaveChanges(); }, o => IsValid());
			DiscardChangesCommand = new RelayCommand(o => { DiscardChanges(); }, o => true);
			AddNewQuestionCommand = new RelayCommand(o => AddNewQuestion(), o => true);
			SingleQuestionViewModel.ShowQuestionEvent += ShowQuestion;
		}

		private void AddNewQuestion()
		{
			SingleQuestions.Add(new SingleQuestionViewModel(new QuestionModel()));
			SingleQuestions[^1].QuestionModel.QuestionNumber = SingleQuestions.Count;
			for (int i = 0; i < 4; i++)
			{
				SingleQuestions[^1].QuestionModel.Answers.Add(new AnswerModel());
			}
		}

		public void SaveChanges()
		{
			SQLiteDataAccess.RemoveQuestions(DisplayedQuizID);
			SQLiteDataAccess.AddQuestions(DisplayedQuizID, SingleQuestions.ToList());
		}

		public void DiscardChanges()
		{
			ReloadLists(DisplayedQuizID);

			DisplayedQuestion = SingleQuestions[DisplayedQuestionID].QuestionModel;
			OnPropertyChanged(nameof(DisplayedQuestion));
		}

		private void ShowQuestion(QuestionModel question, int questionID)
		{
			DisplayedQuestion = question;
			DisplayedQuestionID = questionID;
			OnPropertyChanged(nameof(DisplayedQuestion));
		}

		private static void ReloadLists(int quizID)
		{
			SingleQuestions.Clear();
			QuestionsList.Clear();
			SQLiteDataAccess.GetQuestions(quizID).ForEach(x => QuestionsList.Add(x));
			int index = 0;
			QuestionsList.ForEach(x => { SingleQuestions.Add(new SingleQuestionViewModel(x, index++)); });
		}

		public static void InitializeEditMode(int quizID, string quizName)
		{
			QuizName = quizName;
			DisplayedQuizID = quizID;
			ReloadLists(DisplayedQuizID);
		}


		private bool IsValid()
		{
			CustomValidator.TryValidateObject(this, out var errorMessage);
			if (!string.IsNullOrEmpty(errorMessage))
			{
				return false;
			}

			foreach (var question in SingleQuestions)
			{
				CustomValidator.TryValidateObject(question.QuestionModel, out errorMessage);
				if (!string.IsNullOrEmpty(errorMessage))
				{
					return false;
				}

				if (!question.QuestionModel.Answers.Any(a => a.IsCorrect))
				{
					return false;
				}

				foreach (var answer in question.QuestionModel.Answers)
				{
					CustomValidator.TryValidateObject(answer, out errorMessage);
					if (!string.IsNullOrEmpty(errorMessage))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
