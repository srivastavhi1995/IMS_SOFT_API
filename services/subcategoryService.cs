using System.Collections;
using System.Text.Json;
using MySql.Data.MySqlClient;


public class subcategoryService
{

    dbServices ds = new dbServices();

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


    public async Task<responseData> subcategoryAdd(requestData reqData)
    {

        responseData resData = new responseData();
        var sq = @"INSERT INTO m_subcategory (subcate_name,cate_id,subcate_hsn,subcate_gst) 
        VALUES (@subcate_name,@cate_id,@subcate_hsn,@subcate_gst);";

        MySqlParameter[] myParams = new MySqlParameter[] {
                         new MySqlParameter("@subcate_name", reqData.addInfo["subcateName"]),
                         new MySqlParameter("@cate_id", reqData.addInfo["cateId"]),
                         new MySqlParameter("@subcate_hsn", reqData.addInfo["subcateHsn"]),
                         new MySqlParameter("@subcate_gst", reqData.addInfo["subcateGst"])
          };
        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = "errors.err[100]";
        }
        else
        {
            resData.eventID = reqData.eventID;
            resData.rData["rMessage"] = "Data Inserted Successfully";
        }

        return resData;
    }

    public async Task<responseData> subcategoryUpdate(requestData reqData)
    {

        responseData resData = new responseData();
        var sq = @"update  m_subcategory set subcate_name=@subcate_name,cate_id=@cate_id,subcate_hsn=@subcate_hsn,subcate_gst=@subcate_gst where subcate_id=@subcate_id";

        MySqlParameter[] myParams = new MySqlParameter[] {
                         new MySqlParameter("@subcate_id", reqData.addInfo["subcateId"]),
                         new MySqlParameter("@subcate_name", reqData.addInfo["subcateName"]),
                         new MySqlParameter("@cate_id", reqData.addInfo["cateId"]),
                         new MySqlParameter("@subcate_hsn", reqData.addInfo["subcateHsn"]),
                         new MySqlParameter("@subcate_gst", reqData.addInfo["subcateGst"])
          };
        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = "errors.err[100]";
        }
        else
        {
            resData.eventID = reqData.eventID;
            resData.rData["rMessage"] = "Date Update Successfully";
        }

        return resData;
    }

    public async Task<responseData> subcategoryDelete(requestData reqData)
    {

        responseData resData = new responseData();
        var sq = @"update m_subcategory set subcate_status=0 where subcate_id=@subcate_id";

        MySqlParameter[] myParams = new MySqlParameter[] {
                         new MySqlParameter("@subcate_id", reqData.addInfo["subcateId"])
                        
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
            resData.rData["rMessage"] = "Date Delete Successfully";
        }

        return resData;
    }

    public async Task<responseData> subcategorySelct(requestData reqData)
    {

        responseData resData = new responseData();

        var sq = @"select subcate_id, subcate_name, cate.cate_id, subcate_hsn, subcate_gst, cate.cate_name from m_subcategory subcate
	               inner join m_category cate on cate.cate_id= subcate.cate_id where subcate.subcate_status =1;";
        MySqlParameter[] myParams = new MySqlParameter[] {
            };

        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = "errors.err[100];";
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
                        myDict.Add("subcateId", dbdata[0][i][0].ToString());
                        myDict.Add("subcateName", dbdata[0][i][1].ToString());
                        myDict.Add("cateId", dbdata[0][i][2].ToString());
                        myDict.Add("subcateHsn", dbdata[0][i][3].ToString());
                        myDict.Add("subcateGst", dbdata[0][i][4].ToString());
                        myDict.Add("cateName", dbdata[0][i][5].ToString());
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

    public async Task<responseData> subcategorySelctById(requestData reqData)
    {

        responseData resData = new responseData();

        var sq = @"select subcate_id, subcate_name, cate.cate_id, subcate_hsn, subcate_gst, cate.cate_name from m_subcategory subcate
	               inner join m_category cate on cate.cate_id= subcate.cate_id where subcate.subcate_id = @subcateid and subcate.subcate_status =1;";
        MySqlParameter[] myParams = new MySqlParameter[] {
            new MySqlParameter("@subcateid", reqData.addInfo["subcateId"])
            };

        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = "errors.err[100];";
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
                        myDict.Add("subcateId", dbdata[0][i][0].ToString());
                        myDict.Add("subcateName", dbdata[0][i][1].ToString());
                        myDict.Add("cateId", dbdata[0][i][2].ToString());
                        myDict.Add("subcateHsn", dbdata[0][i][3].ToString());
                        myDict.Add("subcateGst", dbdata[0][i][4].ToString());
                        myDict.Add("cateName", dbdata[0][i][5].ToString());
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