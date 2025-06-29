// SPDX-FileCopyrightText: 2022 0x6273 <0x40@keemail.me>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <drsmugleaf@gmail.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+emogarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 SimpleStation14 <130339894+SimpleStation14@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using System.Threading;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Storage;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Medical.BiomassReclaimer
{
    [RegisterComponent]
    public sealed partial class BiomassReclaimerComponent : Component
    {
        /// <summary>
        ///     This gets set for each mob it processes.
        ///     When it hits 0, there is a chance for the reclaimer to either spill blood or throw an item.
        /// </summary>
        [ViewVariables]
        public float RandomMessTimer = 0f;

        /// <summary>
        ///     The interval for <see cref="RandomMessTimer"/>.
        /// </summary>
        [DataField]
        public TimeSpan RandomMessInterval = TimeSpan.FromSeconds(5);

        /// <summary>
        ///     This gets set for each mob it processes.
        ///     When it hits 0, spit out biomass.
        /// </summary>
        [ViewVariables]
        public float ProcessingTimer;

        /// <summary>
        ///     Amount of biomass that the mob being processed will yield.
        ///     This is calculated from the YieldPerUnitMass.
        ///     Also stores non-integer leftovers.
        /// </summary>
        [ViewVariables]
        public float CurrentExpectedYield;

        /// <summary>
        ///     The reagent that will be spilled while processing a mob.
        /// </summary>
        [ViewVariables]
        public string? BloodReagent;

        /// <summary>
        ///     Entities that can be randomly spawned while processing a mob.
        /// </summary>
        public List<EntitySpawnEntry> SpawnedEntities = new();

        /// <summary>
        ///     How many units of biomass it produces for each unit of mass.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        public float YieldPerUnitMass = default;

        /// <summary>
        ///     The base yield per mass unit when no components are upgraded.
        /// </summary>
        [DataField]
        public float BaseYieldPerUnitMass = 0.4f;

        /// <summary>
        ///     Machine part whose rating modifies the yield per mass.
        /// </summary>
        [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<MachinePartPrototype>))]
        public string MachinePartYieldAmount = "MatterBin";

        /// <summary>
        ///     How much the machine part quality affects the yield.
        ///     Going up a tier will multiply the yield by this amount.
        /// </summary>
        [DataField]
        public float PartRatingYieldAmountMultiplier = 1.25f;

        /// <summary>
        ///     How many seconds to take to insert an entity per unit of its mass.
        /// </summary>
        [DataField]
        public float BaseInsertionDelay = 0.1f;

        /// <summary>
        ///     How much to multiply biomass yield from botany produce.
        /// </summary>
        [DataField]
        public float ProduceYieldMultiplier = 0.25f;

        /// <summary>
        ///     The time it takes to process a mob, per mass.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        public float ProcessingTimePerUnitMass;

        /// <summary>
        ///     The base time per mass unit that it takes to process a mob
        ///     when no components are upgraded.
        /// </summary>
        [DataField]
        public float BaseProcessingTimePerUnitMass = 0.5f;

        /// <summary>
        ///     The machine part that increses the processing speed.
        /// </summary>
        [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<MachinePartPrototype>))]
        public string MachinePartProcessingSpeed = "Manipulator";

        /// <summary>
        ///     How much the machine part quality affects the yield.
        ///     Going up a tier will multiply the speed by this amount.
        /// </summary>
        [DataField]
        public float PartRatingSpeedMultiplier = 1.35f;

        /// <summary>
        ///     Will this refuse to gib a living mob?
        /// </summary>
        [DataField]
        public bool SafetyEnabled = true;
    }
}
