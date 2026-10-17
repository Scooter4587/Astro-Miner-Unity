# Drill režim a pripravená chodba — 17. 10. 2026

Stav: implementované a technicky overené vo feature/drill-mode, Unreleased; Adam po Play teste prijal pilotovanie na commite 3265d33c2622acf82accbc85820f146f6bd300e3. Samotné kopanie toto prijatie nezahŕňa.

## Rozsah

Prvý krok MVP-03A: R prechody, RCS posuv a otáčanie, C bez retrográdneho otočenia, LMB náhľad, debug a jedna pripravená chodba. Pôvodné prijaté prefaby sa zachovávajú ako základ nových variant.

Skutočný mining, runtime stav asteroidu, odolnosť, suroviny, hladké steny, saves a Player build do tohto kroku nepatria. Pripravená dutina nie je vydávaná za výsledok vŕtania.

## Výsledok

- ShipDrillController vlastní režim a používa obmedzené AddForce/AddTorque; Flight controller má explicitné pozastavenie riadenia.
- ShipKeyboardInput smeruje WASD/C podľa režimu, vyžaduje neutrál po R/fokuse a LMB rieši nezávisle od pohybu.
- DrillModePrototype je priradený profil; [kontrakt](../../design/DRILL_MODE.md) obsahuje význam a východiskové hodnoty.
- Nové ShipDrillPrototype a DrillTestAsteroid sú prefab varianty. FlightTest štartuje pri (18,4) pred vopred pripravenou L chodbou. Pevná Tilemap má 3124 buniek, neblokujúce vnútro 1393; polomer otočenia aktuálneho trupu je 1.4000001 jednotky.
- Nepribudol balík, runtime asteroidový framework, zápis velocity alebo vypínanie kolízií.

## Poznatky počas overovania

- Edit-mode polomer otočenia musí vychádzať z transformovaného lokálneho stredu hmotnosti. Physics worldCenterOfMass môže po editorovom presune zostať nezosynchronizovaný. Výpočet je opravený a nepredpokladá pivot v strede Transformu.
- Unity môže doručiť focus callback aj vypnutému vstupnému komponentu. Vypnutý adaptér nesmie rušiť povely iného aktívneho riadenia; callback teraz rešpektuje isActiveAndEnabled.
- Priame syntetické klávesy bez izolácie záviseli od smerovania vstupu v Editore. Testy používajú oficiálny InputTestFixture, ktorý obnovuje pôvodný Input System stav po skúške; herné pravidlá sa kvôli testu neobchádzajú.
- GetUsedTilesCount počíta typy použitých Tile assetov, nie obsadené bunky. Dôkaz nemenného asteroidu v teste náhľadu preto porovnáva skutočnú obsadenosť.
- Agregované behy po opakovanom načítaní skriptov vracali 0 vykonaných prípadov; neboli započítané ako úspešné. RequestScriptReload obnovil stav nástroja a nový beh skutočne vykonal všetkých 17 prípadov.
- Jeden nový obsolete warning FindFirstObjectByType v authoring nástroji bol opravený na FindAnyObjectByType.

## Overenie

| Kontrola | Skutočný výsledok |
| --- | --- |
| Nové drill scenáre | 7/7 prešlo, 38.37 s; vstup pri rýchlosti (3,-4) a rotácii 90 °/s, pohyb/cúvanie/C, celý obrat v reálnej chodbe, skutočný kontakt slepého konca a odchod, návrat do letu, klávesy R/C a fokus/LMB bez mazania buniek. |
| Celá PlayMode sada | 17/17 prešlo, 61.00 s: 7 drill, 6 pôvodných letových a 4 pôvodné asteroidové scenáre. Spustené bez filtra po obnovení domény Editora. |
| EditMode | 6/6 prešlo, 0.03 s; existujúce parallax scenáre. Celkom 23 Unity prípadov v tomto stave. |
| Živá FlightTest a obraz | Cez verejné controller povely iba v Play: loď vošla do chodby a zastavila pri (18,12.4553719), velocity (0,0), angular velocity 0, Mode Drill, náhľad zapnutý, hlavný motor vypnutý, kamera bez odchýlky. Game záber bol zachytený a prezretý. Ide o scénovú skúšku cez API; Adamov pocit z klávesnice tým nenahrádzame. |
| Dokumentácia, meta a diff | 55 unikátnych GUID, bez chýbajúcich meta; odkazy overené, žiadne zmeny pôvodných prefabov, Packages ani ProjectSettings. C#/Markdown/asmdef bez whitespace chýb; Unity generované assety ponechávajú štandardné prázdne serializované polia. |
| Adamov test | Prijaté 17. 10. 2026: potvrdil, že pilotovanie funguje a dáva zmysel. Upozornil, že reálna skúška miningu príde až pri skutočnom kopaní. |
| Player build | Nevykonané; tento krok je skúška ovládania v Editore. |

Po skúškach je uložená čistá zastavená FlightTest: loď (18,4), Mode Flight, vstup zapnutý, 0 missing scripts a 0 aktuálnych Console errors/warnings. Neuložili sa runtime presuny ani testovacie povely.

D-01 až D-06 vlastní kontrakt. Prijaté letové scenáre F a statický kontakt S sa majú znovu overiť; miningové M scenáre zostávajú budúce.

## Odovzdanie

Postup, nové Unity objekty, Inspector a ladenie vlastní [sprievodca](../../guides/DRILL_MODE_TEST.md).

Zmena je v PR #3 a Adam pilotovanie prijal. Nový mining krok pokračuje oddelene z tohto základu. Samostatné zlúčenie, nová verzia a Discord vydanie týmto záznamom nie sú tvrdené.
