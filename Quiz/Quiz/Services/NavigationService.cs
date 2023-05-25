using Quiz.Core;
using System;

namespace Quiz.Services
{
	public interface INavigationService
	{
		ViewModel CurrentView { get; }
		void NavigateTo<T>() where T : ViewModel;
	}

	public class NavigationService : ObservableObject, INavigationService
	{
		//Properties
		private ViewModel _currentView;
		private readonly Func<Type, ViewModel> _viewModelFactory;
		public ViewModel CurrentView
		{
			get => _currentView;
			private set
			{
				_currentView = value;
				OnPropertyChanged();
			}
		}

		//Constructor
		public NavigationService(Func<Type, ViewModel> viewModelFactory)
		{
			_viewModelFactory = viewModelFactory;
		}

		//Methods
		public void NavigateTo<TViewModel>() where TViewModel : ViewModel
		{
			ViewModel viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
			CurrentView = viewModel;
		}
	}
}
