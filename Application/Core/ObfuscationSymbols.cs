using System;
using System.Reflection;

[assembly: Obfuscation(Feature = "encrypt symbol names with password ks5n!f68H-7Lkqw", Exclude = false)]
[assembly: Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]

// merging a embedding jsou dva zpusoby jak z vice resources udelat jednu assembly, navic pridava do assembly encryption a compression
// merge - vykonnejsi, rychlejsi startup, funguje i v omezenych prostredich (widows phone etc.), obcas nejde pouzit
// embeding jen vlozi resources do do jednoho assembly - pomalejsi ale nerozbije aplikaci
[assembly: Obfuscation(Feature = "embed Lidgren.Network.dll", Exclude = false)]
[assembly: Obfuscation(Feature = "embed MB.Tools.dll", Exclude = false)]
[assembly: Obfuscation(Feature = "embed irrKlang.NET4.dll", Exclude = false)]
[assembly: Obfuscation(Feature = "embed ShaderEffectLibrary.dll", Exclude = false)]
[assembly: Obfuscation(Feature = "embed log4net.dll", Exclude = false)]
[assembly: Obfuscation(Feature = "embed WPFLocalizeExtension.dll", Exclude = false)]
[assembly: Obfuscation(Feature = "embed XAMLMarkupExtensions.dll", Exclude = false)]
[assembly: Obfuscation(Feature = "embed Petzold.Media3D.dll", Exclude = false)]

// zapnuti debugovani v release
//[assembly: Obfuscation(Feature = "debug", Exclude = false)]
//[assembly: Obfuscation(Feature = "debug renaming", Exclude = false)]