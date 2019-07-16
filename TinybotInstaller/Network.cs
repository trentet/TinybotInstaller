using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    static class Network
    {
        public static bool PingTest(string address, int timeout)
        {
            bool isConnected = false;
            try
            {
                Ping myPing = new Ping();
                byte[] buffer = new byte[32];
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(address, timeout, buffer, pingOptions);

                if (reply.Status == IPStatus.Success)
                {
                    isConnected = true;
                }

            }
            catch (Exception)
            {
                isConnected = false;
            }

            return isConnected;
        }

        public static bool WebsiteTest(string url)
        {
            bool isConnected = false;
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead(url))
                {
                    isConnected = true;
                }
            }
            catch
            {
                isConnected = false;
            }

            return isConnected;
        }

        public static bool TestInternetConnection()
        {
            bool isConnected = false;
            try
            {
                if (PingTest("8.8.8.8", 1000) == true)
                {
                    isConnected = true;
                }
                else
                {
                    try
                    {
                        if (WebsiteTest("http://clients3.google.com/generate_204") == true)
                        {
                            isConnected = true;
                        }
                    }
                    catch
                    {
                        isConnected = false;
                    }
                }
            }
            catch (Exception)
            {
                isConnected = false;
            }

            return isConnected;
        }
    }
}
