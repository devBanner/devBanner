using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devBanner.Options
{
    public class BannerOptions
    {
        public int MaxSubtextLength { get; set; }

        public float MaxSubtextWidth { get; set; }

        public int MaxSubtextWraps { get; set; }

        public bool CacheAvatars { get; set; }
    }
}
