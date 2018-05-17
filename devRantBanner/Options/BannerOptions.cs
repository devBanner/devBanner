namespace devBanner.Options
{
    public class BannerOptions
    {
        public int MaxSubtextLength { get; set; }

        public float MaxSubtextWidth { get; set; }

        public int MaxSubtextWraps { get; set; }

        public bool CacheAvatars { get; set; }

        // In minutes
        public int MaxCacheAvatarAge { get; set; }

        public float WidthToHeightRatio { get; set; }
    }
}
