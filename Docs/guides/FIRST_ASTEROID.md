# Prvý asteroid — prílet a kontakt

Stav: pripravené na Adamovu skúšku vo vetve feature/static-asteroid. Prijatý let, WASD/C aj kamera zostali rovnaké. Nový krok ešte nie je zlúčený do main ani vydaný na Discord.

## Spustenie a krátka skúška

1. Použi rovnaký lokálny projekt `C:\Development\Astro Miner Unity`. Už je v ňom pripravená pracovná vetva; netreba sťahovať ďalší klon.
2. V **Project > Assets > AstroMiner > Scenes** dvojklikni na **FlightTest**.
3. Stlač **Play** a klikni do **Game**.
4. Doprava hore uvidíš časť asteroidu. Krátkym **W + D** letíš diagonálne k nemu. Skúšaj krátke zábery a sleduj povrch; zotrvačnosť zostáva zachovaná.
5. **C** spúšťa Full Stop. Začni brzdiť s predstihom: loď sa musí najprv otočiť, až potom brzdí hlavným motorom.
6. Pri malej rýchlosti sa jemne dotkni povrchu. Pevná skala loď zachytí; kontakt sám v tomto kroku neodoberá hull.
7. Vyskúšaj manéver popri okraji a potom nový smer od skaly, napríklad **A + S** od strany otočenej k štartu. Loď sa otočí, zaberie motor a odletí.
8. Zastav Play a prípadne ho spusti znovu pre čistý pokus. Loď, kamera aj asteroid sa vrátia do uloženého stavu.

LMB zatiaľ nevŕta. Tmavé škvrny na asteroide sú textúra pevného povrchu, nie diery. Podrobnejší let vysvetľuje [FIRST_FLIGHT.md](FIRST_FLIGHT.md).

Očakávame čitateľný prílet, pevnú horninu a možnosť z kontaktu odletieť. Pozoruj najmä prípadné zachytávanie o okraj, neprimerané odhodenie alebo rozdiel medzi viditeľným povrchom a dotykom. Po tvojej skúške uzavrieme výsledok v PR.

## Čo pribudlo v Hierarchy a Inspectore

Rozbaľ **Asteroid - Static Prototype**. Je to inštancia prefabu, teda uloženej predlohy objektu.

| Objekt alebo komponent | Čo znamená |
| --- | --- |
| **Asteroid - Static Prototype > Transform** | Poloha celého asteroidu: X 18, Y 16, Z 0. Rotácia 0 a mierka 1. |
| **Grid** na koreni | Súradnicová sieť. Cell Size je 0.25 x 0.25; nejde o orientačnú mriežku nakreslenú vo svete. |
| **Rock - Tilemap > Tilemap** | Dáta horniny. Obsadená bunka má dlaždicu; prázdna bunka ju nemá. |
| **Tilemap Renderer** | Kreslí obsadené bunky a ich farby. Order in Layer 5 dáva horninu pred mriežku a za loď. |
| **Rigidbody 2D** | Body Type Static znamená pevné teleso, ktoré náraz neposúva. |
| **Tilemap Collider 2D** | Vytvára fyzické tvary dlaždíc. Composite Operation Merge ich prepája so spoločným colliderom. |
| **Composite Collider 2D** | Zlučuje susedné bunky do vyplneného povrchu, takže vnútorné hrany každej dlaždice nie sú samostatné steny. Tu je priradený Rock Contact materiál. |

Obrázok a fyzická hranica sú dve odlišné úlohy, ale v tomto prototype vychádzajú z rovnakých buniek. Všetkých 4517 buniek tvorí jeden nepravidelný asteroid. Nepribudlo nové ovládanie ani vlastný runtime skript asteroidu.

## Ako si prezrieť fyzický povrch

Mimo Play vyber **Rock - Tilemap** v Hierarchy. V **Scene** zapni **Gizmos** a prezri obrys collidera pri povrchu. Pohľad Scene slúži na prácu s objektmi; Game ukazuje hernú kameru. Kláves **F** s kurzorom nad Scene zameria vybraný objekt.

V Inspectore si prezri najmä Static na Rigidbody 2D, Merge na Tilemap Collider 2D a Polygons na Composite Collider 2D. Vygenerovaný obrys nemusíš ručne upravovať. Cell Size a veľkosť sprite patria spolu; tento krok je pripravený na pozorovanie bez ich zmeny.

