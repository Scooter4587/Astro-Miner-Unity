# Pracovný postup

Adam chce rozumieť vývoju po malých krokoch. Dňa 17. 10. 2026 spresnil pôvodný režim vetva + PR: počas úvodného prototypovania povoľuje priamu prácu v `main`; po prijatí citlivého jadra sa zásahy doň robia cez vetvy. Rozhodnutie [D-008](../DECISIONS.md) nahrádza D-006 v tomto rozsahu. Hlavná vetva tohto repo sa volá `main`, nie `master`.

## Dva režimy práce

### Úvodný prototyp

- Malé dohodnuté kroky možno implementovať, overovať a commitovať priamo v `main`. Nové zadanie stále určuje rozsah; roadmapa neoprávňuje postaviť celé MVP naraz.
- Každý zmysluplný overený krok má samostatný commit a aktualizované dokumenty. Rozpracované pravidlo ani neodskúšaný pocit z letu sa neoznačujú ako prijaté.
- Priamy Git postup neruší kompiláciu, relevantné testy, Play Mode ani Adamovo posúdenie hry. Nedokončený checkpoint nesmie byť označený ako vydanie.
- Dokumentačný základ z PR #1 môže podľa tohto nového zadania prejsť do main bez ďalšieho samostatného schvaľovania Git postupu. Návrh roadmapy zostáva návrhom, kým sa dohodne jeho konkrétny rozsah.

### Prijaté jadro

- Let a vrták sa chránia každý od svojho prijatia, nie až po dokončení celej hry. Stav a overený commit sa zapíšu do [kontraktu jadra](../design/FLIGHT_AND_MINING.md).
- Zmena prijatého letu, vstupov, fyziky, brzdenia, vrtáka, kolízií alebo geometrie tunela ide do samostatnej vetvy a PR. Adam odskúša dotknuté scenáre a schváli merge.
- Nový systém, ktorý mení už prijaté jadro, podlieha rovnakému pravidlu. Ak prvý vrták zasahuje do už prijatého letu, aj tento integračný krok ide cez vetvu.
- Ochrana neznamená zákaz úprav; znamená zachovanie overeného základu, regresné testy a vedomé prijatie zmeny.
- Bežná dokumentácia a malé nesúvisiace prototypové doplnky môžu zostať v main. Pri väčšej alebo nejasnej zmene zvoľ vetvu. Technické vynútenie cez GitHub branch protection zatiaľ nie je nastavené.

## Jeden krok

1. Prečítaj [AGENTS.md](../../AGENTS.md), index, roadmapu, kontext, rozhodnutia a konkrétny dokument systému.
2. Over vetvu a pracovný strom; zachovaj cudzie zmeny. Pri práci s Unity over správny Editor, kompiláciu a Console.
3. Dohodni malý výsledok, hranice a pozorovateľné kritériá. Otvorené herné pravidlo vyrieš pred závislou implementáciou; bežné technické riešenie vysvetli a pokračuj v rozsahu.
4. Podľa režimu vyššie pokračuj v main alebo priprav sústredenú vetvu z aktuálneho main. Rozpracovaný krok pokračuje vo svojej vetve; netreba dlhodobý develop.
5. Implementuj iba dohodnutý krok a vykonaj primerané kontroly podľa [TESTING.md](TESTING.md). Zapíš skutočné dôkazy a obmedzenia.
6. Aktualizuj vlastnícky dokument, roadmapu, kontext a relevantné rozhodnutia. Podstatné poznatky patria do `notes/`, výsledok podľa [šablóny](CHECKPOINT_TEMPLATE.md).
7. Odovzdaj Adamovi konkrétny postup na odskúšanie. Technické overenie a jeho prijatie sú oddelené; pri chránenej zmene je výsledok v PR.
8. Po prijatí zaznamenaj stav a pri jadre aj overený commit. Novú verziu a Discord oznámenie priprav podľa [RELEASING.md](../RELEASING.md).
9. PR spoj až po Adamovom súhlase, odporúčane squash. Po priamom pushi aj merge over výsledný commit a pri novej verzii aj Discord workflow.

