using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System.Collections;

public class paymentService
{
    paymentService payment;
    decryptService cm = new decryptService();

    dbServices ds = new dbServices();

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    
    
    public async Task<responseData> paymentAdd(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"INSERT INTO payment_tbl (payment_type,details)values(@payment_type,@details);";
        MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@payment_type", reqData.addInfo["type"]),
                new MySqlParameter("@details", reqData.addInfo["details"])
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



    public async Task<responseData> paymentUpdate(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = "@UPDATE payment_tbl SET payment_type=@payment_type,details=@details WHERE bill_no_id=@id";
        MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter ("@id",reqData.addInfo["id"]),
                new MySqlParameter("@payment_type", reqData.addInfo["type"]),
                new MySqlParameter("@details", reqData.addInfo["details"])
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



     public async Task<responseData> paymentSelect(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"SELECT * FROM payment_tbl;";
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
                            myDict.Add("bill_no_id", dbdata[0][i][0].ToString());
                            myDict.Add("payment_type", dbdata[0][i][1].ToString());
                            myDict.Add("details", dbdata[0][i][2].ToString());
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


    public async Task<responseData> paymentSelectById(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"SELECT * FROM payment_tbl where bill_no_id=@id;";
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
                resData.rData["payment_type"] = dbdata[0][0][1];
                resData.rData["details"] = dbdata[0][0][2];
            }
            else
            {
                //resData.rData["rMessage"] = "Incorrect Input";
                resData.rData["Error"] = errors.err[103];
            }

        }

        return resData;
    }


    public async Task<responseData> paymentDelete(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"DELETE FROM payment_tbl WHERE bill_no_id=@id;";

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