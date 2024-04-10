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

            project.GUID = new Guid("A2941143-09E9-45AD-8017-0DB4C98D80D2");
            project.Version = new Version("0.7.0");
            project.Platform = Platform.x64;
            project.SourceBaseDir = "F:\\source\\CBriscola.WPF\\CBriscola.WPF\\bin\\Release\\net8.0-windows10.0.22621.0";
            project.LicenceFile = "LICENSE.rtf";
            project.OutDir = "f:\\";
            project.ControlPanelInfo.Manufacturer = "Giulio Sorrentino";
            project.ControlPanelInfo.Name = "CBriscola.Avalonia";
            project.ControlPanelInfo.HelpLink = "https://github.com/numerunix/CBriscola.WPF/issues";
            project.Description = "Simulatore del gioco della briscola in WPF a due giocatori senza multiplayer.";
            //            project.Properties.SetValue("ALLUSERS", 0);
            project.BuildMsi();
        }
    }
}