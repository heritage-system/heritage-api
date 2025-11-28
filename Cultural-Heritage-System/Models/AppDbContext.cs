using Cultural_Heritage_System.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cultural_Heritage_System.Models
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContext;
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContext) : base(options)
        {
            _httpContext = httpContext;
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<ExportLog> ExportLogs { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Heritage> Heritages { get; set; }
        public DbSet<HeritageLocation> HeritageLocations { get; set; }
        public DbSet<HeritageTag> HeritageTags { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Quiz> Quiz { get; set; } 
        public DbSet<QuizQuestion> QuizQuestions { get; set; }    
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }       
        public DbSet<Contributor> Contributors { get; set; }
        //public DbSet<RevenueShare> RevenueShares { get; set; }        
        public DbSet<HeritageMedia> HeritageMedias { get; set; }
        public DbSet<HeritageOccurrence> HeritageOccurrences { get; set; }
        public DbSet<ReviewLike> ReviewLikes { get; set; }
        public DbSet<ReviewReport> ReviewReports { get; set; }
        public DbSet<ReviewMedia> ReviewMedias { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<ContributionReviewLike> ContributionReviewLike { get; set; }
        public override int SaveChanges()
        {
            ApplyUnsignedFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyUnsignedFields();

            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .ToList(); // ép ToList() để snapshot collection

            var logs = new List<SystemLog>();

            foreach (var entry in entries)
            {
                logs.Add(new SystemLog
                {
                    UserId = TryGetUserId(),
                    Action = MapAction(entry),
                    Details = $"{entry.Entity.GetType().Name} {entry.State}",
                    IpAddress = _httpContext.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                });
            }

            if (logs.Any())
            {
                SystemLogs.AddRange(logs);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }



        private int? TryGetUserId()
        {
            var claim = _httpContext.HttpContext?.User?.FindFirst("userId")?.Value;
            return claim != null ? int.Parse(claim) : null;
        }

        private SystemLogAction MapAction(EntityEntry entry)
        {
            return entry.Entity switch
            {
                // 👤 User
                User => entry.State switch
                {
                    EntityState.Added => SystemLogAction.USER_REGISTER,
                    EntityState.Modified => SystemLogAction.USER_PROFILE_UPDATED,
                    EntityState.Deleted => SystemLogAction.USER_STATUS_CHANGED, // hoặc USER_DELETED nếu anh muốn
                    _ => SystemLogAction.ADMIN_ACTION
                },

                // 👮 Staff
                Staff => entry.State switch
                {
                    EntityState.Added => SystemLogAction.STAFF_CREATED,
                    EntityState.Modified => SystemLogAction.STAFF_UPDATED,
                    EntityState.Deleted => SystemLogAction.STAFF_DELETED,
                    _ => SystemLogAction.ADMIN_ACTION
                },

                // 📰 Contribution
                Contribution => entry.State switch
                {
                    EntityState.Added => SystemLogAction.CONTRIBUTION_CREATED,
                    EntityState.Modified => SystemLogAction.CONTRIBUTION_UPDATED,
                    EntityState.Deleted => SystemLogAction.CONTRIBUTION_DELETED,
                    _ => SystemLogAction.ADMIN_ACTION
                },

                // 🏛️ Heritage
                Heritage => entry.State switch
                {
                    EntityState.Added => SystemLogAction.HERITAGE_CREATED,
                    EntityState.Modified => SystemLogAction.HERITAGE_UPDATED,
                    EntityState.Deleted => SystemLogAction.HERITAGE_DELETED,
                    _ => SystemLogAction.ADMIN_ACTION
                },

                // Các entity khác (Review, Favorite, Report…)
                Review => entry.State switch
                {
                    EntityState.Added => SystemLogAction.REVIEW_CREATED,
                    EntityState.Modified => SystemLogAction.REVIEW_UPDATED,
                    EntityState.Deleted => SystemLogAction.REVIEW_DELETED,
                    _ => SystemLogAction.ADMIN_ACTION
                },

                Favorite => entry.State switch
                {
                    EntityState.Added => SystemLogAction.FAVORITE_ADDED,
                    EntityState.Deleted => SystemLogAction.FAVORITE_REMOVED,
                    _ => SystemLogAction.ADMIN_ACTION
                },

                Report => entry.State switch
                {
                    EntityState.Added => SystemLogAction.REPORT_SUBMITTED,
                    EntityState.Modified => SystemLogAction.REPORT_RESOLVED,
                    _ => SystemLogAction.ADMIN_ACTION
                },

             
                PaymentTransaction => entry.State switch
                {
                    EntityState.Added => SystemLogAction.WALLET_TRANSACTION_ADDED,
                    EntityState.Deleted => SystemLogAction.WALLET_TRANSACTION_FAILED,
                    _ => SystemLogAction.ADMIN_ACTION
                },

                Subscription => entry.State switch
                {
                    EntityState.Added => SystemLogAction.SUBSCRIPTION_PURCHASED,
                    EntityState.Deleted => SystemLogAction.SUBSCRIPTION_CANCELED,
                    _ => SystemLogAction.ADMIN_ACTION
                },

                Notification => entry.State switch
                {
                    EntityState.Added => SystemLogAction.NOTIFICATION_SENT,
                    EntityState.Modified => SystemLogAction.NOTIFICATION_READ,
                    _ => SystemLogAction.ADMIN_ACTION
                },

                _ => SystemLogAction.ADMIN_ACTION
            };
        }


        private void ApplyUnsignedFields()
        {
            foreach (var entry in ChangeTracker.Entries<IUnsignedEntity>())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    entry.Entity.GenerateUnsignedFields();
                }
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HeritageTag>()
                 .HasKey(ht => new { ht.HeritageId, ht.TagId });

            modelBuilder.Entity<HeritageTag>()
                .HasOne(ht => ht.Heritage)
                .WithMany(h => h.HeritageTags)
                .HasForeignKey(ht => ht.HeritageId);

            modelBuilder.Entity<HeritageTag>()
                .HasOne(ht => ht.Tag)
                .WithMany(t => t.HeritageTags)
                .HasForeignKey(ht => ht.TagId);


            modelBuilder.Entity<HeritageLocation>()
                .HasKey(hl => new { hl.HeritageId, hl.LocationId });

            modelBuilder.Entity<HeritageLocation>()
                .HasOne(hl => hl.Heritage)
                .WithMany(h => h.HeritageLocations)
                .HasForeignKey(hl => hl.HeritageId);

            modelBuilder.Entity<HeritageLocation>()
                .HasOne(hl => hl.Location)
                .WithMany(l => l.HeritageLocations)
                .HasForeignKey(hl => hl.LocationId);


            modelBuilder.Entity<Heritage>()
                .HasOne(h => h.Category)
                .WithMany(c => c.Heritages)
                .HasForeignKey(h => h.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents heritage deletion;

            modelBuilder.Entity<HeritageMedia>()
               .HasOne(hm => hm.Heritage)
               .WithMany(h => h.Media)
               .HasForeignKey(hm => hm.HeritageId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HeritageOccurrence>()
               .HasOne(hm => hm.Heritage)
               .WithMany(h => h.HeritageOccurrences)
               .HasForeignKey(hm => hm.HeritageId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contribution>()
                .HasOne(c => c.Contributor)
                .WithMany(u => u.Contributions)
                .HasForeignKey(c => c.ContributorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Questions)
                .WithOne(qq => qq.Quiz)
                .HasForeignKey(qq => qq.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Results)
                .WithOne(r => r.Quiz)
                .HasForeignKey(r => r.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuizResult>()
                .HasOne(q => q.User)
                .WithMany(u => u.QuizResults)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<RevenueShare>()
            //    .HasOne(rs => rs.Contribution)
            //    .WithMany()
            //    .HasForeignKey(rs => rs.ContributionId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<RevenueShare>()
            //    .HasOne(rs => rs.Contributor)
            //    .WithMany()
            //    .HasForeignKey(rs => rs.ContributorId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<RevenueShare>()
            //    .HasOne(rs => rs.ContributionUnlock)
            //    .WithMany()
            //    .HasForeignKey(rs => rs.UnlockId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Review → Heritage
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Heritage)
                .WithMany(h => h.Reviews)
                .HasForeignKey(r => r.HeritageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review → ReviewMedias
            modelBuilder.Entity<Review>()
                .HasMany(r => r.ReviewMedias)
                .WithOne(rm => rm.Review)
                .HasForeignKey(rm => rm.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
            // Self-reference: Parent Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.ParentReview)
                .WithMany(r => r.Replies)
                .HasForeignKey(r => r.ParentReviewId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ReviewLike>()
        .HasKey(rl => new { rl.ReviewId, rl.UserId });

            modelBuilder.Entity<ReviewLike>()
                .HasOne(rl => rl.Review)
                .WithMany(r => r.Likes)
                .HasForeignKey(rl => rl.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReviewLike>()
                .HasOne(rl => rl.User)
                .WithMany(u => u.ReviewLikes)
                .HasForeignKey(rl => rl.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ReviewReport>()
                    .HasOne(rp => rp.Review)
                    .WithMany(r => r.Reports)
                    .HasForeignKey(rp => rp.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReviewReport>()
                    .HasOne(rp => rp.User)
                    .WithMany(u => u.ReviewReports)
                    .HasForeignKey(rp => rp.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Heritage)
                .WithMany()
                .HasForeignKey(f => f.HeritageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContributionHeritageTag>()
                .HasOne(rt => rt.Heritage)
                .WithMany(u => u.ContributionHeritageTags)
                .HasForeignKey(rt => rt.HeritageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContributionHeritageTag>()
                .HasOne(rt => rt.Contribution)
                .WithMany(u => u.ContributionHeritageTags)
                .HasForeignKey(rt => rt.ContributionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContributionAccessLog>()
                .HasOne(l => l.User)
                .WithMany(u => u.ContributionAccessLogs)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContributionAccessLog>()
                .HasOne(l => l.Subscription)
                .WithMany()
                .HasForeignKey(l => l.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContributionReview>()
                .HasOne(r => r.User)
                .WithMany(u => u.ContributionReviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

           
            modelBuilder.Entity<ContributionReview>()
                .HasOne(r => r.Contribution)
                .WithMany(h => h.Reviews)
                .HasForeignKey(r => r.ContributionId)
                .OnDelete(DeleteBehavior.Cascade);
           
          
            modelBuilder.Entity<ContributionReview>()
                .HasOne(r => r.ParentReview)
                .WithMany(r => r.Replies)
                .HasForeignKey(r => r.ParentReviewId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ContributionReviewLike>()
                .HasKey(rl => new { rl.ContributionReviewId, rl.UserId });

            modelBuilder.Entity<ContributionReviewLike>()
                .HasOne(rl => rl.ContributionReview)
                .WithMany(r => r.Likes)
                .HasForeignKey(rl => rl.ContributionReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContributionReviewLike>()
                .HasOne(rl => rl.User)
                .WithMany(u => u.ContributionReviewLike)
                .HasForeignKey(rl => rl.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ContributionReport>()
                .HasOne(l => l.User)
                .WithMany(u => u.ContributionReports)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContributionReport>()
                .HasOne(l => l.Contribution)
                .WithMany(u => u.ContributionReports)
                .HasForeignKey(l => l.ContributionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Staff>()
               .HasOne(s => s.User)
               .WithOne(u => u.Staff) 
               .HasForeignKey<Staff>(s => s.UserId)
               .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ContributionAcceptance>()
               .HasOne(ca => ca.Staff)
               .WithMany(s => s.ContributionAcceptances)
               .HasForeignKey(ca => ca.StaffId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContributionAcceptance>()
                .HasOne(ca => ca.Contribution)
                .WithMany(c => c.ContributionAcceptances)
                .HasForeignKey(ca => ca.ContributionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Heritage>()
               .HasMany(q => q.PanoramaTours)
               .WithOne(qq => qq.Heritage)
               .HasForeignKey(qq => qq.HeritageId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PanoramaTour>()
              .HasMany(q => q.Scenes)
              .WithOne(qq => qq.PanoramaTour)
              .HasForeignKey(qq => qq.PanoramaTourId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReportReply>()
                .HasOne(r => r.Staff)
                .WithMany(s => s.ReportReplies)
                .HasForeignKey(r => r.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasMany(q => q.PaymentTransactions)
               .WithOne(qq => qq.User)
               .HasForeignKey(qq => qq.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        
            modelBuilder.Entity<PremiumPackageBenefit>()
                .HasOne(p => p.Package)
                .WithMany(p => p.PackageBenefits)
                .HasForeignKey(p => p.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PremiumPackageBenefit>()
                .HasOne(p => p.Benefit)
                .WithMany()
                .HasForeignKey(p => p.BenefitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
               .HasMany(q => q.UsageRecords)
               .WithOne(qq => qq.Subscription)
               .HasForeignKey(qq => qq.SubscriptionId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}