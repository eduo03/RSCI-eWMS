
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
using Debenhams.Models;
using Debenhams.Adapter;

namespace Debenhams
{
	[Activity (Label = "ActLBScanList",Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActLBStoreListBox : Activity
	{
		private EditText txtloadno,txttlno, txtstorename,txtscanBox;
		private Button btnscanbox, btndone;
		private ListView lvBox;
		tblLoadListDetailBox boxdetails = new tblLoadListDetailBox();
		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayLBoxScanList);

			txtloadno = FindViewById<EditText> (Resource.Id.txtLoadno);
			txttlno = FindViewById<EditText> (Resource.Id.txtTl_no);
			txtstorename = FindViewById<EditText> (Resource.Id.txtStoreName);
			txtscanBox = FindViewById<EditText> (Resource.Id.txtScanBox);

			btnscanbox = FindViewById<Button> (Resource.Id.btnScanBox);
			btndone = FindViewById<Button> (Resource.Id.btnDone);

			lvBox = FindViewById<ListView> (Resource.Id.lvBox);

			txtloadno.Text= Intent.GetStringExtra("load_id");
			txttlno.Text= Intent.GetStringExtra("move_doc");
			txtstorename.Text= Intent.GetStringExtra("store_name");

			lvBox.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs> (lvBox_ItemClicked);
			btnscanbox.Click += delegate {Scanning();};
			txtscanBox.AfterTextChanged += delegate {ScanUPC();};

			btndone.Click += delegate {
				btnDone_Clicked();
			};
			refreshItems ();
		}

		public override void OnBackPressed()
		{
			//btndone.Click += new EventHandler (btnDone_Clicked);
			btnDone_Clicked ();
		}

		private void refreshItems()
		{
			var items = ItemRepository.GetLDetailBox (txttlno.Text,Intent.GetStringExtra("store_id"));
			lvBox.Adapter = new AdpLBListDetailScan (this, items);
		}

		private void Scanning()
		{
			txtscanBox.RequestFocus();
			txtscanBox.Text = "";
			//txtscanBox.Text="01\n";
			var intent = new Intent();    
			intent.SetAction(ACTION_SOFTSCANTRIGGER);    
			intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
			SendBroadcast(intent); 
		}

		private void ScanUPC()
		{
			if (Convert.ToInt32 (txtscanBox.Text.Length) > 1) 
			{
				if (txtscanBox.Text.Substring (Convert.ToInt32 (txtscanBox.Text.Length) - 1, 1)=="\n") 
				{
					var scanItem = ItemRepository.ChkLBoxExist (txttlno.Text, txtscanBox.Text,Intent.GetStringExtra("store_id"));
					if (scanItem != null) 
					{

						if (scanItem.status == "Not Loaded") 
						{
							var scanItem1 = ItemRepository.ChkLBoxExistArray (txtscanBox.Text,Intent.GetStringExtra("store_id"));
							foreach (var x in scanItem1) 
							{
								boxdetails.id = x.id;
								boxdetails.move_doc = x.move_doc;
								boxdetails.box_code = x.box_code;
								boxdetails.store_id = x.store_id;
								boxdetails.status = "Loaded";
								ItemRepository.UpdateLDetailBox (boxdetails);
							}
							refreshItems ();
						} 
						else
						{
							Toast.MakeText (this, "Unable to continue.\nYou've already scan\nBox Code :"+ txtscanBox.Text.Substring (0, Convert.ToInt32 (txtscanBox.Text.Length-1)), ToastLength.Long).Show();
						}	

					}
					else
					{
						var builder = new AlertDialog.Builder(this);
						builder.SetTitle(GlobalVariables.GlobalMessage);
						builder.SetMessage("You've scan a Box not in T.L\nBox Code :("+ txtscanBox.Text.Substring (0, Convert.ToInt32 (txtscanBox.Text.Length-1)) +").");
						builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
						builder.Show ();
					}
				}
			}
		}

		private void lvBox_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpLBListDetailScan)lvBox.Adapter).GetItemDetail(e.Position);
			if (item.status != "Not Loaded")
			{
				boxdetails.id = item.id;
				boxdetails.move_doc = item.move_doc;
				boxdetails.box_code = item.box_code;
				boxdetails.store_id = item.store_id;
				boxdetails.status = "Not Loaded";

				var builder = new AlertDialog.Builder (this);
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("Unload Box Code : (" + item.box_code + ").");
				builder.SetPositiveButton ("Yes", Ok_Clicked);
				builder.SetNegativeButton ("No", delegate {
					builder.Dispose ();
				});
				builder.Show ();
			}
		}

		private void btnDone_Clicked()
		{
			var chkbox = ItemRepository.ChkLoadBox (txttlno.Text);
			if (chkbox != null) {
				tblLoadListDetail loadlistdetail = new tblLoadListDetail();
				var item = ItemRepository.Updateloaddetail (txtloadno.Text, txttlno.Text);
				loadlistdetail.id = item.id;
				loadlistdetail.load_id = item.load_id;
				loadlistdetail.move_doc = item.move_doc;
				loadlistdetail.store_id = item.store_id;
				loadlistdetail.store_name = item.store_name;
				loadlistdetail.status = "In Process";
				ItemRepository.UpdateLoadListDetail (loadlistdetail);
				var builder = new AlertDialog.Builder (this);
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("Loading Box is not Completed\nDo you want to continue");
				builder.SetPositiveButton ("Yes", delegate {Finish ();});
				builder.SetNegativeButton ("No", delegate {builder.Dispose ();});
				builder.Show ();
			}
			else 
			{
				tblLoadListDetail loadlistdetail = new tblLoadListDetail();
				var item = ItemRepository.Updateloaddetail (txtloadno.Text, txttlno.Text);
				loadlistdetail.id = item.id;
				loadlistdetail.load_id = item.load_id;
				loadlistdetail.move_doc = item.move_doc;
				loadlistdetail.store_id = item.store_id;
				loadlistdetail.store_name = item.store_name;
				loadlistdetail.status = "Complete";
				ItemRepository.UpdateLoadListDetail (loadlistdetail);
				Finish ();
			}
		}

		private void Ok_Clicked(object sender, DialogClickEventArgs args)
		{
			//ItemRepository.UpdateLDetailBox (boxdetails);

			var scanItem1 = ItemRepository.ChkLBoxExistArray1 (boxdetails.box_code,boxdetails.store_id);
			foreach (var x in scanItem1) 
			{
				boxdetails.id = x.id;
				boxdetails.move_doc = x.move_doc;
				boxdetails.box_code = x.box_code;
				boxdetails.store_id = x.store_id;
				boxdetails.status = "Not Loaded";
				ItemRepository.UpdateLDetailBox (boxdetails);
			}
			refreshItems ();
		}
	}
}

