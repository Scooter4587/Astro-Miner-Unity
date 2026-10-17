# Prvý let — FlightTest

Stav 17. 10. 2026: let je implementovaný, skompilovaný a technicky overený. Adam prvý pocit z letu prijal; baseline je `8f135f9`. Ťažba ani palubné zdroje nie sú aktívne. Stále pracujeme s jednoduchou grafikou.

V main je navyše prijatá statická hornina na skúšku kontaktu; vysvetľuje ju [sprievodca asteroidom](FIRST_ASTEROID.md). Ovládanie lode zostáva rovnaké.

## Spustenie a ovládanie

1. V Unity v **Project** otvor **Assets > AstroMiner > Scenes** a dvojklikni na **FlightTest**.
2. Stlač horné **Play** a klikni dovnútra **Game**, aby klávesy ovládali hru.
3. **WASD** určuje smer na obrazovke: W hore, S dole, A doľava, D doprava. Dve klávesy dovolia diagonálu.
4. Loď sa najprv cez RCS otočí. Hlavný motor zaberie až po zarovnaní. Pustením klávesov zhasne, ale loď letí ďalej so zachovanou rýchlosťou.
5. **C** jedným stlačením zapne Full Stop. Loď sa otočí proti aktuálnemu pohybu a motor ju postupne zabrzdí. C nemusíš držať. Nový smer WASD brzdenie preruší.
6. Nový pokus začneš zastavením **Play** a jeho opätovným spustením. Loď aj kamera sa vrátia do uloženého začiatku.

Ak pri C už držíš smer, brzdenie sa tým samo nezruší. Na jeho prerušenie smer zmeň alebo pusti a stlač znovu. Opakované C pokračuje v brzdení; nie je to prepínač na drift. C pri nulovej rýchlosti neotáča loď.

Šípky, Shift a Home už neposúvajú kameru. Kamera sleduje loď bez otáčania, oneskorenia a look-ahead. Preto je loď v strede; pohyb vidíš na mriežke, majákoch a hviezdach. Neotáčaš ani neposúvaš ručne kameru.

## Krátka skúška

| Skús | Čo očakávať |
| --- | --- |
| Krátko podrž W a pusti. | Rozbeh hore, potom drift rovnakou rýchlosťou; hlavná tryska zhasne. |
| Za driftu krátko stlač D. | Nos sa otáča doprava, ale pôvodný pohyb hore sa okamžite nezmení. Až po zarovnaní príde ťah doprava. |
| Za letu stlač C a pusti. | Otočenie proti pohybu, až potom brzdenie; žiadne okamžité zastavenie. Na konci sa loď ustáli. |
| Počas brzdenia stlač nový smer. | Prevezmeš ovládanie a loď začne sledovať nový smer. |
| Stlač C v pokoji. | Loď sa neroztočí. |

Najprv skúšaj krátke zábery približne 1–2 sekundy a potom C. Nie je zavedený herný rýchlostný limit. Pevná orientačná mriežka siaha od -80 do +80, čo nie je stena sveta; pri dlhom zrýchľovaní ju môžeš opustiť. Pozadie sa opakuje aj za ňou. Nový Play obnoví pokus. Aktuálny pocit letu je prijatý; ďalšie ladenie môžeme robiť vedome cez vetvu a PR.

## Čo pribudlo v Unity

V **Hierarchy** klikni na **Ship - Flight Prototype** a pozri **Inspector**:

| Komponent | Význam |
| --- | --- |
| **Transform** | Poloha, otočenie a mierka objektu. Počas hry jeho polohu riadi fyzika. |
| **Rigidbody 2D** | Fyzikálne teleso, ktoré vlastní rýchlosť a otáčanie. Dynamic; Gravity Scale aj tlmenie sú 0. Interpolate vyhladzuje obraz medzi fyzikálnymi krokmi. |
| **Capsule Collider 2D** | Dočasný kapsulový obal lode pre budúci kontakt. V tomto kroku nemáme asteroid ani test priechodnosti tunela. |
| **Ship Flight Controller** | Ťah, riadené otáčanie, drift a Full Stop; tu sú parametre letu. |
| **Ship Keyboard Input** | Číta WASD/C a odovzdáva povely controlleru. Samotné rýchlosti nevypočítava. |
| **Ship Thrust Visuals** | Zobrazuje jednoduchú odozvu motora a RCS podľa ich skutočnej aktivity. |

Rozbaľ loď šípkou v Hierarchy: pôvodný trup, kokpit a nos dopĺňa **Main Engine Exhaust** a dve skupiny **RCS ... Exhaust**. Tie iba zobrazujú trysky; fyziku vykonáva controller. Nos vrtáka je naďalej neaktívny obrázok.

Na **Main Camera** je teraz **Ship Camera Follow**. Pole **Target** ukazuje na loď. Ide o referenciu medzi objektmi — kamera z nej číta polohu lode. Starý **Camera Preview Pan** tu už nie je.

**Background** stále používa pôvodné Parallax Layer 2D. **World** drží pevnú mriežku a majáky. Scéna je ich uložená zostava; **prefab** je uložená predloha lode. Súčasná loď v scéne zostáva inštanciou toho istého prefabu ShipPlaceholder, hoci už nie je iba obrázkom.

