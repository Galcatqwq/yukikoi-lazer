// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Utils;
using osu.Framework.Testing;
using osu.Game.Rulesets.Osu.Objects.Drawables;
using osu.Game.Rulesets.Osu.UI;

namespace osu.Game.Rulesets.Osu.Tests.Mods
{
    public partial class TestSceneOsuModNoScope : OsuModTestScene
    {
        [Test]
        public void TestVisibleDuringBreak()
        {
            AddUntilStep("wait for cursor to hide", () => cursorAlphaAlmostEquals(0));
            AddUntilStep("wait for start of break", isBreak);
            AddUntilStep("wait for cursor to show", () => cursorAlphaAlmostEquals(1));
            AddUntilStep("wait for end of break", () => !isBreak());
            AddUntilStep("wait for cursor to hide", () => cursorAlphaAlmostEquals(0));
        }

        [Test]
        public void TestVisibleDuringSpinner()
        {

            AddUntilStep("wait for cursor to hide", () => cursorAlphaAlmostEquals(0));
            AddUntilStep("wait for start of spinner", isSpinning);
            AddUntilStep("wait for cursor to show", () => cursorAlphaAlmostEquals(1));
            AddUntilStep("wait for end of spinner", () => !isSpinning());
            AddUntilStep("wait for cursor to hide", () => cursorAlphaAlmostEquals(0));
        }

        [Test]
        public void TestVisibleAfterComboBreak()
        {
            AddAssert("cursor must start visible", () => cursorAlphaAlmostEquals(1));
            AddUntilStep("wait for combo", () => Player.ScoreProcessor.Combo.Value >= 2);
            AddAssert("cursor must dim after combo", () => !cursorAlphaAlmostEquals(1));
            AddStep("break combo", () => Player.ScoreProcessor.Combo.Value = 0);
            AddUntilStep("wait for cursor to show", () => cursorAlphaAlmostEquals(1));
        }

        private bool isSpinning() => Player.ChildrenOfType<DrawableSpinner>().SingleOrDefault()?.Progress > 0;

        private bool isBreak() => Player.IsBreakTime.Value;

        private OsuPlayfield playfield => (OsuPlayfield)Player.DrawableRuleset.Playfield;

        private bool cursorAlphaAlmostEquals(float alpha) =>
            Precision.AlmostEquals(playfield.Cursor.AsNonNull().Alpha, alpha, 0.1f) &&
            Precision.AlmostEquals(playfield.Smoke.Alpha, alpha, 0.1f);
    }
}
