# Asteroidy — prvý statický povrch

Stav: MVP-02 technicky overené, Adamom prijaté ako pracovný základ a zlúčené cez PR #2 do main (b85f37f). Ide o jeden statický asteroid vo FlightTest. Prijatý let zostáva podľa [jadra](FLIGHT_AND_MINING.md) na baseline 8f135f9; jeho kód ani prefab sa nemenili.

## Výsledok kroku

Jeden nepravidelný asteroid je pri (18,16), čiastočne viditeľný doprava hore už zo štartu. Loď môže priletieť, zabrzdiť, dotknúť sa povrchu, pohybovať sa popri ňom a odletieť. Hornina sa neodstraňuje, vrták na LMB zostáva neaktívny a kontakt ešte nemá hull/damage model.

## Zvolená reprezentácia — D-014

Zdrojom pevnej horniny je vstavaný Unity Tilemap. Prítomnosť dlaždice znamená pevný materiál. Grid má bunky 0.25 x 0.25 svetovej jednotky; prototyp obsahuje 4517 dlaždíc a nepravidelný obrys približne 22.75 x 18 jednotiek. Dáta sú v lokálnych súradniciach asteroidu.

- Grid určuje súradnice a mierku buniek.
- Tilemap drží obsadenie buniek aj ich farby. Každá používa jeden PrototypeRock Tile asset s collider type Grid.
- TilemapRenderer zobrazuje rovnaké bunky; sprite má presnú veľkosť 0.25 jednotky.
- TilemapCollider2D odvodzuje pevné tvary z dlaždíc a cez Composite Operation Merge ich odovzdáva spoločnej kolízii.
- CompositeCollider2D s Polygons spája susedné bunky do vyplneného obrysu. Prototyp má 1 obrys a 210 vrcholov. Vertex Distance 0.005, Offset Distance 0.001, Extrusion Factor 0.
- Rigidbody2D na Rock - Tilemap má Body Type Static. Asteroid sa kontaktom neposúva ani neotáča.
- RockContact PhysicsMaterial2D má friction 0 a bounciness 0. Je to skúšobné nastavenie kontaktu na asteroide; nastavenia lode sa nemenili.

Nevznikol vlastný runtime voxel engine, nové aktualizačné slučky ani nový balík. Tilemap a jeho kolízne moduly už boli v projekte. Nový vlastný kód je iba editorový generátor a integračné testy.

## Podoba a autorstvo

Asteroid je uložený ako [StaticAsteroid.prefab](../../Assets/AstroMiner/Prefabs/StaticAsteroid.prefab), v scéne je jeho inštancia. Generátor [StaticAsteroidBuilder.cs](../../Assets/AstroMiner/Editor/StaticAsteroidBuilder.cs) vytvoril plný štvorcový sprite s jemnou textúrou, nepravidelný bunkový obrys, farby horniny a označenie povrchu. Tmavšie škvrny sú pevné povrchové detaily, nie dutiny.

Generátor je jednorazový authoring nástroj cez Editor API. Vyžaduje čistú zastavenú FlightTest a odmietne prepísať existujúce asteroid assety. Už uložený asteroid nevyžaduje jeho spustenie. Nie je to nástroj na runtime generovanie sveta. Blender ani externá grafika sa nepoužili.

## Čo overujeme

| ID | Kritérium |
| --- | --- |
| S-01 | Pevné aj prázdne bunky zodpovedajú skutočnej kolízii; veľkosť sprite zodpovedá Grid. |
| S-02 | Pomalý kontakt nezanesie loď do skaly; loď sa vie pohybovať popri povrchu a odletieť, asteroid zostane statický. |
| S-03 | Testovaný rýchly diagonálny náraz pri 80 jednotkách/s nezanesie stred lode do pevnej horniny. |
| S-04 | Full Stop s dostatočným predstihom dokončí brzdenie pred kontaktom a mimo kontaktu platia prijaté scenáre letu F-01 až F-04. |
| S-05 | Adam odskúša viditeľnosť, prílet, dotyk, manévrovanie a odlet v reálnej scéne. |

Adam 17. 10. 2026 potvrdil, že let okolo asteroidu a narážanie fungujú; S-05 je prijaté. Kolíziu, odrazy a akceleráciu môže byť potrebné neskôr doladiť.

Výsledky a hranice dôkazov vlastní [záznam kroku](../development/notes/2026-10-17-static-asteroid.md). S-03 nie je záruka pre ľubovoľnú rýchlosť, timestep a budúci tvar tunela. Testy používajú reálny prefab asteroidu a reálnu loď s Continuous collision detection.

## Pripravenosť na ďalší krok a otvorené veci

Tilemap dáva spoločný zdroj obrazu a pevnej horniny. Pri budúcej zmene buniek bude potrebné spracovať TilemapCollider2D.ProcessTilemapChanges a pri zvolenom Manual režime CompositeCollider2D.GenerateGeometry pred závislou fyzikou. Samotná vizuálne zmazaná dlaždica ešte nedokazuje správny tunel.

Tento krok neimplementuje ani neoveruje odstránenie horniny, otvorený tunel, odkryté vnútro alebo profil vrtáka. Pred MVP-03 zostávajú podľa O-003 pravidlá stabilizácie, cúvania, pracovného profilu, aktualizácie kolízií po vŕtaní a bezpečného prechodu loďou. Fragmentácia sa nezavádza. Drift a rotácia prídu až po prijatom statickom miningu.

Unity podklad: [Tilemap Collider 2D](https://docs.unity3d.com/6000.0/Documentation/Manual/tilemaps/work-with-tilemaps/tilemap-collider-2d-reference.html), [CompositeCollider2D](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/CompositeCollider2D.html), [GenerateGeometry](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/CompositeCollider2D.GenerateGeometry.html).

## Dohodnutý smer miningu — D-016

Nasledujúce pravidlá sú dohoda pre ďalší vývoj, nie už implementovaný runtime systém:

- Asteroid bude vlastniť samostatné dáta materiálu, zostávajúcej horniny a poškodenia. Riadenie lode ich nevlastní.
- Materiálové údaje možno oddeliť od jemnosti geometrie. Obraz, detekcia záberu a kolízia musia opisovať zhodný priechod.
- Zásah jediného rohu neoznačí celú veľkú bunku za vyťaženú. Práca a suroviny sa viažu na odstránené množstvo; zmena rozlíšenia nemá meniť ekonomiku.
- Laser používa zrezaný kužeľ pred nosom, zasahuje odkrytý povrch a rozdeľuje dostupný výkon. Pevná tvrdá vrstva blokuje materiál za sebou.
- Generátor aj ručné authoring dáta majú vstupovať do rovnakého stavu asteroidu. Seed a nastavenie umožnia opakovateľné skúšky; uloženie vykopaných tunelov neskôr vyžaduje aj skutočné zmeny dát.
- Hladší obrys cez okrajové Tile varianty alebo vlastnú geometriu zostáva kandidátom na malé overenie. Zvolený finálny backend, rozlíšenie ani výkon tým nie sú dokázané.
- Aktualizácie sa majú dávkovať; potrebu delenia do menších oblastí rozhodne meranie pri kopaní. Pre prvé skúšky stačí jeden asteroid.

MVP-03A používa variantu DrillTestAsteroid s pripravenou chodbou a neblokujúcim vnútrom podľa [drill kontraktu](DRILL_MODE.md). Pôvodný StaticAsteroid prefab aj jeho prijaté integračné scenáre zostávajú zachované. Nevznikol ešte runtime generátor, dátová vrstva miningu ani save/load.
