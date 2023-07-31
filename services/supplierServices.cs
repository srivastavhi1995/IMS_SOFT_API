using System.Collections;
using System.Text.Json;
using MySql.Data.MySqlClient;


public class supplierServices
{

    supplierServices supplier;
    decryptService cm = new decryptService();

    dbServices ds = new dbServices();

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


    public async Task<responseData> supplierAdd(requestData reqData)
    {
        responseData resData = new responseData();
        try
        {
            var sq = @"INSERT INTO m_sup_cust_dtl(name,orgnigation,contact,email_id,address,state_id,city_id,gstno,type,sup_cust_type,pin) 
       VALUES (@name,@orgnigation,@contact,@email_id,@address,@state_id,@city_id,@gstno,@type,@sup_cust_type,@pin);";
            MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@name", reqData.addInfo["name"]),
                    new MySqlParameter("@contact", reqData.addInfo["contact"]),
                    new MySqlParameter("@email_id", reqData.addInfo["emailId"]),
                    new MySqlParameter("@address", reqData.addInfo["address"]),
                    new MySqlParameter("@state_id", reqData.addInfo["stateId"]),
                    new MySqlParameter("@city_id", reqData.addInfo["cityId"]),
                    new MySqlParameter("@gstno", reqData.addInfo["gstNo"]),
                    new MySqlParameter("@orgnigation", reqData.addInfo["orgnigation"]),
                    new MySqlParameter("@type",reqData.addInfo["type"]),
                    new MySqlParameter("@sup_cust_type",reqData.addInfo["supCustType"]),
                    new MySqlParameter("@pin",reqData.addInfo["pin"])
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
        }
        catch (System.Exception ex)
        {

            resData.rData["rCode"] = 105;
            resData.rData["rData"] = errors.err[105] + Environment.NewLine + ex.Message;
        }

        return resData;
    }


    public async Task<responseData> supplierUpdate(requestData reqData)
    {
        responseData resData = new responseData();
        try
        {
            var sq = @"UPDATE m_sup_cust_dtl SET name=@name,orgnigation=@orgnigation,contact=@contact,
            email_id=@email_id,address=@address,state_id=@state_id,city_id=@city_id,gstno=@gstno,type =@type,sup_cust_type=@sup_cust_type WHERE id=@id;";
            MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@id", reqData.addInfo["id"]),
                    new MySqlParameter("@name", reqData.addInfo["name"]),
                    new MySqlParameter("@orgnigation", reqData.addInfo["orgnigation"]),
                    new MySqlParameter("@contact", reqData.addInfo["contact"]),
                    new MySqlParameter("@email_id", reqData.addInfo["emailId"]),
                    new MySqlParameter("@address", reqData.addInfo["address"]),
                    new MySqlParameter("@state_id", reqData.addInfo["stateId"]),
                    new MySqlParameter("@city_id", reqData.addInfo["cityId"]),
                    new MySqlParameter("@gstno", reqData.addInfo["gstNo"]),
                    new MySqlParameter("@type",reqData.addInfo["type"]),
                    new MySqlParameter("@sup_cust_type",reqData.addInfo["supCustType"])
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
        }
        catch (System.Exception ex)
        {
            resData.rData["rCode"] = 105;
            resData.rData["rData"] = errors.err[105] + Environment.NewLine + ex.Message;
        }

        return resData;
    }

