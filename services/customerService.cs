using System.Collections;
using System.Text.Json;
using MySql.Data.MySqlClient;




public class customerService
{
    customerService customer;
    decryptService cm = new decryptService();

    dbServices ds = new dbServices();

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

    public async Task<responseData> customerAdd(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"INSERT INTO m_customer_tbl(cutm_name,contact_no,email_id,customer_type,establishment_year,gst_no,user_id,states,city,pin) 
                   VALUES (@cutm_name,@contact_no,@email_id,@customer_type,@establishment_year,@gst_no,@user_id,@states,@city,@pin);";
        MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@cutm_name", reqData.addInfo["cutmname"]),
                    new MySqlParameter("@contact_no", reqData.addInfo["contactno"]),
                    new MySqlParameter("@email_id", reqData.addInfo["emailid"]),
                    new MySqlParameter("@establishment_year", reqData.addInfo["establishmentyear"]),
                    new MySqlParameter("@gst_no", reqData.addInfo["gstno"]),
                    new MySqlParameter("@customer_type", reqData.addInfo["customertype"]),
                    new MySqlParameter("@user_id", reqData.addInfo["userid"]),
                    new MySqlParameter("@states", reqData.addInfo["states"]),
                    new MySqlParameter("@city", reqData.addInfo["city"]),
                    new MySqlParameter("@pin", reqData.addInfo["zip"])


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


    public async Task<responseData> customerUpdate(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"UPDATE m_customer_tbl SET cutm_name=@cutm_name,contact_no=@contact_no ,email_id=@email_id,
                    customer_type=@customer_type,establishment_year=@establishment_year,gst_no=@gst_no,state=@state,city=@city,pin=@pin WHERE cus_id=@id;";
        MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@id", reqData.addInfo["id"]),
                    new MySqlParameter("@cutm_name", reqData.addInfo["cutmname"]),
                    new MySqlParameter("@contact_no", reqData.addInfo["contactno"]),
                    new MySqlParameter("@email_id", reqData.addInfo["emailid"]),
                    new MySqlParameter("@location", reqData.addInfo["location"]),
                    new MySqlParameter("@customer_type", reqData.addInfo["customertype"]),
                    new MySqlParameter("@establishment_year", reqData.addInfo["establishmentyear"]),
                    new MySqlParameter("@gst_no", reqData.addInfo["gstno"]),
                    new MySqlParameter("@user_id", reqData.addInfo["userid"]),
                    new MySqlParameter("@state", reqData.addInfo["state"]),
                    new MySqlParameter("@city", reqData.addInfo["city"]),
                    new MySqlParameter("@pin", reqData.addInfo["zip"])
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


    public async Task<responseData> customerSelect(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"select cus_id, cutm_name, contact_no,email_id,customer_type,establishment_year,gst_no,pin, st.state_name ,city.city_name from m_customer_tbl cus inner join states st on st.id = cus.states inner join  cities city on  city.id = cus.city;";
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
                try
                {
                    resData.eventID = reqData.eventID;
                    var list = new ArrayList();
                    Dictionary<string, string> myDict =
                    new Dictionary<string, string>();
                    for (var i = 0; i < dbdata[0].Count(); i++)
                    {
                        myDict = new Dictionary<string, string>();
                        myDict.Add("cus_id", dbdata[0][i][0].ToString());
                        myDict.Add("cutm_name", dbdata[0][i][1].ToString());
                        myDict.Add("contact_no", dbdata[0][i][2].ToString());
                        myDict.Add("email_id", dbdata[0][i][3].ToString());
                        myDict.Add("customer_type", dbdata[0][i][4].ToString());
                        myDict.Add("establishment_year", dbdata[0][i][5].ToString());
                        myDict.Add("gst_no", dbdata[0][i][6].ToString());
                        myDict.Add("states", dbdata[0][i][7].ToString());
                        myDict.Add("city", dbdata[0][i][8].ToString());
                        myDict.Add("zip", dbdata[0][i][9].ToString());
                        list.Add(myDict);

                    }
                    resData.rData["rMessage"] = list;
                }
                catch (System.Exception ex)
                {

                    Console.Write(ex.Message);
                }
            else
            {
                resData.rData["rMessage"] = "Incorrect Input";
            }

        }

        return resData;
    }


    public async Task<responseData> customerSelectById(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"SELECT * FROM m_customer_tbl where cus_id=@id;";
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
                resData.rData["id"] = dbdata[0][0][0];
                resData.rData["customername"] = dbdata[0][0][1];
                resData.rData["contactno"] = dbdata[0][0][2];
                resData.rData["emailid"] = dbdata[0][0][3];
                resData.rData["customertype"] = dbdata[0][0][4];
                resData.rData["establishmentyear"] = dbdata[0][0][5]; //Convert.ToDateTime(dbdata[0][0][3]).ToString("dd/MM/yyyy")
                resData.rData["gstIN_no"] = dbdata[0][0][6];
                resData.rData["userid"] = dbdata[0][0][7];
                resData.rData["states"] = dbdata[0][0][8];
                resData.rData["city"] = dbdata[0][0][9];
                resData.rData["zip"] = dbdata[0][0][10];
            }
            else
            {
                //resData.rData["rMessage"] = "Incorrect Input";
                resData.rData["Error"] = errors.err[103];
            }

        }

        return resData;
    }


    public async Task<responseData> customerDelete(requestData reqData)
    {
        responseData resData = new responseData();

        var sq = @"DELETE FROM m_customer_tbl WHERE cus_id=@id;";

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