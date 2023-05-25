using Quiz.Core;
using Quiz.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Quiz.ViewModels
{
	public class NewQuestionViewModel : ViewModel
	{
		public ObservableCollection<AnswerModel> Answers { get; set; } = new ObservableCollection<AnswerModel>();
		public int QuestionNumber { get; set; }

		private string _question;
		[Required(ErrorMessage = "Must not be empty.")]
		public string Question
		{
			get => _question;
			set
			{
				_question = value;
				OnPropertyChanged();
				CustomValidator.TryValidateProperty(value, nameof(Question), out var errorMessage, this);
				if (!string.IsNullOrEmpty(errorMessage))
				{
					throw new ValidationException(errorMessage);
				}
			}
		}

		public RelayCommand RemoveQuestionCommand { get; set; }

		public static Action<object> RemoveQuestionEvent { get; set; }
		public NewQuestionViewModel()
		{
			RemoveQuestionCommand = new RelayCommand(o => { RemoveQuestionEvent?.Invoke(this); }, o => true);
			for (int i = 0; i < 4; i++)
			{
				Answers.Add(new AnswerModel());
			}
		}
	}
}
