using Verse;
using RimWorld;
using AbilityUser;

namespace RWBYRemnant
{
    [DefOf]
    public static class RWBYDefOf
    {
        #region "general Defs"

        public static DamageDef Bomb_Fire;
        public static DamageDef RWBY_Burn_Blunt;
        public static DamageDef RWBY_Inflame_Blunt;
        public static DamageDef RWBY_Ice_Blunt;
        public static DamageDef Bomb_Lightning;
        public static DamageDef RWBY_Lightning_Slash;
        public static DamageDef RWBY_Lightning_Blunt;
        public static DamageDef RWBY_Lightning_Bullet;

        public static JobDef RWBY_PickUp;
        public static JobDef RWBY_StealAura;
        public static JobDef RWBY_ShootProjectileAbility;
        public static JobDef RWBY_TransformWeapon;

        public static SoundDef Shoot_Fireball;

        public static HediffDef RWBY_Thermos_Caffeine;
        public static HediffDef RWBY_Lightning_Damage;
        public static HediffDef RWBY_Ice_Damage;
        public static HediffDef RWBY_InjectedFireCrystal;
        public static HediffDef RWBY_InjectedIceCrystal;
        public static HediffDef RWBY_InjectedLightningCrystal;
        public static HediffDef RWBY_InjectedGravityCrystal;
        public static HediffDef RWBY_InjectedHardLightCrystal;
        public static HediffDef RWBY_AuraStolen;
        public static HediffDef RWBY_ApathyTiredness;
        public static HediffDef RWBY_SilverEyes;

        public static ThoughtDef RWBY_AuraStolen_Relation;

        public static ThingDef RWBY_Crocea_Mors_Shield;
        public static ThingDef Bullet_Melodic_Cudgel_Hook_Return;
        public static ThingDef FireDustCrystal;
        public static ThingDef IceDustCrystal;
        public static ThingDef LightningDustCrystal;
        public static ThingDef GravityDustCrystal;
        public static ThingDef HardLightDustCrystal;
        public static ThingDef RWBY_Grimm_Glove;

        public static FactionDef Creatures_of_Grimm;
        public static PawnKindDef Grimm_Boarbatusk;
        public static PawnKindDef Grimm_Beowolf;
        public static PawnKindDef Grimm_Ursa;
        public static PawnKindDef Grimm_Griffon;
        public static PawnKindDef Grimm_Nevermore;
        public static PawnKindDef Grimm_Lancer;
        public static PawnKindDef Grimm_LancerQueen;
        public static PawnKindDef Grimm_DeathStalker;
        public static PawnKindDef Grimm_Nuckelavee;
        public static PawnKindDef Grimm_Apathy;
        public static FleshTypeDef Grimm;

        #endregion

        #region "Character specific Defs"

        // Aura
        public static TraitDef RWBY_Aura;
        public static SoundDef AuraFlicker;
        public static SoundDef AuraBreak;
        public static SemblanceDef Ability_SilverEyes;
        public static HediffDef RWBY_SilverEye_Exhaustion;
        public static ThingDef RWBY_SilverEye_Mote;
        public static ThingDef RWBY_SilverEyes_Projectile;

        // Ruby
        public static TraitDef Semblance_Ruby;
        public static SemblanceDef Ruby_BurstIntoRosePetals;
        public static SemblanceDef Ruby_CarryPawn;
        public static ThingDef RWBY_Rose_Petal;
        public static HediffDef RWBY_RubyDashForm;
        public static ThingDef RWBY_Crescent_Rose_Rifle;
        public static ThingDef RWBY_Crescent_Rose_Scythe;
        public static ThoughtDef RWBY_RubyUsedCrescentRose;

        // Yang
        public static TraitDef Semblance_Yang;
        public static SemblanceDef Yang_ReturnDamage;
        public static HediffDef RWBY_YangReturnDamage;

