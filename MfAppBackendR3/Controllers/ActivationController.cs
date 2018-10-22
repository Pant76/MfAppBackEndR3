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

            var res = File.ReadAllText(csvPath).Contains(code);
            if (!res)
                return "Activation failed. Invalid code entered.";

            var rows = File.ReadAllLines(activationFilePath);
            var r = rows.FirstOrDefault(i => i.Contains(code));

            var rowId = Array.IndexOf(rows, r);


            var cols = r.Split(',');

            var isDeviceActivated = cols.FirstOrDefault(i => i == deviceId)!=null;
            if (isDeviceActivated)
                return "Your content has been activated";

            var col = cols.FirstOrDefault(i => i == "free");
            if (col == null)
                return "Sorry, maximum number of activations reached.";
            else
            {
                var i=Array.IndexOf(cols, col);
                cols[i] = deviceId;
            }
            r = string.Join(",", cols);

            
            rows[rowId] = r;

            File.WriteAllLines(activationFilePath, rows);
            return "Activation Succeded!";
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