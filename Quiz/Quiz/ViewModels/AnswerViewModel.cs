using Quiz.Core;
using Quiz.Models;
using Quiz.Repository;
using Quiz.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

namespace Quiz.ViewModels
{
	public class AnswerViewModel : ViewModel
	{
		//Properties
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

		public static ObservableCollection<QuestionModel> Questions { get; set; } = new ObservableCollection<QuestionModel>();
		private static List<bool> UserAnswers { get; set; } = new List<bool>();
		public AnswerModel AnswerA { get; set; } = new AnswerModel();
		public AnswerModel AnswerB { get; set; } = new AnswerModel();
		public AnswerModel AnswerC { get; set; } = new AnswerModel();
		public AnswerModel AnswerD { get; set; } = new AnswerModel();
		public static int TotalQuestionsCout { get; set; }
		private static Action InicializeEvent { get; set; }

		//Commands
		public RelayCommand NextQuestionCommand { get; set; }
		public RelayCommand FinishQuizzCommand { get; set; }

		//Timer
		public static DispatcherTimer timer = new DispatcherTimer();
		public static Stopwatch stopwatch = new Stopwatch();
		private string time;

		public string Time
		{
			get => time;
			set
			{
				time = value;
				OnPropertyChanged();
			}
		}


		//Constructor
		public AnswerViewModel(INavigationService navigation)
		{
			Navigation = navigation;
			NextQuestionCommand = new RelayCommand(MoveToNextQuestion, o => true);
			FinishQuizzCommand = new RelayCommand(o => EndQuiz(), o => true);
			InitializeQuestion();

			timer.Interval = TimeSpan.FromMilliseconds(1);
			timer.Tick += Timer_Tick;

			InicializeEvent += Initialize;
		}

		//Methods
		public static void InitializeQuiz(int quizID)
		{
			stopwatch.Reset();
			Questions.Clear();
			SQLiteDataAccess.GetQuestions(quizID).ForEach(x => Questions.Add(x));
			UserAnswers.Clear();
			TotalQuestionsCout = Questions.Count;
			InicializeEvent?.Invoke();
			timer.Start();
			stopwatch.Start();
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			TimeSpan elapsed = stopwatch.Elapsed;
			Time = string.Format("{0:00}:{1:00}:{2:00}", elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds / 10);
		}

		private void MoveToNextQuestion(object parameter)
		{
			if (Questions.Count >= 0)
			{
				UserAnswers.Add(Questions[0].Answers[int.Parse(parameter.ToString())].IsCorrect);
			}

			if (Questions.Count > 0)
				Questions.RemoveAt(0);

			if (Questions.Count > 0)
			{
				InitializeQuestion();
			}

			if (Questions.Count == 0)
			{
				EndQuiz();
			}
		}

		private void EndQuiz()
		{
			ResultViewModel.InicializeResult(TotalQuestionsCout, UserAnswers.Count(x => x == true), Time);
			Navigation.NavigateTo<ResultViewModel>();
			timer.Stop();
			stopwatch.Stop();
		}

		private void Initialize()
		{
			InitializeQuestion();
		}

		private void InitializeQuestion()
		{
			AnswerA = Questions[0].Answers[0];
			AnswerB = Questions[0].Answers[1];
			AnswerC = Questions[0].Answers[2];
			AnswerD = Questions[0].Answers[3];
			OnPropertyChanged(nameof(AnswerA));
			OnPropertyChanged(nameof(AnswerB));
			OnPropertyChanged(nameof(AnswerC));
			OnPropertyChanged(nameof(AnswerD));
		}
	}
}
