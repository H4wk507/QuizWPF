using Quiz.Core;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Models
{
	public class AnswerModel : ViewModel
	{
		public int ID { get; set; }
		private string answer;
		[Required(ErrorMessage = "Must not be empty.")]
		public string Answer
		{
			get => answer;
			set
			{
				answer = value;
				OnPropertyChanged();
				CustomValidator.TryValidateProperty(value, nameof(Answer), out var errorMessage, this);
				if (!string.IsNullOrEmpty(errorMessage))
				{
					throw new ValidationException(errorMessage);
				}
			}
		}
		public bool IsCorrect { get; set; } = false;
	}
}
