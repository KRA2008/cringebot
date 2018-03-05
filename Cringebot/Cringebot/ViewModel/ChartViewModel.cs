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
    public class ChartViewModel : FreshBasePageModel
    {
        private IEnumerable<Memory> _memories;

        public string Title { get; set; }
        public bool IsDataPresent { get; set; }
        public Command ViewStatsCommand { get; set; }
        public Command SetLast7DaysCommand { get; set; }
        public Command SetLast30DaysCommand { get; set; }
        public Command SetLast52WeeksCommand { get; set; }
        public Command SetTimeOfDayCommand { get; set; }
        public Command SetDayOfWeekCommand { get; set; }
        public LineChart Chart { get; set; }

        public ChartViewModel()
        {
            ViewStatsCommand = new Command(async () =>
            {
                await ViewStats();
            });
            SetLast7DaysCommand = new Command(() => {});
            SetLast30DaysCommand = new Command(() => { });
            SetLast52WeeksCommand = new Command(() => { });
            SetTimeOfDayCommand = new Command(() => { });
            SetDayOfWeekCommand = new Command(() => { });
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            _memories = (IEnumerable<Memory>) initData;
            Title = "Chart";
            if (_memories.Count() == 1)
            {
                Title += " (" + _memories.First().Description + ")";
            }

            if (_memories.Any() && _memories.Any(m => m.Occurrences.Any()))
            {
                IsDataPresent = true;
                SetLast7Days();
                SetLast7DaysCommand = new Command(SetLast7Days);
                SetLast30DaysCommand = new Command(SetLast30Days);
                SetLast52WeeksCommand = new Command(SetLast52Weeks);
                SetTimeOfDayCommand = new Command(SetTimeOfDay);
                SetDayOfWeekCommand = new Command(SetDayOfWeek);
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
                    ValueLabel = occurrencesOnDay.ToString(),
                    Label = day.Day.ToString()
                });
            }
            
            Chart = new LineChart
            {
                Entries = dayEntries,
                LineMode = LineMode.Straight,
                LabelTextSize = 35
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

        private void SetLast52Weeks()
        {
            GetOccurrencesAndDays(out var occurrences, out var days, 365);
            var weekStackedOccurrences = occurrences.Select(o => o.Date.AddDays(-(int) o.DayOfWeek)).ToList();
            var weekStarters = days.Where(d => d.DayOfWeek == 0);
            var weekEntries = new List<Entry>();
            foreach (var day in weekStarters.Reverse())
            {
                var occurrencesInWeek = weekStackedOccurrences.Count(o => o.Date == day.Date);
                weekEntries.Add(new Entry(occurrencesInWeek)
                {
                    ValueLabel = occurrencesInWeek.ToString()
                });
            }

            Chart = new LineChart
            {
                Entries = weekEntries,
                LineMode = LineMode.Straight,
                LabelTextSize = 15
            };
        }

        private void SetTimeOfDay()
        {
            var hours = Enumerable.Range(0, 23);
            var occurrencesByHour = _memories.SelectMany(m => m.Occurrences).Select(o => o.TimeOfDay.Hours).ToList();
            var hourlyEntries = new List<Entry>();
            foreach (var hour in hours)
            {
                var occurrencesInHour = occurrencesByHour.Count(o => o == hour);
                hourlyEntries.Add(new Entry(occurrencesInHour)
                {
                    ValueLabel = occurrencesInHour.ToString(),
                    Label = hour.ToString()
                });
            }

            Chart = new LineChart
            {
                Entries = hourlyEntries,
                LineMode = LineMode.Straight,
                LabelTextSize = 15
            };
        }

        private void SetDayOfWeek()
        {

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