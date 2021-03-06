﻿using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Features;
using Orchard.Environment.Shell.Descriptor;
using Orchard.Environment.Shell.Descriptor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Orchard.Environment.Shell
{
    public class ShellFeaturesManager : IShellFeaturesManager
    {
        private readonly IExtensionManager _extensionManager;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IShellDescriptorFeaturesManager _shellDescriptorFeaturesManager;

        public ShellFeaturesManager(
            IExtensionManager extensionManager,
            IShellDescriptorManager shellDescriptorManager,
            IShellDescriptorFeaturesManager shellDescriptorFeaturesManager)
        {
            _extensionManager = extensionManager;
            _shellDescriptorManager = shellDescriptorManager;
            _shellDescriptorFeaturesManager = shellDescriptorFeaturesManager;
        }

        private Task<ShellDescriptor> GetCurrentShell() {
            return _shellDescriptorManager.GetShellDescriptorAsync();
        }

        public async Task<IEnumerable<IFeatureInfo>> GetEnabledFeaturesAsync()
        {
            var shellDescriptor = await GetCurrentShell();

            return _extensionManager.GetFeatures(
                shellDescriptor.Features.Select(x => x.Id).ToArray());
        }

        public async Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(IEnumerable<IFeatureInfo> features)
        {
            return await _shellDescriptorFeaturesManager.EnableFeaturesAsync(await GetCurrentShell(), features);
        }

        public async Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(IEnumerable<IFeatureInfo> features, bool force)
        {
            return await _shellDescriptorFeaturesManager.EnableFeaturesAsync(await GetCurrentShell(), features, force);
        }

        public async Task<IEnumerable<IFeatureInfo>> GetDisabledFeaturesAsync()
        {
            var shellDescriptor = await GetCurrentShell();

            var enabledFeatures = _extensionManager.GetFeatures(
                shellDescriptor.Features.Select(x => x.Id).ToArray());

            var allFeatures = _extensionManager.GetFeatures();

            return allFeatures.Except(enabledFeatures);
        }

        public async Task<IEnumerable<IFeatureInfo>> DisableFeaturesAsync(IEnumerable<IFeatureInfo> features)
        {
            return await _shellDescriptorFeaturesManager.DisableFeaturesAsync(await GetCurrentShell(), features);
        }

        public async Task<IEnumerable<IFeatureInfo>> DisableFeaturesAsync(IEnumerable<IFeatureInfo> features, bool force)
        {
            return await _shellDescriptorFeaturesManager.DisableFeaturesAsync(await GetCurrentShell(), features, force);
        }
    }
}
