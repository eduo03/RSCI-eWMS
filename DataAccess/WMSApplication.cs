using System;
using Android.App;
using Android.Runtime;
using Debenhams.DataAccess;

namespace Debenhams.DataAccess
{
	[Application]

	public class WMSApplication : Application
	{
		public IItemRepository ItemRepository { get; set; }

		public WMSApplication(IntPtr handle, JniHandleOwnership transfer)
			: base(handle, transfer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();
			ItemRepository = new OrmItemRepository(this);
		}
	}

	public class GlobalVariables
	{
		public static string GlobalUrl = "";
		public static string GlobalUserid = "";
		public static string GlobalFname = "";
		public static string GlobalLname = "";
		public static string box_code = "";
		public static string box_total = "";
		public static string box_count = "";
		public static string STbox_code = "";
		public static string STmts_no = "";
		public static string STupc = "";
		public static string GlobalMessage = "RSCI(Warehouse)";


	}
}