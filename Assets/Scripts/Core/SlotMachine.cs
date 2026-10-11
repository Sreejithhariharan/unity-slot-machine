using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Main controller. Manages the balance, bet, spin flow, win and jackpot results, and the UI text.
public class SlotMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ReelController[] reels;     // The three reel objects
    [SerializeField] private SymbolData[] symbols;       // All symbol assets that can appear on the reels
    [SerializeField] private Button spinButton;          // The lever button
    [SerializeField] private Button betUpButton;
    [SerializeField] private Button betDownButton;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text messageText;       // Shows win, loss, and status messages

    [Header("Economy")]
    [SerializeField] private int startingBalance = 1000;
    [SerializeField] private int[] betLevels = { 10, 20, 50, 100 };   // Bet options the player can cycle through

    [Header("Timing")]
    [SerializeField] private float baseSpinDuration = 1.2f;   // Time the first reel spins for, in seconds
    [SerializeField] private float reelStagger = 0.3f;        // Each reel stops this much later than the previous one

    [Header("RNG")]
    [Tooltip("0 = random every run. Any other value gives repeatable results for testing.")]
    [SerializeField] private int seed = 0;

    [Header("Lever")]
    [SerializeField] private LeverAnimator lever;   // Optional. The game still runs without it.

    [Header("Jackpot")]
    [SerializeField] private SymbolData jackpotSymbol;          // The Seven asset. Three of these is a jackpot.
    [SerializeField] private int jackpotBonusMultiplier = 5;    // Multiplies the normal win payout on a jackpot

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;         // Optional. Sounds are skipped if it's empty.

    private RandomOutcomeGenerator rng;
    private int balance;
    private int betIndex;
    private int reelsRunning;   // Counts how many reels are still spinning

    // The bet currently selected, read from the bet levels list
    private int CurrentBet => betLevels[betIndex];

    // True only when all three reels show the jackpot symbol
    private bool IsJackpot(SymbolData[] results)
    {
        if (jackpotSymbol == null) return false;

        foreach (var result in results)
        {
            if (result != jackpotSymbol)
                return false;
        }
        return true;
    }

    // Creates the RNG, fills the reels with symbols, and connects the buttons.
    private void Start()
    {
        // A seed of 0 means a random sequence every run. Any other value makes results repeatable.
        rng = new RandomOutcomeGenerator(symbols, seed == 0 ? (int?)null : seed);

        foreach (var reel in reels)
            reel.ShowRandomSymbols(rng);

        balance = startingBalance;
        betIndex = 0;

        // Buttons are wired in code rather than the Inspector, so each click runs once
        spinButton.onClick.AddListener(() => StartCoroutine(SpinRoutine()));
        betUpButton.onClick.AddListener(() => ChangeBet(1));
        betDownButton.onClick.AddListener(() => ChangeBet(-1));

        RefreshUI("Press the lever to play");
    }

    // Moves the bet up or down one level, staying within the list.
    private void ChangeBet(int direction)
    {
        betIndex = Mathf.Clamp(betIndex + direction, 0, betLevels.Length - 1);
        audioManager?.PlayButton();
        RefreshUI("Bet changed");
    }

    // Runs one full spin: charge the bet, pull the lever, spin the reels, then pay out.
    private IEnumerator SpinRoutine()
    {
        if (balance < CurrentBet)
        {
            RefreshUI("Not enough balance!");
            yield break;
        }

        // Lock the controls so the player can't start a second spin mid-way
        SetControlsInteractable(false);

        balance -= CurrentBet;
        RefreshUI("Spinning...");

        audioManager?.PlayLever();
        if (lever != null)
            yield return StartCoroutine(lever.Pull());

        // Results are chosen before any animation starts, so the visuals always match the outcome
        SymbolData[] results = new SymbolData[reels.Length];
        for (int i = 0; i < reels.Length; i++)
            results[i] = rng.PickSymbol();

        audioManager?.StartSpinLoop();

        // Start every reel now, each with a longer duration so they stop one after another
        reelsRunning = reels.Length;
        for (int i = 0; i < reels.Length; i++)
        {
            float duration = baseSpinDuration + i * reelStagger;
            StartCoroutine(RunReel(reels[i], results[i], duration));
        }

        // Wait until every reel has reported that it stopped
        yield return new WaitUntil(() => reelsRunning == 0);

        audioManager?.StopSpinLoop();

        ResolveSpin(results);

        if (lever != null)
            yield return StartCoroutine(lever.Release());

        SetControlsInteractable(true);
    }

    // Spins one reel and plays its stop sound when it lands.
    private IEnumerator RunReel(ReelController reel, SymbolData result, float duration)
    {
        yield return reel.Spin(result, rng, duration);
        audioManager?.PlayReelStop();
        reelsRunning--;
    }

    // Pays out and updates the message. The jackpot is checked first, so it replaces a normal win.
    private void ResolveSpin(SymbolData[] results)
    {
        float multiplier = WinEvaluator.Evaluate(results);

        if (IsJackpot(results))
        {
            // Jackpot pays the normal win multiplied by the bonus
            int jackpotWin = Mathf.RoundToInt(CurrentBet * jackpotBonusMultiplier * multiplier);
            balance += jackpotWin;
            RefreshUI($"JACKPOT! +{jackpotWin}");

            audioManager?.PlayJackpot();
        }
        else if (multiplier > 0f)
        {
            int winAmount = Mathf.RoundToInt(CurrentBet * multiplier);
            balance += winAmount;
            RefreshUI($"WIN! +{winAmount}");

            audioManager?.PlayWin();
        }
        else
        {
            RefreshUI("No win. Try again!");
        }
    }

    // Enables or disables the lever and bet buttons together.
    private void SetControlsInteractable(bool interactable)
    {
        spinButton.interactable = interactable;
        betUpButton.interactable = interactable;
        betDownButton.interactable = interactable;
    }

    // Updates the balance, bet, and message text on screen.
    private void RefreshUI(string message)
    {
        balanceText.text = $"Balance: {balance}";
        betText.text = $"Bet: {CurrentBet}";
        messageText.text = message;
    }
}