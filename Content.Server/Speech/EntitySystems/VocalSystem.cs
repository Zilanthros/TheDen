// SPDX-FileCopyrightText: 2023 Alex Evgrashin
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT
// SPDX-FileCopyrightText: 2024 Mnemotechnican
// SPDX-FileCopyrightText: 2025 Cam
// SPDX-FileCopyrightText: 2025 Tabitha
// SPDX-FileCopyrightText: 2025 VMSolidus
// SPDX-FileCopyrightText: 2025 sleepyyapril
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Server._DEN.Vocal;
using Content.Server.Actions;
using Content.Server.Chat.Systems;
using Content.Server.Speech.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.CCVar;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Humanoid;
using Content.Shared.Speech;
using Content.Shared.Speech.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems;

public sealed class VocalSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly ActionBlockerSystem _actionBlocker = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly AdditionalVocalSoundsSystem _additionalVocalSounds = default!;

    [ValidatePrototypeId<ReplacementAccentPrototype>]
    private const string MuzzleAccent = "mumble";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VocalComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<VocalComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<VocalComponent, SexChangedEvent>(OnSexChanged);
        SubscribeLocalEvent<VocalComponent, EmoteEvent>(OnEmote);
        SubscribeLocalEvent<VocalComponent, ScreamActionEvent>(OnScreamAction);
    }

    private void OnMapInit(EntityUid uid, VocalComponent component, MapInitEvent args)
    {
        if (_config.GetCVar(CCVars.AllowScreamAction))
            _actions.AddAction(uid, ref component.ScreamActionEntity, component.ScreamAction);

        LoadSounds(uid, component);
    }

    private void OnShutdown(EntityUid uid, VocalComponent component, ComponentShutdown args)
    {
        // remove scream action when component removed
        if (component.ScreamActionEntity != null)
        {
            _actions.RemoveAction(uid, component.ScreamActionEntity);
        }
    }

    private void OnSexChanged(EntityUid uid, VocalComponent component, SexChangedEvent args)
    {
        LoadSounds(uid, component, args.NewSex);
    }

    private void OnEmote(EntityUid uid, VocalComponent component, ref EmoteEvent args)
    {
        if (args.Handled
            || !args.Emote.Category.HasFlag(EmoteCategory.Vocal)
            || !_actionBlocker.CanSpeak(uid)
            || TryComp<ReplacementAccentComponent>(uid, out var replacement) && replacement.Accent == MuzzleAccent)
            return;

        var sounds = component.EmoteSounds?.Sounds;

        if (TryComp<AdditionalVocalSoundsComponent>(uid, out var additionalVocalSounds))
            sounds = _additionalVocalSounds.GetVocalSounds((uid, additionalVocalSounds), component.EmoteSounds);

        // snowflake case for wilhelm scream easter egg
        if (args.Emote.ID == component.ScreamId)
        {
            args.Handled = TryPlayScreamSound(uid, component);
            return;
        }

        // just play regular sound based on emote proto
        if (sounds == null)
            args.Handled = _chat.TryPlayEmoteSound(uid, component.EmoteSounds, args.Emote.ID);
        else
            args.Handled = _chat.TryPlayEmoteSound(uid, sounds, args.Emote.ID, component.EmoteSounds?.GeneralParams);
    }

    private void OnScreamAction(EntityUid uid, VocalComponent component, ScreamActionEvent args)
    {
        if (args.Handled || !_config.GetCVar(CCVars.AllowScreamAction))
            return;

        _chat.TryEmoteWithChat(uid, component.ScreamId);
        args.Handled = true;
    }

    private bool TryPlayScreamSound(EntityUid uid, VocalComponent component)
    {
        if (_random.Prob(component.WilhelmProbability))
        {
            _audio.PlayPvs(component.Wilhelm, uid, component.Wilhelm.Params);
            return true;
        }

        return _chat.TryPlayEmoteSound(uid, component.EmoteSounds, component.ScreamId);
    }

    private void LoadSounds(EntityUid uid, VocalComponent component, Sex? sex = null)
    {
        if (component.Sounds == null)
            return;

        sex ??= CompOrNull<HumanoidAppearanceComponent>(uid)?.Sex ?? Sex.Unsexed;

        if (!component.Sounds.TryGetValue(sex.Value, out var protoId))
            return;
        _proto.TryIndex(protoId, out component.EmoteSounds);
    }
}
