# Drill režim — MVP-03A

Stav: implementácia vo vetve feature/drill-mode; technické výsledky vlastní [záznam kroku](../development/notes/2026-10-17-drill-mode.md). Adam po Play teste prijal pilotovanie na commite 3265d33; PR #3 je samostatný krok. Aktuálne rozšírenie o skutočné kopanie vlastní [LASER_MINING.md](LASER_MINING.md). Tento krok pripravuje riadenie a statickú skúšobnú chodbu; neimplementuje ťažbu.

## Dohodnuté správanie — D-015

- R prepína medzi letom a drill režimom. Tá istá loď zostáva Dynamic Rigidbody2D so zapnutými kolíziami.
- Pri zapnutí sa hlavný motor okamžite odstaví. Režim Stabilizing pomocou obmedzených RCS síl dobrzdí posuv aj otáčanie. Nemení okamžite velocity ani požadovaný smer nosa.
- Po dosiahnutí pracovnej rýchlosti nastúpi Drill. W/S znamená posun dopredu/dozadu podľa nosa; A/D otáčanie. Nie sú to svetové smery prijatého letu.
- Bez smerového vstupu RCS plynulo brzdí posuv aj rotáciu. Toto je explicitné správanie drill režimu; voľný let naďalej zachováva lineárny drift.
- C zruší povel na posuv aj otáčanie a začne RCS brzdenie bez retrográdneho otočenia. Už prebiehajúce otáčanie sa dobrzdí, nezamkne sa okamžite uhol.
- Po C musí vzniknúť nový nenulový smerový vstup, aby sa pohyb obnovil. Už držané W C okamžite nezruší.
- Po R treba pustiť WASD, R, C aj LMB; až neutrál uvoľní vstup v novom režime. Zabránime neúmyselnému preloženiu podržaného letového povelu.
- R je možné použiť aj počas driftu, rotácie, kontaktu alebo v prázdnom priestore. Vysoká rýchlosť potrebuje skutočnú brzdnú dráhu.
- Odchod do letu zachová aktuálnu rýchlosť. Hlavný motor čaká na nový vstup a bežné zarovnanie podľa [letu](FLIGHT_AND_MINING.md).
- LMB je v tomto kroku iba náhľad pracovného profilu. Viditeľný je až v stave Drill, horninu nemení. Po pustení LMB zostáva drill režim zapnutý.
- Strata fokusu alebo vypnutie vstupného komponentu zruší ovládacie povely a náhľad. Návrat fokusu vyžaduje neutrálny vstup. Samotný výpadok vstupu vo voľnom lete nebrzdí drift.
- Automatické mouse aim, teleport, vypínanie kolízií ani mazanie okolo trupu sa nezavádzajú.

## Vlastníctvo a integrácia

ShipDrillController vlastní režim, drill povely a fyzikálny krok. Vstup číta ShipKeyboardInput; v režime Drill smeruje povely iba drill regulátoru. ShipFlightController má explicitné ControlSuspended; keď je pozastavený, nepridáva sily a odmieta letové povely. Výpočty prijatého letu sa nemenia.

DrillModeProfile je uložený ScriptableObject s autoritatívnymi skúšobnými číslami. Je priradený v ShipDrillPrototype prefabe. Runtime stav neprepisuje tento zdieľaný asset. Budúce upgrady budú vytvárať účinné parametre nad základným profilom; samotný upgrade systém ešte neexistuje.

ShipDrillFeedback zobrazuje režim, náhľad lúča a translačné RCS. Pôvodné rotačné výfuky čítajú povel práve aktívneho regulátora. ShipDrillDebug zdieľa výpočet záberu s náhľadom, aby kresba neukazovala iný kužeľ.

## Prvý skúšobný profil

Zdroj čísel: Assets/AstroMiner/Settings/DrillModePrototype.asset. Toto je čitateľný prehľad rovnakého profilu, nie druhá nezávislá konfigurácia. Čísla sú prototypové, nie finálny balance.

| Parameter | Hodnota | Význam |
| --- | --- | --- |
| Move Speed | 1.5 jednotky/s | Cieľová rýchlosť dopredu aj dozadu. |
| Acceleration | 3 jednotky/s² | Maximálna veľkosť RCS zrýchlenia vrátane brzdenia. |
| Response Time | 0.25 s | Plynulosť dobiehania cieľovej rýchlosti/otáčania. |
| Turn Speed | 60 °/s | Cieľová rýchlosť otáčania na A/D. |
| Turn Acceleration | 240 °/s² | Obmedzenie rozbehu a brzdenia otáčania. |
| Ready Speed | 0.1 jednotky/s | Prah vstupu zo Stabilizing do Drill. |
| Ready Turn Speed | 2 °/s | Súčasne požadovaný prah otáčania. |
| Contact Skin | 0.08 jednotky | Predstih pri preverovaní priestoru celou kapsulou. |
| Turn Clearance | 0.35 jednotky | Debug rezerva nad skutočným polomerom otočenia. |
| Beam Range | 2.5 jednotky | Dosah zrezaného kužeľa od emitora. |
| Beam Near Width | 3.2 jednotky | Počiatočná šírka záberu. |
| Beam Half Angle | 15 ° | Polovica uhla rozširovania kužeľa. |

