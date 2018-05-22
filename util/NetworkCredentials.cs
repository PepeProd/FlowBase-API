using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlowBaseAPI.util
{
    public class NetworkCredentials
    {

        public NetworkCredentials() {
            NetworkUserEmail = "";
            NetworkUserPassword = "";
        }

        public string NetworkUserPassword {get; set;}
        public string NetworkUserEmail {get; set;}
        

    }
}