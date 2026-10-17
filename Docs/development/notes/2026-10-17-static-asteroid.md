# Statický asteroid a kontakt — 17. 10. 2026

Stav: implementované a technicky overené vo feature/static-asteroid, následne Adamom prijaté ako pracovný základ a zlúčené cez PR #2 do main. Testovaný head 12e966a; squash commit b85f37fc4928e2848cc0ba8c4e39d547138df19c. Základ vetvy je e7a8427 (prijatie letu/0.0.2); chránený let zostáva 8f135f9. Prijatý krok je uzavretý v changelogu 0.0.3; oznámenie odosiela existujúci release workflow.

## Rozsah a výsledok

Adam schválil ďalší malý krok: jeden statický asteroid vo FlightTest, čitateľný povrch a fyzický kontakt, skúška príletu/brzdenia/dotyku/manévrovania/odletu. Hornina sa pripraví na neskorší vrták, ktorý sa teraz neimplementuje. Bez damage modelu, odberov zdrojov, driftu a rotácie asteroidu.

D-014 volí vstavaný Tilemap, ktorý už je súčasťou balíkov projektu. Vznikol StaticAsteroid prefab pri (18,16), Grid s bunkou 0.25, 4517 pevných dlaždíc, renderer a zlúčená Polygons kolízia (1 obrys, 210 bodov). Jeden Tile asset, generovaný RockCell sprite a RockContact materiál (friction/bounciness 0) určujú pracovnú podobu. Dáta sú lokálne k asteroidu. [ASTEROIDS.md](../../design/ASTEROIDS.md) vlastní konkrétne pravidlá a limity.

Kód letu, vstupov, kamery a prefab lode sa nemenili. Nový vlastný kód je editorový StaticAsteroidBuilder a integračné StaticAsteroidTests. Nie je zavedený runtime voxel framework ani nový balík. Viditeľné aj pevné bunky vychádzajú z rovnakého Tilemap. Generátor odmieta prepísať existujúce assety a v uloženej scéne je vypnutý.

## Overenie

| Kontrola | Skutočný výsledok |
| --- | --- |
| Kompilácia a Console | Finálny import úspešný; 0 aktuálnych errors a warnings. Jedna opravená chyba počas tvorby nástroja sa týkala spriteMeshType: nastavenie patrí do TextureImporterSettings, nie priamo TextureImporter. |
| PlayMode | 10/10 prešlo, 22.73 s: 6 nezmenených scenárov letu a 4 nové integračné skúšky. |
| S-01 | Každý stred bunky v bounds skutočného prefabu porovnaný s CompositeCollider2D.OverlapPoint; pevné aj prázdne bunky sedia. Veľkosť sprite zodpovedá Grid 0.25. |
| S-02 | Skutočný prefab lode dosiahol IsTouching s asteroidom pri rýchlosti 2; penetrácia zostala v tolerancii 0.05. Dokázal manéver popri povrchu aj odlet, asteroid nezmenil polohu. |
| S-03 | Diagonálny náraz pri 80 jednotkách/s a dt 0.02 zachytila Continuous kolízia; stred lode nevstúpil do horniny a penetrácia zostala v tolerancii 0.05. Nejde o záruku ľubovoľných rýchlostí. |
| S-04 | Full Stop zo 3 jednotiek/s so začiatkom 14 jednotiek pred hit pointom dobehol bez kontaktu. Pôvodné F-01 až F-04 prešli tiež. |
| EditMode | 6/6 existujúcich prípadov parallaxu prešlo. Spolu ide o 16 Unity prípadov; samostatné Node testy nie sú testmi fyziky. |
| Živá scéna | Preverený prílet a odlet so syntetickými klávesmi; kamera sledovala viditeľnú loď s chybou 0 a loď zostala mimo skaly. Samostatný okamžitý snapshot po prílete už kontakt nezachytil. |
| Kontrolovaná scénová skúška | Iba v Play boli nastavené počiatočné podmienky pri skutočnom asteroide: štart (18,4.25), povrch (18,7.25), rýchlosť nahor 2. Pri povrchu rýchlosť Y klesla na 0, snapshot separation 0.147; následný povel cez prijatý controller odletel na (13.72,-6.95), separation 13.382. Stred zostal mimo skaly, kamera error 0 a asteroid (18,16). Okamžité snapshoty nie sú tvrdením IsTouching=true; skutočný kontakt dokladá integračný S-02. |
| Vizuál | Záber od štartu a pri povrchu bol vyrenderovaný a prezretý. Asteroid je čiastočne viditeľný doprava hore; povrch a tmavšie pevné škvrny sú čitateľné ako prototyp. |
| Uložené assety | 114 SpriteRenderer a 1 TilemapRenderer, 0 missing scripts, skutočná inštancia prefabu, Static telo, správny kontaktný materiál. |
| Zachovanie základu | Git diff voči e7a8427 nemá zmeny v Runtime, ShipPlaceholder.prefab, Packages, ProjectSettings ani SampleScene. |
| Koncový stav Editora | FlightTest uložená a nie dirty, Play zastavený, loď (0,0,0), kamera (0,0,-10). Dočasné podmienky scénovej skúšky sa neuložili. |
| Player build | Nevykonaný; cieľ kroku je Editor kontakt. Build startup stále zostáva SampleScene. |
| Adamov test S-05 | Prijaté 17. 10. 2026: Adam potvrdil let okolo aj narážanie. Kolízia, odrazy a akcelerácia sa môžu neskôr ladiť; nejde o finálny balance. |

Asteroid integračné testy načítavajú skutočné projektové prefab assety cez AssetDatabase a sú pod UNITY_EDITOR. Nie sú testami samostatného Playera. Statický tvar je serializovaný v prefabe; bežné otvorenie ani spustenie scény nepotrebuje generátor znovu spustiť.

## Poznatky pre ďalší krok

Pri zmene buniek treba zachovať konzistenciu obrazu a pevnej kolízie. Zvolený Composite je v režime Manual; v budúcom vŕtaní bude treba spracovať Tilemap zmeny a vygenerovať geometriu pred závislou fyzikou. Diera, tunel, odkryté vnútro a manévrovacia komora ešte neboli implementované ani testované. O-003 zostáva v týchto častiach otvorené.

Jednorazové vloženie syntetického KeyboardState nemalo medzi pokusmi rovnaké načasovanie príletu. Preto sa dôkaz samotného kontaktu opiera o riadený PlayMode test s reálnymi prefabmi, nie o jeden screenshot. Kód vstupu sa kvôli tomu nemenil.

Dokumentačné relatívne odkazy prešli kontrolou; vlastné assety majú príslušné .meta a GUID sa neopakujú. Staged diff bol kontrolovaný aj na zachovanie prijatého jadra.

## Odovzdanie

[Sprievodca prvým asteroidom](../../guides/FIRST_ASTEROID.md) opisuje WASD/C, prílet, kontakt a odlet, nové objekty a komponenty, súbory, ukážku collidera cez Scene/Gizmos a ručný ekvivalent vytvorenia cez Editor API. Ďalšie zásahy do prijatého letu zostávajú cez vetvu/PR podľa D-008; tento krok bol po Adamovom prijatí zlúčený. Výsledné Assets, Packages a ProjectSettings sú Git diffom potvrdené totožné s testovaným headom 12e966a. Nasledujúca zmena je iba záznam prijatia/verzia, bez zmeny hry.
