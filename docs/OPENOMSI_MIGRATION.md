# Migration to openOMSI

## Decision

The current C# OMSI64 prototype should be treated as an experiment/archive. Future compatibility work should be based on `turbo-devv/openOMSI`, because upstream already contains the major subsystems we would otherwise need to recreate:

- OMSI script compiler + VM,
- vehicle runtime and rigid-body physics,
- map/scenery/spline/O3D parsers,
- timetable and HOF handling,
- AI traffic, pedestrians and passengers,
- audio,
- wgpu renderer,
- native OMSI plugin bridge + 32-bit plugin host,
- Lua plugins,
- multiplayer and dedicated server,
- Windows x64 builds.

Upstream: https://github.com/turbo-devv/openOMSI

## Important caveat

openOMSI is still an early release. Its README states the goal of 1:1 OMSI 2.2.032 behaviour, but current issues and the roadmap show that compatibility is not yet perfect. We should treat "fully compatible" as the target, not as something already proven for every add-on.

## Current upstream gaps / areas to verify

### Explicitly current in upstream roadmap

- bus stop shelters with waiting people inside,
- wear over a duty (bulb lifetimes, battery age),
- depot chooser informing the workshop whether the bus is standing in a depot.

### Areas with recent bug reports / fixes

These must be tested against the latest `main`, because some are already addressed after the latest packaged release:

- field-of-view / mesh deformation,
- passenger boarding behaviour,
- collision behaviour under bridges and with map objects,
- wheel angle configuration and force feedback,
- texture path case sensitivity,
- graphics-device-lost crashes / backend stability,
- parked cars and spline placement,
- glass/transparency materials.

### Compatibility claims that deserve regression tests

- stock script sets and modded script edge cases,
- rigid-body physics and steering matching OMSI,
- .o3d v4/v5 and .x meshes,
- materials, envmaps, bump maps, Z-bias/no-Z-write,
- map chrono and date-dependent HOF/timetable behaviour,
- original 32-bit OMSI plugin DLLs through the plugin host,
- large third-party maps and high-memory vehicle packs,
- articulated vehicles and multi-part trains,
- ticketing, IBIS and matrix displays.

## Our priorities

1. **Compatibility first** — avoid adding flashy features until common OMSI content runs correctly.
2. **Player vehicle fidelity** — steering, suspension, braking, articulated buses, FFB and scripts.
3. **Mod compatibility** — test popular buses/maps without modifying their files.
4. **Stability on Windows x64** — Vulkan/DX12 fallback, GPU-loss handling, memory use.
5. **Performance** — measure only after behaviour is correct.

## Proposed test matrix

### Stock baseline
- Grundorf
- Berlin-Spandau
- MAN SD200
- MAN SD202
- MAN NL202

### For every test vehicle
Check:
- spawn,
- engine start,
- gearbox,
- doors,
- dashboard,
- IBIS,
- destination display,
- lights,
- sounds,
- mirrors,
- scripts,
- steering,
- braking,
- suspension,
- passengers,
- timetable duty,
- save/load situation.

### For every test map
Check:
- terrain,
- splines,
- scenery,
- traffic lights,
- AI,
- parked cars,
- pedestrians,
- stops,
- chrono,
- HOF,
- timetable,
- night lighting,
- weather,
- collisions.

## Git workflow after fork exists

Repository: `Adryjan06/openOMSI`

Keep `main` close to upstream and make changes on focused branches:

- `compat/<bus-or-format>`
- `fix/<bug>`
- `perf/<subsystem>`
- `test/<coverage>`

Preferred flow:

1. sync from `turbo-devv/openOMSI:main`,
2. create a focused branch,
3. reproduce the issue,
4. add a regression test where possible,
5. implement the fix,
6. build Windows x64,
7. test against a real OMSI 2 installation,
8. open a PR upstream when the change is generic.

## First development task

Build a repeatable **compatibility smoke-test harness** around the existing `omsi-check` / parser crates so we can point it at an OMSI 2 installation and get a report such as:

- maps parsed / failed,
- buses parsed / failed,
- script sets compiled / failed,
- missing assets,
- unsupported keywords,
- plugin-host status,
- warnings grouped by add-on.

This gives us objective targets instead of fixing issues ad hoc.

## Existing prototype

The current `Adryjan06/omsi2` C# code remains useful as an archive of our initial exploration, but it should not become a second independent engine.
