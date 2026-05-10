using System.Collections.Generic;

namespace STS2Plus.Features;

internal sealed record RouteSuggestion(string ProfileId, int Score, object StartPoint, IReadOnlyList<object> Steps);
