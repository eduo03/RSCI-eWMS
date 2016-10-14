using System;
using Debenhams.Models;
using SQLite;
using System.Collections.Generic;
using Android.Graphics;
using System.Linq;

namespace Debenhams.DataAccess
{
	public class ItemRepository
	{

		public ItemRepository ()
		{
		}

		#region >>>>>>>>>>POLIST<<<<<<<<<<

		public static tblLoadListDetailBox[] ChkloadList123 (string load_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Query<tblLoadListDetailBox>("select tblLoadListDetailBox.move_doc,tblLoadListDetailBox.box_code,tblLoadListDetailBox.status from tblLoadListDetailBox " +
						"inner join tblLoadListDetail on tblLoadListDetailBox.move_doc=tblLoadListDetail.move_doc " +
						"inner join tblLoadList on tblLoadListDetail.load_id=tblLoadList.load_id where tblLoadList.load_id='"+ load_code +"'" ).ToArray ();
			}
		}

		public static tblPoList ChkPoListExist(string receiver_num,string division)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPoList> ()
					.Where(t=>t.receiver_num==receiver_num && t.division_id==division)
					.SingleOrDefault ();
			}
		}

		public static long UserLogin(tblUser item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static long AddRPoList(string po_num, string receiver_num, string piler_id, string division_id, string division,string status)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
				(new tblPoList
					{
						po_num = po_num,
						receiver_num = receiver_num,
						piler_id = piler_id,
						division_id = division_id,
						division = division,
						slot_num = "",
						status = status
					}
				);
			}
		}

		public static long DeleteRPoList(tblPoList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static long DeleteRPoListDetail(tblPoListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static tblPoListDetail[] ChkRPoList ( string receiver_num,string division_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPoListDetail> ()
					.Where(t => t.receiver_num==receiver_num && t.division_id==division_id).ToArray ();
			}
		}


		public static long AddRPoListDetail(string receiver_num, string division_id,string sku,string upc,string description,string oqty,string rqty,string status)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
				(new tblPoListDetail
					{
						receiver_num = receiver_num,
						division_id = division_id,
						sku = sku,
						upc=upc,
						description = description,
						oqty = oqty,
						rqty = rqty,
						status=status
					}
				);
			}
		}


		public static long UpdateRPoListDetail(tblPoListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static long UpdateRPoList(tblPoList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static List<tblPoListDetail> GetRPoListDetail(string receiver_num,string division_id)
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

		public static tblPoListDetail GetRPoUPC(long id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPoListDetail> ()
					.Where (t => t.id==id)
					.FirstOrDefault ();
			}
		}


		public static tblPoList GetRPoID(long id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPoList> ()
					.Where (t => t.id==id)
					.FirstOrDefault ();
			}
		}

		public static void DeletePoListDetail(string receiver_no)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblPoListDetail>("delete from tblPoListDetail where receiver_num='"+ receiver_no +"'");
			}
		}

		#endregion

		#region >>>>>>>>>>PICKING TL LIST<<<<<<<<<<

		public static tblPickingList ChkPTLListExist(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingList> ()
					.Where(t=>t.move_doc==move_doc)
					.SingleOrDefault ();
			}
		}

		public static tblPickingList ChkPickingListExist(string receiver_num)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingList> ()
					.Where(t=>t.move_doc==receiver_num)
					.SingleOrDefault ();
			}
		}

		public static long DeleteRTLList(tblPickingList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static tblPickingListDetail[] ChkRTLListDetail ( string receiver_num)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingListDetail> ()
					.Where(t => t.move_doc==receiver_num).ToArray ();
			}
		}

		public static long DeleteRTLListDetail(tblPickingListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static List<tblPickingListDetail> GetPickingListDetail(string move_doc)
		{
			tblPickingListDetail itemlist = new tblPickingListDetail();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingListDetail> ()
					.Where (t => t.move_doc == move_doc)
					.OrderBy (t => t.dept).ToList()
					.OrderBy (t => t.style).ToList()
					.OrderBy (t=> t.status).ToList();
			}
		}

		public static tblPickingListDetail GetPTLUPC(string move_doc,string upc)
		{
			upc = upc.Substring (0, Convert.ToInt32 (upc.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingListDetail> ()
					.Where (t => t.move_doc==move_doc && t.upc==upc).SingleOrDefault ();
			}
		}

		public static tblPickingListDetail GetPTLUPC1(string move_doc,string upc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingListDetail> ()
					.Where (t => t.move_doc==move_doc && t.upc==upc).SingleOrDefault ();
			}
		}
		public static long UpdatePTLListDetail(tblPickingListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}


		public static tblPickingListDetail[] ChkRTLListVariance ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingListDetail> ()
					.Where(t => t.move_doc==move_doc && t.oqty != t.rqty).ToArray ();
			}
		}

		public static tblPickingListDetail[] ApiPTLListDetail ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingListDetail> ()
					.Where(t => t.move_doc==move_doc).ToArray ();
			}
		}


		public static tblPickingList GetPTLListFirst(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingList> ()
					.Where(t=>t.move_doc==move_doc )
					.SingleOrDefault ();
			}
		}

		public static long DeletePTLList(tblPickingList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}


		public static void DeleteTLListDetail(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblPickingListDetail>("delete from tblPickingListDetail where move_doc='"+ move_doc +"'");
			}
		}


		#endregion

		#region >>>>>>>>>>BOXLIST<<<<<<<<<<

		public static List<tblBox> GetBoxList(string store_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblBox>()
					.Where(t=> t.store_id==store_id)
					.OrderByDescending(t=> t.id)
					.ToList ();
			}
		}

		public static tblBox GetPBoxCount(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblBox> ()
					.Where (t => t.move_doc==move_doc)
					.OrderByDescending(t => t.number)
					.FirstOrDefault ();
			}
		}

		public static tblBox[] GetPBoxCountAll ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblBox> ()
					.Where (t => t.move_doc==move_doc)
					.OrderByDescending(t => t.number)
					.ToArray() ;
			}
		}

		public static tblBox GetPBoxCode(string boxcode)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblBox> ()
					.Where (t => t.box_code==boxcode)
					.SingleOrDefault ();
			}
		}

		public static long AddPBox(string box_code, string store_id, string move_doc,string number,string total)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblBox
						{
							box_code = box_code,
							store_id = store_id,
							move_doc = move_doc,
							number=number,
							total = total
						}
					);
			}
		}

		public static long DeleteBoxCode(tblBox item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}


		#region >>>>>>>>>>BOXLIST_DETAILS<<<<<<<<<<

		public static tblBoxDetail ChkBoxDetailUPC(string box_code,string move_doc,string upc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblBoxDetail> ()
					.Where (t => t.box_code==box_code && t.move_doc==move_doc && t.upc==upc)
					.SingleOrDefault ();
			}
		}

		public static tblBoxDetail ChkBoxDetailMoveDoc(string move_doc,string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblBoxDetail> ()
					.Where (t => t.move_doc==move_doc && t.box_code==box_code)
					.FirstOrDefault ();
			}
		}

		public static long AddBoxDetail(string box_code,string upc_id, string move_doc,string upc,string rqty)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblBoxDetail
						{
							box_code = box_code,
							upc_id=upc_id,
							move_doc = move_doc,
							upc=upc,
							rqty = rqty
						}
					);
			}
		}

		public static long UpdateBoxDetail(tblBoxDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static tblBoxDetail[] ApiAddBoxDetail (string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblBoxDetail> ()
					.Where(t => t.move_doc==move_doc && t.rqty != "0" ).ToArray ();
			}
		}

		public static tblBoxDetail[] ApiAddBox(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblBoxDetail> ()
					.Where(t => t.move_doc==move_doc)
 					.ToArray ();
			}
		}

		public static tblBox GetPTLBoxCodes(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblBox> ()
					.Where (t => t.box_code==box_code)
					.FirstOrDefault ();
			}
		}

		public static tblBoxDetail ChkBoxupcnumber(string move_doc,string upc,string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblBoxDetail> ()
					.Where(t=>t.move_doc==move_doc && t.upc==upc && t.box_code==box_code)
					.FirstOrDefault ();
			}
		}

		public static void DeleteBox(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblBox>("delete from tblBox where move_doc='"+ move_doc +"'");
			}
		}

		public static void DeleteBoxDetail(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblBoxDetail>("delete from tblBoxDetail where move_doc='"+ move_doc +"'");
			}
		}


		#endregion
		#endregion


		#region >>>>>>>>>>LOADLIST<<<<<<<<<<


		public static void DeleteLoadingList(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadingList>("delete from tblLoadingList where load_id='"+ load_id +"'");
			}
		}

		public static void DeleteLoadingSTList(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadingSTList>("delete from tblLoadingSTList where load_id='"+ load_id +"'");
			}
		}

		public static void DeleteLoadingListDetail(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadingListDetail>("delete from tblLoadingListDetail where load_id='"+ load_id +"'");
			}
		}

		public static void DeleteLoadingSTListDetail(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadingSTListDetail>("delete from tblLoadingSTListDetail where load_id='"+ load_id +"'");
			}
		}

		public static tblLoadingList ChkLoadingListExist(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadingList> ()
					.Where(t=>t.load_id==load_id)
					.SingleOrDefault ();
			}
		}

		public static long AddLoadingList(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadingList
						{
							load_id = load_id,
							piler_id = GlobalVariables.GlobalUserid,
							status = "Open"
						}
					);
			}
		}

		public static long AddLoadingSTList(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadingSTList
						{
							load_id = load_id,
							piler_id = GlobalVariables.GlobalUserid,
							status = "Open"
						}
					);
			}
		}

		public static long AddLoadingListDetail(string load_id, string box_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadingListDetail
						{
							load_id = load_id,
							box_code=box_code,
							status="Not Loaded"
						}
					);
			}
		}

		public static long AddLoadingSTListDetail(string load_id, string box_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadingSTListDetail
						{
							load_id = load_id,
							box_code=box_code,
							status="Not Loaded"
						}
					);
			}
		}


		public static List<tblLoadingList> GetLoadList1(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadingList>().ToList()
					.Where(t => t.load_id.Contains(load_id) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}
	
		public static List<tblLoadingSTList> GetLoadList1ST(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadingSTList>().ToList()
					.Where(t => t.load_id.Contains(load_id) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}

		public static List<tblLoadingSTListDetail> GetLoadDetail1ST(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadingSTListDetail> ()
					.Where (t => t.load_id == load_id)
					.OrderByDescending (t=> t.status).ToList();
			}
		}

		public static long UpdateLoadList1(tblLoadingList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static List<tblLoadingListDetail> GetLoadDetail1(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadingListDetail> ()
					.Where (t => t.load_id == load_id)
					.OrderByDescending (t=> t.status).ToList();
			}
		}
			
		public static List<tblLoadingSTListDetail> GetLoadSTDetail1(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadingSTListDetail> ()
					.Where (t => t.load_id == load_id)
					.OrderByDescending (t=> t.status).ToList();
			}
		}

		public static tblLoadingListDetail ChkLBoxExist1(string load_id,string box_code)
		{
			box_code = box_code.Substring (0, Convert.ToInt32 (box_code.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadingListDetail> ()
					.Where (t => t.load_id==load_id && t.box_code==box_code).SingleOrDefault ();
			}
		}

		public static tblLoadingSTListDetail ChkLBoxSTExist1(string load_id,string box_code)
		{
			box_code = box_code.Substring (0, Convert.ToInt32 (box_code.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadingSTListDetail> ()
					.Where (t => t.load_id==load_id && t.box_code==box_code).SingleOrDefault ();
			}
		}

		public static long UpdateLDetailBox1(tblLoadingListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static long UpdateLDetailBox1ST(tblLoadingSTListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}


		public static List<tblLoadList> GetLoadList(string load_id)
		{
			tblLoadList itemlist = new tblLoadList();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadList>().ToList()
					.Where(t => t.load_id.Contains(load_id) && t.piler_id.Equals(GlobalVariables.GlobalUserid)).ToList ();
			}
		}

		public static tblLoadList GetLBListFirst(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadList> ()
					.Where(t=>t.load_id==load_id )
					.SingleOrDefault ();
			}
		}

		public static tblLoadList ChkLoadListExist(string receiver_num)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadList> ()
					.Where(t=>t.load_id==receiver_num)
					.SingleOrDefault ();
			}
		}

		public static long DeleteLoadList(tblLoadList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static tblLoadListDetail[] ChkloadList ( string receiver_num)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadListDetail> ()
					.Where(t => t.load_id==receiver_num).ToArray ();
			}
		}

		public static long DeleteLoadListDetail(tblLoadListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static long DeleteLBList(tblLoadList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static long UpdateLoadList(tblLoadList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static List<tblLoadListDetail> GetLoadDetail(string load_id)
		{
			tblLoadListDetail itemlist = new tblLoadListDetail();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadListDetail> ()
					.Where (t => t.load_id == load_id)
					.OrderBy (t => t.store_name).ToList()
					.OrderByDescending (t=> t.status).ToList();
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

		/*
		public static tblLoadListDetail ChkLoad(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadListDetail> ()
					.Where (t => t.load_id==load_id && (t.status=="Open" || t.status=="In Process") )
					.FirstOrDefault ();
			}
		}
		*/
		public static tblLoadListDetail[] ChkLoad (string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadListDetail> ()
					.Where (t => t.load_id==load_id && (t.status=="Open" || t.status=="In Process") )
					.ToArray ();
			}
		}

		public static tblLoadingListDetail[] ChkLoading (string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadingListDetail> ()
					.Where (t => t.load_id==load_id && t.status=="Not Loaded" )
					.ToArray ();
			}
		}


		public static tblLoadingSTListDetail[] ChkLoadingST (string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadingSTListDetail> ()
					.Where (t => t.load_id==load_id && t.status=="Not Loaded" )
					.ToArray ();
			}
		}

		public static tblLoadingListDetail[] ChkLoadingbox (string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadingListDetail> ()
					.Where (t => t.load_id==load_id)
					.ToArray ();
			}
		}

		public static tblLoadingSTListDetail[] ChkLoadingboxST (string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadingSTListDetail> ()
					.Where (t => t.load_id==load_id)
					.ToArray ();
			}
		}

		public static List<tblLoadListDetailBox> GetLDetailBox(string move_doc,string store_id)
		{
			tblLoadListDetailBox itemlist = new tblLoadListDetailBox();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadListDetailBox> ()
					.Where (t => t.move_doc == move_doc && t.store_id == store_id)
					.ToList();
			}
		}

		public static tblLoadListDetailBox ChkLoadBox(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadListDetailBox> ()
					.Where (t => t.move_doc==move_doc && t.status=="Not Loaded")
					.FirstOrDefault ();
			}
		}


		public static tblLoadList ChkLbListExist(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadList> ()
					.Where(t=>t.load_id==load_id)
					.SingleOrDefault ();
			}
		}

		public static long AddLbtList(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadList
						{
							load_id = load_id,
							piler_id = GlobalVariables.GlobalUserid,
							status = "Open"
						}
					);
			}
		}

		public static long AddRLbListDetail(string load_id, string move_doc,string store_id,string store_name)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadListDetail
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

		public static long AddRLbListDetailBox(string move_doc, string box_code,string store_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadListDetailBox
						{
							move_doc = move_doc,
							box_code=box_code,
							store_id = store_id,
							status="Not Loaded"
						}
					);
			}
		}


		/*
		public static List<tblLoadListDetail> GetLScanBox(string load_id)
		{
			tblLoadListDetail itemlist = new tblLoadListDetail();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadListDetail> ()
					.Where (t => t.load_id == load_id)
					.ToList();
			}
		}
		*/
		public static long UpdateLDetailBox(tblLoadListDetailBox item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static tblLoadListDetailBox ChkLBoxExist(string move_doc,string box_code,string store_id)
		{
			box_code = box_code.Substring (0, Convert.ToInt32 (box_code.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadListDetailBox> ()
					.Where (t => t.move_doc==move_doc && t.box_code==box_code && t.store_id==store_id).SingleOrDefault ();
			}
		}

		public static tblLoadListDetailBox[] ChkLBoxExistArray(string box_code,string store_id)
		{
			box_code = box_code.Substring (0, Convert.ToInt32 (box_code.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadListDetailBox> ()
					.Where (t => t.box_code==box_code && t.store_id==store_id).ToArray ();
			}
		}

		public static tblLoadListDetailBox[] ChkLBoxExistArray1(string box_code,string store_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadListDetailBox> ()
					.Where (t => t.box_code==box_code && t.store_id==store_id).ToArray ();
			}
		}

		public static long AddLBoxScan(string load_id,string box_code)
		{
			box_code = box_code.Substring (0, Convert.ToInt32 (box_code.Length-1));
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadListDetail
						{
							load_id =load_id,
							move_doc = box_code
						}
					);
			}
		}

		public static long DeleteLBoxScan(tblLoadListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static void DeleteLoadListDetailBox(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadListDetailBox>("delete from tblLoadListDetailBox where move_doc='"+ move_doc +"'");
			}
		}


		public static void DeleteLoadListDetail(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadListDetail>("delete from tblLoadListDetail where load_id='"+ load_id +"'");
			}
		}
		public static void DeleteLoadListDetailBox1(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadListDetailBox>("delete from tblLoadListDetailBox where move_doc='"+ move_doc +"'");
			}
		}

		public static void DeleteNewLoadList(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadingList>("delete from tblLoadingList where load_id='"+ load_id +"'");
			}
		}

		public static void DeleteNewLoadSTList(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadingSTList>("delete from tblLoadingSTList where load_id='"+ load_id +"'");
			}
		}

		public static void DeleteNewLoadListDetail(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadingListDetail>("delete from tblLoadingListDetail where load_id='"+ load_id +"'");
			}
		}

		public static void DeleteNewLoadSTListDetail(string load_id)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				database.Query<tblLoadingSTListDetail>("delete from tblLoadingSTListDetail where load_id='"+ load_id +"'");
			}
		}

		#endregion

		#region >>>>>>>>>>Store List<<<<<<<<<<

		public static List<tblStoreList> GetStoreList(string keyword)
		{
			tblStoreList itemlist = new tblStoreList ();
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblStoreList>().ToList()
					.Where (t=> t.store_id.Contains(keyword) || t.store_name.Contains(keyword) || t.address.Contains(keyword)).ToList()
					.OrderBy(t=> t.store_name).ToList();
			}
		}


		public static tblUser UserLoginasd(string userid)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblUser> ()
					.Where (t => t.userid == userid).FirstOrDefault ();
			}
		}


		#endregion
	}
}

