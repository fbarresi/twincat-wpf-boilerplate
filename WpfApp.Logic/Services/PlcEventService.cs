using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ninject;
using WpfApp.Interfaces.Enums;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Logic.Services
{
    public class PlcEventService : IInitializable, IDisposable, IPlcEventService
    {
        private readonly IPlcProvider plcProvider;
        private readonly IPlcEventLogService plcEventLogService;

        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ErrorSetting setting;
        private readonly BehaviorSubject<PlcEvent> lastEventSubject = new BehaviorSubject<PlcEvent>(null);
        private readonly BehaviorSubject<PlcEvent[]> activeEventSubject = new BehaviorSubject<PlcEvent[]>(new PlcEvent[]{});

        public PlcEventService(ErrorSetting setting, 
            IPlcProvider plcProvider, 
            IPlcEventLogService plcEventLogService)
        {
            this.setting = setting;
            this.plcProvider = plcProvider;
            this.plcEventLogService = plcEventLogService;
        }

        public void Initialize()
        {
            CreateEventSources(setting.ErrorCodeSettings);
        }

        private void CreateEventSources(List<ErrorCodeSetting> errorCodeSettings)
        {
            foreach (var codeSetting in errorCodeSettings)
            {
                var plc = string.IsNullOrEmpty(codeSetting.PlcName) ? plcProvider.GetHardware() : plcProvider.GetHardware(codeSetting.PlcName);
                plc.CreateNotification(codeSetting.ErrorCodeAddress)
                    .DistinctUntilChanged()
                    .Where(value => value != null)
                    .Do(value => HandleNewErrorCode(value, codeSetting))
                    .Subscribe()
                    .AddDisposableTo(disposables)
                    ;
            }

            lastEventSubject.AddDisposableTo(disposables);
            activeEventSubject.AddDisposableTo(disposables);
        }

        private void HandleNewErrorCode(object value, ErrorCodeSetting errorCodeSetting)
        {
            var eventInfo = CreateAndLogEvent(value, errorCodeSetting);

            if (eventInfo.Severity == Severity.Ignored)
                return;
            
            if (eventInfo.Severity == Severity.NoError) // remove error from active and last error
            {
                if(lastEventSubject.Value.Source.Equals(eventInfo.Source)) 
                    lastEventSubject.OnNext(null);
                if (activeEventSubject.Value.Any(e => e.Source.Equals(eventInfo.Source)))
                {
                    var activeEvents = activeEventSubject.Value
                        .ToList();
                    activeEvents.RemoveAll(e => e.Source.Equals(eventInfo.Source));
                    activeEventSubject.OnNext(activeEvents.ToArray());
                }
            }
            else
            {
                lastEventSubject.OnNext(eventInfo); //set last event
                //update active events
                var activeEvents = activeEventSubject.Value
                    .ToList();
                activeEvents.RemoveAll(e => e.Source.Equals(eventInfo.Source));
                activeEvents.Add(eventInfo);
                activeEventSubject.OnNext(activeEvents.ToArray());
            }
        }

        private PlcEvent CreateAndLogEvent([NotNull]object value, ErrorCodeSetting errorCodeSetting)
        {
            var isNotError = value.ToString().Equals(errorCodeSetting.NoErrorValue.ToString());

            var errorCodeDescription = errorCodeSetting.CodeDescriptions.FirstOrDefault(description => description.Value.ToString().Equals(value.ToString()));

            var ignored = errorCodeSetting.IgnoreNotDescribedValues && errorCodeDescription == null;
            
            var plcEvent = new PlcEvent()
            {
                Timestamp = DateTime.Now,
                Source = $"{errorCodeSetting.PlcName}::{errorCodeSetting.ErrorCodeAddress}",
                Value = value,
                ExpectedValue = errorCodeSetting.NoErrorValue,
                Description = errorCodeDescription?.Description,
                LongDescription = errorCodeDescription?.LongDescription
            };

            if (isNotError)
                plcEvent.Severity = Severity.NoError;
            else if (ignored)
                plcEvent.Severity = Severity.Ignored;
            else
                plcEvent.Severity = errorCodeDescription?.Severity ?? Severity.Info;

            plcEventLogService.LogEvent(plcEvent);
            return plcEvent;
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }

        public IObservable<PlcEvent[]> ActiveEvents => activeEventSubject.AsObservable();
        public IObservable<PlcEvent> LatestEvent => lastEventSubject.AsObservable();
    }
}