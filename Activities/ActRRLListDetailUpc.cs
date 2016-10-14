﻿
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
	public class ActRRLListDetailUpc : Activity
	{
		private EditText txtponum;
		private EditText txtdivision;
		//private EditText txtslot;
		private EditText txtupc;
		private EditText txtdescription;
		private EditText txtqty;
		private Button btndone,btnUnload,btnqtyminus;

		tblRRLListDetail RRLListDetail =new tblRRLListDetail();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayRSTUpc);
			txtponum = FindViewById<EditText> (Resource.Id.txtponum);
			txtdivision = FindViewById<EditText> (Resource.Id.txtdivision);
			txtupc = FindViewById<EditText> (Resource.Id.txtupc);
			txtdescription = FindViewById<EditText> (Resource.Id.txtdescription);
			txtqty = FindViewById<EditText> (Resource.Id.txtqty);

			btndone = FindViewById<Button> (Resource.Id.btnDone);
			btnUnload = FindViewById<Button> (Resource.Id.btnUnload);
			btnqtyminus = FindViewById<Button> (Resource.Id.btnqtyminus);
			btndone.Click += new EventHandler (btndone_Clicked);
			btnUnload.Click += new EventHandler (btnUnload_Clicked);
			btnqtyminus.Click += new EventHandler (btnqty_Clicked);

			ViewDetails ();
		}

		private void ViewDetails()
		{
			var scanItem = DBConnection.GetRRLUPCRqty(Convert.ToInt32( Intent.GetStringExtra("id")));
			txtponum.Text = Intent.GetStringExtra("mts_no");
			txtdivision.Text = Intent.GetStringExtra("location");
			txtupc.Text = Intent.GetStringExtra("upc");
			txtdescription.Text = Intent.GetStringExtra("description");
			txtqty.Text = scanItem.rqty;
		}

		private void btndone_Clicked(object sender, EventArgs e)
		{	
			Finish ();
		}

		private void btnUnload_Clicked(object sender, EventArgs e)
		{	
			var builder = new AlertDialog.Builder (this);
			builder.SetTitle (GlobalVariables.GlobalMessage);

			if (Intent.GetStringExtra ("oqty") == "0") 
			{
				builder.SetMessage ("Unload UPC?\n" + txtupc.Text);
				builder.SetPositiveButton ("Yes", Unload_Clicked);
				builder.SetNegativeButton ("No", CancelDialog_Clicked);

			} else 
			{
				builder.SetMessage ("Unable to unload.\nUPC:" + txtupc.Text+" is included in MTS");
				builder.SetPositiveButton ("Ok", delegate {});
			}
			builder.Show ();
		}


		private void Unload_Clicked(object sender, DialogClickEventArgs args)
		{
			tblRRLListDetail item = new tblRRLListDetail ();
			item.id=Convert.ToInt32(Intent.GetStringExtra ("id"));
			DBConnection.DeleteRRLtUPC (item);
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
				RRLListDetail.id = Convert.ToInt32 (Intent.GetStringExtra ("id"));
				RRLListDetail.mts_no = Intent.GetStringExtra ("mts_no");
				RRLListDetail.upc = Intent.GetStringExtra ("upc");
				RRLListDetail.description = Intent.GetStringExtra ("description");
				RRLListDetail.oqty = Intent.GetStringExtra ("oqty");
				RRLListDetail.rqty = txtqty.Text;
				RRLListDetail.status = stat;

				DBConnection.UpdateRRLtListDetail (RRLListDetail);
			}
		}

		private void CancelDialog_Clicked(object sender, DialogClickEventArgs args)
		{


		}
	}
}

