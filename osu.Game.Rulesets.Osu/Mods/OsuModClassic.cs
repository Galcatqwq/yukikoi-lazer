// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Objects.Drawables;
using osu.Game.Rulesets.Osu.Scoring;
using osu.Game.Rulesets.Osu.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModClassic : ModClassic, IApplicableToHitObject, IApplicableToDrawableHitObject, IApplicableToDrawableRuleset<OsuHitObject>, IApplicableHealthProcessor
    {
        public override Type[] IncompatibleMods => base.IncompatibleMods.Append(typeof(OsuModStrictTracking)).ToArray();

        [SettingSource("去除滑条头的准确率要求", "滑条分数与其命中的滑条刻成比例.")]
        public Bindable<bool> NoSliderHeadAccuracy { get; } = new BindableBool(true);

        [SettingSource("应用v1 note锁", "将note锁应用与完整的打击窗口.")]
        public Bindable<bool> ClassicNoteLock { get; } = new BindableBool(true);

        [SettingSource("永远播放滑条尾音效", "总是播放滑条尾音效，无论该滑条是否已经滑完.")]
        public Bindable<bool> AlwaysPlayTailSample { get; } = new BindableBool(true);

        [SettingSource("Fade out hit circles earlier", "Make hit circles fade out into a miss, rather than after it.")]
        public Bindable<bool> FadeHitCircleEarly { get; } = new Bindable<bool>(true);

        [SettingSource("经典血量机制", "More closely resembles the original HP drain mechanics.")]
        public Bindable<bool> ClassicHealth { get; } = new Bindable<bool>(true);

        private bool usingHiddenFading;

        public void ApplyToHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case Slider slider:
                    slider.ClassicSliderBehaviour = NoSliderHeadAccuracy.Value;
                    break;
            }
        }

        public void ApplyToDrawableRuleset(DrawableRuleset<OsuHitObject> drawableRuleset)
        {
            var osuRuleset = (DrawableOsuRuleset)drawableRuleset;

            if (ClassicNoteLock.Value)
            {
                double hittableRange = OsuHitWindows.MISS_WINDOW - (drawableRuleset.Mods.OfType<OsuModAutopilot>().Any() ? 200 : 0);
                osuRuleset.Playfield.HitPolicy = new LegacyHitPolicy(hittableRange);
            }

            usingHiddenFading = drawableRuleset.Mods.OfType<OsuModHidden>().SingleOrDefault()?.OnlyFadeApproachCircles.Value == false;
        }

        public void ApplyToDrawableHitObject(DrawableHitObject obj)
        {
            switch (obj)
            {
                case DrawableSliderHead head:
                    if (FadeHitCircleEarly.Value && !usingHiddenFading)
                        applyEarlyFading(head);

                    if (ClassicNoteLock.Value)
                        blockInputToObjectsUnderSliderHead(head);

                    break;

                case DrawableSliderTail tail:
                    tail.SamplePlaysOnlyOnHit = !AlwaysPlayTailSample.Value;
                    break;

                case DrawableHitCircle circle:
                    if (FadeHitCircleEarly.Value && !usingHiddenFading)
                        applyEarlyFading(circle);

                    break;
            }
        }

        /// <summary>
        /// On stable, slider heads that have already been hit block input from reaching objects that may be underneath them
        /// until the sliders they're part of have been fully judged.
        /// The purpose of this method is to restore that behaviour.
        /// In order to avoid introducing yet another confusing config option, this behaviour is roped into the general notion of "note lock".
        /// </summary>
        private static void blockInputToObjectsUnderSliderHead(DrawableSliderHead slider)
        {
            slider.HitArea.CanBeHit = () => !slider.DrawableSlider.AllJudged;
        }

        private void applyEarlyFading(DrawableHitCircle circle)
        {
            circle.ApplyCustomUpdateState += (dho, state) =>
            {
                using (dho.BeginAbsoluteSequence(dho.StateUpdateTime))
                {
                    if (state != ArmedState.Hit)
                    {
                        double okWindow = dho.HitObject.HitWindows.WindowFor(HitResult.Ok);
                        double lateMissFadeTime = dho.HitObject.HitWindows.WindowFor(HitResult.Meh) - okWindow;
                        dho.Delay(okWindow).FadeOut(lateMissFadeTime);
                    }
                }
            };
        }

        public HealthProcessor? CreateHealthProcessor(double drainStartTime) => ClassicHealth.Value ? new OsuLegacyHealthProcessor(drainStartTime) : null;
    }
}
