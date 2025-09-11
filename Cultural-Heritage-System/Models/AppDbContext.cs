using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

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
        public DbSet<QuizCategory> QuizCategories { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizRank> QuizRanks { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ContributionPurchase> ContributionPurchases { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<RevenueShare> RevenueShares { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<HeritageMedia> HeritageMedias { get; set; }
        public DbSet<HeritageOccurrence> HeritageOccurrences { get; set; }
        public DbSet<ReviewLike> ReviewLikes { get; set; }
        public DbSet<ReviewReport> ReviewReports { get; set; }
        public DbSet<ReviewMedia> ReviewMedias { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }
        public override int SaveChanges()
        {
            ApplyUnsignedFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyUnsignedFields();
            return base.SaveChangesAsync(cancellationToken);
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

            modelBuilder.Entity<Contribution>()
                .HasOne(c => c.Reviewer)
                .WithMany(u => u.ReviewedContributions)
                .HasForeignKey(c => c.ReviewedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<QuizResult>()
                .HasOne(q => q.User)
                .WithMany(u => u.QuizResults)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RevenueShare>()
                .HasOne(rs => rs.Contribution)
                .WithMany()
                .HasForeignKey(rs => rs.ContributionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RevenueShare>()
                .HasOne(rs => rs.Contributor)
                .WithMany()
                .HasForeignKey(rs => rs.ContributorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RevenueShare>()
                .HasOne(rs => rs.ContributionPurchase)
                .WithMany()
                .HasForeignKey(rs => rs.PurchaseId)
                .OnDelete(DeleteBehavior.Restrict);

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

        }
    }
}