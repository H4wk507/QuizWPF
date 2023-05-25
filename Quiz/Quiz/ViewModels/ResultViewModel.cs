using Quiz.Core;
using Quiz.Services;

namespace Quiz.ViewModels
{
	public class ResultViewModel : ViewModel
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

		public static string Result { get; set; }
		public static string Time { get; set; }
		public RelayCommand NavigateToMainCommand { get; set; }
		public RelayCommand NavigateToSearchCommand { get; set; }

		public ResultViewModel(INavigationService navigationService)
		{
			Navigation = navigationService;
			NavigateToMainCommand = new RelayCommand(o => { Navigation.NavigateTo<MainViewModel>(); }, o => true);
			NavigateToSearchCommand = new RelayCommand(o => { Navigation.NavigateTo<SearchViewModel>(); }, o => true);
		}

		public static void InicializeResult(int questionsCout, int userCorrectAnswers, string time)
		{
			Result = $"You have scored {userCorrectAnswers} points out of {questionsCout}!";
			Time = $"At time {time}";
		}
	}
}
