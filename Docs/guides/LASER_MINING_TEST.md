# Prvé skutočné kopanie — Play test

Výsledok MVP-03B: loď razí vlastnú chodbu v jednom plnom statickom asteroide. Pripravená L chodba z predchádzajúceho kroku zostáva v historickom prefabe, aktuálna FlightTest už začína pri plnej stene.

## Spustenie

1. V Unity Project otvor Assets → AstroMiner → Scenes → FlightTest.
2. Stlač Play a klikni do Game.
3. Loď začína pri (18,4), asteroid je nad ňou pri (18,16).
4. Stlač R a uvoľni ovládacie klávesy/myš. Oranžový prstenec je stabilizácia, azúrový pripravený Drill.
5. Podrž LMB. Hornina v dosahu pred nosom začne postupne ustupovať. Ak ešte nedosiahneš na stenu, pomaly sa priblíž W.
6. Drž W + LMB na razenie dopredu. W/S sa riadia nosom; A/D otáčajú.
7. C postupne zabrzdí pohyb aj otáčanie bez presmerovania nosa. Laser je nezávislý: držané LMB ďalej kope, pustenie ho zastaví.
8. Nový pokus: Stop Play a znovu Play. Tunel sa dnes neukladá medzi spusteniami.

Pustenie LMB ukončí laser, drill režim zostáva. R vráti bežný let so zotrvačnosťou. Zmena fokusu preruší vstup; po návrate uvoľni klávesy a myš.

## Čo skúsiť

- Rovný tunel: kope sa postupne, loď prejde odkrytým miestom a nezaberá hlavný motor. Pri aktívnom laseri dopredný RCS postup počká na dostatočne vyčistenú šírku.
- Zastav v dostatočne vyrazenom úseku a otoč sa o celý kruh. Pri stene loď nemá nekontrolovane odskakovať.
- Vyletieť von možno otočením a W alebo cúvaním S.
- Pred vstupom natoč loď a vyskúšaj diagonálny tunel.
- Pri razení s LMB pomaly zatáčaj, vyraz odbočku a rozšír miesto na komoru. Popíš mi, ak sa niekde zasekne roh trupu.
- Pusť laser: otvor zostáva otvorený, ďalšia hornina sa sama neodstraňuje.
- R počas kopania vráti Flight a zastaví ťažbu.

Zaujíma nás čitateľnosť postupu, rýchlosť kopania, šírka chodby, otočenie, kontakt s čerstvou stenou a prípadné trhnutie. Toto je prvý mining prototyp; pocit nie je uzamknutý.

## Kde to je v Unity

V Hierarchy vyber Asteroid - Runtime Mining. Na koreni sú:
- Rigidbody2D: jedno statické fyzikálne teleso celého asteroidu.
- Composite Collider 2D: spoločná kolízia z jednotlivých oblastí.
- Asteroid Runtime: vlastník pracovných dát a aktualizácie povrchu.

Pod koreňom je 30 Rock chunk objektov. Každý má Mesh Filter (tvar), Mesh Renderer (vzhľad), Polygon Collider 2D (pevný obrys) a Asteroid Chunk (obnova danej oblasti). Nie je to 30 samostatných asteroidov. Interior - nonblocking original footprint je iba tmavá podlaha, bez Collidera.

Na Ship - Flight Drill and Mining pribudol Ship Laser Miner. Jeho Asteroid referencia ukazuje na aktuálny asteroid a Profile na LaserMiningPrototype. Ship Drill Controller ďalej vlastní režim a pohyb; mining mu nepreberá riadenie.

Prefab je uložený opakovateľný objekt. Nový ShipMiningPrototype je variant pôvodnej drill lode. MineableAsteroid má nový dátový backend; pôvodné StaticAsteroid aj DrillTestAsteroid ostali uložené pre staršie skúšky.

## Profil a bezpečné ladenie

V Project → Assets → AstroMiner → Settings:
- LaserMiningPrototype → Work Per Second mení rozpočet práce.
- MiningDrillModePrototype → Beam Near Width, Range a Half Angle menia profil.
- RCS hodnoty v tom istom drill profile ostávajú podľa predchádzajúcej skúšky.

Hodnoty upravuj pri vypnutom Play. ScriptableObject je zdieľaný uložený asset; jeho úprava v Play môže zostať aj po Stop. Rozmery a density pole PrototypeAsteroid zatiaľ ručne nemeň — patria k sebe a vytvára ich authoring nástroj.

Source of truth hodnôt aj hraníc je [LASER_MINING.md](../design/LASER_MINING.md). Debug zapneš cez Scene/Game → Gizmos a na lodi Ship Drill Debug → Show Zones / Show Contacts. Azúrový kužeľ ukazuje maximálny pracovný profil; konečný obrys má rozlíšenie dátového poľa.

## Čo ešte nebude

Ruda nepribúda do inventára, laser nemíňa zásobu energie a loď sa neprehrieva. Tieto systémy prídu samostatne po prijatí samotného kopania. Koniec Play obnoví pôvodný asteroid; save/load zatiaľ neexistuje.
