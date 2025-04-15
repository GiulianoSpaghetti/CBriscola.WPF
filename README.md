## CBriscola.WPF

## ATTENZIONE

Il WPF è il vecchio modo di programmare della Microsoft ed è dichiarato deprecato, oggi esiste il WinUI3 per Windows, ed il MAUI multipiattaforma.
E' stato dimostrato che basta modificare leggermente il sorgente per far crashare il programma, per cui questo progetto viene dichiarato DEPRECATO (legacy).
Vi rimando al nuovo progetto che usa le più moderne tecnologie, tra cui il LINQ: https://github.com/GiulianoSpaghetti/TrumpSuitGame

## Descrizione

Quello che avete davanti non è il gioco della briscola come si intende oggi, perché oggi tutti i simulatori di briscola dicono "hai preso l'asso, bravo" e finisce lì. Quello che avete davanti è un simulatore equo e professionale, con punteggio aggiornato in tempo reale, in modo da poter decidere se "rischiare" o meno coscientemente, in WPF, si può installare sia su amd64 che su arm64 e gira nativamente.

Sembra strano a dirsi, ma è hard core (i vecchietti che urlano "guarda che sto piombo a denari" davanti al monitor), perché consente di cambiare in ogni istante l'andamento della partita coscientemente con le proprie scelte.

Questo gioco dimostra che la teoria dei giochi è vera: l'algorimo brevettato funziona su tutti i giochi di carte senza piatto.


## Video di presentazione

https://photos.app.goo.gl/weGrzWJnXSUPeqfH9

## Come installare
## Su Windows

[![winget](https://user-images.githubusercontent.com/49786146/159123313-3bdafdd3-5130-4b0d-9003-40618390943a.png)](https://marticliment.com/wingetui/share?pid=GiulioSorrentino.CBriscola.WPF&pname=CBriscola.WPF&psource=Winget:%20winget)

## Internazionalizzazione
Aprire il file it_IT.xml, tradurlo interamente, salvarlo con la sigla del linguaggio che si è utilizzato, e poi modificare opportunamente il file app.xaml

## Librerie
Per funzionare ha bisogno del .net framework 8.0

## Interazione con la wxBriscola
Questa versione della briscola, oramai, supporta i mazzi di carte aggiuntivi della wxBriscola, basta che siano installati in c:\Program Files (x64)\wxBriscola e compariranno per essere selezionati e salvati.
Il mazzo Napoletano è inserito all'interno dell'eseguibile e quindi sempre e comunque pienamente raggiungibile.

## Bug noti

Quando si modifica il mazzo di gioco con almeno una carta giocata, le carte sul tavolo non vengono cambiate.

## Donazione

http://numerone.altervista.org/donazioni.php
