// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

/*using System;
using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Platform;
using osu.Framework.Threading;

namespace osu.Game.Graphics.UserInterface
{
    public class FPSCounterTooltipCustom : IDisposable
    {
        [Resolved]
        private GameHost host { get; set; } = null!;

        private readonly FrameSyncMultipliers multipliers = new FrameSyncMultipliers
        {
            VSync = 3, // 原值2 → 改为3倍
            Limit2X = 3, // 原值2 → 改为3倍
            Limit4X = 5, // 原值4 → 改为5倍
            Limit8X = 10 // 原值8 → 改为10倍
        };

        private ScheduledDelegate? updateDelegate;
        private PropertyInfo? maximumDrawHzProp;
        private PropertyInfo? maximumUpdateHzProp;

        [BackgroundDependencyLoader]
        private void load()
        {
            // 反射获取私有字段
            var hostType = host.GetType();
            maximumDrawHzProp = hostType.GetProperty("MaximumDrawHz", BindingFlags.Instance | BindingFlags.Public);
            maximumUpdateHzProp = hostType.GetProperty("MaximumUpdateHz", BindingFlags.Instance | BindingFlags.Public);

            // 劫持原始更新逻辑
            updateDelegate = host.Scheduler.AddDelayed(() =>
            {
                var frameSyncMode = (FrameSync)hostType.GetProperty("FrameSyncMode")!.GetValue(host)!;
                int refreshRate = getCurrentRefreshRate();

                // 应用自定义倍率
                switch (frameSyncMode)
                {
                    case FrameSync.VSync:
                        setFPSLimits(
                            int.MaxValue,
                            refreshRate * multipliers.VSync
                        );
                        break;

                    case FrameSync.Limit2x:
                        setFPSLimits(
                            refreshRate * multipliers.Limit2X,
                            refreshRate * multipliers.Limit2X * 2
                        );
                        break;

                    // 其他模式同理...
                }
            }, 0, true);
        }

        private int getCurrentRefreshRate()
        {
            var windowProp = host.GetType().GetProperty("Window");
            var window = windowProp?.GetValue(host) as IWindow;
            return window?.CurrentDisplayMode.Value.RefreshRate is > 0 ? (int)Math.Round(window.CurrentDisplayMode.Value.RefreshRate) : 60;
        }

        private void setFPSLimits(int drawHz, int updateHz)
        {
            maximumDrawHzProp?.SetValue(host, Math.Min(drawHz, maximum_sane_fps));
            maximumUpdateHzProp?.SetValue(host, Math.Min(updateHz, maximum_sane_fps));
        }

        public void Dispose() => updateDelegate?.Cancel();

        private const int maximum_sane_fps = 10000; // 与框架默认值保持一致

        private class FrameSyncMultipliers
        {
            public int VSync { get; set; }
            public int Limit2X { get; set; }
            public int Limit4X { get; set; }
            public int Limit8X { get; set; }
        }
    }
}*/