    public async Task<responseData> GetSupplierCustomer(requestData reqData)
    {
        responseData resData = new responseData();
        var supplierQuery = @"SELECT supcust.id,supcust.name,orgnigation,contact,email_id,address,st.state_name,city.city_name,pin,gstno,supty.m_type,supcust.status FROM m_sup_cust_dtl supcust
        INNER JOIN states st ON st.id = supcust.state_id
        INNER JOIN cities city ON city.id = supcust.city_id
        INNER JOIN m_type ty ON ty.id = supcust.sup_cust_type
        INNER JOIN m_sup_type supty ON supty.m_id= supcust.type WHERE supcust.sup_cust_type = 101;";
        var CustomerQuery = @"SELECT supcust.id,supcust.name,orgnigation,contact,email_id,address,st.state_name,city.city_name,pin,gstno,supty.m_type FROM m_sup_cust_dtl supcust
                              INNER JOIN states st ON st.id = supcust.state_id
                              INNER JOIN cities city ON  city.id = supcust.city_id
                              INNER JOIN m_type ty ON  ty.id = supcust.sup_cust_type
                              INNER JOIN m_sup_type supty ON supty.m_id= supcust.type  WHERE supcust.status =0 and supcust.sup_cust_type = 102";
        MySqlParameter[] supplierParams = new MySqlParameter[] { };
        MySqlParameter[] CustomerParams = new MySqlParameter[] { };

        var supplierData = ds.executeSQL(supplierQuery, supplierParams);
        var CustomerData = ds.executeSQL(CustomerQuery, CustomerParams);

        if (supplierData == null || CustomerData == null)
        {
            resData.rStatus = 100; // database error, this error is caught at the app level
            resData.rData["Error"] = errors.err[100];
        }
        else
        {
            if (supplierData[0].Count() > 0 && CustomerData[0].Count() > 0)
            {
                resData.eventID = reqData.eventID;
                var supplierList = new ArrayList();
                var CustomerList = new ArrayList();

                for (var i = 0; i < supplierData[0].Count(); i++)
                {
                    Dictionary<string, string> stateDict = new Dictionary<string, string>();
                    stateDict.Add("id", supplierData[0][i][0].ToString());
                    stateDict.Add("name", supplierData[0][i][1].ToString());
                    stateDict.Add("orgnigation", supplierData[0][i][2].ToString());
                    stateDict.Add("contact", supplierData[0][i][3].ToString());
                    stateDict.Add("email_id", supplierData[0][i][4].ToString());
                    stateDict.Add("address", supplierData[0][i][5].ToString());
                    stateDict.Add("state_name", supplierData[0][i][6].ToString());
                    stateDict.Add("city_name", supplierData[0][i][7].ToString());
                    stateDict.Add("pin", supplierData[0][i][8].ToString());
                    stateDict.Add("gstno", supplierData[0][i][9].ToString());
                    stateDict.Add("m_type", supplierData[0][i][10].ToString());
                    stateDict.Add("status", supplierData[0][i][11].ToString());
                    supplierList.Add(stateDict);
                }

                for (var i = 0; i < CustomerData[0].Count(); i++)
                {
                    Dictionary<string, string> cityDict = new Dictionary<string, string>();
                    cityDict.Add("id", CustomerData[0][i][0].ToString());
                    cityDict.Add("name", CustomerData[0][i][1].ToString());
                    cityDict.Add("orgnigation", CustomerData[0][i][2].ToString());
                    cityDict.Add("contact", CustomerData[0][i][3].ToString());
                    cityDict.Add("email_id", CustomerData[0][i][4].ToString());
                    cityDict.Add("address", CustomerData[0][i][5].ToString());
                    cityDict.Add("state_name", CustomerData[0][i][6].ToString());
                    cityDict.Add("city_name", CustomerData[0][i][7].ToString());
                    cityDict.Add("pin", CustomerData[0][i][8].ToString());
                    cityDict.Add("gstno", CustomerData[0][i][9].ToString());
                    cityDict.Add("m_type", CustomerData[0][i][10].ToString());
                    CustomerList.Add(cityDict);
                }

                resData.rData["supplier"] = supplierList;
                resData.rData["Customer"] = CustomerList;
            }
            else
            {
                resData.rData["Error"] = errors.err[103];
            }
        }

        return resData;
    }

