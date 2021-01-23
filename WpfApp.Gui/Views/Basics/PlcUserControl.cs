using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using WpfApp.Gui.ViewModels;
using WpfApp.Gui.ViewModels.Basics;

namespace WpfApp.Gui.Views.Basics
{
    public class PlcUserControl : UserControl, IViewFor<PlcVariableViewModel>
    {
        public PlcUserControl()
        {
            this.WhenActivated(disposables =>
            {
                if (ViewModel != null)
                {
                    ViewModel.SetupVariableViewModel(PlcName, VariablePath, Label, Description);
                    ViewModel.DisposeWith(disposables);
                }
            });
        }

        public readonly DependencyProperty VariablePathProperty = DependencyProperty.Register(nameof(VariablePath)+Guid.NewGuid(), typeof(string), typeof(PlcUserControl));
        public readonly DependencyProperty PlcNameProperty = DependencyProperty.Register(nameof(PlcNameProperty)+Guid.NewGuid(), typeof(string), typeof(PlcUserControl));
        public readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(LabelProperty)+Guid.NewGuid(), typeof(string), typeof(PlcUserControl));
        public readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(DescriptionProperty)+Guid.NewGuid(), typeof(string), typeof(PlcUserControl));

        public string VariablePath
        {
            get => (string) GetValue(VariablePathProperty);
            set => SetValue(VariablePathProperty, value);
        }

        public string PlcName 
        {
            get => (string) GetValue(PlcNameProperty);
            set => SetValue(PlcNameProperty, value);
        }
        public string Label 
        {
            get => (string) GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
        public string Description 
        {
            get => (string) GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as PlcVariableViewModel;
        }

        public PlcVariableViewModel? ViewModel
        {
            get => DataContext as PlcVariableViewModel;
            set => DataContext = value;
        }
    }
}