package md5718af39fa154ec8f2b1a0282db7f814a;


public class PreMainActivity
	extends android.support.v7.app.ActionBarActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("POSV1.PreMainActivity, POSV1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PreMainActivity.class, __md_methods);
	}


	public PreMainActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PreMainActivity.class)
			mono.android.TypeManager.Activate ("POSV1.PreMainActivity, POSV1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
