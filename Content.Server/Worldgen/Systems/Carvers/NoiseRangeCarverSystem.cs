// SPDX-FileCopyrightText: 2023 Moony <moony@hellomouse.net>
// SPDX-FileCopyrightText: 2024 SimpleStation14 <130339894+SimpleStation14@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Server.Worldgen.Components.Carvers;
using Content.Server.Worldgen.Systems.Debris;

namespace Content.Server.Worldgen.Systems.Carvers;

/// <summary>
///     This handles carving out holes in world generation according to a noise channel.
/// </summary>
public sealed class NoiseRangeCarverSystem : EntitySystem
{
    [Dependency] private readonly NoiseIndexSystem _index = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    /// <inheritdoc />
    public override void Initialize()
    {
        SubscribeLocalEvent<NoiseRangeCarverComponent, PrePlaceDebrisFeatureEvent>(OnPrePlaceDebris);
    }

    private void OnPrePlaceDebris(EntityUid uid, NoiseRangeCarverComponent component,
        ref PrePlaceDebrisFeatureEvent args)
    {
        var coords = WorldGen.WorldToChunkCoords(args.Coords.ToMapPos(EntityManager, _transform));
        var val = _index.Evaluate(uid, component.NoiseChannel, coords);

        foreach (var (low, high) in component.Ranges)
        {
            if (low > val || high < val)
                continue;

            args.Handled = true;
            return;
        }
    }
}

