namespace codingTracker._0lcm.Models;

internal class Enums
{
    internal enum MainMenuOption
    {
        NewSession,
        ViewSessions,
        StartTimer,
        Exit
    }

    internal enum LoadSessionOption
    {
        Delete,
        Update,
        Return
    }

    internal enum FilterSessionDateOption
    {
        Today,
        Other,
        Default
    }

    internal enum FilterSessionAscendingOption
    {
        Ascending,
        Descending,
        Default
    }
}