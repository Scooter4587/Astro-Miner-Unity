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
| D-009 | 2026-10-17 | Najbližší cieľ je roadmapa k 0.1 MVP: loď, ovládanie, asteroid a mining. Podrobné hranice a jednotlivé implementačné kroky sa ešte dohodnú; nejde o zadanie vyrobiť celé MVP naraz. | [Roadmapa](ROADMAP.md) |

## Otvorené — rozhodovať včas, nie všetko naraz

| ID | Rozhodnutie | Kedy ho potrebujeme |
| --- | --- | --- |
| O-001 | Smerové klávesy a súradnice smeru, prerušenie C, stav pri nulovej rýchlosti a skúšobné parametre letu. | Pred prvou letovou implementáciou; ďalšia diskusia. |
| O-002 | Rigidbody2D riešenie, členenie prvých tried a testovacie assemblies. | S malým plánom letového kroku; technický návrh vysvetlí agent. |
| O-003 | Reprezentácia horniny, aktualizácia kolízií, profil vrtáka, cúvanie a komora. | Po prijatí letu, pred statickým asteroidom. |
| O-004 | Samostatné PR kontroly, automatické Unity testy/buildy a potrebná licencia alebo runner. | Pri prvom hernom kroku určiť lokálny základ; vzdialenú Unity CI zaviesť samostatne podľa potreby. Zatiaľ nie je nastavená. |
| O-005 | Saves, recovery, zdroje lode, ekonomika, upgrady, ďalšie platformy a prípadný assisted flight. | Až po overení jadra, osobitné herné rozhodnutia. |
| O-006 | Či 0.1 uzavrie iba let a vŕtanie, alebo aj jednu surovinu a odovzdanie pri základni. | Pri odsúhlasení návrhu roadmapy; agent odporúča krátky cyklus s jedným výsledkom výpravy. |

Pred letovým krokom netreba vybrať celý finálny tech stack ani celý tech tree. Otvorená otázka neblokuje nesúvisiacu prácu; blokuje iba implementáciu, ktorá by bez odpovede zaviedla nové herné pravidlo.
