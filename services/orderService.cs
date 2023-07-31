using System.Collections;
using System.Text.Json;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

public class orderService
{
    orderService order;
    decryptService cm = new decryptService();

    dbServices ds = new dbServices();

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


    public async Task<responseData> callcategorySubcategory(requestData reqData)
    {
        responseData resData = new responseData();

        var orgnigationQuery = @"SELECT id,orgnigation FROM m_sup_cust_dtl WHERE TYPE=1 AND STATUS =0 AND sup_cust_type = '101';";
        var brandQuery = @"SELECT * FROM m_item_brand;";
        var categoryQuery = @"SELECT * FROM m_category where cate_status =1;";
        var subcategoryQuery = @"SELECT subcate.subcate_id, subcate.subcate_name, cate.cate_id, subcate.subcate_hsn, subcate.subcate_gst, cate.cate_name 
                                FROM m_subcategory subcate
                                INNER JOIN m_category cate ON cate.cate_id = subcate.cate_id where subcate_status =1 ;";
        var itemQuery = @"select item_id, item_name, cate.cate_id, subcate.subcate_id, cate.cate_name,subcate.subcate_name,item_price FROM m_item_tbl item
                          inner join m_category cate ON cate.cate_id = item.cate_id 
                          inner join m_subcategory subcate on subcate.subcate_id= item.subcate_id ;";
        MySqlParameter[] myParams = new MySqlParameter[] {

         };
        // MySqlParameter[] itemMyParams = new MySqlParameter[] {

        //          new MySqlParameter("@subcate_id", reqData.addInfo["subcateId"]),
        //  };

        var orgnigationData = ds.executeSQL(orgnigationQuery, myParams);
        var brandData = ds.executeSQL(brandQuery, myParams);
        var categoryData = ds.executeSQL(categoryQuery, myParams);
        var subcategoryData = ds.executeSQL(subcategoryQuery, myParams);
        var itemData = ds.executeSQL(itemQuery, myParams);

        if (categoryData == null || subcategoryData == null || itemData == null)
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
                    var orgnigationList = new ArrayList();
                    var brandList = new ArrayList();
                    var categoryList = new ArrayList();
                    var subcategoryList = new ArrayList();
                    var itemList = new ArrayList();

                    for (var i = 0; i < orgnigationData[0].Count(); i++)
                    {
                        var orgnigationDict = new Dictionary<string, string>();
                        orgnigationDict.Add("Id", orgnigationData[0][i][0].ToString());
                        orgnigationDict.Add("orgnigationName", orgnigationData[0][i][1].ToString());
                        orgnigationList.Add(orgnigationDict);
                    }
                    for (var i = 0; i < brandData[0].Count(); i++)
                    {
                        var brandDict = new Dictionary<string, string>();
                        brandDict.Add("Id", brandData[0][i][0].ToString());
                        brandDict.Add("brandName", brandData[0][i][1].ToString());
                        brandList.Add(brandDict);
                    }
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
                    for (var i = 0; i < itemData[0].Count(); i++)
                    {
                        var itemDict = new Dictionary<string, string>();
                        itemDict.Add("item_id", itemData[0][i][0].ToString());
                        itemDict.Add("item_name", itemData[0][i][1].ToString());
                        itemDict.Add("subcate_id", itemData[0][i][3].ToString());
                        itemDict.Add("subcate_name", itemData[0][i][5].ToString());
                        itemDict.Add("item_price", itemData[0][i][6].ToString());
                        itemList.Add(itemDict);
                    }

                    var resultDict = new Dictionary<string, ArrayList>();
                    resultDict["orgnigationList"] = orgnigationList;
                    resultDict["brandList"] = brandList;
                    resultDict["categoryList"] = categoryList;
                    resultDict["subcategoryList"] = subcategoryList;
                    resultDict["itemList"] = itemList;

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

    public async Task<responseData> orderAdd(requestData reqData)
    {
        responseData resData = new responseData();
        var sq = @"INSERT INTO  order_tbl (order_date,sup_id)values(@order_date,@sup_id);";
        MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@order_date", reqData.addInfo["date"]),
                new MySqlParameter("@sup_id", reqData.addInfo["sup_id"])
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
    public async Task<responseData> AddTransaction(requestData reqData)
    {
        responseData resData = new responseData();
        // Insert e_inv_trans data
        var orderNosq = @"SELECT REPLACE(e_inv_trans.t_vou_no, concat('PO','~'), '')+1 t_vou_no FROM sdc_ims.e_inv_trans WHERE t_vou_no LIKE CONCAT('PO~', '%') ORDER BY e_inv_tran_id DESC LIMIT 1;";
        MySqlParameter[] orderNoParams = new MySqlParameter[]
        {};
        var orderNodbdata = ds.executeSQL(orderNosq, orderNoParams);
        var invTransSq = @"INSERT INTO e_inv_trans (mnu_id, sup_cust_id, t_vou_no, t_date, t_type) VALUES (101, @sup_cust_id, @t_vou_no, @t_date, @t_type);";
        MySqlParameter[] invTransParams = new MySqlParameter[] {
    new MySqlParameter("@sup_cust_id", reqData.addInfo["supCustId"]),
    new MySqlParameter("@t_vou_no", "PO~"+ orderNodbdata[0][0][0]),
    new MySqlParameter("@t_date", reqData.addInfo["orderdate"]), // order trans date
 new MySqlParameter("@t_type", reqData.addInfo.ContainsKey("transtype") ? reqData.addInfo["transtype"] : null)
  };
        var invTransDbData = ds.executeSQL(invTransSq, invTransParams);
        string orderDetailsString = reqData.addInfo["orderdetails"].ToString();
        List<dynamic> orderDetails = JsonConvert.DeserializeObject<List<dynamic>>(orderDetailsString);

        var selectSq = @"SELECT LAST_INSERT_ID() AS e_inv_trans;";
        MySqlParameter[] selectParams = new MySqlParameter[] { };
        var selectDbData = ds.executeSQL(selectSq, selectParams);

        for (int i = 0; i < orderDetails.Count; i++)
        {
            dynamic detail = orderDetails[i];
            // Insert e_inv_trans_dets data
            var invTransDetsSq = @"INSERT INTO e_inv_trans_dets(trans_id,cate_id,sub_cat_id,item_id,brand_id,qty,price_per_unit,gst_per,gst_amount,price_with_gst,price_without_gst,total_gst) VALUES (@trans_id,@cate_id,@sub_cat_id,@item_id,@brand_id,@qty,@price_per_unit,@gst_per,@gst_amount,@price_with_gst,@price_without_gst,@total_gst);";

            MySqlParameter[] invTransDetsParams = new MySqlParameter[]
            {
            new MySqlParameter("@trans_id", selectDbData[0][0][0]),
            new MySqlParameter("@cate_id", detail.cateId.ToString()),
            new MySqlParameter("@sub_cat_id", detail.subCatId.ToString()),
            new MySqlParameter("@item_id", detail.itemId.ToString()),
            new MySqlParameter("@brand_id", detail.brandId.ToString()),
            new MySqlParameter("@qty", detail.qty.ToString()),
            new MySqlParameter("@price_per_unit", detail.pricePerUnit.ToString()),
            new MySqlParameter("@gst_per", detail.gstPer.ToString()),
            new MySqlParameter("@gst_amount", detail.gstAmount.ToString()),
            new MySqlParameter("@price_with_gst", detail.priceWithGst.ToString()),
            new MySqlParameter("@price_without_gst", detail.priceWithoutGst.ToString()),
            new MySqlParameter("@total_gst", detail.totalGst.ToString())
            };

            var invTransDetsDbData = ds.executeSQL(invTransDetsSq, invTransDetsParams);
        }

        if (invTransDbData == null) // error occurred
        {
            resData.rStatus = 100; // database error, this error is caught at the app level
            resData.rData["Error"] = errors.err[100];
        }

        // Set response data
        resData.eventID = reqData.eventID;
        resData.rData["rMessage"] = "Data Inserted Successfully";

        return resData;
    }

    public async Task<responseData> SelectTransaction(requestData reqData)
    {
        responseData resData = new responseData();
        try
        {
            var sq = @"SELECT DISTINCT einvtrans.e_inv_tran_id, einvtrans.t_vou_no, supcust.contact, supcust.email_id, supcust.orgnigation, supcust.name
                        FROM e_inv_trans einvtrans
                        INNER JOIN m_sup_cust_dtl supcust ON supcust.id = einvtrans.sup_cust_id
                        inner JOIN e_inv_trans_dets einvtransdets ON einvtransdets.trans_id = einvtrans.e_inv_tran_id where t_vou_no like'%PO~%'; ";
            MySqlParameter[] myParams = new MySqlParameter[] { };
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
                        {
                            resData.eventID = reqData.eventID;
                            var list = new ArrayList();
                            Dictionary<string, string> myDict =
                            new Dictionary<string, string>();
                            for (var i = 0; i < dbdata[0].Count(); i++)
                            {
                                myDict = new Dictionary<string, string>();
                                myDict.Add("orderNo", dbdata[0][i][0].ToString());
                                myDict.Add("supplierId", dbdata[0][i][1].ToString());
                                myDict.Add("contactNumber", dbdata[0][i][2].ToString());
                                myDict.Add("contactEmail", dbdata[0][i][3].ToString());
                                myDict.Add("supplierName", dbdata[0][i][4].ToString());
                                myDict.Add("contactPerson", dbdata[0][i][5].ToString());
                                list.Add(myDict);
                            }
                            resData.rData["rMessage"] = list;

                        }


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

    public async Task<responseData> SelectTransactionById(requestData reqData)
    {
        responseData resData = new responseData();
        try
        {
            var transinvSq = @"SELECT einvtrans.t_vou_no,supcust.orgnigation , einvtrans.t_date from e_inv_trans einvtrans inner join m_sup_cust_dtl supcust on supcust.id = einvtrans.sup_cust_id where e_inv_tran_id =@e_inv_tran_id;";
            var sq = @"SELECT trans_id,cate.cate_name,subcate.subcate_name , mitem.item_name, brand.brand_name,qty, einvtransdets.price_per_unit, einvtransdets.gst_per, gst_amount, price_with_gst, price_without_gst,total_gst, inv_id,mitem.item_id,brand.brand_id
            FROM e_inv_trans_dets einvtransdets
            INNER JOIN m_category cate ON cate.cate_id = einvtransdets.cate_id
            INNER JOIN m_subcategory subcate ON subcate.subcate_id= einvtransdets.sub_cat_id
            LEFT JOIN m_item_tbl mitem ON mitem.item_id= einvtransdets.item_id
            LEFT JOIN e_inv_trans einvtrans ON einvtrans.t_id = einvtransdets.trans_id
            INNER JOIN m_item_brand brand ON brand.brand_id = einvtransdets.brand_id
            LEFT JOIN m_sup_cust_dtl supcust ON supcust.id = einvtrans.sup_cust_id
            WHERE trans_id = @trans_id;";
            MySqlParameter[] myParams = new MySqlParameter[] {
        new MySqlParameter("@trans_id", reqData.addInfo["orderNo"]),
        new MySqlParameter("@e_inv_tran_id", reqData.addInfo["orderNo"])
      };
            var transinvdbdata = ds.executeSQL(transinvSq, myParams);
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


                        {
                            resData.eventID = reqData.eventID;
                            var list = new ArrayList();
                            Dictionary<string, string> myDict =
                            new Dictionary<string, string>();
                            for (var i = 0; i < dbdata[0].Count(); i++)
                            {
                                myDict = new Dictionary<string, string>();
                                myDict.Add("orderId", dbdata[0][i][0].ToString());
                                myDict.Add("cateName", dbdata[0][i][1].ToString());
                                myDict.Add("subcateName", dbdata[0][i][2].ToString());
                                myDict.Add("itemName", dbdata[0][i][3].ToString());
                                myDict.Add("brandName", dbdata[0][i][4].ToString());
                                myDict.Add("pricePerUnit", dbdata[0][i][6].ToString());
                                myDict.Add("quantity", dbdata[0][i][5].ToString());
                                myDict.Add("gstPer", dbdata[0][i][7].ToString());
                                myDict.Add("gstAmount", dbdata[0][i][8].ToString());
                                myDict.Add("priceWithGst", dbdata[0][i][9].ToString());
                                myDict.Add("priceWithoutGst", dbdata[0][i][10].ToString());
                                myDict.Add("totalGst", dbdata[0][i][11].ToString());
                                myDict.Add("invId", dbdata[0][i][12].ToString());
                                myDict.Add("itemId", dbdata[0][i][13].ToString());
                                myDict.Add("brandId", dbdata[0][i][14].ToString());
                                list.Add(myDict);

                            }
                            resData.rData["orderNo"] = transinvdbdata[0][0][0].ToString();
                            resData.rData["orgnigation"] = transinvdbdata[0][0][1].ToString();
                            resData.rData["orderDate"] = transinvdbdata[0][0][2].ToString();
                            resData.rData["supplyOrderDetails"] = list;


                        }
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


    public async Task<responseData> OderDetailsSelect(requestData reqData)
    {
        responseData resData = new responseData();
        try
        {
            var invtrans = @"select t_id, supcust.name, t_vou_no from e_inv_trans einvtrans 
                        inner join m_sup_cust_dtl supcust on supcust.id = einvtrans.sup_cust_id;";
            MySqlParameter[] invtransmyParams = new MySqlParameter[] { };
            var invtransdbdata = ds.executeSQL(invtrans, invtransmyParams);

            if (invtransdbdata == null)
            {
                resData.rStatus = 100; // Database error; this error is caught at the app level
                resData.rData["Error"] = errors.err[100];
            }
            else
            {
                if (invtransdbdata[0].Count() > 0)
                    try
                    {
                        resData.eventID = reqData.eventID;
                        var list = new ArrayList();
                        Dictionary<string, string> myDict =
                        new Dictionary<string, string>();
                        for (var i = 0; i < invtransdbdata[0].Count(); i++)
                        {
                            myDict = new Dictionary<string, string>();
                            myDict.Add("orderNo", invtransdbdata[0][i][0].ToString());
                            myDict.Add("supCustId", invtransdbdata[0][i][1].ToString());
                            myDict.Add("transVoucharNo", invtransdbdata[0][i][2].ToString());
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

    public async Task<responseData> receiveOrder(requestData reqData)
    {
        responseData resData = new responseData();

        // Get e_inv_trans data
        var invTransSq = @"SELECT * FROM sdc_ims.e_inv_trans WHERE e_inv_tran_id = @e_inv_tran_id";
        MySqlParameter[] invTransParams = new MySqlParameter[]
        {
    new MySqlParameter("@e_inv_tran_id", reqData.addInfo["orderId"])
        };
        var invTransdbdata = ds.executeSQL(invTransSq, invTransParams);

        // Check if the e_inv_trans data was found
        if (invTransdbdata == null || invTransdbdata.Count == 0)
        {
            resData.rStatus = 100; // database error, this error is caught at the app level
            resData.rData["Error"] = "Trans Id Not Found";
        }
        else
        {
            var orderNosq = @"SELECT REPLACE(e_inv_trans.t_vou_no, concat('RO','~'), '')+1 t_vou_no FROM sdc_ims.e_inv_trans WHERE t_vou_no LIKE CONCAT('RO~', '%') ORDER BY e_inv_tran_id DESC LIMIT 1;";
            MySqlParameter[] orderNoParams = new MySqlParameter[]
            {};
            var orderNodbdata = ds.executeSQL(orderNosq, orderNoParams);

            // Insert e_inv_trans data
            var invreceivedSq = @"INSERT INTO e_inv_trans (mnu_id, sup_cust_id, t_vou_no, ref_t_id, t_date, t_type) VALUES (102, @sup_cust_id, @t_vou_no, @ref_t_id, @t_date, @t_type);";
            MySqlParameter[] invreceivedParams = new MySqlParameter[]
            {
      new MySqlParameter("@sup_cust_id", invTransdbdata[0][0][3]),
      new MySqlParameter("@t_vou_no", "RO~"+orderNodbdata[0][0][0]),
      new MySqlParameter("@ref_t_id", invTransdbdata[0][0][0]),
      new MySqlParameter("@t_date", reqData.addInfo.ContainsKey("transtype") ? reqData.addInfo["transtype"] : null), // order trans date
 new MySqlParameter("@t_type", reqData.addInfo.ContainsKey("transtype") ? reqData.addInfo["transtype"] : null)
            };
            var invreceiveddbdata = ds.executeSQL(invreceivedSq, invreceivedParams);
            if (invreceiveddbdata == null) // error occurred
            {
                resData.rStatus = 100; // database error, this error is caught at the app level
                resData.rData["Error"] = errors.err[100];
            }
            else
            {
                var selectSq = @"SELECT LAST_INSERT_ID() AS e_inv_trans;";
                MySqlParameter[] selectParams = new MySqlParameter[] { };
                var selectDbData = ds.executeSQL(selectSq, selectParams);
                if (selectDbData == null) // error occurred
                {
                    resData.rStatus = 100; // database error, this error is caught at the app level
                    resData.rData["Error"] = "Data Not Found";
                }
                else
                {
                    var receiveddetailsSq = @"INSERT INTO e_inv_trans_dets(trans_id,item_id,brand_id,qty,purchase_date,warranty_date,mfg_date,model_no,rec_qty,rec_price)VALUES (@trans_id,@item_id,@brand_id,@qty,@purchase_date,@warranty_date,@mfg_date,@model_no,@rec_qty,@rec_price);";
                    MySqlParameter[] receiveddetailsParams = new MySqlParameter[]
                    {
new MySqlParameter("@trans_id", selectDbData[0][0][0]),
          new MySqlParameter("@item_id", reqData.addInfo["itemId"]),
          new MySqlParameter("@brand_id", reqData.addInfo["brandId"]),
          new MySqlParameter("@rec_qty", reqData.addInfo["receivedQuantity"]),
          new MySqlParameter("@qty", reqData.addInfo["orderedQuantity"]),
          new MySqlParameter("@model_no", reqData.addInfo["modelNo"]),
          new MySqlParameter("@purchase_date", reqData.addInfo["purchaseDate"]),
          new MySqlParameter("@warranty_date", reqData.addInfo["warrantyDate"]),
          new MySqlParameter("@mfg_date", reqData.addInfo["manufacturingDate"]),
          new MySqlParameter("@rec_price", reqData.addInfo["receivedPrice"])
                    };
                    var receivedDetailssDbData = ds.executeSQL(receiveddetailsSq, receiveddetailsParams);
                    List<string> serialNo = JsonConvert.DeserializeObject<List<string>>(reqData.addInfo["serialNo"].ToString());

                    for (int i = 0; i < serialNo.Count; i++)
                    {
                        string detail = serialNo[i];
                        // Insert e_inv_trans_dets data
                        var invTransDetsSq = @"INSERT INTO e_item_received_details(trans_id,item_id,serial_no)VALUES(@trans_id,@item_id,@serial_no);";
                        MySqlParameter[] invTransDetsParams = new MySqlParameter[]
                        {
            new MySqlParameter("@trans_id", selectDbData[0][0][0]),
            new MySqlParameter("@item_id", reqData.addInfo["itemId"] ),
            new MySqlParameter("@serial_no", detail),
                        };

                        var invTransDetsDbData = ds.executeSQL(invTransDetsSq, invTransDetsParams);
                    }
                    var stockSq = @"INSERT INTO e_stock(recv_id, item_id, stock_in, status) VALUES (@recv_id, @item_id, @in, 'IN');";
                    MySqlParameter[] stockParams = new MySqlParameter[]
                    {
            new MySqlParameter("@recv_id", selectDbData[0][0][0]),
            new MySqlParameter("@item_id", reqData.addInfo["itemId"]),
            new MySqlParameter("@in", reqData.addInfo["receivedQuantity"]),
                    };
                    var stockDbData = ds.executeSQL(stockSq, stockParams);
                }
            }
        }

        // Set response data
        resData.eventID = reqData.eventID;
        resData.rData["rMessage"] = "Data Inserted Successfully";

        return resData;
    }
    public async Task<responseData> SelectVoucherNo(requestData reqData)
    {
        responseData resData = new responseData();
        var voucherNoSq = @"select t_vou_no from e_inv_trans where t_vou_no like '%RO~%' ;";
        MySqlParameter[] voucherNoParams = new MySqlParameter[] { };
        var voucherNodbdata = ds.executeSQL(voucherNoSq, voucherNoParams);
        if (voucherNodbdata == null)
        {
            resData.rStatus = 100; // database error, this error is caught at the app level
            resData.rData = new Dictionary<string, object>
          {
            { "Error", "voucherNo is incorrect " }
          };
        }

        else
        {
            if (voucherNodbdata[0].Count() > 0)
                try
                {
                    resData.eventID = reqData.eventID;
                    var list = new ArrayList();
                    Dictionary<string, string> myDict =
                    new Dictionary<string, string>();
                    for (var i = 0; i < voucherNodbdata[0].Count(); i++)
                    {
                        myDict = new Dictionary<string, string>();

                        myDict.Add("voucherNo", voucherNodbdata[0][i][0].ToString());
                        list.Add(myDict);

                    }

                    resData.rData["voucherNo"] = list;
                }
                catch (System.Exception ex)
                {

                    Console.Write(ex.Message);
                }

        }
        return resData;

    }




    public async Task<responseData> returnSelectOrder(requestData reqData)
    {
        responseData resData = new responseData();
        var invTransSq = @"select invtrans.t_vou_no,rec_price , rec_qty, itemreceiveddetails.serial_no, itemreceiveddetails.recvd_id ,invtrans.t_date, 
                           invtransdets.purchase_date ,item.item_name,itembrand.brand_name  from  e_inv_trans invtrans
                           inner join e_inv_trans_dets invtransdets  on invtransdets.trans_id = invtrans.e_inv_tran_id
                           inner join e_item_received_details itemreceiveddetails on itemreceiveddetails.trans_id = invtrans.e_inv_tran_id 
                           inner join m_item_tbl item on item.item_id = invtransdets.item_id
                           inner join m_item_brand itembrand on itembrand.brand_id = invtransdets.brand_id where invtrans.t_vou_no like @RO;";
        MySqlParameter[] invTransParams = new MySqlParameter[] {
        new MySqlParameter("@RO", reqData.addInfo["RO"]),
      };
        var invTransdbdata = ds.executeSQL(invTransSq, invTransParams);
        if (invTransdbdata == null || invTransdbdata.Count == 0)
        {
            resData.rStatus = 100; // database error, this error is caught at the app level
            resData.rData = new Dictionary<string, object>
          {
            { "Error", "Trans Id Not Found" }
          };
        }
        else
        {
            if (invTransdbdata[0].Count() > 0)
                try
                {
                    resData.eventID = reqData.eventID;
                    var list = new ArrayList();
                    Dictionary<string, string> myDict =
                    new Dictionary<string, string>();
                    for (var i = 0; i < invTransdbdata[0].Count(); i++)
                    {
                        myDict = new Dictionary<string, string>();

                        myDict.Add("receivePrice", invTransdbdata[0][i][1].ToString());
                        myDict.Add("receiveQty", invTransdbdata[0][i][2].ToString());
                        myDict.Add("serialNo", invTransdbdata[0][i][3].ToString());
                        myDict.Add("receiveId", invTransdbdata[0][i][4].ToString());
                        myDict.Add("orderDate", invTransdbdata[0][i][5].ToString());
                        myDict.Add("purchaseDate", invTransdbdata[0][i][6].ToString());
                        myDict.Add("itemName", invTransdbdata[0][i][7].ToString());
                        myDict.Add("brandName", invTransdbdata[0][i][8].ToString());
                        list.Add(myDict);

                    }
                    resData.rData["voucherNo"] = invTransdbdata[0][0][0].ToString();
                    resData.rData["rMessage"] = list;
                }
                catch (System.Exception ex)
                {

                    Console.Write(ex.Message);
                }


        }




        return resData;
    }

}

