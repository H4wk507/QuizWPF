using Quiz.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Models
{
	public class QuestionModel : ObservableObject
	{
		public int ID { get; set; }
		private string question;
		[Required(ErrorMessage = "Must not be empty.")]
		public string Question
		{
			get => question;
			set
			{
				question = value;
				OnPropertyChanged();
				CustomValidator.TryValidateProperty(value, nameof(Question), out var errorMessage, this);
				if (!string.IsNullOrEmpty(errorMessage))
				{
					throw new ValidationException(errorMessage);
				}
			}
		}

		private int questionNumber;
		public int QuestionNumber
		{
			get { return questionNumber; }
			set
			{
				questionNumber = value;
				OnPropertyChanged(nameof(QuestionNumber));
			}
		}

		public List<AnswerModel> Answers { get; set; } = new List<AnswerModel>();
	}
}
