# Baraf-Paani — Multiplayer

The networked build of *Baraf-Paani*, a 3D freeze-tag game in Unity and C#.
*Baraf-Paani* is the South Asian playground version of freeze tag:
a catcher freezes runners on touch, and runners free frozen teammates by reaching
them first.


Built as my final-year BSc Software Engineering project.

The single-player game — including the AI catcher, AI runners and ragdoll NPCs —
is in [Freeze_Tag_Single](https://github.com/Aoisama19/Freeze_Tag_Single), and is
also present in this repository.

---

## What's in here

This repository contains two things:

1. **The full single-player game** — same maps, AI agents, power-ups and menus as
   `Freeze_Tag_Single`. This is a slightly earlier snapshot of that code: the
   runner AI here still does chaser evasion and uses power-ups while fleeing, which
   the single-player repository later replaced with a simpler rescue-focused
   behaviour. See that repository's README for how the AI works.
2. **A networked multiplayer layer** built on **Mirror 96.0.1**, in
   `Assets/zzzzz multiplayer/`. This is a working host/client prototype of the
   freeze mechanic, on its own bare scene — it does not yet run the full game's
   maps, AI agents or power-ups over the network.

---

## The multiplayer layer

Mirror, in a host/client model: one peer runs as host (server + local client) and
others connect as clients. Roles are assigned server-side — the first connection
becomes the catcher, everyone after is a runner.

**Server-authoritative freezing — `PlayerFreezeManager.cs`.** Freeze state is a
`[SyncVar]` with a change hook, so the server owns the truth and every client is
told about a change rather than deciding locally. A client that thinks it can freeze
someone doesn't apply it directly: it range-checks locally, then issues a
`[Command]` to the server, and the server re-checks the target's role before
committing. Role is validated on both sides of that call, so a catcher can only
freeze unfrozen runners and only a runner can unfreeze. The `SyncVar` hook then
drives the local effects on every client — disabling movement, swapping the
material colour, and toggling the floating "FROZEN" tag above the player.

**Networked movement — `ThirdPersonCharacterNetwork.cs`.** A rigidbody third-person
controller that disables itself and goes kinematic on non-local players, so each
client only simulates the character it owns. Movement is camera-relative, with a
raycast ground check, an extra-gravity multiplier for a less floaty fall, and root
motion driven through `OnAnimatorMove`. Freezing zeroes velocity and clamps the
animator rather than just blocking input, so a frozen player stops dead instead of
sliding.

**Camera and UI.** `CinemachineTargetSetter.cs` binds the scene's Cinemachine
FreeLook rig to the local player only. `FrozenUIManager.cs` shows a live frozen
count, and only to the catcher.

`PlayerController.cs` is an earlier, simpler pass at the same mechanic — direct
transform movement and `OverlapSphere` targeting — kept in the repository as the
first working version.

---

## Stack

- **Unity 2021.3.18f1** (URP 12.1.10)
- **C#**
- **Mirror 96.0.1** for networking
- Unity AI Navigation (NavMesh), Input System, Cinemachine, Visual Effect Graph,
  TextMeshPro
- Ready Player Me avatar SDK + glTFast for character loading

---

## Running it

```bash
git clone https://github.com/Aoisama19/Freeze_Tag_Multiplayer.git
```

Open the project in **Unity 2021.3.18f1**. First import takes a while — the Ready
Player Me and glTFast packages are pulled from Git URLs, so you need a network
connection on first open.

**To try the multiplayer prototype:**

1. Open `Assets/zzzzz multiplayer/Scenes/zzzzz.unity`.
2. Build the project (`File → Build and Run`) so you have a standalone player, and
   also press Play in the Editor — you need two instances.
3. On the first instance, use the Mirror NetworkManager HUD to click **Host**. The
   host becomes the catcher.
4. On the second, click **Client** to connect to `localhost`. Connecting clients
   become runners.
5. Move with **WASD**, jump with **Space**. Press **F** to freeze as the catcher, or
   to unfreeze a frozen teammate as a runner — within roughly 3 metres of the target.

**To play the single-player game:** open `Assets/Scenes/Intro.unity` and press Play.
Controls for that mode are documented in the
[Freeze_Tag_Single README](https://github.com/Aoisama19/Freeze_Tag_Single).

