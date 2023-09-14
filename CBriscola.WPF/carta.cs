/*
  *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 1.1.3
 *
 *  Created by Giulio Sorrentino (numerone) on 29/01/23.
 *  Copyright 2023 Some rights reserved.
 *
 */


using CBriscola.WPF;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace org.altervista.numerone.framework
{
    public class Carta
    {
        private readonly UInt16 seme,
                   valore,
                   punteggio;
        private string semeStr;
        private readonly CartaHelperBriscola helper;
        private readonly static Carta[] carte = new Carta[40];
        private BitmapImage img;

        private Carta(UInt16 n, CartaHelperBriscola h)
        {
            helper = h;
            seme = helper.GetSeme(n);
            valore = helper.GetValore(n);
            punteggio = helper.GetPunteggio(n);
        }
        public static void Inizializza(UInt16 n, CartaHelperBriscola h)
        {
            for (UInt16 i = 0; i < n; i++)
            {
                carte[i] = new Carta(i, h);

            }
        }
        public static Carta GetCarta(UInt16 quale) { return carte[quale]; }
        public UInt16 GetSeme() { return seme; }
        public UInt16 GetValore() { return valore; }
        public UInt16 GetPunteggio() { return punteggio; }
        public string GetSemeStr() { return semeStr; }
        public bool StessoSeme(Carta c1) { if (c1 == null) return false; else return seme == c1.GetSeme(); }
        public int CompareTo(Carta c1)
        {
            if (c1 == null)
                return 1;
            else
                return helper.CompareTo(helper.GetNumero(GetSeme(), GetValore()), helper.GetNumero(c1.GetSeme(), c1.GetValore()));
        }

        public override string ToString()
        {
            return $"{valore + 1} di {semeStr}{(StessoSeme(helper.GetCartaBriscola()) ? "*" : " ")} ";
        }

        public static BitmapImage GetImmagine(UInt16 quale)
        {
            return carte[quale].img;
        }

        public BitmapImage GetImmagine()
        {
            return img;
        }

        public static void CaricaImmagini(Mazzo m, UInt16 n, CartaHelperBriscola helper, ResourceDictionary d)
        {
            String s = "C:\\Program Files\\wxBriscola\\Mazzi\\";
            for (UInt16 i = 0; i < n; i++)
            {
                if (m.GetNome() != "Napoletano")
                    try
                    {
                        carte[i].img = new BitmapImage(new Uri(s + m.GetNome() + "\\" + i + ".png"));
                    }
                    catch (Exception ex)
                    {
                        new ToastContentBuilder().AddArgument((string)d["MazzoIncompleto"] as string).AddText($"{d["CaricatoNapoletano"] as string}").AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                        m.SetNome("Napoletano");
                        CaricaImmagini(m, n, helper, d);
                        return;
                    }
                else
                    carte[i].img = new BitmapImage(new Uri("pack://application:,,,/resources/images/" + i + ".png"));

                carte[i].semeStr = helper.GetSemeStr(i, m.GetNome(), d);
            }
        }
    }
}
