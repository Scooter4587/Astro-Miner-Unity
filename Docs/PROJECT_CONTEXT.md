# Technický kontext projektu

Kontrola: 17. 10. 2026. Prvá scéna bola vizuálne prezretá Adamom; nasledujúci prvý fyzikálny let je implementovaný a technicky overený. Adam prijal pocit z letu F-05; baseline `8f135f9` je odteraz chránený pracovným pravidlom vetva/PR. Statický asteroid a kontakt sú technicky overené a Adamom prijaté ako pracovný základ; PR #2 je zlúčený do main (b85f37f). MVP-03A teraz pridáva drill riadenie a pripravenú chodbu; Adamovo prijatie čaká. Samotná ťažba a palubné systémy zatiaľ neexistujú.

## Overený základ

| Oblasť | Stav |
| --- | --- |
| Projekt | `C:\Development\Astro Miner Unity`, GitHub `Scooter4587/Astro-Miner-Unity`, main. Žiadna súvislosť s Tanker Service. |
| Editor | Unity 6000.6.2f1, rev. 770e33f6875c; živé spojenie overené. |
| Rendering | Universal 2D, URP 17.6.0, Sprite-Unlit materiál prototypu. |
| Vstup | Input System 1.20.0; samostatný ShipKeyboardInput číta Keyboard.current. WASD/C podľa D-012, bez action map a rebindingu. |
| Testovanie | Test Framework 1.8.0; 6 EditMode prípadov parallaxu; nová celá PlayMode sada prešla 17/17 (7 drill, 6 let, 4 asteroid/kontakt). |
| Nástroje | Pipeline 0.8.0-exp.1, Unity IDE 2.0.26 a VS Code; prvotné overenie debuggeru v [SETUP.md](SETUP.md). |
| Cieľ | Windows, klávesnica/myš; Player build aktuálnej scény ešte nebol overený. |

Verzie určujú [manifest.json](../Packages/manifest.json) a [packages-lock.json](../Packages/packages-lock.json). Letový krok nezmenil balíky, pipeline ani ProjectSettings. Predchádzajúci scénový krok pridal predvolený SceneTemplateSettings.json.

## Scéna a štart — základ pred MVP-03A

[FlightTest.unity](../Assets/AstroMiner/Scenes/FlightTest.unity) obsahuje:

- **Main Camera**: ortografická veľkosť 12, Z=-10, ShipCameraFollow s referenciou na loď. Bez panu šípkami, rotácie alebo look-ahead.
- **Background - repeating parallax layers**: 4 vrstvy, každá 3 x 3 dlaždice veľkosti 64. Pomery 0.03, 0.07, 0.20 a 0.45; rodičia sa aktualizujú po kamere v LateUpdate.
- **World - fixed reference objects**: pevná mriežka s rozostupom 8 a rozsahom ±80, 8 majákov, kruh začiatku. Nejde o steny sveta.
- **Ship - Flight Prototype**: skutočná inštancia [ShipPlaceholder.prefab](../Assets/AstroMiner/Prefabs/ShipPlaceholder.prefab). Dynamic Rigidbody2D, CapsuleCollider2D, ShipFlightController, ShipKeyboardInput a ShipThrustVisuals. Vizuál vrtáka je stále neaktívny.

Fyzika: mass 1, gravity/linear damping/angular damping 0, interpolation Interpolate, Continuous. Povely menia silu a moment v FixedUpdate, nezapisujú transform ani velocity lode. RCS stabilizuje otáčanie bez rušenia lineárneho driftu. Kamera sleduje interpolovaný transform v LateUpdate pred parallaxom.

WASD zadáva svetový smer, C spustí postupný Full Stop, nový smer ho preruší. Parametre a hraničné pravidlá sú v [kontrakte](design/FLIGHT_AND_MINING.md). Nový pokus: Stop Play a znovu Play. Bez herného rýchlostného limitu. Šípky/Shift/Home v aktuálnej scéne nemajú túto funkciu.

Nový **Asteroid - Static Prototype** pri (18,16) má Grid s bunkou 0.25 a dieťa Rock - Tilemap: 4517 dlaždíc, TilemapRenderer, Static Rigidbody2D, TilemapCollider2D Merge a CompositeCollider2D Polygons. Vznikol [StaticAsteroid.prefab](../Assets/AstroMiner/Prefabs/StaticAsteroid.prefab), [PrototypeRock.asset](../Assets/AstroMiner/Tiles/PrototypeRock.asset), RockCell sprite a RockContact PhysicsMaterial2D (friction/bounciness 0). Kolízia má 1 obrys a 210 vrcholov. Žiadny nový vlastný runtime systém; zdrojom horniny je vstavaný Tilemap. [Kontrakt asteroidu](design/ASTEROIDS.md) zachováva limity.

SampleScene zostala nezmenená a je stále jedinou zapnutou scénou v [EditorBuildSettings.asset](../ProjectSettings/EditorBuildSettings.asset). Aktuálnu scénu preto skúšame explicitne v Editore; startup pre Player ešte nie je zavedený.

## Kód a assemblies

