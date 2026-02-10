using System;
using System.Collections.Generic;
using System.Text;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._NullLink.Protos;


[Prototype]
public sealed partial class AchievmentPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public SpriteSpecifier Icon = default!;
    [DataField]
    public SpriteSpecifier LockedIcon = default!;

    [DataField(required: true)]
    public LocId Name = "";

    [DataField(required: true)]
    public LocId Description = "";

    [DataField]
    public LocId? SecretDescription = null;
}
