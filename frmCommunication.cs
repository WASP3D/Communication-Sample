using BeeSys.Wasp.Workflow;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeeSys.Wasp3D.Utility
{

    /***
      Terminology
      Device : Communication device that are create for a application and it handle the channels
      Link   : Instance of mapped channel
      Channel: Send/Recieve the data on particular channel..
      Port   : ListeningPort where we recieve the communication commands or data
      KCResponsePort :Local port for KC Response for Device and channel registration
     ***/
    public partial class frmCommunication : Form
    {
        Device _communicationDevice;
        BeeSys.Wasp.Workflow.Link _link = null;
        string _channelName = "TIMER CHECK";
        public frmCommunication()
        {
            InitializeComponent();

            string kcURL = "net.tcp://localhost:10001/RemoteManager"; // KC URL can be obtained from common config

            if (ConfigurationManager.AppSettings["REMOTEMANAGERURL"] != null)
                kcURL = ConfigurationManager.AppSettings["REMOTEMANAGERURL"];

            string applicationName = "SampleApplication"; // Register this app as a device for communication

            int ListeningPort = 21053;  // Local port to listen to communication commands

            int kcResponsePort = 21054; //Local port for KC Response for Device and channel registration

            _communicationDevice = new Device(kcURL, kcResponsePort , applicationName, ListeningPort);
            _communicationDevice.OnLinkCreation += _communicationDevice_OnLinkCreation;            
        }

        private void _communicationDevice_OnLinkCreation()
        {
            try
            {
                if (!string.IsNullOrEmpty(_channelName) && _link == null)
                {
                    _link = _communicationDevice.GetLink(_channelName);
                    if (_link != null)
                    {
                        _link.Register();
                        _link.OnLinkDataResponse += CommResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                //WRITE LOG  HERE
            }
        }
        private void CommResponse(LinkDataResponseArgs data)
        {
             
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_link != null)
                _link.Send(txtCommand.Text);
        }
        
        protected override void OnClosed(EventArgs e)
        {
            if (_link != null)
            {
                _link.OnLinkDataResponse -= CommResponse;
                _link.UnRegister();
                _link = null;
            }
            if (_communicationDevice != null)
            {
                _communicationDevice.UnRegister();
                _communicationDevice.Dispose();
                _communicationDevice = null;
            }
           
            base.OnClosed(e);
        }
    }
}
