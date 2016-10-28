# Utility
Basic helper functions for my projects.

You need to add code below to your project in LoginAttribute.cs file:

	string ntLogin;
	var ntLoginFilePath = ClientConfigurationHelper.Instance.getValue<string>("NtLoginFilePath");

	try
	{
		ntLogin = System.IO.File.ReadAllText(ntLoginFilePath);
		if (string.IsNullOrEmpty(ntLogin))
		{
			ntLogin = HttpContext.Current.Request.LogonUserIdentity != null ? HttpContext.Current.Request.LogonUserIdentity.Name.ToUpperInvariant() : @"COMPANYDOMAIN\UNKNOWN";
		}
	}
	catch (Exception)
	{
		ntLogin = HttpContext.Current.Request.LogonUserIdentity != null ? HttpContext.Current.Request.LogonUserIdentity.Name.ToUpperInvariant() : @"COMPANYDOMAIN\UNKNOWN";
	}

	var userAuth = Core.Instance.Authentication.Authenticate(new LoginInfo() { UserName = ntLogin, LoginSystem = sid });

Then you need to add key below to your config file:

 	<add key="NtLoginFilePath" value="~filepath~"/>
