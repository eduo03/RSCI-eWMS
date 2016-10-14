
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
using Debenhams.Models;
using Debenhams.DataAccess;
namespace Debenhams.Activities
{
	[Activity (Label = "ActRPoUpc", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActRPoUpc : Activity
	{
		private EditText txtponum;
		private EditText txtdivision;
		//private EditText txtslot;
		private EditText txtupc;
		private EditText txtdescription;
		private EditText txtqty;
		private Button btndone,btnUnload,btnqtyminus,btnupdateqty;

		tblPoListDetail item =new tblPoListDetail();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayRPoUpc);
			txtponum = FindViewById<EditText> (Resource.Id.txtponum);
			txtdivision = FindViewById<EditText> (Resource.Id.txtdivision);
			txtupc = FindViewById<EditText> (Resource.Id.txtupc);
			txtdescription = FindViewById<EditText> (Resource.Id.txtdescription);
			txtqty = FindViewById<EditText> (Resource.Id.txtqty);

			btndone = FindViewById<Button> (Resource.Id.btnDone);
			btnUnload = FindViewById<Button> (Resource.Id.btnUnload);
			btnqtyminus = FindViewById<Button> (Resource.Id.btnqtyminus);
			btnupdateqty = FindViewById<Button> (Resource.Id.btnupdateqty);

			btndone.Click += new EventHandler (btndone_Clicked);
			btnUnload.Click += new EventHandler (btnUnload_Clicked);
			btnqtyminus.Click += new EventHandler (btnqty_Clicked);
			btnupdateqty.Click += new EventHandler (btnUpdateqty_Clicked);
			ViewDetails ();
		}

		private void ViewDetails()
		{
			var scanItem = ItemRepository.GetRPoUPC(Convert.ToInt32( Intent.GetStringExtra("id")));

			txtponum.Text = Intent.GetStringExtra("ponum");
			txtdivision.Text = Intent.GetStringExtra("division");
			//txtslot.Text = Intent.GetStringExtra("slot");
			txtupc.Text = Intent.GetStringExtra("upc");
			txtdescription.Text = Intent.GetStringExtra("description");
			txtqty.Text = scanItem.rqty ;
		}

		private void btnUpdateqty_Clicked(object sender, EventArgs e)
		{ 
			var inputView = LayoutInflater.Inflate(Resource.Layout.LayInputCredential, null);
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle("Authorized person are allowed to update quantity.");
			builder.SetMessage ("Please enter your credential.");
			builder.SetView(inputView);
			builder.SetPositiveButton("Login", Login_Clicked);
			builder.SetNegativeButton("Cancel", delegate { builder.Dispose(); });
			builder.Show();
		}

		private void Login_Clicked(object sender, DialogClickEventArgs args)
		{
			var inputView = LayoutInflater.Inflate(Resource.Layout.LayInputUpc, null);
			var builder = new AlertDialog.Builder(this);

			var txtupc1 = (EditText)inputView.FindViewById (Resource.Id.txtupc);
			var txtqty1 = (EditText)inputView.FindViewById (Resource.Id.txtqty);
			txtupc1.Enabled = false;
			txtupc1.Text = txtupc.Text;
			builder.SetTitle("Update Quantity");
			builder.SetView(inputView);
			//if (txtqty1.Text != "") 
			//{
				builder.SetPositiveButton ("Update", Add_Clicked);
			//}
			//else
			//{
			//	builder.SetPositiveButton ("Update", delegate {
			//		Toast.MakeText(this,"Unable to Continue.\nPlease input valid quantity.",ToastLength.Long).Show();
			//	});
			//}
			builder.SetNegativeButton("Cancel", delegate { builder.Dispose(); });
			builder.Show();
		}

		private void Add_Clicked(object sender, DialogClickEventArgs args)
		{
			var dialog = (AlertDialog)sender;
			var txtupc1 = (EditText)dialog.FindViewById (Resource.Id.txtupc);
			var txtqty1 = (EditText)dialog.FindViewById (Resource.Id.txtqty);

			if (txtqty1.Text != "") {
					
				string stat = "0";	
				try{
					if (Convert.ToInt32 (txtqty1.Text) >= Convert.ToInt32 (Intent.GetStringExtra ("oqty"))) {
						stat = "1";
					}
					item.id = Convert.ToInt32 (Intent.GetStringExtra ("id"));
					item.receiver_num = Intent.GetStringExtra ("receiver_num");
					item.division_id = Intent.GetStringExtra ("division_id");
					item.sku = Intent.GetStringExtra ("sku");
					item.upc = Intent.GetStringExtra ("upc");
					item.description = Intent.GetStringExtra ("description");
					item.oqty = Intent.GetStringExtra ("oqty");
					item.rqty = txtqty1.Text;
					item.status = stat;
					((WMSApplication)Application).ItemRepository.UpdateRPoListDetail (item);
					txtqty.Text = txtqty1.Text;
					Toast.MakeText(this,"Quantity Successfully Updated.",ToastLength.Long).Show();
				}
				catch(Exception ex) 
				{
					Toast.MakeText(this,"Unable to Continue.\nPlease input valid quantity.",ToastLength.Long).Show();
				}
			}
			else
			{
				Toast.MakeText(this,"Unable to Continue.\nPlease input valid quantity.",ToastLength.Long).Show();
			}
		}


		private void btndone_Clicked(object sender, EventArgs e)
		{	
			/*
			string stat= "0";
			if(txtqty.Text.Equals("")) txtqty.Text="0";
			var inta =Intent.GetStringExtra ("status");
			if (Convert.ToInt32(txtqty.Text) == Convert.ToInt32(Intent.GetStringExtra ("oqty"))) 
			{
				stat="1";
			}
			item.id = Convert.ToInt32 (Intent.GetStringExtra ("id"));
			item.receiver_num = Intent.GetStringExtra ("receiver_num");
			item.division_id = Intent.GetStringExtra ("division_id");
			item.upc = Intent.GetStringExtra ("upc");
			item.description = Intent.GetStringExtra ("description");
			item.oqty = Intent.GetStringExtra ("oqty");
			item.rqty = txtqty.Text;
			item.status = stat;

			((WMSApplication)Application).ItemRepository.UpdateRPoListDetail (item);
			*/
			Finish ();
		}

		private void btnUnload_Clicked(object sender, EventArgs e)
		{	
			var builder = new AlertDialog.Builder (this);
			builder.SetTitle (GlobalVariables.GlobalMessage);

			if (Intent.GetStringExtra ("oqty") == "0") 
			{
				builder.SetMessage ("Unload UPC?.\n" + txtupc.Text);
				builder.SetPositiveButton ("Yes", Unload_Clicked);
				builder.SetNegativeButton ("No", CancelDialog_Clicked);

			} else 
			{
				builder.SetMessage ("Unable to unload.\nUPC:" + txtupc.Text+" is included in P.O");
				builder.SetPositiveButton ("Ok", delegate {});
			}
			builder.Show ();
		}


		private void Unload_Clicked(object sender, DialogClickEventArgs args)
		{
			tblPoListDetail item = new tblPoListDetail ();
			item.id=Convert.ToInt32(Intent.GetStringExtra ("id"));
			((WMSApplication)Application).ItemRepository.DeleteUPC (item);
			Finish ();

		}
	
		private void btnqty_Clicked(object sender, EventArgs e)
		{
			if (Convert.ToInt32 (txtqty.Text) >= 1) 
			{	
				txtqty.Text = Convert.ToString( Convert.ToInt32 (txtqty.Text) - 1);
				string stat = "0";	
				if (Convert.ToInt32 (txtqty.Text) >= Convert.ToInt32 (Intent.GetStringExtra ("oqty"))) {
					stat = "1";
				}
				item.id = Convert.ToInt32 (Intent.GetStringExtra ("id"));
				item.receiver_num = Intent.GetStringExtra ("receiver_num");
				item.division_id = Intent.GetStringExtra ("division_id");
				item.sku = Intent.GetStringExtra ("sku");
				item.upc = Intent.GetStringExtra ("upc");
				item.description = Intent.GetStringExtra ("description");
				item.oqty = Intent.GetStringExtra ("oqty");
				item.rqty = txtqty.Text;
				item.status = stat;

				((WMSApplication)Application).ItemRepository.UpdateRPoListDetail (item);
			}
		}

		private void CancelDialog_Clicked(object sender, DialogClickEventArgs args)
		{


		}
	}
}

