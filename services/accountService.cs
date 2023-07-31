using System.Collections;
using System.Text.Json;
using MySql.Data.MySqlClient;





public class accountService
{
    accountService account;
    decryptService cm = new decryptService();

    dbServices ds = new dbServices();

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


    public async Task<responseData> accountAdd(requestData reqData)
    {
                     
        responseData resData = new responseData();
        

        var sq = @"INSERT INTO m_account_tbl(acc_hol_name,account_no,ifsc_code,bank_name,bank_branch,address) 
                   VALUES (@acc_hol_name,@account_no,@ifsc_code,@bank_name,@bank_branch,@address);";
        MySqlParameter[] myParams = new MySqlParameter[] {
                         new MySqlParameter("@acc_hol_name", reqData.addInfo["accholname"]),
                         new MySqlParameter("@account_no", reqData.addInfo["accountno"]),
                         new MySqlParameter("@ifsc_code", reqData.addInfo["ifsccode"]),
                         new MySqlParameter("@bank_name", reqData.addInfo["bankname"]),
                         new MySqlParameter("@bank_branch", reqData.addInfo["bankbranch"]),
                         new MySqlParameter("@address", reqData.addInfo["address"])

                };
        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error this error is caught ar app level
            resData.rData["Error"] = errors.err[100];
            resData.rData["rMessage"]=errors.err[104];
        }
        else
        {
            resData.eventID = reqData.eventID;
            resData.rData["rMessage"] = "Data Inserted Successfully";
        }


        return resData;
    }


    public async Task<responseData> accountUpdate(requestData reqData)
    {
        responseData resData = new responseData();
        // UPDATE `sdc_ims`.`account_tbl` SET `ifsc_code` = 'dsgfdsgdfgsd' WHERE (`id` = '1');



        var sq = @"UPDATE m_account_tbl SET acc_hol_name=@acc_hol_name,account_no=@account_no,ifsc_code=@ifsc_code,bank_name=@bank_name,bank_branch=@bank_branch,
              address=@address WHERE acc_id=@id;";
        MySqlParameter[] myParams = new MySqlParameter[] {
                        new MySqlParameter("@id", reqData.addInfo["id"]),
                        new MySqlParameter("@acc_hol_name", reqData.addInfo["accholname"]),
                        new MySqlParameter("@account_no", reqData.addInfo["accountno"]),
                        new MySqlParameter("@ifsc_code", reqData.addInfo["ifsccode"]),
                        new MySqlParameter("@bank_name", reqData.addInfo["bankname"]),
                        new MySqlParameter("@bank_branch", reqData.addInfo["bankbranch"]),
                        new MySqlParameter("@address", reqData.addInfo["address"])
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


    public async Task<responseData> accountSelect(requestData reqData)
    {
        responseData resData = new responseData();
        

        var sq = @"SELECT acc_id,acc_hol_name,account_no,ifsc_code,bank_name, bank_branch,address FROM sdc_ims.m_account_tbl;";
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
            try
                    {
                        resData.eventID = reqData.eventID;
                        var list = new ArrayList();
                        Dictionary<string, string> myDict =
                        new Dictionary<string, string>();
                        for (var i = 0; i < dbdata[0].Count(); i++)
                        {
                            myDict = new Dictionary<string, string>();
                            myDict.Add("acc_id", dbdata[0][i][0].ToString());
                            myDict.Add("acc_hol_name", dbdata[0][i][1].ToString());
                            myDict.Add("account_no", dbdata[0][i][2].ToString());
                            myDict.Add("ifsc_code", dbdata[0][i][3].ToString());
                            myDict.Add("bank_name", dbdata[0][i][4].ToString());
                            myDict.Add("bank_branch", dbdata[0][i][5].ToString());
                            myDict.Add("address", dbdata[0][i][6].ToString());
                            list.Add(myDict);

                        }
                        resData.rData["rMessage"] = list;
                    }
                    catch (System.Exception ex)
                    {

                        Console.Write(ex.Message);
                    }

        }


        return resData;
    }
    //accountselebyid

    public async Task<responseData> accountSelectById(requestData reqData)
    {
        responseData resData = new responseData();

        var sq = @"SELECT * FROM m_account_tbl where acc_id=@id";
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
                resData.rData["acc_id"] = dbdata[0][0][0];
                resData.rData["account_holder_name"] = dbdata[0][0][1];
                resData.rData["account_no"] = dbdata[0][0][2];
                resData.rData["ifsc_code"] = dbdata[0][0][3];
                resData.rData["bank_name"] = dbdata[0][0][4];
                resData.rData["bank_branch"] = dbdata[0][0][5];
                resData.rData["address"] = dbdata[0][0][6];
            }
            else
            {
               // resData.rData["rMessage"] = "Incorrect Input";
                resData.rData["Error"]= errors.err[103];
            }

        }


        return resData;
    }


    public async Task<responseData> accountDelete(requestData reqData)
    {
        responseData resData = new responseData();

        var sq = @"DELETE FROM m_account_tbl WHERE acc_id=@id;";

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