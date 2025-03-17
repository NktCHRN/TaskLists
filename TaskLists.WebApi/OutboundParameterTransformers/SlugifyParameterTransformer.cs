using System.Text.RegularExpressions;

namespace TaskLists.WebApi.OutboundParameterTransformers;

public partial class SlugifyParameterTransformer : IOutboundParameterTransformer, IParameterPolicy
{
    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex GetRegexPattern();

    public string? TransformOutbound(object? value)
    {
        // Slugify value
        return value is null ? null : GetRegexPattern().Replace(value.ToString()!, "$1-$2").ToLower();
    }
}
