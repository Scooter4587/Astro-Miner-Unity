# Let a vŕtanie — dohodnuté jadro

Stav: správanie dohodnuté v koncepte, zatiaľ bez hernej implementácie. Číselné parametre a technické voľby nižšie zostávajú otvorené. Kontext dáva [Herný koncept](GAME_CONCEPT.md).

## Let

- Klávesnica zadáva požadovaný smer. RCS otočí loď a hlavný motor zaberie až po dostatočnom zarovnaní.
- Vypnutie motorov alebo pustenie vstupu neruší lineárnu zotrvačnosť. Otočenie nosa samo nepresmeruje existujúcu rýchlosť.
- Ťah mení rýchlosť postupne, nie okamžitým nastavením smeru velocity.
- `C` spustí Full Stop: otočenie proti aktuálnemu pohybu a postupné zabrzdenie. Rotačné tlmenie nesmie skryto brzdiť lineárny pohyb.
- Myš zatiaľ nesmeruje loď. Assisted/arcade režim je neskoršia možnosť, nie povinná súčasť hry.

Pred implementáciou letového kroku dohodneme smerové klávesy a ich súradnicový význam, prerušenie Full Stop a správanie pri takmer nulovej rýchlosti. WASD je návrh. Zrýchlenie, RCS, tolerancia zarovnania, brzdenie a prípadný rýchlostný limit budú označené skúšobné parametre, nie hodnoty prevzaté zo starého Godotu.

## Vŕtanie a priestor

- Držanie `LMB` aktivuje predný vrták; smer sleduje nos lode riadenej klávesnicou.
- Režim vŕtania má umožniť pomalý kontrolovaný postup a manévrovanie so stabilizáciou. Konkrétne pravidlá zapojenia a uvoľnenia ešte treba overiť.
- Pracovný profil musí vytvoriť priechod pre skutočný kolízny tvar lode aj diagonálne a v zákrute. Viditeľná diera s neviditeľnou kolíziou je chyba.
- Chodba širšia než trup nemusí stačiť na otočenie dlhej lode. Hráč musí vedieť vyvŕtať väčšiu komoru a otočiť sa v nej.
- Hornina aj jej kolízia sa odstraňujú konzistentne; tmavšia vnútorná vrstva po vyvŕtaní neblokuje loď.
- Stabilizácia sama nemaže skalu za loďou alebo mimo dosahu vrtáka. Mazanie kruhu okolo celého trupu, teleport a vypnutie kolízií nie sú schváleným riešením manévrovania.

Pred týmto krokom vyberieme a vysvetlíme reprezentáciu horniny, aktualizáciu kolízií, profil vrtáka, cúvanie, rozširovanie komory a prechod po pustení LMB. Bunkový model, Tilemap a vlastná geometria sú kandidáti; žiadny ešte nie je uzavretá architektúra.

## Asteroid

Tunely nerozdeľujú asteroid na samostatné telesá. Prvý asteroid je statický. Drift a potom rotácia prídu až po prijatom statickom prototype.

Budúci pohyb vyžaduje materiál v lokálnych súradniciach asteroidu a overenie relatívneho pohybu lode v tuneli. Samotné priradenie lode pod transform asteroidu správnu fyziku nepotvrdzuje.

## Akceptačné scenáre

Tieto scenáre sú očakávania pre budúce kroky, nie tvrdenie o úspešných testoch. Postup overenia a výsledok zapíšeme podľa [TESTING.md](../development/TESTING.md).

| ID | Očakávané správanie | Kedy |
| --- | --- | --- |
| F-01 | Po rozbehu a pustení vstupu zostáva bez ďalších síl zachovaný vektor rýchlosti v dohodnutej numerickej tolerancii. | Letový krok. |
| F-02 | Samotné otočenie nosa okamžite nepresmeruje pohyb lode. | Letový krok. |
| F-03 | Hlavný motor čaká na zarovnanie; jeho ťah potom mení rýchlosť postupne. | Letový krok. |
| F-04 | C otočí loď proti pohybu a zabrzdí postupne, bez okamžitého zastavenia a trvalého kmitania. | Letový krok. |
| F-05 | Pocit z orientácie, RCS, kamery a brzdenia odskúša Adam v reálnom Play Mode. | Prijatie letu. |
| M-01 | Rovný aj diagonálny tunel zodpovedajú kolízii a loď nimi prejde. | Statický asteroid a vrták. |
| M-02 | Loď vyrazí zákrutu a odbočku bez opakovaného odrážania alebo zasekávania na rohoch. | Statický asteroid a vrták. |
| M-03 | Vŕtaním a manévrovaním vytvorí komoru, otočí sa a vyletí von. | Statický asteroid a vrták. |
| M-04 | Vrták odstraňuje len horninu vo svojom pracovnom profile; mimo neho sa skala nestráca. | Statický asteroid a vrták. |
| M-05 | Tunel cez asteroid nespustí fragmentáciu a odkryté vnútro neblokuje priechod. | Statický asteroid a vrták. |
| A-01 | Drift a rotácia nenarušia priechod, lokálne vŕtanie ani uvoľnenie stabilizácie. | Až pohybujúci sa asteroid. |

Najbližší krok rieši iba scenáre F. Celkové predvedenie prvého herného prototypu neskôr spojí prílet, brzdenie, zakrivený tunel, komoru, otočenie a výlet von.

## Prijatie a ochrana základu

Podľa [D-008](../DECISIONS.md) sa prvý prototyp môže vyvíjať v main. Po technickom overení a Adamovom prijatí zapíšeme overený commit a výsledky scenárov. Od tohto bodu sa každá zmena príslušného jadra robí vo vetve a PR; nový systém nesmie toto pravidlo obísť.

| Oblasť | Stav | Prijatý commit | Dôkaz |
| --- | --- | --- | --- |
| Let a ovládanie | Zatiaľ neimplementované a neprijaté. | — | F-01 až F-05 čakajú. |
| Vrták a tunely | Zatiaľ neimplementované a neprijaté. | — | M-01 až M-05 čakajú. |

Po prijatí letu môže jeho integrácia s prvým vrtákom vyžadovať vetvu ešte pred prijatím samotného vrtáka. Číselné ladenie, ak mení výsledné správanie chránenej oblasti, je tiež jej zmena.
