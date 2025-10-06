namespace Cultural_Heritage_System.Common
{
    public enum SystemLogAction
    {
        // 🔐 Authentication & Security
        USER_LOGIN,
        USER_LOGOUT,
        USER_REGISTER,
        USER_PASSWORD_CHANGED,
        USER_2FA_ENABLED,
        USER_2FA_DISABLED,

        // 👤 User & Profile Management
        USER_PROFILE_UPDATED,
        USER_ROLE_CHANGED,
        USER_STATUS_CHANGED,

        // 📰 Contribution & Review
        CONTRIBUTION_CREATED,
        CONTRIBUTION_UPDATED,
        CONTRIBUTION_DELETED,
        CONTRIBUTION_SUBMITTED,       // Contributor gửi bài
        CONTRIBUTION_ACCEPTED,        // Staff duyệt
        CONTRIBUTION_REJECTED,
        CONTRIBUTION_REVIEWED,

        // 🏛️ Heritage Management
        HERITAGE_CREATED,
        HERITAGE_UPDATED,
        HERITAGE_DELETED,

        // 📦 Favorites, Reviews, Reports
        FAVORITE_ADDED,
        FAVORITE_REMOVED,
        REVIEW_CREATED,
        REVIEW_UPDATED,
        REVIEW_DELETED,
        REPORT_SUBMITTED,
        REPORT_RESOLVED,

        // 💰 Finance / Wallet
        WALLET_CREATED,
        WALLET_TRANSACTION_ADDED,
        WALLET_TRANSACTION_FAILED,
        SUBSCRIPTION_PURCHASED,
        SUBSCRIPTION_CANCELED,

        // 📢 Notifications
        NOTIFICATION_SENT,
        NOTIFICATION_READ,

        // 👮 Staff & Admin Operations
        STAFF_CREATED,
        STAFF_UPDATED,
        STAFF_DELETED,
        STAFF_ASSIGNED_CONTRIBUTION, // Staff được phân công duyệt bài
        ADMIN_LOGIN,
        ADMIN_ACTION
    }
}
