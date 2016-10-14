
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

using Debenhams.DataAccess;
using Debenhams.Adapter;
using Debenhams.Models;
using Debenhams.ApiConnection;

namespace Debenhams.Activities
{
	[Activity (Label = "ActPTScanList",Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActPSTListDetail : Activity
	{

		private ListView lvUpc;
		private EditText txtScanUPC,txtTlno,txtStoreName,txtBoxCode;
		private Button btnScanUpc,btnDone;


		tblPickingSTListDetail PSTLDetails = new tblPickingSTListDetail();
		tblSTBoxDetail boxdetails = new tblSTBoxDetail();

		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayPSTListDetail);

			lvUpc = FindViewById<ListView> (Resource.Id.lvUpc);

			txtScanUPC = FindViewById < EditText> (Resource.Id.txtScanUPC);
			txtTlno= FindViewById < EditText> (Resource.Id.txtTlno);
			txtStoreName= FindViewById < EditText> (Resource.Id.txtStoreName);
			txtBoxCode= FindViewById < EditText> (Resource.Id.txtBoxCode);

			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			btnScanUpc = FindViewById<Button> (Resource.Id.btnScanUpc);

			txtTlno.Text = Intent.GetStringExtra ("move_doc");
			txtStoreName.Text = Intent.GetStringExtra ("store_name");
			txtBoxCode.Text = Intent.GetStringExtra ("box_code");
			GlobalVariables.STmts_no = Intent.GetStringExtra ("move_doc");
			GlobalVariables.STbox_code=Intent.GetStringExtra ("box_code");
			btnScanUpc.Click += delegate {Scanning();};
			btnDone.Click += new EventHandler (btnDone_Clicked);

			lvUpc.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs> (lvUpc_ItemClicked);

			txtScanUPC.AfterTextChanged += delegate {ScanUPC();};
			refreshItems ();
		}

		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}

		private void refreshItems()
		{
			var items = DBConnection.GetPickingSTListDetail (Intent.GetStringExtra("move_doc"));
			lvUpc.Adapter = new AdpPSTListDetail (this, items);
		}


