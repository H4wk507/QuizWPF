using Quiz.Core;
using Quiz.Models;
using System;

namespace Quiz.ViewModels
{
	public class SingleQuestionViewModel : ViewModel
	{
		public RelayCommand GetQuestionCommand { get; set; }
		public QuestionModel QuestionModel { get; set; }

		public int QuestionId { get; set; }

		public RelayCommand RemoveQuestionCommand { get; set; }
		public static Action<QuestionModel, int> ShowQuestionEvent;
		public SingleQuestionViewModel(QuestionModel model, int questionID)
		{
			QuestionId = questionID;
			QuestionModel = model;
			GetQuestionCommand = new RelayCommand(o => { ShowQuestionEvent?.Invoke(QuestionModel, QuestionId); }, o => true);
			RemoveQuestionCommand = new RelayCommand(o => RemoveQuestion(), o => true);
		}
		public SingleQuestionViewModel(QuestionModel model)
		{
			QuestionModel = model;
			GetQuestionCommand = new RelayCommand(o => { ShowQuestionEvent?.Invoke(QuestionModel, QuestionId); }, o => true);
			RemoveQuestionCommand = new RelayCommand(o => EditViewModel.SingleQuestions.Remove(this), o => true);
		}

		private void RemoveQuestion()
		{
			EditViewModel.SingleQuestions.Remove(this);
		}
	}
}
