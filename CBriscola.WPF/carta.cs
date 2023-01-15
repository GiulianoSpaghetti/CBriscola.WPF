/*
 *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 0.1
 *
 *  Created by numerunix on 22/05/22.
 *  Copyright 2022 Some rights reserved.
 *
 */


using CBriscola.WPF;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CBriscola
{
	class carta {
		private UInt16 seme,
				   valore,
				   punteggio;
		private string semeStr;
		private cartaHelperBriscola helper;
		private static carta[] carte = new carta[40];
		private BitmapImage img;
		private carta(UInt16 n, cartaHelperBriscola h, mazzo m) {
			helper = h;
			seme = helper.getSeme(n);
			valore = helper.getValore(n);
			punteggio = helper.getPunteggio(n);
		}
		public static void inizializza(UInt16 n, cartaHelperBriscola h, mazzo m, MainWindow mw) {
			for (UInt16 i = 0; i < n; i++) {
				carte[i] = new carta(i, h, m);
            }
            CaricaImmagini(m, n, h, mw);
        }
        public static carta getCarta(UInt16 quale) { return carte[quale]; }
		public UInt16 getSeme() { return seme; }
		public UInt16 getValore() { return valore; }
		public UInt16 getPunteggio() { return punteggio; }
		public string getSemeStr() { return semeStr; }
		public bool stessoSeme(carta c1) { if (c1 == null) return false; else return seme == c1.getSeme(); }
		public int CompareTo(carta c1) {
			if (c1 == null)
				return 1;
			else
				return helper.CompareTo(helper.getNumero(getSeme(), getValore()), helper.getNumero(c1.getSeme(), c1.getValore()));
		}

		public override string ToString()
		{
			return $"{ valore + 1} di {semeStr}{(stessoSeme(helper.getCartaBriscola())?"*":" ")} ";
	    }

		public static BitmapImage getImmagine(UInt16 quale)
		{
			return carte[quale].img;
		}

		public BitmapImage getImmagine()
		{
			return img;
		}

		public static void CaricaImmagini(mazzo m, UInt16 n, cartaHelperBriscola helper, MainWindow mw)
		{
			String s = "C:\\Program Files\\wxBriscola\\Mazzi\\";
			for (UInt16 i = 0; i < n; i++)
			{
				if (m.getNome()!="Napoletano")
				try
				{
					carte[i].img = new BitmapImage(new Uri(s + m.getNome() + "\\" + i + ".png"));
				} catch (System.IO.FileNotFoundException ex)
				{
                        new ToastContentBuilder().AddArgument((string)mw.FindResource("MazzoIncompleto") as string).AddText($"{(string)mw.FindResource("CaricatoNapoletano") as string}").AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                        m.setNome("Napoletano");
					CaricaImmagini(m, n, helper, mw);
					return;
                }
				else
                    carte[i].img = new BitmapImage(new Uri("pack://application:,,,/resources/images/" + i + ".png"));

                carte[i].semeStr = helper.getSemeStr(i, m.getNome(), mw);
			}
        }
    }
}
