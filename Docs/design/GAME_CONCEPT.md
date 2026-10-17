# Herný koncept

Stav: zachovaný smer hry z Adamovho pitchu, handoffu a následných spresnení. Cieľ 0.1 teraz podľa D-010 zahŕňa let, mining, zber, odovzdávanie a jednoduché funkčné systémy Hull, Fuel, Power, Heat a Cargo. Dlhodobá vízia je širšia; implementované funkcie vlastní [projektový kontext](../PROJECT_CONTEXT.md), poradie [roadmapa](../ROADMAP.md).

## Jedna veta

Po havárii veľkej lode v neznámom asteroidovom páse prežije poškodená autonómna ťažobná loď, ktorá si razí vlastné tunely, zbiera suroviny, vylepšuje sa a postupne získava možnosť dostať sa preč.

## Čo má byť príjemné

- Let stroja s hmotnosťou, zotrvačnosťou, viditeľnou prácou RCS a postupným brzdením.
- Fyzické razenie vlastných chodieb, odbočiek a komôr vo vnútri asteroidu.
- Objavovanie skrytých miest a návrat s nákladom, ktorý umožní ďalšiu výpravu.

Má to byť menšia pokojná 2D hra s miernym rizikom. Dve náročné mechanické témy sú let a vŕtanie. Najprv musia fungovať samy osebe; ďalšie systémy ich podporujú.

## Zamýšľaný herný cyklus

1. Vyletieť od vraku k asteroidu alebo zaujímavému signálu.
2. Priblížiť sa, zabrzdiť a zvoliť miesto vstupu.
3. Vyraziť tunel, vyťažiť suroviny a prípadne objaviť skrytý priestor.
4. Vrátiť sa s nákladom k vraku alebo využiteľnej základni.
5. Vložiť rudu do jednoduchého spracovateľa a získať materiál alebo súčiastku.
6. Opraviť či vylepšiť loď a vydať sa ďalej.

Spracovanie nemá prerásť do tovární, pásov a rozsiahlych výrobných reťazcov. Recepty, ceny, dĺžka hry, konečná podoba úniku ani tech tree zatiaľ nie sú určené.

## Loď a zdroje pre 0.1

Hráč ovláda autonómny stroj, nie ľudskú posádku. Začiatok má byť poškodený, slabý a lokálny. Tieto systémy patria do cieľa 0.1, ale pribudnú až po jadre a nie do prvého letového prototypu. Konkrétne parametre a následky sa dohodnú pri ich krokoch:

| Systém | Dohodnutý význam | Otvorené |
| --- | --- | --- |
| Hull | Loď sa dá poškodiť a zničiť; zničenie je skutočná prehra. | Damage model, strata po zničení, checkpointy a saves. |
| Fuel | Obmedzuje pohon a dosah; bez paliva môže loď uviaznuť. | Spotreba a recovery. Bezplatný návrat nie je schválený. |
| Power | Reaktor priebežne obnovuje jednu zásobu energie; vyčerpanie vyvolá dočasný výpadok. | Kapacita, výkon, odbery a priority. Power a energy nie sú dva zdroje. |
| Heat | Prehriatie dočasne odstaví činnosť, loď potrebuje vychladnúť. | Prahy a odstavené systémy. Vypnutie motora samo nezruší zotrvačnosť. |
| Cargo | Kapacita pre suroviny. | Hmotnosť alebo objem, zber a strata nákladu. |

Zlyhanie má mať následky bez automatického predpokladu permadeath. Strata nákladu, checkpoint alebo núdzová pomoc sú otvorené možnosti; konkrétny recovery postup nie je uzavretý.

RCS, vrták, reaktor, chladenie, hull, cargo, fuel tank a scanner môžu dostať jednoduché upgrady. Číselné hodnoty a konkrétne vetvenie sa určia neskôr.

## Svet a objavy

Asteroidy majú horninu, ktorú možno odstrániť, a tmavšie vnútro odkryté tunelom. Tunel predstavuje chodbu v jednej vrstve hmoty, preto asteroid netreba rozdeliť na fyzikálne kusy. Pomalý drift a rotácia celých asteroidov sú neskorší cieľ po statickom prototype.

Pás môže obsahovať dutiny, vraky, skrýše a záhadné signály. Scanner a príbeh cez objavy sú budúci obsah. Singularita alebo lokálna gravitácia sú možnosťou neskorého prekvapenia, nie záväznou funkciou ani súčasťou prvých krokov.

## Hranice najbližšieho vývoja

Najprv letová skúšobňa, potom statický asteroid a vŕtanie, až následne pohyb asteroidu. Zdroje lode, zber a odovzdávanie patria do 0.1 ako ďalšie samostatné kroky po základe letu a miningu. Ekonomika, finálna grafika, príbeh, gravitácia, boj, multiplayer a rozsiahly procedurálny svet prvé úlohy nerozširujú. Presná hranica upgradov, spracovania a saves ešte nie je uzavretá.

Ovládanie a kritériá priechodného tunela vlastní [Let a vŕtanie](FLIGHT_AND_MINING.md). Technika asteroidu ani fyzikálny model ešte nie sú uzamknuté konkrétnou implementáciou.

## Pôvod a spresnenia

Podklady dodal Adam 17. 10. 2026: `Astro_Miner_Herny_Napad.docx`, `Astro_Miner_2_Unity_Handoff.md` a úplná staršia konverzácia. Historické [Godot repo](https://github.com/Scooter4587/Astro-Miner) je informačná referencia.

Pitch je opis vízie; handoff a Adamove spresnenia rozlišujú otvorené veci. Návrh záchranného návratu preto nie je schválený systém a veta o zastavení pri prehriatí neznamená okamžité vynulovanie rýchlosti vo vesmíre.
