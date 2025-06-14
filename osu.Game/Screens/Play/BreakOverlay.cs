// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Beatmaps.Timing;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using osu.Game.Screens.Play.Break;

namespace osu.Game.Screens.Play
{
    public partial class BreakOverlay : Container
    {
        /// <summary>
        /// The duration of the break overlay fading.
        /// </summary>
        public const double BREAK_FADE_DURATION = BreakPeriod.MIN_BREAK_DURATION / 2;

        private const float remaining_time_container_max_size = 0.3f;
        private const int vertical_margin = 25;

        //private readonly Container fadeContainer;

        private IReadOnlyList<BreakPeriod> breaks;

        public IReadOnlyList<BreakPeriod> Breaks
        {
            get => breaks;
            set
            {
                breaks = value;

                if (IsLoaded)
                    initializeBreaks();
            }
        }

        public override bool RemoveCompletedTransforms => false;

        private readonly Container remainingTimeAdjustmentBox = null;
        private readonly Container remainingTimeBox = null;
        private readonly RemainingTimeCounter remainingTimeCounter = null;
        private readonly BreakArrows breakArrows = null;
        private readonly ScoreProcessor scoreProcessor = null;
        private readonly BreakInfo info = null;

        protected override void LoadComplete()
        {
            base.LoadComplete();
            initializeBreaks();

            if (scoreProcessor != null)
            {
                info.AccuracyDisplay.Current.BindTo(scoreProcessor.Accuracy);
                ((IBindable<ScoreRank>)info.GradeDisplay.Current).BindTo(scoreProcessor.Rank);
            }
        }

        protected override void Update()
        {
            base.Update();

            remainingTimeBox.Height = Math.Min(8, remainingTimeBox.DrawWidth);
        }

        private void initializeBreaks()
        {
            FinishTransforms(true);
            Scheduler.CancelDelayedTasks();

            if (breaks == null) return; // we need breaks.

            foreach (var b in breaks)
            {
                if (!b.HasEffect)
                    continue;

                using (BeginAbsoluteSequence(b.StartTime))
                {
                    //fadeContainer.FadeIn(BREAK_FADE_DURATION);
                    breakArrows.Show(BREAK_FADE_DURATION);

                    remainingTimeAdjustmentBox
                        .ResizeWidthTo(remaining_time_container_max_size, BREAK_FADE_DURATION, Easing.OutQuint)
                        .Delay(b.Duration - BREAK_FADE_DURATION)
                        .ResizeWidthTo(0);

                    remainingTimeBox
                        .ResizeWidthTo(0, b.Duration - BREAK_FADE_DURATION)
                        .Then()
                        .ResizeWidthTo(1);

                    remainingTimeCounter.CountTo(b.Duration).CountTo(0, b.Duration);

                    using (BeginDelayedSequence(b.Duration - BREAK_FADE_DURATION))
                    {
                        //fadeContainer.FadeOut(BREAK_FADE_DURATION);
                        breakArrows.Hide(BREAK_FADE_DURATION);
                    }
                }
            }
        }
    }
}
