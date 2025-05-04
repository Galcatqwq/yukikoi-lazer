// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;

namespace osu.Game.Rulesets.Mods
{
    public class ModTouchDevice : Mod, IApplicableMod
    {
        public sealed override string Name => "触屏";
        public sealed override string Acronym => "TD";
        public sealed override IconUsage? Icon => OsuIcon.PlayStyleTouch;
        public sealed override LocalisableString Description => "我去是高贵的触屏大神.";
        public sealed override double ScoreMultiplier => 1;
        public sealed override ModType Type => ModType.System;
        public sealed override bool ValidForMultiplayer => false;
        public sealed override bool ValidForMultiplayerAsFreeMod => false;
        public sealed override bool AlwaysValidForSubmission => true;
        public override Type[] IncompatibleMods => new[] { typeof(ICreateReplayData) };
    }
}
