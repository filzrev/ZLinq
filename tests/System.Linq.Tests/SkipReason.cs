namespace System;

public static class SkipReason
{
    public const string FailedByDefault = "Test failed on .NET v9.0.3";

    public const string RefStruct = "There is no compatibility because ZLinq use `ref struct`";

    public const string EnumeratorBehaviorDifference = "ZLinq's Enumerator property is not compatible with LINQ";

    public const string NotCompatibile = "There is no compatibility.";

    /// <summary>
    /// Used for tests that has no comptiblity by design.
    /// </summary>
    public const string NotCompatibile_Verified = "There is no compatibility. And it's not expected be supported by ZLinq.";

    public const string RequiresTooMuchResource = "Test require memory/times to execute.";

    /// <summary>
    /// Dispose method is not called on some cases.
    /// </summary>
    public const string Issue0081 = "See: https://github.com/Cysharp/ZLinq/issues/81";

    /// <summary>
    /// IEnumerable<T>::Take() optimization is missing
    /// </summary>
    public const string Issue0090 = "See: https://github.com/Cysharp/ZLinq/issues/90";

    /// <summary>
    /// IEnumerable<T>::Min() optimization is missing
    /// </summary>
    public const string Issue0092 = "See: https://github.com/Cysharp/ZLinq/issues/92";

    /// <summary>
    /// Concat operation don't check length overflow
    /// </summary>
    public const string Issue0093 = "See: https://github.com/Cysharp/ZLinq/issues/93";

    // Dummy code.
    public const string ZLinq_IssueNNNN = "See: https://github.com/Cysharp/ZLinq/issues/{placeholder}";
}
