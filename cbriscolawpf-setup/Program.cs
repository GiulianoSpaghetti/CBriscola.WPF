using System;
using System.Runtime.Remoting.Lifetime;
using WixSharp;

namespace windatefrom_setup
{
    internal class Program
    {
        static void Main()
        {
            var project = new Project("CBriscolaWPF",
                              new Dir(@"[ProgramFiles64Folder]\\CBriscolaWPF",
                                  new DirFiles(@"*.*")
                                ),
                        new Dir(@"%ProgramMenu%",
                         new ExeFileShortcut("CBriscolaWPF", "[ProgramFiles64Folder]\\CBriscolaWPF\\CBriscola.WPF.exe", "") { WorkingDirectory = "[INSTALLDIR]" }
                      )//,
                       //new Property("ALLUSERS","0")
            );

            project.GUID = new Guid("DD0F1B46-75C9-4672-AFD4-3985863526E7");
            project.Version = new Version("0.7.2");
            project.Platform = Platform.x64;
            project.SourceBaseDir = "D:\\source\\CBriscola.WPF\\CBriscola.WPF\\bin\\Release\\net9.0-windows10.0.26100.0";
            project.LicenceFile = "LICENSE.rtf";
            project.OutDir = "D:\\";
            project.ControlPanelInfo.Manufacturer = "Giulio Sorrentino";
            project.ControlPanelInfo.Name = "CBriscolaWPF";
            project.ControlPanelInfo.HelpLink = "https://github.com/numerunix/CBriscola.WPF/issues";
            project.Description = "Simulatore del gioco della briscola in WPF a due giocatori senza multiplayer.";
            //            project.Properties.SetValue("ALLUSERS", 0);
            project.BuildMsi();
        }
    }
}