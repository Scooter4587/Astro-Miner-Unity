# Changelog

Nový Unity vývoj Astro Miner začína vlastným číslovaním. Najnovšie vydané verzie sú navrchu; rozpracované zmeny patria do Unreleased.

## [Unreleased]

### Added
- MVP-03A: prepínanie R medzi letom a presným RCS drill režimom, stabilizácia pri vstupe, W/S posuv, A/D otáčanie a C brzdenie bez presmerovania nosa.
- Nastavovací drill profil, režimový prstenec, viditeľné translačné RCS, LMB pracovný kužeľ a prepínateľné Gizmos zóny.
- Zachované prefab varianty lode a asteroidu s pripravenou L chodbou pre skúšky prijatého pilotovania.
- MVP-03B: skutočné povrchové kopanie na LMB v jednom plnom statickom asteroide, nezávislé runtime dáta a odoberané množstvo podľa skutočne odstránenej plochy.
- Jemnejší polygonálny povrch po 0.125 so zhodnou kolíziou, lokálna obnova 30 oblastí, spoločná Composite kolízia a neblokujúce tmavé vnútro.
- Samostatné MiningDrillModePrototype a LaserMiningPrototype profily; kontrola vyčistenej šírky pred dopredným postupom pri aktívnom laseri.
- 7 drill PlayMode, 4 mining PlayMode a 5 mining/geometria EditMode scenárov; celá sada má 21 PlayMode a 11 EditMode úspešných prípadov.

### Changed
- FlightTest začína pred plnou stenou. Chodba vzniká až pri runtime kopaní, pôvodné pripravené prefaby zostávajú zachované.
- C v drill režime brzdí posuv a otáčanie nezávisle od držaného LMB.

### Documentation
- Kontrakty drill režimu a skutočného miningu, Unity sprievodcovia, pracovné profily, dôkazy skúšok a poznatky o geometrii.
- Adamovo prijatie pilotovania MVP-03A je zaznamenané na 3265d33; skutočné kopanie má vlastné čakajúce prijatie.

### Known limitations
- Adamov Play test a prijatie skutočného kopania čakajú; tento krok ešte nie je vydanie.
- Jeden statický asteroid, jeden uniformný materiál a prototypový povrch; plynulé oblúky a pocit z čerstvých stien sa budú ladiť.
- Cargo, získavanie rudy do inventára, zdroje lode, generátor sveta a save/load ešte nie sú implementované. Stop Play obnoví pôvodný asteroid.
- Výsledky v Editore nie sú overením samostatného Player buildu ani výkonu väčšieho sveta.

## [0.0.3] - 2026-10-17

### Added
- Jeden statický asteroid vo FlightTest, viditeľný doprava hore zo štartu; bunková hornina cez vstavaný Tilemap a spoločný kolízny povrch.
- Jednoduchá grafika horniny, prefab, Tile asset a skúšobný fyzikálny materiál kontaktu.
- Štyri PlayMode integračné scenáre skutočných prefabov pre súlad horniny a kolízie, kontakt/odlet, rýchly náraz a zabrzdenie pred povrchom.

### Documentation
- Zvolená reprezentácia horniny a sprievodca prvým asteroidom vrátane Hierarchy, Inspector a ručného ekvivalentu tvorby.
- Adam odskúšal let okolo asteroidu a narážanie; PR #2 zlúčený po prijatí pracovného základu. Let zostal nezmenený.

### Known limitations
- Kolízny pocit, odrazy a akcelerácia zostávajú otvorené ďalšiemu ladeniu.
- Vrták, ťažba a damage ešte nie sú implementované; Player build sa v tomto kroku nerobil.

## [0.0.2] - 2026-10-17

### Added
- Prvá scéna FlightTest: jednoduché vesmírne pozadie, opakované parallax vrstvy, pevná orientačná mriežka a majáky.
- Prefab lode s Rigidbody2D, WASD vo svetových smeroch, otáčaním RCS, ťahom po zarovnaní, zachovaným driftom a postupným Full Stop na C.
- Kamera sledujúca loď a jednoduchá vizuálna odozva hlavného motora a RCS; nahradený dočasný náhľad šípkami.
- 6 Unity EditMode prípadov parallaxu a 6 PlayMode scenárov fyzikálneho letu.

### Documentation
- Sprievodca prvým letom a zapísané potvrdené pravidlá WASD/C; prvý pocit z letu Adam odskúšal a prijal.
- Sprievodca prvou scénou: súbory, Hierarchy, Inspector, ovládanie a ručný ekvivalent tvorby v Unity.
- Praktický flow jedného lokálneho projektu medzi Unity, VS Code, Blenderom a GitHubom; odovzdanie skúšky a priebežné vysvetľovanie Unity.
- Trvalý dokumentačný základ: koncept hry, dohodnuté jadro, technický stav, rozhodnutia a poznatky z vývoja.
- Povinné čítanie a aktualizácia dokumentácie v AGENTS.md a postup budúcich Unity testov.
- Roadmapa k 0.1 MVP: celý cyklus letu, miningu, zberu a odovzdávania so základnými systémami Hull, Fuel, Power, Heat a Cargo.
- Spresnený workflow: úvodný prototyp v main, zásahy do prijatého letu alebo vrtáka cez vetvu a PR.

- Prijatý základ letu je `8f135f9`; ďalšie zásahy doň idú cez vetvu, PR a Adamovu skúšku.

### Known limitations
- Zatiaľ bez asteroidu, miningu a palubných systémov; Windows Player build nebol vykonaný.

## [0.0.1] - 2026-10-17

### Added
- Nový verejný repozitár a samostatný Unity projekt Astro Miner.
- Unity 6000.6.2f1, Universal 2D, URP 2D Renderer a Input System.
- Funkčné lokálne ovládanie Unity Editora cez Unity CLI a Pipeline.
- VS Code s Unity, C# a C# Dev Kit; overené otvorenie skriptu z Unity aj pripojenie debuggeru.
- Základné projektové pokyny, roadmapa a setup dokumentácia.
- Automatické Discord oznámenia nových changelog verzií cez GitHub Actions secret.

### Known limitations
- Pripravené je vývojové prostredie. Let lode, vrták a herná ekonomika ešte nie sú implementované.
- Pri prvom Play Mode sa objavila výnimka interného indexovania Unity; pri opakovanom teste sa nevrátila.
