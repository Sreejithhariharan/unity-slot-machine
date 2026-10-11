using UnityEngine;

// Data for one reel symbol. Create one asset per symbol via Assets > Create > Slot > Symbol.
[CreateAssetMenu(fileName = "Symbol", menuName = "Slot/Symbol")]
public class SymbolData : ScriptableObject
{
    public string symbolName;   // Label for the Inspector only
    public Sprite icon;         // Picture shown on the reel

    [Min(0)] public int weight = 10;           // Spawn chance, relative to the total of all weights
    [Min(0)] public float payMultiplier = 2f;  // Win payout as a multiple of the bet

    [Tooltip("Wild symbols substitute for any other symbol when forming a win.")]
    public bool isWild;   // Only the Seven is wild in this game
}