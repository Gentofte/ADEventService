using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

using GK.AppCore.Utility;

namespace ADEventService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        // -----------------------------------------------------------------------------
        public ProjectInstaller()
        {
            InitializeComponent();

            AssemblyInfo asmInfo = new AssemblyInfo(typeof(ADEventServiceMain));

            AssemblyInfoSummaryOptions options =
                AssemblyInfoSummaryOptions.Description |
                AssemblyInfoSummaryOptions.Version |
                AssemblyInfoSummaryOptions.Configuration;

            serviceInstaller1.ServiceName = asmInfo.Title + asmInfo.Configuration.ToUpperInvariant();
            serviceInstaller1.DisplayName = string.Format("{0} ({1})", asmInfo.Title, asmInfo.Configuration.ToUpperInvariant());

            serviceInstaller1.Description = asmInfo.GetSummary(options);
        }
    }
}
