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
	[Activity (Label="Debenhams", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]	
	public class ActRRLList : Activity
	{
		private EditText txtsearch;
		private ListView lvpo;
		private Button btnsearch;

		tblUser tbluser = new tblUser();
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayRRLList);


			#region >>>>>>>>>>MENU<<<<<<<<<<

			var menuButton = FindViewById (Resource.Id.menuHdrButton);
			menuButton.Click += (sender, e) => {
				leftdrawer();
			};

			var MenuPiler= FindViewById<TextView>(Resource.Id.MenuPilerName);
			MenuPiler.Text=" "+GlobalVariables.GlobalFname+" "+GlobalVariables.GlobalLname;

			var MenuReceivingRL = FindViewById (Resource.Id.MenuReceivingRL);
			MenuReceivingRL.Click += (sender, e) => {leftdrawer ();};


			var MenuReceiveing = FindViewById (Resource.Id.MenuReceiving);
			MenuReceiveing.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActRPoList));
				StartActivity (intent);
			};


			var MenuPicking = FindViewById (Resource.Id.MenuPicking);
			MenuPicking.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActPTLList));
				StartActivity (intent);
			};

			var MenuLoad = FindViewById (Resource.Id.MenuLoad);
			MenuLoad.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActLBList));
				StartActivity (intent);
			};

			var MenuReceivingST = FindViewById (Resource.Id.MenuReceivingST);
			MenuReceivingST.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActRSTList));
				StartActivity (intent);
			};


			var MenuLoadingST = FindViewById (Resource.Id.MenuLoadingST);
			MenuLoadingST.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActSTLoadTL));
				StartActivity (intent);
			};


			var MenuPickingST = FindViewById (Resource.Id.MenuPickingST);
			MenuPickingST.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActPSTList));
				StartActivity (intent);
			};

			var MenuLogout = FindViewById (Resource.Id.MenuLogout);
			MenuLogout.Click += (sender, e) => 
			{
				GlobalVariables.GlobalUserid = "";
				tbluser.id = 1;
				tbluser.userid = "";
				tbluser.password= "";
				tbluser.userid = "";
				tbluser.fname = "";
				tbluser.lname = "";
				tbluser.token = "";
				tbluser.status = "0";
				((WMSApplication)Application).ItemRepository.UserLogin (tbluser);
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActLogin));
				StartActivity (intent);
			};
			#endregion
				
			lvpo = FindViewById<ListView>(Resource.Id.lvpo);
			lvpo.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs>(lvpo_ItemClicked);

			txtsearch = FindViewById<EditText>(Resource.Id.txtsearch);
			txtsearch.AfterTextChanged += delegate 
			{
				refreshItems();
			};

			btnsearch = FindViewById<Button> (Resource.Id.btnsearch);
			btnsearch.Click += delegate {
				ApiRSTlist();
			};
		}
			

		public async void ApiRSTlist()
		{
			var progressDialog = ProgressDialog.Show(this, "Please wait...", "Downloading Data From Server...", true);
			try
			{
				await ApiConnection1.ApiRRLList(GlobalVariables.GlobalUrl +"/RRLList/"+ GlobalVariables.GlobalUserid);
				Toast.MakeText (this,"Downloading Successfully.", ToastLength.Long).Show ();
				refreshItems ();
			}
			catch(Exception ex) 
			{
				Toast.MakeText (this,"Unable To Download Data.\n" + ex.Message, ToastLength.Long).Show ();
			}
			progressDialog.Cancel();
		}

		public override void OnBackPressed()
		{
			leftdrawer ();
		}
	
		private void leftdrawer()
		{
			var menu = FindViewById<ActLeftDrawer> (Resource.Id.ActLeftDrawer);
			menu.AnimatedOpened = !menu.AnimatedOpened;
		}

		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}

		private void refreshItems()
		{
			var items = DBConnection.GetRRLList (txtsearch.Text);
			lvpo.Adapter = new AdpRRLList(this, items);
			var nrf = FindViewById<TextView> (Resource.Id.txtempty);
			if (items.Count()!=0) {
				nrf.Visibility = ViewStates.Invisible;
			} 
			else
			{
				nrf.Visibility = ViewStates.Visible;
			}
		}

		private void lvpo_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			tblRRLList RRLlist = new tblRRLList();
			var item = ((AdpRRLList)lvpo.Adapter).GetItemDetail(e.Position);
			if (item.status == "Open") 
			{
				RRLlist.id = item.id;
				RRLlist.mts_no = item.mts_no;
				RRLlist.piler_id = item.piler_id;
				RRLlist.location_id = item.location_id;
				RRLlist.location_name = item.location_name;
				RRLlist.status = "In Process";
				DBConnection.UpdateRRLList (RRLlist);
			}
			var intent = new Intent();
			intent.SetClass(this, typeof(ActRRLListDetail));
			intent.PutExtra("id",item.id.ToString());
			intent.PutExtra("mts_no", item.mts_no);
			intent.PutExtra("piler_id", item.piler_id);
			intent.PutExtra("location_id", item.location_id);
			intent.PutExtra("location", item.location_name);
			intent.PutExtra("slot_num", item.slot_num);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

	}
}

