using Cultural_Heritage_System.Dtos.Request.Category;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Quiz;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Category;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Quiz;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {

        private readonly IQuizService quizService;

        public QuizController(IQuizService quizService)
        {
            this.quizService = quizService;
        }

       
        [HttpGet("search_quiz")]
     
        public async Task<ApiResponse<PageResponse<QuizListResponse>>> GetListQuiz([FromQuery] QuizListRequest request)

        {
            return new ApiResponse<PageResponse<QuizListResponse>>
            {
                code = 200,
                result = await quizService.GetListQuiz(request)
            };
        }

        [HttpGet("get_quiz_detail")]
        public async Task<ApiResponse<QuizDetailResponse>> GetQuizDetail(long id)
        {
            var result = await quizService.GetQuizDetail(id);
            return new ApiResponse<QuizDetailResponse>(
                code: 200,
                message: "Get quiz details successfully",
                result: result
            );
        }

        [HttpGet("get_quiz_overview")]
        public async Task<ApiResponse<QuizOverviewResponse>> GetQuizOverview(long id)
        {
            var result = await quizService.GetQuizOverview(id);
            return new ApiResponse<QuizOverviewResponse>(
                code: 200,
                message: "Get quiz overview successfully",
                result: result
            );
        }

        [HttpPost("save_quiz_result")]
        public async Task<ApiResponse<bool>> SaveQuizResult([FromBody] SaveQuizResultRequest request)
        {
            return new ApiResponse<bool>(
                code: 201,
                message: "Save quiz result successfully",
                result: await quizService.SaveQuizResult(request)
            );

        }

        [HttpPost("unlock_quiz")]
        public async Task<ApiResponse<bool>> UnlockContribution(int id)
        {
            return new ApiResponse<bool>(
                code: 201,
                message: "Unlock quiz successfully",
                result: await quizService.UnlockQuiz(id)
            );
        }

        [HttpPost("create_quiz")]
        //[Authorize(Roles ="ADMIN")]
        public async Task<ApiResponse<bool>> CreateQuiz([FromBody] QuizCreationRequest request)
        {
         
            return new ApiResponse<bool>(
                code: 201,
                message: "Quiz created successfully",
                result: await quizService.CreateQuiz(request)
            );
        }



        [HttpPut("update_quiz")]
        //[Authorize(Roles ="ADMIN")]
        public async Task<ApiResponse<bool>> UpdateQuiz([FromBody] QuizUpdateRequest request)
        {
           
            return new ApiResponse<bool>(
                code: 200,
                message: "Quiz updated successfully",
                result: await quizService.UpdateQuiz(request)
            );
        }


        [HttpDelete("delete_quiz")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<long?>> DeleteQuiz([FromQuery] long id)
        {
            
            return new ApiResponse<long?>(
                code: 200,
                message: "Delete quiz successfully",
                result: await quizService.DeleteQuiz(id)
            );
        }

        [HttpPost("create_quiz_question")]
        //[Authorize(Roles ="ADMIN")]
        public async Task<ApiResponse<bool>> CreateQuizQuestion([FromBody] QuizQuestionCreationRequest request)
        {

            return new ApiResponse<bool>(
                code: 201,
                message: "Quiz question created successfully",
                result: await quizService.CreateQuizQuestion(request)
            );
        }



        [HttpPut("update_quiz_question")]
        //[Authorize(Roles ="ADMIN")]
        public async Task<ApiResponse<bool>> UpdateQuizQuestion([FromBody] QuizQuestionUpdateRequest request)
        {

            return new ApiResponse<bool>(
                code: 200,
                message: "Quiz question updated successfully",
                result: await quizService.UpdateQuizQuestion(request)
            );
        }


        [HttpDelete("delete_quiz_question")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<long?>> DeleteQuizQuestion([FromQuery] long id)
        {

            return new ApiResponse<long?>(
                code: 200,
                message: "Delete question quiz successfully",
                result: await quizService.DeleteQuizQuestion(id)
            );
        }

        [HttpGet("admin/detail/{quizId}")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<QuizDetailAdminResponse>> GetQuizDetailAdmin([FromRoute] long quizId)
        {
            var result = await quizService.GetQuizDetailAdmin(quizId);

            return new ApiResponse<QuizDetailAdminResponse>
            {
                code = 200,
                message = "Get quiz detail for admin successfully",
                result = result
            };
        }

    }
}
