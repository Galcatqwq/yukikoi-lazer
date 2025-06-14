// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Allocation;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Resources.Localisation.Web;
using osuTK.Graphics;

namespace osu.Game.Overlays.BeatmapListing
{
    public partial class BeatmapSearchGeneralFilterRow : BeatmapSearchMultipleSelectionFilterRow<SearchGeneral>
    {
        public BeatmapSearchGeneralFilterRow()
            : base(BeatmapsStrings.ListingSearchFiltersGeneral)
        {
        }

        protected override MultipleSelectionFilter CreateMultipleSelectionFilter() => new GeneralFilter();

        private partial class GeneralFilter : MultipleSelectionFilter
        {
            protected override MultipleSelectionFilterTabItem CreateTabItem(SearchGeneral value)
            {
                if (value == SearchGeneral.FeaturedArtists)
                    return new FeaturedArtistsTabItem();

                return new MultipleSelectionFilterTabItem(value);
            }
        }

        private partial class FeaturedArtistsTabItem : MultipleSelectionFilterTabItem
        {
            //private Bindable<bool> disclaimerShown;

            public FeaturedArtistsTabItem()
                : base(SearchGeneral.FeaturedArtists)
            {
            }

            [Resolved]
            private OsuColour colours { get; set; }

            [Resolved]
            private SessionStatics sessionStatics { get; set; }

            [Resolved(canBeNull: false)]
            private IDialogOverlay dialogOverlay { get; set; }

            /*protected override void LoadComplete()
            {
                base.LoadComplete();

                disclaimerShown = sessionStatics.GetBindable<bool>(Static.FeaturedArtistDisclaimerShownOnce);
            }*/

            protected override Color4 ColourNormal => colours.Orange1;
            protected override Color4 ColourActive => colours.Orange2;

            /*protected override bool OnClick(ClickEvent e)
            {
                if (!disclaimerShown.Value && dialogOverlay != null)
                {
                    return true;
                }

                return base.OnClick(e);
            }*/
        }
    }
}
