using Altamira.Bodega.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Altamira.Bodega.Presentation.Services;

public sealed class NavigationService : INavigationService
{
    private readonly Dictionary<Type, Type> _customMappings = new();
    private readonly Func<Type, FrameworkElement> _viewFactory;
    private readonly Dictionary<Type, object> _viewInstances = new();
    private readonly Stack<Type> _history = new();

    public event Action<object>? ViewChanged;

    public NavigationService(Func<Type, FrameworkElement> viewFactory)
    {
        _viewFactory = viewFactory;
    }

    public void Register<TViewModel, TView>()
        where TViewModel : class
        where TView : FrameworkElement
    {
        _customMappings[typeof(TViewModel)] = typeof(TView);
    }

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        var viewModelType = typeof(TViewModel);
        var viewType = ResolveViewType(viewModelType);

        if (viewType == null)
            throw new InvalidOperationException($"No hay vista registrada para el ViewModel {viewModelType.Name}.");

        if (!_viewInstances.TryGetValue(viewModelType, out var view))
        {
            view = _viewFactory(viewType);
            _viewInstances[viewModelType] = view;
        }

        _history.Push(viewModelType);
        ViewChanged?.Invoke(view ?? throw new InvalidOperationException("Vista no es FrameworkElement"));
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
        if (_viewInstances.TryGetValue(previous, out var view))
        {
            ViewChanged?.Invoke(view ?? throw new InvalidOperationException("Vista no es FrameworkElement"));
        }
    }

    private Type? ResolveViewType(Type viewModelType)
    {
        // 1. Buscar en mapeos personalizados
        if (_customMappings.TryGetValue(viewModelType, out var customViewType))
            return customViewType;

        // 2. Convención: ViewModel -> View (reemplazar "ViewModel" con "View")
        var viewModelName = viewModelType.Name;
        if (viewModelName.EndsWith("ViewModel"))
        {
            var viewName = viewModelName.Replace("ViewModel", "View");
            var viewType = viewModelType.Assembly.GetType(viewModelType.Namespace + "." + viewName);
            if (viewType != null && typeof(FrameworkElement).IsAssignableFrom(viewType))
                return viewType;
        }

        return null;
    }
}