Materiál **RockContact** má Friction 0 a Bounciness 0. Určuje skúšobné trenie a pružnosť kontaktu. Je to projektový asset; jeho úprava je trvalá aj vtedy, keď ho otvoríš počas Play. Bežné zmeny Transformu objektu v scéne počas Play sa po zastavení vrátia. Aktuálne hodnoty necháme pre tvoju prvú skúšku.

## Kde sú súbory

| Súbor | Úloha |
| --- | --- |
| [FlightTest.unity](../../Assets/AstroMiner/Scenes/FlightTest.unity) | Pôvodná scéna s novou inštanciou asteroidu. |
| [StaticAsteroid.prefab](../../Assets/AstroMiner/Prefabs/StaticAsteroid.prefab) | Grid, obsadené bunky, renderer a kolízia. |
| [PrototypeRock.asset](../../Assets/AstroMiner/Tiles/PrototypeRock.asset) | Predloha jednej pevnej dlaždice; odkazuje na obrázok a má Collider Type Grid. |
| [RockCell.png](../../Assets/AstroMiner/Art/Prototype/RockCell.png) | Jednoduchý nový sprite horniny, 32 x 32 pixelov pri 128 pixels per unit. |
| [RockContact.physicsMaterial2D](../../Assets/AstroMiner/Physics/RockContact.physicsMaterial2D) | Trenie a pružnosť kontaktu. |
| [StaticAsteroidBuilder.cs](../../Assets/AstroMiner/Editor/StaticAsteroidBuilder.cs) | Jednorazové vytvorenie asteroidu cez Unity Editor API. |
| [StaticAsteroidTests.cs](../../Assets/AstroMiner/Tests/PlayMode/StaticAsteroidTests.cs) | Štyri integračné skúšky skutočných prefabov lode a horniny. |

Dvojklik na prefab ho otvorí v Prefab Mode. Dvojklik na skript ho otvorí vo VS Code. Dvojklik na materiál alebo Tile asset ukáže ich vlastnosti v Inspectore. Súčasná loď zostáva tým istým ShipPlaceholder prefabom z prijatého letu.

## Ručný ekvivalent vytvorenia

Asteroid som vytvoril editorovým C# nástrojom cez Editor API. Skript vytvoril aj jednoduchý obrázok horniny; Blender sa nepoužil. Nemusíš tieto kroky opakovať, už sú uložené. Manuálny ekvivalent by bol:

1. V Hierarchy vytvoriť **2D Object > Tilemap > Rectangular**. Unity pripraví Grid a pod ním Tilemap s rendererom.
2. Pomenovať koreň Asteroid - Static Prototype, nastaviť pozíciu (18,16,0) a Cell Size (0.25,0.25,1).
3. Pripraviť Tile asset so sprite veľkosti 0.25 jednotky a Collider Type Grid; jeho dlaždicami vyplniť nepravidelný obrys. Ja som bunky a farby zapísal priamo cez API.
4. Na Rock - Tilemap cez **Add Component** pridať **Rigidbody 2D** a nastaviť Static, potom **Tilemap Collider 2D** a **Composite Collider 2D**.
5. Nastaviť Composite Operation na Merge, spoločný collider na Polygons, priradiť RockContact a po vytvorení buniek vygenerovať geometriu. Pri zvolenom Manual režime sa prepočet vykoná výslovne; je súčasťou nástroja.
6. Pretiahnuť koreň z Hierarchy do Project > Assets > AstroMiner > Prefabs a uložiť scénu cez Ctrl+S.

Menu **Astro Miner > Setup > Add Static Test Asteroid** je po vytvorení vypnuté, aby generátor neprepísal existujúcu prácu. Projekt pri bežnom otvorení nepotrebuje generátor znovu spustiť.

## Čo bude potom

Po tvojom prijatí príde samostatný návrh vrtáka a aktualizácie horniny. Zmiznutie dlaždice musí neskôr odstrániť aj jej kolíziu; priechodný tunel, komoru a otočenie lode overíme až vtedy. Aktuálne sú pripravené statické dáta a kontakt.