| Vlastník | Obsah |
| --- | --- |
| AstroMiner.Runtime, Assets/AstroMiner/Runtime | Flight: ShipFlightController a ShipKeyboardInput. Presentation: ParallaxLayer2D, ShipCameraFollow, ShipThrustVisuals a historický nepripojený CameraPreviewPan. Referencia Unity.InputSystem, bez UnityEditor závislosti. |
| Predvolená editorová assembly, Assets/AstroMiner/Editor | EnvironmentCheck, historický FlightTestSceneBuilder a nový StaticAsteroidBuilder na jednorazové pridanie asteroidu. Generátor vytvára iba pôvodný vizuálny krok a existujúcu scénu neprepisuje; nie je rekonštrukciou aktuálneho letu. |
| AstroMiner.EditModeTests | Editor-only, 6 prípadov parallaxu. |
| AstroMiner.PlayModeTests | 6 scenárov ShipFlightTests a 4 nové StaticAsteroidTests s reálnymi prefabmi; reference runtime a TestAssemblies. Asteroid testy sú pod UNITY_EDITOR, pretože načítavajú projektové prefab assety cez AssetDatabase. |
| scripts/ a tests/ v koreni | Samostatný Node nástroj a 6 testov Discord changelogu; nesúvisia s fyzikou Unity. |

Grafika zostáva v Assets/AstroMiner/Art/Prototype. Trysky používajú existujúci Triangle sprite; Blender ani nové balíky neboli potrebné. Bez per-frame výroby obrázkov/objektov. C# používa AstroMiner.*, súkromné serializované polia s podčiarkovníkom, štyri medzery a UTF-8/LF.

## Dôkazy a prijatie

[Záznam prvého letu](development/notes/2026-10-17-first-flight.md) dokladá 6/6 PlayMode a 6/6 EditMode, živé klávesové skúšky a kameru bez odchýlky od viditeľnej lode. Drift Y 13.6799879 zostal medzi meraniami zachovaný a Full Stop dosiahol (0,0) bez trvalého kmitania. Záber letu s tryskou bol vizuálne prezretý.

[Sprievodca letom](guides/FIRST_FLIGHT.md) vlastní spustenie, ovládanie, Hierarchy, Inspector, súbory a ručný ekvivalent pridania komponentov. Adam 17. 10. 2026 let prijal po skúške a spresnil, že oproti Godotu hodnotí viditeľnú prácu RCS ako zlepšenie. Prijatý commit letu je `8f135f934b25ec3092d805337f1929a2571c2ebe`; ďalšie zásahy doň idú cez vetvu a PR podľa D-008. Prijatý krok je zapísaný ako verzia 0.0.2 v changelogu.

Historický Unity Search error a Pipeline timeout sú v [SETUP.md](SETUP.md), nie nová regresia. GitHub CI spúšťa iba Node testy changelogu, nie Unity. Discord oznámenie 0.0.2 odosiela vydávací workflow po pushi nového changelogu; výsledok doručenia sa overuje v GitHub Actions. Toto nevytvára herný build ani technickú GitHub branch protection.

## Ďalší krok

Let a F-05 sú prijaté, základ je zaznamenaný. Zásahy do prijatého letu idú cez vetvu/PR podľa D-008. Statický asteroid a kontakt sú prijaté a v main. Nasleduje samostatný návrh prvého vrtáka a priechodného tunela; dnešné parametre kolízie, odrazov a akcelerácie zostávajú pracovné a môžu sa ďalej ladiť. Mining, zdroje, saves, ekonomika a HUD ešte nemajú implementáciu.

## MVP-03A — drill riadenie, stav vetvy

FlightTest používa ShipDrillPrototype a DrillTestAsteroid, varianty pôvodných prijatých prefabov. Loď a kamera začínajú pri (18,4); asteroid ostáva pri (18,16). Vnútri je vopred vytvorená L chodba so stredovou osou (18,5) → (18,19) → (24,19) a neblokujúcou tmavou podlahou. Nejde o výsledok runtime ťažby.

Pribudli DrillModeProfile, ShipDrillController, ShipDrillFeedback a ShipDrillDebug. ShipKeyboardInput smeruje vstup podľa R režimu; prijatý letový výpočet má iba explicitnú bránu riadenia. V runtime sa nemení transform/velocity lode a nevypínajú sa kolízie. Dnešná geometria je stále Tilemap 0.25; AsteroidState, generovanie sveta, hladký obrys, suroviny a saves ešte nie sú implementované.

Nový profil je v Assets/AstroMiner/Settings/DrillModePrototype.asset; autoritatívne správanie a prehľad jeho čísel vlastní [DRILL_MODE.md](design/DRILL_MODE.md). Sedem nových PlayMode scenárov používa skutočné prefab varianty; vstupné prípady izoluje oficiálny InputTestFixture z už nainštalovaného Input Systemu. Nepribudol nový balík ani zmena ProjectSettings.

Pôvodné podrobnosti scény a prijatia vyššie zachovávajú históriu prvých krokov; aktuálnu Play skúšku vlastní [DRILL_MODE_TEST.md](guides/DRILL_MODE_TEST.md). Technické výsledky a Adamovo prijatie vlastní [záznam](development/notes/2026-10-17-drill-mode.md).
