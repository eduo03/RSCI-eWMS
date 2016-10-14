
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
	public class ActRPoScanList : Activity
	{
		private ListView lvUpc;

		private EditText txtScanUPC,txtponum,txtdivision,txtSlot,txtScanSLOT;

		private Button btnScanSKU,btnDone,btnScanSlot;
	
		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";


		tblPoList PoList = new tblPoList();
		tblPoListDetail PoDetails = new tblPoListDetail();


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayRPoScanList);

			lvUpc = FindViewById<ListView> (Resource.Id.lvUpc);
			txtScanUPC = FindViewById < EditText> (Resource.Id.txtScanUPC);
			txtponum = FindViewById < EditText> (Resource.Id.txtponum);
			txtdivision = FindViewById < EditText> (Resource.Id.txtdivision);
			btnScanSKU = FindViewById<Button> (Resource.Id.btnScanSKU);
			btnDone = FindViewById<Button> (Resource.Id.btnDone);

			txtScanSLOT = FindViewById < EditText> (Resource.Id.txtScanSLOT);
			txtSlot = FindViewById < EditText> (Resource.Id.txtslot);
			btnScanSlot = FindViewById<Button> (Resource.Id.btnScanSlot);
			btnScanSlot.Click += new EventHandler (btnScanSLOT_Clicked);
			txtScanSLOT.AfterTextChanged += delegate {ScanSLOT();};

			btnScanSKU.Click += new EventHandler (btnScanUPC_Clicked);
			btnDone.Click += new EventHandler (btnDone_Clicked);
			lvUpc.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs> (lvUpc_ItemClicked);

			txtScanUPC.AfterTextChanged += delegate {ScanUPC();};

			txtponum.Text =  Intent.GetStringExtra("po_num");
			txtdivision.Text = Intent.GetStringExtra("division"); 
			txtSlot.Text = Intent.GetStringExtra("slot_num"); 

		}
		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}
			
		public void refreshItems()
		{
			var items = ((WMSApplication)Application).ItemRepository.GetRPoListDetail (Intent.GetStringExtra ("receiver_num").ToString (),Intent.GetStringExtra ("division_id").ToString ());
			lvUpc.Adapter = new AdpRPoListScan (this, items);
		}




		private void lvUpc_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpRPoListScan)lvUpc.Adapter).GetItemDetail(e.Position);

			var intent = new Intent();
			intent.SetClass(this, typeof(ActRPoUpc));
			intent.PutExtra("id", Convert.ToString(item.id));
			intent.PutExtra("ponum", txtponum.Text);
			intent.PutExtra("receiver_num",item.receiver_num);
			intent.PutExtra("division_id", item.division_id);
			intent.PutExtra("division", txtdivision.Text);
			intent.PutExtra("sku", item.sku);
			intent.PutExtra("upc", item.upc);
			intent.PutExtra("description", item.description);
			intent.PutExtra("oqty", item.oqty);
			intent.PutExtra("rqty", item.rqty);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

		private void btnScanUPC_Clicked(object sender, EventArgs e)
		{ 
			txtScanUPC.RequestFocus();
			txtScanUPC.Text = "";
			var intent = new Intent();    
			intent.SetAction(ACTION_SOFTSCANTRIGGER);    
			intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
			SendBroadcast(intent); 
		}

		private void btnScanSLOT_Clicked(object sender, EventArgs e)
		{ 
			txtScanSLOT.RequestFocus();
			txtScanSLOT.Text = "";
			//txtScanSLOT.Text = "123\n";
			var intent = new Intent();    
			intent.SetAction(ACTION_SOFTSCANTRIGGER);    
			intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
			SendBroadcast(intent); 
		}

		private void ScanSLOT()
		{
			if (Convert.ToInt32 (txtScanSLOT.Text.Length) > 1) 
			{
				if (txtScanSLOT.Text.Substring (Convert.ToInt32 (txtScanSLOT.Text.Length) - 1, 1)=="\n") 
				{
					tblPoList polist = new tblPoList ();
					if (txtSlot.Text == "" )
					{
						txtSlot.Text = txtScanSLOT.Text.Substring (0, Convert.ToInt32 (txtScanSLOT.Text.Length-1));
						var polists= ItemRepository.GetRPoID(Convert.ToInt32( Intent.GetStringExtra("id")));
						polist.id = polists.id;
						polist.po_num = polists.po_num;
						polist.receiver_num = polists.receiver_num;
						polist.piler_id = polists.piler_id;
						polist.division_id = polists.division_id;
						polist.division = polists.division;
						polist.slot_num = txtSlot.Text;
						polist.status = "In Process";
						ItemRepository.UpdateRPoList (polist);
					} 
					else if(txtSlot.Text != txtScanSLOT.Text.Substring (0, Convert.ToInt32 (txtScanSLOT.Text.Length-1)) ) 
					{
						var builder = new AlertDialog.Builder(this);
						builder.SetTitle(GlobalVariables.GlobalMessage);
						builder.SetMessage ("Change Slot Number :\n"+ txtSlot.Text +"\nTo\n"+ txtScanSLOT.Text.Substring (0, Convert.ToInt32 (txtScanSLOT.Text.Length-1)));
						builder.SetPositiveButton("Yes", delegate {
						txtSlot.Text = txtScanSLOT.Text.Substring (0, Convert.ToInt32 (txtScanSLOT.Text.Length-1));
						var polists= ItemRepository.GetRPoID(Convert.ToInt32( Intent.GetStringExtra("id")));
						polist.id = polists.id;
						polist.po_num = polists.po_num;
						polist.receiver_num = polists.receiver_num;
						polist.piler_id = polists.piler_id;
						polist.division_id = polists.division_id;
						polist.division = polists.division;
						polist.slot_num= txtSlot.Text;
						polist.status = "In Process";
						ItemRepository.UpdateRPoList (polist);
						});
						builder.SetNegativeButton("No",delegate { builder.Dispose(); });
						builder.Show ();

					}
				}
			}
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
						if (txtSlot.Text != "") 
						{
							var scanItem = ((WMSApplication)Application).ItemRepository.GetRPoUPC (Intent.GetStringExtra("receiver_num"),Intent.GetStringExtra("division_id"),txtScanUPC.Text);
							if (scanItem != null) 
							{
								oqty = scanItem.oqty;
								rqty = scanItem.rqty;

								string stat = "0";
								if (Convert.ToInt32 (rqty)+1 == Convert.ToInt32 (oqty)) {
									stat = "1";
								}
								PoDetails.id = scanItem.id;
								PoDetails.receiver_num = scanItem.receiver_num;
								PoDetails.division_id = scanItem.division_id;
								PoDetails.sku = scanItem.sku;
								PoDetails.upc = scanItem.upc;
								PoDetails.description = scanItem.description;
								PoDetails.oqty = oqty;
								PoDetails.rqty = Convert.ToString (Convert.ToInt32 (rqty) + 1);
								PoDetails.status = stat;

								((WMSApplication)Application).ItemRepository.UpdateRPoListDetail (PoDetails);
								refreshItems ();
							} 
							else 
							{
								//var builder = new AlertDialog.Builder(this);
								//builder.SetTitle(GlobalVariables.GlobalMessage);
								//builder.SetMessage ("You scanned a UPC not in the P.O.\nUPC: "+txtScanUPC.Text+"\nDo you want to add UPC in P.O?");
								//builder.SetPositiveButton("Yes", AddInvalidUPC_Clicked);
								//builder.SetNegativeButton("No",delegate { builder.Dispose(); });
								//builder.Show ();
								AddInvalidUPC_Clicked (sender:null,args:null);
							}
						}
						else
						{
							var builder = new AlertDialog.Builder(this);
							builder.SetTitle(GlobalVariables.GlobalMessage);
							builder.SetMessage("Unable to continue.\nPlease scan slot first.");
							builder.SetCancelable (false);
							builder.SetPositiveButton("OK", delegate {builder.Dispose();});
							builder.Show ();
						}
					}
				}
			}
		}

		private void btnDone_Clicked(object sender, EventArgs e)
		{
			if (txtSlot.Text != "") 
			{
				var builder = new AlertDialog.Builder (this);
				//var chkitem = ((WMSApplication)Application).ItemRepository.ChkRPoListVariance (Intent.GetStringExtra ("receiver_num"), Intent.GetStringExtra ("division_id"));
				//if (chkitem.Count () == 0) {
				builder.SetTitle (GlobalVariables.GlobalMessage);
					builder.SetMessage ("Done Receiving Item?");
				//} else {
				//	string upc = "";
				//	foreach (var i in chkitem) {
				//		upc = upc + i.upc + "\n";
				//	}
				//	builder. ("Debenhams");
				//	builder.SetMessage ("Done Receiving Item?\nThere are still variance with the other following UPC/s\n\n" + upc);
				//}
				builder.SetPositiveButton ("Yes", YesDialog_Clicked);
				builder.SetNegativeButton ("No", delegate {
					builder.Dispose ();
				});
				builder.Show ();
			}
			else
			{
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("Unable to continue.\nPlease scan Slot first.");
				builder.SetCancelable (false);
				builder.SetPositiveButton("OK", delegate {
					builder.Dispose();
				});
				builder.Show ();
			}
		}

		private void AddInvalidUPC_Clicked(object sender, DialogClickEventArgs args)
		{
			ItemRepository.AddRPoListDetail (
				Intent.GetStringExtra ("receiver_num"),
				Intent.GetStringExtra("division_id"),
				"0",
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
			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Updating P.O...", true);
			try
			{

				var updateupc=((WMSApplication)Application).ItemRepository.UpdateRPoListDetail (Intent.GetStringExtra("receiver_num"),Intent.GetStringExtra("division_id"));
				foreach (var i in updateupc) 
				{
					if (Convert.ToInt32 (i.oqty) != 0) 
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RPoListDetailUpdate/" + i.receiver_num + "/" + i.division_id + "/" + i.upc + "/" + i.rqty));
					} else 
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RPoListDetailAdd/" + i.receiver_num + "/" + i.division_id + "/" + i.sku +"/" + i.upc + "/" + i.rqty +"/"+ GlobalVariables.GlobalUserid));
					}
				}
				tblPoList item = new tblPoList();
				var polist = ((WMSApplication)Application).ItemRepository.GetRPoListFirst (Intent.GetStringExtra ("receiver_num"),Intent.GetStringExtra("division_id"));
				item.id = polist.id;
				ItemRepository.DeletePoListDetail(polist.receiver_num);
				((WMSApplication)Application).ItemRepository.DeleteRPoList (item);
				progressDialog.Cancel ();
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("P.O Successfully Updated");
				builder.SetCancelable (false);
				builder.SetPositiveButton("OK", Closed_Clicked);
				builder.Show ();
			} catch (Exception ex) {
				progressDialog.Cancel ();
				Toast.MakeText (this, "Unable To Update P.O.\n" + ex.Message, ToastLength.Long).Show ();
			}
		}

		private void Closed_Clicked(object sender, DialogClickEventArgs args)
		{
			Finish ();
		}




	}
}

	