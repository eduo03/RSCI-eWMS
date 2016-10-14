
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
	public class ActPSTUpc : Activity
	{
		private EditText txtStoreName,txtUPC,txtDescr,txtBoxNo,txtOQty,txtRqty;
		private Button btndone,btnqtyminus;

		tblPickingSTListDetail PTLDetails =new tblPickingSTListDetail();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayPSTUpc);



			txtStoreName = FindViewById<EditText> (Resource.Id.txtStoreName);
			txtUPC = FindViewById<EditText> (Resource.Id.txtUPC);
			txtDescr = FindViewById<EditText> (Resource.Id.txtDescr);
			txtBoxNo = FindViewById<EditText> (Resource.Id.txtBoxNo);
			txtOQty = FindViewById<EditText> (Resource.Id.txtOQty);
			txtRqty = FindViewById<EditText> (Resource.Id.txtRQTY);

			btndone = FindViewById<Button> (Resource.Id.btnDone);
			btnqtyminus = FindViewById<Button> (Resource.Id.btnqtyminus);

			btndone.Click += new EventHandler (btndone_Clicked);
			btnqtyminus.Click += new EventHandler (btnqty_Clicked);
			ViewDetails ();
		}

		private void ViewDetails()
		{
			var scanItem = DBConnection.GetPSTUPCNocut (Intent.GetStringExtra("move_doc"),Intent.GetStringExtra("upc"));

			txtStoreName.Text = Intent.GetStringExtra("store_name");
			txtUPC.Text = Intent.GetStringExtra("upc");
			txtDescr.Text = Intent.GetStringExtra("descr");
			txtBoxNo.Text = Intent.GetStringExtra("box_code");
			//txtOQty.Text = Intent.GetStringExtra("oqty");
			txtRqty.Text = scanItem.rqty;
			var items = DBConnection.ChkBoxupcnumber (Intent.GetStringExtra ("move_doc"), Intent.GetStringExtra ("upc"),Intent.GetStringExtra("box_code"));
			if(items!=null)
			{
				txtOQty.Text = items.rqty;
			}
			else
			{
				txtOQty.Text = "0";
			}
		}

		private void btnqty_Clicked(object sender, EventArgs e)
		{
			tblSTBoxDetail boxdetails = new tblSTBoxDetail();
			if (Convert.ToInt32 (txtRqty.Text) >= 1) 
			{	
				var boxdetail = DBConnection.ChkSTBoxDetailUPC (txtBoxNo.Text,Intent.GetStringExtra ("move_doc"),txtUPC.Text);
				int upccount = 0;
				if (boxdetail != null) upccount = Convert.ToInt32(boxdetail.rqty);
				if (upccount > 0 ) 
				{
					txtRqty.Text = Convert.ToString( Convert.ToInt32 (txtRqty.Text) - 1);
					string stat = "0";	
					if (Convert.ToInt32 (txtRqty.Text) == Convert.ToInt32 (Intent.GetStringExtra ("oqty"))) stat = "1";

					PTLDetails.id = Convert.ToInt32(Intent.GetStringExtra ("id"));
					PTLDetails.mts_no = Intent.GetStringExtra ("move_doc").ToString ();
					PTLDetails.picking_id = Intent.GetStringExtra ("picking_id").ToString ();
					PTLDetails.upc =Intent.GetStringExtra ("upc").ToString ();
					PTLDetails.oqty =Intent.GetStringExtra ("oqty").ToString ();
					PTLDetails.rqty =txtRqty.Text;
					PTLDetails.status = stat;
					DBConnection.UpdatePSTListDetail (PTLDetails);

					boxdetails.id = boxdetail.id;
					boxdetails.box_code = boxdetail.box_code;
					boxdetails.upc_id = boxdetail.upc_id;
					boxdetails.mts_no = boxdetail.mts_no;
					boxdetails.upc = boxdetail.upc;
					boxdetails.rqty = Convert.ToString (Convert.ToInt32 (boxdetail.rqty) - 1);
					DBConnection.UpdateSTBoxDetail (boxdetails);
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

