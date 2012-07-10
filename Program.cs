using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
namespace Relayserver
{
    class Program
    {

        static void Main(string[] args)
        {
            UdpClient uc = new UdpClient(4505);
            Dictionary<String, IPEndPoint> LookUp = new Dictionary<String, IPEndPoint>();
            while (true)
            {

                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                String message = Encoding.UTF8.GetString(uc.Receive(ref remote));
                Console.WriteLine(message + ":" + remote.ToString());
                if (message.Substring(0, 3).Equals("get"))
                {
                    string uname = message.Substring(4);
                    if (LookUp.ContainsKey(uname))
                    {
                        IPEndPoint ret = LookUp[uname];
                        byte[] reply = Encoding.UTF8.GetBytes(ret.ToString());
                        uc.Send(reply, reply.Length, remote);
                    }
                    else
                    {
                        byte[] reply = Encoding.UTF8.GetBytes("Failed");
                        uc.Send(reply, reply.Length, remote);
                    }
                }
                else if (message.Substring(0, 3).Equals("set"))
                {
                    string uname = message.Substring(4);
                    if (LookUp.ContainsKey(uname))
                        LookUp[uname] = remote;
                    else
                    {
                        LookUp.Add(uname, remote);
                    }
                    byte[] send = Encoding.UTF8.GetBytes("Changed");
                    uc.Send(send, send.Length, remote);
                }
                else if (message.Substring(0, 3).Equals("out"))
                {
                    string uname = message.Substring(4);
                    if (LookUp.ContainsKey(uname))
                        LookUp.Remove(uname);
                    byte[] send = Encoding.UTF8.GetBytes("Removed");
                    uc.Send(send, send.Length, remote);
                }
                else if (message.Substring(0, 3).Equals("snd"))
                {
                    string remain = message.Substring(4);
                    int pos = remain.IndexOf(' ');
                    string send = remain.Substring(0, pos);
                    string remain1 = remain.Substring(pos + 1);
                    string recv = remain1.Substring(0, remain1.IndexOf(' '));
                    string mess = remain1.Substring(remain1.IndexOf(' ') + 1);
                    IPEndPoint getter = LookUp[recv];
                    byte[] tosend = Encoding.UTF8.GetBytes(send + ' ' + mess);
                    uc.Send(tosend, tosend.Length, getter);
                }

                else if (message.Substring(0, 3).Equals("Iam"))
                {
                    string result = remote.ToString();
                    byte[] b = Encoding.UTF8.GetBytes(result);
                    uc.Send(b, b.Length, remote);
                }


            }
        }

    }
}
