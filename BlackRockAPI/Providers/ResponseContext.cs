using BlackRockAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using BlackRockAPI.Models;

namespace BlackRockAPI.Providers
{
    public static class ResponseContext<T>
    {
        /// <summary>
        /// creates response with single data object if data is available
        /// </summary>
        /// <param name="objResponseData"></param>
        /// <param name="message"></param>
        /// <param name="objData"></param>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        public static ResponseMessage<T> CreateResponse(ResponseMessage<T> objResponseData, string message, T objData, HttpStatusCode responseCode)
        {
            objResponseData.Status = true;
            objResponseData.Message = message;
            objResponseData.Data = objData;
            objResponseData.StatusCode = responseCode;
            return objResponseData;
        }

        /// <summary>
        /// creates response if data is unavailable
        /// </summary>
        /// <param name="objResponseData"></param>
        /// <param name="message"></param>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        public static ResponseMessage<T> CreateResponse(ResponseMessage<T> objResponseData, string message, HttpStatusCode responseCode)
        {
            objResponseData.Status = true;
            objResponseData.Message = message;
            objResponseData.Data = default(T);
            objResponseData.StatusCode = responseCode;
            return objResponseData;
        }

        /// <summary>
        /// creates response with list of data object if data is available
        /// </summary>
        /// <param name="objResponseData"></param>
        /// <param name="message"></param>
        /// <param name="objData"></param>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        public static ResponseMessage<List<T>> CreateResponse(ResponseMessage<List<T>> objResponseData, string message, List<T> objData, HttpStatusCode responseCode)
        {
            objResponseData.Status = true;
            objResponseData.Message = message;
            objResponseData.Data = objData;
            objResponseData.StatusCode = responseCode;
            return objResponseData;
        }

        /// <summary>
        /// creates response with list of data object if data is unavailable
        /// </summary>
        /// <param name="objResponseData"></param>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ResponseMessage<List<T>> CreateResponse(ResponseMessage<List<T>> objResponseData, string message, HttpStatusCode code)
        {
            objResponseData.Status = true;
            objResponseData.Message = message;
            objResponseData.Data = new List<T>();
            objResponseData.StatusCode = code;
            return objResponseData;
        }

        /// <summary>
        /// creates error response with single object if an exception is encountered
        /// </summary>
        /// <param name="objResponseData"></param>
        /// <returns></returns>
        public static ResponseMessage<T> CreateErrorResponse(ResponseMessage<T> objResponseData)
        {
            objResponseData.Status = false;
            objResponseData.Message = "Internal Server Error.";
            objResponseData.Data =  default(T);
            objResponseData.StatusCode = HttpStatusCode.InternalServerError;
            return objResponseData;
        }

        /// <summary>
        /// creates error response with list of object if an exception is encountered
        /// </summary>
        /// <param name="objResponseData"></param>
        /// <returns></returns>
        public static ResponseMessage<List<T>> CreateErrorResponse(ResponseMessage<List<T>> objResponseData)
        {
            objResponseData.Status = false;
            objResponseData.Message = "Internal Server Error.";
            objResponseData.Data = default(List<T>);
            objResponseData.StatusCode = HttpStatusCode.InternalServerError;
            return objResponseData;
        }
        
    }
}