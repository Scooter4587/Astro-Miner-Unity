# Astro Miner Unity

Nový Unity 2D prototyp Astro Miner. Po havárii veľkej lode prežije autonómna ťažobná loď, ktorá lieta medzi asteroidmi, razí vlastné tunely a postupne sa opravuje.

## Aktuálny rozsah

Vývojové prostredie a dokumentačný základ sú pripravené. Cieľ 0.1 MVP je let, mining, zber, odovzdávanie a jednoduché funkčné systémy Hull, Fuel, Power, Heat a Cargo. Pripravená je prvá scéna s parallaxom, fyzikálnym letom, RCS, Full Stop a kamerou sledujúcou loď. Prvý let Adam odskúšal a prijal. Jeden statický asteroid a kontakt Adam odskúšal a prijal ako pracovný základ; PR #2 je zlúčený do main. Mining a systémy lode ešte nie sú implementované.

- Unity 6000.6.2f1, šablóna Universal 2D a URP 2D Renderer.
- Klávesnica a myš; prvý cieľ je Windows.
- VS Code pre čítanie kódu, IntelliSense a debugger.
- Oficiálne lokálne ovládanie Editora cez Unity CLI a Pipeline.

## Začiatok práce

1. Otvor tento priečinok v Unity Hub a použi verziu uvedenú v `ProjectSettings/ProjectVersion.txt`.
2. Otvor rovnaký priečinok vo VS Code a vyber vygenerované riešenie.
3. Začni [indexom dokumentácie](Docs/README.md), ktorý odkazuje na koncept hry, pravidlá jadra, aktuálny stav a workflow.
4. Najbližší krok vlastní [Docs/ROADMAP.md](Docs/ROADMAP.md); nastavenie nástrojov je v [Docs/SETUP.md](Docs/SETUP.md).

Historické [Godot repo](https://github.com/Scooter4587/Astro-Miner) je iba informačná referencia.

Changelog a vydávanie nových krokov: [CHANGELOG.md](CHANGELOG.md) a [Docs/RELEASING.md](Docs/RELEASING.md).

Statický povrch a jeho skúšku opisuje [sprievodca prvým asteroidom](Docs/guides/FIRST_ASTEROID.md).

Prvú scénu a let si otvor podľa [sprievodcu letom](Docs/guides/FIRST_FLIGHT.md). WASD určuje svetový smer a C spustí postupné brzdenie. [Starší sprievodca scénou](Docs/guides/FIRST_SCENE.md) vysvetľuje pozadie a zostavu.
