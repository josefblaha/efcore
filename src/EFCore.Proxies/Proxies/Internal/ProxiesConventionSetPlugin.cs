// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Proxies.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class ProxiesConventionSetPlugin : IConventionSetPlugin
    {
        private readonly IDbContextOptions _options;
        private readonly IProxyFactory _proxyFactory;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public ProxiesConventionSetPlugin(
            IProxyFactory proxyFactory,
            IDbContextOptions options,
            LazyLoaderParameterBindingFactoryDependencies lazyLoaderParameterBindingFactoryDependencies,
            ProviderConventionSetBuilderDependencies conventionSetBuilderDependencies)
        {
            _proxyFactory = proxyFactory;
            _options = options;
            LazyLoaderParameterBindingFactoryDependencies = lazyLoaderParameterBindingFactoryDependencies;
            ConventionSetBuilderDependencies = conventionSetBuilderDependencies;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected virtual ProviderConventionSetBuilderDependencies ConventionSetBuilderDependencies { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected virtual LazyLoaderParameterBindingFactoryDependencies LazyLoaderParameterBindingFactoryDependencies { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ConventionSet ModifyConventions(ConventionSet conventionSet)
        {
            var extension = _options.FindExtension<ProxiesOptionsExtension>();

            ConventionSet.AddAfter(
                conventionSet.ModelInitializedConventions,
                new ProxyChangeTrackingConvention(extension),
                typeof(DbSetFindingConvention));

            conventionSet.ModelFinalizingConventions.Add(
                new ProxyBindingRewriter(
                    _proxyFactory,
                    extension,
                    LazyLoaderParameterBindingFactoryDependencies,
                    ConventionSetBuilderDependencies));

            return conventionSet;
        }
    }
}
