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
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(string code, string deviceId)
        {
            var csvFileName = "";
            var activationFileName = "mfappactivation.csv";
            var codes = new List<string>();
            string csvPath = HostingEnvironment.MapPath("~/Uploads/") + Path.GetFileName(activationFileName);
            string activationFilePath = HostingEnvironment.MapPath("~/Uploads/") + Path.GetFileName(activationFileName);

            var res = File.ReadAllText(csvPath).Contains(code);
            if (!res)
                return "CodeNotValid";

            var rows = File.ReadAllLines(activationFilePath);
            var r = rows.FirstOrDefault(i => i.Contains(code));

            var rowId = Array.IndexOf(rows, r);

            if (r == null)
                return "WrongCode";

            var cols = r.Split(',');
            var col = cols.FirstOrDefault(i => i == "free");
            if (col == null)
                return "MaxExceeded";
            else
            {
                var i=Array.IndexOf(cols, col);
                cols[i] = deviceId;
            }
            r = string.Join(",", cols);

            
            rows[rowId] = r;

            File.WriteAllLines(activationFilePath, rows);
            return "OK";
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