using ConsoleApp2.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp2.Class.WeatherEvents;

namespace ConsoleApp2
{
    class Program
    {

        delegate string TaskN(List<WeatherEvents> weatherEvents);
        static void Main()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string path = @"D:\VisualStudio\ConsoleApp1\ConsoleApp2\Dataset\WeatherEvents_Jan2016-Dec2020.csv";
            using (StreamReader sr = new StreamReader(path))
            {
                List<string> data = sr.ReadToEnd().Trim().Split('\n').Skip(1).ToList();
                List<WeatherEvents> weatherEvents = new List<WeatherEvents>();
                foreach (string line in data)
                {
                    weatherEvents.Add(new WeatherEvents(WeatherEvents.Parse(line).ToList()));
                }
                /*                string? line = sr.ReadLine();
                                for (int i = 0; i < 100; i++)
                                {
                                    line = sr.ReadLine();
                                    weatherEvents.Add(new WeatherEvents(WeatherEvents.Parse(line).ToList()));
                                }*/
                List<TaskN> tasks = new List<TaskN> { Task0, Task1, Task2, Task3, Task4, Task5, Task6 };
                TimeSpan timer1 = new TimeSpan(0, 0, 0), timer2 = new TimeSpan(0, 0, 0), timer3 = new TimeSpan(0, 0, 0);
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                foreach (var task in tasks)
                {
                    string ans = task(weatherEvents);
                    Console.WriteLine(ans);
                }
                stopWatch.Stop();
                timer1 = stopWatch.Elapsed;
                string elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timer1.Hours, timer1.Minutes, timer1.Seconds,
                timer1.Milliseconds / 10);


                stopWatch.Start();
                Parallel.ForEach<TaskN>(
                    tasks,
                    task => Console.WriteLine(task(weatherEvents))
                    );
                stopWatch.Stop();
                timer2 = stopWatch.Elapsed;
                string elapsedTime2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timer2.Hours, timer2.Minutes, timer2.Seconds,
                timer2.Milliseconds / 10);



