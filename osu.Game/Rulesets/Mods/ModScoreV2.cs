// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Mods
{
    /// <remarks>
    /// This mod is used strictly to mark osu!stable scores set with the "Score V2" mod active.
    /// It should not be used in any real capacity going forward.
    /// </remarks>
    public sealed class ModScoreV2 : Mod
    {
        public override string Name => "Score V2";
        public override string Acronym => @"SV2";
        public override ModType Type => ModType.System;
        public override LocalisableString Description => "stable所使用的第二代经典计分,现已在lazer上可用!(什么";
        public override double ScoreMultiplier => 1;
        public override bool UserPlayable => true;
        public override bool ValidForMultiplayer => false;
        public override bool ValidForMultiplayerAsFreeMod => false;
    }
}
