using System.Text.RegularExpressions;
using Apps.AmazonTranslate.Constants;

namespace Apps.AmazonTranslate.Extensions;

public static class StringExtensions
{
    public static string GetBucket(this string str)
        => Regex.Match(str, RegexPatterns.GetBucketFromUri).Groups[1].Value;
}