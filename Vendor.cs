using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallhacks_C
{
    public class Vendor 
    {
        public Vendor(string name_in, string login_in, string pass_in, string ip_in)
        {
            name = name_in;
            default_login = login_in;
            default_pass = pass_in;
            default_ip = ip_in;
        }

        public override string ToString()
        {
            string res = " Vendor: " + name + ". Default admin login: " + default_login + ". Default admin pass: " + default_pass + ".";
            return res;
        }


        public string name;
        public string default_ip;
        public string default_login;
        public string default_pass;
    }   
}
