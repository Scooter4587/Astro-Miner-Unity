# Technický kontext projektu

Kontrola: 17. 10. 2026. Základ pred prvou scénou bol `6d5ea57`. Pripravená je scéna a náhľad priestoru; Adamovo vizuálne prijatie čaká. Skutočný let a mining nie sú implementované.

## Overený základ

| Oblasť | Stav a dôkaz |
| --- | --- |
| Projekt | `C:\Development\Astro Miner Unity`, GitHub `Scooter4587/Astro-Miner-Unity`, hlavná vetva main. |
| Editor | Unity `6000.6.2f1`, revízia `770e33f6875c` podľa [ProjectVersion.txt](../ProjectSettings/ProjectVersion.txt). Živé spojenie bolo pri kroku overené. |
| Rendering | Universal 2D, URP 17.6.0. [QualitySettings.asset](../ProjectSettings/QualitySettings.asset) používa [UniversalRP.asset](../Assets/Settings/UniversalRP.asset) a [Renderer2D.asset](../Assets/Settings/Renderer2D.asset). Nová scéna zobrazuje sprity spoločným unlit materiálom. |
| Vstup | Input System 1.20.0; vlastný CameraPreviewPan používa Keyboard.current iba na náhľad. Finálne herné vstupy a action map ešte nie sú vytvorené. |
| Testovanie | Test Framework 1.8.0, vlastná EditMode assembly so 6 prípadmi parallaxu. PlayMode testovacia assembly zatiaľ neexistuje; náhľad bol overený v reálnom Play Mode. |
| Nástroje | Pipeline 0.8.0-exp.1, Unity IDE integrácia 2.0.26 a VS Code; [SETUP.md](SETUP.md) zachováva prvé overenie debuggeru. Dostupnosť Editora sa overuje pred každým zásahom. |
| Cieľ | Windows, klávesnica a myš. Player build pre novú scénu ešte nebol overený. |

Verzie balíkov určujú [manifest.json](../Packages/manifest.json) a [packages-lock.json](../Packages/packages-lock.json). Balíky a existujúce nastavenia projektu sa nemenili. Unity pri prvom vytvorení scény vygenerovalo nový ProjectSettings/SceneTemplateSettings.json s predvolenými pravidlami šablón; je súčasťou tohto kroku.

## Scény, objekty a štart

- [FlightTest.unity](../Assets/AstroMiner/Scenes/FlightTest.unity) je otvorená vlastná skúšobňa. Obsahuje Main Camera, štyri opakované vrstvy pozadia, pevnú mriežku a majáky, vizuálny prefab lode.
- Kamera má ortografickú veľkosť 12, šípky na náhľad, Shift na zrýchlenie a Home na návrat. Pohybuje sa kamera, loď zostáva pevná a nemá Rigidbody2D ani collider.
- Vlastné pozadie má 9 dlaždíc na vrstvu s veľkosťou 64. Pomery pohybu voči obrazu sú 0.03, 0.07, 0.20 a 0.45. Svetové značky neparallaxujú.
- Nová scéna má 109 SpriteRenderer komponentov a 0 missing scripts; loď je skutočná inštancia [ShipPlaceholder.prefab](../Assets/AstroMiner/Prefabs/ShipPlaceholder.prefab).
- Pôvodná `Assets/Scenes/SampleScene.unity` zostala nezmenená a stále je jedinou zapnutou scénou v [EditorBuildSettings.asset](../ProjectSettings/EditorBuildSettings.asset). Nie je zavedený herný startup ani načítanie sveta.

## Kód a assemblies

| Vlastník | Obsah |
| --- | --- |
| `AstroMiner.Runtime` v Assets/AstroMiner/Runtime | ParallaxLayer2D a CameraPreviewPan v namespace AstroMiner.Presentation; referencia na Unity.InputSystem, bez UnityEditor kódu. |
| Predvolená editorová assembly, Assets/AstroMiner/Editor | EnvironmentCheck a FlightTestSceneBuilder. Generátor vytvoril PNG, materiál, prefab a scénu cez Editor API; existujúcu scénu neprepisuje. |
| `AstroMiner.EditModeTests` v Assets/AstroMiner/Tests/EditMode | Editor-only testy, referencia na AstroMiner.Runtime a TestAssemblies. |
| `scripts/` a `tests/` v koreni | Samostatný Node nástroj a 6 testov Discord changelogu; nejde o Unity testy. |

Grafické podklady sú v `Assets/AstroMiner/Art/Prototype/`. Použité sú jednoduché generované PNG, nie finálna grafika ani Blender assety. Počas Update sa nevyrábajú nové obrázky ani objekty; parallax posúva iba štyroch rodičov vrstiev.

C# používa namespace AstroMiner.*, súkromné serializované polia s podčiarkovníkom a štyri medzery. UTF-8 a LF určuje [.editorconfig](../.editorconfig). Let, vrták, materiál asteroidu a zdroje zatiaľ nemajú runtime architektúru.

## Overenie a limity

6/6 Unity EditMode prípadov prešlo. V Play Mode bolo overené simulované držanie šípky doprava, zodpovedajúci posun vrstiev a nehybná loď; Home po oprave vrátil kameru aj pozadie na začiatok. Screenshot pôvodného a posunutého záberu bol prezretý. Výsledky a opravy sú v [zázname kroku](development/notes/2026-10-17-first-scene.md).

[Sprievodca scénou](guides/FIRST_SCENE.md) obsahuje presné otvorenie, ovládanie, Hierarchy, Inspector a ručný ekvivalent tvorby. Play Mode bol po kontrole zastavený.

Prvá interná výnimka Unity Search a starý Pipeline timeout zostávajú historicky v [SETUP.md](SETUP.md). CI zatiaľ spúšťa iba Node testy changelogu, nie Unity testy ani Player build. Discord úspešne odoslal 0.0.1; nové pracovné zmeny sú zatiaľ v Unreleased.

## Ďalšie rozhodnutia

Presné vstupy a parametre letu ostávajú podľa [kontraktu](design/FLIGHT_AND_MINING.md) otvorené. CameraPreviewPan sa pred skutočným letom vypne alebo nahradí. Reprezentácia horniny a kolízií sa rozhodne pred miningom; zdroje, save systém, ekonomika a gameplay UI ešte nie sú zavedené.
