
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debenhams.Models;
using Debenhams.DataAccess;
using Debenhams.Adapter;
using Debenhams.ApiConnection;

namespace Debenhams.Activities
{
	[Activity (Label = "ActRPoScanList", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActRSTListDetail : Activity
	{
		private ListView lvUpc;

		private EditText txtScanUPC,txtponum,txtdivision;

		private Button btnScanSKU,btnDone,btnAddUpc;

		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";


		tblStList StList = new tblStList();
		tblStListDetail StDetails = new tblStListDetail();

	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayRSTScanList);

			lvUpc = FindViewById<ListView> (Resource.Id.lvUpc);
			txtScanUPC = FindViewById < EditText> (Resource.Id.txtScanUPC);
			txtponum = FindViewById < EditText> (Resource.Id.txtponum);
			txtdivision = FindViewById < EditText> (Resource.Id.txtdivision);
			btnScanSKU = FindViewById<Button> (Resource.Id.btnScanSKU);
			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			btnAddUpc = FindViewById<Button> (Resource.Id.btnAddUPC);


			btnAddUpc.Click += new EventHandler (btnAddUpc_Clicked);
			btnScanSKU.Click += new EventHandler (btnScanUPC_Clicked);
			btnDone.Click += new EventHandler (btnDone_Clicked);
			lvUpc.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs> (lvUpc_ItemClicked);

			txtScanUPC.AfterTextChanged += delegate {ScanUPC();};

			txtponum.Text =  Intent.GetStringExtra("mts_no");
			txtdivision.Text = Intent.GetStringExtra("location"); 

			refreshItems();
		}
		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}
			
		public void refreshItems()
		{
			var items = DBConnection.GetRStListDetail (txtponum.Text);
			lvUpc.Adapter = new AdpRStListDetail (this, items);
		}




