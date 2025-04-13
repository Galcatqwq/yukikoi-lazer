// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using NUnit.Framework;
using osu.Game.Rulesets.Osu;
using osu.Game.Tests.Resources;

namespace osu.Game.Tests.Visual.Multiplayer
{
    public partial class TestSceneMultiplayerResults : ScreenTestScene
    {
        [Test]
        public void TestDisplayResults()
        {
            AddStep("show results screen", () =>
            {
                var rulesetInfo = new OsuRuleset().RulesetInfo;
                var beatmapInfo = CreateBeatmap(rulesetInfo).BeatmapInfo;
                var score = TestResources.CreateTestScoreInfo(beatmapInfo);
            });
        }
    }
}
