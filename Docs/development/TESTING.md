# Testovanie a dôkazy

Cieľom je overovať hráčske pravidlá a dôležité hranice. Počet testov sám nepotvrdzuje funkčnú hru. [Akceptačné scenáre](../design/FLIGHT_AND_MINING.md) sú podkladom pre výber kontrol, nie hotová testovacia sada.

## Aktuálny stav

- Unity Test Framework `1.8.0` je nainštalovaný. Vlastné EditMode a PlayMode testy ešte nie sú vytvorené ani spustené.
- Existuje 6 Node testov pre výber changelog verzie a formát Discord správy. Nie sú dôkazom fungovania letu.
- CI zatiaľ spúšťa tieto Node testy v oznamovacom workflow po zmene changelogu v main. Neoveruje každý PR a nekompiluje Unity.
- Setup má doložené overenie kompilácie editorového nástroja, Play Mode šablóny a debuggeru v [SETUP.md](../SETUP.md).

## Aké overenie k akej zmene

| Zmena | Primeraná kontrola |
| --- | --- |
| Iba dokumentácia | Relatívne odkazy, súlad s kódom/nastaveniami, pravdivé stavy a čistý diff. Bez zbytočného Unity buildu. |
| C# pravidlo alebo výpočet | Kompilácia a zameraný EditMode test pre čistú logiku, hranice a regresiu, ak je pravidlo stabilné a test má význam. |
| Let, vstup, fyzika, kolízia, scéna alebo prefab | Kompilácia, relevantné PlayMode integračné testy a skutočný Play Mode. Adam osobitne posúdi pocit z ovládania. |
| Oprava chyby | Reprodukcia alebo dôkaz príčiny, oprava a relevantný regresný test/scenár. Oddeľ pôvodné chyby od nových. |
| Platforma alebo build | Cielený Player build a spustenie dohodnutej cesty na cieľovej platforme. Editor sám nestačí. |
| Discord nástroj/workflow | Existujúca Node sada a dry run; reálne odoslanie iba pri autorizovanom vydaní alebo cielenej skúške. |

## Ako budú pribúdať Unity testy

Pri prvom relevantnom hernom kroku vytvoríme potrebné testovacie assemblies cez Editor/štandardné Unity nástroje, s referenciami na skutočný runtime. Dohodnuté miesto pre vlastné testy je `Assets/AstroMiner/Tests/EditMode/` a `Assets/AstroMiner/Tests/PlayMode/`; priečinky zatiaľ neexistujú.

- EditMode: nezávislé výpočty a rozhodovanie, ak ich oddelenie dáva zmysel.
- PlayMode: aplikácia síl, vstupy, prepínanie režimov, kolízie a priechod tunelom.
- Fyzikálne výsledky porovnávaj s vysvetlenou toleranciou a kontrolovaným časovým krokom. Nespoliehaj sa na náhodné načasovanie alebo presnú zhodu floatov.
- Testuj pozorovateľné správanie a hraničné prípady, nie kópiu výpočtu alebo názvy súkromných metód.
- Na nízkorizikovú textovú úpravu nevyrábaj testy, ktoré iba kontrolujú presné znenie.
- Po primeranej úspešnej kontrole ju nerozširuj a neopakuj bez novej zmeny alebo konkrétneho rizika.

## Lokálne Node kontroly

Spusti z koreňa repozitára:

```powershell
node --test tests/discord-changelog-workflow.test.mjs
node scripts/post-discord-changelog.mjs --dry-run
```

Dry run nevyužíva webhook a neodosiela správu. Unity testy sa po vytvorení spustia v Unity Test Runneri alebo dostupným podporovaným nástrojom; príkaz doplníme až po skutočnom overení. Vzdialená Unity CI a runner/licencia sú [O-004](../DECISIONS.md), nie už fungujúca služba.

## Záznam výsledku

Pri dôležitom teste zapíš commit alebo jednoznačný stav vetvy, scénu/scenár, nástroj, očakávaný a skutočný výsledok. Používaj `prešlo`, `zlyhalo`, `nevykonané` alebo `neplatí` s dôvodom. Pri hernom kroku uveď zvlášť `Adamov test: čaká/prešlo/zlyhalo` a jeho vyjadrenie, ak existuje.

Console bez nových chýb je jeden dôkaz; sama nepotvrdzuje správny pohyb. Zelené Node testy nepotvrdzujú kompiláciu Unity. Úspešný build nepotvrdzuje príjemný let. Dokumentácia nemá tieto hranice skrývať.
