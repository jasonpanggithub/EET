using Microsoft.AspNetCore.Mvc;
using EET.Service;
using EET.Entity;

namespace EET.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MetaDataController : ControllerBase
    {
        private IMetaDataService _service; 
        public MetaDataController(IMetaDataService service)
        {
            _service = service;
        }

        
        [HttpGet("{movieId}")]
        public ActionResult<MetaDataEntity> Get(int movieId)
        {
            var data = _service.GetMetaDataByMovieId(movieId);
            if (data.Count > 0)
            {
                return new JsonResult(data);
            }
            else
            {
                return NotFound(string.Format("Metadata for movied with id = {0} is not found.", movieId));
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] MetaDataEntity entity)
        {
            // no validation
            if (_service.SaveMetaData(entity))
            {
                return Ok();
            }
            else
            {
                return Problem("something went wrong");
            }

        }

        
    }
}
