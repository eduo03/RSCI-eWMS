
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Debenhams.Activities;

namespace Debenhams
{
	//[Activity (MainLauncher = true, Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	//
	[Activity (Label="Debenhams", Theme = "@android:style/Theme.Light.NoTitleBar")]			
	public class Login : Activity
	{
		#region > Variables <


		private Button btnLogin = null;

		#endregion

		#region > Events <

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			CopyExistingDB ();

			btnLogin = FindViewById<Button>(Resource.Id.myButton);
			//btnLogin.Click += new EventHandler (Login_Click);

		}

		#endregion

		#region > Methods <

		private void Login_Click(object sender, EventArgs e)
		{
			
		}

		private void CopyExistingDB()
		{
			string dbName = "DebenhamsDB";
			string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);

			//if (File.Exists(dbPath))
			//{
				using (BinaryReader br = new BinaryReader(Assets.Open(dbName)))
				{
					using (BinaryWriter bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
					{
						byte[] buffer = new byte[2048];
						int len = 0;
						while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
						{
							bw.Write(buffer, 0, len);
						}
						bw.Flush();
						bw.Close();
					}
				}
			//}
			var intent = new Intent ();
			intent.SetClass (this, typeof(ActRPoList));
			StartActivity (intent);
		}

		#endregion
	}
}

