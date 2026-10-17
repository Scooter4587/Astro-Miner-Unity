# Vývojové prostredie

## Unity

- Editor: 6000.6.2f1, rovnaký pre celý projekt.
- Šablóna: Universal 2D, URP 2D Renderer.
- Input System je súčasťou projektu.
- Unity Pipeline umožňuje lokálne ovládanie Editora cez Unity CLI.
- Unity Cloud služby nie sú pre tento prototyp potrebné.

## VS Code

Používame oficiálne rozšírenia:

- Unity: VisualStudioToolsForUnity.vstuc
- C#: ms-dotnettools.csharp
- C# Dev Kit: ms-dotnettools.csdevkit

Unity stranu integrácie zabezpečuje balík com.unity.ide.visualstudio. Starý com.unity.ide.vscode neinštalujeme.

V Unity je VS Code nastavený ako External Script Editor. Dvojklik na skript ho otvorí vo VS Code. Vo VS Code možno debugger pripojiť konfiguráciou Attach to Unity.

## Lokálne ovládanie Editora

Pri otvorenom projekte:

```powershell
unity status --format json
unity command --project-path "C:\Development\Astro Miner Unity"
```

Dostupnosť inštalovaného balíka sama nepotvrdzuje živé spojenie. Pred zmenou scény treba overiť spojenie so správnym projektom.

## Verziovanie

Git sleduje Assets, Packages, ProjectSettings, dokumentáciu a .meta súbory. Generované riešenia pre VS Code a cache Editora sa necommitujú.

## Malý overovací nástroj

Assets/AstroMiner/Editor/EnvironmentCheck.cs je iba editorový nástroj. Menu Astro Miner > Setup > Check Environment vypíše základný stav projektu do Unity Console. Nie je to herná mechanika.

## Overenie 17. 10. 2026

- Editor odpovedá cez Pipeline 0.8.0-exp.1 pre tento konkrétny projekt.
- Kompilácia editorového kontrolného nástroja prešla.
- Aktívna scéna je Assets/Scenes/SampleScene.unity; kamera je ortografická a používa sa UniversalRP.
- Pri prvom Play Mode sa objavila výnimka ArgumentOutOfRangeException z UnityEditor.Search.SearchDatabase pri internom indexovaní. Pri opakovanom Play Mode sa neobjavila žiadna nová chyba. Žiadny vlastný herný kód ešte neexistuje.

- VS Code načítal riešenie Astro Miner Unity.slnx s dvomi editorovými projektmi a bez hlásených problémov.
- Unity rozšírenie 1.3.2, C# 2.160.4 a C# Dev Kit 3.40.210 sú aktívne. C# Dev Kit a jeho závislosti boli povolené pre tento workspace; pôvodné globálne vypnutie sa nenahrádzalo globálnym zapnutím.
- Konfigurácia Attach to Unity sa úspešne pripojila k Unity Editoru. Overovacie ladenie bolo potom odpojené.

- Skutočné pripojenie debuggeru bolo potvrdené aj z Unity cez System.Diagnostics.Debugger.IsAttached = true; po skúške sa debugger odpája.
- Pri prvom pripojení Unity požiadalo o Debug Mode. Povolilo sa iba ladenie pre aktuálnu Editor reláciu. Kým bolo toto dialógové okno otvorené, jeden kontrolný Pipeline príkaz vypršal; po potvrdení Editor znovu odpovedal. Pri budúcom otvorení Editora môže Unity zobraziť rovnakú otázku: vyber Enable debugging for this session.
- Automatické úvodné okno Universal 2D šablóny je vypnuté pre tento projekt. Ručne zostáva dostupné cez Tutorials.
- Otvorenie EnvironmentCheck.cs priamo cez Unity AssetDatabase.OpenAsset úspešne otvorilo súbor vo VS Code.
