﻿using Microsoft.AspNetCore.Http;
using Orchard.Environment.Cache;
using Orchard.Environment.Extensions.Features;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Orchard.Environment.Cache.CacheContextProviders
{
    public class FeaturesCacheContextProvider : ICacheContextProvider
    {
        private const string FeaturesPrefix = "features:";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFeatureHash _featureHash;
        
        public FeaturesCacheContextProvider(
            IHttpContextAccessor httpContextAccessor,
            IFeatureHash featureHash
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _featureHash = featureHash;
        }

        public Task PopulateContextEntriesAsync(IEnumerable<string> contexts, List<CacheContextEntry> entries)
        {
            if (contexts.Any(ctx => String.Equals(ctx, "features", StringComparison.OrdinalIgnoreCase)))
            {
                // Add a hash of the enabled features
                var hash = _featureHash.GetFeatureHash();
                entries.Add(new CacheContextEntry("features", hash.ToString(CultureInfo.InvariantCulture)));

                // If we track any changed feature, we don't need to look into specific ones
                return Task.CompletedTask;
            }

            foreach(var context in contexts.Where(ctx => ctx.StartsWith(FeaturesPrefix, StringComparison.OrdinalIgnoreCase)))
            {
                var featureName = context.Substring(FeaturesPrefix.Length);
                var hash = _featureHash.GetFeatureHash(featureName);

                entries.Add(new CacheContextEntry("features", hash.ToString(CultureInfo.InvariantCulture)));
            }

            return Task.CompletedTask;
        }
    }
}
