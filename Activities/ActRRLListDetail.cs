
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
	public class ActRRLListDetail : Activity
	{
		private ListView lvUpc;

		private EditText txtScanUPC,txtponum,txtdivision,txtSlot,txtScanSLOT;

		private Button btnScanSKU,btnDone,btnAddUpc,btnScanSlot;

		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";


		tblStList StList = new tblStList();
		tblRRLListDetail RLDetails = new tblRRLListDetail();

	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayRRLListsDetail);

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

			btnScanSlot = FindViewById<Button> (Resource.Id.btnScanSlot);
			txtScanSLOT = FindViewById < EditText> (Resource.Id.txtScanSLOT);
			btnScanSlot.Click += new EventHandler (btnScanSLOT_Clicked);
			txtScanSLOT.AfterTextChanged += delegate {ScanSLOT();};

			txtSlot = FindViewById < EditText> (Resource.Id.txtslot);

			txtSlot.Text = Intent.GetStringExtra("slot_num"); 
			txtponum.Text =  Intent.GetStringExtra("mts_no");
			txtdivision.Text = Intent.GetStringExtra("location"); 

		}
		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}
			
		public void refreshItems()
		{
			var items = DBConnection.GetRRLListDetail (txtponum.Text);
			lvUpc.Adapter = new AdpRRLListDetail (this, items);
		}

		private void lvUpc_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpRRLListDetail)lvUpc.Adapter).GetItemDetail(e.Position);

			var intent = new Intent();
			intent.SetClass(this, typeof(ActRRLListDetailUpc));
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

		private void btnAddUpc_Clicked(object sender, EventArgs e)
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
					tblRRLList polist = new tblRRLList ();
					if (txtSlot.Text == "" )
					{
						txtSlot.Text = txtScanSLOT.Text.Substring (0, Convert.ToInt32 (txtScanSLOT.Text.Length-1));
						var polists= DBConnection.GetRSTID(Convert.ToInt32( Intent.GetStringExtra("id")));
						polist.id = polists.id;
						polist.mts_no = polists.mts_no;
						polist.piler_id = polists.piler_id;
						polist.location_id = polists.location_id;
						polist.location_name = polists.location_name;
						polist.slot_num = txtSlot.Text;
						polist.status = "In Process";
						DBConnection.UpdateRSTList (polist);
					} 
					else if(txtSlot.Text != txtScanSLOT.Text.Substring (0, Convert.ToInt32 (txtScanSLOT.Text.Length-1)) ) 
					{
						var builder = new AlertDialog.Builder(this);
						builder.SetTitle (GlobalVariables.GlobalMessage);
						builder.SetMessage ("Change Slot Number :\n"+ txtSlot.Text +"\nTo\n"+ txtScanSLOT.Text.Substring (0, Convert.ToInt32 (txtScanSLOT.Text.Length-1)));
						builder.SetPositiveButton("Yes", delegate {
							txtSlot.Text = txtScanSLOT.Text.Substring (0, Convert.ToInt32 (txtScanSLOT.Text.Length-1));
							var polists= DBConnection.GetRSTID(Convert.ToInt32( Intent.GetStringExtra("id")));
							polist.id = polists.id;
							polist.mts_no = polists.mts_no;
							polist.piler_id = polists.piler_id;
							polist.location_id = polists.location_id;
							polist.location_name = polists.location_name;
							polist.slot_num= txtSlot.Text;
							polist.status = "In Process";
							DBConnection.UpdateRSTList (polist);
						});
						builder.SetNegativeButton("No",delegate { builder.Dispose(); });
						builder.Show ();

					}
				}
			}
		}



		private void Login_Clicked(object sender, DialogClickEventArgs args)
		{
			var inputView = LayoutInflater.Inflate(Resource.Layout.LayInputUpc, null);
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle("ENCODE UPC");
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

			var scanItem = DBConnection.GetRRLtUPC1 (txtponum.Text,txtupc.Text);
			if (scanItem == null) {
				DBConnection.AddRRLListDetail (
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
					RLDetails.id = scanItem.id;
					RLDetails.mts_no = scanItem.mts_no;
					RLDetails.upc = scanItem.upc;
					RLDetails.description = scanItem.description;
					RLDetails.oqty = scanItem.oqty;
					RLDetails.rqty = txtqty.Text;
					RLDetails.status = stat;
					DBConnection.UpdateRRLtListDetail (RLDetails);
					refreshItems ();
				});
				builder.SetNegativeButton("No",delegate { builder.Dispose(); });
				builder.Show ();
			}
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
						var scanItem = DBConnection.GetRRLtUPC (txtponum.Text,txtScanUPC.Text);
						if (scanItem != null) 
						{
							oqty = scanItem.oqty;
							rqty = scanItem.rqty;

							string stat = "0";
							if (Convert.ToInt32 (rqty)+1 == Convert.ToInt32 (oqty)) {
								stat = "1";
							}
							RLDetails.id = scanItem.id;
							RLDetails.mts_no = scanItem.mts_no;
							RLDetails.upc = scanItem.upc;
							RLDetails.description = scanItem.description;
							RLDetails.oqty = oqty;
							RLDetails.rqty = Convert.ToString (Convert.ToInt32 (rqty) + 1);
							RLDetails.status = stat;
							DBConnection.UpdateRRLtListDetail (RLDetails);
							refreshItems ();
						} 
						else 
						{
							//var builder = new AlertDialog.Builder(this);
							//builder.SetTitle (GlobalVariables.GlobalMessage);
							//builder.SetMessage ("You scanned a UPC not in the MTS.\n\nUPC: "+txtScanUPC.Text+"\nDo you want to add UPC in MTS?");
							//builder.SetPositiveButton("Yes", AddInvalidUPC_Clicked);
							//builder.SetNegativeButton("No",delegate { builder.Dispose(); });
							//builder.Show ();
							AddInvalidUPC_Clicked(sender:null,args:null);
						}
					}
				}
			}
		}
		
		private void btnDone_Clicked(object sender, EventArgs e)
		{
			if (txtSlot.Text != "") 
			{
				var builder = new AlertDialog.Builder(this);
				//var chkitem =DBConnection.ChkVarianceRRLListDetail (txtponum.Text);
				//if (chkitem.Count()==0) 
				//{
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("Finish Receiving MTS?");
				//} 
				//else
				//{
				//	string upc = "";
				//	foreach (var i in chkitem) 
				//	{
				//		upc=upc+i.upc+"\n";
				//	}
				//	builder.SetTitle(GlobalVariables.GlobalMessage  );
				//	builder.SetMessage ("Finish Receiving MTS?\nThere are still variance with the other following UPC/s\n\n"+ upc);
				//}
				builder.SetPositiveButton("Yes", YesDialog_Clicked);
				builder.SetNegativeButton("No", delegate { builder.Dispose(); });
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
			DBConnection.AddRRLListDetail (
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
				var updateupc=DBConnection.GetRRLListDetailArray (txtponum.Text);
				var aa=updateupc.ToList().FirstOrDefault();
				foreach (var i in updateupc) 
				{
					if (Convert.ToInt32 (i.oqty) != 0) 
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RRLListDetailUpdate/" + i.mts_no + "/" + i.upc + "/" + i.rqty));
					} else 
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RRLListDetailAdd/"  + i.mts_no + "/" + i.upc + "/" + i.rqty));
					}
				}
				await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RRLListUpdateStatus/"  + txtponum.Text));
				tblRRLList item = new tblRRLList();
				var stlist = DBConnection.GetRRLListFirst (txtponum.Text);
				item.id = stlist.id;
				DBConnection.DeleteRRLListDetail(txtponum.Text);
				DBConnection.DeleteRRLList(item);
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

	