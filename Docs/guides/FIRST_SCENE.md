# Prvá scéna — FlightTest

Stav: prvú scénu s náhľadom Adam prezrel 17. 10. 2026 a následne zadal prvý let. Aktuálna FlightTest už obsahuje pohyblivú loď a kameru, ktorá ju sleduje. Ovládanie a novú zostavu vlastní [sprievodca prvým letom](FIRST_FLIGHT.md). Blender sa nepoužil.

## Otvorenie a orientácia

V **Project > Assets > AstroMiner > Scenes** dvojklikni na FlightTest, stlač **Play** a klikni do **Game**. WASD a C už ovládajú loď. Staré posúvanie kamery cez šípky, Shift a Home bolo nahradené. Kamera aj loď sa do uloženého začiatku vrátia po zastavení a opätovnom spustení Play.

V Hierarchy sú **Main Camera**, **Background - repeating parallax layers**, **World - fixed reference objects** a **Ship - Flight Prototype**. Kliknutím vyberieš objekt; **Inspector** ukáže jeho komponenty. **Scene** je pracovný pohľad a **Game** obraz hernej kamery. Vybraný objekt zameriaš klávesom **F** s kurzorom nad Scene.

## Čo znamená parallax

Vzdialenejšie veci pri posune kamery vizuálne cestujú pomalšie. Použité pomery pohybu voči obrazu sú: hmla 0.03, vzdialené hviezdy 0.07, stredné 0.20 a blízke 0.45. Pevná mriežka a majáky používajú bežný pohyb sveta voči kamere.

V Inspectore vyber napríklad **Far Stars** a prezri komponent **Parallax Layer 2D**: Camera Transform je referencia na Main Camera, Motion Ratio je 0.07 a Tile Size je 64. Nie je potrebné nič meniť. Opakovanie dlaždíc zabraňuje tomu, aby sa pri ceste odhalil okraj pozadia.

## Kde sú súbory

| Cesta v projekte | Obsah |
| --- | --- |
| [Scenes/FlightTest.unity](../../Assets/AstroMiner/Scenes/FlightTest.unity) | Uložená zostava objektov a referencií. |
| [Prefabs/ShipPlaceholder.prefab](../../Assets/AstroMiner/Prefabs/ShipPlaceholder.prefab) | Opakovateľná predloha lode; po letovom kroku má aj fyziku a ovládanie. |
| [Art/Prototype](../../Assets/AstroMiner/Art/Prototype) | Jednoduché PNG hviezd, hmly a tvarov; spoločný unlit materiál nepotrebuje osvetlenie na zobrazenie. |
| [ParallaxLayer2D.cs](../../Assets/AstroMiner/Runtime/Presentation/ParallaxLayer2D.cs) | Posun a opakovanie vrstvy podľa kamery. |
| [CameraPreviewPan.cs](../../Assets/AstroMiner/Runtime/Presentation/CameraPreviewPan.cs) | Historický náhľad kamery; v aktuálnej FlightTest už nie je pripojený. |
| [FlightTestSceneBuilder.cs](../../Assets/AstroMiner/Editor/FlightTestSceneBuilder.cs) | Historický generátor prvého scénového kroku; nie obnova aktuálneho letu. |
| [ParallaxLayer2DTests.cs](../../Assets/AstroMiner/Tests/EditMode/ParallaxLayer2DTests.cs) | Prvých 6 Unity EditMode testovacích prípadov parallaxu. |

## Ako to bolo vytvorené — ručný ekvivalent

Scénu som vytvoril cez Unity Editor API, nie sériou ručných kliknutí. Skripty som napísal do súborov. Obrázky hviezd, hmly a tvarov vygeneroval editorový C# nástroj; nejde o import z Blenderu. Ručný postup s už pripravenými obrázkami by bol:

1. **File > New Scene**: vytvoriť prázdnu scénu; **Ctrl+S** ju uložiť do `Assets/AstroMiner/Scenes` ako FlightTest.
2. **GameObject > Camera**: pridať kameru, v Inspectore nastaviť Projection na Orthographic, Size na 12 a pozíciu Z na -10. Nastaviť tag MainCamera.
3. Cez **Create Empty** v Hierarchy vytvoriť rodičovské objekty Background a World. Rodič zoskupuje deti; jeho Transform ovplyvňuje ich umiestnenie.
4. Pretiahnuť sprite z Project do scény. Unity vytvorí objekt so **SpriteRenderer**, ktorý zobrazuje obrázok. V Inspectore sa nastaví farba, veľkosť, pozícia a Order in Layer.
5. Pre pozadie opakovať sprite v sieti 3 x 3 s rozostupom 64. Na rodiča cez **Add Component** pridať ParallaxLayer2D a pretiahnuť Main Camera do jeho Camera Transform.
6. Poskladať loď z detských sprite objektov a pretiahnuť jej koreň z Hierarchy do priečinka Prefabs. Tým vznikne uložená predloha.
7. V pôvodnom kroku bola pridaná CameraPreviewPan. V aktuálnej scéne ju nahradila ShipCameraFollow podľa sprievodcu letom.

V menu **Astro Miner > Setup > Create Flight Test Scene** je pripravený generátor pre prvé vytvorenie. Keď scéna existuje, položka je vypnutá, aby neprepísala prácu. Na bežné otvorenie používaj dvojklik na scénu v Project.

## Čo zatiaľ ladiť iba ako pokus

Počas Play si môžeš prezrieť Motion Ratio alebo letové parametre podľa nového sprievodcu. Zmeny v scéne počas Play sú spravidla dočasné; hodnoty, ktoré chceme zachovať, uložíme vedome mimo Play. Na prvú skúšku stačí pozorovať bez zmien.

Scéna zatiaľ nie je zaradená namiesto SampleScene do build nastavení a Player build sa v tomto kroku nevytváral. Letový krok už náhľad nahradil kamerou sledujúcou loď.

Ďalšie vysvetlenie: [Unity — scény](https://docs.unity3d.com/6000.0/Documentation/Manual/scenes-working-with.html), [pohyb v Scene view](https://docs.unity3d.com/Manual/SceneViewNavigation.html).
