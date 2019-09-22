using Microsoft.AspNetCore.SignalR;
using Seagull.Core.Data.Interfaces;
using System.Collections.Generic;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Doctors.Areas.Admin.Models;
using System.Linq;
using Seagull.Doctors.Areas.Admin.Controllers;
using System.IO;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;
using System.Net;
using System;
using Seagull.Doctors.Areas.Admin.ViewModel;
using AutoMapper;
using Seagull.Doctors.Helper.SignalR.Hub;

namespace Seagull.Core.Helper.SignalR.SeagullHub
{
    public class SeagullHub : Hub , ISeagullHub
    {
        private readonly IHubContext<SeagullHub> _lineHubContext;
        #region Fields
        private readonly IGlobalSettings _globalSettings;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor
        public SeagullHub(IGlobalSettings globalSettings, IMapper mapper, IHubContext<SeagullHub> lineHubContext)
        {
            _globalSettings = globalSettings;
            _lineHubContext = lineHubContext;
            _mapper = mapper;
        }
        #endregion

        #region Methods
        public void CheckNotify()
        {
            var data = new object();
            // Call the broadcastMessage method to update clients.
            Clients.All.SendAsync("notifyUsers", data);
        }


        public void NotifyClients()
        {
            _lineHubContext.Clients.All.SendAsync("addNotification", "");
        }

        #endregion

    
    }
}
