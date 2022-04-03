using EET.Entity;
using System.Collections.Generic;


namespace EET.Service
{
    public interface IMetaDataService
    {
        public List<MetaDataEntity> GetMetaDataByMovieId(int id);
        public bool SaveMetaData(MetaDataEntity entity);
        
    }
}
