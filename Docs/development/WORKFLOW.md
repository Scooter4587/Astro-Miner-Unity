# Pracovný postup

Základ spolupráce: Adam chce vidieť, čo sa stavia, po malých krokoch. Režim vetva + PR + Adamovo odskúšanie a schválenie bol výslovne zvolený 17. 10. 2026; pozri [D-006](../DECISIONS.md).

## Jeden krok od zadania po merge

1. Prečítaj [AGENTS.md](../../AGENTS.md), dokumentačný index, roadmapu, technický kontext, rozhodnutia a dokumenty dotknutého systému.
2. Over vetvu a pracovný strom. Pri hernej práci aj správny Editor, kompiláciu a Console. Zachovaj cudzie rozpracované zmeny.
3. Pred hernou implementáciou ujasni malý výsledok, hranice a pozorovateľné akceptačné kritériá. Otvorené herné pravidlo vyrieš s Adamom; rutinné technické riešenie stručne vysvetli a pokračuj v schválenom rozsahu.
4. Začni sústredenú vetvu z aktuálneho `main`. Pokračovanie rozpracovaného kroku používa jeho existujúcu vetvu. Názvy napríklad `feat/flight-test`, `fix/drill-collision`, `docs/project-foundation`; dlhodobý `develop` teraz nepotrebujeme.
5. Implementuj iba tento krok. Udržuj diagnostiku a parametre tak, aby sa dalo správanie vysvetliť a naladiť.
6. Vykonaj primerané kontroly podľa [TESTING.md](TESTING.md). Zapíš dôkazy, nevykonané kontroly a známe obmedzenia.
7. Aktualizuj dotknutý kontrakt, stav roadmapy a technický kontext. Rozhodnutie zachovaj v [DECISIONS.md](../DECISIONS.md), poznatok v `notes/`. Vyhni sa duplicitným denníkom bez nových informácií.
8. Priprav PR s výsledkom, overením, obmedzeniami a postupom Adamovho testu. Použi primerané časti [šablóny](CHECKPOINT_TEMPLATE.md). Rozpracovaný výsledok označ ako draft.
9. Adam odskúša viditeľný výsledok a schváli krok. Ak treba úpravy, pokračuj v tej istej vetve a zopakuj dotknuté kontroly. Pri čistej dokumentácii nahrádza herný test kontrola textu a rozhodnutí.
10. Po schválení priprav dátum a novú verziu changelogu, ak ešte nie je pripravená. Ak sa po schválení mení správanie alebo rozsah, vyžiadaj nové posúdenie dotknutej zmeny.
11. Merge vykonaj až s Adamovým súhlasom. Odporúčaný spôsob je squash do `main`; potom zosúlaď lokálny checkout bez straty cudzej práce. Príchod novej changelog verzie do main odošle Discord správu podľa [RELEASING.md](../RELEASING.md).

Schválené zadanie oprávňuje vykonať implementáciu a kontroly. Nepýtaj sa znovu na každý súbor alebo rutinný test. Nový herný rozsah a finálne prijatie kroku zostávajú vedomým rozhodnutím.

## Čo znamená hotovo

- Dohodnutý výsledok a hranice sú splnené.
- Príslušné testy a kompilácia prešli; viditeľné správanie bolo reálne prezreté. Čo nebolo vykonané, je označené.
- Dokumenty hovoria o skutočnom stave, nie iba o pláne.
- PR má konkrétny postup na odskúšanie a známe obmedzenia.
- Adamovo prijatie je doložené jeho vyjadrením, nie odhadom agenta.
- Po merge je overený výsledný commit a pri novej verzii aj Discord workflow.

`Technicky overené, čaká na Adamov test` je platný priebežný stav. Nie je to prijatá herná funkcia. Zlyhanie Discordu spätne neruší implementáciu, ale treba ho zaznamenať a vyriešiť bez slepého opakovaného posielania správ.

## Dokumentácia a rozsah infraštruktúry

Dokumentačný krok nevyžaduje spúšťať Unity ani meniť assety. Overujú sa odkazy, súlad s konfiguráciou a pravdivosť stavov. Zatiaľ nie sú nastavené povinné PR kontroly ani Unity build CI; pravidlo v dokumente nie je technické vynútenie v GitHube.

Keď sa objaví prvý stabilný herný kód, pripravíme jeho konkrétne testy a vhodné PR kontroly. Nezavádzame teraz prázdnu testovaciu infraštruktúru alebo automatické vydávanie hry iba kvôli dokumentácii.
