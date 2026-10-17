# Skutočné kopanie a dátový asteroid — 17. 10. 2026

Stav: implementované vo feature/laser-excavation nad prijatým pilotovaním 3265d33 (PR #3). Unreleased. Adamovo prijatie skutočného kopania čaká.

## Rozsah

Adam schválil MVP-03B: jeden plný statický asteroid, vlastné runtime dáta, postupný povrchový laser na LMB, zhodný obraz a kolízia, otočenie a návrat z vlastného tunela. Cargo, ruda do inventára, zdroje lode, saves a Player build do tohto kroku nepatria.

[Kontrakt](../../design/LASER_MINING.md) vlastní pravidlá a hodnoty; [sprievodca](../../guides/LASER_MINING_TEST.md) presnú scénu, ovládanie, Hierarchy a Inspector.

## Výsledok

- AsteroidDefinition je nemenný počiatočný asset. AsteroidField je nezávislá runtime kópia; hustota neprepisuje definíciu ani iné inštancie.
- Pole má 191 × 153 vzoriek po 0.125; počiatočná plocha je 282.286438 jednotiek². Jeden materiál má Hardness 1.
- Povrchový laser končí na prvom pevnom úseku každej dráhy cez gridové trojuholníky. Rozpočet je limitovaný skutočnou odstránenou plochou; celý hrubý tile sa nepripíše za zásah rohu.
- AsteroidSurface vytvára mesh aj uzavreté kolízne obrysy z rovnakých priesečníkov. Tridsať oblastí po najviac 32 × 32 bunkách sa obnovuje lokálne a zlieva cez jeden CompositeCollider2D na statickom Rigidbody2D.
- Tmavý pôdorys za horninou nemá kolíziu. Nevzniká fragmentácia, teleport ani mazanie okolo trupu.
- ShipLaserMiner používa explicitnú scénovú referenciu na jeden asteroid. Nový ShipMiningPrototype je variant drill lode; MineableAsteroid má nový geometrický backend. Pôvodné prefaby a ich skúšky zostávajú zachované.
- Parametre sú samostatné assety: MiningDrillModePrototype (near width 4.0, range 2.5, half angle 15°) a LaserMiningPrototype (work 6/s, interval 0.06 s). Čísla nie sú finálny balance.

## Nájdené problémy a opravy

1. Natívna geometrická skúška overila uzavretú dutinu: PolygonCollider2D vracia stenu ako pevnú a stred diery ako prázdny. Spoločná Composite kolízia bola následne overená aj cez hranice štyroch oblastí.
2. Prvé razenie zanechalo zub horniny pri ramene lode za začiatkom lúča. Živý cast skončil pri (18.6164, 7.74886), zatiaľ čo emitujúca hrana už bola pri Y 8.13647. Dátové pole potvrdilo pevný materiál; nešlo o neviditeľnú zastaranú kolíziu.
3. Pri aktívnom laseri sa preto dopredný povel obmedzuje kontrolou vyčistenej šírky otočenia pred blízkou hranou lúča. Šírka kužeľa má rezervu 4.0 oproti požadovaným 3.5. Hornina za loďou sa tým nemaže. Diagonálny priechod po tejto oprave prešiel.
4. Odhad práce podľa počtu vzoriek nevyužíval dostupný rozpočet na strmých či čiastočne prázdnych gradientových oblastiach. Krok teraz vychádza z maximálnej malej zmeny exponovaného povrchu a zmenšuje sa podľa skutočnej odstránenej plochy.
5. C brzdí posuv a otáčanie bez zrušenia držaného LMB; vstup aj verejné povely majú zhodné správanie. Skúška držaného lúča bola presunutá pred úplné vyprázdnenie dosahu — laser v prázdne správne nemusí meniť revíziu.
6. Nová skúška po návrate von pôvodne čakala iba 0.8 s na úplné dobrzdenie z 1.5 jednotky/s. Zvyšok 0.0758 zodpovedal obmedzenému RCS s dobiehaním cieľa; skúška teraz overuje ustálenie do 1.6 s, bez zmeny prijatých zrýchlení.
7. Jedno zostavenie zastavilo použitie neexistujúceho RectInt.MinMaxRect; opravené na konštruktor s min súradnicami a rozmermi.
8. Následný celý beh Test Runnera vrátil 0 prípadov; nebol započítaný. RequestScriptReload obnovil zber a nový beh zaznamenal 21 začatých a 21 úspešných prípadov.
9. Prvý živý Game záber ukázal neviditeľný mesh napriek platným dátam a kolízii. Pôvodný Sprite-Unlit materiál používal sprite údaje nevhodné pre tento MeshRenderer. Nový jednoduchý URP VertexColorUnlit shader číta priamo pozíciu a farbu vrcholov; bol priradený iba 31 asteroidovým rendererom. Shader je podporovaný a ShaderUtil hlási 0 správ. Nový záber bol prezretý a ukazuje horninu aj skutočne vyrazenú chodbu. Po tejto izolovanej oprave vzhľadu sa zopakovala živá scénová skúška; runtime pravidlá sa po úspešnej 32-prípadovej sade nemenili.

Tieto neúspešné pokusy nie sú započítané ako úspešné overenie. Technické testy nenahrádzajú Adamov pocit z nových stien.

## Overenie

| Kontrola | Skutočný výsledok |
| --- | --- |
| Cielené mining PlayMode | 4/4 prešlo, 46.87 s: skutočný rovný tunel, 360° otočenie bez aktívneho laseru a návrat, diagonála, pravouhlá odbočka cez oblasti, aktivácia/pustenie LMB, nezávislé C, nemenný asset a zhoda aktualizovanej kolízie s dátami. |
| Celá PlayMode sada | 21/21 prešlo, 107.79 s: 4 mining, 7 drill, 6 let, 4 pôvodný asteroid/kontakt. Žiadne preskočené ani inconclusive prípady. |
| Celá EditMode sada | Konečných 11/11 prešlo, 0.31 s: 5 mining/geometria a 6 parallax. Celkovo 32 Unity prípadov. |
| Živá FlightTest a obraz | Prešlo cez verejné riadiace povely v Play: rovný tunel, otočenie doprava, odbočka a zastavenie za 16.84 s. Loď (22.80422,13.62465), velocity (0,0), odstránená plocha 55.82462, zostáva 226.4618, dátová aj geometrická revízia 175. Najmenší odstup trupu 0.04974, kamera bez odchýlky. Game záber bol uložený a vizuálne prezretý po oprave materiálu. |
| Výkon | 175 lokálnych vzoriek celého mining kroku v Editore: medián 3.713 ms, P95 6.0091 ms, maximum 6.1262 ms; najviac 4 z 30 oblastí obnovené naraz. Ide o jednu lokálnu vzorku, nie benchmark Playera, FPS záruku ani dôkaz väčšieho sveta. |
| Dokumentácia a meta | 29 Markdown dokumentov, 197 relatívnych odkazov, 0 neplatných; 104 unikátnych GUID, 0 duplicít a 0 chýbajúcich meta. Pôvodné prefaby, Flight kód, Packages a ProjectSettings zostali nezmenené. |
| Adamov test | Čaká; pilotovanie MVP-03A je už prijaté, reálne kopanie ešte nie. |
| Player build | Nevykonané; schválený krok sa skúša v Editore. |

## Obmedzenia a ďalší krok

- Ide o jeden statický asteroid s jedným uniformným materiálom a počiatočnými dátami odvodenými z pôvodného authoringu. Nie je to generátor sveta ani viacmateriálový systém.
- Obrys je lineárna aproximácia poľa 0.125. Hrana záberu má konečné rozlíšenie, nie analyticky presný rez.
- Automatická odbočka používa zastavenie a otočenie na mieste. Plynulé oblúky, zvláštne úzke kríženia a herný pocit pri čerstvých stenách potrebuje posúdiť Adam.
- Tunel zostáva počas Play; Stop obnoví definíciu. Save/load nie je zavedené.
- Kapacita Cargo, energia, teplo, suroviny a ekonomika zostávajú ďalšie samostatné kroky.

Po skúške je uložená čistá zastavená FlightTest: loď (18,4), rotácia 0, Mode Flight, klávesový vstup zapnutý, asteroid (18,16), runtime stav resetovaný, 0 missing scripts, 0 aktuálnych Console errors/warnings. Runtime polohy ani vykopaný tunel sa neuložili do počiatočného assetu.

Nový krok ide do samostatného PR nad PR #3. Merge, nové vydanie a Discord zatiaľ neprebehli.