                Task[] taski = new Task[7];
                stopWatch.Start();
                for (var i = 0; i < taski.Length; i++)
                {
                    int j = i;
                    taski[i] = new Task(() =>
                    {
                        Console.WriteLine(tasks[j](weatherEvents));
                    });

                    taski[i].Start();
                }
                Task.WaitAll(taski); // ожидаем завершения всех задач
                stopWatch.Stop();
                timer3 = stopWatch.Elapsed;
                string elapsedTime3 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timer3.Hours, timer3.Minutes, timer3.Seconds,
                timer3.Milliseconds / 10);
                Console.WriteLine("RunTime1 " + elapsedTime1);
                Console.WriteLine("RunTime2 " + elapsedTime2);
                Console.WriteLine("RunTime3 " + elapsedTime3);
                Dictionary<string, TimeSpan> dick = new Dictionary<string, TimeSpan>()
                {
                    { "RunTime1 ", timer1 },
                    { "RunTime2 ", timer2},
                    { "RunTime3 ", timer3 }
                }
                .OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                Console.WriteLine($"The fastest is {dick.First().Key} with {dick.First().Value}");
                Console.WriteLine($"{dick.Skip(1).First().Key} is {(double)(dick.Skip(1).First().Value.Seconds - dick.First().Value.Seconds) / (double)dick.First().Value.Seconds * 100:F1}% slower than {dick.First().Key} with {dick.Skip(1).First().Value}");
                Console.WriteLine($"{dick.Skip(2).First().Key} is {((double)(dick.Skip(2).First().Value.Seconds - dick.First().Value.Seconds) / (double)dick.First().Value.Seconds) * 100:F1}% slower than {dick.First().Key} with {dick.Skip(2).First().Value}");
            }
        }
        static string Task0(List<WeatherEvents> weatherEvents)
        {
            int eventsCount = weatherEvents
                .Where(x => x.StartTime.Year == 2018)
                .Count();
            return $"Task0: \n{eventsCount} weather events in 2018.";

        }
        static string Task1(List<WeatherEvents> weatherEvents)
        {
            int citiesCount = weatherEvents
                .GroupBy(x => x.City)
                .Count();
            int statesCount = weatherEvents
                .GroupBy(x => x.State)
                .Count();
            return "Task1:\n" + citiesCount + " cities." + statesCount + " states.";

        }
        static string Task2(List<WeatherEvents> weatherEvents)
        {
            var rainyCities = weatherEvents
                .Where(x => x.Type == Types.Rain & x.StartTime.Year == 2019)
                .GroupBy(x => x.City)
                .Select(g => new
                {
                    city = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .Take(3);

            string ans = "Task2:\n";
            foreach (var city in rainyCities)
            {
                ans += city.city + " - " + city.count + " rains\n";
            }
            return ans;
        }
        static string Task3(List<WeatherEvents> weatherEvents)
        {
            var longestRains = weatherEvents
                .Where(g => g.Type == Types.Snow)
                .Select(g => new
                {
                    year = g.StartTime.Year,
                    duration = g.EndTime.Subtract(g.StartTime),
                    startTime = g.StartTime,
                    endTime = g.EndTime,
                    city = g.City
                })
                .GroupBy(g => g.year)
                .Select(g => g.OrderByDescending(x => x.duration).First());
            string ans = "Task3:\n";
            foreach (var rain in longestRains)
            {
                ans += $"{rain.year}: from {rain.startTime} to {rain.endTime} in {rain.city}\n";
            }
            return ans;
        }
        static string Task4(List<WeatherEvents> weatherEvents)
        {
            var countOfFirstShortEventsInStates = weatherEvents
                .Where(x => x.StartTime.Year == 2019)
                .GroupBy(g => g.State)
                .Select(g => new
                {
                    state = g.Key,
                    count = g
                            .OrderBy(x => x.StartTime)
                            .TakeWhile(x => x.EndTime.Subtract(x.StartTime) <= new TimeSpan(2, 0, 0))
                            .Count()
                });
            string ans = "Task4:\n";
            foreach (var stateEvent in countOfFirstShortEventsInStates)
            {
                ans += $"{stateEvent.state}: {stateEvent.count} weather events.\n";
            }
            return ans;
        }
        static string Task5(List<WeatherEvents> weatherEvents)
        {
            var citiesInStates = weatherEvents
                .Where(x => x.Severity == Severities.Severe & x.StartTime.Year == 2017)
                .Select(x => new
                {
                    state = x.State,
                    city = x.City,
                    duration = x.EndTime.Subtract(x.StartTime)
                })
                .GroupBy(x => x.state)
                .Select(x => new
                {
                    state = x.Key,
                    city = x
                            .GroupBy(g => g.city)
                            .Select(h => new
                            {
                                city = h.Key,
                                sumDuration = h
                                        .Select(j => j.duration)
                                        .Sum(span => span.Hours)
                            })
                            .OrderByDescending(x => x.sumDuration)
                            .First()
                })
                .Select(x => new
                {
                    state = x.state,
                    city = x.city.city,
                    maxSumDuration = x.city.sumDuration
                })
                .OrderByDescending(x => x.maxSumDuration);
            string ans = "Task5:\n";
            foreach (var cities in citiesInStates)
            {
                ans += $"{cities.state}: {cities.city} with {cities.maxSumDuration} hours of severe weather events.\n";
            }
            return ans;
        }
        static string Task6(List<WeatherEvents> weatherEvents)
        {
            var eventsInfo = weatherEvents
                .GroupBy(x => x.StartTime.Year)
                .Select(x => new
                {
                    Year = x.Key,
                    EventInfo = x
                            .GroupBy(g => g.Type)
                            .Select(h => new
                            {
                                type = h.Key,
                                count = h.Count(),
                                sum = h.Select(k => k.EndTime - k.StartTime).Sum(span => span.Hours)
                            })
                            .OrderBy(y => y.count)
                })
                .Select(x => new
                {
                    year = x.Year,
                    rarely = x.EventInfo.First().type,
                    countR = x.EventInfo.First().count,
                    averageR = (double)x.EventInfo.First().sum / x.EventInfo.First().count,
                    often = x.EventInfo.Last().type,
                    countO = x.EventInfo.Last().count,
                    averageO = (double)x.EventInfo.Last().sum / x.EventInfo.Last().count
                });
            string ans = "Task6:\n";
            foreach (var eventInfo in eventsInfo)
            {
                ans += $"{eventInfo.year}: " +
                    $"rare events - {eventInfo.rarely} {eventInfo.countR} times in average {eventInfo.averageR:F2} hours, " +
                    $"often events - {eventInfo.often} {eventInfo.countO} times in average {eventInfo.averageO:F2} hours.\n";
            }
            return ans;
        }
    }
}