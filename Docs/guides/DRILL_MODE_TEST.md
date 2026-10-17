# Skúška drill ovládania a orientácia v Unity

Aktuálnu FlightTest s plným asteroidom a skutočným LMB opisuje [LASER_MINING_TEST.md](LASER_MINING_TEST.md). Nasledujúci postup zachováva pripravenú skúšku MVP-03A a jej prefaby.

Toto je MVP-03A: R režim, RCS pohyb, kontakt a náhľad lúča. Chodba je pripravená vopred. LMB ešte neodstraňuje horninu a nič nezbiera.

## Spustenie

1. Unity Hub: otvor C:\Development\Astro Miner Unity, Editor 6000.6.2f1.
2. V paneli Project otvor Assets > AstroMiner > Scenes > FlightTest.
3. Stlač Play a klikni do okna Game, aby malo fokus.
4. Loď je pod vstupom do jedného asteroidu. Svetlá šípka ukazuje pripravený vstup.
5. Stlač R a pusti všetky ovládacie klávesy aj tlačidlo myši. Pri rozbehnutej lodi najskôr prebehne brzdenie.

Oranžový prstenec znamená Stabilizing, azúrový Drill. Bez prstenca je aktívny Flight. V pokojnom stave môže byť oranžový prechod veľmi krátky.

## Ovládanie

| Režim | Ovládanie |
| --- | --- |
| Flight | Pôvodné WASD podľa obrazovky, RCS otočí loď a hlavný motor počká na zarovnanie. Pustenie vstupu zachová drift. C spustí pôvodný Full Stop. |
| Drill | W dopredu podľa nosa, S dozadu, A otočenie doľava, D doprava. Pustenie vstupu postupne zastaví. |
| Drill – C | Zastaví posuv a dobrzdí rotáciu cez RCS. Nos sa neotáča proti pohybu. Nový smerový vstup brzdenie preruší. |
| R | Prepne režim. Potom pusti WASD/R/C/LMB; podržaná klávesa sa neprenesie do nového ovládania. |
| LMB v Drill | Zobrazí obrys pracovného kužeľa. Je to náhľad: skala zostáva pevná. |

Po návrate z iného okna tiež najskôr pusti vstupy. Nový pokus je Stop a znovu Play; samotné R neresetuje polohu.

## Odporúčaná skúška

1. Zo štartu zapni R a prejdi na W pomaly do tmavej chodby. Pusti W; loď má postupne zastaviť.
2. Podrž A alebo D bez W/S. Loď sa má otáčať na mieste, bez hlavného výfuku. V strede chodby je priestor na celý obrat.
3. Skús C pri pohybe rôznym smerom. Nos nesmie dostať povel otočiť sa proti rýchlosti.
4. Horná časť chodby odbočuje doprava. Zastav, natoč sa a pokračuj do slepého konca.
5. Opri sa pomaly o stenu, pusti vstup a skús S. Loď sa má upokojiť a umožniť cúvanie.
6. Vráť sa do voľného priestoru. Rozbehni sa, prepni R späť na Flight a po uvoľnení vstupov skús let. Prepnutie nesmie vytvoriť náhly rýchlostný skok.
7. Vo Flight sa rozbehni a stlač R. Hlavný motor sa odstaví; RCS potrebuje čas aj dráhu na brzdenie.
8. Podrž LMB v Drill. Vidíš kužeľ; hornina sa nemení. Pusti LMB – drill ovládanie zostáva zapnuté.
9. Počas pohybu alebo náhľadu prepni do iného okna. Povely sa majú zrušiť; po návrate nič nezačne samo.

Pri spätnej väzbe stačí opísať konkrétny pocit: príliš pomalé otáčanie, dlhé brzdenie, prudký kontakt, nejasný prechod alebo problematické cúvanie.

## Čo pribudlo v Hierarchy

- Ship - Flight and Drill Prototype je nová prefab varianta pôvodnej lode.
- Drill Emitter označuje začiatok pracovného profilu pred nosom.
- Drill Mode Indicator kreslí stavový prstenec; Laser Working Profile je obrys náhľadu.
- Drill RCS Translation sú štyri malé výfuky pre presný posuv a brzdenie.
- Asteroid - Prepared Drill Corridor je varianta pôvodného asteroidu.
- Rock - Tilemap obsahuje zostávajúcu pevnú horninu. Interior - nonblocking prepared tunnel je tmavá podlaha bez kolízie.

Prefab varianta preberá základ z pôvodného prefabu a ukladá doplnky alebo odlišné nastavenia. Pôvodné prefaby zostávajú použiteľné na regresné skúšky.

## Debug v Scene a Game

1. V hornej lište Scene alebo Game zapni Gizmos.
2. V Hierarchy vyber loď.
3. V Inspectore nájdi a rozbaľ Ship Drill Debug. Show Zones vypína všetky jeho zóny, Show Contacts samostatne kontaktné normály.
4. Azúrový kužeľ ukazuje presný maximálny pracovný profil; žltý kruh polomer otočenia s rezervou.
5. Zelená čiara ukazuje RCS zrýchlenie a červené čiary pri kontakte smer od steny.
6. Vybratý Capsule Collider 2D ukáže skutočný fyzický obal.

Kužeľ zatiaľ nevyhodnocuje tienenie horninou. Žltý kruh nie je ďalšia kolízia ani plocha automatického mazania skaly.

## Kde sa ladia čísla

Project > Assets > AstroMiner > Settings > DrillModePrototype.

Je to ScriptableObject: uložený nastavovací asset, ktorý môže používať prefab. V Inspectore nájdeš rýchlosť, zrýchlenie, otáčanie, brzdenie a kužeľ. Ich význam a východiskové hodnoty vlastní [kontrakt režimu](../design/DRILL_MODE.md).

Na prvé pocitové ladenie dávajú zmysel Move Speed, Acceleration, Turn Speed a Turn Acceleration. Beam Range, Beam Near Width a Beam Half Angle dnes menia iba náhľad. Zmeny profilu ovplyvnia jeho všetkých používateľov. Zmeny ScriptableObject assetu počas Play môžu zostať uložené aj po Stop; bezpečnejšie je upravovať ich pri zastavenej hre a vedome kontrolovať Git diff.

## Ako vznikla zostava a ručný ekvivalent

Zostavu pripravil editorový nástroj cez Unity Editor API. Nevznikla ručným písaním scénového YAML. Jeho menu je Astro Miner > Setup > Add Drill Mode Test; po vytvorení je deaktivované, aby neprepísalo assety.

Ručný ekvivalent práce by bol:

1. Vytvoriť profil cez Create > Astro Miner > Drill Mode Profile.
2. Z pôvodnej lode vytvoriť prefab variantu, pridať ShipDrillController, emitter, debug a spätnú väzbu; profil a objekty priradiť v Inspectore.
3. Z asteroidu vytvoriť variantu a v jeho Tilemap vymazať skúšobnú chodbu. Pod ňu pridať tmavú Tilemap bez Collider komponentu.
4. Vložiť varianty do scény a v ShipCameraFollow priradiť novú loď.
5. Uložiť prefaby aj scénu a spustiť Play.

Prvý krok ešte nevytvoril runtime asteroidové dáta, mining, suroviny, saves, hladké steny, damage ani upgradovanie.
