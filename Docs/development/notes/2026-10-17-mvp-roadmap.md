# Návrh 0.1 MVP a spresnenie workflow — 17. 10. 2026

## Zadanie a výsledok

Adam chce začať roadmapu k 0.1 MVP s loďou, ovládaním, asteroidom a miningom. Zároveň povoľuje úvodnú prácu priamo v hlavnej vetve a vetvy chce používať pre zásahy do citlivého letu/vrtáka po ich prijatí.

- Hlavná vetva je `main`, nie `master`; nič sa nepremenováva.
- Dokumentačný základ `3d55791` bol bez prepisu histórie prevzatý do lokálneho main. Ďalšia dokumentačná zmena pokračuje tam podľa nového zadania.
- Vznikol návrh etáp MVP-00 až MVP-10 po spresnení rozsahu. Implementácia nebola spustená.
- D-008 nahrádza pôvodný plošný režim vetva + PR. Prijatie sa eviduje pre let a vrták samostatne; integračný zásah do už prijatého letu vyžaduje vetvu aj pri prvom vrtáku.
- D-009 zachováva pôvodný cieľ, D-010 jeho Adamovo spresnenie: 0.1 obsahuje celý cyklus let, mining, zber a odovzdávanie s Hull, Fuel, Power, Heat a Cargo. O-006 je vyriešené; číselné parametre, damage a recovery ešte nie sú uzavreté.
- Drift a rotácia sú odporúčané po 0.1, nie zrušené. Hranica upgradov, spracovania rudy a perzistentných saves ostáva otvorená; podrobné systémové pravidlá sa dohodnú po krokoch.

## Dôkazy a hranice

Unity editor_status potvrdil spojenie so správnym projektom `C:\Development\Astro Miner Unity`, verziu 6000.6.2f1, stav ready, bez prebiehajúcej kompilácie a so zastaveným Play Mode. Tento krok nespúšťal Play Mode ani build a nemenil kód, assety alebo balíky.

Finálna kontrola po D-010 prešla: 80 relatívnych odkazov v 16 Markdown súboroch, UTF-8 a kontrola diffu; bez webhook URL. Existujúca Node sada prešla 6/6 lokálne aj v GitHub Actions pri predchádzajúcom dokumentačnom commite; jej kód sa nemenil. Vlastné Unity testy stále neexistujú. Zmeny zostávajú pod Unreleased; nejde o vydanie MVP ani nové Discord oznámenie.

## Ďalší krok

Dohodnúť malý plán MVP-01: vstupy, Full Stop, skúšobné parametre, členenie kódu a prvé zmysluplné testy. Roadmapa neudeľuje pokyn implementovať všetky etapy naraz.
