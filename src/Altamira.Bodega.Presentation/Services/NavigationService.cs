using Altamira.Bodega.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Altamira.Bodega.Presentation.Services;

public sealed class NavigationService : INavigationService
{
    private readonly Dictionary<Type, Type> _viewModelToViewMap = new();
    private readonly Func<Type, FrameworkElement> _viewFactory;
    private readonly Dictionary<Type, object> _viewInstances = new();
    private readonly Stack<Type> _history = new();

    public event Action<FrameworkElement>? ViewChanged;

    public NavigationService(Func<Type, FrameworkElement> viewFactory)
    {
        _viewFactory = viewFactory;
    }

    public void Register<TViewModel, TView>()
        where TViewModel : class
        where TView : FrameworkElement
    {
        _viewModelToViewMap[typeof(TViewModel)] = typeof(TView);
    }

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        var viewModelType = typeof(TViewModel);
        if (!_viewModelToViewMap.TryGetValue(viewModelType, out var viewType))
            throw new InvalidOperationException($"No hay vista registrada para el ViewModel {viewModelType.Name}.");

        // Reutilizar instancia si ya existe
        if (!_viewInstances.TryGetValue(viewModelType, out var view))
        {
            view = _viewFactory(viewType);
            _viewInstances[viewModelType] = view;
        }

        _history.Push(viewModelType);
        ViewChanged?.Invoke(view as FrameworkElement ?? throw new InvalidOperationException("Vista no es FrameworkElement"));
    }

    public void NavigateTo<TViewModel, TParameter>(TParameter parameter) where TViewModel : class
    {
        NavigateTo<TViewModel>();
    }

    public bool CanGoBack => _history.Count > 1;

    public void GoBack()
    {
        if (!CanGoBack) return;
        _history.Pop();
        var previous = _history.Peek();
        if (_viewModelToViewMap.TryGetValue(previous, out var viewType) &&
            _viewInstances.TryGetValue(previous, out var view))
        {
            ViewChanged?.Invoke(view as FrameworkElement ?? throw new InvalidOperationException("Vista no es FrameworkElement"));
        }
    }
}
