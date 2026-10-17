# Roadmapa k 0.1 MVP

Rozsah 0.1 spresnil Adam 17. 10. 2026: let, mining, zber, odovzdávanie a jednoduché funkčné systémy lode — Hull, Fuel, Power, Heat a Cargo. Rozdelenie nižšie je pracovný plán; konkrétne pravidlá a parametre sa dohodnú pred príslušnou implementáciou. Roadmapa nie je zadanie postaviť celú hru naraz.

Dohodnuté jadro vlastní [FLIGHT_AND_MINING.md](design/FLIGHT_AND_MINING.md), skutočný stav [PROJECT_CONTEXT.md](PROJECT_CONTEXT.md), rozhodnutia [DECISIONS.md](DECISIONS.md). Rozcestník: [Docs/README.md](README.md).

## Čo má 0.1 dokázať

Hráč ovláda jednu loď so zotrvačnosťou, priletí k statickému asteroidu, postupne zabrzdí, predným vrtákom vyrazí priechodný tunel, nazbiera materiál, vytvorí komoru, otočí sa a vráti sa odovzdať náklad pri jednoduchej základni. Výpravu možno zopakovať.

Loď pritom používa skutočné zdroje: pohon míňa palivo, reaktor obnovuje energiu, činnosť produkuje teplo, cargo má dohodnutú kapacitu a poškodenie znižuje hull. Vyčerpanie alebo prehriatie má dohodnutý funkčný následok; HUD nie je iba dekorácia.

Ide o malú kompletnú mining slučku s jednoduchými systémami, nie iba technickú ukážku letu a mazania horniny. Presné počty materiálov, kapacity, spotreby a prahy ešte nie sú uzamknuté.

Cieľová podoba: hrateľná Windows verzia s placeholder grafikou. `0.1.0` je výsledný míľnik; priebežné prijaté kroky môžu dostať verzie `0.0.x` podľa [RELEASING.md](RELEASING.md).

## Etapy a overenie

| Krok | Výsledok | Kritérium prijatia | Stav |
| --- | --- | --- | --- |
| MVP-00 — prostredie a kontext | Unity, VS Code, Git, dokumentácia a Discord pripravené. | Doložený setup, správny projekt, živé spojenie s Editorom a odoslané 0.0.1. | Technický základ overený. |
| MVP-01 — letová skúšobňa | Jedna loď, orientačné body, kamera, smer z klávesnice, RCS, motor po zarovnaní a Full Stop na C. | F-01 až F-04 overené testami/Play Mode, F-05 Adamov pocit z letu. Potom záznam prijatého commitu letu. | Najbližší krok na detailnú dohodu, nezačaté. |
| MVP-02 — statický asteroid | Jeden pripravený asteroid, čitateľný povrch a správny kontakt; ešte bez damage modelu. | Loď neprechádza neodstránenou horninou a kontakt nenaruší prijatý let mimo asteroidu. | Nezačaté, po prijatí letu. |
| MVP-03 — vrták a tunely | LMB, predný profil, odstránenie horniny aj kolízie, neblokujúce vnútro a kontrolovaný postup. | M-01 až M-05: rovný/diagonálny tunel, zákruta, odbočka, komora, otočenie, výlet von; bez mazania mimo vrtáka a fragmentácie. Potom záznam prijatého commitu vrtáka. | Nezačaté, po technickom pláne horniny. |
| MVP-04 — zber a Cargo | Materiál z ťažby sa zbiera do nákladu s jednoduchou kapacitou a viditeľným stavom. | Bez duplicitného zisku alebo straty; plné cargo má dohodnuté správanie. Počet typov materiálu a model kapacity dohodneme pred kódom. | Nezačaté. |
| MVP-05 — odovzdanie | Jednoduchá základňa prijme náklad a ukáže výsledok výpravy. | Korektný presun materiálu, bez opakovaného započítania; výpravu možno zopakovať. | Nezačaté. |
| MVP-06 — Power | Reaktor priebežne obnovuje energiu, činnosti ju spotrebúvajú a nedostatok má reálny následok. | Kapacita, obnova, odber, dočasný výpadok a opätovné zapojenie podľa dohodnutých pravidiel. | Nezačaté. |
| MVP-07 — Heat | Činnosť zohrieva loď a chladenie ju ochladzuje; prehriatie dočasne odstaví dohodnuté systémy. | Prahy, odstavenie a obnova fungujú bez kmitania stavov; vypnutie pohonu magicky neruší zotrvačnosť. | Nezačaté. |
| MVP-08 — Fuel | Palivo obmedzuje pohon a dosah. | Spotreba pohonov a správanie pri vyčerpaní podľa dohody; spôsob doplnenia a ďalšieho pokusu sa určí pred implementáciou. | Nezačaté. |
| MVP-09 — Hull a zlyhanie | Poškodenie má skutočný následok, loď sa dá zničiť a hráč má dohodnutý ďalší pokus/recovery. | Overený zdroj damage, zničenie a strata/reset podľa dohody; žiadny vymyslený bezplatný návrat alebo permadeath. | Nezačaté. |
| MVP-10 — celá výprava a Windows build | Základná spätná väzba, čitateľné stavy systémov, opakovateľný herný cyklus a samostatná verzia. | Celá cesta a hraničné situácie fungujú mimo Editora, relevantné testy prejdú, Adam prijme výsledok a obmedzenia; potom 0.1.0 a Discord. | Nezačaté. |

