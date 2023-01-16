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

namespace CBriscola.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static giocatore g, cpu, primo, secondo, temp;
        private static mazzo m;
        private static carta c, c1, briscola;
        private static Image cartaCpu = new Image();
        private static Image i, i1;
        private static UInt16 secondi = 5;
        private static bool avvisaTalloneFinito = true, briscolaDaPunti = false;
        private static DispatcherTimer t;
        private string s;
        RegistryKey k1;
        elaboratoreCarteBriscola e;
        public MainWindow()
        {
            String nomeUtente="", nomeCpu="", nomeMazzo = "Napoletano";
            this.InitializeComponent();


            RegistryKey k = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
            k1 = k.OpenSubKey("CBriscola", true);
            if (k1 != null)
            {
                nomeUtente=(String)k1.GetValue("NomeUtente", "");
                nomeCpu=(String) k1.GetValue("NomeCpu", "");
                secondi = UInt16.Parse((String)k1.GetValue("Secondi", "-1"));
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
                    new ToastContentBuilder().AddArgument((string)this.FindResource("Errore") as string).AddText((string)this.FindResource("ImpossibileCreareChiaveDiRegistro") as String).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    return;
                }
                k1.SetValue("NomeUtente", "Utente");
                nomeUtente = "Utente";
            }
            if (nomeCpu == "")
            {
                if (k1 == null)
                {
                    new ToastContentBuilder().AddArgument((string)this.FindResource("Errore") as string).AddText((string)this.FindResource("ImpossibileCreareChiaveDiRegistro") as String).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    return;
                }
                k1.SetValue("NomeCpu", "Cpu");
                nomeCpu = "Cpu";
            }
            if (secondi==0)
            {
                if (k1 == null)
                {
                    new ToastContentBuilder().AddArgument((string)this.FindResource("Errore") as string).AddText((string)this.FindResource("ImpossibileCreareChiaveDiRegistro") as String).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    return;
                }
                k1.SetValue("Secondi", 5);
                secondi = 5;
            }
            if (nomeMazzo=="Napoletano")
                cartaCpu.Source = new BitmapImage(new Uri("pack://application:,,,/resources/images/retro carte pc.png"));
            else
                cartaCpu.Source = new BitmapImage(new Uri("C:\\Program Files\\wxBriscola\\Mazzi\\" + nomeMazzo + "\\retro carte pc.png"));
            e = new elaboratoreCarteBriscola(briscolaDaPunti);
            m = new mazzo(e);
            m.setNome(nomeMazzo);
            carta.inizializza(40, cartaHelperBriscola.getIstanza(e), m, this);

            g = new giocatore(new giocatoreHelperUtente(), nomeUtente, 3);
            cpu = new giocatore(new giocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), nomeCpu, 3);

            primo = g;
            secondo = cpu;
            briscola = carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
            Image[] img = new Image[3];
            for (UInt16 i = 0; i < 3; i++)
            {
                g.addCarta(m);
                cpu.addCarta(m);

            }
            NomeUtente.Content = g.getNome();
            NomeCpu.Content = cpu.getNome();
            Utente0.Source = g.getImmagine(0);
            Utente1.Source = g.getImmagine(1);
            Utente2.Source = g.getImmagine(2);
            Cpu0.Source = cartaCpu.Source;
            Cpu1.Source = cartaCpu.Source;
            Cpu2.Source = cartaCpu.Source;
            PuntiCpu.Content = $"{this.FindResource("PuntiDi")} {cpu.getNome()}: {cpu.getPunteggio()}";
            PuntiUtente.Content = $"{this.FindResource("PuntiDi")} {g.getNome()}: {g.getPunteggio()}";
            NelMazzoRimangono.Content = $"{this.FindResource("NelMazzoRimangono")} {m.getNumeroCarte()} {this.FindResource("carte")}";
            CartaBriscola.Content = $"{this.FindResource("IlSemeDiBriscolaE")}: {briscola.getSemeStr()}";
            lbCartaBriscola.Content = $"{this.FindResource("BriscolaDaPunti")}";
            lbAvvisaTallone.Content = $"{this.FindResource("AvvisaTallone")}";
            opNomeUtente.Content = $"{this.FindResource("NomeUtente")}: ";
            opNomeCpu.Content = $"{this.FindResource("NomeCpu")}: ";
            Secondi.Content = $"{this.FindResource("secondi")}: ";
            InfoApplicazione.Content = $"{this.FindResource("Applicazione")}";
            OpzioniApplicazione.Content = $"{this.FindResource("Applicazione")}";
            OpzioniInformazioni.Content = $"{this.FindResource("Informazioni")}";
            AppInformazioni.Content = $"{this.FindResource("Informazioni")}";
            AppOpzioni.Content = $"{this.FindResource("Opzioni")}";
            Briscola.Source = briscola.getImmagine();
            t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(secondi);
            t.Tick += (s, e) =>
            {
                c = primo.getCartaGiocata();
                c1 = secondo.getCartaGiocata();
                if ((c.CompareTo(c1) > 0 && c.stessoSeme(c1)) || (c1.stessoSeme(briscola) && !c.stessoSeme(briscola)))
                {
                    temp = secondo;
                    secondo = primo;
                    primo = temp;
                }

                primo.aggiornaPunteggio(secondo);
                PuntiCpu.Content = $"{this.FindResource("PuntiDi")} {cpu.getNome()}: {cpu.getPunteggio()}";
                PuntiUtente.Content = $"{this.FindResource("PuntiDi")} {g.getNome()}: {g.getPunteggio()}";
                if (aggiungiCarte())
                {
                    NelMazzoRimangono.Content = $"{this.FindResource("NelMazzoRimangono")} {m.getNumeroCarte()} {this.FindResource("carte")}";
                    CartaBriscola.Content = $"{this.FindResource("IlSemeDiBriscolaE")}: {briscola.getSemeStr()}";
                    if (Briscola.IsVisible && m.getNumeroCarte() == 0)
                    {
                        NelMazzoRimangono.Visibility = Visibility.Collapsed;
                        Briscola.Visibility = Visibility.Collapsed;
                        if (m.getNumeroCarte() == 2 && avvisaTalloneFinito)
                            new ToastContentBuilder().AddArgument((string)this.FindResource("TalloneFinito") as string).AddText((string)this.FindResource("IlTalloneEFinito") as String).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    }
                    Utente0.Source = g.getImmagine(0);
                    if (cpu.getNumeroCarte() > 1)
                        Utente1.Source = g.getImmagine(1);
                    if (cpu.getNumeroCarte() > 2)
                        Utente2.Source = g.getImmagine(2);
                    i.Visibility = Visibility.Visible;
                    i1.Visibility = Visibility.Visible;
                    Giocata0.Visibility = Visibility.Collapsed;
                    Giocata1.Visibility = Visibility.Collapsed;
                    if (cpu.getNumeroCarte() == 2)
                    {
                        Utente2.Visibility = Visibility.Collapsed;
                        Cpu2.Visibility = Visibility.Collapsed;
                    }
                    if (cpu.getNumeroCarte() == 1)
                    {
                        Utente1.Visibility = Visibility.Collapsed;
                        Cpu1.Visibility = Visibility.Collapsed;
                    }
                    if (primo == cpu)
                    {
                        i1 = giocaCpu();
                        if (cpu.getCartaGiocata().stessoSeme(briscola))
                            new ToastContentBuilder().AddArgument((string)this.FindResource("GiocataBriscola") as string).AddText($"{(string)this.FindResource("LaCpuHaGiocatoIl") as string} {cpu.getCartaGiocata().getValore() + 1} {(string)this.FindResource("DiBriscola") as string}").AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                        else if (cpu.getCartaGiocata().getPunteggio() > 0)
                            new ToastContentBuilder().AddArgument((string)this.FindResource("GiocataCartaDiValore") as string).AddText($"{(string)this.FindResource("LaCpuHaGiocatoIl") as string} {cpu.getCartaGiocata().getValore() + 1} {(string)this.FindResource("di") as string} {cpu.getCartaGiocata().getSemeStr()}").AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                    }
                }
                else
                {
                    if (g.getPunteggio() == cpu.getPunteggio())
                        s = $"{this.FindResource("PartitaPatta")}";
                    else
                    {
                        if (g.getPunteggio() > cpu.getPunteggio())
                            s = $"{this.FindResource("HaiVinto")}";
                        else
                            s = $"{this.FindResource("HaiPerso")}";
                        s = $"{s} {this.FindResource("per")} {Math.Abs(g.getPunteggio() - cpu.getPunteggio())} {this.FindResource("punti")}";
                    }
                    fpRisultrato.Content = $"{this.FindResource("PartitaFinita")}. {s} {this.FindResource("NuovaPartita")}?";
                    Applicazione.Visibility = Visibility.Collapsed;
                    FinePartita.Visibility = Visibility.Visible;
                }
                t.Stop();
            };
        }
        private Image giocaUtente(Image img)
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
            g.gioca(quale);
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
            }
            if (!mazzi.Contains("Napoletano"))
                mazzi.Add("\\Napoletano");
            mazzi.Sort();
            lsmazzi.Items.Clear();
            foreach (var s in mazzi)
            lsmazzi.Items.Add(s.Substring(s.LastIndexOf("\\")+1));
            Info.Visibility = Visibility.Collapsed;
            Applicazione.Visibility = Visibility.Collapsed;
            GOpzioni.Visibility = Visibility.Visible;
            txtNomeUtente.Text = g.getNome();
            txtCpu.Text = cpu.getNome();
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
            e = new elaboratoreCarteBriscola(cartaBriscola);
            m = new mazzo(e);
            briscola = carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
            g = new giocatore(new giocatoreHelperUtente(), g.getNome(), 3);
            cpu = new giocatore(new giocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), cpu.getNome(), 3);
            for (UInt16 i = 0; i < 3; i++)
            {
                g.addCarta(m);
                cpu.addCarta(m);

            }
            Utente0.Source = g.getImmagine(0);
            Utente0.Visibility = Visibility.Visible;
            Utente1.Source = g.getImmagine(1);
            Utente1.Visibility = Visibility.Visible;
            Utente2.Source = g.getImmagine(2);
            Utente2.Visibility = Visibility.Visible;
            Cpu0.Source = cartaCpu.Source;
            Cpu0.Visibility = Visibility.Visible;
            Cpu1.Source = cartaCpu.Source;
            Cpu1.Visibility = Visibility.Visible;
            Cpu2.Source = cartaCpu.Source;
            Cpu2.Visibility = Visibility.Visible;
            Giocata0.Visibility = Visibility.Collapsed;
            Giocata1.Visibility = Visibility.Collapsed;
            PuntiUtente.Content = $"Punti di {g.getNome()}: {g.getPunteggio()}";
            PuntiCpu.Content = $"{this.FindResource("PuntiDi")} {cpu.getNome()}: {cpu.getPunteggio()}";
            PuntiUtente.Content = $"{this.FindResource("PuntiDi")} {g.getNome()}: {g.getPunteggio()}";
            NelMazzoRimangono.Content = $"{this.FindResource("NelMazzoRimangono")} {m.getNumeroCarte()} {this.FindResource("carte")}";
            NelMazzoRimangono.Visibility = Visibility.Visible;
            CartaBriscola.Content = $"{this.FindResource("IlSemeDiBriscolaE")}: {briscola.getSemeStr()}";
            CartaBriscola.Visibility = Visibility.Visible;
            lbmazzi.Content = $"{this.FindResource("Mazzo")} :";
            fpOk.Content = $"{this.FindResource("Ok")}";
            fpCancel.Content = $"{this.FindResource("Annulla")}";
            fpShare.Content = $"{this.FindResource("Condividi")}";
            Briscola.Source = briscola.getImmagine();
            Briscola.Visibility = Visibility.Visible;
            primo = g;
            secondo = cpu;
            Briscola.Source = briscola.getImmagine();
            Applicazione.Visibility = Visibility.Visible;
        }
        private void OnCancelFp_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private Image giocaCpu()
        {
            UInt16 quale = 0;
            Image img1 = Cpu0;
            if (primo == cpu)
                cpu.gioca(0);
            else
                cpu.gioca(0, g);
            quale = cpu.getICartaGiocata();
            if (quale == 1)
                img1 = Cpu1;
            if (quale == 2)
                img1 = Cpu2;
            Giocata1.Visibility = Visibility.Visible;
            Giocata1.Source = cpu.getCartaGiocata().getImmagine();
            img1.Visibility = Visibility.Collapsed;
            return img1;
        }
        private static bool aggiungiCarte()
        {
            try
            {
                primo.addCarta(m);
                secondo.addCarta(m);
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
            return true;
        }

        private void Image_Tapped(object Sender, EventArgs arg)
        {
            Image img = (Image)Sender;
            t.Start();
            i = giocaUtente(img);
            if (secondo == cpu)
                i1 = giocaCpu();
        }
        public void OnOk_Click(Object source, EventArgs evt)
        {
            g.setNome(txtNomeUtente.Text);
            cpu.setNome(txtCpu.Text);
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
                txtSecondi.Text = $"{this.FindResource("ValoreNonValido")}";
                return;
            }
            t.Interval = TimeSpan.FromSeconds(secondi);
            NomeUtente.Content = g.getNome();
            NomeCpu.Content = cpu.getNome();
            if (lsmazzi.SelectedValue != null)
            {
                m.setNome(lsmazzi.SelectedValue.ToString());
                carta.CaricaImmagini(m, 40, cartaHelperBriscola.getIstanza(e), this);
                Utente0.Source = g.getImmagine(0);
                Utente1.Source = g.getImmagine(1);
                Utente2.Source = g.getImmagine(2);

                briscola = carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
                Briscola.Source = briscola.getImmagine();
                if (m.getNome() != "Napoletano") 
                    cartaCpu.Source = new BitmapImage(new Uri(@"C:\\Program Files\\wxBriscola\\Mazzi\\" + m.getNome() + "\\retro carte pc.png"));
                else
                    cartaCpu.Source = new BitmapImage(new Uri("pack://application:,,,/resources/images/retro carte pc.png"));

                Cpu0.Source = cartaCpu.Source;
                Cpu1.Source = cartaCpu.Source;
                Cpu2.Source = cartaCpu.Source;
                CartaBriscola.Content = $"{this.FindResource("IlSemeDiBriscolaE")}: {briscola.getSemeStr()}";
            }
            k1.SetValue("NomeUtente", g.getNome());
            k1.SetValue("NomeCpu", cpu.getNome());
            k1.SetValue("Secondi", secondi);
            k1.SetValue("BriscolaDaPunti", briscolaDaPunti);
            k1.SetValue("AvvisaTalloneFinito", avvisaTalloneFinito);
            k1.SetValue("Mazzo", m.getNome());


            GOpzioni.Visibility = Visibility.Collapsed;
            Applicazione.Visibility = Visibility.Visible;
         
        }

        private void OnFPShare_Click(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = $"https://twitter.com/intent/tweet?text=Con%20la%20CBriscola%20la%20partita%20{g.getNome()}%20contro%20{cpu.getNome()}%20%C3%A8%20finita%20{g.getPunteggio()}%20a%20{cpu.getPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscola.wpf",
                UseShellExecute = true
            };
            Process.Start(psi);
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