		private void lvUpc_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpPSTListDetail)lvUpc.Adapter).GetItemDetail(e.Position);

			var intent = new Intent();
			intent.SetClass(this, typeof(ActPSTUpc));

			intent.PutExtra("box_code",Intent.GetStringExtra ("box_code"));
			intent.PutExtra("store_id", Intent.GetStringExtra ("store_id"));
			intent.PutExtra("store_name",txtStoreName.Text);
			intent.PutExtra("id", Convert.ToString(item.id));
			intent.PutExtra("move_doc", txtTlno.Text);
			intent.PutExtra("picking_id", item.picking_id);
			intent.PutExtra("upc", item.upc);
			intent.PutExtra("oqty", item.oqty);
			intent.PutExtra("rqty", item.rqty);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

		private void Scanning()
		{
			txtScanUPC.RequestFocus();
			txtScanUPC.Text = "";
			//txtScanUPC.Text="123\n";
			//txtScanUPC.Text="1420056481476\n";
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
					var scanItem = DBConnection.GetPSTUPC (txtTlno.Text,txtScanUPC.Text);
					if (scanItem != null) 
					{
						oqty = scanItem.oqty;
						rqty = scanItem.rqty;

						string stat = "0";
						if (Convert.ToInt32 (rqty)+1 == Convert.ToInt32 (oqty)) {
							stat = "1";
						}
						PSTLDetails.id = scanItem.id;
						PSTLDetails.mts_no = scanItem.mts_no;
						PSTLDetails.picking_id = scanItem.picking_id;
						PSTLDetails.upc = scanItem.upc;
						PSTLDetails.oqty = oqty;
						PSTLDetails.rqty = Convert.ToString (Convert.ToInt32 (rqty) + 1);
						PSTLDetails.status = stat;
						DBConnection.UpdatePSTListDetail (PSTLDetails);

						var boxdetail = DBConnection.ChkSTBoxDetailUPC (txtBoxCode.Text,txtTlno.Text,scanItem.upc);
						if (boxdetail != null) 
						{
							boxdetails.id = boxdetail.id;
							boxdetails.box_code = txtBoxCode.Text;
							boxdetails.upc_id = boxdetail.upc_id;
							boxdetails.mts_no = txtTlno.Text;
							boxdetails.upc = scanItem.upc;
							boxdetails.rqty = Convert.ToString (Convert.ToInt32 (boxdetail.rqty) + 1);
							DBConnection.UpdateSTBoxDetail (boxdetails);
						}
						else
						{
							DBConnection.AddSTBoxDetail (txtBoxCode.Text,scanItem.picking_id, txtTlno.Text, scanItem.upc, "1");
						}

						refreshItems ();
					} 
					else
					{
						var builder = new AlertDialog.Builder(this);
						builder.SetTitle(GlobalVariables.GlobalMessage);
						builder.SetMessage("Invalid UPC ("+ txtScanUPC.Text +")\nYou've scan UPC not in MTS.");
						builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
						builder.Show ();
					}
				}
			}
		}

		private void btnDone_Clicked(object sender, EventArgs e)
		{
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle(GlobalVariables.GlobalMessage);
			builder.SetMessage("Add new Box?");
			builder.SetPositiveButton("Yes",delegate {Finish();});
			builder.SetNegativeButton("No", ClosedTL_Clicked);
			builder.Show ();
		}

		private void ClosedTL_Clicked(object sender, EventArgs e)
		{
			var builder = new AlertDialog.Builder (this);
			//var chkitem = DBConnection.ChkPSTListVariance (txtTlno.Text);
			//if (chkitem.Count () == 0) {
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("Finish Picking?");
			//} else {
			//	string upc = "";
			//	foreach (var i in chkitem) {
			//		upc = upc + i.upc + "\n";
			//	}
			//	builder.SetTitle (GlobalVariables.GlobalMessage);
			//	builder.SetMessage ("Finish Picking?\nThere are still variance with the other following UPC/s\n\n" + upc);
			//}
			builder.SetPositiveButton ("Yes", YesDialog_Clicked);
			builder.SetNegativeButton ("No", delegate {builder.Dispose ();});
			builder.Show ();
		}

		private async void YesDialog_Clicked(object sender, DialogClickEventArgs args)
		{
			
			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Updating TL...", true);
			try
			{
				
				var updateupc=DBConnection.ApiPSTListDetail (txtTlno.Text);
				foreach (var i in updateupc) 
				{
					await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/PSTTLListDetailUpdate/" + i.picking_id + "/" + i.upc + "/" + i.rqty ));
				}
				await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/PSTTLListUpdate/"+ txtTlno.Text));
				progressDialog.Cancel ();

				progressDialog = ProgressDialog.Show (this, "Please wait... ", "Updating Box...", true);

				var updateboxdetail=DBConnection.ApiAddSTBoxDetail (txtTlno.Text);
				foreach (var b in updateboxdetail) 
				{
					await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/AddSTBoxDetail/" + b.upc_id + "/" + b.box_code + "/" + b.rqty));
				}

				var updatebox=DBConnection.ApiAddSTBox (txtTlno.Text);
				foreach (var b in updatebox) 
				{
					var boxcode = DBConnection.GetPTLBoxCodes (b.box_code);
					await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/PSTTLBoxUpdate/"+ b.box_code +"/"+ boxcode.location_id +"/"+ b.mts_no + "/"+ boxcode.number +"/"+ boxcode.total  ));
				}

				tblPickingSTList item = new tblPickingSTList();
				var polist = DBConnection.GetPSTListFirst (txtTlno.Text);
				item.id = polist.id;
				DBConnection.DeletePSTList (item);

				progressDialog.Cancel ();

				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("MTS Successfully Updated");
				builder.SetCancelable (false);
				builder.SetPositiveButton("OK",Closed_Clicked);
				builder.Show();
			} catch (Exception ex) {
				progressDialog.Cancel ();
				Toast.MakeText (this, "Unable To Update TL.\n" + ex.Message, ToastLength.Long).Show ();
			}
		}

		private void Closed_Clicked(object sender, DialogClickEventArgs args)
		{
			var intent = new Intent();
			intent.SetClass(this, typeof(ActPSTList));
			StartActivity(intent);
		}

	}
}

