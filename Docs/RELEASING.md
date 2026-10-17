# Changelog a Discord

Používame rovnaký princíp ako Evolution Game, prispôsobený vetve main a názvu Astro Miner.

1. Počas práce zbieraj zmeny pod `## [Unreleased]` v CHANGELOG.md.
2. Keď je malý krok dokončený, overený a prijatý Adamom, priprav v jeho vetve novú sekciu `## [verzia] - YYYY-MM-DD` nad staršími verziami. Sekcie používajú `### Added`, `### Changed`, `### Fixed`, `### Documentation` alebo `### Known limitations`; položky začínajú pomlčkou.
3. Po Adamovom schválení spoj PR s dokončenou novou verziou do main. GitHub Actions ju porovná s predchádzajúcim changelogom a odošle najnovšiu novú verziu do Discordu.
4. Oprava textu už existujúcej verzie ani úprava Unreleased automaticky neposiela ďalšiu správu.

Dohodnutá cesta do main je vetva a PR podľa [WORKFLOW.md](development/WORKFLOW.md). Oznamovací mechanizmus technicky reaguje na push do main vrátane merge; to neobchádza pravidlo Adamovho schválenia. Pri jednej zmene changelogu vydávaj jeden nový malý krok. Toto automaticky nevytvára GitHub Release ani herný build.

## Tajná konfigurácia

Repository secret sa volá `DISCORD_CHANGELOG_WEBHOOK_URL`. Jeho hodnota patrí výhradne do GitHub Settings > Secrets and variables > Actions. Webhook URL nesmie byť v Gite, lokálnych konfiguračných súboroch ani logoch.

Workflow má len `contents: read`. Do odosielacieho kroku dostane secret cez environment a nepoužíva ho v pull requestoch. Správa má vypnuté Discord mentions. Odoslanie čaká na potvrdenie vytvorenia správy od Discordu.

## Kontrola pred vydaním

```powershell
node --test tests/discord-changelog-workflow.test.mjs
node scripts/post-discord-changelog.mjs --dry-run
```

Dry run vypíše iba obsah správy; webhook nepotrebuje a nič neposiela. Po publikovaní skontroluj výsledok workflow Discord Changelog v GitHub Actions.

Ručné opätovné odoslanie je možné cez Actions > Discord Changelog > Run workflow a konkrétnu existujúcu verziu. Je to zámerný resend a môže vytvoriť druhú správu.
