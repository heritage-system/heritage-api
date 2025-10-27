using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Quiz;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Quiz;
using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class QuizService : IQuizService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IQuizRepository quizRepository;
        private readonly IQuizQuestionRepository quizQuestionRepository;
        private readonly IQuizResultRepository quizResultRepository;
        private readonly IUserRepository userRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IMapper mapper;

        private readonly ILogger<QuizService> logger;

        public QuizService(IQuizRepository quizRepository, IUserRepository userRepository, ILogger<QuizService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, 
            ISubscriptionRepository subscriptionRepository, IQuizResultRepository quizResultRepository, IQuizQuestionRepository quizQuestionRepository)
        {
            this.quizRepository = quizRepository;
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.subscriptionRepository = subscriptionRepository;
            this.quizResultRepository = quizResultRepository;
            this.quizQuestionRepository = quizQuestionRepository;
        }

        private static readonly List<QuizQuestion> QuestionBank = new() { new QuizQuestion { Question = "Hội Gióng đền Phù Đổng được UNESCO công nhận là Di sản văn hóa phi vật thể đại diện của nhân loại vào năm nào?", OptionA = "2008", OptionB = "2009", OptionC = "2010", OptionD = "2011", CorrectOption = "C", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.EASY }, new QuizQuestion { Question = "Hội Gióng đền Sóc được tổ chức hằng năm tại đâu?", OptionA = "Sóc Sơn, Hà Nội", OptionB = "Gia Lâm, Hà Nội", OptionC = "Phù Đổng, Bắc Ninh", OptionD = "Phù Linh, Bắc Giang", CorrectOption = "A", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.EASY }, new QuizQuestion { Question = "Nhân vật trung tâm được tôn vinh trong Hội Gióng là ai?", OptionA = "Thánh Gióng", OptionB = "Thánh Tản Viên", OptionC = "Thánh Trần Hưng Đạo", OptionD = "Thánh Linh Lang", CorrectOption = "A", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.EASY }, new QuizQuestion { Question = "Lễ hội Yên Thế tưởng niệm cuộc khởi nghĩa nào trong lịch sử Việt Nam?", OptionA = "Khởi nghĩa Lam Sơn", OptionB = "Khởi nghĩa Hương Khê", OptionC = "Khởi nghĩa Yên Thế", OptionD = "Khởi nghĩa Ba Đình", CorrectOption = "C", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.MEDIUM }, new QuizQuestion { Question = "Thủ lĩnh của cuộc khởi nghĩa Yên Thế là ai?", OptionA = "Nguyễn Huệ", OptionB = "Phan Đình Phùng", OptionC = "Hoàng Hoa Thám (Đề Thám)", OptionD = "Trương Định", CorrectOption = "C", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.MEDIUM }, new QuizQuestion { Question = "Lễ hội Thổ Hà được tổ chức để tưởng nhớ vị vua nào?", OptionA = "Lý Nhân Tông", OptionB = "Lý Thánh Tông", OptionC = "Trần Nhân Tông", OptionD = "Lý Công Uẩn", CorrectOption = "B", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.MEDIUM }, new QuizQuestion { Question = "Lễ hội Thổ Hà có phong tục đặc trưng nào sau đây?", OptionA = "Rước nước từ sông Cầu", OptionB = "Chọi trâu", OptionC = "Đua thuyền", OptionD = "Đấu vật và ném còn", CorrectOption = "A", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.MEDIUM }, new QuizQuestion { Question = "Lễ hội Nhảy lửa là nghi lễ của dân tộc nào?", OptionA = "Người Dao", OptionB = "Người Pà Thẻn", OptionC = "Người Tày", OptionD = "Người H'Mông", CorrectOption = "B", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.HARD }, new QuizQuestion { Question = "Ý nghĩa của nghi lễ Nhảy lửa của người Pà Thẻn là gì?", OptionA = "Thể hiện sức mạnh, niềm tin vào thần linh và xua đuổi tà ma", OptionB = "Cầu mùa màng bội thu", OptionC = "Tưởng nhớ tổ tiên", OptionD = "Chào mừng năm mới", CorrectOption = "A", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.HARD }, new QuizQuestion { Question = "Lễ hội Nhảy lửa thường diễn ra vào thời điểm nào trong năm?", OptionA = "Đầu năm mới", OptionB = "Sau vụ thu hoạch, cuối năm âm lịch", OptionC = "Giữa mùa hè", OptionD = "Trong dịp Tết Trung thu", CorrectOption = "B", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.HARD } };

        public async Task<List<QuizQuestionResponse>> GenerateQuestionSet(int numberOfQuestions)
        {
            var query =  quizQuestionRepository.GetQuizQuestionQueryable();

            if (query == null || !query.Any())
                return new List<QuizQuestionResponse>();

            var random = new Random();
        
            int easyCount = (int)Math.Round(numberOfQuestions * 0.4);
            int mediumCount = (int)Math.Round(numberOfQuestions * 0.4);
            int hardCount = numberOfQuestions - easyCount - mediumCount; // phần còn lại

            var easy = query
                .Where(q => q.QuizLevel == QuizLevel.EASY)
                .OrderBy(q => Guid.NewGuid()) 
                .Take(easyCount)
                .ToList();

            var medium = query
                .Where(q => q.QuizLevel == QuizLevel.MEDIUM)
                .OrderBy(q => Guid.NewGuid())
                .Take(mediumCount)
                .ToList();

            var hard = query
                .Where(q => q.QuizLevel == QuizLevel.HARD)
                .OrderBy(q => Guid.NewGuid())
                .Take(hardCount)
                .ToList();


            var selected = easy.Concat(medium).Concat(hard).ToList();

            for (int i = 0; i < selected.Count; i++)
                selected[i].Id = i + 1;


            return mapper.Map<List<QuizQuestionResponse>>(selected);
        }

        public async Task<QuizDetailResponse> GetQuizDetail(long quizId)
        {
            var existingQuiz = await quizRepository.GetQuizById(quizId);

            if (existingQuiz == null)
                throw new AppException(ErrorCode.QUIZ_NOT_FOUND);

            var response = mapper.Map<QuizDetailResponse>(existingQuiz);

            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            var userId = int.Parse(accountIdClaim);
            // Nếu free thì trả luôn          
            if (existingQuiz.PremiumType == PremiumType.FREE)
            {
                var resultFree = await quizResultRepository.GetQuizResult(quizId, userId);
                if (resultFree != null)
                {
                    response.NumberOfClear = resultFree.NumberOfClear;
                }
                return response;
            }

            // Premium → check used        
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                // chưa login → chỉ preview
                response.Questions = new List<QuizQuestionResponse>();
                return response;
            }

            // Lấy subscription active
            var activeSub = await subscriptionRepository.GetActiveSubscription(userId);

            if (activeSub == null)
            {
                // không có sub → chỉ preview
                response.Questions = new List<QuizQuestionResponse>();
                return response;
            }
      
            var result = await quizResultRepository.GetQuizResult(quizId, userId);
            if (result != null)
            {
                response.NumberOfClear = result.NumberOfClear;
            }
  
            return response;
        }

        public async Task<PageResponse<QuizListResponse>> GetListQuiz(QuizListRequest request)
        {
            try
            {
                var query = quizRepository.GetQuizQueryable();


                // Keyword search
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    var searchTerm = request.Keyword.Trim().ToLower();
                    var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                    query = query.Where(h =>
                        // Contribution title
                        h.Title.ToLower().Contains(searchTerm) ||
                        h.TitleUnsigned.Contains(unsignedTerm)               
                    );
                }


                switch (request.SortBy)
                {
                    case SortBy.IDASC:
                        query = query.OrderBy(h => h.Id);
                        break;
                    case SortBy.IDDESC:
                        query = query.OrderByDescending(h => h.Id);
                        break;
                    case SortBy.NAMEASC:
                        query = query.OrderBy(h => h.Title);
                        break;
                    case SortBy.NAMEDESC:
                        query = query.OrderByDescending(h => h.Title);
                        break;
                    default:
                        query = query.OrderByDescending(h => h.CreatedAt);
                        break;
                }


                var dtoQuery = query.ProjectTo<QuizListResponse>(mapper.ConfigurationProvider);

                // Pagination
                var response = await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);

                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (!string.IsNullOrEmpty(accountIdClaim) && response.Items != null)
                {
                    var userId = int.Parse(accountIdClaim);

                    foreach (var item in response.Items)
                    {
                        var result = await quizResultRepository.GetQuizResult(item.Id, userId);
                        if (result != null)
                        {
                            item.NumberOfClear = result.NumberOfClear;
                        }
                    }
                }


                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching quizz");
                throw;
            }
        }

        public async Task<bool> SaveQuizResult(SaveQuizResultRequest request)
        {
            var existingQuiz = await quizRepository.GetQuizById(request.QuizId);

            if (existingQuiz == null)
                throw new AppException(ErrorCode.QUIZ_NOT_FOUND);
          
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var userId = int.Parse(accountIdClaim);

            var existingQuizResult = await quizResultRepository.GetQuizResult(request.QuizId, userId);

            if (existingQuizResult == null)
            {
                var result = new QuizResult
                {
                    UserId = userId,
                    QuizId = request.QuizId,
                    NumberOfClear = request.NumberOfClear,
                };

                await quizResultRepository.AddAsync(result);
                return true;
            }
            else
            {
                if(existingQuizResult.NumberOfClear < request.NumberOfClear)
                {
                    existingQuizResult.UpdatedAt = DateTime.UtcNow;
                    existingQuizResult.NumberOfClear = request.NumberOfClear;
                    await quizResultRepository.UpdateAsync(existingQuizResult);
                }             
                return true;
            }       
        }

        public async Task<bool> CreateQuiz(QuizCreationRequest request)
        {
            try
            {
                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                    throw new AppException(ErrorCode.UNAUTHORIZED);

                var staff = await userRepository.FindUserById(int.Parse(accountIdClaim));
             
                var quiz = mapper.Map<Quiz>(request);
                quiz.CreatedBy = accountIdClaim;
           
                // Add quiz vào context
                await quizRepository.AddAsync(quiz);              
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuiz(QuizUpdateRequest request)
        {
            try
            {
                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                    throw new AppException(ErrorCode.UNAUTHORIZED);

                var quiz = await quizRepository.GetQuizById(request.Id);
                if (quiz == null)
                    throw new AppException(ErrorCode.QUIZ_NOT_FOUND);

                // --- Update basic fields ---
                quiz.Title = request.Title;
                //quiz.Description = request.Description;
                quiz.BannerUrl = request.BannerUrl;

                quiz.UpdatedAt = DateTime.Now;

                var oldQuestions = quiz.Questions.ToList();
                foreach (var oldQues in oldQuestions)
                {
                     await quizQuestionRepository.DeleteAsync(oldQues);
                }
                foreach (var quizReq in request.Questions)
                {
                    var quizQuestion = mapper.Map<QuizQuestion>(quizReq);
                    quizQuestion.QuizId = quiz.Id;
                    quiz.Questions.Add(quizQuestion);
                }

                await quizRepository.UpdateAsync(quiz);
              
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<long?> DeleteQuiz(long id)
        {
            var quiz = await quizRepository.GetQuizById(id);
            if (quiz == null)
                return null;

            var relatedResults = quiz.Results.ToList();

            foreach (var r in relatedResults)
            {
                await quizResultRepository.DeleteAsync(r);
            }


            await quizRepository.DeleteAsync(quiz);
            return id;
        }

        public async Task<bool> CreateQuizQuestion(QuizQuestionCreationRequest request)
        {
            try
            {
                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                    throw new AppException(ErrorCode.UNAUTHORIZED);

                var staff = await userRepository.FindUserById(int.Parse(accountIdClaim));

                var quizQuestion = mapper.Map<QuizQuestion>(request);
                quizQuestion.CreatedBy = accountIdClaim;

                // Add quiz vào context
                await quizQuestionRepository.AddAsync(quizQuestion);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuizQuestion(QuizQuestionUpdateRequest request)
        {
            try
            {
                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                    throw new AppException(ErrorCode.UNAUTHORIZED);

                if(request.QuizId != null)
                {
                    var quiz = await quizRepository.GetQuizById((long)request.QuizId);
                    if (quiz == null)
                        throw new AppException(ErrorCode.QUIZ_NOT_FOUND);
                }

                var quizQuestion = await quizQuestionRepository.GetQuizQuestionById(request.Id);

                if (quizQuestion == null)
                {
                    throw new AppException(ErrorCode.QUIZ_QUESTION_NOT_FOUND);
                }

                mapper.Map(request, quizQuestion);
                quizQuestion.UpdatedAt = DateTime.Now;
           
                await quizQuestionRepository.UpdateAsync(quizQuestion);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<long?> DeleteQuizQuestion(long id)
        {
            var quizQuestion = await quizQuestionRepository.GetQuizQuestionById(id);
            if (quizQuestion == null)
                return null;

            await quizQuestionRepository.DeleteAsync(quizQuestion);
            return id;
        }
    }
}
