namespace Core.Const
{
    public static class Errors
    {
        public const string FileSize = "The file size exceeds the allowed limit (2 MB).";
        public const string RequiredField = "{0} is a required field.";
        public const string MaxLength = "The number of characters should not exceed 200.";
        public const string MaxMinLength = "{0} must contain between {1} and {2} characters.";
        public const string Duplicated = "{0} is already registered!";
        public const string NotAllowedExtension = "Only .png, .jpg, .jpeg files are allowed!";
        public const string NotAllowFutureDates = "The date cannot be in the future!";
        public const string OnlyEnglishLetters = "Only English letters are allowed.";
        public const string OnlyNumbers = "Please enter numbers only.";
        public const string AgeLimit = "{0} must be between {21} and {60}!.";
        public const string invalidUrl = "Enter a valid URL.";
        public const string Invalid = "Enter a valid {0}.";
    }
}