function joinServerPassGui::enterPass (%null)
{
	%pass = JSP_txtPass.getValue ();
	if (%pass !$= "")
	{
		$conn = new GameConnection (ServerConnection);
		$conn.setConnectArgs ($pref::Player::Name);
		$conn.setJoinPassword (%pass);
		$conn.connect ($ServerInfo::Address);
	}
}

