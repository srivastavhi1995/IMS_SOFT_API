
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;


public class aouthService
{
    aouthService aouth;


    decryptService cm = new decryptService();

    dbServices ds = new dbServices();


    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();



    public async Task<responseData> ValidateUser(requestData reqData)
    {
        responseData resData = new responseData();
        try
        {
            
        MySqlParameter[] myParams = new MySqlParameter[] {
            new MySqlParameter("@userid",reqData.addInfo["email"].ToString()),
            new MySqlParameter("@guid",reqData.addInfo["guid"].ToString()),
            new MySqlParameter("@password",reqData.addInfo["password"].ToString())
            };
        var sq = "CALL verifyUser(@userid,@guid,@password);";


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
                if (dbdata[0][0][0].ToString() != "0") // invalid user
                {
                    resData.rData["rCode"] = dbdata[0][0][0];
                    resData.rData["rMessage"] = dbdata[0][0][1];
                    resData.rData["Error"]= errors.err[101];
                }
                else
                {
                    //valid user
                    resData.rStatus = 0;
                    resData.rData["rMessage"] = "LOGIN Successfully";
                    resData.rData["user_name"]=dbdata[0][0][7];
                    resData.rData["user_email"]=dbdata[0][0][4];
                    var unm = dbdata[0][0][3].ToString(); // mobile number
                    var uid = dbdata[0][0][2].ToString(); // uid
                    var guid = dbdata[0][0][5].ToString(); // guid
                    var email = dbdata[0][0][4].ToString(); // email


                    //GENERATE TOKEN HERE IF USER IS VALID
                    var claims = new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier,uid),
                    new Claim(ClaimTypes.Name, unm),
                    new Claim(ClaimTypes.SerialNumber, guid),
                    new Claim(ClaimTypes.Email,email)
                };
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appsettings["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
                    var tokenDescriptor = new JwtSecurityToken(issuer: appsettings["Jwt:Issuer"], audience: appsettings["Jwt:Audience"], claims: claims,
                        expires: DateTime.Now.AddMinutes(Int16.Parse(appsettings["Jwt:ExpiryDuration"])), signingCredentials: credentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

                    resData.rData["rCode"] = dbdata[0][0][0];
                    resData.rData["rMessage"] = dbdata[0][0][1];
                    resData.rData["jwt"] = token; //adding a key/value using the Add() method                
                    resData.eventID = reqData.eventID;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                

            }

        }
        }
        catch (System.Exception ex)
        {
            
            resData.rData["rCode"]=105;
            resData.rData["rData"]= errors.err[105] + Environment.NewLine + ex.Message;
        }

        return resData;
    }


    public async Task<responseData> Registration(requestData reqData)
    {
        responseData resData = new responseData();
        try
        {
             var sq = @"insert into reg_users(mobile_no,email_id,guid,password,name,role_id) values(@mobile_no,@email_id,@guid,@pass,@name,@role_id);";

        MySqlParameter[] myParams = new MySqlParameter[]{
                    new MySqlParameter("@mobile_no", reqData.addInfo["mobile"]),
                    new MySqlParameter("@email_id", reqData.addInfo["email"]),
                    new MySqlParameter("@guid", reqData.addInfo["guid"]),
                    new MySqlParameter("@pass", reqData.addInfo["password"]),
                    new MySqlParameter("@name", reqData.addInfo["name"]),
                    new MySqlParameter("@role_id", reqData.addInfo["role_id"])
                   
                };

        var dbdata = ds.executeSQL(sq, myParams);
        if (dbdata == null) // error occured
        {
            resData.rStatus = 100; // database error
            resData.rData["Eroor"]= errors.err[104];
        }
        else
        {
            resData.rStatus = 0;
            resData.rData["rMessage"] = "Register Successfully";
            resData.eventID = reqData.eventID;
        }
        }
        catch (System.Exception ex)
        {
            resData.rData["rCode"]=105;
            resData.rData["rData"]= errors.err[105] + Environment.NewLine + ex.Message;
            
        }
       

        return resData;
    }


}