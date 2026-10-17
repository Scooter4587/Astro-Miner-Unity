# Rozhodnutia a otvorené otázky

Dohody sa ukladajú sem a do dokumentu, ktorý vlastní pravidlo. Identifikátory zostávajú stabilné; nahradené rozhodnutie sa označí a prepojí na nové. Návrh nie je schválenie implementácie.

## Prijaté

| ID | Dátum | Rozhodnutie a dôvod | Vlastnícky dokument |
| --- | --- | --- | --- |
| D-001 | 2026-10-17 | Samostatné verejné Unity repo; Godot je historická referencia. Overíme prácu v Unity bez prenášania starej architektúry. | [Projektový kontext](PROJECT_CONTEXT.md) |
| D-002 | 2026-10-17 | Malé vysvetlené kroky, najprv let, potom statické vŕtanie a až potom pohyb asteroidu. Adam chce rozumieť tomu, čo sa stavia. | [Roadmapa](ROADMAP.md) |
| D-003 | 2026-10-17 | Smer z klávesnice, otáčanie RCS, motor po zarovnaní, zotrvačnosť, postupný Full Stop na C, predný vrták na LMB. Zachovať identitu lode. | [Let a vŕtanie](design/FLIGHT_AND_MINING.md) |
| D-004 | 2026-10-17 | Tunely nefragmentujú asteroid. Dvojvrstvový princíp oddeľuje odstrániteľnú horninu od neblokujúceho vnútra. | [Let a vŕtanie](design/FLIGHT_AND_MINING.md) |
| D-005 | 2026-10-17 | Nové verzie changelogu po príchode do main posiela GitHub Actions na Discord cez repository secret. | [Vydávanie](RELEASING.md) |
| D-006 | 2026-10-17 | Pôvodne zvolená vetva + PR + Adamov test/schválenie pre každý krok. Git režim neskôr spresnil D-008; oddelenie technického overenia a herného prijatia zostáva. | [Workflow](development/WORKFLOW.md) |
| D-007 | 2026-10-17 | Dokumentácia a poznatky patria do repo; agent má pred prácou obnoviť kontext a časom pridávať primerané testy. | [Index](README.md), [testovanie](development/TESTING.md) |
| D-008 | 2026-10-17 | Úvodný prototyp možno robiť priamo v main. Po prijatí letu alebo vrtáka sa zásahy do príslušného jadra robia cez vetvu a PR; vrátane nových systémov meniacich jeho správanie. Nahrádza plošné pravidlo D-006 podľa nového Adamovho zadania. | [Workflow](development/WORKFLOW.md), [stav jadra](design/FLIGHT_AND_MINING.md) |
| D-009 | 2026-10-17 | Najbližší cieľ je roadmapa k 0.1 MVP: loď, ovládanie, asteroid a mining. Podrobný rozsah dopĺňa D-010; jednotlivé implementačné kroky sa dohodnú samostatne, nejde o zadanie vyrobiť celé MVP naraz. | [Roadmapa](ROADMAP.md) |
| D-010 | 2026-10-17 | Adam spresnil 0.1 ako celý malý cyklus let, mining, zber a odovzdávanie s jednoduchými funkčnými systémami Hull, Fuel, Power, Heat a Cargo. Zdroje patria do 0.1, hoci nie do prvého letu. | [Roadmapa](ROADMAP.md), [koncept](design/GAME_CONCEPT.md) |
| D-011 | 2026-10-17 | Prvý realizačný krok pripravuje scénu, jednoduché vesmírne pozadie a čitateľný pohyb s parallaxom. Dočasný náhľad kamery nezavádza finálne ovládanie lode; Blender zatiaľ nie je potrebný. | [Prvá scéna](guides/FIRST_SCENE.md) |
| D-012 | 2026-10-17 | Adam potvrdil WASD vo svetových smeroch, C jedným stlačením spustí postupný Full Stop, nový smer ho preruší a nulová rýchlosť nevyvolá otočenie. Kamera sleduje loď. Prvý fyzikálny model používa Rigidbody2D, oddelený vstup a silu/torque; skúšobné hodnoty sú v Inspectore. | [Let](design/FLIGHT_AND_MINING.md), [sprievodca](guides/FIRST_FLIGHT.md) |
| D-013 | 2026-10-17 | Adam po reálnej skúške prijal prvý let a označil viditeľné RCS za zlepšenie oproti Godotu. Baseline letu je `8f135f934b25ec3092d805337f1929a2571c2ebe`; aktivuje sa pracovné pravidlo vetva + PR + Adamovo prijatie pre zásahy do letu podľa D-008. Technická branch protection tým nevzniká. | [Prijatie letu](design/FLIGHT_AND_MINING.md), [dôkazy](development/notes/2026-10-17-first-flight.md) |

## Otvorené — rozhodovať včas, nie všetko naraz

| ID | Rozhodnutie | Kedy ho potrebujeme |
| --- | --- | --- |
| O-003 | Reprezentácia horniny, aktualizácia kolízií, profil vrtáka, cúvanie a komora. | Po prijatí letu, pred statickým asteroidom. |
| O-004 | Samostatné PR kontroly, automatické Unity testy/buildy a potrebná licencia alebo runner. | Pri prvom hernom kroku určiť lokálny základ; vzdialenú Unity CI zaviesť samostatne podľa potreby. Zatiaľ nie je nastavená. |
| O-005 | Hranica upgradov, spracovania rudy na part, perzistentných saves a ďalšieho obsahu. | Nie sú automaticky uzavreté slovom kompletné systémy; upresniť pred ich zahrnutím do 0.1. |
| O-007 | Konkrétne kapacity, odbery, obnova, teplotné prahy a výpadky systémov lode. | Pred príslušným systémovým krokom; ich prítomnosť v 0.1 už určuje D-010. |
| O-008 | Damage, doplnenie paliva/oprava, zničenie a ďalší pokus/recovery vrátane strát. | Pred implementáciou Fuel/Hull a súvisiacich následkov. |

Pred letovým krokom netreba vybrať celý finálny tech stack ani celý tech tree. Otvorená otázka neblokuje nesúvisiacu prácu; blokuje iba implementáciu, ktorá by bez odpovede zaviedla nové herné pravidlo.

## Vyriešené otázky

- O-006: Adam 17. 10. 2026 zvolil širšiu hranicu než samotný let a vrták: zber, odovzdávanie a základné systémy lode patria do 0.1. Rozhodnutie D-010; konkrétny počet materiálov a číselné parametre zostávajú otvorené.

- O-001 a O-002: prvý implementačný kontrakt a fyzikálne členenie uzatvára D-012. Adamov test F-05 je prijatý podľa D-013; budúce číselné ladenie je zmena prijatého jadra.
