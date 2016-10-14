using System.Collections.Generic;
using Debenhams.Models;
using Android.Graphics;
using System;

namespace Debenhams.DataAccess
{
	public interface IItemRepository
	{
		#region     >>>>>Receiving<<<<<

		List<tblPoList> GetRPoList(string po);

		//List<tblPoList> ChkRPoList(string po);

		List <tblPoListDetail> GetRPoListDetail(string receiver_num,string division_id);
	
		tblPoListDetail GetRPoUPC (string receiver_num,string division_id,string upc);

		long DeleteRPoList(tblPoList item);
		long DeleteUPC(tblPoListDetail item);
		long UpdateRPoListSlot(tblPoList item);
		long UpdateRPoListDetail(tblPoListDetail item);

		tblPoListDetail[] ChkRPoListVariance(string receiver_num,string division_id);
		tblPoListDetail[] UpdateRPoListDetail(string receiver_num,string division_id);


		tblPoList GetRPoListFirst (string receiver_num,string division);

		#endregion



		#region     >>>>>PICKING<<<<<

		List<tblPickingList> GetPTLList(string po);

		#endregion

	#region     >>>>>User<<<<<

		long UserLogin(tblUser item);
		tblUser GetUser ();
	#endregion



		tblConnectionURL GetConnectionURL();
		long UpdateConnectionURL(tblConnectionURL item);


	}	
}