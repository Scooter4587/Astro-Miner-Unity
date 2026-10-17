# Prvá scéna — FlightTest

Stav: scéna a náhľad priestoru technicky overené 17. 10. 2026. Adamovo vizuálne prijatie čaká. Toto ešte nie je letový model; loď sa nehýbe a nemá fyziku ani funkčný vrták. Blender sa pri tomto kroku nepoužil.

## Otvorenie a prvá skúška

1. V Unity v okne **Project** rozbaľ `Assets > AstroMiner > Scenes`.
2. Dvojklikni na **FlightTest**. Je to [uložený súbor scény](../../Assets/AstroMiner/Scenes/FlightTest.unity).
3. Stlač horné tlačidlo **Play** a klikni do okna **Game**.
4. Drž **šípky**: pohybuje sa kamera. **Shift + šípka** pohyb zrýchli. **Home** stlač alebo krátko pridrž pre návrat na začiatok.
5. Sleduj mriežku, majáky a hviezdy: pevné predmety prechádzajú záberom výraznejšie než vzdialené hviezdy.
6. Znovu stlač **Play**, čím skúšku zastavíš. Kamera sa vráti do uloženého stavu scény.

Náhľad kamery je obmedzený na 48 jednotiek od začiatku v každom smere. Nie je to hranica budúceho sveta ani náhrada fyziky lode. WASD, C a LMB ešte v tejto scéne neriadia loď.

## Čo vidíš v Hierarchy

| Objekt | Úloha a čo si prezrieť |
| --- | --- |
| **Main Camera** | Pohľad hry. V Inspectore má Transform, Camera s ortografickým zobrazením a veľkosťou 12, URP nastavenia a Camera Preview Pan. |
| **Background - repeating parallax layers** | Vrstvy Distant Haze, Far Stars, Middle Stars a Near Stars. Každá má 9 opakovaných obrázkov a komponent Parallax Layer 2D. |
| **World - fixed reference objects** | Pevná mriežka s rozostupom 8 jednotiek, orientačné majáky a kruh pri začiatku. Zostávajú na svojich svetových súradniciach. |
| **Ship Placeholder - visual only** | Jednoduchá loď s trupom, kokpitom a vizuálnym nosom vrtáka. Je inštanciou prefabu; zatiaľ nemá Rigidbody2D, collider ani ovládanie. |

Kliknutím na objekt v Hierarchy ho vyberieš. **Inspector** ukazuje jeho komponenty a ich hodnoty. **Scene** je pracovný pohľad na zostavu; **Game** je obraz hernej kamery. Vybraný objekt môžeš v Scene zamerať klávesom **F**, keď je kurzor nad Scene.

## Čo znamená parallax

Vzdialenejšie veci pri posune kamery vizuálne cestujú pomalšie. Použité pomery pohybu voči obrazu sú: hmla 0.03, vzdialené hviezdy 0.07, stredné 0.20 a blízke 0.45. Pevná mriežka a majáky používajú bežný pohyb sveta voči kamere.

V Inspectore vyber napríklad **Far Stars** a prezri komponent **Parallax Layer 2D**: Camera Transform je referencia na Main Camera, Motion Ratio je 0.07 a Tile Size je 64. Nie je potrebné nič meniť. Opakovanie dlaždíc zabraňuje tomu, aby sa pri ceste odhalil okraj pozadia.

## Kde sú súbory

| Cesta v projekte | Obsah |
| --- | --- |
| [Scenes/FlightTest.unity](../../Assets/AstroMiner/Scenes/FlightTest.unity) | Uložená zostava objektov a referencií. |
| [Prefabs/ShipPlaceholder.prefab](../../Assets/AstroMiner/Prefabs/ShipPlaceholder.prefab) | Opakovateľná predloha vizuálnej lode; scéna obsahuje jej inštanciu. |
| [Art/Prototype](../../Assets/AstroMiner/Art/Prototype) | Jednoduché PNG hviezd, hmly a tvarov; spoločný unlit materiál nepotrebuje osvetlenie na zobrazenie. |
| [ParallaxLayer2D.cs](../../Assets/AstroMiner/Runtime/Presentation/ParallaxLayer2D.cs) | Posun a opakovanie vrstvy podľa kamery. |
| [CameraPreviewPan.cs](../../Assets/AstroMiner/Runtime/Presentation/CameraPreviewPan.cs) | Dočasný náhľad kamery cez šípky, Shift a Home. |
| [FlightTestSceneBuilder.cs](../../Assets/AstroMiner/Editor/FlightTestSceneBuilder.cs) | Editorový nástroj, ktorým bola scéna a jednoduché obrázky vytvorené. |
| [ParallaxLayer2DTests.cs](../../Assets/AstroMiner/Tests/EditMode/ParallaxLayer2DTests.cs) | Prvých 6 Unity EditMode testovacích prípadov parallaxu. |

## Ako to bolo vytvorené — ručný ekvivalent

Scénu som vytvoril cez Unity Editor API, nie sériou ručných kliknutí. Skripty som napísal do súborov. Obrázky hviezd, hmly a tvarov vygeneroval editorový C# nástroj; nejde o import z Blenderu. Ručný postup s už pripravenými obrázkami by bol:

1. **File > New Scene**: vytvoriť prázdnu scénu; **Ctrl+S** ju uložiť do `Assets/AstroMiner/Scenes` ako FlightTest.
2. **GameObject > Camera**: pridať kameru, v Inspectore nastaviť Projection na Orthographic, Size na 12 a pozíciu Z na -10. Nastaviť tag MainCamera.
3. Cez **Create Empty** v Hierarchy vytvoriť rodičovské objekty Background a World. Rodič zoskupuje deti; jeho Transform ovplyvňuje ich umiestnenie.
4. Pretiahnuť sprite z Project do scény. Unity vytvorí objekt so **SpriteRenderer**, ktorý zobrazuje obrázok. V Inspectore sa nastaví farba, veľkosť, pozícia a Order in Layer.
5. Pre pozadie opakovať sprite v sieti 3 x 3 s rozostupom 64. Na rodiča cez **Add Component** pridať ParallaxLayer2D a pretiahnuť Main Camera do jeho Camera Transform.
6. Poskladať loď z detských sprite objektov a pretiahnuť jej koreň z Hierarchy do priečinka Prefabs. Tým vznikne uložená predloha.
7. Na Main Camera pridať CameraPreviewPan, uložiť scénu a spustiť Play.

V menu **Astro Miner > Setup > Create Flight Test Scene** je pripravený generátor pre prvé vytvorenie. Keď scéna existuje, položka je vypnutá, aby neprepísala prácu. Na bežné otvorenie používaj dvojklik na scénu v Project.

## Čo zatiaľ ladiť iba ako pokus

Počas Play si môžeš prezrieť Pan Speed alebo Motion Ratio. Zmeny v scéne počas Play sú spravidla dočasné; hodnoty, ktoré chceme zachovať, uložíme vedome mimo Play. Na prvú skúšku stačí pozorovať bez zmien.

Scéna zatiaľ nie je zaradená namiesto SampleScene do build nastavení a Player build sa v tomto kroku nevytváral. Pred nástupom skutočného letu náhľad kamery vypneme alebo nahradíme kamerou sledujúcou loď.

Ďalšie vysvetlenie: [Unity — scény](https://docs.unity3d.com/6000.0/Documentation/Manual/scenes-working-with.html), [pohyb v Scene view](https://docs.unity3d.com/Manual/SceneViewNavigation.html).
