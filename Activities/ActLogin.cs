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
using SQLite;
using Debenhams.ApiConnection;
using Debenhams.DataAccess;
using Debenhams.Models;
using Debenhams.Activities;
namespace Debenhams.Activities
{
	[Activity (Label = "ActLogin",Theme = "@android:style/Theme.Holo.Light.NoActionBar")]	
	//[Activity (MainLauncher=true, Label = "Debenhams",Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActLogin : Activity
	{
		private Button btnlogin;
		private EditText txtuser,txtpass;
		tblUser tbluser = new tblUser ();
		protected override void OnCreate (Bundle bundle)
		{
			CopyExistingDB ();
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayLogin);

			var url=((WMSApplication)Application).ItemRepository.GetConnectionURL ();
			GlobalVariables.GlobalUrl = url.url;

			txtuser = FindViewById<EditText> (Resource.Id.txtusername);
			txtpass = FindViewById<EditText> (Resource.Id.txtpassword);

			btnlogin = FindViewById<Button> (Resource.Id.btnlogin);
			btnlogin.Click+= new EventHandler(btnlogin_click);

			txtuser.Text="Stock.Piler";
			txtpass.Text="12345";

			var user = ((WMSApplication)Application).ItemRepository.GetUser ();
			if (user.userid!="")
			{
				var intent = new Intent ();
				GlobalVariables.GlobalUserid = user.userid;
				GlobalVariables.GlobalFname = user.fname;
				GlobalVariables.GlobalLname = user.lname;
				intent.SetClass (this, typeof(ActRPoList));
				Toast.MakeText (this,user.fname + " "+user.lname+" Successfully Login.", ToastLength.Long).Show ();
				StartActivity (intent);
			}
		}

		public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Menu) {
				var inputView = LayoutInflater.Inflate(Resource.Layout.LayInputURL, null);
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle("Change Server Address");
				builder.SetMessage("Server Address: "+ GlobalVariables.GlobalUrl);
				builder.SetView(inputView);
				builder.SetPositiveButton("Change", OkDialog_Clicked);
				builder.SetNegativeButton("Cancel", delegate { builder.Dispose(); });
				builder.Show();
			}
			return base.OnKeyUp (keyCode, e);
		}

		private async void btnlogin_click(object sender, EventArgs e)
		{
			//var intent1 = new Intent ();
			//intent1.SetClass (this, typeof(ActRPoList));
			//Toast.MakeText (this, "Stock Piler Successfully Login.", ToastLength.Long).Show ();
			//StartActivity (intent1);

			if (txtuser.Text != "" && txtpass.Text != "") {
				var progressDialog = ProgressDialog.Show (this, "Please wait...", "Verifying Credentials...", true);
				try {
					await (ApiConnection1.UserLogin (GlobalVariables.GlobalUrl + "/login/" + txtuser.Text + "/" + txtpass.Text));

					var users = ItemRepository.UserLoginasd(GlobalVariables.GlobalUserid);
					if(users.userid != "")
					{
						GlobalVariables.GlobalUserid =users.userid;
						GlobalVariables.GlobalFname=users.fname;
						GlobalVariables.GlobalLname=users.lname;
						var intent = new Intent ();
						intent.SetClass (this, typeof(ActRPoList));
						StartActivity (intent);
						Toast.MakeText (this,users.fname + " "+users.lname+" Successfully Login.", ToastLength.Long).Show ();
						progressDialog.Cancel ();
					} else {
						Toast.MakeText (this, "Login Failed\nIncorect UserName Or Password..", ToastLength.Long).Show ();
					}
				} catch (Exception ex) {
						Toast.MakeText (this, "Unable To Connect To Server.\n" + ex.Message, ToastLength.Long).Show ();
				}
				progressDialog.Cancel ();
			} 
			else 
			{
				Toast.MakeText (this, "Unable To Login.\nPlease Complete Your Credential..", ToastLength.Long).Show ();
			}
			txtpass.Text = "";
				
		}
		public override void OnBackPressed ()
		{
			this.FinishAffinity();
		}

		private void OkDialog_Clicked(object sender, DialogClickEventArgs args)
		{
			tblConnectionURL connection = new tblConnectionURL();
			var dialog = (AlertDialog)sender;
			var txtnewurl = (EditText)dialog.FindViewById (Resource.Id.txturl);
			connection.id=1;
			connection.url=txtnewurl.Text;
			((WMSApplication)Application).ItemRepository.UpdateConnectionURL(connection);
			var url=((WMSApplication)Application).ItemRepository.GetConnectionURL ();
			GlobalVariables.GlobalUrl = url.url;
		}
		private void CopyExistingDB()
		{
			string dbName = "DebenhamsDB";
			string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);

			if (!File.Exists(dbPath))
			{
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
			}

		}

	}
}

