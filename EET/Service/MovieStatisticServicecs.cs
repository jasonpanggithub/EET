using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EET.Model;
using EET.Entity;
using System;
using System.Text;

namespace EET.Service
{
    public class MovieStatisticServicecs : IMovieStatistic
    {
        IConfiguration _configuration;
        public MovieStatisticServicecs(IConfiguration configuraiton)
        {
            _configuration = configuraiton;
        }

        public List<MovieStatistic> GetMovieStatistic()
        {
            string stats = _configuration.GetValue<string>("stats", "stats.csv");
            var watchDurations = File.ReadAllLines(stats)
                .ToList()
                .Skip(1)
                .Select(row => row.Split(','))
                .ToArray()
                .Select(row => new WatchDurationEntity
                {
                    MovieId = Int32.Parse(row[0]),
                    WatchDurationMs = Int32.Parse(row[1])
                }).ToList()
                .GroupBy(g => g.MovieId)
                .Select(g => new MovieStatistic
                { 
                    MovieId = g.Key,
                    Watches = g.Count(),
                    AverageWatchDurations = (int)g.Average<WatchDurationEntity>(d=>d.WatchDurationMs)
                })
                .ToList();

            string database = _configuration.GetValue<string>("metadata", "metadata.csv");
            var movieInfo = File.ReadAllLines(database, Encoding.Unicode) 
                 .Skip(1)        
                 .Select(row => row.Split(','))    
                 .ToArray()
                 .Where(row => row[2] != string.Empty &&
                               row[3] != string.Empty &&
                               row[4] != string.Empty &&
                               row[5] != String.Empty)  // every field must has value
                .Select(row => new MetaDataEntity       // populate record
                {
                    Id = Int32.Parse(row[0]),
                    MovieId = Int32.Parse(row[1]),
                    Title = Convert.ToString(row[2]),
                    Language = Convert.ToString(row[3]),
                    Duration = Convert.ToString(row[4]),
                    ReleaseYear = Convert.ToString(row[5])
                })
                .GroupBy(metaData => metaData.MovieId) 
                .Select(g => g.OrderByDescending(g => g.Id).FirstOrDefault())  // order by id desc and get the first one
                .OrderBy(o => o.ReleaseYear)
                .ToList();

            var data = watchDurations
                    .Join(movieInfo,
                        w => w.MovieId,
                        m => m.MovieId,
                        (w, m) => new MovieStatistic
                        {
                            MovieId = w.MovieId,
                            Title = m.Title,
                            AverageWatchDurations = w.AverageWatchDurations,
                            Watches = w.Watches,
                            ReleaseYear = m.ReleaseYear
                        })
                    .OrderByDescending(d=>d.Watches)
                    .ThenBy(d=>d.ReleaseYear)
                    .ToList();

            return data;
        }
    }
}
