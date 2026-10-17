# Let a vŕtanie — dohodnuté jadro

Stav: prvý let je implementovaný; technické dôkazy vlastní [záznam kroku](../development/notes/2026-10-17-first-flight.md). Adam 17. 10. 2026 prijal pocit z letu F-05; základ je commit `8f135f934b25ec3092d805337f1929a2571c2ebe`. MVP-03B teraz implementuje mining vo vlastnej vetve podľa [LASER_MINING.md](LASER_MINING.md); jeho overenie a Adamovo prijatie sú samostatné. Kontext dáva [Herný koncept](GAME_CONCEPT.md).

## Let

- Klávesnica zadáva požadovaný smer. RCS otočí loď a hlavný motor zaberie až po dostatočnom zarovnaní.
- Vypnutie motorov alebo pustenie vstupu neruší lineárnu zotrvačnosť. Otočenie nosa samo nepresmeruje existujúcu rýchlosť.
- Ťah mení rýchlosť postupne, nie okamžitým nastavením smeru velocity.
- `C` spustí Full Stop: otočenie proti aktuálnemu pohybu a postupné zabrzdenie. Rotačné tlmenie nesmie skryto brzdiť lineárny pohyb.
- Myš zatiaľ nesmeruje loď. Assisted/arcade režim je neskoršia možnosť, nie povinná súčasť hry.

### Prvý letový krok — D-012

Adam 17. 10. 2026 potvrdil WASD v súradniciach obrazovky/sveta: W hore (+Y), S dole, A doľava, D doprava. Dve klávesy tvoria normalizovaný diagonálny smer; protichodné klávesy sa vyrušia. Nejde o relatívne ovládanie podľa nosa.

C sa aktivuje jedným stlačením a brzdenie pokračuje po pustení C. Nový nenulový smer WASD Full Stop preruší. Smer už držaný v okamihu C brzdenie hneď nezruší; treba smer zmeniť alebo pustiť a znovu stlačiť. Opakované C brzdenie obnoví, nie prepne na drift. C pri nulovej rýchlosti neotáča loď. Zvyšok rýchlosti do 0.02 jednotky/s sa dočistí silou najviac jedného bežného kroku ťahu; nejde o okamžité vynulovanie ľubovoľnej rýchlosti. Pustenie vstupu stabilizuje iba otáčanie, nie lineárny pohyb.

Kamera sleduje interpolovanú polohu lode v LateUpdate, zachová Z=-10, neotáča sa a nepridáva look-ahead ani oneskorenie. Loď zostáva v strede; pohyb ukazuje svet a parallax. Šípky, Shift a Home už kameru v FlightTest neovládajú. Nový pokus je Stop Play a znovu Play.

Technika: Rigidbody2D Dynamic, mass 1, gravity scale 0, linear/angular damping 0, interpolation Interpolate, continuous collision detection. Ťah používa AddForce a RCS AddTorque v FixedUpdate. Rotačný regulátor je obmedzený, nemení lineárnu velocity. CapsuleCollider2D je dočasný obal 1.4 x 2.8 s offsetom Y=0.2; priechodnosť tunela sa týmto krokom neoveruje.

Skúšobné parametre v Ship Flight Controller: Main Acceleration 4 jednotky/s², Max Turn Speed 160°/s, Turn Acceleration 600°/s², Alignment Tolerance 5°, Stop Epsilon 0.02 jednotky/s. Bez herného obmedzovača rýchlosti. Nejde o finálny balance ani o čísla zo starého Godotu. Trysky sú jednoduchá vizuálna odozva skutočného povelu; vrták zostáva neaktívny.

[Sprievodca prvým letom](../guides/FIRST_FLIGHT.md) opisuje ovládanie, Inspector aj manuálny ekvivalent pridania komponentov.

## Drill riadenie — MVP-03A

D-015 pridáva samostatný R režim, RCS posuv/otáčanie a C bez retrográdneho otočenia. Implementované správanie, profil a akceptačné scenáre vlastní [DRILL_MODE.md](DRILL_MODE.md). Samotné odstraňovanie horniny ani získavanie surovín ešte neexistujú. Adam pilotovanie MVP-03A prijal na commite 3265d33. Skutočný mining MVP-03B má vlastný [kontrakt](LASER_MINING.md) a čaká na svoje overenie a prijatie.

## Vŕtanie a priestor

