using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests.Commands;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.WebApi.Models;
using Rebus.Bus;

namespace Pubquiz.WebApi.Controllers
{
    [Route("api/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBus _bus;
        private readonly QuizrSettings _quizrSettings;
        private readonly ILoggerFactory _loggerFactory;

        public QuizController(IUnitOfWork unitOfWork, IBus bus, QuizrSettings quizrSettings,
            ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork;
            _bus = bus;
            _quizrSettings = quizrSettings;
            _loggerFactory = loggerFactory;
        }
        
        [HttpGet]
        [Authorize(AuthPolicy.Admin)]
        public async Task<ActionResult<List<QuizRef>>> GetQuizzes()
        {
            var query = new GetQuizzesQuery(_unitOfWork) {ActorId = User.GetId()};
            var result = await query.Execute();
            return Ok(result);
        }

        /// <summary>
        /// Upload a zip file containing an excel file and associated media files.
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("uploadzippedexcelquiz")]
        [Authorize(AuthPolicy.QuizMaster)]
        [RequestSizeLimit(2147483648)]
        public async Task<IActionResult> UploadZippedExcelQuiz(IFormFile formFile)
        {
            await using var fileStream = formFile.OpenReadStream();
            var command =
                new ImportZippedExcelQuizCommand(_unitOfWork, _bus, fileStream, formFile.FileName, _quizrSettings,
                    _loggerFactory);
            command.ActorId = User.GetId();
            var result = await command.Execute();

            return Ok(new ImportZippedExcelQuizResponse
                {Code = ResultCode.Ok, Message = "Quiz successfully imported.", QuizRefs = result.QuizRefs});
        }
    }
}