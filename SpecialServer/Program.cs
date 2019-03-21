using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SpecialServer
{
    // this delegate is going to be used to create the client thread to process the client requests
    // the client processing function returns void and accepts a stream as its input
    // make sure the client processor function returns void and takes a stream as a parameter
    delegate void clientProcessor(Stream s);
   
    class Program
    {

        static void Main(string[] args)
        {
            // create the ThreadID variable so we can monitor threads in this multithreaded applicaiton
            // ThreadID will be used in our logging to see what is happening
            int ThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            // this is the instance of our delegate which will be used to actuall start the client thread
            // the instance is attached to the ProcessClient method which has the appropriate signature
            // create an instance of the delegate attaching it to the client processing function           
            clientProcessor p = new clientProcessor(ProcessClient);
            
            // make the IPAddress
            string address = "127.0.0.1";
            // string address = "192.168.10.63";
            int Port = 31415;
            IPAddress localAddress = IPAddress.Parse(address);

            // Make the TcpListener
            TcpListener server = new TcpListener(localAddress,Port);
            // Start the listener
            server.Start();
            // Start a while loop
            while(true)
            {
                Console.WriteLine($"{ThreadID}>Waiting for a Client to connect");
                // accept a client
                TcpClient c = server.AcceptTcpClient();
                // listener returns a TCP client when it connects
                Console.WriteLine($"{ThreadID}A Client Has Connected...");
                // get the stream from the TCPClient
                // pass it to the client thread
                // make sure the client processor function returns void and takes a stream as a parameter
                // make sure you have a delegate that matches the client processing function
   
                // use Instance.BeginInvoke(stream, null, null) to create the thread
                p.BeginInvoke(c.GetStream(),null,null);
                // original thread returns - the while loop ends and the thread loops

            }  // end of while loop
        }


        // [write an appropriate function with appropriate signature]
        // make sure the client processor function returns void and takes a stream as a parameter
       // [function is loaded into delegate by main thread - see line 28 ]
       // the new thread process current client using a void function that takes a stream as a parameter
        static void ProcessClient(Stream s)
        {
            // create the ThreadID variable so we can monitor threads in this multithreaded applicaiton
            // ThreadID will be used in our logging to see what is happening
            // this is the threadid for each client processing thread
            // new thread starts executing the body of the function- as decribed below
            int ThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"{ThreadID}>New Client Thread Created....");
            // create a stream reader attached to the stream to read from the client
            // pass the stream to the constructor of the streamreader
            System.IO.StreamReader reader = new System.IO.StreamReader(s);
            // create a stream writer attached to the stream to write to the client
            // pass the stream to the constructor of the stream writer
            System.IO.StreamWriter writer = new System.IO.StreamWriter(s);
            // send "hello" message to client using the writer
            writer.WriteLine("HELLO From SpecialServer");  writer.Flush();
            Console.WriteLine($"{ThreadID}>Server announcing itself to Client");
            Console.WriteLine($"{ThreadID}>Server accepting Client Username");
            // recieve username from the client using the reader. remember it in a variable
            string Username = reader.ReadLine();
            // send acknowledgment message for username using the writer
            Console.WriteLine($"{ThreadID}>Server accepted Client Username as '{Username}'");
            writer.WriteLine($"{Username} Accepted"); writer.Flush();
            // while loop starts here
            while (true)
            {
                // recieve "command message" using the reader
                Console.WriteLine($"{ThreadID}>Server accepting Client Command");
                string command = reader.ReadLine();
                Console.WriteLine($"{ThreadID}>Server accepted Client Command as '{command}'");
                // parse the command message into command and parameters
                if (string.IsNullOrWhiteSpace(command))
                {
                    Console.WriteLine($"{ThreadID}>the command is empty: quiting the client '{command}'");
                    writer.WriteLine("Quitting due to empty Command line");
                    reader.Close();
                    writer.Close();
                    s.Close();
 
                    return;  // exit server thread on empty command line
                }
                // first character is the command
                char cmd = command[0];  
                string parameters = "";
                // second character is space
                // rest are parameters
                if (command.Length>2) parameters = command.Substring(2);
                // execute command (probably a c# switch)
                switch(char.ToUpper(cmd))
                {
                    case ('U'):
                        Console.WriteLine($"{ThreadID}>Executing 'U'");
                        // send response containing answer  - toupper for U
                        writer.WriteLine(parameters.ToUpper()); writer.Flush();
                        break;
                    case ('L'):
                        Console.WriteLine($"{ThreadID}>Executing 'L'");
                        // send response containing answer  - tolower for L
                        writer.WriteLine(parameters.ToLower()); writer.Flush();
                        break;
                    case ('R'):
                        Console.WriteLine($"{ThreadID}>Executing 'R'");
                        // send response containing answer  - yuck for R (try google search for reversing string)
                        writer.WriteLine(parameters.Reverse().ToArray()); writer.Flush();
                        break;
                        // check if Q- exit the loop, otherwise break out
                    case ('Q'):
                        Console.WriteLine($"{ThreadID}>Executing 'Q'");
                        writer.WriteLine("Quitting..."); writer.Flush();
                        reader.Close();
                        writer.Dispose();
                       s.Close();
                        return;  // exit the server thread on quit
                       
                       
                        
                    default:
                        Console.WriteLine($"{ThreadID}>Unrecognized command'{cmd}'");
                        writer.WriteLine("Unrecognized Command"); writer.Flush();
                       
                        break;
                }  // end of switch



            } // end of while


        }
    }

    
}
