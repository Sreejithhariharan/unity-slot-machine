using System.Collections.Generic;

// Plain C# class that picks symbols by weight. SlotMachine creates it with "new".
public class RandomOutcomeGenerator
{
    private readonly List<SymbolData> symbols;
    private readonly System.Random rng;
    private int totalWeight;

    // seed: optional, gives repeatable results when set
    public RandomOutcomeGenerator(IList<SymbolData> symbolSet, int? seed = null)
    {
        symbols = new List<SymbolData>(symbolSet);
        rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();

        foreach (var symbol in symbols)
            totalWeight += symbol.weight;
    }

    // Returns one symbol. Higher weight means a bigger slice of the roll.
    public SymbolData PickSymbol()
    {
        int roll = rng.Next(totalWeight);

        foreach (var symbol in symbols)
        {
            if (roll < symbol.weight)
                return symbol;
            roll -= symbol.weight;
        }

        return symbols[symbols.Count - 1];   // Only reached if all weights are 0
    }
}