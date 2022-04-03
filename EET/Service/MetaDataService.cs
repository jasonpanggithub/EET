using EET.Entity;
using EET.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace EET.Service
{
    public class MetaDataService : IMetaDataService
    {
        private IConfiguration _configuration;
                
        public MetaDataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<MetaDataEntity> GetMetaDataByMovieId(int id)
        {
            string database = _configuration.GetValue<string>("database", "metadata.csv");
            var metaData = File.ReadAllLines(database, Encoding.Unicode) // read the csv file
                 .Skip(1)        // skip the head line
                 .Select(row => row.Split(','))    // split colume values
                 .ToArray()
                 .Where(row => Int32.Parse(row[1]) == id)   // select by movie id
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
                .GroupBy(metaData => metaData.Language) // group by language
                .Select(g => g.OrderByDescending(g => g.Id ).FirstOrDefault())  // order by id desc and get the first one
                .OrderBy(o=>o.Language)
                .ToList();

           return metaData;
        }

        
        public bool SaveMetaData(MetaDataEntity entity)
        {
            //no validation, assume the file exists
            //don't generate new id, just save as 999
            try
            {
                string newLine = $"999,{entity.MovieId},{entity.Title},{entity.Language},{entity.Duration},{entity.ReleaseYear}"+Environment.NewLine;
                string database = _configuration.GetValue<string>("database", "metadata.csv");
                File.AppendAllText(database, newLine);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }
    }

}

