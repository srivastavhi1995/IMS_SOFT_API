using System.Collections;
using System.Text.Json;
using MySql.Data.MySqlClient;


public class itemService
{

    dbServices ds = new dbServices();

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


    public async Task<responseData> GetCategoriesAndSubcategories(requestData reqData)
    {
        responseData resData = new responseData();

        var categoryQuery = @"SELECT * FROM m_category where cate_status =1;";
        var subcategoryQuery = @"SELECT subcate.subcate_id, subcate.subcate_name, cate.cate_id, subcate.subcate_hsn, subcate.subcate_gst, cate.cate_name 
                            FROM m_subcategory subcate
                            INNER JOIN m_category cate ON cate.cate_id = subcate.cate_id where subcate_status =1 ;";
        MySqlParameter[] myParams = new MySqlParameter[] { };

        var categoryData = ds.executeSQL(categoryQuery, myParams);
        var subcategoryData = ds.executeSQL(subcategoryQuery, myParams);

        if (categoryData == null || subcategoryData == null)
        {
            resData.rStatus = 100;
            resData.rData["Error"] = "errors.err[100]";
        }
        else
        {
            if (categoryData[0].Count() > 0)
            {
                try
                {
                    resData.eventID = reqData.eventID;
                    var categoryList = new ArrayList();
                    var subcategoryList = new ArrayList();

                    for (var i = 0; i < categoryData[0].Count(); i++)
                    {
                        var categoryDict = new Dictionary<string, string>();
                        categoryDict.Add("cateId", categoryData[0][i][0].ToString());
                        categoryDict.Add("cateName", categoryData[0][i][1].ToString());
                        categoryList.Add(categoryDict);
                    }

                    for (var i = 0; i < subcategoryData[0].Count(); i++)
                    {
                        var subcategoryDict = new Dictionary<string, string>();
                        subcategoryDict.Add("subcateId", subcategoryData[0][i][0].ToString());
                        subcategoryDict.Add("subcateName", subcategoryData[0][i][1].ToString());
                        subcategoryDict.Add("cateId", subcategoryData[0][i][2].ToString());
                        subcategoryDict.Add("subcateHsn", subcategoryData[0][i][3].ToString());
                        subcategoryDict.Add("subcateGst", subcategoryData[0][i][4].ToString());
                        subcategoryList.Add(subcategoryDict);
                    }

                    var resultDict = new Dictionary<string, ArrayList>();
                    resultDict["categoryList"] = categoryList;
                    resultDict["subcategoryList"] = subcategoryList;

                    resData.rData["rMessage"] = resultDict;
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
    public async Task<responseData> itemBrand(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"INSERT INTO m_item_brand(brand_name) VALUES (@brand_name);";

        MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@brand_name", reqData.addInfo["brandName"]),
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

    public async Task<responseData> itemsAdd(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"insert into m_item_tbl (item_name,cate_id,subcate_id,item_hsn,item_price,item_gst) 
                    VALUES (@item_name,@cate_id,@subcate_id,@item_hsn,@item_price,@item_gst);";

        MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@item_name", reqData.addInfo["itemName"]),
                    new MySqlParameter("@cate_id", reqData.addInfo["cateId"]),
                    new MySqlParameter("@subcate_id", reqData.addInfo["subcateId"]),
                    
                    new MySqlParameter("@item_hsn", reqData.addInfo["itemHsn"]),
                    new MySqlParameter("@item_price", reqData.addInfo["itemPrice"]),
                    new MySqlParameter("@item_gst", reqData.addInfo["itemGst"])
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

    public async Task<responseData> itemsUpdate(requestData reqData)
    {
        responseData resData = new responseData();
        // UPDATE `sdc_ims`.`account_tbl` SET `ifsc_code` = 'dsgfdsgdfgsd' WHERE (`id` = '1');



        var sq = @"UPDATE m_item_tbl SET item_name=@itemname,cate_id=@cateid,subcate_id=@subcateid,item_model_no=@itemmodelno,item_purchase_date=@itempurchasedate,warrenty=@warrenty,
        item_hsn=@itemhsn,item_price=@itemprice,item_gst=@itemgst WHERE item_id=@itemid;";
        MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@itemid", reqData.addInfo["itemId"]),
                     new MySqlParameter("@itemname", reqData.addInfo["itemName"]),
                    new MySqlParameter("@cateid", reqData.addInfo["cateId"]),
                    new MySqlParameter("@subcateid", reqData.addInfo["subcateId"]),
                    new MySqlParameter("@itemmodelno", reqData.addInfo["itemModelNo"]),
                    new MySqlParameter("@itempurchasedate", reqData.addInfo["itemPurchaseDate"]),
                    new MySqlParameter("@warrenty", reqData.addInfo["warrenty"]),
                    new MySqlParameter("@itemhsn", reqData.addInfo["itemHsn"]),
                    new MySqlParameter("@itemprice", reqData.addInfo["itemPrice"]),
                    new MySqlParameter("@itemgst", reqData.addInfo["itemGst"])
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

    public async Task<responseData> itemsDelete(requestData reqData)
    {
        responseData resData = new responseData();

        var sq = @"UPDATE m_item_tbl SET item_status=0 WHERE item_id=@itemid;";
        MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@itemid", reqData.addInfo["itemId"]),

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

    //     select item_id,item_name,cate.cate_name,subcate.subcate_name,item_model_no,item_purchase_date,warrenty,item_hsn,item_price,item_gst from m_item item
    // inner join m_category cate on cate.cate_id= item.cate_id
    // inner join m_subcategory subcate on subcate.subcate_id=item.subcate_id

    public async Task<responseData> itemsSelect(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"select item_id,item_name,cate.cate_name,subcate.subcate_name,item_hsn,item_price,item_gst from m_item_tbl item
                    inner join m_category cate on cate.cate_id= item.cate_id
                    inner join m_subcategory subcate on subcate.subcate_id=item.subcate_id where item_status =1";
        MySqlParameter[] myParams = new MySqlParameter[] {
            };

        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = "errors.err[100]";
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
                        myDict.Add("itemId", dbdata[0][i][0].ToString());
                        myDict.Add("itemName", dbdata[0][i][1].ToString());
                        myDict.Add("cateName", dbdata[0][i][2].ToString());
                        myDict.Add("subcateName", dbdata[0][i][3].ToString());
                        myDict.Add("itemHsn", dbdata[0][i][4].ToString());
                        myDict.Add("itemPrice", dbdata[0][i][5].ToString());
                        myDict.Add("itemGst", dbdata[0][i][6].ToString());
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

    public async Task<responseData> itemsSelectById(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"select item_id,item_name,cate.cate_name,subcate.subcate_name,item_hsn,item_price,item_gst from m_item_tbl item
                    inner join m_category cate on cate.cate_id= item.cate_id
                    inner join m_subcategory subcate on subcate.subcate_id=item.subcate_id where item_status =1";
        MySqlParameter[] myParams = new MySqlParameter[] {
            new MySqlParameter("@itemid", reqData.addInfo["itemId"]),
            };

        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = "errors.err[100]";
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
                        myDict.Add("itemId", dbdata[0][i][0].ToString());
                        myDict.Add("itemName", dbdata[0][i][1].ToString());
                        myDict.Add("cateName", dbdata[0][i][2].ToString());
                        myDict.Add("subcateName", dbdata[0][i][3].ToString());
                        myDict.Add("itemHsn", dbdata[0][i][4].ToString());
                        myDict.Add("itemPrice", dbdata[0][i][5].ToString());
                        myDict.Add("itemGst", dbdata[0][i][6].ToString());
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