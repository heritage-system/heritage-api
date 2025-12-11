namespace Cultural_Heritage_System.Common
{
    public enum EventStatus
    {
        DRAFT,
        UPCOMING,
        LIVE,
        CLOSED,
        ARCHIVED
    }

    public enum EventCategory
    {
        GENERAL,
        HERITAGE_TALK,
        FESTIVAL,
        WORKSHOP,
        ONLINE_TOUR
    }

    [Flags]
    public enum EventTag
    {
        NONE,
        FEATURED,
        FREE,
        PREMIUM,
        RECORDED,
        QNA
    }
}
