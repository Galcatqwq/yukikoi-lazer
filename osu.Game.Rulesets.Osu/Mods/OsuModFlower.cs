﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Replays;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModFlower : ModAutoplay
    {
        public override string Name => "Cusordance";

        public override string Acronym => "CD";

        public override LocalisableString Description => "观看 lazer!dance";

        public override IconUsage? Icon => null;

        public override bool RequiresConfiguration => true;

        [SettingSource("Jump multiplier")]
        public BindableFloat JumpMultiplier { get; set; } = new BindableFloat
        {
            Value = 0.6f,
            Default = 0.6f,
            MinValue = 0f,
            MaxValue = 2f,
            Precision = 0.01f,
        };

        [SettingSource("Angle offset")]
        public BindableFloat AngleOffset { get; set; } = new BindableFloat
        {
            Value = 0.45f,
            Default = 0.45f,
            MinValue = 0f,
            MaxValue = 2f,
            Precision = 0.01f,
        };

        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(OsuModMagnetised), typeof(OsuModAutopilot), typeof(OsuModSpunOut), typeof(OsuModAlternate), typeof(OsuModSingleTap) }).ToArray();

        public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
            => new ModReplayData(new OsuFlowerGenerator(beatmap, mods, JumpMultiplier.Value, AngleOffset.Value).Generate(), new ModCreatedUser { Username = "lazer!dance" });
    }
}
