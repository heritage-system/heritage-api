using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class QuizService : IQuizService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IQuizRepository quizRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        private readonly ILogger<QuizService> logger;

        public QuizService(IQuizRepository quizRepository, IUserRepository userRepository, ILogger<QuizService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.quizRepository = quizRepository;
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        private static readonly List<QuizQuestion> QuestionBank = new() { new QuizQuestion { Question = "Hội Gióng đền Phù Đổng được UNESCO công nhận là Di sản văn hóa phi vật thể đại diện của nhân loại vào năm nào?", OptionA = "2008", OptionB = "2009", OptionC = "2010", OptionD = "2011", CorrectOption = "C", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.EASY }, new QuizQuestion { Question = "Hội Gióng đền Sóc được tổ chức hằng năm tại đâu?", OptionA = "Sóc Sơn, Hà Nội", OptionB = "Gia Lâm, Hà Nội", OptionC = "Phù Đổng, Bắc Ninh", OptionD = "Phù Linh, Bắc Giang", CorrectOption = "A", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.EASY }, new QuizQuestion { Question = "Nhân vật trung tâm được tôn vinh trong Hội Gióng là ai?", OptionA = "Thánh Gióng", OptionB = "Thánh Tản Viên", OptionC = "Thánh Trần Hưng Đạo", OptionD = "Thánh Linh Lang", CorrectOption = "A", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.EASY }, new QuizQuestion { Question = "Lễ hội Yên Thế tưởng niệm cuộc khởi nghĩa nào trong lịch sử Việt Nam?", OptionA = "Khởi nghĩa Lam Sơn", OptionB = "Khởi nghĩa Hương Khê", OptionC = "Khởi nghĩa Yên Thế", OptionD = "Khởi nghĩa Ba Đình", CorrectOption = "C", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.MEDIUM }, new QuizQuestion { Question = "Thủ lĩnh của cuộc khởi nghĩa Yên Thế là ai?", OptionA = "Nguyễn Huệ", OptionB = "Phan Đình Phùng", OptionC = "Hoàng Hoa Thám (Đề Thám)", OptionD = "Trương Định", CorrectOption = "C", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.MEDIUM }, new QuizQuestion { Question = "Lễ hội Thổ Hà được tổ chức để tưởng nhớ vị vua nào?", OptionA = "Lý Nhân Tông", OptionB = "Lý Thánh Tông", OptionC = "Trần Nhân Tông", OptionD = "Lý Công Uẩn", CorrectOption = "B", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.MEDIUM }, new QuizQuestion { Question = "Lễ hội Thổ Hà có phong tục đặc trưng nào sau đây?", OptionA = "Rước nước từ sông Cầu", OptionB = "Chọi trâu", OptionC = "Đua thuyền", OptionD = "Đấu vật và ném còn", CorrectOption = "A", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.MEDIUM }, new QuizQuestion { Question = "Lễ hội Nhảy lửa là nghi lễ của dân tộc nào?", OptionA = "Người Dao", OptionB = "Người Pà Thẻn", OptionC = "Người Tày", OptionD = "Người H'Mông", CorrectOption = "B", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.HARD }, new QuizQuestion { Question = "Ý nghĩa của nghi lễ Nhảy lửa của người Pà Thẻn là gì?", OptionA = "Thể hiện sức mạnh, niềm tin vào thần linh và xua đuổi tà ma", OptionB = "Cầu mùa màng bội thu", OptionC = "Tưởng nhớ tổ tiên", OptionD = "Chào mừng năm mới", CorrectOption = "A", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.HARD }, new QuizQuestion { Question = "Lễ hội Nhảy lửa thường diễn ra vào thời điểm nào trong năm?", OptionA = "Đầu năm mới", OptionB = "Sau vụ thu hoạch, cuối năm âm lịch", OptionC = "Giữa mùa hè", OptionD = "Trong dịp Tết Trung thu", CorrectOption = "B", QuizCategory = QuizCategory.RITUAL, QuizLevel = QuizLevel.HARD } };

        public async Task<List<QuizQuestionResponse>> GenerateQuestionSet(int numberOfQuestions)
        {
            if (QuestionBank == null || !QuestionBank.Any())
                return new List<QuizQuestionResponse>();

            var random = new Random();
        
            int easyCount = (int)Math.Round(numberOfQuestions * 0.4);
            int mediumCount = (int)Math.Round(numberOfQuestions * 0.4);
            int hardCount = numberOfQuestions - easyCount - mediumCount; // phần còn lại

            // ✅ Lấy ngẫu nhiên theo từng nhóm độ khó
            var easy = QuestionBank
                .Where(q => q.QuizLevel == QuizLevel.EASY)
                .OrderBy(_ => random.Next())
                .Take(easyCount)
                .ToList();

            var medium = QuestionBank
                .Where(q => q.QuizLevel == QuizLevel.MEDIUM)
                .OrderBy(_ => random.Next())
                .Take(mediumCount)
                .ToList();

            var hard = QuestionBank
                .Where(q => q.QuizLevel == QuizLevel.HARD)
                .OrderBy(_ => random.Next())
                .Take(hardCount)
                .ToList();

        
            var selected = easy.Concat(medium).Concat(hard).ToList();

            for (int i = 0; i < selected.Count; i++)
                selected[i].Id = i + 1;

            return mapper.Map<List<QuizQuestionResponse>>(selected);
        }

    }
}
