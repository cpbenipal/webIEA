using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webIEA.Contracts;
using webIEA.Repositories;

namespace webIEA
{
    public class NinjectDependencyResolver : NinjectDependencyScope, 
        System.Web.Http.Dependencies.IDependencyResolver, 
        System.Web.Mvc.IDependencyResolver
    {
        private IKernel _kernel;
        public NinjectDependencyResolver(IKernel kernel) : base(kernel)
        {
            _kernel = kernel;
            _kernel.Unbind<System.Web.Mvc.ModelValidatorProvider>();
            bind();
        }

        public System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(_kernel.BeginBlock());
        }

        // Random crypto initialisation string. 
        // NEVER EVER change it or all passwords in the database will become invalid.
        private const string _crypInit = "Web log-in value";

        private void bind()
        {
            // extensions
            //_kernel.Bind<Flexpage.Abstract.Extensions.ICmsExtension>().To(typeof(Flexpage.Service.Extensions.CmsExtensionCounter));

            // primary services
            _kernel.Bind<Pluritech.Authentication.Abstract.IUser>().To(typeof(Pluritech.Pluriworks.Service.PluriworksUser));
            _kernel.Bind<Pluritech.Caching.Abstract.ICache>().To(typeof(Pluritech.Caching.HttpContextCache));
            _kernel.Bind<Pluritech.FileSystem.Abstract.IFileContentProcessor>().To(typeof(Pluritech.Pluriworks.Service.FileContentProcessor));
            _kernel.Bind<Pluritech.FileSystem.Abstract.IPathSystem>().To(typeof(Pluritech.Pluriworks.Service.FileContentProcessor));
            _kernel.Bind<Pluritech.Properties.Abstract.IPropertyProvider>().To(typeof(Pluritech.Pluriworks.Service.PropertiesProvider));
            _kernel.Bind<Pluritech.FileSystem.Abstract.IFileConverter>().To(typeof(Pluritech.Pluriworks.Service.FileConverter));
            _kernel.Bind<Pluritech.Shared.Abstract.IObjectQueryBuilder>().To(typeof(Pluritech.Pluriworks.Service.FilesQueryBuilder)).Named("FilesQueryBuilder");
            _kernel.Bind<Pluritech.Shared.Abstract.IObjectQueryBuilder>().To(typeof(Pluritech.Pluriworks.Service.FoldersQueryBuilder)).Named("FoldersQueryBuilder");
            _kernel.Bind<Pluritech.Shared.Abstract.IObjectQueryBuilder>().To(typeof(Pluritech.Pluriworks.Service.ContactsQueryBuilder)).Named("ContactsQueryBuilder");
            _kernel.Bind<Pluritech.Search.Abstract.ISearchFacility>().To(typeof(Flexpage.Business.Search.PageSearchFacility)).Named("PageSearch");
            _kernel.Bind<Pluritech.Search.Abstract.ISearchFacility>().To(typeof(Pluritech.Pluriworks.Service.FileContentSearchFacility)).Named("FileContentSearchFacility");
            _kernel.Bind<Pluritech.Search.Abstract.ISearchFacility>().To(typeof(Pluritech.Pluriworks.Service.FileCustomPropertiesSearchFacility)).Named("FileCustomPropertiesSearchFacility");
            _kernel.Bind<Pluritech.Search.Abstract.ISearchFacility>().To(typeof(Pluritech.Pluriworks.Service.FileNameSearchFacility)).Named("FileNameSearchFacility");
            
            _kernel.Bind<Pluritech.Services.FileContentStorage.IFileContentStorage>().To(typeof(Pluritech.Services.FileContentStorage.HashedFileContentStorage));
            _kernel.Bind<Pluritech.Settings.Abstract.ISettings>().To(typeof(Pluritech.AppSettingsProvider.AppSettings));
            _kernel.Bind<Pluritech.Settings.Abstract.ILocalization>().To(typeof(Flexpage.Service.LocalizationService));
            _kernel.Bind<Pluritech.Settings.Abstract.IMapPath>().To(typeof(Pluritech.Settings.ServerPathMapper));
            _kernel.Bind<Flexpage.Abstract.IFlexpageSettings>().To(typeof(Flexpage.Service.FlexpageSettings));
            _kernel.Bind<Flexpage.Abstract.IFlexpage>().To(typeof(Flexpage.Service.Flexpage));
            _kernel.Bind<Flexpage.Service.Abstract.IFlexpageService>().To<Flexpage.Service.FlexpageService>();
            _kernel.Bind<Flexpage.Service.Abstract.IBlockManager>().To<Flexpage.Service.BlockManager>();

            _kernel.Bind<Pluritech.Permissions.Abstract.IPermissionsService>().To(typeof(Pluritech.Pluriworks.Service.PermissionsProvider));
            _kernel.Bind<Pluritech.Authentication.Abstract.IRoleProvider>().To(typeof(Pluritech.Pluriworks.Service.PluriworksRoleProvider));
            _kernel.Bind<Pluritech.Authentication.Abstract.IAuthProvider>().To(typeof(Pluritech.Pluriworks.Service.FormsAuthProvider));
            _kernel.Bind<Pluritech.Contact.Abstract.IContactProvider>().To(typeof(Pluritech.Pluriworks.Service.ContactProvider));
            _kernel.Bind<IObserver<Pluritech.FileSystem.Abstract.DTO.FileOperation>>().To(typeof(Pluritech.Pluriworks.Service.FilePublicationObserver));
            //_kernel.Bind<IObserver<Pluritech.FileSystem.Abstract.DTO.FileOperation>>().To(typeof(Pluritech.Pluritech.Events.Service.FileSystemObserver));

            _kernel.Bind<Pluritech.Pluriworks.Service.Abstract.IServiceObjectConverter>().To(typeof(Pluritech.Pluriworks.Service.Converters.ServiceObjectConverter));
            _kernel.Bind<Pluritech.Pluriworks.Service.Abstract.IServicePropertyConverter>().To(typeof(Pluritech.Pluriworks.Service.Converters.ServicePropertyConverter));
            _kernel.Bind<Pluritech.Pluriworks.Service.Abstract.IServiceContactConverter>().To(typeof(Pluritech.Pluriworks.Service.Converters.ServiceContactConverter));

            _kernel.Bind<Pluritech.UserProfile.Abstract.IUserGeneralPermissionProvider>().To(typeof(Pluritech.Pluriworks.Service.UserGeneralPermissionProvider));
            _kernel.Bind<Pluritech.UserProfile.Abstract.IUserProfileProvider>().To(typeof(Pluritech.Pluriworks.Service.UserProfileProvider));

            // Don't enable plurishell for now
            //_kernel.Bind<Pluritech.PlurishellShared.Abstract.IPluriShellService>().To(typeof(Pluritech.PluriShell.Service.PluriShellService));
            //_kernel.Bind<Pluritech.PluriShell.Service.Abstract.IClientProcessor>().To(typeof(Pluritech.PluriShell.Service.ClientProcessor));
            //_kernel.Bind<IObserver<Pluritech.FileSystem.Abstract.DTO.FileOperation>>().To(typeof(Pluritech.PluriShell.Service.FileSystemObserver));
            //_kernel.Bind<IObserver<Pluritech.Contact.Abstract.DTO.ContactOperation>>().To(typeof(Pluritech.PluriShell.Service.PermissionsObserver));
           
            // TODO: do NOT handle file operations in PermissionsObserver, there is a cycle reference in this case
            //_kernel.Bind<IObserver<Pluritech.FileSystem.Abstract.DTO.FileOperation>>().To(typeof(Pluritech.PluriShell.Service.PermissionsObserver));
            //_kernel.Bind<IObserver<Pluritech.Properties.Abstract.DTO.CustomPropertyOperation>>().To(typeof(Pluritech.PluriShell.Service.PermissionsObserver));
            //_kernel.Bind<IObserver<Pluritech.Permissions.Abstract.DTO.ObjectPermissionOperation>>().To(typeof(Pluritech.PluriShell.Service.PermissionsObserver));
            //_kernel.Bind<Pluritech.TaskProcessor.Abstract.IServiceTask>().To(typeof(Pluritech.PluriShell.Service.DirtyNodesProcessor));
            //_kernel.Bind<Pluritech.TaskProcessor.Abstract.IServiceTask>().To(typeof(Pluritech.PluriShell.Service.DirtyAreaProcessor));

            _kernel.Bind<Pluritech.Pluriworks.Service.Abstract.IEnumProcessor>().To(typeof(Pluritech.Pluriworks.Service.EnumProcessor));
            _kernel.Bind<Pluritech.Pluriworks.Service.Abstract.IObjectProcessor>().To(typeof(Pluritech.Pluriworks.Service.ObjectProcessor));
            _kernel.Bind<Pluritech.Pluriworks.Service.Abstract.IFileSearchProcessor>().To(typeof(Pluritech.Pluriworks.Service.FileSearchProcessor));
            _kernel.Bind<Pluritech.Pluriworks.Service.Abstract.ILinkTypeManagerProvider>().To(typeof(Pluritech.Pluriworks.Service.LinkTypeManagerProvider));

            _kernel.Bind<Pluritech.Log.Abstract.ILogging>().To(typeof(Pluritech.EventLog.Logging.SystemLogging));
            _kernel.Bind<Pluritech.TaskProcessor.Abstract.IServiceTask>().To(typeof(Pluritech.Pluriworks.Service.FileSearchService));
            _kernel.Bind<Pluritech.TaskProcessor.Abstract.IServiceTask>().To(typeof(Pluritech.Pluriworks.Service.FileSearchCleaningService));

            //_kernel.Bind<Pluritech.TaskProcessor.Abstract.ITaskManager>().To(typeof(Pluritech.TaskProcessor.Service.ThreadsManager)).Named("ThreadsManager");
            //_kernel.Bind<Pluritech.TaskProcessor.Abstract.ITaskManager>().To(typeof(Pluritech.TaskProcessor.Service.TaskScheduler)).Named("QuartzManager");
            //_kernel.Bind<Pluritech.TaskProcessor.Abstract.ITask>().To(typeof(Pluritech.TaskProcessor.Service.FileTask));
            //_kernel.Bind<Pluritech.TaskProcessor.Abstract.ITask>().To(typeof(Pluritech.TaskProcessor.Service.LongFileTask));
            //_kernel.Bind<Pluritech.TaskProcessor.Abstract.ITask>().To(typeof(Pluritech.Notifications.Service.FileNotificationTask));

            _kernel.Bind<Pluritech.Notifications.Abstract.INotificationsService>().To(typeof(Pluritech.Notifications.Service.NotificationsService));
            //_kernel.Bind<Pluritech.ContactSync.Abstract.IContactSyncService>().To(typeof(Pluritech.ContactSync.Service.ContactSyncService));
            //_kernel.Bind<IObserver<Pluritech.Contact.Abstract.DTO.ContactOperation>>().To(typeof(Pluritech.ContactSync.Service.ContactProviderObserver));
            //_kernel.Bind<IObserver<Pluritech.FileSystem.Abstract.DTO.FileOperation>>().To(typeof(Pluritech.ContactSync.Service.ContactProviderObserver));
            //_kernel.Bind<IObserver<Pluritech.Properties.Abstract.DTO.CustomPropertyOperation>>().To(typeof(Pluritech.ContactSync.Service.ContactProviderObserver));

            //_kernel.Bind<Pluritech.ContactSync.Abstract.IContactSyncProvider>().To(typeof(Pluritech.ContactSync.Service.ContactSyncProvider));

            _kernel.Bind<Flexpage.Abstract.IWizardProvider>().To(typeof(Flexpage.Service.WizardProvider));
            _kernel.Bind<Flexpage.Abstract.ISitemapProvider>().To(typeof(Flexpage.Service.SitemapProvider));
            _kernel.Bind<Flexpage.Abstract.IBlockProvider>().To(typeof(Flexpage.Service.BlockProvider));
            _kernel.Bind<Flexpage.Abstract.ISitemapMenuBlockProvider>().To(typeof(Flexpage.Service.SitemapMenuBlockProvider));
            _kernel.Bind<Flexpage.Abstract.IBreadcrumbsBlockProvider>().To(typeof(Flexpage.Service.BreadcrumbsBlockProvider));
            _kernel.Bind<Flexpage.Abstract.ISitemapTreeViewBlockProvider>().To(typeof(Flexpage.Service.SitemapTreeViewBlockProvider));
            _kernel.Bind<Flexpage.Abstract.ILocalizedModelProvider>().To(typeof(Flexpage.Service.LocalizedModelProvider));
            _kernel.Bind<Flexpage.Abstract.IPageProvider>().To(typeof(Flexpage.Service.PageProvider));
            _kernel.Bind<Flexpage.Abstract.IPageAccessProvider>().To(typeof(Flexpage.Service.PageAccessProvider));
            _kernel.Bind<Flexpage.Abstract.ILogProvider>().To(typeof(Flexpage.Service.LogProvider));
            _kernel.Bind<Flexpage.Abstract.IUserSettingsProvider>().To(typeof(Flexpage.Service.UserSettingsProvider));
            _kernel.Bind<Flexpage.Abstract.INewsBlockProvider>().To(typeof(Flexpage.Service.NewsBlockProvider));
            _kernel.Bind<Flexpage.Abstract.IEventsProvider>().To(typeof(Flexpage.Service.EventsProvider));
            _kernel.Bind<Flexpage.Abstract.IScheduleProvider>().To(typeof(Flexpage.Service.ScheduleProvider));
            _kernel.Bind<Flexpage.Abstract.ICalendarBlockProvider>().To(typeof(Flexpage.Service.CalendarBlockProvider));
            _kernel.Bind<Flexpage.Abstract.IFAQBlockProvider>().To(typeof(Flexpage.Service.FAQBlockProvider));
            _kernel.Bind<Flexpage.Abstract.IFavoritesBlockProvider>().To(typeof(Flexpage.Service.FavoritesBlockProvider));
            _kernel.Bind<Flexpage.Abstract.ICmsTextBlockProvider>().To(typeof(Flexpage.Service.CmsTextBlockProvider));
            _kernel.Bind<Flexpage.Abstract.IWebFormBlockProvider>().To(typeof(Flexpage.Service.WebFormBlockProvider));
            _kernel.Bind<Flexpage.Abstract.ICSSSettingsProvider>().To(typeof(Flexpage.Service.CSSSettingsProvider));
            _kernel.Bind<Flexpage.Service.Abstract.ISettingsProvider>().To(typeof(Flexpage.Service.SettingsProvider));
            _kernel.Bind<Flexpage.Abstract.IWebsiteProvider>().To<Flexpage.Service.WebsiteProvider>();
            _kernel.Bind<Flexpage.Abstract.IParticipantsBlockProvider>().To<Flexpage.Service.ParticipantsBlockProvider>();
            _kernel.Bind<Flexpage.Abstract.IBlockLoader>().To<Flexpage.Service.ParticipantsBlockProvider>();

            // service converters
            _kernel.Bind<Flexpage.Service.Converters.ICmsTextConverter>().To(typeof(Flexpage.Service.Converters.CmsTextConverter));
            _kernel.Bind<Flexpage.Service.Converters.IEventConverter>().To(typeof(Flexpage.Service.Converters.EventConverter));
            _kernel.Bind<Flexpage.Service.Converters.INewsConverter>().To(typeof(Flexpage.Service.Converters.NewsConverter));
            //_kernel.Bind<Pluritech.ContactSync.Service.Converters.IMailboxConverter>().To(typeof(Pluritech.ContactSync.Service.Converters.MailboxConverter));
            _kernel.Bind<Flexpage.Service.Converters.IWebFormConverter>().To(typeof(Flexpage.Service.Converters.WebFormConverter));
            _kernel.Bind<Flexpage.Service.Converters.IParticipantsConverter>().To<Flexpage.Service.Converters.ParticipantsConverter>();
            _kernel.Bind<Flexpage.Service.Converters.IBlockConverter>().To(typeof(Flexpage.Service.Converters.BlockConverter));
            _kernel.Bind<Flexpage.Abstract.IViewConverter>().To(typeof(FlexPage2.Areas.Flexpage.Helpers.ViewConverter));

            // business layer
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IBusinessObjectConverter>().To(typeof(Pluritech.Pluriworks.Business.BusinessObjectConverter));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IFileSystem>().To(typeof(Pluritech.Pluriworks.Business.FileSystem));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IAuthenticatedUser>().To(typeof(Pluritech.Pluriworks.Business.AuthenticatedUser));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IPermissionsSystem>().To(typeof(Pluritech.Pluriworks.Business.PermissionsSystem));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IPropertySystem>().To(typeof(Pluritech.Pluriworks.Business.PropertySystem));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IUserRoleProvider>().To(typeof(Pluritech.Pluriworks.Business.UserRoleProvider));
            _kernel.Bind<Pluritech.Encryption.Abstract.ITextEncryptor>().To(typeof(Pluritech.SimpleEncryptionService.TextEncryption)).Named("TextEncryptor")
                .WithConstructorArgument("initialisationString", _crypInit);
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IContactSystem>().To(typeof(Pluritech.Pluriworks.Business.ContactSystem));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.ISearchProcessor>().To(typeof(Pluritech.Pluriworks.Business.SearchProcessor));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IFilePublicationDispatcher>().To(typeof(Pluritech.Pluriworks.Business.FilePublicationDispatcher));

            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.IClientList>().To(typeof(Pluritech.PluriShell.Business.ClientList));
            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.IFileOperationDispatcher>().To(typeof(Pluritech.PluriShell.Business.FileOperationDispatcher));
            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.INodeLockList>().To(typeof(Pluritech.PluriShell.Business.NodeLockList));
            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.IServerFileSystem>().To(typeof(Pluritech.PluriShell.Business.LocalFileOperator));
            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.ISynchronization>().To(typeof(Pluritech.PluriShell.Business.Synchronization));
            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.ITestHelper>().To(typeof(Pluritech.PluriShell.Business.TestHelper));
            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.ITransactionList>().To(typeof(Pluritech.PluriShell.Business.TransactionList));
            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.IPermissionOperationProcessor>().To(typeof(Pluritech.PluriShell.Business.PermissionOperationProcessor));

            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IEnumProcessor>().To(typeof(Pluritech.Pluriworks.Business.EnumProcessor));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IObjectProcessor>().To(typeof(Pluritech.Pluriworks.Business.ObjectProcessor));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.IPublicationProcessor>().To(typeof(Pluritech.Pluriworks.Business.PublicationProcessor));
            _kernel.Bind<Pluritech.Pluriworks.Business.Abstract.ILinkTypeManagerProcessor>().To(typeof(Pluritech.Pluriworks.Business.LinkTypeManagerProcessor));

            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.IAreaProcessor>().To(typeof(Pluritech.PluriShell.Business.AreaProcessor));
            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.INodeProcessor>().To(typeof(Pluritech.PluriShell.Business.NodeProcessor));
            //_kernel.Bind<Pluritech.PluriShell.Business.Abstract.ILocking>().To(typeof(Pluritech.PluriShell.Business.LockProcessor));

            //_kernel.Bind<Pluritech.Events.Abstract.IEventQueue>().To(typeof(Pluritech.Events.Domain.Context.EventsRepository));
            _kernel.Bind<Pluritech.Notifications.Abstract.INotificationsRepository>().To(typeof(Pluritech.Notifications.Domain.Context.NotificationsRepository));

            //_kernel.Bind<Pluritech.ContactSync.Business.Abstract.IContactSyncProcessor>().To(typeof(Pluritech.ContactSync.Business.ContactSyncProcessor));
            //_kernel.Bind<Pluritech.ContactSync.Business.Abstract.IContactOperationDispatcher>().To(typeof(Pluritech.ContactSync.Business.ContactOperationDispatcher));

            _kernel.Bind<Flexpage.Business.Abstract.ISitemapProcessor>().To(typeof(Flexpage.Business.SitemapProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.ISitemapMenuBlockProcessor>().To(typeof(Flexpage.Business.SitemapProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IBreadcrumbsProcessor>().To(typeof(Flexpage.Business.BreadcrumbsProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IBlockProcessor>().To(typeof(Flexpage.Business.BlockProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IPageProcessor>().To(typeof(Flexpage.Business.PageProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IRolesProcessor>().To(typeof(Flexpage.Business.RolesProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.ICmsTextProcessor>().To(typeof(Flexpage.Business.CmsTextProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.INewsProcessor>().To(typeof(Flexpage.Business.NewsProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IEventsProcessor>().To(typeof(Flexpage.Business.EventsProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.ILogProcessor>().To(typeof(Flexpage.Business.LogProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IScheduleProcessor>().To(typeof(Flexpage.Business.ScheduleProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.ICalendarProcessor>().To(typeof(Flexpage.Business.CalendarProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IFAQProcessor>().To(typeof(Flexpage.Business.FAQProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IFavoriesProcessor>().To(typeof(Flexpage.Business.FavoritesProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.ICssSettingsProcessor>().To(typeof(Flexpage.Business.CssSettingProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IInternalCommentProcessor>().To(typeof(Flexpage.Business.InternalCommentProcessor));
            _kernel.Bind<Flexpage.Abstract.IWizardProcessor>().To(typeof(Flexpage.Service.WizardProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IWebFormProcessor>().To(typeof(Flexpage.Business.WebFormProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IResourceProcessor>().To(typeof(Flexpage.Business.ResourceProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IUserSettingsProcessor>().To(typeof(Flexpage.Business.UserSettingsProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.ISettingsProcessor>().To(typeof(Flexpage.Business.SettingsProcessor));
            _kernel.Bind<Flexpage.Business.Abstract.IWebsiteProcessor>().To<Flexpage.Business.WebsiteProcessor>();
            _kernel.Bind<Flexpage.Business.Abstract.IParticipantsBlockProcessor>().To<Flexpage.Business.ParticipantsBlockProcessor>();

            // business converters
            _kernel.Bind<Flexpage.Business.Converters.IWebFormConverter>().To(typeof(Flexpage.Business.Converters.WebFormConverter));
            _kernel.Bind<Flexpage.Business.Converters.INewsConverter>().To(typeof(Flexpage.Business.Converters.NewsConverter));
            _kernel.Bind<Flexpage.Business.Converters.ICssSettingConverter>().To(typeof(Flexpage.Business.Converters.CssSettingConverter));
            //_kernel.Bind<Pluritech.ContactSync.Business.Converters.IContactSyncMailboxConverter>().To(typeof(Pluritech.ContactSync.Business.Converters.ContactSyncMailboxConverter));

            // domain layer
            _kernel.Bind<Pluritech.Pluriworks.Domain.Abstract.IPluriworksRepository>().To(typeof(Pluritech.Pluriworks.Domain.Context.PluriworksRepository));
            _kernel.Bind<Pluritech.Pluriworks.Domain.Abstract.ITokenRepository>().To(typeof(Pluritech.Pluriworks.Domain.Context.PluriworksRepository));// .ToConstant(Pluriworks.Domain.Context.DALHelper.Context);
            //_kernel.Bind<Pluritech.PluriShell.DAL.Abstract.ISynchronizationDAL>().To(typeof(Pluritech.PluriShell.DAL.SynchronizationDAL));
            //_kernel.Bind<Pluritech.PluriShell.ServerDAL.Abstract.ISyncRepository>().To(typeof(Pluritech.PluriShell.ServerDAL.Context.SyncRepository));
            _kernel.Bind<Pluritech.EventLog.Domain.Abstract.IRepository>().To(typeof(Pluritech.EventLog.Domain.EventLogRepository));

            //_kernel.Bind<Pluritech.ContactSync.Domain.Abstract.IOutlookSyncRepository>().To(typeof(Pluritech.ContactSync.Context.ContactSyncRepository));

            _kernel.Bind<Flexpage.Domain.Abstract.IFlexpageRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.ISitemapRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IBlockRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IWizardRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IPageRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IUserSettingsRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IScheduleRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.INewsRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IEventRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.ICmsTextRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.ICalendarRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IFAQRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IFavoritesRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.ICssRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IWebFormRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IWebsiteRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);
            _kernel.Bind<Flexpage.Domain.Abstract.IParticipantsRepository>().To<Flexpage.Domain.Context.ParticipantsRepository>();
            _kernel.Bind<Flexpage.Domain.Abstract.ILanguageRepository>().ToConstant(Flexpage.Domain.Context.DALHelper.Context);

            // other 
            _kernel.Bind<Pluritech.Log.Abstract.ILog>().To(typeof(Pluritech.EventLog.EventLog));
            _kernel.Bind<Pluritech.Encryption.Abstract.ITextEncryptor>().To(typeof(Pluritech.SimpleEncryptionService.AESEncryptor)).Named("AESEncryptor");

            // Workflow
            ////_kernel.Bind<Pluritech.Workflow.Business.Abstract.ICodeCompiler>().To<Pluritech.Workflow.Business.CodeCompiler>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IWorkflowContext>().To<Pluritech.Workflow.Domain.Context.WorkflowContext>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IWorkflowDefinitionContext>().To<Pluritech.Workflow.Domain.Context.WorkflowContext>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IPluriworksContext>().To<Pluritech.Workflow.Domain.Context.WorkflowContext>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IWorkflow>().To<Pluritech.Workflow.Business.WorkflowProcessor>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IWorkflowDefinition>().To<Pluritech.Workflow.Business.WorkflowDefinitionProcessor>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IDictionaries>().To<Pluritech.Workflow.Business.DictionariesProcessor>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IFilesProcessor>().To<Pluritech.Workflow.Business.FilesProcessor>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IContentProcessor>().To<Pluritech.Workflow.Business.FileContentStorage>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IPathProcessor>().To<Pluritech.Workflow.Business.PathProcessor>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IDocumentsBlockProcessor>().To<Pluritech.Workflow.Business.DocumentsBlockProcessor>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IDocumentsBlockRepository>().To<Pluritech.Workflow.Domain.Context.DocumentsBlockRepository>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IPermissions>().To<Pluritech.Workflow.Business.PermissionsProcessor>();
            //_kernel.Bind<Pluritech.Workflow.Business.Abstract.IDownloadsProcessor>().To<Pluritech.Workflow.Business.DownloadsProcessor>();

            //_kernel.Bind<Pluritech.Workflow.InternalAbstract.IExternalCode>().To<Pluritech.Workflow.Business.CustomScriptAPI>();

            //_kernel.Bind<Pluritech.Workflow.Domain.Converter.IThreadConverter>().To<Pluritech.Workflow.Domain.Converter.ThreadConverter>();
            //_kernel.Bind<Pluritech.Workflow.Domain.Converter.IThreadDefinitionConverter>().To<Pluritech.Workflow.Domain.Converter.ThreadDefinitionConverter>();
            //_kernel.Bind<Pluritech.Workflow.Service.Converter.IFilterConverter>().To<Pluritech.Workflow.Service.Converter.FilterConverter>();

            //_kernel.Bind<Pluritech.Workflow.Abstract.IWorkflowProvider>().To<Pluritech.Workflow.Service.WorkflowProvider>();
            //_kernel.Bind<Pluritech.Workflow.Abstract.IDictionariesProvider>().To<Pluritech.Workflow.Service.DictionariesProvider>();
            //_kernel.Bind<Pluritech.Workflow.Abstract.IDefinitionsProvider>().To<Pluritech.Workflow.Service.DefinitionsProvider>();
            //_kernel.Bind<Pluritech.Workflow.Service.Abstract.IDocumentsBlockProvider>().To<Pluritech.Workflow.Service.DocumentsBlockProvider>();
            //_kernel.Bind<Pluritech.Workflow.Service.Abstract.IDocumentsBlockConverter>().To<Pluritech.Workflow.Service.Converter.DocumentsBlockConverter>();
            //_kernel.Bind<Pluritech.Workflow.Service.Abstract.IFilesHelper>().To<Pluritech.Workflow.Service.FilesHelper>();
            //_kernel.Bind<Pluritech.Workflow.Abstract.IDownloadsProvider>().To<Pluritech.Workflow.Service.DownloadsProvider>();
            //_kernel.Bind<Pluritech.Workflow.Web.Controllers.IParamsController>().To<Pluritech.Workflow.Web.Controllers.ParamsController>();
            //_kernel.Bind<Pluritech.Workflow.Abstract.IPermissionsProvider>().To<Pluritech.Workflow.Service.PermissionsProvider>();

            //_kernel.Bind<Flexpage.Abstract.IBlockLoader>().To<Pluritech.Workflow.Service.DocumentsBlockProvider>();

            //_kernel.Bind<Pluritech.Workflow.InternalAbstract.ICustomerDataPreparation>().To<CopaCogeca.Workflow.Extension.DataPreparationCode>();
            //_kernel.Bind<Pluritech.Workflow.InternalAbstract.ICustomerExtensionRegistrator>().To<CopaCogeca.Workflow.Extension.ExtensionRegistrator>();

            // Scheduler
            //_kernel.Bind<Pluritech.Scheduler.Abstract.ISchedulerProvider>().To<Pluritech.Scheduler.Service.SchedulerProvider>();
            //_kernel.Bind<Pluritech.Scheduler.Business.ISchedulerProcessor>().To<Pluritech.Scheduler.Business.SchedulerProcessor>();
            //_kernel.Bind<Pluritech.Scheduler.Business.ISchedulerContext>().To<Pluritech.Scheduler.Domain.Context.SchedulerContext>();
            //_kernel.Bind<Flexpage.Abstract.IBlockLoader>().To<Pluritech.Scheduler.Service.ScheduleBlockProvider>();
            //_kernel.Bind<Pluritech.Scheduler.Service.Abstract.IScheduleBlockProvider>().To<Pluritech.Scheduler.Service.ScheduleBlockProvider>();
            //_kernel.Bind<Pluritech.Scheduler.Business.SchedulerBlockProcessor>().To<Pluritech.Scheduler.Business.SchedulerBlockProcessor>();
            //_kernel.Bind<Flexpage.Abstract.Extensions.ICmsExtension>().To<Pluritech.Scheduler.Service.Extensions.Appointments>();

            // Categories
            //_kernel.Bind<Pluritech.Categories.Abstract.ICategoriesProvider>().To<Pluritech.Categories.Service.CategoriesProvider>();
            //_kernel.Bind<Pluritech.Categories.Service.Abstract.ICategoryConverter>().To<Pluritech.Categories.Service.CategoryConverter>();
            //_kernel.Bind<Pluritech.Categories.Business.Abstract.ICategories>().To<Pluritech.Categories.Business.CategoriesProcessor>();
            //_kernel.Bind<Pluritech.Categories.Business.Abstract.ICategoriesRepository>().To<Pluritech.Categories.Domain.CategoriesRepository>();

            //_kernel.Bind<Pluritech.SearchQueryParser.Abstract.ISearchQueryParser>().To<Pluritech.SearchQueryParser.Service.SearchQueryParser>();
            //_kernel.Bind<Pluritech.Office.Api.Abstract.IOffice>().To<Pluritech.Office.Api.OfficeApiProvider>();

            // Email engine
            //_kernel.Bind<Pluritech.EmailEngine.Abstract.IEmailProvider>().To<Pluritech.EmailEngine.Service.EmailProvider>();
            //_kernel.Bind<Pluritech.EmailEngine.Service.Abstract.IServiceConverter>().To<Pluritech.EmailEngine.Service.ServiceConverter>();
            //_kernel.Bind<Pluritech.EmailEngine.Business.Abstract.IEmailProcessor>().To<Pluritech.EmailEngine.Business.EmailProcessor>();
            //_kernel.Bind<Pluritech.EmailEngine.Business.Abstract.IBusinessConverter>().To<Pluritech.EmailEngine.Business.BusinessConverter>();
            //_kernel.Bind<Pluritech.EmailEngine.Business.Abstract.IMailCleint>().To<Pluritech.EmailEngine.Business.Clients.ClientSmtp>();
            //_kernel.Bind<Pluritech.EmailEngine.Business.Abstract.ITemplateFieldsProcessor>().To<Pluritech.EmailEngine.Business.TextProcessing.MailerReplacementFields>();
            //_kernel.Bind<Pluritech.EmailEngine.Business.Abstract.IResourceStringProvider>().To<Pluritech.EmailEngine.Business.TextProcessing.NoResouceProvider>();
            //_kernel.Bind<Pluritech.EmailEngine.Domain.Abstract.IEmailRepository>().To<Pluritech.EmailEngine.Domain.EmailsRepository>();

            // Member profile

            _kernel.Bind<IRepositoryWrapper>().To<RepositoryWrapper>();
            _kernel.Bind<IMemberManager>().To<MemberManager>(); 
            _kernel.Bind<IStatusManager>().To<StatusManager>();
            _kernel.Bind<IUnitOfWork>().To<UnitOfWorkManager>();
            _kernel.Bind<IHashManager>().To<HashManager>();
            _kernel.Bind<IHistoryChangesManager>().To<HistoryChangesManager>();

        }

    }

    public class NinjectDependencyScope : System.Web.Http.Dependencies.IDependencyScope
    {
        private IResolutionRoot _resolver;

        internal NinjectDependencyScope(IResolutionRoot resolver)
        {
            this._resolver = resolver;
        }

        public void Dispose()
        {
            var disposable = this._resolver as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            this._resolver = null;
        }

        public object GetService(Type serviceType)
        {
            if (this._resolver == null)
            {
                throw new ObjectDisposedException("this", "This scope has already been disposed");
            }

            return this._resolver.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (this._resolver == null)
            {
                throw new ObjectDisposedException("this", "This scope has already been disposed");
            }

            return this._resolver.GetAll(serviceType);
        }
    }
}