using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;

using Android.Content;
using Android.Widget;
using Debenhams.Models;
using Debenhams.DataAccess;
using Debenhams;
using Android.OS;
using SQLite;
using Debenhams.Activities;


namespace Debenhams.Adapter
{
	public class AdpPSTListDetail : BaseAdapter
	{
		private List<tblPickingSTListDetail> _items;
		private Activity _context;

		bool minusqty = true;
		tblPickingSTListDetail PoDetails = new tblPickingSTListDetail();

		public AdpPSTListDetail(Activity context, List<tblPickingSTListDetail> items)
		{
			_context = context;
			_items = items;

		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			minusqty=true;
			LinearLayout view;
			tblPickingSTListDetail item;
			ImageButton btnqtyminus;
			TextView receivedqty;
			item = _items.ElementAt (position);

			view = (convertView
				?? _context.LayoutInflater.Inflate (Resource.Layout.LayPSTListDetails,parent, false)
			) as LinearLayout;

			view.FindViewById<TextView>(Resource.Id.txtupc).Text = item.upc;
			view.FindViewById<TextView>(Resource.Id.txtqty).Text = item.rqty;
			view.FindViewById<TextView>(Resource.Id.txtdescr).Text ="Qty To Pick : " + item.oqty;

			receivedqty = view.FindViewById<TextView> (Resource.Id.txtqty);

			btnqtyminus = view.FindViewById<ImageButton>(Resource.Id.imgbtnqtyminus);

			receivedqty.TextChanged+= (object sender, Android.Text.TextChangedEventArgs e) => 
			{
				minusqty=true;
			};

			btnqtyminus.Click += (object sender, EventArgs e) => 
			{
				tblSTBoxDetail boxdetails = new tblSTBoxDetail();
				tblPickingSTListDetail PTLDetails =new tblPickingSTListDetail();
				if(minusqty==true)  
				{
					if (Convert.ToInt32( receivedqty.Text) > 0)
					{

					
						var boxdetail = DBConnection.ChkSTBoxDetailUPC (GlobalVariables.STbox_code,GlobalVariables.STmts_no,item.upc);
						int upccount = 0;
						if (boxdetail != null) upccount = Convert.ToInt32(boxdetail.rqty);
						if (upccount > 0 ) 
						{
							receivedqty.Text = Convert.ToString( Convert.ToInt32(view.FindViewById<TextView>(Resource.Id.txtqty).Text) - 1);
							view.FindViewById<TextView>(Resource.Id.txtqty).Text = receivedqty.Text;

							string stat = "0";	
							if (Convert.ToInt32 (receivedqty.Text) == Convert.ToInt32 (receivedqty.Text)) stat = "1";

							PTLDetails.id = Convert.ToInt32(item.id);
							PTLDetails.mts_no = item.mts_no;
							PTLDetails.picking_id = item.picking_id;
							PTLDetails.upc =item.upc;
							PTLDetails.oqty =item.oqty;
							PTLDetails.rqty =receivedqty.Text;
							PTLDetails.status = stat;
							DBConnection.UpdatePSTListDetail (PTLDetails);

							boxdetails.id = boxdetail.id;
							boxdetails.box_code = boxdetail.box_code;
							boxdetails.upc_id = boxdetail.upc_id;
							boxdetails.mts_no = boxdetail.mts_no;
							boxdetails.upc = boxdetail.upc;
							boxdetails.rqty = Convert.ToString (Convert.ToInt32 (boxdetail.rqty) - 1);
							DBConnection.UpdateSTBoxDetail (boxdetails);
						}
						else
						{
							var builder = new AlertDialog.Builder(_context);
							builder.SetTitle("Debenhams");
							builder.SetMessage("Unable to continue..\nThere are no UPC ("+ item.upc +")\nIn the selected Box ("+ GlobalVariables.STbox_code +")..");
							builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
							builder.Show ();
						}

						/*
						string stat = "0";
						if (Convert.ToInt32 (item.rqty)-1 == Convert.ToInt32 (item.oqty)) stat = "1";

						PoDetails.id = item.id;
						PoDetails.mts_no = item.mts_no;
						PoDetails.picking_id = item.picking_id;
						PoDetails.upc = item.upc;
						PoDetails.oqty = item.oqty;
						PoDetails.rqty = receivedqty.Text;
						PoDetails.status = stat;
						DBConnection.UpdatePSTListDetail (PoDetails);
						*/
						minusqty=false;
						t = new System.Timers.Timer();
						t.Interval = 500;
						t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
						t.Start();

					}
				}
			};
			return view;
		}

		System.Timers.Timer t;
		protected void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			t.Stop();
			minusqty=true;
		}

		public override int Count
		{
			get 
			{ 
				return _items.Count();
			}
		}

		public tblPickingSTListDetail GetItemDetail(int position)
		{
			return _items.ElementAt (position);
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;

		}

		public override long GetItemId(int position)
		{
			return position;
		}
	}
}
