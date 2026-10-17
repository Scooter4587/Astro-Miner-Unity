# Technický kontext projektu

Konfigurácia overená 17. 10. 2026 na základe `28c9b35`; nad `3d55791` sa dopĺňa iba dokumentácia a roadmapa. Živý Editor bol znovu overený cez editor_status: správny projekt `C:\Development\Astro Miner Unity`, Unity 6000.6.2f1, ready, bez prebiehajúcej kompilácie, Play Mode zastavený. Nešlo o nový herný test ani Player build.

## Overený základ

| Oblasť | Stav a dôkaz |
| --- | --- |
| Projekt | Samostatný Unity projekt Astro Miner Unity, verejný GitHub `Scooter4587/Astro-Miner-Unity`. Lokálna cesta je v [SETUP.md](SETUP.md). |
| Editor | Unity `6000.6.2f1`, revízia `770e33f6875c` podľa [ProjectVersion.txt](../ProjectSettings/ProjectVersion.txt). |
| Rendering | Universal 2D, URP `17.6.0`. [QualitySettings.asset](../ProjectSettings/QualitySettings.asset) odkazuje na [UniversalRP.asset](../Assets/Settings/UniversalRP.asset), ktorý používa [Renderer2D.asset](../Assets/Settings/Renderer2D.asset). |
| Vstup | Input System `1.20.0`; `activeInputHandler: 1` v [ProjectSettings.asset](../ProjectSettings/ProjectSettings.asset). Vlastné herné mapovanie vstupov ešte neexistuje. |
| Testovanie | Unity Test Framework `1.8.0` je v [manifest.json](../Packages/manifest.json) aj [packages-lock.json](../Packages/packages-lock.json). Vlastné EditMode/PlayMode testy zatiaľ neexistujú. |
| Nástroje | Pipeline `0.8.0-exp.1`, Unity IDE integrácia `2.0.26`; verzie z balíkov. Živé spojenie a debugger boli overené pri setupe; dostupnosť sa kontroluje pred zásahom. |
| Prvý cieľ | Windows, klávesnica a myš. Herný Player build ešte nebol overený. |

## Štruktúra a štart

- `Assets/AstroMiner/Editor/EnvironmentCheck.cs`: jediný vlastný C# skript, editorová kontrola prostredia s namespace `AstroMiner.Editor`; nemá gameplay.
- `Assets/Scenes/SampleScene.unity`: jediná zapnutá scéna v [EditorBuildSettings.asset](../ProjectSettings/EditorBuildSettings.asset). Zatiaľ šablónová scéna, bez vlastnej hernej štartovacej sekvencie.
- `Assets/Settings/`: šablónové nastavenia URP, renderer a Input Actions. `Assets/Welcome/` je obsah šablóny s vlastným `Unity.2D.Welcome.asmdef`.
- Vlastné runtime assemblies, prefaby lode, let, vrták a materiál asteroidu ešte nie sú vytvorené. Vlastný editorový skript zatiaľ používa predvolenú editorovú assembly Unity.
- `scripts/` a `tests/`: Node nástroj a šesť testov pre Discord changelog. Nie sú to herné testy.
- `Docs/` vlastní dokumentáciu. `Library/`, `Temp/`, `Logs/`, `UserSettings/` a generované `.slnx`/`.csproj` sú lokálne výstupy.

## Architektúra a konvencie

Herná architektúra zatiaľ neexistuje. Dohodnuté je oddeľovať let, vrták a materiál podľa skutočnej potreby, nie vopred zaviesť veľký framework. Rigidbody2D a bunková hornina sú návrhy na overenie, nie potvrdené súčasti implementácie.

Existujúci C# má namespace `AstroMiner.Editor`, zložené zátvorky na nových riadkoch a štyri medzery. UTF-8 a LF určuje [.editorconfig](../.editorconfig). Štýl runtime tried a serializovaných polí sa ustáli pri prvom kóde; jeden editorový skript ho neurčuje celý.

## Čo bolo skutočne overené

[SETUP.md](SETUP.md) zachováva overenie otvorenia projektu, kompilácie nástroja, opakovaného Play Mode, otvorenia skriptu vo VS Code a pripojenia debuggeru z 17. 10. 2026. Prvá interná výnimka Unity Search sa pri druhom Play Mode nevrátila; nejde o dôkaz bezchybnosti budúcej hry.

Discord workflow úspešne odoslal `0.0.1`; [beh #1](https://github.com/Scooter4587/Astro-Miner-Unity/actions/runs/39024356401) overil aj 6 Node testov. CI zatiaľ nekompiluje Unity, nespúšťa Unity testy, netvorí Player build a nemá samostatný kontrolný workflow pre PR.

## Čo sa musí zistiť pri ďalšom kroku

- Aktuálny stav správneho Editora, Console a pracovného stromu.
- Presné vstupy a parametre letovej skúšobne podľa [kontraktu](design/FLIGHT_AND_MINING.md).
- Technika asteroidu a kolízií až pred vŕtaním.
- Unity testovacie assemblies a prípadná CI infraštruktúra pri prvom relevantnom použití.

Nie sú zavedené vlastné sieťovanie, save systém, ekonomika ani gameplay UI. Prítomnosť všeobecných šablónových balíkov nepotvrdzuje implementáciu týchto systémov.
