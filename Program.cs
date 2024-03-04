using System;
using WixSharp;

namespace cbriscola_wpf
{
    internal class Program
    {
        static void Main()
        {
            var project = new Project("CBriscola.WPF",
                              new Dir(@"[ProgramFiles64Folder]\\CBriscola.WPF",
                                  new DirFiles(@"*.*")
                        ),
                        new Dir(@"%ProgramMenu%",
                         new ExeFileShortcut("CBriscola.WPF", "[ProgramFiles64Folder]\\CBriscola.WPF\\CBriscola.WPF.exe", "") { WorkingDirectory = "[INSTALLDIR]" }
                      )
            );

            project.GUID = new Guid("DD0F1B46-75C9-4672-AFD4-3985863526E7");
            project.Version = new Version("0.6.7");
            project.Platform = Platform.x64;
            project.SourceBaseDir = "e:\\source\\CBriscola.WPF\\CBriscola.WPF\\bin\\Release\\net8.0-windows10.0.22621.0";
            project.LicenceFile = "LICENSE.rtf";
            project.OutDir = "e:\\";
            project.ControlPanelInfo.Manufacturer = "Giulio Sorrentino";
            project.ControlPanelInfo.Name = "CBriscola.WPF";
            project.ControlPanelInfo.HelpLink = "https://github.com/numerunix/cbriscola.wpf/issues";
            project.Description = "Simulatore del gioco della briscola in WPF a due giocatori senza multiplayer";
            project.BuildMsi();
        }
    }
}