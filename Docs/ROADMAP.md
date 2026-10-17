# Roadmapa k 0.1 MVP

Stav: pracovný návrh na odsúhlasenie rozsahu, 17. 10. 2026. Adam zadal cieľ loď, ovládanie, asteroid a mining. Podrobné etapy nižšie nie sú pokynom implementovať celé MVP naraz.

Dohodnuté jadro vlastní [FLIGHT_AND_MINING.md](design/FLIGHT_AND_MINING.md), skutočný stav [PROJECT_CONTEXT.md](PROJECT_CONTEXT.md), otvorené otázky [DECISIONS.md](DECISIONS.md). Rozcestník: [Docs/README.md](README.md).

## Čo má 0.1 dokázať

Hráč ovláda jednu loď so zotrvačnosťou, priletí k statickému asteroidu, postupne zabrzdí, predným vrtákom vyrazí priechodný tunel, vytvorí komoru, otočí sa a vyletí von.

Odporúčaný malý doplnok pre MVP: pri vŕtaní získa jednu surovinu a odovzdá ju pri jednoduchej základni. Výprava tak má viditeľný výsledok bez ekonomiky alebo výrobných reťazcov. Tento doplnok zatiaľ čaká na rozhodnutie O-006; bez neho by bola 0.1 technickou ukážkou letu a vŕtania.

Cieľová podoba: malá hrateľná Windows verzia s placeholder grafikou, nie finálny obsah celej hry. `0.1.0` je výsledný míľnik; priebežné prijaté kroky môžu dostať verzie `0.0.x` podľa [RELEASING.md](RELEASING.md).

## Etapy a overenie

| Krok | Výsledok | Kritérium prijatia | Stav |
| --- | --- | --- | --- |
| MVP-00 — prostredie a kontext | Unity, VS Code, Git, dokumentácia a Discord pripravené. | Doložený setup, správny projekt, živé spojenie s Editorom a odoslané 0.0.1. | Technický základ overený; roadmapa ešte návrh. |
| MVP-01 — letová skúšobňa | Jedna placeholder loď, orientačné body, kamera, smer z klávesnice, RCS, motor po zarovnaní a Full Stop na C. | F-01 až F-04 overené vhodnými testami/Play Mode, F-05 Adamov pocit z letu. Potom záznam prijatého commitu letu. | Navrhnuté, nezačaté. |
| MVP-02 — statický asteroid a kolízia | Jeden pripravený asteroid, čitateľný povrch a správny kontakt s loďou; zatiaľ bez damage systému. | Loď neprechádza neodstránenou horninou a kontakt nenaruší prijatý let mimo asteroidu. | Navrhnuté, po prijatí letu. |
| MVP-03 — vrták a tunely | LMB, predný pracovný profil, odstránenie horniny aj kolízie, tmavšie neblokujúce vnútro a kontrolovaný pohyb pri razení. | M-01 až M-05: rovný/diagonálny tunel, zákruta, odbočka, komora, otočenie, výlet von, žiadne mazanie mimo vrtáka ani fragmentácia. Potom záznam prijatého commitu vrtáka. | Navrhnuté, po malom technickom pláne horniny. |
| MVP-04 — výsledok výpravy | Jedna surovina, jednoduchý prehľad nákladu a odovzdanie pri placeholder základni. | Získaný materiál sa započíta raz, odovzdanie ho korektne odčíta a výpravu možno zopakovať. Bez receptov, cien a upgradov. | Odporúčanie, čaká na O-006. |
| MVP-05 — uzavretie 0.1 | Zrozumiteľné ovládanie, základná spätná väzba, opakovateľná skúška a samostatný Windows build. | Celá dohodnutá cesta funguje aj mimo Editora, relevantné testy prejdú, Adam prijme výsledok a obmedzenia; potom 0.1.0 a Discord. | Navrhnuté, až po ostatných prijatých krokoch. |

Každá etapa sa môže rozdeliť na menšie úlohy. MVP-02/MVP-03 môžu zasiahnuť do už prijatého letu; v takom prípade sa podľa workflow robia vo vetve aj počas úvodného MVP.

## Najbližší samostatný krok

Pripraviť a dohodnúť iba MVP-01. Pred kódom uzavrieť:

- smerové klávesy a význam smeru v priestore;
- čo preruší Full Stop a čo spraví C pri takmer nulovej rýchlosti;
- skúšobné parametre RCS, ťahu, zarovnania a brzdenia;
- najmenšie členenie kódu a prvé zmysluplné Unity testy.

Rigidbody2D je kandidát na vysvetlený technický návrh. V tomto dokumentačnom kroku nevzniká loď, scéna ani herný kód. Fuel, heat, power, cargo kapacita a príbeh prvý letový krok nerozširujú.

## Čo odporúčame nechať po 0.1

- Drift a rotácia asteroidov: samostatná náročná skúška relatívneho pohybu až po prijatom statickom vŕtaní.
- Fuel, power, heat, hull damage, checkpointy/saves, recovery a kapacitný model carga.
- Recepty, spracovateľské reťazce, upgrade strom, scanner, príbeh a väčší svet.
- Gravitácia/singularita, assisted flight, multiplayer a finálna grafika.

Toto je navrhnutá hranica MVP, nie zrušenie dlhodobej vízie. Ak sa rozsah zmení, upravíme najprv roadmapu a dotknuté kontrakty.

## Kedy je 0.1 hotová

- Dohodnuté etapy sú implementované, technicky overené a prijaté Adamom; plán nie je vydanie.
- Let a vrták majú záznam prijatia, overeného commitu a relevantných regresných scenárov v [kontrakte](design/FLIGHT_AND_MINING.md).
- Windows build prejde dohodnutú cestu od štartu cez let a tunel späť von, prípadne aj odovzdanie suroviny podľa O-006.
- Skúšku možno zopakovať bez ručného opravovania scény; známe obmedzenia sú viditeľné v dokumentácii.
- Dokumenty zodpovedajú skutočnosti, 0.1.0 je v changelogu a odoslanie na Discord je overené.

Automatický Unity build na GitHube nie je podmienkou prvého lokálneho MVP. Reálne lokálne testy a Player build podmienkou prijatia sú.
