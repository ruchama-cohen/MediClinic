using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("[Controller]")]
    [ApiController]

    public class ExceptionsControler
    {
        private readonly ILogger _logger;

        public ExceptionsControler(ILogger<ExceptionsControler> logger)
        {
            _logger = logger;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
    }
}
