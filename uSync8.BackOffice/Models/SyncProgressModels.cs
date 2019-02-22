﻿using System.Collections.Generic;
using System.Linq;
using uSync8.BackOffice.SyncHandlers;

namespace uSync8.BackOffice
{
    public class SyncProgressSummary
    {
        public int Processed { get; set; }
        public int TotalSteps { get; set; }
        public string Message { get; set; }
        public List<SyncHandlerSummary> Handlers { get; set; }

        public SyncProgressSummary(
            IEnumerable<ISyncHandler> handlers, 
            string message,
            int totalSteps)
        {
            this.TotalSteps = totalSteps;
            this.Message = message;

            this.Handlers = handlers.Select(x => new SyncHandlerSummary()
            {
                Icon = x.Icon,
                Name = x.Name,
                Status = HandlerStatus.Pending
            }).ToList();
        }

        public void UpdateHandler(string name, HandlerStatus status)
        {
            var item = this.Handlers.FirstOrDefault(x => x.Name == name);
            if (item != null)
                item.Status = status;
        }

        public void UpdateHandler(string name, HandlerStatus status, string message)
        {
            UpdateHandler(name, status);
            this.Message = message;
        }

    }

    public class SyncHandlerSummary
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public HandlerStatus Status { get; set; }
    }

    public enum HandlerStatus
    {
        Pending,
        Processing,
        Complete,
        Error
    }
}