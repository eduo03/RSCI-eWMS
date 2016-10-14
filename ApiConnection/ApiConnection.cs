using System;
using Android.Widget;
using Debenhams.Activities;
using System.Threading.Tasks;
using System.Json;
using System.Net;
using System.IO;
using System.Net.Http;
using Android.App;
using Debenhams.DataAccess;
using Debenhams.Models;
using SQLite;
using System.Collections.Generic;
using Android.Graphics;
using System.Linq;
using Debenhams;
namespace Debenhams.ApiConnection
{
	public class ApiConnection1
	{

		public ApiConnection1 ()
		{
		
		}

		public static async Task<bool> ApiPoRList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string po_num = Convert.ToString ((int)item ["purchase_order_no"]);
							string status="Open";
							string receiver_num = Convert.ToString ((int)item ["receiver_no"]);
							string division_id = Convert.ToString ((string)item ["division_id"]);
							string division = Convert.ToString ((string)item ["division"]);
							var po = ItemRepository.ChkPoListExist (receiver_num, division_id);
							if (po == null) 
							{
								try 
								{
									await ApiPoRListDetail (GlobalVariables.GlobalUrl + "/RPoListDetail/" + receiver_num + "/" + division_id);
									ItemRepository.AddRPoList (Convert.ToString ((int)item ["purchase_order_no"]),receiver_num,GlobalVariables.GlobalUserid,
										division_id,Convert.ToString ((string)item ["division"]),status);
								}
								catch(Exception ex)
								{
									tblPoList PoList = new tblPoList();
									tblPoListDetail PoListDetail = new tblPoListDetail();
									var podel = ItemRepository.ChkPoListExist (receiver_num, division_id);
									PoList.id = podel.id;
									ItemRepository.DeleteRPoList(PoList);
									var chkitem=ItemRepository.ChkRPoList (receiver_num, division_id);
									foreach (var i in chkitem) 
									{
										PoListDetail.id = i.id;
										ItemRepository.DeleteRPoListDetail (PoListDetail);
									}
									Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n PO :"+ po_num + " in Division :"+division + "\n"+ex.Message, ToastLength.Long).Show ();
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiPoRListDetail(string url)
		{
			bool result = false;
	
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string status="Open";
							string receiver_num = Convert.ToString ((int)item ["receiver_no"]);
							string division_id = Convert.ToString ((int)item ["division"]);
							if ((int)item ["po_status"] == 3)status = "In Process";else if((int)item ["po_status"] == 4) status = "Done";



							string a1=Convert.ToString ((int)item ["sku"]);
							string a2=Convert.ToString ((string)item ["upc"]);
							string a3=Convert.ToString ((int)item ["quantity_ordered"]);
							string a4=Convert.ToString ((int)item ["quantity_delivered"]);
							string a5=Convert.ToString ((string)item ["description"]);
							string a=Convert.ToString ((int)item ["receiver_no"]);
							ItemRepository.AddRPoListDetail (Convert.ToString ((int)item ["receiver_no"]),division_id,Convert.ToString ((int)item ["sku"]),
								(string)item ["upc"],(string)item ["description"],Convert.ToString((int)item ["quantity_ordered"]),
								Convert.ToString((int)item ["quantity_delivered"]),status
							);
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiDebsUpdateData(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
				
				}
			}
			return result;
		}

		#region >>>>>>>>>>Picking<<<<<<<<<<

		public static async Task<bool> ApiPTLList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							var movedoc = Convert.ToString ((int)item ["move_doc_number"]);
							var store_code = Convert.ToString ((int)item ["store_code"]);
							var store_name = Convert.ToString ((string)item ["store_name"]);
							var tl = ItemRepository.ChkPTLListExist (movedoc);
							if (tl == null) 
							{
								try
								{
									await ApiPTLListDetail (GlobalVariables.GlobalUrl + "/PTLListDetail/" + Convert.ToString ((int)item ["move_doc_number"]));
									using (var database = WMSDatabase.NewConnection ()) {
										database.Insert
										(new  tblPickingList {
											move_doc = movedoc,
											store_id =store_code,
											piler_id =GlobalVariables.GlobalUserid,
											store_name = store_name,
											status = "Open"
										}
										);
									}
								}
								catch(Exception ex)
								{
									tblPickingList PoList = new tblPickingList();
									tblPickingListDetail PoListDetail = new tblPickingListDetail();
									var podel = ItemRepository.ChkPickingListExist (movedoc);
									PoList.id = podel.id;
									ItemRepository.DeleteRTLList(PoList);
									var chkitem=ItemRepository.ChkRTLListDetail (movedoc);
									foreach (var i in chkitem) 
									{
										PoListDetail.id = i.id;
										ItemRepository.DeleteRTLListDetail (PoListDetail);
									}
									Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n TL :"+ movedoc + "\n"+ ex.Message, ToastLength.Long).Show ();
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiPTLListDetail(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							using (var database = WMSDatabase.NewConnection ())
							{
								database.Insert
								(new  tblPickingListDetail
									{
										move_doc = Convert.ToString((int)item["move_doc_number"]),
										picking_id= Convert.ToString((int)item["id"]),
										upc = item["upc"],
										sku = item["sku"],
										dept = item["dept"],
										style = item["short_description"],
										descr = item["short_description"],
										oqty = Convert.ToString((int)item["quantity_to_pick"]),
										rqty = Convert.ToString((int)item["moved_qty"]),
										status = "Open"
									}
								);
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> GetPTLBoxCode(string url)
		{
			tblBox tblbox = new tblBox ();
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							var box_code = item ["box_code"];
							var store_id = item ["store_id"];
							var move_doc = item ["move_doc"];
							var number = item ["number"];
							var total = item ["total"];
							var box = ItemRepository.GetPBoxCode (box_code);
							if (box == null) 
							{
								ItemRepository.AddPBox (box_code, store_id, move_doc, number, total);
							}
						}
					}
				}
			}
			return result;
		}


		public static async Task<bool> GetPSTTLBoxCode(string url)
		{
			tblBox tblbox = new tblBox ();
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							var box_code = item ["box_code"];
							var store_id = item ["store_id"];
							var move_doc = item ["move_doc"];
							var number = item ["number"];
							var total = item ["total"];
							var box = DBConnection.GetPBoxCode (box_code);
							if (box == null) 
							{
								DBConnection.AddPBox (box_code, store_id, move_doc, number, total);
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> PTLGetLastBoxCode(string url)
		{
			tblBox tblbox = new tblBox ();
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							try
							{
								string code = item ["box_code"];
								string total = item ["total"];
								code =code.Substring(Convert.ToInt32(code.Length)-5,5);
								GlobalVariables.box_code=(Convert.ToInt32 (code) + 1).ToString("00000");
								GlobalVariables.box_total = (Convert.ToInt32 (total) + 1).ToString();
							}
							catch
							{
								GlobalVariables.box_code="00001";
								GlobalVariables.box_total = "1";
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiPTLBoxValidate(string url)
		{
			tblBox tblbox = new tblBox ();
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string code = Convert.ToString((int)item["boxes_count"]);
							GlobalVariables.box_count=code;
						}
					}
				}
			}
			return result;
		}
		#endregion


		#region>>>>>>>>>>User<<<<<<<<<< 
		public static async Task<bool> UserLogin(string url)
		{
			tblUser tbluser = new tblUser ();
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							tbluser.id= 1;
							tbluser.userid=Convert.ToString((int)item["id"]);
							tbluser.username=(string)item ["username"];
							tbluser.password=(string)item ["password"];
							tbluser.fname=(string)item ["fname"];
							tbluser.lname=(string)item ["lname"];
							tbluser.token="";
							tbluser.status="1";
							GlobalVariables.GlobalUserid=tbluser.userid;
							ItemRepository.UserLogin (tbluser);
						}
					}
				}
			}
			return result;
		}
		#endregion

		#region>>>>>>>>>> Loading <<<<<<<<<<

		public static async Task<bool> ApiNewLoadingList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string load_code = (string)item ["load_code"];

							var load = ItemRepository.ChkLoadingListExist(load_code);
							if (load == null) 
							{

								try
								{
									await ApiLoadingListDetail (GlobalVariables.GlobalUrl + "/NewLoadingListDetail/" + load_code);
									ItemRepository.AddLoadingList (load_code);
								}
								catch(Exception ex)
								{
									ItemRepository.DeleteLoadingList (load_code);
									ItemRepository.DeleteLoadingListDetail (load_code);

									Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n Load :"+ load_code +"\n"+ex.Message, ToastLength.Long).Show ();

								}

							}
						}
					}
				}
			}
			return result;
		}


		public static async Task<bool> ApiLoadingListDetail(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string load_id=Convert.ToString ((string)item ["load_code"]);
							string box_code=Convert.ToString ((string)item ["box_number"]);
							ItemRepository.AddLoadingListDetail (load_id, box_code);
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiNewLoadingSTList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string load_code = (string)item ["load_code"];

							var load = ItemRepository.ChkLoadingListExist(load_code);
							if (load == null) 
							{

								try
								{
									await ApiLoadingSTListDetail (GlobalVariables.GlobalUrl + "/NewLoadingSTListDetail/" + load_code);
									ItemRepository.AddLoadingSTList (load_code);
								}
								catch(Exception ex)
								{
									ItemRepository.DeleteLoadingSTList (load_code);
									ItemRepository.DeleteLoadingSTListDetail (load_code);

									Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n Load :"+ load_code +"\n"+ex.Message, ToastLength.Long).Show ();

								}

							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiLoadingSTListDetail(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string load_id=Convert.ToString ((string)item ["load_code"]);
							string box_code=Convert.ToString ((string)item ["box_number"]);
							ItemRepository.AddLoadingSTListDetail (load_id, box_code);
						}
					}
				}
			}
			return result;
		}


		public static async Task<bool> ApiLBList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string load_code = (string)item ["load_code"];

							var load = ItemRepository.ChkLbListExist(load_code);
							if (load == null) 
							{
								try
								{
									await ApiLBListDetail (GlobalVariables.GlobalUrl + "/LBListDetail/" + load_code);
									ItemRepository.AddLbtList (load_code);
								}
								catch(Exception ex)
								{
									tblLoadList PoList = new tblLoadList();
									tblLoadListDetail PoListDetail = new tblLoadListDetail();
									tblLoadListDetailBox PoListDetailBox = new tblLoadListDetailBox();
									var podel = ItemRepository.ChkLoadListExist (load_code);
									PoList.id = podel.id;
									ItemRepository.DeleteLoadList(PoList);
									var chkitem=ItemRepository.ChkloadList (load_code);
									foreach (var i in chkitem) 
									{
										PoListDetail.id = i.id;
										ItemRepository.DeleteLoadListDetailBox (i.move_doc);
										ItemRepository.DeleteLoadListDetail (PoListDetail);
									}
									Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n Load :"+ load_code +"\n"+ex.Message, ToastLength.Long).Show ();
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiLBListDetail(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string load_id=Convert.ToString ((string)item ["load_code"]);
							string move_doc=Convert.ToString ((string)item ["move_doc_number"]);
							string store_id=Convert.ToString ((string)item ["store_code"]);
							string store_name=Convert.ToString ((string)item ["store_name"]);

							//try
							//{
								await ApiLBListDetailBox (GlobalVariables.GlobalUrl + "/LBListDetailBox/" + move_doc);
								ItemRepository.AddRLbListDetail (load_id, move_doc, store_id, store_name);
							//}
							//catch(Exception ex)
							//{
							//	load_id=Convert.ToString ((int)item ["load_code"]);
							//}

						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiLBListDetailBox(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string move_doc=Convert.ToString ((string)item ["move_doc_number"]);
							string box_code=Convert.ToString ((string)item ["box_code"]);
							string store_id=Convert.ToString ((string)item ["store_code"]);
							ItemRepository.AddRLbListDetailBox (move_doc, box_code, store_id);
						}
					}
				}
			}
			return result;
		}
		#endregion


		#region>>>>>>>>>> STOCK TRANSFER <<<<<<<<<<


		public static async Task<bool> ApiRStList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string mts_no = Convert.ToString ((string)item ["mts_no"]);
							string location_id = Convert.ToString ((string)item ["location_id"]);
							string location_name = Convert.ToString ((string)item ["location_name"]);

							var po = DBConnection.ChkRSTListExist (mts_no);
							if (po == null) 
							{
								DBConnection.AddRSTList (mts_no,GlobalVariables.GlobalUserid,
									location_id,location_name);

								//try 
								//{
								await AddRSTListDetail (GlobalVariables.GlobalUrl + "/RSTListDetail/" + mts_no);
									/*}
								catch(Exception ex)
								{

									tblPoList PoList = new tblPoList();
									tblPoListDetail PoListDetail = new tblPoListDetail();
									var podel = ItemRepository.ChkPoListExist (receiver_num, division_id);
									PoList.id = podel.id;
									ItemRepository.DeleteRPoList(PoList);
									var chkitem=ItemRepository.ChkRPoList (receiver_num, division_id);
									foreach (var i in chkitem) 
									{
										PoListDetail.id = i.id;
										ItemRepository.DeleteRPoListDetail (PoListDetail);
									}
									Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n PO :"+ po_num + " in Division :"+division + "\n"+ex.Message, ToastLength.Long).Show ();
								}
								*/
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> AddRSTListDetail(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string mts_no=Convert.ToString ((string)item ["mts_no"]);
							string upc=Convert.ToString ((string)item ["upc"]);
							string description=Convert.ToString ((string)item ["description"]);
							string rqty=Convert.ToString ((int)item ["rqty"]);
							string oqty=Convert.ToString ((int)item ["oqty"]);
							DBConnection.AddRSTListDetail (mts_no,upc,description,oqty,rqty);
						}
					}
				}
			}
			return result;
		}



		public static async Task<bool> ApiPSTList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string mts_no = Convert.ToString ((string)item ["mts_no"]);
							string location_id = Convert.ToString ((int)item ["location_id"]);
							string location_name = Convert.ToString ((string)item ["location_name"]);
							string from_id = Convert.ToString ((string)item ["from_id"]);
							string from_name = Convert.ToString ((string)item ["from_name"]);

							var po = DBConnection.ChkPSTListExist (mts_no);
							if (po == null) 
							{
								/*
								try 
								{
								*/
								await ApiPSTListDetail (GlobalVariables.GlobalUrl + "/PSTListDetail/" + mts_no);
								DBConnection.AddPSTList (mts_no,GlobalVariables.GlobalUserid,
									location_id,location_name,from_id,from_name);
								/*}
								catch(Exception ex)
								{
									tblPoList PoList = new tblPoList();
									tblPoListDetail PoListDetail = new tblPoListDetail();
									var podel = ItemRepository.ChkPoListExist (receiver_num, division_id);
									PoList.id = podel.id;
									ItemRepository.DeleteRPoList(PoList);
									var chkitem=ItemRepository.ChkRPoList (receiver_num, division_id);
									foreach (var i in chkitem) 
									{
										PoListDetail.id = i.id;
										ItemRepository.DeleteRPoListDetail (PoListDetail);
									}
									Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n PO :"+ po_num + " in Division :"+division + "\n"+ex.Message, ToastLength.Long).Show ();
								}
								*/
							}
						}
					}
				}
			}
			return result;
		}


		public static async Task<bool> ApiPSTListDetail(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string mts_no=Convert.ToString ((string)item ["mts_no"]);
							string upc=Convert.ToString ((string)item ["upc"]);
							string picking_id=Convert.ToString ((int)item ["picking_id"]);
							string rqty=Convert.ToString ((int)item ["rqty"]);
							string oqty=Convert.ToString ((int)item ["oqty"]);
							DBConnection.AddPSTListDetail (mts_no,upc,picking_id,oqty,rqty);
						}
					}
				}
			}
			return result;
		}


		#endregion


		#region>>>>>>>>>> REVERSE LOGISTIC <<<<<<<<<<

		public static async Task<bool> ApiRRLList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string mts_no = Convert.ToString ((string)item ["mts_no"]);
							string location_id = Convert.ToString ((string)item ["location_id"]);
							string location_name = Convert.ToString ((string)item ["location_name"]);

							var po = DBConnection.ChkRRLListExist (mts_no);
							if (po == null) 
							{

								//try 
								//{
								await ApiRRLListDetail (GlobalVariables.GlobalUrl + "/RRLListDetail/" + mts_no);
								DBConnection.AddRRLList (mts_no,GlobalVariables.GlobalUserid,
									location_id,location_name);
									/*}
								catch(Exception ex)
								{
									tblPoList PoList = new tblPoList();
									tblPoListDetail PoListDetail = new tblPoListDetail();
									var podel = ItemRepository.ChkPoListExist (receiver_num, division_id);
									PoList.id = podel.id;
									ItemRepository.DeleteRPoList(PoList);
									var chkitem=ItemRepository.ChkRPoList (receiver_num, division_id);
									foreach (var i in chkitem) 
									{
										PoListDetail.id = i.id;
										ItemRepository.DeleteRPoListDetail (PoListDetail);
									}
									Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n PO :"+ po_num + " in Division :"+division + "\n"+ex.Message, ToastLength.Long).Show ();
								}
								*/
							}
						}
					}
				}
			}
			return result;
		}


		public static async Task<bool> ApiRRLListDetail(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string mts_no=Convert.ToString ((string)item ["mts_no"]);
							string upc=Convert.ToString ((string)item ["upc"]);
							string description=Convert.ToString ((string)item ["description"]);
							string rqty=Convert.ToString ((int)item ["rqty"]);
							string oqty=Convert.ToString ((int)item ["oqty"]);
							DBConnection.AddRRLListDetail (mts_no,upc,description,oqty,rqty);
						}
					}
				}
			}
			return result;
		}

		#endregion

		#region>>>>>>>>>> Loading <<<<<<<<<<

		public static async Task<bool> STLBList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string load_code = (string)item ["load_code"];

							var load = DBConnection.ChkLbListExist(load_code);
							if (load == null) 
							{
								try
								{
									await ApiSTLBListDetail (GlobalVariables.GlobalUrl + "/STLBListDetail/" + load_code);
									DBConnection.AddSTLList (load_code);
								}
								catch(Exception ex)
								{
									/*
									tblLoadList PoList = new tblLoadList();
									tblLoadListDetail PoListDetail = new tblLoadListDetail();
									tblLoadListDetailBox PoListDetailBox = new tblLoadListDetailBox();
									var podel = ItemRepository.ChkLoadListExist (load_code);
									PoList.id = podel.id;
									ItemRepository.DeleteLoadList(PoList);
									var chkitem=ItemRepository.ChkloadList (load_code);
									foreach (var i in chkitem) 
									{
										PoListDetail.id = i.id;
										ItemRepository.DeleteLoadListDetailBox (i.move_doc);
										ItemRepository.DeleteLoadListDetail (PoListDetail);
									}
									*/
									Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n Load :"+ load_code +"\n"+ex.Message, ToastLength.Long).Show ();

								}
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiSTLBListDetail(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string load_id=Convert.ToString ((string)item ["load_code"]);
							string move_doc=Convert.ToString ((string)item ["move_doc_number"]);
							string store_id=Convert.ToString ((int)item ["from_store_code"]);
							string store_name=Convert.ToString ((string)item ["store_name"]);

							//try
							//{
							await ApiSTLBListDetailBox (GlobalVariables.GlobalUrl + "/STLBListDetailBox/" + move_doc);
							DBConnection.AddSTLoadListStore (load_id, move_doc, store_id, store_name);
							//}
							//catch(Exception ex)
							//{
							//	load_id=Convert.ToString ((int)item ["load_code"]);
							//}

						}
					}
				}
			}
			return result;
		}
	
		public static async Task<bool> ApiSTLBListDetailBox(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string move_doc=Convert.ToString ((string)item ["tl_number"]);
							string box_code=Convert.ToString ((string)item ["box_code"]);
							string store_id=Convert.ToString ((string)item ["store_code"]);
							DBConnection.AddSTLoadListDetailBox (move_doc, box_code, store_id);
						}
					}
				}
			}
			return result;
		}

		#endregion
	}
}

