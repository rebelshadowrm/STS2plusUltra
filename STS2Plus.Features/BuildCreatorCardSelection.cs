namespace STS2Plus.Features;

internal readonly record struct BuildCreatorCardSelection(int BaseCount, int UpgradedCount)
{
	public int TotalCount => BaseCount + UpgradedCount;

	public bool HasAny => TotalCount > 0;
}
