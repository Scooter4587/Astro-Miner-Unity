# Dokumentačný základ — 17. 10. 2026

Stav: dokumentačný krok, Unreleased; pripravený na Adamovu kontrolu. Herné mechaniky sa nemenia.

## Rozsah a zdroje

Adam požiadal o trvalý kontext hry v repozitári, povinné čítanie dokumentácie v AGENTS.md a základ pre budúce testovanie. Podkladom bol existujúci Unity setup, dodaný handoff, pitch, staršia konverzácia a pracovný postup Evolution Game. Godot ani Evolution Game sa nemenili.

Vznikol index dokumentácie, koncept, kontrakt letu a vŕtania, technický kontext, rozhodnutia, workflow, testovací postup a šablóna ďalších poznatkov. Roadmapa oddeľuje hotový setup od ešte nezačatého herného vývoja.

## Rozhodnutia a poznatky

- Adam výslovne schválil režim vetva + PR + vlastné odskúšanie a schválenie pred merge: D-006.
- Koncept odlišuje budúce zdroje a recovery od jadra. Núdzový návrat, WASD aj technika horniny zostávajú otvorené.
- Unity Test Framework je nainštalovaný, vlastné herné testy nie sú vytvorené. Discord sada má 6 testov a jej úspech nedokazuje let ani Unity build.
- Samotný AGENTS.md nestačí na udržanie kontextu; pravidlá majú vlastnícky dokument a aktualizujú sa spolu s implementáciou.

## Overenie a hranice

Technické tvrdenia boli porovnané s verziou Editora, manifestom/lockom balíkov, nastaveniami vstupu, URP, scén a existujúcim kódom na základe `28c9b35`.

Prešlo: 70 relatívnych odkazov v 15 Markdown súboroch, kontrola UTF-8 a diffu a existujúca Node sada 6/6. Kontrola dokumentácie nenašla webhook URL. Play Mode a Player build sa v tomto dokumentačnom kroku nevykonávajú; assety, balíky a projektové nastavenia sa nemenia.

Adamovo prijatie dokumentačného výsledku: čaká. Schválenie pracovného režimu nie je automatické schválenie všetkého textu alebo merge.

Ďalší herný krok: spoločne uzavrieť malý rozsah letovej skúšobne a otázky O-001/O-002. Implementácia letu ešte nebola zadaná.

## Následné spresnenie pracovného režimu

Neskoršie zadanie z 17. 10. 2026 povoľuje úvodný prototyp priamo v main a presúva vetvy/PR na zásahy do prijatého jadra. D-008 nahrádza Git časť D-006. Dokumentačný základ bol prevzatý do main podľa tohto zadania; vyššie uvedené čakanie na PR opisuje pôvodný stav. Roadmapa 0.1 je nový pracovný návrh, nie schválená implementácia celého MVP. Pozri [záznam plánovania](2026-10-17-mvp-roadmap.md).
