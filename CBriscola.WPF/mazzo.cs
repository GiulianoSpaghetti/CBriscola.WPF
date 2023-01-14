/*
 *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 0.1
 *
 *  Created by numerunix on 22/05/22.
 *  Copyright 2022 Some rights reserved.
 *
 */

using System;
namespace CBriscola
{
    class mazzo
    {
        private UInt16[] carte;
        private UInt16 numeroCarte;
        private elaboratoreCarte elaboratore;
        private void mischia()
        {
            for (numeroCarte = 0; numeroCarte < 40; numeroCarte++)
                carte[numeroCarte] = elaboratore.getCarta();
        }

        public mazzo(elaboratoreCarte e)
        {
            elaboratore = e;
            carte = new UInt16[40];
            mischia();
        }
        public UInt16 getNumeroCarte() { return numeroCarte; }
        public UInt16 getCarta()
        {
            if (numeroCarte > 40)
                throw new IndexOutOfRangeException();
            UInt16 c = carte[--numeroCarte];
            return c;
        }
    };
}