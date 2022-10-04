namespace Models.Common
{
    public sealed class Error
    {
        public string Code { get; }
        
        public string Message { get; }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static Error None => new Error(string.Empty, string.Empty);
    }
}