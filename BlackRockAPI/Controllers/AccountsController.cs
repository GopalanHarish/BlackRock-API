using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BlackRockAPI.DataModel;
using BlackRockAPI.Providers;
using BlackRockAPI.Helpers;
using System.Net.Http.Formatting;
using System.Web;
using BlackRockAPI.Models;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.IO;

namespace BlackRockAPI.Controllers
{

    public class AccountsController : ApiController
    {
        private BlackRockEntities entity;
        public AccountsController()
        {
            entity = new BlackRockEntities();
        }

        [HttpGet]
        public HttpResponseMessage GetUsers(long roleId)
        {
            ResponseMessage<List<AppUser>> objResponseData = new ResponseMessage<List<AppUser>>();
            try
            {
                List<AppUser> userList = entity.Users.Where(x => (x.RoleId == roleId && x.IsDeleted == false))
                                            .Select(x => new AppUser()
                                            {
                                                Id = x.Id,
                                                FirstName = x.FirstName,
                                                LastName = x.LastName,
                                                ContactNumber = x.ContactNumber,
                                                Email = x.Email,
                                                IsActive = x.IsActive,
                                                RegisteredOn = x.RegisteredOn,
                                                LastLoggedOn = x.LastLoggedOn,
                                            }).ToList();
                objResponseData = ResponseContext<AppUser>.CreateResponse(objResponseData, "success", userList, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                objResponseData = ResponseContext<AppUser>.CreateErrorResponse(objResponseData);
            }
            return Request.CreateResponse(objResponseData);
        }

        [HttpGet]
        public HttpResponseMessage GetUser(long userId)
        {
            ResponseMessage<AppUser> objResponseData = new ResponseMessage<AppUser>();
            try
            {
                PhysicalPath path = entity.PhysicalPaths.Where(x => x.Identifier == "Image").FirstOrDefault();
                AppUser user = entity.Users.Where(x => x.Id == userId && x.IsDeleted == false)
                                            .Select(x => new AppUser()
                                            {
                                                Id = x.Id,
                                                FirstName = x.FirstName,
                                                LastName = x.LastName,
                                                Email = x.Email,
                                                ContactNumber = x.ContactNumber,
                                                RegisteredOn = x.RegisteredOn,
                                                IsActive = x.IsActive,
                                                LastLoggedOn = x.LastLoggedOn,
                                                Bio = x.Bio,
                                                ImageUrl = x.ImageUrl
                                            })
                                            .FirstOrDefault();

                if(user.ImageUrl == null || user.ImageUrl == string.Empty)
                {
                    user.ImageUrl = path.BaseUrl + path.DefaultImageUrl;
                }
                else
                {
                    user.ImageUrl = path.BaseUrl + user.ImageUrl;
                }

                user.Funds = entity.Schemes.Where(x => x.FundManagerId == user.Id)
                                          .Select( x => new FundsUnderManagement() {
                                               Title = x.Title,
                                               ManagingSince = x.ManagingSince,
                                               AUM = x.AUM,
                                               ExpenseRatio = x.ExpenseRatio
                                          }).ToList();

                objResponseData = ResponseContext<AppUser>.CreateResponse(objResponseData, "success", user, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                objResponseData = ResponseContext<AppUser>.CreateErrorResponse(objResponseData);
            }
            return Request.CreateResponse(objResponseData);
        }

        //[HttpGet]
        //public HttpResponseMessage GetFundManagers()
        //{
        //    ResponseMessage<List<AppUser>> objResponseData = new ResponseMessage<List<AppUser>>();
        //    try
        //    {
        //        long roleId = entity.Roles.Where(x => x.RoleName == "FundManager").Select(x => x.Id).FirstOrDefault();

        //        List<AppUser> userList = entity.Users.Where(x => x.RoleId == roleId)
        //                                    .Select(x => new AppUser()
        //                                    {
        //                                        Id = x.Id,
        //                                        FirstName = x.FirstName,
        //                                        LastName = x.LastName,
        //                                        ContactNumber = x.ContactNumber,
        //                                        Email = x.Email,
        //                                        IsActive = x.IsActive,
        //                                        RegisteredOn = x.RegisteredOn,
        //                                        LastLoggedOn = x.LastLoggedOn,
        //                                    })
        //                                    .ToList();
        //        objResponseData = ResponseContext<AppUser>.CreateResponse(objResponseData, "success", userList, HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        objResponseData = ResponseContext<AppUser>.CreateErrorResponse(objResponseData);
        //    }
        //    return Request.CreateResponse(objResponseData);
        //}

        [HttpGet]
        public HttpResponseMessage GetAppNavbar(long roleId)
        {
            ResponseMessage<List<Navigation>> objResponseData = new ResponseMessage<List<Navigation>>();

            try
            {
                List<Navigation> navigations = entity.Navigations.Where(x => x.AssignedToUserRole == roleId).ToList();
                objResponseData = ResponseContext<Navigation>.CreateResponse(objResponseData, "success", navigations, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                objResponseData = ResponseContext<Navigation>.CreateErrorResponse(objResponseData);
            }
            return Request.CreateResponse(objResponseData);
        }

        [HttpPost]
        public HttpResponseMessage ValidateUser(LoginModel userData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string baseAddress = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/token";

                    HttpClient client = new HttpClient();

                    var form = new Dictionary<string, string> { { "grant_type", "password" }, { "username", userData.Email }, { "password", userData.Password } };

                    var tokenResponse = client.PostAsync(baseAddress, new FormUrlEncodedContent(form)).Result;
                    var token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;

                    if (tokenResponse.IsSuccessStatusCode)
                    {
                        userData.Password = new TripleDES().BytesToStringPadded(new TripleDES().Encrypt(userData.Password));

                        UserDetails userDetails = entity.Users
                                                  .Where(x => (x.Email == userData.Email && x.Password == userData.Password))
                                                  .Select(x => new UserDetails
                                                  {
                                                      userId = x.Id.ToString(),
                                                      roleId = x.RoleId.ToString(),
                                                      userName = x.FirstName + " " + x.LastName,
                                                      isActive = x.IsActive
                                                  })
                                                  .FirstOrDefault();

                        long roleId = Convert.ToInt64(userDetails.roleId);
                        userDetails.role = entity.Roles.FirstOrDefault(x => x.Id == roleId).RoleName;

                        userDetails.userId = Base64Encoder.EncodeTo64(userDetails.userId);
                        userDetails.roleId = Base64Encoder.EncodeTo64(userDetails.roleId);

                        List<Navigation> navigations = entity.Navigations
                                                             .Where(x => x.AssignedToUserRole == roleId)
                                                             .ToList();

                        if(userDetails.isActive)
                        {
                            var response = new
                            {
                                status = true,
                                message = "User Authentication Successful.",
                                data = userDetails,
                                navigationLinks = navigations,
                                statusCode = HttpStatusCode.OK,
                                token = new { key = token.AccessToken, issue = DateTime.Now, expire = DateTime.Now.AddSeconds(token.ExpiresIn) }
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            var response = new
                            {
                                status = false,
                                message = "Your account is not active",
                                data = string.Empty,
                                navigationLinks = string.Empty,
                                statusCode = HttpStatusCode.Forbidden,
                                token = new { key = string.Empty }
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                    }
                    else
                    {
                        var response = new
                        {
                            status = false,
                            message = "Incorrect username or password",
                            data = string.Empty,
                            navigationLinks = string.Empty,
                            statusCode = HttpStatusCode.NotFound,
                            token = new { key = string.Empty }
                        };
                        return Request.CreateResponse(HttpStatusCode.NotFound, response);
                    }
                }
                else
                {
                    var response = new
                    {
                        status = false,
                        message = "Please fill all mandatory fields",
                        data = string.Empty,
                        navigationLinks = string.Empty,
                        StatusCode = HttpStatusCode.BadRequest,
                        token = new { key = string.Empty }
                    };
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                var response = new
                {
                    status = false,
                    message = "Problem with server. Please try again !",
                    data = string.Empty,
                    navigationLinks = string.Empty,
                    statusCode = HttpStatusCode.InternalServerError,
                    token = new { key = string.Empty }
                };
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        public HttpResponseMessage RegisterUser(User AppUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(entity.Users.Where(x => (x.Email == AppUser.Email || x.ContactNumber == AppUser.ContactNumber)).FirstOrDefault() != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Ambiguous, new
                        {
                            status = false,
                            message = "User already exist",
                            StatusCode = HttpStatusCode.Ambiguous
                        });
                    }

                    AppUser.Password = new TripleDES().BytesToStringPadded(new TripleDES().Encrypt(AppUser.Password));
                    AppUser.RegisteredOn = System.DateTime.Now;
                    AppUser.RoleId = entity.Roles.Where(x => x.RoleName == "Investor").Select(x => x.Id).FirstOrDefault();
                    entity.Users.Add(AppUser);
                    entity.SaveChanges();

                    var response = new
                    {
                        status = true,
                        message = "Account created.",
                        statusCode = HttpStatusCode.OK,
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    var response = new
                    {
                        status = false,
                        message = "Please fill all mandatory fields",
                        StatusCode = HttpStatusCode.BadRequest,
                    };
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                var response = new
                {
                    status = false,
                    message = "Problem with server. Please try again !",
                    statusCode = HttpStatusCode.InternalServerError,
                };
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        public HttpResponseMessage CreateUser()
        {
            ResponseMessage<string> objResponseData = new ResponseMessage<string>();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                User appUser = JsonConvert.DeserializeObject<User>(httpRequest.Form["userData"]);
                string roleName = httpRequest.Form["userRole"];

                if (ModelState.IsValid)
                {
                    appUser.Password = new TripleDES().BytesToStringPadded(new TripleDES().Encrypt("BlackRock"));
                    appUser.RegisteredOn = System.DateTime.Now;
                    appUser.RoleId = entity.Roles.Where(x => x.RoleName == roleName).Select(x => x.Id).FirstOrDefault();
                    entity.Users.Add(appUser);
                    entity.SaveChanges();

                    if (httpRequest.Files.Count > 0)
                    {
                        var postedFile = httpRequest.Files[0];
                        var filePath = HttpContext.Current.Server.MapPath("~/UserData/" + appUser.Id + "/ProfileImage/");
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        postedFile.SaveAs(filePath + postedFile.FileName);
                        appUser.ImageUrl = "/UserData/" + appUser.Id + "/ProfileImage/" + postedFile.FileName;

                        entity.Users.Add(appUser);
                        entity.Entry(appUser).State = System.Data.Entity.EntityState.Modified;
                        entity.SaveChanges();
                    }

                    objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "success", "Account Created", HttpStatusCode.OK);
                    return Request.CreateResponse(objResponseData);
                }
                else
                {
                    objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "failed", "Validation Failed", HttpStatusCode.BadRequest);
                    return Request.CreateResponse(objResponseData);
                }
            }
            catch (Exception ex)
            {
                objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "failed", "Internal Server Error", HttpStatusCode.InternalServerError);
                return Request.CreateResponse(objResponseData);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateUser()
        {
            ResponseMessage<string> objResponseData = new ResponseMessage<string>();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var userData = JsonConvert.DeserializeObject<User>(httpRequest.Form["userData"]);

                if (userData != null)
                {
                    User appUser = entity.Users.Where(x => x.Id == userData.Id).FirstOrDefault();

                    if(appUser != null)
                    {
                        appUser.FirstName = userData.FirstName;
                        appUser.LastName = userData.LastName;
                        appUser.Email = userData.Email;
                        appUser.ContactNumber = userData.ContactNumber;
                        appUser.Bio = userData.Bio;

                        entity.Users.Add(appUser);
                        entity.Entry(appUser).State = System.Data.Entity.EntityState.Modified;
                        entity.SaveChanges();

                        if (httpRequest.Files.Count > 0)
                        {
                            var postedFile = httpRequest.Files[0];
                            var filePath = HttpContext.Current.Server.MapPath("~/UserData/" + appUser.Id + "/ProfileImage/");
                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }
                            postedFile.SaveAs(filePath + postedFile.FileName);
                            appUser.ImageUrl = "/UserData/" + appUser.Id + "/ProfileImage/" + postedFile.FileName;

                            entity.Users.Add(appUser);
                            entity.Entry(appUser).State = System.Data.Entity.EntityState.Modified;
                            entity.SaveChanges();
                        }
                        objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "success", "User details updated", HttpStatusCode.OK);
                    }
                    else
                    {
                        objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "failed", "failed to update details", HttpStatusCode.NotFound);
                    }
                    
                    return Request.CreateResponse(objResponseData);
                }
                else
                {
                    objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "failed", "Validation Failed", HttpStatusCode.BadRequest);
                    return Request.CreateResponse(objResponseData);
                }
            }
            catch (Exception ex)
            {
                objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "failed", "Internal Server Error", HttpStatusCode.InternalServerError);
                return Request.CreateResponse(objResponseData);
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteUser(long userId)
        {
            ResponseMessage<string> objResponseData = new ResponseMessage<string>();
            try
            {
                if (userId > 0)
                {
                    User appUser = entity.Users.Where(x => x.Id == userId).FirstOrDefault();

                    if (appUser != null)
                    {
                        entity.Users.Add(appUser);
                        entity.Entry(appUser).State = System.Data.Entity.EntityState.Modified;
                        entity.SaveChanges();
                        objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "success", "User deleted successfully", HttpStatusCode.OK);
                    }
                    else
                    {
                        objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "failed", "User not found", HttpStatusCode.NotFound);
                    }
                    
                    return Request.CreateResponse(objResponseData);
                }
                else
                {
                    objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "failed", "Validation Failed", HttpStatusCode.BadRequest);
                    return Request.CreateResponse(objResponseData);
                }
            }
            catch (Exception ex)
            {
                objResponseData = ResponseContext<string>.CreateResponse(objResponseData, "failed", "Internal Server Error", HttpStatusCode.InternalServerError);
                return Request.CreateResponse(objResponseData);
            }
        }
    }


}
