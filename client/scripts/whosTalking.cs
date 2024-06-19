function WhoTalk_Init ()
{
	if (isObject ($WhoTalkSO))
	{
		$WhoTalkSO.delete ();
	}
	$WhoTalkSO = new ScriptObject ("")
	{
		class = WhoTalkSO;
		textObj = chatWhosTalkingText.getId ();
		count = 0;
	};
}

function WhoTalk_addID (%client)
{
	if (!isObject ($WhoTalkSO))
	{
		WhoTalk_Init ();
	}
	$WhoTalkSO.addID (%client);
}

function WhoTalk_removeID (%client)
{
	if (!isObject ($WhoTalkSO))
	{
		return;
	}
	$WhoTalkSO.removeID (%client);
}

function WhoTalkSO::addID (%this, %client)
{
	if (%this.HasID (%client))
	{
		return;
	}
	%this.id[%this.count] = %client;
	%this.count += 1;
	%this.Display ();
}

function WhoTalkSO::removeID (%this, %client)
{
	%i = 0;
	while (%i < %this.count)
	{
		if (%this.id[%i] == %client)
		{
			%j = %i + 1;
			while (%j < %this.count)
			{
				%this.id[%j - 1] = %this.id[%j];
				%j += 1;
			}
			%this.count -= 1;
			%this.Display ();
			return;
		}
		%i += 1;
	}
}

function WhoTalkSO::HasID (%this, %client)
{
	%i = 0;
	while (%i < %this.count)
	{
		if (%this.id[%i] == %client)
		{
			return 1;
		}
		%i += 1;
	}
	return 0;
}

function WhoTalkSO::Display (%this)
{
	%text = %this.textObj;
	if (isObject (%text))
	{
		%buff = "";
		%i = 0;
		while (%i < %this.count)
		{
			%buff = %buff @ " " @ lstAdminPlayerList.getRowTextById (%this.id[%i]);
			%i += 1;
		}
		%text.setText (%buff);
	}
	else 
	{
		error ("ERROR: WhoTalkSO::Display() - text object not found.");
	}
}

