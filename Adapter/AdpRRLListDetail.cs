﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;

using Android.Content;
using Android.Widget;
using Debenhams.Models;
using Debenhams.DataAccess;
using Debenhams;
using SQLite;
using Debenhams.Activities;

namespace Debenhams.Adapter
{
	public class AdpRRLListDetail : BaseAdapter
	{
		private List<tblRRLListDetail> _items;
		private Activity _context;

		bool minusqty = true;
		tblRRLListDetail RRLDetails = new tblRRLListDetail();

		public AdpRRLListDetail(Activity context, List<tblRRLListDetail> items)
		{
			_context = context;
			_items = items;

		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			minusqty=true;
			LinearLayout view;
			tblRRLListDetail item;
			ImageButton btnqtyminus;
			TextView receivedqty;
			item = _items.ElementAt (position);

			view = (convertView
				?? _context.LayoutInflater.Inflate (Resource.Layout.LayRRLListsDetails,parent, false)
			) as LinearLayout;

			view.FindViewById<TextView>(Resource.Id.txtupc).Text = item.upc;
			view.FindViewById<TextView>(Resource.Id.txtqty).Text = item.rqty;
			view.FindViewById<TextView>(Resource.Id.txtdescr).Text = item.description;

			receivedqty = view.FindViewById<TextView> (Resource.Id.txtqty);

			btnqtyminus = view.FindViewById<ImageButton>(Resource.Id.imgbtnqtyminus);

			receivedqty.TextChanged+= (object sender, Android.Text.TextChangedEventArgs e) => 
			{
				minusqty=true;
			};

			btnqtyminus.Click += (object sender, EventArgs e) => 
			{
				if(minusqty==true)  
				{
					if (Convert.ToInt32( receivedqty.Text) > 0)
					{

						receivedqty.Text = Convert.ToString( Convert.ToInt32( view.FindViewById<TextView>(Resource.Id.txtqty).Text) - 1);

						view.FindViewById<TextView>(Resource.Id.txtqty).Text = receivedqty.Text;
						string stat = "0";
						if (Convert.ToInt32 (item.rqty)-1 == Convert.ToInt32 (item.oqty)) stat = "1";

						RRLDetails.id = item.id;
						RRLDetails.mts_no = item.mts_no;
						RRLDetails.upc = item.upc;
						RRLDetails.description = item.description;
						RRLDetails.oqty = item.oqty;
						RRLDetails.rqty = receivedqty.Text;
						RRLDetails.status = stat;
						DBConnection.UpdateRRLListDetail (RRLDetails);

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

		public tblRRLListDetail GetItemDetail(int position)
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
