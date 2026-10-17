# Prvá scéna a orientácia v priestore — 17. 10. 2026

Stav: technicky overené. Adam scénu prezrel, prijal prvý vizuálny krok a následne zadal pohyb lode; let sa tým ešte neprijal. Základ pred zmenou: `6d5ea57`. Pracovný výsledok je v Unreleased, nie prijaté vydanie letu alebo miningu.

## Rozsah

Adam zadal prvé kroky najmä pre svet, pozadie a čitateľný pohyb s parallaxom. Použili sa jednoduché obrázky vytvorené editorovým nástrojom; Blender nebol potrebný. Finálne smerové vstupy, Full Stop, fyzika, mining a zdroje lode sa neimplementovali.

Vznikla scéna FlightTest, vizuálny prefab lode, hmla a tri opakované vrstvy hviezd, pevná mriežka a majáky. CameraPreviewPan je oddelený nástroj na náhľad priestoru, nie flight controller.

## Overenie

- Kompilácia prešla; kontrola novej scény nenašla chýbajúce skripty.
- 6/6 nových Unity EditMode prípadov parallaxu prešlo; nejde o pôvodných 6 Node testov Discordu.
- V Play Mode simulované držanie RightArrow posunulo kameru na X 19.711. Far Stars X 18.332 a Near Stars X 10.841 zodpovedali pomerom 0.07 a 0.45 pohybu voči obrazu. Loď zostala na (0, 0, 0).
- Home po zjednodušení resetu na držaný stav vrátil kameru z (12, 8, -10) na (0, 0, -10) aj vrstvu pozadia na začiatok.
- Náhľad pôvodnej aj posunutej kamery bol vyrenderovaný a vizuálne skontrolovaný. Chyba priehľadnosti orientačného kruhu bola opravená; hrúbka mriežky bola zvýšená pre čitateľnosť.
- Scéna je uložená, má 109 SpriteRenderer komponentov, 0 missing scripts a loď je skutočná inštancia prefabu bez Rigidbody2D.
- Play Mode po kontrole zastavený. Pôvodná SampleScene, balíky a existujúce nastavenia projektu sa nemenili. Unity navyše vygenerovalo ProjectSettings/SceneTemplateSettings.json s predvolenými pravidlami šablón; tento nový súbor je zahrnutý v zmene. Player build nebol vykonaný.

Kontrola dokumentácie prešla cez 97 relatívnych odkazov v 18 Markdown súboroch. Vlastné assety majú .meta súbory a 27 kontrolovaných GUID sa neopakuje. Runtime kód nemá závislosť na UnityEditor.

## Poznatky

Po externých zmenách C# treba nechať Unity dokončiť import a kompiláciu pred vyvolaním editorového generátora. Vrstvy sa opakujú po celých dlaždiciach; celá scéna používa 2D sprity a jeden spoločný unlit materiál. Reset náhľadu je idempotentný: opakované držanie Home má rovnaký výsledok.

## Odovzdanie

[Sprievodca prvou scénou](../../guides/FIRST_SCENE.md) opisuje súbory, Hierarchy, Inspector, ručný ekvivalent tvorby a skúšku cez Play. Pôvodné ovládanie: šípky, Shift, Home. Následný [letový krok](2026-10-17-first-flight.md) ich nahradil WASD/C a sledovaním lode.
