using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;
using WpfApp.Gui.Design;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Hardware;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Gui.ViewModels
{
    public class GraphViewModel: ViewModelBase
    {
        private readonly IPlcProvider plcProvider;
        private readonly ApplicationSetting setting;
        private PlotModel plotModel;

        public GraphViewModel(IPlcProvider plcProvider, ApplicationSetting setting)
        {
            this.plcProvider = plcProvider;
            this.setting = setting;
            Title = "Graph View";
        }
        
        protected override void Initialize()
        {
            PlotModel = new PlotModel();
            PlotModel.Series.Add(new LineSeries());
            
            var plc = plcProvider.GetHardware();

            plc.CreateNotification<double>(setting.DoubleSignalName)
                .ObserveOnDispatcher()
                .Do(d =>
                {
                    var plotModelSeries = PlotModel.Series[0] as LineSeries;
                    plotModelSeries?.Points.Add(new DataPoint(plotModelSeries.Points.Count, d));
                    if(plotModelSeries?.Points.Count > 100) plotModelSeries?.Points.RemoveAt(0);
                    PlotModel.InvalidatePlot(true);
                })
                .Subscribe()
                .AddDisposableTo(Disposables)
                ;
            
            Logger?.Debug("Initialized Graph view");
        }

        public PlotModel PlotModel
        {
            get => plotModel;
            set
            {
                if(plotModel == value) return;
                raisePropertyChanged();
                plotModel = value;
            }
        }
    }

    internal class DesignGraphViewModel : GraphViewModel
    {
        public DesignGraphViewModel() : base(new DesignPlcProvider(), new ApplicationSetting())
        {
            Init();
        }

        
    }
}