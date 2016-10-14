
using System;
using Debenhams.Models;
using SQLite;
using System.Collections.Generic;
using Android.Graphics;
using System.Linq;

namespace Debenhams.DataAccess
{
	public class DBConnection
	{

		public DBConnection ()
		{
		}

		#region >>>>>>>>>>>>>>> Stock Transfer List <<<<<<<<<<<<<<<

		public static List<tblStList> GetRStList(string mts)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblStList>().ToList()
					.Where(t => t.mts_no.Contains(mts) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}

		public static long UpdateRStList(tblStList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static List<tblStListDetail> GetRStListDetail(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblStListDetail>()
					.Where (t=> t.mts_no==mts_no)
					.OrderByDescending(t=> t.rqty)
					.OrderBy(t=> t.status).ToList();
			}
		}

		public static long UpdateRStListDetail(tblStListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}


		public static tblStListDetail GetRStUPC(string mts_no,string upc)
		{
			upc = upc.Substring (0, Convert.ToInt32 (upc.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblStListDetail> ()
					.Where (t => t.mts_no==mts_no && t.upc==upc).FirstOrDefault ();
			}
		}

		public static tblStListDetail GetRStUPCRqty(long id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblStListDetail> ()
					.Where (t => t.id==id)
					.FirstOrDefault ();
			}
		}

