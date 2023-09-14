using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Image = System.Windows.Controls.Image;
using org.altervista.numerone.framework;
using System.Globalization;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace CBriscola.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Giocatore g, cpu, primo, secondo, temp;
        private static Mazzo m;
        private static Carta c, c1, briscola;
        private static Image cartaCpu = new Image();
        private static Image i, i1;
        private static UInt16 secondi = 5;
        private static bool avvisaTalloneFinito = true, briscolaDaPunti = false, primaUtente=true;
        private static DispatcherTimer t;
        public static string piattaforma;
        RegistryKey k1;
        ResourceDictionary d;
        ElaboratoreCarteBriscola e;
        public MainWindow()
        {
            this.InitializeComponent();
            EasClientDeviceInformation eas = new EasClientDeviceInformation();
            piattaforma = eas.SystemProductName;
            if (piattaforma == "System Product Name")
                piattaforma = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            try
            {
                d = this.FindResource(CultureInfo.CurrentCulture.TwoLetterISOLanguageName) as ResourceDictionary;
            }
            catch (ResourceReferenceKeyNotFoundException ex)
            { d = this.FindResource("en") as ResourceDictionary; }
            String nomeUtente="", nomeCpu="", nomeMazzo = "Napoletano";

            RegistryKey k = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
            k1 = k.OpenSubKey("CBriscola", true);
            if (k1 != null)
            {
                nomeUtente=(String)k1.GetValue("NomeUtente", "");
                nomeCpu=(String) k1.GetValue("NomeCpu", "");
                try
                {

                    secondi = UInt16.Parse((String)k1.GetValue("Secondi", "0"));
                } catch (System.InvalidCastException ex)
                {
                    secondi = 0;
                }
                briscolaDaPunti = bool.Parse((String)k1.GetValue("BriscolaDaPunti", "False"));
                avvisaTalloneFinito = bool.Parse((String)k1.GetValue("AvvisaTalloneFinito", "True"));
                nomeMazzo=(string)k1.GetValue("Mazzo", "Napoletano");
            }
            else
                k1 = k.CreateSubKey("CBriscola");

            if (nomeUtente=="")
            {
                if (k1 == null)
                {
                    new ToastContentBuilder().AddArgument(d["Errore"] as string).AddText(d["ImpossibileCreareChiaveDiRegistro"] as string).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    return;
                }
                k1.SetValue("NomeUtente", "Utente");
                nomeUtente = "Utente";
            }
            if (nomeCpu == "")
            {
                if (k1 == null)
                {
                    new ToastContentBuilder().AddArgument(d["Errore"] as string).AddText(d["ImpossibileCreareChiaveDiRegistro"] as string).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    return;
                }
                k1.SetValue("NomeCpu", "Cpu");
                nomeCpu = "Cpu";
            }
            if (secondi==0)
            {
                if (k1 == null)
                {
                    new ToastContentBuilder().AddArgument(d["Errore"] as string).AddText(d["ImpossibileCreareChiaveDiRegistro"] as string).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    return;
                }
                k1.SetValue("Secondi", 5);
                secondi = 5;
            }
            if (nomeMazzo=="Napoletano")
                cartaCpu.Source = new BitmapImage(new Uri("pack://application:,,,/resources/images/retro carte pc.png"));
            else
                cartaCpu.Source = new BitmapImage(new Uri("C:\\Program Files\\wxBriscola\\Mazzi\\" + nomeMazzo + "\\retro carte pc.png"));
            e = new ElaboratoreCarteBriscola(briscolaDaPunti);
            m = new Mazzo(e);
            m.SetNome(nomeMazzo);
            Carta.Inizializza(40, CartaHelperBriscola.GetIstanza(e));
            Carta.CaricaImmagini(m, 40, CartaHelperBriscola.GetIstanza(e), d);

            g = new Giocatore(new GiocatoreHelperUtente(), nomeUtente, 3);
            cpu = new Giocatore(new GiocatoreHelperCpu(ElaboratoreCarteBriscola.GetCartaBriscola()), nomeCpu, 3);

            primo = g;
            secondo = cpu;
            briscola = Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola());
            Image[] img = new Image[3];
            for (UInt16 i = 0; i < 3; i++)
            {
                g.AddCarta(m);
                cpu.AddCarta(m);

            }
            NomeUtente.Content = g.GetNome();
            NomeCpu.Content = cpu.GetNome();
            Utente0.Source = g.GetImmagine(0);
            Utente1.Source = g.GetImmagine(1);
            Utente2.Source = g.GetImmagine(2);
            Cpu0.Source = cartaCpu.Source;
            Cpu1.Source = cartaCpu.Source;
            Cpu2.Source = cartaCpu.Source;
            PuntiCpu.Content = $"{d["PuntiDiPrefisso"]}{cpu.GetNome()}{d["PuntiDiSuffisso"]}: {cpu.GetPunteggio()}";
            PuntiUtente.Content = $"{d["PuntiDiPrefisso"]}{g.GetNome()}{d["PuntiDiSuffisso"]}: {g.GetPunteggio()}";
            NelMazzoRimangono.Content = $"{d["NelMazzoRimangono"]} {m.GetNumeroCarte()} {d["carte"]}";
            CartaBriscola.Content = $"{d["IlSemeDiBriscolaE"]}: {briscola.GetSemeStr()}";
            lbCartaBriscola.Content = $"{d["BriscolaDaPunti"]}";
            lbAvvisaTallone.Content = $"{d["AvvisaTallone"]}";
            opNomeUtente.Content = $"{d["NomeUtente"]}: ";
            opNomeCpu.Content = $"{d["NomeCpu"]}: ";
            Secondi.Content = $"{d["secondi"]}: ";
            InfoApplicazione.Content = $"{d["Applicazione"]}";
            OpzioniApplicazione.Content = $"{d["Applicazione"]}";
            OpzioniInformazioni.Content = $"{d["Informazioni"]}";
            AppInformazioni.Content = $"{d["Informazioni"]}";
            AppOpzioni.Content = $"{d["Opzioni"]}";
            fpOk.Content = $"{d["Ok"]}";
            fpCancel.Content = $"{d["Annulla"]}";
            fpShare.Content = $"{d["Condividi"]}";
            lblinfo.Content = $"{d["info"]}";
            Briscola.Source = briscola.GetImmagine();
            t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(secondi);
            t.Tick += (s, e) =>
            {
                c = primo.GetCartaGiocata();
                c1 = secondo.GetCartaGiocata();
                if ((c.CompareTo(c1) > 0 && c.StessoSeme(c1)) || (c1.StessoSeme(briscola) && !c.StessoSeme(briscola)))
                {
                    temp = secondo;
                    secondo = primo;
                    primo = temp;
                }

                primo.AggiornaPunteggio(secondo);
                PuntiCpu.Content = $"{d["PuntiDiPrefisso"]}{cpu.GetNome()}{d["PuntiDiSuffisso"]}: {cpu.GetPunteggio()}";
                PuntiUtente.Content = $"{d["PuntiDiPrefisso"]}{g.GetNome()}{d["PuntiDiSuffisso"]}: {g.GetPunteggio()}";
                if (AggiungiCarte())
                {
                    NelMazzoRimangono.Content = $"{d["NelMazzoRimangono"]} {m.GetNumeroCarte()} {d["carte"]}";
                    CartaBriscola.Content = $"{d["IlSemeDiBriscolaE"]}: {briscola.GetSemeStr()}";
                    if (Briscola.IsVisible && m.GetNumeroCarte() == 0)
                    {
                        NelMazzoRimangono.Visibility = Visibility.Collapsed;
                        Briscola.Visibility = Visibility.Collapsed;
                        if (m.GetNumeroCarte() == 2 && avvisaTalloneFinito)
                            new ToastContentBuilder().AddArgument(d["TalloneFinito"] as string).AddText(d["IlTalloneEFinito"] as string).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    }
                    Utente0.Source = g.GetImmagine(0);
                    if (cpu.GetNumeroCarte() > 1)
                        Utente1.Source = g.GetImmagine(1);
                    if (cpu.GetNumeroCarte() > 2)
                        Utente2.Source = g.GetImmagine(2);
                    i.Visibility = Visibility.Visible;
                    i1.Visibility = Visibility.Visible;
                    Giocata0.Visibility = Visibility.Collapsed;
                    Giocata1.Visibility = Visibility.Collapsed;
                    if (cpu.GetNumeroCarte() == 2)
                    {
                        Utente2.Visibility = Visibility.Collapsed;
                        Cpu2.Visibility = Visibility.Collapsed;
                    }
                    if (cpu.GetNumeroCarte() == 1)
                    {
                        Utente1.Visibility = Visibility.Collapsed;
                        Cpu1.Visibility = Visibility.Collapsed;
                    }
                    if (primo == cpu)
                    {
                        i1 = GiocaCpu();
                        if (cpu.GetCartaGiocata().StessoSeme(briscola))
                            new ToastContentBuilder().AddArgument(d["GiocataBriscola"] as string).AddText($"{d["LaCpuHaGiocatoIl"] as string} {cpu.GetCartaGiocata().GetValore() + 1} {d["DiBriscola"] as string}").AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                        else if (cpu.GetCartaGiocata().GetPunteggio() > 0)
                            new ToastContentBuilder().AddArgument(d["GiocataCartaDiValore"] as string).AddText($"{d["LaCpuHaGiocatoIl"] as string} {cpu.GetCartaGiocata().GetValore() + 1} {d["di"] as string} {cpu.GetCartaGiocata().GetSemeStr()}").AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    }
                }
                else
                {
                    if (g.GetPunteggio() == cpu.GetPunteggio())
                        s = $"{d["PartitaPatta"]}";
                    else
                    {
                        if (g.GetPunteggio() > cpu.GetPunteggio())
                            s = $"{d["HaiVinto"]}";
                        else
                            s = $"{d["HaiPerso"]}";
                        s = $"{s} {d["per"]} {Math.Abs(g.GetPunteggio() - cpu.GetPunteggio())} {d["punti"]}";
                    }
                    fpRisultrato.Content = $"{d["PartitaFinita"]}. {s}. {d["NuovaPartita"]}?";
                    Applicazione.Visibility = Visibility.Collapsed;
                    FinePartita.Visibility = Visibility.Visible;
                }
                t.Stop();
            };
        }
        private Image GiocaUtente(Image img)
        {
            UInt16 quale = 0;
            Image img1 = Utente0;
            if (img == Utente1)
            {
                quale = 1;
                img1 = Utente1;
            }
            if (img == Utente2)
            {
                quale = 2;
                img1 = Utente2;
            }
            Giocata0.Visibility = Visibility.Visible;
            Giocata0.Source = img1.Source;
            img1.Visibility = Visibility.Collapsed;
            g.Gioca(quale);
            return img1;
        }

        private void OnInfo_Click(object sender, EventArgs e)
        {
            Applicazione.Visibility = Visibility.Collapsed;
            GOpzioni.Visibility = Visibility.Collapsed;
            Info.Visibility = Visibility.Visible;
        }

        private void OnApp_Click(object sender, EventArgs e)
        {
            GOpzioni.Visibility = Visibility.Collapsed;
            Info.Visibility = Visibility.Collapsed;
            Applicazione.Visibility = Visibility.Visible;
        }
        private void OnOpzioni_Click(object sender, EventArgs e)
        {
            List<String> mazzi;
            String dirs = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)+"\\wxBriscola\\Mazzi";
            try
            {
                mazzi = new List<String>(Directory.EnumerateDirectories(dirs));
            } catch (System.IO.DirectoryNotFoundException ex)
            {
                mazzi = new List<string>();
                new ToastContentBuilder().AddArgument(d["Attenzione"] as string).AddText(d["CercaNuoviMazzi"] as string).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();

            }
            mazzi.Sort();
            lsmazzi.Items.Clear();
            foreach (var s in mazzi)
                lsmazzi.Items.Add(s.Substring(s.LastIndexOf("\\") + 1));
            if (!lsmazzi.Items.Contains("Napoletano"))
                lsmazzi.Items.Add("Napoletano");
            Info.Visibility = Visibility.Collapsed;
            Applicazione.Visibility = Visibility.Collapsed;
            GOpzioni.Visibility = Visibility.Visible;
            txtNomeUtente.Text = g.GetNome();
            txtCpu.Text = cpu.GetNome();
            txtSecondi.Text = secondi.ToString();
            cbCartaBriscola.IsChecked = briscolaDaPunti;
            cbAvvisaTallone.IsChecked = avvisaTalloneFinito;
            mazzi.Clear();
        }

        private void OnOkFp_Click(object sender, EventArgs evt)
        {
            bool cartaBriscola = true;
            FinePartita.Visibility = Visibility.Collapsed;
            if (cbCartaBriscola.IsChecked == false)
                cartaBriscola = false;
            e = new ElaboratoreCarteBriscola(cartaBriscola);
            m = new Mazzo(e);
            briscola = Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola());
            g = new Giocatore(new GiocatoreHelperUtente(), g.GetNome(), 3);
            cpu = new Giocatore(new GiocatoreHelperCpu(ElaboratoreCarteBriscola.GetCartaBriscola()), cpu.GetNome(), 3);
            for (UInt16 i = 0; i < 3; i++)
            {
                g.AddCarta(m);
                cpu.AddCarta(m);

            }
            Utente0.Source = g.GetImmagine(0);
            Utente0.Visibility = Visibility.Visible;
            Utente1.Source = g.GetImmagine(1);
            Utente1.Visibility = Visibility.Visible;
            Utente2.Source = g.GetImmagine(2);
            Utente2.Visibility = Visibility.Visible;
            Cpu0.Source = cartaCpu.Source;
            Cpu0.Visibility = Visibility.Visible;
            Cpu1.Source = cartaCpu.Source;
            Cpu1.Visibility = Visibility.Visible;
            Cpu2.Source = cartaCpu.Source;
            Cpu2.Visibility = Visibility.Visible;
            Giocata0.Visibility = Visibility.Collapsed;
            Giocata1.Visibility = Visibility.Collapsed;
            PuntiCpu.Content = $"{d["PuntiDiPrefisso"]}{cpu.GetNome()}{d["PuntiDiSuffisso"]}: {cpu.GetPunteggio()}";
            PuntiUtente.Content = $"{d["PuntiDiPrefisso"]}{g.GetNome()}{d["PuntiDiSuffisso"]}: {g.GetPunteggio()}";
            PuntiUtente.Content = $"{d["PuntiDi"]} {g.GetNome()}: {g.GetPunteggio()}";
            NelMazzoRimangono.Content = $"{d["NelMazzoRimangono"]} {m.GetNumeroCarte()} {d["carte"]}";
            NelMazzoRimangono.Visibility = Visibility.Visible;
            CartaBriscola.Content = $"{d["IlSemeDiBriscolaE"]}: {briscola.GetSemeStr()}";
            CartaBriscola.Visibility = Visibility.Visible;
            lbmazzi.Content = $"{d["Mazzo"]} :";
            fpOk.Content = $"{d["Ok"]}";
            fpCancel.Content = $"{d["Annulla"]}";
            fpShare.Content = $"{d["Condividi"]}";
            Briscola.Source = briscola.GetImmagine();
            Briscola.Visibility = Visibility.Visible;
            primaUtente = !primaUtente;
            if (primaUtente)
            {
                primo = g;
                secondo = cpu;
            } else
            {
                primo = cpu;
                secondo = g;
                i1 = GiocaCpu();
            }
            Applicazione.Visibility = Visibility.Visible;
        }
        private void OnCancelFp_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private Image GiocaCpu()
        {
            UInt16 quale = 0;
            Image img1 = Cpu0;
            if (primo == cpu)
                cpu.Gioca(0);
            else
                cpu.Gioca(0, g);
            quale = cpu.GetICartaGiocata();
            if (quale == 1)
                img1 = Cpu1;
            if (quale == 2)
                img1 = Cpu2;
            Giocata1.Visibility = Visibility.Visible;
            Giocata1.Source = cpu.GetCartaGiocata().GetImmagine();
            img1.Visibility = Visibility.Collapsed;
            return img1;
        }
        private static bool AggiungiCarte()
        {
            try
            {
                primo.AddCarta(m);
                secondo.AddCarta(m);
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
            return true;
        }

        private void Image_Tapped(object Sender, EventArgs arg)
        {
            if (t.IsEnabled)
                return;
            Image img = (Image)Sender;
            t.Start();
            i = GiocaUtente(img);
            if (secondo == cpu)
                i1 = GiocaCpu();
        }
        public void OnOk_Click(Object source, EventArgs evt)
        {
            g.SetNome(txtNomeUtente.Text);
            cpu.SetNome(txtCpu.Text);
            if (cbCartaBriscola.IsChecked == false)
                briscolaDaPunti = false;
            else
                briscolaDaPunti = true;
            if (cbAvvisaTallone.IsChecked == false)
                avvisaTalloneFinito = false;
            else
                avvisaTalloneFinito = true;
            try
            {
                secondi = UInt16.Parse(txtSecondi.Text);
            }
            catch (FormatException ex)
            {
                txtSecondi.Text = $"{d["ValoreNonValido"]}";
                return;
            }
            t.Interval = TimeSpan.FromSeconds(secondi);
            NomeUtente.Content = g.GetNome();
            NomeCpu.Content = cpu.GetNome();
            if (lsmazzi.SelectedValue != null)
            {
                m.SetNome(lsmazzi.SelectedValue.ToString());
                Carta.CaricaImmagini(m, 40, CartaHelperBriscola.GetIstanza(e), d);
                Utente0.Source = g.GetImmagine(0);
                Utente1.Source = g.GetImmagine(1);
                Utente2.Source = g.GetImmagine(2);

                briscola = Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola());
                Briscola.Source = briscola.GetImmagine();
                if (m.GetNome() != "Napoletano") 
                    cartaCpu.Source = new BitmapImage(new Uri(@"C:\\Program Files\\wxBriscola\\Mazzi\\" + m.GetNome() + "\\retro carte pc.png"));
                else
                    cartaCpu.Source = new BitmapImage(new Uri("pack://application:,,,/resources/images/retro carte pc.png"));

                Cpu0.Source = cartaCpu.Source;
                Cpu1.Source = cartaCpu.Source;
                Cpu2.Source = cartaCpu.Source;
                CartaBriscola.Content = $"{d["IlSemeDiBriscolaE"]}: {briscola.GetSemeStr()}";
            }
            k1.SetValue("NomeUtente", g.GetNome());
            k1.SetValue("NomeCpu", cpu.GetNome());
            k1.SetValue("Secondi", secondi);
            k1.SetValue("BriscolaDaPunti", briscolaDaPunti);
            k1.SetValue("AvvisaTalloneFinito", avvisaTalloneFinito);
            k1.SetValue("Mazzo", m.GetNome());


            GOpzioni.Visibility = Visibility.Collapsed;
            Applicazione.Visibility = Visibility.Visible;
         
        }

        private void OnFPShare_Click(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = $"https://twitter.com/intent/tweet?text={d["ColGioco"]}{g.GetNome()}%20{d["contro"]}%20{cpu.GetNome()}%20{d["efinito"]}%20{g.GetPunteggio()}%20{d["a"]}%20{cpu.GetPunteggio()}%20{d["colmazzo"]}%20{m.GetNome()}%20{d["piattaforma"]}%20{piattaforma}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscola.wpf",
                UseShellExecute = true
            };
            Process.Start(psi);
            fpShare.IsEnabled = false;
        }


        private void OnSito_Click(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/numerunix/cbriscola.WPF",
                UseShellExecute = true
            };
            Process.Start(psi);
        }


    }
}
