// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Shuttles.Components;

namespace Content.Server.Shuttles.Events;

/// <summary>
/// Raised whenever 2 airlocks dock.
/// </summary>
public sealed class DockEvent : EntityEventArgs
{
    public DockingComponent DockA = default!;
    public DockingComponent DockB = default!;

    public EntityUid GridAUid = default!;
    public EntityUid GridBUid = default!;
}