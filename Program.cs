using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using proto1.contexts;
using proto1.ServiceImpl;
using Proto;

namespace proto1
{
    class Program
    {
        const int Port = 50051;

        public static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { EmployeeService.BindService(new EmployeeImpl(new EFDBContext())) },
                Ports = { new ServerPort("", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            testGRPC();
            Console.ReadKey();
            
    
            server.ShutdownAsync().Wait();
        }

        private static void testGRPC()
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            var client = new EmployeeService.EmployeeServiceClient(channel);
            var reply = client.GetEmployee(new EmployeeRequest {Id = 1});
            Console.WriteLine("Greeting: " + reply.Name);
            ListFeatures().Wait();

        }
        
        public static async Task ListFeatures()
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            var client = new EmployeeService.EmployeeServiceClient(channel);
            try
            {
                
                using (var call = client.GetEmployees(new EmployeeRequest{ Id = 1}))
                {
                    var responseStream = call.ResponseStream;
                    StringBuilder responseLog = new StringBuilder("Result: ");

                    while (await responseStream.MoveNext())
                    {
                        Employee employee = responseStream.Current;
                        responseLog.Append(employee.ToString());
                    }
                    Console.WriteLine(responseLog.ToString());
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine("RPC failed " + e); 
                throw;
            }
      
        }
    }
}