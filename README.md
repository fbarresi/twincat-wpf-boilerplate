# twincat-wpf-boilerplate
an hackable template for building fabulous wpf hmi with twincat 

![.NET Core Build](https://github.com/fbarresi/twincat-wpf-boilerplate/workflows/.NET%20Core%20Build/badge.svg)

## Main features

- :heavy_check_mark: An easy-to-use alternative to other Beckhoff HMI engines (web or plc based)
- :heavy_check_mark: runs on every network connected device
- :heavy_check_mark: Professional application (Embedded Database, cleaned architecture, open source)
- :heavy_check_mark: Low code and XAML based: get nice usable dashboards in seconds only with XAML design
- :heavy_check_mark: Fully hackable, customizable and extendable with c#


## Quickstart

### Create your application

1. Install the template via console or powershell
```
dotnet new --install TwincatWpfHMI.template
```

2. Create your application with name `MyFancyHMI`

```
dotnet new tchmi -n MyFancyHMI -o MyFancyWorkingDirectory
```

### Design your HMI in XAML

#### With included controls

`todo`

#### With your custom controls

Create new user controls by:

- creating your custom controls and view models in `WpfApp.Gui\View` and `WpfApp.Gui\ViewModels`
- implementing `PlcUserControl` in XAML and in code-behind
- adding one of the included DataContext (`DataContext="{Binding PlcVariableViewModel, Source={StaticResource Locator}}"`)

XAML (.xaml)
```xml
<basics:PlcUserControl x:Class="WpfApp.Gui.Views.Basics.PlcVariableView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:local="clr-namespace:WpfApp.Gui.Views"
             xmlns:basics="clr-namespace:WpfApp.Gui.Views.Basics"
             DataContext="{Binding PlcVariableViewModel, Source={StaticResource Locator}}"
             mc:Ignorable="d"
             d:DesignHeight="300" d:DesignWidth="300">
    <StackPanel>
        <!-- add your stuff here -->
        <Label Content="{Binding VariablePath}" />
        <Label Content="{Binding RawValue}" />
    </StackPanel>
</basics:PlcUserControl>

```

Code-behind (xaml.cs)
```csharp
public partial class PlcVariableView : PlcUserControl
{
    public PlcVariableView()
    {
        InitializeComponent();
    }
}
```

### Run the application

`todo`

