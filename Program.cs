using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http; // added by me
using Microsoft.Net.Http.Headers; // added by me
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.IO;
using System.Collections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


WebHost.CreateDefaultBuilder().
ConfigureServices(s =>
{
    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    //IConfiguration appsettings = new ConfigurationBuilder()
    //                        .AddJsonFile("Properties/appsettings.json")
    //                        .Build();
    // this service will validate users and generate java web tokens
    //s.AddSingleton<grievanceService>();// this service will validate users and generate java web tokens
    s.AddSingleton<accountService>();
    s.AddSingleton<categoryService>();
    s.AddSingleton<customerService>();
    s.AddSingleton<itemService>();
    s.AddSingleton<supplierServices>();
    s.AddSingleton<aouthService>();
    s.AddSingleton<subcategoryService>();
    s.AddSingleton<orderDetailsServices>();
    s.AddSingleton<orderService>();
    s.AddSingleton<paymentService>();
    s.AddSingleton<homeService>();
    s.AddAuthorization();
    s.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = appsettings["Jwt:Issuer"],
            ValidAudience = appsettings["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appsettings["Jwt:Key"]))
        };

    });
    s.AddCors();

    s.AddControllers();



}).

Configure(app =>
 {
     app.UseStaticFiles();
     app.UseRouting();
     app.UseAuthentication();
     app.UseAuthorization();


     app.UseCors(options =>
         options.WithOrigins("https://localhost:5005", "http://localhost:5003")
         .AllowAnyHeader().AllowAnyMethod().AllowCredentials());

     app.UseEndpoints(e =>
     {
         var accservice = e.ServiceProvider.GetRequiredService<accountService>();
         var category = e.ServiceProvider.GetRequiredService<categoryService>();
         var subcategory = e.ServiceProvider.GetRequiredService<subcategoryService>();
         var cusService = e.ServiceProvider.GetRequiredService<customerService>();
         var item = e.ServiceProvider.GetRequiredService<itemService>();
         var suppServices = e.ServiceProvider.GetRequiredService<supplierServices>();
         var aouth = e.ServiceProvider.GetRequiredService<aouthService>();
         var order = e.ServiceProvider.GetRequiredService<orderService>();
         var orderdetails = e.ServiceProvider.GetRequiredService<orderDetailsServices>();
         var payment = e.ServiceProvider.GetRequiredService<paymentService>();
         var home = e.ServiceProvider.GetRequiredService<homeService>();


         try
         {
             e.MapPost("/account",
             [Authorize] async (HttpContext http) =>
          {
              var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
              requestData rData = JsonSerializer.Deserialize<requestData>(body);
              try
              {
                  if (rData.eventID == "0001")
                      await http.Response.WriteAsJsonAsync(await accservice.accountAdd(rData));

                  else if (rData.eventID == "0002")
                      await http.Response.WriteAsJsonAsync(await accservice.accountUpdate(rData));

                  else if (rData.eventID == "0003")
                      await http.Response.WriteAsJsonAsync(await accservice.accountSelect(rData));
                  else if (rData.eventID == "0004")
                      await http.Response.WriteAsJsonAsync(await accservice.accountSelectById(rData));

                  else if (rData.eventID == "0005")
                      await http.Response.WriteAsJsonAsync(await accservice.accountDelete(rData));

              }
              catch (System.Exception ex)
              {

                  Console.WriteLine(ex);
              }
          });
             e.MapPost("/orderdetails",
                [Authorize] async (HttpContext http) =>
             {
                 var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                 requestData rData = JsonSerializer.Deserialize<requestData>(body);
                 try
                 {
                     if (rData.eventID == "0001")
                         await http.Response.WriteAsJsonAsync(await orderdetails.orderDetailAdd(rData));
                     else if (rData.eventID == "0002")
                         await http.Response.WriteAsJsonAsync(await orderdetails.orderDetailUpdate(rData));
                     else if (rData.eventID == "0003")
                         await http.Response.WriteAsJsonAsync(await orderdetails.orderDetailSelect(rData));
                     else if (rData.eventID == "0004")
                         await http.Response.WriteAsJsonAsync(await orderdetails.orderDetailSelectById(rData));
                     else if (rData.eventID == "0005")
                         await http.Response.WriteAsJsonAsync(await orderdetails.orderDetailDelete(rData));


                 }
                 catch (System.Exception ex)
                 {

                     Console.WriteLine(ex);
                 }
             });

             e.MapPost("/payment",
               [Authorize] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                try
                {
                    if (rData.eventID == "0001")
                        await http.Response.WriteAsJsonAsync(await payment.paymentAdd(rData));
                    else if (rData.eventID == "0002")
                        await http.Response.WriteAsJsonAsync(await payment.paymentUpdate(rData));
                    else if (rData.eventID == "0003")
                        await http.Response.WriteAsJsonAsync(await payment.paymentSelect(rData));
                    else if (rData.eventID == "0004")
                        await http.Response.WriteAsJsonAsync(await payment.paymentSelectById(rData));
                    else if (rData.eventID == "0005")
                        await http.Response.WriteAsJsonAsync(await payment.paymentDelete(rData));


                }
                catch (System.Exception ex)
                {

                    Console.WriteLine(ex);
                }
            });


             e.MapPost("/category",
             [Authorize] async (HttpContext http) =>
             {
                 var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                 requestData rData = JsonSerializer.Deserialize<requestData>(body);
                 try
                 {

                     if (rData.eventID == "CATEGORY_ADD")
                         await http.Response.WriteAsJsonAsync(await category.categoryAdd(rData));
                     if (rData.eventID == "CATEGORY_UPDATE")
                         await http.Response.WriteAsJsonAsync(await category.categoryUpdate(rData));
                     if (rData.eventID == "CATEGORY_DELETE")
                         await http.Response.WriteAsJsonAsync(await category.categoryDelete(rData));
                     if (rData.eventID == "CATEGORY_SELECT")
                         await http.Response.WriteAsJsonAsync(await category.categorySelect(rData));
                     if (rData.eventID == "CATEGORY_SELECT_BY_ID")
                         await http.Response.WriteAsJsonAsync(await category.categorySelectByID(rData));


                 }
                 catch (System.Exception ex)
                 {

                     Console.WriteLine(ex);
                 }


             });

             e.MapPost("/subcategory",
              [Authorize] async (HttpContext http) =>
              {
                  var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                  requestData rData = JsonSerializer.Deserialize<requestData>(body);
                  try
                  {
                      if (rData.eventID == "GET_CATEGORY")
                          await http.Response.WriteAsJsonAsync(await category.categorySelect(rData));
                      if (rData.eventID == "SUBCATEGORY_ADD")
                          await http.Response.WriteAsJsonAsync(await subcategory.subcategoryAdd(rData));
                      if (rData.eventID == "SUBCATEGORY_DELETE")
                          await http.Response.WriteAsJsonAsync(await subcategory.subcategoryDelete(rData));
                      if (rData.eventID == "SUBCATEGORY_SELECT")
                          await http.Response.WriteAsJsonAsync(await subcategory.subcategorySelct(rData));
                      if (rData.eventID == "SUBCATEGORY_UPDATE")
                          await http.Response.WriteAsJsonAsync(await subcategory.subcategoryUpdate(rData));
                      if (rData.eventID == "SUBCATEGORY_SELECT_BY_ID")
                          await http.Response.WriteAsJsonAsync(await subcategory.subcategorySelctById(rData));

                  }
                  catch (System.Exception ex)
                  {

                      Console.WriteLine(ex);
                  }


              });
             e.MapPost("/item",
            [Authorize] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                try
                {

                    if (rData.eventID == "GET_CATEGORT_SUBCATEGORY")
                        await http.Response.WriteAsJsonAsync(await item.GetCategoriesAndSubcategories(rData));
                    if (rData.eventID == "ITEM_BRAND")
                        await http.Response.WriteAsJsonAsync(await item.itemBrand(rData));
                    if (rData.eventID == "ITEM_ADD")
                        await http.Response.WriteAsJsonAsync(await item.itemsAdd(rData));
                    if (rData.eventID == "ITEM_UPDATE")
                        await http.Response.WriteAsJsonAsync(await item.itemsUpdate(rData));
                    if (rData.eventID == "ITEM_DELETE")
                        await http.Response.WriteAsJsonAsync(await item.itemsDelete(rData));
                    if (rData.eventID == "ITEM_SELECT")
                        await http.Response.WriteAsJsonAsync(await item.itemsSelect(rData));
                    if (rData.eventID == "ITEM_SELECT_BY_ID")
                        await http.Response.WriteAsJsonAsync(await item.itemsSelectById(rData));

                }
                catch (System.Exception ex)
                {

                    Console.WriteLine(ex);
                }


            });

             e.MapPost("/order",
                [Authorize] async (HttpContext http) =>
             {
                 var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                 requestData rData = JsonSerializer.Deserialize<requestData>(body);
                 try
                 {
                     if (rData.eventID == "CALL_CATEGORY_SUBCATEGORY")
                         await http.Response.WriteAsJsonAsync(await order.callcategorySubcategory(rData));
                     else if (rData.eventID == "ORDER_ADD")
                         await http.Response.WriteAsJsonAsync(await order.orderAdd(rData));
                     else if (rData.eventID == "TRANSACTION_ADD")
                         await http.Response.WriteAsJsonAsync(await order.AddTransaction(rData));
                     else if (rData.eventID == "ORDER_SELECT")
                         await http.Response.WriteAsJsonAsync(await order.SelectTransaction(rData));
                     else if (rData.eventID == "ORDER_SELECT_BY_ID")
                         await http.Response.WriteAsJsonAsync(await order.SelectTransactionById(rData));
                     else if (rData.eventID == "ORDERDETAILS_SELECT")
                         await http.Response.WriteAsJsonAsync(await order.OderDetailsSelect(rData));
                     else if (rData.eventID == "RECEIVE_ORDER")
                         await http.Response.WriteAsJsonAsync(await order.receiveOrder(rData));
                     else if (rData.eventID == "RETURN_ORDER_SELECT")
                         await http.Response.WriteAsJsonAsync(await order.returnSelectOrder(rData));
                     else if (rData.eventID == "VOUCHER_NO")
                         await http.Response.WriteAsJsonAsync(await order.SelectVoucherNo(rData));



                 }
                 catch (System.Exception ex)
                 {

                     Console.WriteLine(ex);
                 }
             });

             e.MapPost("/customer",
               [Authorize] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                try
                {
                    if (rData.eventID == "COUSTOMER_ADD")
                        await http.Response.WriteAsJsonAsync(await cusService.customerAdd(rData));

                    else if (rData.eventID == "COUSTOMER_UPDATE")
                        await http.Response.WriteAsJsonAsync(await cusService.customerUpdate(rData));

                    else if (rData.eventID == "COUSTOMER_SELECT")
                        await http.Response.WriteAsJsonAsync(await cusService.customerSelect(rData));
                    else if (rData.eventID == "COUSTOMER_SELECT_BY_ID")
                        await http.Response.WriteAsJsonAsync(await cusService.customerSelectById(rData));

                    else if (rData.eventID == "COUSTOMER_DELETE")
                        await http.Response.WriteAsJsonAsync(await cusService.customerDelete(rData));

                }
                catch (System.Exception ex)
                {

                    Console.WriteLine(ex);
                }
            });


             e.MapPost("/supplier",
                [Authorize] async (HttpContext http) =>
             {
                 var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                 requestData rData = JsonSerializer.Deserialize<requestData>(body);
                 try
                 {
                     if (rData.eventID == "0001")
                         await http.Response.WriteAsJsonAsync(await suppServices.supplierAdd(rData));
                    else if (rData.eventID == "0002")
                         await http.Response.WriteAsJsonAsync(await suppServices.supplierUpdate(rData));
                     else if (rData.eventID == "0003")
                         await http.Response.WriteAsJsonAsync(await suppServices.supplierDelete(rData));
                     else if (rData.eventID == "0004")
                         await http.Response.WriteAsJsonAsync(await suppServices.GetSupplierCustomer(rData));
                     else if (rData.eventID == "0005")
                         await http.Response.WriteAsJsonAsync(await suppServices.supplierSelectById(rData));
                     else if (rData.eventID == "0006")
                         await http.Response.WriteAsJsonAsync(await suppServices.GetCitiesAndStates(rData));

                 }
                 catch (System.Exception ex)
                 {

                     Console.WriteLine(ex);
                 }
             });

             e.MapPost("/user",
               [AllowAnonymous] async (HttpContext http) =>
               {
                   var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                   requestData rData = JsonSerializer.Deserialize<requestData>(body);
                   try
                   {
                       if (rData.eventID == "LOGIN")  // login
                           await http.Response.WriteAsJsonAsync(await aouth.ValidateUser(rData));

                       else if (rData.eventID == "REGISTRATION")
                           await http.Response.WriteAsJsonAsync(await aouth.Registration(rData));


                   }
                   catch (System.Exception ex)
                   {

                       Console.WriteLine(ex);
                   }


               });

             e.MapPost("/home",
          [Authorize] async (HttpContext http) =>
          {

              var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
              requestData rData = JsonSerializer.Deserialize<requestData>(body);
              if (rData.eventID == "HOME")
                  await http.Response.WriteAsJsonAsync(await home.getHomeDetails(rData));

          });


             e.MapGet("/",

                 async c => await c.Response.WriteAsJsonAsync("Hello Word!.."));

             //e.MapDefaultControllerRoute();

         }
         catch (Exception ex)
         {
             Console.Write(ex);
         }

     });
 }).Build().Run();



public record requestData
{ //request data
  //SOURCE.srv.fn_CS({ rID: "F000", rData: {encData: encrypted}}, page_OS, $("#progressBarFooter")[0]);
    [Required]
    public string eventID { get; set; } //  request ID this is the ID of entity requesting the API (UTI/CDAC/CAC) this is used to pick up the respective private key for the requesting user
    [Required]
    public IDictionary<string, object> addInfo { get; set; } // request data .. previously addInfo 
}

public record responseData
{ //response data
    public responseData()
    { // set default values here
        eventID = "";
        rStatus = 0;
        rData = new Dictionary<string, object>();

    }
    [Required]
    public int rStatus { get; set; } = 0; // this will be defaulted 0 fo success and other numbers for failures
    [Required]
    public string eventID { get; set; } //  response ID this is the ID of entity requesting the
    public IDictionary<string, object> addInfo { get; set; } // request data .. previously addInfo 
    public Dictionary<string, object> rData { get; set; }
    //public ArrayList rData {get;set;}
}
