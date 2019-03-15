using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace MfAppBackendR3.Controllers
{
    public class ActivationController : ApiController
    {

        int maxActivations = 2;
        // GET api/<controller>/5

        [HttpGet]
        public string Activate(string code, string deviceId,int book)
        {
            var activationFileName = "";
            if (book == 1)
                activationFileName = "mfappactivation1.csv";
            else
                activationFileName= "mfappactivation2.csv";

            var codes = new List<string>();
            string csvPath = HostingEnvironment.MapPath("~/Uploads/") + Path.GetFileName(activationFileName);
            string activationFilePath = HostingEnvironment.MapPath("~/Uploads/") + Path.GetFileName(activationFileName);
            var upperCode = code.ToUpperInvariant();
            var res = File.ReadAllText(csvPath).ToUpperInvariant().Contains(upperCode);
            if (!res)
                return "KO-Activation failed. Invalid code entered.";

            var rows = File.ReadAllLines(activationFilePath);
            var r = rows.FirstOrDefault(i => i.ToUpperInvariant().Contains(upperCode));

            var rowId = Array.IndexOf(rows, r);


            var cols = r.Split(',');

            var isDeviceActivated = cols.FirstOrDefault(i => i == deviceId)!=null;
            if (isDeviceActivated)
                return "OK-Your content has been activated";

            var activationCount= cols.Count(i => i != "free");
            var col = cols.FirstOrDefault(i => i == "free");

            //il +1 serve perchè la prima colonna è !=free ma occupata dal codice del libro
            if (activationCount>= maxActivations+1)
                return "KO-Sorry, maximum number of activations reached.";
            else
            {
                var i=Array.IndexOf(cols, col);
                cols[i] = deviceId;
            }
            r = string.Join(",", cols);

            
            rows[rowId] = r;

            File.WriteAllLines(activationFilePath, rows);
            return "OK-Activation Succeded!";
        }



        private string InnnerActivateBook(string code, string deviceId, int book)
        {
            var _activationFileName = book==1 ? "mfappactivation1.csv" : "mfappactivation2.csv";

            var codes = new List<string>();
            string csvPath = HostingEnvironment.MapPath("~/Uploads/") + Path.GetFileName(_activationFileName);
            string activationFilePath = HostingEnvironment.MapPath("~/Uploads/") + Path.GetFileName(_activationFileName);
            var upperCode = code.ToUpperInvariant();
            var res = File.ReadAllText(csvPath).ToUpperInvariant().Contains(upperCode);
            if (!res)
                return "InvalidCode";

            var rows = File.ReadAllLines(activationFilePath);
            var r = rows.FirstOrDefault(i => i.ToUpperInvariant().Contains(upperCode));

            var rowId = Array.IndexOf(rows, r);


            var cols = r.Split(',');

            var isDeviceActivated = cols.FirstOrDefault(i => i == deviceId) != null;
            if (isDeviceActivated)
                return "OK";

            var activationCount = cols.Count(i => i != "free");
            var col = cols.FirstOrDefault(i => i == "free");

            //il +1 serve perchè la prima colonna è !=free ma occupata dal codice del libro
            if (activationCount >= maxActivations + 1)
                return "MaxReached";
            else
            {
                var i = Array.IndexOf(cols, col);
                cols[i] = deviceId;
            }
            r = string.Join(",", cols);


            rows[rowId] = r;

            File.WriteAllLines(activationFilePath, rows);
            return "OK";
        }

        [HttpGet]
        [Route("api/Activation/ActivateBoth")]
        public string ActivateBoth(string code, string deviceId, string book)
        {
            var res1 = "";
            var res2 = "";
            var cod1 = code.Split('|')[0];
            var cod2 = code.Split('|')[1];

            if (book == "any")
            {

                res1 = InnnerActivateBook(cod1, deviceId, 1);
                res2 = InnnerActivateBook(cod2, deviceId, 2);

                if (res1 == "OK" && res2 == "OK")
                {
                    return "ok1|ok2/Both books have been activated!";
                }
                if (res1 != "OK" && res2 == "OK")
                {
                    return "ko1|ok2/Invalid code or maximum activation reached for: First Level Book";
                }
                if (res1 == "OK" && res2 != "OK")
                {
                    return "ok1|ko2/Invalid code or maximum activation reached for: Second Level Book";
                }
                return "ko1|ko2/Something went wrong during activation!";
            }
            else if (book == "1")
            {
                res1 = InnnerActivateBook(cod1, deviceId, 1);
                if (res1 != "OK")
                    return "ko/Invalid code or maximum activation reached for: First Level Book";
                else
                    return "ok/First Level Book activated";
            }
            else if (book == "2")
            {
                res2 = InnnerActivateBook(cod2, deviceId, 2);
                if (res2 != "OK")
                    return "ko/Invalid code or maximum activation reached for: Second Level Book";
                else
                    return "ok/Second Level Book activated";
            }
            else
                return "A problem occured during activation";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}