using System.Collections;
using System.Text.Json;
using MySql.Data.MySqlClient;


public class categoryService
{

    dbServices ds = new dbServices();

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();



    public async Task<responseData> categoryAdd(requestData reqData)
    {

        responseData resData = new responseData();
        var sq = @"INSERT INTO m_category (cate_name) VALUES (@cateName);";
        MySqlParameter[] myParams = new MySqlParameter[] {
                         new MySqlParameter("@cateName", reqData.addInfo["cateName"])
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
            resData.rData["rMessage"] = "Data Inserted Successfully";
        }

        return resData;
    }


    public async Task<responseData> categoryUpdate(requestData reqData)
    {

        responseData resData = new responseData();
        //var sq = @"INSERT INTO m_category (cate_name) VALUES (@cateName);";
        var sq = @"UPDATE m_category SET cate_name=@cate_name WHERE cate_id=@cateid;";
        MySqlParameter[] myParams = new MySqlParameter[] {
                         new MySqlParameter("@cateid", reqData.addInfo["cateId"]),
                         new MySqlParameter("@cate_name", reqData.addInfo["cateName"])
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
            resData.rData["rMessage"] = "Data Update Successfully";
        }

        return resData;
    }
    public async Task<responseData> categoryDelete(requestData reqData)
    {
        responseData resData = new responseData();

        var sq = @"UPDATE m_category SET cate_status=0 WHERE cate_id=@cateid;";

        MySqlParameter[] myParams = new MySqlParameter[] {
            new MySqlParameter("@cateid", reqData.addInfo["cateId"])
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

    public async Task<responseData> categorySelect(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"SELECT * FROM m_category WHERE cate_status =1 ;";
        MySqlParameter[] myParams = new MySqlParameter[] { };

        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occurred
        {
            resData.rStatus = 100; // database error; this error is caught at the app level
            resData.rData["Error"] = errors.err[100];
        }
        else
        {
            if (dbdata[0].Count > 0)
            {
                try
                {
                    resData.eventID = reqData.eventID;
                    var list = new ArrayList();
                    Dictionary<string, string> myDict;

                    for (var i = 0; i < dbdata[0].Count; i++)
                    {
                      
                            myDict = new Dictionary<string, string>();
                            myDict.Add("cateId", dbdata[0][i][0].ToString());
                            myDict.Add("cateName", dbdata[0][i][1].ToString());
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


    public async Task<responseData> categorySelectByID(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"SELECT * FROM m_category where cate_status=1 and cate_id=@cateid;";
        MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@cateid", reqData.addInfo["cateId"])
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
                        myDict.Add("cateId", dbdata[0][i][0].ToString());
                        myDict.Add("cateName", dbdata[0][i][1].ToString());
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


}