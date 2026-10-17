# Astro Miner Unity

## Spolupráca

- Komunikuj po slovensky, priamo a bez zbytočného pritakávania.
- Postupuj po malých dohodnutých krokoch. Pred implementáciou vysvetli, čo sa stavia a ako to overíme.
- Rozlišuj schválené správanie, vlastný návrh, technické overenie a Adamovo posúdenie pocitu z hry.
- Aktuálny rozsah je v Docs/ROADMAP.md. Setup nie je súhlas postaviť celú hru.
- Zachovaj cudzie necommitnuté zmeny. Nikdy neukladaj heslá ani tokeny do kódu, logov alebo dokumentácie.

## Herné hranice

- Základ je let so zotrvačnosťou a razenie tunelov predným vrtákom.
- Klávesnica zadáva smer; RCS otočí loď a hlavný motor zaberie po zarovnaní.
- Pustenie vstupu neruší lineárnu zotrvačnosť. C má vykonať postupné zabrzdenie po otočení proti pohybu.
- LMB aktivuje vrták pred nosom. Mouse aim nie je schválený.
- Asteroidy sa delením tunelov nerozpadajú. Pohyb asteroidu príde až po statickom teste.
- Ekonomika, zdroje lode, príbeh, gravitácia a assisted flight sú ďalšie rozhodnutia, nie obsah prvého kroku.

## Unity a kód

- Najprv over skutočný stav Editora a aktívny projekt. Pri viacerých Editoroch používaj presný project path.
- Scény, prefaby a serializované assety upravuj cez Editor API; keď je Editor dostupný, neupravuj ich ručne ako YAML.
- C# zdroje upravuj priamo. Rozdeľuj zodpovednosti podľa skutočnej potreby, bez univerzálnej architektúry dopredu.
- Nové balíky inštaluj cez Package Manager alebo oficiálne nástroje. Needituj manifest ručne.
- Assety commituj spolu s ich .meta súbormi. Library, Temp, Logs a UserSettings zostávajú mimo Gitu.
- Po zmene kódu over kompiláciu. Pocit z letu a kolízie vyžadujú Play Mode; kompilácia sama nestačí.

## Changelog a vydania

- Pri dokončenom a overenom kroku aktualizuj CHANGELOG.md podľa Docs/RELEASING.md.
- Discord automaticky oznamuje novú verziu po zmene CHANGELOG.md v main. Oprava tej istej verzie sa znovu neposiela.
- Webhook je výhradne v GitHub repository secret DISCORD_CHANGELOG_WEBHOOK_URL. Nikdy ho nekopíruj do súborov ani výstupov.
