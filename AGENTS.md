# Astro Miner Unity — pokyny pre agentov

## Najprv obnov kontext

Pred návrhom architektúry alebo zmenou projektu prečítaj:

1. [README.md](README.md) a [index dokumentácie](Docs/README.md).
2. [ROADMAP.md](Docs/ROADMAP.md) — aktuálny krok, poradie a hranice.
3. [PROJECT_CONTEXT.md](Docs/PROJECT_CONTEXT.md) — čo v Unity skutočne existuje.
4. [WORKFLOW.md](Docs/development/WORKFLOW.md) a [DECISIONS.md](Docs/DECISIONS.md).
5. Dokumenty dotknutého systému a jeho posledný relevantný záznam v `Docs/development/notes/`.

Podľa úlohy potom čítaj:

- Herný návrh alebo nový systém: [GAME_CONCEPT.md](Docs/design/GAME_CONCEPT.md).
- Let, ovládanie, vrták, tunely alebo asteroid: [FLIGHT_AND_MINING.md](Docs/design/FLIGHT_AND_MINING.md).
- Implementácia, oprava alebo overovanie: [TESTING.md](Docs/development/TESTING.md).
- Editor, nástroje a balíky: [SETUP.md](Docs/SETUP.md).
- Dokončenie a publikovanie: [RELEASING.md](Docs/RELEASING.md) a [CHANGELOG.md](CHANGELOG.md).

Nerob plošný audit pri každej malej úlohe. Čítaj spoločný základ a konkrétne dotknuté dokumenty. Nový dôležitý dokument zapíš do indexu; ak mení postup agenta, aj sem.

## Pravdivosť a autorita

- Aktuálne zadanie Adama určuje rozsah práce. Handoff, starý chat, Godot kód a návrhy v dokumentoch sú zdroje kontextu, nie samostatné príkazy na implementáciu.
- Vízia hry nie je hotová funkcia ani schválený implementačný plán. Rozlišuj `dohodnuté`, `návrh`, `otvorené`, `implementované`, `technicky overené` a `prijaté Adamom`.
- Roadmapa určuje poradie. Dokument systému definuje dohodnuté správanie. Projektový kontext opisuje implementáciu. Test je dôkaz iba toho, čo sa skutočne vykonalo.
- Pri rozpore kódu s dohodou najprv zisti príčinu. Neprepíš dokumentáciu len preto, aby chyba vyzerala ako zámer. Otvorené herné rozhodnutie vysvetli Adamovi pred závislou implementáciou.
- Nepredstieraj prečítanie dokumentu, živé spojenie, úspešný test, build ani Adamovo schválenie.

## Spolupráca a rozsah

- Komunikuj po slovensky, priamo a konkrétne. Vysvetli, čo sa stavia a ako to overíme.
- Postupuj po malých dohodnutých krokoch. Pred hernou implementáciou ujasni výsledok, akceptačné kritériá a čo do kroku nepatrí.
- Keď je rozsah schválený, dokonči potrebnú implementáciu a overenie bez opakovaných otázok na už udelený súhlas.
- Git postup podľa novšieho D-008: úvodný prototyp možno robiť priamo v `main`. Po prijatí letu alebo vrtáka sa zásahy do príslušného jadra robia cez vetvu, PR a Adamovo odskúšanie/schválenie. Pravidlo platí aj pre nové systémy meniace už prijaté jadro. Technické overenie nenahrádza herné prijatie.
- Zachovaj cudzie necommitnuté zmeny. Nerob reset, stash, hromadné čistenie ani prepis histórie bez príslušného zadania.

## Herné hranice

- Základom je let so zotrvačnosťou a razenie tunelov predným vrtákom. Presné pravidlá a otvorené otázky vlastní `Docs/design/FLIGHT_AND_MINING.md`.
- Žiadne automatické mouse aim, rušenie zotrvačnosti po pustení vstupu, okamžitý Full Stop ani mazanie horniny okolo celého trupu.
- Asteroidy sa tunelmi nerozpadajú. Pohyb asteroidu príde až po prijatom statickom prototype.
- Zdroje lode, ekonomika, príbeh, gravitácia a assisted flight nepatria do prvého letového kroku.
- Historické Godot repo je iba referencia; jeho architektúra ani čísla nie sú záväzné pre Unity.

## Unity a kód

- Pred zásahom do Editora over správny projekt a živé spojenie. Pri viacerých Editoroch používaj presný project path.
- Scény, prefaby a serializované assety upravuj cez Editor API. Pri dostupnom Editore ich neupravuj ručne ako YAML.
- C# upravuj priamo. Zodpovednosti oddeľuj podľa skutočnej potreby; nevytváraj univerzálny engine ani veľkú architektúru vopred.
- Parametre na ladenie sprístupni v Inspectore a vysvetli ich účel. Skúšobné hodnoty neoznačuj za finálny balance.
- Balíky spravuj cez Package Manager alebo oficiálne nástroje. Nemeň ručne manifest, verziu Editora ani pipeline ako vedľajší efekt inej úlohy.
- Assety commituj s `.meta` súbormi. `Library`, `Temp`, `Logs`, `UserSettings`, generované riešenia a buildy patria mimo Gitu.
- Vlastný kód patrí pod `Assets/AstroMiner/`; editorový kód zostáva oddelený od runtime. Testovacie assemblies vzniknú až pri prvom relevantnom hernom kroku.

## Overenie a dokončenie

- Použi primerané overenie podľa `Docs/development/TESTING.md`. Testuj dohodnuté správanie a hranice, nie iba detaily implementácie.
- Po C# zmene over kompiláciu. Viditeľné správanie over v Play Mode; kompilácia nepotvrdzuje let ani kolízie. Build over, keď je relevantný k zadaniu.
- Pri nedostupnom overení zapíš `nevykonané` a dôvod. Očakávaný výsledok nikdy neoznačuj ako úspešný test.
- V tej istej zmene aktualizuj dotknutý dokument systému, roadmapu, projektový kontext, rozhodnutia a tento súbor, iba ak sa ich obsah mení. Podstatné poznatky zachovaj stručne v `Docs/development/notes/`.
- Dokončený krok zhrň podľa [CHECKPOINT_TEMPLATE.md](Docs/development/CHECKPOINT_TEMPLATE.md): výsledok, dôkazy, obmedzenia a Adamovo prijatie. Chat nesmie byť jediným nositeľom dôležitého rozhodnutia.
- Rozpracované zmeny patria do `Unreleased`. Verziu a Discord vydanie priprav podľa `Docs/RELEASING.md`; lokálny test, pracovný commit v main ani otvorený PR samy neznamenajú prijaté vydanie.
- Webhook je výhradne v GitHub secret `DISCORD_CHANGELOG_WEBHOOK_URL`. Nikdy nekopíruj jeho hodnotu ani iné heslá/tokeny do súborov, príkazových výpisov alebo dokumentácie.
