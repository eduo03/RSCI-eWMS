
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
using Debenhams.ApiConnection;
using Debenhams.Models;

namespace Debenhams.Activities
{
	[Activity (Label = "ActPBox",Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActPBox : Activity
	{
		private EditText txttl_no, store_name;
		private TextView lbltl_no;
		private Spinner spnbox;
		private Button btncreatebox, btnaddbox, btndonebox;
		private ImageButton imgspnrefresh;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.LayPBox);

			txttl_no = FindViewById<EditText> (Resource.Id.txttl_no);
			store_name = FindViewById<EditText> (Resource.Id.txtstore_name);

			lbltl_no = FindViewById<TextView> (Resource.Id.lbltl_no);

			btncreatebox = FindViewById<Button> (Resource.Id.btncreatebox);
			btnaddbox = FindViewById<Button> (Resource.Id.btnaddbox);
			btndonebox = FindViewById<Button> (Resource.Id.btndonebox);

			spnbox = FindViewById<Spinner> (Resource.Id.spnbox);

			imgspnrefresh=FindViewById<ImageButton> (Resource.Id.imgspnrefresh);

			txttl_no.Text = Intent.GetStringExtra ("move_doc");
			store_name.Text = Intent.GetStringExtra ("store_name");

			var boxlist = ItemRepository.GetBoxList (Intent.GetStringExtra ("store_id")).Select (t => t.box_code).ToList ();
			var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, boxlist);
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spnbox.Adapter = adapter;

			spnbox.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spnbox_clicked);

			btncreatebox.Click += new EventHandler (btncreatebox_Clicked);
			btnaddbox.Click += new EventHandler (btnaddbox_Clicked);
			btndonebox.Click += new EventHandler (btndonebox_Clicked);

