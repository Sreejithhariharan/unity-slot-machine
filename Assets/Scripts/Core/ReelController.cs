using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Drives one reel. The reel is a vertical strip of symbol cells inside a masked window.
// Spinning scrolls the strip and eases it to a stop so the chosen result lands in the middle row.
public class ReelController : MonoBehaviour
{
    [SerializeField] private RectTransform strip;   // Child of the masked window. Anchor and pivot: top-center.
    [SerializeField] private Image cellPrefab;      // The symbol cell prefab. Used as the template for every cell.
    [SerializeField] private int stripLength = 30;  // Total cells in the strip. Only the last three are the result window.
    [SerializeField] private Vector2 cellSize = new Vector2(150f, 150f);   // Must match the cell prefab's size

    private Image[] cells;
    private float cellHeight;

    // True while this reel is scrolling
    public bool IsSpinning { get; private set; }

    // Builds the strip once at startup. Each cell sits below the one before it, anchored top-center.
    private void Awake()
    {
        cellHeight = cellSize.y;
        cells = new Image[stripLength];

        for (int i = 0; i < stripLength; i++)
        {
            cells[i] = Instantiate(cellPrefab, strip);

            RectTransform rt = cells[i].rectTransform;
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = cellSize;

            // Cells go downward, so each one sits at a negative Y offset from the strip's top
            rt.anchoredPosition = new Vector2(0f, -i * cellHeight);
        }
    }

    // Fills the strip with random symbols so the reels show something before the first spin.
    public void ShowRandomSymbols(RandomOutcomeGenerator rng)
    {
        foreach (var cell in cells)
            cell.sprite = rng.PickSymbol().icon;

        strip.anchoredPosition = Vector2.zero;
    }

    // Scrolls the strip and stops with the result in the middle row.
    // Runs as a coroutine, so the reel animates over several frames.
    public IEnumerator Spin(SymbolData result, RandomOutcomeGenerator rng, float duration)
    {
        IsSpinning = true;

        // Fill the strip with random symbols so the motion looks like a real spin
        foreach (var cell in cells)
            cell.sprite = rng.PickSymbol().icon;

        // With the strip at offset k, the visible rows are cells k (top), k+1 (middle), and k+2 (bottom).
        // The result must land in the middle row, so it goes in cell stripLength - 2.
        int resultIndex = stripLength - 2;
        cells[resultIndex].sprite = result.icon;

        // The strip offset that brings the result into the middle row
        float endOffset = (resultIndex - 1) * cellHeight;

        strip.anchoredPosition = Vector2.zero;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Ease-out cubic: fast at the start, slowing into the stop, so the reel feels weighty
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            strip.anchoredPosition = new Vector2(0f, endOffset * eased);
            yield return null;
        }

        // Snap to the exact final position to remove small floating-point errors
        strip.anchoredPosition = new Vector2(0f, endOffset);
        IsSpinning = false;
    }
}