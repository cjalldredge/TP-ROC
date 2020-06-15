using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ROCServer
{
    [RunInstaller(true)]
    public class ROCServiceInstaller : Installer
    {
        public ROCServiceInstaller()
        {
            var spi = new ServiceProcessInstaller();
            var si = new ServiceInstaller();

            spi.Account = ServiceAccount.LocalSystem;
            spi.Username = null;
            spi.Password = null;

            si.DisplayName = "ROC Server";
            si.ServiceName = "Remote OBS Controller Server";
            si.StartType = ServiceStartMode.Automatic;

            Installers.Add(spi);
            Installers.Add(si);
        }
    }
}
