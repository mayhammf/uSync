﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.Core;
using uSync.Core.Serialization;

using static Umbraco.Cms.Core.Constants;

namespace uSync.BackOffice.SyncHandlers.Handlers
{
    [SyncHandler("macroHandler", "Macros", "Macros", uSyncBackOfficeConstants.Priorites.Macros,
        Icon = "icon-settings-alt", EntityType = UdiEntityType.Macro)]
    public class MacroHandler : SyncHandlerBase<IMacro, IMacroService>, ISyncExtendedHandler, ISyncItemHandler,
        INotificationHandler<SavedNotification<IMacro>>,
        INotificationHandler<DeletedNotification<IMacro>>
    {
        private readonly IMacroService macroService;
        private readonly IShortStringHelper shortStringHelper;

        public MacroHandler(
            IShortStringHelper shortStringHelper,
            ILogger<MacroHandler> logger,
            uSyncConfigService uSyncConfig,
            IMacroService macroService,
            IEntityService entityService,
            AppCaches appCaches,
            ISyncSerializer<IMacro> serializer,
            ISyncItemFactory syncItemFactory,
            SyncFileService syncFileService)
            : base(logger, uSyncConfig, appCaches, serializer, syncItemFactory, syncFileService, entityService)
        {
            this.shortStringHelper = shortStringHelper;
            this.macroService = macroService;
        }

        /// <summary>
        ///  overrider the default export, because macros, don't exist as an object type???
        /// </summary>
        public override IEnumerable<uSyncAction> ExportAll(int parent, string folder, HandlerSettings config, SyncUpdateCallback callback)
        {
            // we clean the folder out on an export all. 
            syncFileService.CleanFolder(folder);

            var actions = new List<uSyncAction>();

            var items = macroService.GetAll().ToList();
            int count = 0;
            foreach (var item in items)
            {
                count++;
                callback?.Invoke(item.Name, count, items.Count);
                actions.AddRange(Export(item, folder, config));
            }

            return actions;
        }

        protected override IMacro GetFromService(int id)
            => macroService.GetById(id);

        // not sure we can trust macro guids in the path just yet.
        protected override string GetItemPath(IMacro item, bool useGuid, bool isFlat)
        {
            if (useGuid) return item.Key.ToString();
            return item.Alias.ToSafeAlias(shortStringHelper);
        }

        protected override IMacro GetFromService(Guid key)
            => macroService.GetById(key);

        protected override IMacro GetFromService(string alias)
            => macroService.GetByAlias(alias);

        protected override void DeleteViaService(IMacro item)
            => macroService.Delete(item);

        protected override string GetItemName(IMacro item)
            => item.Name;
        protected override string GetItemAlias(IMacro item)
            => item.Alias;

        protected override IEnumerable<IEntity> GetChildItems(int parent)
        {
            if (parent == -1)
            {
                return macroService.GetAll().Where(x => x is IEntity)
                    .Select(x => x as IEntity);
            }

            return Enumerable.Empty<IEntity>();
        }

    }

}