- Držanie `LMB` aktivuje predný vrták; smer sleduje nos lode riadenej klávesnicou.
- R prepína samostatný drill režim so stabilizáciou; LMB ovláda predný laser bez vypnutia režimu po pustení. V MVP-03A je LMB iba náhľad podľa DRILL_MODE.md.
- Pracovný profil musí vytvoriť priechod pre skutočný kolízny tvar lode aj diagonálne a v zákrute. Viditeľná diera s neviditeľnou kolíziou je chyba.
- Pracovný profil a rozumný trup majú vytvoriť chodbu umožňujúcu zastavenie a otočenie na mieste aj pri slepom konci. Komora môže chodbu rozšíriť, nemá byť povinná pri každom návrate. Samotná maximálna šírka kužeľa priechodnosť nedokazuje.
- Hornina aj jej kolízia sa odstraňujú konzistentne; tmavšia vnútorná vrstva po vyvŕtaní neblokuje loď.
- Stabilizácia sama nemaže skalu za loďou alebo mimo dosahu vrtáka. Mazanie kruhu okolo celého trupu, teleport a vypnutie kolízií nie sú schváleným riešením manévrovania.

MVP-02 zvolil vstavaný Tilemap s bunkami 0.25 podľa [ASTEROIDS.md](ASTEROIDS.md), D-014. Jeho pôvodný prefab zostáva prijatou regresnou skúškou. MVP-03A overil cúvanie, stabilizáciu a prechody v pripravenej chodbe. Aktuálny MVP-03B používa samostatné dáta po 0.125, polygonálny mesh a zhodné kolízne obrysy podľa [LASER_MINING.md](LASER_MINING.md). Mining má vlastné technické dôkazy a čakajúce Adamovo prijatie.

## Asteroid

Tunely nerozdeľujú asteroid na samostatné telesá. Prvý asteroid je statický a jeho kontakt je prijatý ako pracovný základ a zlúčený cez PR #2 podľa [kontraktu asteroidu](ASTEROIDS.md). Drift a potom rotácia prídu až po prijatom statickom prototype.

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
| M-03 | Vlastný vykopaný priechod umožní zastavenie, otočenie na mieste a výlet von; hráč ho môže rozšíriť na komoru. | Statický asteroid a vrták. |
| M-04 | Vrták odstraňuje len horninu vo svojom pracovnom profile; mimo neho sa skala nestráca. | Statický asteroid a vrták. |
| M-05 | Tunel cez asteroid nespustí fragmentáciu a odkryté vnútro neblokuje priechod. | Statický asteroid a vrták. |
| A-01 | Drift a rotácia nenarušia priechod, lokálne vŕtanie ani uvoľnenie stabilizácie. | Až pohybujúci sa asteroid. |

MVP-03A rieši scenáre D z DRILL_MODE.md na pripravenej dutine. Skutočné scenáre M rieši MVP-03B podľa [záznamu](../development/notes/2026-10-17-laser-mining.md); technické overenie a Adamovo prijatie sú oddelené. Celkové predvedenie prvého herného prototypu neskôr spojí prílet, brzdenie, zakrivený tunel, komoru, otočenie a výlet von.

## Prijatie a ochrana základu

Podľa [D-008](../DECISIONS.md) sa prvý prototyp môže vyvíjať v main. Po technickom overení a Adamovom prijatí zapíšeme overený commit a výsledky scenárov. Od tohto bodu sa každá zmena príslušného jadra robí vo vetve a PR; nový systém nesmie toto pravidlo obísť.

| Oblasť | Stav | Prijatý commit | Dôkaz |
| --- | --- | --- | --- |
| Let a ovládanie | Technicky overené a prijaté Adamom 17. 10. 2026. Odteraz zmeny cez vetvu/PR. | `8f135f934b25ec3092d805337f1929a2571c2ebe` | [Technické dôkazy a Adamova skúška](../development/notes/2026-10-17-first-flight.md); F-01 až F-04 overené, F-05 prijaté. |
| Vrták a tunely | MVP-03B implementované a technicky overené vo vetve; Adamovo prijatie čaká. | — | [Kontrakt](LASER_MINING.md) a [dôkazy](../development/notes/2026-10-17-laser-mining.md). |

Po prijatí letu môže jeho integrácia s prvým vrtákom vyžadovať vetvu ešte pred prijatím samotného vrtáka. Číselné ladenie, ak mení výsledné správanie chránenej oblasti, je tiež jej zmena.
