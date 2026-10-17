# Postup vývoja

Poradie krokov vlastní tento dokument; dohodnuté správanie je v [kontrakte jadra](design/FLIGHT_AND_MINING.md), skutočný technický stav v [projektovom kontexte](PROJECT_CONTEXT.md). Rozcestník je [Docs/README.md](README.md).

## Aktuálny krok — dokumentačný základ

Pripravené na kontrolu cez PR; Adamovo prijatie a merge čakajú. Koncept, pravidlá čítania, register rozhodnutí a testovací postup zachovávajú kontext pre ďalšie malé kroky. Poznatky sú v [zázname kroku](development/notes/2026-10-17-project-foundation.md).

Najbližšia diskusia: dohodnúť malý rozsah letovej skúšobne a otázky O-001/O-002 v [rozhodnutiach](DECISIONS.md). Herná implementácia ešte nezačala.

## 0. Vývojové prostredie

Stav: hotové 17. 10. 2026.

Unity projekt sa otvoril, editorový C# nástroj sa skompiloval, prázdna scéna prešla opakovaným Play Mode a VS Code debugger sa pripojil k Editoru.

Verejné GitHub repo, čistý Universal 2D projekt, VS Code, lokálne napojenie Editora a prvý overený commit. Bez herných mechaník.

## 1. Letová skúšobňa

Zatiaľ nezačaté. Jedna placeholder loď, orientačné body, kamera, smerovanie klávesnicou, RCS, motor po zarovnaní a Full Stop na C. Najprv technické overenie, potom Adamov herný test.

## 2. Statický asteroid a vrták

Zatiaľ nezačaté. Jeden asteroid, správne kolízie, predný pracovný profil na LMB a odkryté tmavšie vnútro. Overiť rovný aj diagonálny tunel, zákrutu, odbočku, komoru, otočenie a výlet von.

## 3. Pohyb asteroidu

Zatiaľ nezačaté. Až po prijatom statickom prototype: drift, neskôr rotácia a správanie lode v pohybujúcom sa tuneli.

Ďalšie systémy sa dohodnú samostatne.
