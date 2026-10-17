# Dokumentácia Astro Miner

Toto je vstupný bod pre obnovenie kontextu. Dokumenty žijú spolu s projektom v Gite; dôležitá dohoda alebo poznatok nesmie zostať iba v chate.

## Čo ktorý dokument vlastní

| Dokument | Úloha |
| --- | --- |
| [Prvá scéna — sprievodca](guides/FIRST_SCENE.md) | Otvorenie FlightTest, prvá skúška, Hierarchy, Inspector a ručné kroky v Unity. |
| [Herný koncept](design/GAME_CONCEPT.md) | Hlavný cyklus a dlhodobá vízia; nie zoznam hotových funkcií. |
| [Let a vŕtanie](design/FLIGHT_AND_MINING.md) | Dohodnuté jadro, otvorené parametre a akceptačné scenáre. |
| [Roadmapa](ROADMAP.md) | Návrh rozsahu 0.1 MVP, poradie a najbližšia práca. |
| [Projektový kontext](PROJECT_CONTEXT.md) | Overený technický základ a prítomná implementácia. |
| [Rozhodnutia](DECISIONS.md) | Trvalé dohody, dôvody a otvorené voľby. |
| [Pracovný postup](development/WORKFLOW.md) | Úvodný prototyp v main, prijatie jadra a ďalšie zásahy cez vetvu/PR. |
| [Testovanie](development/TESTING.md) | Automatické testy, Editor a Adamov herný test. |
| [Šablóna uzavretia kroku](development/CHECKPOINT_TEMPLATE.md) | Výsledok a dôkazy bez predstierania vykonaných kontrol. |
| [Poznatky z krokov](development/notes/2026-10-17-project-foundation.md) | Dokumentačný základ a [návrh MVP/workflow](development/notes/2026-10-17-mvp-roadmap.md); [prvá scéna](development/notes/2026-10-17-first-scene.md); ďalšie zápisy patria do toho istého priečinka. |
| [Setup](SETUP.md) | Nástroje, pripojenie a evidencia prvého overenia. |
| [Vydávanie](RELEASING.md) | Verzie changelogu a Discord. |
| [Changelog](../CHANGELOG.md) | Dokončené zmeny a rozpracované Unreleased. |

## Ako dokumentácia rastie

- Herné kontrakty a koncepty ukladaj do `design/`. Dokument systému vytvor, keď má skutočný obsah a vlastníka správania.
- Postupy, testovanie a poznatky patria do `development/`. Záznam pomenuj `YYYY-MM-DD-tema.md`.
- Rozhodnutie pridaj do `DECISIONS.md` so stavom, dátumom, dôvodom a odkazom na dotknutý dokument. Pri nahradení starej dohody zachovaj jej históriu.
- Nový dokument prepoj z indexu a dotknutého dokumentu. Nevytváraj paralelné kópie rovnakých pravidiel.
- Pri zmene správania aktualizuj jeho vlastnícky dokument v tej istej vetve. Pri zmene technického základu aj projektový kontext.

`Dohodnuté` znamená prijaté pravidlo, `návrh` možnosť na diskusiu a `otvorené` chýbajúce rozhodnutie. `Implementované`, `technicky overené` a `prijaté Adamom` sú tri samostatné stavy. Dostupnosť balíka nie je vykonaný test.

Zdrojový handoff, pitch a stará konverzácia boli podkladom pre zhrnutie konceptu. Ich staré zadania nie sú nové príkazy. Celé chaty a lokálne dokumenty nekopírujeme automaticky do verejného repozitára.