## Parametre na pozorovanie a neskoršie ladenie

V **Ship Flight Controller**:

- **Main Acceleration = 4**: ako rýchlo narastá alebo klesá rýchlosť pri zábere motora.
- **Max Turn Speed = 160**: horné tempo otáčania v stupňoch za sekundu.
- **Turn Acceleration = 600**: ako rýchlo RCS roztočí alebo stabilizuje loď.
- **Alignment Tolerance = 5**: koľko stupňov od cieľového smeru sa smie odchyľovať nos pri zapnutí motora.
- **Stop Epsilon = 0.02**: malý zvyšok rýchlosti pri dokončení brzdenia, nie hranica okamžitého Full Stop.

Prvé štyri sú vhodné na spoločné ladenie; Stop Epsilon zatiaľ nechaj. Hodnoty počas Play môžeš pozorovať. Zmeny komponentov v scéne počas Play sa po zastavení spravidla vrátia. Trvalé ladenie uložíme vedome mimo Play; nejde ešte o finálny balance.

## Kde je kód

Všetko je pod lokálnym priečinkom `C:\Development\Astro Miner Unity`. V Project aj vo VS Code nájdeš:

| Súbor | Úloha |
| --- | --- |
| [FlightTest.unity](../../Assets/AstroMiner/Scenes/FlightTest.unity) | Uložená scéna s referenciou kamery na loď. |
| [ShipPlaceholder.prefab](../../Assets/AstroMiner/Prefabs/ShipPlaceholder.prefab) | Vizuál, fyzika, controller, vstup a trysky lode. |
| [ShipFlightController.cs](../../Assets/AstroMiner/Runtime/Flight/ShipFlightController.cs) | Pravidlá pohybu a síl. |
| [ShipKeyboardInput.cs](../../Assets/AstroMiner/Runtime/Flight/ShipKeyboardInput.cs) | Preklad klávesov na povely. |
| [ShipCameraFollow.cs](../../Assets/AstroMiner/Runtime/Presentation/ShipCameraFollow.cs) | Sledovanie lode kamerou. |
| [ShipThrustVisuals.cs](../../Assets/AstroMiner/Runtime/Presentation/ShipThrustVisuals.cs) | Viditeľná aktivita trysiek. |
| [ShipFlightTests.cs](../../Assets/AstroMiner/Tests/PlayMode/ShipFlightTests.cs) | Šesť fyzikálnych scenárov; nie test príjemnosti letu. |

Dvojklik na skript v Project ho otvorí vo VS Code. Pri prepnutí späť do Unity sa uložený C# importuje a kompiluje; po zmene počkaj na dokončenie kompilácie pred Play.

## Ako sa to pridáva ručne

Komponenty a referencie som pridal cez Unity Editor API, skripty som napísal do súborov. Nemusíš tento postup opakovať — výsledok už je uložený. Manuálny ekvivalent:

1. V Project dvojklik na prefab **ShipPlaceholder** otvorí jeho predlohu v Prefab Mode.
2. Vyber koreň lode a cez **Add Component** pridaj **Rigidbody 2D**. Nastav Dynamic, Mass 1, Gravity Scale 0, Linear/Angular Damping 0, Interpolate a Continuous.
3. Cez **Add Component** pridaj **Capsule Collider 2D** s veľkosťou 1.4 x 2.8 a Offset Y 0.2.
4. Rovnakým tlačidlom pridaj pripravené skripty **Ship Flight Controller**, **Ship Keyboard Input** a **Ship Thrust Visuals**. Controller a vstup sú schopnosti na tom istom objekte; preto zostáva zostava prehľadná.
5. Pridaj detské sprite objekty trysiek a pretiahni ich z Hierarchy do troch referenčných polí vo **Ship Thrust Visuals**. Ulož prefab a vráť sa do scény.
6. Na **Main Camera** odstráň Camera Preview Pan cez menu komponentu a **Remove Component**. Pridaj **Ship Camera Follow** a pretiahni loď z Hierarchy do **Target**.
7. **Ctrl+S** uloží scénu; potom Play a skúška v Game.

Nevytváral sa nový projekt, nová scéna ani nový balík. Historický generátor FlightTestSceneBuilder vytvára iba pôvodný prvý scénový krok a existujúcu scénu odmietne prepísať. Nie je to tlačidlo na obnovu súčasného letu.

## Hranice overenia

Prešlo 6 PlayMode scenárov letu a 6 EditMode prípadov parallaxu. Klávesy, drift, Full Stop a kamera boli overené aj priamo vo FlightTest cez Input System. [Záznam kroku](../development/notes/2026-10-17-first-flight.md) obsahuje dôkazy a limity.

Player build nebol vykonaný; v build nastaveniach stále zostáva SampleScene. F-05 — pocit z ovládania — Adam prijal 17. 10. 2026. Prijatý commit letu je `8f135f934b25ec3092d805337f1929a2571c2ebe`; ďalšie zásahy do jadra idú cez vetvu a PR.

Technické vysvetlenie: [Unity AddForce](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Rigidbody2D.AddForce.html) pridáva silu v čase; [linear damping](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Rigidbody2D-linearDamping.html) pri hodnote 0 neznižuje rýchlosť automaticky.
