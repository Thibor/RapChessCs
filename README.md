# RapCsChess
C# chess engine

## Setup GUI Arena

You need download program Arena (http://www.playwitharena.com/?Download).

Selest in menu Engines/Install New Engine and choose RapChessCs.exe.
 
 ## Setup GUI Winboard
 
 You need download program Winboard (http://www.open-aurec.com/wbforum/viewtopic.php?t=51528).
 
Inside Winboard directory please create directory <b>Jsuci</b> with file rapchess.js and jsuci.exe, and you should click in menu <b>Engine / Edit Engine List</b> and add line:
 
<b>"RapChessCs" -fd "..\RapCsChess" -fcp "RapChessCs.exe" /fUCI</b>
