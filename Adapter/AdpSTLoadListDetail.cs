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
	public class AdpSTLoadListDetail : BaseAdapter
	{
		private List<tblSTLoadListStore> _items;
		private Activity _context;

		public AdpSTLoadListDetail(Activity context, List<tblSTLoadListStore> items)
		{
			_context = context;
			_items = items;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LinearLayout view;
			tblSTLoadListStore item;
			item = _items.ElementAt (position);

			view = (convertView
				?? _context.LayoutInflater.Inflate (Resource.Layout.LaySTLoadTLDetail, parent, false)
			) as LinearLayout;

			view.FindViewById<TextView>(Resource.Id.txttl_no).Text = item.move_doc;
			view.FindViewById<TextView>(Resource.Id.txtstore_name).Text = item.store_name;
			view.FindViewById<TextView>(Resource.Id.txtstatus).Text = item.status;

			return view;
		}

		public override int Count
		{
			get 
			{ 
				return _items.Count();
			}
		}

		public tblSTLoadListStore GetItemDetail(int position)
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
