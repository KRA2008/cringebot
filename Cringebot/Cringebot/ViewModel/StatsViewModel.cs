using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;

namespace Cringebot.ViewModel
{
    public class StatsViewModel : FreshBasePageModel
    {
        public ObservableCollection<MemoryStatistic> Statistics { get; }
        public string Title { get; set; }
        public bool IsDataPresent { get; set; }

        public StatsViewModel()
        {
            Statistics = new ObservableCollection<MemoryStatistic>();
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            var memories = (IEnumerable<Memory>) initData;

            Title = "Stats";

            if (memories.Any())
            {
                if (memories.Count() == 1)
                {
                    Title += " (" + memories.First().Description + ")";
                }

                var occurrences = memories.SelectMany(m => m.Occurrences).OrderByDescending(d => d).ToList();
                if (occurrences.Any())
                {
                    IsDataPresent = true;

                    var now = SystemTime.Now();
                    var firstCringe = occurrences.Min();
                    var daysUsingCringebot = (now.Date - firstCringe.Date).Days + 1;

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "total occurrences",
                        Number = occurrences.Count
                    });

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "total days on Cringebot",
                        Number = daysUsingCringebot
                    });

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "average occurrences per day",
                        Number = Math.Round((double)occurrences.Count / daysUsingCringebot, 1)
                    });

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "most occurrences in single day",
                        Number = occurrences.GroupBy(o => o.Date).OrderByDescending(d => d.Count()).First().Count()
                    });

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "total days without occurrence",
                        Number = daysUsingCringebot - occurrences.Select(d => d.Date).Distinct().Count()
                    });

                    var contiguousDaysWithout = new List<int>();
                    for (var ii = 0; ii < occurrences.Count - 1; ii++)
                    {
                        contiguousDaysWithout.Add((occurrences[ii].Date - occurrences[ii + 1].Date).Days);
                    }

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "most days without occurrence",
                        Number = contiguousDaysWithout.Any() ? contiguousDaysWithout.Max() : 0
                    });
                }
            }
        }
    }
}