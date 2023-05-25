using Quiz.Core;
using Quiz.Services;
using System.Windows;

namespace Quiz.ViewModels
{
	public class MainWindowViewModel : ViewModel
	{
		private INavigationService _navigation;
		public INavigationService Navigation
		{
			get { return _navigation; }
			set
			{
				_navigation = value;
				OnPropertyChanged();
			}
		}

		public RelayCommand CloseAppCommand { get; set; }
		public RelayCommand MinimalizeAppCommand { get; set; }
		public RelayCommand MouseDownCommand { get; set; }

		public MainWindowViewModel(INavigationService navigationService)
		{
			Navigation = navigationService;
			Navigation.NavigateTo<MainViewModel>();

			CloseAppCommand = new RelayCommand(o => { Application.Current.Shutdown(); }, o => true);
			MinimalizeAppCommand = new RelayCommand(o => { Application.Current.MainWindow.WindowState = WindowState.Minimized; }, o => true);
			MouseDownCommand = new RelayCommand(o => { Application.Current.MainWindow.DragMove(); }, o => true);
		}
	}
}
