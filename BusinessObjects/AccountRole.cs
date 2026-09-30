namespace BusinessObjects;

public static class AccountRole
{
    public const int Admin = 0;
    public const int Staff = 1;
    public const int Lecturer = 2;

    public static string GetName(int? role) => role switch
    {
        Admin => "Admin",
        Staff => "Staff",
        Lecturer => "Lecturer",
        _ => "Unknown"
    };
}
