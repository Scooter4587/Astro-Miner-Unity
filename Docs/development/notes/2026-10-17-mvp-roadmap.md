# Návrh 0.1 MVP a spresnenie workflow — 17. 10. 2026

## Zadanie a výsledok

Adam chce začať roadmapu k 0.1 MVP s loďou, ovládaním, asteroidom a miningom. Zároveň povoľuje úvodnú prácu priamo v hlavnej vetve a vetvy chce používať pre zásahy do citlivého letu/vrtáka po ich prijatí.

- Hlavná vetva je `main`, nie `master`; nič sa nepremenováva.
- Dokumentačný základ `3d55791` bol bez prepisu histórie prevzatý do lokálneho main. Ďalšia dokumentačná zmena pokračuje tam podľa nového zadania.
- Vznikol návrh etáp MVP-00 až MVP-05. Implementácia nebola spustená.
- D-008 nahrádza pôvodný plošný režim vetva + PR. Prijatie sa eviduje pre let a vrták samostatne; integračný zásah do už prijatého letu vyžaduje vetvu aj pri prvom vrtáku.
- D-009 zachováva vysokú úroveň cieľa. O-006 ponecháva otvorenú hranicu jednej suroviny a odovzdania pri základni. Tento malý herný cyklus odporúča agent, Adam ho zatiaľ nevybral.
- Drift a rotácia sú v návrhu po 0.1, nie zrušené. Presný rozsah roadmapy ešte čaká na dohodu.

## Dôkazy a hranice

Unity editor_status potvrdil spojenie so správnym projektom `C:\Development\Astro Miner Unity`, verziu 6000.6.2f1, stav ready, bez prebiehajúcej kompilácie a so zastaveným Play Mode. Tento krok nespúšťal Play Mode ani build a nemenil kód, assety alebo balíky.

Prešlo: 77 relatívnych odkazov v 16 Markdown súboroch, UTF-8 a kontrola diffu, existujúca Node sada 6/6. Dokumentácia neobsahuje webhook URL. Vlastné Unity testy stále neexistujú. Zmeny zostávajú pod Unreleased; nejde o vydanie MVP ani nové Discord oznámenie.

## Ďalší krok

Dohodnúť hranicu 0.1 a potom malý plán MVP-01: vstupy, Full Stop, skúšobné parametre, členenie kódu a prvé zmysluplné testy. Roadmapa neudeľuje pokyn implementovať všetky etapy naraz.