        // Weiss
        public static TraitDef Semblance_Weiss;
        public static SemblanceDef Weiss_SummonBoar;
        public static SemblanceDef Weiss_SummonBeowolf;
        public static SemblanceDef Weiss_SummonUrsa;
        public static SemblanceDef Weiss_SummonGriffon;
        public static SemblanceDef Weiss_SummonNevermore;
        public static SemblanceDef Weiss_SummonLancer;
        public static SemblanceDef Weiss_SummonLancerQueen;
        public static SemblanceDef Weiss_SummonDeathStalker;
        public static SemblanceDef Weiss_SummonNuckelavee;
        public static SemblanceDef Weiss_SummonApathy;
        public static SemblanceDef Weiss_SummonArmaGigas;
        public static SemblanceDef Weiss_SummonArmaGigasSword;
        public static PawnKindDef Grimm_Boarbatusk_Summoned;
        public static PawnKindDef Grimm_Beowolf_Summoned;
        public static PawnKindDef Grimm_Ursa_Summoned;
        public static PawnKindDef Grimm_Griffon_Summoned;
        public static PawnKindDef Grimm_Nevermore_Summoned;
        public static PawnKindDef Grimm_Lancer_Summoned;
        public static PawnKindDef Grimm_LancerQueen_Summoned;
        public static PawnKindDef Grimm_DeathStalker_Summoned;
        public static PawnKindDef Grimm_Nuckelavee_Summoned;
        public static PawnKindDef Grimm_Apathy_Summoned;
        public static PawnKindDef Grimm_ArmaGigas_Summoned;
        public static PawnKindDef Grimm_ArmaGigasSword_Summoned;
        public static SemblanceDef Weiss_TimeDilationGlyph_Summon;
        public static ThingDef Weiss_Glyph_Summon;
        public static ThingDef Weiss_Glyph_TimeDilation;
        public static HediffDef RWBY_TimeDilation;

        // Blake
        public static TraitDef Semblance_Blake;
        public static ThingDef Blake_ShadowClone;
        public static SemblanceDef Blake_UseStoneClone;
        public static ThingDef Blake_ShadowClone_Stone;
        public static SemblanceDef Blake_UseIceClone;
        public static ThingDef Blake_ShadowClone_Ice;
        public static SemblanceDef Blake_UseFireClone;
        public static ThingDef Blake_ShadowClone_Fire;

        // Nora
        public static TraitDef Semblance_Nora;
        public static HediffDef RWBY_LightningBuff;

        // Jaune
        public static TraitDef Semblance_Jaune;
        public static SemblanceDef Jaune_AmplifyAura;
        public static HediffDef RWBY_AmplifiedAura;
        public static ThingDef RWBY_PumpkinPetes;
        public static ThingDef Apparel_PumpkinPetes;
        public static ThoughtDef RWBY_JauneAtePumpkinPetes;

        // Pyrrha
        public static TraitDef Semblance_Pyrrha;
        public static SemblanceDef Pyrrha_UnlockAura;
        public static SemblanceDef Pyrrha_UseMagnetism;
        public static SemblanceDef Pyrrha_MagneticPulse;
        public static ThoughtDef RWBY_PyrrhaHurtFriendly;

        // Ren
        public static TraitDef Semblance_Ren;
        public static SemblanceDef Ren_MaskEmotions;
        public static HediffDef RWBY_MaskedEmotions;

        // Qrow
        public static TraitDef Semblance_Qrow;

        // Raven
        public static TraitDef Semblance_Raven;
        public static SemblanceDef Raven_FormBond;
        public static ThingDef Raven_Portal;
        public static JobDef GoThroughPortal;

        // Cinder
        public static TraitDef Semblance_Cinder;
        public static SemblanceDef Cinder_ShootFireCrystal;
        public static SemblanceDef Cinder_SummonExplosives;
        public static ThingDef Cinder_ExplosiveMine;
        public static SemblanceDef Cinder_CreateBlades;
        public static ThingDef RWBY_Cinder_Blades;
        public static SemblanceDef Cinder_CreateBow;
        public static ThingDef RWBY_Cinder_Bow;
        public static SemblanceDef Cinder_CreateScimitar;
        public static ThingDef RWBY_Cinder_Scimitar;
        public static SemblanceDef Cinder_CreateSpear;
        public static ThingDef RWBY_Cinder_Spear;
        public static SoundDef Throw_Cinder_Spear;
        public static SoundDef Ricochet1;
        public static SoundDef Ricochet2;
        public static SoundDef Ricochet3;
        public static SoundDef Ricochet4;

        // Hazel
        public static TraitDef Semblance_Hazel;
        public static SemblanceDef Hazel_IgnorePain;
        public static HediffDef RWBY_IgnorePain;

        // Velvet
        public static TraitDef Semblance_Velvet;
        public static ThingDef RWBY_Anesidora_Camera;
        public static ThingDef RWBY_Anesidora_Box;
        public static ThoughtDef RWBY_PictureTaken;
        public static JobDef RWBY_TakePhotos;
        public static HediffDef RWBY_VelvetMimicMoves;
        public static ThingDef Bullet_Velvet_Camera;
        public static SoundDef Shot_Velvet_Camera;

        // Adam
        public static TraitDef Semblance_Adam;
        public static SemblanceDef Adam_UnleashDamage;
        public static ThingDef RWBY_Ability_Adam_Projectile;
        public static SoundDef Draw_Gambol_Shroud_Katana;

        #endregion
    }
}