Pri brzdení môže skutočná rýchlosť dočasne presiahnuť cieľovú Move Speed. Žiadny tvrdý clamp velocity nepribudol. Malé zvyšky do 0.02 jednotky/s a 0.1 °/s sa dočisťujú silou/momentom v rámci bežného limitu jedného fyzikálneho kroku.

Priechodnosť nemožno vyvodiť iba z maximálnej šírky kužeľa. Profil emitora, trup a brzdná dráha sa pri budúcom miningu overia spoločne. Aktuálny náhľad nevyhodnocuje povrch, tienenie tvrdou vrstvou, odber výkonu ani suroviny.

## Kontakt a hranice prototypu

Celá CapsuleCollider2D preveruje smer posuvu do brzdnej vzdialenosti. Obmedzuje sa požadovaná približovacia rýchlosť, nie existujúca hybnosť. Pri kontakte sa odoberá časť zrýchlenia tlačiaca do steny; kontakt obmedzí aj rotačný povel smerujúci do steny. Cúvanie a tangenciálny pohyb zostávajú povolené.

Regulátor používa AddForce/AddTorque v FixedUpdate. Nepripína loď k pôvodnej polohe. Pripravená chodba má rezervu na celé otočenie; nejde o dôkaz, že už zvládame každý budúci tvar vykopaného tunela alebo ľubovoľnú rýchlosť nárazu.

Polomer otočenia sa odvodzuje zo súčasného kapsulového obalu a skutočného stredu hmotnosti. Zmena tvaru kolízie bude vyžadovať zodpovedajúce overenie; dnešná pomocná geometria nie je univerzálny systém všetkých trupov.

## Skúšobná scéna a debug

FlightTest používa nové prefab varianty ShipDrillPrototype a DrillTestAsteroid. Pôvodné ShipPlaceholder a StaticAsteroid sa nemenia. Asteroid má predpripravenú chodbu v tvare L: os vstupu (18,5) → (18,19) → (24,19), s voľným polomerom aspoň 2.1 jednotky. Bunky zostávajú 0.25; jemný povrch sa ešte nerieši.

Loď začína pri (18,4), otočená hore. Tmavá vnútorná Tilemap je neblokujúca podlaha len v pripravenej dutine. Nie je to runtime ťažba ani nový generátor sveta.

Scene a Game majú tlačidlo Gizmos. ShipDrillDebug poskytuje Show Zones a Show Contacts. Náhľad kužeľa je azúrový, kruh potrebnej rezervy žltý, RCS zrýchlenie zelené a normály kontaktov červené. Fyzický obal zobrazí Unity pri vybraní CapsuleCollider2D. Režimový prstenec a LMB náhľad sú herná spätná väzba nezávislá od Gizmos.

Presný návod vlastní [sprievodca skúškou](../guides/DRILL_MODE_TEST.md).

## Rozšírenie MVP-03B

Aktuálna mining loď má ShipLaserMiner, vlastný profil MiningDrillModePrototype so šírkou 4.0 a skutočne odoberá horninu. Pri aktívnom laseri dopredný RCS povel preverí aj celú požadovanú šírku otočenia pred nosom. C brzdí posuv/otáčanie nezávisle od držaného LMB. Pôvodné pripravené prefaby a ich scenáre zostávajú zachované; nové správanie a hodnoty vlastní [LASER_MINING.md](LASER_MINING.md).

## Akceptačné scenáre

| ID | Očakávanie |
| --- | --- |
| D-01 | R pri rýchlom drifte/rotácii odstaví hlavný motor a brzdí obmedzeným RCS, bez okamžitého zastavenia. |
| D-02 | W/S umožnia pomalý posun aj cúvanie; pustenie vstupu a C zastavia bez presmerovania nosa. |
| D-03 | V pripravenej chodbe sa loď otočí o 360° bez dotyku stien a posunu stredu hmotnosti. |
| D-04 | Skutočný dotyk slepého konca sa ustáli a loď môže vycúvať; žiadne trvalé pinball odrazy v testovanom scenári. |
| D-05 | Návrat do letu zachová hybnosť a čerstvý letový vstup opäť ovláda loď. |
| D-06 | Podržané vstupy neobídu R/C ani návrat fokusu. LMB náhľad nemení horninu. |
| D-07 | Adam odskúša pocit z pohybu, rotácie, kontaktu, prechodov a čitateľnosť režimov. |

D-07 Adam prijal po reálnom Play teste 17. 10. 2026. M-01 až M-05 z jadra miningu zostávajú budúce scenáre skutočného vŕtania.
