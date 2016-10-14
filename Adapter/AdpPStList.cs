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
	public class AdpPStList : BaseAdapter
	{
		private List<tblPickingSTList> _items;
		private Activity _context;

		public AdpPStList(Activity context, List<tblPickingSTList> items)
		{
			_context = context;
			_items = items;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LinearLayout view;
			tblPickingSTList item;
			item = _items.ElementAt (position);

			view = (convertView
				?? _context.LayoutInflater.Inflate (Resource.Layout.LayPSTListCs, parent, false)
			) as LinearLayout;

			view.FindViewById<TextView>(Resource.Id.txttl_no).Text = item.mts_no;
			view.FindViewById<TextView>(Resource.Id.txtstore_name).Text = item.location_name;
			view.FindViewById<TextView>(Resource.Id.txtfrom_location).Text = item.from_name;
			return view;
		}

		public override int Count
		{
			get 
			{ 
				return _items.Count();
			}
		}

		public tblPickingSTList GetItemDetail(int position)
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