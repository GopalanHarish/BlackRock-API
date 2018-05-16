using BlackRockAPI.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BlackRockAPI.Helpers;
using BlackRockAPI.Providers;
using BlackRockAPI.Models;

namespace BlackRockAPI.Controllers
{
    public class FundsController : ApiController
    {
        private BlackRockEntities entity;
        public FundsController()
        {
            entity = new BlackRockEntities();
        }
        public HttpResponseMessage GetFunds(long roleId, long userId)
        {
            ResponseMessage<List<Funds>> objResponseData = new ResponseMessage<List<Funds>>();

            try
            {
                if (roleId == Convert.ToInt64(roles.Administrator) || roleId == Convert.ToInt64(roles.Broker))
                {
                    List<Funds> funds = entity.GetFunds()
                                              .Select(x => new Funds()
                                              {
                                                  Id = x.Id,
                                                  Inception_Date = x.Inception_Date,
                                                  Title = x.Title,
                                                  Category = x.Category,
                                                  Type = x.Type,
                                                  Goal = x.Goal,
                                                  Horizone = x.Investment_Horizone,
                                                  Fund_Manager = x.Fund_Manager
                                              }).ToList();

                    objResponseData = ResponseContext<Funds>.CreateResponse(objResponseData, "success", funds, HttpStatusCode.OK);
                }
                else
                {
                    List<Funds> funds = entity.GetFunds().Where(x => x.FundManagerId == userId)
                                              .Select(x => new Funds()
                                              {
                                                  Id = x.Id,
                                                  Inception_Date = x.Inception_Date,
                                                  Title = x.Title,
                                                  Category = x.Category,
                                                  Type = x.Type,
                                                  Goal = x.Goal,
                                                  Horizone = x.Investment_Horizone,
                                                  Fund_Manager = x.Fund_Manager
                                              }).ToList();


                    objResponseData = ResponseContext<Funds>.CreateResponse(objResponseData, "success", funds, HttpStatusCode.OK);
                }

            }
            catch(Exception ex)
            {
                objResponseData = ResponseContext<Funds>.CreateErrorResponse(objResponseData);
            }
            return Request.CreateResponse(objResponseData);
        }

        public HttpResponseMessage GetGenericIdTitleCollection(string ddRequest)
        {
            ResponseMessage<object> objResponseData = new ResponseMessage<object>();
            
            try
            {
                switch(ddRequest)
                {
                    case "FundTypes" : object schemes = entity.SchemeTypes.Select(x => new { x.Id, x.Title }).ToList();
                        objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "success", schemes, HttpStatusCode.OK);
                        break;
                    case "FundCategories": object categories = entity.SchemeCategories.Select(x => new { x.Id, x.Title }).ToList();
                        objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "success", categories, HttpStatusCode.OK);
                        break;
                    case "FundPlans": object plans = entity.SchemePlans.Select(x => new { x.Id, x.Title }).ToList();
                        objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "success", plans, HttpStatusCode.OK);
                        break;
                    case "Benchmarks": object benchmarks = entity.Benchmarks.Select(x => new { x.Id, x.Title }).ToList();
                        objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "success", benchmarks, HttpStatusCode.OK);
                        break;
                    case "Horizones": object horizones = entity.InvestmentHorizones.Select(x => new { x.Id, x.Title }).ToList();
                        objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "success", horizones, HttpStatusCode.OK);
                        break;
                    case "Goals": object goals = entity.InvestmentGoals.Select(x => new { x.Id, x.Title }).ToList();
                        objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "success", goals, HttpStatusCode.OK);
                        break;
                    case "Risks":
                        object riskLables = entity.Risks.Select(x => new { x.Id, x.RiskLabel }).ToList();
                        objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "success", riskLables, HttpStatusCode.OK);
                        break;
                    case "FundManagers": object fundmanagers = entity.Users.Where(x => x.RoleId == (long)roles.FundManager).Select(x => new { x.Id, x.FirstName, x.LastName }).ToList();
                        objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "success", fundmanagers, HttpStatusCode.OK);
                        break;
                    default: objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "Not Found", new object(), HttpStatusCode.NotFound);
                        break;
                }
            }
            catch (Exception ex)
            {
                objResponseData = ResponseContext<object>.CreateErrorResponse(objResponseData);
            }

            return Request.CreateResponse(objResponseData);
        }        

        public HttpResponseMessage GetNewFundEssentials()
        {
            ResponseMessage<object> objResponseData = new ResponseMessage<object>();
            try
            {
                List<NewFundEssentials> fundEssentials = entity.GetNewFundEssentials().Select(x => new NewFundEssentials {
                      Id = x.Id,
                      Title = x.Title,
                      Identifier = x.Identifier
                }).ToList();
                objResponseData = ResponseContext<object>.CreateResponse(objResponseData, "success", fundEssentials, HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                objResponseData = ResponseContext<object>.CreateErrorResponse(objResponseData);
            }
            return Request.CreateResponse(objResponseData);            
        }   

        [HttpPost]
        public HttpResponseMessage CreateFund(Scheme fund)
        {
            ResponseMessage<bool> objResponseData = new ResponseMessage<bool>();

            try
            {
                fund.InceptionDate = System.DateTime.Now;
                if (ModelState.IsValid)
                {
                    entity.Schemes.Add(fund);
                    entity.SaveChanges();

                    objResponseData = ResponseContext<bool>.CreateResponse(objResponseData, "success", true, HttpStatusCode.OK);
                }
                else
                {
                    objResponseData = ResponseContext<bool>.CreateResponse(objResponseData, "failed", false, HttpStatusCode.BadRequest);
                }
            }
            catch(Exception ex)
            {
                objResponseData = ResponseContext<bool>.CreateErrorResponse(objResponseData);
            }

            return Request.CreateResponse(objResponseData);
        }
    }
}
