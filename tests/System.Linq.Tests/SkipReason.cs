namespace System;

public static class SkipReason
{
    public const string FailedByDefault = "Test failed on .NET v9.0.3";

    public const string RefStruct = "There is no compatibility because ZLinq use `ref struct`";

    public const string EnumeratorBehaviorDifference = "ZLinq's Enumerator property is not compatible with LINQ";

    public const string NotCompatibile = "There is no compatibility.";

    public const string ZLinq_Issue0070 = "See: https://github.com/Cysharp/ZLinq/issues/70";

    public const string ZLinq_Issue0081 = "See: https://github.com/Cysharp/ZLinq/issues/81";

    public const string ZLinq_Issue0082 = "See: https://github.com/Cysharp/ZLinq/issues/82";

    public const string ZLinq_Issue0086 = "See: https://github.com/Cysharp/ZLinq/issues/86";

    public const string ZLinq_Issue0087 = "See: https://github.com/Cysharp/ZLinq/issues/87";

    public const string ZLinq_Issue0088 = "See: https://github.com/Cysharp/ZLinq/issues/88";

    public const string ZLinq_Issue0089 = "See: https://github.com/Cysharp/ZLinq/issues/89";

    public const string ZLinq_IssueNNNN = "Min optimization when NaN value contained.";

}
