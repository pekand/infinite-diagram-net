using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;

#nullable disable

namespace Diagram
{
    /// <summary>
    /// message server between running processies
    /// </summary>
    public class Server //UID0876993960
    {
        /*************************************************************************************************************************/
        
        public Main main = null; // parent

        public bool mainProcess = false; // is true when server runing in this process, false if server already run in other process

        private volatile bool _shouldStop = false; // signal for server loop to stop
        private TcpListener tcpListener; // server
        private Thread listenThread; // thread for server loop

        Mutex serverMutex = null;

        /*************************************************************************************************************************/
        // SERVER LOOP

        public Server(Main main) //UID9899460299
        {
            this.main = main;

            this.mainProcess = false;
        }

        

        public bool ServerExists()
        {

            string mutexName = "Global\\InfiniteDiagram-"+ main.programOptions.server_default_port;
            bool createdNew;

            try
            {
                serverMutex = new Mutex(true, mutexName, out createdNew);
                if (createdNew)
                {
                    return false; // created success -> server not exist
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {


            }

            return false;
        }

        public bool StartServer()
        {
            try
            {
                if (!this.ServerExists()) // check if server exists
                {
                    Program.log.Write("Server: StartServer");
                    long port = main.programOptions.server_default_port;
                    IPAddress localAddr = IPAddress.Parse(main.programOptions.server_default_ip);

                    this.tcpListener = new TcpListener(localAddr, (Int32)port);
                    this.listenThread = new Thread(new ThreadStart(ListenForClients)); // start thread with server
                    this.listenThread.Start();
                    this.mainProcess = true;
                    Program.log.Write("Server: start on " + main.programOptions.server_default_ip + ":" + main.programOptions.server_default_port);

                    return true;
                }
                else
                {
                    this.mainProcess = false;
                    Program.log.Write("Server: already exist");   
                    return false;                 
                }
            }
            catch (Exception ex)
            {
                Program.log.Write("Server: "+ex.Message);
            }  
            
            return false;   
        }

        // start server loop UID8117850972
        public void ListenForClients()
        {
            try
            {
                this.tcpListener.Start();

                while (!_shouldStop) // wait for signal to end server
                {
                    //blocks until a client has connected to the server
                    TcpClient client = this.tcpListener.AcceptTcpClient(); // wait for message from client

                    Program.log.Write("Server: ListenForClients");
                    //create a thread to handle communication
                    //with connected client
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientCommunication)); // process message from client in thread
                    clientThread.Start(client);
                }

                Program.log.Write("Server: close");
            }
            catch (Exception ex)
            {
				Program.log.Write("Server: error: " + ex.Message);
            }
        }

        // process message catched from server UID1561149138
        private void HandleClientCommunication(object client)
        {
            try
            {
                Program.log.Write("Server: HandleClientCommunication");

                TcpClient tcpClient = (TcpClient)client;
                NetworkStream clientStream = tcpClient.GetStream();

                int bytesRead;

                bool requestIsValid = true;

                // recive message
                byte[] data = new byte[tcpClient.ReceiveBufferSize];
                bytesRead = clientStream.Read(data, 0, tcpClient.ReceiveBufferSize);
                ASCIIEncoding encoder = new ASCIIEncoding();
                string message = encoder.GetString(data, 0, bytesRead);                    
                requestIsValid = this.ParseMessage(message);

                // send response
                try
                {
                    // Send back a response.
                    string response = "400 Bad Request";
                    if (requestIsValid)
                    {
                        response = "200 OK";
                    }
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
                    clientStream.Write(msg, 0, msg.Length);

                    Program.log.Write("Server: HandleClientComm: send response:" + response);
                }
                catch (Exception ex) {
                    Program.log.Write("Server: HandleClientComm: client refuse response:" + ex.Message);
                }

                tcpClient.Close();
            }
            catch (Exception ex)
            {
                Program.log.Write("Server: HandleClientComm: error:" + ex.Message);
            }
        }

        /*************************************************************************************************************************/
        // MESSAGES

        // send message to server UID8096061355
        public bool SendMessage(String Messsage)
        {
			Program.log.Write("Server: SendMessage: " + Messsage);

            try
            {
                // connect to server
                TcpClient client = new TcpClient
                {
                    SendTimeout = 1000
                };
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(main.programOptions.server_default_ip), (Int32)main.programOptions.server_default_port);
                client.Connect(serverEndPoint);
                NetworkStream clientStream = client.GetStream();

                // prepare message for server
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(Messsage);

                // send message
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();

                // if close message is send not response arrive
                if (Messsage == "stop") {
                    return true;
                }

                try
                {
                    // get response from server
                    clientStream.ReadTimeout = 1000;
                    byte[] resp = new byte[2048];
                    using (var memStream = new MemoryStream())
                    {
                        int bytesread = clientStream.Read(resp, 0, resp.Length);
                        while (bytesread > 0)
                        {
                            memStream.Write(resp, 0, bytesread);
                            bytesread = clientStream.Read(resp, 0, resp.Length);
                        }
                        string response = Encoding.UTF8.GetString(memStream.ToArray());
                        Program.log.Write("Server: SendMessage(" + Messsage + "): response: " + response);
                    }
                }
                catch (Exception ex)
                {
                    Program.log.Write("Server: SendMessage(" + Messsage + "): no response from server: " + ex.Message);
                }

                return true;
            }
            catch (Exception ex)
            {
				Program.log.Write("Server: SendMessage("+Messsage+"): error: " + ex.Message);                
            }

            return false;
        }

        // parde message from server UID9190377024
        public bool ParseMessage(String Messsage)
        {
            // send message
			Program.log.Write("Server: ParseMessage: " + Messsage);

            if (Messsage == "ping") // check if server is live
            {
                return true;
            }
            else
            if (Messsage == "close") //UID5024907634
            {
                main.mainform.Invoke(new Action(() => main.mainform.TerminateApplication()));
                return true;
            }
            else
            {

                Match match = Regex.Match(Messsage, @"open:(.*)", RegexOptions.IgnoreCase); //UID0548148814
                if (match.Success)
                {
                    string FileName = match.Groups[1].Value;
                    main.mainform.Invoke(new Action(() => main.mainform.OpenDiagram(FileName))); //UID7984925616

                    return true;
                }
            }

            return false;
        }

        // send close message to server UID3713513860
        public void RequestStop()
        {
            _shouldStop = true;
            SendMessage("stop");
        }
    }
}
