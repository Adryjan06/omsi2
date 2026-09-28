# OMSI64 — Milestone 2

Eksperymentalny, niezależny runtime **x64** do odczytu zawartości map OMSI 2.

> To nie jest modyfikacja `Omsi.exe` ani kopia kodu gry. OMSI64 ładuje pliki z legalnie zainstalowanej przez użytkownika kopii OMSI 2 i nie zawiera assetów gry.

## Co działa

- .NET 8 / x64,
- odczyt `global.cfg`,
- wykrywanie kafelków mapy,
- parser `tile_*.map`,
- odczyt standardowych `[object]`,
- odczyt `[spline]` i `[spline_h]`,
- zliczanie obiektów i spline'ów,
- kontrola brakujących plików `.sco` / `.sli`,
- debugowy renderer 3D:
  - niebieski — granice kafelków,
  - zielony — pozycje obiektów,
  - pomarańczowy — przybliżony przebieg spline'ów,
- tryb bez okna `--no-viewer` do samej diagnostyki mapy.

## Wymagania

- Windows 10/11 64-bit,
- .NET 8 SDK,
- karta graficzna ze wsparciem OpenGL 3.3,
- własna instalacja OMSI 2.

## Uruchomienie

```powershell
dotnet restore
dotnet run --project .\src\Omsi64.Engine -- "C:\Program Files (x86)\Steam\steamapps\common\OMSI 2\maps\Grundorf"
```

Sama analiza bez renderera:

```powershell
dotnet run --project .\src\Omsi64.Engine -- --no-viewer "C:\Program Files (x86)\Steam\steamapps\common\OMSI 2\maps\Grundorf"
```

Program wypisze m.in. liczbę kafelków, obiektów, spline'ów, brakujących assetów i ostrzeżeń parsera.

## Sterowanie viewerem

- `W/A/S/D` — przesuwanie,
- `Q/E` — zoom,
- `Shift` — szybszy ruch,
- `1` — kafelki on/off,
- `2` — obiekty on/off,
- `3` — spliny on/off,
- `R` — wycentrowanie,
- `Esc` — zamknięcie.

## Ważne ograniczenie Milestone 2

Renderer pokazuje na razie **geometrię diagnostyczną**, a nie finalne modele. Pozycje obiektów są zaznaczane krzyżykami, a spliny są odtwarzane z pozycji, obrotu, długości i promienia. W kolejnych etapach będziemy czytać właściwe `.sco`, `.sli` i geometrię `.o3d`.

## Plan rozwoju

### Milestone 3 — prawdziwe spliny i obiekty

- parser `.sli`,
- parser `.sco`,
- resolver zależności i tekstur,
- pierwsze profile dróg,
- podstawowe transformacje obiektów,
- przygotowanie importera `.o3d`.

### Milestone 4 — teren i tekstury

- pliki terenu kafelków,
- wysokości,
- warstwy tekstur,
- materiały,
- LOD i frustum culling.

### Milestone 5 — pojazd

- konfiguracja autobusu,
- model 3D,
- kamera kierowcy,
- podstawowa fizyka.

### Później

- kompatybilność skryptów OMSI,
- ruch AI,
- pasażerowie,
- rozkłady,
- audio,
- pogoda i oświetlenie.

## Technologia

- C#,
- .NET 8,
- x64,
- OpenTK 4.9.4,
- OpenGL 3.3 Core.

## Status

To projekt eksperymentalny. Celem jest stopniowe zbudowanie nowoczesnego 64-bitowego runtime'u kompatybilnego z możliwie dużą częścią istniejącej zawartości OMSI 2, bez dystrybucji oryginalnych assetów gry.
