# Changelog

Nový Unity vývoj Astro Miner začína vlastným číslovaním. Najnovšie vydané verzie sú navrchu; rozpracované zmeny patria do Unreleased.

## [Unreleased]

### Added
- Prvá scéna FlightTest: jednoduché vesmírne pozadie, opakované parallax vrstvy, pevná orientačná mriežka a majáky.
- Prefab lode s Rigidbody2D, WASD vo svetových smeroch, otáčaním RCS, ťahom po zarovnaní, zachovaným driftom a postupným Full Stop na C.
- Kamera sledujúca loď a jednoduchá vizuálna odozva hlavného motora a RCS; nahradený dočasný náhľad šípkami.
- 6 Unity EditMode prípadov parallaxu a 6 PlayMode scenárov fyzikálneho letu.

### Documentation
- Sprievodca prvým letom a zapísané potvrdené pravidlá WASD/C; prijatie pocitu z letu ešte čaká.
- Sprievodca prvou scénou: súbory, Hierarchy, Inspector, ovládanie a ručný ekvivalent tvorby v Unity.
- Praktický flow jedného lokálneho projektu medzi Unity, VS Code, Blenderom a GitHubom; odovzdanie skúšky a priebežné vysvetľovanie Unity.
- Trvalý dokumentačný základ: koncept hry, dohodnuté jadro, technický stav, rozhodnutia a poznatky z vývoja.
- Povinné čítanie a aktualizácia dokumentácie v AGENTS.md a postup budúcich Unity testov.
- Roadmapa k 0.1 MVP: celý cyklus letu, miningu, zberu a odovzdávania so základnými systémami Hull, Fuel, Power, Heat a Cargo.
- Spresnený workflow: úvodný prototyp v main, zásahy do prijatého letu alebo vrtáka cez vetvu a PR.

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