		private void lvUpc_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpRStListDetail)lvUpc.Adapter).GetItemDetail(e.Position);

			var intent = new Intent();
			intent.SetClass(this, typeof(ActRSTListDetailUpc));
			intent.PutExtra("id", Convert.ToString(item.id));
			intent.PutExtra("mts_no", item.mts_no );
			intent.PutExtra("upc", item.upc );
			intent.PutExtra("description", item.description );
			intent.PutExtra("oqty", item.oqty );
			intent.PutExtra("rqty", item.rqty );
			intent.PutExtra("status", item.status);
			intent.PutExtra ("location", txtdivision.Text);
			StartActivity(intent);
		}

		private void btnScanUPC_Clicked(object sender, EventArgs e)
		{ 
			txtScanUPC.RequestFocus();
			txtScanUPC.Text = "";
			//txtScanUPC.Text = "1234\n";
			var intent = new Intent();    
			intent.SetAction(ACTION_SOFTSCANTRIGGER);    
			intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
			SendBroadcast(intent); 
		}
			





		private void ScanUPC()
		{
			string oqty,rqty;

			if (Convert.ToInt32 (txtScanUPC.Text.Length) > 1) 
			{
				if (txtScanUPC.Text.Substring (Convert.ToInt32 (txtScanUPC.Text.Length) - 1, 1)=="\n") 
				{
					if (txtScanUPC.Text != ("")) 
					{
						var scanItem = DBConnection.GetRStUPC (txtponum.Text,txtScanUPC.Text);
						if (scanItem != null) 
						{
							oqty = scanItem.oqty;
							rqty = scanItem.rqty;

							string stat = "0";
							if (Convert.ToInt32 (rqty)+1 == Convert.ToInt32 (oqty)) {
								stat = "1";
							}
							StDetails.id = scanItem.id;
							StDetails.mts_no = scanItem.mts_no;
							StDetails.upc = scanItem.upc;
							StDetails.description = scanItem.description;
							StDetails.oqty = oqty;
							StDetails.rqty = Convert.ToString (Convert.ToInt32 (rqty) + 1);
							StDetails.status = stat;
							DBConnection.UpdateRStListDetail (StDetails);
							refreshItems ();
						} 
						else 
						{
							var builder = new AlertDialog.Builder(this);
							builder.SetTitle (GlobalVariables.GlobalMessage);
							builder.SetMessage ("You scanned a UPC not in the MTS.\n\nUPC: "+txtScanUPC.Text+"\nDo you want to add UPC in MTS?");
							builder.SetPositiveButton("Yes", AddInvalidUPC_Clicked);
							builder.SetNegativeButton("No",delegate { builder.Dispose(); });
							builder.Show ();
						}
					}
				}
			}
		}

		private void btnAddUpc_Clicked(object sender, EventArgs e)
		{ 
			var inputView = LayoutInflater.Inflate(Resource.Layout.LayInputCredential, null);
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle("Login");
			builder.SetView(inputView);
			builder.SetPositiveButton("Login", Login_Clicked);
			builder.SetNegativeButton("Cancel", delegate { builder.Dispose(); });
			builder.Show();
		}
		private void Login_Clicked(object sender, DialogClickEventArgs args)
		{
			var inputView = LayoutInflater.Inflate(Resource.Layout.LayInputUpc, null);
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle("Login");
			builder.SetView(inputView);
			builder.SetPositiveButton("Add",  Add_Clicked);
			builder.SetNegativeButton("Cancel", delegate { builder.Dispose(); });
			builder.Show();
		}

		private void Add_Clicked(object sender, DialogClickEventArgs args)
		{
			var dialog = (AlertDialog)sender;
			var txtupc = (EditText)dialog.FindViewById (Resource.Id.txtupc);
			var txtqty = (EditText)dialog.FindViewById (Resource.Id.txtqty);

			var scanItem = DBConnection.GetRSTtUPC1 (txtponum.Text,txtupc.Text);
			if (scanItem == null) {
				DBConnection.AddRSTListDetail (
					txtponum.Text,
					txtupc.Text,
					"Not Available",
					"0",
					txtqty.Text,
					"0"
				);
				refreshItems ();
			}
			else 
			{
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("The UPC ("+ txtupc.Text +") is already exist in MTS.\nUpdate Quantity?");
				builder.SetPositiveButton("Yes", delegate 
					{
						string stat = "0";
						if (Convert.ToInt32 (txtqty.Text)+1 == Convert.ToInt32 (scanItem.rqty)) {
							stat = "1";
						}
						StDetails.id = scanItem.id;
						StDetails.mts_no = scanItem.mts_no;
						StDetails.upc = scanItem.upc;
						StDetails.description = scanItem.description;
						StDetails.oqty = scanItem.oqty;
						StDetails.rqty = txtqty.Text;
						StDetails.status = stat;
						DBConnection.UpdateRSTtListDetail (StDetails);
						refreshItems ();
					});
				builder.SetNegativeButton("No",delegate { builder.Dispose(); });
				builder.Show ();
			}
		}


		private void btnDone_Clicked(object sender, EventArgs e)
		{
			var builder = new AlertDialog.Builder(this);
			//var chkitem =DBConnection.ChkVarianceRStListDetail (txtponum.Text);
			//if (chkitem.Count()==0) 
			//{
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("Done Scanning Item?");
			//} 
			//else
			//{
			//	string upc = "";
			//	foreach (var i in chkitem) 
			//	{
			//		upc=upc+i.upc+"\n";
			//	}
			//	builder.SetTitle(GlobalVariables.GlobalMessage  );
			//	builder.SetMessage ("Done Scanning Item?\nThere are still variance with the other following UPC/s\n"+ upc);
			//}
			builder.SetPositiveButton("Yes", YesDialog_Clicked);
			builder.SetNegativeButton("No", delegate { builder.Dispose(); });
			builder.Show ();
		}

		private void AddInvalidUPC_Clicked(object sender, DialogClickEventArgs args)
		{
			DBConnection.AddRStListDetail (
				txtponum.Text,
				txtScanUPC.Text.Substring (0, Convert.ToInt32 (txtScanUPC.Text.Length - 1)),
				"Not Available",
				"0",
				"1",
				"0"
			);
			refreshItems ();
		}


		private async void YesDialog_Clicked(object sender, DialogClickEventArgs args)
		{

			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Updating MTS...", true);
			try
			{
				var updateupc=DBConnection.GetRStListDetailArray (txtponum.Text);
				foreach (var i in updateupc) 
				{
					if (Convert.ToInt32 (i.oqty) != 0) 
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RSTListDetailUpdate/" + i.mts_no + "/" + i.upc + "/" + i.rqty));
					} else 
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RSTListDetailAdd/" + i.mts_no + "/" + i.upc + "/" + i.rqty));
					}
				}
				await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RSTListUpdateStatus/" + txtponum.Text));
				tblStList item = new tblStList();
				var stlist = DBConnection.GetRStListFirst (txtponum.Text);
				item.id = stlist.id;
				DBConnection.DeleteRStList(item);
				DBConnection.DeleteRSTListDetail(txtponum.Text);
				progressDialog.Cancel ();
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("MTS Successfully Updated");
				builder.SetCancelable (false);
				builder.SetPositiveButton("OK", Closed_Clicked);
				builder.Show ();
			} catch (Exception ex) {
				progressDialog.Cancel ();
				Toast.MakeText (this, "Unable To Update MTS.\n" + ex.Message, ToastLength.Long).Show ();
			}

		}

		private void Closed_Clicked(object sender, DialogClickEventArgs args)
		{
			Finish ();
		}




	}
}

	