using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Debenhams.Models;
using Debenhams.DataAccess;
using SQLite;
using Android.Graphics;
using System;

namespace Debenhams.DataAccess
{
	public class OrmItemRepository : IItemRepository
	{
		static object locker = new object ();

		public OrmItemRepository(Context context)
		{

		}

		#region > Receiving Header <



		public List<tblPoList> GetRPoList(string po)
		{
			tblPoList itemlist = new tblPoList();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPoList>().ToList()
					.Where(t => t.po_num.Contains(po) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}

		/*
		public List<tblPoList> ChkRPoList(string po)
		{
			tblPoList itemlist = new tblPoList();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPoList>().ToList()
					.Where(t => t.po_num.Contains(po)).ToList ();
			}
		}
		*/


		public long UpdateRPoListSlot(tblPoList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public long DeleteRPoList(tblPoList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public tblPoList GetRPoListFirst(string receiver_num,string division)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPoList> ()
					.Where(t=>t.receiver_num==receiver_num && t.division_id==division)
					.SingleOrDefault ();
			}
		}

		public List<tblPoListDetail> GetRPoListDetail(string receiver_num,string division_id)
		{
			tblPoListDetail itemlist = new tblPoListDetail ();
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPoListDetail>().ToList()
					.Where (t=> t.receiver_num.Equals(receiver_num) && t.division_id.Equals(division_id)).ToList()
					.OrderBy(t=> t.upc).ToList()
					.OrderByDescending(t=> t.rqty).ToList()
					.OrderBy(t=> t.status).ToList();
			}
		}
			
		public tblPoListDetail GetRPoUPC(string receiver_num,string division_id,string upc)
		{
			upc = upc.Substring (0, Convert.ToInt32 (upc.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPoListDetail> ()
					.Where (t => t.receiver_num==receiver_num && t.division_id==division_id && t.upc==upc).SingleOrDefault ();
			}
		}

		public long UpdateRPoListDetail(tblPoListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}
			
		public tblPoListDetail[] ChkRPoListVariance ( string receiver_num,string division_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPoListDetail> ()
					.Where(t => t.receiver_num==receiver_num && t.division_id==division_id && t.oqty != t.rqty).ToArray ();
			}
		}


		public tblPoListDetail[] UpdateRPoListDetail ( string receiver_num,string division_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPoListDetail> ()
					.Where(t => t.receiver_num==receiver_num && t.division_id==division_id).ToArray ();
			}
		}

		public long DeleteUPC(tblPoListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		#endregion


		#region >>>>>PICKING<<<<<

		public List<tblPickingList> GetPTLList(string po)
		{
			tblPickingList itemlist = new tblPickingList();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingList>().ToList()
					.Where(t => (t.move_doc.Contains(po) || t.store_name.Contains(po)) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}

		#endregion

		#region >>>>>USER<<<<<

		public long UserLogin(tblUser item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}
			
		public tblUser GetUser()
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblUser> ().SingleOrDefault ();
			}
		}
		#endregion


		public tblConnectionURL GetConnectionURL()
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblConnectionURL> ().SingleOrDefault ();
			}
		}

		public long UpdateConnectionURL(tblConnectionURL item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

	}
}