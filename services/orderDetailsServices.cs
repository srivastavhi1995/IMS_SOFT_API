using System.Collections;
using System.Text.Json;
using MySql.Data.MySqlClient;




public class orderDetailsServices
{
    orderDetailsServices orderDetail;
    decryptService cm = new decryptService();

    dbServices ds = new dbServices();

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();



    public async Task<responseData> orderDetailAdd(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"INSERT INTO  order_detail_tbl (unit_price,size,quantity,total,date,item_id,ord_id,cate_id)values(@unit_price,@size,@quantity,@total,@date,@item_id,@ord_id,cate_id);";
        MySqlParameter[] myParams = new MySqlParameter[] {
             new MySqlParameter("@unit_price", reqData.addInfo["unitprice"]),
                    new MySqlParameter("@size", reqData.addInfo["size"]),
                    new MySqlParameter("@quantity", reqData.addInfo["quantity"]),
                    new MySqlParameter("@total", reqData.addInfo["total"]),
                    new MySqlParameter("@date", reqData.addInfo["date"]),
                    new MySqlParameter("@item_id", reqData.addInfo["item_id"]),
                    new MySqlParameter("@ord_id", reqData.addInfo["ord_id"]),
                    new MySqlParameter("@cate_id", reqData.addInfo["cate_id"])
            };
        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null)
        {
            resData.rStatus = 100;
            resData.rData["Error"] = errors.err[100];
        }
        else
        {
            resData.eventID = reqData.eventID;
            resData.rData["rMessage"] = "Data Inserted Successfully";
        }

        return resData;
    }


    public async Task<responseData> orderDetailUpdate(requestData reqData)
    {
        responseData resData = new responseData();
        //
        var sq = @"UPDATE order_detail_tbl SET unit_price=@unit_price,size=@size,quantity=@quantity,total=@total,date=@date,item_id=@item_id,ord_id=@ord_id,cate_id=@cate_id  WHERE ord_del_id=@id;";
        MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@id", reqData.addInfo["id"]),
                    new MySqlParameter("@unit_price", reqData.addInfo["unitprice"]),
                    new MySqlParameter("@size", reqData.addInfo["size"]),
                    new MySqlParameter("@quantity", reqData.addInfo["quantity"]),
                    new MySqlParameter("@total", reqData.addInfo["total"]),
                     new MySqlParameter("@date", reqData.addInfo["date"]),
                    new MySqlParameter("@item_id", reqData.addInfo["item_id"]),
                    new MySqlParameter("@ord_id", reqData.addInfo["ord_id"]),
                    new MySqlParameter("@cate_id", reqData.addInfo["cate_id"])
                };
        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = errors.err[100];
        }
        else
        {
            resData.eventID = reqData.eventID;
            resData.rData["rMessage"] = "Data UPDATE Successfully";
        }


        return resData;
    }


    public async Task<responseData> orderDetailSelect(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"SELECT * FROM order_detail_tbl;";
        MySqlParameter[] myParams = new MySqlParameter[] {
            };

        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = errors.err[100];
        }
        else
        {
            if (dbdata[0].Count() > 0)
            {
                try
                {
                    resData.eventID = reqData.eventID;
                    var list = new ArrayList();
                    Dictionary<string, string> myDict =
                    new Dictionary<string, string>();
                    for (var i = 0; i < dbdata[0].Count(); i++)
                    {
                        myDict = new Dictionary<string, string>();
                        myDict.Add("ord_del_id", dbdata[0][i][0].ToString());
                        myDict.Add("unit_price", dbdata[0][i][1].ToString());
                        myDict.Add("quantity", dbdata[0][i][2].ToString());
                        myDict.Add("total", dbdata[0][i][3].ToString());
                        myDict.Add("item_id", dbdata[0][i][4].ToString());
                        myDict.Add("ord_id", dbdata[0][i][5].ToString());
                        myDict.Add("cate_id", dbdata[0][i][6].ToString());
                        list.Add(myDict);

                    }
                    resData.rData["rMessage"] = list;
                }
                catch (System.Exception ex)
                {

                    Console.Write(ex.Message);
                }
            }
            else
            {
                resData.rData["rMessage"] = "Incorrect Input";
            }

        }

        return resData;
    }
    //categoryselebyid

    public async Task<responseData> orderDetailSelectById(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"SELECT * FROM order_detail_tbl where ord_del_id=@id;";
        MySqlParameter[] myParams = new MySqlParameter[] {
             new MySqlParameter("@id", reqData.addInfo["id"])
            };

        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = errors.err[100];
        }
        else
        {
            if (dbdata[0].Count() > 0)
            {
                resData.eventID = reqData.eventID;
                resData.rData["unit_price"] = dbdata[0][0][1];
                resData.rData["quantity"] = dbdata[0][0][2];
                resData.rData["total"] = dbdata[0][0][3];
                resData.rData["item_id"] = dbdata[0][0][4];
                resData.rData["ord_id"] = dbdata[0][0][5];
                resData.rData["cate_id"] = dbdata[0][0][6];
            }
            else
            {
                //resData.rData["rMessage"] = "Incorrect Input";
                resData.rData["Error"] = errors.err[103];
            }

        }

        return resData;
    }


    public async Task<responseData> orderDetailDelete(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"DELETE FROM order_detail_tbl WHERE ord_del_id=@id;";

        MySqlParameter[] myParams = new MySqlParameter[] {
            new MySqlParameter("@id", reqData.addInfo["id"])
            };
        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = errors.err[100];
        }
        else
        {

            resData.eventID = reqData.eventID;
            resData.rData["rMessage"] = "Data Delete Successfully";

        }

        return resData;
    }

}
