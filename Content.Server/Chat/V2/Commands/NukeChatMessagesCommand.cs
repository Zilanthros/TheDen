// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using System.Diagnostics;
using Content.Server.Administration;
using Content.Server.Chat.V2.Repository;
using Content.Shared.Administration;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Content.Server.Chat.V2.Commands;

[ToolshedCommand, AdminCommand(AdminFlags.Admin)]
public sealed class NukeChatMessagesCommand : ToolshedCommand
{
    [Dependency] private readonly IEntitySystemManager _manager = default!;

    [CommandImplementation("usernames")]
    public void Command(IInvocationContext ctx, string usernamesCsv)
    {
        var usernames = usernamesCsv.Split(',');

        foreach (var username in usernames)
        {
            if (!_manager.GetEntitySystem<ChatRepositorySystem>().NukeForUsername(username, out var reason))
            {
                ctx.ReportError(new NukeMessagesForUsernameError(reason));
            }
        }
    }
}

public record struct NukeMessagesForUsernameError(string Reason) : IConError
{
    public FormattedMessage DescribeInner()
    {
        return FormattedMessage.FromUnformatted(Reason);
    }

    public string? Expression { get; set; }
    public Vector2i? IssueSpan { get; set; }
    public StackTrace? Trace { get; set; }
}
