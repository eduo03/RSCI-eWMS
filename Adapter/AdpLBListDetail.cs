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
	public class AdpLBListDetail : BaseAdapter
	{
		private List<tblLoadListDetail> _items;
		private Activity _context;

		public AdpLBListDetail(Activity context, List<tblLoadListDetail> items)
		{
			_context = context;
			_items = items;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LinearLayout view;
			tblLoadListDetail item;
			item = _items.ElementAt (position);

			view = (convertView
				?? _context.LayoutInflater.Inflate (Resource.Layout.LayLBStoreLists, parent, false)
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

		public tblLoadListDetail GetItemDetail(int position)
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
