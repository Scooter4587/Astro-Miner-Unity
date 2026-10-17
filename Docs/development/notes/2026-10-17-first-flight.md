# Prvý fyzikálny let — 17. 10. 2026

Stav: implementované, technicky overené a Adamom prijaté 17. 10. 2026 (F-05). Základ pred zmenou: `ba868dcfd7a387867ef8fa6075f7eacad4d4878e` v main. Prijatý let je commit `8f135f934b25ec3092d805337f1929a2571c2ebe`; changelog ho uzatvára vo verzii 0.0.2.

## Dohoda a rozsah

Adam zadal prvý let podľa pôvodného konceptu a osobitne potvrdil WASD podľa obrazovky, Full Stop jedným C, prerušenie novým smerom a C bez otočenia v pokoji. Kamera sleduje loď. D-012 a [kontrakt](../../design/FLIGHT_AND_MINING.md) zachovávajú presné správanie.

Existujúci prefab dostal Dynamic Rigidbody2D, kapsulový collider, controller, klávesový vstup a jednoduché vizuály trysiek. Kamera nahradila dočasný pan sledovaním interpolovanej polohy lode. Parallax zostal. Fyzika sa aplikuje cez AddForce/AddTorque v FixedUpdate; runtime priamo nenastavuje velocity ani polohu lode.

Neboli pridané mining, asteroid, palivo, energia, teplo, hull, cargo, HUD, assisted flight ani rýchlostný limit. CapsuleCollider2D nie je dôkaz budúcej priechodnosti tunela. Historický generátor scény zostáva nástrojom prvého vizuálneho kroku, nie rekonštrukciou celého aktuálneho letu.

## Technické dôkazy

Unity 6000.6.2f1, pripojený správny projekt `C:\Development\Astro Miner Unity`. Pred zmenou čistý main a otvorená uložená FlightTest. Console mala 0 aktuálnych chýb a warningov; historický Pipeline buffer nie je nová regresia.

- 6/6 nových PlayMode scenárov prešlo, čas sady 10.63 s: drift, otáčanie bez presmerovania velocity a čakanie na zarovnanie, postupný ťah, Full Stop a ustálenie, prerušenie novým smerom, nulová/zvyšková rýchlosť bez otočenia.
- 6/6 existujúcich EditMode prípadov parallaxu prešlo, 0.31 s.
- Živá skúška FlightTest cez simulované klávesy Input System: po uvoľnení W bola velocity Y pri dvoch meraniach presne rovnaká 13.6799879; poloha Y rástla 33.9264 → 51.7105. Kamera mala nulovú odchýlku od interpolovaného transformu lode.
- C za letu: prvé meranie ešte rýchlosť Y 12.8804 a IsStopping=true; po dobrzdení velocity (0,0), IsStopping=false, engine=false. Kamera aj parallax pokračovali so skutočnou polohou lode; v pozadí sa správne uplatnilo opakovanie dlaždíc.
- Druhá živá skúška: C v pokoji zachovalo angle=0, speed=0, bez motora a rotácie. C počas držania pôvodného W aktivovalo brzdenie; nový smer D ho prerušil.
- Záber za letu s aktívnym hlavným motorom bol vyrenderovaný a vizuálne skontrolovaný. Loď bola v strede, mriežka a majáky ukazovali posun, tryska bola viditeľná.

Živá skúška dopĺňa integračné testy; presné trvanie držania klávesu zahŕňa latenciu nástroja a nie je benchmark. Po finálnej kompilácii prešla krátka živá skúška W/C: rýchlosť 4.319997 s aktívnym motorom a následne 0 s vypnutým motorom a ukončeným Full Stop. Kontrola scény potvrdila 114 SpriteRenderer komponentov, 0 missing scripts, správny Target kamery, zachovaný prefab a uložený začiatok (loď 0,0,0; kamera 0,0,-10). Play bol zastavený a scéna nebola dirty. Finálna Console: 0 aktuálnych chýb a warningov.

## Poznatky a limity

Pred zapnutím motorov sa meria skutočná orientácia telesa, nie len požadovaný smer. Pri brzdení sa limituje posledný ťah, aby sa nepreklopila zložka rýchlosti proti tryske. Veľmi malý zvyšok do 0.02 sa vyrovná obmedzenou silou najviac jedného kroku. Ide o ukončenie numerického zvyšku, nie okamžitý Full Stop.

CameraFollow má lokálne DefaultExecutionOrder -50, aby parallax v rovnakom LateUpdate čítal už sledovanú kameru. Nezmenilo sa globálne Script Execution Order. Interpolovaný transform zámerne mierne zaostáva za surovou fyzikálnou polohou; kamera sleduje viditeľnú loď, nie starý snapshot fyziky.

Scéna má konečnú orientačnú mriežku ±80, zatiaľ bez rýchlostného limitu a bez herných stien. Balíky, pôvodná SampleScene aj ProjectSettings zostali nezmenené. Windows Player build nebol vykonaný a startup/build scéna zostáva SampleScene. GitHub CI stále overuje iba Node changelog nástroj, nie Unity.

## Odovzdanie a prijatie

[Sprievodca letom](../../guides/FIRST_FLIGHT.md) obsahuje otvorenie FlightTest, WASD/C, krátke skúšky, nové komponenty, parametre, súbory a ručný ekvivalent práce cez Editor API. Play Mode po overení zastavený a uložená scéna pripravená od začiatku.

Adamovo prijatie prvého vizuálneho kroku je zachované v [predchádzajúcom zázname](2026-10-17-first-scene.md). Následne Adam odskúšal let: správanie prirovnal ku Godotu a výslovne spresnil, že „takmer“ znamenalo zlepšenie vďaka viditeľným RCS. Táto spätná väzba uzatvára prijatie prvého pocitu z letu F-05; neznamená tvrdenie, že osobne zopakoval každý automatický test. Baseline `8f135f934b25ec3092d805337f1929a2571c2ebe` je odteraz chránený workflow vetva/PR/Adamovo prijatie podľa D-008 a D-013. Záznam prijatia a vydanie 0.0.2 menia iba dokumentáciu, nie odskúšaný herný kód alebo assety.
