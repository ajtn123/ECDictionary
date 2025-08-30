namespace ECDictionary;

public static class Config
{
    public static string DbPath => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\tools\\ecdict.db";
    public static string TableName => "stardict";
}