			imgspnrefresh.Click += new EventHandler (imgspnrefresh_Clicked);
			imgspnrefresh_Clicked (sender:null,e:null);
		}
		protected override void OnResume()
		{
			base.OnResume();
			//imgspnrefresh_Clicked (sender:null,e:null);
		}

		public override void OnBackPressed()
		{
			Finish ();
		}

		private void spnbox_clicked(object sender,EventArgs e )
		{
			var box = ItemRepository.GetPBoxCode (spnbox.SelectedItem.ToString ());
			if (box != null) 
			{
				lbltl_no.Text = box.move_doc;
				//var boxcount = ItemRepository.GetPBoxCount (lbltl_no.Text);
			}
		}	

		private async void  btnaddbox_Clicked(object sender,EventArgs e)
		{
			var boxcount = ItemRepository.GetPBoxCountAll (txttl_no.Text);
			if (boxcount.Count () != 0)
			{
				var progressDialog = ProgressDialog.Show (this, "Please wait...", "Getting Box Number...", true);
				try {
					await (ApiConnection1.PTLGetLastBoxCode (GlobalVariables.GlobalUrl + "/PTLGetLastBoxCode/" + Intent.GetStringExtra ("store_id")+"/"+ txttl_no.Text));
					GlobalVariables.box_code = Intent.GetStringExtra ("store_id") + GlobalVariables.box_code;
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle (GlobalVariables.GlobalMessage);
					builder.SetMessage ("Adding Box.." +
					"\nBox Code: " + GlobalVariables.box_code +
						"\nMTS No: " + lbltl_no.Text);
					//"\nBox No: " + GlobalVariables.box_total + " Of " + GlobalVariables.box_total);
					builder.SetPositiveButton ("Yes", AddBox_Clicked);
					builder.SetNegativeButton ("No", delegate {
						builder.Dispose ();
					});
					builder.Show ();
				} catch (Exception ex) {
					Toast.MakeText (this, "Unable To Connect To Server.\n" + ex.Message, ToastLength.Long).Show ();
				}
				progressDialog.Cancel ();
			}
			else
			{
				var builder = new AlertDialog.Builder (this);
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("Unable To Add New Box\n in this MTS :("+ txttl_no.Text +").\nPlease create Box first or refresh the Box");
				builder.SetPositiveButton ("Ok", delegate {
					builder.Dispose ();
				});
				builder.Show ();
			}
		}

		private async void  btncreatebox_Clicked(object sender,EventArgs e)
		{
			var boxcount = ItemRepository.GetPBoxCountAll (txttl_no.Text);
			if ( boxcount.Count()==0 ) 
			{
				var progressDialog = ProgressDialog.Show (this, "Please wait...", "Getting Box Number...", true);
				try {
					await (ApiConnection1.PTLGetLastBoxCode (GlobalVariables.GlobalUrl + "/PTLGetLastBoxCode1/" + Intent.GetStringExtra ("store_id")));
					GlobalVariables.box_code = Intent.GetStringExtra ("store_id") + GlobalVariables.box_code;
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle (GlobalVariables.GlobalMessage);
					builder.SetMessage ("Creating Box.." +
					"\nBox Code: " + GlobalVariables.box_code +
						"\nMTS No: " + txttl_no.Text );//+
					//"\nBox No: 1 of 1");
					builder.SetPositiveButton ("Yes", YesDialog_Clicked);
					builder.SetNegativeButton ("No", delegate {
						builder.Dispose ();
					});
					builder.Show ();
				} catch (Exception ex) {
					Toast.MakeText (this, "Unable To Connect To Server.\n" + ex.Message, ToastLength.Long).Show ();
				}
				progressDialog.Cancel ();
			}
			else
			{
				string upc = "";
				foreach (var i in boxcount) 
				{
					upc=upc+i.box_code +"\n";
				}
				var builder = new AlertDialog.Builder (this);
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("Unable To Create New Box." +
					"\nYou've already create a box for this MTS number" +
					"\nBox Number :\n"+ upc +
					"\nOr try Add Box Button");
				builder.SetPositiveButton ("OK", delegate {builder.Dispose ();});
				builder.Show ();
			}
		}


		private async void  imgspnrefresh_Clicked(object sender,EventArgs e)
		{
			var progressDialog = ProgressDialog.Show (this, "Please wait...", "Updating Box Number...", true);
			try 
			{
				tblBox tblboxes = new tblBox();
				var boxcodelist = ItemRepository.GetBoxList (Intent.GetStringExtra ("store_id"));
				foreach (var item in boxcodelist)
				{
					var boxcodelistdetail = ItemRepository.ChkBoxDetailMoveDoc (item.move_doc,item.box_code);
					if (boxcodelistdetail == null) 
					{
						tblboxes.id = item.id;
						ItemRepository.DeleteBoxCode (tblboxes);
					}
				}
			} 
			catch (Exception ex) 
			{
				Toast.MakeText (this, "Unable To Refresh.\n" + ex.Message, ToastLength.Long).Show ();
			}

			try {
				await (ApiConnection1.GetPTLBoxCode (GlobalVariables.GlobalUrl + "/PTLGetBoxCode/" + Intent.GetStringExtra("store_id")));

				var boxlist = ItemRepository.GetBoxList (Intent.GetStringExtra ("store_id")).Select (t => t.box_code).ToList ();
				var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, boxlist);
				adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
				spnbox.Adapter = adapter;
				Toast.MakeText (this, "Box Successfully Refreshed..", ToastLength.Long).Show ();
			} catch (Exception ex) {
				Toast.MakeText (this, "Unable To Connect To Server.\n" + ex.Message, ToastLength.Long).Show ();
			}
			progressDialog.Cancel ();
		}

		private void  btndonebox_Clicked(object sender,EventArgs e)
		{
			try
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActPTLScanList));
				intent.PutExtra ("move_doc", Intent.GetStringExtra ("move_doc"));
				intent.PutExtra ("store_name", Intent.GetStringExtra ("store_name"));
				intent.PutExtra ("box_code", spnbox.SelectedItem.ToString ());
				StartActivity (intent);
			}
			catch
			{
				Toast.MakeText (this, "Unable To Continue..\nPlease Select Box First..", ToastLength.Long).Show ();
			}
		}

		private async void YesDialog_Clicked(object sender, DialogClickEventArgs args)
		{
			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Creating Box Code...", true);

			try
			{
				await (ApiConnection1.ApiPTLBoxValidate(GlobalVariables.GlobalUrl + "/PTLBoxValidate/" + GlobalVariables.box_code));
				if (GlobalVariables.box_count=="0")
				{
					try
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/PTLNewBoxCode/" + GlobalVariables.box_code + "/" + Intent.GetStringExtra("store_id") + "/" + txttl_no.Text + "/1/1"));
						Toast.MakeText (this, "Box Successfully Refresh..", ToastLength.Long).Show ();
						imgspnrefresh_Clicked (sender,args);
					} catch (Exception ex) {
						Toast.MakeText (this, "Unable To Update Box.\n" + ex.Message, ToastLength.Long).Show ();
					}
				}
				else
				{
					Toast.MakeText (this, "Unable To Create Box.\nBox code already create by the other piler.\nPlease try creating box again" , ToastLength.Long).Show ();
				}

			} catch (Exception ex) {
				Toast.MakeText (this, "Unable To Update Box.\n" + ex.Message, ToastLength.Long).Show ();
			}

			progressDialog.Cancel ();
		}

		private async void AddBox_Clicked(object sender, DialogClickEventArgs args)
		{
			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Creating Box Code...", true);
			try
			{
				await (ApiConnection1.ApiPTLBoxValidate(GlobalVariables.GlobalUrl + "/PTLBoxValidate/" + GlobalVariables.box_code));
				if (GlobalVariables.box_count=="0")
				{
					try
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/PTLNewBoxCode/" + GlobalVariables.box_code + "/" + Intent.GetStringExtra("store_id") + "/" + txttl_no.Text + "/"+ GlobalVariables.box_total +"/"+ GlobalVariables.box_total));
						Toast.MakeText (this, "Box Successfully Refreshed..", ToastLength.Long).Show ();

					} catch (Exception ex) {
						Toast.MakeText (this, "Unable To Update Box.\n" + ex.Message, ToastLength.Long).Show ();
					}
				}
				else
				{
					Toast.MakeText (this, "Unable To Create Box.\nBox code already create by the other piler.\nPlease try creating box again" , ToastLength.Long).Show ();
				}
			} 
			catch (Exception ex) 
			{
				Toast.MakeText (this, "Unable To Update Box.\n" + ex.Message, ToastLength.Long).Show ();
			}
			progressDialog.Cancel ();
			imgspnrefresh_Clicked (sender,args);
		}

	}
}