Každá etapa sa môže rozdeliť na menšie úlohy; ich poradie môžeme spresniť podľa skutočných závislostí. Dopĺňanie zdrojov, repair a recovery sa majú vyriešiť spolu s ich systémom, nie tichým predpokladom až na konci.

## Ochrana jadra

Úvodný prototyp možno robiť priamo v main. Let a vrták chránime každý od jeho vlastného technického overenia a Adamovho prijatia; zapíšeme overený commit do [kontraktu](design/FLIGHT_AND_MINING.md).

Od tohto bodu ide každá zmena chránenej oblasti cez vetvu a PR. Platí to aj pre integráciu nového systému: vrták, spotreba paliva, výpadok energie či prehriatie môžu meniť už prijatý let. Tieto zásahy preto nemusia zostať v main iba preto, že 0.1 ešte nie je dokončená.

## Najbližší samostatný krok

Dohodnúť a pripraviť iba MVP-01:

- smerové klávesy a význam smeru v priestore;
- prerušenie Full Stop a C pri takmer nulovej rýchlosti;
- skúšobné parametre RCS, ťahu, zarovnania a brzdenia;
- najmenšie členenie kódu a prvé zmysluplné Unity testy.

Rigidbody2D je kandidát na vysvetlený technický návrh. Tento dokumentačný krok nevytvára loď, scénu ani herný kód. Systémy lode patria do cieľa 0.1, ale nerozširujú prvú letovú úlohu.

## Ešte otvorené hranice

- Presné pravidlá odberov, priorít, kapacít, teplotných prahov a návratu do prevádzky.
- Opravy, doplnenie paliva, zničenie, recovery a prípadné straty; treba ich dohodnúť pred príslušnou etapou.
- Či 0.1 zahŕňa aj upgrady, jednoduché spracovanie rudy na part alebo perzistentný save. Zatiaľ nie sú automatickou súčasťou schváleného rozsahu ship/mining/systems.

Drift a rotáciu asteroidov odporúčame nechať po 0.1, po prijatom statickom miningu. Gravitácia/singularita, assisted flight, multiplayer, väčší príbeh a finálna grafika sú ďalej mimo najbližších implementačných krokov.

## Kedy je 0.1 hotová

- Celý dohodnutý cyklus let → mining → zber → odovzdanie je implementovaný a opakovateľný.
- Hull, Fuel, Power, Heat a Cargo menia skutočné správanie lode; dohodnuté nedostatky, výpadky, zničenie a ďalší pokus boli overené.
- Let a vrták majú záznam prijatia a relevantné regresné scenáre. Rozšírené systémové scenáre sa doplnia spolu s konkrétnymi pravidlami do [TESTING.md](development/TESTING.md).
- Windows build prejde dohodnutú cestu aj hraničné stavy bez ručného opravovania scény. Adam prijme výsledok a známe obmedzenia.
- Dokumenty zodpovedajú skutočnosti, 0.1.0 je v changelogu a doručenie Discord oznámenia je overené.

Automatický Unity build na GitHube nie je podmienkou prvého lokálneho MVP. Reálne lokálne testy a Player build podmienkou prijatia sú.
