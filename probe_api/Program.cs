using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

// Probe: reflects over sts2.dll and dumps key game types for GAME_API.md
// Uses MetadataLoadContext so no dependencies need to resolve.

var gameDir = @"C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64";
var sts2Path = Path.Combine(gameDir, "sts2.dll");

// Gather DLL paths — game dir first, then runtime (exclude duplicates by filename)
var gameDlls = Directory.GetFiles(gameDir, "*.dll");
var runtimeDlls = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
var gameNames = new HashSet<string>(gameDlls.Select(Path.GetFileName)!, StringComparer.OrdinalIgnoreCase);
var allDlls = gameDlls.Concat(runtimeDlls.Where(r => !gameNames.Contains(Path.GetFileName(r)!))).ToArray();

var resolver = new PathAssemblyResolver(allDlls);
using var ctx = new MetadataLoadContext(resolver, "System.Private.CoreLib");
var asm = ctx.LoadFromAssemblyPath(sts2Path);

// Namespaces to dump
var targetNamespaces = new[]
{
    "MegaCrit.Sts2.Core.MonsterMoves.Intents",
    "MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine",
    "MegaCrit.Sts2.Core.Entities.Creatures",
    "MegaCrit.Sts2.Core.Runs",
    "MegaCrit.Sts2.Core.Models",
    "MegaCrit.Sts2.Core.Multiplayer.Game",
    "MegaCrit.Sts2.Core.Nodes.Combat",
    "MegaCrit.Sts2.Core.Combat",
};

var types = asm.GetTypes()
    .Where(t => targetNamespaces.Any(ns => t.Namespace == ns))
    .OrderBy(t => t.Namespace)
    .ThenBy(t => t.Name)
    .ToList();

var lines = new List<string>();
lines.Add("# STS2 Game API Reference");
lines.Add("");
lines.Add($"Auto-generated from `sts2.dll` v{asm.GetName().Version}. Do not edit manually — re-run `probe_api` to regenerate.");
lines.Add("");

string? currentNs = null;

foreach (var type in types)
{
    if (type.Namespace != currentNs)
    {
        currentNs = type.Namespace;
        lines.Add($"## {currentNs}");
        lines.Add("");
    }

    // Type header
    string kind = type.IsInterface ? "interface" : type.IsAbstract && !type.IsSealed ? "abstract class" : type.IsSealed ? "sealed class" : "class";
    if (type.IsEnum) kind = "enum";
    string baseInfo = "";
    if (!type.IsEnum && type.BaseType != null && type.BaseType.FullName != "System.Object")
        baseInfo = $" : {ShortName(type.BaseType)}";

    var ifaces = type.GetInterfaces();
    // Only show interfaces declared directly on this type (not inherited)
    if (ifaces.Length > 0 && !type.IsEnum)
    {
        var directIfaces = type.BaseType != null
            ? ifaces.Except(type.BaseType.GetInterfaces()).ToArray()
            : ifaces;
        if (directIfaces.Length > 0)
            baseInfo += (baseInfo == "" ? " : " : ", ") + string.Join(", ", directIfaces.Select(i => ShortName(i)));
    }

    lines.Add($"### `{kind} {type.Name}{baseInfo}`");
    lines.Add("");

    if (type.IsEnum)
    {
        var names = type.GetFields(BindingFlags.Public | BindingFlags.Static).Select(f => f.Name);
        lines.Add($"Values: `{string.Join("`, `", names)}`");
        lines.Add("");
        continue;
    }

    // Properties
    var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(p => !p.Name.StartsWith("<"))
        .OrderBy(p => p.Name)
        .ToList();
    if (props.Any())
    {
        lines.Add("**Properties**");
        lines.Add("");
        foreach (var p in props)
        {
            string access = p.GetMethod?.IsPublic == true ? "public" : "internal";
            lines.Add($"- `{access} {ShortName(p.PropertyType)} {p.Name}` {{ {(p.GetMethod != null ? "get" : "")}{(p.SetMethod != null ? "; set" : "")} }}");
        }
        lines.Add("");
    }

    // Fields (skip compiler-generated backing fields)
    var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(f => !f.Name.StartsWith("<") && !f.Name.Contains("k__BackingField"))
        .OrderBy(f => f.Name)
        .ToList();
    if (fields.Any())
    {
        lines.Add("**Fields**");
        lines.Add("");
        foreach (var f in fields)
        {
            string access = f.IsPublic ? "public" : f.IsFamily ? "protected" : "private";
            lines.Add($"- `{access} {ShortName(f.FieldType)} {f.Name}`");
        }
        lines.Add("");
    }

    // Methods (skip property accessors, compiler-generated, and Object methods)
    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(m => !m.IsSpecialName && !m.Name.StartsWith("<") && m.Name != "Finalize" && m.Name != "MemberwiseClone")
        .OrderBy(m => m.Name)
        .ToList();
    if (methods.Any())
    {
        lines.Add("**Methods**");
        lines.Add("");
        foreach (var m in methods)
        {
            string access = m.IsPublic ? "public" : m.IsFamily ? "protected" : "internal";
            string isVirtual = m.IsVirtual && !m.IsFinal ? " virtual" : "";
            string isStatic = m.IsStatic ? " static" : "";
            var parms = string.Join(", ", m.GetParameters().Select(p => $"{ShortName(p.ParameterType)} {p.Name}"));
            lines.Add($"- `{access}{isStatic}{isVirtual} {ShortName(m.ReturnType)} {m.Name}({parms})`");
        }
        lines.Add("");
    }
}

var output = string.Join("\n", lines);
var outPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "GAME_API.md"));
File.WriteAllText(outPath, output);
Console.WriteLine($"Written to: {outPath}");
Console.WriteLine($"Types documented: {types.Count}");

static string ShortName(Type t)
{
    if (t.IsGenericType)
    {
        var def = t.GetGenericTypeDefinition().Name;
        def = def.Substring(0, def.IndexOf('`'));
        var args = string.Join(", ", t.GetGenericArguments().Select(ShortName));
        return $"{def}<{args}>";
    }
    return t.Name switch
    {
        "Void" => "void",
        "Boolean" => "bool",
        "Int32" => "int",
        "Int64" => "long",
        "String" => "string",
        "Decimal" => "decimal",
        "Single" => "float",
        "Double" => "double",
        "Object" => "object",
        _ => t.Name
    };
}
