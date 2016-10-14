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
	public class ActSTLoadTL : Activity
	{
		private EditText txtsearch;
		private ListView lvpo;
		private Button btnsearch;

		tblUser tbluser = new tblUser ();
		protected override void OnCreate (Bundle savedInstanceState)
		{
			//ApiRPolist ();
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LaySTLoadList);


			var menuButton = FindViewById (Resource.Id.menuHdrButton);
			menuButton.Click += (sender, e) => {
				leftdrawer();
			};

			var MenuPiler= FindViewById<TextView>(Resource.Id.MenuPilerName);
			MenuPiler.Text=" "+GlobalVariables.GlobalFname+" "+GlobalVariables.GlobalLname;

			var MenuLoadingST = FindViewById (Resource.Id.MenuLoadingST);
			MenuLoadingST.Click += (sender, e) => {leftdrawer ();};


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

			var MenuReceivingST = FindViewById (Resource.Id.MenuReceivingST);
			MenuReceivingST.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActRSTList));
				StartActivity (intent);
			};

			var MenuPickingST = FindViewById (Resource.Id.MenuPickingST);
			MenuPickingST.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActPSTList));
				StartActivity (intent);
			};

			var MenuLoad = FindViewById (Resource.Id.MenuLoad);
			MenuLoad.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActLoading));
				StartActivity (intent);
			};

			var MenuReceivingRL = FindViewById (Resource.Id.MenuReceivingRL);
			MenuReceivingRL.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActRRLList));
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


			lvpo = FindViewById<ListView>(Resource.Id.lvpo);
			lvpo.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs>(lvpo_ItemClicked);

			txtsearch = FindViewById<EditText>(Resource.Id.txtsearch);
			txtsearch.AfterTextChanged += delegate 
			{
				refreshItems();
			};

			btnsearch = FindViewById<Button> (Resource.Id.btnsearch);
			btnsearch.Click += delegate {
				ApiRPolist();
			};

		}


		public async void ApiRPolist()
		{
			var progressDialog = ProgressDialog.Show(this, "Please wait...", "Downloading Data From Server...", true);
			try
			{
				await ApiConnection1.ApiNewLoadingSTList(GlobalVariables.GlobalUrl +"/NewLoadingSTList/"+ GlobalVariables.GlobalUserid);
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
			var items = ItemRepository.GetLoadList1ST(txtsearch.Text);
			lvpo.Adapter = new AdpLoadingST(this, items);
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
			tblLoadingList loadlist = new tblLoadingList();
			var item = ((AdpLoadingST)lvpo.Adapter).GetItemDetail(e.Position);
			var intent = new Intent();
			if (item.status == "Open") 
			{
				loadlist.id = item.id;
				loadlist.load_id = item.load_id;
				loadlist.piler_id = item.piler_id;
				loadlist.status = "In Process";
				ItemRepository.UpdateLoadList1 (loadlist);
			}
			intent.SetClass(this, typeof(ActSTLoadTLBox));
			intent.PutExtra("id",item.id.ToString());
			intent.PutExtra("load_id", item.load_id);;
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

	}
}

