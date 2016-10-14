using SQLite;

namespace Debenhams.Models
{
	public class tblPoList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string po_num { get; set; }
		public string receiver_num { get; set; }
		public string piler_id { get; set; }
		public string division_id { get; set; }
		public string division { get; set; }
		public string slot_num { get; set; }
		public string status { get; set; }
	}

	public class tblPoListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string receiver_num { get; set; }
		public string sku { get; set; }
		public string division_id { get; set; }
		public string upc { get; set; }
		public string description { get; set; }
		public string oqty { get; set; }
		public string rqty { get; set; }
		public string status { get; set; }
	}
		
	public class tblUser
	{
		[PrimaryKey]
		public long id { get; set; }
		public string userid { get; set; }
		public string username { get; set; }
		public string password { get; set; }
		public string fname { get; set; }
		public string lname { get; set; }
		public string token { get; set; }
		public string status { get; set; }
	}

	public class tblConnectionURL
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string url { get; set; }
	}

	public class tblProductList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string upc { get; set; }
		public string descr { get; set; }
	}

	public class tblPickingList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string move_doc { get; set; }
		public string piler_id { get; set; }
		public string store_id { get; set; }
		public string store_name { get; set; }
		public string status { get; set; }
	}

	public class tblPickingListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string move_doc { get; set; }
		public string picking_id { get; set; }
		public string upc { get; set; }
		public string sku { get; set; }
		public string dept { get; set; }
		public string style { get; set; }
		public string descr { get; set; }
		public string oqty { get; set; }
		public string rqty { get; set; }
		public string status { get; set; }
	}

	public class tblBox
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string box_code { get; set; }
		public string store_id { get; set; }
		public string move_doc { get; set; }
		public string number { get; set; }
		public string total { get; set; }
	}

	public class tblBoxDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string box_code { get; set; }
		public string upc_id { get; set; }
		public string move_doc { get; set; }
		public string upc { get; set; }
		public string rqty { get; set; }
	}
		
	public class tblLoadList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_id { get; set; }
		public string piler_id { get; set; }
		public string status { get; set; }
	}

	public class tblLoadListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_id { get; set; }
		public string move_doc { get; set; }
		public string store_id { get; set; }
		public string store_name { get; set; }
		public string status { get; set; }
	}

	public class tblLoadListDetailBox
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string move_doc { get; set; }
		public string box_code { get; set; }
		public string store_id { get; set; }
		public string status { get; set; }
	}

	public class tblStoreList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string store_id { get; set; }
		public string store_name { get; set; }
		public string address { get; set; }
	}
		
	public class tblStList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string mts_no { get; set; }
		public string piler_id { get; set; }
		public string location_id { get; set; }
		public string location_name { get; set; }
		public string status { get; set; }
	}

	public class tblStListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string mts_no { get; set; }
		public string upc { get; set; }
		public string description { get; set; }
		public string oqty { get; set; }
		public string rqty { get; set; }
		public string status { get; set; }
	}

	public class tblPickingSTList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string mts_no { get; set; }
		public string piler_id { get; set; }
		public string location_id { get; set; }
		public string location_name { get; set; }
		public string from_id { get; set; }
		public string from_name { get; set; }
		public string status { get; set; }
	}

	public class tblPickingSTListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string mts_no { get; set; }
		public string picking_id { get; set; }
		public string upc { get; set; }
		public string oqty { get; set; }
		public string rqty { get; set; }
		public string status { get; set; }
	}

	public class tblSTBox
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string box_code { get; set; }
		public string location_id { get; set; }
		public string mts_no { get; set; }
		public string number { get; set; }
		public string total { get; set; }
	}

	public class tblSTBoxDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string box_code { get; set; }
		public string upc_id { get; set; }
		public string mts_no { get; set; }
		public string upc { get; set; }
		public string rqty { get; set; }
	}

	public class tblRRLList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string mts_no { get; set; }
		public string piler_id { get; set; }
		public string location_id { get; set; }
		public string location_name { get; set; }
		public string slot_num { get; set; }
		public string status { get; set; }
	}

	public class tblRRLListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string mts_no { get; set; }
		public string upc { get; set; }
		public string description { get; set; }
		public string oqty { get; set; }
		public string rqty { get; set; }
		public string status { get; set; }
	}

	public class tblPickingRLList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string mts_no { get; set; }
		public string piler_id { get; set; }
		public string location_id { get; set; }
		public string location_name { get; set; }
		public string status { get; set; }
	}

	public class tblPickingRLListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string mts_no { get; set; }
		public string picking_id { get; set; }
		public string upc { get; set; }
		public string oqty { get; set; }
		public string rqty { get; set; }
		public string status { get; set; }
	}

	public class tblRLBox
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string box_code { get; set; }
		public string location_id { get; set; }
		public string mts_no { get; set; }
		public string number { get; set; }
		public string total { get; set; }
	}

	public class tblRLBoxDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string box_code { get; set; }
		public string upc_id { get; set; }
		public string mts_no { get; set; }
		public string upc { get; set; }
		public string rqty { get; set; }
	}
		
	public class tblSTLoadList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_id { get; set; }
		public string piler_id { get; set; }
		public string status { get; set; }
	}
		
	public class tblSTLoadListStore
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_id { get; set; }
		public string move_doc { get; set; }
		public string store_id { get; set; }
		public string store_name { get; set; }
		public string status { get; set; }
	}

	public class tblSTLoadListBox
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string move_doc { get; set; }
		public string box_code { get; set; }
		public string store_id { get; set; }
		public string status { get; set; }
	}

	public class tblLoadingList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_id { get; set; }
		public string piler_id { get; set; }
		public string status { get; set; }
	}
	public class tblLoadingListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_id { get; set; }
		public string box_code { get; set; }
		public string status { get; set; }
	}
	public class tblLoadingSTList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_id { get; set; }
		public string piler_id { get; set; }
		public string status { get; set; }
	}
	public class tblLoadingSTListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_id { get; set; }
		public string box_code { get; set; }
		public string status { get; set; }
	}
}