		public static long AddRStListDetail(string mts,string upc,string description,string oqty,string rqty,string status)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblStListDetail
						{
							mts_no = mts,
							upc=upc,
							description = description,
							oqty = oqty,
							rqty = rqty,
							status=status
						}
					);
			}
		}

		public static tblStListDetail[] ChkVarianceRStListDetail(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblStListDetail> ()
					.Where(t => t.mts_no == mts_no && t.oqty != t.rqty).ToArray ();
			}
		}
			
		public static tblStListDetail[] GetRStListDetailArray ( string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblStListDetail> ()
					.Where(t => t.mts_no==mts_no).ToArray ();
			}
		}

		public static long DeleteRStUPC(tblStListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static tblStList GetRStListFirst(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblStList> ()
					.Where(t=>t.mts_no==mts_no)
					.SingleOrDefault ();
			}
		}

		public static long DeleteRStList(tblStList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static tblPickingSTList ChkPSTListExist(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingSTList> ()
					.Where(t=>t.mts_no==mts_no)
					.SingleOrDefault ();
			}
		}

		public static long AddPSTList(string mts_no, string piler_id, string location_id, string location_name, string from_id,string from_name)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblPickingSTList
						{
							mts_no = mts_no,
							piler_id = piler_id,
							location_id = location_id,
							location_name = location_name,
							from_id = from_id,
							from_name = from_name,
							status = "Open"
						}
					);
			}
		}

		public static long AddPSTListDetail(string mts_no, string upc,string picking_id,string oqty,string rqty)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblPickingSTListDetail
						{
							mts_no = mts_no,
							upc = upc,
							picking_id = picking_id,
							oqty=oqty,
							rqty = rqty,
							status="0"
						}
					);
			}
		}
			
		public static tblStList ChkRSTListExist(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblStList> ()
					.Where(t=>t.mts_no==mts_no)
					.SingleOrDefault ();
			}
		}

		public static long AddRSTList(string mts_no, string piler_id, string location_id, string location_name)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblStList
						{
							mts_no = mts_no,
							piler_id = piler_id,
							location_id = location_id,
							location_name = location_name,
							status = "Open"
						}
					);
			}
		}

		public static long AddRSTListDetail(string mts_no, string upc,string description,string oqty,string rqty)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblStListDetail
						{
							mts_no = mts_no,
							upc = upc,
							description = description,
							oqty=oqty,
							rqty = rqty,
							status="0"
						}
					);
			}
		}
			
		public static void DeleteRSTListDetail(string mts)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblStListDetail>("delete from tblStListDetail where mts_no='"+ mts +"'");
			}
		}

		public static tblStListDetail GetRSTtUPC1(string mts_no,string upc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblStListDetail> ()
					.Where (t => t.mts_no==mts_no && t.upc==upc).SingleOrDefault ();
			}
		}

		public static long AddRSTListDetail(string mts,string upc,string description,string oqty,string rqty,string status)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblStListDetail
						{
							mts_no = mts,
							upc=upc,
							description = description,
							oqty = oqty,
							rqty = rqty,
							status=status
						}
					);
			}
		}

		public static long UpdateRSTtListDetail(tblStListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static tblRRLList GetRSTID(long id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblRRLList> ()
					.Where (t => t.id==id)
					.FirstOrDefault ();
			}
		}

		public static long UpdateRSTList(tblRRLList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		#endregion
		#region>>>>>>>>>>Stock Transfer Picking<<<<<<<<<<

		public static List<tblPickingSTList> GetPRStList(string mts)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingSTList>().ToList()
					.Where(t => t.mts_no.Contains(mts) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}

		public static List<tblPickingSTListDetail> GetPickingSTListDetail(string mts_no)
		{
			tblPickingListDetail itemlist = new tblPickingListDetail();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingSTListDetail> ()
					.Where (t => t.mts_no == mts_no)
					.OrderBy (t=> t.status).ToList();
			}
		}

		public static tblPickingSTListDetail GetPSTUPC(string mts_no,string upc)
		{
			upc = upc.Substring (0, Convert.ToInt32 (upc.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingSTListDetail> ()
					.Where (t => t.mts_no==mts_no && t.upc==upc).FirstOrDefault ();
			}
		}

		public static tblPickingSTListDetail GetPSTUPCNocut(string mts_no,string upc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingSTListDetail> ()
					.Where (t => t.mts_no==mts_no && t.upc==upc).SingleOrDefault ();
			}
		}

		public static long UpdatePSTListDetail(tblPickingSTListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}





		#endregion

		#region>>>>>>>>>>Stock Transfer Box<<<<<<<<<<



		public static tblSTBox[] GetPBoxCountAll ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblSTBox> ()
					.Where (t => t.mts_no==move_doc)
					.OrderByDescending(t => t.number)
					.ToArray() ;
			}
		}


		public static long DeleteBoxCode(tblSTBox item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}


		public static tblSTBox GetPBoxCount(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTBox> ()
					.Where (t => t.mts_no==move_doc)
					.OrderByDescending(t => t.number)
					.FirstOrDefault ();
			}
		}


		public static tblSTBoxDetail ChkBoxDetailMoveDoc(string move_doc,string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTBoxDetail> ()
					.Where (t => t.mts_no==move_doc && t.box_code==box_code)
					.FirstOrDefault ();
			}
		}
			
		public static List<tblSTBox> GetBoxList(string store_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblSTBox>()
					.Where(t=> t.location_id==store_id)
					.OrderByDescending(t=> t.id)
					.ToList ();
			}
		}

		public static tblSTBox GetPBoxCode(string boxcode)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTBox> ()
					.Where (t => t.box_code==boxcode)
					.SingleOrDefault ();
			}
		}

		public static long AddPBox(string box_code, string store_id, string move_doc,string number,string total)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblSTBox
						{
							box_code = box_code,
							location_id = store_id,
							mts_no = move_doc,
							number=number,
							total = total
						}
					);
			}
		}


		public static List<tblSTBox> GetSTBoxList(string location_id)
		{
			tblBox itemlist = new tblBox();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblSTBox>()
					.Where(t=> t.location_id==location_id)
					.OrderByDescending(t=> t.id)
					.ToList ();
			}
		}

		public static tblSTBox GetSTBoxCode(string boxcode)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTBox> ()
					.Where (t => t.box_code==boxcode)
					.SingleOrDefault ();
			}
		}

		public static tblSTBox GetSTBoxCount(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTBox> ()
					.Where (t => t.mts_no==mts_no)
					.OrderByDescending(t => t.number)
					.FirstOrDefault ();
			}
		}

		public static tblSTBoxDetail ChkSTBoxDetailUPC(string box_code,string mts_no,string upc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTBoxDetail> ()
					.Where (t => t.box_code==box_code && t.mts_no==mts_no && t.upc==upc)
					.SingleOrDefault ();
			}
		}

		public static long UpdateSTBoxDetail(tblSTBoxDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static long AddSTBoxDetail(string box_code,string upc_id, string mts_no,string upc,string rqty)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblSTBoxDetail
						{
							box_code = box_code,
							upc_id=upc_id,
							mts_no = mts_no,
							upc=upc,
							rqty = rqty
						}
					);
			}
		}


		public static tblPickingSTList GetPSTListFirst(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingSTList> ()
					.Where(t=>t.mts_no==move_doc )
					.SingleOrDefault ();
			}
		}

		public static long DeletePSTList(tblPickingSTList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static tblPickingSTListDetail[] ChkPSTListVariance ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingSTListDetail> ()
					.Where(t => t.mts_no==move_doc && t.oqty != t.rqty).ToArray ();
			}
		}

		public static tblPickingSTListDetail[] ApiPSTListDetail ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingSTListDetail> ()
					.Where(t => t.mts_no==move_doc).ToArray ();
			}
		}

		public static tblSTBoxDetail[] ApiAddSTBoxDetail (string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblSTBoxDetail> ()
					.Where(t => t.mts_no==move_doc && t.rqty != "0" ).ToArray ();
			}
		}


		public static tblSTBox GetPTLBoxCodes(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTBox> ()
					.Where (t => t.box_code==box_code)
					.FirstOrDefault ();
			}
		}

		public static tblSTBoxDetail[] ApiAddSTBox(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblSTBoxDetail> ()
					.Where(t => t.mts_no==move_doc)
					.ToArray ();
			}
		}

		public static tblSTBoxDetail ChkBoxupcnumber(string move_doc,string upc,string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTBoxDetail> ()
					.Where(t=>t.mts_no==move_doc && t.upc==upc && t.box_code==box_code)
					.FirstOrDefault ();
			}
		}


		#endregion


		#region >>>>>>>>>>>>>>> Stock Transfer Load <<<<<<<<<<<<<<<

		public static List<tblSTLoadList> GetLoadList(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblSTLoadList>().ToList()
					.Where(t => t.load_id.Contains(load_id) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}


		public static long UpdateLoadList(tblSTLoadList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static List<tblSTLoadListStore> GetLoadDetail(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblSTLoadListStore> ()
					.Where (t => t.load_id == load_id)
					.OrderBy (t => t.store_name).ToList()
					.OrderByDescending (t=> t.status).ToList();
			}
		}


		public static List<tblSTLoadListBox> GetLDetailBox(string move_doc,string store_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblSTLoadListBox> ()
					.Where (t => t.move_doc == move_doc && t.store_id == store_id)
					.ToList();
			}
		}

		public static long UpdateLDetailBox(tblSTLoadListBox item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static tblSTLoadListBox ChkLoadBox(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTLoadListBox> ()
					.Where (t => t.move_doc==move_doc && t.status=="Not Loaded")
					.FirstOrDefault ();
			}
		}
			

		public static tblLoadListDetail UpdateloaddetailOrig(string load_id,string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadListDetail> ()
					.Where (t => t.load_id==load_id && t.move_doc==move_doc)
					.FirstOrDefault ();
			}
		}

		public static tblLoadListDetail Updateloaddetail(string load_id,string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadListDetail> ()
					.Where (t => t.load_id==load_id && t.move_doc==move_doc)
					.FirstOrDefault ();
			}
		}

		public static tblSTLoadListStore UpdateSTloaddetail(string load_id,string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTLoadListStore> ()
					.Where (t => t.load_id==load_id && t.move_doc==move_doc)
					.FirstOrDefault ();
			}
		}

		public static long UpdateLoadListDetail(tblLoadListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}
		public static long UpdateSTLoadListDetail(tblSTLoadListStore item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}


		public static tblSTLoadListBox ChkLBoxExist(string move_doc,string box_code,string store_id)
		{
			box_code = box_code.Substring (0, Convert.ToInt32 (box_code.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTLoadListBox> ()
					.Where (t => t.move_doc==move_doc && t.box_code==box_code && t.store_id==store_id).SingleOrDefault ();
			}
		}

		public static tblSTLoadListStore[] ChkLoad (string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblSTLoadListStore> ()
					.Where (t => t.load_id==load_id && (t.status=="Open" || t.status=="In Process") )
					.ToArray ();
			}
		}


		#endregion

		#region >>>>>>>>>>>>>>> Reverse Logistic <<<<<<<<<<<<<<<


		public static List<tblRRLList> GetRRLList(string mts)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblRRLList>().ToList()
					.Where(t => t.mts_no.Contains(mts) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}

		public static long UpdateRRLList(tblRRLList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static long DeleteRRLList(tblRRLList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static tblRRLList GetRRLListFirst(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblRRLList> ()
					.Where(t=>t.mts_no==mts_no)
					.SingleOrDefault ();
			}
		}

		public static List<tblRRLListDetail> GetRRLListDetail(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblRRLListDetail>()
					.Where (t=> t.mts_no==mts_no)
					.OrderByDescending(t=> t.rqty)
					.OrderBy(t=> t.status).ToList();
			}
		}

		public static tblRRLListDetail GetRRLtUPC(string mts_no,string upc)
		{
			upc = upc.Substring (0, Convert.ToInt32 (upc.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblRRLListDetail> ()
					.Where (t => t.mts_no==mts_no && t.upc==upc).SingleOrDefault ();
			}
		}

		public static tblRRLListDetail GetRRLtUPC1(string mts_no,string upc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblRRLListDetail> ()
					.Where (t => t.mts_no==mts_no && t.upc==upc).SingleOrDefault ();
			}
		}

		public static long UpdateRRLtListDetail(tblRRLListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static tblRRLListDetail[] ChkVarianceRRLListDetail(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblRRLListDetail> ()
					.Where(t => t.mts_no == mts_no && t.oqty != t.rqty).ToArray ();
			}
		}

		public static long AddRRLListDetail(string mts,string upc,string description,string oqty,string rqty,string status)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblRRLListDetail
						{
							mts_no = mts,
							upc=upc,
							description = description,
							oqty = oqty,
							rqty = rqty,
							status=status
						}
					);
			}
		}

		public static tblRRLListDetail[] GetRRLListDetailArray ( string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblRRLListDetail> ()
					.Where(t => t.mts_no==mts_no).ToArray ();
			}
		}

		public static tblRRLListDetail GetRRLUPCRqty(long id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblRRLListDetail> ()
					.Where (t => t.id==id)
					.FirstOrDefault ();
			}
		}

		public static long DeleteRRLtUPC(tblRRLListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static long UpdateRRLListDetail(tblRRLListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static long AddRRLList(string mts_no, string piler_id, string location_id, string location_name)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblRRLList
						{
							mts_no = mts_no,
							piler_id = piler_id,
							location_id = location_id,
							location_name = location_name,
							status = "Open"
						}
					);
			}
		}

		public static long AddRRLListDetail(string mts_no, string upc,string description,string oqty,string rqty)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblRRLListDetail
						{
							mts_no = mts_no,
							upc = upc,
							description = description,
							oqty=oqty,
							rqty = rqty,
							status="0"
						}
					);
			}
		}

		public static tblRRLList ChkRRLListExist(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblRRLList> ()
					.Where(t=>t.mts_no==mts_no)
					.SingleOrDefault ();
			}
		}


		public static void DeleteRRLListDetail(string mts)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblRRLListDetail>("delete from tblRRLListDetail where mts_no='"+ mts +"'");
			}
		}

		#endregion

		#region>>>>>>>>>>Reverse Logistic Picking<<<<<<<<<<

		public static List<tblPickingRLList> GetPRLList(string mts)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingRLList>().ToList()
					.Where(t => t.mts_no.Contains(mts) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}

		public static List<tblPickingRLListDetail> GetPickingRLListDetail(string mts_no)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingRLListDetail> ()
					.Where (t => t.mts_no == mts_no)
					.OrderBy (t=> t.status).ToList();
			}
		}

		public static long UpdatePRLListDetail(tblPickingRLListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static tblPickingRLListDetail GetPRLUPC(string mts_no,string upc)
		{
			upc = upc.Substring (0, Convert.ToInt32 (upc.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingRLListDetail> ()
					.Where (t => t.mts_no==mts_no && t.upc==upc).SingleOrDefault ();
			}
		}
			
		public static tblPickingRLListDetail[] ChkPRLListVariance (string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingRLListDetail> ()
					.Where(t => t.mts_no==move_doc && t.oqty != t.rqty).ToArray ();
			}
		}

		public static tblPickingRLListDetail[] ApiPRLListDetail ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingRLListDetail> ()
					.Where(t => t.mts_no==move_doc).ToArray ();
			}
		}

		public static tblPickingRLList GetRLLListFirst(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingRLList> ()
					.Where(t=>t.mts_no==move_doc )
					.SingleOrDefault ();
			}
		}

		public static long DeletePRLList(tblPickingRLList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}
		#endregion

		#region>>>>>>>>>>Reverse Logistic Box<<<<<<<<<<

		public static List<tblRLBox> GetRLBoxList(string location_id)
		{
			tblBox itemlist = new tblBox();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblRLBox>()
					.Where(t=> t.location_id==location_id)
					.OrderByDescending(t=> t.id)
					.ToList ();
			}
		}

		public static tblRLBoxDetail ChkRLBoxDetailUPC(string box_code,string mts_no,string upc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblRLBoxDetail> ()
					.Where (t => t.box_code==box_code && t.mts_no==mts_no && t.upc==upc)
					.SingleOrDefault ();
			}
		}

		public static long UpdateRLBoxDetail(tblRLBoxDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static long AddRLBoxDetail(string box_code,string upc_id, string mts_no,string upc,string rqty)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblRLBoxDetail
						{
							box_code = box_code,
							upc_id=upc_id,
							mts_no = mts_no,
							upc=upc,
							rqty = rqty
						}
					);
			}
		}

		public static tblRLBoxDetail[] ApiAddRLBoxDetail (string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblRLBoxDetail> ()
					.Where(t => t.mts_no==move_doc && t.rqty != "0" ).ToArray ();
			}
		}

		public static tblRLBoxDetail[] ApiAddRLBox(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblRLBoxDetail> ()
					.Where(t => t.mts_no==move_doc)
					.ToArray ();
			}
		}

		public static tblRLBox GetPRLBoxCodes(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblRLBox> ()
					.Where (t => t.box_code==box_code)
					.FirstOrDefault ();
			}
		}



		public static long AddSTLList(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblSTLoadList
						{
							load_id = load_id,
							piler_id = GlobalVariables.GlobalUserid,
							status = "Open"
						}
					);
			}
		}

		public static tblSTLoadList ChkLbListExist(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTLoadList> ()
					.Where(t=>t.load_id==load_id)
					.SingleOrDefault ();
			}
		}


		public static long AddSTLoadListStore(string load_id, string move_doc,string store_id,string store_name)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblSTLoadListStore
						{
							load_id = load_id,
							move_doc = move_doc,
							store_id = store_id,
							store_name = store_name,
							status="Open"
						}
					);
			}
		}


		public static long AddSTLoadListDetailBox(string move_doc, string box_code,string store_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblSTLoadListBox
						{
							move_doc = move_doc,
							box_code=box_code,
							store_id = store_id,
							status="Not Loaded"
						}
					);
			}
		}


		public static tblSTLoadListBox[] ChkLBoxExistArray(string box_code,string store_id)
		{
			box_code = box_code.Substring (0, Convert.ToInt32 (box_code.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTLoadListBox> ()
					.Where (t => t.box_code==box_code && t.store_id==store_id).ToArray ();
			}
		}

		public static tblSTLoadListBox[] ChkLBoxExistArray1(string box_code,string store_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTLoadListBox> ()
					.Where (t => t.box_code==box_code && t.store_id==store_id).ToArray ();
			}
		}

		public static tblSTLoadListBox[] ChkloadList123 (string load_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Query<tblSTLoadListBox>("select tblSTLoadListBox.move_doc,tblSTLoadListBox.box_code,tblSTLoadListBox.status from tblSTLoadListBox " +
						"inner join tblSTLoadListStore on tblSTLoadListBox.move_doc=tblSTLoadListStore.move_doc " +
						"inner join tblSTLoadList on tblSTLoadListStore.load_id=tblSTLoadList.load_id where tblSTLoadList.load_id='"+ load_code +"'" ).ToArray ();
			}
		}

		public static tblSTLoadList GetLBListFirst(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblSTLoadList> ()
					.Where(t=>t.load_id==load_id )
					.SingleOrDefault ();
			}
		}


		public static void DeleteLoadListDetail(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblSTLoadListStore>("delete from tblSTLoadListStore where load_id='"+ load_id +"'");
			}
		}

		public static void DeleteLoadListDetailBox1(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblSTLoadListBox>("delete from tblSTLoadListBox where move_doc='"+ move_doc +"'");
			}
		}
		public static long DeleteLBList(tblSTLoadList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		#endregion

	}
}