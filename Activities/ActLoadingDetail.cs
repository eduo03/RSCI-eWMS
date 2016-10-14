using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
	[Activity (Label = "ActLStoreList", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActLoadingDetail : Activity
	{
		private EditText txtloadnum, txtdate, txtscanBox;
		private Button btndone, btnscanbox;
		private Boolean dialogdate=true; 
		private Int32 dating=0;
		private ListView lvpo;
		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";


		tblLoadingListDetail loading =new tblLoadingListDetail();

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayLoadingDetail);
			btndone = FindViewById<Button> (Resource.Id.btnLoadDone);
			btndone.Click += delegate {btnDone_Clicked();};

			txtloadnum = FindViewById<EditText> (Resource.Id.txtLoadnum);
			txtdate = FindViewById<EditText> (Resource.Id.txtdate);
			txtscanBox = FindViewById<EditText> (Resource.Id.txtScanBox);

			txtloadnum.Text = Intent.GetStringExtra ("load_id");
			txtdate.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");

			btnscanbox = FindViewById<Button> (Resource.Id.btnScanBox);

			lvpo = FindViewById<ListView>(Resource.Id.lvpo);

			lvpo.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs>(lvpo_ItemClicked);
			btnscanbox.Click += delegate {Scanning();};
			txtscanBox.AfterTextChanged += delegate {ScanUPC();};

			txtdate.Touch += (sender, e) => 
			{
				dating+=1;
				if (dialogdate==true && dating==1)
				{
					dialogdate=false;
					var inputView = LayoutInflater.Inflate(Resource.Layout.LayInputDATE, null);
					var builder = new AlertDialog.Builder(this);
					var datep = (DatePicker)inputView.FindViewById(Resource.Id.datepicker);
					builder.SetTitle(GlobalVariables.GlobalMessage);
					builder.SetMessage("Change Shipped Date: ");
					builder.SetView(inputView);
					builder.SetPositiveButton("Change", OkDialog_Clicked);
					builder.SetNegativeButton("Cancel", delegate { builder.Dispose(); });
					builder.Show();
				}
				else
				{	
					if (dating==3) dating=0;
					dialogdate=true;
				}
			};
		}
			
		private void OkDialog_Clicked(object sender,DialogClickEventArgs args)
		{
			var dialog = (AlertDialog)sender;
			var datepicker = (DatePicker)dialog.FindViewById (Resource.Id.datepicker);
			txtdate.Text = datepicker.DateTime.ToString ("yyyy-MM-dd");
		}

		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}

		private void refreshItems()
		{
			var items = ItemRepository.GetLoadDetail1(txtloadnum.Text);
			lvpo.Adapter = new AdpLoadingDetail(this, items);
		}


		private void lvpo_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpLoadingDetail)lvpo.Adapter).GetItemDetail(e.Position);
			if (item.status == "Loaded") 
			{
				var builder = new AlertDialog.Builder (this);
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("Unload Box Code : (" + item.box_code + ").");
				builder.SetPositiveButton ("Yes", delegate {
					loading.id = item.id;
					loading.load_id = item.load_id;
					loading.box_code = item.box_code;
					loading.status = "Not Loaded";
					ItemRepository.UpdateLDetailBox1 (loading);
					refreshItems();
				});
				builder.SetNegativeButton ("No", delegate {
					builder.Dispose ();
				});
				builder.Show ();
			}

		}

		private void Scanning()
		{
			txtscanBox.RequestFocus();
			txtscanBox.Text = "";
			//txtscanBox.Text="123\n";
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
					var scanItem = ItemRepository.ChkLBoxExist1 (txtloadnum.Text, txtscanBox.Text);
					if (scanItem != null) 
					{
						if (scanItem.status == "Not Loaded") 
						{
							//var scanItem1 = ItemRepository.ChkLBoxExistArray (txtscanBox.Text,Intent.GetStringExtra("store_id"));
							//foreach (var x in scanItem1) 
							//{
							loading.id = scanItem.id;
							loading.load_id = scanItem.load_id;
							loading.box_code = scanItem.box_code;
							loading.status = "Loaded";
							ItemRepository.UpdateLDetailBox1 (loading);
							//}
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
						builder.SetMessage("You've scan a Box not in PELL\nBox Code :("+ txtscanBox.Text.Substring (0, Convert.ToInt32 (txtscanBox.Text.Length-1)) +").");
						builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
						builder.Show ();
					}
				}
			}
		}


		private void btnDone_Clicked()
		{
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle(GlobalVariables.GlobalMessage);
			var chkbox = ItemRepository.ChkLoading (txtloadnum.Text);
			if (chkbox.Count()==0) 
			{
				builder.SetMessage("Finish Loading?.");
			}
			else
			{
				string tl = "";
				foreach (var i in chkbox) 
				{
					tl=tl + i.box_code +"\n";
				}
				builder.SetMessage("Finish Loading?.\nThe following Box Number is not Loaded\n"+ tl);
			}
			builder.SetPositiveButton("Yes", YesDialog_Clicked);
			builder.SetNegativeButton("No", delegate { builder.Dispose(); });
			builder.Show ();

		}

		private async void YesDialog_Clicked(object sender, DialogClickEventArgs args)
		{
			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Updating PELL...", true);
			try
			{
				var boxes= ItemRepository.ChkLoadingbox(txtloadnum.Text);
				foreach (var i in boxes) 
				{
					var stat="1";
					if (i.status=="Not Loaded")
					{
						stat="0";
					}
					await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateNewLoadingBoxStatus/" + i.load_id + "/" + i.box_code + "/" + stat));
				}
				await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateLoadingStatus/" + txtloadnum.Text + "/" + txtdate.Text));

				ItemRepository.DeleteNewLoadListDetail(txtloadnum.Text);
				ItemRepository.DeleteNewLoadList(txtloadnum.Text);
				progressDialog.Cancel ();
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("PELL Number Successfully Updated");
				builder.SetCancelable (false);
				builder.SetPositiveButton("OK", Closed_Clicked);
				builder.Show ();
			} catch (Exception ex) {
				progressDialog.Cancel ();
				Toast.MakeText (this, "Unable To Update Load Number.\n" + ex.Message, ToastLength.Long).Show ();
			}
		}

		private void Closed_Clicked(object sender, DialogClickEventArgs args)
		{
			Finish ();
		}

	}
}

