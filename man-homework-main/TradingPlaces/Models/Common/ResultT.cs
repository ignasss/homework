namespace Models.Common
{
    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        public TValue Value => _value;

        public Result(TValue value, bool isSuccess, Error error)
            : base(isSuccess, error)
            => _value = value;
    }
}