function clientCmdGameStart (%null)
{
	PlayerListGui.zeroScores ();
}

function clientCmdGameEnd (%null)
{
	alxStopAll ();
	EndGameGuiList.clear ();
	%i = 0;
	while (%i < PlayerListGuiList.rowCount ())
	{
		%text = PlayerListGuiList.getRowText (%i);
		%id = PlayerListGuiList.getRowId (%i);
		EndGameGuiList.addRow (%id, %text);
		%i += 1;
	}
	EndGameGuiList.sortNumerical (1, 0);
	Canvas.setContent (EndGameGui);
}

