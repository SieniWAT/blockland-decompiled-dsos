function SAD (%password)
{
	if (%password !$= "")
	{
		commandToServer ('SAD', %password);
	}
}

function SADSetPassword (%password)
{
	commandToServer ('SADSetPassword', %password);
}

function buildwall ()
{
	%j = 0;
	while (%j < 16)
	{
		%i = 0;
		while (%i < 10)
		{
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('plantBrick');
			commandToServer ('shiftBrick', 0, 0, 3);
			commandToServer ('shiftBrick', 2, 0, -30);
			%i += 1;
		}
		commandToServer ('shiftBrick', -20, 2, 0);
		%j += 1;
	}
}

function stress ()
{
	%j = 0;
	while (%j < 16)
	{
		%i = 0;
		while (%i < 10)
		{
			%k = 0;
			while (%k < 30)
			{
				commandToServer ('plantBrick');
				commandToServer ('shiftBrick', 0, 0, 3);
				%k += 1;
			}
			commandToServer ('shiftBrick', 2, 0, %k * -3);
			%i += 1;
		}
		commandToServer ('shiftBrick', -20, 2, 0);
		%j += 1;
	}
}

function buildstairs ()
{
	%i = 0;
	while (%i < 30)
	{
		commandToServer ('plantBrick');
		commandToServer ('shiftBrick', 1, 0, 3);
		%i += 1;
	}
}

function clientCmdSyncClock (%time)
{
	HudClock.setTime (%time);
}

function GameConnection::prepDemoRecord (%this)
{
	%this.demoChatLines = HudMessageVector.getNumLines ();
	%i = 0;
	while (%i < %this.demoChatLines)
	{
		%this.demoChatText[%i] = HudMessageVector.getLineText (%i);
		%this.demoChatTag[%i] = HudMessageVector.getLineTag (%i);
		echo ("Chat line " @ %i @ ": " @ %this.demoChatText[%i]);
		%i += 1;
	}
}

function GameConnection::prepDemoPlayback (%this)
{
	%i = 0;
	while (%i < %this.demoChatLines)
	{
		HudMessageVector.pushBackLine (%this.demoChatText[%i], %this.demoChatTag[%i]);
		%i += 1;
	}
	Canvas.setContent (PlayGui);
}

function getWords (%phrase, %start, %end)
{
	if (%start > %end)
	{
		return;
	}
	%returnPhrase = getWord (%phrase, %start);
	if (%start == %end)
	{
		return %returnPhrase;
	}
	%i = %start + 1;
	while (%i <= %end)
	{
		%returnPhrase = %returnPhrase @ " " @ getWord (%phrase, %i);
		%i += 1;
	}
	return %returnPhrase;
}

function getLine (%phrase, %lineNum)
{
	%offset = 0;
	%lineCount = 0;
	while (%lineCount <= %lineNum)
	{
		%pos = strpos (%phrase, "\n", %offset);
		if (%pos >= 0)
		{
			%len = %pos - %offset;
		}
		else 
		{
			%len = 99999;
		}
		%line = getSubStr (%phrase, %offset, %len);
		if (%lineCount == %lineNum)
		{
			return %line;
		}
		%lineCount += 1;
		%offset = %pos + 1;
		if (%pos == -1)
		{
			return "";
		}
	}
	return "";
}

function getLineCount (%phrase)
{
	%offset = 0;
	%lineCount = 0;
	while (%offset >= 0)
	{
		%offset = strpos (%phrase, "\n", %offset + 1);
		%lineCount += 1;
	}
	return %lineCount;
}

