using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Content.IntegrationTests;
using DotNetGraph.Compilation;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.UnitTesting.Pool;

PoolManager.Startup();

var testContext = new ExternalTestContext(
    "Content.PrototypeGraph",
    Console.Out);

var poolSettings = new PoolSettings
{
    DummyTicker = true,
    Connected = false,
    Destructive = false,
    Fresh = true
};

var pair = await PoolManager.GetServerClient(
    poolSettings,
    testContext);

var protos = new Dictionary<string, PrototypeInfo>();

foreach (var entProto in pair.Server.ProtoMan.EnumeratePrototypes<EntityPrototype>())
{
    var entid = entProto.ID;
    if (!protos.ContainsKey(entid))
        protos[entid] = new PrototypeInfo(false, new(), new());
    var data = protos[entid];
    data.Abstr = entProto.Abstract;

    if (entProto.Parents == null)
        continue;

    foreach (var parent in entProto.Parents)
    {
        if (!protos.ContainsKey(parent))
            protos[entid] = new PrototypeInfo(false, new(), new());
        var pdata = protos[parent];
        pdata.Children.Add(entid);
    }
}

void FillDescendants(string baseId, string needle)
{
    var data = protos[needle];
    data.Bases.Add(baseId);

    foreach (var children in data.Children)
    {
        if (children.StartsWith("Base"))
            continue;
        FillDescendants(baseId, needle);
    }
}

var Bases = protos.Where(kv => kv.Key.StartsWith("Base")).ToDictionary();
var _ = Bases.Select(x => {FillDescendants(x.Key, x.Key); return 0;});

var BaseGroups = Bases.Select(x => (x.Key, new DotSubgraph()
        .WithIdentifier(x.Key)
        )).ToDictionary();

protos.Select(x =>
{
    var data = x.Value;
    data.Node = new DotNode()
        .WithIdentifier(x.Key)
        .WithLabel(x.Key);
    return 0;
});

foreach (var proto in protos)
{
    foreach (var child in proto.Value.Children)
    {
        var edge = new DotEdge()
            .From(proto.Key)
            .To(child);
        proto.Value.Bases.Select
    }
}

var graph = new DotGraph();
BaseGroups.Select(x => {graph.Add(x.Value); return 0;});

File.WriteAllText("entityPrototypes.dot",results);

PoolManager.Shutdown();

/// <summary>
/// A stripped down prototype
/// </summary>
/// <param name="Abstr">is the prototype abstract</param>
/// <param name="Children">Direct Children of this prototype</param>
/// <param name="Descendants">Descendants of this prototype (only filled out for Base* prototypes)</param>
record struct PrototypeInfo(bool Abstr, List<string> Children, List<string> Bases, DotNode? Node = null);
