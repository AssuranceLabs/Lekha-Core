namespace Makuri.Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public class CommonHelper
    {
        ///// <summary>
        ///// Generate random digit code
        ///// </summary>
        ///// <param name="length">Length</param>
        ///// <returns>Result string</returns>
        //public static string GenerateRandomDigitCode(int length)
        //{
        //    using var random = new SecureRandomNumberGenerator();
        //    var str = string.Empty;
        //    for (var i = 0; i < length; i++)
        //        str = string.Concat(str, random.Next(10).ToString());
        //    return str;
        //}

        /// <summary>
        /// Generate apims transaction id
        /// </summary>
        /// <param name="apiCode">Api code</param>
        /// <returns>Transaction id</returns>
        public static string GenerateApimsTransactionId(string apiCode) => $"{apiCode}-{DateTime.Now:yyyyMMddHHmmssfff}";
    }
}
