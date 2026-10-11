# Unity Slot Machine

A 3-reel slot machine built in Unity 6 for a game development assignment.

## Game Overview

Pull the lever to spin three reels. A player wins when all reels show the same symbol. The **Seven** is the wild symbol: it substitutes for any other symbol when forming a win. Three Sevens hit the **jackpot**, which pays a bonus multiplier and plays a particle burst.

| Symbol | Pay multiplier | Notes |
|---|---|---|
| Seven | 50x | Wild, jackpot symbol |
| Bell | 5x | |
| Bar | 10x | |
| Cherry | 2x | |

Bet levels are 10, 20, 50, and 100. The player starts with a balance of 1000.

## Run the WebGL Build

The WebGL build is in `Build/WebGL`. Browsers won't load it from a file path, so serve it with a local web server:

1. Clone or download this repository.
2. Open a terminal in the `Build/WebGL` folder.
3. Start a server:
```bash
   python -m http.server 8000
```
4. Open `http://localhost:8000` in Chrome.
5. Click the lever once, since browsers block audio until the page is clicked.

## Bonus Features

- **Wild symbol:** the Seven substitutes for any other symbol.
- **Jackpot:** three Sevens pay a bonus multiplier and trigger a particle effect.
- **Animated lever:** the handle swings down when a spin starts and returns when the reels stop.
- **Sound effects:** lever pull, looping reel spin, reel stop clicks, win, jackpot, and button press.
- **Staggered reel stops:** each reel stops slightly after the one before it.

## Approach

- **Outcomes are chosen before animation.** Each reel's result is picked by weighted RNG before any reel moves, so the visuals always match the outcome. Weights control how often each symbol appears.
- **Win logic is separate from Unity.** `WinEvaluator` is a static class with no Unity dependencies, so it's easy to test.
- **Reel animation** scrolls a vertical strip of cells and eases it to a stop with an ease-out cubic curve. Each reel is driven by its own `ReelController`.
- **Game flow** lives in `SlotMachine`, which handles the balance, bet, spin sequence, payouts, and UI.
- **Audio** uses two `AudioSource`s: one for one-shot effects and one for the looping spin sound, so the loop can start and stop without cutting off other sounds.

## Project Structure