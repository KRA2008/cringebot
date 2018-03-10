using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;

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
        public ObservableCollection<ChartDataPoint> Data { get; set; }

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
            SetDays(7);
        }

        private void SetLast30Days()
        {
            SetDays(30);
        }

        private void SetLast52Weeks()
        {
            GetOccurrencesAndDays(out var occurrences, out var days, 365);
            var weekStackedOccurrences = occurrences.Select(o => o.Date.AddDays(-(int) o.DayOfWeek)).ToList();
            var weekStarters = days.Where(d => d.DayOfWeek == 0);
            var weekEntries = new List<ChartDataPoint>();
            foreach (var day in weekStarters.Reverse())
            {
                var occurrencesInWeek = weekStackedOccurrences.Count(o => o.Date == day.Date);
                var endOfWeek = day.AddDays(6);
                weekEntries.Add(new ChartDataPoint(
                    string.Format("{0}/{1}-{2}/{3}", day.Month, day.Day, endOfWeek.Month, endOfWeek.Day),
                    occurrencesInWeek));
            }

            Data = new ObservableCollection<ChartDataPoint>(weekEntries);
        }

        private void SetTimeOfDay()
        {
            var hours = Enumerable.Range(0, 24);
            var occurrencesByHour = _memories.SelectMany(m => m.Occurrences).Select(o => o.TimeOfDay.Hours).ToList();
            var hourlyEntries = new List<ChartDataPoint>();
            foreach (var hour in hours)
            {
                var occurrencesInHour = occurrencesByHour.Count(o => o == hour);
                hourlyEntries.Add(new ChartDataPoint(string.Format("{0}:00-{0}:59", hour),
                    occurrencesInHour));
            }

            Data = new ObservableCollection<ChartDataPoint>(hourlyEntries);
        }

        private void SetDayOfWeek()
        {
            var daysOfWeek = Enumerable.Range(0, 7);
            var occurrencesByDayOfWeek = _memories.SelectMany(m => m.Occurrences).Select(o => o.DayOfWeek).ToList();
            var dailyOccurrences = new List<ChartDataPoint>();
            foreach (var day in daysOfWeek)
            {
                var occurrencesOnDay = occurrencesByDayOfWeek.Count(o => (int)o == day);
                dailyOccurrences.Add(new ChartDataPoint(string.Format(((DayOfWeek)day).ToString(), day),
                    occurrencesOnDay));
            }

            Data = new ObservableCollection<ChartDataPoint>(dailyOccurrences);
        }

        private void SetDays(int numberOfDays)
        {
            GetOccurrencesAndDays(out var occurrences, out var days, numberOfDays);
            var occurrencesList = occurrences.ToList();
            var dayEntries = new List<ChartDataPoint>();
            foreach (var day in days.Reverse())
            {
                var occurrencesOnDay = occurrencesList.Count(o => o.Date == day.Date);
                dayEntries.Add(new ChartDataPoint(day.Month + "/" + day.Day, occurrencesOnDay));
            }

            Data = new ObservableCollection<ChartDataPoint>(dayEntries);
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