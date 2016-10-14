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
	public class ActSTLoadTLStore : Activity
	{
		private EditText txtloadnum, txtdate;
		private Button btndone;
		private Boolean dialogdate=true; 
		private ListView lvpo;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayLBStoreList);
			btndone = FindViewById<Button> (Resource.Id.btnLoadDone);
			btndone.Click += delegate {btnDone_Clicked();};


			txtloadnum = FindViewById<EditText> (Resource.Id.txtLoadnum);
			txtdate = FindViewById<EditText> (Resource.Id.txtdate);

			txtloadnum.Text = Intent.GetStringExtra ("load_id");
			txtdate.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");

			lvpo = FindViewById<ListView>(Resource.Id.lvpo);
			lvpo.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs>(lvpo_ItemClicked);

			txtdate.Touch += (sender, e) => 
			{
				if (dialogdate==true)
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
			var items = DBConnection.GetLoadDetail(txtloadnum.Text);
			lvpo.Adapter = new AdpSTLoadListDetail(this, items);
		}


		private void lvpo_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpSTLoadListDetail)lvpo.Adapter).GetItemDetail(e.Position);
			var intent = new Intent();
			tblSTLoadListStore loadlist = new tblSTLoadListStore();
			if (item.status == "Open") 
			{
				loadlist.id = item.id;
				loadlist.load_id = item.load_id;
				loadlist.move_doc = item.move_doc;
				loadlist.store_id = item.store_id;
				loadlist.store_name = item.store_name;
				loadlist.status = "In Process";
				//DBConnection.UpdateLoadListDetail (loadlist);
			}
			intent.SetClass(this, typeof(ActSTLoadTLBox));
			intent.PutExtra("id",item.id.ToString());
			intent.PutExtra("load_id", item.load_id);
			intent.PutExtra("move_doc", item.move_doc);
			intent.PutExtra("store_id", item.store_id);
			intent.PutExtra("store_name", item.store_name);
			StartActivity(intent);
		}


		private void btnDone_Clicked()
		{
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle(GlobalVariables.GlobalMessage);
			var chkbox = DBConnection.ChkLoad (txtloadnum.Text);
			if (chkbox.Count()==0) 
			{
				builder.SetMessage("Finish Loading?.");
			}
			else
			{
				string tl = "";
				foreach (var i in chkbox) 
				{
					tl=tl+i.move_doc+" "+i.store_name +"\n";
				}
				builder.SetMessage("Finish Loading?.\nThe following TL Number is not Complete\n"+ tl);
			}
			builder.SetPositiveButton("Yes", YesDialog_Clicked);
			builder.SetNegativeButton("No", delegate { builder.Dispose(); });
			builder.Show ();

		}
		private async void YesDialog_Clicked(object sender, DialogClickEventArgs args)
		{
			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Updating Load...", true);
			try
			{
				var boxes= DBConnection.ChkloadList123(txtloadnum.Text);
				foreach (var i in boxes) 
				{
					var stat="1";
					if (i.status=="Not Loaded")
					{
						stat="0";
					}
					await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateSTLoadingBoxStatus/" + i.move_doc + "/" + i.box_code + "/" + stat));
				}
				await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateLoadingStatus/" + txtloadnum.Text + "/" + txtdate.Text));

				tblSTLoadList item = new tblSTLoadList();
				var load = DBConnection.GetLBListFirst (txtloadnum.Text);
				item.id = load.id;
				DBConnection.DeleteLoadListDetail(load.load_id);

				foreach (var i in boxes) 
				{
					DBConnection.DeleteLoadListDetailBox1(i.move_doc);
				}
				DBConnection.DeleteLBList (item);
				progressDialog.Cancel ();
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("Load Number Successfully Updated");
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

