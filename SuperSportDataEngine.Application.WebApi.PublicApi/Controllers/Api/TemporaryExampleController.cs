namespace SuperSportDataEngine.Application.WebApi.PublicApi.Controllers.Api
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    // TODO: [Davide] Temporary example reference code for team (delete this later).
    public class TemporaryExampleController : ApiController
    {
        private readonly ITemporaryExampleService _temporaryExampleService;

        public TemporaryExampleController(ITemporaryExampleService temporaryExampleService)
        {
            _temporaryExampleService = temporaryExampleService;
        }

        // GET: api/TemporaryExample
        public IEnumerable<string> Get()
        {
            //return new string[] { "value1", "value2" };
            return new string[] { _temporaryExampleService.HelloMessage() };
        }

        // GET: api/TemporaryExample/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TemporaryExample
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/TemporaryExample/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/TemporaryExample/5
        public void Delete(int id)
        {
        }
    }
}