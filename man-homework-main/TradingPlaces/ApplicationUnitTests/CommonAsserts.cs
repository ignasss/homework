using Models.Common;
using NUnit.Framework;

namespace ApplicationUnitTests
{
    public static class CommonAsserts
    {
        public static void AssertFailure(Result result)
        {
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.AreNotEqual(Error.None.Code, result.Error.Code);
            Assert.AreNotEqual(Error.None.Message, result.Error.Message);
        }

        public static void AssertFailure<T>(Result<T> result)
        {
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.AreNotEqual(Error.None.Code, result.Error.Code);
            Assert.AreNotEqual(Error.None.Message, result.Error.Message);
        }

        public static void AssertSuccess(Result result)
        {
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(Error.None.Code, result.Error.Code);
            Assert.AreEqual(Error.None.Message, result.Error.Message);
        }

        public static void AssertSuccess<T>(Result<T> result)
        {
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(Error.None.Code, result.Error.Code);
            Assert.AreEqual(Error.None.Message, result.Error.Message);
        }
    }
}