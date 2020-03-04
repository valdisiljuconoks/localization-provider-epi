using System.Globalization;
using DbLocalizationProvider.AdminUI;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.EPiServer.Sample.Models;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.EPiServer.Sample
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class InitLocalization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            ConfigurationContext.Setup(_ =>
                                       {
                                           _.DiagnosticsEnabled = true;
                                           _.EnableLegacyMode = () => true;
                                           _.CustomAttributes = new[]
                                                                {
                                                                    new CustomAttributeDescriptor(typeof(HelpTextAttribute), false)
                                                                };

                                           _.ForeignResources.Add(typeof(VersionStatus));
                                           _.EnableInvariantCultureFallback = true;
                                           _.DefaultResourceCulture = CultureInfo.InvariantCulture;
                                           _.CacheManager.OnRemove += CacheManagerOnOnRemove;
                                           _.PopulateCacheOnStartup = false;
                                       });

            UiConfigurationContext.Setup(_ =>
                                         {
                                             //_.DefaultView = ResourceListView.Tree;
                                             _.TreeViewExpandedByDefault = true;
                                             _.ShowInvariantCulture = true;

                                             //_.AuthorizedAdminRoles.Clear();
                                             _.AuthorizedAdminRoles.Add("SomeFancyAdminRole");

                                             //_.AuthorizedEditorRoles.Clear();
                                             _.AuthorizedEditorRoles.Add("SomeFancyEditorRole");

                                             //_.DisableView(ResourceListView.Table);

                                             _.DisableRemoveTranslationButton = true;

                                             _.Events.OnNewResourceCreated += OnNewResourceCreated;
                                         });
        }

        private void OnNewResourceCreated(CreateNewResources.EventArgs args)
        {
            var translation = LocalizationProvider.Current.GetString(args.Key);
            SampleSelectionFactory.AddNewValue(args.Key, translation);
        }

        public void Uninitialize(InitializationEngine context)
        {
            ConfigurationContext.Current.CacheManager.OnRemove -= CacheManagerOnOnRemove;
        }

        private void CacheManagerOnOnRemove(CacheEventArgs cacheEventArgs) { }
    }
}
