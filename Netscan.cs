using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Linq.Expressions;

namespace Wallhacks_C
{
    internal class Netscan
    {
        public Netscan()
        {
            vendorsInstantiated = false;
            vendors = new List<Vendor>();
            string[] vendor_input = null;
            string name, login, pass, ip;
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"\Vendors.csv");
                using (TextFieldParser csvParser = new TextFieldParser(path))
                {
                    csvParser.CommentTokens = new string[] { "," };
                    csvParser.SetDelimiters(new string[] { "," });
                    csvParser.HasFieldsEnclosedInQuotes = true;
                    do
                    {
                        vendor_input = csvParser.ReadFields();
                        name = vendor_input[0];
                        login = vendor_input[1];
                        pass = vendor_input[2];
                        ip = vendor_input[3];
                        vendors.Add(new Vendor(name, login, pass, ip));
                    } while (!csvParser.EndOfData);
                    vendorsInstantiated = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());   
            }
        }

        /**
         * Given a vendor, determine if a Camera exists on the addressable IP on the 
         * system's local network.
         **/
        bool Camera_Exists(Vendor v)
        {
            bool res = false;
            bool scanRange = v.default_ip.Contains('x'); bool noIPExists = v.default_ip.Contains('n');
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Uri uri = null;
            string host_str = "";

            if (noIPExists)
                return false;

            if (scanRange == true) //the default IP address is a range
            {
                string ip = v.default_ip.Remove(v.default_ip.IndexOf('x'));
                for(int i = 0; i <= 255; i++)
                {
                    try
                    {
                        host_str = ip + i;
                      //  Console.WriteLine("Attempting to connect: " + host_str);
                        res = AttemptConnection(host_str);
                     //   Console.WriteLine(v.ToString());
                        return res;
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
            else // explicit ip range
            {
                try
                {
                    host_str = v.default_ip;
                    // Console.WriteLine(v.ToString());
                    res = AttemptConnection(host_str);
                    return res;
                }
                catch(Exception e)
                {
                    return res;
                }
            }
            return res;
        }

        bool AttemptConnection(String target_host)
        {
            bool res = false;
            try
            {
                IPHostEntry host = Dns.GetHostEntry(target_host);
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 80);
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //now we'll attempt to connnect to the target host, send a byte, and see if it lands
                sender.Connect(remoteEP);
                bool blockingState = sender.Blocking;
                byte[] tmp = new byte[1];
                sender.Blocking = false;
                sender.Send(tmp, 0, 0);
               // Console.WriteLine("got here!" + sender.ToString());
                res = true;
            }
            catch (Exception e)
            {
               // Console.WriteLine(e);
            }

            return res;
        }

        /*
         * Run the scan for all vendors on the vendor list
         **/
        public void ScanForCamerasCLI()
        {
            int numCameras = 0;  
            List<Vendor> cameras = new List<Vendor>();
            Console.Write("Scanning local network for cameras...");
            using (var spinner = new Spinner(0, 2))
            {
                spinner.Start();
                for (int i = 0; i < vendors.Count; i++)
                {
                    spinner.Start();
                    if (Camera_Exists(vendors[i]))
                    {
                        cameras.Add(vendors[i]);
                        Console.WriteLine("Discovered potential camera: " + vendors[i].ToString());
                    }
                }
                Thread.Sleep(10000);
            }
            Console.WriteLine("Cameras detected: " + cameras.Count);
            foreach (Vendor v in cameras)
            {
                Console.WriteLine(v.ToString() + "\n");
            }
        }

        List<Vendor> vendors;
        bool vendorsInstantiated;
    }
}
