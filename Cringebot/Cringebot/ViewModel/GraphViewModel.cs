using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;
using Microcharts;
using Xamarin.Forms;
using Entry = Microcharts.Entry;

namespace Cringebot.ViewModel
{
    public class GraphViewModel : FreshBasePageModel
    {
        private IEnumerable<Memory> _memories;

        public string Title { get; set; }
        public Command ViewStatsCommand { get; set; }
        public Command SetLast7DaysCommand { get; set; }
        public Command SetLast30DaysCommand { get; set; }
        public Command SetLast365DaysCommand { get; set; }
        public LineChart Chart { get; set; }

        public GraphViewModel()
        {
            ViewStatsCommand = new Command(async () =>
            {
                await ViewStats();
            });
            SetLast7DaysCommand = new Command(SetLast7Days);
            SetLast30DaysCommand = new Command(SetLast30Days);
            SetLast365DaysCommand = new Command(SetLast365Days);
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            _memories = (IEnumerable<Memory>) initData;
            Title = "Graph";
            if (_memories.Count() == 1)
            {
                Title += " (" + _memories.First().Description + ")";
            }

            if (_memories.Any())
            {
                SetLast7Days();
            }
        }

        private void SetLast7Days()
        {
            GetOccurrencesAndDays(out var occurrences, out var days, 7);
            var occurrencesList = occurrences.ToList();
            var dayEntries = new List<Entry>();
            foreach (var day in days.Reverse())
            {
                var occurrencesOnDay = occurrencesList.Count(o => o.Date == day.Date);
                dayEntries.Add(new Entry(occurrencesOnDay)
                {
                    Label = day.DayOfWeek.ToString(),
                    ValueLabel = occurrencesOnDay.ToString()
                });
            }
            
            Chart = new LineChart
            {
                Entries = dayEntries,
                LineMode = LineMode.Straight,
                LabelTextSize = 25
            };
        }

        private void SetLast30Days()
        {
            GetOccurrencesAndDays(out var occurrences, out var days, 30);
            var occurrencesList = occurrences.ToList();
            var dayEntries = new List<Entry>();
            foreach (var day in days.Reverse())
            {
                var occurrencesOnDay = occurrencesList.Count(o => o.Date == day.Date);
                dayEntries.Add(new Entry(occurrencesOnDay)
                {
                    ValueLabel = occurrencesOnDay.ToString()
                });
            }

            Chart = new LineChart
            {
                Entries = dayEntries,
                LineMode = LineMode.Straight,
                LabelTextSize = 25
            };
        }

        private void SetLast365Days()
        {
            GetOccurrencesAndDays(out var occurrences, out var days, 365);
            var occurrencesList = occurrences.ToList();
            var dayEntries = new List<Entry>();
            foreach (var day in days.Reverse())
            {
                var occurrencesOnDay = occurrencesList.Count(o => o.Date == day.Date);
                dayEntries.Add(new Entry(occurrencesOnDay));
            }

            Chart = new LineChart
            {
                Entries = dayEntries,
                LineMode = LineMode.Straight,
                LabelTextSize = 25
            };
        }

        private void GetOccurrencesAndDays(out IEnumerable<DateTime> occurrences, out IEnumerable<DateTime> days,
            int range)
        {
            var now = SystemTime.Now();
            occurrences = _memories.SelectMany(m => m.Occurrences).Where(o => o > now.AddDays(-1*range)).ToList();
            days = Enumerable.Range(0, range).Select(i => DateTime.Now.Date.AddDays(-i)).ToArray();
        }

        public async Task ViewStats()
        {
            await CoreMethods.PushPageModel<StatsViewModel>(_memories);
        }
    }
}