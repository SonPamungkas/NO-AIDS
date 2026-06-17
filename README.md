<img width="225" height="225" alt="1000119834" src="https://github.com/user-attachments/assets/7c326273-5ab3-41e1-a601-0649b70bcce3" />
Bomb stations get an accurate CCIP impact-point computer for toss and dive bombing — but non-combat airdrops never did, even though they fall under the exact same ballistics. NO-AIDS fixes that with no UI changes you'll notice: it's the same HUD you already know from bombing, now active for cargo too.

## What it does
- Routes qualifying cargo/troop weapon stations through the game's real bombing HUD (`HUDBombingState`) instead of leaving them with no drop assist at all.
- Computes ballistics using the actual cargo item's real mass, since the engine's drag/fin-area data is normally tied to a `Missile` component that cargo objects don't have.
- Predict impact point (close enough) for parachute-equipped drops, since the stock CCIP math assumes constant free-fall and otherwise overshoots once a chute opens mid-descent. 

## Compatibility
- Pure Harmony patches, no asset/prefab edits. Doesn't touch real bomb, missile, turret, or gun stations.
- Selecting a cargo/troop station shows the bombing HUD in place of the cargo ammo/status readout while that station is active.
- Fully client sides, works in multiplayer.

## Install
Drop `NO-AIDS.dll` into `BepInEx/plugins/`.

## Notes
The parachute distance correction is a tuned approximation, not a full two-phase trajectory simulation — depending on the wind and speed, the error are anywhere around 200m (They're unguided) if the cargo starts falling exactly at 0.
