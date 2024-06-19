function newChatHud_Init ()
{
	if ($Pref::Chat::CacheLines $= "")
	{
		$Pref::Chat::CacheLines = 1000;
		$Pref::Chat::MaxDisplayLines = 10;
		$Pref::Chat::LineTime = 4000;
	}
	if ($Pref::Chat::CacheLines < 100)
	{
		$Pref::Chat::CacheLines = 100;
	}
	if ($Pref::Chat::CacheLines > 50000)
	{
		$Pref::Chat::CacheLines = 50000;
	}
	if ($Pref::Chat::MaxDisplayLines < 4)
	{
		$Pref::Chat::MaxDisplayLines = 4;
	}
	if ($Pref::Chat::MaxDisplayLines > 100)
	{
		$Pref::Chat::MaxDisplayLines = 100;
	}
	if ($Pref::Chat::LineTime < 1000)
	{
		$Pref::Chat::LineTime = 1000;
	}
	if ($Pref::Chat::LineTime > 60000)
	{
		$Pref::Chat::LineTime = 60000;
	}
	$NewChatSO = new ScriptObject ("")
	{
		class = NewChatSO;
		size = $Pref::Chat::CacheLines;
		maxLines = $Pref::Chat::MaxDisplayLines;
		lineTime = $Pref::Chat::LineTime;
		head = 0;
		tail = 0;
		textObj = newChatText.getId ();
		pageUpEnd = -1;
	};
}

function newChatHud_AddLine (%line)
{
	if (!isObject ($NewChatSO))
	{
		newChatHud_Init ();
	}
	$NewChatSO.addLine (%line);
	newChatText.forceReflow ();
	newMessageHud.updatePosition ();
}

function NewChatSO::displayLatest (%this)
{
	if (isEventPending (%this.displaySchedule))
	{
		cancel (%this.displaySchedule);
	}
	%text = %this.textObj;
	if (isObject (%text))
	{
		%buff = "";
		%currTime = getSimTime ();
		%i = 0;
		while (%i < %this.maxLines)
		{
			%pos = (%this.head - 1) - %i;
			if (%pos == -1)
			{
				%pos = %this.size + %pos;
			}
			if (%currTime - %this.time[%pos] > %this.lineTime || %i == %this.maxLines - 1)
			{
				if (%pos != %this.head - 1)
				{
					%this.displaySchedule = %this.schedule (500, displayLatest);
				}
				%i -= 1;
				while (%i >= 0)
				{
					%pos = (%this.head - 1) - %i;
					if (%pos < 0)
					{
						%pos = %this.size + %pos;
					}
					%buff = %buff @ %this.line[%pos] @ "\n";
					%i -= 1;
				}
				break;
			}
			%i += 1;
		}
		%text.setValue (%buff);
		%text.forceReflow ();
		newMessageHud.updatePosition ();
	}
	else 
	{
		error ("ERROR: NewChatSO::AddLine() - %this.textObj not defined");
	}
}

function NewChatSO::addLine (%this, %line)
{
	%this.line[%this.head] = %line;
	%this.time[%this.head] = getSimTime ();
	%doPage = 0;
	if (%this.pageUpEnd == %this.head)
	{
		%doPage = 1;
	}
	%this.head += 1;
	if (%this.head >= %this.size)
	{
		%this.head -= %this.size;
	}
	if (%this.head == %this.tail)
	{
		%this.tail += 1;
	}
	if (%this.tail >= %this.size)
	{
		%this.tail = 0;
	}
	if (%this.pageUpEnd == -1)
	{
		%this.displayLatest ();
	}
	if (%doPage)
	{
		%this.pageUpEnd = %this.head;
		%this.displayPage ();
	}
}

function NewChatSO::pageUp (%this)
{
	if (isEventPending (%this.displaySchedule))
	{
		cancel (%this.displaySchedule);
	}
	if (%this.pageUpEnd == -1)
	{
		%this.pageUpEnd = %this.head;
	}
	else if (%this.tail == 0)
	{
		if (%this.pageUpEnd <= %this.tail + %this.maxLines)
		{
			
		}
		else 
		{
			%this.pageUpEnd -= %this.maxLines;
		}
	}
	else if (%this.pageUpEnd - %this.maxLines > %this.maxLines)
	{
		%this.pageUpEnd -= %this.maxLines;
	}
	else if ((%this.tail + %this.maxLines) % %this.size > %this.pageUpEnd - %this.maxLines)
	{
		%this.pageUpEnd = (%this.tail + %this.maxLines) % %this.size;
	}
	else 
	{
		%this.pageUpEnd -= %this.maxLines;
	}
	%this.displayPage ();
}

function NewChatSO::pageDown (%this)
{
	if (%this.pageUpEnd == %this.head || %this.pageUpEnd == -1)
	{
		%this.pageUpEnd = -1;
		%this.displayLatest ();
	}
	else 
	{
		if (isEventPending (%this.displaySchedule))
		{
			cancel (%this.displaySchedule);
		}
		if (%this.head - %this.pageUpEnd <= %this.maxLines)
		{
			%this.pageUpEnd = %this.head;
		}
		else 
		{
			%this.pageUpEnd += %this.maxLines;
			if (%this.pageUpEnd >= %this.size)
			{
				%this.pageUpEnd -= %this.size;
			}
		}
		%this.displayPage ();
	}
}

function NewChatSO::displayPage (%this)
{
	%text = %this.textObj;
	if (!isObject (%text))
	{
		error ("ERROR: NewChatSO::DisplayPage() - textObj is not defined for object " @ %this.getName () @ " (" @ %this @ ")");
		return;
	}
	%start = %this.pageUpEnd - %this.maxLines;
	if (%start < %this.tail)
	{
		%start = %this.tail;
	}
	if (%start < 0)
	{
		%start += %this.size;
	}
	%buff = "";
	%i = 0;
	while (%i < %this.maxLines)
	{
		%pos = (%start + %i) % %this.size;
		%buff = %buff @ %this.line[%pos] @ "\n";
		%i += 1;
	}
	%text.setValue (%buff);
}

