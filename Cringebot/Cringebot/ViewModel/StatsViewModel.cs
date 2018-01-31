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

        public StatsViewModel()
        {
            Statistics = new ObservableCollection<MemoryStatistic>();
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            var memories = (IEnumerable<Memory>) initData;

            if (memories.Any())
            {
                var occurrences = memories.SelectMany(m => m.Occurrences).OrderByDescending(d => d).ToList();
                if (occurrences.Any())
                {
                    var now = SystemTime.Now();
                    var firstCringe = occurrences.Min();
                    var daysUsingCringebot = (now.Date - firstCringe.Date).Days + 1;

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "total cringes",
                        Number = occurrences.Count
                    });

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "total days using Cringebot",
                        Number = daysUsingCringebot
                    });

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "average cringes per day",
                        Number = Math.Round((double)occurrences.Count / daysUsingCringebot, 1)
                    });

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "most cringes in single day",
                        Number = memories.OrderByDescending(m => m.Occurrences.Count).First().Occurrences.Count
                    });

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "total days without cringe",
                        Number = daysUsingCringebot - occurrences.Select(d => d.Date).Distinct().Count()
                    });

                    var contiguousDaysWithout = new List<int>();
                    for (var ii = 0; ii < occurrences.Count - 1; ii++)
                    {
                        contiguousDaysWithout.Add((occurrences[ii].Date - occurrences[ii + 1].Date).Days);
                    }

                    Statistics.Add(new MemoryStatistic
                    {
                        Description = "most days without cringe",
                        Number = contiguousDaysWithout.Max()
                    });
                }
            }
        }
    }
}