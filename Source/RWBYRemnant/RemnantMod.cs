using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class RemnantMod : Mod
    {
        RemnantModSettings remnantModSettings;

        public RemnantMod(ModContentPack content) : base(content)
        {
            this.remnantModSettings = GetSettings<RemnantModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("AuraEveryoneLabel".Translate(), ref remnantModSettings.everyoneHasAura, "AuraEveryoneTooltip".Translate());
            listingStandard.CheckboxLabeled("AuraLabel".Translate(), ref remnantModSettings.auraUnlockable, "AuraTooltip".Translate());
            listingStandard.CheckboxLabeled("SemblanceLabel".Translate(), ref remnantModSettings.semblanceUnlockable, "SemblanceTooltip".Translate());
            listingStandard.CheckboxLabeled("AmmunitionLabel".Translate(), ref remnantModSettings.autoCollectAmmunition, "AmmunitionTooltip".Translate());
            listingStandard.CheckboxLabeled("EveryoneMakesPhotosForJoyLabel".Translate(), ref remnantModSettings.everyoneMakesPhotosForJoy, "EveryoneMakesPhotosForJoyTooltip".Translate());
            listingStandard.CheckboxLabeled("HideAuraWhenMultiselectLabel".Translate(), ref remnantModSettings.hideAuraWhenMultiselect, "HideAuraWhenMultiselectTooltip".Translate());
            listingStandard.CheckboxLabeled("QrowFriendlyFireLabel".Translate(), ref remnantModSettings.qrowFriendlyFire, "QrowFriendlyFireTooltip".Translate());
            listingStandard.CheckboxLabeled("FixTraitDisabledWorkTagsLabel".Translate(), ref remnantModSettings.fixTraitDisabledWorkTags, "FixTraitDisabledWorkTagsTooltip".Translate());
            listingStandard.End();
        }

        public override string SettingsCategory()
        {
            return "RWBY World of Remnant";
        }
    }
}