    public async Task<responseData> supplierSelect(requestData reqData)
    {
        responseData resData = new responseData();

        try
        {
            var sq = @"SELECT supcust.id,supcust.name,orgnigation,contact,email_id,address,st.state_name,city.city_name,pin,gstno,supty.m_type FROM m_sup_cust_dtl supcust
                       INNER JOIN states st ON st.id = supcust.state_id
                       INNER JOIN cities city ON  city.id = supcust.city_id
                       INNER JOIN m_type ty ON  ty.id = supcust.sup_cust_type
                       INNER JOIN m_sup_type supty ON supty.m_id= supcust.type;";
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
                            myDict.Add("id", dbdata[0][i][0].ToString());
                            myDict.Add("name", dbdata[0][i][1].ToString());
                            myDict.Add("orgnigation", dbdata[0][i][2].ToString());
                            myDict.Add("contact", dbdata[0][i][3].ToString());
                            myDict.Add("emailId", dbdata[0][i][4].ToString());
                            myDict.Add("address", dbdata[0][i][5].ToString());
                            myDict.Add("stateName", dbdata[0][i][6].ToString());
                            myDict.Add("cityName", dbdata[0][i][7].ToString());
                            myDict.Add("pin", dbdata[0][i][8].ToString());
                            myDict.Add("gstno", dbdata[0][i][9].ToString());
                            myDict.Add("m_type", dbdata[0][i][10].ToString());

                            list.Add(myDict);

                        }
                        resData.rData["rMessage"] = list;
                    }
                    catch (System.Exception ex)
                    {

                        Console.Write(ex.Message);
                    }

            }
        }
        catch (System.Exception ex)
        {
            resData.rData["rCode"] = 105;
            resData.rData["rData"] = errors.err[105] + Environment.NewLine + ex.Message;
        }


        return resData;
    }

    public async Task<responseData> supplierSelectById(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"SELECT supcust.id,supcust.name,orgnigation,contact,email_id,address,st.id,city.city_name,pin,gstno,supty.m_id FROM m_sup_cust_dtl supcust
                       INNER JOIN states st ON st.id = supcust.state_id
                       INNER JOIN cities city ON  city.id = supcust.city_id
                       INNER JOIN m_type ty ON  ty.id = supcust.sup_cust_type
                       INNER JOIN m_sup_type supty ON supty.m_id= supcust.type  WHERE supcust.status =0 and supcust.id = @id;";
        MySqlParameter[] myParams = new MySqlParameter[] {
              new MySqlParameter("@id", reqData.addInfo["id"]),
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
                resData.rData["name"] = dbdata[0][0][1];
                resData.rData["orgnigation"] = dbdata[0][0][2];
                resData.rData["contact"] = dbdata[0][0][3];
                resData.rData["emailId"] = dbdata[0][0][4];
                resData.rData["address"] = dbdata[0][0][5];
                resData.rData["stateId"] = dbdata[0][0][6];
                resData.rData["cityName"] = dbdata[0][0][7];
                resData.rData["pin"] = dbdata[0][0][8];
                resData.rData["gstNo"] = dbdata[0][0][9];
                resData.rData["mType"] = dbdata[0][0][10];

            }
            else
            {
                // resData.rData["rMessage"] = "Incorrect Input";
                resData.rData["Error"] = errors.err[103];
            }

        }
        return resData;
    }


    public async Task<responseData> supplierDelete(requestData reqData)
    {
        responseData resData = new responseData();

        var sq = @"UPDATE m_sup_cust_dtl SET status=@status where id=@id;";

        MySqlParameter[] myParams = new MySqlParameter[] {
            new MySqlParameter("@id", reqData.addInfo["id"]),
            new MySqlParameter("@status", reqData.addInfo["status"])
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

    public async Task<responseData> GetCitiesAndStates(requestData reqData)
    {
        responseData resData = new responseData();
        var statesQuery = @"select id, state_name from states;";
        var citiesQuery = @"select id, city_name ,state_id from cities;";

        MySqlParameter[] statesParams = new MySqlParameter[] { };
        MySqlParameter[] citiesParams = new MySqlParameter[] {

    };

        var statesData = ds.executeSQL(statesQuery, statesParams);
        var citiesData = ds.executeSQL(citiesQuery, citiesParams);

        if (statesData == null || citiesData == null)
        {
            resData.rStatus = 100; // database error, this error is caught at the app level
            resData.rData["Error"] = errors.err[100];
        }
        else
        {
            if (statesData[0].Count() > 0 && citiesData[0].Count() > 0)
            {
                resData.eventID = reqData.eventID;
                var statesList = new ArrayList();
                var citiesList = new ArrayList();

                for (var i = 0; i < statesData[0].Count(); i++)
                {
                    Dictionary<string, string> stateDict = new Dictionary<string, string>();
                    stateDict.Add("id", statesData[0][i][0].ToString());
                    stateDict.Add("states", statesData[0][i][1].ToString());
                    statesList.Add(stateDict);
                }

                for (var i = 0; i < citiesData[0].Count(); i++)
                {
                    Dictionary<string, string> cityDict = new Dictionary<string, string>();
                    cityDict.Add("id", citiesData[0][i][0].ToString());
                    cityDict.Add("cities", citiesData[0][i][1].ToString());
                    cityDict.Add("state_id", citiesData[0][i][2].ToString());
                    citiesList.Add(cityDict);
                }

                resData.rData["States"] = statesList;
                resData.rData["Cities"] = citiesList;
            }
            else
            {
                resData.rData["Error"] = errors.err[103];
            }
        }

        return resData;
    }



}

