using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Debenhams.Models;
using Debenhams.DataAccess;
using Debenhams;


namespace Debenhams.Adapter
{
	public class AdpPTLListScan : BaseAdapter
	{
		private List<tblPickingListDetail> _items;
		private Activity _context;

		public AdpPTLListScan(Activity context, List<tblPickingListDetail> items)
		{
			_context = context;
			_items = items;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LinearLayout view;
			tblPickingListDetail item;
			item = _items.ElementAt (position);

			view = (convertView
				?? _context.LayoutInflater.Inflate (Resource.Layout.LayPTLScanLists, parent, false)
			) as LinearLayout;

			view.FindViewById<TextView>(Resource.Id.lblDept).Text = item.dept;
			view.FindViewById<TextView>(Resource.Id.lblStyle).Text = item.style;
			view.FindViewById<TextView>(Resource.Id.lblSKU).Text = item.sku;
			view.FindViewById<TextView>(Resource.Id.lblUPC).Text = item.upc;
			view.FindViewById<TextView>(Resource.Id.lblOQty).Text = item.oqty;
			view.FindViewById<TextView>(Resource.Id.lblRQty).Text = item.rqty;

			return view;
		}

		public override int Count
		{
			get 
			{ 
				return _items.Count();
			}
		}

		public tblPickingListDetail GetItemDetail(int position)
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