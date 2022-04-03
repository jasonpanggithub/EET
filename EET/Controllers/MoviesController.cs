using Microsoft.AspNetCore.Mvc;
using EET.Entity;
using EET.Model;
using EET.Service;

namespace EET.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        IMovieStatistic _service;
        public MoviesController(IMovieStatistic service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("stats")]
        public ActionResult<MovieStatisticServicecs> Get()
        {
            var data = _service.GetMovieStatistic();

            if (data.Count > 0)
            {
                return new JsonResult(data);
            }
            else
            {
                return Problem("something went wrong");
            }
        }
    }
}
