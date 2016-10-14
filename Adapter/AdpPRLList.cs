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
	public class AdpPRLList : BaseAdapter
	{
		private List<tblPickingRLList> _items;
		private Activity _context;

		public AdpPRLList(Activity context, List<tblPickingRLList> items)
		{
			_context = context;
			_items = items;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LinearLayout view;
			tblPickingRLList item;
			item = _items.ElementAt (position);

			view = (convertView
				?? _context.LayoutInflater.Inflate (Resource.Layout.LayPTLLists, parent, false)
			) as LinearLayout;

			view.FindViewById<TextView>(Resource.Id.txttl_no).Text = item.mts_no;
			view.FindViewById<TextView>(Resource.Id.txtstore_name).Text = item.location_name;
			return view;
		}

		public override int Count
		{
			get 
			{ 
				return _items.Count();
			}
		}

		public tblPickingRLList GetItemDetail(int position)
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