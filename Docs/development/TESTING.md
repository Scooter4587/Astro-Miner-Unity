# Testovanie a dôkazy

Cieľom je overovať hráčske pravidlá a dôležité hranice. Počet testov sám nepotvrdzuje funkčnú hru. [Akceptačné scenáre](../design/FLIGHT_AND_MINING.md) sú podkladom pre výber kontrol, nie hotová testovacia sada.

## Aktuálny stav

- Unity Test Framework `1.8.0` je nainštalovaný. AstroMiner.EditModeTests má 6 prípadov parallaxu a AstroMiner.PlayModeTests 6 fyzikálnych scenárov letu; všetky prešli. FlightTest bola navyše overená živými vstupmi v Play Mode.
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

## Ako pribúdajú Unity testy

Prvý scénový krok vytvoril `Assets/AstroMiner/Runtime/AstroMiner.Runtime.asmdef` a editorovú testovaciu assembly `Assets/AstroMiner/Tests/EditMode/AstroMiner.EditModeTests.asmdef` s referenciou na runtime a TestAssemblies. Jej testy pokrývajú pomer parallaxu, opakovanie pri dlhom pohybe oboma smermi, návrat k začiatku a neplatnú veľkosť dlaždice.

Prvý let pridal `Assets/AstroMiner/Tests/PlayMode/` a assembly AstroMiner.PlayModeTests. Jej 6 scenárov overuje drift, otočenie bez presmerovania velocity/čakanie na zarovnanie, postupný ťah, Full Stop s ustálením, prerušenie a C pri nulovej/zvyškovej rýchlosti. Fyzikálny krok je kontrolovaných 0.02 s a pôvodná hodnota sa po teste obnoví. Podrobné výsledky vlastní [záznam letu](notes/2026-10-17-first-flight.md).

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

Dry run nevyužíva webhook a neodosiela správu. Unity testy možno spustiť v Unity Test Runneri cez Window > General > Test Runner, karta EditMode, assembly AstroMiner.EditModeTests; pre fyziku karta PlayMode a AstroMiner.PlayModeTests. Cez pripojený Pipeline bol overený aj run_tests s mode editor a filtrom tejto assembly; výsledky sa preberajú cez test_status. Pre fyziku sa používa run_tests s mode playmode a filtrom assembly AstroMiner.PlayModeTests. Vzdialená Unity CI a runner/licencia sú [O-004](../DECISIONS.md), nie už fungujúca služba.

## Záznam výsledku

Pri dôležitom teste zapíš commit alebo jednoznačný stav vetvy, scénu/scenár, nástroj, očakávaný a skutočný výsledok. Používaj `prešlo`, `zlyhalo`, `nevykonané` alebo `neplatí` s dôvodom. Pri hernom kroku uveď zvlášť `Adamov test: čaká/prešlo/zlyhalo` a jeho vyjadrenie, ak existuje.

Console bez nových chýb je jeden dôkaz; sama nepotvrdzuje správny pohyb. Zelené Node testy nepotvrdzujú kompiláciu Unity. Úspešný build nepotvrdzuje príjemný let. Dokumentácia nemá tieto hranice skrývať.

## Systémy lode v 0.1

Rozsah podľa D-010 zahŕňa Cargo, Power, Heat, Fuel a Hull aj odovzdávanie. Keď sa dohodnú ich presné pravidlá, pridajú sa cielené scenáre a testy:

- Zber a odovzdanie: materiál sa nepripíše dvakrát, presun zachová dohodnuté množstvo a plné cargo má definované správanie.
- Power a Fuel: spotreba a obnova podľa pravidiel, hranica vyčerpania, výpadok, doplnenie a ďalší platný stav.
- Heat: produkcia, chladenie, prekročenie prahu, dočasné odstavenie a návrat bez kmitania stavov.
- Hull: dohodnutý damage, hranica zničenia a schválený ďalší pokus/recovery vrátane strát.
- Integrácia: systémové výpadky nesmú obísť pravidlá letu alebo miningu; napríklad vypnutie motora okamžite nezruší zotrvačnosť.

Toto je plán pokrytia, nie tvrdenie o existujúcich testoch. Finálny Windows build sa overí celou výpravou aj relevantnými hraničnými stavmi, nielen štartom aplikácie.
