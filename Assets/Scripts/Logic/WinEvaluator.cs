// Static helper that returns the payout multiplier for a spin. No object needed.
public static class WinEvaluator
{
    // Returns the multiplier, or 0 if the spin is not a win.
    public static float Evaluate(SymbolData[] results)
    {
        SymbolData matchSymbol = null;

        foreach (var result in results)
        {
            if (result.isWild)
                continue;   // Wilds fit any line

            if (matchSymbol == null)
                matchSymbol = result;
            else if (result != matchSymbol)
                return 0f;  // Two different non-wild symbols: no win
        }

        // All wilds: pay the wild's own multiplier
        if (matchSymbol == null)
            return results[0].payMultiplier;

        return matchSymbol.payMultiplier;
    }
}