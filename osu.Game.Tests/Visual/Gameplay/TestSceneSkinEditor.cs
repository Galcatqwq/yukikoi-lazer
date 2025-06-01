// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Testing;
using osu.Game.Database;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.SkinEditor;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Play.HUD;
using osu.Game.Screens.Play.HUD.HitErrorMeters;
using osu.Game.Skinning;
using osu.Game.Tests.Resources;
using osuTK;
using osuTK.Input;

namespace osu.Game.Tests.Visual.Gameplay
{
    public partial class TestSceneSkinEditor : PlayerTestScene
    {
        private SkinEditor skinEditor = null!;

        protected override bool Autoplay => true;

        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Blue);

        [Cached]
        public readonly EditorClipboard Clipboard = new EditorClipboard();

        [Resolved]
        private SkinManager skins { get; set; } = null!;

        private SkinComponentsContainer targetContainer => Player.ChildrenOfType<SkinComponentsContainer>().First();

        [SetUpSteps]
        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("reset skin", () => skins.CurrentSkinInfo.SetDefault());
            AddUntilStep("wait for hud load", () => targetContainer.ComponentsLoaded);

            AddStep("reload skin editor", () =>
            {
                if (skinEditor.IsNotNull())
                    skinEditor.Expire();
                Player.ScaleTo(0.4f);
                LoadComponentAsync(skinEditor = new SkinEditor(Player), Add);
            });
            AddUntilStep("wait for loaded", () => skinEditor.IsLoaded);
        }

        [Test]
        public void TestCyclicSelection()
        {
            List<SkinBlueprint> blueprints = new List<SkinBlueprint>();

            AddStep("clear list", () => blueprints.Clear());

            for (int i = 0; i < 3; i++)
            {
                AddStep("store box", () =>
                {
                    // Add blueprints one-by-one so we have a stable order for testing reverse cyclic selection against.
                    blueprints.Add(skinEditor.ChildrenOfType<SkinBlueprint>().Single(s => s.IsSelected));
                });
            }

            AddAssert("Selection is last", () => skinEditor.SelectedComponents.Single(), () => Is.EqualTo(blueprints[2].Item));

            AddStep("move cursor to black box", () =>
            {
                // Slightly offset from centre to avoid random failures (see https://github.com/ppy/osu-framework/issues/5669).
                InputManager.MoveMouseTo(((Drawable)blueprints[0].Item).ScreenSpaceDrawQuad.Centre + new Vector2(1));
            });

            AddStep("click on black box stack", () => InputManager.Click(MouseButton.Left));
            AddAssert("Selection is second last", () => skinEditor.SelectedComponents.Single(), () => Is.EqualTo(blueprints[1].Item));

            AddStep("click on black box stack", () => InputManager.Click(MouseButton.Left));
            AddAssert("Selection is last", () => skinEditor.SelectedComponents.Single(), () => Is.EqualTo(blueprints[0].Item));

            AddStep("click on black box stack", () => InputManager.Click(MouseButton.Left));
            AddAssert("Selection is first", () => skinEditor.SelectedComponents.Single(), () => Is.EqualTo(blueprints[2].Item));

            AddStep("select all boxes", () =>
            {
                skinEditor.SelectedComponents.Clear();
            });

            AddAssert("all boxes selected", () => skinEditor.SelectedComponents, () => Has.Count.EqualTo(2));
            AddStep("click on black box stack", () => InputManager.Click(MouseButton.Left));
            AddStep("click on black box stack", () => InputManager.Click(MouseButton.Left));
            AddStep("click on black box stack", () => InputManager.Click(MouseButton.Left));
            AddAssert("all boxes still selected", () => skinEditor.SelectedComponents, () => Has.Count.EqualTo(2));
        }

        [Test]
        public void TestUndoEditHistory()
        {
            SkinComponentsContainer firstTarget = null!;
            TestSkinEditorChangeHandler changeHandler = null!;
            byte[] defaultState = null!;
            IEnumerable<ISerialisableDrawable> testComponents = null!;

            AddStep("Load necessary things", () =>
            {
                firstTarget = Player.ChildrenOfType<SkinComponentsContainer>().First();
                changeHandler = new TestSkinEditorChangeHandler(firstTarget);

                changeHandler.SaveState();
                defaultState = changeHandler.GetCurrentState();

                testComponents = new[]
                {
                    targetContainer.Components.First(),
                    targetContainer.Components[targetContainer.Components.Count / 2],
                    targetContainer.Components.Last()
                };
            });

            AddStep("Press undo", () => InputManager.Keys(PlatformAction.Undo));
            AddAssert("Nothing changed", () => defaultState.SequenceEqual(changeHandler.GetCurrentState()));

            AddStep("Add components", () =>
            {
                InputManager.Click(MouseButton.Left);
                InputManager.Click(MouseButton.Left);
                InputManager.Click(MouseButton.Left);
            });
            revertAndCheckUnchanged();

            AddStep("Move components", () =>
            {
                changeHandler.BeginChange();
                testComponents.ForEach(c => ((Drawable)c).Position += Vector2.One);
                changeHandler.EndChange();
            });
            revertAndCheckUnchanged();

            AddStep("Select components", () => skinEditor.SelectedComponents.AddRange(testComponents));
            AddStep("Bring to front", () => skinEditor.BringSelectionToFront());
            revertAndCheckUnchanged();

            AddStep("Remove components", () => testComponents.ForEach(c => firstTarget.Remove(c, false)));
            revertAndCheckUnchanged();

            void revertAndCheckUnchanged()
            {
                AddStep("Revert changes", () => changeHandler.RestoreState(int.MinValue));
                AddAssert("Current state is same as default",
                    () => Encoding.UTF8.GetString(defaultState),
                    () => Is.EqualTo(Encoding.UTF8.GetString(changeHandler.GetCurrentState())));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestBringToFront(bool alterSelectionOrder)
        {
            AddAssert("Ensure over three components available", () => targetContainer.Components.Count, () => Is.GreaterThan(3));

            IEnumerable<ISerialisableDrawable> originalOrder = null!;

            AddStep("Save order of components before operation", () => originalOrder = targetContainer.Components.Take(3).ToArray());

            if (alterSelectionOrder)
                AddStep("Select first three components in reverse order", () => skinEditor.SelectedComponents.AddRange(originalOrder.Reverse()));
            else
                AddStep("Select first three components", () => skinEditor.SelectedComponents.AddRange(originalOrder));

            AddAssert("Components are not front-most", () => targetContainer.Components.TakeLast(3).ToArray(), () => Is.Not.EqualTo(skinEditor.SelectedComponents));

            AddStep("Bring to front", () => skinEditor.BringSelectionToFront());
            AddAssert("Ensure components are now front-most in original order", () => targetContainer.Components.TakeLast(3).ToArray(), () => Is.EqualTo(originalOrder));
            AddStep("Bring to front again", () => skinEditor.BringSelectionToFront());
            AddAssert("Ensure components are still front-most in original order", () => targetContainer.Components.TakeLast(3).ToArray(), () => Is.EqualTo(originalOrder));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestSendToBack(bool alterSelectionOrder)
        {
            AddAssert("Ensure over three components available", () => targetContainer.Components.Count, () => Is.GreaterThan(3));

            IEnumerable<ISerialisableDrawable> originalOrder = null!;

            AddStep("Save order of components before operation", () => originalOrder = targetContainer.Components.TakeLast(3).ToArray());

            if (alterSelectionOrder)
                AddStep("Select last three components in reverse order", () => skinEditor.SelectedComponents.AddRange(originalOrder.Reverse()));
            else
                AddStep("Select last three components", () => skinEditor.SelectedComponents.AddRange(originalOrder));

            AddAssert("Components are not back-most", () => targetContainer.Components.Take(3).ToArray(), () => Is.Not.EqualTo(skinEditor.SelectedComponents));

            AddStep("Send to back", () => skinEditor.SendSelectionToBack());
            AddAssert("Ensure components are now back-most in original order", () => targetContainer.Components.Take(3).ToArray(), () => Is.EqualTo(originalOrder));
            AddStep("Send to back again", () => skinEditor.SendSelectionToBack());
            AddAssert("Ensure components are still back-most in original order", () => targetContainer.Components.Take(3).ToArray(), () => Is.EqualTo(originalOrder));
        }

        [Test]
        public void TestToggleEditor()
        {
            AddToggleStep("toggle editor visibility", _ => skinEditor.ToggleVisibility());
        }

        [Test]
        public void TestEditComponent()
        {
            BarHitErrorMeter hitErrorMeter = null!;

            AddStep("select bar hit error blueprint", () =>
            {
                var blueprint = skinEditor.ChildrenOfType<SkinBlueprint>().First(b => b.Item is BarHitErrorMeter);

                hitErrorMeter = (BarHitErrorMeter)blueprint.Item;
                skinEditor.SelectedComponents.Clear();
                skinEditor.SelectedComponents.Add(blueprint.Item);
            });

            AddStep("move by keyboard", () => InputManager.Key(Key.Right));

            AddAssert("hitErrorMeter moved", () => hitErrorMeter.X != 0);

            AddAssert("value is default", () => hitErrorMeter.JudgementLineThickness.IsDefault);

            AddStep("hover first slider", () =>
            {
                InputManager.MoveMouseTo(
                    skinEditor.ChildrenOfType<SkinSettingsToolbox>().First()
                              .ChildrenOfType<SettingsSlider<float>>().First()
                              .ChildrenOfType<SliderBar<float>>().First()
                );
            });

            AddStep("adjust slider via keyboard", () => InputManager.Key(Key.Left));

            AddAssert("value is less than default", () => hitErrorMeter.JudgementLineThickness.Value < hitErrorMeter.JudgementLineThickness.Default);
        }

        [Test]
        public void TestCopyPaste()
        {
            AddStep("paste", () =>
            {
                InputManager.PressKey(Key.LControl);
                InputManager.Key(Key.V);
                InputManager.ReleaseKey(Key.LControl);
            });
            // no assertions. just make sure nothing crashes.

            AddStep("select bar hit error blueprint", () =>
            {
                var blueprint = skinEditor.ChildrenOfType<SkinBlueprint>().First(b => b.Item is BarHitErrorMeter);
                skinEditor.SelectedComponents.Clear();
                skinEditor.SelectedComponents.Add(blueprint.Item);
            });
            AddStep("copy", () =>
            {
                InputManager.PressKey(Key.LControl);
                InputManager.Key(Key.C);
                InputManager.ReleaseKey(Key.LControl);
            });
            AddStep("paste", () =>
            {
                InputManager.PressKey(Key.LControl);
                InputManager.Key(Key.V);
                InputManager.ReleaseKey(Key.LControl);
            });
            AddAssert("three hit error meters present",
                () => skinEditor.ChildrenOfType<SkinBlueprint>().Count(b => b.Item is BarHitErrorMeter),
                () => Is.EqualTo(3));
        }

        private SkinComponentsContainer globalHUDTarget => Player.ChildrenOfType<SkinComponentsContainer>()
                                                                 .Single(c => c.Lookup.Target == SkinComponentsContainerLookup.TargetArea.MainHUDComponents && c.Lookup.Ruleset == null);

        private SkinComponentsContainer rulesetHUDTarget => Player.ChildrenOfType<SkinComponentsContainer>()
                                                                  .Single(c => c.Lookup.Target == SkinComponentsContainerLookup.TargetArea.MainHUDComponents && c.Lookup.Ruleset != null);

        [Test]
        public void TestMigrationArgon()
        {
            Live<SkinInfo> importedSkin = null!;

            AddStep("import old argon skin", () => skins.CurrentSkinInfo.Value = importedSkin = importSkinFromArchives(@"argon-layout-version-0.osk").SkinInfo);
            AddUntilStep("wait for load", () => globalHUDTarget.ComponentsLoaded && rulesetHUDTarget.ComponentsLoaded);

            AddStep("save skin", () => skins.Save(skins.CurrentSkin.Value));

            AddStep("select another skin", () => skins.CurrentSkinInfo.SetDefault());
            AddStep("select skin again", () => skins.CurrentSkinInfo.Value = importedSkin);
            AddUntilStep("wait for load", () => globalHUDTarget.ComponentsLoaded && rulesetHUDTarget.ComponentsLoaded);
        }

        [Test]
        public void TestMigrationTriangles()
        {
            Live<SkinInfo> importedSkin = null!;

            AddStep("import old triangles skin", () => skins.CurrentSkinInfo.Value = importedSkin = importSkinFromArchives(@"triangles-layout-version-0.osk").SkinInfo);
            AddUntilStep("wait for load", () => globalHUDTarget.ComponentsLoaded && rulesetHUDTarget.ComponentsLoaded);
            AddAssert("no combo in global target", () => !globalHUDTarget.Components.OfType<DefaultComboCounter>().Any());
            AddAssert("combo placed in ruleset target", () => rulesetHUDTarget.Components.OfType<DefaultComboCounter>().Count() == 1);

            AddStep("add combo to global target", () => globalHUDTarget.Add(new DefaultComboCounter
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(2f),
            }));
            AddStep("save skin", () => skins.Save(skins.CurrentSkin.Value));

            AddStep("select another skin", () => skins.CurrentSkinInfo.SetDefault());
            AddStep("select skin again", () => skins.CurrentSkinInfo.Value = importedSkin);
            AddUntilStep("wait for load", () => globalHUDTarget.ComponentsLoaded && rulesetHUDTarget.ComponentsLoaded);
            AddAssert("combo placed in global target", () => globalHUDTarget.Components.OfType<DefaultComboCounter>().Count() == 1);
            AddAssert("combo placed in ruleset target", () => rulesetHUDTarget.Components.OfType<DefaultComboCounter>().Count() == 1);
        }

        [Test]
        public void TestMigrationLegacy()
        {
            Live<SkinInfo> importedSkin = null!;

            AddStep("import old classic skin", () => skins.CurrentSkinInfo.Value = importedSkin = importSkinFromArchives(@"classic-layout-version-0.osk").SkinInfo);
            AddUntilStep("wait for load", () => globalHUDTarget.ComponentsLoaded && rulesetHUDTarget.ComponentsLoaded);
            AddAssert("no combo in global target", () => !globalHUDTarget.Components.OfType<LegacyComboCounter>().Any());
            AddAssert("combo placed in ruleset target", () => rulesetHUDTarget.Components.OfType<LegacyComboCounter>().Count() == 1);

            AddStep("add combo to global target", () => globalHUDTarget.Add(new LegacyDefaultComboCounter
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(2f),
            }));
            AddStep("save skin", () => skins.Save(skins.CurrentSkin.Value));

            AddStep("select another skin", () => skins.CurrentSkinInfo.SetDefault());
            AddStep("select skin again", () => skins.CurrentSkinInfo.Value = importedSkin);
            AddUntilStep("wait for load", () => globalHUDTarget.ComponentsLoaded && rulesetHUDTarget.ComponentsLoaded);
            AddAssert("combo placed in global target", () => globalHUDTarget.Components.OfType<LegacyComboCounter>().Count() == 1);
            AddAssert("combo placed in ruleset target", () => rulesetHUDTarget.Components.OfType<LegacyComboCounter>().Count() == 1);
        }

        private Skin importSkinFromArchives(string filename)
        {
            var imported = skins.Import(new ImportTask(TestResources.OpenResource($@"Archives/{filename}"), filename)).GetResultSafely();
            return imported.PerformRead(skinInfo => skins.GetSkin(skinInfo));
        }

        protected override Ruleset CreatePlayerRuleset() => new OsuRuleset();

        private partial class TestSkinEditorChangeHandler : SkinEditorChangeHandler
        {
            public TestSkinEditorChangeHandler(Drawable targetScreen)
                : base(targetScreen)
            {
            }

            public byte[] GetCurrentState()
            {
                using var stream = new MemoryStream();

                WriteCurrentStateToStream(stream);
                byte[] newState = stream.ToArray();

                return newState;
            }
        }
    }
}
