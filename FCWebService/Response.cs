using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCWebService
{
    public class Response
    {
        private readonly List<PersonData> data;

        public List<PersonData> Data
        {
            get { return data; }
        }
        private readonly string error;

        public string Error
        {
            get { return error; }
        }
    }
}