
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
	[Activity (Label = "ActPTLUpc",Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActPTLUpc : Activity
	{
		private EditText txtmts,txtStoreName,txtUPC,txtDescr,txtBoxNo,txtOQty,txtRqty;
		private Button btndone,btnqtyminus,btnupdateqty;

		tblPickingListDetail PTLDetails =new tblPickingListDetail();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayPTLUpc);

			txtmts = FindViewById<EditText> (Resource.Id.txtmts);
			txtStoreName = FindViewById<EditText> (Resource.Id.txtStoreName);
			txtUPC = FindViewById<EditText> (Resource.Id.txtUPC);
			txtDescr = FindViewById<EditText> (Resource.Id.txtDescr);
			txtBoxNo = FindViewById<EditText> (Resource.Id.txtBoxNo);
			txtOQty = FindViewById<EditText> (Resource.Id.txtOQty);
			txtRqty = FindViewById<EditText> (Resource.Id.txtRQTY);

			btndone = FindViewById<Button> (Resource.Id.btnDone);
			btnqtyminus = FindViewById<Button> (Resource.Id.btnqtyminus);
			btnupdateqty = FindViewById<Button> (Resource.Id.btnupdateqty);

			btndone.Click += new EventHandler (btndone_Clicked);
			btnqtyminus.Click += new EventHandler (btnqty_Clicked);
			btnupdateqty.Click += new EventHandler (btnUpdateqty_Clicked);
			ViewDetails ();
		}

		private void ViewDetails()
		{
			txtmts.Text = Intent.GetStringExtra("move_doc");
			txtStoreName.Text = Intent.GetStringExtra("store_name");
			txtUPC.Text = Intent.GetStringExtra("upc");
			txtDescr.Text = Intent.GetStringExtra("descr");
			txtBoxNo.Text = Intent.GetStringExtra("box_code");
			//txtOQty.Text = Intent.GetStringExtra("oqty");
			txtRqty.Text = Intent.GetStringExtra("rqty");
			var items = ItemRepository.ChkBoxupcnumber (Intent.GetStringExtra ("move_doc"), Intent.GetStringExtra ("upc"),Intent.GetStringExtra("box_code"));
			if(items!=null)
			{
				txtOQty.Text = items.rqty;
			}
			else
			{
				txtOQty.Text = "0";
			}
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
			txtupc1.Text = txtUPC.Text;
			builder.SetTitle("Update Quantity");
			builder.SetView(inputView);
			//if (txtqty1.Text != "") 
			//{
			builder.SetPositiveButton ("Add", Add_Clicked);
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

			tblBoxDetail boxdetails = new tblBoxDetail();

			if (txtqty1.Text != "") 
			{
				try{
					if (Convert.ToInt32 (Intent.GetStringExtra("oqty")) >= ((Convert.ToInt32(txtRqty.Text)+Convert.ToInt32(txtqty1.Text)))) 
					{	
						var scanItem = ItemRepository.GetPTLUPC1 (Intent.GetStringExtra ("move_doc"), txtUPC.Text);
						string stat = "0";
						if (Convert.ToInt32 (Intent.GetStringExtra("oqty")) == ((Convert.ToInt32(txtRqty.Text)+Convert.ToInt32(txtqty1.Text)))) 
						{
							stat = "1";
						}
						PTLDetails.id = scanItem.id;
						PTLDetails.move_doc = scanItem.move_doc;
						PTLDetails.picking_id = scanItem.picking_id;
						PTLDetails.upc = scanItem.upc;
						PTLDetails.sku = scanItem.sku;
						PTLDetails.dept = scanItem.dept;
						PTLDetails.style = scanItem.style;
						PTLDetails.descr = scanItem.descr;
						PTLDetails.oqty = scanItem.oqty;
						PTLDetails.rqty = Convert.ToString (Convert.ToInt32 (scanItem.rqty) + Convert.ToInt32 (txtqty1.Text));
						PTLDetails.status = stat;
						ItemRepository.UpdatePTLListDetail (PTLDetails);

						var boxdetail = ItemRepository.ChkBoxDetailUPC (txtBoxNo.Text,Intent.GetStringExtra ("move_doc"),scanItem.upc);
						if (boxdetail != null) 
						{
							boxdetails.id = boxdetail.id;
							boxdetails.box_code = txtBoxNo.Text;
							boxdetails.upc_id = boxdetail.upc_id;
							boxdetails.move_doc =boxdetail.move_doc;
							boxdetails.upc = scanItem.upc;
							boxdetails.rqty = Convert.ToString (Convert.ToInt32 (boxdetail.rqty) + Convert.ToInt32 (txtqty1.Text));
							ItemRepository.UpdateBoxDetail (boxdetails);
						}
						else
						{
							ItemRepository.AddBoxDetail (txtBoxNo.Text,scanItem.picking_id, Intent.GetStringExtra ("move_doc"), scanItem.upc, txtqty1.Text);
						}
						txtOQty.Text = Convert.ToString (Convert.ToInt32 (boxdetail.rqty) + Convert.ToInt32 (txtqty1.Text));
						txtRqty.Text=  Convert.ToString (Convert.ToInt32 (scanItem.rqty) + Convert.ToInt32 (txtqty1.Text));
					} 
					else
					{
						Toast.MakeText(this,"Unable to Continue.\nQuantity over picked.",ToastLength.Long).Show();
					}
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

		private void btnqty_Clicked(object sender, EventArgs e)
		{
			tblBoxDetail boxdetails = new tblBoxDetail();
			if (Convert.ToInt32 (txtRqty.Text) >= 1) 
			{	
				var boxdetail = ItemRepository.ChkBoxDetailUPC (txtBoxNo.Text,Intent.GetStringExtra ("move_doc"),txtUPC.Text);
				int upccount = 0;
				if (boxdetail != null) upccount = Convert.ToInt32(boxdetail.rqty);
				if (upccount > 0 ) 
				{
					txtOQty.Text = Convert.ToString( Convert.ToInt32 (txtOQty.Text) - 1);
					txtRqty.Text = Convert.ToString( Convert.ToInt32 (txtRqty.Text) - 1);
					string stat = "0";	
					if (Convert.ToInt32 (txtRqty.Text) == Convert.ToInt32 (Intent.GetStringExtra ("oqty"))) stat = "1";

					PTLDetails.id = Convert.ToInt32(Intent.GetStringExtra ("id"));
					PTLDetails.move_doc = Intent.GetStringExtra ("move_doc").ToString ();
					PTLDetails.picking_id = Intent.GetStringExtra ("picking_id").ToString ();
					PTLDetails.upc =Intent.GetStringExtra ("upc").ToString ();
					PTLDetails.sku =Intent.GetStringExtra ("sku").ToString ();
					PTLDetails.dept = Intent.GetStringExtra ("dept").ToString ();
					PTLDetails.style = Intent.GetStringExtra ("style").ToString ();
					PTLDetails.descr =Intent.GetStringExtra ("descr").ToString ();
					PTLDetails.oqty =Intent.GetStringExtra ("oqty").ToString ();
					PTLDetails.rqty =txtRqty.Text;
					PTLDetails.status = stat;
					ItemRepository.UpdatePTLListDetail (PTLDetails);

					boxdetails.id = boxdetail.id;
					boxdetails.box_code = boxdetail.box_code;
					boxdetails.upc_id = boxdetail.upc_id;
					boxdetails.move_doc = boxdetail.move_doc;
					boxdetails.upc = boxdetail.upc;
					boxdetails.rqty = Convert.ToString (Convert.ToInt32 (boxdetail.rqty) - 1);
					ItemRepository.UpdateBoxDetail (boxdetails);
				}
				else
				{
					var builder = new AlertDialog.Builder(this);
					builder.SetTitle(GlobalVariables.GlobalMessage);
					builder.SetMessage("Unable to continue..\nThere is no UPC ("+ txtUPC.Text +")\nIn the selected Box ("+ txtBoxNo.Text +")..");
					builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
					builder.Show ();
				}
			}
		}

		private void btndone_Clicked(object sender, EventArgs e)
		{	
			Finish ();
		}


	}
}

