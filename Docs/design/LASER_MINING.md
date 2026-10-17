# Laser a dátový asteroid — MVP-03B

Stav: implementované a technicky overené vo feature/laser-excavation; Adamovo prijatie čaká. Rozsah Adam schválil 17. 10. 2026 po prijatí pilotovania MVP-03A. Technické výsledky a stav prijatia vlastní [záznam](../development/notes/2026-10-17-laser-mining.md).

## Výsledok a hranice

Jedna loď v existujúcej FlightTest razí tunel v jednom statickom asteroide. R/WASD/C ostávajú podľa [drill kontraktu](DRILL_MODE.md). Držané LMB v pripravenom stave Drill postupne odoberá odkrytú horninu pred nosom. Pustenie LMB, odchod z režimu a strata vstupu ťažbu zastavia; otvor zostáva v runtime stave asteroidu.

Nie sú tu Cargo, získavanie rudy, odovzdanie, Power, Heat, Fuel, Hull ani perzistentný save. Work Per Second je rozpočet práce laseru, nie implementovaná zásoba energie. Reset skúšky je Stop Play a znovu Play. Jeden druh materiálu má uniformnú prototypovú tvrdosť; ďalšie materiály potrebujú samostatný krok.

## Dáta a vlastníctvo

- PrototypeAsteroid.asset je nemenný počiatočný AsteroidDefinition: vzorky geometrie v lokálnych súradniciach, rozstup, počiatok, uniformná tvrdosť a prototypové farby.
- AsteroidRuntime si v Awake vytvorí vlastný AsteroidField. Počas hry nemení asset ani iné inštancie.
- Pole 191 × 153 vzoriek s rozstupom 0.125 predstavuje lokálnu skalárnu geometriu: kladná hodnota je pevná hornina, záporná dutina. Medzi vzorkami je hustota lineárna na dvoch trojuholníkoch každej bunky.
- Hranica nula vytvára priebežne posúvaný polygonálny obrys. Toto je jemnejšia geometria, nie nezávislá kozmetická diera nad odlišnou kolíziou.
- Materiálová tvrdosť nie je odvodená od počtu geometrických buniek. Prvý materiál má Hardness 1. Práca sa delí tvrdosťou a odstránená plocha sa počíta z rovnakých trojuholníkov ako mesh.
- Removed Area je diagnostické množstvo v jednotkách², pri rovnakom priereze úmerné množstvu horniny. Nie je inventárom ani tvrdením o fyzikálnom 3D objeme.

AsteroidField vlastní zmeny a skutočne odstránené množstvo. ShipLaserMiner iba podáva pracovný profil a rozpočet; nevlastní mapu horniny. Pre tento jeden asteroid je cieľ explicitná referencia na scénovej inštancii lode, bez globálneho hľadania objektov. Výber medzi viacerými asteroidmi ešte nie je zavedený.

## Povrchový zásah

Zrezaný kužeľ sa rozdelí na dostatočne husté dráhy od jeho blízkej hrany k vzdialenej. Každá dráha prechádza presne trojuholníkmi poľa a končí na prvom pevnom povrchu. Tenká zvyšná vrstva sa nepreskakuje pevným krokom raymarchingu. Geometria ovplyvnená zásahom má konečné rozlíšenie; obrys na hrane záberu aproximuje pole 0.125, nejde o analyticky presný CAD rez.

Váhy okolo viditeľných zásahov rozdeľujú spoločný rozpočet. Hustota klesá postupne. Zmena skutočnej plochy má horný limit work / hardness; pri prekročení sa krok zmenší. Zvýšenie hustoty lúčov samo nezvýši dostupnú prácu. Práca v prázdne neprodukuje materiál a neodkladá sa na budúci zásah.

Pri aktívnom laseri sa dopredný povel navyše obmedzuje BoxCastom celej požadovanej šírky otočenia pred blízkou hranou lúča. Loď počká na vyčistenie priechodu skôr, než jej ramená prejdú za túto hranicu. Cúvanie ostáva riadené pôvodnou kontrolou celého trupu. C brzdí pohyb nezávisle od držaného LMB.

Nemaže sa kruh okolo lode, skala za trupom ani celý objem kužeľa naraz. Prechod na tvrdšie vrstvy a materiálové rozhrania ešte vyžaduje dátové rozšírenie a skúšky; dnešná geometrická oklúzia overuje princíp tienenia v jednom materiáli.

## Spoločný obraz a kolízia

AsteroidSurface oreže gridové trojuholníky podľa hustoty. Tie isté priesečníky vytvoria:
- trojuholníky viditeľného MeshRendereru,
- uzavreté orientované obrysy PolygonCollider2D vrátane dier.

Povrch je rozdelený na 30 oblastí po najviac 32 × 32 bunkách (4 × 4 jednotky). Po zásahu sa obnovia iba oblasti pretínajúce zmenené bunky. Všetky PolygonCollider2D sa zlúčia cez jeden manuálne aktualizovaný CompositeCollider2D na statickom Rigidbody2D, takže vnútorné hranice oblastí netvoria samostatné pevné steny. Po aktualizácii obrazu aj kolízie nasleduje fyzikálny krok.

Mesh povrchu aj vnútra používa jednoduchý VertexColorUnlit shader pre URP, ktorý číta polohu a farbu vrcholov bez údajov SpriteRendereru. Materiál lode a pozadia zostáva zachovaný.

Tmavé vnútro je neblokujúci mesh pôvodného pôdorysu. Zostáva za horninou a odkryje sa až po reálnom odstránení. Asteroid sa nefragmentuje, nemení transform a jeho počiatočný pôdorys sa nemení na úlomky.

Runtime meshe majú vlastný životný cyklus a pri zničení oblasti sa uvoľnia. Authoring meshe a počiatočné dáta zostávajú zachované.

## Pracovné hodnoty

| Vlastník v Inspectore | Hodnota | Význam |
| --- | --- | --- |
| MiningDrillModePrototype / Beam Near Width | 4.0 | Širší profil na prvú skúšku otočenia súčasného trupu. Pôvodný profil MVP-03A ostáva 3.2. |
| MiningDrillModePrototype / Beam Range | 2.5 | Dosah od emitora. |
| MiningDrillModePrototype / Beam Half Angle | 15° | Rozširovanie záberu. |
| LaserMiningPrototype / Work Per Second | 6 | Horný rozpočet odobratej plochy za sekundu pri Hardness 1. |
| LaserMiningPrototype / Update Interval | 0.06 s | Dávkovanie miningu; riadenie lode naďalej používa bežný fyzikálny krok. |
| PrototypeAsteroid / Hardness | 1 | Uniformná odolnosť prvej horniny. |

Sú to skúšobné hodnoty. Zmena šírky, dosahu alebo trupu vyžaduje znovu overiť otočenie a zákrutu. Zmena vzorkovania je authoring operácia, nie bezpečné posúvanie rozmerov existujúceho density assetu v Inspectore.

## Akceptačné skúšky

- M-01: skutočné rovné aj diagonálne razenie a priechod bez neviditeľných stien.
- M-02: vyrazenie zákruty a odbočky; Adam posúdi správanie pri nových stenách.
- M-03: zastavenie, celé otočenie vo vlastnom tuneli a návrat von.
- M-04: povrchové tienenie, hranice záberu s konečným rozlíšením, limit práce a množstvo podľa odstránenej plochy.
- M-05: trvalá runtime dutina, neblokujúce vnútro, žiadna fragmentácia.

Automatické a živé dôkazy musia byť zapísané zvlášť od Adamovho prijatia. Počet testov nie je záruka všetkých tvarov tunela ani budúcej mierky sveta.
