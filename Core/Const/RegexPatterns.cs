namespace Core.Const
{
    public static class RegexPatterns
    {
        public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
        public const string url = "^http(s)?://([\\w-]+.)+[\\w-]+(/[\\w- ./?%&=])?$";
        public const string Username = "^[a-zA-Z0-9-._@+]*$";
        public const string CharactersOnly_Eng = "^[a-zA-Z-_ ]*$";
        public const string DenySpecialCharacters = "^[^<>!#%$]*$";
        public const string MobileNumber = "^05[0,1,2,3,4,5,6,7,8,9][0-9]{7}$";
        public const string EnglishOrNumbers = "^[a-zA-Z0-9]*$";
        public const string NumbersOnly = "^[0-9]*$";
    }
}