Schválené zadanie oprávňuje vykonať implementáciu a kontroly. Nepýtaj sa znovu na každý súbor alebo test. Rozšírenie herného rozsahu a prijatie výsledku zostávajú samostatné rozhodnutia.

## Dokončenie a infraštruktúra

Hotový krok spĺňa dohodnutý výsledok, má primerané úspešné overenie, pravdivú dokumentáciu a záznam Adamovho prijatia, ak sa posudzuje herné správanie. `Technicky overené, čaká na Adamov test` je platný priebežný stav.

Dokumentačný krok overuje odkazy, súlad a stav; nevyžaduje zbytočný Unity build. Povinné PR kontroly ani Unity build CI zatiaľ nie sú nastavené. EditMode a PlayMode assemblies už existujú pre parallax a prvý let; rozširujú sa s konkrétnym správaním. Zlyhanie Discordu treba zaznamenať a vyriešiť bez slepého opakovaného odosielania; nemení spätne výsledok hry.

## Nástroje, odovzdanie a priebežné učenie

Pracujeme v jednom lokálnom projekte `C:\Development\Astro Miner Unity`. Unity a VS Code používajú jeho rovnaké súbory. Git zaznamenáva lokálne verzie; push ich odošle do GitHub repo. GitHub Desktop slúži na prezeranie zmien a histórie správneho lokálneho repo. Na skúšanie výsledku na tomto PC netreba ďalší klon.

| Nástroj | Úloha |
| --- | --- |
| Lokálny priečinok | Kód, scény, prefaby, assety, nastavenia a dokumentácia. |
| Unity Editor | Skladanie scény a komponentov, konfigurácia cez Inspector, kompilácia a skúšanie cez Play Mode. |
| VS Code | Čítanie a úprava C# a dokumentácie, debugger. Uložené skripty Unity po obnovení spracuje a skompiluje. |
| Blender | Tvorba grafických podkladov, keď ich konkrétny krok potrebuje; exportované modely alebo renderované sprity potom importujeme do Unity. Prvý let môže použiť placeholdery. Spojenie s Blenderom sa overí pred prvým použitím. |
| Git a GitHub | Uloženie zmysluplných krokov a vzdialená história. Oznamovanie nového vydania rieši samostatný Discord workflow. |

Pri každom kroku:

1. Dohodneme malý výsledok a skúšku, ktorou sa overí.
2. Agent vysvetlí, čo vytvorí v Unity a čo napíše do súborov, potom vykoná dohodnutú prácu. Scény a komponenty mení cez Editor, C# zdroje priamo.
3. Agent najprv overí kompiláciu a relevantné správanie/testy. Ak overenie nebolo dostupné, označí ho.
4. Adam dostane konkrétny návod: presná scéna, spustenie cez Play, ovládanie, očakávaný výsledok a parametre na skúšobné ladenie. Pred zmenami alebo ďalším testom sa dohodne, kto práve ovláda Editor, aby si zásahy neprekážali.
5. Adam skúsi herný pocit a opíše rozdiel od očakávania. Úpravy, overenie a prijatie sa zaznamenajú podľa aktuálneho Git režimu.

Agent priebežne učí pojmy na reálnom kroku: GameObject ako objekt v scéne, komponent ako jeho schopnosť/nastavenie, Inspector ako miesto úpravy komponentov, prefab ako opakovateľný uložený objekt a scéna ako uložená zostava prostredia. Nedáva dlhú teóriu dopredu a nevydáva plánovanú scénu či skript za existujúci súbor.

Pri ladení vysvetlí, ktoré zmeny sú iba počas Play Mode a ktoré sa ukladajú do projektu; stabilné hodnoty uložíme vedome. Výsledky priebežne skúšame v Editore a pri príslušnom míľniku aj ako samostatný Windows build.
